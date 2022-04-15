


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
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using xlsxWriter;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{

    partial class Program
    {
        public static bool ReadOnlyMode { get; set; } = false;

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
                        .Include("EmailMapping.EmailFileTypes.FileTypes")
                        .Include("EmailMapping.EmailMappingRexExs")
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
                                    .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
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
                                ApplicationSettingsId = appSetting.ApplicationSettingsId,
                                EmailMappings = appSetting.EmailMapping.ToList(),
                                DevMode = Settings.Default.DevMode
                            };

                            var msgLst = Task.Run(() => EmailDownloader.EmailDownloader.CheckEmails(Utils.Client)).Result
                                .ToList();
                            // get downloads
                            Console.WriteLine($"{msgLst.Count()} Emails Processed");

                            var processedFileTypes = new List<Tuple<FileTypes, FileInfo[], int>>();

                            foreach (var msg in msgLst.OrderBy(x => x.Key.Item2.FileTypes.Any(z => z.CreateDocumentSet == true)).ThenBy(x => x.Key.Item2.EmailUniqueId))
                            {
                                var desFolder = Path.Combine(appSetting.DataFolder, msg.Key.Item1,
                                    msg.Key.Item2.EmailUniqueId.ToString());

                                if (!msg.Key.Item2.EmailMapping.EmailFileTypes
                                    .All(x => x.IsRequired != true || new DirectoryInfo(desFolder).GetFiles()
                                                  .Any(z => Regex.IsMatch(z.FullName,
                                                      x.FileTypes.FilePattern,
                                                      RegexOptions.IgnoreCase) && z.LastWriteTime >= beforeImport))) continue;


                                var emailFileTypes = msg.Key.Item2.EmailMapping.InfoFirst == true ? msg.Key.Item2.FileTypes.OrderByDescending(x => x.Type == "Info").ToList() : msg.Key.Item2.FileTypes.OrderBy(x => x.Type == "Info").ToList();
                                foreach (var emailFileType in emailFileTypes)
                                {
                                    var fileType = BaseDataModel.GetFileType(emailFileType);
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
                                                x.CustomsOperationId == (int) CustomsOperations.Warehouse);
                                            ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{
                                                    reference
                                                }',{cp.Document_TypeId},{cp.Customs_ProcedureId},0)");

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
                                                csvFiles, ndocSet?.AsycudaDocumentSetId ?? ctx.AsycudaDocumentSetExs.Where(x =>
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

                        ctx.SessionActions.OrderBy(x => x.Id)
                            .Include(x => x.Actions)
                            .Where(x => x.Sessions.Name == "End").ToList()
                            .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
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
                            .OrderBy(x => x.Id)
                            .ToList();


                        if (sLst.Any())
                        {
                            foreach (var item in sLst)
                            {
                                
                                item.Sessions.SessionActions
                                    .Where(x => item.ActionId == null || x.ActionId == item.ActionId)
                                    .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                                    .Select(x => SessionsUtils.SessionActions[x.Actions.Name])
                                    .ForEach(x =>  x.Invoke());
                            }

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
                //Console.WriteLine($"Press ENTER to Close...");
                //var keyInfo = Console.ReadKey();
                //while (keyInfo.Key != ConsoleKey.Enter)
                //    keyInfo = Console.ReadKey();

                // Application.SetSuspendState(PowerState.Suspend, true, true);
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                    new[] { "Joseph@auto-brokerage.com" }, $"{e.Message}\r\n{e.StackTrace}", Array.Empty<string>());



            }
        }



       
    }

}
