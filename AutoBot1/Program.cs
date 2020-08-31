


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
using Core.Common.Utils;
using MoreLinq.Extensions;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;


namespace AutoBot
{

    partial class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var beforeImport = DateTime.Now;
                Console.WriteLine($"{beforeImport}");
                using (var ctx = new CoreEntitiesContext() { })
                {
                    //ctx.Configuration.LazyLoadingEnabled = true;
                    //ctx.Configuration.ProxyCreationEnabled = true;
                    var applicationSettings = ctx.ApplicationSettings.AsNoTracking()
                        .Include(x => x.FileTypes)
                        .Include("FileTypes.FileTypeContacts.Contacts")
                        .Include("FileTypes.FileTypeActions.Actions")
                        .Include("FileTypes.AsycudaDocumentSetEx")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileTypeActions.Actions")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileTypeContacts.Contacts")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.AsycudaDocumentSetEx")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileTypeMappings")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.ChildFileTypes")
                        .Include("EmailMapping.EmailFileTypes.FileTypes.ChildFileTypes")
                        .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                        .Include("FileTypes.ChildFileTypes")
                        .Include("FileTypes.FileTypeMappings")
                        .ToList();

                    foreach (var appSetting in applicationSettings)
                    {
                        Console.WriteLine($"{appSetting.SoftwareName} Emails Processed");
                        if(appSetting.DataFolder != null) appSetting.DataFolder = StringExtensions.UpdateToCurrentUser(appSetting.DataFolder);
                        if(appSetting.TestMode == null) continue;
                        // set BaseDataModel CurrentAppSettings
                        BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                        //check emails

                        if (appSetting.TestMode == true)
                        {
                            var lastAction = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                                .OrderByDescending(p => p.Id)
                                .FirstOrDefault(x => x.ApplicationSettingId == appSetting.ApplicationSettingsId);

                            if (lastAction != null)
                            {
                                lastAction.Sessions.SessionActions
                                    .Where(x => lastAction.ActionId == null || x.ActionId == lastAction.ActionId)
                                    .Select(x => Utils.SessionActions[x.Actions.Name])
                                    .ForEach(x => x.Invoke());
                               
                                continue;
                            }

                        }

                        if (!string.IsNullOrEmpty(appSetting.Email))
                        {
                            //
                            Utils.Client = new EmailDownloader.Client
                            {
                                CompanyName = appSetting.CompanyName,
                                DataFolder = appSetting.DataFolder,
                                Password = appSetting.EmailPassword,
                                Email = appSetting.Email,
                                EmailMappings = appSetting.EmailMapping.ToList()
                            };

                            var msgLst = Task.Run(() => EmailDownloader.EmailDownloader.CheckEmails(Utils.Client,
                                    appSetting.FileTypes.Select(x => x.FilePattern).ToList())).Result.OrderBy(x =>
                                    x.Key.Item2.EmailMapping.EmailFileTypes.Sum(z => z.FileTypes.FileTypeActions.Count))
                                .ToList();
                            // get downloads
                            Console.WriteLine($"{msgLst.Count()} Emails Processed");

                            var processedFileTypes = new List<Tuple<FileTypes, FileInfo[]>>();

                            foreach (var msg in msgLst)
                            {
                                var desFolder = Path.Combine(appSetting.DataFolder, msg.Key.Item1,
                                    msg.Key.Item2.EmailId.ToString());

                                if (!msg.Key.Item2.EmailMapping.EmailFileTypes
                                    .All(x => x.IsRequired != true || new DirectoryInfo(desFolder).GetFiles()
                                                  .Any(z => msg.Value.Contains(z.Name)
                                                            && Regex.IsMatch(z.FullName, x.FileTypes.FilePattern,
                                                                RegexOptions.IgnoreCase)
                                                            && z.LastWriteTime >= beforeImport))) continue;




                                foreach (var emailFileType in msg.Key.Item2.EmailMapping.EmailFileTypes.OrderBy(x =>
                                    x.FileTypes.Type == "Info"))
                                {




                                    var fileType = emailFileType.FileTypes;
                                    fileType.Data
                                        .Clear(); // because i am using emailmapping from email, its not a lookup
                                    fileType.EmailInfoMappings = msg.Key.Item2.EmailMapping.EmailInfoMappings;


                                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                                        .Where(x => msg.Value.Contains(x.Name) &&
                                                    Regex.IsMatch(x.FullName, fileType.FilePattern,
                                                        RegexOptions.IgnoreCase) &&
                                                    x.LastWriteTime >= beforeImport).ToArray();

                                    fileType.EmailId = msg.Key.Item3;

                                    if (csvFiles.Length == 0)
                                    {
                                        if (emailFileType.IsRequired == true) break;
                                        continue;
                                    }

                                    //todo emailinfomapping


                                    if (fileType.CreateDocumentSet)
                                    {
                                        var docSet =
                                            ctx.AsycudaDocumentSetExs
                                                .FirstOrDefault(x =>
                                                    x.Declarant_Reference_Number == msg.Key.Item1 &&
                                                    x.ApplicationSettingsId == BaseDataModel.Instance
                                                        .CurrentApplicationSettings.ApplicationSettingsId);
                                        if (docSet == null)
                                        {
                                            var cp = BaseDataModel.Instance.Customs_Procedures.First(x =>
                                                x.CustomsOperationId == (int) CustomsOperations.Warehouse);
                                            ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{
                                                    msg.Key.Item1
                                                }',{cp.Document_TypeId},{cp.Customs_ProcedureId},0)");

                                        }
                                    }

                                    var ndocSet =
                                        ctx.AsycudaDocumentSetExs
                                            .FirstOrDefault(x =>
                                                x.Declarant_Reference_Number == msg.Key.Item1 &&
                                                x.ApplicationSettingsId ==
                                                BaseDataModel.Instance.CurrentApplicationSettings
                                                    .ApplicationSettingsId);
                                    if (ndocSet != null)
                                    {
                                        fileType.AsycudaDocumentSetId = ndocSet.AsycudaDocumentSetId;
                                        fileType.Data.Add(new KeyValuePair<string, string>("AsycudaDocumentSetId",
                                            fileType.AsycudaDocumentSetId.ToString()));


                                        Utils.SaveAttachments(csvFiles, fileType, msg.Key.Item2);



                                    }

                                    ExecuteDataSpecificFileActions(fileType, csvFiles, appSetting);

                                    processedFileTypes.Add(new Tuple<FileTypes, FileInfo[]>(fileType, csvFiles));





                                }
                            }

                            var pfg = processedFileTypes
                                .Where(x => x.Item1.FileTypeActions.Any(z =>
                                    z.Actions.IsDataSpecific == null || z.Actions.IsDataSpecific != true))
                                .GroupBy(x => x.Item1.AsycudaDocumentSetId).ToList();

                            foreach (var docSetId in pfg)
                            {
                                var pf = docSetId.DistinctBy(x => x.Item1.Id).ToList();
                                foreach (var t in pf)
                                {
                                    ExecuteNonSpecificFileActions(t.Item1, t.Item2, appSetting);
                                }
                            }


                        }

                        ctx.SessionActions.OrderBy(x => x.Id)
                            .Include(x => x.Actions)
                            .Where(x => x.Sessions.Name == "End").ToList()
                            .Select(x => Utils.SessionActions[x.Actions.Name])
                            .ForEach(x =>
                                x.Invoke());


                        var sLst = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                            .Where(x => x.RunDateTime >= SqlFunctions.DateAdd("MINUTE", x.Sessions.WindowInMinutes * -1,
                                            DateTime.Now)
                                        && x.RunDateTime <= SqlFunctions.DateAdd("MINUTE", x.Sessions.WindowInMinutes,
                                            DateTime.Now))
                            .Where(x =>
                                (x.ApplicationSettingId == null || x.ApplicationSettingId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                            .ToList();


                        if (sLst.Any())
                        {
                            foreach (var item in sLst)
                            {
                                
                                item.Sessions.SessionActions
                                    .Where(x => item.ActionId == null || x.ActionId == item.ActionId)
                                    .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode ?? false))
                                    .Select(x => Utils.SessionActions[x.Actions.Name])
                                    .ForEach(x =>  x.Invoke());
                            }

                        }
                        else
                        {
                            if (appSetting.AssessIM7 == true)
                                ctx.SessionActions.OrderBy(x => x.Id)
                                    .Include(x => x.Actions)
                                    .Where(x => x.Sessions.Name == "AssessIM7").ToList()
                                    .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode ?? false))
                                    .Select(x => Utils.SessionActions[x.Actions.Name])
                                    .ForEach(x =>
                                         x.Invoke());

                            if (appSetting.AssessEX == true)
                                ctx.SessionActions.OrderBy(x => x.Id)
                                    .Include(x => x.Actions)
                                    .Where(x => x.Sessions.Name == "AssessEX").ToList()
                                    .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode ?? false))
                                    .Select(x => Utils.SessionActions[x.Actions.Name])
                                    .ForEach(x =>
                                        x.Invoke());
                        }


                    }
                }


                // Application.SetSuspendState(PowerState.Suspend, true, true);
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                    new[] { "Josephbartholomew@outlook.com" }, $"{e.Message}\r\n{e.StackTrace}", Array.Empty<string>());



            }
        }
        private static void ExecuteDataSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                fileType.FileTypeActions.Where(x => x.Actions.IsDataSpecific == true).OrderBy(x => x.Id)
                    .Where(x => (x.AssessIM7.Equals(null) && x.AssessEX.Equals(null)) ||
                                (appSetting.AssessIM7 == x.AssessIM7 ||
                                 appSetting.AssessEX == x.AssessEX))
                    .Where(x => x.Actions.TestMode ==
                                (BaseDataModel.Instance.CurrentApplicationSettings.TestMode ??
                                 false))
                    .Select(x => Utils.FileActions[x.Actions.Name]).ToList()
                    .ForEach(x => { x.Invoke(fileType, files); });
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                    new[] { "Josephbartholomew@outlook.com" }, $"{e.Message}\r\n{e.StackTrace}",
                    Array.Empty<string>());
            }
        }
        private static void ExecuteNonSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                fileType.FileTypeActions.Where(x => x.Actions.IsDataSpecific == null || x.Actions.IsDataSpecific != true).OrderBy(x => x.Id)
                    .Where(x => (x.AssessIM7.Equals(null) && x.AssessEX.Equals(null)) ||
                                (appSetting.AssessIM7 == x.AssessIM7 ||
                                 appSetting.AssessEX == x.AssessEX))
                    .Where(x => x.Actions.TestMode ==
                                (BaseDataModel.Instance.CurrentApplicationSettings.TestMode ??
                                 false))
                    .Select(x => Utils.FileActions[x.Actions.Name]).ToList()
                    .ForEach(x => { x.Invoke(fileType, files); });
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                    new[] {"Josephbartholomew@outlook.com"}, $"{e.Message}\r\n{e.StackTrace}",
                    Array.Empty<string>());
            }
        }
    }

}
