using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using EmailDownloader;
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
using TrackableEntities;
using TrackableEntities.Client;
using ValuationDS.Business.Entities;
using WaterNut.DataSpace;
using WaterNut.DataSpace.Asycuda;
using AsycudaDocument_Attachments = CoreEntities.Business.Entities.AsycudaDocument_Attachments;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
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
                {"CreatePOEntries",(ft, fs) => CreatePOEntries(ft.AsycudaDocumentSetId) },
                {"ExportPOEntries",(ft, fs) => ExportPOEntries(ft.AsycudaDocumentSetId) },
                {"AssessPOEntry",(ft, fs) => AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId)},
                {"EmailPOEntries",(ft, fs) => EmailPOEntries(ft.AsycudaDocumentSetId,ft.FileTypeContacts.Select(x => x.Contacts).ToList()) },
                {"DownloadSalesFiles",(ft, fs) => DownloadSalesFiles(false) },
                {"Xlsx2csv",(ft, fs) => Xlsx2csv(fs, ft) },
                {"SaveInfo",(ft, fs) => SaveInfo(fs, ft.AsycudaDocumentSetId) },
                {"CleanupEntries",(ft, fs) => CleanupEntries() },
                {"SubmitToCustoms",(ft, fs) => SubmitSalesXMLToCustoms() },
                {"MapUnClassifiedItems", (ft, fs) => MapUnClassifiedItems(ft,fs) },
                {"UpdateSupplierInfo", (ft, fs) => UpdateSupplierInfo(ft,fs) },
                {"ImportPDF", (ft, fs) => ImportPDF(fs,ft) },
                //{"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
                


            };




        public static Dictionary<string, Action> SessionActions =>
            new Dictionary<string, Action>
            {

                {"CreateDiscpancyEntries",() => CreateDiscpancyEntries(false) },
                {"RecreateDiscpancyEntries",() => CreateDiscpancyEntries(true) },
                {"AutoMatch", AutoMatch },
                {"AssessDiscpancyEntries", AssessDiscpancyEntries },
                {"ExportDiscpancyEntries", ExportDiscpancyEntries },
                {"SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", AllocateSales },
                {"CreateEx9",() => CreateEx9(false) },
                {"ExportEx9Entries", ExportEx9Entries },
                {"AssessEx9Entries", AssessEx9Entries },
                {"SubmitToCustoms", SubmitSalesXMLToCustoms },
                {"CleanupEntries", CleanupEntries },
                {"ClearAllocations", ClearAllocations },
                {"AssessDISEntries", AssessDISEntries },
                {"DownloadSalesFiles",() => DownloadSalesFiles(3) },
                {"ImportSalesEntries",ImportSalesEntries },
                {"SubmitDiscrepanciesToCustoms",SubmitDiscrepanciesToCustoms },
                {"DownloadPDFs",DownloadPDFs },
                {"LinkPDFs", LinkPDFs },
                {"RemoveDuplicateEntries", RemoveDuplicateEntries },
                {"FixIncompleteEntries", FixIncompleteEntries },
                {"EmailEntriesExpiringNextMonth", EmailEntriesExpiringNextMonth },
                {"EmailWarehouseErrors", EmailWarehouseErrors },
                {"ImportC71", ImportC71 },
                {"ImportLicense", ImportLicense },
                {"DownLoadC71", DownLoadC71 },
                {"DownLoadLicense", DownLoadLicence },
                {"CreateC71", CreateC71 },
                {"CreateLicense", CreateLicence },
                {"SubmitMissingInvoices", SubmitMissingInvoices },
                {"SubmitIncompleteEntryData", SubmitIncompleteEntryData },
                {"SubmitUnclassifiedItems", SubmitUnclassifiedItems },
                {"SubmitInadequatePackages", SubmitInadequatePackages },
                {"AssessC71", AssessC71 },
                {"AssessLicense", AssessLicense },
                {"AttachToDocSetByRef", AttachToDocSetByRef },
                {"CreatePOEntries",CreatePOEntries },
                {"ExportPOEntries", ExportPOEntries },
                {"AssessPOEntries", AssessPOEntries },
                {"DownloadPOFiles", DownloadPOFiles },
                {"SubmitPDFs", SubmitPDFs },
                {"RecreateEx9",() => CreateEx9(true) },
                {"ReDownloadSalesFiles",ReDownloadSalesFiles },
                {"CleanupDiscpancies", CleanupDiscpancies },
                {"SubmitDiscrepanciesPreAssessmentReportToCustoms", SubmitDiscrepanciesPreAssessmentReportToCustoms },
                {"ClearAllDiscpancyEntries", ClearAllDiscpancyEntries },
                {"ImportPDF", ImportPDF },

            };

        private static void SubmitPDFs()
        {
            try
            {


                Console.WriteLine("Submit PDFs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    
                        var lst = ctx.TODO_ImportCompleteEntries
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .GroupBy(x => x.AsycudaDocumentSetId)
                            .Join(ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z });
                        
                            foreach (var doc in lst)
                    {



                        var body = $"Please see attached entries for {doc.z.Declarant_Reference_Number}.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        //if (emailIds.Key == null)
                        //{
                        //    EmailDownloader.EmailDownloader.SendEmail(Client, "", "Error:Missing Invoices",
                        //        contacts, body, attlst.ToArray());
                        //}
                        //else
                        //{
                        //    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Error:Missing Invoices", body, contacts, attlst.ToArray());
                        //}


                        ctx.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void DownloadPOFiles()
        {
            DownloadSalesFiles(false); //download all for now
        }

        private static void ImportPDF()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var fileType = ctx.FileTypes
                   
                    .FirstOrDefault(x => x.Id == 17);
                var files = new FileInfo[]
                    {new FileInfo(@"C:\Users\josep\OneDrive\Clients\Budget Marine\Emails\30-15900\7006133.pdf")};
                ImportPDF(files,fileType);
            }
        }


        private static void ImportPDF(FileInfo[] csvFiles, FileTypes fileType)
            //(int? fileTypeId, int? emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSet, string fileType)
        {
            Console.WriteLine("Importing PDF " + fileType.Type);
            foreach (var file in csvFiles)
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
                InvoiceReader.Import(file.FullName, fileTypeId,emailId,true,SaveCSVModel.Instance.GetDocSets(fileType),fileType.Type);
            }
        }

        private static void ExportPOEntries()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                foreach (var docset in ctx.TODO_PODocSet.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                {
                    ExportPOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        private static void CreatePOEntries()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                foreach (var docset in ctx.TODO_PODocSet.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId ))
                {
                    CreatePOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        private static void AssessLicense()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSet.ToList();

                foreach (var doc in res)
                {

                    
                    var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "LIC-Instructions.txt");
                    if (!File.Exists(instrFile)) continue;
                    var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "LIC-InstructionResults.txt");
                    var lcont = 0;
                    while (AssessLICComplete(instrFile, resultsFile, out lcont) == false)
                    {
                        RunSiKuLi(doc.AsycudaDocumentSetId, "AssessLIC", lcont.ToString());
                    }
                }
            }
        }

        private static void AssessC71()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSet.ToList();

                foreach (var doc in res)
                {
                    
                    
                    var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "C71-Instructions.txt");
                    if (!File.Exists(instrFile)) continue;
                    var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "C71-InstructionResults.txt");
                    var lcont = 0;
                    while (AssessC71Complete(instrFile, resultsFile, out lcont) == false)
                    {
                        RunSiKuLi(doc.AsycudaDocumentSetId, "AssessC71", lcont.ToString());
                    }
                }
            }
        }

        private static void AttachToDocSetByRef()
        {
            //try
            //{
            //    using (var ctx = new VaContext())
            //    {
            //        //get C71 with just one docset

            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
        }


        private static void UpdateSupplierInfo(FileTypes ft, FileInfo[] fs)
        {
            using (var ctx = new EntryDataDSContext() { StartTracking = true })
            {
                foreach (var file in fs)
                {
                    var dt = CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["TariffCode"].ToString())) continue;
                        var supplierCode = row["SupplierCode"].ToString();
                        var itm = ctx.Suppliers.First(x => x.SupplierCode == supplierCode && x.ApplicationSettingsId ==
                                                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        itm.SupplierName = row["SupplierName"].ToString();
                        itm.SupplierName = row["SupplierAddress"].ToString();
                        itm.SupplierName = row["CountryCode"].ToString();
                        
                        ctx.SaveChanges();
                    }

                }
            }
        }

        private static void MapUnClassifiedItems(FileTypes ft, FileInfo[] fs)
        {
            using (var ctx = new InventoryDSContext(){StartTracking = true})
            {
                foreach (var file in fs)
                {
                    var dt = CSV2DataTable(file,"YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach(DataRow row in dt.Rows)
                    {
                        if(string.IsNullOrEmpty(row["TariffCode"].ToString())) continue;
                        var itmNumber = row["ItemNumber"].ToString();
                        var itm = ctx.InventoryItems.First(x => x.ItemNumber == itmNumber && x.ApplicationSettingsId ==
                                   BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        itm.TariffCode = row["TariffCode"].ToString();
                        ctx.SaveChanges();
                    }
                    
                }
            }
        }

        private static void CreateLicence()
        {
            try

            {
                Console.WriteLine("Create License Files");
                var pOs = CurrentPOInfo();

                using (var ctx = new LicenseDSContext())
                {
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = pO.Item2;
                        if (!Directory.Exists(directoryName)) continue;

                        
                        var lst = new CoreEntitiesContext().Database
                            .SqlQuery<TODO_LicenseToXML>(
                                $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {pO.Item1.AsycudaDocumentSetId}")
                            .ToList();
                        foreach (var itm in lst)
                        {
                            var fileName = Path.Combine(directoryName, $"{itm.EntryDataId}-LIC.xml");
                            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault() &&
                                File.Exists(fileName)) continue;
                            var contact = new CoreEntitiesContext().Contacts.FirstOrDefault(x =>
                                x.Role == "Broker" && x.ApplicationSettingsId == BaseDataModel.Instance
                                    .CurrentApplicationSettings.ApplicationSettingsId);
                            Suppliers supplier;

                            using (var ectx = new EntryDataDSContext())
                            {
                                //var elst = lst.Select(s => s.EntryDataId).ToList();
                                supplier = ectx.Suppliers.FirstOrDefault(x =>
                                    ectx.EntryData.Where(z => z.EntryDataId == itm.EntryDataId).Select(z => z.SupplierCode)
                                        .Any(z => z == x.SupplierCode) &&
                                    x.ApplicationSettingsId == pO.Item1.ApplicationSettingsId);
                            }

                            var lic = LicenseToDataBase.Instance.CreateLicense(new List<TODO_LicenseToXML>(){itm}, contact, supplier,
                                itm.EntryDataId);
                            var invoices = lst.Where(x => x.EntryDataId == itm.EntryDataId).Select(x => x.EntryDataId).Distinct().ToList();
                            ctx.xLIC_License.Add(lic);
                            ctx.SaveChanges();
                            LicenseToDataBase.Instance.ExportLicense(pO.Item1.AsycudaDocumentSetId, lic, fileName,
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



        private static void CreateC71()
        {
            try

            {
                Console.WriteLine("Create C71 Files");
                var pOs = CurrentPOInfo();
                using (var ctx = new ValuationDSContext())
                {
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = pO.Item2;
                        var fileName = Path.Combine(directoryName, "C71.xml");
                        if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault() && File.Exists(fileName)) continue;
                        var lst = new CoreEntitiesContext().TODO_C71ToXML.Where(x =>
                            x.ApplicationSettingsId == pO.Item1.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == pO.Item1.AsycudaDocumentSetId)
                            .GroupBy(x => x.AsycudaDocumentSetId)
                            .Where(x => (x.Sum(z => z.InvoiceTotal) > 184 && x.Any(z => z.Currency == "USD")) || (x.Sum(z => z.InvoiceTotal) > 500 && x.Any(z => z.Currency == "XCD"))).ToList();
                        var supplierCode = lst.SelectMany(x => x.Select(z => z.SupplierCode)).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                        Suppliers supplier = new Suppliers();
                        if (supplierCode == null)
                        {

                        }
                        else
                        {
                            supplier = new EntryDataDSContext().Suppliers.FirstOrDefault(x =>
                                x.SupplierCode == supplierCode &&
                                x.ApplicationSettingsId == pO.Item1.ApplicationSettingsId);
                        }
                        if(!lst.Any()) continue;
                        var c71 = C71ToDataBase.Instance.CreateC71(supplier, lst.SelectMany(x => x.Select(z => z)).ToList(), pO.Item1.Declarant_Reference_Number);
                        ctx.xC71_Value_declaration_form.Add(c71);
                        ctx.SaveChanges();
                        C71ToDataBase.Instance.ExportC71(pO.Item1.AsycudaDocumentSetId,c71, fileName);


                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        


        private static void DownLoadLicence()
        {
            try

            {
                var pOs = CurrentPOInfo();
                foreach (var pO in pOs)
                {
                    var directoryName = pO.Item2;
                    Console.WriteLine("Download License Files");
                    var lcont = 0;
                    while (ImportLICComplete(directoryName, out lcont) == false)
                    {
                        RunSiKuLi(pO.Item1.AsycudaDocumentSetId, "LIC", lcont.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void DownLoadC71()
        {
            try

            {
                var pOs = CurrentPOInfo();
                foreach (var pO in pOs)
                {
                    var directoryName = pO.Item2;
                    Console.WriteLine("Download C71 Files");
                    var lcont = 0;
                    while (ImportC71Complete(directoryName, out lcont) == false)
                    {
                        RunSiKuLi(pO.Item1.AsycudaDocumentSetId, "C71", lcont.ToString());
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

                    var lst = ctx.TODO_Error_IncompleteItems
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => new {x.ASYCUDA_Id, x.SourceFileName, x.AsycudaDocumentSetId});

                    foreach (var doc in lst)
                    {
                        BaseDataModel.Instance.DeleteAsycudaDocument(doc.Key.ASYCUDA_Id).Wait();
                        BaseDataModel.Instance.ImportDocuments(doc.Key.AsycudaDocumentSetId.GetValueOrDefault(),new List<string>(){doc.Key.SourceFileName},true,true,false,true,true).Wait();
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
                var salesinfo = CurrentSalesInfo();

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitXMLToCustoms.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.AssessmentDate >= salesinfo.Item1 && x.AssessmentDate <= salesinfo.Item2).ToList()
                        .GroupBy(x =>  x.EmailId );
                    foreach (var emailIds in lst)
                    {
                       
                        

                            var body = "The Following Ex-Warehouse Entries were Assessed. \r\n" +
                                       $"Start Date: {salesinfo.Item1} End Date {salesinfo.Item2}: \r\n" +
                                       "\tCNumber\t\tReference\t\tAssessmentDate\r\n" +
                                       $"{emailIds.Select(current => $"\t{current.CNumber}\t\t{current.ReferenceNumber}\t\t{current.RegistrationDate.Value:yyyy-MM-dd} \r\n").Aggregate((old, current) => old + current)}" +
                                       $"\r\n" +
                                       $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
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

        private static void SubmitDiscrepanciesToCustoms()
        {
            try
            {


                Console.WriteLine("Submit Discrepancies To Customs");

                // var saleInfo = CurrentSalesInfo();
                

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitDiscrepanciesToCustoms.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId).ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Discrepancies Entries were Assessed. \r\n" +
                                   
                                   $"\t{"CNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"AssessmentDate".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.ReferenceNumber.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"\r\n" +
                                   $"Please open the attached email to view Email Thread.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";


                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Assessed Shipping Discrepancy Entries",
                                contacts, body, new string[0]);
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Assessed Shipping Discrepancy Entries", body, new string[0], contacts);
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

                
                var info = CurrentSalesInfo();
                var directory = info.Item4;

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs" || x.Role == "Clerk").Select(x => x.EmailAddress).ToArray();
                    var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId && x.Type == "DIS").ToList();
                    var errors = ctx.TODO_SubmitDiscrepanciesErrorReport.Where(x =>
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
                            comment = x.comment,
                            Status = x.Status,
                            DutyFreePaid = x.DutyFreePaid,
                            subject = x.subject,
                            emailDate = x.emailDate

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
                            emailDate = x.EmailDate,
                            Reference = x.Reference,
                            DocumentType = x.DocumentType,

                        })
                        .ToList();

                    var errBreakdown = errors.GroupBy(x => x.Status);
                    //foreach (var emailIds in lst)
                    //{



                    var body = $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                               $"\r\n" +
                               $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(20)}{"Percentage".FormatedSpace(20)}\r\n" +
                               $"{errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(20)}{(Math.Round((double)(((double)current.Count()/ (double)totaladjustments.Count())*100),0)).ToString().FormatedSpace(20)}% \r\n").Aggregate((old, current) => old + current)}" +
                               $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(20)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n" +
                               $"\r\n" +
                               $"Please see attached for list of Errors and Executions details.\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";

                    var errorfile = Path.Combine(directory, "DiscrepancyExecutionErrors.csv");

                    var errRes = new ExportToCSV<SubmitDiscrepanciesErrorReport, List<SubmitDiscrepanciesErrorReport>>();
                    errRes.dataToPrint = errors;
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                    }

                    var goodfile = Path.Combine(directory, "DiscrepancyExecutions.csv");

                    var goodRes = new ExportToCSV<DiscrepancyPreExecutionReport, List<DiscrepancyPreExecutionReport>>();
                    goodRes.dataToPrint = goodadj;
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => goodRes.SaveReport(goodfile), CancellationToken.None, TaskCreationOptions.None, sta);
                    }

                    if ((errors.Any() && File.Exists(errorfile)) && (goodadj.Any() && File.Exists(goodfile)))
                        EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Discrepancy Pre-Assessment Report for  {totaladjustments.Min(x => x.EffectiveDate)} To: {totaladjustments.Max(x => x.EffectiveDate)}", contacts.ToArray(), body, new string[] { errorfile, goodfile });

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitIncompleteEntryData()
        {
            try
            {


                Console.WriteLine("Submit Incomplete Entry Data");

                // var saleInfo = CurrentSalesInfo();
                

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitIncompleteEntryData.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId).ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Invoices Total do not match Imported Totals . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Supplier Code".FormatedSpace(20)}{"Invoice Total".FormatedSpace(20)}{"Imported Total".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SupplierCode.FormatedSpace(20)}{current.InvoiceTotal.Value.ToString("C").FormatedSpace(20)}{current.ImportedTotal.Value.ToString("C").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
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

        private static void SubmitMissingInvoices()
        {
            try
            {


                Console.WriteLine("Submit Missing Invoices");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitDocSetWithIncompleteInvoices.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId).ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {
                        


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Invoices. {emailIds.FirstOrDefault().ImportedInvoices} were Imported out of {emailIds.FirstOrDefault().TotalInvoices} . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Invoice Date".FormatedSpace(20)}{"Invoice Value".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.InvoiceDate.ToShortDateString().FormatedSpace(20)}{current.InvoiceTotal.Value.ToString().FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
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
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Error:Missing Invoices", body, contacts, attlst.ToArray());
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

        public static void SubmitUnclassifiedItems()
        {



            var info = CurrentSalesInfo();
            var directory = info.Item4;
            



            using (var ctx = new CoreEntitiesContext())
            {

                var emails = ctx.TODO_SubmitUnclassifiedItems
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .GroupBy(x => x.EmailId).ToList();
                foreach (var email in emails)
                {
                    var errorfile = Path.Combine(directory, $"UnclassifiedItems-{email.Key}.csv");
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
                        EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(email.Key),Client, 
                            $"Error:UnClassified Items", "Please Fill out the attached Tarrif Codes and resend CSV...",
                            contacts.Select(x => x.EmailAddress).ToArray(),
                            new string[] {errorfile});

                    
                }
            }



        }

        public static void SubmitIncompleteSuppliers()
        {



            var info = CurrentSalesInfo();
            var directory = info.Item4;




            using (var ctx = new EntryDataDSContext())
            {

                var suppliers = ctx.Suppliers
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.CountryCode == null || x.SupplierName == null || x.Street == null);
                    
                
                    var errorfile = Path.Combine(directory, $"IncompleteSuppliers.csv");
                    var errors = suppliers.Select(x => new IncompleteSupplier()
                    {
                        SupplierName = x.SupplierName,
                        SupplierCode = x.SupplierCode,
                        SupplierAddress = x.Street,
                        CountryCode = x.CountryCode
                    }).ToList();


                    var res =
                        new ExportToCSV<IncompleteSupplier,
                            List<IncompleteSupplier>>();
                    res.dataToPrint = errors;
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    var contacts = new CoreEntitiesContext().Contacts.Where(x => x.Role == "Broker" || x.Role == "PO Clerk").ToList();
                    if (File.Exists(errorfile))
                        EmailDownloader.EmailDownloader.SendEmail(Client,  directory, 
                            $"Error:InComplete Supplier Info",contacts.Select(x => x.EmailAddress).ToArray(), "Please Fill out the attached Supplier Info and resend CSV...",
                            new string[] { errorfile });


                }
            



        }

        private static void SubmitInadequatePackages()
        {
            try
            {
                Console.WriteLine("Submit Inadequate Packages");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId).ToList();
                        
                    foreach (var docSet in lst)
                    {



                        var body = $"Error: Inadequate Lines\r\n" +
                                   $"\r\n" +
                                   $"The {docSet.Declarant_Reference_Number} has {docSet.TotalPackages} Packages with {docSet.TotalLines} Lines.\r\n" +
                                   $"Your Maximum Lines per Entry is {docSet.MaxEntryLines}.\r\n" +
                                   $"The minium required packages is {docSet.RequiredPackages}\r\n" +
                                   $"Please increase the Maxlines using \"MaxLines:\"\r\n" +
                                   $"Please note the System will automatically switch from \"Entry per Invoice\" to \"Group Invoices per Entry\", if there are not enough packages per invoice. \r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
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
                    .Select(x => new {x.ASYCUDA_Id, x.AsycudaDocumentSetId}).ToList();
                foreach (var itm in lst)
                {
                    using (var dtx = new DocumentDSContext())
                    {
                        var docEds = dtx.AsycudaDocumentEntryDatas.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).ToList();
                        foreach (var ed in docEds)
                        {
                            var docsetEd = dtx.AsycudaDocumentSetEntryDatas.First(x =>
                                x.AsycudaDocumentSetId == itm.AsycudaDocumentSetId && x.EntryDataId == ed.EntryDataId);
                            dtx.AsycudaDocumentSetEntryDatas.Remove(docsetEd);
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

            var info = CurrentSalesInfo();
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
                               $"\t{"CNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"Document Type".FormatedSpace(20)}{"RegistrationDate".FormatedSpace(20)}{"ExpiryDate".FormatedSpace(20)}\r\n" +
                               $"{errors.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.Reference.FormatedSpace(20)}{current.DocumentType.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)}{current.ExpiryDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                               $"\r\n" +
                               $"{Client.CompanyName} is kindly requesting these Entries be extended an additional 730 days to facilitate ex-warehousing. \r\n" +
                               $"\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Entries Expiring {DateTime.Now.ToString("yyyy-MM-dd")} - {DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[] { errorfile });
                }
            }

        }

        public static void EmailWarehouseErrors()
        {

            var info = CurrentSalesInfo();
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
                               $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
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

            var info = CurrentSalesInfo();
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

            var info = CurrentSalesInfo();
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

        public static void EmailPOEntries(int asycudaDocumentSetId, List<Contacts> contacts)
        {
            try
            {

                
                if (!contacts.Any() || BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true) return;
                if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                var lst = CurrentPOInfo();
                foreach (var poInfo in lst.Where(x => x.Item1.AsycudaDocumentSetId == asycudaDocumentSetId))
                {

                    if (poInfo.Item1 == null) return;
                    
                    var reference = poInfo.Item1.Declarant_Reference_Number;
                    var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        reference);
                    if (!Directory.Exists(directory)) continue;
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

                }
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
                    var lst = ctx.TODO_ImportCompleteEntries
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs, x => x.Key, z => z.AsycudaDocumentSetId, (x,z)=> new {x,z}).ToList();
                    foreach (var doc in lst)
                    {
                        var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, doc.z.Declarant_Reference_Number)); ;
                        Console.WriteLine("Download PDF Files");
                        var lcont = 0;
                        while (ImportPDFComplete(directoryName, out lcont) == false)
                        {
                            RunSiKuLi(doc.z.AsycudaDocumentSetId, "IM7-PDF", lcont.ToString());
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

        private static void LinkPDFs()
        {
            try

            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var lst = ctx.TODO_ImportCompleteEntries
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs, x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new {x, z})
                        .ToList();
                    foreach (var doc in lst)
                    {
                        var directoryName = StringExtensions.UpdateToCurrentUser(
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                doc.z.Declarant_Reference_Number));
                        ;
                        Console.WriteLine("Link PDF Files");
                        
                            var csvFiles = new DirectoryInfo(directoryName).GetFiles()
                                .Where(x => Regex.IsMatch(x.FullName,
                                    @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<CNumber>\d+).*.pdf",
                                    RegexOptions.IgnoreCase)).ToArray();
                            foreach (var file in csvFiles)
                            {
                                var dfile = ctx.Attachments.FirstOrDefault(x => x.FilePath == file.FullName);
                                if (dfile != null) continue;
                                var mat = Regex.Match(file.FullName,
                                    @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<CNumber>\d+).*.pdf",
                                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                                if (!mat.Success) continue;

                                var cnumber = mat.Groups["CNumber"].Value;
                                var cdoc = ctx.AsycudaDocuments.FirstOrDefault(x => x.CNumber == cnumber);
                                if (cdoc == null) continue;


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
            DownloadSalesFiles(true);
        }

        public static void DownloadSalesFiles(bool redownload)
        {
            try

            {
                var directoryName = CurrentSalesInfo().Item4;
                Console.WriteLine("Download Entries");
                var lcont = 0;
                while (ImportComplete(directoryName,redownload, out lcont) == false)
                {
                    RunSiKuLi(CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void DownloadSalesFiles(int trytimes)
        {
            try

            {
                var directoryName = CurrentSalesInfo().Item4;
                Console.WriteLine("Download Entries");
                var lcont = 0;
                
                for (int i = 0; i < trytimes; i++)
                {
                    ImportComplete(directoryName,false, out lcont);
                    RunSiKuLi(CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
                    if (ImportComplete(directoryName, false ,out lcont)) break;
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
            var saleinfo = CurrentSalesInfo();
            AssessSalesEntry(saleinfo.Item3.Declarant_Reference_Number, saleinfo.Item3.AsycudaDocumentSetId);
        }

        public static void AssessDISEntries()
        {
            Console.WriteLine("Assessing Discrepancy Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_DiscrepanciesToAssess.ToList();
                foreach (var doc in res)
                {
                    AssessPOEntry(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                }
                
            }

        }

        public static void AssessPOEntries()
        {
            Console.WriteLine("Assessing PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSetToExport.ToList();
                foreach (var doc in res)
                {
                    AssessPOEntry(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                }

            }

        }

        public static void AssessPOEntry(string docReference, int asycudaDocumentSetId)
        {
            if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;

            if (docReference == null) return;
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "Instructions.txt");

            var lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
            }


        }

        public static void AssessSalesEntry(string docReference, int asycudaDocumentSetId)
        {
            if (docReference == null) return;
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "Instructions.txt");

            var lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
            }


        }

        public static bool ImportLICComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            if (File.Exists(Path.Combine(desFolder, "LICOverView-PDF.txt")))
            {
                var lines = File.ReadAllText(Path.Combine(directoryName, "LICOverView-PDF.txt"))
                    .Split(new[] { $"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                if (lines.FirstOrDefault() == "No Data") return true;
                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.Split('\t');
                    if (p.Length < 5) continue;
                    if (string.IsNullOrEmpty(p[3])) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[3]}-LIC.xml")))
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
            if (File.Exists(Path.Combine(desFolder, "C71OverView-PDF.txt")))
            {
                var lines = File.ReadAllText(Path.Combine(directoryName, "C71OverView-PDF.txt"))
                    .Split(new[] { $"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

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

            var desFolder = directoryName + "\\";
            var overviewFile = Path.Combine(desFolder, "OverView.txt");
            if (File.Exists(overviewFile))
            {
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

                if (redownload &&  (DateTime.Now - new FileInfo(overviewFile).LastWriteTime).Minutes > 5) return false;
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

                    if (p[1] == r[1] && r.Length == 5 && r[4] == "Success")//for attachment
                    {
                        lcont += 1;
                        continue;
                    }
                    if (p[1] == r[1] && r.Length == 3 && r[2] == "Success")// for file
                    {
                        lcont += 1;
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
                    if(string.IsNullOrEmpty(res[lcont])) return false;
                    var r = res[lcont].Split('\t');
                    
                    if (p[1] == r[1] && r.Length == 3 && r[2] == "Success")
                    {
                        lcont += 1;
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

        public static void ImportSalesEntries()
        {
            Console.WriteLine("Import Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docSetId = CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                var ft = ctx.FileTypes.FirstOrDefault(x => x.Type == "XML" && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (ft == null) return;
                var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                        .Declarant_Reference_Number);
                var csvFiles = new DirectoryInfo(desFolder).GetFiles().Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase)).ToArray();
                if (csvFiles.Length > 0)
                    BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId, csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();
                RemoveDuplicateEntries();
                FixIncompleteEntries();
            }
        }

        public static void ImportC71()
        {
            Console.WriteLine("Import C71");
            using (var ctx = new CoreEntitiesContext())
            {
                var docSets = CurrentPOInfo();
                foreach (var poInfo in docSets)
                {
                    if (poInfo.Item1 == null) return;
                    var reference = poInfo.Item1.Declarant_Reference_Number;
                    var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        reference);
                    if (!Directory.Exists(directory)) continue;
                    
                    var ft = ctx.FileTypes.FirstOrDefault(x => x.Type == "C71" && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == poInfo.Item1.AsycudaDocumentSetId)
                            .Declarant_Reference_Number);
                    var lastC71 = new DirectoryInfo(desFolder).GetFiles().LastOrDefault(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase));
                    var csvFiles = new FileInfo[]{ lastC71 };

                    if (csvFiles.Length > 0)
                    {
                        
                        BaseDataModel.Instance.ImportC71(poInfo.Item1.AsycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList());
                        ft.AsycudaDocumentSetId = poInfo.Item1.AsycudaDocumentSetId;
                        BaseDataModel.Instance.SaveAttachedDocuments(csvFiles, ft).Wait();

                    }


                }
            }
        }

        public static void ImportLicense()
        {
            Console.WriteLine("Import License");
            using (var ctx = new CoreEntitiesContext())
            {
                var docSets = CurrentPOInfo();
                foreach (var poInfo in docSets)
                {
                    if (poInfo.Item1 == null) return;
                    var reference = poInfo.Item1.Declarant_Reference_Number;
                    var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        reference);
                    if (!Directory.Exists(directory)) continue;

                    var ft = ctx.FileTypes.FirstOrDefault(x => x.Type == "LIC" && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == poInfo.Item1.AsycudaDocumentSetId)
                            .Declarant_Reference_Number);
                    var csvFiles = new DirectoryInfo(desFolder).GetFiles().Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase)).ToArray();
                    if (csvFiles.Length > 0)
                    {
                        BaseDataModel.Instance.ImportLicense(poInfo.Item1.AsycudaDocumentSetId, csvFiles.Select(x => x.FullName).ToList());
                        ft.AsycudaDocumentSetId = poInfo.Item1.AsycudaDocumentSetId;
                        BaseDataModel.Instance.SaveAttachedDocuments(csvFiles, ft).Wait();
                    }

                }
            }
        }

        public static void CreateEx9(bool overwrite)
        {
            Console.WriteLine("Create Ex9");
           
            var saleInfo = CurrentSalesInfo();
            if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;
            
            var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;
            if (overwrite)
            {
                BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
                //BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0); don't overwrite previous entries
            }

            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.Database.SqlQuery<string>(
                    $@"SELECT EX9AsycudaSalesAllocations.ItemNumber
                    FROM    EX9AsycudaSalesAllocations INNER JOIN
                                     ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                                     EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                                     AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
                    WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                                     (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                                     EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                                     (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) AND (EX9AsycudaSalesAllocations.DocumentType = 'IM7' OR
                                     EX9AsycudaSalesAllocations.DocumentType = 'OS7') AND (AllocationErrors.ItemNumber IS NULL) AND (ApplicationSettings.ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}) AND (EX9AsycudaSalesAllocations.InvoiceDate >= '{saleInfo.Item1.ToShortDateString()}') AND 
                                     (EX9AsycudaSalesAllocations.InvoiceDate <= '{saleInfo.Item2.ToShortDateString()}')
                    GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId--, EX9AsycudaSalesAllocations.pQuantity, EX9AsycudaSalesAllocations.PreviousItem_Id
                    HAVING (SUM(EX9AsycudaSalesAllocations.PiQuantity) < SUM(EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL)");
                if (!res.Any()) return;
            }


            var filterExpression =
                $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                $"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                $" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                "&& (TaxAmount == 0 || TaxAmount != 0)" +
                "&& PreviousItem_Id != null" +
                "&& (xBond_Item_Id == 0)" +
                "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                "&& (PiQuantity < pQtyAllocated)" +
                "&& (Status == null || Status == \"\")" +
                (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                    ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                    : "") +
                (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue
                    ? $" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\""
                    : "");

            
           
            AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, false, docset).Wait();
        }


        public static void CleanupDiscpancies()
        {
            try
            {
                Console.WriteLine("Clean up Discrepancies");
                
                using (var ctx = new CoreEntitiesContext())
                {
                    var lst = ctx.TODO_DiscrepanciesAlreadyXMLed.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                            //    && x.AsycudaDocumentSetId == 1462
                            //     && x.InvoiceNo == "53371108"
                            //&& x.InvoiceDate >= saleInfo.Item1
                            //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        .GroupBy(x => new {x.AsycudaDocumentSetId, x.InvoiceNo}).ToList();

                    using (var dtx = new DocumentDSContext())
                    {
                        foreach (var doc in lst)
                        {
                            var res = dtx.AsycudaDocumentSetEntryDatas.First(x =>
                                x.AsycudaDocumentSetId == doc.Key.AsycudaDocumentSetId
                                && x.EntryDataId == doc.Key.InvoiceNo);
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

        public static void ClearAllDiscpancyEntries()
        {
            Console.WriteLine("Clear DIS Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        && x.Type == "DIS")
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

        public static void CreateDiscpancyEntries(bool overwrite)
        {
            Console.WriteLine("Create DIS Short Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_AdjustmentsToXML.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        && x.AdjustmentType == "DIS"
                    //   && x.AsycudaDocumentSetId == 1462
                    //     && x.InvoiceNo == "53371108"
                    //&& x.ItemNumber == "TOH/99998PBA2M"
                //&& x.InvoiceDate >= saleInfo.Item1
                //    &&  x.InvoiceDate <= saleInfo.Item2
                ).ToList();
                    // group bugging and leaving out invoice nos
                    //.GroupBy(x => new { x.AsycudaDocumentSetId, x.InvoiceNo }).ToList();
                if (overwrite)
                {
                    foreach (var doc in lst.Select(x => x.AsycudaDocumentSetId).Distinct())
                    {
                        BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait();
                        BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                    }
                }

                foreach (var doc in lst)
                {
                    var filterExpression =
                        $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                        //$"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                        //$" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                        $" && EntryDataId == \"{doc.InvoiceNo}\"";
                    
                    new AdjustmentShortService().CreateIM9(filterExpression, true, false, doc.AsycudaDocumentSetId, "IM4-801", "Duty Free").Wait();
                    new AdjustmentOverService().CreateOPS(filterExpression, true, doc.AsycudaDocumentSetId).Wait();

                }

            }

        }

        public static void ExportDiscpancyEntries()
        {
            Console.WriteLine("Export Discrepancy Entries");

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

                    BaseDataModel.Instance.ExportDocSet(doc.Key.AsycudaDocumentSetId,
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Key.Declarant_Reference_Number), true).Wait();
                    ExportDocSetSalesReport(doc.Key.AsycudaDocumentSetId,
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Key.Declarant_Reference_Number)).Wait();

                }

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

        public static void SubmitDiscrepancyErrors()
        {
            try
            {


                Console.WriteLine("Submit Discrepancy Entries");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.TODO_DiscrepanciesToSubmit.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.AdjustmentType == "DIS").ToList()
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
                                       $"\t{"Item Number".FormatedSpace(20)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"CNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                                       $"{doc.Select(current => $"\t{current.ItemNumber.FormatedSpace(20)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.CNumber.FormatedSpace(15)}{current.Comment.FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
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

                    .Include("FileTypes.FileTypeMappings").First(x => x.ApplicationSettingsId == id);
                
                    // set BaseDataModel CurrentAppSettings
                    BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                    //check emails
                
            }
        }

        public static Tuple<DateTime, DateTime, AsycudaDocumentSetEx, string> CurrentSalesInfo()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23);
            var docRef = startDate.ToString("MMMM") + " " + startDate.Year.ToString();
            var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
            if (docSet == null)
            {
                using (var ctx = new DocumentDSContext())
                {
                    var doctype = ctx.Document_Type.Include(x => x.Customs_Procedure).First(x =>
                        x.Type_of_declaration == "IM" && x.Declaration_gen_procedure_code == "7");
                    ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{docRef}',{doctype.Document_TypeId},{
                            doctype.Customs_Procedure.First(z => z.IsDefault == true).Customs_ProcedureId
                        },0)");
                    
                }
            }
            var dirPath = StringExtensions.UpdateToCurrentUser( Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docRef));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return new Tuple<DateTime, DateTime, AsycudaDocumentSetEx, string>(startDate, endDate, docSet, dirPath);
        }

        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo()
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var poDocSet = ctx.TODO_PODocSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (poDocSet != null)
                    {
                        var docSet = ctx.AsycudaDocumentSetExs.Where(x =>
                            poDocSet.Any(z => z.AsycudaDocumentSetId == x.AsycudaDocumentSetId));
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

        public static void CreatePOEntries(int asycudaDocumentSetId)
        {
            
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                    {
                        if (ctx.TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;

                    }
                    Console.WriteLine("CreatePOEntries");
                    var res = ctx.ToDo_POToXML.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == asycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new
                        {
                            DocSetId = x.Key,
                            Entrylst = x.Select(z => new { z.EntryDataDetailsId, z.IsClassified }).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        
                        BaseDataModel.Instance.ClearAsycudaDocumentSet(docSetId.DocSetId).Wait();
                        BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docSetId.DocSetId, 0);

                        //BaseDataModel.Instance.AddToEntry(
                        //        docSetId.Entrylst.Where(x => x.IsClassified == true).Select(x => x.EntryDataDetailsId),
                        //        docSetId.DocSetId,
                        //        BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true).Wait();
                        //BaseDataModel.Instance.AddToEntry(
                        //        docSetId.Entrylst.Where(x => x.IsClassified == false).Select(x => x.EntryDataDetailsId),
                        //        docSetId.DocSetId,
                        //        BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true)
                        //    .Wait();


                        BaseDataModel.Instance.AddToEntry(
                               (IEnumerable<int>) docSetId.Entrylst.Select(x => x.EntryDataDetailsId),
                                docSetId.DocSetId,
                               (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true), true).Wait();

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ExportPOEntries(int asycudaDocumentSetId)
        {
            
            try
            {
                using (var ctx = new DocumentDS.Business.Entities.DocumentDSContext())
                {
                    
                    IQueryable<xcuda_ASYCUDA> docs;
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true)
                    {
                        if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                            x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                        Console.WriteLine("Export PO Entries");
                        docs = ctx.xcuda_ASYCUDA
                                .Include(x => x.xcuda_Declarant)
                                
                                .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                            && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId == asycudaDocumentSetId
                                            && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                            && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type.Type_of_declaration == "IM"
                                            && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type.Declaration_gen_procedure_code == "7")
                            ;
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
                                        && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type
                                            .Type_of_declaration == "IM"
                                        && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type
                                            .Declaration_gen_procedure_code == "7");
                    }

                    var res = docs.GroupBy(x => new
                            {
                                x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId,
                                ReferenceNumber = x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                    .Declarant_Reference_Number
                            }).Select(x => new
                        {
                            DocSet = x.Key,
                            Entrylst = x.Select(z => new { z, ReferenceNumber = z.xcuda_Declarant.Number }).ToList()
                        })
                        .ToList();


                    foreach (var docSetId in res)
                    {
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            docSetId.DocSet.ReferenceNumber);
                        if (!Directory.Exists(directoryName)) continue;
                        if(File.Exists(Path.Combine(directoryName, "Instructions.txt"))) File.Delete(Path.Combine(directoryName, "Instructions.txt"));
                        foreach (var item in docSetId.Entrylst)
                        {
                            var expectedfileName = Path.Combine(directoryName, item.ReferenceNumber + ".xml");
                            //if (File.Exists(expectedfileName)) continue;
                            BaseDataModel.Instance.ExportDocument(expectedfileName, item.z).Wait();

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

        public static void ExportEx9Entries()
        {
            Console.WriteLine("Export EX9 Entries");
            try
            {
                var i = CurrentSalesInfo();
                BaseDataModel.Instance.ExportDocSet(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number), false).Wait();
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

        public static ObservableCollection<SaleReportLine> IM9AdjustmentsReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AllocationQSContext())
                {
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
                            .Where(x => x.pItemNumber.Length <= 17) // to match the entry
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
                                SalesType = "Duty Free",
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
                            .Where(x => x.pItemNumber.Length <= 17) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                CustomerName = s.CustomerName,
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



        public static void SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {

                    foreach (var file in csvFiles)
                    {

                        var attachment =
                            ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                                x.Attachments.FilePath == file.FullName &&
                                x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);

                        using (var ctx1 = new CoreEntitiesContext() {StartTracking = true})
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
                                            Reference = file.Name.Replace(file.Extension, ""),
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

        public static FileInfo[] CanSaveFileInfo(FileInfo[] csvFiles)
        {
            var res = new List<FileInfo>();
            
                using (var ctx = new CoreEntitiesContext())
                {
                    var infos = ctx.InfoMapping.Where(x => x.EntityType == "AsycudaDocumentSet")
                        .ToList();
                foreach (var file in csvFiles)
                {
                    var fileTxt = File.ReadAllLines(file.FullName);
                    foreach (var line in fileTxt)
                    {
                        var match = Regex.Match(line,
                            @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))", RegexOptions.IgnoreCase);
                        if (match.Success)
                            foreach (var infoMapping in infos.Where(x =>
                                x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower()))
                            {
                                res.Add(file);
                            }
                    }

                    
                }
            }
            return res.ToArray();
        }
        public static void SaveInfo(FileInfo[] csvFiles, int oldDocSet)
        {
            foreach (var file in csvFiles.Where(x => x.Name == "Info.txt"))
            {
                var fileTxt = File.ReadAllLines(file.FullName);
                SaveAsycudaDocumentSet(oldDocSet, fileTxt);
                // SaveEntryData(oldDocSet, fileTxt);
            }
        }

        public static void SaveAsycudaDocumentSet(int oldDocSet, string[] fileTxt)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.InfoMapping.Where(x => x.EntityType == "AsycudaDocumentSet")
                    .ToList();
                string dbStatement = "";
                foreach (var line in fileTxt)
                {
                    var match = Regex.Match(line,
                        @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))", RegexOptions.IgnoreCase);
                    if (match.Success)
                        foreach (var infoMapping in res.Where(x =>
                            x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower()))
                        {
                            dbStatement +=
                                $@" Update AsycudaDocumentSet Set {infoMapping.Field} = '{
                                        ReplaceSpecialChar(match.Groups["Value"].Value.Trim(),
                                            "")
                                    }' Where AsycudaDocumentSetId = '{oldDocSet}';";
                        }
                }

                if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
            }
        }

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



        public static void RunSiKuLi(int docSetId, string scriptName, string lastCNumber = "")
        {
            try
            {

                if (docSetId == 0) return;

                Console.WriteLine($"Executing {scriptName}");
                var docRef = new AsycudaDocumentSetExService().GetAsycudaDocumentSetExByKey(docSetId.ToString()).Result
                    .Declarant_Reference_Number;

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "java.exe";
                startInfo.Arguments =
                    $@"-jar C:\Users\{Environment.UserName}\OneDrive\Clients\AutoBot\sikulix.jar -r C:\Users\{
                            Environment.UserName
                        }\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                            BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin
                        } {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} {
                            (string.IsNullOrEmpty(lastCNumber) ? "" : lastCNumber + " ")
                        }""{Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docRef) + "\\"}";
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                process.Start();
                var timeoutCycles = 0;
                while (!process.HasExited && process.Responding)
                {
                    if (timeoutCycles > 12) break;
                    Console.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Thread.Sleep(1000 * 60);
                    timeoutCycles += 1;
                }

                if (!process.HasExited) process.Kill();

                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA"))
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
                    SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType,  true)
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
                            var att = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed","")));
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
            var dt = CSV2DataTable(file,"NO");
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
            var dic = new Dictionary<string, Func<Dictionary<string, string>, string>>()
            {
                {"CurrentDate", (dt)=> DateTime.Now.Date.ToShortDateString() },
                { "DIS-Reference", (dt) => $"DIS-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"},
                { "ADJ-Reference", (dt) => $"ADJ-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"},
                {"Quantity", (dt) => Convert.ToString(Math.Abs(Convert.ToDouble(dt["Received Quantity"].Replace("\"","")) - Convert.ToDouble(dt["Invoice Quantity"].Replace("\"",""))), CultureInfo.CurrentCulture) },
                {"ZeroCost", (x) => "0" },
                {"ABS-Added", (dt) => Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                {"ABS-Removed", (dt) => Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                {"ADJ-Quantity", (dt) => Convert.ToString(Math.Abs((Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))) - Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))))), CultureInfo.CurrentCulture) },
                {"Cost2USD", (dt) => dt.ContainsKey("{XCDCost}") && Convert.ToDouble(dt["{XCDCost}"].Replace("\"","")) > 0 ? (Convert.ToDouble(dt["{XCDCost}"].Replace("\"",""))/2.7169).ToString(CultureInfo.CurrentCulture) : "{NULL}" },
            };

            foreach (var file in files)
            {
                var dfile = new FileInfo($@"{file.DirectoryName}\{file.Name.Replace(file.Extension, ".csv")}");
                if (dfile.Exists && dfile.LastWriteTime >= file.LastWriteTime) return;
                // Reading from a binary Excel file (format; *.xlsx)
                FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                excelReader.Close();

                string a = "";
                int row_no = 0;

                while (row_no < result.Tables[0].Rows.Count)
                {
                    if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
                            .All(x => result.Tables[0].Rows[row_no].ItemArray.Contains(x)))
                    {
                        //if(dic.ContainsKey())
                        a = "";
                    }

                    for (int i = 0; i < result.Tables[0].Columns.Count; i++)
                    {

                        a += StringToCSVCell(result.Tables[0].Rows[row_no][i].ToString()) + ",";
                    }
                    row_no++;
                    a += "\n";


                }
                string output = Path.ChangeExtension(file.FullName, ".csv");
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(a);
                csv.Close();

                FixCsv(new FileInfo(output), fileType, dic);

            }
        }

        public static void FixCsv(FileInfo file, FileTypes fileType,
            Dictionary<string, Func<Dictionary<string, string>, string>> dic)
        {
            try
            {


                if (fileType.FileTypeMappings.Count == 0)
                {
                    throw new ApplicationException($"Missing File Type Mappings for {fileType.FilePattern}");
                    
                }
                var dfile = new FileInfo(
                    $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}");
                if (dfile.Exists && dfile.LastWriteTime >= file.LastWriteTime) return;
                // Reading from a binary Excel file (format; *.xlsx)
                var dt = CSV2DataTable(file,"NO");


                string table = "";
                int row_no = 0;
                var dRows = new List<DataRow>();


                //delete rows till header
                while (row_no < dt.Rows.Count)
                {
                    if (dt.Rows[row_no].ItemArray.Count(x => !string.IsNullOrEmpty(x.ToString())) >=
                        fileType.FileTypeMappings.Count(x => x.Required))
                        dRows.Add(dt.Rows[row_no]);

                    row_no++;

                }
                // delete duplicate headers

                var dupheaders = dRows.Where(x =>
                        x.ItemArray.Contains(fileType.FileTypeMappings.OrderBy(z => z.Id).First(z => !z.OriginalName.Contains("{")).OriginalName)).Skip(1)
                    .ToList();
                foreach (var row in dupheaders)
                {
                    dRows.Remove(row);
                }

                row_no = 0;
                var header = dRows[0];
                for (int i = 0; i < header.ItemArray.Length; i++)
                {
                    header[i] = header[i].ToString().ToUpper();
                }

                //if (fileType.FileTypeMappings.Where(x => x.Required)
                //    .Any(item => !header.ItemArray.Contains(item.OriginalName.ToUpper())))
                //    return; // not right file type


                while (row_no < dRows.Count)
                {
                    var row = new Dictionary<string, string>();
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
                                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                        new[] {"Josephbartholomew@outlook.com"},
                                        $"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} dose not exists.",
                                        Array.Empty<string>());
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
                                if (dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
                                {

                                    val += dic[map.Replace("{", "").Replace("}", "")].Invoke(row);
                                }
                                else
                                {
                                    var index = Array.LastIndexOf(header.ItemArray, map.ToUpper()); //last index of because of Cost USD file has two columns
                                    val += dRows[row_no][index];
                                }

                                if (maps.Length > 1 && map != maps.Last()) val += " - ";
                            }


                        }

                        if (val == "{NULL}") continue;
                        if (val == "" && row_no == 0) continue;
                        if (val == "" && row_no > 0 && (!header.ItemArray.Contains(mapping.OriginalName.ToUpper()) &&
                                                        !dic.ContainsKey(mapping.OriginalName.Replace("{", "").Replace("}", "")))) continue;
                        if (row_no > 0)
                            if (string.IsNullOrEmpty(val) && mapping.Required == true)
                            {
                                if (mapping.DestinationName == "Invoice #")
                                {
                                    val += dic["DIS-Reference"].Invoke(row);

                                }
                                else
                                {
                                    //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                    //    new[] { "Josephbartholomew@outlook.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                                    
                                    break;
                                }

                            }

                            else if (mapping.DataType == "Number")
                            {
                                if (string.IsNullOrEmpty(val)) val = "0";
                                if (val.ToCharArray().All(x => !char.IsDigit(x)))
                                {
                                    //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                    //    new[] { "Josephbartholomew@outlook.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                                    break;
                                    //val = "";
                                }
                            }
                            else if (mapping.DataType == "Date")
                            {
                                if (DateTime.TryParse(val, out var tmp) == false)
                                {
                                    //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                                    //    new[] { "Josephbartholomew@outlook.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                                    break;
                                    //  val = "";
                                }
                            }

                        if (row.ContainsKey(mapping.DestinationName))
                        {
                            row[mapping.DestinationName] = StringToCSVCell(val);
                        }
                        else
                        {
                            row.Add(mapping.DestinationName, StringToCSVCell(val));
                        }





                    }

                    row_no++;

                    if (row.Count > 0 && row.Count >= fileType.FileTypeMappings.DistinctBy(x => x.DestinationName).Count(x => x.Required == true))
                        table += row.Where(x => x.Key.StartsWith("{") == false).Select(x => x.Value).Aggregate((a, x) => a + "," + x) + "\n";
                }

                string output = $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}";
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(table);
                csv.Close();
                if (fileType.ChildFileTypes.Any())
                {
                    var fileTypes = fileType.ChildFileTypes.First();
                    fileTypes.AsycudaDocumentSetId = fileType.AsycudaDocumentSetId;
                    SaveCsv(new FileInfo[]{new FileInfo(output)}, fileTypes);
                }
                 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }



        public static DataTable CSV2DataTable(FileInfo file, string headers)
        {
            OleDbConnection conn = new OleDbConnection(string.Format(
                @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};" +
                $"Extended Properties=\"Text;HDR={headers};FMT=Delimited;CharacterSet=65001\"",
                file.DirectoryName
            ));
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
            return sb.ToString();
            //}

            //return str;
        }
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
