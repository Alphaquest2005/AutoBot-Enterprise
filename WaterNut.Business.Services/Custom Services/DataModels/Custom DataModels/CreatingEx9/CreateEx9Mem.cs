using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.Interfaces;
using xBondAllocations = DocumentItemDS.Business.Entities.xBondAllocations;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using Attachments = DocumentItemDS.Business.Entities.Attachments;
using InventoryItem = InventoryDS.Business.Entities.InventoryItem;
using ItemSalesPiSummary = WaterNut.DataSpace.ItemSalesPiSummary;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9DataByDateRange;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using Tesseract;


//using xcuda_Item = AllocationDS.Business.Entities.xcuda_Item;
//using xcuda_PreviousItem = AllocationDS.Business.Entities.xcuda_PreviousItem;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9
{
    
    public partial class CreateEx9Mem : ICreateEx9
    {
        private static readonly CreateEx9Mem _instance;

        static CreateEx9Mem()
        {
            _instance = new CreateEx9Mem();
        }

        public static CreateEx9Mem Instance => _instance;


        public bool PerIM7 { get; set; }


        public bool Process7100 { get; set; }

       
        public async Task<List<DocumentCT>> Execute(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks,
            AsycudaDocumentSet docSet, string documentType, string ex9BucketType, bool isGrouped,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket, bool applyHistoricChecks,  bool perInvoice, bool autoAssess, bool overPIcheck, bool universalPIcheck, bool itemPIcheck)
        {
            // Make CurrentChecks always on
           
            try
            {
                PerIM7 = perIM7;
                Process7100 = process7100;
                
                return await ProcessEx9(docSet, filterExpression, documentType, ex9BucketType, isGrouped, checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket, applyHistoricChecks, applyCurrentChecks, perInvoice, autoAssess, overPIcheck, universalPIcheck, itemPIcheck).ConfigureAwait(continueOnCapturedContext: false);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public  List<EX9AsycudaSalesAllocations> _ex9AsycudaSalesAllocations = null;
        private async Task<List<DocumentCT>> ProcessEx9(AsycudaDocumentSet docSet, string filterExp, string documentType,
            string ex9BucketType, bool isGrouped,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket,
            bool applyHistoricChecks, bool applyCurrentChecks, bool perInvoice, bool autoAssess, bool overPIcheck,
            bool universalPIcheck, bool itemPIcheck)
        {
            try
            {
             if (!filterExp.Contains("InvoiceDate"))
             {
                 throw new ApplicationException("Filter string dose not contain 'Invoice Date'");

             }
             SQLBlackBox.RunSqlBlackBox();

            
             //DocSetPi.Clear();// moved here because data is cached wont update automatically
             freashStart = true;
             _ex9AsycudaSalesAllocations = null;
          
             var docs = new List<DocumentCT>();

             var checkedFilterExpression = EnsureEx9Filters(filterExp);
               

             var realStartDate = GetWholeRangeFilter(checkedFilterExpression, out var realEndDate,  out var exPro, out var rdateFilter, out var realFilterExp);
             var startDate = realStartDate;
             var ex9DataMemTask = Task.Run(() => PrepareEx9MemData(realFilterExp,rdateFilter));
             var universalDataTask = Task.Run(() => InitializeUniversalData(docSet.ApplicationSettingsId));
             var filterTask = Task.Run(() =>CreateFilters(checkedFilterExpression, checkForMultipleMonths, startDate, realEndDate, realStartDate));
             Task.WaitAll(ex9DataMemTask, universalDataTask,filterTask);
             var getEx9DataMem = ex9DataMemTask.Result;

             var filters = filterTask.Result;

            filters
                .AsParallel()
                .AsOrdered()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))

                //.WithDegreeOfParallelism(1)
                .ForAll(async filter => await CreateDutyFreePaidEntries(docSet, documentType, ex9BucketType, isGrouped,
                    checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket, applyHistoricChecks,
                    applyCurrentChecks, perInvoice, autoAssess, overPIcheck, universalPIcheck, itemPIcheck, filter,
                    exPro, getEx9DataMem, docs).ConfigureAwait(false));



                DataSpace.BaseDataModel.RenameDuplicateDocuments(docSet.AsycudaDocumentSetId);
             StatusModel.StopStatusUpdate();
             return docs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        List<string> RequiredEx9Filters = new List<string>()
        {
            "&& (PiQuantity < pQtyAllocated)",
            "&& ( TaxAmount == 0 ||  TaxAmount != 0)",
            "&& (Status == null || Status == \"\")"
        };
        private string EnsureEx9Filters(string filterExp)
        {
            var missingFilters = RequiredEx9Filters.Where(x => !filterExp.Contains(x));
            return missingFilters.DefaultIfEmpty("")
                .Aggregate(filterExp, (o, n) => $"{o} {n}");
        }

        private async Task CreateDutyFreePaidEntries(AsycudaDocumentSet docSet, string documentType, string ex9BucketType,
            bool isGrouped, bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket,
            bool applyHistoricChecks, bool applyCurrentChecks, bool perInvoice, bool autoAssess, bool overPIcheck,
            bool universalPIcheck, bool itemPIcheck, (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter,
            string exPro, GetEx9DataMem getEx9DataMem,  List<DocumentCT> docs)
        {
            List<string> errors = new List<string>();
            var docPreviousItems =
                new Dictionary<int,
                    List<PreviousItems>>(); // moved here because the reloaded month data already has data 
            var allocationDataBlocks =
                (await new CreateAllocationDataBlocks().Execute(filter.currentFilter + exPro, errors, filter, getEx9DataMem, PerIM7)
                    .ConfigureAwait(false))
                .Where(x => x.Allocations.Count > 0).ToList();

            if (allocationDataBlocks.ToList().Any())
            {
                await CreateDutyFreePaidEntires(docSet, documentType, ex9BucketType, isGrouped,
                    checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket,
                    applyHistoricChecks, applyCurrentChecks, perInvoice, autoAssess, overPIcheck,
                    universalPIcheck, itemPIcheck, allocationDataBlocks, filter, docs,
                    docPreviousItems).ConfigureAwait(false);
            }
        }

        private List<(string currentFilter, string dateFilter, DateTime startDate, DateTime endDate)> CreateFilters(string filterExp, bool checkForMultipleMonths, DateTime startDate, DateTime realEndDate,
            DateTime realStartDate)
        {
            var filters = new List<(string currentFilter, string dateFilter, DateTime startDate, DateTime endDate)>();
            while (startDate < realEndDate)
            {
                var firstOfNextMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);
                var endDate = checkForMultipleMonths ? firstOfNextMonth.AddDays(-1).AddHours(23) : realEndDate;

                var currentFilter = CreateCurrentFilter(filterExp, realStartDate, startDate, realEndDate, endDate);

                var dateFilter = CreateDateFilter(startDate, endDate);

                filters.Add((currentFilter, dateFilter, startDate, endDate));
                startDate = checkForMultipleMonths
                    ? new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1)
                    : realEndDate;
            }

            return filters;
        }

        private async Task CreateDutyFreePaidEntires(AsycudaDocumentSet docSet, string documentType,
            string ex9BucketType,
            bool isGrouped, bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths,
            bool applyEx9Bucket,
            bool applyHistoricChecks, bool applyCurrentChecks, bool perInvoice, bool autoAssess, bool overPIcheck,
            bool universalPIcheck, bool itemPIcheck, List<AllocationDataBlock> allocationDataBlocks,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter, List<DocumentCT> docs, Dictionary<int, List<PreviousItems>> docPreviousItems)
        {
            var dutylst = new List<string>() { "Duty Paid", "Duty Free" };
            Debug.WriteLine($"*********************Create EX9 For {filter.startDate.Date.ToShortDateString()}");

            foreach (var dfp in dutylst)
            {
                docSet.Customs_Procedure = GetCustomsProcedure(dfp);

                await CreateDutyFreePaidEntries(docSet, documentType, ex9BucketType, isGrouped,
                    checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket,
                    applyHistoricChecks, applyCurrentChecks, perInvoice, autoAssess, overPIcheck,
                    universalPIcheck, itemPIcheck, allocationDataBlocks, dfp, filter, docs, docPreviousItems).ConfigureAwait(false);
            }
        }

        public DateTime GetWholeRangeFilter(string filterExp, out DateTime realEndDate, 
            out string exPro, out string rdateFilter, out string realFilterExp)
        {
            var realStartDate = GetRealStartDate(filterExp);
            realEndDate = GetRealEndDate(filterExp);
           
            exPro = GetDataFilters();

            rdateFilter = CreateDateRangeFilter(realStartDate, realEndDate);

            realFilterExp = filterExp + exPro;
            return realStartDate;
        }

        private string CreateDateFilter(DateTime startDate, DateTime endDate) => $@"InvoiceDate >= ""{startDate:MM/dd/yyyy}"" && InvoiceDate <= ""{endDate:MM/dd/yyyy HH:mm:ss}""";

        private string CreateCurrentFilter(string filterExp, DateTime realStartDate, DateTime startDate, DateTime realEndDate, DateTime endDate) =>
            filterExp.Replace($@"InvoiceDate >= ""{realStartDate:MM/dd/yyyy}""",
                    $@"InvoiceDate >= ""{startDate:MM/dd/yyyy}""")
                .Replace($@"InvoiceDate <= ""{realEndDate:MM/dd/yyyy HH:mm:ss}""",
                    $@"InvoiceDate <= ""{endDate:MM/dd/yyyy HH:mm:ss}""");

        private async Task CreateDutyFreePaidEntries(AsycudaDocumentSet docSet, string documentType,
            string ex9BucketType,
            bool isGrouped, bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths,
            bool applyEx9Bucket,
            bool applyHistoricChecks, bool applyCurrentChecks, bool perInvoice, bool autoAssess, bool overPIcheck,
            bool universalPIcheck, bool itemPIcheck, List<AllocationDataBlock> allocationDataBlocks, string dfp,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter, List<DocumentCT> docs,
            Dictionary<int, List<PreviousItems>> docPreviousItems)
        {
            var entryTypes = allocationDataBlocks.GroupBy(x => x.Type).ToList();
            foreach (var entryType in entryTypes)
            {
                var res = entryType.Where(x => x.DutyFreePaid == dfp);
                var itemSalesPiSummarylst = GetItemSalesPiSummary(filter.startDate, filter.endDate, dfp, entryType.Key);
                var genDocs = await new CreateDutyFreePaidDocument().Execute(dfp, res, docSet, documentType, isGrouped,
                        itemSalesPiSummarylst.Where(x =>
                                x.DutyFreePaid == dfp || x.DutyFreePaid == "All" ||
                                x.DutyFreePaid == "Universal")
                            .ToList(), checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths,
                        applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks,
                        autoAssess, perInvoice, overPIcheck, universalPIcheck, itemPIcheck,docPreviousItems,PerIM7)
                    .ConfigureAwait(false);
                docs.AddRange(genDocs);
            }

        }

        private  Customs_Procedure GetCustomsProcedure(string dfp) =>
            DataSpace.BaseDataModel.Instance.Customs_Procedures
                .Where(x => x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Sales == true && x.IsPaid == (dfp == "Duty Paid"))
                .OrderByDescending(x => x.IsDefault == true)
                .First();

        private  string CreateDateRangeFilter(DateTime realStartDate, DateTime realEndDate) => $@"InvoiceDate >= ""{realStartDate:MM/dd/yyyy}"" && InvoiceDate <= ""{realEndDate.AddHours(23):MM/dd/yyyy HH:mm:ss}""";

        private  string GetDataFilters() =>
            " && (PreviousDocumentItem != null && PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations.Name == \"Warehouse\" )" +
            " && (PreviousDocumentItem != null && PreviousDocumentItem.AsycudaDocument.Cancelled == null || PreviousDocumentItem.AsycudaDocument.Cancelled == false)" +
            "&& (PreviousDocumentItem != null && PreviousDocumentItem.xWarehouseError == null)";

        private  DateTime GetRealStartDate(string filterExp) =>
            DateTime.Parse(
                filterExp.Substring(filterExp.IndexOf("InvoiceDate >= ") + "InvoiceDate >= ".Length + 1, 10),
                CultureInfo.CurrentCulture);

        private DateTime GetRealEndDate(string filterExp) =>
            DateTime.Parse(
                filterExp.Substring(filterExp.IndexOf("InvoiceDate <= ") + "InvoiceDate <= ".Length + 1, 19),
                CultureInfo.CurrentCulture);

        private GetEx9DataMem PrepareEx9MemData(string filterExp, string rdateFilter) => new GetEx9DataMem(filterExp,rdateFilter);


        public List<ItemSalesPiSummary> GetItemSalesPiSummary(DateTime startDate,
            DateTime endDate, string dfp, string entryDataType) =>
            GetPiSummary(startDate, endDate, dfp, entryDataType);

        private static ConcurrentDictionary<(int, int, string DutyFreePaid, DateTime? EntryDataDate), SummaryData> _universalData = null;
        private static List<ItemSalesPiSummary> universalDataSummary = null;
        private static List<ItemSalesPiSummary> allSalesSummary = null;
        private static List<SummaryData> allSales = null;
        private bool freashStart = true;
        private  List<ItemSalesPiSummary> GetPiSummary(DateTime startDate,
            DateTime endDate, string dfp, string entryType)
        {
            try
            {


                var res = new List<ItemSalesPiSummary>();
                lock (Identity)
                {
                    SummaryInititalization(entryType, startDate, endDate);
                }
                

               // var testres = allSales.Where(x => x.Summary.PreviousItem_Id == 782698).ToList();
                //var testlistQtyAllocated = testres.Select(x => x.Summary.QtyAllocated).ToList();
                //var testQtyAllocated = testres.Sum(x => x.Summary.QtyAllocated);
                res.AddRange(universalDataSummary);
                res.AddRange(allSalesSummary);


                var allHistoricSales = allSales
                    .Where(x => (x.Summary.Type ?? x.Summary.EntryDataType) == entryType)// || x.Summary.Type == null
                    .Where(x => x.Summary.EntryDataDate <= endDate)
                    .Where(x => x.Summary.DutyFreePaid == dfp).ToList();

                //var test = allSales.Where(x => x.Summary.PreviousItem_Id == 16758);
                //var test2 = allHistoricSales.Where(x => x.Summary.PreviousItem_Id == 16758);

                res.AddRange(allHistoricSales
                    .GroupBy(g => new
                    {
                        g.Summary.PreviousItem_Id,
                        g.Summary.pCNumber,
                        g.Summary.pLineNumber,
                        //  ItemNumber = g.ItemNumber,
                        g.Summary.DutyFreePaid,
                        EntryType = g.Summary.Type ?? g.Summary.EntryDataType

                    })
                    .Select(x => new ItemSalesPiSummary
                    {
                        PreviousItem_Id = (int) x.Key.PreviousItem_Id,
                        ItemNumber = x.First().Summary.ItemNumber,
                        QtyAllocated = x.Select(z => z.Summary.QtyAllocated).DefaultIfEmpty(0).Sum(),
                        pQtyAllocated = x.Select(z => z.Summary.pQtyAllocated).Distinct().DefaultIfEmpty(0).Sum(),
                        PiQuantity =
                            (double) x.DistinctBy(q => q.Summary.PreviousItem_Id)
                                .SelectMany(z => z.pIData.Where(zz => zz.AssessmentDate <= endDate && zz.EntryDataType == entryType).Select(zz => zz.PiQuantity))
                                .DefaultIfEmpty(0)
                                .Sum(), // x.Select(z => z.Summary.PiQuantity).DefaultIfEmpty(0).Sum(),
                        pCNumber = x.Key.pCNumber,
                        pLineNumber = (int) x.Key.pLineNumber,
                        DutyFreePaid = x.Key.DutyFreePaid,
                        Type = "Historic",
                        EntryDataType = x.Key.EntryType,
                        EndDate = endDate
                    }).ToList());

                res.AddRange(allHistoricSales
                    .Where(x => x.Summary.EntryDataDate >= startDate /*&& x.Summary.EntryDataDate <= endDate*/)
                    .GroupBy(g => new
                    {
                        g.Summary.PreviousItem_Id,
                        g.Summary.pCNumber,
                        g.Summary.pLineNumber,
                        //ItemNumber = g.ItemNumber,
                        g.Summary.DutyFreePaid,
                        EntryType = g.Summary.Type ?? g.Summary.EntryDataType

                    })
                    .Select(x => new ItemSalesPiSummary
                    {
                        PreviousItem_Id = (int) x.Key.PreviousItem_Id,
                        ItemNumber = x.First().Summary.ItemNumber,
                        QtyAllocated = x.Select(z => z.Summary.QtyAllocated).DefaultIfEmpty(0).Sum(),
                        pQtyAllocated = x.Select(z => z.Summary.pQtyAllocated).Distinct().DefaultIfEmpty(0).Sum(),
                        PiQuantity =
                            (double) x.DistinctBy(q => q.Summary.PreviousItem_Id)
                                .SelectMany(z => z.pIData.Where(zz => zz.AssessmentDate >= startDate && zz.AssessmentDate <= endDate && zz.EntryDataType == entryType).Select(zz => zz.PiQuantity))
                                .DefaultIfEmpty(0)
                                .Sum(), // x.Select(z => z.Summary.PiQuantity).DefaultIfEmpty(0).Sum(),
                        pCNumber = x.Key.pCNumber,
                        pLineNumber = (int) x.Key.pLineNumber,
                        DutyFreePaid = x.Key.DutyFreePaid,
                        Type = "Current",
                        EntryDataType = entryType,
                        StartDate = startDate,
                        EndDate = endDate
                    }).ToList());
                return res;//.DistinctBy(x => new {x.PreviousItem_Id, x.Type, x.DutyFreePaid}).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SummaryInititalization(string entryType,
            DateTime startDate, DateTime endDate)
        {
            
            //var test = _universalData.Where(x => x.Summary.PreviousItem_Id == 1178474);
            var uData = _universalData
                .Where(x => x.Key.EntryDataDate >= startDate
                            && x.Key.EntryDataDate <= endDate)
                .Select(x => x.Value).ToList();
                    universalDataSummary = uData
                        .GroupBy(g => new
                        {
                            g.Summary.PreviousItem_Id,
                            g.Summary.pCNumber,
                            g.Summary.pLineNumber,
                            // ItemNumber = g.ItemNumber, /// took out all itemnumber because the pos can have different itemnumbers in entrydatadetails... c#14280 - 64
                        })
                        .Select(x => new ItemSalesPiSummary
                        {
                            PreviousItem_Id = (int)x.Key.PreviousItem_Id,
                            ItemNumber = x.First().Summary.ItemNumber,
                            QtyAllocated = (double)x.DistinctBy(q => q.Summary.Id).Select(z => z.Summary.QtyAllocated)
                                .DefaultIfEmpty(0).Sum(),
                            pQtyAllocated = x.DistinctBy(q => new { q.Summary.DutyFreePaid, q.Summary.pQtyAllocated })
                                .Select(z => z.Summary.pQtyAllocated).DefaultIfEmpty(0).Sum(),
                            PiQuantity =
                                (double)x.DistinctBy(q => q.Summary.PreviousItem_Id)
                                    .SelectMany(z => z.pIData.Select(zz => zz.PiQuantity))
                                    .DefaultIfEmpty(0).Sum(),
                            pCNumber = x.Key.pCNumber,
                            pLineNumber = (int)x.Key.pLineNumber,
                            DutyFreePaid = "Universal",
                            Type = "Universal",
                            EntryDataType = "Universal",
                            StartDate = startDate,
                            EndDate = endDate
                        }).ToList();
                    allSales = uData;
                    allSalesSummary = allSales
                        .GroupBy(g => new
                        {
                            g.Summary.PreviousItem_Id,
                            g.Summary.pCNumber,
                            g.Summary.pLineNumber,
                            // entryType = g.Summary.Type??"Sales",

                            // ItemNumber = g.ItemNumber,
                        })
                        .Select(x => new ItemSalesPiSummary
                        {
                            PreviousItem_Id = (int)x.Key.PreviousItem_Id,
                            ItemNumber = x.First().Summary.ItemNumber,
                            QtyAllocated = (double)x.DistinctBy(q => q.Summary.Id).Select(z => z.Summary.QtyAllocated)
                                .DefaultIfEmpty(0).Sum(),
                            pQtyAllocated = x.DistinctBy(q => new { q.Summary.DutyFreePaid, q.Summary.pQtyAllocated })

                                .Select(z => z.Summary.pQtyAllocated).DefaultIfEmpty(0).Sum(),
                            PiQuantity =
                                (double)x.DistinctBy(q => new { q.Summary.DutyFreePaid, q.Summary.PreviousItem_Id })
                                    .SelectMany(z => z.pIData.Select(zz => zz.PiQuantity))
                                    .DefaultIfEmpty(0)
                                    .Sum(), //x.DistinctBy(q => q.Summary.Id).Select(z => z.Summary.PiQuantity).DefaultIfEmpty(0).Sum(),
                            pCNumber = x.Key.pCNumber,
                            pLineNumber = (int)x.Key.pLineNumber,
                            DutyFreePaid = "All",
                            Type = "All",
                            EntryDataType = entryType, //x.Key.entryType,
                            StartDate = startDate,
                            EndDate = endDate
                        }).ToList();
                    
                
            
        }

        private void InitializeUniversalData(int applicationSettingsId)
        {
            lock (Identity)
            {
                if (_universalData != null && !freashStart) return;
                _universalData = new ConcurrentDictionary<(int Id, int PreviousItem_Id, string DutyFreePaid, DateTime? EntryDataDate), SummaryData>(GetUniversalData(applicationSettingsId));
                freashStart = false;
            }
        }

        private Dictionary<(int Id, int PreviousItem_Id, string DutyFreePaid, DateTime? EntryDataDate), SummaryData> GetUniversalData(int applicationSettingsId)
        {
            using (var ctx = new AllocationDSContext(){StartTracking = false})
            {
                ctx.Database.CommandTimeout = 0;
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

               return ctx.ItemSalesAsycudaPiSummary
                    .GroupJoin(ctx.AsycudaItemPiQuantityData,
                        pis => new { PreviousItem_Id = (int)pis.PreviousItem_Id, pis.DutyFreePaid },
                        pid => new { PreviousItem_Id = pid.Item_Id, pid.DutyFreePaid },
                        (pis, pid) => new SummaryData { Summary = pis, pIData = pid })
                    //.Where(x => x.ItemNumber == "14479" || x.ItemNumber == "014479")
                    .Where(x => x.Summary.ApplicationSettingsId == applicationSettingsId)
                .ToDictionary(x => (x.Summary.Id,x.Summary.PreviousItem_Id.GetValueOrDefault(), x.Summary.DutyFreePaid, x.Summary.EntryDataDate), x => x);
            }
        }


        // private static Dictionary<int, List<PreviousItems>> docPreviousItems = new Dictionary<int, List<PreviousItems>>();
        private static readonly object Identity = new object();
  

       
    }
}