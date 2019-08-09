


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


                    foreach (var appSetting in ctx.ApplicationSettings.AsNoTracking()
                        .Include(x => x.FileTypes)
                        .Include("FileTypes.FileTypeContacts.Contacts")
                        .Include("FileTypes.FileTypeActions.Actions")
                        .Include("FileTypes.AsycudaDocumentSetEx")
                        .Include(x => x.EmailMapping)

                        .Include("FileTypes.FileTypeMappings").ToList())
                    {
                        appSetting.DataFolder = StringExtensions.UpdateToCurrentUser(appSetting.DataFolder);
                        // set BaseDataModel CurrentAppSettings
                        BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                        //check emails

                        var salesInfo = Utils.CurrentSalesInfo();
                        var dref = salesInfo.Item1.ToString("MMMM yyyy");
                        if (salesInfo.Item3 == null)
                        {

                            var doctype = ctx.Document_Type.Include(x => x.Customs_Procedure).First(x =>
                                x.Type_of_declaration == "IM" && x.Declaration_gen_procedure_code == "7");
                            ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({appSetting.ApplicationSettingsId},'{dref}',{doctype.Document_TypeId},{
                                    doctype.Customs_Procedure.First(z => z.IsDefault == true).Customs_ProcedureId
                                },0)");
                            Directory.CreateDirectory(Path.Combine(appSetting.DataFolder, dref));
                        }

                        if (!Directory.Exists(Path.Combine(appSetting.DataFolder, dref)))
                            Directory.CreateDirectory(Path.Combine(appSetting.DataFolder, dref));
                        //
                        Utils.Client = new EmailDownloader.Client
                        {
                            CompanyName = appSetting.CompanyName,
                            DataFolder = appSetting.DataFolder,
                            Password = appSetting.EmailPassword,
                            Email = appSetting.Email,
                            EmailMappings = appSetting.EmailMapping.Select(x => x.Pattern).ToList()
                        };
                        
                        var msgLst = Task.Run(() => EmailDownloader.EmailDownloader.CheckEmails(Utils.Client, appSetting.FileTypes.Select(x => x.FilePattern).ToList())).Result;
                        // get downloads
                        Console.WriteLine($"{msgLst.Count} Emails Processed");
                        foreach (var msg in msgLst)
                        {
                            var desFolder = Path.Combine(appSetting.DataFolder, msg.Key.Item1);
                            foreach (var fileType in appSetting.FileTypes)
                            {
                                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                                    .Where(x => msg.Value.Contains(x.Name) && Regex.IsMatch(x.FullName, fileType.FilePattern, RegexOptions.IgnoreCase) &&
                                                x.LastWriteTime >= beforeImport).ToArray();

                                if (csvFiles.Length == 0)
                                {

                                    continue;
                                }

                                var oldDocSet =
                                    ctx.AsycudaDocumentSetExs
                                        .FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                                if (fileType.CreateDocumentSet)
                                {
                                    var docSet =
                                        ctx.AsycudaDocumentSetExs
                                            .FirstOrDefault(x => x.Declarant_Reference_Number == msg.Key.Item1);
                                    if (docSet == null)
                                    {
                                        ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({oldDocSet.ApplicationSettingsId},'{msg.Key.Item1}',{oldDocSet.Document_TypeId},{
                                                oldDocSet.Customs_ProcedureId
                                            },0)");

                                    }
                                }

                                var ndocSet =
                                    ctx.AsycudaDocumentSetExs
                                        .First(x => x.Declarant_Reference_Number == msg.Key.Item1);

                                if (fileType.Type == "Info")
                                    Utils.SaveInfo(csvFiles, ndocSet.AsycudaDocumentSetId);
                                else
                                {
                                    fileType.AsycudaDocumentSetId = ndocSet.AsycudaDocumentSetId;
                                    Utils.SaveAttachments(csvFiles, fileType, msg.Key.Item2);
                                }
                            }
                        }

                        var docLst = new List<AsycudaDocumentSetEx>()
                        {
                            Utils.CurrentSalesInfo().Item3,
                        };
                        foreach (var i in Utils.CurrentPOInfo().Select(x => x.Item1))
                        {
                            if (docLst.FirstOrDefault(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                                           && x.AsycudaDocumentSetId == i.AsycudaDocumentSetId) == null)
                                docLst.AddRange(Utils.CurrentPOInfo().Select(x => x.Item1));
                        }

                        var fdate = DateTime.Now.Date;
                        docLst.AddRange(ctx.AsycudaDocumentSetExs.Where(x => x.ApplicationSettingsId == appSetting.ApplicationSettingsId && x.AsycudaDocumentSet_Attachments.Any(z => z.FileDate == fdate)));



                        foreach (var dSet in docLst.DistinctBy(x => x.AsycudaDocumentSetId))
                        {
                            foreach (var fileType in appSetting.FileTypes
                            ) //.Where(x => x.Type != "Sales" && x.Type != "PO")
                            {
                                var desFolder = Path.Combine(appSetting.DataFolder, dSet.Declarant_Reference_Number);
                                fileType.AsycudaDocumentSetId = fileType.CreateDocumentSet? dSet.AsycudaDocumentSetId : fileType.AsycudaDocumentSetId;
                                fileType.DocReference = fileType.CreateDocumentSet ? dSet.Declarant_Reference_Number: fileType.AsycudaDocumentSetEx.Declarant_Reference_Number;
                                if (!Directory.Exists(desFolder)) continue;
                                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                                    .Where(x =>
                                        Regex.IsMatch(x.FullName, fileType.FilePattern, RegexOptions.IgnoreCase) &&
                                        x.LastWriteTime >= beforeImport ).ToArray();// set to 10 mins so it works within the email import time
                                if (!csvFiles.Any()) continue;

                                fileType.FileTypeActions.OrderBy(x => x.Priority)
                                    .Select(x => Utils.FileActions[x.Actions.Name]).ToList()
                                    .ForEach(x =>
                                    {
                                        x.Invoke(fileType, csvFiles);

                                    });

                            }
                        }

                        ctx.SessionActions.OrderBy(x => x.Id)
                           .Include(x => x.Actions)
                           .Where(x => x.Sessions.Name == "End").ToList()
                           .Select(x => Utils.SessionActions[x.Actions.Name])
                           .ForEach(x =>
                               x.Invoke());


                       var sLst = ctx.SessionSchedule.Include("Sessions.SessionActions.Actions")
                                           .Where(x => x.RunDateTime >= SqlFunctions.DateAdd("MINUTE", x.Sessions.WindowInMinutes * -1, DateTime.Now)
                                                        && x.RunDateTime <= SqlFunctions.DateAdd("MINUTE", x.Sessions.WindowInMinutes, DateTime.Now)).ToList();
                        foreach (var item in sLst.Where(x => x.ApplicationSettingId == null || x.ApplicationSettingId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        {
                            item.Sessions.SessionActions
                                .Select(x => Utils.SessionActions[x.Actions.Name])
                                .ForEach(x => x.Invoke());
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
