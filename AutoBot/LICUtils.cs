﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Asycuda421;
using Core.Common.Converters;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using LicenseDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.DataSpace.Asycuda;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class LICUtils
    {
        public static async Task ImportLicense(FileTypes ft)
        {
            Console.WriteLine("Import License");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docSets = ctx.TODO_LICToCreate
                    //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                    .ToList();
                
                foreach (var poInfo in docSets)
                {
                    await ImportLicense(poInfo.Declarant_Reference_Number, poInfo.AsycudaDocumentSetId).ConfigureAwait(false);
                }
            }

        }

        public static async Task<bool> ImportLicense(string declarant_Reference_Number,int asycudaDocumentSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;

                var reference = declarant_Reference_Number;
                var directory = BaseDataModel.GetDocSetDirectoryName(reference);
                if (!Directory.Exists(directory)) return false;

                var lastdbfile =
                    ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId && x.FileTypes.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic);
                var lastfiledate = lastdbfile  != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1);

                var ft = ctx.FileTypes
                    .Include(x => x.FileImporterInfos)
                    .FirstOrDefault(x =>
                    x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (ft == null) return true;
               
                var desFolder = Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), "LIC");
                if (!Directory.Exists(desFolder)) Directory.CreateDirectory(desFolder);
                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                    .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                    .Where(x => x.LastWriteTime >= lastfiledate)
                    .ToList();
                if (!csvFiles.Any()) return false;

                foreach (var file in csvFiles.ToList())
                {
                    var a = Licence.LoadFromFile(file.FullName);

                    if (a.General_segment.Exporter_address.Text.Any(x => x.Contains(file.Name.Replace("-LIC.xml", ""))))
                        csvFiles.Remove(file);//the po should be different from the reference number
                    if(!ctx.AsycudaDocumentSetEntryDataEx.Any( x => x.AsycudaDocumentSetId == asycudaDocumentSetId
                                                                    && a.General_segment.Exporter_address.Text.Any(z => z.Contains(x.EntryDataId))) 
                       && !new EntryDataDSContext().EntryData.OfType<PurchaseOrders>().Any(x => x.EntryDataEx.AsycudaDocumentSetId == asycudaDocumentSetId
                           && a.General_segment.Exporter_address.Text.Any(z => z.Contains(x.SupplierInvoiceNo))))

                        csvFiles.Remove(file);
                }

                await BaseDataModel.Instance.ImportLicense(asycudaDocumentSetId,
                    csvFiles.Select(x => x.FullName).ToList()).ConfigureAwait(false);
                ft.AsycudaDocumentSetId = asycudaDocumentSetId;
                await BaseDataModel.Instance.SaveAttachedDocuments(csvFiles.ToArray(), ft).ConfigureAwait(false);
                return false;
            }
        }

        public static async Task<bool> ImportAllLicense()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;

                var docSet = ctx.AsycudaDocumentSetExs.First(x =>
                                    x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                    x.Declarant_Reference_Number == "Imports");
                
                var ft = ctx.FileTypes
                    .Include(x => x.FileImporterInfos)
                    .FirstOrDefault(x =>
                    x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (ft == null) return true;
              
                var desFolder = Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), "LIC");
                if (!Directory.Exists(desFolder)) Directory.CreateDirectory(desFolder);


                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                    .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                    .ToList();
                if (!csvFiles.Any()) return false;

                var ifiles = new LicenseDSContext().xLIC_License.OfType<Registered>()
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Select(x =>  x.RegistrationNumber).ToList();
                
                foreach (var file in csvFiles.ToList())
                {
                    var match = Regex.Match(file.FullName, ft.FilePattern, RegexOptions.IgnoreCase).Groups["RegNumber"].Value;
                    if (!ifiles.Contains(match)) continue;
                    csvFiles.Remove(file);
                }

                await BaseDataModel.Instance.ImportLicense(docSet.AsycudaDocumentSetId,
                    csvFiles.Select(x => x.FullName).ToList()).ConfigureAwait(false);
                
                await BaseDataModel.Instance.SaveAttachedDocuments(csvFiles.ToArray(), ft).ConfigureAwait(false);
                return false;
            }
        }


        public static void DownLoadLicence(bool redownload, FileTypes ft)
        {
            try


            {
                using (var ctx = new CoreEntitiesContext())
                {

                    ctx.Database.CommandTimeout = 10;
                    var pOs = ctx.TODO_LICToCreate
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();

                    if (!pOs.Any()) return;
                    var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports", "LIC")); ;
                    Console.WriteLine("Download License Files");
                    var lcont = 0;
                    if (redownload)
                    {
                        Utils.RunSiKuLi(directoryName, "LIC", lcont.ToString());
                    }
                    else
                    {
                        while (ImportLICComplete(directoryName, out lcont) == false)
                        {
                            Utils.RunSiKuLi(directoryName, "LIC", lcont.ToString());
                        }
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool ImportLICComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var overviewFile = Path.Combine(directoryName + "\\", "LICOverView-PDF.txt");
            if (File.Exists(overviewFile))
            {
                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-1)) return false;
                var lines = File.ReadAllText(Path.Combine(directoryName, "LICOverView-PDF.txt"))
                    .Split(new[] { $"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                if (lines.FirstOrDefault() == "No Data" && File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-0.25)) return false;
                if (lines.FirstOrDefault() == "No Data") return true;
                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.Split('\t');
                    if (p.Length < 5) continue;
                    if (string.IsNullOrEmpty(p[3])
                        && (DateTime.Now - File.GetLastWriteTime(Path.Combine(directoryName, "LICOverView-PDF.txt"))).TotalMinutes > 60) return false;
                    if (File.Exists(Path.Combine(directoryName, $"{p[3]}-LIC.xml")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                if (lines.Length == lcont && existingfiles == 0)
                    return true;
                else
                    return existingfiles != 0;
            }
            else
            {

                return false;
            }
        }

        public static async Task CreateLicence(FileTypes ft)
        {


            try

            {
                Console.WriteLine("Create License Files");


                using (var ctx = new LicenseDSContext())
                {

                    ctx.Database.CommandTimeout = 10;
                    var pOs = ctx.TODO_LICToCreate
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 || 
                        .ToList();
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            pO.Declarant_Reference_Number);
                        if (!Directory.Exists(directoryName)) continue;


                        var instructions = Path.Combine(directoryName, "LIC-Instructions.txt");
                            if (File.Exists(instructions)) File.Delete(instructions);


                        // --- Fetch Consignee Info --- Moved inside the outer loop
                        string consigneeCode = null;
                        string consigneeName = null;
                            string consigneeAddress = null;
                            int? docSetAppId = pO.ApplicationSettingsId; 
                            int? asycudaDocumentSetId = pO.AsycudaDocumentSetId; 

                            if (asycudaDocumentSetId.HasValue && docSetAppId.HasValue)
                            {
                                using (var coreCtx = new CoreEntitiesContext())
                                {
                                    var docSetData = coreCtx.AsycudaDocumentSet
                                                        .Where(ds => ds.AsycudaDocumentSetId == asycudaDocumentSetId.Value)
                                                        .Select(ds => new { ds.ConsigneeName, ds.Customs_ProcedureId })
                                                        .FirstOrDefault();

                                    if (docSetData != null && !string.IsNullOrEmpty(docSetData.ConsigneeName))
                                    {
                                        consigneeName = docSetData.ConsigneeName;
                                        int? docSetCustomsProcId = docSetData.Customs_ProcedureId;

                                        // Fetch primary address
                                        var consignee = coreCtx.Consignees
                                                               .FirstOrDefault(c => c.ConsigneeName == consigneeName && c.ApplicationSettingsId == docSetAppId.Value);
                                        
                                        consigneeCode = consignee?.ConsigneeCode;
                                        consigneeAddress = consignee?.Address;

                                        // Fallback Logic
                                        if (string.IsNullOrWhiteSpace(consigneeAddress) && docSetCustomsProcId.HasValue)
                                        {
                                            var customsProcedureStr = coreCtx.Customs_Procedure
                                                                           .Where(cp => cp.Customs_ProcedureId == docSetCustomsProcId.Value)
                                                                           .Select(cp => cp.CustomsProcedure)
                                                                           .FirstOrDefault();

                                            if (!string.IsNullOrEmpty(customsProcedureStr))
                                            {
                                                var exportTemplate = BaseDataModel.Instance.ExportTemplates
                                                                            .FirstOrDefault(et => et.ApplicationSettingsId == docSetAppId.Value
                                                                                                 && et.Customs_Procedure == customsProcedureStr);

                                                consigneeAddress = exportTemplate?.Consignee_Address;
                                            }
                                        }
                                    }
                                }
                            }
                            // --- End Fetch Consignee Info ---


                            var llst = new CoreEntitiesContext().Database
                            .SqlQuery<TODO_LicenseToXML>(
                                $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {pO.AsycudaDocumentSetId} and LicenseDescription is not null").ToList();

                        var lst = llst
                            .Where(x => !string.IsNullOrEmpty(x.LicenseDescription))
                            .GroupBy(x => x.EntryDataId)
                            .SelectMany(x => x.OrderByDescending(z => z.SourceFile))//.FirstOrDefault()
                            .GroupBy(x => new { x.EntryDataId, x.TariffCategoryCode, x.SourceFile })
                            .ToList();




                        foreach (var itm in lst)
                        {
                            var fileName = Path.Combine(directoryName, $"{itm.Key.EntryDataId}-{itm.Key.TariffCategoryCode}-LIC.xml");


                            if (File.Exists(fileName))
                            {

                                var instrFile = Path.Combine(
                                    BaseDataModel.GetDocSetDirectoryName(pO.Declarant_Reference_Number), "LIC-Instructions.txt");
                                if (File.Exists(instrFile))
                                {
                                    var resultsFile = Path.Combine(
                                        BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                        pO.Declarant_Reference_Number, "LIC-InstructionResults.txt");
                                    var lcont = 0;
                                    if (AssessLICComplete(instrFile, resultsFile, out lcont) == true)
                                        continue;
                                }

                            }

                            var contact = new CoreEntitiesContext().Contacts.FirstOrDefault(x =>
                                x.Role == "Broker" && x.ApplicationSettingsId == BaseDataModel.Instance
                                    .CurrentApplicationSettings.ApplicationSettingsId);
                            Suppliers supplier;

                            using (var ectx = new EntryDataDSContext())
                            {
                                //var elst = lst.Select(s => s.EntryDataId).ToList();
                                supplier = ectx.Suppliers.FirstOrDefault(x =>
                                    ectx.EntryData.Where(z => z.EntryDataId == itm.Key.EntryDataId).Select(z => z.SupplierCode)
                                        .Any(z => z == x.SupplierCode) &&
                                    x.ApplicationSettingsId == pO.ApplicationSettingsId);
                            }
                            // Pass consigneeName and consigneeAddress to CreateLicense
                            // IMPORTANT: LicenseToDataBase.CreateLicense signature must be manually updated elsewhere
                            var lic = LicenseToDataBase.Instance.CreateLicense(new List<TODO_LicenseToXML>(itm), contact, supplier,
                                itm.Key.EntryDataId, consigneeCode ,consigneeName, consigneeAddress ?? ""); // Pass fetched values
                            var invoices = itm.Select(x => new Tuple<string, string>(x.EntryDataId, Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.EntryDataId}.pdf"))).Where(x => File.Exists(x.Item2)).Distinct().ToList();
                            if (!invoices.Any()) continue;
                            ctx.xLIC_License.Add(lic);
                            ctx.SaveChanges();
                            await LicenseToDataBase.Instance.ExportLicense(pO.AsycudaDocumentSetId, lic, fileName,
                                invoices).ConfigureAwait(false);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool AssessLICComplete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;
            try
            {
                if (File.Exists(instrFile))
                {
                    if (!File.Exists(resultsFile)) return false;
                    var instructions = File.ReadAllLines(instrFile);
                    var res = File.ReadAllLines(resultsFile);
                    if (res.Length == 0)
                    {

                        return false;
                    }


                    foreach (var inline in instructions)
                    {
                        var p = inline.Split('\t');
                        // --- disable because it when it finished it cause it to repeat... better to check thru all to decide if to repeat or not
                        if (lcont >= res.Length)
                        {
                            if ((res.Length / 2) == (instructions.Length / 3) || ((res.Length - 1) / 2) == (instructions.Length / 3) || (res.Length == 2 && instructions.Length == 3) || (res.Length == 3 && instructions.Length == 6)) return true; else return false;
                        }
                        if (string.IsNullOrEmpty(res[lcont])) return false;
                        var isSuccess = false;
                        foreach (var rline in res)
                        {
                            var r = rline.Split('\t');

                            if (r.Length == 5 && p[1] == r[1] && r[4] == "Success") //for attachment
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                        }

                        if (isSuccess == true) continue;
                        return false;
                    }

                    return true;
                }
                else
                {

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public static async Task SubmitBlankLicenses(FileTypes ft)
        {
            try
            {
                var info = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
                var directory = info.Item4;




                using (var ctx = new CoreEntitiesContext())
                {

                    var llst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_LicenseToXML>(
                            $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {ft.AsycudaDocumentSetId}").ToList();

                    var emails = llst
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )//
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId && x.EmailId != null)//ft.AsycudaDocumentSetId == 0 || 
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {

                        //if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;


                        var errorfile = Path.Combine(directory, $"BlankLicenseDescription-{email.Key.AsycudaDocumentSetId}.csv");
                        var errors = email.Select(x => new Utils.BlankLicenseDescription()
                        {
                            InvoiceNo = x.EntryDataId,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode,
                            LicenseDescription = x.LicenseDescription
                        }).ToList();


                        var res =
                            new ExportToCSV<Utils.BlankLicenseDescription, List<Utils.BlankLicenseDescription>>()
                            {
                                dataToPrint = errors
                            };
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            await Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta).ConfigureAwait(false);
                        }

                        var contacts = MoreEnumerable.DistinctBy(ctx.Contacts.Where(x => x.Role == "Broker")
                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId), x => x.EmailAddress).ToList();
                        if (File.Exists(errorfile))
                           await EmailDownloader.EmailDownloader.ForwardMsgAsync(email.Key.EmailId, Utils.Client,
                               $"Error:Blank License Description",
                               "Please Fill out the attached License Description and resend CSV...",
                               contacts.Select(x => x.EmailAddress).ToArray(),
                               new string[] { errorfile }).ConfigureAwait(false);

                        // LogDocSetAction(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems");


                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AssessLicense(FileTypes ft)
        {

            try
            {
                SQLBlackBox.RunSqlBlackBox();

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 60;
                    var res = ctx.TODO_LICToAssess
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();

                    foreach (var doc in res)
                    {

                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Declarant_Reference_Number);
                        var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Declarant_Reference_Number, "LIC-Instructions.txt");
                        if (!File.Exists(instrFile)) continue;
                        var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Declarant_Reference_Number, "LIC-InstructionResults.txt");
                        var lcont = 0;
                        while (LICUtils.AssessLICComplete(instrFile, resultsFile, out lcont) == false)
                        {
                            Utils.RunSiKuLi(directoryName, "AssessLIC", lcont.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task ReImportLIC()
        {
            Console.WriteLine("Export Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docset =
                    ctx.TODO_PODocSet.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                   await ImportLicense(docset.Declarant_Reference_Number, docset.AsycudaDocumentSetId).ConfigureAwait(false);
                }
            }
        }
    }
}
