using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using EntryDataDetails = AllocationDS.Business.Entities.EntryDataDetails;
using xBondAllocations = DocumentItemDS.Business.Entities.xBondAllocations;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;
using System.Data.Entity;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AllocationQS.Business.Entities;
using CoreEntities.Business.Enums;
using DocumentItemDS.Business.Entities;
using EntryDataDS.Business.Entities;
using java.security;
using MoreLinq;
using PreviousDocumentQS.Business.Entities;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Utils;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using Attachments = DocumentItemDS.Business.Entities.Attachments;
using InventoryItem = InventoryDS.Business.Entities.InventoryItem;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;

//using xcuda_Item = AllocationDS.Business.Entities.xcuda_Item;
//using xcuda_PreviousItem = AllocationDS.Business.Entities.xcuda_PreviousItem;

namespace WaterNut.DataSpace
{
    public class CreateEx9Class
    {
        private static readonly CreateEx9Class _instance;

        static CreateEx9Class()
        {
            _instance = new CreateEx9Class();
        }

        public static CreateEx9Class Instance
        {
            get { return _instance; }
        }

        


        public bool PerIM7 { get; set; }


        public bool Process7100 { get; set; }


        public async Task<List<DocumentCT>> CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks,
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

        public static List<PiSummary> DocSetPi = new List<PiSummary>();

        private async Task<List<DocumentCT>> ProcessEx9(AsycudaDocumentSet docSet, string filterExp, string documentType,
            string ex9BucketType, bool isGrouped,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket,
            bool applyHistoricChecks, bool applyCurrentChecks, bool perInvoice, bool autoAssess, bool overPIcheck,
            bool universalPIcheck, bool itemPIcheck)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox();

                DocSetPi.Clear();
                
                //DocSetPi.Clear();// moved here because data is cached wont update automatically
                freashStart = true;
                _ex9AsycudaSalesAllocations = null;
                InventoryDataCache =
                    InventoryItemUtils.GetInventoryItems(BaseDataModel.Instance.CurrentApplicationSettings
                        .ApplicationSettingsId, true);
                var docs = new List<DocumentCT>();
                //docPreviousItems = new Dictionary<int, List<previousItems>>();
                //var dutylst = CreateDutyList(slst);
                var dutylst = new List<string>() {"Duty Paid", "Duty Free"};
                if (!filterExp.Contains("InvoiceDate"))
                {
                    throw new ApplicationException("Filter string dose not contain 'Invoice Date'");

                }

                var realStartDate =
                    DateTime.Parse(
                        filterExp.Substring(filterExp.IndexOf("InvoiceDate >= ") + "InvoiceDate >= ".Length + 1, 10),
                        CultureInfo.CurrentCulture);
                var realEndDate =
                    DateTime.Parse(
                        filterExp.Substring(filterExp.IndexOf("InvoiceDate <= ") + "InvoiceDate <= ".Length + 1, 19),
                        CultureInfo.CurrentCulture);
                DateTime startDate = realStartDate;

                while (startDate < realEndDate)
                {
                     
                    docPreviousItems = new Dictionary<int, List<previousItems>>();// moved here because the reloaded month data already has data 

                    DateTime firstOfNextMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);
                    DateTime endDate = checkForMultipleMonths ? firstOfNextMonth.AddDays(-1).AddHours(23) : realEndDate;

                    var currentFilter = filterExp.Replace($@"InvoiceDate >= ""{realStartDate:MM/dd/yyyy}""",
                            $@"InvoiceDate >= ""{startDate:MM/dd/yyyy}""")
                        .Replace($@"InvoiceDate <= ""{realEndDate:MM/dd/yyyy HH:mm:ss}""",
                            $@"InvoiceDate <= ""{endDate:MM/dd/yyyy HH:mm:ss}""");

                    var dateFilter = $@"InvoiceDate >= ""{startDate:MM/dd/yyyy}"" && InvoiceDate <= ""{endDate:MM/dd/yyyy HH:mm:ss}""";
                    //  var salesSummary = GetSalesSummary(startDate, endDate);
                    List<string> errors = new List<string>();
                    //using (var ctx = new AllocationDSContext())
                    //{

                    //    errors = ctx.AllocationErrors
                    //        .Where(x => x.EntryDataDate >= startDate && x.EntryDataDate <= endDate 
                    //                    && x.ApplicationSettingsId == docSet.ApplicationSettingsId)
                    //        .Select(x => x.ItemNumber).Distinct().ToList();
                    //}

                    var exPro =
                        " && (PreviousDocumentItem != null && PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations.Name == \"Warehouse\" )" +
                        " && (PreviousDocumentItem != null && PreviousDocumentItem.AsycudaDocument.Cancelled == null || PreviousDocumentItem.AsycudaDocument.Cancelled == false)" +
                        "&& (PreviousDocumentItem != null && PreviousDocumentItem.xWarehouseError == null)"
                       //+ " && Type == \"ADJ\""
                       ;
                    var slst =
                        (await CreateAllocationDataBlocks(currentFilter + exPro, errors, dateFilter).ConfigureAwait(false))
                        .Where(x => x.Allocations.Count > 0);


                    foreach (var dfp in dutylst)
                    {
                        var cp =
                            BaseDataModel.Instance.Customs_Procedures
                                .Where(x => x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Sales == true && x.IsPaid == (dfp == "Duty Paid"))
                                .OrderByDescending(x => x.IsDefault == true)
                                .First();

                        docSet.Customs_Procedure = cp;

                        if (slst != null && slst.ToList().Any())
                        {
                            var types = slst.GroupBy(x => x.Type).ToList();
                            foreach (var entrytype in types)
                            {
                                //DocSetPi.Clear();// move this to top because the data is cached now

                                var res = entrytype.Where(x => x.DutyFreePaid == dfp);
                                List<ItemSalesPiSummary> itemSalesPiSummarylst;
                                itemSalesPiSummarylst = GetItemSalesPiSummary(docSet.ApplicationSettingsId, startDate,
                                    endDate,
                                    dfp, entrytype.Key); //res.SelectMany(x => x.Allocations).Select(z => z.AllocationId).ToList(), 
                               var genDocs =  await CreateDutyFreePaidDocument(dfp, res, docSet, documentType, isGrouped,
                                        itemSalesPiSummarylst.Where(x =>
                                                x.DutyFreePaid == dfp || x.DutyFreePaid == "All" ||
                                                x.DutyFreePaid == "Universal")
                                            .ToList(), checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths,
                                        applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks,
                                        autoAssess, perInvoice, overPIcheck, universalPIcheck, itemPIcheck)
                                    .ConfigureAwait(false);
                                docs.AddRange(genDocs);

                               
                            }
                        }

                      

                    }

