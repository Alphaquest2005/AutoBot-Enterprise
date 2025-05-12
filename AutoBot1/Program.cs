using AutoBot; // For FolderProcessor
using CoreEntities.Business.Entities;
// using CoreEntities.Business.Enums; // Keep if used
using System;
using System.Collections.Generic;
using System.Data.Entity; // For EF6 async methods and DbFunctions
// using System.Data.Entity.SqlServer; // Only if SqlFunctions are used directly and DbFunctions don't suffice
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoBot.Properties; // For Settings.Default.DevMode
using AutoBotUtilities;    // Assuming this contains your Utils class definition
using Core.Common.Utils;  // For StringExtensions
// using EntryDataDS.Business.Entities; // Keep if used
using MoreLinq;
using WaterNut.Business.Services.Utils; // Assuming ImportUtils, FileTypeManager might be here or related
using WaterNut.DataSpace;             // Assuming SessionsUtils might be here or related
using MailKit.Net.Imap; // Added for ImapClient type

namespace AutoBot
{
    partial class Program
    {
        public static bool ReadOnlyMode { get; set; } = false;

        static void Main(string[] args) // Main is synchronous
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Cancellation requested...");
                cts.Cancel();
                e.Cancel = true;
            };

            try
            {
                MainAsync(args, cts.Token).GetAwaiter().GetResult(); // Run async part and wait
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Main operation was canceled.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unhandled error in Main: {e}");
                // Attempt to send error email (synchronously waiting for the async send)
                try
                {
                    // Ensure Utils.Client is either available or a default is created for error reporting
                    var errorReportingClient = Utils.Client ?? new EmailDownloader.Client { Email = "default-error-reporter@example.com", CompanyName = "ErrorReporter" };
                    string[] devContacts = { "developer@example.com" }; // Fallback
                    try { devContacts = EmailDownloader.EmailDownloader.GetContacts("Developer"); } catch {/* ignored */}

                    EmailDownloader.EmailDownloader.SendEmailAsync(errorReportingClient, null, $"Bug Found in AutoBot",
                         devContacts, $"{e.Message}\r\n{e.StackTrace}", Array.Empty<string>(), CancellationToken.None)
                         .GetAwaiter().GetResult();
                }
                catch (Exception mailEx)
                {
                    Console.WriteLine($"Failed to send error email: {mailEx}");
                }
            }
            finally
            {
                // Console.WriteLine("Press ENTER to close..."); // Optional
                // Console.ReadLine();
            }
        }

        static async Task MainAsync(string[] args, CancellationToken cancellationToken) // Async helper
        {
            Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");

            var timeBeforeImport = DateTime.Now;
            Console.WriteLine($"{timeBeforeImport}");
            using (var ctx = new CoreEntitiesContext() { })
            {
                ctx.Database.CommandTimeout = 10;

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

                foreach (var appSetting in applicationSettings)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Console.WriteLine($"{appSetting.SoftwareName} Emails Processed");
                    if (appSetting.DataFolder != null) appSetting.DataFolder = StringExtensions.UpdateToCurrentUser(appSetting.DataFolder);

                    BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                    var folderProcessor = new FolderProcessor();

                    if (appSetting.TestMode == true)
                    {
                        if (ExecuteLastDBSessionAction(ctx, appSetting)) continue;
                    }

                    await ProcessEmailsAsync(appSetting, timeBeforeImport, ctx, cancellationToken).ConfigureAwait(false);

                    ExecuteDBSessionActions(ctx, appSetting);

                    if (BaseDataModel.Instance.CurrentApplicationSettings.ProcessDownloadsFolder == true)
                        await folderProcessor.ProcessDownloadFolder(appSetting).ConfigureAwait(false); // Original was missing cancellationToken
                }
            }
        }

        private static bool ExecuteLastDBSessionAction(CoreEntitiesContext ctx, ApplicationSettings appSetting)
        {
            var lastAction = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(x => x.ApplicationSettingId == appSetting.ApplicationSettingsId);

            if (lastAction != null)
            {
                lastAction.Sessions.SessionActions
                    .Where(x => lastAction.ActionId == null || x.ActionId == lastAction.ActionId)
                    .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                    .ForEach(x => x.Invoke());
                return true;
            }
            return false;
        }

        private static async Task ProcessEmailsAsync(ApplicationSettings appSetting, DateTime beforeImport,
            CoreEntitiesContext ctx, CancellationToken cancellationToken)
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
                var filesForNonSpecificActions = new List<Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>>();

                ImapClient imapClient = null; // Declare ImapClient here to manage its lifecycle
                try
                {
                    imapClient = await EmailDownloader.EmailDownloader.GetOpenImapClientAsync(Utils.Client, cancellationToken).ConfigureAwait(false);

                    if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated)
                    {
                        Console.WriteLine($"Failed to open IMAP client for {Utils.Client.Email}. Skipping email processing for this appSetting.");
                        return; // Exit if client can't be established
                    }

                    // Pass the connected imapClient to StreamEmailResultsAsync
                    foreach (Task<EmailDownloader.EmailProcessingResult> emailTask in
                             EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClient, Utils.Client, cancellationToken))
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
                                Console.WriteLine("IMAP client disconnected during task processing. Aborting further email checks for this appSetting.");
                                break; // Exit the foreach loop for tasks
                            }
                            continue;
                        }

                        if (currentEmailResult == null)
                        {
                            continue;
                        }

                        processedEmailCount++;

                        await ImportUtils.ExecuteEmailMappingActions(currentEmailResult.EmailKey.Item2.EmailMapping,
                            new CoreEntities.Business.Entities.FileTypes() { EmailId = currentEmailResult.EmailKey.Item2.EmailId },
                            currentEmailResult.AttachedFiles.ToArray(), appSetting).ConfigureAwait(false);

                        var msgResult = currentEmailResult;
                        // Inner processing loop for the current email (msgResult)
                        // This block remains the same as your existing production code's second loop,
                        // but operates on 'msgResult' (which is currentEmailResult).
                        // All 'await' calls within should use .ConfigureAwait(false).
                        // START OF YOUR EXISTING PRODUCTION CODE'S SECOND LOOP LOGIC (ADAPTED)
                        var emailKeyTuple = msgResult.EmailKey;
                        var attachments = msgResult.AttachedFiles;
                        var emailForLog = emailKeyTuple.Item2;
                        var emailIdForLogging = emailForLog?.EmailUniqueId.ToString() ?? $"UnknownEmailId_{emailKeyTuple.Item1}";

                        try
                        {
                            Console.WriteLine($"Attempting to process email: {emailIdForLogging}");

                            var desFolder = Path.Combine(appSetting.DataFolder, emailKeyTuple.Item1,
                                emailForLog.EmailUniqueId.ToString());

                            if (!emailForLog.EmailMapping.EmailFileTypes
                                .All(x => x.IsRequired != true || attachments
                                    .Any(att => Regex.IsMatch(att.FullName, x.FileTypes.FilePattern, RegexOptions.IgnoreCase) &&
                                                att.LastWriteTime >= beforeImport)))
                            {
                                Console.WriteLine($"Skipping email {emailIdForLogging}, required files criteria not met.");
                                continue; // to next emailTask in the outer loop
                            }

                            var emailFileTypes = emailForLog.EmailMapping.InfoFirst == true
                                ? emailForLog.FileTypes.OrderByDescending(x => x.FileImporterInfos.EntryType == "Info").ToList()
                                : emailForLog.FileTypes.OrderBy(x => x.FileImporterInfos.EntryType == "Info").ToList();

                            foreach (var emailFileTypeDefinition in emailFileTypes)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var fileTypeInstance = FileTypeManager.GetFileType(emailFileTypeDefinition); // Ensure this returns a new/cloned instance if state is modified
                                fileTypeInstance.Data.Clear();
                                fileTypeInstance.EmailInfoMappings = emailForLog.EmailMapping.EmailInfoMappings;

                                var csvFiles = attachments
                                    .Where(x => Regex.IsMatch(x.FullName, fileTypeInstance.FilePattern, RegexOptions.IgnoreCase) &&
                                                x.LastWriteTime >= beforeImport)
                                    .ToArray();

                                fileTypeInstance.EmailId = emailForLog.EmailId;
                                fileTypeInstance.FilePath = desFolder;
                                if (csvFiles.Length == 0) continue;

                                var reference = emailKeyTuple.Item1;

                                var docSet = await ctx.AsycudaDocumentSetExs
                                    .FirstOrDefaultAsync(x => x.Declarant_Reference_Number.Contains(reference) &&
                                                               x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                    .ConfigureAwait(false);

                                if (fileTypeInstance.CreateDocumentSet)
                                {
                                    if (docSet == null || docSet.Declarant_Reference_Number != reference)
                                    {
                                        docSet = await ctx.AsycudaDocumentSetExs
                                           .FirstOrDefaultAsync(x => x.Declarant_Reference_Number == reference &&
                                                                   x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                           .ConfigureAwait(false);
                                        if (docSet == null)
                                        {
                                            var cp = BaseDataModel.Instance.Customs_Procedures.First(x =>
                                                x.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation() && x.IsDefault == true);

                                            await ctx.Database.ExecuteSqlCommandAsync(TransactionalBehavior.EnsureTransaction,
                                                $@"INSERT INTO AsycudaDocumentSet (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                                   VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{reference.Replace("'", "''")}',{cp.Customs_ProcedureId},0)",
                                                cancellationToken).ConfigureAwait(false);

                                            docSet = await ctx.AsycudaDocumentSetExs
                                                .FirstOrDefaultAsync(x => x.Declarant_Reference_Number == reference &&
                                                                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                                .ConfigureAwait(false);
                                        }
                                    }
                                }

                                if (docSet != null)
                                {
                                    fileTypeInstance.AsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                                    fileTypeInstance.Data.Add(new KeyValuePair<string, string>("AsycudaDocumentSetId", fileTypeInstance.AsycudaDocumentSetId.ToString()));
                                }

                                // Utils.SaveAttachments was called in original code, assuming it's still relevant
                                // If attachments are already saved by EmailDownloader in the correct place, this might be redundant
                                // or it might perform additional DB logging/linking. Keeping for consistency with original.
                                // Utils.SaveAttachments(csvFiles, fileTypeInstance, emailForLog); // Assuming sync or make async

                                if (!ReadOnlyMode)
                                {
                                    await ImportUtils.ExecuteDataSpecificFileActions(fileTypeInstance, csvFiles, appSetting).ConfigureAwait(false);
                                    if (emailForLog.EmailMapping.IsSingleEmail == true)
                                    {
                                        await ImportUtils.ExecuteNonSpecificFileActions(fileTypeInstance, csvFiles, appSetting).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        int currentDocSetId = 0;
                                        if (docSet != null) currentDocSetId = docSet.AsycudaDocumentSetId;
                                        else
                                        {
                                            var lastDocSet = await ctx.AsycudaDocumentSet
                                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                                .OrderByDescending(x => x.AsycudaDocumentSetId)
                                                .FirstOrDefaultAsync().ConfigureAwait(false);
                                            if (lastDocSet != null) currentDocSetId = lastDocSet.AsycudaDocumentSetId;
                                        }
                                        filesForNonSpecificActions.Add(new Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>(
                                            fileTypeInstance, csvFiles, currentDocSetId));
                                    }
                                }
                            } // end foreach emailFileTypeDefinition
                            Console.WriteLine($"Successfully processed email: {emailIdForLogging}");
                        }
                        catch (OperationCanceledException) { throw; }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing specific email content for {emailIdForLogging}: {ex.Message}");
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
                            catch (Exception dex) { Console.WriteLine($"Error disconnecting IMAP client: {dex.Message}"); }
                        }
                        imapClient.Dispose();
                    }
                }

                Console.WriteLine($"{processedEmailCount} Emails processed individually.");

                if (ReadOnlyMode) return;

                var pfg = filesForNonSpecificActions
                    .Where(x => x.Item1.FileTypeActions.Any(z => z.Actions.IsDataSpecific == null || z.Actions.IsDataSpecific != true))
                    .GroupBy(x => x.Item3).ToList();

                foreach (var docSetIdGroup in pfg)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var pf = docSetIdGroup.DistinctBy(x => x.Item1.Id).ToList();
                    foreach (var t in pf)
                    {
                        t.Item1.AsycudaDocumentSetId = docSetIdGroup.Key;
                        await ImportUtils.ExecuteNonSpecificFileActions(t.Item1, t.Item2, appSetting).ConfigureAwait(false);
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
                Console.WriteLine($"Critical error in ProcessEmailsAsync for appSetting {appSetting.ApplicationSettingsId}: {e}");
                // Consider if this error should stop all processing or just this appSetting.
                // For now, re-throwing will stop the current appSetting.
                throw;
            }
        }

        private static void ExecuteDBSessionActions(CoreEntitiesContext ctx, ApplicationSettings appSetting)
        {
            ctx.SessionActions.OrderBy(x => x.Id)
                .Include(x => x.Actions)
                .Where(x => x.Sessions.Name == "End").ToList()
                .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                .ForEach(x => x.Invoke());

            var sLst = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                .Include("ParameterSet.ParameterSetParameters.Parameters")
                .Where(x => x.RunDateTime >= DbFunctions.AddMinutes(DateTime.Now, (x.Sessions.WindowInMinutes) * -1) // Assuming WindowInMinutes is not nullable based on previous error
                            && x.RunDateTime <= DbFunctions.AddMinutes(DateTime.Now, x.Sessions.WindowInMinutes))
                .Where(x => (x.ApplicationSettingId == null || x.ApplicationSettingId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                .OrderBy(x => x.Id)
                .ToList();

            BaseDataModel.Instance.CurrentSessionSchedule = sLst;

            if (sLst.Any())
            {
                foreach (var item in sLst)
                {
                    item.Sessions.SessionActions
                        .Where(x => item.ActionId == null || x.ActionId == item.ActionId)
                        .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                        .Select(x => (SessionAction: x, Action: SessionsUtils.SessionActions[x.Actions.Name]))
                        .ForEach(x => { BaseDataModel.Instance.CurrentSessionAction = x.SessionAction; x.Action.Invoke(); });
                }
                BaseDataModel.Instance.CurrentSessionAction = null;
                BaseDataModel.Instance.CurrentSessionSchedule = new List<SessionSchedule>();
            }
            else
            {
                if (appSetting.AssessIM7 == true)
                    ctx.SessionActions.OrderBy(x => x.Id).Include(x => x.Actions)
                        .Where(x => x.Sessions.Name == "AssessIM7").ToList()
                        .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                        .Select(x => SessionsUtils.SessionActions[x.Actions.Name]).ForEach(x => x.Invoke());

                if (appSetting.AssessEX == true)
                    ctx.SessionActions.OrderBy(x => x.Id).Include(x => x.Actions)
                        .Where(x => x.Sessions.Name == "AssessEX").ToList()
                        .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                        .Select(x => SessionsUtils.SessionActions[x.Actions.Name]).ForEach(x => x.Invoke());
            }
        }
    }
}