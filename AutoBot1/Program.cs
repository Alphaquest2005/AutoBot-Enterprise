﻿


using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoBot.Properties;
using AutoBotUtilities;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using xlsxWriter;
using FileTypes = CoreEntities.Business.Entities.FileTypes;



namespace AutoBot
{

    partial class Program
    {
        public static bool ReadOnlyMode { get; set; } = false;

        static async Task Main(string[] args)
        {
            try
            {
                Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");


                var timeBeforeImport = DateTime.Now;
                Console.WriteLine($"{timeBeforeImport}");
                using (var ctx = new CoreEntitiesContext() { })
                {
                    //ctx.Configuration.LazyLoadingEnabled = true;
                    //ctx.Configuration.ProxyCreationEnabled = true;
                    ctx.Database.CommandTimeout = 10;

                    var applicationSettings = ctx.ApplicationSettings.AsNoTracking()
                        .Include(x => x.FileTypes)
                        .Include(x => x.Declarants)
                        .Include("FileTypes.FileTypeReplaceRegex")
                        //.Include("FileTypes.FileTypeContacts.Contacts")
                        //.Include("FileTypes.FileTypeActions.Actions")
                        //.Include("FileTypes.AsycudaDocumentSetEx")
                        //.Include("EmailMapping.EmailFileTypes.FileTypes.FileTypeActions.Actions")
                        //.Include("EmailMapping.EmailFileTypes.FileTypes.FileTypeContacts.Contacts")
                        //.Include("EmailMapping.EmailFileTypes.FileTypes.AsycudaDocumentSetEx")
                        //.Include("EmailMapping.EmailFileTypes.FileTypes.FileTypeMappings")
                        //.Include("EmailMapping.EmailFileTypes.FileTypes.ChildFileTypes")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos")
                        .Include("EmailMapping.EmailMappingRexExs")
                        .Include("EmailMapping.EmailMappingActions.Actions")
                        .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                        //.Include("FileTypes.ChildFileTypes")
                        //.Include("FileTypes.FileTypeMappings")
                        .Where(x => x.IsActive)
                        .ToList();

                  

                    foreach (var appSetting in applicationSettings)
                    {
                        

                        Console.WriteLine($"{appSetting.SoftwareName} Emails Processed");
                        if(appSetting.DataFolder != null) appSetting.DataFolder = StringExtensions.UpdateToCurrentUser(appSetting.DataFolder);
                       
                        // set BaseDataModel CurrentAppSettings
                        BaseDataModel.Instance.CurrentApplicationSettings = appSetting;

                        if (appSetting.TestMode == true)
                        {
                            if (ExecuteLastDBSessionAction(ctx, appSetting)) continue;
                        }

                        ProcessEmails(appSetting, timeBeforeImport, ctx);

                        ExecuteDBSessionActions(ctx, appSetting);

                        await ProcessDownloadFolder(appSetting).ConfigureAwait(false);
                    }
                }
                //Console.WriteLine($"Press ENTER to Close...");
                //var keyInfo = Console.ReadKey();
                //while (keyInfo.Key != ConsoleKey.Enter)
                //    keyInfo = Console.ReadKey();

                // Application.SetSuspendState(PowerState.Suspend, true, true);
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                     EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{e.Message}\r\n{e.StackTrace}", Array.Empty<string>());



            }
        }

     
        private static async Task ProcessDownloadFolder(ApplicationSettings appSetting)
        {
            var downloadFolder = new DirectoryInfo(Path.Combine(appSetting.DataFolder, "Downloads"));

            if (!downloadFolder.Exists) downloadFolder.Create();

            foreach (var file in downloadFolder.GetFiles("*.pdf").ToList())
            {
                try
                {
                     await ProcessFile(appSetting, file).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        private static async Task ProcessFile(ApplicationSettings appSetting, FileInfo file)
        {
            var documentsFolder = CreateDocumentsFolder(appSetting, file);
            var destFileName = CopyFileToDocumentsFolder(file, documentsFolder);

            var fileTypes = GetUnknownFileTypes(file);

            fileTypes.ForEach(x => x.EmailId = file.Name);

            var allgood = await ProcessFileTypes(fileTypes, destFileName, file).ConfigureAwait(false);

            if (allgood) file.Delete();
        }

        private static DirectoryInfo CreateDocumentsFolder(ApplicationSettings appSetting, FileInfo file)
        {
            var documentsFolder = new DirectoryInfo(Path.Combine(appSetting.DataFolder, "Documents", file.Name.Replace(file.Extension, "")));
            if (!documentsFolder.Exists) documentsFolder.Create();
            return documentsFolder;
        }

        private static string CopyFileToDocumentsFolder(FileInfo file, DirectoryInfo documentsFolder)
        {
            var destFileName = Path.Combine(documentsFolder.FullName, file.Name);
            if (!File.Exists(destFileName)) file.CopyTo(destFileName);
            return destFileName;
        }

        private static List<FileTypes> GetUnknownFileTypes(FileInfo file)
        {
            return FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, file.FullName)
                .Where(x => x.Description == "Unknown")
                .ToList();
        }

        private static async Task<bool> ProcessFileTypes(List<FileTypes> fileTypes, string destFileName, FileInfo file)
        {
            var allgood = true;
            foreach (var fileType in fileTypes)
            {
                var fileInfos = new FileInfo[] { new FileInfo(destFileName) };
                var res = PDFUtils.ImportPDF(fileInfos, fileType);
                if (!res.Any(x => x.Value.DocumentType.ToString() == FileTypeManager.EntryTypes.ShipmentInvoice && x.Value.Status == ImportStatus.Success))
                {
                    var res2 = await PDFUtils.ImportPDFDeepSeek(fileInfos, fileType).ConfigureAwait(false);
                    if (!res2.Any() 
                        || res2.Any(x => x.Value.status != ImportStatus.Success))
                    {
                        NotifyUnknownPDF(file, res2);
                        allgood = false;
                        continue;
                    }
                }

                if (allgood) ShipmentUtils.CreateShipmentEmail(fileType, fileInfos);
            }

            return allgood;
        }

        private static void NotifyUnknownPDF(FileInfo file, List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>> res2)
        {
            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Unknown PDF Found",
                EmailDownloader.EmailDownloader.GetContacts("Developer"),
                $"Unknown PDF Found: {file.Name}\r\n{res2.First(x => x.Value.status != ImportStatus.Success).Value.DocumentType}",
                res2.Select(x => x.Value.FileName).Distinct().ToArray());
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

        private static void ProcessEmails(ApplicationSettings appSetting, DateTime beforeImport, CoreEntitiesContext ctx)
        {
            if (!string.IsNullOrEmpty(appSetting.Email))
            {
                //
                Utils.Client = new EmailDownloader.Client
                {
                    CompanyName = appSetting.CompanyName,
                    DataFolder = appSetting.DataFolder,
                    Password = appSetting.EmailPassword,
                    Email = appSetting.Email,
                    ApplicationSettingsId = appSetting.ApplicationSettingsId,
                    EmailMappings = appSetting.EmailMapping.ToList(),
                    DevMode = Settings.Default.DevMode,
                    NotifyUnknownMessages = appSetting.NotifyUnknownMessages??false
                };

                var msgLst = Task.Run(() => EmailDownloader.EmailDownloader.CheckEmails(Utils.Client)).Result
                    .ToList();
                // get downloads
                Console.WriteLine($"{msgLst.Count()} Emails Processed");

                var processedFileTypes = new List<Tuple<FileTypes, FileInfo[], int>>();

                foreach (var msg in msgLst.OrderBy(x => x.Key.Item2.EmailMapping.Id))
                {

                    ImportUtils.ExecuteEmailMappingActions(msg.Key.Item2.EmailMapping, new FileTypes(){EmailId = msg.Key.Item2.EmailId}, new FileInfo[]{}, appSetting);
                }

                foreach (var msg in msgLst.OrderBy(x => x.Key.Item2.FileTypes.Any(z => z.CreateDocumentSet == true)).ThenBy(x => x.Key.Item2.EmailUniqueId))
                {
                    var desFolder = Path.Combine(appSetting.DataFolder, msg.Key.Item1,
                        msg.Key.Item2.EmailUniqueId.ToString());

                    if (!msg.Key.Item2.EmailMapping.EmailFileTypes
                            .All(x => x.IsRequired != true || new DirectoryInfo(desFolder).GetFiles()
                                .Any(z => Regex.IsMatch(z.FullName,
                                    x.FileTypes.FilePattern,
                                    RegexOptions.IgnoreCase) && z.LastWriteTime >= beforeImport))) continue;



                    var emailFileTypes = msg.Key.Item2.EmailMapping.InfoFirst == true ? msg.Key.Item2.FileTypes.OrderByDescending(x => x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Info).ToList() : msg.Key.Item2.FileTypes.OrderBy(x => x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Info).ToList();
                    foreach (var emailFileType in emailFileTypes)
                    {
                        var fileType = FileTypeManager.GetFileType(emailFileType);
                        fileType.Data
                            .Clear(); // because i am using emailmapping from email, its not a lookup
                        fileType.EmailInfoMappings = msg.Key.Item2.EmailMapping.EmailInfoMappings;


                        var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                            .Where(x => msg.Value.Select(z => z.Name).Any(z => z == x.Name) &&
                                        Regex.IsMatch(x.FullName, fileType.FilePattern,
                                            RegexOptions.IgnoreCase) &&
                                        x.LastWriteTime >= beforeImport).ToArray();

                        fileType.EmailId = msg.Key.Item2.EmailId;//msg.Key.Item3;
                        fileType.FilePath = desFolder;
                        if (csvFiles.Length == 0)
                        {
                            // if (emailFileType.IsRequired == true) break;
                            continue;
                        }

                                    
                                    
                        var docSetLst = 
                            ctx.AsycudaDocumentSetExs
                                .Where(x =>
                                    x.Declarant_Reference_Number.Contains(msg.Key.Item1) &&
                                    x.ApplicationSettingsId == BaseDataModel.Instance
                                        .CurrentApplicationSettings.ApplicationSettingsId);

                        //todo emailinfomapping
                        //var reference = msg.Key.Item2.EmailMapping.IsSingleEmail == true
                        //                                            ? msg.Key.Item1 + docSetLst.Count()
                        //                                            : msg.Key.Item1;
                        var reference = msg.Key.Item1;


                        if (fileType.CreateDocumentSet)
                        {

                            var docSet = docSetLst.FirstOrDefault(x => x.Declarant_Reference_Number == reference);
                            if (docSet == null)
                            {
                                var cp = BaseDataModel.Instance.Customs_Procedures.First(x =>
                                    x.CustomsOperationId == BaseDataModel.GetDefaultCustomsOperation() && x.IsDefault == true);
                                ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{
                                        reference
                                    }',{cp.Customs_ProcedureId},0)");

                            }
                        }

                        var ndocSet =
                            ctx.AsycudaDocumentSetExs
                                .FirstOrDefault(x =>
                                    x.Declarant_Reference_Number == reference &&
                                    x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings
                                        .ApplicationSettingsId);
                        if (ndocSet != null)
                        {
                            fileType.AsycudaDocumentSetId = ndocSet.AsycudaDocumentSetId;
                            fileType.Data.Add(new KeyValuePair<string, string>("AsycudaDocumentSetId",
                                fileType.AsycudaDocumentSetId.ToString()));
                        }
                                    
                        Utils.SaveAttachments(csvFiles, fileType, msg.Key.Item2);
                        if (!ReadOnlyMode)
                        {
                            ImportUtils.ExecuteDataSpecificFileActions(fileType, csvFiles, appSetting);
                            //if (fileType.ProcessNextStep.FirstOrDefault() == "Kill") return;
                            if (msg.Key.Item2.EmailMapping.IsSingleEmail == true)
                            {
                                ImportUtils.ExecuteNonSpecificFileActions(fileType, csvFiles, appSetting);
                            }
                            else
                            {
                                processedFileTypes.Add(new Tuple<FileTypes, FileInfo[], int>(fileType,
                                    csvFiles, ndocSet?.AsycudaDocumentSetId ?? ctx.AsycudaDocumentSet.Where(x =>
                                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                                        .FirstOrDefault().AsycudaDocumentSetId));
                            }
                        }





                    }
                }

                if (ReadOnlyMode) return;
                            
                var pfg = processedFileTypes
                    .Where(x => x.Item1.FileTypeActions.Any(z =>
                        z.Actions.IsDataSpecific == null || z.Actions.IsDataSpecific != true))
                    .GroupBy(x => x.Item3).ToList();

                foreach (var docSetId in pfg)
                {
                                    
                    var pf = docSetId.DistinctBy(x => x.Item1.Id).ToList();
                    foreach (var t in pf)
                    {
                        //if(t.Item1.ProcessNextStep == "Kill") return;
                        t.Item1.AsycudaDocumentSetId = docSetId.Key;
                        ImportUtils.ExecuteNonSpecificFileActions(t.Item1, t.Item2, appSetting);
                    }
                }
                            

            }

            
        }

      
        private static void ExecuteDBSessionActions(CoreEntitiesContext ctx, ApplicationSettings appSetting)
        {
            ctx.SessionActions.OrderBy(x => x.Id)
                .Include(x => x.Actions)
                .Where(x => x.Sessions.Name == "End").ToList()
                .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                .ForEach(x =>
                    x.Invoke());


            var sLst = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                .Include("ParameterSet.ParameterSetParameters.Parameters")
                .Where(x => x.RunDateTime >= SqlFunctions.DateAdd("MINUTE", x.Sessions.WindowInMinutes * -1,
                                DateTime.Now)
                            && x.RunDateTime <= SqlFunctions.DateAdd("MINUTE", x.Sessions.WindowInMinutes,
                                DateTime.Now))
                .Where(x =>
                    (x.ApplicationSettingId == null || x.ApplicationSettingId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
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
                        .Select(x => (SessionAction: x, Action:SessionsUtils.SessionActions[x.Actions.Name]))
                        .ForEach(x =>
                        {
                            BaseDataModel.Instance.CurrentSessionAction = x.SessionAction;
                            x.Action.Invoke();
                        });
                }
                BaseDataModel.Instance.CurrentSessionAction = null;
                BaseDataModel.Instance.CurrentSessionSchedule = new List<SessionSchedule>();

            }
            else
            {
                if (appSetting.AssessIM7 == true)
                    ctx.SessionActions.OrderBy(x => x.Id)
                        .Include(x => x.Actions)
                        .Where(x => x.Sessions.Name == "AssessIM7").ToList()
                        .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                        .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                        .ForEach(x =>
                            x.Invoke());

                if (appSetting.AssessEX == true)
                    ctx.SessionActions.OrderBy(x => x.Id)
                        .Include(x => x.Actions)
                        .Where(x => x.Sessions.Name == "AssessEX").ToList()
                        .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                        .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                        .ForEach(x =>
                            x.Invoke());
            }
        }
    }

}
