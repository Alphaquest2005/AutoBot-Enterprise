using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataQS.Business.Entities;
using ExcelDataReader;
using InventoryDS.Business.Entities;
using LicenseDS.Business.Entities;
using MoreLinq;
using SalesDataQS.Business.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AdjustmentQS.Business.Entities;
using Asycuda421;
using AutoBotUtilities;
using EmailDownloader;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using SimpleMvvmToolkit.ModelExtensions;
using TrackableEntities;
using TrackableEntities.Client;
using ValuationDS.Business.Entities;
using WaterNut.DataSpace;
using WaterNut.DataSpace.Asycuda;
using ApplicationException = System.ApplicationException;
using AsycudaDocument_Attachments = CoreEntities.Business.Entities.AsycudaDocument_Attachments;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using Contacts = CoreEntities.Business.Entities.Contacts;
using CoreEntitiesContext = CoreEntities.Business.Entities.CoreEntitiesContext;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class Utils
    {
        public class FileAction
        {

            public string Filetype { get; set; }

            public List<Action<FileTypes, FileInfo[]>> Actions { get; set; } =
                new List<Action<FileTypes, FileInfo[]>>();
        }

        public static Dictionary<string, Action<FileTypes, FileInfo[]>> FileActions =>
            new Dictionary<string, Action<FileTypes, FileInfo[]>>
            {
                {"ImportSalesEntries",(ft, fs) => ImportSalesEntries() },
                {"AllocateSales",(ft, fs) => AllocateSales() },
                {"CreateEx9",(ft, fs) => CreateEx9(false) },
                {"ExportEx9Entries",(ft, fs) => ExportEx9Entries() },
                {"AssessEx9Entries",(ft, fs) => AssessEx9Entries() },
                {"SaveCsv",(ft, fs) => SaveCsv(fs, ft) },
                {"ReplaceCSV",(ft, fs) => ReplaceCSV(fs, ft) },
                {"RecreatePOEntries",(ft, fs) => RecreatePOEntries(ft.AsycudaDocumentSetId) },
                {"ExportPOEntries",(ft, fs) => ExportPOEntries(ft.AsycudaDocumentSetId) },
                {"AssessPOEntry",(ft, fs) => AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId)},
                {"EmailPOEntries",(ft, fs) => EmailPOEntries(ft.AsycudaDocumentSetId,ft.FileTypeContacts.Select(x => x.Contacts).ToList()) },
                {"DownloadSalesFiles",(ft, fs) => DownloadSalesFiles(3, "IM7History",false) },
                {"Xlsx2csv",(ft, fs) => Xlsx2csv(fs, ft) },
                {"SaveInfo",(ft, fs) => TrySaveFileInfo(fs, ft) },
                {"CleanupEntries",(ft, fs) => CleanupEntries() },
                {"SubmitToCustoms",(ft, fs) => SubmitSalesXMLToCustoms() },
                {"MapUnClassifiedItems", (ft, fs) => MapUnClassifiedItems(ft,fs) },
                {"UpdateSupplierInfo", (ft, fs) => UpdateSupplierInfo(ft,fs) },
                {"ImportPDF", (ft, fs) => ImportPDF(fs,ft) },
                {"CreateShipmentEmail", CreateShipmentEmail },
               //{"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
                
                //{"AttachToDocSetByRef", (ft, fs) => AttachToDocSetByRef(ft.AsycudaDocumentSetId) },

                {"ClearDocSetEntries",(ft, fs) => ClearDocSetEntries(ft) },
               
                {"SubmitDocSetUnclassifiedItems",(ft, fs) => SubmitDocSetUnclassifiedItems(ft) },
                {"AllocateDocSetDiscrepancies",(ft, fs) => AllocateDocSetDiscrepancies(ft) },
                {"CleanupDocSetDiscpancies",(ft, fs) => CleanupDocSetDiscpancies(ft) },
                {"RecreateDocSetDiscrepanciesEntries",(ft, fs) => RecreateDocSetDiscrepanciesEntries(ft) },
                {"ExportDocSetDiscpancyEntries",(ft, fs) => ExportDocSetDiscpancyEntries("DIS",ft) },
                {"SubmitDocSetDiscrepancyErrors",(ft, fs) => SubmitDocSetDiscrepancyErrors(ft) },
                {"SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms",(ft, fs) => SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(ft) },
                {"AssessDiscrepancyExecutions", AssessDiscrepancyExecutions },
                {"AttachEmailPDF", AttachEmailPDF },
                {"ReSubmitDiscrepanciesToCustoms", ReSubmitDiscrepanciesToCustoms },
                {"ReSubmitSalesToCustoms", ReSubmitSalesToCustoms },


                {"SubmitMissingInvoices",  (ft, fs) => SubmitMissingInvoices(ft) },
                {"SubmitIncompleteEntryData",(ft, fs) =>  SubmitIncompleteEntryData(ft) },
                {"SubmitUnclassifiedItems",(ft, fs) =>  SubmitUnclassifiedItems(ft) },
                {"SubmitInadequatePackages",(ft, fs) => SubmitInadequatePackages(ft) },
                {"SubmitIncompleteSuppliers",(ft, fs) => SubmitIncompleteSuppliers(ft) },
                {"CreateC71",(ft, fs) => CreateC71(ft) },
                {"CreateLicense",(ft, fs) =>  CreateLicence(ft)},
                { "AssessC71",(ft, fs) => AssessC71(ft) },
                {"AssessLicense",(ft, fs) => AssessLicense(ft) },
                {"DownLoadC71", (ft, fs) => DownLoadC71(ft) },
                {"DownLoadLicense", (ft, fs) => DownLoadLicence(false, ft) },
                { "ImportC71", (ft, fs) => ImportC71(ft) },
                {"ImportLicense", (ft, fs) => ImportLicense(ft) },
               
                { "AttachToDocSetByRef",(ft, fs) =>  AttachToDocSetByRef(ft) },
                
                {"AssessPOEntries",(ft, fs) =>  AssessPOEntries(ft) },
                {"AssessDiscpancyEntries", AssessDiscpancyEntries },
                {"DeletePONumber", DeletePONumber },
                { "SubmitPOs", SubmitPOs },
                {"SubmitEntryCIF", SubmitEntryCIF },
                {"SubmitBlankLicenses", (ft,fs) => SubmitBlankLicenses(ft) },
                {"ProcessUnknownCSVFileType", (ft,fs) => ProcessUnknownCSVFileType(ft, fs) },
                {"ProcessUnknownPDFFileType", (ft,fs) => ProcessUnknownPDFFileType(ft, fs) },
                {"ImportUnAttachedSummary", (ft,fs) => ImportUnAttachedSummary(ft, fs) },
               

            };

     


        public static Dictionary<string, Action> SessionActions =>
            new Dictionary<string, Action>
            {

                {"CreateDiscpancyEntries",() => CreateAdjustmentEntries(false, "DIS") },
                {"RecreateDiscpancyEntries",() => CreateAdjustmentEntries(true, "DIS") },
                {"CreateAdjustmentEntries",() => CreateAdjustmentEntries(false, "ADJ") },
                {"RecreateAdjustmentEntries",() => CreateAdjustmentEntries(true, "ADJ") },
                {"AutoMatch", AutoMatch },
                {"AssessDiscpancyEntries", AssessDiscpancyEntries },
                {"ExportDiscpancyEntries", () => ExportDiscpancyEntries("DIS") },
                {"ExportAdjustmentEntries", () => ExportDiscpancyEntries("ADJ") },
                //{"SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", AllocateSales },
                {"CreateEx9",() => CreateEx9(false) },
                {"ExportEx9Entries", ExportEx9Entries },
                {"AssessEx9Entries", AssessEx9Entries },
                {"SubmitToCustoms", SubmitSalesXMLToCustoms },
                {"CleanupEntries", CleanupEntries },
                {"ClearAllocations", ClearAllocations },
                {"AssessDISEntries",() => AssessDISEntries("DIS") },
                {"AssessADJEntries",() => AssessDISEntries("ADJ") },
                {"DownloadSalesFiles",() => DownloadSalesFiles(20,"IM7History") },
                {"ImportSalesEntries",ImportSalesEntries },
                {"SubmitDiscrepanciesToCustoms",SubmitDiscrepanciesToCustoms },
                {"DownloadPDFs",DownloadPDFs },
                {"LinkPDFs", LinkPDFs },
                {"RemoveDuplicateEntries", RemoveDuplicateEntries },
                {"FixIncompleteEntries", FixIncompleteEntries },
                {"EmailEntriesExpiringNextMonth", EmailEntriesExpiringNextMonth },
                {"EmailWarehouseErrors", EmailWarehouseErrors },
                {"RecreateLatestPOEntries", RecreateLatestPOEntries },
                {"ReImportC71", ReImportC71 },
                {"ReImportLIC", ReImportLIC },
               
                {"DownLoadLicense", () => DownLoadLicence(false,new FileTypes()) },
                {"ReDownLoadLicence", () => DownLoadLicence(true, new FileTypes()) },
                {"CreateC71", () => CreateC71(new FileTypes()) },
                {"CreateLicense",() =>  CreateLicence(new FileTypes()) },
                { "ImportC71", () => ImportC71(new FileTypes()) },
                {"ImportLicense", () => ImportLicense(new FileTypes()) },
                {"DownLoadC71", () => DownLoadC71(new FileTypes()) },
                {"SubmitMissingInvoices", () => SubmitMissingInvoices(new FileTypes()) },
                {"SubmitIncompleteEntryData",() =>  SubmitIncompleteEntryData(new FileTypes()) },
                {"SubmitUnclassifiedItems",() =>  SubmitUnclassifiedItems(new FileTypes()) },
                {"SubmitInadequatePackages",() =>  SubmitInadequatePackages(new FileTypes()) },
                {"SubmitIncompleteSuppliers",() =>  SubmitIncompleteSuppliers(new FileTypes()) },
                {"AssessC71",() =>  AssessC71(new FileTypes()) },
                {"AssessLicense",() =>  AssessLicense(new FileTypes()) },
                {"RecreatePOEntries", RecreatePOEntries },
               
                {"ExportPOEntries", ExportPOEntries },
                {"AssessPOEntries",() =>  AssessPOEntries(new FileTypes()) },
                { "AttachToDocSetByRef",  AttachToDocSetByRef },
                {"DownloadPOFiles", DownloadPOFiles },
                {"SubmitPOs", SubmitPOs },
                {"RecreateEx9",() => CreateEx9(true) },
                {"ReDownloadSalesFiles",ReDownloadSalesFiles },
                {"CleanupDiscpancies", CleanupDiscpancies },
                {"SubmitDiscrepanciesPreAssessmentReportToCustoms", SubmitDiscrepanciesPreAssessmentReportToCustoms },
                {"ClearAllDiscpancyEntries", () => ClearAllAdjustmentEntries("DIS") },
                {"ClearAllAdjustmentEntries", () => ClearAllAdjustmentEntries("ADJ") },
                {"ImportPDF", ImportPDF },
                {"CreateInstructions", CreateInstructions },
                {"SubmitUnknownDFPComments", SubmitUnknownDFPComments },
                {"ClearPOEntries", ClearPOEntries },
               
                {"ExportLatestPOEntries", ExportLatestPOEntries },
                {"EmailLatestPOEntries", EmailLatestPOEntries },
                {"LinkEmail", LinkEmail },
                {"RenameDuplicateDocuments", RenameDuplicateDocuments },
                {"RenameDuplicateDocumentCodes", RenameDuplicateDocumentCodes },
                {"ReLinkPDFs", ReLinkPDFs },
                {"ImportAllSalesEntries", ImportAllSalesEntries },
                {"RebuildSalesReport", RebuildSalesReport },
                {"Ex9AllAllocatedSales",() => Ex9AllAllocatedSales(true) },
                {"SubmitSalesToCustoms", SubmitSalesToCustoms },
                {"ImportExpiredEntires", ImportExpiredEntires }


            };

        private static void ImportUnAttachedSummary(FileTypes ft, FileInfo[] fs)
        {
            foreach (var file in fs)
            {
                var reference = xlsxWriter.XlsxWriter.SaveUnAttachedSummary(file);
                var res = reference.Split('-');
                ft.EmailId = res[2];
                CreateShipmentEmail(ft, fs);
            }

        }

        private static void ProcessUnknownPDFFileType(FileTypes ft, FileInfo[] fs)
        {
            
        }


        private static void ProcessUnknownCSVFileType(FileTypes ft, FileInfo[] fs)
        {
            throw new NotImplementedException();
        }

        private static void CreateShipmentEmail(FileTypes fileType, FileInfo[] files)
        {
            try
            {

           
            var emailId = Convert.ToInt32(fileType.EmailId);
            
                // Todo quick paks etc put Man reg#, bl numer, TotalWeight & Totalfreight in spreadsheet. so invoice can generate the entry.
                // change the required documents to match too

                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Database.ExecuteSqlCommand(@"EXEC [dbo].[PreProcessShipmentSP]");
                }


                var shipments = new Shipment(){ShipmentName = "Next Shipment",EmailId = emailId, TrackingState = TrackingState.Added}
                                    .LoadEmailPOs()
                                    .LoadEmailInvoices()
                                    .LoadEmailRiders()
                                    .LoadEmailBL()
                                    .LoadEmailManifest()
                                    .LoadEmailFreight()
                                    .LoadDBBL()
                                    .LoadDBRiders()
                                    .LoadDBBL()
                                    .LoadDBRiders()
                                    .LoadDBFreight()
                                    .LoadDBManifest()
                                    .LoadDBInvoices()
                                    .ProcessShipment()
                                    //.SaveShipment()
                                    ;
               
                var contacts = new CoreEntitiesContext().Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer" || x.Role == "PO Clerk")
                        .Select(x => x.EmailAddress).ToArray();

               

                shipments.ForEach(shipment =>
                {
                    EmailDownloader.EmailDownloader.SendEmail(Client,"",
                                        $"CSVs for {shipment.ShipmentName}", contacts, shipment.ToString(), shipment.ShipmentAttachments.Select(x => x.Attachments.FilePath).ToArray());
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }




        private static void AttachEmailPDF(FileTypes ft, FileInfo[] fs)
        {
            BaseDataModel.AttachEmailPDF(ft.AsycudaDocumentSetId, ft.EmailId);
        }

        public static void ImportExpiredEntires()
        {

            try
            {
                var docSet = BaseDataModel.CurrentSalesInfo();
                var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docSet.Item3.Declarant_Reference_Number);
                var expFile = Path.Combine(directoryName, "ExpiredEntries.csv");
                if (File.Exists(expFile)) File.Delete(expFile);

                while (!File.Exists(expFile))
                {
                    RunSiKuLi(directoryName, "ExpiredEntries", "0");
                }
                ImportExpiredEntires(expFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportExpiredEntires(string expFile)
        {
            
            var fileType = new CoreEntitiesContext().FileTypes.First(x =>
                x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                x.Type == "ExpiredEntries");
            SaveCsv(new FileInfo[] {new FileInfo(expFile)}, fileType);
        }

        private static void RenameDuplicateDocumentCodes()
        {
            try
            {
                Console.WriteLine("Rename Duplicate Documents");
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();
                    var doclst = ctx.AsycudaDocuments.Where(x => x.AsycudaDocumentSetId == docset.AsycudaDocumentSetId)
                        .Select(x => x.ASYCUDA_Id).ToList();
                    if (docset != null)
                    {
                        BaseDataModel.RenameDuplicateDocumentCodes(doclst);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void RenameDuplicateDocuments()
        {
            try
            {
                Console.WriteLine("Rename Duplicate Documents");
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();
                    if (docset != null)
                    {
                        BaseDataModel.RenameDuplicateDocuments(docset.AsycudaDocumentSetId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }



        private static void CreateInstructions()
        {
            var dir = new DirectoryInfo(@"C:\Users\josep\OneDrive\Clients\Rouge\2019\October");
            var files = dir.GetFiles().Where(x => Regex.IsMatch(x.Name, @".*(P|F)\d+.xml"));
            if (File.Exists(Path.Combine(dir.FullName, "Instructions.txt")))
            {
                File.Delete(Path.Combine(dir.FullName, "Instructions.txt"));
                File.Delete(Path.Combine(dir.FullName, "InstructionResults.txt"));
            }
            foreach (var file in files)
            {
                File.AppendAllText(Path.Combine(dir.FullName, "Instructions.txt"), $"File\t{file.FullName}\r\n");
                if (File.Exists(file.FullName.Replace("xml", "csv")) && !File.Exists(file.FullName.Replace("xml", "csv.pdf")))
                    File.Copy(file.FullName.Replace("xml", "csv"), file.FullName.Replace("xml", "csv.pdf"));
                File.AppendAllText(Path.Combine(dir.FullName, "Instructions.txt"), $"Attachment\t{file.FullName.Replace("xml", "csv.pdf")}\r\n");
            }
        }

        private static void SubmitPOs()
        {
            try
            {


                Console.WriteLine("Submit POs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer")
                        .Select(x => x.EmailAddress).ToArray();

                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                        .Select(x => x.EmailAddress).ToArray();

                    var lst = ctx.TODO_SubmitPOInfo
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.FileTypeId != null)
                        .Where (x => x.IsSubmitted == false)
                        .Where(x => x.CNumber != null)
                        .ToList()
                        .MaxBy(x => x.AsycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments"),
                            x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new {x, z})
                        .ToList();

                    foreach (var doc in lst)
                    {
                        SubmitPOs(doc.z, doc.x.ToList(), contacts, poContacts);
                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitPOs(FileTypes ft, FileInfo[] fs)
        {
            try
            {


                Console.WriteLine("Submit POs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var poList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();

                    var contacts = ctx.Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer")
                        .Select(x => x.EmailAddress).ToArray();

                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                        .Select(x => x.EmailAddress).ToArray();
                    var docSet = ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments")
                                           .FirstOrDefault(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                    if (docSet == null)
                    {
                        throw new ApplicationException($"Asycuda Document Set not Found: {ft.AsycudaDocumentSetId}");
                    }
                    var rlst = ctx.TODO_SubmitPOInfo
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number))
                        .ToList();
                   List<TODO_SubmitPOInfo> lst;
                    if (rlst.Any())
                    {
                        lst = rlst.Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number))
                            .ToList();
                    }
                    else
                    {
                        lst = ctx.AsycudaDocuments.Where(x =>
                            x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number)
                            || poList.Contains(x.CNumber))
                            .Where(x => x.ImportComplete == true)
                            .Select(x => new {
                                ApplicationSettingsId = docSet.ApplicationSettingsId,
                                AssessedAsycuda_Id = x.ASYCUDA_Id,
                                CNumber = x.CNumber
                            }).ToList()
                            .Select(x => new TODO_SubmitPOInfo()
                        {
                            ApplicationSettingsId = docSet.ApplicationSettingsId,
                            ASYCUDA_Id = x.AssessedAsycuda_Id,
                            CNumber = x.CNumber
                        }).ToList();
                    }

                    if (poList.Any())
                    {
                        lst = lst.Where(x => poList.Contains(x.CNumber))
                            .ToList();
                    }



                    SubmitPOs(docSet, lst, contacts, poContacts);

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitPOs(AsycudaDocumentSetEx docSet, List<TODO_SubmitPOInfo> pOs, string[] contacts,
            string[] poContacts)
        {
            if (!pOs.Any())
            {
                EmailDownloader.EmailDownloader.SendEmail(Client, "",
                    $"Document Package for {docSet.Declarant_Reference_Number}",
                    contacts, "No Entries imported", Array.Empty<string>());

                EmailDownloader.EmailDownloader.SendEmail(Client, "",
                    $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                    poContacts, "No Entries imported", Array.Empty<string>());
                return;
            }

            using (var ctx = new CoreEntitiesContext())
            {
                try
                {

                    //var pdfs = Enumerable
                    //    .Select<AsycudaDocumentSet_Attachments, string>(docSet.AsycudaDocumentSet_Attachments,
                    //        x => x.Attachments.FilePath)
                    //    .Where(x => x.ToLower().EndsWith(".pdf"))
                    //    .ToList();

                    var Assessedpdfs = new List<string>();

                    var pdfs = new List<string>();

                    var poInfo = CurrentPOInfo(docSet.AsycudaDocumentSetId).FirstOrDefault();
                    if (!Directory.Exists(poInfo.Item2)) return;
                    foreach (var itm in pOs)
                    {
                        List<string> ndp = new List<string>();
                        var newEntry = ctx.AsycudaDocuments.FirstOrDefault(x =>
                            x.ReferenceNumber == itm.Reference &&
                            (x.ImportComplete == null || x.ImportComplete == false));
                        if (newEntry != null)
                        {
                            
                            ndp = ctx.AsycudaDocument_Attachments
                                .Where(x => x.AsycudaDocumentId == newEntry.ASYCUDA_Id)
                                .GroupBy(x => x.Attachments.Reference)
                                .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                                .Select(x => x.Attachments.FilePath)
                                .Where(x => x.ToLower().EndsWith(".pdf"))
                                .ToList();
                        }


                        var adp = ctx.AsycudaDocument_Attachments
                            .Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                            .GroupBy(x => x.Attachments.Reference)
                            .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                            .Select(x => x.Attachments.FilePath)
                            .Where(x => x.ToLower().EndsWith(".pdf"))
                            
                            .ToList();

                        if(!adp.Any())
                        {
                            BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id });
                            adp = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToList();
                        }

                        pdfs.AddRange(ndp);
                        pdfs.AddRange(adp);
                        Assessedpdfs.AddRange(adp);
                    }

                    pdfs = pdfs.Distinct().ToList();
                    Assessedpdfs = Assessedpdfs.Distinct().ToList();

                    var body =
                        $"Please see attached documents entries for {docSet.Declarant_Reference_Number}.\r\n" +
                        $"\r\n" +
                        $"Please open the attached email to view Email Thread.\r\n" +
                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                        $"\r\n" +
                        $"Regards,\r\n" +
                        $"AutoBot";


                    var xRes = Enumerable.Select<TODO_SubmitPOInfo, AssessedEntryInfo>(pOs, x =>
                        new AssessedEntryInfo()
                        {
                            DocumentType = x.DocumentType,
                            CNumber = x.CNumber,
                            Reference = x.Reference,
                            Date = x.Date,
                            PONumber = x.PONumber,
                            Invoice = x.SupplierInvoiceNo,
                            Taxes = x.Totals_taxes.GetValueOrDefault().ToString("C"),
                            CIF = x.Total_CIF.ToString("C"),
                            BillingLine = x.BillingLine
                        }).ToList();


                    var summaryFile = Path.Combine(poInfo.Item2, "Summary.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<AssessedEntryInfo, List<AssessedEntryInfo>>();
                    errRes.dataToPrint = xRes;
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    pdfs.Add(summaryFile);
                    Assessedpdfs.Add(summaryFile);


                    
                    var emailIds = Enumerable.Min<TODO_SubmitPOInfo>(pOs, x => x.EmailId);

                    if (emailIds == null)
                    {
                        EmailDownloader.EmailDownloader.SendEmail(Client, "",
                            $"Document Package for {docSet.Declarant_Reference_Number}",
                            contacts, body, pdfs.ToArray());

                        EmailDownloader.EmailDownloader.SendEmail(Client, "",
                            $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                            poContacts, body, Assessedpdfs.ToArray());
                    }
                    else
                    {
                        EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds), Client,
                            $"Document Package for {docSet.Declarant_Reference_Number}", body, contacts,
                            pdfs.ToArray());

                        EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds), Client,
                            $"Assessed Entries for {docSet.Declarant_Reference_Number}", body, poContacts,
                            Assessedpdfs.ToArray());
                    }

                    foreach (var item in pOs)
                    {
                        var sfile = Queryable.FirstOrDefault(ctx.AsycudaDocuments, x =>
                            x.ASYCUDA_Id == item.ASYCUDA_Id &&
                            x.ApplicationSettingsId == item.ApplicationSettingsId);
                        var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                            x.Attachments.FilePath == sfile.SourceFileName);
                        if (eAtt == null)
                        {
                            var att = ctx.Attachments.OrderByDescending(x => x.Id).FirstOrDefault(x => x.FilePath == sfile.SourceFileName);
                            eAtt = new AsycudaDocumentSet_Attachments()
                            {
                                AsycudaDocumentSetId = item.AsycudaDocumentSetId,
                                AttachmentId = att.Id,
                                DocumentSpecific = true,
                                FileDate = DateTime.Now,
                                EmailUniqueId = (string.IsNullOrEmpty(att.EmailId) ? null : (int?)Convert.ToInt32(att.EmailId)),
                                TrackingState = TrackingState.Added
                            };
                        }
                        ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = eAtt.Id,
                                AsycudaDocumentSet_Attachments = eAtt,
                                Status = "Submit PO Entries",
                                TrackingState = TrackingState.Added
                            });
                    }


                    ctx.SaveChanges();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }


        }

        private static void DownloadPOFiles()
        {
            DownloadSalesFiles(3, "IM7", false); //download all for now
        }

        private static void ImportPDF()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var fileType = ctx.FileTypes

                    .FirstOrDefault(x => x.Id == 17);
                var files = new FileInfo[]
                    {new FileInfo(@"C:\Users\josep\OneDrive\Clients\Budget Marine\Emails\30-16170\7006359.pdf")};
                ImportPDF(files, fileType);
            }
        }




        private static void ImportPDF(FileInfo[] csvFiles, FileTypes fileType)
        //(int? fileTypeId, int? emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSet, string fileType)
        {
            Console.WriteLine("Importing PDF " + fileType.Type);
            var failedFiles = new List<string>();
            foreach (var file in csvFiles.Where(x => x.Extension.ToLower() == ".pdf"))
            {
                int? emailId = 0;
                int? fileTypeId = 0;
                using (var ctx = new CoreEntitiesContext())
                {

                    var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == file.FullName)
                        .Select(x => new { x.EmailUniqueId, x.FileTypeId }).FirstOrDefault();
                    emailId = res?.EmailUniqueId;
                    fileTypeId = res?.FileTypeId;
                }
                var success = InvoiceReader.Import(file.FullName, fileTypeId.GetValueOrDefault(), emailId.GetValueOrDefault(), true, SaveCSVModel.Instance.GetDocSets(fileType), fileType, Utils.Client);
               
            }
        }

        private static void ExportPOEntries()
        {

            try
            {
                Console.WriteLine("Export PO Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    foreach (var docset in
                        ctx.TODO_PODocSetToExport
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    {
                        ExportPOEntries(docset.AsycudaDocumentSetId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void ExportLatestPOEntries()
        {
            Console.WriteLine("Export Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.AsycudaDocumentSetExs.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                     .OrderByDescending(x => x.AsycudaDocumentSetId)
                                     .FirstOrDefault();
                if(docset != null)
                {
                    ExportPOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        private static void RecreatePOEntries()
        {
            
            Console.WriteLine("Create PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
               var docset =
                    ctx.TODO_PODocSet
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    RecreatePOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        private static void RecreateLatestPOEntries()
        {
            Console.WriteLine("Create Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docSet =
                    ctx.AsycudaDocumentSetExs.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                var res = new EntryDataDSContext().EntryDataDetails.Where(x =>
                        x.EntryData.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                        x.EntryData.AsycudaDocumentSets.Any(z => z.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId))

                    .Select(z => z.EntryDataDetailsId).ToList();
            
                
                    CreatePOEntries(docSet.AsycudaDocumentSetId, res);
                
            }
        }

        private static void AssessLicense(FileTypes ft)
        {
            
                
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var res = ctx.TODO_LICToCreate
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
                    while (AssessLICComplete(instrFile, resultsFile, out lcont) == false)
                    {
                        RunSiKuLi(directoryName, "AssessLIC", lcont.ToString());
                    }
                }
            }
        }

        private static void AssessC71(FileTypes ft)
        {
            
                
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var res = ctx.TODO_C71ToCreate
                    .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                    .OrderByDescending(x => x.Id)
                    .Take(1)
                    .ToList();

                foreach (var doc in res)
                {

                    var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number);
                    var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "C71-Instructions.txt");
                    if (!File.Exists(instrFile)) continue;
                    var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "C71-InstructionResults.txt");
                    var lcont = 0;
                    while (AssessC71Complete(instrFile, resultsFile, out lcont) == false)
                    {
                         
                        RunSiKuLi(directoryName, "AssessC71", lcont.ToString());
                    }
                }
            }
        }

        private static void AttachToDocSetByRef(FileTypes ft)
        {
            BaseDataModel.Instance.AttachToExistingDocuments(ft.AsycudaDocumentSetId);
           // BaseDataModel.Instance.CalculateDocumentSetFreight(asycudaDocumentSetId).Wait();
        }

        
        public static void AttachToDocSetByRef()
        {
            
                
            Console.WriteLine("Attach Documents To DocSet");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var lst = ctx.TODO_PODocSet
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.AttachToExistingDocuments(doc);
                }

            }
        }


        private static void UpdateSupplierInfo(FileTypes ft, FileInfo[] fs)
        {
            using (var ctx = new EntryDataDSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 0;
                foreach (var file in fs)
                {
                    var dt = CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["SupplierCode"].ToString())) continue;
                        var supplierCode = row["SupplierCode"].ToString();
                        var itm = ctx.Suppliers.First(x => x.SupplierCode == supplierCode && x.ApplicationSettingsId ==
                                                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        itm.SupplierName = row["SupplierName"].ToString();
                        itm.Street = row["SupplierAddress"].ToString();
                        itm.CountryCode = row["CountryCode"].ToString();

                        ctx.SaveChanges();
                    }

                }
            }
        }

        private static void MapUnClassifiedItems(FileTypes ft, FileInfo[] fs)
        {
            Console.WriteLine("Mapping unclassified items");
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var file in fs)
                {
                    var dt = CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["TariffCode"].ToString())) continue;
                        var itmNumber = row["ItemNumber"].ToString();
                        var itms = ctx.InventoryItems.Where(x => x.ItemNumber == itmNumber && x.ApplicationSettingsId ==
                                   BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                        foreach (var itm in itms)
                        {
                            itm.TariffCode = row["TariffCode"].ToString();
                        }

                        ctx.SaveChanges();
                    }

                }
            }
        }


        private static void AssessDiscrepancyExecutions(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext() {StartTracking = true})
                {
                    ctx.Database.CommandTimeout = 0;
                    foreach (var file in fs)
                    {
                        var dt = CSV2DataTable(file, "YES");
                        if (dt.Rows.Count == 0) continue;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (string.IsNullOrEmpty(row["Reference"].ToString())) continue;
                            var reference = row["Reference"].ToString();
                            var doc = ctx.AsycudaDocuments.Include(x => x.AsycudaDocumentSetEx).FirstOrDefault(x =>
                                    x.ReferenceNumber == reference && x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                ?.AsycudaDocumentSetEx;
                            if(doc != null) AssessEntries(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
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

        private static void CreateLicence(FileTypes ft)
        {
            
                
            try

            {
                Console.WriteLine("Create License Files");
                

                using (var ctx = new LicenseDSContext())
                {

                    ctx.Database.CommandTimeout = 0;
                    var pOs = ctx.TODO_LICToCreate
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                       .ToList();
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName =  Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            pO.Declarant_Reference_Number);
                        if (!Directory.Exists(directoryName)) continue;


                        var llst = new CoreEntitiesContext().Database
                            .SqlQuery<TODO_LicenseToXML>(
                                $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {pO.AsycudaDocumentSetId} and LicenseDescription is not null").ToList();

                            var lst = llst
                                .Where(x => !string.IsNullOrEmpty(x.LicenseDescription))
                                .GroupBy(x => x.EntryDataId)
                                .SelectMany(x => x.OrderByDescending(z => z.SourceFile))//.FirstOrDefault()
                                .GroupBy(x => new {x.EntryDataId, x.TariffCategoryCode, x.SourceFile})
                            .ToList();

                        
                       

                        foreach (var itm in lst)
                        {
                            var fileName = Path.Combine(directoryName, $"{itm.Key.EntryDataId}-{itm.Key.TariffCategoryCode}-LIC.xml");


                            if (File.Exists(fileName))
                            {

                                var instrFile = Path.Combine(
                                    BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                    pO.Declarant_Reference_Number, "LIC-Instructions.txt");
                                if (File.Exists(instrFile))
                                {
                                    var resultsFile = Path.Combine(
                                        BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                        pO.Declarant_Reference_Number, "LIC-InstructionResults.txt");
                                    var lcont = 0;
                                    if(AssessLICComplete(instrFile, resultsFile, out lcont) == true)
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

                            var lic = LicenseToDataBase.Instance.CreateLicense(new List<TODO_LicenseToXML>(itm) , contact, supplier,
                                itm.Key.EntryDataId);
                            var invoices = itm.Select(x => new Tuple<string, string>(x.EntryDataId, Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.EntryDataId}.pdf"))).Where(x => File.Exists(x.Item2)).Distinct().ToList();
                            if(!invoices.Any()) continue;
                            ctx.xLIC_License.Add(lic);
                            ctx.SaveChanges();
                            LicenseToDataBase.Instance.ExportLicense(pO.AsycudaDocumentSetId, lic, fileName,
                                invoices);

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

        private static void SubmitBlankLicenses( FileTypes ft)
        {
            try
            {
                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;




                using (var ctx = new CoreEntitiesContext())
                {

                    var llst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_LicenseToXML>(
                            $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {ft.AsycudaDocumentSetId}").ToList();

                    var emails = llst
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.EmailId != null)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {

                        //if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;


                        var errorfile = Path.Combine(directory, $"BlankLicenseDescription-{email.Key.EmailId}.csv");
                        var errors = email.Select(x => new BlankLicenseDescription()
                        {
                            InvoiceNo = x.EntryDataId,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode,
                            LicenseDescription = x.LicenseDescription
                        }).ToList();


                        var res =
                            new ExportToCSV<BlankLicenseDescription,
                                List<BlankLicenseDescription>>();
                        res.dataToPrint = errors;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = ctx.Contacts.Where(x => x.Role == "Broker").ToList();
                        if (File.Exists(errorfile))
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(email.Key.EmailId), Client,
                                $"Error:Blank License Description",
                                "Please Fill out the attached License Description and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

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


        private static void CreateC71(FileTypes ft)
        {
            try

            {
                Console.WriteLine("Create C71 Files");
                
                using (var ctx = new ValuationDSContext())
                {





                    ctx.Database.CommandTimeout = 0;
                    var pOs = ctx.TODO_C71ToCreate
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            pO.Declarant_Reference_Number);
                        var fileName = Path.Combine(directoryName, "C71.xml");
                        if (File.Exists(fileName) && (pO.TotalCIF.HasValue && Math.Abs(pO.TotalCIF.GetValueOrDefault() - pO.C71Total) <= 0.01)) continue;
                        var c71results = Path.Combine(directoryName, "C71-InstructionResults.txt");
                        if (File.Exists(c71results))
                        {
                            var c71instructions = Path.Combine(directoryName, "C71-Instructions.txt");
                            //if (AssessC71Complete(c71instructions, c71results, out int lcont) == true) continue;


                            File.Delete(c71results);
                            if (File.Exists(Path.Combine(directoryName, "C71OverView-PDF.txt"))) File.Delete(Path.Combine(directoryName, "C71OverView-PDF.txt"));
                        } 
                        var lst = new CoreEntitiesContext().TODO_C71ToXML.Where(x =>
                            x.ApplicationSettingsId == pO.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == pO.AsycudaDocumentSetId)
                            .GroupBy(x => x.AsycudaDocumentSetId)
                            .Where(x => x.Sum(z => z.InvoiceTotal * z.CurrencyRate) > 1 ).ToList();
                        if (!lst.Any()) continue;
                        var supplierCode = lst.SelectMany(x => x.Select(z => z.SupplierCode)).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                        Suppliers supplier = new Suppliers();
                        if (supplierCode == null)
                        {

                        }
                        else
                        {
                            supplier = new EntryDataDSContext().Suppliers.FirstOrDefault(x =>
                                x.SupplierCode == supplierCode &&
                                x.ApplicationSettingsId == pO.ApplicationSettingsId);
                        }
                        
                        var c71 = C71ToDataBase.Instance.CreateC71(supplier, lst.SelectMany(x => x.Select(z => z)).ToList(), pO.Declarant_Reference_Number);
                        ctx.xC71_Value_declaration_form.Add(c71);
                        ctx.SaveChanges();
                        C71ToDataBase.Instance.ExportC71(pO.AsycudaDocumentSetId, c71, fileName);


                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }




        private static void DownLoadLicence(bool redownload, FileTypes ft)
        {
            try
            
                
            {
                using (var ctx = new CoreEntitiesContext())
                {

                    ctx.Database.CommandTimeout = 0;
                    var pOs = ctx.TODO_LICToCreate
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();

                    if (!pOs.Any()) return;
                        var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports", "LIC")); ;
                        Console.WriteLine("Download License Files");
                        var lcont = 0;
                        if (redownload)
                        {
                            RunSiKuLi(directoryName, "LIC", lcont.ToString());
                        }
                        else
                        {
                            while (ImportLICComplete(directoryName, out lcont) == false)
                            {
                                RunSiKuLi(directoryName, "LIC", lcont.ToString());
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

        private static void DownLoadC71(FileTypes ft)
        {
            try

            {
                Console.WriteLine("Attempting Download C71 Files");
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;

                    var lst = ctx.TODO_C71ToCreate
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId) 
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .OrderByDescending(x => x.Id)
                        .Take(1)
                        .ToList();


                    if (!lst.Any()) return;
                        var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports","C71"));

                    Console.WriteLine("Download C71 Files");
                    var notries = 2;
                    var tries = 0;
                    var lcont = 0;
                        while (ImportC71Complete(directoryName, out lcont) == false )
                        {
                            RunSiKuLi(directoryName, "C71", lcont.ToString());
                            tries += 1;
                            if (tries >= notries) break;
                        }
                   

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void FixIncompleteEntries()
        {
            try
            {


                Console.WriteLine("ReImport Incomplete Entries");



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;

                    var lst = ctx.TODO_Error_IncompleteItems
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => new { x.ASYCUDA_Id, x.SourceFileName, x.AsycudaDocumentSetId });

                    foreach (var doc in lst)
                    {
                        BaseDataModel.Instance.DeleteAsycudaDocument(doc.Key.ASYCUDA_Id).Wait();
                        BaseDataModel.Instance.ImportDocuments(doc.Key.AsycudaDocumentSetId.GetValueOrDefault(), new List<string>() { doc.Key.SourceFileName }, true, true, false, true, true).Wait();
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void RemoveDuplicateEntries()
        {
            try
            {


                Console.WriteLine("Remove DuplicateEntries");



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.TODO_Error_DuplicateEntry
                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                .GroupBy(x => x.id);

                    foreach (var dup in lst)
                    {
                        var doc = dup.Last();
                        BaseDataModel.Instance.DeleteAsycudaDocument(doc.ASYCUDA_Id).Wait();
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        private static void ClearAllocations()
        {
            Console.WriteLine("Clear Allocations");
            AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
        }


        

        private static void SubmitSalesXMLToCustoms()
        {
            try
            {


                Console.WriteLine("Submit XML To Customs");

                // var saleInfo = CurrentSalesInfo();
                var salesinfo = BaseDataModel.CurrentSalesInfo();

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitXMLToCustoms.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.AssessmentDate >= salesinfo.Item1 && x.AssessmentDate <= salesinfo.Item2).ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Ex-Warehouse Entries were Assessed. \r\n" +
                                   $"Start Date: {salesinfo.Item1} End Date {salesinfo.Item2}: \r\n" +
                                   "\tCNumber\t\tReference\t\tAssessmentDate\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.CNumber}\t\t{current.ReferenceNumber}\t\t{current.RegistrationDate.Value:yyyy-MM-dd} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                        foreach (var itm in emailIds)
                        {

                            attlst.AddRange(ctx.AsycudaDocument_Attachments
                                .Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath)
                                .ToList());
                        }


                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Assessed Ex-Warehouse Entries",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Assessed Ex-Warehouse Entries", body, contacts, attlst.ToArray());
                        }

                        foreach (var item in emailIds)
                        {
                            var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                                x.ASYCUDA_Id == item.ASYCUDA_Id &&
                                x.ApplicationSettingsId == item.ApplicationSettingsId);
                            var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                                x.Attachments.FilePath == sfile.SourceFileName);
                            if (eAtt != null)
                            {
                                ctx.AttachmentLog.Add(new AttachmentLog(true)
                                {
                                    DocSetAttachment = eAtt.Id,
                                    Status = "Submit XML To Customs",
                                    TrackingState = TrackingState.Added
                                });
                            }
                        }

                        ctx.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void ReSubmitDiscrepanciesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                var emailId = Convert.ToInt32(ft.EmailId);
                var lst = GetSubmitEntryData(ft);
                SubmitDiscrepanciesToCustoms(lst.Where(x => x.Key == emailId));

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void SubmitDiscrepanciesToCustoms()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 60;
               
                IEnumerable<IGrouping<int?, TODO_SubmitDiscrepanciesToCustoms>> lst;
                lst = ctx.TODO_SubmitDiscrepanciesToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId)

                    .ToList()

                    .GroupBy(x => x.EmailId);
                SubmitDiscrepanciesToCustoms(lst);

            }
        }

               
        private static void SubmitDiscrepanciesToCustoms(IEnumerable<IGrouping<int?, TODO_SubmitDiscrepanciesToCustoms>> lst)
        {
            try
            {


                Console.WriteLine("Submit Discrepancies To Customs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 60;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs").Select(x => x.EmailAddress).ToArray();
                   
                    foreach (var data in lst)
                    {
                        var emailIds = data.DistinctBy(x => x.CNumber).ToList();
                        if(!emailIds.Any())
                        {
                            throw new NotImplementedException();
                        }
                        var pdfs = new List<string>();
                        foreach (var itm in emailIds)
                        {
                            
                            var res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                            if (!res.Any())
                            {
                                BaseDataModel.LinkPDFs( new List<int>() {itm.ASYCUDA_Id});
                                res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                            }
                            pdfs.AddRange(res);
                        }

                        if (pdfs.Count == 0) continue;

                        var body = "The Following Discrepancies Entries were Assessed. \r\n" +

                                   $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"To Be Paid".FormatedSpace(20)}{"AssessmentDate".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.ReferenceNumber.FormatedSpace(20)}{current.ToBePaid.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"\r\n" +
                                   $"Please open the attached email to view Email Thread.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";


                        var info = CurrentPOInfo(emailIds.First().AsycudaDocumentSetId.GetValueOrDefault()).FirstOrDefault();
                        var directory = info.Item2;

                        var summaryFile = Path.Combine(directory, $"Summary.csv");

                        var executionFile = Path.Combine(directory, $"ExecutionReport.csv");
                        var execData = new CoreEntitiesContext().TODO_DiscrepanciesExecutionReport
                            .Where(x => x.EmailId == data.Key)
                            .Select(x => new DiscpancyExecData() 
                            {
                                InvoiceNo = x.InvoiceNo,
                                InvoiceDate = x.InvoiceDate,
                                ItemNumber = x.ItemNumber,
                                InvoiceQty = x.InvoiceQty,
                                ReceivedQty = x.ReceivedQty,
                                CNumber = x.CNumber,
                                Status = x.Status,
                                xCNumber = x.xCNumber,
                                xLineNumber = x.xLineNumber,
                                xRegistrationDate = x.xRegistrationDate

                            })
                            .ToList();

                        var exeRes =
                            new ExportToCSV<DiscpancyExecData,
                                List<DiscpancyExecData>>();
                        exeRes.dataToPrint = execData;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => exeRes.SaveReport(executionFile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }
                        pdfs.Add(executionFile);

                        var sumData = emailIds
                            .Select(x => new SubmitEntryData()
                            {
                                CNumber = x.CNumber,
                                ReferenceNumber = x.ReferenceNumber,
                                DocumentType = x.DocumentType,
                                CustomsProcedure = x.CustomsProcedure,
                                RegistrationDate = x.RegistrationDate,
                                AssessmentDate = x.AssessmentDate
                            })
                            .ToList();

                        var sumres =
                            new ExportToCSV<SubmitEntryData,
                                List<SubmitEntryData>>();
                        sumres.dataToPrint = sumData;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => sumres.SaveReport(summaryFile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }
                        pdfs.Add(summaryFile);

                        string reference = emailIds.First().ReferenceNumber.Substring(0, emailIds.First().ReferenceNumber.IndexOf('-'));

                        if (data.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", $"Assessed Shipping Discrepancy Entries: {reference}",
                                contacts, body, pdfs.ToArray());
                        }
                        else
                        {
                            
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(data.Key), Client, $"Assessed Shipping Discrepancy Entries: {reference}", body, contacts, pdfs.ToArray());
                        }

                        foreach (var item in emailIds)
                        {
                            var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                                x.ASYCUDA_Id == item.ASYCUDA_Id &&
                                x.ApplicationSettingsId == item.ApplicationSettingsId);
                            var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                                x.Attachments.FilePath == sfile.SourceFileName);
                            if (eAtt != null)
                            {
                                ctx.AttachmentLog.Add(new AttachmentLog(true)
                                {
                                    DocSetAttachment = eAtt.Id,
                                    Status = "Submit XML To Customs",
                                    TrackingState = TrackingState.Added
                                });
                            }
                        }

                        ctx.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitDiscrepanciesPreAssessmentReportToCustoms()
        {
            try
            {


                Console.WriteLine("Submit Discrepancies PreAssessment Report to Customs");


                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs" || x.Role == "Clerk").Select(x => x.EmailAddress).ToArray();
                    var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId && x.Type == "DIS").ToList();
                    var errors = ctx.TODO_DiscrepanciesErrors.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId)
                        .Select(x => new SubmitDiscrepanciesErrorReport()
                        {
                            Type = x.Type,
                            InvoiceDate = x.InvoiceDate,
                            EffectiveDate = x.EffectiveDate,
                            InvoiceNo = x.InvoiceNo,
                            LineNumber = x.LineNumber,
                            ItemNumber = x.ItemNumber,
                            ItemDescription = x.ItemDescription,
                            InvoiceQty = x.InvoiceQty,
                            ReceivedQty = x.ReceivedQty,
                            Quantity = x.Quantity,
                            Cost = x.Cost,
                            PreviousCNumber = x.PreviousCNumber,
                            PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                            comment = x.Comment,
                            Status = x.Status,
                            DutyFreePaid = x.DutyFreePaid,
                            subject = x.Subject,
                            emailDate = x.EmailDate

                        })
                        .ToList();

                    var goodadj = ctx.TODO_DiscrepancyPreExecutionReport.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId)
                        .Select(x => new DiscrepancyPreExecutionReport()
                        {
                            Type = x.Type,
                            InvoiceDate = x.InvoiceDate,
                            EffectiveDate = x.EffectiveDate,
                            InvoiceNo = x.EntryDataId,
                            LineNumber = x.LineNumber,
                            ItemNumber = x.ItemNumber,
                            ItemDescription = x.ItemDescription,
                            InvoiceQty = x.InvoiceQty,
                            ReceivedQty = x.ReceivedQty,
                            Quantity = x.Quantity,
                            Cost = x.Cost,
                            PreviousCNumber = x.PreviousCNumber,
                            PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                            comment = x.Comment,
                            Status = x.Status,
                            DutyFreePaid = x.DutyFreePaid,
                            subject = x.Subject,
                            emailDate = x.EmailDate ?? DateTime.MinValue,
                            Reference = x.Reference,
                            DocumentType = x.DocumentType,

                        })
                        .ToList();
                    var attachments = new System.Collections.Generic.List<string>();

                    var errBreakdown = errors.GroupBy(x => x.Status).ToList();

                    if (errors.Any())
                    {

                        //foreach (var emailIds in lst)
                        //{



                        var errorfile = Path.Combine(directory, "DiscrepancyExecutionErrors.csv");
                        if (File.Exists(errorfile)) File.Delete(errorfile);
                        var errRes =
                            new ExportToCSV<SubmitDiscrepanciesErrorReport, List<SubmitDiscrepanciesErrorReport>>();
                        errRes.dataToPrint = errors;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => errRes.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }
                        attachments.Add(errorfile);
                    }

                    if (goodadj.Any())
                    {
                        var goodfile = Path.Combine(directory, "DiscrepancyExecutions.csv");
                        if (File.Exists(goodfile)) File.Delete(goodfile);
                        var goodRes =
                            new ExportToCSV<DiscrepancyPreExecutionReport, List<DiscrepancyPreExecutionReport>>();
                        goodRes.dataToPrint = goodadj;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => goodRes.SaveReport(goodfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }
                        attachments.Add(goodfile);

                    }
                    if (attachments.Any())
                    {
                        var body =
                            $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                            $"\r\n" +
                            $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(20)}{"Percentage".FormatedSpace(20)}\r\n" +
                            $"{string.Join(",", errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(20)}{(Math.Round((double)(((double)current.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n"))}" +
                            $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(20)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n" +
                            $"\r\n" +
                            $"Please see attached for list of Errors and Executions details.\r\n" +
                            $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                            $"\r\n" +
                            $"Regards,\r\n" +
                            $"AutoBot";
                        EmailDownloader.EmailDownloader.SendEmail(Client, directory,
                                $"Discrepancy Pre-Assessment Report for  {totaladjustments.Min(x => x.EffectiveDate)} To: {totaladjustments.Max(x => x.EffectiveDate)}",
                                contacts.ToArray(), body, attachments.ToArray());
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(FileTypes fileType)
        {
            try
            {


                Console.WriteLine("Submit Discrepancies PreAssessment Report to Customs");


                

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var docset = ctx.AsycudaDocumentSetExs.FirstOrDefault(x =>
                        x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                    if (docset == null) return;

                    var info = CurrentPOInfo(fileType.AsycudaDocumentSetId).FirstOrDefault();
                                    var directory = info.Item2;

                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs" || x.Role == "Clerk").Select(x => x.EmailAddress).ToArray();
                    var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId 
                                    && x.Type == "DIS"
                                    && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).ToList();
                    var errors = ctx.TODO_DiscrepanciesErrors.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                            && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                        .Select(x => new SubmitDiscrepanciesErrorReport()
                        {
                            Type = x.Type,
                            InvoiceDate = x.InvoiceDate,
                            EffectiveDate = x.EffectiveDate,
                            InvoiceNo = x.InvoiceNo,
                            LineNumber = x.LineNumber,
                            ItemNumber = x.ItemNumber,
                            ItemDescription = x.ItemDescription,
                            InvoiceQty = x.InvoiceQty,
                            ReceivedQty = x.ReceivedQty,
                            Quantity = x.Quantity,
                            Cost = x.Cost,
                            PreviousCNumber = x.PreviousCNumber,
                            PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                            comment = x.Comment,
                            Status = x.Status,
                            DutyFreePaid = x.DutyFreePaid,
                            subject = x.Subject,
                            emailDate = x.EmailDate

                        })
                        .ToList();

                    var goodadj = ctx.TODO_DiscrepancyPreExecutionReport.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                            && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                        .Select(x => new DiscrepancyPreExecutionReport()
                        {
                            Type = x.Type,
                            InvoiceDate = x.InvoiceDate,
                            EffectiveDate = x.EffectiveDate,
                            InvoiceNo = x.EntryDataId,
                            LineNumber = x.LineNumber,
                            ItemNumber = x.ItemNumber,
                            ItemDescription = x.ItemDescription,
                            InvoiceQty = x.InvoiceQty,
                            ReceivedQty = x.ReceivedQty,
                            Quantity = x.Quantity,
                            Cost = x.Cost,
                            PreviousCNumber = x.PreviousCNumber,
                            PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                            comment = x.Comment,
                            Status = x.Status,
                            DutyFreePaid = x.DutyFreePaid,
                            subject = x.Subject,
                            emailDate = x.EmailDate??DateTime.MinValue,
                            Reference = x.Reference,
                            DocumentType = x.DocumentType,

                        })
                        .ToList();
                    var attachments = new System.Collections.Generic.List<string>();

                    var errBreakdown = errors.GroupBy(x => x.Status).ToList();

                    if (errors.Any())
                    {

                        //foreach (var emailIds in lst)
                        //{



                        var errorfile = Path.Combine(directory, "DiscrepancyExecutionErrors.csv");
                        if (File.Exists(errorfile)) File.Delete(errorfile);
                        var errRes =
                            new ExportToCSV<SubmitDiscrepanciesErrorReport, List<SubmitDiscrepanciesErrorReport>>();
                        errRes.dataToPrint = errors;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => errRes.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }
                        attachments.Add(errorfile);
                        var errorBody = "The Attached File contains details of errors found trying to assessed the attached email's Shipping Discrepancies \r\n" +
                                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                            $"\r\n" +
                                            $"Regards,\r\n" +
                                            $"AutoBot";

                        EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(fileType.EmailId), Client, "Error Found Assessing Shipping Discrepancy Entries", errorBody,contacts ,attachments.ToArray() );
                    }

                    if (goodadj.Any())
                    {
                        var goodfile = Path.Combine(directory, "DiscrepancyExecutions.csv");
                        if (File.Exists(goodfile)) File.Delete(goodfile);
                        var goodRes =
                            new ExportToCSV<DiscrepancyPreExecutionReport, List<DiscrepancyPreExecutionReport>>();
                        goodRes.dataToPrint = goodadj;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => goodRes.SaveReport(goodfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }
                        attachments.Add(goodfile);

                    }
                    if (attachments.Any())
                    {
                        var body =
                            $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                            $"\r\n" +
                            $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(20)}{"Percentage".FormatedSpace(20)}\r\n" +
                            $"{string.Join(",", errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(20)}{(Math.Round((double)(((double)current.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n"))}" +
                            $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(20)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n" +
                            $"\r\n" +
                            $"Please see attached for list of Errors and Executions details.\r\n" +
                            $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                            $"\r\n" +
                            $"Regards,\r\n" +
                            $"AutoBot";
                        EmailDownloader.EmailDownloader.SendEmail(Client, directory,
                                $"Discrepancy Pre-Assessment Report for  {docset.Declarant_Reference_Number}",
                                contacts.ToArray(), body, attachments.ToArray());
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void DeletePONumber(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                Console.WriteLine("Delete PO Numbers");
                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Database.CommandTimeout = 60;
                    var cnumberList = ft.Data.Where(z => z.Key == "PONumber").Select(x => x.Value).ToList();

                    foreach (var itm in cnumberList)
                    {
                        var res = ctx.EntryData.FirstOrDefault(x => x.EntryDataId == itm &&
                                                 x.ApplicationSettingsId == BaseDataModel.Instance
                                                     .CurrentApplicationSettings.ApplicationSettingsId);
                        if (res != null) ctx.EntryData.Remove(res);
                    }

                    ctx.SaveChanges();


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void ReSubmitSalesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {

                
                    var lst = GetSubmitEntryData(ft);


                    SubmitSalesToCustoms(lst);

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static IEnumerable<IGrouping<int?, TODO_SubmitDiscrepanciesToCustoms>> GetSubmitEntryData(FileTypes ft)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 60;
                var cnumberList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();

                IEnumerable<IGrouping<int?, TODO_SubmitDiscrepanciesToCustoms>> lst;
                List<TODO_SubmitAllXMLToCustoms> res;
                if (cnumberList.Any())
                {
                    res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                .ApplicationSettingsId && cnumberList.Contains(x.CNumber))
                        .ToList();
                }
                else
                {
                    var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(ft.AsycudaDocumentSetId).Result;
                    res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                .ApplicationSettingsId && x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number))
                        .ToList();
                }

                lst = res.Select(x => new TODO_SubmitDiscrepanciesToCustoms()
                    {
                        CNumber = x.CNumber,
                        ApplicationSettingsId = x.ApplicationSettingsId,
                        AssessmentDate = x.AssessmentDate,
                        ASYCUDA_Id = x.ASYCUDA_Id,
                        AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                        CustomsProcedure = x.CustomsProcedure,
                        DocumentType = x.DocumentType,
                        EmailId = x.EmailId,
                        ReferenceNumber = x.ReferenceNumber,
                        RegistrationDate = x.RegistrationDate,
                        Status = x.Status
                    })
                    .GroupBy(x => x.EmailId);
                return lst;
            }
        }

        private static void SubmitSalesToCustoms()
        {
            try
            {

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 60;
                

                    IEnumerable<IGrouping<int?, TODO_SubmitDiscrepanciesToCustoms>> lst;
                    lst = ctx.TODO_SubmitSalesToCustoms.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)

                        .ToList()
                        .Select(x => new TODO_SubmitDiscrepanciesToCustoms()
                        {
                            CNumber = x.CNumber,
                            ApplicationSettingsId = x.ApplicationSettingsId,
                            AssessmentDate = x.AssessmentDate,
                            ASYCUDA_Id = x.ASYCUDA_Id,
                            AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                            CustomsProcedure = x.CustomsProcedure,
                            DocumentType = x.DocumentType,
                            EmailId = x.EmailId,
                            ReferenceNumber = x.ReferenceNumber,
                            RegistrationDate = x.RegistrationDate,
                            Status = x.Status
                        })
                        .GroupBy(x => x.EmailId);
                    SubmitSalesToCustoms(lst);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        private static void SubmitSalesToCustoms(IEnumerable<IGrouping<int?, TODO_SubmitDiscrepanciesToCustoms>> lst)
        {
            try
            {


                Console.WriteLine("Submit Sales To Customs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 60;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs").Select(x => x.EmailAddress).ToArray();
                   var pdfs = new List<string>();
                    var RES = lst.SelectMany(x => x).DistinctBy(x => x.ASYCUDA_Id);
                    foreach (var itm in RES)
                    {

                       

                            var res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                            if (!res.Any())
                            {
                                BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id });
                                res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                            }
                            pdfs.AddRange(res);
                       

                        if (pdfs.Count == 0) continue;
                    }
                        var body = "The Following Sales Entries were Assessed. \r\n" +

                                   $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"AssessmentDate".FormatedSpace(20)}\r\n" +
                                   $"{RES.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.ReferenceNumber.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"\r\n" +
                                   $"Please open the attached email to view Email Thread.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";


                        var info = CurrentPOInfo(RES.First().AsycudaDocumentSetId.GetValueOrDefault()).FirstOrDefault();
                        var directory = info.Item2;

                        var summaryFile = Path.Combine(directory, $"SalesSummary.csv");
                       


                        var sumres =
                            new ExportToCSV<TODO_SubmitDiscrepanciesToCustoms,
                                List<TODO_SubmitDiscrepanciesToCustoms>>();
                        sumres.dataToPrint = RES.ToList();
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => sumres.SaveReport(summaryFile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        //if (RES.Key == null)
                        //{
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Assessed Ex-Warehoused Entries",
                                contacts, body, pdfs.ToArray());
                        //}
                        //else
                        //{
                        //    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Assessed Ex-Warehoused Entries", body, contacts, pdfs.ToArray());
                        //}

                        foreach (var item in RES)
                        {
                            var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                                x.ASYCUDA_Id == item.ASYCUDA_Id &&
                                x.ApplicationSettingsId == item.ApplicationSettingsId);
                            var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                                x.Attachments.FilePath == sfile.SourceFileName);
                            if (eAtt != null)
                            {
                                ctx.AttachmentLog.Add(new AttachmentLog(true)
                                {
                                    DocSetAttachment = eAtt.Id,
                                    Status = "Submit Sales To Customs",
                                    TrackingState = TrackingState.Added
                                });
                            }
                        }

                        ctx.SaveChanges();

                    

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        private static void SubmitIncompleteEntryData(FileTypes ft)
        {
            try
            {


                Console.WriteLine("Submit Incomplete Entry Data");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitIncompleteEntryData
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Invoices Total do not match Imported Totals . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Supplier Code".FormatedSpace(20)}{"Invoice Total".FormatedSpace(20)}{"Imported Total".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SupplierCode.FormatedSpace(20)}{current.InvoiceTotal.Value.ToString("C").FormatedSpace(20)}{current.ImportedTotal.Value.ToString("C").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Error:Incomplete Invoice Data",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Error:Incomplete Invoice Data", body, contacts, attlst.ToArray());
                        }


                        ctx.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitMissingInvoices(FileTypes ft)
        {
            try
            {


                Console.WriteLine("Submit Missing Invoices");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitDocSetWithIncompleteInvoices
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        if (GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices").Any()) continue;


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Invoices. {emailIds.FirstOrDefault().ImportedInvoices} were Imported out of {emailIds.FirstOrDefault().TotalInvoices} . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Invoice Date".FormatedSpace(20)}{"Invoice Value".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.InvoiceDate.ToShortDateString().FormatedSpace(20)}{current.InvoiceTotal.Value.ToString().FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Error:Missing Invoices",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key.EmailId), Client, "Error:Missing Invoices", body, contacts, attlst.ToArray());
                        }


                        ctx.SaveChanges();


                        //LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices");


                    }

                }
                SubmitMissingInvoicePDFs(ft);
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        private static void SubmitMissingInvoicePDFs(FileTypes ft)
        {
            try
            {


                Console.WriteLine("Submit Missing Invoice PDFs");


                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitMissingInvoicePDFs
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        if (GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs").Any()) continue;


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Invoice PDF Attachments. \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Source File".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SourceFile.FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please email CSV with Coresponding PDF to prevent this error.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Error:Missing Invoices PDF Attachments",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key.EmailId), Client, "Error:Missing Invoices PDF Attachments", body, contacts, attlst.ToArray());
                        }


                        ctx.SaveChanges();


                        //LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs");


                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitEntryCIF(FileTypes ft, FileInfo[] fs)
        {
            try
            {


                Console.WriteLine("Submit Shipment CIF");


                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitEntryCIF
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();
                    if (!lst.Any()) return;
                    if (GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitEntryCIF").Any()) return;

                   
                        


                        var body = $"Please see attached CIF for Shipment: {lst.First().Declarant_Reference_Number} . \r\n" +
                                 
                                   $"Please double check against your shipment rider.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                    var poInfo = CurrentPOInfo(ft.AsycudaDocumentSetId).FirstOrDefault();

                    var summaryFile = Path.Combine(poInfo.Item2, "CIFValues.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<TODO_SubmitEntryCIF, List<TODO_SubmitEntryCIF>>();
                    errRes.dataToPrint = lst;
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    attlst.Add(summaryFile);
                  

                    EmailDownloader.EmailDownloader.SendEmail(Client, "", $"CIF Values for Shipment: {lst.First().Declarant_Reference_Number}",
                                contacts, body, attlst.ToArray());
                     


                        LogDocSetAction(ft.AsycudaDocumentSetId, "SubmitEntryCIF");


                  

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void SubmitUnclassifiedItems(FileTypes ft)
        {
            
                
            try
            {
                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;




                using (var ctx = new CoreEntitiesContext())
                {

                    var emails = ctx.TODO_SubmitUnclassifiedItems
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.EmailId != null)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {

                        //if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;


                        var errorfile = Path.Combine(directory, $"UnclassifiedItems-{email.Key.EmailId}.csv");
                        var errors = email.Select(x => new UnClassifiedItem()
                        {
                            InvoiceNo = x.InvoiceNo,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode
                        }).ToList();


                        var res =
                            new ExportToCSV<UnClassifiedItem,
                                List<UnClassifiedItem>>();
                        res.dataToPrint = errors;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = ctx.Contacts.Where(x => x.Role == "Broker").ToList();
                        if (File.Exists(errorfile))
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(email.Key.EmailId), Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

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

        public static void SubmitDocSetUnclassifiedItems( FileTypes fileType)
        {

            try
            {
               
                using (var ctx = new CoreEntitiesContext())
                {

                    var emails = ctx.TODO_SubmitUnclassifiedItems
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                    && x.EmailId != null
                                    && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {

                        // if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;
                        var info = CurrentPOInfo(email.Key.AsycudaDocumentSetId).FirstOrDefault();
                        var directory = info.Item2;

                        var errorfile = Path.Combine(directory, $"UnclassifiedItems-{email.Key.EmailId}.csv");
                        var errors = email.Select(x => new UnClassifiedItem()
                        {
                            InvoiceNo = x.InvoiceNo,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode
                        }).ToList();


                        var res =
                            new ExportToCSV<UnClassifiedItem,
                                List<UnClassifiedItem>>();
                        res.dataToPrint = errors;
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = ctx.Contacts.Where(x => x.Role == "Broker").ToList();
                        if (File.Exists(errorfile))
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(email.Key.EmailId), Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

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

        public static void SubmitIncompleteSuppliers(FileTypes ft)
        {


            try
            {


                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;


                using (var ctx = new CoreEntitiesContext())
                {

                    var suppliers = ctx.TODO_SubmitIncompleteSupplierInfo
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .GroupBy(x => new {x.SupplierCode}).ToList();

                    if (!suppliers.Any()) return;

                    if (GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitIncompleteSuppliers").Any()) return;

                    var errorfile = Path.Combine(directory, $"IncompleteSuppliers.csv");
                    var errors = suppliers.SelectMany(x => x.Select(z => new IncompleteSupplier()
                    {
                        SupplierName = z.SupplierName,
                        SupplierCode = z.SupplierCode,
                        SupplierAddress = z.Street,
                        CountryCode = z.CountryCode
                    }).ToList()).ToList();


                    var res =
                        new ExportToCSV<IncompleteSupplier,
                            List<IncompleteSupplier>>();
                    res.dataToPrint = errors;
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    var contacts = new CoreEntitiesContext().Contacts
                        .Where(x => x.Role == "Broker" || x.Role == "PO Clerk").ToList();
                    if (File.Exists(errorfile))
                        EmailDownloader.EmailDownloader.SendEmail(Client, directory,
                            $"Error:InComplete Supplier Info", contacts.Select(x => x.EmailAddress).ToArray(),
                            "Please Fill out the attached Supplier Info and resend CSV...",
                            new string[] {errorfile});

                    //LogDocSetAction(sup.Key.AsycudaDocumentSetId, "SubmitIncompleteSuppliers");


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private static void SubmitInadequatePackages(FileTypes ft)
        {
            
                
            try
            {
                Console.WriteLine("Submit Inadequate Packages");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();

                    foreach (var docSet in lst)
                    {

                        if (GetDocSetActions(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages").Any()) continue;

                        var body = $"Error: Inadequate Lines\r\n" +
                                   $"\r\n" +
                                   $"The {docSet.Declarant_Reference_Number} has {docSet.TotalPackages} Packages with {docSet.TotalLines} Lines.\r\n" +
                                   $"Your Maximum Lines per Entry is {docSet.MaxEntryLines}.\r\n" +
                                   $"The minium required packages is {docSet.RequiredPackages}\r\n" +
                                   $"Please increase the Maxlines using \"MaxLines:\"\r\n" +
                                   $"Please note the System will automatically switch from \"Entry per Invoice\" to \"Group Invoices per Entry\", if there are not enough packages per invoice. \r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                        EmailDownloader.EmailDownloader.SendEmail(Client, "", $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());

                        //LogDocSetAction(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages");



                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void LogDocSetAction(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var action = ctx.Actions.FirstOrDefault(x => x.Name == actionName);
                if (action == null) throw new ApplicationException($"Action with name:{actionName} not found");
                ctx.ActionDocSetLogs.Add(new ActionDocSetLogs(true)
                {
                    ActonId = action.Id,
                    AsycudaDocumentSetId = docSetId,
                    TrackingState = TrackingState.Added
                });
                ctx.SaveChanges();
            }
        }

        private static List<ActionDocSetLogs> GetDocSetActions(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                return ctx.ActionDocSetLogs.Where(x => x.Actions.Name == actionName && x.AsycudaDocumentSetId == docSetId).ToList();

            }
        }

        private static void SubmitUnknownDFPComments()
        {
            try
            {

                Console.WriteLine("Submit UnknownDFP Comments");
                return;
                /// Fix up this
                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Clerk" || x.Role == "Broker").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId).ToList();

                    foreach (var docSet in lst)
                    {



                        var body = $"Error: UnknownDFP Comments\r\n" +
                                   $"\r\n" +
                                   $"The Following Comments Classification(DutyFree Or Duty Paid) are Unknown.\r\n" +
                                   $"{docSet.MaxEntryLines}.\r\n" +

                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();




                        EmailDownloader.EmailDownloader.SendEmail(Client, "", $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public static void AutoMatch()
        {
            Console.WriteLine("AutoMatch ...");
            new AdjustmentShortService()
                .AutoMatch(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
        }

        public static void CleanupEntries()
        {
            Console.WriteLine("Cleanup ...");
            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_DocumentsToDelete
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => new { x.ASYCUDA_Id, x.AsycudaDocumentSetId }).ToList();
                foreach (var itm in lst)
                {
                    using (var dtx = new DocumentDSContext())
                    {
                        var docEds = dtx.AsycudaDocumentEntryDatas.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).ToList();
                        foreach (var ed in docEds)
                        {
                            var docsetEd = dtx.AsycudaDocumentSetEntryDatas.FirstOrDefault(x =>
                                x.AsycudaDocumentSetId == itm.AsycudaDocumentSetId && x.EntryData_Id == ed.EntryData_Id);
                            if(docsetEd != null) dtx.AsycudaDocumentSetEntryDatas.Remove(docsetEd);
                        }

                        dtx.SaveChanges();
                    }

                    BaseDataModel.Instance.DeleteAsycudaDocument(itm.ASYCUDA_Id).Wait();
                }

                var doclst = ctx.TODO_DeleteDocumentSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.AsycudaDocumentSetId).ToList();


                foreach (var itm in doclst)
                {
                    BaseDataModel.Instance.DeleteAsycudaDocumentSet(itm).Wait();
                }

                // this wont work because i am saving entrydata in system documentsets

                //ctx.Database.ExecuteSqlCommand(@"delete from xcuda_ASYCUDA where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where AsycudaDocumentSetId is null)

                //                                delete from EntryData where EntryDataId not in (SELECT EntryDataId
                //                                FROM    AsycudaDocumentSetEntryData)");

            }
        }

        public static void EmailEntriesExpiringNextMonth()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "EntriesExpiringNextMonth.csv");

            using (var ctx = new CoreEntitiesContext())
            {
                var errors = ctx.TODO_EntriesExpiringNextMonth
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();

                SaveCSVReport(errors, errorfile);

                var contacts = ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Customs").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList();
                contacts.AddRange(ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Sales").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList());
                contacts = ctx.Contacts.Where(x => x.Role == "Developer").ToList();
                if (File.Exists(errorfile))
                {
                    var body = "The following entries are expiring within the next month. \r\n" +
                               $"Start Date: {DateTime.Now.ToString("yyyy-MM-dd")} End Date {DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd")}: \r\n" +
                               $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"Document Type".FormatedSpace(20)}{"RegistrationDate".FormatedSpace(20)}{"ExpiryDate".FormatedSpace(20)}\r\n" +
                               $"{errors.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.Reference.FormatedSpace(20)}{current.DocumentType.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)}{current.ExpiryDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                               $"\r\n" +
                               $"{Client.CompanyName} is kindly requesting these Entries be extended an additional 730 days to facilitate ex-warehousing. \r\n" +
                               $"\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Entries Expiring {DateTime.Now.ToString("yyyy-MM-dd")} - {DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[] { errorfile });
                }
            }

        }

        public static void EmailWarehouseErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "WarehouseErrors.csv");
            if (File.Exists(errorfile)) return;
            using (var ctx = new CoreEntitiesContext())
            {
                var errors = ctx.TODO_ERRReport_SubmitWarehouseErrors
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();

                var res = new ExportToCSV<TODO_ERRReport_SubmitWarehouseErrors, List<TODO_ERRReport_SubmitWarehouseErrors>>();
                res.IgnoreFields.AddRange(typeof(IIdentifiableEntity).GetProperties());
                res.IgnoreFields.AddRange(typeof(IEntityWithKey).GetProperties());
                res.IgnoreFields.AddRange(typeof(ITrackable).GetProperties());
                res.IgnoreFields.AddRange(typeof(Core.Common.Business.Entities.BaseEntity<TODO_ERRReport_SubmitWarehouseErrors>).GetProperties());
                res.IgnoreFields.AddRange(typeof(ITrackingCollection<TODO_ERRReport_SubmitWarehouseErrors>).GetProperties());
                res.dataToPrint = errors;
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

                var contacts = ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Customs").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList();
                contacts.AddRange(ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Sales").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList());
                contacts = ctx.Contacts.Where(x => x.Role == "Developer").ToList();
                if (File.Exists(errorfile))
                {
                    var body = "Attached are Issues that have been found on Assessed Entries that prevent Ex-warehousing. \r\n" +

                               $"{Client.CompanyName} is kindly requesting Technical Assistance in resolving these issues to facilitate Ex-Warehousing. \r\n" +
                               $"\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Warehouse Errors", contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[] { errorfile });
                }
            }

        }

        private static void SaveCSVReport<T>(List<T> errors, string errorfile) where T : class, IIdentifiableEntity, ITrackable, INotifyPropertyChanged
        {
            var res = new ExportToCSV<T, List<T>>();
            res.IgnoreFields.AddRange(typeof(IIdentifiableEntity).GetProperties());
            res.IgnoreFields.AddRange(typeof(IEntityWithKey).GetProperties());
            res.IgnoreFields.AddRange(typeof(ITrackable).GetProperties());
            res.IgnoreFields.AddRange(typeof(Core.Common.Business.Entities.BaseEntity<T>).GetProperties());
            res.IgnoreFields.AddRange(typeof(Core.Common.Business.Entities.BaseEntity<T>).GetGenericTypeDefinition().GetProperties());
            res.IgnoreFields.AddRange(typeof(ITrackingCollection<T>).GetProperties());
            res.dataToPrint = errors;
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
            }
        }

        public static void EmailSalesErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "SalesErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                .Where(x => x.Status != null)
                                .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>();
                res.dataToPrint = errors;
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Sales").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Sales Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile });
            }

        }
        public static void EmailAdjustmentErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "AdjustmentErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                    .Where(x => x.Type == "ADJ")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Status != null)
                    .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>();
                res.dataToPrint = errors;
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Sales").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Adjustment Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile });
            }

        }

        private static void EmailLatestPOEntries()
        {
            Console.WriteLine("Create Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.AsycudaDocumentSetExs.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                var contacts = ctx.Contacts.Where(x =>
                    x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                    && x.Role == "Broker").ToList();
                if (docset != null)
                {
                    EmailPOEntries(docset.AsycudaDocumentSetId, contacts);
                }
            }
        }

        public static void EmailPOEntries(int asycudaDocumentSetId, List<Contacts> contacts)
        {
            try
            {


                if (!contacts.Any() || BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true) return;
                //if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == asycudaDocumentSetId); //CurrentPOInfo();
                if (docSet == null) return;
                //foreach (var poInfo in lst.Where(x => x.Item1.AsycudaDocumentSetId == asycudaDocumentSetId))
                //{

                   // if (poInfo.Item1 == null) return;

                    var reference = docSet.Declarant_Reference_Number;
                    var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        reference);
                    if (!Directory.Exists(directory)) return;
                    var sourcefiles = Directory.GetFiles(directory, "*.xml");

                    var emailres = new FileInfo(Path.Combine(directory, "EmailResults.txt"));
                    var instructions = new FileInfo(Path.Combine(directory, "Instructions.txt"));
                    if (!instructions.Exists) return;
                    if (emailres.Exists) File.Delete(emailres.FullName);
                    Console.WriteLine("Emailing Po Entries");
                    string[] files;
                    if (File.Exists(emailres.FullName))
                    {
                        var eRes = File.ReadAllLines(emailres.FullName);
                        var insts = File.ReadAllLines(instructions.FullName);
                        files = sourcefiles.Where(x => insts.Any(z => z.Contains(x)) && !eRes.Any(z => z.Contains(x)))
                            .ToArray();
                    }
                    else
                    {
                        var insts = File.ReadAllLines(instructions.FullName);
                        files = sourcefiles.ToList().Where(x => insts.Any(z => z.Contains(x))).ToArray();
                    }

                    if (files.Length > 0)
                        EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Entries for {reference}",
                            contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", files);

                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void DownloadPDFs()
        {
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.TODO_ImportCompleteEntries
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                    var lst = entries

                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();
                    foreach (var doc in lst)
                    {
                        var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, doc.z.Declarant_Reference_Number)); ;
                        Console.WriteLine("Download PDF Files");
                        var lcont = 0;
                        while (ImportPDFComplete(directoryName, out lcont) == false)
                        {
                            RunSiKuLi(directoryName, "IM7-PDF", lcont.ToString());
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

        private static bool ImportPDFComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            if (File.Exists(Path.Combine(desFolder, "OverView-PDF.txt")))
            {
                var lines = File.ReadAllText(Path.Combine(directoryName, "OverView-PDF.txt"))
                    .Split(new[] { $"\r\n{DateTime.Now.Year}\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.StartsWith($"{DateTime.Now.Year}\t")
                        ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                        : line.Split('\t');
                    if (p.Length < 8) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}.pdf"))
                        && File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}-Assessment.pdf")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                return existingfiles != 0;
            }
            else
            {

                return false;
            }
        }


        private static void ReLinkPDFs()
        {
            Console.WriteLine("ReLink PDF Files");
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {
                    
                    var directoryName = StringExtensions.UpdateToCurrentUser(
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports"))//doc.z.Declarant_Reference_Number));
                    ;
                   
                    
                        

                        var csvFiles = new DirectoryInfo(directoryName).GetFiles($"*.pdf")
                            .Where(x => 
                                        //Regex.IsMatch(x.FullName,@".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",RegexOptions.IgnoreCase)&& 
                                        x.LastWriteTime.ToString("d") == DateTime.Today.ToString("d")).ToArray();

                        foreach (var file in csvFiles)
                        {
                            var mat = Regex.Match(file.FullName,
                                @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",
                                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                            if (!mat.Success) continue;

                            var dfile = ctx.Attachments.Include(x => x.AsycudaDocument_Attachments).FirstOrDefault(x => x.FilePath == file.FullName);

                        


                        var cnumber = mat.Groups["pCNumber"].Value;
                            var cdoc = ctx.AsycudaDocuments.FirstOrDefault(x => x.CNumber == cnumber);
                            if (cdoc == null) continue;
                            if (dfile != null && dfile.AsycudaDocument_Attachments.Any(x => x.AsycudaDocumentId == cdoc.ASYCUDA_Id)) continue;


                        ctx.AsycudaDocument_Attachments.Add(
                                new AsycudaDocument_Attachments(true)
                                {
                                    AsycudaDocumentId = cdoc.ASYCUDA_Id,
                                    Attachments = new Attachments(true)
                                    {
                                        FilePath = file.FullName,
                                        DocumentCode = "NA",
                                        Reference = file.Name.Replace(file.Extension, ""),
                                        TrackingState = TrackingState.Added

                                    },

                                    TrackingState = TrackingState.Added
                                });




                            ctx.SaveChanges();

                        }
                    }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void LinkPDFs()
        {
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.TODO_ImportCompleteEntries
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.AssessedAsycuda_Id)
                        .Distinct()
                        .ToList();
                    //var lst = entries
                    //    .GroupBy(x => x.AsycudaDocumentSetId)
                    //    .Join(ctx.AsycudaDocumentSetExs.Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();

                    BaseDataModel.LinkPDFs(entries);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        

        public static void LinkEmail()
        {
            try

            {
                Console.WriteLine("Link Emails");
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.TODO_ImportCompleteEntries
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                    var lst = entries
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();

                    foreach (var itm in entries)
                    {
                       
                            
                        
                        var idoc = ctx.AsycudaDocuments.First(x => x.ASYCUDA_Id == itm.AssessedAsycuda_Id);
                        var cdoc = ctx.AsycudaDocuments.First(x => x.ReferenceNumber == idoc.ReferenceNumber);

                        if (cdoc == null) continue;


                        //ctx.AsycudaDocument_Attachments.Add(
                        //    new AsycudaDocument_Attachments(true)
                        //    {
                        //        AsycudaDocumentId = cdoc.ASYCUDA_Id,
                        //        Attachments = new Attachments(true)
                        //        {
                        //            FilePath = file.FullName,
                        //            DocumentCode = "NA",
                        //            Reference = file.Name.Replace(file.Extension, ""),
                        //            TrackingState = TrackingState.Added

                        //        },

                        //        TrackingState = TrackingState.Added
                        //    });


                        

                         

                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ReDownloadSalesFiles()
        {
            //var directoryName = CurrentSalesInfo().Item4;
            //var overview = Path.Combine(directoryName, "OverView.txt");
            //if(File.Exists(overview)) File.Delete(overview);
            DownloadSalesFiles(3, "IM7History", true);
        }

        //public static void DownloadSalesFiles(bool redownload, string script)
        //{
        //    try

        //    {
        //        var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports")); //$@"{Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports")}\"; //BaseDataModel.CurrentSalesInfo().Item4;
        //        //var asycudaDocumentSetId =
        //        //    new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
        //        //        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
        //        //        x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ?? BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
        //        Console.WriteLine("Download Entries");
        //        var lcont = 0;
        //        while (ImportComplete(directoryName, redownload, out lcont) == false)
        //        {
        //            //RunSiKuLi(BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
        //            RunSiKuLi(directoryName, script, lcont.ToString());
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        public static void DownloadSalesFiles(int trytimes, string script, bool redownload = false)
        {
            try

            {
                var directoryName = $@"{Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports")}";
               // var directoryName = BaseDataModel.CurrentSalesInfo().Item4;//$@"{Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports")}\";
                Console.WriteLine("Download Entries");
                var lcont = 0;

                for (int i = 0; i < trytimes; i++)
                {
                    if (ImportComplete(directoryName, false, out lcont)) break;//ImportComplete(directoryName,false, out lcont);
                    RunSiKuLi(directoryName, script, lcont.ToString());
                    if (ImportComplete(directoryName, false, out lcont)) break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AssessEx9Entries()
        {
            Console.WriteLine("Assessing Ex9 Entries");
            var saleinfo = BaseDataModel.CurrentSalesInfo();
            AssessSalesEntry(saleinfo.Item3.Declarant_Reference_Number, saleinfo.Item3.AsycudaDocumentSetId);
        }

        public static void AssessDISEntries(string adjustmentType)
        {
            Console.WriteLine("Assessing Discrepancy Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_DiscrepanciesToAssess
                            .Where(x => x.Type == adjustmentType)
                            .ToList();
                foreach (var doc in res)
                {
                    if (doc.Declarant_Reference_Number == null) return;
                    var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number);
                   var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "InstructionResults.txt");
                    var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "Instructions.txt");

                    var lcont = 0;
                    while (AssessComplete(instrFile, resultsFile, out lcont) == false)
                    {
                       // RunSiKuLi(doc.AsycudaDocumentSetId, "AssessIM7", lcont.ToString());
                        RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
                    }
                }

            }

        }

        public static void AssessPOEntries(FileTypes ft)
        {
            
                
            Console.WriteLine("Assessing PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSetToAssess
                    .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                    .ToList();
                foreach (var doc in res)
                {
                    AssessPOEntry(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                }

            }

            SubmitAssessPOErrors(ft);

        }

        private static void SubmitAssessPOErrors(FileTypes ft)
        {

            
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSetToAssessErrors.Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                    .ToList();
                if (!res.Any()) return;
                Console.WriteLine("Emailing Assessment Errors - please check Mail");
                var docSet = ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                    .Select(x => x.EmailAddress).ToArray();
                var body =
                    $"Please see attached documents .\r\n" +
                    $"Please open the attached email to view Email Thread.\r\n" +
                    $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                    $"\r\n" +
                    $"Regards,\r\n" +
                    $"AutoBot";



                var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docSet.Declarant_Reference_Number);

                var summaryFile = Path.Combine(directory, "POAssesErrors.csv");
                if (File.Exists(summaryFile)) File.Delete(summaryFile);
                var errRes =
                    new ExportToCSV<TODO_PODocSetToAssessErrors, List<TODO_PODocSetToAssessErrors>>();
                errRes.dataToPrint = res;
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                        TaskCreationOptions.None, sta);
                }

               
               
                EmailDownloader.EmailDownloader.SendEmail(Client, directory,
                    $"PO Assessment Errors for Shipment: {docSet.Declarant_Reference_Number}",
                    poContacts, body, new string[]{summaryFile});

                    
               
            }
        }

        public static void AssessPOEntry(string docReference, int asycudaDocumentSetId)
        {
            try
            {


                if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId))
                {

                    return;
                }

                if (docReference == null) return;
                var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docReference);
                var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docReference, "InstructionResults.txt");
                var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docReference, "Instructions.txt");

                var lcont = 0;
                while (AssessComplete(instrFile, resultsFile, out lcont) == false)
                {
                    // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                    RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void AssessEntries(string docReference, int asycudaDocumentSetId)
        {
            

            if (docReference == null) return;
            var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference);
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "Instructions.txt");

            var lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
            }
        }




        public static void AssessSalesEntry(string docReference, int asycudaDocumentSetId)
        {
            if (docReference == null) return;
            var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference);
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "Instructions.txt");

            var lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
               // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
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
                        && (DateTime.Now - File.GetLastWriteTime(Path.Combine(directoryName, "LICOverView-PDF.txt"))).TotalMinutes > 60 ) return false;
                    if (File.Exists(Path.Combine(directoryName, $"{p[3]}-LIC.xml")) )
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
        public static bool ImportC71Complete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            var overviewFile = Path.Combine(desFolder, "C71OverView-PDF.txt");
            if (File.Exists(overviewFile))
            {


                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddMinutes(-10)) return false;
                var lines = File.ReadAllText(Path.Combine(directoryName, "C71OverView-PDF.txt"))
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
                    if (p.Length < 3) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[1]}-C71.xml")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                return existingfiles != 0;
            }
            else
            {

                return false;
            }
        }

        public static bool ImportComplete(string directoryName, bool redownload, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + (directoryName.EndsWith(@"\") ? "" : "\\");
            var overviewFile = Path.Combine(desFolder, "OverView.txt");
            if (File.Exists(overviewFile) || File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-2))
            {
                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-1)) return false;
                var lines = File.ReadAllText(overviewFile)
                    .Split(new[] { $"\r\n{DateTime.Now.Year}\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.StartsWith($"{DateTime.Now.Year}\t")
                        ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                        : line.Split('\t');
                    if (p.Length < 8) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}.xml")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                if (redownload && (DateTime.Now - new FileInfo(overviewFile).LastWriteTime).Minutes > 5) return false;
                return existingfiles != 0;
            }
            else
            {

                return false;
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
                        if (lcont >= res.Length) 
                        {
                            if ((res.Length/2) == (instructions.Length/3)) return true; else return false;
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

                            if ( r.Length == 3 && p[1] == r[1] && r[2] == "Success") // for file
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

        public static bool AssessC71Complete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;


            if (File.Exists(instrFile))
            {
                if (!File.Exists(resultsFile)) return false;
                var lines = File.ReadAllLines(instrFile);
                var res = File.ReadAllLines(resultsFile);
                if (res.Length == 0)
                {

                    return false;
                }


                foreach (var line in lines)
                {
                    var p = line.Split('\t');
                    if (lcont >= res.Length) return false;
                    if (string.IsNullOrEmpty(res[lcont])) return false;
                    var r = res[lcont].Split('\t');
                    lcont += 1;
                    if (p[1] == r[1] && r.Length == 5 && r[4] == "Success")
                    {
                        continue;
                    }
                    return false;
                }

                return true;
            }
            else
            {

                return true;
            }
        }
        public static bool AssessComplete(string instrFile, string resultsFile, out int lcont)
        {
            try
            {


                lcont = 0;
                var rcount = 0;

                if (File.Exists(instrFile))
                {
                    if (!File.Exists(resultsFile)) return false;
                    var lines = File.ReadAllLines(instrFile).Where(x => x.StartsWith("File\t")).ToArray();
                    var res = File.ReadAllLines(resultsFile).Where(x => x.StartsWith("File\t")).ToArray();
                    if (res.Length == 0)
                    {

                        return false;
                    }


                    foreach (var line in lines)
                    {
                        var p = line.Split('\t');
                        if (lcont >= res.Length) return false;
                        if (string.IsNullOrEmpty(res[lcont])) return false;
                        rcount += 1;
                        var isSuccess = false;
                        foreach (var rline in res)
                        {
                            var r = rline.Split('\t');

                            if ( r.Length == 3 && p[1] == r[1] && r[2] == "Success")
                            {
                               if(r[0] == "File") lcont = rcount-1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] == r[0] && r[2] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if ( r.Length == 4  && p[2] == r[2] && r[3] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] != r[1] && r[2] == "Error")
                            {

                                if (r[0] == "Screenshot")
                                {

                                    SubmitScriptErrors(r[1]);
                                    return true;
                                }
                                //isSuccess = true;
                                //break;
                            }

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Error")
                            {
                                // email error
                                //if (r[0] == "File") lcont = rcount - 1;
                                //isSuccess = true;
                                //break;
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
            catch (Exception)
            {

                throw;
            }
        }

        public static void ImportSalesEntries()
        {
            try
            {
                Console.WriteLine("Import Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    //var docSetId = BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                    var docSetId = ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                                                                 x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ?? BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;

                    var lastdbfile =
                        ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == docSetId);
                    var lastfiledate = lastdbfile != null?File.GetCreationTime(lastdbfile.Attachments.FilePath):DateTime.Today.AddDays(-1);
                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.Type == "XML" && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                            .Declarant_Reference_Number);
                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                        .Where(x => x.LastWriteTime.Date >= lastfiledate)
                        .ToArray();
                    if (csvFiles.Length > 0)
                        BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();
                    RemoveDuplicateEntries();
                    FixIncompleteEntries();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ImportAllSalesEntries()
        {
            try
            {
                Console.WriteLine("Import Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    //var docSetId = BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                    var docSetId = ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                                                                 x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ?? BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;

                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.Type == "XML" && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                            .Declarant_Reference_Number);
                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                        .ToArray();
                    if (csvFiles.Length > 0)
                        BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();
                    RemoveDuplicateEntries();
                    FixIncompleteEntries();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ImportC71(FileTypes ft)
        {
            
                
            Console.WriteLine("Import C71");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var docSets = ctx.TODO_C71ToCreate.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                    .ToList();
                foreach (var poInfo in docSets)
                {
                    ImportC71(poInfo.Declarant_Reference_Number, poInfo.AsycudaDocumentSetId);

                }
            }
        }


        public static void ReImportC71()
        {
            Console.WriteLine("Export Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var docset =
                    ctx.TODO_PODocSet.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    ImportC71(docset.Declarant_Reference_Number, docset.AsycudaDocumentSetId);
                }
            }
        }

        public static void ReImportLIC()
        {
            Console.WriteLine("Export Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var docset =
                    ctx.TODO_PODocSet.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    ImportLicense(docset.Declarant_Reference_Number, docset.AsycudaDocumentSetId);
                }
            }
        }

        private static bool ImportC71(string declarant_Reference_Number, int asycudaDocumentSetId)
        {
            try
            {

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    //var reference = declarant_Reference_Number;
                    //var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    //    reference);
                    //if (!Directory.Exists(directory)) return false;

                    var lastdbfile =
                        ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId && x.FileTypes.Type == "C71");
                    var lastfiledate = lastdbfile != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1);


                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.Type == "C71" && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return true;
                    //var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == asycudaDocumentSetId).Declarant_Reference_Number);
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports", "C71");
                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => x.LastWriteTime >= lastfiledate)
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase) && x.Name != "C71.xml")
                        .ToList();
                    //if (lastC71 == null) return false;
                    //var csvFiles = new FileInfo[] {lastC71};

                    if (csvFiles.Any())
                    {
                        BaseDataModel.Instance.ImportC71(asycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList());
                        ft.AsycudaDocumentSetId = asycudaDocumentSetId;
                        //BaseDataModel.Instance.SaveAttachedDocuments(csvFiles, ft).Wait();
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportLicense(FileTypes ft)
        {
            Console.WriteLine("Import License");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;
                var docSets = ctx.TODO_LICToCreate.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => ft.AsycudaDocumentSetId == 0 || x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                    .ToList();
                
                foreach (var poInfo in docSets)
                {
                    ImportLicense(poInfo.Declarant_Reference_Number, poInfo.AsycudaDocumentSetId);
                }
            }

        }

        private static bool ImportLicense(string declarant_Reference_Number,int asycudaDocumentSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;

                var reference = declarant_Reference_Number;
                var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    reference);
                if (!Directory.Exists(directory)) return false;

                var lastdbfile =
                    ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId && x.FileTypes.Type == "LIC");
                var lastfiledate = lastdbfile  != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1);

                var ft = ctx.FileTypes.FirstOrDefault(x =>
                    x.Type == "LIC" && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (ft == null) return true;
                //var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == asycudaDocumentSetId).Declarant_Reference_Number);
                var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports", "LIC");
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

                BaseDataModel.Instance.ImportLicense(asycudaDocumentSetId,
                    csvFiles.Select(x => x.FullName).ToList());
                ft.AsycudaDocumentSetId = asycudaDocumentSetId;
                BaseDataModel.Instance.SaveAttachedDocuments(csvFiles.ToArray(), ft).Wait();
                return false;
            }
        }

        public static void CreateEx9(bool overwrite)
        {
            try
            {


                Console.WriteLine("Create Ex9");

                var saleInfo = BaseDataModel.CurrentSalesInfo();
                if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;

                var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;
                if (overwrite)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
                    //BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0); don't overwrite previous entries
                }

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var str = $@"SELECT EX9AsycudaSalesAllocations.ItemNumber
                    FROM    EX9AsycudaSalesAllocations INNER JOIN
                                     ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                                     EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                                     AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
                    WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                                     (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                                     EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                                     (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) AND (EX9AsycudaSalesAllocations.CustomsOperationId = {(int) CustomsOperations.Warehouse}) AND (AllocationErrors.ItemNumber IS NULL) 
                          AND (ApplicationSettings.ApplicationSettingsId = {
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        }) AND (EX9AsycudaSalesAllocations.InvoiceDate >= '{
                            saleInfo.Item1.ToShortDateString()
                        }') AND 
                                     (EX9AsycudaSalesAllocations.InvoiceDate <= '{saleInfo.Item2.ToShortDateString()}')
                    GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId--, EX9AsycudaSalesAllocations.pQuantity, EX9AsycudaSalesAllocations.PreviousItem_Id
                    HAVING (SUM(EX9AsycudaSalesAllocations.PiQuantity) < SUM(EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL)";

                    var res = ctx.Database.SqlQuery<string>(str);
                    if (!res.Any()) return;
                }


                var filterExpression =
                    $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                    $"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                    $" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                    //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                    "&& ( TaxAmount == 0 ||  TaxAmount != 0)" +
                    "&& PreviousItem_Id != null" +
                    "&& (xBond_Item_Id == 0 )" +
                    "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                    "&& (PiQuantity < pQtyAllocated)" +
                    "&& (Status == null || Status == \"\")" +
                    (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                        ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                        : "") +
                    ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");



                AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, true, docset, "Sales", "Historic", true, true, true,true,true,false,true,true, true).Wait();
                //await CreateDutyFreePaidDocument(dfp, res, docSet, "Sales", true,
                //        itemSalesPiSummarylst.Where(x => x.DutyFreePaid == dfp || x.DutyFreePaid == "All")
                //            .ToList(), true, true, true, "Historic", true, ApplyCurrentChecks,
                //        true, false, true)
                //    .ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void Ex9AllAllocatedSales(bool overwrite)
        {
            try
            {


                Console.WriteLine("Ex9 All Allocated Sales");

                //var saleInfo = BaseDataModel.CurrentSalesInfo();
                //if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;
                

                    var fdocSet =
                        new CoreEntitiesContext().AsycudaDocumentSetExs.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .First();

                var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(fdocSet.AsycudaDocumentSetId).Result;
                if (overwrite)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0);// don't overwrite previous entries
                }



                var filterExpression =
                    $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                    $"&& (InvoiceDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate:MM/01/yyyy}\" " +
                    $" && InvoiceDate <= \"{DateTime.Now:MM/dd/yyyy HH:mm:ss}\")" +
                    //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                    "&& (TaxAmount == 0 || TaxAmount != 0)" +
                   // "&& (ItemNumber == \"WA99004\")" +//A002416,A002402,X35019044,AB111510
                   // "&&  PreviousItem_Id == 388376" +
                    "&& PreviousItem_Id != null" +
                    //"&& (xBond_Item_Id == 0)" + not relevant because it could be assigned to another sale but not exwarehoused
                    "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                    "&& (PiQuantity < pQtyAllocated)" +
                    //"&& (pQuantity - pQtyAllocated  < 0.001)" + // prevents spill over allocations
                    "&& (Status == null || Status == \"\")" +
                    (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                        ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                        : "") +
                    ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");



                AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, false, docset, "Sales", "Historic", true, true,true,false,false, false, true, true, true).Wait();
                //await CreateDutyFreePaidDocument(dfp, res, docSet, "Sales", true,
                //        itemSalesPiSummarylst.Where(x => x.DutyFreePaid == dfp || x.DutyFreePaid == "All")
                //            .ToList(), true, true, true, "Historic", true, ApplyCurrentChecks,
                //        true, false, true)
                //    .ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        public static void CleanupDiscpancies()
        {
            try
            {
                Console.WriteLine("Clean up Discrepancies");

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.TODO_DiscrepanciesAlreadyXMLed.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                        //    && x.AsycudaDocumentSetId == 1462
                        //     && x.InvoiceNo == "53371108"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        .Select(x => new { x.AsycudaDocumentSetId, x.EntryData_Id }).Distinct().ToList();

                    using (var etx = new EntryDataDSContext())
                    {
                        var res = etx.EntryDataDetailsEx
                            .Where(x => x.DoNotAllocate == true || x.IsReconciled == true)
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .Select(x => new {x.AsycudaDocumentSetId, x.EntryData_Id }).Distinct().ToList();

                        lst.AddRange(res);
                    }

                    using (var dtx = new DocumentDSContext())
                    {


                        foreach (var doc in lst)
                        {
                            var res = dtx.AsycudaDocumentSetEntryDatas
                                .FirstOrDefault(x =>
                                   x.AsycudaDocumentSet.SystemDocumentSet == null
                                 && x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId
                                && x.EntryData_Id == doc.EntryData_Id);
                            if (res != null)
                                dtx.AsycudaDocumentSetEntryDatas.Remove(res);

                        }



                        dtx.SaveChanges();
                    }

                    var docsToDelete = ctx.TODO_DeleteDocumentSet.ToList();
                    foreach (var doc in docsToDelete)
                    {
                        BaseDataModel.Instance.DeleteAsycudaDocumentSet(doc.AsycudaDocumentSetId).Wait();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void CleanupDocSetDiscpancies(FileTypes fileType)
        {
            try
            {
                Console.WriteLine("Clean up Discrepancies");

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var disLst = ctx.TODO_DiscrepanciesAlreadyXMLed.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                            //     && x.InvoiceNo == "53371108"
                            //&& x.InvoiceDate >= saleInfo.Item1
                            //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        .GroupBy(x => new {x.AsycudaDocumentSetId, x.EntryData_Id, x.InvoiceNo})
                        .ToList();
                    foreach (var g in disLst)
                    {
                        var body = "Discrepancy Already Executed: \r\n" +
                                   "The following were already executed: \r\n" +
                                   $"\t{"Item Number".FormatedSpace(20)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                                   $"{g.Select(current => $"\t{current.ItemNumber.FormatedSpace(20)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.pCNumber.FormatedSpace(15)}{current.Comment.FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        EmailDownloader.EmailDownloader.SendBackMsg(Convert.ToInt32(fileType.EmailId), Client,
                            body);



                        var entryLst = new EntryDataDSContext().EntryDataDetailsEx
                            //.Where(x => x.DoNotAllocate == true || x.IsReconciled == true)
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                                        && x.EntryData_Id == g.Key.EntryData_Id)
                            .ToList();
                        if (entryLst.Count() != g.Count()) return;

                        using (var dtx = new DocumentDSContext())
                        {
                            var res = Queryable.FirstOrDefault(dtx.AsycudaDocumentSetEntryDatas, x =>
                                x.AsycudaDocumentSet.SystemDocumentSet == null
                                && x.AsycudaDocumentSetId == g.Key.AsycudaDocumentSetId
                                && x.EntryData_Id == g.Key.EntryData_Id);
                            if (res != null)
                                dtx.AsycudaDocumentSetEntryDatas.Remove(res);

                            dtx.SaveChanges();
                        }

                        //var docsToDelete = ctx.TODO_DeleteDocumentSet.ToList();
                        //foreach (var doc in docsToDelete)
                        //{

                        BaseDataModel.Instance.DeleteAsycudaDocumentSet(g.Key.AsycudaDocumentSetId).Wait();
                        // }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void ClearDocSetEntryData(int asycudaDocumentSetId)
        {
            using (var dtx = new DocumentDSContext())
            {
                var res = dtx.AsycudaDocumentSetEntryDatas.Where(x =>
                    x.AsycudaDocumentSet.SystemDocumentSet == null
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (res.Any())
                {
                    dtx.AsycudaDocumentSetEntryDatas.RemoveRange(res);
                    dtx.SaveChanges();
                }

            }
        }

        private static void ClearDocSetAttachments(int asycudaDocumentSetId, int? emailId = null)
        {
            using (var dtx = new DocumentDSContext())
            {
                var res = dtx.AsycudaDocumentSet_Attachments.Where(x =>
                    x.AsycudaDocumentSet.SystemDocumentSet == null
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId && emailId == null ? true : x.EmailUniqueId != emailId);
                if (res.Any())
                {
                    dtx.AsycudaDocumentSet_Attachments.RemoveRange(res);
                    dtx.SaveChanges();
                }

            }
        }

        public static void ClearAllAdjustmentEntries(string adjustmentType)
        {
            Console.WriteLine($"Clear {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        && x.Type == adjustmentType)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                }

            }
        }


        public static void ClearDocSetEntries(FileTypes fileType)
        {
            
            Console.WriteLine($"Clear {fileType.Type} Entries");

            // var saleInfo = CurrentSalesInfo();
            //BaseDataModel.Instance.ClearAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Wait();
            //BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId, 0);

            ClearDocSetEntryData(fileType.AsycudaDocumentSetId);
            ClearDocSetAttachments(fileType.AsycudaDocumentSetId, fileType.EmailId == null? null :(int?) Convert.ToInt32(fileType.EmailId) );
            var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Result;
            string directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                                        docSet.Declarant_Reference_Number);
            var instFile = Path.Combine(directoryName, "Instructions.txt");
            var resFile = Path.Combine(directoryName, "InstructionResults.txt");
            if (File.Exists(instFile)) File.Delete(instFile);
            if (File.Exists(resFile)) File.Delete(resFile);
        }



        public static void ClearPOEntries()
        {
            Console.WriteLine("Clear PO Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_PODocSetToExport.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                }

            }
        }

        public static void CreateAdjustmentEntries(bool overwrite, string adjustmentType, FileTypes fileType)
        {
            Console.WriteLine($"Create {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_AdjustmentsToXML>(
                            $"select * from [TODO-AdjustmentsToXML]  where AsycudaDocumentSetId = {fileType.AsycudaDocumentSetId}" +
                            $"and AdjustmentType = '{adjustmentType}'")//because emails combined// and EmailId = '{fileType.EmailId}'
                        .ToList()
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .ToList();

                    CreateAdjustmentEntries(overwrite, adjustmentType, lst, fileType.EmailId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void RecreateDocSetDiscrepanciesEntries(FileTypes fileType)
        {
            CreateAdjustmentEntries(false,"DIS", fileType);
        }

        public static void CreateAdjustmentEntries(bool overwrite, string adjustmentType)
        {
            Console.WriteLine($"Create {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_AdjustmentsToXML>(
                            $"select * from [TODO-AdjustmentsToXML]  where ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}" +
                            $"and AdjustmentType = '{adjustmentType}'")
                        .ToList()
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .ToList();

                   CreateAdjustmentEntries(overwrite, adjustmentType, lst, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static  void CreateAdjustmentEntries(bool overwrite, string adjustmentType,
            List<IGrouping<int, TODO_AdjustmentsToXML>> lst, string emailId)
        {
            if (overwrite)
            {
                foreach (var doc in lst.Select(x => x.Key).Distinct<int>())
                {

                    BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                }
            }

            foreach (var doc in lst)
            {
                try
                {
                    // do duty Paid
                    var itemFilter = $" && ({Enumerable.Select<TODO_AdjustmentsToXML, string>(doc, x => $"EntryDataDetailsId == {x.EntryDataDetailsId.ToString()}").Distinct().Aggregate((old, current) => old + " || " + current)})";
                    var entryDataDetailsIds = doc.Select(x => x.EntryDataDetailsId).ToList();
                    var t1 = Task.Run(() =>
                    {


                        var filterExpressionp =
                                $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")"

                                //$"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                                //$" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                                //+ $" && EntryDataId == \"{doc.InvoiceNo}\""
                                + $" && DutyFreePaid == \"Duty Paid\""
                                + itemFilter
                            ;



                        new AdjustmentShortService().CreateIM9(filterExpressionp, false,
                            doc.Key, "Duty Paid", adjustmentType, entryDataDetailsIds, emailId).Wait();

                    });
                        var  filterExpressionf =
                        $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")"

                        //$"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                        //$" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                        // + $" && EntryDataId == \"ADJ-June 2019\""
                        //+ $" && ItemNumber == \"7IT/PR-LBL-RBN\""
                        + $" && DutyFreePaid == \"Duty Free\""
                        + itemFilter
                        // + $" && \"{Enumerable.Select<TODO_AdjustmentsToXML, string>(doc, x => x.InvoiceNo).Distinct().Aggregate((old, current) => old + "," + current)}\".Contains(EntryDataId)"
                        ;
                    var t2 = Task.Run(() =>
                    {
                        new AdjustmentShortService().CreateIM9(filterExpressionf, false, 
                            doc.Key, "Duty Free", adjustmentType, entryDataDetailsIds, emailId).Wait();
                    });
                    var t3 = Task.Run(() =>
                    {
                        new AdjustmentOverService()
                            .CreateOPS(filterExpressionf, false, doc.Key, adjustmentType, entryDataDetailsIds, emailId)
                            .Wait();
                    });
                    if (entryDataDetailsIds.Count > 7)
                    {
                        Task.WhenAll(t1,  t3).Wait();
                       // t1.Wait();
                        t2.Wait();
                       // t3.Wait();
                    }
                    else
                    {
                        Task.WhenAll(t1, t2, t3).Wait();
                    }
                    

                    BaseDataModel.RenameDuplicateDocuments(doc.Key);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public static void ExportDiscpancyEntries(string adjustmentType)
        {
            Console.WriteLine($"Export Last {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.TODO_DiscrepanciesToAssess.AsNoTracking()
                        .Where(x => x.Type == adjustmentType
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )
                        .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                    foreach (var doc in lst)
                    {

                        string directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                                        doc.Key.Declarant_Reference_Number);
                     
                        BaseDataModel.Instance.ExportLastDocumentInDocSet(doc.Key.AsycudaDocumentSetId,
                            directoryName, true).Wait();
                        ExportLastDocSetSalesReport(doc.Key.AsycudaDocumentSetId,
                            directoryName).Wait();

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ExportDocSetDiscpancyEntries(string adjustmentType, FileTypes fileType)
        {
            Console.WriteLine($"Export Last {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.TODO_DiscrepanciesToAssess.AsNoTracking()
                        .Where(x => x.Type == adjustmentType
                                && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                            //&& x.InvoiceDate >= saleInfo.Item1
                            //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )
                        .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                    foreach (var doc in lst)
                    {
                        var docSetEx = ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == doc.Key.AsycudaDocumentSetId);
                        if (docSetEx.ClassifiedLines != docSetEx.TotalLines) continue;

                        BaseDataModel.Instance.ExportLastDocumentInDocSet(doc.Key.AsycudaDocumentSetId,
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                doc.Key.Declarant_Reference_Number), true).Wait();
                        ExportLastDocSetSalesReport(doc.Key.AsycudaDocumentSetId,
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                doc.Key.Declarant_Reference_Number)).Wait();

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AssessDiscpancyEntries()
        {
            Console.WriteLine("Assess Discrepancy Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_CreateDiscrepancyEntries.Where(x =>
                    x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                    && x.AdjustmentType == "DIS"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                foreach (var doc in lst)
                {

                    AssessPOEntry(doc.Key.Declarant_Reference_Number, doc.Key.AsycudaDocumentSetId);
                }

            }

        }

        public static void AssessDiscpancyEntries(FileTypes ft, FileInfo[] fs)
        {
            Console.WriteLine("Assess Discrepancy Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_AssessDiscrepancyEntries.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                            && x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId
                            && x.AdjustmentType == "DIS"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                    )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                foreach (var doc in lst)
                {

                    AssessEntries(doc.Key.Declarant_Reference_Number, doc.Key.AsycudaDocumentSetId);
                }

            }

        } 


        public static void SubmitScriptErrors(string file)
        {
            try
            {


                Console.WriteLine("Submit Script Errors");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var contacts = ctx.Contacts.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                                            && x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                   

                            var body = $"Please see attached.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                     
                    var msg = EmailDownloader.EmailDownloader.CreateMessage(Client, "AutoBot Script Error", contacts, body, new string[]{file});
                    EmailDownloader.EmailDownloader.SendEmail(Client, msg);
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }




        public static void SubmitDocSetDiscrepancyErrors(FileTypes fileType)
        {
            try
            {


                Console.WriteLine("Submit Discrepancy Entries");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.TODO_DiscrepanciesErrors.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                                && x.Type == "DIS").ToList()
                        .GroupBy(x => new { x.AsycudaDocumentSetId, x.EmailId });
                    foreach (var doc in lst)
                    {
                        var emailIds = ctx.AsycudaDocumentSet_Attachments
                            .Where(x => x.AsycudaDocumentSetId == doc.Key.AsycudaDocumentSetId &&
                                        x.Attachments.FilePath.EndsWith(".xlsx"))
                            .Where(x => x.AttachmentLog.All(z => z.Status != "Submit Discrepancy Errors"))
                            .ToList();
                        foreach (var eId in emailIds)
                        {

                            var body = "Discrepancies Found: \r\n" +
                                       "System could not Generate Entries the following items on the CNumbers Stated: \r\n" +
                                       $"\t{"Item Number".FormatedSpace(20)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                                       $"{doc.Select(current => $"\t{current.ItemNumber.FormatedSpace(20)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.PreviousCNumber.FormatedSpace(15)}{current.Status.FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
                                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                            if (!EmailDownloader.EmailDownloader.SendBackMsg(Convert.ToInt32(eId.EmailUniqueId), Client,
                                body)) continue;
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = eId.Id,
                                Status = "Submit Discrepancy Errors",
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();


                        }


                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public static void SetCurrentApplicationSettings(int id)
        {
            using (var ctx = new CoreEntitiesContext() { })
            {


                var appSetting = ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes)
                    .Include("FileTypes.FileTypeContacts.Contacts")
                    .Include("FileTypes.FileTypeActions.Actions")
                    .Include(x => x.EmailMapping)
                    .Include("FileTypes.FileTypeMappings")
                    .Where(x => x.IsActive)
                    .First(x => x.ApplicationSettingsId == id);

                // set BaseDataModel CurrentAppSettings
                BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                //check emails

            }
        }



        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo()
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var poDocSet = ctx.TODO_PODocSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                    if (poDocSet.Any())
                    {
                        var itmlst = poDocSet.Select(x => x.AsycudaDocumentSetId).ToList();
                        var docSet = ctx.AsycudaDocumentSetExs.Where(x =>
                            itmlst.Any(z => z == x.AsycudaDocumentSetId)).ToList();
                        var lst = new List<Tuple<AsycudaDocumentSetEx, string>>();
                        foreach (var item in docSet)
                        {

                            var dirPath = StringExtensions.UpdateToCurrentUser(Path.Combine(
                                BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                item.Declarant_Reference_Number));
                            lst.Add(new Tuple<AsycudaDocumentSetEx, string>(item, dirPath));
                        }

                        return lst;
                    }

                    return new List<Tuple<AsycudaDocumentSetEx, string>>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo(int docSetId)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var docSet = ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId);
                    var dirPath = StringExtensions.UpdateToCurrentUser(Path.Combine(
                        BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        docSet.Declarant_Reference_Number));
                    return new List<Tuple<AsycudaDocumentSetEx, string>>(){ new Tuple<AsycudaDocumentSetEx, string>(docSet, dirPath)};
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentLICInfo()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var poDocSet = ctx.LicenceSummary.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Select(x => x.AsycudaDocumentSetId).Distinct().ToList();

                var docSet = ctx.AsycudaDocumentSetExs.Where(x => poDocSet.Any(z => z == x.AsycudaDocumentSetId)
                                                            && x.AsycudaDocumentSet_Attachments.All(z => z.FileTypes.Type != "LIC"));
                var lst = new List<Tuple<AsycudaDocumentSetEx, string>>();
                foreach (var item in docSet)
                {

                    var dirPath = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        item.Declarant_Reference_Number));
                    lst.Add(new Tuple<AsycudaDocumentSetEx, string>(item, dirPath));
                }
                return lst;


            }
        }

        public static void AllocateSales()
        {
            Console.WriteLine("Allocations Started");
            using (var ctx = new CoreEntitiesContext())
            {
                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true
                    && !ctx.TODO_UnallocatedSales.Any(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)) return;
                AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings
                    .ApplicationSettingsId).Wait();
                AllocationsBaseModel.Instance
                    .AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false)
                    .Wait();
                EmailSalesErrors();
            }
        }

        public static void RebuildSalesReport()
        {
            Console.WriteLine("Allocations Started");
            BuildSalesReportClass.Instance.ReBuildSalesReports();
        }


        public static void AllocateDocSetDiscrepancies(FileTypes fileType)
        {
            try
            {
                Console.WriteLine("Allocate DocSet Discrepancies");
                    List<KeyValuePair<int, string>> lst;
                    using (var ctx = new CoreEntitiesContext())
                    {
                        ctx.Database.CommandTimeout = 0;
                        
                       
                       
                        lst = new AdjustmentQSContext()
                            .AdjustmentDetails
                            .Where(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId 
                                        && x.Type == "DIS")
                            .Select(x => new {x.EntryDataDetailsId, x.ItemNumber})
                            .Distinct()
                            .ToList()
                            .Select(x => new KeyValuePair<int,string> (x.EntryDataDetailsId, x.ItemNumber))
                            .ToList();
                    }

                if (!lst.Any()) return;
                AllocationsModel.Instance.ClearDocSetAllocations(lst.Select(x => $"'{x.Value}'").Aggregate((o, n) => $"{o},{n}")).Wait();

                AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);

             

                


                new AdjustmentShortService()
                        .AutoMatchDocSet(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            fileType.AsycudaDocumentSetId).Wait();

                new AdjustmentShortService()
                    .ProcessDISErrorsForAllocation(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        lst.Select(x => $"{x.Key.ToString()}-{x.Value}").Aggregate((o, n) => $"{o},{n}")).Wait();

                new AllocationsBaseModel()
                    .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false,
                        lst.Select(x => $"{x.Key.ToString()}-{x.Value}").Aggregate((o,n) => $"{o},{n}")).Wait();

                new AllocationsBaseModel()
                    .MarkErrors(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void AllocateShorts()
        {
            try
            {
                return;
                Console.WriteLine("Short Allocations Started");
                using (var ctx = new CoreEntitiesContext())
                {
                    if (!ctx.TODO_UnallocatedShorts.Any(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)) return;
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings
                        .ApplicationSettingsId).Wait();
                    AllocationsBaseModel.Instance
                        .AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false)
                        .Wait();
                    EmailAdjustmentErrors();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void RecreatePOEntries(int asycudaDocumentSetId)
        {

            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                    {
                        if (ctx.TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;

                    }
                    Console.WriteLine("RecreatePOEntries");
                    var res = ctx.ToDo_POToXML.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == asycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new
                        {
                            DocSetId = x.Key,
                            Entrylst = x.Select(z =>  z.EntryDataDetailsId ).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        CreatePOEntries(docSetId.DocSetId, docSetId.Entrylst);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void CreatePOEntries(int docSetId, List<int> entrylst)
        {
            BaseDataModel.Instance.ClearAsycudaDocumentSet((int) docSetId).Wait();
            BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docSetId, 0);

            var po = CurrentPOInfo(docSetId).FirstOrDefault();
            var insts = Path.Combine(po.Item2, "InstructionResults.txt");
            if(File.Exists(insts)) File.Delete(insts);

            //BaseDataModel.Instance.AddToEntry(
            //        docSetId.Entrylst.Where(x => x.IsClassified == true).Select(x => x.EntryDataDetailsId),
            //        docSetId.DocSetId,
            //        BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true).Wait();
            //BaseDataModel.Instance.AddToEntry(
            //        docSetId.Entrylst.Where(x => x.IsClassified == false).Select(x => x.EntryDataDetailsId),
            //        docSetId.DocSetId,
            //        BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true)
            //    .Wait();


            BaseDataModel.Instance.AddToEntry(entrylst,docSetId,
                (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true), true, false).Wait();
        }

        public static void ExportPOEntries(int asycudaDocumentSetId)
        {


            using (var ctx = new DocumentDSContext())
            {
                try
                {
                    IQueryable<xcuda_ASYCUDA> docs;
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true)
                    {
                        if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                            x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                        Console.WriteLine("Export PO Entries");
                        docs = ctx.xcuda_ASYCUDA
                            .Include(x => x.xcuda_Declarant)

                            .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                        asycudaDocumentSetId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                        && (x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsOperationId ==
                                            (int) CustomsOperations.Import
                                            || x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                                .CustomsOperationId == (int) CustomsOperations.Warehouse));
                    }
                    else
                    {


                        docs = ctx.xcuda_ASYCUDA
                            .Include(x => x.xcuda_Declarant)
                            .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                        asycudaDocumentSetId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                        && (x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsOperationId ==
                                            (int) CustomsOperations.Import
                                            || x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                                .CustomsOperationId == (int) CustomsOperations.Warehouse));
                    }

                    var res = docs.GroupBy(x => new
                        {
                            x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId,
                            ReferenceNumber = x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                .Declarant_Reference_Number
                        }).Select(x => new
                        {
                            DocSet = x.Key,
                            Entrylst = x.Select(z => new {z, ReferenceNumber = z.xcuda_Declarant.Number}).ToList()
                        })
                        .ToList();


                    foreach (var docSetId in res)
                    {
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            docSetId.DocSet.ReferenceNumber);
                        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName); //continue;
                        if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
                            File.Delete(Path.Combine(directoryName, "Instructions.txt"));
                        foreach (var item in docSetId.Entrylst)
                        {
                            var expectedfileName = Path.Combine(directoryName, item.ReferenceNumber + ".xml");
                            //if (File.Exists(expectedfileName)) continue;
                            BaseDataModel.Instance.ExportDocument(expectedfileName, item.z).Wait();

                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }


        }

        public static void ExportEx9Entries()
        {
            Console.WriteLine("Export EX9 Entries");
            try
            {
                var i = BaseDataModel.CurrentSalesInfo();
                BaseDataModel.Instance.ExportDocSet(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number), true).Wait();
                ExportDocSetSalesReport(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number)).Wait();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task ExportDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            var doclst =
                await
                    new SalesDataService().GetSalesDocuments(
                        asycudaDocumentSetId)
                        .ConfigureAwait(false);
            if (doclst == null || !doclst.ToList().Any()) return;


            var exceptions = new ConcurrentQueue<Exception>();

            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                {
                    var s = new ExportToCSV<SaleReportLine, List<SaleReportLine>>();
                    s.StartUp();
                    foreach (var doc in doclst)
                    {
                        try
                        {
                            var data = GetDocumentSalesReport(doc.ASYCUDA_Id).Result;
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv.pdf");
                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            else
                            {
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                            }

                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    }
                    s.ShutDown();
                },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public static async Task ExportLastDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            IEnumerable<AsycudaDocument> doclst = await GetSalesDocumentsWithEntryData
                (asycudaDocumentSetId).ConfigureAwait(false);
            if (doclst == null || !doclst.ToList().Any()) return;


            var exceptions = new ConcurrentQueue<Exception>();

            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                {
                    var s = new ExportToCSV<SaleReportLine, List<SaleReportLine>>();
                    s.StartUp();
                    foreach (var doc in doclst.Where(x => x != null).ToList())
                    {
                        try
                        {
                            var data = GetDocumentSalesReport(doc.ASYCUDA_Id)?.Result;
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv.pdf");
                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            else
                            {
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                            }

                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    }
                    s.ShutDown();
                },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        private static async Task<IEnumerable<AsycudaDocument>> GetSalesDocumentsWithEntryData(int asycudaDocumentSetId)
        {
            try
            {
                var docset = await BaseDataModel.Instance.GetDocSetWithEntryDataDocs(asycudaDocumentSetId).ConfigureAwait(false);

                return docset.Documents.Select(x =>
                                   new SalesDataService().GetSalesDocument(
                                       x.ASYCUDA_Id).Result).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static async Task<IEnumerable<SaleReportLine>> GetDocumentSalesReport(int ASYCUDA_Id)
        {
            var alst = await Ex9SalesReport(ASYCUDA_Id).ConfigureAwait(false);
            if (alst.Any()) return alst;

            alst = IM9AdjustmentsReport(ASYCUDA_Id);
            return alst;
        }
        public class SaleReportLine
        {
            public int Line { get; set; }
            public DateTime Date { get; set; }
            public string InvoiceNo { get; set; }
            public string CustomerName { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double SalesQuantity { get; set; }

            public double SalesFactor { get; set; }
            public double xQuantity { get; set; }
            public double Price { get; set; }
            public string SalesType { get; set; }
            public double GrossSales { get; set; }
            public string PreviousCNumber { get; set; }
            public string PreviousLineNumber { get; set; }
            public string PreviousRegDate { get; set; }
            public double CIFValue { get; set; }
            public double DutyLiablity { get; set; }
            public string Comment { get; set; }
        }

        public class UnClassifiedItem
        {
            public string InvoiceNo { get; set; }
            public int LineNumber { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }

        }

        public class BlankLicenseDescription
        {
            public string InvoiceNo { get; set; }
            public int LineNumber { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public string LicenseDescription { get; set; }

        }

        public static ObservableCollection<SaleReportLine> IM9AdjustmentsReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AllocationQSContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var alst =
                         ctx.AdjustmentShortAllocations.Where(x => x.xASYCUDA_Id == ASYCUDA_Id
                                                                         && x.EntryDataDetailsId != 0
                                                                         && x.PreviousItem_Id != 0
                                                                         && x.pRegistrationDate != null).ToList();

                    //    $"xASYCUDA_Id == {} " + "&& EntryDataDetailsId != null " +
                    //    "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                    //.ConfigureAwait(false)).ToList();

                    var d =
                        alst.Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                Comment = s.Comment,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = s.DutyFreePaid,
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated)
                            }).Distinct();



                    return new ObservableCollection<SaleReportLine>(d);


                }
            }
            catch (Exception Ex)
            {
            }

            return null;
        }

        public static async Task<ObservableCollection<SaleReportLine>> Ex9SalesReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AllocationQSContext())
                {
                    var alst =
                        ctx.AsycudaSalesAllocationsExs.Where(
                                $"xASYCUDA_Id == {ASYCUDA_Id} " + "&& EntryDataDetailsId != null " +
                                "&& PreviousItem_Id != null" + "&& pRegistrationDate != null").ToList();

                    var d =
                        alst.Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                CustomerName = s.CustomerName.StartsWith("- ") ? s.CustomerName.Substring("- ".Length) : s.CustomerName,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = s.DutyFreePaid,
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                //Comments = s.Comments
                            }).Distinct();



                    return new ObservableCollection<SaleReportLine>(d);


                }
            }
            catch (Exception Ex)
            {

            }

            return null;

        }



        public static void SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {

                    foreach (var file in csvFiles)
                    {
                        var newReference = file.Name.Replace(file.Extension, "");

                        var attachment =
                            ctx.AsycudaDocumentSet_Attachments
                                .Include(x => x.Attachments)
                                .FirstOrDefault(x => x.Attachments.FilePath == file.FullName 
                                                     && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                        var existingRefCount = ctx.Attachments.Select(x => x.Reference).Where(x => x.Contains(newReference)).Count();
                        if (existingRefCount > 0) newReference = $"{existingRefCount + 1}-{newReference}";
                        using (var ctx1 = new CoreEntitiesContext() { StartTracking = true })
                        {
                            var oldemail = ctx1.Emails.FirstOrDefault(x => x.EmailUniqueId == email.EmailId);
                            if (oldemail == null)
                            {
                                oldemail = ctx.Emails.Add(new Emails(true)
                                {
                                    EmailUniqueId = email.EmailId,
                                    Subject = email.Subject,
                                    EmailDate = email.EmailDate,
                                    TrackingState = TrackingState.Added
                                });
                                ctx.SaveChanges();
                            }

                            
                            if (attachment == null)
                            {
                                ctx1.AsycudaDocumentSet_Attachments.Add(
                                    new AsycudaDocumentSet_Attachments(true)
                                    {
                                        AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                                        Attachments = new Attachments(true)
                                        {
                                            FilePath = file.FullName,
                                            DocumentCode = fileType.DocumentCode,
                                            Reference = newReference,
                                            EmailId = email.EmailId.ToString(),
                                        },
                                        DocumentSpecific = fileType.DocumentSpecific,
                                        FileDate = file.LastWriteTime,
                                        EmailUniqueId = email.EmailId,
                                        FileTypeId = fileType.Id,
                                        TrackingState = TrackingState.Added
                                    });

                            }
                            else
                            {
                                attachment.DocumentSpecific = fileType.DocumentSpecific;
                                attachment.FileDate = file.LastWriteTime;
                                attachment.EmailUniqueId = email.EmailId;
                                attachment.FileTypeId = fileType.Id;
                                attachment.Attachments.Reference = newReference;
                                attachment.Attachments.DocumentCode = fileType.DocumentCode;
                                attachment.Attachments.EmailId = email.EmailId.ToString();
                            }

                            ctx1.SaveChanges();
                        }
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static FileInfo[] TrySaveFileInfo(FileInfo[] csvFiles, FileTypes fileType)
        {
            try
            {

                var res = new List<FileInfo>();
                foreach (var file in csvFiles)
                {
                    var fileTxt = File.ReadAllLines(file.FullName);
                    string dbStatement = "";
                    foreach (var line in fileTxt)
                    {
                        if (line.ToLower().Contains("Not Found".ToLower())) continue;
                        var im = fileType.EmailInfoMappings.SelectMany(x => x.InfoMapping.InfoMappingRegEx.Select(z => new
                        {
                            Em = x,
                            Rx = z,
                            Key = Regex.Match(line, z.KeyRegX, RegexOptions.IgnoreCase),
                            Field = Regex.Match(line, z.FieldRx, RegexOptions.IgnoreCase)
                        }))
                            .Where(z => z.Key.Success && z.Field.Success).ToList();

                        if (!im.Any()) continue;
                        if (!res.Contains(file)) res.Add(file);

                        im.ForEach(x =>
                        {
                            try
                            {
                               var key = string.IsNullOrEmpty(x.Key.Groups["Key"].Value.Trim())
                                    ? x.Rx.InfoMapping.Key
                                    : x.Rx.KeyReplaceRx == null
                                        ? x.Key.Groups["Key"].Value.Trim()
                                        : Regex.Match(
                                                Regex.Replace(line, x.Rx.KeyRegX, x.Rx.KeyReplaceRx,
                                                    RegexOptions.IgnoreCase), x.Rx.KeyRegX,
                                                RegexOptions.IgnoreCase)
                                            .Value.Trim();

                                var value = string.IsNullOrEmpty(x.Field.Groups["Value"].Value.Trim())
                                    ? x.Field.Groups[0].Value.Trim()
                                    : x.Rx.FieldReplaceRx == null
                                        ? x.Field.Groups["Value"].Value.Trim()
                                        : Regex.Match(
                                                Regex.Replace(line, x.Rx.FieldRx, x.Rx.FieldReplaceRx,
                                                    RegexOptions.IgnoreCase), x.Rx.FieldRx,
                                                RegexOptions.IgnoreCase)
                                            .Value.Trim();
                                fileType.Data.Add(
                                    new KeyValuePair<string, string>(key, value));

                                if (x.Em.UpdateDatabase == true)
                                {
                                    dbStatement +=
                                        $@" Update {x.Rx.InfoMapping.EntityType} Set {x.Rx.InfoMapping.Field} = '{
                                                ReplaceSpecialChar(value,
                                                    "")
                                            }' Where {x.Rx.InfoMapping.EntityKeyField} = '{
                                                fileType.Data.First(z => z.Key == x.Rx.InfoMapping.EntityKeyField).Value}';";
                                }
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        });


                    }

                    if (!string.IsNullOrEmpty(dbStatement))
                        new CoreEntitiesContext().Database.ExecuteSqlCommand(dbStatement);
                }

                return res.ToArray();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public static void SaveInfo(FileInfo[] csvFiles, FileTypes fileType, List<EmailInfoMappings> emailMappings)
        //{
        //    foreach (var file in csvFiles.Where(x => x.Name == "Info.txt"))
        //    {
        //        var fileTxt = File.ReadAllLines(file.FullName);
        //        SaveAsycudaDocumentSet(fileTxt, fileType,emailMappings);
        //        // SaveEntryData(oldDocSet, fileTxt);
        //    }
        //}

        //public static void SaveAsycudaDocumentSet(string[] fileTxt, FileTypes fileType, List<EmailInfoMappings> emailMappings)
        //{
        //    try
        //    {


        //        using (var ctx = new CoreEntitiesContext())
        //        {
        //            var res = emailMappings.Where(x => x.UpdateDatabase == true).ToList();
        //            string dbStatement = "";
        //            foreach (var line in fileTxt)
        //            {

        //                var im = res.SelectMany(x => x.InfoMapping.InfoMappingRegEx.Select(z => new
        //                {
        //                    Rx = z,
        //                    Key = Regex.Match(line, z.KeyRegX, RegexOptions.IgnoreCase),
        //                    Field = Regex.Match(line, z.FieldRx, RegexOptions.IgnoreCase)
        //                }))
        //                    .Where(z => z.Key.Success && z.Field.Success).ToList();

        //                //match = Regex.Match(line,
        //                //    @"[.]*((?<Key>[a-zA-Z\(\)]+)[:\s\#]+(?<Value>[a-zA-Z0-9\- :$,/\.]*))", RegexOptions.IgnoreCase);
        //                //res.Where(x =>
        //                //                            x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower())

        //                if (im.Any())
        //                    foreach (var infoMapping in im)
        //                    {
        //                        var val = infoMapping.Rx.FieldReplaceRx == null
        //                            ? infoMapping.Field.Groups["Value"].Value.Trim()
        //                            : Regex.Match(
        //                                    Regex.Replace(line, infoMapping.Rx.FieldRx, infoMapping.Rx.FieldReplaceRx,
        //                                        RegexOptions.IgnoreCase), infoMapping.Rx.FieldRx,
        //                                    RegexOptions.IgnoreCase)
        //                                .Value.Trim();

        //                        dbStatement +=
        //                            $@" Update AsycudaDocumentSet Set {infoMapping.Rx.InfoMapping.Field} = '{
        //                                    ReplaceSpecialChar(val,
        //                                        "")
        //                                }' Where AsycudaDocumentSetId = '{fileType.Data.First(z => z.Key == )}';";
        //                    }
        //            }

        //            if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        //public static void SaveEntryData(int oldDocSet, string[] fileTxt)
        //{
        //    using (var ctx = new CoreEntitiesContext())
        //    {
        //        var res = ctx.InfoMapping.Where(x => x.EntityType == "Discrepancy")
        //            .ToList();
        //        string dbStatement = "";
        //        foreach (var line in fileTxt)
        //        {
        //            var match = Regex.Match(line,
        //                @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.]*))", RegexOptions.IgnoreCase);
        //            if (match.Success)
        //                foreach (var infoMapping in res.Where(x =>
        //                    x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower()))
        //                {
        //                    dbStatement +=
        //                        $@" Update AsycudaDocumentSet Set {infoMapping.Field} = '{
        //                                ReplaceSpecialChar(match.Groups["Value"].Value.Trim(),
        //                                    "")
        //                            }' Where AsycudaDocumentSetId = '{oldDocSet}';";
        //                }
        //        }

        //        if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
        //    }
        //}

        public static EmailDownloader.Client Client { get; set; }

        public static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s]+", rstring);
        }



        public static void RunSiKuLi(string directoryName, string scriptName, string lastCNumber = "")
        {
            try
            {

                if (string.IsNullOrEmpty(directoryName)) return;

                Console.WriteLine($"Executing {scriptName}");
               

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "java.exe";
                startInfo.Arguments =
                     $@"-jar C:\Users\{Environment.UserName}\OneDrive\Clients\AutoBot\sikulix-2.0.0.jar -r C:\Users\{
                   // $@"-jar C:\Users\{Environment.UserName}\OneDrive\Clients\AutoBot\sikulix.jar -r C:\Users\{
                            Environment.UserName
                        }\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                            BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin
                        } {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} {
                            (string.IsNullOrEmpty(lastCNumber) ? "" : lastCNumber + " ")
                        }""{directoryName + "\\"}";
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                process.Start();
                var timeoutCycles = 0;
                while (!process.HasExited && process.Responding)
                {
                    //if(File.Exists(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, $"{docRef}/I")))
                    if (timeoutCycles > 12) break;
                    //Console.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Debug.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Thread.Sleep(1000 * 60);
                    timeoutCycles += 1;
                }

                if (!process.HasExited) process.Kill();

                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA") || x.MainWindowTitle.Contains("Acrobat Reader"))
                    .ToList())
                {
                    process1.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw; 
            }
        }



        public static void SaveCsv(FileInfo[] csvFiles, FileTypes fileType)
        {
            Console.WriteLine("Importing CSV " + fileType.Type);
            foreach (var file in csvFiles)
            {

                try
                {
                    
                    SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, fileType.OverwriteFiles ?? false)//set to false to merge
                        .Wait();


                    //if (VerifyCSVImport(file))
                    //    return;
                    //else
                    //    continue;

                }
                catch (Exception e)
                {

                    using (var ctx = new CoreEntitiesContext())
                    {
                        if (ctx.AttachmentLog.FirstOrDefault(x =>
                                x.AsycudaDocumentSet_Attachments.Attachments.FilePath == file.FullName
                                && x.Status == "Sender Informed of Error") == null)
                        {
                            var att = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")));
                            var body = "Error While Importing: \r\n" +
                                       $"File: {file}\r\n" +
                                       $"Error: {(e.InnerException ?? e).Message.Replace(file.FullName, file.Name)} \r\n" +
                                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                            var emailId = ctx.AsycudaDocumentSet_Attachments
                                .FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")))?.EmailUniqueId;
                            EmailDownloader.EmailDownloader.SendBackMsg(Convert.ToInt32(emailId), Client, body);
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = att.Id,
                                Status = "Sender Informed of Error",
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();
                        }
                    }


                }
            }
        }

        public static void ReplaceCSV(FileInfo[] csvFiles, FileTypes fileType)
        {
            Console.WriteLine("Importing CSV " + fileType.Type);
            foreach (var file in csvFiles)
            {

                try
                {
                    SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, true)//set to false to merge
                        .Wait();


                    //if (VerifyCSVImport(file))
                    //    return;
                    //else
                    //    continue;

                }
                catch (Exception e)
                {

                    using (var ctx = new CoreEntitiesContext())
                    {
                        if (ctx.AttachmentLog.FirstOrDefault(x =>
                                x.AsycudaDocumentSet_Attachments.Attachments.FilePath == file.FullName
                                && x.Status == "Sender Informed of Error") == null)
                        {
                            var att = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")));
                            var body = "Error While Importing: \r\n" +
                                       $"File: {file}\r\n" +
                                       $"Error: {(e.InnerException ?? e).Message.Replace(file.FullName, file.Name)} \r\n" +
                                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                            var emailId = ctx.AsycudaDocumentSet_Attachments
                                .FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")))?.EmailUniqueId;
                            EmailDownloader.EmailDownloader.SendBackMsg(Convert.ToInt32(emailId), Client, body);
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = att.Id,
                                Status = "Sender Informed of Error",
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();
                        }
                    }


                }
            }
        }

        public static bool VerifyCSVImport(FileInfo file)
        {
            var dt = CSV2DataTable(file, "NO");
            if (dt.Rows.Count == 0) return false;
            var fileRes = dt.AsEnumerable()
                .Where(x => x[0] != DBNull.Value)
                .Select(x => new
                {
                    Invoice = x.Table.Columns.Contains("Invoice #") ? x["Invoice #"].ToString() : x["Invoice#"].ToString(),
                    Total = Convert.ToDouble(x["Quantity"]) * Convert.ToDouble(x["Cost"])
                })
                .GroupBy(x => x.Invoice)
                .Select(x => new
                {
                    Invoice = x.Key,
                    Total = Math.Round(x.Sum(z => z.Total), 2)
                }).ToList();

            using (var ctx = new EntryDataQSContext())
            {
                var dbres = ctx.EntryDataDetailsExes
                    .Select(x => new
                    {
                        Invoice = x.EntryDataId,
                        Total = x.Quantity * x.Cost
                    })
                    .GroupBy(x => x.Invoice)
                    .Select(x => new
                    {
                        Invoice = x.Key,
                        Total = Math.Round(x.Sum(z => z.Total), 2)
                    }).ToList();
                var res = fileRes.GroupJoin(dbres, x => x.Invoice, y => y.Invoice,
                        (x, y) => new { file = x, db = y.SingleOrDefault() })
                    .Where(x => x.file.Total != x.db.Total)
                    .Select(x => new { x.file.Invoice, FileTotal = x.file.Total, dbTotal = x.db.Total })
                    .ToList();

                if (!res.Any())
                {
                    //TODO: Log Message
                    return true;
                }
            }

            return false;
        }


        public static void Xlsx2csv(FileInfo[] files, FileTypes fileType)
        {
            try
            {

            var adjReference = $"ADJ-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}";
            var disReference = $"DIS-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}";
            var dic = new Dictionary<string, Func<Dictionary<string, string>, string>>()
            {
                {"CurrentDate", (dt)=> DateTime.Now.Date.ToShortDateString() },
                { "DIS-Reference", (dt) => disReference},
                { "ADJ-Reference", (dt) => adjReference},
                {"Quantity", (dt) => dt.ContainsKey("Received Quantity") && dt.ContainsKey("Invoice Quantity")? Convert.ToString(Math.Abs(Convert.ToDouble(dt["Received Quantity"].Replace("\"","")) - Convert.ToDouble(dt["Invoice Quantity"].Replace("\"",""))), CultureInfo.CurrentCulture) : Convert.ToDouble(dt["Quantity"].Replace("\"","")).ToString(CultureInfo.CurrentCulture) },
                {"ZeroCost", (x) => "0" },
                {"ABS-Added", (dt) => Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                {"ABS-Removed", (dt) => Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                {"ADJ-Quantity", (dt) => Convert.ToString(Math.Abs((Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))) - Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))))), CultureInfo.CurrentCulture) },
                {"Cost2USD", (dt) => dt.ContainsKey("{XCDCost}") && Convert.ToDouble(dt["{XCDCost}"].Replace("\"","")) > 0 ? (Convert.ToDouble(dt["{XCDCost}"].Replace("\"",""))/2.7169).ToString(CultureInfo.CurrentCulture) : "{NULL}" },
            };

            foreach (var file in files)
            {
                var dfile = new FileInfo($@"{file.DirectoryName}\{file.Name.Replace(file.Extension, ".csv")}");
                if (dfile.Exists && dfile.LastWriteTime >= file.LastWriteTime.AddMinutes(5)) return;
                // Reading from a binary Excel file (format; *.xlsx)
                FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                excelReader.Close();
                

                int row_no = 0;

                if (result.Tables.Contains("MisMatches") && result.Tables.Contains("POTemplate")) ReadMISMatches(result.Tables["MisMatches"], result.Tables["POTemplate"]);

                result.Tables[0].Columns.Add("LineNumber", typeof(int));

                var rows = new List<DataRow>();
                ///insert linenumber
                while (row_no < result.Tables[0].Rows.Count)
                {

                    var dataRow = result.Tables[0].Rows[row_no];
                    dataRow["LineNumber"] = row_no;
                    rows.Add(dataRow);
                    row_no++;
                }

                var table = new ConcurrentDictionary<int, string>();
                Parallel.ForEach(rows, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 },
                    row =>
                    {
                        StringBuilder a = new StringBuilder();

                        if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
                                .All(x => row.ItemArray.Contains(x)))
                        {
                            //if(dic.ContainsKey())
                            a.Append("");
                        }

                        for (int i = 0; i < result.Tables[0].Columns.Count - 1; i++)
                        {

                            a.Append(StringToCSVCell(row[i].ToString()) + ",");
                        }


                        a.Append("\n");
                        table.GetOrAdd(Convert.ToInt32(row["LineNumber"]), a.ToString());
                    });




                string output = Path.ChangeExtension(file.FullName, ".csv");
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(table.OrderBy(x => x.Key).Select(x => x.Value).Aggregate((a, x) => a + x));
                csv.Close();

                FixCsv(new FileInfo(output), fileType, dic);

            }

            }
            catch (Exception e)
            {

                throw;
            }
        }

        private static void ReadMISMatches(DataTable misMatches, DataTable poTemplate)
        {
            try
            {

                var misHeaderRow = misMatches.Rows[0].ItemArray.ToList();
                var poHeaderRow = poTemplate.Rows[0].ItemArray.ToList();
                foreach (DataRow misMatch in misMatches.Rows)
                {
                    if (misMatch == misMatches.Rows[0]) continue;
                    var InvoiceNo = misMatch[misHeaderRow.IndexOf("InvoiceNo")].ToString();
                    var invItemCode = misMatch[misHeaderRow.IndexOf("INVItemCode")].ToString();
                    var poItemCode = misMatch[misHeaderRow.IndexOf("POItemCode")].ToString();
                    if (!string.IsNullOrEmpty(misMatch[misHeaderRow.IndexOf("PONumber")].ToString()) &&
                        !string.IsNullOrEmpty(InvoiceNo) &&
                        !string.IsNullOrEmpty(poItemCode) &&
                        !string.IsNullOrEmpty(invItemCode))
                    {

                        DataRow row;
                        var addrow = true;
                        if (string.IsNullOrEmpty(poTemplate.Rows[1][poHeaderRow.IndexOf("PO Number")].ToString()))
                        {
                            row = poTemplate.Rows[1];
                            addrow = false;
                        }
                        else
                        {
                            row = poTemplate.NewRow();
                        }

                        row[poHeaderRow.IndexOf("PO Number")] = misMatch[misHeaderRow.IndexOf("PONumber")];
                        row[poHeaderRow.IndexOf("Date")] = poTemplate.Rows[1][poHeaderRow.IndexOf("Date")];
                        row[poHeaderRow.IndexOf("PO Item Number")] = poItemCode;
                        row[poHeaderRow.IndexOf("Supplier Item Number")] = invItemCode;
                        row[poHeaderRow.IndexOf("PO Item Description")] =
                            misMatch[misHeaderRow.IndexOf("PODescription")];
                        row[poHeaderRow.IndexOf("Supplier Item Description")] =
                            misMatch[misHeaderRow.IndexOf("INVDescription")];
                        row[poHeaderRow.IndexOf("Cost")] =
                            ((double) misMatch[misHeaderRow.IndexOf("INVCost")] /
                             ((misHeaderRow.IndexOf("INVSalesFactor") > -1
                               && !string.IsNullOrEmpty(misMatch[misHeaderRow.IndexOf("INVSalesFactor")].ToString()))
                                 ? Convert.ToInt32(misMatch[misHeaderRow.IndexOf("INVSalesFactor")])
                                 : 1));
                        row[poHeaderRow.IndexOf("Quantity")] = misMatch[misHeaderRow.IndexOf("POQuantity")];
                        row[poHeaderRow.IndexOf("Total Cost")] = misMatch[misHeaderRow.IndexOf("INVTotalCost")];
                        if (addrow) poTemplate.Rows.Add(row);
                    }

                    using (var ctx = new EntryDataDSContext())
                    {
                        InvoiceDetails invRow;
                        EntryDataDetails poRow;

                        var invItm = ctx.InventoryItems.Include(x => x.AliasItems).FirstOrDefault(x =>
                            x.ItemNumber == invItemCode
                            && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        if (invItm == null)
                        {
                            invItm = new InventoryItems()
                            {
                                ApplicationSettingsId =
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                                Description = misMatch[misHeaderRow.IndexOf("INVDescription")].ToString(),
                                ItemNumber = invItemCode,
                                TrackingState = TrackingState.Added
                            };
                            ctx.InventoryItems.Add(invItm);
                        }

                        var poItm = ctx.InventoryItems.Include(x => x.AliasItems).FirstOrDefault(x =>
                            x.ItemNumber == poItemCode && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        if (poItm == null)
                        {
                            poItm = new InventoryItems()
                            {
                                ApplicationSettingsId =
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                                Description = misMatch[misHeaderRow.IndexOf("PODescription")].ToString(),
                                ItemNumber = poItemCode,
                                TrackingState = TrackingState.Added
                            };
                            ctx.InventoryItems.Add(poItm);
                        }

                        if (!poItm.AliasItems.Any(x => x.AliasItemId == invItm.Id) &&
                            !invItm.AliasItems.Any(x => x.InventoryItemId == poItm.Id))
                        {
                            ctx.InventoryItemAlias.Add(new InventoryItemAlias(true)
                            {
                                InventoryItems = poItm,
                                AliasItem = invItm,
                                AliasName = invItm.ItemNumber,
                                TrackingState = TrackingState.Added
                            });
                        }

                        //var itmAlias = ctx.InventoryItemAlias
                        ctx.SaveChanges();


                        var INVDetailsId = misMatch[misHeaderRow.IndexOf("INVDetailsId")].ToString();
                        if (string.IsNullOrEmpty(INVDetailsId))
                        {

                            invRow = new InvoiceDetails() {TrackingState = TrackingState.Added};
                            var inv = ctx.ShipmentInvoice.FirstOrDefault(x => x.InvoiceNo == InvoiceNo);
                            if (inv == null) continue;
                            invRow.ShipmentInvoiceId = inv.Id;
                            ctx.ShipmentInvoiceDetails.Add(invRow);

                        }
                        else
                        {

                            invRow = ctx.ShipmentInvoiceDetails.FirstOrDefault(x =>
                                x.Id.ToString() == INVDetailsId);
                            if (invRow == null) continue;

                        }

                        invRow.ItemDescription = misMatch[misHeaderRow.IndexOf("INVDescription")].ToString();
                        invRow.ItemNumber = misMatch[misHeaderRow.IndexOf("INVItemCode")].ToString();
                        invRow.Cost = (double) misMatch[misHeaderRow.IndexOf("INVCost")];
                        invRow.TotalCost = (double) misMatch[misHeaderRow.IndexOf("INVTotalCost")];
                        if (misHeaderRow.IndexOf("INVSalesFactor") > -1 &&
                            !string.IsNullOrEmpty(misMatch[misHeaderRow.IndexOf("INVSalesFactor")].ToString()))
                            invRow.SalesFactor = Convert.ToDouble(misMatch[misHeaderRow.IndexOf("INVSalesFactor")].ToString());
                        else
                        {
                            invRow.SalesFactor = 1;
                        }

                        invRow.Quantity = (double) misMatch[misHeaderRow.IndexOf("INVQuantity")];
                        invRow.InventoryItemId = invItm.Id;
                        ctx.SaveChanges();

                        var PODetailsId = misMatch[misHeaderRow.IndexOf("PODetailsId")].ToString();
                        if (string.IsNullOrEmpty(PODetailsId))
                        {

                            poRow = new EntryDataDetails() {TrackingState = TrackingState.Added};
                            var pO = ctx.EntryData.FirstOrDefault(x =>
                                x.EntryDataId == misMatch[misHeaderRow.IndexOf("PONumber")].ToString());
                            if (pO == null) continue;
                            poRow.EntryData_Id = pO.EntryData_Id;
                            ctx.EntryDataDetails.Add(poRow);

                        }
                        else
                        {

                            poRow = ctx.EntryDataDetails.FirstOrDefault(x =>
                                x.EntryDataDetailsId.ToString() == PODetailsId);
                            if (poRow == null) continue;

                        }

                        poRow.ItemDescription = misMatch[misHeaderRow.IndexOf("PODescription")].ToString();
                        poRow.ItemNumber = misMatch[misHeaderRow.IndexOf("POItemCode")].ToString();
                        poRow.Cost = (double) misMatch[misHeaderRow.IndexOf("POCost")];
                        poRow.TotalCost = (double) misMatch[misHeaderRow.IndexOf("POTotalCost")];
                        //poRow.SalesFactor = (int)misMatch["INVSalesFactor"];
                        poRow.Quantity = (double) misMatch[misHeaderRow.IndexOf("POQuantity")];
                        poRow.InventoryItemId = poItm.Id;
                        ctx.SaveChanges();
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static string StringToCSVCell(string str)
        {
            //bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") 
            //                  || str.Contains("\n") || str.Contains(".") || str.Contains("'") || str.Contains("#"));
            //if (mustQuote)
            //{
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char nextChar in str)
            {
                sb.Append(nextChar);
                if (nextChar == '"')
                    sb.Append("\"");
            }
            sb.Append("\"");
            return sb.ToString().Trim();
            //}

            //return str;
        }

        public static void FixCsv(FileInfo file, FileTypes fileType,
            Dictionary<string, Func<Dictionary<string, string>, string>> dic)
        {
            try
            {


                if (fileType.FileTypeMappings.Count == 0 && fileType.ReplicateHeaderRow == false)
                {
                    throw new ApplicationException($"Missing File Type Mappings for {fileType.FilePattern}");

                }
                var dfile = new FileInfo(
                    $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}");
                //if (dfile.Exists && dfile.LastWriteTime >= file.LastWriteTime) return;
                if(File.Exists(dfile.FullName))File.Delete(dfile.FullName);
                // Reading from a binary Excel file (format; *.xlsx)
                var dt = CSV2DataTable(file, "NO");


                var table = new System.Collections.Concurrent.ConcurrentDictionary<int, string>();
                int drow_no = 0;
                var dRows = new List<DataRow>();

                dt.Columns.Add("LineNumber", typeof(string));
                List<DataColumn> deleteColumns = new List<DataColumn>();
               

                //delete rows till header
                DataRow currentReplicatedHeading = null;
                var headerRow = dt.Rows[0].ItemArray.ToList();
                while (drow_no < dt.Rows.Count)
                {
                    if (fileType.ReplicateHeaderRow == true)
                    {
                        if (fileType.FileTypeMappings.Where(x => x.Required).All(x => !string.IsNullOrEmpty(dt.Rows[drow_no][headerRow.IndexOf(x.OriginalName)].ToString())))
                        {
                            if (dt.Rows[drow_no].ItemArray.Count(x => !string.IsNullOrEmpty(x.ToString())) >=
                                (fileType.FileTypeMappings.Any()
                                    ? fileType.FileTypeMappings.Count(x => x.Required)
                                    : 1))
                            {
                                currentReplicatedHeading = dt.Rows[drow_no];
                                dRows.Add(dt.Rows[drow_no]);
                            }
                        }
                        else
                        {

                            if (dt.Rows[drow_no].ItemArray.Count(x => !string.IsNullOrEmpty(x.ToString())) >=
                                (fileType.FileTypeMappings.Any()?fileType.FileTypeMappings.Count(x => x.Required):1))
                            {

                                var row = dt.NewRow();
                                foreach (DataColumn col in dt.Columns)
                                {
                                    var val = dt.Rows[drow_no][col.ColumnName].ToString();
                                    if (!string.IsNullOrEmpty(val) &&
                                        string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                        row[col.ColumnName] = val;
                                    if (string.IsNullOrEmpty(val) &&
                                        !string.IsNullOrEmpty(currentReplicatedHeading[col.ColumnName].ToString()))
                                        row[col.ColumnName] = currentReplicatedHeading[col.ColumnName].ToString();
                                }

                                dRows.Add(row);
                            }

                        }
                        //dRows.Add(dt.Rows[drow_no]);
                    }
                    else
                    {
                        if (dt.Rows[drow_no].ItemArray.Count(x => !string.IsNullOrEmpty(x.ToString())) >=
                            fileType.FileTypeMappings.Count(x => x.Required))
                        {
                            //dt.Rows[drow_no]["LineNumber"] = drow_no;// give value to prevent upper from bugging later
                            dRows.Add(dt.Rows[drow_no]);
                        }
                    }
                    //else
                    //    throw new ApplicationException(
                    //        $"Missing Required Field on Line {row_no + 1} in File:'{file.FullName}'");

                    drow_no++;

                }
                // delete duplicate headers
                if (fileType.FileTypeMappings.Any())
                {
                    var dupheaders = dRows.Where(x =>
                            x.ItemArray.Contains(fileType.FileTypeMappings.OrderBy(z => z.Id)
                                .First(z => !z.OriginalName.Contains("{")).OriginalName)).Skip(1)
                        .ToList();
                    foreach (var row in dupheaders)
                    {
                        dRows.Remove(row);
                    }
                }

                var header = dRows[0];
                for (int i = 0; i < header.ItemArray.Length - 1; i++)
                {
                    if (string.IsNullOrEmpty(header[i].ToString()))
                    {
                        deleteColumns.Add(dt.Columns[i]);
                        continue;
                    }
                    header[i] = header[i].ToString().ToUpper();
                }
                
                deleteColumns.ForEach(x => dt.Columns.Remove(x));

                for (int i = 0; i < dRows.Count; i++)
                {
                    dRows[i]["LineNumber"] = i;
                }

                header["LineNumber"] = "LineNumber";
                var headerlst = header.ItemArray.ToList();

                var missingMaps = fileType.FileTypeMappings.Where(x => x.Required && !x.OriginalName.Contains("{"))
                    .GroupBy(x => x.DestinationName)
                    .Where(item => item.All(z => !headerlst.Any(q => String.Equals(q.ToString(), z.OriginalName, StringComparison.CurrentCultureIgnoreCase)))).ToList();// !headerlst  (item.OriginalName.ToUpper())
                if (missingMaps.Any())
                {
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                        new[] { "Joseph@auto-brokerage.com" },
                        $"Required Field - '{missingMaps.Select(x => x.Key).Aggregate((o, n) => o + "," + n)}' in File: {file.Name} dose not exists.",
                        Array.Empty<string>());
                    return; // not right file type
                }

                var mappingMailSent = false;
                Parallel.ForEach(dRows, new ParallelOptions() { MaxDegreeOfParallelism =  1 },//Environment.ProcessorCount *
            drow =>
            {
                var row = new Dictionary<string, string>();
                var row_no = drow["LineNumber"].ToString() == $"LineNumber" ? 0 : Convert.ToInt32(drow["LineNumber"]);
                if (fileType.FileTypeMappings.Any())
                {
                    foreach (var mapping in fileType.FileTypeMappings.OrderBy(x => x.Id))
                    {
                        var maps = mapping.OriginalName.Split('+');
                        var val = "";
                        foreach (var map in maps)
                        {
                            if (!header.ItemArray.Contains(map.ToUpper()) &&
                                !dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
                            {
                                if (mapping.Required)
                                {
                                    if (mappingMailSent) return;
                                    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToUInt16(fileType.EmailId),
                                        Utils.Client, $"Bug Found",
                                        $"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} dose not exists.",
                                        new[] {"Joseph@auto-brokerage.com"}, Array.Empty<string>()
                                    );
                                    mappingMailSent = true;
                                    return;
                                }

                                //TODO: log error
                                continue;
                            }

                            if (row_no == 0)
                            {
                                val += mapping.DestinationName;
                                if (maps.Length > 1) break;
                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(dt.Rows[row_no][map].ToString())) continue;
                                if (map.Contains("{") && dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
                                {

                                    val += dic[map.Replace("{", "").Replace("}", "")].Invoke(row);
                                }
                                else
                                {
                                    var index = Array.LastIndexOf(header.ItemArray,
                                        map.ToUpper()); //last index of because of Cost USD file has two columns
                                    val += drow[index];
                                }

                                if (maps.Length > 1 && map != maps.Last()) val += " - ";
                            }


                        }


                        if (val == "{NULL}") continue;
                        if (val == "" && row_no == 0) continue;
                        if (val == "" && row_no > 0 &&
                            (!header.ItemArray.Contains(mapping.OriginalName.ToUpper()) &&
                             !dic.ContainsKey(mapping.OriginalName.Replace("{", "").Replace("}", "")))) continue;
                        if (row_no > 0)
                            if (string.IsNullOrEmpty(val) &&
                                (mapping.Required == true || mapping.DestinationName == "Invoice #")
                            ) // took out because it will replace invoice no regardless
                            {
                                if (mapping.DestinationName == "Invoice #")
                                {
                                    val += dic["DIS-Reference"].Invoke(row);

                                }
                                else
                                {
                                    //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                    //    new[] { "Joseph@auto-brokerage.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                                    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToUInt16(fileType.EmailId),
                                        Utils.Client, $"Bug Found",
                                        $"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} has no Value.",
                                        new[] {"Joseph@auto-brokerage.com"}, Array.Empty<string>()
                                    );
                                    return;
                                }

                            }

                            else if (mapping.DataType == "Number")
                            {
                                if (string.IsNullOrEmpty(val)) val = "0";
                                if (val.ToCharArray().All(x => !char.IsDigit(x)))
                                {
                                    //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                    //    new[] { "Joseph@auto-brokerage.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                                    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToUInt16(fileType.EmailId),
                                        Utils.Client, $"Bug Found",
                                        $"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} has Value ='{val}' cannot be converted to Number.",
                                        new[] {"Joseph@auto-brokerage.com"}, Array.Empty<string>()
                                    );
                                    return;
                                    //val = "";
                                }
                            }
                            else if (mapping.DataType == "Date")
                            {
                                if (DateTime.TryParse(val, out var tmp) == false)
                                {
                                    //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                    //    new[] { "Joseph@auto-brokerage.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                                    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToUInt16(fileType.EmailId),
                                        Utils.Client, $"Bug Found",
                                        $"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} has Value ='{val}' cannot be converted to date.",
                                        new[] {"Joseph@auto-brokerage.com"}, Array.Empty<string>()
                                    );
                                    return;
                                    //  val = "";
                                }
                            }

                        if (row.ContainsKey(mapping.DestinationName))
                        {
                            if (row_no == 0)
                            {
                                row.Remove(mapping.DestinationName);
                                var nrow = new Dictionary<string, string>();
                                nrow = row.Clone();
                                nrow.Add(mapping.DestinationName, StringToCSVCell(val));
                                row = nrow;
                                // row.Add(mapping.DestinationName, StringToCSVCell(val));

                            }
                            else
                            {
                                row[mapping.DestinationName] = StringToCSVCell(val);
                            }

                        }
                        else
                        {
                            row.Add(mapping.DestinationName, StringToCSVCell(val));
                        }





                    }
                }
                else
                {
                    
                        foreach (var h in header.ItemArray)
                        {
                            var index = Array.LastIndexOf(header.ItemArray,
                                h); //last index of because of Cost USD file has two columns
                            var val = drow[index].ToString();
                            if(!row.ContainsKey(h.ToString()))row.Add(h.ToString(), StringToCSVCell(val));
                        }
                    
                }


                if (row.Count > 0 && row.Count >= fileType.FileTypeMappings.DistinctBy(x => x.DestinationName)
                        .Count(x => x.Required == true))
                {
                    var value = fileType.FileTypeMappings.Any()
                                    ?fileType.FileTypeMappings.OrderBy(x => x.Id).Select(x => x.DestinationName).Where(x => !x.StartsWith("{")).Distinct()
                                    .Select(x => row.ContainsKey(x)? row[x]:"")
                                    .Aggregate((a, x) => a + "," + x) + "\n"
                        :row.Values.Aggregate((a, x) => a + "," + x) + "\n";
                    table.GetOrAdd(row_no, value);
                }

            });



                string output = $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}";
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(table.OrderBy(x => x.Key).Select(x => x.Value).Aggregate((a, x) => a + x));
                csv.Close();
                if (fileType.ChildFileTypes.Any())
                {
                    var fileTypes = BaseDataModel.GetFileType(fileType.ChildFileTypes.First());
                    fileTypes.AsycudaDocumentSetId = fileType.AsycudaDocumentSetId;
                    fileTypes.EmailId = fileType.EmailId;
                    SaveCsv(new FileInfo[] { new FileInfo(output) }, fileTypes);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }


        }



        public static DataTable CSV2DataTable(FileInfo file, string headers)
        {
            OleDbConnection conn = null;

            if (Environment.Is64BitOperatingSystem == false)
            {
                conn = new OleDbConnection(string.Format(
                    @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};" +
                    $"Extended Properties=\"Text;HDR={headers};FMT=Delimited;CharacterSet=65001\"",
                    file.DirectoryName));
            }
            else
            {
                conn = new OleDbConnection(string.Format(
                    @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0};" +
                    $"Extended Properties=\"Text;HDR={headers};FMT=Delimited;CharacterSet=65001\"",
                    file.DirectoryName));
            }



            conn.Open();

            string sql = string.Format("select * from [{0}]", Path.GetFileName(file.Name));
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();

            return dt;
        }

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>

    }

   

    internal class AssessedEntryInfo
    {
        public string CNumber { get; set; }
        public string DocumentType { get; set; }
        public string Reference { get; set; }
        public string Date { get; set; }
        public string PONumber { get; set; }
        public string Invoice { get; set; }
        public string Taxes { get; set; }
        public string CIF { get; set; }
        public string BillingLine { get; set; }
    }

    public class IncompleteSupplier
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string CountryCode { get; set; }
    }

    public partial class SubmitDiscrepanciesErrorReport
    {

        public string Type { get; set; }

        public System.DateTime InvoiceDate { get; set; }

        public Nullable<System.DateTime> EffectiveDate { get; set; }

        public string InvoiceNo { get; set; }

        public Nullable<int> LineNumber { get; set; }

        public string ItemNumber { get; set; }

        public string ItemDescription { get; set; }

        public Nullable<double> InvoiceQty { get; set; }

        public Nullable<double> ReceivedQty { get; set; }

        public double Cost { get; set; }

        public string PreviousCNumber { get; set; }

        public string PreviousInvoiceNumber { get; set; }

        public string comment { get; set; }

        public string Status { get; set; }

        public string DutyFreePaid { get; set; }
        public string subject { get; set; }

        public System.DateTime emailDate { get; set; }
        public double Quantity { get; set; }
    }

    public class DiscrepancyPreExecutionReport
    {
        public string Type { get; set; }

        public System.DateTime InvoiceDate { get; set; }

        public Nullable<System.DateTime> EffectiveDate { get; set; }

        public string InvoiceNo { get; set; }

        public Nullable<int> LineNumber { get; set; }

        public string ItemNumber { get; set; }

        public string ItemDescription { get; set; }

        public Nullable<double> InvoiceQty { get; set; }

        public Nullable<double> ReceivedQty { get; set; }

        public double Cost { get; set; }

        public string PreviousCNumber { get; set; }

        public string PreviousInvoiceNumber { get; set; }

        public string comment { get; set; }

        public string Status { get; set; }

        public string DutyFreePaid { get; set; }
        public string subject { get; set; }

        public System.DateTime emailDate { get; set; }
        public string DocumentType { get; set; }
        public string Reference { get; set; }
        public double Quantity { get; set; }
    }
}
