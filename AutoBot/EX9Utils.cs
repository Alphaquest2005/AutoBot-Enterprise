using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
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
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace AutoBot
{
    public class EX9Utils
    {
        public static void RecreateEx9()
        {
            var genDocs = CreateEx9(true);

            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                ExportEx9Entries();
                AssessEx9Entries();
                DownloadSalesFiles(10, "IM7", false);
                SalesUtils.ImportSalesEntries();
                ImportWarehouseErrorsUtils.ImportWarehouseErrors();
                RecreateEx9();
                Application.Exit();
            }
            else // reimport and submit to customs
            {
                PDFUtils.LinkPDFs();
                SalesUtils.SubmitSalesXMLToCustoms();
                EntryDocSetUtils.CleanupEntries();
                Application.Exit();
            }
        }

        public static List<DocumentCT> CreateEx9(bool overwrite)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox();

                Console.WriteLine("Create Ex9");

                var saleInfo = BaseDataModel.CurrentSalesInfo();
                if (saleInfo.Item3.AsycudaDocumentSetId == 0) return new List<DocumentCT>();

                var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;
                if (overwrite)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
                    //BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0); don't overwrite previous entries
                }

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 20;
                    var str = $@"SELECT EX9AsycudaSalesAllocations.ItemNumber
                    FROM    EX9AsycudaSalesAllocations INNER JOIN
                                     ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                                     EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                                     AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
                    WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                                     (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                                     EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                                     (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) AND (EX9AsycudaSalesAllocations.CustomsOperationId = {(int)CustomsOperations.Warehouse}) AND (AllocationErrors.ItemNumber IS NULL) 
                          AND (ApplicationSettings.ApplicationSettingsId = {
                              BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                          }) AND (EX9AsycudaSalesAllocations.InvoiceDate >= '{
                              saleInfo.Item1.ToShortDateString()
                          }') AND 
                                     (EX9AsycudaSalesAllocations.InvoiceDate <= '{saleInfo.Item2.ToShortDateString()}')
                    GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId--, EX9AsycudaSalesAllocations.pQuantity, EX9AsycudaSalesAllocations.PreviousItem_Id
                    HAVING (SUM(EX9AsycudaSalesAllocations.PiQuantity) < SUM(EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL)";

                    var res = ctx.Database.SqlQuery<string>(str);
                    if (!res.Any()) return new List<DocumentCT>();
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



                return AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, true, docset, "Sales", "Historic", true, true, true, true, true, false, true, true, true, true).Result;



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
                var i = BaseDataModel.CurrentSalesInfo();

                SalesUtils.ExportDocSetSalesReport(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number)).Wait();

                BaseDataModel.Instance.ExportDocSet(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number), true).Wait();


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
            while (Utils.AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                Utils.RunSiKuLi(directoryName, "AssessIM7", lcont.ToString());
                //RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
            }


        }

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
                    if (Utils.ImportComplete(directoryName, false, out lcont)) break;//ImportComplete(directoryName,false, out lcont);
                    Utils.RunSiKuLi(directoryName, script, lcont.ToString());
                    if (Utils.ImportComplete(directoryName, false, out lcont)) break;
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
            var genDocs = EX9Utils.CreateEx9(true);

            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                filetype.ProcessNextStep.AddRange(new List<string>() { "ExportEx9Entries", "AssessEx9Entries", "DownloadPOFiles", "ImportSalesEntries", "ImportWarehouseErrors", "RecreateEx9" });
            }
            else // reimport and submit to customs
            {
                filetype.ProcessNextStep.AddRange(new List<string>() { "LinkPDFs", "SubmitToCustoms", "CleanupEntries", "Kill" });
            }
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
                            .Select(s => new EX9Utils.SaleReportLine
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

            var info = BaseDataModel.CurrentSalesInfo();
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
            var fileType = GetxSalesFileType();
            CSVUtils.SaveCsv(new FileInfo[] { new FileInfo(testFile) }, fileType);
        }

        public static FileTypes GetxSalesFileType()
        {
            return Utils.GetFileType("xSales");
        }
    }

}