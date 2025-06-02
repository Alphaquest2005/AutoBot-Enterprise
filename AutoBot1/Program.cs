﻿using AutoBot; // For FolderProcessor
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity; // For EF6 async methods and DbFunctions
using System.Diagnostics; // Added for Stopwatch
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoBot.Properties; // For Settings.Default.DevMode
using AutoBotUtilities;    // Assuming this contains your Utils class definition
using Core.Common.Utils;  // For StringExtensions
using MoreLinq;
using WaterNut.Business.Services.Utils; // Assuming ImportUtils, FileTypeManager might be here or related
using WaterNut.DataSpace;             // Assuming SessionsUtils might be here or related
using MailKit.Net.Imap; // Added for ImapClient type
using Serilog;
using Serilog.Context;
using Serilog.Events; // Though often not directly used, good for reference
using Core.Common.Extensions;

namespace AutoBot
{
    partial class Program
    {
        public static bool ReadOnlyMode { get; set; } = false;
        private static LevelOverridableLogger _centralizedLogger;

        static void Main(string[] args) // Main is synchronous
        {
            // Configure LogFilterState.EnabledCategoryLevels for AutoBot (similar to test projects)
            LogFilterState.EnabledCategoryLevels = new Dictionary<LogCategory, LogEventLevel>
            {
                { LogCategory.MethodBoundary, LogEventLevel.Information },
                { LogCategory.ActionBoundary, LogEventLevel.Information },
                { LogCategory.ExternalCall, LogEventLevel.Information },
                { LogCategory.StateChange, LogEventLevel.Information },
                { LogCategory.Security, LogEventLevel.Information },
                { LogCategory.MetaLog, LogEventLevel.Warning },
                { LogCategory.InternalStep, LogEventLevel.Warning },
                { LogCategory.DiagnosticDetail, LogEventLevel.Warning },
                { LogCategory.Performance, LogEventLevel.Warning },
                { LogCategory.Undefined, LogEventLevel.Information }
            };

            // Create inner Serilog logger with high minimum level (filtering will be done by LevelOverridableLogger)
            var innerSerilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose() // Set to lowest level, filtering handled by LevelOverridableLogger
                .Enrich.FromLogContext() // Ensures LogContext properties are included
                .Enrich.WithThreadId()   // Requires Serilog.Enrichers.Thread package
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ThreadId}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .Filter.ByIncludingOnly(evt =>
                {
                    // Extract LogCategory from event properties
                    LogCategory category = LogCategory.Undefined;
                    if (evt.Properties.TryGetValue("LogCategory", out var categoryProperty) &&
                        categoryProperty is ScalarValue scalarValue &&
                        scalarValue.Value is LogCategory logCategory)
                    {
                        category = logCategory;
                    }

                    // Extract SourceContext for detailed targeting
                    string sourceContext = null;
                    if (evt.Properties.TryGetValue("SourceContext", out var sourceContextProperty) &&
                        sourceContextProperty is ScalarValue sourceScalarValue)
                    {
                        sourceContext = sourceContextProperty.ToString().Trim('"');
                    }

                    // Extract MemberName for method-specific targeting
                    string memberName = null;
                    if (evt.Properties.TryGetValue("MemberName", out var memberNameProperty) &&
                        memberNameProperty is ScalarValue memberScalarValue)
                    {
                        memberName = memberNameProperty.ToString().Trim('"');
                    }

                    // Check for detailed targeting (similar to test projects)
                    if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails) &&
                        !string.IsNullOrEmpty(sourceContext) &&
                        sourceContext.Contains(LogFilterState.TargetSourceContextForDetails))
                    {
                        bool methodMatch = string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) ||
                                           (!string.IsNullOrEmpty(memberName) && memberName.Equals(LogFilterState.TargetMethodNameForDetails, StringComparison.OrdinalIgnoreCase));

                        if (methodMatch)
                        {
                            return evt.Level >= LogFilterState.DetailTargetMinimumLevel;
                        }
                    }

