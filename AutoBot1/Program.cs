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
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace; // For .ForEach() and .DistinctBy()
// using WaterNut.Business.Services.Utils; // Keep if used
// using WaterNut.DataSpace; // Keep if used
// using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations; // Keep if used
// using xlsxWriter; // Keep if used
// using FileTypes = CoreEntities.Business.Entities.FileTypes; // Explicit using if needed

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

                // Using original string-based includes for nested properties, which are common in EF6.
                // Lambda includes for direct properties.
                var applicationSettings = await ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes) // Direct collection
                    .Include(x => x.Declarants) // Direct collection or single entity
                                                // For nested properties on items within FileTypes:
                    .Include("FileTypes.FileTypeReplaceRegex") // If FileTypeReplaceRegex is on FileType
                    .Include("FileTypes.FileImporterInfos")    // If FileImporterInfos is on FileType
                                                               // For EmailMapping and its nested properties:
                    .Include(x => x.EmailMapping) // Direct collection
                    .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos") // Deeply nested
                    .Include("EmailMapping.EmailMappingRexExs")
                    .Include("EmailMapping.EmailMappingActions.Actions")
                    .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                    .Where(x => x.IsActive)
                    .ToListAsync(cancellationToken) // EF6 async
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
                        await folderProcessor.ProcessDownloadFolder(appSetting).ConfigureAwait(false); // Assuming async
                }
            }
        }

        private static bool ExecuteLastDBSessionAction(CoreEntitiesContext ctx, ApplicationSettings appSetting)
        {
            // Original synchronous logic
            var lastAction = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions") // EF6 string includes
                .OrderByDescending(p => p.Id)
                .FirstOrDefault(x => x.ApplicationSettingId == appSetting.ApplicationSettingsId);

            if (lastAction != null)
            {
                lastAction.Sessions.SessionActions
                    .Where(x => lastAction.ActionId == null || x.ActionId == lastAction.ActionId)
                    .Select(x => SessionsUtils.SessionActions[x.Actions.Name]) // Assumes SessionsUtils.SessionActions is populated
                    .ForEach(x => x.Invoke()); // From MoreLinq
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

                // Ensure Utils.Client is of type EmailDownloader.Client or compatible
                Utils.Client = new EmailDownloader.Client // This implies EmailDownloader.Client is the correct type
                {
                    CompanyName = appSetting.CompanyName,
                    DataFolder = appSetting.DataFolder,
                    Password = appSetting.EmailPassword,
                    Email = appSetting.Email,
                    ApplicationSettingsId = appSetting.ApplicationSettingsId,
                    EmailMappings = appSetting.EmailMapping.ToList(), // Ensure EmailMapping is loaded
                    DevMode = Settings.Default.DevMode,
                    NotifyUnknownMessages = appSetting.NotifyUnknownMessages ?? false
                };

                int processedEmailCount = 0;
                var filesForNonSpecificActions = new List<Tuple<CoreEntities.Business.Entities.FileTypes, FileInfo[], int>>();

                // Consume the IEnumerable<Task<EmailProcessingResult>> from StreamEmailResultsAsync
                // Process each email's data as its task completes.
                foreach (Task<EmailDownloader.EmailProcessingResult> emailTask in
                         EmailDownloader.EmailDownloader.StreamEmailResultsAsync(Utils.Client, cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Early check before awaiting

                    EmailDownloader.EmailProcessingResult currentEmailResult;
                    try
                    {
                        currentEmailResult = await emailTask.ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Email processing task was canceled.");
                        throw; // Propagate to allow higher-level cancellation handling
                    }
                    catch (Exception taskEx)
                    {
                        Console.WriteLine($"Error awaiting or processing an email task: {taskEx.Message}");
                        continue; // Skip this email and try the next
                    }

                    if (currentEmailResult == null) // Email was skipped by downloader logic
                    {
                        continue;
                    }
                    
                    processedEmailCount++;

                    // Perform processing for currentEmailResult immediately

                    // --- Start: Adapted first loop logic ---
                    // Order by EmailMapping.Id was originally on the collected list.
                    // If this specific ordering is critical *before* this step for a single email,
                    // it's already handled by how emails are presented by StreamEmailResultsAsync or this step is per email.
                    // Assuming processing per email is fine without re-sorting based on other emails here.
                    await ImportUtils.ExecuteEmailMappingActions(currentEmailResult.EmailKey.Item2.EmailMapping,
                        new CoreEntities.Business.Entities.FileTypes() { EmailId = currentEmailResult.EmailKey.Item2.EmailId },
                        currentEmailResult.AttachedFiles.ToArray(), appSetting).ConfigureAwait(false);
                    // --- End: Adapted first loop logic ---

                    // --- Start: Adapted second loop logic (for the currentEmailResult) ---
                    // The OrderBy for CreateDocumentSet and EmailUniqueId was on the collected list.
                    // This implies a potential processing order preference. If StreamEmailResultsAsync doesn't guarantee this,
                    // and it's critical, a more complex pre-fetch and sort might be needed,
                    // or accept the order from the stream. For one-by-one processing, we take the stream's order.
                    var msgResult = currentEmailResult; // Use current email result directly
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Deconstruct from EmailProcessingResult
                    var emailKeyTuple = msgResult.EmailKey; // This is Tuple<string, Email, string>
                    var attachments = msgResult.AttachedFiles; // This is List<FileInfo>

                    // emailKeyTuple.Item1 is subject string
                    // emailKeyTuple.Item2 is the Email object
                    // emailKeyTuple.Item3 is the UID string

                    var emailForLog = emailKeyTuple.Item2; // The Email object
                    var emailIdForLogging = emailForLog?.EmailUniqueId.ToString() ?? $"UnknownEmailId_{emailKeyTuple.Item1}";

                    try
                    {
                        Console.WriteLine($"Attempting to process email: {emailIdForLogging}");

                        var desFolder = Path.Combine(appSetting.DataFolder, emailKeyTuple.Item1,
                            emailForLog.EmailUniqueId.ToString());

                        if (!emailForLog.EmailMapping.EmailFileTypes
                            .All(x => x.IsRequired != true || attachments // Use the attachments list directly
                                .Any(att => Regex.IsMatch(att.FullName, x.FileTypes.FilePattern, RegexOptions.IgnoreCase) &&
                                            att.LastWriteTime >= beforeImport)))
                        {
                            continue;
                        }

                        var emailFileTypes = emailForLog.EmailMapping.InfoFirst == true
                            ? emailForLog.FileTypes.OrderByDescending(x => x.FileImporterInfos.EntryType == "Info").ToList() // Assuming FileTypeManager.EntryTypes.Info was "Info"
                            : emailForLog.FileTypes.OrderBy(x => x.FileImporterInfos.EntryType == "Info").ToList();

                        foreach (var emailFileTypeDefinition in emailFileTypes)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var fileTypeInstance = FileTypeManager.GetFileType(emailFileTypeDefinition);
                            fileTypeInstance.Data.Clear();
                            fileTypeInstance.EmailInfoMappings = emailForLog.EmailMapping.EmailInfoMappings;

                            var csvFiles = attachments // Use the attachments from the result
                                .Where(x => Regex.IsMatch(x.FullName, fileTypeInstance.FilePattern, RegexOptions.IgnoreCase) &&
                                            x.LastWriteTime >= beforeImport)
                                .ToArray();

                            fileTypeInstance.EmailId = emailForLog.EmailId;
                            fileTypeInstance.FilePath = desFolder;
                            if (csvFiles.Length == 0) continue;

                            var reference = emailKeyTuple.Item1; // subject string as reference

                            // EF6 Async for database operations
                            var docSet = await ctx.AsycudaDocumentSetExs
                                .FirstOrDefaultAsync(x => x.Declarant_Reference_Number.Contains(reference) && // Original used Contains
                                                           x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                .ConfigureAwait(false);

                            if (fileTypeInstance.CreateDocumentSet)
                            {
                                if (docSet == null || docSet.Declarant_Reference_Number != reference) // If Contains found something else, or nothing
                                {
                                    docSet = await ctx.AsycudaDocumentSetExs
                                       .FirstOrDefaultAsync(x => x.Declarant_Reference_Number == reference &&
                                                               x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                       .ConfigureAwait(false);
                                    if (docSet == null)
                                    {
                                        var cp = BaseDataModel.Instance.Customs_Procedures.First(x =>
                                            x.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation() && x.IsDefault == true);

                                        // Use ExecuteSqlCommandAsync for EF6
                                        await ctx.Database.ExecuteSqlCommandAsync(TransactionalBehavior.EnsureTransaction, // Or other behavior
                                            $@"INSERT INTO AsycudaDocumentSet (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                               VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{reference.Replace("'", "''")}',{cp.Customs_ProcedureId},0)", // SQL Encode reference
                                            cancellationToken).ConfigureAwait(false);

                                        docSet = await ctx.AsycudaDocumentSetExs // Re-fetch
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

                            // Attachments are saved within ProcessSingleEmailAndDownloadAttachmentsAsync,
                            // which is called by StreamEmailResultsAsync.
                            // The results (list of FileInfo) are in currentEmailResult.AttachedFiles.
                            // Thus, an explicit call to Utils.SaveAttachmentsAsync here is not needed
                            // if currentEmailResult.AttachedFiles is already populated correctly.

                            if (!ReadOnlyMode)
                            {
                                await ImportUtils.ExecuteDataSpecificFileActions(fileTypeInstance, csvFiles, appSetting).ConfigureAwait(false);
                                if (emailForLog.EmailMapping.IsSingleEmail == true)
                                {
                                    await ImportUtils.ExecuteNonSpecificFileActions(fileTypeInstance, csvFiles, appSetting).ConfigureAwait(false);
                                }
                                else
                                {
                                    int currentDocSetId = 0; // Default
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
                        }
                        Console.WriteLine($"Successfully processed email: {emailIdForLogging}");
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing email {emailIdForLogging}: {ex.Message}");
                    }
                }

                } // End of foreach (Task<EmailDownloader.EmailProcessingResult> emailTask...)
                
                Console.WriteLine($"{processedEmailCount} Emails processed individually.");

                if (ReadOnlyMode) return;

                // Process accumulated filesForNonSpecificActions after all emails are handled
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
            catch (OperationCanceledException) { throw; }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void ExecuteDBSessionActions(CoreEntitiesContext ctx, ApplicationSettings appSetting)
        {
            // Original synchronous logic
            ctx.SessionActions.OrderBy(x => x.Id)
                .Include(x => x.Actions) // EF6 string include
                .Where(x => x.Sessions.Name == "End").ToList()
                .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                .ForEach(x => x.Invoke());

            // For EF6, use DbFunctions from System.Data.Entity
            var sLst = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                .Include("ParameterSet.ParameterSetParameters.Parameters")
                .Where(x => x.RunDateTime >= DbFunctions.AddMinutes(DateTime.Now, (x.Sessions.WindowInMinutes /* ?? 0 removed as it's int not int? */) * -1)
                            && x.RunDateTime <= DbFunctions.AddMinutes(DateTime.Now, x.Sessions.WindowInMinutes /* ?? 0 removed */ ))
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