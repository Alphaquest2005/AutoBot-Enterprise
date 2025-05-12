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

                    await EmailProcessor.ProcessEmailsAsync(appSetting, timeBeforeImport, ctx, cancellationToken).ConfigureAwait(false);

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