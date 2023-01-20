using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using Newtonsoft.Json.Linq;
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace AutoBot
{
    public class EX9Utils
    {
        public static void RecreateEx9(int months)
        {
            var genDocs = CreateEX9Utils.CreateEx9(true, months);
           

            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                ExportEx9Entries(months);
                AssessEx9Entries(months);
                DownloadSalesFiles(10, "IM7", false);
                DocumentUtils.ImportSalesEntries(true);
                ImportWarehouseErrorsUtils.ImportWarehouseErrors(months);
                RecreateEx9(months);
                Application.Exit();
            }
            else // reimport and submit to customs
            {
                PDFUtils.LinkPDFs();
                SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms();
                EntryDocSetUtils.CleanupEntries();
                Application.Exit();
            }
        }


        public static void ExportEx9Entries(int months)
        {
            Console.WriteLine("Export EX9 Entries");
            try
            {
                var saleInfo =  BaseDataModel.CurrentSalesInfo(months);

                ExportDocSetSalesReportUtils.ExportDocSetSalesReport(saleInfo.DocSet.AsycudaDocumentSetId,
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number)).Wait();

                BaseDataModel.Instance.ExportDocSet(saleInfo.DocSet.AsycudaDocumentSetId,
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number), true).Wait();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void AssessEx9Entries(int months) => AssessSalesEntry(BaseDataModel.CurrentSalesInfo(months).Item3.Declarant_Reference_Number);

        public static void AssessSalesEntry(string docReference)
        {
            while (docReference != null && Utils.AssessComplete(GetInstructionFile(docReference),
                       GetInstructionResultsFile(docReference), out var lcont) == false)
                Utils.RunSiKuLi(BaseDataModel.GetDocSetDirectoryName(docReference), "AssessIM7",
                    lcont.ToString()); //RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
        }

       

        private static string GetInstructionResultsFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "InstructionResults.txt");

        private static string GetInstructionFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "Instructions.txt");

        public static void DownloadSalesFiles(int trytimes, string script, bool redownload = false)
        {
            try
            {
                var directoryName = BaseDataModel.GetDocSetDirectoryName("Imports");
                Console.WriteLine("Download Entries");
                var lcont = 0;

                for (int i = 0; i < trytimes; i++)
                {
                    if (Utils.ImportComplete(directoryName, redownload, out lcont))
                        break; //ImportComplete(directoryName,false, out lcont);
                    Utils.RunSiKuLi(directoryName, script, lcont.ToString());
                    if (Utils.ImportComplete(directoryName, redownload, out lcont)) break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

        public static void RecreateEx9(FileTypes filetype, FileInfo[] files)
        {
            var genDocs = CreateEX9Utils.CreateEx9(true, -1);
            var saleInfo = BaseDataModel.CurrentSalesInfo(-1);
            filetype.AsycudaDocumentSetId = saleInfo.DocSet.AsycudaDocumentSetId;
            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                filetype.ProcessNextStep.AddRange(new List<string>() { "ExportEx9Entries", "AssessEx9Entries", "DownloadPOFiles", "ImportSalesEntries", "ImportWarehouseErrors", "RecreateEx9" });
            }
            else // reimport and submit to customs
            {
                filetype.ProcessNextStep.AddRange(new List<string>() { "LinkPDFs", "SubmitToCustoms", "CleanupEntries", "Kill" });
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

                    //////////// I left this because i only want to use interface to remove All ALLOCATED 
                    //"&& (pQuantity > PiQuantity)" +
                    //"&& (pQuantity - pQtyAllocated  < 0.001)" + // prevents spill over allocations
                    "&& (Status == null || Status == \"\")" +
                    (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                        ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                        : "") +
                    ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");



                AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, false, docset, "Sales", "Historic", true, true, true, false, false, false, true, true, true, false).Wait();
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

        public static void EmailEntriesExpiringNextMonth()
        {

            var info = BaseDataModel.CurrentSalesInfo(-1);
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "EntriesExpiringNextMonth.csv");

            using (var ctx = new CoreEntitiesContext())
            {
                var errors = ctx.TODO_EntriesExpiringNextMonth
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();

                CSVUtils.SaveCSVReport(errors, errorfile);


                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                {
                    var body = "The following entries are expiring within the next month. \r\n" +
                               $"Start Date: {DateTime.Now:yyyy-MM-dd} End Date {DateTime.Now.AddMonths(1):yyyy-MM-dd}: \r\n" +
                               $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"Document Type".FormatedSpace(20)}{"RegistrationDate".FormatedSpace(20)}{"ExpiryDate".FormatedSpace(20)}\r\n" +
                               $"{errors.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.Reference.FormatedSpace(20)}{current.DocumentType.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)}{current.ExpiryDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                               $"\r\n" +
                               $"{Utils.Client.CompanyName} is kindly requesting these Entries be extended an additional 730 days to facilitate ex-warehousing. \r\n" +
                               $"\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory, $"Entries Expiring {DateTime.Now:yyyy-MM-dd} - {DateTime.Now.AddMonths(1):yyyy-MM-dd}", contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[]
                    {
                        errorfile
                    });
                }
            }

        }

        public static void EmailWarehouseErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo(-1);
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

                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                {
                    var body = "Attached are Issues that have been found on Assessed Entries that prevent Ex-warehousing. \r\n" +

                               $"{Utils.Client.CompanyName} is kindly requesting Technical Assistance in resolving these issues to facilitate Ex-Warehousing. \r\n" +
                               $"\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory, $"Warehouse Errors", contacts.Select(x => x.EmailAddress).Distinct().ToArray(), body, new string[]
                    {
                        errorfile
                    });
                }
            }

        }

        public static void relinkAllPreviousItems()
        {
            try
            {

                Console.WriteLine("ReLink All Previous Items");

                //////// all what i fucking try aint work just can't load the navigation properties fucking ef shit
                List<xcuda_ASYCUDA> docLst;
                List<xcuda_Identification> idlst;
                using (var ctx = new DocumentDSContext() { StartTracking = true })
                {


                    idlst = ctx.xcuda_Identification
                        .Include(x => x.xcuda_Registration)
                        .Include(x => x.xcuda_Type)
                        .Where(x => x.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type
                            .Declaration_gen_procedure_code == "7")
                        .ToList();
                }

                using (var ctx = new DocumentDSContext() { StartTracking = true })
                {
                    docLst = ctx.xcuda_ASYCUDA
                        //.Include(x => x.xcuda_Identification.xcuda_Registration)
                        //.Include(x => x.xcuda_Identification.xcuda_Type)
                        .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type
                            .Declaration_gen_procedure_code == "7")
                        .ToList();
                }
                List<xcuda_Tarification> itmCodes;
                using (var ctx = new DocumentItemDSContext() { StartTracking = true })
                {

                    itmCodes = ctx.xcuda_Tarification
                        .Include(x => x.xcuda_HScode)
                        .ToList();
                }

                using (var ctx = new DocumentItemDSContext() { StartTracking = true })
                {
                    foreach (var doc in docLst)
                    {
                        ctx.Database.ExecuteSqlCommand($@"DELETE FROM EntryPreviousItems
                    FROM    EntryPreviousItems INNER JOIN
                    xcuda_Item ON EntryPreviousItems.Item_Id = xcuda_Item.Item_Id
                    WHERE(xcuda_Item.ASYCUDA_Id = {doc.ASYCUDA_Id})");

                        var itms = ctx.xcuda_Item
                            //.Include(x => x.xcuda_Tarification.xcuda_HScode)
                            .Where(x => x.ASYCUDA_Id == doc.ASYCUDA_Id)
                            .ToList();

                        doc.xcuda_Identification = idlst.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                        foreach (var itm in itms)
                        {
                            itm.xcuda_Tarification = itmCodes.First(x => x.Item_Id == itm.Item_Id);

                        }

                        BaseDataModel.Instance.LinkExistingPreviousItems(doc, itms, true).Wait();




                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportXSalesFiles(string testFile)
        {
            var fileTypes = GetxSalesFileType(testFile);
            foreach (var fileType in fileTypes)
            {
                new FileTypeImporter(fileType).Import(testFile);
            }
           
        }

        public static List<FileTypes> GetxSalesFileType(string fileName)
        {
            return Utils.GetFileType(FileTypeManager.EntryTypes.xSales, FileTypeManager.FileFormats.Csv, fileName);
        }
    }

}