                    // Use category-based filtering
                    if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var enabledMinLevelForCategory))
                    {
                        return evt.Level >= enabledMinLevelForCategory;
                    }

                    return false;
                })
                .CreateLogger();

            // Create LevelOverridableLogger wrapper
            _centralizedLogger = new LevelOverridableLogger(innerSerilogLogger);

            // Set as global logger for backward compatibility
            Log.Logger = _centralizedLogger;

            var globalStopwatch = Stopwatch.StartNew();
            string globalInvocationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("InvocationId", globalInvocationId))
            {
                try
                {
                    Log.Information("APPLICATION_START: AutoBot starting. Args: {Args}", (object)args); // Cast args to object for Serilog array handling

                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (s, e) =>
                    {
                        Log.Warning("CANCELLATION_REQUESTED: User requested cancellation via Console.CancelKeyPress.");
                        cts.Cancel();
                        e.Cancel = true; // Prevent the process from terminating immediately
                    };

                    MainAsync(args, cts.Token).GetAwaiter().GetResult(); // Run async part and wait

                    globalStopwatch.Stop();
                    Log.Information("APPLICATION_END_SUCCESS: AutoBot completed successfully. Total duration: {TotalDurationMs}ms.", globalStopwatch.ElapsedMilliseconds);
                }
                catch (OperationCanceledException)
                {
                    globalStopwatch.Stop();
                    Log.Warning("APPLICATION_END_CANCELLED: Main operation was canceled. Total duration: {TotalDurationMs}ms.", globalStopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    globalStopwatch.Stop();
                    Log.Fatal(e, "GLOBAL_ERROR: Unhandled {ExceptionType} in Main. Duration: {TotalDurationMs}ms.", e.GetType().Name, globalStopwatch.ElapsedMilliseconds);
                    
                    try
                    {
                        var errorReportingClient = Utils.Client ?? new EmailDownloader.Client { Email = "default-error-reporter@example.com", CompanyName = "ErrorReporter" };
                        string[] devContacts = { "developer@example.com" }; // Fallback
                        try { devContacts = EmailDownloader.EmailDownloader.GetContacts("Developer", _centralizedLogger); }
                        catch (Exception contactEx) {
                            Log.Warning(contactEx, "INTERNAL_STEP (Main - ErrorHandling): Failed to get developer contacts for error reporting.");
                        }

                        Log.Information("INTERNAL_STEP (Main - ErrorHandling): Attempting to send error email to {DevContacts}.", (object)devContacts);
                        EmailDownloader.EmailDownloader.SendEmailAsync(errorReportingClient, null, $"Bug Found in AutoBot",
                             devContacts, $"{e.Message}\r\n{e.StackTrace}", Array.Empty<string>(), _centralizedLogger, CancellationToken.None)
                             .GetAwaiter().GetResult();
                        Log.Information("INTERNAL_STEP (Main - ErrorHandling): Successfully sent error email.");
                    }
                    catch (Exception mailEx)
                    {
                        Log.Error(mailEx, "ERROR_REPORTING_FAILED: Failed to send error email after unhandled exception. Reporter: {ReporterEmail}", Utils.Client?.Email);
                    }
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
        }

        static async Task MainAsync(string[] args, CancellationToken cancellationToken) // Async helper
        {
            // This InvocationId is for the entire MainAsync operation.
            // Specific operations within loops will get their own InvocationIds.
            string mainAsyncInvocationId = Guid.NewGuid().ToString(); 
            using (LogContext.PushProperty("InvocationId", mainAsyncInvocationId))
            {
                var mainAsyncStopwatch = Stopwatch.StartNew();
                Log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]", 
                    nameof(MainAsync), $"Args: {(args != null ? string.Join(",", args) : "null")}");

                Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");
                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Z.EntityFramework.Extensions license applied.", nameof(MainAsync), "Initialization");

                var timeBeforeImport = DateTime.Now;
                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Timestamp before import operations: {TimeBeforeImport}", nameof(MainAsync), "PreImportTimestamp", timeBeforeImport);
                
                using (var ctx = new CoreEntitiesContext() { })
                {
                    ctx.Database.CommandTimeout = 10;
                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Database command timeout set to {Timeout} seconds.", nameof(MainAsync), "DBContextSetup", ctx.Database.CommandTimeout);

                    var appSettingsStopwatch = Stopwatch.StartNew();
                    var applicationSettings = await ctx.ApplicationSettings.AsNoTracking()
                        .Include(x => x.FileTypes)
                        .Include(x => x.Declarants)
                        .Include("FileTypes.FileTypeReplaceRegex")
                        .Include("FileTypes.FileImporterInfos")
                        .Include(x => x.EmailMapping)
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos")
                        .Include("EmailMapping.EmailMappingRexExs")
                        .Include("EmailMapping.EmailMappingActions.Actions")
                        .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                        .Where(x => x.IsActive)
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);
                    appSettingsStopwatch.Stop();
                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Retrieved {Count} active application settings. Duration: {DurationMs}ms.", 
                        nameof(MainAsync), "LoadAppSettings", applicationSettings.Count, appSettingsStopwatch.ElapsedMilliseconds);

                    if (!applicationSettings.Any())
                    {
                        Log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                            nameof(MainAsync), "ProcessAppSettingsLoop", "ApplicationSettings", "No active application settings found to process.");
                    }

                    foreach (var appSetting in applicationSettings)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        string appSettingProcessingInvocationId = Guid.NewGuid().ToString();
                        using (LogContext.PushProperty("InvocationId", appSettingProcessingInvocationId)) // New InvocationId for this specific appSetting's processing
                        {
                            var appSettingOverallStopwatch = Stopwatch.StartNew();
                            Log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]", 
                                "ProcessAppSetting", $"AppSettingId: {appSetting.ApplicationSettingsId}, SoftwareName: {appSetting.SoftwareName}");

                            if (appSetting.DataFolder != null) 
                            {
                                string originalDataFolder = appSetting.DataFolder;
                                appSetting.DataFolder = StringExtensions.UpdateToCurrentUser(appSetting.DataFolder);
                                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Updated DataFolder from '{OriginalDataFolder}' to '{NewDataFolder}' for AppSetting {AppSettingId}.",
                                    "ProcessAppSetting", "UpdateDataFolder", originalDataFolder, appSetting.DataFolder, appSetting.ApplicationSettingsId);
                            }

                            BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                            var folderProcessor = new FolderProcessor();

                            if (appSetting.TestMode == true)
                            {
                                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): TestMode is TRUE for AppSetting {AppSettingId}. Attempting to execute last DB session action.",
                                    "ProcessAppSetting", "TestModeCheck", appSetting.ApplicationSettingsId);
                                if (ExecuteLastDBSessionAction(ctx, appSetting, _centralizedLogger))
                                {
                                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Last DB session action executed for AppSetting {AppSettingId}, continuing to next appSetting.",
                                        "ProcessAppSetting", "TestModeLastActionExecuted", appSetting.ApplicationSettingsId);
                                    appSettingOverallStopwatch.Stop();
                                    Log.Information("ACTION_END_SUCCESS: {ActionName} (TestMode Early Exit). Total observed duration: {TotalObservedDurationMs}ms.", 
                                        "ProcessAppSetting", appSettingOverallStopwatch.ElapsedMilliseconds);
                                    continue;
                                }
                                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): No last DB session action executed or it returned false for AppSetting {AppSettingId}.",
                                    "ProcessAppSetting", "TestModeNoLastAction", appSetting.ApplicationSettingsId);
                            }
                            
                            var emailProcessingStopwatch = Stopwatch.StartNew();
                            Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", 
                                $"EmailProcessor.ProcessEmailsAsync for AppSetting {appSetting.ApplicationSettingsId}", "ASYNC_EXPECTED");
                            await EmailProcessor.ProcessEmailsAsync(appSetting, timeBeforeImport, ctx , cancellationToken, _centralizedLogger).ConfigureAwait(false);
                            emailProcessingStopwatch.Stop();
                            Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                $"EmailProcessor.ProcessEmailsAsync for AppSetting {appSetting.ApplicationSettingsId}", emailProcessingStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                            // ExecuteDBSessionActions is synchronous but called from async context
                            var dbSessionActionsStopwatch = Stopwatch.StartNew();
                            Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", 
                                $"ExecuteDBSessionActions for AppSetting {appSetting.ApplicationSettingsId}", "SYNC_EXPECTED"); // It's sync
                            ExecuteDBSessionActions(ctx, appSetting, _centralizedLogger);
                            dbSessionActionsStopwatch.Stop();
                             Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                $"ExecuteDBSessionActions for AppSetting {appSetting.ApplicationSettingsId}", dbSessionActionsStopwatch.ElapsedMilliseconds, "Sync call returned.");


                            if (BaseDataModel.Instance.CurrentApplicationSettings.ProcessDownloadsFolder == true)
                            {
                                var downloadFolderStopwatch = Stopwatch.StartNew();
                                Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", 
                                    $"FolderProcessor.ProcessDownloadFolder for AppSetting {appSetting.ApplicationSettingsId}", "ASYNC_EXPECTED");
                                await folderProcessor.ProcessDownloadFolder(appSetting).ConfigureAwait(false); // Assuming cancellationToken should be passed if available in FolderProcessor
                                downloadFolderStopwatch.Stop();
                                Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                    $"FolderProcessor.ProcessDownloadFolder for AppSetting {appSetting.ApplicationSettingsId}", downloadFolderStopwatch.ElapsedMilliseconds, "Async call completed (await).");
                            }
                            appSettingOverallStopwatch.Stop();
                            Log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.", 
                                "ProcessAppSetting", appSettingOverallStopwatch.ElapsedMilliseconds);
                        }
                    }
                }
                mainAsyncStopwatch.Stop();
                Log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.", 
                    nameof(MainAsync), mainAsyncStopwatch.ElapsedMilliseconds);
            }
        }

        private static bool ExecuteLastDBSessionAction(CoreEntitiesContext ctx, ApplicationSettings appSetting, ILogger log)
        {
            string methodInvocationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("InvocationId", methodInvocationId)) // Overrides AppSetting's InvocationId for this method scope
            {
                var methodStopwatch = Stopwatch.StartNew();
                Log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                                    nameof(ExecuteLastDBSessionAction), $"AppSettingId: {appSetting.ApplicationSettingsId}");

                var dbQueryStopwatch = Stopwatch.StartNew();
                var lastAction = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefault(x => x.ApplicationSettingId == appSetting.ApplicationSettingsId);
                dbQueryStopwatch.Stop();
                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Queried for last session action. Duration: {DurationMs}ms. Found: {Found}", 
                    nameof(ExecuteLastDBSessionAction), "QueryLastAction", dbQueryStopwatch.ElapsedMilliseconds, lastAction != null);

                if (lastAction != null)
                {
                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found last action with ID {LastActionId} for Session {SessionName}.", 
                        nameof(ExecuteLastDBSessionAction), "ActionFound", lastAction.Id, lastAction.Sessions?.Name);
                    
                    var actionsToInvoke = lastAction.Sessions.SessionActions
                        .Where(x => lastAction.ActionId == null || x.ActionId == lastAction.ActionId)
                        .ToList();

                    if (!actionsToInvoke.Any())
                    {
                         Log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                            nameof(ExecuteLastDBSessionAction), "InvokeActions", $"SessionActions for SessionId {lastAction.Sessions.Id}", "No actions to invoke for the matched lastAction criteria.");
                    }

                    foreach(var sessionActionDetail in actionsToInvoke)
                    {
                        string operationDesc = $"SessionActionDelegate:{sessionActionDetail.Actions?.Name ?? "UnknownAction"} (ID: {sessionActionDetail.ActionId})";
                        var invokeTimer = Stopwatch.StartNew();
                        Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", operationDesc, "SYNC_EXPECTED");
                        try
                        {
                            SessionsUtils.SessionActions[sessionActionDetail.Actions.Name].Invoke(log);
                            invokeTimer.Stop();
                            Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                operationDesc, invokeTimer.ElapsedMilliseconds, "Sync call returned successfully.");
                        }
                        catch(Exception ex)
                        {
                            invokeTimer.Stop();
                            Log.Error(ex, "OPERATION_END_FAILURE: {OperationDescription}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                operationDesc, invokeTimer.ElapsedMilliseconds, ex.Message);
                            // Decide if this error should stop further processing or be rethrown
                        }
                    }
                    methodStopwatch.Stop();
                    Log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                                        nameof(ExecuteLastDBSessionAction), methodStopwatch.ElapsedMilliseconds);
                    return true;
                }
                
                Log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                    nameof(ExecuteLastDBSessionAction), "NoActionFound", "SessionSchedule", "No last action found for this app setting.");
                methodStopwatch.Stop();
                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): {MethodName} (Returned False). Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ExecuteLastDBSessionAction), "MethodExit", nameof(ExecuteLastDBSessionAction), methodStopwatch.ElapsedMilliseconds);
                return false;
            }
        }

        private static void ExecuteDBSessionActions(CoreEntitiesContext ctx, ApplicationSettings appSetting, ILogger log)
        {
            string methodInvocationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("InvocationId", methodInvocationId))
            {
                var methodStopwatch = Stopwatch.StartNew();
                Log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                                    nameof(ExecuteDBSessionActions), $"AppSettingId: {appSetting.ApplicationSettingsId}");

                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing 'End' session actions.", nameof(ExecuteDBSessionActions), "ProcessEndActions");
                var endActions = ctx.SessionActions.OrderBy(x => x.Id)
                    .Include(x => x.Actions)
                    .Where(x => x.Sessions.Name == "End").ToList();
                
                if (!endActions.Any()) {
                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                        nameof(ExecuteDBSessionActions), "ProcessEndActions", "EndActions", "No 'End' actions found.");
                }
                foreach(var endActionDetail in endActions)
                {
                    string operationDesc = $"EndSessionActionDelegate:{endActionDetail.Actions?.Name ?? "UnknownAction"}";
                    var invokeTimer = Stopwatch.StartNew();
                    Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", operationDesc, "SYNC_EXPECTED");
                    try
                    {
                        SessionsUtils.SessionActions[endActionDetail.Actions.Name].Invoke(log);
                        invokeTimer.Stop();
                        Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                            operationDesc, invokeTimer.ElapsedMilliseconds, "Sync call returned successfully.");
                    }
                    catch(Exception ex)
                    {
                        invokeTimer.Stop();
                        Log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                                    operationDesc, invokeTimer.ElapsedMilliseconds, ex.Message);
                    }
                }


                var scheduledDbQueryStopwatch = Stopwatch.StartNew();
                var sLst = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                    .Include("ParameterSet.ParameterSetParameters.Parameters")
                    .Where(x => x.RunDateTime >= DbFunctions.AddMinutes(DateTime.Now, (x.Sessions.WindowInMinutes) * -1) 
                                && x.RunDateTime <= DbFunctions.AddMinutes(DateTime.Now, x.Sessions.WindowInMinutes))
                    .Where(x => (x.ApplicationSettingId == null || x.ApplicationSettingId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    .OrderBy(x => x.Id)
                    .ToList();
                scheduledDbQueryStopwatch.Stop();
                Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Queried for scheduled session actions. Duration: {DurationMs}ms. Found: {Count}", 
                    nameof(ExecuteDBSessionActions), "QueryScheduledActions", scheduledDbQueryStopwatch.ElapsedMilliseconds, sLst.Count);

                BaseDataModel.Instance.CurrentSessionSchedule = sLst;

                if (sLst.Any())
                {
                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {Count} scheduled sessions to process.", 
                        nameof(ExecuteDBSessionActions), "ScheduledSessionsFound", sLst.Count);
                    
                    foreach (var item in sLst)
                    {
                        using (LogContext.PushProperty("ScheduledSessionId", item.Id)) // Context for this specific scheduled session
                        {
                            Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing scheduled session '{SessionName}' (ID: {ScheduledSessionId}).", 
                                nameof(ExecuteDBSessionActions), "ProcessScheduledSession", item.Sessions?.Name, item.Id);
                            
                            var actionsToInvoke = item.Sessions.SessionActions
                                .Where(x => item.ActionId == null || x.ActionId == item.ActionId)
                                .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                                .ToList();

                            if (!actionsToInvoke.Any()) {
                                Log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                                    nameof(ExecuteDBSessionActions), "InvokeScheduledActions", $"SessionActions for ScheduledSessionId {item.Id}", "No actions to invoke for this scheduled session's criteria.");
                            }

                            foreach(var sessionActionDetail in actionsToInvoke)
                            {
                                BaseDataModel.Instance.CurrentSessionAction = sessionActionDetail; 
                                string operationDesc = $"ScheduledSessionActionDelegate:{sessionActionDetail.Actions?.Name ?? "UnknownAction"} (ID: {sessionActionDetail.ActionId})";
                                var invokeTimer = Stopwatch.StartNew();
                                Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", operationDesc, "SYNC_EXPECTED");
                                try
                                {
                                    SessionsUtils.SessionActions[sessionActionDetail.Actions.Name].Invoke(log);
                                    invokeTimer.Stop();
                                    Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                        operationDesc, invokeTimer.ElapsedMilliseconds, "Sync call returned successfully.");
                                }
                                catch(Exception ex)
                                {
                                    invokeTimer.Stop();
                                    Log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                                                        operationDesc, invokeTimer.ElapsedMilliseconds, ex.Message);
                                }
                            }
                        }
                    }
                    BaseDataModel.Instance.CurrentSessionAction = null;
                    BaseDataModel.Instance.CurrentSessionSchedule = new List<SessionSchedule>();
                }
                else
                {
                    Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): No scheduled sessions found within the current window.", nameof(ExecuteDBSessionActions), "NoScheduledSessionsInWindow");
                    
                    if (appSetting.AssessIM7 == true)
                    {
                        Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing AssessIM7 actions as no scheduled sessions found.", nameof(ExecuteDBSessionActions), "AssessIM7Fallback");
                        var im7Actions = ctx.SessionActions.OrderBy(x => x.Id).Include(x => x.Actions)
                            .Where(x => x.Sessions.Name == "AssessIM7" && x.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                            .ToList();
                        if (!im7Actions.Any()) {
                             Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                                nameof(ExecuteDBSessionActions), "AssessIM7Fallback", "IM7Actions", "No AssessIM7 actions found matching criteria.");
                        }
                        foreach(var im7ActionDetail in im7Actions)
                        {
                            string operationDesc = $"AssessIM7ActionDelegate:{im7ActionDetail.Actions?.Name ?? "UnknownAction"}";
                            var invokeTimer = Stopwatch.StartNew();
                            Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", operationDesc, "SYNC_EXPECTED");
                            try {
                                SessionsUtils.SessionActions[im7ActionDetail.Actions.Name].Invoke(log);
                                invokeTimer.Stop();
                                Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                    operationDesc, invokeTimer.ElapsedMilliseconds, "Sync call returned successfully.");
                            } catch (Exception ex) {
                                invokeTimer.Stop();
                                Log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                                                    operationDesc, invokeTimer.ElapsedMilliseconds, ex.Message);
                            }
                        }
                    }

                    if (appSetting.AssessEX == true)
                    {
                        Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing AssessEX actions as no scheduled sessions found.", nameof(ExecuteDBSessionActions), "AssessEXFallback");
                        var exActions = ctx.SessionActions.OrderBy(x => x.Id).Include(x => x.Actions)
                            .Where(x => x.Sessions.Name == "AssessEX" && x.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                            .ToList();
                        if (!exActions.Any()) {
                             Log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}", 
                                nameof(ExecuteDBSessionActions), "AssessEXFallback", "EXActions", "No AssessEX actions found matching criteria.");
                        }
                        foreach(var exActionDetail in exActions)
                        {
                            string operationDesc = $"AssessEXActionDelegate:{exActionDetail.Actions?.Name ?? "UnknownAction"}";
                            var invokeTimer = Stopwatch.StartNew();
                            Log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", operationDesc, "SYNC_EXPECTED");
                            try {
                                SessionsUtils.SessionActions[exActionDetail.Actions.Name].Invoke(log);
                                invokeTimer.Stop();
                                Log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})", 
                                    operationDesc, invokeTimer.ElapsedMilliseconds, "Sync call returned successfully.");
                            } catch (Exception ex) {
                                invokeTimer.Stop();
                                Log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                                                    operationDesc, invokeTimer.ElapsedMilliseconds, ex.Message);
                            }
                        }
                    }
                }
                methodStopwatch.Stop();
                Log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                                    nameof(ExecuteDBSessionActions), methodStopwatch.ElapsedMilliseconds);
            }
        }
    }
}