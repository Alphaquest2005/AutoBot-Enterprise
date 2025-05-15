using System;
using System.Data.Entity;
using System.Diagnostics; 
using System.Linq;
using System.Threading.Tasks;
using AutoBot;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;
using FileInfo = System.IO.FileInfo;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace AutoBotUtilities
{
    using System.Text.RegularExpressions;
    using WaterNut.Business.Services.Utils;
    // Using Utils = WaterNut.DataSpace.Utils; // This specific alias might conflict if WaterNut.DataSpace.Utils is also a class name. Let's be explicit or remove if not strictly needed.

    public class ImportUtils
    {
        private static readonly ILogger _log = Log.ForContext<ImportUtils>();

        public static async Task ExecuteEmailMappingActions(EmailMapping emailMapping, FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            string operationName = nameof(ExecuteEmailMappingActions);
            string operationInvocationId = Guid.NewGuid().ToString();

            // It's good to have FileType context if available
            using (LogContext.PushProperty("OperationInvocationId", operationInvocationId))
            using (LogContext.PushProperty("FileTypeId", fileType?.Id))
            using (LogContext.PushProperty("EmailId", fileType?.EmailId))
            using (LogContext.PushProperty("AsycudaDocumentSetId", fileType?.AsycudaDocumentSetId))
            using (LogContext.PushProperty("EmailMappingId", emailMapping?.EmailMappingId))
            {
                var stopwatch = Stopwatch.StartNew();
                _log.Information("ACTION_START: {ActionName}. EmailMappingId: {EmailMappingId}, FileTypeId: {FileTypeId}, FileCount: {FileCount}", 
                    operationName, emailMapping?.EmailMappingId, fileType?.Id, files?.Length ?? 0);

                try
                {
                    if (emailMapping == null)
                    {
                        _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): EmailMapping is null. Cannot execute actions.", operationName, "ParameterValidation");
                        stopwatch.Stop();
                        _log.Information("ACTION_END_SUCCESS: {ActionName} (Validation Failed: No EmailMapping). Duration: {TotalObservedDurationMs}ms", operationName, stopwatch.ElapsedMilliseconds);
                        return;
                    }

                    var missingActions = emailMapping.EmailMappingActions
                        .Where(x => x.Actions.IsDataSpecific == true && !FileUtils.FileActions.ContainsKey(x.Actions.Name))
                        .ToList();

                    if (missingActions.Any())
                    {
                        string missingActionsStr = string.Join(", ", missingActions.Select(x => x.Actions.Name));
                        _log.Error("INTERNAL_STEP ({OperationName} - {Stage}): The following actions were missing: {MissingActionsList}", operationName, "Validation", missingActionsStr);
                        throw new ApplicationException($"The following actions were missing: {missingActionsStr}");
                    }

                    var actionsToPerform = emailMapping.EmailMappingActions.OrderBy(x => x.Id)
                        .Where(x => x.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                        .Where(x => FileUtils.FileActions.ContainsKey(x.Actions.Name)) // Ensure action delegate exists
                        .Select(x => (Name: x.Actions.Name, Delegate: FileUtils.FileActions[x.Actions.Name]))
                        .ToList();
                    
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {ActionCount} email mapping actions to execute.", operationName, "ActionEnumeration", actionsToPerform.Count);

                    foreach (var actionInfo in actionsToPerform)
                    {
                        // ExecuteActions will log its own start/end and details
                        await ExecuteActions(fileType, files, actionInfo).ConfigureAwait(false);
                    }
                    
                    stopwatch.Stop();
                    _log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms", operationName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    _log.Error(e, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}", 
                        operationName, stopwatch.ElapsedMilliseconds, e.Message);
                    try
                    {
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to forward error email. EmailId: {EmailIdForForward}", operationName, "ErrorForwarding", fileType?.EmailId);
                        await EmailDownloader.EmailDownloader.ForwardMsgAsync(fileType.EmailId, BaseDataModel.GetClient(), $"Bug Found in {operationName}",
                            $"{e.Message}\r\n{e.StackTrace}", EmailDownloader.EmailDownloader.GetContacts("Developer"),
                            Array.Empty<string>()).ConfigureAwait(false);
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Successfully forwarded error email.", operationName, "ErrorForwarding");
                    }
                    catch (Exception forwardEx)
                    {
                        _log.Error(forwardEx, "INTERNAL_STEP ({OperationName} - {Stage}): Failed to forward error email.", operationName, "ErrorForwarding");
                    }
                    // Rethrowing to allow higher-level handlers if necessary, or it could be handled here.
                    // For now, let's assume the error email is sufficient notification from this level.
                }
            }
        }

        public static async Task ExecuteActions(FileTypes fileType, FileInfo[] files, (string Name, Func<FileTypes, FileInfo[], Task> Action) actionInfo)
        {
            string operationName = nameof(ExecuteActions);
            // This InvocationId is for this specific action execution.
            // It will inherit the OperationInvocationId from the caller (e.g., ExecuteEmailMappingActions) via LogContext.
            string actionInvocationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("ActionInvocationId", actionInvocationId))
            using (LogContext.PushProperty("ActionName", actionInfo.Name)) // Add ActionName to context
            {
                var contextProperties = new {
                    FileTypeId = fileType?.Id,
                    EmailId = fileType?.EmailId,
                    AsycudaDocumentSetId = fileType?.AsycudaDocumentSetId == 0 ? (int?)null : fileType?.AsycudaDocumentSetId,
                    DocSetReference = fileType?.DocSetRefernece
                };

                _log.Information("ACTION_START: {ActionName_Context}. Context: {ActionContextProperties}, FileCount: {FileCount}", 
                    actionInfo.Name, contextProperties, files?.Length ?? 0);
                var stopwatch = Stopwatch.StartNew();
                
                bool isContinueProcessingMainAction = true;

                try
                {
                    if (fileType != null && fileType.ProcessNextStep != null && fileType.ProcessNextStep.Any())
                    {
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Starting ProcessNextStep sequence for Action {ActionName_Context}. Initial steps: {ProcessNextStepList}",
                            operationName, "ProcessNextStepStart", actionInfo.Name, string.Join(",", fileType.ProcessNextStep));
                        bool hitContinueInLoop = false;

                        while (fileType.ProcessNextStep.Any())
                        {
                            var nextActionName = fileType.ProcessNextStep.First();
                            using (LogContext.PushProperty("NextActionName", nextActionName))
                            {
                                if (!FileUtils.FileActions.TryGetValue(nextActionName, out var nextActionAsyncFunc))
                                {
                                    _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Action '{NextActionName_Context}' not found in FileUtils.FileActions. Skipping. For main action {ActionName_Context}",
                                        operationName, "ProcessNextStepActionNotFound", nextActionName, actionInfo.Name);
                                }
                                else
                                {
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Executing ProcessNextStep action '{NextActionName_Context}' for main action {ActionName_Context}",
                                        operationName, "ProcessNextStepActionExecute", nextActionName, actionInfo.Name);
                                    var nextStopwatch = Stopwatch.StartNew();
                                    try
                                    {
                                        if (nextActionName == "Continue")
                                        {
                                            hitContinueInLoop = true;
                                            nextStopwatch.Stop();
                                            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): 'Continue' action encountered in ProcessNextStep for main action {ActionName_Context}. Proceeding to main action. Duration: {DurationMs}ms",
                                                operationName, "ProcessNextStepContinue", actionInfo.Name, nextStopwatch.ElapsedMilliseconds);
                                            fileType.ProcessNextStep.RemoveAt(0); // Remove "Continue"
                                            break; 
                                        }

                                        _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) (ProcessNextStep for {ActionName_Context})", 
                                            nextActionName, "ASYNC_EXPECTED", actionInfo.Name);
                                        await nextActionAsyncFunc.Invoke(fileType, files).ConfigureAwait(false);
                                        nextStopwatch.Stop();
                                        _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) (ProcessNextStep for {ActionName_Context})",
                                            nextActionName, nextStopwatch.ElapsedMilliseconds, "Async call completed (await).", actionInfo.Name);
                                    }
                                    catch (Exception nextEx)
                                    {
                                        nextStopwatch.Stop();
                                        _log.Error(nextEx, "OPERATION_END_FAILURE: {OperationDescription} (ProcessNextStep for {ActionName_Context}). Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                            nextActionName, actionInfo.Name, nextStopwatch.ElapsedMilliseconds, nextEx.Message);
                                        isContinueProcessingMainAction = false;
                                        if (fileType.ProcessNextStep.Any() && fileType.ProcessNextStep.First() == nextActionName)
                                        {
                                            fileType.ProcessNextStep.RemoveAt(0);
                                        }
                                        throw; 
                                    }
                                }
                                // Removal logic adjusted for clarity
                                if (fileType.ProcessNextStep.Any() && fileType.ProcessNextStep.First() == nextActionName)
                                { // If it was the action just processed (and not 'Continue' which breaks)
                                    fileType.ProcessNextStep.RemoveAt(0);
                                } 
                                else if (nextActionName != "Continue" && fileType.ProcessNextStep.Any()) 
                                { // If it was skipped and not 'Continue', remove the head
                                     _log.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Removing '{NextActionName_Context}' from ProcessNextStep queue after processing/skipping.", operationName, "ProcessNextStepRemove", nextActionName);
                                     fileType.ProcessNextStep.RemoveAt(0);
                                }
                            }
                        } // End While

                        if (!hitContinueInLoop && isContinueProcessingMainAction) // isContinueProcessingMainAction could be false due to an error
                        {
                            isContinueProcessingMainAction = false;
                            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): ProcessNextStep sequence completed without 'Continue' for main action {ActionName_Context}. Main action will NOT run.",
                                operationName, "ProcessNextStepNoContinue", actionInfo.Name);
                        }
                    }
                    
                    if (isContinueProcessingMainAction)
                    {
                        _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) (Main Action)", 
                            actionInfo.Name, "ASYNC_EXPECTED");
                        var mainActionInvokeStopwatch = Stopwatch.StartNew(); // Stopwatch for the main action invoke itself
                        await actionInfo.Action.Invoke(fileType, files).ConfigureAwait(false);
                        mainActionInvokeStopwatch.Stop();
                        _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) (Main Action)",
                            actionInfo.Name, mainActionInvokeStopwatch.ElapsedMilliseconds, "Async call completed (await).");
                        
                        stopwatch.Stop(); // This is the overall stopwatch for ExecuteActions
                        _log.Information("ACTION_END_SUCCESS: {ActionName_Context}. Duration: {TotalObservedDurationMs}ms. Context: {ActionContextProperties}", 
                            actionInfo.Name, stopwatch.ElapsedMilliseconds, contextProperties);
                    }
                    else
                    {
                        stopwatch.Stop();
                        _log.Warning("ACTION_SKIPPED: {ActionName_Context} (Due to ProcessNextStep logic). Duration: {TotalObservedDurationMs}ms. Context: {ActionContextProperties}", 
                            actionInfo.Name, stopwatch.ElapsedMilliseconds, contextProperties);
                    }
                }
                catch (Exception e)
                {
                    if(stopwatch.IsRunning) stopwatch.Stop();
                    _log.Error(e, "ACTION_END_FAILURE: {ActionName_Context}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}. Context: {ActionContextProperties}", 
                        actionInfo.Name, stopwatch.ElapsedMilliseconds, e.Message, contextProperties);
                    try
                    {
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to send error email for failed action {ActionName_Context}.", operationName, "ErrorEmailing", actionInfo.Name);
                        await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null, $"Bug Found in Action: {actionInfo.Name}",
                            EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{e.Message}\r\n{e.StackTrace}",
                            Array.Empty<string>()).ConfigureAwait(false);
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Successfully sent error email for action {ActionName_Context}.", operationName, "ErrorEmailing", actionInfo.Name);
                    }
                    catch (Exception emailEx)
                    {
                         _log.Error(emailEx, "INTERNAL_STEP ({OperationName} - {Stage}): Failed to send error email for action {ActionName_Context}.", operationName, "ErrorEmailing", actionInfo.Name);
                    }
                    throw; // Rethrow to allow higher-level handlers
                }
            }
        }

        public static async Task ExecuteDataSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            string operationName = nameof(ExecuteDataSpecificFileActions);
            string operationInvocationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("OperationInvocationId", operationInvocationId))
            using (LogContext.PushProperty("FileTypeId", fileType?.Id))
            {
                var stopwatch = Stopwatch.StartNew();
                _log.Information("ACTION_START: {ActionName}. FileTypeId: {FileTypeId}, FileCount: {FileCount}, AppSettingId: {AppSettingId}", 
                    operationName, fileType?.Id, files?.Length ?? 0, appSetting?.ApplicationSettingsId);
                try
                {
                    var missingActions = fileType.FileTypeActions
                        .Where(x => x.Actions.IsDataSpecific == true && !FileUtils.FileActions.ContainsKey(x.Actions.Name))
                        .ToList();

                    if (missingActions.Any())
                    {
                        string missingActionsStr = string.Join(", ", missingActions.Select(x => x.Actions.Name));
                        _log.Error("INTERNAL_STEP ({OperationName} - {Stage}): The following data-specific actions were missing: {MissingActionsList}", operationName, "Validation", missingActionsStr);
                        throw new ApplicationException($"The following data-specific actions were missing: {missingActionsStr}");
                    }

                    using (var ctx = new CoreEntitiesContext())
                    {
                        var orderedFileActions = ctx.FileTypeActions
                            .Include(fta => fta.Actions)
                            .Where(fta => fta.FileTypeId == fileType.Id)
                            .Where(fta => fta.Actions.IsDataSpecific == true)
                            .Where(fta => (fta.AssessIM7 == null && fta.AssessEX == null) ||
                                          (appSetting.AssessIM7 == fta.AssessIM7 || appSetting.AssessEX == fta.AssessEX))
                            .Where(fta => fta.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                            .OrderBy(fta => fta.Id)
                            .ToList();

                        var actionsToExecute = orderedFileActions
                            .Where(fta => FileUtils.FileActions.ContainsKey(fta.Actions.Name))
                            .Select(fta => (Name: fta.Actions.Name, Action: FileUtils.FileActions[fta.Actions.Name]))
                            .ToList();
                        
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {ActionCount} data-specific file actions to execute.", operationName, "ActionEnumeration", actionsToExecute.Count);
                        if (!actionsToExecute.Any() && orderedFileActions.Any()) {
                            _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Found {OrderedCount} FileTypeActions from DB, but {ExecutableCount} are executable (exist in FileUtils.FileActions). Check action name matching.", 
                                operationName, "ActionMismatchWarning", orderedFileActions.Count, actionsToExecute.Count);
                        }


                        foreach (var actionTuple in actionsToExecute)
                        {
                            await ExecuteActions(fileType, files, actionTuple).ConfigureAwait(false);
                        }
                    }
                    stopwatch.Stop();
                    _log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms", operationName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    _log.Error(e, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}", 
                        operationName, stopwatch.ElapsedMilliseconds, e.Message);
                    try
                    {
                        await EmailDownloader.EmailDownloader.ForwardMsgAsync(fileType?.EmailId, BaseDataModel.GetClient(), $"Bug Found in {operationName}",
                            $"{e.Message}\r\n{e.StackTrace}", EmailDownloader.EmailDownloader.GetContacts("Developer"),
                            Array.Empty<string>()).ConfigureAwait(false);
                    } catch (Exception fex) { _log.Error(fex, "Failed to forward error email during {ActionName} exception handling.", operationName); }
                    // throw; // Consider re-throwing
                }
            }
        }

        public static async Task ExecuteNonSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            string operationName = nameof(ExecuteNonSpecificFileActions);
            string operationInvocationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("OperationInvocationId", operationInvocationId))
            using (LogContext.PushProperty("FileTypeId", fileType?.Id))
            {
                var stopwatch = Stopwatch.StartNew();
                _log.Information("ACTION_START: {ActionName}. FileTypeId: {FileTypeId}, FileCount: {FileCount}, AppSettingId: {AppSettingId}", 
                    operationName, fileType?.Id, files?.Length ?? 0, appSetting?.ApplicationSettingsId);
                try
                {
                    var missingActionsCheck = fileType.FileTypeActions
                       .Where(x => (x.Actions.IsDataSpecific == null || x.Actions.IsDataSpecific != true) && !FileUtils.FileActions.ContainsKey(x.Actions.Name))
                       .ToList();
                    if (missingActionsCheck.Any())
                    {
                        _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Non-specific actions missing implementation: {MissingActionsList}", 
                            operationName, "ValidationWarning", string.Join(", ", missingActionsCheck.Select(x => x.Actions.Name)));
                    }

                    using (var ctx = new CoreEntitiesContext())
                    {
                        var orderedFileActions = ctx.FileTypeActions
                            .Include(fta => fta.Actions)
                            .Where(fta => fta.FileTypeId == fileType.Id)
                            .Where(fta => fta.Actions.IsDataSpecific == null || fta.Actions.IsDataSpecific != true)
                            .Where(fta => (fta.AssessIM7 == null && fta.AssessEX == null) ||
                                          (appSetting.AssessIM7 == fta.AssessIM7 || appSetting.AssessEX == fta.AssessEX))
                            .Where(fta => fta.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                            .OrderBy(fta => fta.Id)
                            .ToList();

                        var actionsToExecute = orderedFileActions
                           .Where(fta => FileUtils.FileActions.ContainsKey(fta.Actions.Name))
                           .Select(fta => (Name: fta.Actions.Name, Action: FileUtils.FileActions[fta.Actions.Name]))
                           .ToList();
                        
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {ActionCount} non-specific file actions to execute.", operationName, "ActionEnumeration", actionsToExecute.Count);
                         if (!actionsToExecute.Any() && orderedFileActions.Any()) {
                            _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Found {OrderedCount} FileTypeActions from DB, but {ExecutableCount} are executable (exist in FileUtils.FileActions). Check action name matching.", 
                                operationName, "ActionMismatchWarning", orderedFileActions.Count, actionsToExecute.Count);
                        }

                        foreach (var actionTuple in actionsToExecute)
                        {
                            await ExecuteActions(fileType, files, actionTuple).ConfigureAwait(false);
                        }
                    }
                    stopwatch.Stop();
                    _log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms", operationName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    _log.Error(e, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}", 
                        operationName, stopwatch.ElapsedMilliseconds, e.Message);
                    try
                    {
                        await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null, $"Bug Found in {operationName}",
                            EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{e.Message}\r\n{e.StackTrace}",
                            Array.Empty<string>()).ConfigureAwait(false);
                    } catch (Exception fex) { _log.Error(fex, "Failed to send error email during {ActionName} exception handling.", operationName); }
                    // throw; // Consider re-throwing
                }
            }
        }

        // Instance method - requires an instance of ImportUtils to be called.
        // If this is intended to be static, it needs `static` keyword and potentially different parameter handling.
        // For now, instrumenting as an instance method.
        public async Task SavePDF(string droppedFilePath, string fileTypeHint, int docSetId, bool overwrite)
        {
            string operationName = nameof(SavePDF);
            string operationInvocationId = Guid.NewGuid().ToString();
            // If called from a static context, `this` isn't available for Log.ForContext(this.GetType())
            // Using _log (static logger for the class) or Log.ForContext<ImportUtils>() is fine.
            // For instance methods, Log.ForContext(GetType()) is also an option.
            ILogger instanceLog = _log.ForContext("DroppedFilePath", droppedFilePath)
                                     .ForContext("FileTypeHint", fileTypeHint)
                                     .ForContext("DocSetId", docSetId)
                                     .ForContext("Overwrite", overwrite);

            using (LogContext.PushProperty("OperationInvocationId", operationInvocationId))
            {
                var stopwatch = Stopwatch.StartNew();
                instanceLog.Information("ACTION_START: {ActionName}. DroppedFilePath: {DroppedFilePath}, FileTypeHint: {FileTypeHint}, DocSetId: {DocSetId}, Overwrite: {Overwrite}",
                    operationName, droppedFilePath, fileTypeHint, docSetId, overwrite);
                try
                {
                    var importableFileTypesPdf = await FileTypeManager.GetImportableFileType(fileTypeHint, FileTypeManager.FileFormats.PDF, droppedFilePath).ConfigureAwait(false);
                    var dfileType = importableFileTypesPdf.FirstOrDefault(x =>
                        Regex.IsMatch(droppedFilePath, x.FilePattern, RegexOptions.IgnoreCase));

                    if (dfileType != null)
                    {
                        instanceLog.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found matching FileType {MatchedFileTypeId} ('{MatchedFilePattern}') for PDF.",
                            operationName, "FileTypeMatched", dfileType.Id, dfileType.FilePattern);
                        dfileType.AsycudaDocumentSetId = docSetId;
                        
                        instanceLog.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for FileType {MatchedFileTypeId}", 
                            "InvoiceReader.ImportPDF", "ASYNC_EXPECTED", dfileType.Id);
                        var importPdfStopwatch = Stopwatch.StartNew();
                        
                        // Assuming InvoiceReader.InvoiceReader.ImportPDF is the correct static call path
                        await InvoiceReader.InvoiceReader.ImportPDF(new[] { new FileInfo(droppedFilePath) }, dfileType).ConfigureAwait(false);
                        
                        importPdfStopwatch.Stop();
                        instanceLog.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for FileType {MatchedFileTypeId}",
                            "InvoiceReader.ImportPDF", importPdfStopwatch.ElapsedMilliseconds, "Async call completed (await).", dfileType.Id);
                    }
                    else
                    {
                        instanceLog.Warning("INTERNAL_STEP ({OperationName} - {Stage}): No suitable PDF file type found for {DroppedFilePath} with fileType hint {FileTypeHint}. PDF will not be imported by this action.",
                            operationName, "NoFileTypeMatch", droppedFilePath, fileTypeHint);
                    }
                    stopwatch.Stop();
                    instanceLog.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms", operationName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    instanceLog.Error(e, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        operationName, stopwatch.ElapsedMilliseconds, e.Message);
                    throw; // Rethrowing to allow higher-level handlers
                }
            }
        }
    }
}