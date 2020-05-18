


using CoreEntities.Business.Entities;
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
                        .Include("FileTypes.ChildFileTypes")
                        .Include("FileTypes.FileTypeMappings").ToList();

                    foreach (var appSetting in applicationSettings)
                    {
                        appSetting.DataFolder = StringExtensions.UpdateToCurrentUser(appSetting.DataFolder);
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
                            appSetting.FileTypes.Select(x => x.FilePattern).ToList())).Result;
                        // get downloads
                        Console.WriteLine($"{msgLst.Count} Emails Processed");
                        foreach (var msg in msgLst)
                        {
                            var desFolder = Path.Combine(appSetting.DataFolder, msg.Key.Item1, msg.Key.Item2.EmailId.ToString());
                            foreach (var fileType in msg.Key.Item2.EmailMapping.EmailFileTypes.Select(x => x.FileTypes).ToList())
                            {
                                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                                    .Where(x => msg.Value.Contains(x.Name) &&
                                                Regex.IsMatch(x.FullName, fileType.FilePattern,
                                                    RegexOptions.IgnoreCase) &&
                                                x.LastWriteTime >= beforeImport).ToArray();

                                if (csvFiles.Length == 0)
                                {

                                    continue;
                                }

                                if (fileType.Type == "Info" && !Utils.CanSaveFileInfo(csvFiles).Any()) continue;

                                var oldDocSet =
                                    ctx.AsycudaDocumentSetExs
                                        .FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
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
                                        ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({oldDocSet.ApplicationSettingsId},'{msg.Key.Item1}',{
                                                oldDocSet.Document_TypeId
                                            },{oldDocSet.Customs_ProcedureId},0)");

                                    }
                                }

                                var ndocSet =
                                    ctx.AsycudaDocumentSetExs
                                        .FirstOrDefault(x =>
                                            x.Declarant_Reference_Number == msg.Key.Item1 && x.ApplicationSettingsId ==
                                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                                if (ndocSet != null)
                                {
                                    if (fileType.Type == "Info")
                                        Utils.SaveInfo(csvFiles, ndocSet.AsycudaDocumentSetId);
                                    else
                                    {

                                        fileType.AsycudaDocumentSetId = ndocSet.AsycudaDocumentSetId;
                                        Utils.SaveAttachments(csvFiles, fileType, msg.Key.Item2);
                                    }
                                }

                                //else
                                //{
                                try
                                {
                                    fileType.FileTypeActions.OrderBy(x => x.Id)
                                        .Where(x => (x.AssessIM7.Equals(null) && x.AssessEX.Equals(null)) ||
                                                    (appSetting.AssessIM7 == x.AssessIM7 || appSetting.AssessEX == x.AssessEX))
                                        .Where(x => x.Actions.TestMode == (BaseDataModel.Instance.CurrentApplicationSettings.TestMode ?? false))
                                        .Select(x => Utils.FileActions[x.Actions.Name]).ToList()
                                        .ForEach(x => { x.Invoke(fileType, csvFiles); });
                                }
                                catch (Exception e)
                                {
                                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                        new[] {"Josephbartholomew@outlook.com"}, $"{e.Message}\r\n{e.StackTrace}",
                                        Array.Empty<string>());
                                }

                                //}

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
    }

}
