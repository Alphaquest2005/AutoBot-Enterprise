using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics; // Added for Stopwatch
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using AutoBotUtilities;

using CoreEntities.Business.Entities;

using MailKit.Net.Imap;

using MoreLinq;

using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace AutoBot
{
    using global::AutoBot.Properties;

    public class EmailProcessor
    {
        
        public static async Task ProcessEmailsAsync(
            ApplicationSettings appSetting,
            DateTime beforeImport,
            CoreEntitiesContext ctx,
            CancellationToken cancellationToken,
            ILogger log)
        {
            string operationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("InvocationId", operationId))
            using (LogContext.PushProperty("AppSettingId", appSetting?.ApplicationSettingsId))
            using (LogContext.PushProperty("EmailAccount", appSetting?.Email))
            {
                var stopwatch = Stopwatch.StartNew();
                int processedEmailCount = 0; // Declare and initialize processedEmailCount here
                log.Information("ACTION_START: {ActionName} for {EmailAccount} (AppSettingId: {AppSettingId})",
                    nameof(ProcessEmailsAsync), appSetting?.Email, appSetting?.ApplicationSettingsId);

                try
                {
                    if (string.IsNullOrEmpty(appSetting?.Email))
                    {
                        log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Email is null or empty. Aborting. AppSettingId: {AppSettingId}",
                                                    nameof(ProcessEmailsAsync), "ParameterValidation", appSetting?.ApplicationSettingsId);
                        stopwatch.Stop();
                        log.Information("ACTION_END_SUCCESS: {ActionName} (Validation Failed: No Email). Duration: {TotalObservedDurationMs}ms",
                            nameof(ProcessEmailsAsync), stopwatch.ElapsedMilliseconds);
                        return;
                    }
                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Email parameter validated for AppSettingId: {AppSettingId}",
                                            nameof(ProcessEmailsAsync), "ParameterValidation", appSetting?.ApplicationSettingsId);

                    Utils.Client = new EmailDownloader.Client
                    {
                        CompanyName = appSetting.CompanyName,
                        DataFolder = appSetting.DataFolder,
                        Password = appSetting.EmailPassword,
                        Email = appSetting.Email,
                        ApplicationSettingsId = appSetting.ApplicationSettingsId,
                        EmailMappings = appSetting.EmailMapping.ToList(),
                        DevMode = Settings.Default.DevMode,
                        NotifyUnknownMessages = appSetting.NotifyUnknownMessages ?? false
                    };
                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): EmailDownloader.Client configured for {EmailAccount}",
                       nameof(ProcessEmailsAsync), "ClientSetup", Utils.Client.Email);

                    var filesForNonSpecificActions =
                        new List<Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>>();

                    ImapClient imapClient = null;
                    try
                    {
                       log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to connect IMAP client for {EmailAccount}",
                            nameof(ProcessEmailsAsync), "ImapConnectAttempt", Utils.Client.Email);
                        
                        var imapConnectStopwatch = Stopwatch.StartNew();
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            "EmailDownloader.GetOpenImapClientAsync", "ASYNC_EXPECTED");
                        
                        imapClient = await EmailDownloader.EmailDownloader
                                         .GetOpenImapClientAsync(Utils.Client, log, cancellationToken).ConfigureAwait(false);
                        
                        imapConnectStopwatch.Stop();
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) Connected: {IsConnected}, Authenticated: {IsAuthenticated}",
                            "EmailDownloader.GetOpenImapClientAsync", imapConnectStopwatch.ElapsedMilliseconds, "Async call completed (await).", imapClient?.IsConnected ?? false, imapClient?.IsAuthenticated ?? false);

                        if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                        {
                            log.Error("ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. AppSettingId: {AppSettingId}",
                                                            nameof(ProcessEmailsAsync), stopwatch.ElapsedMilliseconds, appSetting.ApplicationSettingsId);
                                                        stopwatch.Stop(); // Ensure stopwatch is stopped before return
                                                        return;
                        }
                        log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Successfully connected IMAP client for {EmailAccount}",
                            nameof(ProcessEmailsAsync), "ImapConnectSuccess", Utils.Client.Email);

                        foreach (Task<EmailDownloader.EmailProcessingResult> emailTask in EmailDownloader.EmailDownloader
                                     .StreamEmailResultsAsync(imapClient, Utils.Client, log, cancellationToken))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            string emailIterationInvocationId = Guid.NewGuid().ToString();
                            using (LogContext.PushProperty("InvocationId", emailIterationInvocationId))
                            {
                                EmailDownloader.EmailProcessingResult currentEmailResult = null;
                                var emailTaskStopwatch = Stopwatch.StartNew();
                                try
                                {
                                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Awaiting next email processing task. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "StreamEmailLoopAwait", emailIterationInvocationId);
                                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                        "emailTask (StreamEmailResultsAsync item)", "ASYNC_EXPECTED");

                                    currentEmailResult = await emailTask.ConfigureAwait(false);

                                    emailTaskStopwatch.Stop();
                                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) ResultIsNull: {ResultIsNull}, Subject: {EmailSubject}, Attachments: {AttachmentCount}",
                                        "emailTask (StreamEmailResultsAsync item)", emailTaskStopwatch.ElapsedMilliseconds, "Async call completed (await).",
                                        currentEmailResult == null, currentEmailResult != null ? currentEmailResult.EmailKey.SubjectIdentifier : "N/A", currentEmailResult != null ? (currentEmailResult.AttachedFiles?.Count ?? 0) : 0);
                                }
                                catch (OperationCanceledException oce)
                                {
                                    emailTaskStopwatch.Stop();
                                    log.Warning(oce, "INTERNAL_STEP ({OperationName} - {Stage}): Email processing task was canceled during await. Duration: {DurationMs}ms. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "StreamEmailLoopCancel", emailTaskStopwatch.ElapsedMilliseconds, emailIterationInvocationId);
                                    throw; // Propagate cancellation
                                }
                                catch (Exception taskEx)
                                {
                                    emailTaskStopwatch.Stop();
                                        log.Error(taskEx, "INTERNAL_STEP ({OperationName} - {Stage}): Exception during email task processing. Duration: {DurationMs}ms. InvocationId: {InvocationId}, Subject (if available): {EmailSubject}",
                                        nameof(ProcessEmailsAsync), "StreamEmailLoopException", emailTaskStopwatch.ElapsedMilliseconds, emailIterationInvocationId, currentEmailResult != null ? currentEmailResult.EmailKey.SubjectIdentifier : "N/A");
                                    
                                    if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                                    {
                                            log.Error("INTERNAL_STEP ({OperationName} - {Stage}): IMAP client disconnected during task processing. Aborting further email checks for AppSettingId: {AppSettingId}. InvocationId: {InvocationId}",
                                            nameof(ProcessEmailsAsync), "StreamEmailLoopImapError", appSetting.ApplicationSettingsId, emailIterationInvocationId);
                                        break;
                                    }
                                    continue; // Next email task
                                }

                                if (currentEmailResult == null)
                                {
                                        log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Current email processing result is null, skipping. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "NullEmailResult", emailIterationInvocationId);
                                    continue;
                                }

                                processedEmailCount++; // Increment processedEmailCount
                                var emailKey = currentEmailResult.EmailKey;
                                var attachments = currentEmailResult.AttachedFiles.ToArray();
                                string emailSubjectForLog = emailKey.SubjectIdentifier ?? "UnknownSubject";

                                log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing email {EmailSubject} with {AttachmentCount} attachments. InvocationId: {InvocationId}",
                                    nameof(ProcessEmailsAsync), "ProcessIndividualEmailStart", emailSubjectForLog, attachments.Length, emailIterationInvocationId);

                                foreach (var att in attachments)
                                {
                                    log.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Attachment: {AttachmentName} (Size: {AttachmentSize} bytes, LastModified: {LastModifiedDate}, IsPDF: {IsPdf}) for Email {EmailSubject}. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "LogAttachmentDetail", att.Name, att.Length, att.LastWriteTime, att.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase), emailSubjectForLog, emailIterationInvocationId);
                                }

                                try
                                {
                                    var execMapActionsStopwatch = Stopwatch.StartNew();
                                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for Email {EmailSubject}. InvocationId: {InvocationId}",
                                        $"ImportUtils.ExecuteEmailMappingActions", "ASYNC_EXPECTED", emailSubjectForLog, emailIterationInvocationId);
                                    
                                    await ImportUtils.ExecuteEmailMappingActions(
                                        emailKey.EmailMessage.EmailMapping,
                                        new FileTypes() { EmailId = emailKey.EmailMessage.EmailId },
                                        attachments,
                                        appSetting).ConfigureAwait(false);
                                    
                                    execMapActionsStopwatch.Stop();
                                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for Email {EmailSubject}. InvocationId: {InvocationId}",
                                        $"ImportUtils.ExecuteEmailMappingActions", execMapActionsStopwatch.ElapsedMilliseconds, "Async call completed (await).", emailSubjectForLog, emailIterationInvocationId);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Error: {ErrorMessage}. InvocationId: {InvocationId}",
                                        $"ExecuteEmailMappingActions for Email {emailSubjectForLog}", emailIterationInvocationId);
                                    throw; // Rethrow to be handled by the main try-catch for ProcessEmailsAsync or the outer loop's catch
                                }

                                var msgResult = currentEmailResult;
                                var emailKeyTuple = msgResult.EmailKey;
                                var msgAttachments = msgResult.AttachedFiles;
                                var emailForLog = emailKeyTuple.EmailMessage;
                                var emailIdForLogging = emailForLog?.EmailUniqueId.ToString() ?? $"UnknownEmailId_{emailKeyTuple.SubjectIdentifier}";

                                try // Inner processing for this specific email's content
                                {
                                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Entering inner content processing for email: {EmailIdForLogging}. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "InnerEmailProcessingStart", emailIdForLogging, emailIterationInvocationId);

                                    var desFolder = Path.Combine(
                                        appSetting.DataFolder,
                                        emailKeyTuple.SubjectIdentifier,
                                        emailForLog.EmailUniqueId.ToString());
                                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Destination folder set to {DestinationFolder} for Email {EmailIdForLogging}.",
                                        nameof(ProcessEmailsAsync), "SetDestinationFolder", desFolder, emailIdForLogging);

                                    bool meetsRequiredFiles = emailForLog.EmailMapping.EmailFileTypes.All(
                                          x => x.IsRequired != true || msgAttachments.Any(
                                                   att => Regex.IsMatch(att.FullName, x.FileTypes.FilePattern, RegexOptions.IgnoreCase) && att.LastWriteTime >= beforeImport));
                                    
                                    if (!meetsRequiredFiles)
                                    {
                                        log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Skipping email {EmailIdForLogging}, required files criteria not met (e.g., missing required files or files too old). BeforeImportDate: {BeforeImportDate}. InvocationId: {InvocationId}",
                                            nameof(ProcessEmailsAsync), "RequiredFilesCheckFail", emailIdForLogging, beforeImport, emailIterationInvocationId);
                                        continue;
                                    }
                                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Required files criteria met for email: {EmailIdForLogging}. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "RequiredFilesCheckPass", emailIdForLogging, emailIterationInvocationId);

                                    var fileTypesForOrdering = emailForLog.FileTypes ?? new List<CoreEntities.Business.Entities.FileTypes>();
                                    var emailFileTypes = emailForLog.EmailMapping.InfoFirst == true
                                                              ? fileTypesForOrdering.OrderByDescending(x => x.FileImporterInfos.EntryType == "Info").ToList()
                                                              : fileTypesForOrdering.OrderBy(x => x.FileImporterInfos.EntryType == "Info").ToList();
                                    
                                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing {FileTypeCount} file types for email {EmailIdForLogging}. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "ProcessFileTypesStart", emailFileTypes.Count, emailIdForLogging, emailIterationInvocationId);

                                    if (!emailFileTypes.Any())
                                    {
                                        log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation} for Email {EmailIdForLogging}. InvocationId: {InvocationId}",
                                            nameof(ProcessEmailsAsync), "ProcessFileTypes", "EmailFileTypes", "No file types defined or matched for this email.", emailIdForLogging, emailIterationInvocationId);
                                    }

                                    foreach (var emailFileTypeDefinition in emailFileTypes)
                                    {
                                        cancellationToken.ThrowIfCancellationRequested();
                                        string fileTypeInvocationId = Guid.NewGuid().ToString();
                                        using(LogContext.PushProperty("InvocationId", fileTypeInvocationId))
                                        {
                                            log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing file type definition {FileTypeId} ('{FilePattern}') for email {EmailIdForLogging}. InvocationId: {InvocationId}",
                                                nameof(ProcessEmailsAsync), "ProcessFileTypeDefinition", emailFileTypeDefinition.Id, emailFileTypeDefinition.FilePattern, emailIdForLogging, fileTypeInvocationId);

                                            var fileTypeInstance = FileTypeManager.GetFileType(emailFileTypeDefinition);
                                            fileTypeInstance.Data.Clear();
                                            fileTypeInstance.EmailInfoMappings = emailForLog.EmailMapping.EmailInfoMappings;

                                            var csvFiles = msgAttachments.Where(
                                                x => Regex.IsMatch(x.FullName, fileTypeInstance.FilePattern, RegexOptions.IgnoreCase) && x.LastWriteTime >= beforeImport)
                                                .Select(x => x)
                                                .ToArray();

                                            fileTypeInstance.EmailId = emailForLog.EmailId;
                                            fileTypeInstance.FilePath = desFolder;

                                            if (csvFiles.Length == 0)
                                            {
                                                log.Information("INTERNAL_STEP ({OperationName} - {Stage}): No matching/recent files for pattern '{FilePattern}' for email {EmailIdForLogging}. Skipping this file type. InvocationId: {InvocationId}",
                                                    nameof(ProcessEmailsAsync), "NoCsvFilesForFileType", fileTypeInstance.FilePattern, emailIdForLogging, fileTypeInvocationId);
                                                continue;
                                            }
                                            log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {CsvFileCount} files for pattern '{FilePattern}' for email {EmailIdForLogging}. InvocationId: {InvocationId}",
                                                nameof(ProcessEmailsAsync), "CsvFilesFound", csvFiles.Length, fileTypeInstance.FilePattern, emailIdForLogging, fileTypeInvocationId);

                                            var reference = emailKeyTuple.SubjectIdentifier;
                                            AsycudaDocumentSetEx docSet = null; // Changed type here

                                            var findDocSetStopwatch = Stopwatch.StartNew();
                                            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for reference '{Reference}'. InvocationId: {InvocationId}",
                                                "ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync (initial search)", "ASYNC_EXPECTED", reference, fileTypeInvocationId);
                                            docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                                  x => x.Declarant_Reference_Number.Contains(reference)
                                                                       && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, cancellationToken)
                                                                              .ConfigureAwait(false);
                                            findDocSetStopwatch.Stop();
                                            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) DocSetFound: {DocSetFound}, Reference: '{Reference}'. InvocationId: {InvocationId}",
                                                "ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync (initial search)", findDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await).", docSet != null, reference, fileTypeInvocationId);
                                            
                                            if (fileTypeInstance.CreateDocumentSet)
                                            {
                                                if (docSet == null || docSet.Declarant_Reference_Number != reference) // If not exact match or not found
                                                {
                                                    var findExactDocSetStopwatch = Stopwatch.StartNew();
                                                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for exact reference '{Reference}'. InvocationId: {InvocationId}",
                                                        "ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync (exact search)", "ASYNC_EXPECTED", reference, fileTypeInvocationId);
                                                    docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                                     x => x.Declarant_Reference_Number == reference
                                                                          && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, cancellationToken)
                                                                                 .ConfigureAwait(false);
                                                    findExactDocSetStopwatch.Stop();
                                                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) DocSetFound: {DocSetFound}, ExactReference: '{Reference}'. InvocationId: {InvocationId}",
                                                        "ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync (exact search)", findExactDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await).", docSet != null, reference, fileTypeInvocationId);
                                                    if (docSet == null)
                                                    {
                                                        var cp = BaseDataModel.Instance.Customs_Procedures.First(
                                                            x => x.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation() && x.IsDefault == true);
                                                        
                                                        var createDocSetStopwatch = Stopwatch.StartNew();
                                                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) to create DocSet for reference '{Reference}'. InvocationId: {InvocationId}",
                                                            "ctx.Database.ExecuteSqlCommandAsync (Insert AsycudaDocumentSet)", "ASYNC_EXPECTED", reference, fileTypeInvocationId);
                                                        await ctx.Database.ExecuteSqlCommandAsync(
                                                            TransactionalBehavior.EnsureTransaction,
                                                            $@"INSERT INTO AsycudaDocumentSet (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                                                VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{reference.Replace("'", "''")}',{cp.Customs_ProcedureId},0)",
                                                            cancellationToken).ConfigureAwait(false);
                                                        createDocSetStopwatch.Stop();
                                                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for reference '{Reference}'. InvocationId: {InvocationId}",
                                                            "ctx.Database.ExecuteSqlCommandAsync (Insert AsycudaDocumentSet)", createDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await).", reference, fileTypeInvocationId);
                                                        var findNewDocSetStopwatch = Stopwatch.StartNew();
                                                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) to find newly created DocSet for reference '{Reference}'. InvocationId: {InvocationId}",
                                                            "ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync (post-create search)", "ASYNC_EXPECTED", reference, fileTypeInvocationId);
                                                        docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                                         x => x.Declarant_Reference_Number == reference
                                                                              && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, cancellationToken)
                                                                                     .ConfigureAwait(false);
                                                        findNewDocSetStopwatch.Stop();
                                                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) DocSetFound: {DocSetFound}, Reference: '{Reference}'. InvocationId: {InvocationId}",
                                                            "ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync (post-create search)", findNewDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await).", docSet != null, reference, fileTypeInvocationId);
                                                    }
                                                }
                                            }
                                            if (docSet != null)
                                            {
                                                fileTypeInstance.AsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                                                fileTypeInstance.Data.Add(new KeyValuePair<string, string>("AsycudaDocumentSetId", fileTypeInstance.AsycudaDocumentSetId.ToString()));
                                               log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Associated FileTypeInstance with AsycudaDocumentSetId {AsycudaDocumentSetId}. InvocationId: {InvocationId}",
                                                    nameof(ProcessEmailsAsync), "AssociateDocSet", docSet.AsycudaDocumentSetId, fileTypeInvocationId);
                                            } else {
                                               log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): AsycudaDocumentSet is null for FileType {FileTypeId}, reference {Reference}. Some operations might be affected. InvocationId: {InvocationId}",
                                                    nameof(ProcessEmailsAsync), "NullDocSet", fileTypeInstance.Id, reference, fileTypeInvocationId);
                                            }
                                            
                                            var saveAttachStopwatch = Stopwatch.StartNew();
                                           log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for {FileCount} files. InvocationId: {InvocationId}",
                                                "Utils.SaveAttachments", "ASYNC_EXPECTED", csvFiles.Length, fileTypeInvocationId);
                                            await Utils.SaveAttachments(csvFiles, fileTypeInstance, emailForLog, log).ConfigureAwait(false);
                                            saveAttachStopwatch.Stop();
                                           log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). InvocationId: {InvocationId}",
                                                "Utils.SaveAttachments", saveAttachStopwatch.ElapsedMilliseconds, "Async call completed (await).", fileTypeInvocationId);
                                            if (!Program.ReadOnlyMode)
                                            {
                                                var execDataSpecificStopwatch = Stopwatch.StartNew();
                                               log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}). InvocationId: {InvocationId}",
                                                    "ImportUtils.ExecuteDataSpecificFileActions", "ASYNC_EXPECTED", fileTypeInvocationId);
                                                await ImportUtils.ExecuteDataSpecificFileActions(fileTypeInstance, csvFiles, appSetting).ConfigureAwait(false);
                                                execDataSpecificStopwatch.Stop();
                                               log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). InvocationId: {InvocationId}",
                                                    "ImportUtils.ExecuteDataSpecificFileActions", execDataSpecificStopwatch.ElapsedMilliseconds, "Async call completed (await).", fileTypeInvocationId);
                                                if (emailForLog.EmailMapping.IsSingleEmail == true)
                                                {
                                                    var execNonSpecificSingleStopwatch = Stopwatch.StartNew();
                                                   log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) (single email mode). InvocationId: {InvocationId}",
                                                        "ImportUtils.ExecuteNonSpecificFileActions", "ASYNC_EXPECTED", fileTypeInvocationId);
                                                    await ImportUtils.ExecuteNonSpecificFileActions(fileTypeInstance, csvFiles, appSetting).ConfigureAwait(false);
                                                    execNonSpecificSingleStopwatch.Stop();
                                                   log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) (single email mode). FileTypeInvocationId: {FileTypeInvocationId_Context}",
                                                        "ImportUtils.ExecuteNonSpecificFileActions", execNonSpecificSingleStopwatch.ElapsedMilliseconds, "Async call completed (await).", fileTypeInvocationId);
                                                }
                                                else
                                                {
                                                    int currentDocSetId = docSet?.AsycudaDocumentSetId ?? 0;
                                                    if (currentDocSetId == 0) // If still 0, try to get last one
                                                    {
                                                        var findLastDocSetStopwatch = Stopwatch.StartNew();
                                                       log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for non-specific actions fallback. InvocationId: {InvocationId}",
                                                            "ctx.AsycudaDocumentSet.FirstOrDefaultAsync (last DocSet for non-specific)", "ASYNC_EXPECTED", fileTypeInvocationId);
                                                        var lastDocSet = await ctx.AsycudaDocumentSet
                                                                             .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                                                             .OrderByDescending(x => x.AsycudaDocumentSetId)
                                                                             .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                                                        findLastDocSetStopwatch.Stop();
                                                       log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) LastDocSetId: {LastDocSetId}. InvocationId: {InvocationId}",
                                                            "ctx.AsycudaDocumentSet.FirstOrDefaultAsync (last DocSet for non-specific)", findLastDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await).", lastDocSet?.AsycudaDocumentSetId, fileTypeInvocationId);
                                                        if (lastDocSet != null) currentDocSetId = lastDocSet.AsycudaDocumentSetId;
                                                    }
                                                    filesForNonSpecificActions.Add(new Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>(fileTypeInstance, csvFiles, currentDocSetId));
                                                   log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Added FileTypeInstance {FileTypeId} to filesForNonSpecificActions with DocSetId {DocSetId}. InvocationId: {InvocationId}",
                                                        nameof(ProcessEmailsAsync), "QueueForNonSpecific", fileTypeInstance.Id, currentDocSetId, fileTypeInvocationId);
                                                }
                                            }
                                            else {
                                               log.Information("INTERNAL_STEP ({OperationName} - {Stage}): ReadOnlyMode is TRUE. Skipping data modification actions (ExecuteDataSpecificFileActions, ExecuteNonSpecificFileActions). InvocationId: {InvocationId}",
                                                    nameof(ProcessEmailsAsync), "ReadOnlySkip", fileTypeInvocationId);
                                            }
                                        } // End FileTypeInvocationId LogContext
                                    } // end foreach emailFileTypeDefinition
                                   log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Successfully processed content for email {EmailIdForLogging}. InvocationId: {InvocationId}",
                                        nameof(ProcessEmailsAsync), "InnerEmailProcessingSuccess", emailIdForLogging, emailIterationInvocationId);
                                }
                                catch (OperationCanceledException oceInner)
                                {
                                   log.Warning(oceInner, "ACTION_END_FAILURE: {ActionName} (Cancelled). InvocationId: {InvocationId}",
                                        $"Inner Email Content Processing for {emailIdForLogging}", emailIterationInvocationId);
                                    throw;
                                }
                                catch (Exception exInner)
                                {
                                   log.Error(exInner, "ACTION_END_FAILURE: {ActionName}. InvocationId: {InvocationId}",
                                        $"Inner Email Content Processing for {emailIdForLogging}", emailIterationInvocationId);
                                    
                                    if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                                    {
                                       log.Error("INTERNAL_STEP ({OperationName} - {Stage}): IMAP client disconnected during inner content processing. Aborting further email checks. EmailIdForLogging: {EmailIdForLogging}. InvocationId: {InvocationId}",
                                            nameof(ProcessEmailsAsync), "InnerEmailImapError", emailIdForLogging, emailIterationInvocationId);
                                        break;
                                    }
                                }
                            } // End EmailIterationInvocationId LogContext
                        } // end foreach emailTask
                    } // end try block for ImapClient operations
                    finally
                    {
                        if (imapClient != null)
                        {
                            if (imapClient.IsConnected)
                            {
                                try
                                {
                                   log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Disconnecting IMAP client for {EmailAccount}.",
                                        nameof(ProcessEmailsAsync), "ImapDisconnectAttempt", Utils.Client.Email);
                                    var disconnectStopwatch = Stopwatch.StartNew();
                                    await imapClient.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false); // Use CancellationToken.None for cleanup
                                    disconnectStopwatch.Stop();
                                   log.Information("INTERNAL_STEP ({OperationName} - {Stage}): IMAP client disconnected. Duration: {DurationMs}ms.",
                                        nameof(ProcessEmailsAsync), "ImapDisconnectSuccess", disconnectStopwatch.ElapsedMilliseconds);
                                }
                                catch (Exception dex)
                                {
                                   log.Warning(dex, "INTERNAL_STEP ({OperationName} - {Stage}): Error disconnecting IMAP client for {EmailAccount}.",
                                        nameof(ProcessEmailsAsync), "ImapDisconnectError", Utils.Client.Email);
                                }
                            }
                            imapClient.Dispose();
                           log.Information("INTERNAL_STEP ({OperationName} - {Stage}): IMAP client disposed for {EmailAccount}.",
                                nameof(ProcessEmailsAsync), "ImapDispose", Utils.Client.Email);
                        }
                    }
                   log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processed {ProcessedEmailCount} emails individually.",
                        nameof(ProcessEmailsAsync), "IndividualEmailProcessingSummary", processedEmailCount);
                    if (Program.ReadOnlyMode)
                    {
                        // methodStopwatch.Stop(); // methodStopwatch is not declared, using stopwatch instead
                       log.Information("INTERNAL_STEP ({OperationName} - {Stage}): {ActionName} (ReadOnlyMode: NonSpecificActions Skipped). Processed {ProcessedEmailCount} emails. Duration: {TotalObservedDurationMs}ms",
                            nameof(ProcessEmailsAsync), "ReadOnlySkip", nameof(ProcessEmailsAsync), processedEmailCount, stopwatch.ElapsedMilliseconds);
                        return;
                    }
                    var pfg = filesForNonSpecificActions
                        .Where(x => x.Item1.FileTypeActions.Any(z => z.Actions.IsDataSpecific == null || z.Actions.IsDataSpecific != true))
                        .GroupBy(x => x.Item3).ToList();
                   log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing {GroupCount} groups for non-specific file actions.",
                        nameof(ProcessEmailsAsync), "GroupedNonSpecificActionsStart", pfg.Count);
                    
                    if (!pfg.Any() && filesForNonSpecificActions.Any()) {
                       log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                            nameof(ProcessEmailsAsync), "GroupedNonSpecificActionsWarning", "filesForNonSpecificActions (grouped)", "filesForNonSpecificActions has items, but grouping resulted in an empty collection. This might indicate an issue with grouping or action definitions.");
                    }
                    foreach (var docSetIdGroup in pfg)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string groupInvocationId = Guid.NewGuid().ToString();
                        using(LogContext.PushProperty("InvocationId", groupInvocationId))
                        {
                           log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing non-specific actions for DocSetIdGroupKey: {DocSetIdGroupKey}. InvocationId: {InvocationId}",
                                nameof(ProcessEmailsAsync), "ProcessDocSetIdGroup", docSetIdGroup.Key, groupInvocationId);
                            
                            var pf = docSetIdGroup.DistinctBy(x => x.Item1.Id).ToList();
                            foreach (var t in pf)
                            {
                                t.Item1.AsycudaDocumentSetId = docSetIdGroup.Key;
                                var execNonSpecificGroupedStopwatch = Stopwatch.StartNew();
                               log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for FileType {FileTypeId}, DocSetId {DocSetId}. InvocationId: {InvocationId}",
                                    "ImportUtils.ExecuteNonSpecificFileActions (grouped)", "ASYNC_EXPECTED", t.Item1.Id, docSetIdGroup.Key, groupInvocationId);
                                
                                await ImportUtils.ExecuteNonSpecificFileActions(t.Item1, t.Item2, appSetting)
                                    .ConfigureAwait(false);
                                
                                execNonSpecificGroupedStopwatch.Stop();
                               log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for FileType {FileTypeId}, DocSetId {DocSetId}. InvocationId: {InvocationId}",
                                    "ImportUtils.ExecuteNonSpecificFileActions (grouped)", execNonSpecificGroupedStopwatch.ElapsedMilliseconds, "Async call completed (await).", t.Item1.Id, docSetIdGroup.Key, groupInvocationId);
                            }
                        }
                    }
                }
                catch (OperationCanceledException oceMain)
                {
                    stopwatch.Stop();
                   log.Warning(oceMain, "ACTION_END_FAILURE: {ActionName} (Cancelled). Processed {ProcessedEmailCount} emails. Duration: {TotalObservedDurationMs}ms. AppSettingId: {AppSettingId}",
                        nameof(ProcessEmailsAsync), processedEmailCount, stopwatch.ElapsedMilliseconds, appSetting.ApplicationSettingsId);
                    throw;
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                   log.Error(e, "ACTION_END_FAILURE: {ActionName}. Processed {ProcessedEmailCount} emails. Duration: {TotalObservedDurationMs}ms. AppSettingId: {AppSettingId}",
                        nameof(ProcessEmailsAsync), processedEmailCount, stopwatch.ElapsedMilliseconds, appSetting.ApplicationSettingsId);
                    throw;
                }
                finally
                {
                    if (stopwatch.IsRunning) stopwatch.Stop(); // Ensure it's stopped
                   log.Information("ACTION_END_SUCCESS: {ActionName}. Processed {ProcessedEmailCount} emails. Total observed duration: {TotalObservedDurationMs}ms. AppSettingId: {AppSettingId}",
                        nameof(ProcessEmailsAsync), processedEmailCount, stopwatch.ElapsedMilliseconds, appSetting.ApplicationSettingsId);
                }
            }
        }
    }
}