

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

    public class EmailProcessor
    {
        public static async Task ProcessEmailsAsync(
            ApplicationSettings appSetting,
            DateTime beforeImport,
            CoreEntitiesContext ctx,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(appSetting.Email)) return;

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
                    imapClient = await EmailDownloader.EmailDownloader
                                     .GetOpenImapClientAsync(Utils.Client, cancellationToken).ConfigureAwait(false);

                    if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                    {
                        Console.WriteLine(
                            $"Failed to open IMAP client for {Utils.Client.Email}. Skipping email processing for this appSetting.");
                        return; // Exit if client can't be established
                    }

                    // Pass the connected imapClient to StreamEmailResultsAsync
                    foreach (Task<EmailDownloader.EmailProcessingResult> emailTask in EmailDownloader.EmailDownloader
                                 .StreamEmailResultsAsync(imapClient, Utils.Client, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        EmailDownloader.EmailProcessingResult currentEmailResult;
                        try
                        {
                            currentEmailResult = await emailTask.ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine("Email processing task was canceled during await.");
                            throw;
                        }
                        catch (Exception taskEx)
                        {
                            Console.WriteLine($"Error awaiting or processing an email task: {taskEx.Message}");
                            // Check if IMAP client is still usable, otherwise break or re-establish
                            if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                            {
                                Console.WriteLine(
                                    "IMAP client disconnected during task processing. Aborting further email checks for this appSetting.");
                                break; // Exit the foreach loop for tasks
                            }

                            continue;
                        }

                        if (currentEmailResult == null)
                        {
                            continue;
                        }

                        processedEmailCount++;

                        await ImportUtils.ExecuteEmailMappingActions(
                            currentEmailResult.EmailKey.EmailMessage.EmailMapping,
                            new FileTypes() { EmailId = currentEmailResult.EmailKey.EmailMessage.EmailId },
                            currentEmailResult.AttachedFiles.ToArray(),
                            appSetting).ConfigureAwait(false);

                        var msgResult = currentEmailResult;
                        // Inner processing loop for the current email (msgResult)
                        // This block remains the same as your existing production code's second loop,
                        // but operates on 'msgResult' (which is currentEmailResult).
                        // All 'await' calls within should use .ConfigureAwait(false).
                        // START OF YOUR EXISTING PRODUCTION CODE'S SECOND LOOP LOGIC (ADAPTED)
                        var emailKeyTuple = msgResult.EmailKey;
                        var attachments = msgResult.AttachedFiles;
                        var emailForLog = emailKeyTuple.EmailMessage; // Changed Item2 to EmailMessage
                        var emailIdForLogging = emailForLog?.EmailUniqueId.ToString()
                                                ?? $"UnknownEmailId_{emailKeyTuple.SubjectIdentifier}"; // Changed Item1 to SubjectIdentifier

                        try
                        {
                            Console.WriteLine($"Attempting to process email: {emailIdForLogging}");

                            var desFolder = Path.Combine(
                                appSetting.DataFolder,
                                emailKeyTuple.SubjectIdentifier, // Changed Item1 to SubjectIdentifier
                                emailForLog.EmailUniqueId.ToString());

                            if (!emailForLog.EmailMapping.EmailFileTypes.All(
                                    x => x.IsRequired != true || attachments.Any(
                                             att => Regex.IsMatch(
                                                        att.FullName,
                                                        x.FileTypes.FilePattern,
                                                        RegexOptions.IgnoreCase) && att.LastWriteTime >= beforeImport)))
                            {
                                Console.WriteLine(
                                    $"Skipping email {emailIdForLogging}, required files criteria not met.");
                                continue; // to next emailTask in the outer loop
                            }

                            var fileTypesForOrdering =
                                emailForLog.FileTypes
                                ?? new List<CoreEntities.Business.Entities.
                                    FileTypes>(); // Defensive: ensure FileTypes is not null
                            var emailFileTypes = emailForLog.EmailMapping.InfoFirst == true
                                                     ? fileTypesForOrdering.OrderByDescending(
                                                         x => x.FileImporterInfos.EntryType == "Info").ToList()
                                                     : fileTypesForOrdering.OrderBy(
                                                         x => x.FileImporterInfos.EntryType == "Info").ToList();

                            foreach (var emailFileTypeDefinition in emailFileTypes)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var fileTypeInstance =
                                    FileTypeManager.GetFileType(
                                        emailFileTypeDefinition); // Ensure this returns a new/cloned instance if state is modified
                                fileTypeInstance.Data.Clear();
                                fileTypeInstance.EmailInfoMappings = emailForLog.EmailMapping.EmailInfoMappings;

                                var csvFiles = attachments.Where(
                                    x => Regex.IsMatch(
                                             x.FullName,
                                             fileTypeInstance.FilePattern,
                                             RegexOptions.IgnoreCase) && x.LastWriteTime >= beforeImport).ToArray();

                                fileTypeInstance.EmailId = emailForLog.EmailId;
                                fileTypeInstance.FilePath = desFolder;
                                if (csvFiles.Length == 0) continue;

                                var reference = emailKeyTuple.SubjectIdentifier; // Changed Item1 to SubjectIdentifier

                                var docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                     x => x.Declarant_Reference_Number.Contains(reference)
                                                          && x.ApplicationSettingsId == BaseDataModel.Instance
                                                              .CurrentApplicationSettings.ApplicationSettingsId)
                                                 .ConfigureAwait(false);

                                if (fileTypeInstance.CreateDocumentSet)
                                {
                                    if (docSet == null || docSet.Declarant_Reference_Number != reference)
                                    {
                                        docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                         x => x.Declarant_Reference_Number == reference
                                                              && x.ApplicationSettingsId == BaseDataModel.Instance
                                                                  .CurrentApplicationSettings.ApplicationSettingsId)
                                                     .ConfigureAwait(false);
                                        if (docSet == null)
                                        {
                                            var cp = BaseDataModel.Instance.Customs_Procedures.First(
                                                x => x.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation()
                                                     && x.IsDefault == true);

                                            await ctx.Database.ExecuteSqlCommandAsync(
                                                TransactionalBehavior.EnsureTransaction,
                                                $@"INSERT INTO AsycudaDocumentSet (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                                   VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{reference.Replace("'", "''")}',{cp.Customs_ProcedureId},0)",
                                                cancellationToken).ConfigureAwait(false);

                                            docSet = await ctx.AsycudaDocumentSetExs.FirstOrDefaultAsync(
                                                             x => x.Declarant_Reference_Number == reference
                                                                  && x.ApplicationSettingsId == BaseDataModel.Instance
                                                                      .CurrentApplicationSettings.ApplicationSettingsId)
                                                         .ConfigureAwait(false);
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
                                await Utils.SaveAttachments(csvFiles, fileTypeInstance, emailForLog)
                                    .ConfigureAwait(false); // Assuming sync or make async

                                if (!Program.ReadOnlyMode)
                                {
                                    await ImportUtils.ExecuteDataSpecificFileActions(
                                        fileTypeInstance,
                                        csvFiles,
                                        appSetting).ConfigureAwait(false);
                                    if (emailForLog.EmailMapping.IsSingleEmail == true)
                                    {
                                        await ImportUtils.ExecuteNonSpecificFileActions(
                                            fileTypeInstance,
                                            csvFiles,
                                            appSetting).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        int currentDocSetId = 0;
                                        if (docSet != null) currentDocSetId = docSet.AsycudaDocumentSetId;
                                        else
                                        {
                                            var lastDocSet = await ctx.AsycudaDocumentSet
                                                                 .Where(
                                                                     x => x.ApplicationSettingsId
                                                                          == BaseDataModel.Instance
                                                                              .CurrentApplicationSettings
                                                                              .ApplicationSettingsId)
                                                                 .OrderByDescending(x => x.AsycudaDocumentSetId)
                                                                 .FirstOrDefaultAsync().ConfigureAwait(false);
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
                                $"Error processing specific email content for {emailIdForLogging}: {ex.Message}");
                            // Decide if IMAP client is still healthy or if we need to break
                            if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                            {
                                Console.WriteLine("IMAP client disconnected during content processing. Aborting.");
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
                                await imapClient.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false);
                            }
                            catch (Exception dex)
                            {
                                Console.WriteLine($"Error disconnecting IMAP client: {dex.Message}");
                            }
                        }

                        imapClient.Dispose();
                    }
                }

                Console.WriteLine($"{processedEmailCount} Emails processed individually.");

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
                        await ImportUtils.ExecuteNonSpecificFileActions(t.Item1, t.Item2, appSetting)
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("ProcessEmailsAsync was canceled.");
                throw; // Re-throw to be caught by MainAsync/Main
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    $"Critical error in ProcessEmailsAsync for appSetting {appSetting.ApplicationSettingsId}: {e}");
                // Consider if this error should stop all processing or just this appSetting.
                // For now, re-throwing will stop the current appSetting.
                throw;
            }
        }
    }
}