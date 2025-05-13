using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace AutoBot
{
    using global::AutoBot.Properties;

    // ReSharper disable once HollowTypeName
    public class EmailProcessor
    {
        public static async Task ProcessEmailsAsync(
            ApplicationSettings appSetting,
            DateTime beforeImport,
            CoreEntitiesContext ctx,
            CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Entering ProcessEmailsAsync for appSetting {appSetting.ApplicationSettingsId}");
            try
            {
                if (string.IsNullOrEmpty(appSetting.Email))
                {
                    Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Parameter Validation: Email is null or empty for appSetting {appSetting.ApplicationSettingsId}. Exiting.");
                    return;
                }
                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Parameter Validation: Email is provided for appSetting {appSetting.ApplicationSettingsId}.");

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

                int processedEmailCount = 0;
                var filesForNonSpecificActions =
                    new List<Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>>();

                ImapClient imapClient = null; // Declare ImapClient here to manage its lifecycle
                try
                {
                    Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Attempting to connect IMAP client for {Utils.Client.Email}");
                    imapClient = await EmailDownloader.EmailDownloader
                                     .GetOpenImapClientAsync(Utils.Client, cancellationToken).ConfigureAwait(false);
                    Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after GetOpenImapClientAsync. IMAP client connected status: {imapClient?.IsConnected}");

                    if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                    {
                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Failed to open IMAP client for {Utils.Client.Email}. State: Connected={imapClient?.IsConnected}, Authenticated={imapClient?.IsAuthenticated}");
                        return; // Exit if client can't be established
                    }
                    Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Successfully connected IMAP client for {Utils.Client.Email}");

                    // Pass the connected imapClient to StreamEmailResultsAsync
                    foreach (Task<EmailDownloader.EmailProcessingResult> emailTask in EmailDownloader.EmailDownloader
                                 .StreamEmailResultsAsync(imapClient, Utils.Client, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        EmailDownloader.EmailProcessingResult currentEmailResult;
                        try
                        {
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Awaiting emailTask.");
                            currentEmailResult = await emailTask.ConfigureAwait(false);
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after emailTask. Result received.");
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine("Email processing task was canceled during await.");
                            throw;
                        }
                        catch (Exception taskEx)
                        {
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Exception caught during email task processing: {taskEx.Message}. StackTrace: {taskEx.StackTrace}");
                            // Check if IMAP client is still usable, otherwise break or re-establish
                            if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                            {
                                Console.WriteLine(
                                    $"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] IMAP client disconnected during task processing. Aborting further email checks for this appSetting.");
                                break; // Exit the foreach loop for tasks
                            }

                            continue;
                        }

                        if (currentEmailResult == null)
                        {
                            continue;
                        }

                        processedEmailCount++;
                        var emailKey = currentEmailResult.EmailKey;
                        var attachments = currentEmailResult.AttachedFiles.ToArray();
                        
                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Processing email {emailKey.SubjectIdentifier} with {attachments.Length} attachments");
                        foreach (var att in attachments)
                        {
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Attachment: {att.Name} (Size: {att.Length} bytes, LastModified: {att.LastWriteTime})");
                            if (att.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] PDF attachment detected: {att.Name}");
                            }
                        }

                        try
                        {
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Calling ExecuteEmailMappingActions for email {emailKey.SubjectIdentifier}");
                            await ImportUtils.ExecuteEmailMappingActions(
                                emailKey.EmailMessage.EmailMapping,
                                new FileTypes() { EmailId = emailKey.EmailMessage.EmailId },
                                attachments,
                                appSetting).ConfigureAwait(false);
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after ExecuteEmailMappingActions.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Exception caught in ExecuteEmailMappingActions for email {emailKey.SubjectIdentifier}: {ex}. StackTrace: {ex.StackTrace}");
                            throw;
                        }

                        var msgResult = currentEmailResult;
                        // Inner processing loop for the current email (msgResult)
                        // This block remains the same as your existing production code's second loop,
                        // but operates on 'msgResult' (which is currentEmailResult).
                        // All 'await' calls within should use .ConfigureAwait(false).
                        // START OF YOUR EXISTING PRODUCTION CODE'S SECOND LOOP LOGIC (ADAPTED)
                        var emailKeyTuple = msgResult.EmailKey;
                        var msgAttachments = msgResult.AttachedFiles;
                        var emailForLog = emailKeyTuple.EmailMessage; // Changed Item2 to EmailMessage
                        var emailIdForLogging = emailForLog?.EmailUniqueId.ToString()
                                                ?? $"UnknownEmailId_{emailKeyTuple.SubjectIdentifier}"; // Changed Item1 to SubjectIdentifier

                        try
                        {
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Entering inner email processing loop for email: {emailIdForLogging}");

                            var desFolder = Path.Combine(
                                appSetting.DataFolder,
                                emailKeyTuple.SubjectIdentifier, // Changed Item1 to SubjectIdentifier
                                emailForLog.EmailUniqueId.ToString());

                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Checking required files criteria for email: {emailIdForLogging}");
                            if (!emailForLog.EmailMapping.EmailFileTypes.All(
                                    x => x.IsRequired != true || msgAttachments.Any(
                                             att => Regex.IsMatch(
                                                        att.FullName,
                                                        x.FileTypes.FilePattern,
                                                        RegexOptions.IgnoreCase) && att.LastWriteTime >= beforeImport)))
                            {
                                Console.WriteLine(
                                    $"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Skipping email {emailIdForLogging}, required files criteria not met.");
                                continue; // to next emailTask in the outer loop
                            }
                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Required files criteria met for email: {emailIdForLogging}");

                            var fileTypesForOrdering =
                                emailForLog.FileTypes
                                ?? new List<CoreEntities.Business.Entities.
                                    FileTypes>(); // Defensive: ensure FileTypes is not null
                            var emailFileTypes = emailForLog.EmailMapping.InfoFirst == true
                                                     ? fileTypesForOrdering.OrderByDescending(
                                                         x => x.FileImporterInfos.EntryType == "Info").ToList()
                                                     : fileTypesForOrdering.OrderBy(
                                                         x => x.FileImporterInfos.EntryType == "Info").ToList();

                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Processing {emailFileTypes.Count} file types for email {emailIdForLogging}");
                            foreach (var emailFileTypeDefinition in emailFileTypes)
                            {
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Processing file type: {emailFileTypeDefinition.FilePattern}");
                                cancellationToken.ThrowIfCancellationRequested();
                                var fileTypeInstance =
                                    FileTypeManager.GetFileType(
                                        emailFileTypeDefinition); // Ensure this returns a new/cloned instance if state is modified
                                fileTypeInstance.Data.Clear();
                                fileTypeInstance.EmailInfoMappings = emailForLog.EmailMapping.EmailInfoMappings;

                                var csvFiles = msgAttachments.Where(
                                    x => Regex.IsMatch(
                                             x.FullName,
                                             fileTypeInstance.FilePattern,
                                             RegexOptions.IgnoreCase) && x.LastWriteTime >= beforeImport)
                                    .Select(x => x)
                                    .ToArray();

                                fileTypeInstance.EmailId = emailForLog.EmailId;
                                fileTypeInstance.FilePath = desFolder;
                                if (csvFiles.Length == 0) continue;

                                var reference = emailKeyTuple.SubjectIdentifier; // Changed Item1 to SubjectIdentifier

                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Searching for existing AsycudaDocumentSet with reference: {reference}");
                                var docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                     x => x.Declarant_Reference_Number.Contains(reference)
                                                          && x.ApplicationSettingsId == BaseDataModel.Instance
                                                              .CurrentApplicationSettings.ApplicationSettingsId)
                                                 .ConfigureAwait(false);
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after FirstOrDefaultAsync (docSet search). DocSet found: {docSet != null}");

                                if (fileTypeInstance.CreateDocumentSet)
                                {
                                    if (docSet == null || docSet.Declarant_Reference_Number != reference)
                                    {
                                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Searching for existing AsycudaDocumentSet with exact reference: {reference}");
                                        docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                         x => x.Declarant_Reference_Number == reference
                                                              && x.ApplicationSettingsId == BaseDataModel.Instance
                                                                  .CurrentApplicationSettings.ApplicationSettingsId)
                                                     .ConfigureAwait(false);
                                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after FirstOrDefaultAsync (exact docSet search). DocSet found: {docSet != null}");
                                        if (docSet == null)
                                        {
                                            var cp = BaseDataModel.Instance.Customs_Procedures.First(
                                                x => x.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation()
                                                     && x.IsDefault == true);

                                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] No existing DocSet found, creating a new one with reference: {reference}");
                                            await ctx.Database.ExecuteSqlCommandAsync(
                                                TransactionalBehavior.EnsureTransaction,
                                                $@"INSERT INTO AsycudaDocumentSet (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                                   VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{reference.Replace("'", "''")}',{cp.Customs_ProcedureId},0)",
                                                cancellationToken).ConfigureAwait(false);
                                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after ExecuteSqlCommandAsync (Insert DocSet).");

                                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Re-searching for the newly created DocSet with reference: {reference}");
                                            docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                             x => x.Declarant_Reference_Number == reference
                                                                  && x.ApplicationSettingsId == BaseDataModel.Instance
                                                                      .CurrentApplicationSettings.ApplicationSettingsId)
                                                         .ConfigureAwait(false);
                                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after FirstOrDefaultAsync (new DocSet search). DocSet found: {docSet != null}");
                                        }
                                    }
                                }

                                if (docSet != null)
                                {
                                    fileTypeInstance.AsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                                    fileTypeInstance.Data.Add(
                                        new KeyValuePair<string, string>(
                                            "AsycudaDocumentSetId",
                                            fileTypeInstance.AsycudaDocumentSetId.ToString()));
                                }

                                // Utils.SaveAttachments was called in original code, assuming it's still relevant
                                // If attachments are already saved by EmailDownloader in the correct place, this might be redundant
                                // or it might perform additional DB logging/linking. Keeping for consistency with original.
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Calling Utils.SaveAttachments for {csvFiles.Length} files.");
                                await Utils.SaveAttachments(csvFiles, fileTypeInstance, emailForLog)
                                    .ConfigureAwait(false); // Assuming sync or make async
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after Utils.SaveAttachments.");

                                if (!Program.ReadOnlyMode)
                                {
                                    Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Calling ImportUtils.ExecuteDataSpecificFileActions.");
                                    await ImportUtils.ExecuteDataSpecificFileActions(
                                        fileTypeInstance,
                                        csvFiles,
                                        appSetting).ConfigureAwait(false);
                                    Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after ExecuteDataSpecificFileActions.");
                                    if (emailForLog.EmailMapping.IsSingleEmail == true)
                                    {
                                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Calling ImportUtils.ExecuteNonSpecificFileActions (single email).");
                                        await ImportUtils.ExecuteNonSpecificFileActions(
                                            fileTypeInstance,
                                            csvFiles,
                                            appSetting).ConfigureAwait(false);
                                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after ExecuteNonSpecificFileActions (single email).");
                                    }
                                    else
                                    {
                                        int currentDocSetId = 0;
                                        if (docSet != null) currentDocSetId = docSet.AsycudaDocumentSetId;
                                        else
                                        {
                                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Searching for the last DocSet for non-specific actions.");
                                            var lastDocSet = await ctx.AsycudaDocumentSet
                                                                 .Where(
                                                                     x => x.ApplicationSettingsId
                                                                          == BaseDataModel.Instance
                                                                              .CurrentApplicationSettings
                                                                              .ApplicationSettingsId)
                                                                 .OrderByDescending(x => x.AsycudaDocumentSetId)
                                                                 .FirstOrDefaultAsync().ConfigureAwait(false);
                                            Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after FirstOrDefaultAsync (last DocSet search). Last DocSet found: {lastDocSet != null}");
                                            if (lastDocSet != null) currentDocSetId = lastDocSet.AsycudaDocumentSetId;
                                        }

                                        filesForNonSpecificActions.Add(
                                            new Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>(
                                                fileTypeInstance,
                                                csvFiles,
                                                currentDocSetId));
                                    }
                                }
                            } // end foreach emailFileTypeDefinition

                            Console.WriteLine($"Successfully processed email: {emailIdForLogging}");
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Exception caught during specific email content processing for {emailIdForLogging}: {ex.Message}. StackTrace: {ex.StackTrace}");
                            // Decide if IMAP client is still healthy or if we need to break
                            if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                            {
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] IMAP client disconnected during content processing. Aborting.");
                                break; // Exit the foreach loop for tasks
                            }
                        }
                        // END OF YOUR EXISTING PRODUCTION CODE'S SECOND LOOP LOGIC (ADAPTED)
                    } // end foreach emailTask
                } // end try block for ImapClient
                finally
                {
                    if (imapClient != null)
                    {
                        if (imapClient.IsConnected)
                        {
                            try
                            {
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Disconnecting IMAP client.");
                                await imapClient.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false);
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after DisconnectAsync.");
                            }
                            catch (Exception dex)
                            {
                                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Error disconnecting IMAP client: {dex.Message}. StackTrace: {dex.StackTrace}");
                            }
                        }

                        imapClient.Dispose();
                    }
                }

                var finalProcessedCount = processedEmailCount;
                Console.WriteLine($"{finalProcessedCount} Emails processed individually.");

                if (Program.ReadOnlyMode) return;

                var pfg = filesForNonSpecificActions
                    .Where(
                        x => x.Item1.FileTypeActions.Any(
                            z => z.Actions.IsDataSpecific == null || z.Actions.IsDataSpecific != true))
                    .GroupBy(x => x.Item3).ToList();

                foreach (var docSetIdGroup in pfg)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var pf = docSetIdGroup.DistinctBy(x => x.Item1.Id).ToList();
                    foreach (var t in pf)
                    {
                        t.Item1.AsycudaDocumentSetId = docSetIdGroup.Key;
                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Calling ImportUtils.ExecuteNonSpecificFileActions (grouped by DocSetId).");
                        await ImportUtils.ExecuteNonSpecificFileActions(t.Item1, t.Item2, appSetting)
                            .ConfigureAwait(false);
                        Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Await transition after ExecuteNonSpecificFileActions (grouped by DocSetId).");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] ProcessEmailsAsync was canceled.");
                throw; // Re-throw to be caught by MainAsync/Main
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Critical error in ProcessEmailsAsync for appSetting {appSetting.ApplicationSettingsId}: {e}. StackTrace: {e.StackTrace}");
                throw;
            }
            finally
            {
                var duration = DateTime.Now - startTime;
                Console.WriteLine($"[{DateTime.Now:O}] [Thread:{Thread.CurrentThread.ManagedThreadId}] Exiting ProcessEmailsAsync for appSetting {appSetting.ApplicationSettingsId}. Duration: {duration.TotalSeconds}s");
            }
        }
    }
}