                    startDate = checkForMultipleMonths ? new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1): realEndDate;
                }
                BaseDataModel.RenameDuplicateDocuments(docSet.AsycudaDocumentSetId);
                StatusModel.StopStatusUpdate();
                return docs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public List<ItemSalesPiSummary> GetItemSalesPiSummary(int applicationSettingsId, DateTime startDate,
            DateTime endDate, string dfp, string entryDataType)
        {
            try
            {

                using (var ctx = new AllocationDSContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    return GetPiSummary(applicationSettingsId, startDate, endDate, dfp, ctx, entryDataType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private static List<SummaryData> _universalData = null;
        private static List<ItemSalesPiSummary> universalDataSummary = null;
        private static List<ItemSalesPiSummary> allSalesSummary = null;
        private static List<SummaryData> allSales = null;
        public static bool freashStart = true;
        private static List<ItemSalesPiSummary> GetPiSummary(int applicationSettingsId, DateTime startDate,
            DateTime endDate, string dfp,
            AllocationDSContext ctx, string entryType)
        {
            try
            {


                var res = new List<ItemSalesPiSummary>();

                SummaryInititalization(applicationSettingsId, ctx, entryType, startDate, endDate);

               // var testres = allSales.Where(x => x.Summary.PreviousItem_Id == 782698).ToList();
                //var testlistQtyAllocated = testres.Select(x => x.Summary.QtyAllocated).ToList();
                //var testQtyAllocated = testres.Sum(x => x.Summary.QtyAllocated);
                res.AddRange(universalDataSummary);
                res.AddRange(allSalesSummary);


                var allHistoricSales = allSales
                    .Where(x => (x.Summary.Type ?? x.Summary.EntryDataType) == entryType)// || x.Summary.Type == null
                    .Where(x => x.Summary.EntryDataDate <= endDate)
                    .Where(x => x.Summary.DutyFreePaid == dfp).ToList();

                var test = allSales.Where(x => x.Summary.PreviousItem_Id == 16758);
                var test2 = allHistoricSales.Where(x => x.Summary.PreviousItem_Id == 16758);

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
                        g.Summary.DutyFreePaid

                    })
                    .Select(x => new ItemSalesPiSummary
                    {
                        PreviousItem_Id = (int) x.Key.PreviousItem_Id,
                        ItemNumber = x.First().Summary.ItemNumber,
                        QtyAllocated = x.Select(z => z.Summary.QtyAllocated).DefaultIfEmpty(0).Sum(),
                        pQtyAllocated = x.Select(z => z.Summary.pQtyAllocated).Distinct().DefaultIfEmpty(0).Sum(),
                        PiQuantity =
                            (double) x.DistinctBy(q => q.Summary.PreviousItem_Id)
                                .SelectMany(z => z.pIData.Where(zz => zz.AssessmentDate >= startDate && zz.AssessmentDate <= endDate).Select(zz => zz.PiQuantity))
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
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void SummaryInititalization(int applicationSettingsId, AllocationDSContext ctx, string entryType,
            DateTime startDate, DateTime endDate)
        {
            if (_universalData == null || freashStart)
            {
                _universalData = ctx.ItemSalesAsycudaPiSummary
                    .GroupJoin(ctx.AsycudaItemPiQuantityData,
                        pis => new {PreviousItem_Id = (int) pis.PreviousItem_Id, pis.DutyFreePaid},
                        pid => new {PreviousItem_Id = pid.Item_Id, pid.DutyFreePaid},
                        (pis, pid) => new SummaryData {Summary = pis, pIData = pid})
                    //.Where(x => x.ItemNumber == "14479" || x.ItemNumber == "014479")
                    .Where(x => x.Summary.ApplicationSettingsId == applicationSettingsId)
                    .ToList();

               var test = _universalData.Where(x => x.Summary.PreviousItem_Id == 1178474);

                universalDataSummary = _universalData
                    .GroupBy(g => new
                    {
                        g.Summary.PreviousItem_Id,
                        g.Summary.pCNumber,
                        g.Summary.pLineNumber,
                        // ItemNumber = g.ItemNumber, /// took out all itemnumber because the pos can have different itemnumbers in entrydatadetails... c#14280 - 64
                    })
                    .Select(x => new ItemSalesPiSummary
                    {
                        PreviousItem_Id = (int) x.Key.PreviousItem_Id,
                        ItemNumber = x.First().Summary.ItemNumber,
                        QtyAllocated = (double) x.DistinctBy(q => q.Summary.Id).Select(z => z.Summary.QtyAllocated)
                            .DefaultIfEmpty(0).Sum(),
                        pQtyAllocated = x.DistinctBy(q => new {q.Summary.DutyFreePaid, q.Summary.pQtyAllocated})
                            .Select(z => z.Summary.pQtyAllocated).DefaultIfEmpty(0).Sum(),
                        PiQuantity =
                            (double) x.DistinctBy(q => q.Summary.PreviousItem_Id)
                                .SelectMany(z => z.pIData.Select(zz => zz.PiQuantity))
                                .DefaultIfEmpty(0).Sum(),
                        pCNumber = x.Key.pCNumber,
                        pLineNumber = (int) x.Key.pLineNumber,
                        DutyFreePaid = "Universal",
                        Type = "Universal",
                        EntryDataType = "Universal",
                        StartDate = startDate,
                        EndDate = endDate
                    }).ToList();
                allSales = _universalData;
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
                        PreviousItem_Id = (int) x.Key.PreviousItem_Id,
                        ItemNumber = x.First().Summary.ItemNumber,
                        QtyAllocated = (double) x.DistinctBy(q => q.Summary.Id).Select(z => z.Summary.QtyAllocated)
                            .DefaultIfEmpty(0).Sum(),
                        pQtyAllocated = x.DistinctBy(q => new {q.Summary.DutyFreePaid, q.Summary.pQtyAllocated})
                            
                            .Select(z => z.Summary.pQtyAllocated).DefaultIfEmpty(0).Sum(),
                        PiQuantity =
                            (double) x.DistinctBy(q => new { q.Summary.DutyFreePaid, q.Summary.PreviousItem_Id})
                                .SelectMany(z => z.pIData.Select(zz => zz.PiQuantity))
                                .DefaultIfEmpty(0)
                                .Sum(), //x.DistinctBy(q => q.Summary.Id).Select(z => z.Summary.PiQuantity).DefaultIfEmpty(0).Sum(),
                        pCNumber = x.Key.pCNumber,
                        pLineNumber = (int) x.Key.pLineNumber,
                        DutyFreePaid = "All",
                        Type = "All",
                        EntryDataType = entryType,//x.Key.entryType,
                        StartDate = startDate,
                        EndDate = endDate
                    }).ToList();
                freashStart = false;
            }
        }

        public class ItemSalesPiSummary
        {
            public string ItemNumber { get; set; }
            public double? QtyAllocated { get; set; }
            public double PiQuantity { get; set; }
            public string pCNumber { get; set; }
            public int pLineNumber { get; set; }
            //public DateTime? pRegistrationDate { get; set; }
            public string DutyFreePaid { get; set; }
            public string Type { get; set; }
            public int PreviousItem_Id { get; set; }
            public double pQtyAllocated { get; set; }
            public string EntryDataType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        

        public async Task<List<DocumentCT>> CreateDutyFreePaidDocument(string dfp,
            IEnumerable<AllocationDataBlock> slst,
            AsycudaDocumentSet docSet, string documentType, bool isGrouped, List<ItemSalesPiSummary> itemSalesPiSummaryLst,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, 
            bool applyEx9Bucket, string ex9BucketType, bool applyHistoricChecks, bool applyCurrentChecks,
            bool autoAssess, bool perInvoice, bool overPIcheck, bool universalPIcheck, bool itemPIcheck, string prefix = null)
        {
            try
            {
               // applyHistoricChecks = false;
               // applyEx9Bucket = false;
                //applyCurrentChecks = false;
                if(InventoryDataCache == null) InventoryDataCache =
                    InventoryItemUtils.GetInventoryItems(BaseDataModel.Instance.CurrentApplicationSettings
                        .ApplicationSettingsId, false);

                var itmcount = 0;
                var docList = new List<DocumentCT>();

             

                if (checkForMultipleMonths)
                    if (slst.ToList().SelectMany(x => x.Allocations).Select(x => x.InvoiceDate.Month).Distinct()
                            .Count() > 1)
                    {
                        throw new ApplicationException(
                            string.Format("Data Contains Multiple Months", dfp));
                    }



                StatusModel.StatusUpdate($"Creating xBond Entries - {dfp}");

                var cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
                Ex9InitializeCdoc(dfp, cdoc, docSet, documentType, prefix);
                var effectiveAssessmentDate =
                    slst.SelectMany(x =>x.Allocations).Select(x =>
                                            x.EffectiveDate == DateTime.MinValue || x.EffectiveDate == null
                                                ? x.InvoiceDate
                                                : x.EffectiveDate).Min();
                foreach (var monthyear in slst) //.Where(x => x.DutyFreePaid == dfp)
                {

                    var prevEntryId = "";
                    var prevIM7 = "";
                    var elst = PrepareAllocationsData(monthyear, isGrouped);

                    
                    
                    foreach (var mypod in elst)
                    {
                        //itmcount = await InitializeDocumentCT(itmcount, prevEntryId, mypod, cdoc, prevIM7, monthyear, dt, dfp).ConfigureAwait(true);
                        if (!cdoc.EmailIds.Contains(mypod.EntlnData.EmailId))
                            cdoc.EmailIds.Add(mypod.EntlnData.EmailId);


                        if (!(mypod.EntlnData.Quantity > 0)) continue;
                        if (MaxLineCount(itmcount)
                            || InvoicePerEntry(perInvoice, prevEntryId, mypod)
                            || (cdoc.Document == null || itmcount == 0)
                            || IsPerIM7(prevIM7, monthyear))
                        {
                            if (itmcount != 0)
                            {
                                if (cdoc.Document != null)
                                {

                                    BaseDataModel.SaveAttachments(docSet, cdoc);
                                    AttachSupplier(cdoc);
                                    await SaveDocumentCT(cdoc).ConfigureAwait(false);
                                    docList.Add(cdoc);
                                    //}
                                    //else
                                    //{
                                    cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);

                                    effectiveAssessmentDate =
                                        monthyear.Allocations.Select(x =>
                                            x.EffectiveDate == DateTime.MinValue || x.EffectiveDate == null
                                                ? x.InvoiceDate
                                                : x.EffectiveDate).Min();
                                }
                            }

                            Ex9InitializeCdoc(dfp, cdoc, docSet, documentType, prefix);
                            if (PerIM7)
                                cdoc.Document.xcuda_Declarant.Number =
                                    cdoc.Document.xcuda_Declarant.Number.Replace(
                                        docSet.Declarant_Reference_Number,
                                        docSet.Declarant_Reference_Number +
                                        "-" +
                                        monthyear.CNumber);
                            InsertEntryIdintoRefNum(cdoc, mypod.EntlnData.EntryDataDetails.First().EntryDataId);

                            //  if (cdoc.Document.xcuda_General_information == null) cdoc.Document.xcuda_General_information = new xcuda_General_information(true) {TrackingState = TrackingState.Added};
                            cdoc.Document.xcuda_General_information.Comments_free_text =
                                $"EffectiveAssessmentDate:{effectiveAssessmentDate.GetValueOrDefault():MMM-dd-yyyy}";
                            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
                                effectiveAssessmentDate;
                            if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                            var curLst = mypod.EntlnData.EntryDataDetails.Select(x => x.Currency).Where(x => x != null)
                                .Distinct().ToList();
                            if (curLst.Any())
                            {
                                if (curLst.Count() > 1)
                                    throw new ApplicationException("EntryDataDetails Contains More than 1 Currencies");

                                if (cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code != curLst.First())
                                {
                                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = curLst.First();
                                }
                            }

                            itmcount = 0;
                        }


                        var newItms = await
                            CreateEx9EntryAsync(mypod, cdoc, itmcount, dfp, monthyear.MonthYear, documentType,
                                    itemSalesPiSummaryLst, checkQtyAllocatedGreaterThanPiQuantity, applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks, overPIcheck, universalPIcheck, itemPIcheck)
                                .ConfigureAwait(false);

                        itmcount += newItms;


                        prevEntryId = mypod.EntlnData.EntryDataDetails.Count() > 0
                            ? mypod.EntlnData.EntryDataDetails[0].EntryDataId
                            : "";
                        prevIM7 = PerIM7 == true ? monthyear.CNumber : "";
                        StatusModel.StatusUpdate();
                    }

                }
                BaseDataModel.SaveAttachments(docSet, cdoc);
                AttachSupplier(cdoc);
                await SaveDocumentCT(cdoc).ConfigureAwait(false);
                if (cdoc.Document.ASYCUDA_Id == 0)
                {
                    //clean up
                    docSet.xcuda_ASYCUDA_ExtendedProperties.Remove(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties);
                    cdoc.Document = null;
                }

                if(cdoc.Document != null) docList.Add(cdoc);
                return docList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void AttachSupplier(DocumentCT cdoc)
        {
            var firstItm = cdoc.DocumentItems.FirstOrDefault();
            if (firstItm == null) return;
            using (var ctx = new DocumentDSContext())
            {
                var exporter = ctx.xcuda_Exporter.FirstOrDefault(x => x.xcuda_Traders.xcuda_ASYCUDA.xcuda_Identification.xcuda_Registration.Number == firstItm.xcuda_PreviousItem.Prev_reg_nbr);
                if(exporter != null)
                {
                    cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name = exporter.Exporter_name;
                }
            }
        }


        private bool IsPerIM7(string prevIM7, AllocationDataBlock monthyear)
        {
            return (PerIM7 == true &&
                    (string.IsNullOrEmpty(prevIM7) ||
                     (!string.IsNullOrEmpty(prevIM7) && prevIM7 != monthyear.CNumber)));
        }

        private bool InvoicePerEntry(bool perInvoice,string prevEntryId, MyPodData mypod)
        {
            return (//BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry == true &&
                    perInvoice == true &&
                    //prevEntryId != "" &&
                    prevEntryId != mypod.EntlnData.EntryDataDetails[0].EntryDataId);
        }

        public bool MaxLineCount(int itmcount)
        {
            return (itmcount != 0 &&
                    itmcount %
                    BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines ==
                    0);
        }

        private async Task SaveDocumentCT(DocumentCT cdoc)
        {
            try
            {

                if (cdoc != null && cdoc.DocumentItems.Any() == true)
                {
                    if (cdoc.Document.xcuda_Valuation == null)
                        cdoc.Document.xcuda_Valuation = new xcuda_Valuation(true)
                        {
                            ASYCUDA_Id = cdoc.Document.ASYCUDA_Id,
                            TrackingState = TrackingState.Added
                        };
                    if (cdoc.Document.xcuda_Valuation.xcuda_Weight == null)
                        cdoc.Document.xcuda_Valuation.xcuda_Weight = new xcuda_Weight(true)
                        {
                            Valuation_Id = cdoc.Document.xcuda_Valuation.ASYCUDA_Id,
                            TrackingState = TrackingState.Added
                        };

                    var xcudaPreviousItems = cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Where(x => x != null)
                        .ToList();
                    if (xcudaPreviousItems.Any())
                    {
                        cdoc.Document.xcuda_Valuation.xcuda_Weight.Gross_weight =
                            (double) xcudaPreviousItems.Sum(x => x.Net_weight);
                    }


                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }




        private async Task<IEnumerable<AllocationDataBlock>> CreateAllocationDataBlocks(string filterExpression,
            List<string> errors,
            string dateFilter)
        {
            try
            {
                StatusModel.Timer("Getting ExBond Data");
                var slstSource = GetEX9Data(filterExpression, dateFilter).Where(x => !errors.Contains(x.ItemNumber)).ToList();
                StatusModel.StartStatusUpdate("Creating xBond Entries", slstSource.Count());
                IEnumerable<AllocationDataBlock> slst;
                slst = CreateWholeAllocationDataBlocks(slstSource);
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<EX9Allocations> GetEX9Data(string FilterExpression, string dateFilter)
        {
            try
            {

                if((BaseDataModel.Instance.CurrentApplicationSettings.ExportExpiredEntries ?? true))
                FilterExpression =
                    FilterExpression.Replace(
                        $"&& (pExpiryDate >= \"{ DateTime.Now.Date.ToShortDateString()}\" || pExpiryDate == null)",
                        "");
                FilterExpression =
                    FilterExpression.Replace("TariffCode.Contains", "TariffCode != null && TariffCode.Contains");

                FilterExpression += "&& DoNotAllocateSales != true " +
                                    "&& DoNotAllocatePreviousEntry != true " +
                                    "&& DoNotEX != true " +
                                    "&& Status == null " + // force no error execution
                                    //"&& AllocationErrors == null" +
                                    "&& WarehouseError == null " +

                                    $"&& (CustomsOperationId == {(int) CustomsOperations.Warehouse})";
                var res = new List<EX9Allocations>();


                var tres = GetEx9AsycudaSalesAllocations(dateFilter);



               var rres = tres
                    .Where(FilterExpression)
                    .ToList();
                foreach (var x in rres)
                {
                        try
                        {
                            var allocations = new EX9Allocations();
                            allocations.Type = x.Type;
                            allocations.AllocationId = x.AllocationId;
                            allocations.EntryData_Id = (int)x.EntryData_Id;
                            allocations.DutyFreePaid = x.DutyFreePaid;
                            allocations.EntryDataDetailsId = x.EntryDataDetailsId;
                            allocations.InvoiceDate = (DateTime)(x.EffectiveDate == null || x.EffectiveDate == DateTime.MinValue
                                ? x.InvoiceDate
                                : x.EffectiveDate.Value);
                            allocations.EffectiveDate = x.EffectiveDate;
                            allocations.InvoiceNo = x.InvoiceNo;
                            allocations.ItemDescription = x.ItemDescription;
                            allocations.ItemNumber = x.ItemNumber;
                            allocations.pCNumber = x.pCNumber;
                            allocations.pItemCost = x.pItemCost;
                            allocations.Status = x.Status;
                            allocations.EntryDataDetails = x.EntryDataDetails;
                            allocations.PreviousItem_Id = x.PreviousItem_Id;
                            allocations.QtyAllocated = x.QtyAllocated;
                            allocations.SalesFactor = x.SalesFactor;
                            allocations.SalesQtyAllocated = x.SalesQtyAllocated;
                            allocations.SalesQuantity = x.SalesQuantity;
                            allocations.pItemNumber = x.pItemNumber;
                            allocations.pItemDescription = x.Commercial_Description;
                            allocations.pTariffCode = x.pTariffCode;
                            allocations.pPrecision1 = x.pPrecision1;
                            allocations.DFQtyAllocated = x.DFQtyAllocated;
                            allocations.DPQtyAllocated = x.DPQtyAllocated;
                            allocations.pLineNumber = x.pLineNumber;
                            allocations.LineNumber = x.SalesLineNumber;
                            allocations.Comment = x.Comment;
                            allocations.Customs_clearance_office_code = x.Customs_clearance_office_code;
                            allocations.pQuantity = x.pQuantity;
                            allocations.pRegistrationDate = x.pRegistrationDate;
                            allocations.pAssessmentDate = x.AssessmentDate;
                            allocations.pExpiryDate = (DateTime)x.pExpiryDate.GetValueOrDefault();
                            allocations.Country_of_origin_code = x.Country_of_origin_code;
                            allocations.Total_CIF_itm = x.Total_CIF_itm;
                            allocations.Net_weight_itm = x.Net_weight_itm;
                            allocations.InventoryItemId = x.InventoryItemId ?? 0;
                            allocations.PIData = x.AsycudaSalesAllocationsPIData;
                            allocations.previousItems = x?.PreviousDocumentItem.EntryPreviousItems
                                .Select(y => y.xcuda_PreviousItem)
                                .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null ||
                                             y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true)
                                            && y.xcuda_Item.AsycudaDocument.Cancelled != true)
                                .Select(z => new previousItems()
                                {
                                    PreviousItem_Id = z.PreviousItem_Id,
                                    DutyFreePaid =
                                        z.xcuda_Item.AsycudaDocument.Customs_Procedure.CustomsOperationId ==
                                        (int)CustomsOperations.Exwarehouse &&
                                        z.xcuda_Item.AsycudaDocument.Customs_Procedure.IsPaid == true
                                            ? "Duty Paid"
                                            : "Duty Free",
                                    Net_weight = (double)z.Net_weight,
                                    Suplementary_Quantity = (double)z.Suplementary_Quantity
                                }).ToList();
                            allocations.TariffSupUnitLkps = x.PreviousDocumentItem?.xcuda_Tarification.xcuda_HScode.TariffCodes == null
                                ? new List<TariffSupUnitLkps>()
                                : x.PreviousDocumentItem?.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory
                                    ?.TariffCategoryCodeSuppUnit?.Select(z => z.TariffSupUnitLkps).ToList(); //.Select(x => (ITariffSupUnitLkp)x)
                            res.Add(allocations);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                }

             



                return res.OrderBy(x => x.AllocationId).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<EX9AsycudaSalesAllocations> _ex9AsycudaSalesAllocations = null;

        private static IQueryable<EX9AsycudaSalesAllocations> GetEx9AsycudaSalesAllocations(string filterExpression)
        {
            try
            {


                using (var ctx = new AllocationDSContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    

                    //if (_ex9AsycudaSalesAllocations == null)
                    //{
                    //////////////////////Cant load all data in memory too much
                        _ex9AsycudaSalesAllocations = ctx.EX9AsycudaSalesAllocations
                            .Include(x => x.AsycudaSalesAllocationsPIData)
                            .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
                            .Include("PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
                            .Include("PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations")
                            .Include("PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps")
                            .Where(filterExpression)
                            .AsNoTracking()

                            .ToList();
                    //}

                    return _ex9AsycudaSalesAllocations.AsQueryable();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }





        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource)
        {
            IEnumerable<AllocationDataBlock> slst;
            if (PerIM7 == true)
            {
                slst = CreateWholeIM7AllocationDataBlocks(slstSource);
            }
            else
            {
                slst = CreateWholeNonIM7AllocationDataBlocks(slstSource);
            }
            return slst;
        }

        private IEnumerable<AllocationDataBlock> CreateWholeNonIM7AllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource)
        {
            try
            {

                IEnumerable<AllocationDataBlock> slst;
                var source = slstSource.OrderBy(x => x.pTariffCode).ToList();

                slst = from s in source
                    group s by new {s.DutyFreePaid, s.Type, MonthYear = "NoMTY"}
                    into g
                    select new AllocationDataBlock
                    {
                        Type = g.Key.Type,
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateWholeIM7AllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            s.Type,
                            MonthYear = "NoMTY",
                            CNumber = s.pCNumber
                        }
                    into g
                    select new AllocationDataBlock
                    {
                        Type = g.Key.Type,
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                        CNumber = g.Key.CNumber
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }



        private IEnumerable<MyPodData> PrepareAllocationsData(AllocationDataBlock monthyear, bool isGrouped)
        {
            try
            {
                List<MyPodData> elst;
                //if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == true)
                //{
                elst = isGrouped 
                    ? GroupAllocationsByPreviousItem(monthyear) 
                    : SingleAllocationPerPreviousItem(monthyear);
                //}
                //else
                //{
                    //elst = GroupAllocations(monthyear);
                //}

                return elst.ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }

        

        private List<MyPodData> GroupAllocationsByPreviousItem(AllocationDataBlock monthyear)
        {
            try
            {
                var elst = monthyear.Allocations
                              .OrderBy(x => x.pTariffCode)
                              .GroupBy(x =>  x.PreviousItem_Id)
                              .Select(s => CreateMyPodData(s.ToList())).ToList();
               


                return elst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static MyPodData CreateMyPodData(List<EX9Allocations> s)
        {
            try
            {

                return new MyPodData
                {
                    Allocations = s.OrderByDescending(x => x.AllocationId).Select(x =>
                        new AsycudaSalesAllocations()
                        {
                            AllocationId = x.AllocationId,
                            PreviousItem_Id = x.PreviousItem_Id,
                            EntryDataDetailsId = x.EntryDataDetailsId,
                            Status = x.Status,
                            QtyAllocated = x.QtyAllocated,
                            PIData = x.PIData,
                            //EntryDataDetails = new EntryDataDetails()
                            //{
                            //    EntryDataDetailsId = x.EntryDataDetailsId.GetValueOrDefault(),
                            //    EntryData_Id = x.EntryData_Id,
                            //    EntryDataId = x.InvoiceNo,
                            //    Quantity = x.SalesQuantity,
                            //    QtyAllocated = x.SalesQtyAllocated,
                            //    EffectiveDate = x.EffectiveDate,
                            //    LineNumber = x.LineNumber,
                            //    Comment = x.Comment,
                            //    AsycudaDocumentItemEntryDataDetails = x.EntryDataDetails.AsycudaDocumentItemEntryDataDetails
                            //},
                            EntryDataDetails = x.EntryDataDetails,


                        }).ToList(),
                    AllNames = s.SelectMany(x =>
                        {
                            var alias = InventoryDataCache
                                .Where(c => c.Id == x.InventoryItemId)
                                .SelectMany(i => i.InventoryItemAlias)
                                .ToList();
                            
                            var aliasNames = alias.Select(a => a.AliasName).ToList();

                            var aliasAlias = InventoryDataCache.Where(z => z.InventoryItemAlias.Any(a => aliasNames.Contains(a.AliasName)) ).SelectMany(z => z.InventoryItemAlias).ToList();
                            //.Where(c => alias.Select(z => z.AliasItemId).ToList().Any(z => z == c.Id))
                            //.SelectMany(i => i.InventoryItemAlias);
                            var aaNames = aliasAlias.Select(a => a.AliasName).ToList();

                            

                            var firstLst = aliasNames.Union(aaNames).ToList();

                            var secondLst = MoreEnumerable.Append(firstLst, x.ItemNumber);

                            return secondLst;
                        })
                        .Distinct()
                        .ToList(),
                    EntlnData = new AlloEntryLineData
                    {
                        ItemNumber = s.LastOrDefault()?.ItemNumber,
                        ItemDescription = s.LastOrDefault()?.ItemDescription,
                        TariffCode = s.LastOrDefault()?.pTariffCode,
                        Cost = s.LastOrDefault()?.pItemCost.GetValueOrDefault()??0.0,
                        FileTypeId = s.LastOrDefault()?.FileTypeId,
                        EmailId = s.LastOrDefault()?.EmailId,
                        Quantity = s.Sum(x => x.QtyAllocated / (x.SalesFactor == 0 ? 1 : x.SalesFactor)),
                        EntryDataDetails = s.DistinctBy(z => z.EntryDataDetailsId).Select(z =>
                            new EntryDataDetailSummary()
                            {
                                EntryDataDetailsId = z.EntryDataDetailsId.GetValueOrDefault(),
                                EntryDataId = z.InvoiceNo,
                                EntryData_Id = z.EntryData_Id,
                                QtyAllocated = z.SalesQuantity,
                                EntryDataDate = z.InvoiceDate,
                                EffectiveDate = z.EffectiveDate.GetValueOrDefault(),
                                Currency = z.Currency,
                                Comment = z.Comment
                            }).ToList(),
                        PreviousDocumentItemId = Convert.ToInt32(s.LastOrDefault()?.PreviousItem_Id ?? 0),
                        InternalFreight = s.LastOrDefault()?.InternalFreight ?? 0.0,
                        Freight = s.LastOrDefault()?.Freight ?? 0.0,
                        Weight = s.LastOrDefault()?.Weight ?? 0.0,
                        pDocumentItem = new pDocumentItem()
                        {
                            DFQtyAllocated = s.LastOrDefault()?.DFQtyAllocated??0,
                            DPQtyAllocated = s.LastOrDefault()?.DPQtyAllocated??0,
                            ItemQuantity = s.LastOrDefault()?.pQuantity.GetValueOrDefault()??0,
                            LineNumber = s.LastOrDefault()?.pLineNumber??0,
                            ItemNumber = s.LastOrDefault()?.pItemNumber,

                            Description = s.LastOrDefault()?.pItemDescription,
                            xcuda_ItemId = s.LastOrDefault()?.PreviousItem_Id.GetValueOrDefault()??0,
                            AssessmentDate = s.LastOrDefault()?.pAssessmentDate??DateTime.MinValue,
                            ExpiryDate = s.LastOrDefault()?.pExpiryDate??DateTime.MinValue,
                            previousItems = s.LastOrDefault().previousItems
                        },
                        EX9Allocation = new EX9Allocation()
                        {
                            SalesFactor = s.LastOrDefault()?.SalesFactor ?? 0.0,
                            Net_weight_itm = s.LastOrDefault()?.Net_weight_itm??0.0,
                            pQuantity = s.LastOrDefault()?.pQuantity.GetValueOrDefault() ?? 0.0,
                            pCNumber = s.LastOrDefault()?.pCNumber,
                            Customs_clearance_office_code = s.LastOrDefault()?.Customs_clearance_office_code,
                            Country_of_origin_code = s.LastOrDefault()?.Country_of_origin_code,
                            pRegistrationDate = s.LastOrDefault()?.pRegistrationDate??DateTime.MinValue,
                            pQtyAllocated = s.LastOrDefault()?.QtyAllocated ?? 0.0,
                            Total_CIF_itm = s.LastOrDefault()?.Total_CIF_itm ?? 0.0,
                            pTariffCode = s.LastOrDefault()?.pTariffCode,
                            pPrecision1 = s.LastOrDefault()?.pPrecision1
                        },
                        TariffSupUnitLkps = s.LastOrDefault()?.TariffSupUnitLkps == null ? new List<ITariffSupUnitLkp>(): s.LastOrDefault()?.TariffSupUnitLkps.Select(x => (ITariffSupUnitLkp)x)
                            .ToList()

                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<InventoryItem> InventoryDataCache { get; private set; }

        private List<MyPodData> SingleAllocationPerPreviousItem(AllocationDataBlock monthyear)
        {
            try
            {
                var xSalesByItemId = monthyear.Allocations
                    .OrderBy(x => x.AllocationId).Select(x => (Item: (x.ItemNumber, x.InventoryItemId), xSale: x))
                    .GroupBy(x => x.Item)
                    .Select(x => (Key: x.Key, Value: x.Select(i => i.xSale).ToList()))
                    .ToList();

                var groupedlst = Utils.CreateItemSet<EX9Allocations>(xSalesByItemId);

                //var groupedlst = monthyear.Allocations
                //    .OrderBy(x => x.AllocationId)
                //    .GroupBy(x => x.PreviousItem_Id)
                //    .ToList();

                var elst = new List<List<EX9Allocations>>();

                foreach (var group in groupedlst)
                {
                    var lst = group.Value.SelectMany(z => z.Value.Select(v => v).ToList()).ToList();
                    var nlst = new List<EX9Allocations>();
                    while (lst.Any())
                    {
                        // focus on returns ignore weight for now

                        var itm = lst.First();
                        if (itm.QtyAllocated > 0)
                        {
                            if (nlst.Sum(x => x.QtyAllocated) + itm.QtyAllocated > 0)
                            {
                                nlst = new List<EX9Allocations> { itm };
                                lst.RemoveAt(0);
                            }
                            else
                            {
                                nlst.Add(itm);
                                lst.RemoveAt(0);
                            }

                        }
                        else
                        {
                            if (nlst.Sum(x => x.QtyAllocated) + itm.QtyAllocated > 0)
                            {
                                if (nlst.Any() && nlst.Sum(x => x.QtyAllocated) > 0) elst.Add(nlst);
                                nlst = new List<EX9Allocations> { itm };
                                lst.RemoveAt(0);
                            }
                            else
                            {
                                nlst.Add(itm);
                                lst.RemoveAt(0);
                            }
                        }

                        if (nlst.Any() && nlst.Sum(x => x.QtyAllocated) > 0) elst.Add(nlst);
                    }

                    if (nlst.Any() && nlst.Sum(x => x.QtyAllocated) < 0)
                        updateXStatus(
                            nlst.Select(x => new AsycudaSalesAllocations()
                            {
                                AllocationId = x.AllocationId, EntryDataDetailsId = x.EntryDataDetailsId,
                                PreviousItem_Id = x.PreviousItem_Id
                            }).ToList(), $"Set < 0: {nlst.Sum(x => x.QtyAllocated)}");
                }

                var res = elst.Select(CreateMyPodData).ToList();

              
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void InsertEntryIdintoRefNum(DocumentCT cdoc, string entryDataId)
        {
            try
            {
                if (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry == true)
                {
                    cdoc.Document.xcuda_Declarant.Number = entryDataId;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<int> CreateEx9EntryAsync(MyPodData mypod, DocumentCT cdoc, int itmcount, string dfp,
            string monthyear,
            string documentType, List<ItemSalesPiSummary> itemSalesPiSummaryLst,
            bool checkQtyAllocatedGreaterThanPiQuantity,
            bool applyEx9Bucket, string ex9BucketType, bool applyHistoricChecks, bool applyCurrentChecks,
            bool overPIcheck, bool universalPIcheck, bool itemPIcheck)
        {
            try
            {
                // clear all xstatus so know what happened
                updateXStatus(mypod.Allocations,null);

                if (mypod.EntlnData.pDocumentItem.ExpiryDate <= DateTime.Now && (BaseDataModel.Instance.CurrentApplicationSettings.ExportExpiredEntries??true) == false)
                {
                    updateXStatus(mypod.Allocations,
                        $@"Expired Entry: '{
                                mypod.EntlnData.pDocumentItem.ExpiryDate.ToShortDateString()}'");
                    return 0;
                }


                /////////////// QtyAllocated >= piQuantity cap
                ///
                /// 
                /// 
                if (checkQtyAllocatedGreaterThanPiQuantity)
                {
                   
                    //var psum = mypod.EntlnData.pDocumentItem.previousItems
                    //    .DistinctBy(x => x.PreviousItem_Id)
                    //    .Select(x => x.Suplementary_Quantity).DefaultIfEmpty(0).Sum();
                    //if (mypod.EntlnData.pDocumentItem.QtyAllocated <= psum)
                    //{
                    //    updateXStatus(mypod.Allocations,
                    //        $@"Failed QtyAllocated <= PiQuantity:: QtyAllocated: {
                    //                mypod.EntlnData.pDocumentItem.QtyAllocated
                    //            } PiQuantity: {psum}");
                    //    return 0;
                    //}

                   
                }
                //////////////////////////////////////////////////////////////////////////
                ///     Sales Cap/ Sales Bucket historic

                List<ItemSalesPiSummary> salesPiAll = new List<ItemSalesPiSummary>();
                List<ItemSalesPiSummary> universalData = new List<ItemSalesPiSummary>(); ;
                List<ItemSalesPiSummary> salesPiHistoric;
                List<ItemSalesPiSummary> salesPiCurrent;
                List<ItemSalesPiSummary> itemSalesPiHistoric = new List<ItemSalesPiSummary>();
                List<ItemSalesPiSummary> itemSalesPiCurrent = new List<ItemSalesPiSummary>();

                universalData = itemSalesPiSummaryLst.Where(x => (mypod.AllNames.Contains(x.ItemNumber) || x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId)
                                                              && x.Type == "Universal").ToList();

                salesPiAll = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                              //--------took out the name and line because the id more precise especially with item aliases - rouge - "14479 vs 014479"
                                                               // x.ItemNumber == mypod.EntlnData.ItemNumber &&
                                                              // x.pCNumber == mypod.EntlnData.EX9Allocation
                                                              //    .pCNumber // donot disable this because previous month pi are not included
                                                              //&& x.pLineNumber == mypod.EntlnData.pDocumentItem
                                                              //    .LineNumber
                                                              && x.Type == "All").ToList();
                salesPiHistoric = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                                   //x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                   // took this out because of changed allocation can lead to overallocation
                                                                   //eg HS/SAN2, nov 2018 - reallocated to HS/SAN2 but orignally allocated to HS-SAN2

                                                                   //&& x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                                   //&& x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                                                                   && x.Type == "Historic").ToList();
                salesPiCurrent = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                                  //  x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                  //&& x.pCNumber == mypod.EntlnData.EX9Allocation
                                                                  //    .pCNumber // donot disable this because previous month pi are not included
                                                                  //&& x.pLineNumber == mypod.EntlnData.pDocumentItem
                                                                  //    .LineNumber
                                                                  && x.Type == "Current").ToList();

                itemSalesPiHistoric = itemSalesPiSummaryLst.Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId
                                                                       //x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                       //&& x.pCNumber == mypod.EntlnData
                                                                       //    .EX9Allocation.pCNumber
                                                                       //&& x.pLineNumber ==
                                                                       //mypod.EntlnData.pDocumentItem.LineNumber
                                                                       && x.Type == "Historic").ToList();
                //itemSalesPiCurrent = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber
                //                                                  && x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                //                                                  && x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                //                                                  && x.Type == "Current").ToList();


                Debug.WriteLine(
                    $"Create EX9 For {mypod.EntlnData.ItemNumber}:{mypod.EntlnData.EntryDataDetails.First().EntryDataDate:MMM-yy} - {mypod.EntlnData.Quantity} | C#{mypod.EntlnData.EX9Allocation.pCNumber}-{mypod.EntlnData.pDocumentItem.LineNumber}");
                salesPiHistoric.ForEach(x =>
                    Debug.WriteLine(
                        $"Sales vs Pi History: {x.QtyAllocated} of {x.pQtyAllocated} - {x.PiQuantity} | C#{x.pCNumber}-{x.pLineNumber}"));
                var entryType = salesPiAll.FirstOrDefault()?.EntryDataType?? documentType;

                var docSetPiLst = DocSetPi
                    .Where(x => x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId)
                    .ToList();

                var docPi = docSetPiLst
                    .Select(x => x.TotalQuantity)
                    .DefaultIfEmpty(0).Sum();

                var itemDocPi = docSetPiLst
                    .Where(x => x.Type == entryType)
                    .Where(x => x.DutyFreePaid == dfp)//c#17227 line 40 -Appid = 7
                    .Select(x => x.TotalQuantity)
                    .DefaultIfEmpty(0).Sum();

                var itemNumberDocPi = docSetPiLst
                    .Where(x => x.Type == entryType)
                    .Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber)
                    .Where(x => x.DutyFreePaid == dfp)//c#17227 line 40 -Appid = 7
                    .Select(x => x.TotalQuantity)
                    .DefaultIfEmpty(0).Sum();

                // var universalSalesAll = (double)universalData.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault()?.pQtyAllocated).DefaultIfEmpty(0.0).Sum();

                var previousItemUniversalData = universalData.GroupBy(x => x.PreviousItem_Id).ToList();

                var universalSalesAll = (double) previousItemUniversalData.Select(x => x.FirstOrDefault(q => q.pQtyAllocated > 0)?.pQtyAllocated).DefaultIfEmpty(0.0).Sum();
                var universalPiAll = (double) previousItemUniversalData.Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();

               // var totalSalesAll = (double)salesPiAll.GroupBy(x => x.PreviousItem_Id).SelectMany(x => x.Select(z => z.QtyAllocated)).DefaultIfEmpty(0.0).Sum();
                var totalSalesAll = (double) salesPiAll.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.pQtyAllocated > 0)?.pQtyAllocated).DefaultIfEmpty(0.0).Sum();
                var totalPiAll = (double) salesPiAll.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();
                var totalSalesHistoric = (double)salesPiHistoric.GroupBy(x => x.PreviousItem_Id).SelectMany(x => x.Select(z => z.QtyAllocated)).DefaultIfEmpty(0.0).Sum();
                //var totalSalesHistoric = (double) salesPiHistoric.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.pQtyAllocated > 0)?.pQtyAllocated).DefaultIfEmpty(0.0).Sum();
                var totalPiHistoric = (double) salesPiHistoric.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();
                var totalSalesCurrent = (double) salesPiCurrent.Select(x => x.QtyAllocated).DefaultIfEmpty(0.0).Sum();
                var totalPiCurrent = (double) salesPiCurrent.GroupBy(x => x.PreviousItem_Id).Select(x => x.FirstOrDefault(q => q.PiQuantity > 0)?.PiQuantity)
                    .DefaultIfEmpty(0.0).Sum();

                var itemPiHistoric = itemSalesPiHistoric.GroupBy(x => x.PreviousItem_Id)
                   .Select(x => x.First().PiQuantity).DefaultIfEmpty(0).Sum();

                var itemSalesHistoric = (double)itemSalesPiHistoric.Select(x => x.QtyAllocated).DefaultIfEmpty(0.0).Sum();

              

                var preEx9Bucket = mypod.EntlnData.Quantity;
                if (applyEx9Bucket)
                    if (ex9BucketType == "Current")
                    {
                        await Ex9Bucket(mypod, dfp, docPi, salesPiCurrent.Any() ? salesPiCurrent : salesPiAll)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        //await Ex9Bucket(mypod, dfp, docPi,
                        //    salesPiHistoric //i assume once you not doing historic checks especially for adjustments just use the specific item history
                        //).ConfigureAwait(false); //historic = 'HCLAMP/060'

                        //await Ex9Bucket(mypod, dfp, docPi,
                        //    salesPiCurrent //i assume once you not doing historic checks especially for adjustments just use the specific item history
                        //).ConfigureAwait(false); //historic = 'HCLAMP/060'

                        Ex9Bucket(mypod, mypod.EntlnData.Quantity, totalSalesHistoric, totalPiHistoric, "Historic");
                        Ex9Bucket(mypod, mypod.EntlnData.Quantity, totalSalesCurrent, totalPiCurrent, "Current");



                    }

                var salesFactor = Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                    ? 1
                    : mypod.EntlnData.EX9Allocation.SalesFactor;


                

                if (overPIcheck)
                    if (Math.Round( totalSalesAll, 2) <
                        Math.Round((totalPiAll  + docPi + mypod.EntlnData.Quantity) * salesFactor, 2))//
                    {
                        var availibleQty = Math.Round(totalSalesAll, 2) - (Math.Round(totalPiAll, 2)+ docPi);
                        if (availibleQty <= 0)
                        {
                            updateXStatus(mypod.Allocations,
                                $@"Failed All Sales Check:: Total All Sales:{Math.Round(totalSalesAll, 2)}
                                            Total All PI: {totalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}");
                            return 0;
                        }

                        Ex9Bucket(mypod, availibleQty, totalSalesAll, totalPiAll, "Total");
                    }

                if (universalPIcheck)
                    if (Math.Round(universalSalesAll, 2) <
                        Math.Round((universalPiAll + docPi + mypod.EntlnData.Quantity) * salesFactor, 2))//
                    {
                        var availibleQty = Math.Round(universalSalesAll, 2) - (Math.Round(universalPiAll, 2)+ docPi);
                        if (availibleQty <= 0)
                        {
                            updateXStatus(mypod.Allocations,
                                $@"Failed universal Sales Check:: Universal Sales:{Math.Round(universalSalesAll, 2)}
                                            Universal PI: {universalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}");
                            return 0;
                        }

                        Ex9Bucket(mypod, availibleQty, universalSalesAll, universalPiAll, "Universal");

                    }

                mypod.EntlnData.Quantity = Math.Round(mypod.EntlnData.Quantity, 2);
                Debug.WriteLine($"EX9Bucket Quantity {mypod.EntlnData.ItemNumber} - {mypod.EntlnData.Quantity}");
                if (mypod.EntlnData.Quantity <= 0)
                {
                    updateXStatus(mypod.Allocations,
                        $@"Failed Ex9Bucket set Qty to Zero:: preQty: {preEx9Bucket}");
                    return 0;
                }

                ////////////////////////----------------- Cap to prevent xQuantity > Sales Quantity
                double qtyAllocated = 0;
                foreach (var allocation in mypod.Allocations)
                {
                    qtyAllocated += allocation.QtyAllocated /
                                    (Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                                        ? 1
                                        : mypod.EntlnData.EX9Allocation.SalesFactor);
                    allocation.xStatus = "";
                }


                // todo: ensure allocations are marked for investigation
                double qty = mypod.EntlnData.Quantity;
                if ((qty - Math.Round(qtyAllocated, 2))  > 0.0001)
                {
                    updateXStatus(mypod.Allocations,
                        $@"Failed Quantity vs QtyAllocated:: Qty: {qty} QtyAllocated: {qtyAllocated}");
                    return 0;
                }
                //////////////////////////////////////////////////////////////////////////
                ///     Sales Cap/ Sales Bucket historic



               


                if (totalSalesAll == 0)// && mypod.Allocations.FirstOrDefault()?.Status != "Short Shipped"
                {
                    updateXStatus(mypod.Allocations,
                        $@"No Sales Found");
                    return 0; // no sales found
                }


                


                if (applyHistoricChecks)
                {

                    if (totalSalesHistoric == 0)// && mypod.Allocations.FirstOrDefault()?.Status != "Short Shipped"
                    {
                        updateXStatus(mypod.Allocations,
                            $@"No Historical Sales Found");
                        return 0; // no sales found
                    }


                    var docDFPpi = DocSetPi.Where(x => x.DutyFreePaid == dfp && x.PreviousItem_Id == mypod.EntlnData.PreviousDocumentItemId).Sum(x => x.TotalQuantity);
                    if (Math.Round(totalSalesHistoric, 2) <
                        Math.Round((totalPiHistoric + docDFPpi + mypod.EntlnData.Quantity) * salesFactor, 2))// take out docpi because its included in sales
                    {
                        //updateXStatus(mypod.Allocations,
                        //    $@"Failed Historical Check:: Total Historic Sales:{Math.Round(totalSalesHistoric, 2)}
                        //       Total Historic PI: {totalPiHistoric}
                        //       xQuantity:{mypod.EntlnData.Quantity}");
                        //return 0;
                        var availibleQty = totalSalesHistoric - (totalPiHistoric + docDFPpi);
                        if (availibleQty != 0) Ex9Bucket(mypod, availibleQty, totalSalesHistoric, totalPiHistoric, "Historic");
                        if (mypod.EntlnData.Quantity == 0)
                        {
                            updateXStatus(mypod.Allocations,
                                $@"Failed Historical Check:: Total Historic Sales:{Math.Round(totalSalesHistoric, 2)}
                                   Total Historic PI: {totalPiHistoric}
                                   xQuantity:{mypod.EntlnData.Quantity}");
                            return 0;
                        }
                    }
                }
                ////////////////////////////////////////////////////////////////////////

                if (applyCurrentChecks)
                {
                    //////////////////////////////////////////////////////////////////////////
                    ///     Sales Cap/ Sales Bucket Current


                    if (totalSalesCurrent == 0)// && mypod.Allocations.FirstOrDefault()?.Status != "Short Shipped"
                    {
                        updateXStatus(mypod.Allocations,
                            $@"No Current Sales Found");
                        return 0;
                    }

                    // no sales found


                    if (Math.Round(totalSalesCurrent, 2) <
                        Math.Round((totalPiCurrent + mypod.EntlnData.Quantity) * salesFactor, 2))
                    {
                        updateXStatus(mypod.Allocations,
                            $@"Failed Current Check:: Total Current Sales:{Math.Round(totalSalesCurrent, 2)}
                               Total Current PI: {totalPiCurrent}
                               xQuantity:{mypod.EntlnData.Quantity}");
                        return 0;
                    }
                }

                // mandatory check to prevent overtaking
                if (mypod.EntlnData.pDocumentItem.ItemQuantity <
                       Math.Round((totalPiAll + docPi + mypod.EntlnData.Quantity), 2))
                {
                    updateXStatus(mypod.Allocations,
                        $@"Failed ItemQuantity < totalPiAll & xQuantity:{
                            mypod.EntlnData.pDocumentItem.ItemQuantity
                        }
                               totalPiAll PI: {totalPiAll}
                               xQuantity:{mypod.EntlnData.Quantity}");
                    return 0;
                }

                //// item sales vs item pi, prevents early exwarehouse when its just one totalsales vs totalpi
                //if(documentType != "DIS")
                if (itemPIcheck)
                {
                    /// re-enabled for EX9ALL because its over taking - Appid = 6, INV: 29916-5803 
                    if (Math.Round(itemSalesHistoric, 2) <
                        Math.Round((itemPiHistoric + itemDocPi + mypod.EntlnData.Quantity) * salesFactor, 2))
                    {
                        //updateXStatus(mypod.Allocations,
                        //    $@"Failed Historical Check:: Total Historic Sales:{Math.Round(totalSalesHistoric, 2)}
                        //       Total Historic PI: {totalPiHistoric}
                        //       xQuantity:{mypod.EntlnData.Quantity}");
                        //return 0;
                        var availibleQty = itemSalesHistoric - (itemPiHistoric + itemDocPi);
                        Ex9Bucket(mypod, availibleQty, itemSalesHistoric, itemPiHistoric, "Historic");
                        if (mypod.EntlnData.Quantity == 0)
                        {
                            updateXStatus(mypod.Allocations,
                                $@"Failed Item Historical Check:: Item Historic Sales:{Math.Round(itemSalesHistoric, 2)}
                                   Item Historic PI: {itemPiHistoric}
                                   xQuantity:{mypod.EntlnData.Quantity}");
                            return 0;
                        }
                    }

                   
                     ////////////////////////////////////////////////////////////////////////
                    //// Cap to prevent over creation of ex9 vs Item Quantity espectially if creating Duty paid and Duty Free at same time

                    if (mypod.EntlnData.pDocumentItem.ItemQuantity <
                        Math.Round(itemPiHistoric + itemDocPi + mypod.EntlnData.Quantity, 2))
                    {

                        var availibleQty = mypod.EntlnData.pDocumentItem.ItemQuantity - (itemPiHistoric + itemDocPi);
                        /// pass item quantity to ex9 bucket
                        Debugger.Break();
                        Ex9Bucket(mypod, availibleQty, itemSalesHistoric, itemPiHistoric, "Historic");
                        if (mypod.EntlnData.Quantity == 0)
                        {
                            updateXStatus(mypod.Allocations,
                                $@"Failed ItemQuantity < ItemPIHistoric & xQuantity:{mypod.EntlnData.pDocumentItem.ItemQuantity}
                               Item Historic PI: {itemPiHistoric}
                               xQuantity:{mypod.EntlnData.Quantity}");
                            return 0;
                        }
                    }


                  

                }
                
                ////////////////////////////////////////////////////////////////////////
                    //// Sales dependent check to prevent sales overexwarehouse even when Allocations changed. all other checks are previous document dependent
                            var xSalesPi = mypod.Allocations
                        .Select(x => new {e = x.EntryDataDetails.Quantity,pi = x.EntryDataDetails.AsycudaDocumentItemEntryDataDetails.Sum(z => z.Quantity)?? 0}).Sum(x => x.e - x.pi) ;
                    ///// the DONT FORGET WE TAKING JUST REMAINING SALES SO IT WILL BE ZERO EVEN IF ITS ALREADY DOUBLE EX-WAREHOUSED
                    if (xSalesPi < Math.Round(mypod.EntlnData.Quantity, 2))//take out docpi its throwing it ouff   //itemDocPi + 
                    {

                        var availibleQty = xSalesPi - Math.Round( mypod.EntlnData.Quantity, 2);//take out docpi its throwing it ouff   //itemDocPi + 
                    /// pass item quantity to ex9 bucket
                    if (availibleQty >= 0)
                            Ex9Bucket(mypod, availibleQty, itemSalesHistoric, itemPiHistoric, "Historic");
                        if (mypod.EntlnData.Quantity <= 0 || availibleQty <= 0)
                        {
                            updateXStatus(mypod.Allocations,
                                $@"Failed Existing xSales already taken out..:{mypod.Allocations.SelectMany(x => x.EntryDataDetails.AsycudaDocumentItemEntryDataDetails.Select(n => $"c#{n.CNumber}|{n.LineNumber}").ToList()).DefaultIfEmpty("").Aggregate((o,n) => $"{o}, {n}")}");
                            return 0;
                        }
                    }

                    if (mypod.EntlnData.Quantity <= 0) return 0;
                //////////////////// can't delump allocations because of returns and 1kg weights issue too much items wont be able to exwarehouse
                var itmsToBeCreated = 1;
                var itmsCreated = 0;


                for (int i = 0; i < itmsToBeCreated; i++)
                {

                    var lineData =
                        mypod.EntlnData; ///itmsToBeCreated == 1 ? mypod.EntlnData : CreateLineData(mypod, i);

                    global::DocumentItemDS.Business.Entities.xcuda_PreviousItem pitm = CreatePreviousItem(
                        lineData,
                        itmcount + i, dfp);
                    if (Math.Round(pitm.Net_weight, 2) < (decimal) 0.01)
                    {
                        updateXStatus(mypod.Allocations,
                            $@"Failed PiNetWeight < 0.01 :: PiNetWeight:{Math.Round(pitm.Net_weight, 2)}");
                        return 0;
                    }

                    pitm.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;


                    global::DocumentItemDS.Business.Entities.xcuda_Item itm =
                        BaseDataModel.Instance.CreateItemFromEntryDataDetail(lineData, cdoc);
                    itm.EmailId = lineData.EmailId;
                    itm.xcuda_Valuation_item.Total_CIF_itm = pitm.Current_value;
                    itm.xcuda_Tarification.xcuda_HScode.Precision_4 = lineData.pDocumentItem.ItemNumber;
                    itm.xcuda_Tarification.xcuda_HScode.Precision_1 = lineData.EX9Allocation.pPrecision1;
                    itm.xcuda_Goods_description.Commercial_Description =
                        BaseDataModel.Instance.CleanText(lineData.pDocumentItem.Description);
                    itm.IsAssessed = false;
                    itm.SalesFactor = lineData.EX9Allocation.SalesFactor;
                    //TODO:Refactor this dup code
                    if (mypod.Allocations != null)
                    {
                        var itmcnt = 1;
                        foreach (
                            var allo in (mypod.Allocations as List<AsycudaSalesAllocations>)) //.Distinct()
                        {
                            itm.xBondAllocations.Add(new xBondAllocations(true)
                            {
                                AllocationId = allo.AllocationId,
                                xcuda_Item = itm,
                                TrackingState = TrackingState.Added
                            });

                            itmcnt = AddFreeText(itmcnt, itm, allo.EntryDataDetails.EntryDataId,
                                allo.EntryDataDetails.LineNumber.GetValueOrDefault(), allo.EntryDataDetails.Comment,
                                mypod.EntlnData.ItemNumber);
                        }


                    }



                    itm.xcuda_PreviousItem = pitm;
                    pitm.xcuda_Item = itm;

                    itm.ItemQuantity = (double) pitm.Suplementary_Quantity; // taking the previous item quantity because thats governing thing when exwarehousing. c#36689 51 9/1/2021

                    var previousItems = new previousItems()
                    {
                        DutyFreePaid = dfp, Net_weight = (double) pitm.Net_weight,
                        PreviousItem_Id = pitm.PreviousItem_Id,
                        Suplementary_Quantity = (double) pitm.Suplementary_Quantity
                    };
                    if (docPreviousItems.ContainsKey(lineData.PreviousDocumentItemId))
                    {
                        docPreviousItems[lineData.PreviousDocumentItemId].Add(previousItems);
                    }
                    else
                    {
                        docPreviousItems.Add(lineData.PreviousDocumentItemId,
                            new List<previousItems>() {previousItems});
                    }

                    


                    var ep = new EntryPreviousItems(true)
                    {
                        Item_Id = lineData.PreviousDocumentItemId,
                        PreviousItem_Id = pitm.PreviousItem_Id,
                        TrackingState = TrackingState.Added
                    };
                    pitm.xcuda_Items.Add(ep);





                    if (cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Count() == 1 || itmcount == 0)
                    {
                        pitm.Packages_number = "1"; //(i.Packages.Number_of_packages).ToString();
                        pitm.Previous_Packages_number = pitm.Previous_item_number == "1" ? "1" : "0";


                        itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                        {
                            Attached_document_code =
                                BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                                    x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                        .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "DFS1",
                            Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                            Attached_document_reference = cdoc.Document.ReferenceNumber,
                            xcuda_Attachments = new List<xcuda_Attachments>()
                            {
                                new xcuda_Attachments(true)
                                {
                                    Attachments = new Attachments(true)
                                    {
                                        FilePath = Path.Combine(
                                            BaseDataModel.Instance.CurrentApplicationSettings.DataFolder == null
                                                ? cdoc.Document.ReferenceNumber + ".csv.pdf"
                                                : $"{BaseDataModel.Instance.CurrentApplicationSettings.DataFolder}\\{cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number}\\{cdoc.Document.ReferenceNumber}.csv.pdf"),
                                        TrackingState = TrackingState.Added,
                                        DocumentCode = BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                                            x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                                .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "DFS1",
                                        EmailId = lineData.EmailId?.ToString(),
                                        Reference = cdoc.Document.ReferenceNumber,
                                    },

                                    TrackingState = TrackingState.Added
                                }
                            },
                            TrackingState = TrackingState.Added
                        });





                    }
                    else
                    {
                        if (pitm.Packages_number == null)
                        {
                            pitm.Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                            pitm.Previous_Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                        }
                    }

                    if (pitm.Previous_Packages_number != null && pitm.Previous_Packages_number != "0")
                    {
                        var pkg = itm.xcuda_Packages.FirstOrDefault();
                        if (pkg == null)
                        {
                            pkg = new xcuda_Packages(true)
                            {
                                Item_Id = itm.Item_Id,
                                Marks1_of_packages = "Marks",
                                TrackingState = TrackingState.Added
                            };
                            itm.xcuda_Packages.Add(pkg);
                        }

                        pkg.Number_of_packages =
                            Convert.ToDouble(pitm.Previous_Packages_number);
                    }





                    itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pitm.Hs_code;
                    itm.xcuda_Goods_description.Country_of_origin_code = pitm.Goods_origin;



                    itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
                    {
                        TrackingState = TrackingState.Added,
                        Gross_weight_itm = (double) pitm.Net_weight,
                        Net_weight_itm = (double) pitm.Net_weight
                    };
                    // adjusting because not using real statistical value when calculating
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency =
                        Convert.ToDouble(Math.Round((pitm.Current_value * (double) pitm.Suplementary_Quantity), 2));
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency =
                        Convert.ToDouble(Math.Round(pitm.Current_value * (double) pitm.Suplementary_Quantity, 2));
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_code = "XCD";
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_rate = 1;

                    DocSetPi.Add(new PiSummary()
                    {
                        ItemNumber = mypod.EntlnData.ItemNumber,
                        PreviousItem_Id = mypod.EntlnData.PreviousDocumentItemId,
                        DutyFreePaid = dfp,
                        Type = entryType,
                        TotalQuantity = mypod.EntlnData.Quantity,
                        pCNumber = mypod.EntlnData.EX9Allocation.pCNumber,
                        pLineNumber = mypod.EntlnData.pDocumentItem.LineNumber.ToString()
                    });

                    BaseDataModel.Instance.ProcessItemTariff(mypod.EntlnData, cdoc.Document, itm);

                    itmsCreated += 1;

                }




                return itmsCreated;
            }
            catch (Exception Ex)
            {
                throw;
            }


        }


        private void Ex9Bucket(MyPodData mypod, double availibleQty, double totalSalesAll, double totalPiAll,
            string type)
        {
            try
            {
                var totalallocations = mypod.Allocations.Count();
                var rejects = new List<AsycudaSalesAllocations>();
                for (int i = totalallocations; i <= totalallocations; i--)
                {
                    var remainingSalesQty = mypod.Allocations.Take(i).Sum(x => x.QtyAllocated - (x.PIData.Sum(z => z.xQuantity) ?? 0));
                    
                    //if (remainingSalesQty > availibleQty && totalallocations > 1)
                    //{
                    if (i <= 0  && remainingSalesQty <= 0)//&& rejects.Any()
                    {
                        updateXStatus(rejects,
                            $@"Failed All Sales Check:: {type} Sales:{Math.Round(totalSalesAll, 2)}
                                            {type} PI: {totalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}");
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated); 
                        return;
                    }
                    var ssa = mypod.Allocations.ElementAt(i-1);
                    var piData = ssa.PIData.Sum(x => x.xQuantity) ?? 0;
                    var nr = mypod.Allocations.Take(i).Sum(x => x.QtyAllocated - (x.PIData.Sum(z => z.xQuantity) ?? 0));

                    if (nr < availibleQty && (nr + (ssa.QtyAllocated- piData)) >= availibleQty)
                    {
                        ssa.QtyAllocated = availibleQty - nr;
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated);
                        break;//put back break because its finished reducing allocations and need to exit --- gonna bug somewhere can't remember
                    }
                    if (nr >= availibleQty)
                    {
                        //ssa.QtyAllocated = availibleQty; this increased the qty because its referenced in next line below.
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated); ;
                        break;
                    }
                    else
                    {
                        mypod.Allocations.RemoveAt(i-1);
                        rejects.Add(ssa);
                        mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated);
                    }
                    //}
                    //else
                    //{

                    //}
                }

                updateXStatus( rejects,
                                            $@"Failed All Sales Check:: {type} Sales:{Math.Round(totalSalesAll, 2)}
                                            {type} PI: {totalPiAll}
                                            xQuantity:{mypod.EntlnData.Quantity}");
                mypod.EntlnData.Quantity = mypod.Allocations.Sum(x => x.QtyAllocated);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private void updateXStatus(List<AsycudaSalesAllocations> allocations, string xstatus)
        {
            using (var ctx = new AllocationDSContext(){StartTracking = true})
            {
                foreach (var a in allocations)
                {
                    var res = ctx.AsycudaSalesAllocations.First(x => x.AllocationId == a.AllocationId);
                    res.xStatus = xstatus;
                    //ctx.ApplyChanges(res);
                }

                ctx.SaveChanges();
            }
        }

        private static Dictionary<int, List<previousItems>> docPreviousItems = new Dictionary<int, List<previousItems>>();


        

        private int AddFreeText(int itmcnt, xcuda_Item itm, string entryDataId, int lineNumber, string comment,
            string itemCode)
        {
            //if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == true) return itmcnt;
            if (itm.Free_text_1 == null) itm.Free_text_1 = "";
            itm.Free_text_1 = $"{entryDataId}|{lineNumber}|{itemCode}";
            itm.PreviousInvoiceItemNumber = itemCode;
            itm.PreviousInvoiceLineNumber = lineNumber.ToString();
            itm.PreviousInvoiceNumber = entryDataId;
            if(!string.IsNullOrEmpty(comment)) itm.Free_text_2 = $"{comment}";
            itmcnt += 1;

            BaseDataModel.LimitFreeText(itm);
            return itmcnt;
        }

        


        private  async Task Ex9Bucket(MyPodData mypod, string dfp,  double docPi, List<ItemSalesPiSummary> itemSalesPiSummaryLst)
        {
            // prevent over draw down of pqty == quantity allocated
            try
            {
                var salesFactor = Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                    ? 1
                    : mypod.EntlnData.EX9Allocation.SalesFactor;
                var entryLine = mypod.EntlnData;
                var asycudaLine = mypod.EntlnData.pDocumentItem;
                var allocations = mypod.Allocations;
                
                if (asycudaLine == null) throw new ApplicationException("Allocation Previous Document is Null");
                //var itemAllocated = (dfp == "Duty Free" ? asycudaLine.DFQtyAllocated : asycudaLine.DPQtyAllocated);
                //var allocationsAllocated = allocations.Sum(x => x.QtyAllocated);
               // var totalSalesQtyAllocatedHistoric = salesSummary.Select(x => x.TotalQtyAllocated).DefaultIfEmpty(0).Sum() / salesFactor; // down to run levels
               var allocationSales = itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum() >
                                     entryLine.EntryDataDetails.Sum(x => x.QtyAllocated)
                   ? itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum() 
                   : entryLine.EntryDataDetails.Sum(x => x.QtyAllocated);/// Can't use just sales data because of partial exwarehouse the pi quantity will over ride it better to use total quantity '322035' columbian emeralds // itemSalesPiSummaryLst.First().Type == "Historic" ? itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum() :  entryLine.EntryDataDetails.Sum(x => x.QtyAllocated); //itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum();//switch to QtyAllocated - 'CLC/124075' -- rouge 'WAM03600'

                var allocationPi = itemSalesPiSummaryLst.GroupBy(x => x.PreviousItem_Id).Select(x => x.First().PiQuantity).DefaultIfEmpty(0).Sum();
            
                var asycudaTotalQuantity = asycudaLine.ItemQuantity;//PdfpAllocated;//

                var alreadyTakenOutItemsLst = asycudaLine.previousItems.DistinctBy(x => x.PreviousItem_Id).ToList();
                if(docPreviousItems.ContainsKey(asycudaLine.xcuda_ItemId)) alreadyTakenOutItemsLst.AddRange(docPreviousItems[asycudaLine.xcuda_ItemId].Where(x => x.DutyFreePaid == dfp).ToList());
                
         
                var alreadyTakenOutTotalQuantity = alreadyTakenOutItemsLst.Sum(xx => xx.Suplementary_Quantity);

                var alreadyTakenOutDFPQuantity = alreadyTakenOutItemsLst.Where(x => x.DutyFreePaid == dfp).Sum(xx => xx.Suplementary_Quantity);

                var remainingQtyToBeTakenOut = Math.Round((double) (allocationSales - (allocationPi )) , 3);//+ docPi taking out docpi here because sales is not total sales 

                if ((Math.Abs(asycudaTotalQuantity - alreadyTakenOutTotalQuantity) < 0.01) 
                    //|| (Math.Abs(dutyFreePaidAllocated - alreadyTakenOutDFPQty) < 0.01)  //////////////Allow historical adjustment
                    || (Math.Abs(remainingQtyToBeTakenOut) <= 0)
                    || allocationSales < allocationPi)
                {
                    updateXStatus(mypod.Allocations,
                        $@"Failed Ex9 Bucket :: PI Quantity: {allocationPi}");
                    allocations.Clear();
                    entryLine.EntryDataDetails.Clear();
                    entryLine.Quantity = 0;
                    return;
                }
                //if (dutyFreePaidAllocated - (alreadyTakenOutDFPQty + entryLine.Quantity) < 0)
                //{
                
                   
                    
                    if (remainingQtyToBeTakenOut + alreadyTakenOutTotalQuantity + docPi>= asycudaTotalQuantity) remainingQtyToBeTakenOut = asycudaTotalQuantity - alreadyTakenOutTotalQuantity - docPi;
                    var salesLst = entryLine.EntryDataDetails.OrderBy(x => x.EntryDataDate).ThenBy(x => x.EntryDataDetailsId).ToList();

                    var totalAllocatedQty = allocations.Sum(x => x.QtyAllocated) / salesFactor;
                   
                        if (remainingQtyToBeTakenOut == totalAllocatedQty) return;
                        if (entryLine.Quantity <= 0.001) return;
                        if (Math.Abs(remainingQtyToBeTakenOut) < 0.001) return;
                        if (salesLst.Any() == false) return;


                        var startAllocationItemIndex = 0;
                        var currentSalesItemIndex = 0;
                        var saleItm = salesLst.ElementAt(currentSalesItemIndex);



                        for (var s = currentSalesItemIndex; s < salesLst.Count(); s++)
                        {
                            var currentAllocationItemIndex = startAllocationItemIndex;
                            if (currentSalesItemIndex != s)
                            {
                                currentSalesItemIndex = s;
                                saleItm = salesLst.ElementAt(currentSalesItemIndex);
                            }



                            if (saleItm == null) break;
                            var saleAllocationsLst = allocations
                                .Where(x => x.EntryDataDetailsId == saleItm.EntryDataDetailsId)
                                .OrderBy(x => x.AllocationId).ToList();

                            if (!saleAllocationsLst.Any()) break;

                            var allocation = saleAllocationsLst.ElementAt(currentAllocationItemIndex);

                            for (var i = currentAllocationItemIndex; i < saleAllocationsLst.Count(); i++)
                            {

                                if (currentAllocationItemIndex != i ||
                                    saleAllocationsLst.ElementAt(currentAllocationItemIndex).AllocationId !=
                                    allocation.AllocationId)
                                {
                                    if (i < 0) i = 0;
                                    currentAllocationItemIndex = i;
                                    allocation = saleAllocationsLst.ElementAt(currentAllocationItemIndex);

                                }

                                var piData = allocation.PIData.Sum(x => x.xQuantity) ?? 0;
                                //var takeOut = piData;//CalculateTakeOut(totalAllocatedQty, remainingQtyToBeTakenOut, allocation.QtyAllocated, piData,  salesFactor);

                                totalAllocatedQty -= piData;
                                entryLine.Quantity -= piData;
                                allocation.QtyAllocated -= piData * salesFactor;
                                saleItm.QtyAllocated -= piData * salesFactor;


                                if (Math.Abs(allocation.QtyAllocated) < 0.001) //&& saleAllocationsLst.Count > 1
                                {
                                    updateXStatus(new List<AsycudaSalesAllocations>() {allocation},
                                        $@"Failed Ex9 Bucket");
                                    allocations.Remove(allocation);

                                    if (totalAllocatedQty < 0) continue;
                                }
                                else
                                {
                                    if (piData > 0)
                                    {


                                        var sql = "";

                                        // Create New allocation
                                        sql +=
                                            $@"Insert into AsycudaSalesAllocations(QtyAllocated, EntryDataDetailsId, PreviousItem_Id, EANumber, SANumber)
                                           Values ({piData * salesFactor},{allocation.EntryDataDetailsId},{
                                               allocation.PreviousItem_Id
                                           },{allocation.EANumber + 1},{allocation.SANumber + 1})";
                                        // update existing allocation
                                        sql += $@" UPDATE       AsycudaSalesAllocations
                                                            SET                QtyAllocated =  QtyAllocated{(piData >= 0 ? $"-{piData * salesFactor}" : $"+{piData * -1 * salesFactor}")}
                                                            where	AllocationId = {allocation.AllocationId}";
                                        using (var ctx = new AllocationDSContext())
                                        {
                                            ctx.Database.CommandTimeout = 0;
                                            ctx.Database
                                                .ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
                                        }
                                    }
                                }


                            }

                            if (Math.Abs(saleItm.QtyAllocated) < 0.001)
                            {
                                entryLine.EntryDataDetails.Remove(saleItm);
                                // salesLst.RemoveAt(0);
                            }


                        }


                        // entryLine.Quantity = remainingQtyToBeTakenOut;
                   
                //}
                //if (entryLine.Quantity + alreadyTakenOutTotalQuantity > asycudaTotalQuantity) entryLine.Quantity = asycudaTotalQuantity - alreadyTakenOutTotalQuantity;


            }
            catch (Exception ex)
            {
                throw;
            }

        }


        private global::DocumentItemDS.Business.Entities.xcuda_PreviousItem CreatePreviousItem(AlloEntryLineData pod, int itmcount, string dfp)
        {

            try
            {
                var previousItem = pod.pDocumentItem;

                var pitm = new global::DocumentItemDS.Business.Entities.xcuda_PreviousItem(true) { TrackingState = TrackingState.Added };
                if (previousItem == null) return pitm;

                

                pitm.Hs_code = pod.EX9Allocation.pTariffCode;
                pitm.Commodity_code = pod.EX9Allocation.pPrecision1;
                pitm.Current_item_number = (itmcount + 1).ToString(); // piggy back the previous item count
                pitm.Previous_item_number = previousItem.LineNumber.ToString();


                SetWeights(pod, pitm, dfp);


                pitm.Previous_Packages_number = "0";


                pitm.Suplementary_Quantity = Math.Round(Convert.ToDecimal(pod.Quantity), 2);
                pitm.Preveious_suplementary_quantity = Convert.ToDouble(pod.EX9Allocation.pQuantity);


                pitm.Goods_origin = pod.EX9Allocation.Country_of_origin_code;
                double pval = pod.EX9Allocation.Total_CIF_itm;//previousItem.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency;
                pitm.Previous_value = Convert.ToDouble((pval / pod.EX9Allocation.pQuantity));
                pitm.Current_value = Convert.ToDouble((pval) / Convert.ToDouble(pod.EX9Allocation.pQuantity));
                pitm.Prev_reg_ser = "C";
                pitm.Prev_reg_nbr = pod.EX9Allocation.pCNumber;
                pitm.Prev_reg_year = pod.EX9Allocation.pRegistrationDate.Year;
                pitm.Prev_reg_cuo = pod.EX9Allocation.Customs_clearance_office_code;
                pitm.Prev_decl_HS_spec = pod.pDocumentItem.ItemNumber;

                return pitm;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetWeights(AlloEntryLineData pod, global::DocumentItemDS.Business.Entities.xcuda_PreviousItem pitm, string dfp)
        {
            try
            {
                var previousItem = pod.pDocumentItem;
                if (previousItem == null) return;
                var plst = previousItem.previousItems.DistinctBy(x => x.PreviousItem_Id);
                var pw = Convert.ToDecimal(Math.Round(pod.EX9Allocation.Net_weight_itm,2));

                var rw =(decimal) plst.ToList().Sum(x => Math.Round(x.Net_weight,2));

                var ra = plst.Sum(x => x.Suplementary_Quantity);

                var iw = ((pw - (decimal) rw) / (decimal) (pod.EX9Allocation.pQuantity - ra)) * Convert.ToDecimal(pod.Quantity);

                //var iw = Convert.ToDouble((Math.Round(pod.EX9Allocation.Net_weight_itm,2)
                //                           / pod.EX9Allocation.pQuantity) * Convert.ToDouble(pod.Quantity));




                if ((pod.EX9Allocation.pQuantity - (plst.Sum(x => x.Suplementary_Quantity) + pod.Quantity))  <= 0 && pod.EX9Allocation.pQuantity > 1)
                {

                    pitm.Net_weight = Math.Round(Convert.ToDecimal(pw - rw), 2, MidpointRounding.ToEven);
                }
                else
                {
                    pitm.Net_weight = Convert.ToDecimal(Math.Round(iw, 2, MidpointRounding.ToEven));
                }

                pitm.Prev_net_weight = pw; //Convert.ToDouble((pw/pod.EX9Allocation.SalesFactor) - rw);
                if (pitm.Net_weight > pitm.Prev_net_weight) pitm.Net_weight = pitm.Prev_net_weight;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Ex9InitializeCdoc(string dfp, DocumentCT cdoc, AsycudaDocumentSet ads, string DocumentType,string prefix = null)
        {
            try
            {

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
                BaseDataModel.Instance.IntCdoc(cdoc, ads, prefix);
                var customsProcedure = BaseDataModel.GetCustomsProcedure(dfp, DocumentType);

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = $"{DocumentType } {dfp} Entries";
                
            

                BaseDataModel.Instance.AttachCustomProcedure(cdoc, customsProcedure);
                AllocationsModel.Instance.AddDutyFreePaidtoRef(cdoc, dfp, ads);
                using (var ctx = new DocumentDSContext())
                {


                    ExportTemplate Exp = ctx.ExportTemplates
                            .Where(x => x.ApplicationSettingsId ==
                                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                            .ApplicationSettingsId)
                            .First(x =>
                                x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsProcedure);
                    cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Destination.Destination_country_code =
                        Exp.Destination_country_code;
                    cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_region =
                        Exp.Trading_country;
                    //if(string.IsNullOrEmpty(ads.Currency_Code)) --- only use template
                        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = Exp.Gs_Invoice_Currency_code;
                    if (string.IsNullOrEmpty(ads.Country_of_origin_code))
                    {
                        cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = Exp.Trading_country;
                            cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination = Exp.Country_first_destination;

                        cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code = Exp.Export_country_code;
                    }

                    cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_code = Exp.Exporter_code;
                    cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name = Exp.Exporter_name;
                    cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_name = Exp.Consignee_name;
                    cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_code = Exp.Consignee_code;

                    //cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = Convert.ToSingle(ads.Exchange_Rate);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

      


        public class AlloEntryLineData: BaseDataModel.IEntryLineData //: AllocationsModel.AlloEntryLineData
        {
            public double Cost { get; set; }
            public List<EntryDataDetailSummary> EntryDataDetails { get; set; }
            public double Weight { get; set; }
            public double InternalFreight { get; set; }
            public double Freight { get; set; }
            public List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }
            public string ItemDescription { get; set; }
            public string ItemNumber { get; set; }
            public int PreviousDocumentItemId { get; set; }
            public double Quantity { get; set; }
            public string TariffCode { get; set; }
            public pDocumentItem pDocumentItem { get; set; }
            public EX9Allocation EX9Allocation { get; set; }
            public int? FileTypeId { get; set; }
            public string EmailId { get; set; }
        }

        public class pDocumentItem
        {
            public int LineNumber { get; set; }
            public double DPQtyAllocated { get; set; }
            public List<previousItems> previousItems { get; set; }
            public double DFQtyAllocated { get; set; }

            public double QtyAllocated => DFQtyAllocated + DPQtyAllocated;
            public DateTime AssessmentDate { get; set; }
            public double ItemQuantity { get; set; }
            public string ItemNumber { get; set; }
            public int xcuda_ItemId { get; set; }
            public string Description { get; set; }
            public DateTime ExpiryDate { get; set; }
        }

        public class EX9Allocation
        {
            public string pPrecision1;
            public string pTariffCode { get; set; }
            public double pQuantity { get; set; }
            public string Country_of_origin_code { get; set; }
            public double Total_CIF_itm { get; set; }
            public string pCNumber { get; set; }
            public DateTime pRegistrationDate { get; set; }
            public string Customs_clearance_office_code { get; set; }
            public double Net_weight_itm { get; set; }
            public double pQtyAllocated { get; set; }
            public double SalesFactor { get; set; }
        }

        public class SalesSummary
        {
            public string ItemNumber { get; set; }
            public string DutyFreePaid { get; set; }
            public double TotalQuantity { get; set; }
            public double TotalQtyAllocated { get; set; }
            public string Type { get; set; }
        }
    }

    internal class SummaryData
    {
        public ItemSalesAsycudaPiSummary Summary { get; set; }
        public IEnumerable<AsycudaItemPiQuantityData> pIData { get; set; }
    }

    internal class x7100SalesPi
    {
        public string ItemNumber { get; set; }
        public double ItemQuantity { get; set; }
        public double PiQuantity { get; set; }
        public double DPQtyAllocated { get; set; }
        public double DPPi { get; set; }
        public double DFQtyAllocated { get; set; }
        public double DFPi { get; set; }
    }

    public class PiSummary
    {
        public string ItemNumber { get; set; }
        public string DutyFreePaid { get; set; }
        public double TotalQuantity { get; set; }
        public string Type
        { get; set; }

        public string pCNumber { get; set; }
        public string pLineNumber { get; set; }
        public string pOffice { get; set; }
        public double pItemQuantity { get; set; }
        public DateTime pAssessmentDate { get; set; }
        public int PreviousItem_Id { get; set; }
    }

    public class previousItems
    {
        public string DutyFreePaid { get; set; }
        public double Suplementary_Quantity { get; set; }
        public double Net_weight { get; set; }
        public int PreviousItem_Id { get; set; }
    }


    public class EX9Allocations
    {
        public string pItemDescription;
        public string pTariffCode { get; set; }
        public string DutyFreePaid { get; set; }
        public string pCNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int? PreviousItem_Id { get; set; }
        public double SalesQuantity { get; set; }
        public int AllocationId { get; set; }
        public int? EntryDataDetailsId { get; set; }
        public string Status { get; set; }
        public string InvoiceNo { get; set; }
        public string ItemNumber { get; set; }
        public string pItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public double? pItemCost { get; set; }
        public double QtyAllocated { get; set; }
        public double SalesQtyAllocated { get; set; }
        public double DFQtyAllocated { get; set; }
        public double DPQtyAllocated { get; set; }
        public int? LineNumber { get; set; }
        public List<previousItems> previousItems { get; set; }
        public string Country_of_origin_code { get; set; }
        public string Customs_clearance_office_code { get; set; }
        public double? pQuantity { get; set; }
        public DateTime pRegistrationDate { get; set; }
        public double Net_weight_itm { get; set; }
        public double Total_CIF_itm { get; set; }
        public double Freight { get; set; }
        public double InternalFreight { get; set; }
        public double Weight { get; set; }
        public List<TariffSupUnitLkps> TariffSupUnitLkps { get; set; }
        public double SalesFactor { get; set; }
        public DateTime pAssessmentDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int pLineNumber { get; set; }
        public string Currency { get; set; }
        public int? FileTypeId { get; set; }
        public string EmailId { get; set; }
        public int EntryData_Id { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }
        public int InventoryItemId { get; set; }
        public string pPrecision1 { get; set; }
        public List<AsycudaSalesAllocationsPIData> PIData { get; set; }
        public DateTime pExpiryDate { get; set; }
        public EntryDataDetails EntryDataDetails { get; set; }
    }

    public class AllocationDataBlock
    {
        public string MonthYear { get; set; }
        public string DutyFreePaid { get; set; }
        public List<EX9Allocations> Allocations { get; set; }
        public string CNumber { get; set; }
        public string Type { get; set; }
    }

    public class MyPodData
    {
        public List<AsycudaSalesAllocations> Allocations { get; set; }
        public CreateEx9Class.AlloEntryLineData EntlnData { get; set; }
        public List<string> AllNames { get; set; }
    }

    public class AlloEntryLineData
    {
    }
}