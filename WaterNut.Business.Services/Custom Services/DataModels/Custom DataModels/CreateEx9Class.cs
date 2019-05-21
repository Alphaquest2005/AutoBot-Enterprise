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
using System.Text.RegularExpressions;
using DocumentItemDS.Business.Entities;
using MoreLinq;
using PreviousDocumentQS.Business.Entities;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;

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

        public bool ApplyCurrentChecks { get; set; }


        public bool PerIM7 { get; set; }


        public bool Process7100 { get; set; }


        public async Task CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks,
            AsycudaDocumentSet docSet)
        {

            try
            {
                PerIM7 = perIM7;
                Process7100 = process7100;
                ApplyCurrentChecks = applyCurrentChecks;

                await ProcessEx9(docSet, filterExpression).ConfigureAwait(false);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static List<PiSummary> docSetPi = new List<PiSummary>();
        private async Task ProcessEx9(AsycudaDocumentSet docSet, string filterExp)
        {
           
            docSetPi = new List<PiSummary>();
            //var dutylst = CreateDutyList(slst);
            var dutylst = new List<string>(){"Duty Paid", "Duty Free"};
            if (!filterExp.Contains("InvoiceDate"))
            {
                throw new ApplicationException("Filter string dose not contain 'Invoice Date'");
                
            }
            var realStartDate = DateTime.Parse(filterExp.Substring(filterExp.IndexOf("InvoiceDate >= ")+ "InvoiceDate >= ".Length + 1, 10), CultureInfo.CurrentCulture);
            var realEndDate = DateTime.Parse(filterExp.Substring(filterExp.IndexOf("InvoiceDate <= ") + "InvoiceDate <= ".Length + 1, 19), CultureInfo.CurrentCulture);
            DateTime startDate = realStartDate;

            while (startDate <= realEndDate)
            {

                DateTime firstOfNextMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);
                DateTime endDate = firstOfNextMonth.AddDays(-1).AddHours(23);

                var currentFilter = filterExp.Replace($@"InvoiceDate >= ""{realStartDate:MM/dd/yyyy}""",
                        $@"InvoiceDate >= ""{startDate:MM/dd/yyyy}""")
                    .Replace($@"InvoiceDate <= ""{realEndDate:MM/dd/yyyy HH:mm:ss}""",
                        $@"InvoiceDate <= ""{endDate:MM/dd/yyyy HH:mm:ss}""");

              //  var salesSummary = GetSalesSummary(startDate, endDate);
              //  var piSummary = GetPiSummary(startDate, endDate);
               
             


                foreach (var dfp in dutylst)
                {
                    var exPro = " && (PreviousDocumentItem.AsycudaDocument.Extended_customs_procedure == \"7000\" || PreviousDocumentItem.AsycudaDocument.Extended_customs_procedure == \"7400\")";
                    var slst =
                        (await CreateAllocationDataBlocks(currentFilter + exPro).ConfigureAwait(false)).Where(
                            x => x.Allocations.Count > 0);
                    if (slst != null && slst.ToList().Any())
                    {
                        var res = slst.Where(x => x.DutyFreePaid == dfp);
                        List<ItemSalesPiSummary> itemSalesPiSummarylst;
                        itemSalesPiSummarylst = GetItemSalesPiSummary(docSet.ApplicationSettingsId, startDate, endDate, res.SelectMany(x => x.Allocations).Select(z => z.AllocationId).ToList());
                        await CreateDutyFreePaidDocument(dfp, res, docSet, "7400", true, itemSalesPiSummarylst.Where(x => x.DutyFreePaid == dfp).ToList(), true, true, dfp == "Duty Free"?"EX9":"IM4", true, true, ApplyCurrentChecks, true)
                            .ConfigureAwait(false);
                    }
                    if (Process7100)
                    {
                        //exPro = " && PreviousDocumentItem.AsycudaDocument.Extended_customs_procedure == \"7500\"";
                        //slst =
                        //    (await CreateAllocationDataBlocks(currentFilter + exPro).ConfigureAwait(false)).Where(
                        //        x => x.Allocations.Count > 0);
                        //if (slst != null && slst.ToList().Any())
                        //{
                        //    if (Process7100) itemSalesPiSummarylst.AddRange(Get7100SalesPiSummaryLst(startDate,endDate));
                        //    await CreateDutyFreePaidDocument(dfp, slst.Where(x => x.DutyFreePaid == dfp), docSet,
                        //            "7100", true, itemSalesPiSummarylst.Where(x => x.DutyFreePaid == dfp).ToList(), true, true, dfp == "Duty Free" ? "EX9" : "IM4", true, true, true, true)
                        //        .ConfigureAwait(false);
                        //}
                    }

                }
                startDate = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);
            }

            StatusModel.StopStatusUpdate();
        }

        private List<ItemSalesPiSummary> Get7100SalesPiSummaryLst(DateTime startDate, DateTime endDate)
        {
            var res =  Historic7100ItemSalesPi(endDate);
            res.AddRange(Current7100ItemSalesPi(startDate,endDate));
            return res;
        }

        private static List<ItemSalesPiSummary> Historic7100ItemSalesPi(DateTime endDate)
        {
            using (var ctx = new AllocationDSContext())
            {
                var res = ctx.Database.SqlQuery<x7100SalesPi>(
                    $@"SELECT        AllocationsItemNameMapping.ItemNumber, SIM.ItemQuantity, ISNULL(SEX.PiQuantity, 0) AS PiQuantity, SIM.DPQtyAllocated, ISNULL(SEX.DPPi, 0) AS DPPi, SIM.DFQtyAllocated, ISNULL(SEX.DFPi, 0) AS DFPi
FROM            (SELECT        ItemNumber, SUM(ItemQuantity) AS ItemQuantity, SUM(DPQtyAllocated) AS DPQtyAllocated, SUM(DFQtyAllocated) AS DFQtyAllocated
                          FROM            (SELECT        AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.DPQtyAllocated, AsycudaDocumentItem.DFQtyAllocated, AsycudaDocumentItem.ItemQuantity, AsycudaDocument.AssessmentDate, 
                                                                              AsycudaDocument.ExpiryDate, AsycudaDocument.CNumber, AsycudaDocumentItem.LineNumber
                                                    FROM            AsycudaDocumentBasicInfo AS AsycudaDocument INNER JOIN
                                                                              AsycudaItemBasicInfo AS AsycudaDocumentItem ON AsycudaDocument.ASYCUDA_Id = AsycudaDocumentItem.ASYCUDA_Id
                                                    WHERE        (AsycudaDocument.Extended_customs_procedure = '7100')
                                                    GROUP BY AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.DPQtyAllocated, AsycudaDocumentItem.DFQtyAllocated, AsycudaDocumentItem.ItemQuantity, AsycudaDocument.AssessmentDate, 
                                                                              AsycudaDocument.ExpiryDate, AsycudaDocument.CNumber, AsycudaDocumentItem.LineNumber) AS IM
                          WHERE        (AssessmentDate <= CONVERT(DATETIME, '{endDate.Date.ToShortDateString()}', 102)) AND (ExpiryDate >= CONVERT(DATETIME, '{endDate.Date.ToShortDateString()}', 102))
                          GROUP BY ItemNumber) AS SIM INNER JOIN
                         AllocationsItemNameMapping ON SIM.ItemNumber = AllocationsItemNameMapping.Precision_4 LEFT OUTER JOIN
                             (SELECT        ItemNumber, SUM(ISNULL(ItemQuantity, 0)) AS PiQuantity, ISNULL(SUM(DPPiQuantity), 0) AS DPPi, ISNULL(SUM(DFPiQuantity), 0) AS DFPi
                               FROM            (SELECT        AsycudaDocumentItem_1.ItemNumber, AsycudaDocumentItem_1.ItemQuantity, CAST(CASE WHEN Extended_customs_procedure = '4071' THEN itemquantity ELSE 0 END AS float) AS DPPiQuantity, 
                                                                                   CAST(CASE WHEN Extended_customs_procedure = '9071' THEN itemquantity ELSE 0 END AS float) AS DFPiQuantity, AsycudaDocument_1.AssessmentDate, AsycudaDocument_1.ExpiryDate, 
                                                                                   AsycudaDocument_1.CNumber
                                                         FROM            AsycudaItemBasicInfo AS AsycudaDocumentItem_1 INNER JOIN
                                                                                   AsycudaDocumentBasicInfo AS AsycudaDocument_1 ON AsycudaDocumentItem_1.ASYCUDA_Id = AsycudaDocument_1.ASYCUDA_Id
                                                         GROUP BY AsycudaDocumentItem_1.ItemNumber, AsycudaDocumentItem_1.ItemQuantity, AsycudaDocument_1.AssessmentDate, AsycudaDocument_1.ExpiryDate, AsycudaDocument_1.CNumber, 
                                                                                   AsycudaDocument_1.Extended_customs_procedure
                                                         HAVING         (AsycudaDocument_1.Extended_customs_procedure = '9071' OR
                                                                                   AsycudaDocument_1.Extended_customs_procedure = '4071')) AS Ex
                               WHERE        (AssessmentDate <= CONVERT(DATETIME, '{endDate.Date.ToShortDateString()}', 102))
                               GROUP BY ItemNumber) AS SEX ON SIM.ItemNumber = SEX.ItemNumber
GROUP BY AllocationsItemNameMapping.ItemNumber, SIM.ItemQuantity, ISNULL(SEX.PiQuantity, 0), SIM.DPQtyAllocated, ISNULL(SEX.DPPi, 0), SIM.DFQtyAllocated, ISNULL(SEX.DFPi, 0)").ToList();

                var lst = res.Where(x => x.DPQtyAllocated > 0).Select(x => new ItemSalesPiSummary()
                {
                    DutyFreePaid = "Duty Paid",
                    ItemNumber = x.ItemNumber,
                    PiQuantity = x.DPPi,
                    QtyAllocated = x.DPQtyAllocated,
                    Type = "Historic"
                }).ToList();

                lst.AddRange(res.Where(x => x.DFQtyAllocated > 0).Select(x => new ItemSalesPiSummary()
                {
                    DutyFreePaid = "Duty Free",
                    ItemNumber = x.ItemNumber,
                    PiQuantity = x.DFPi,
                    QtyAllocated = x.DFQtyAllocated,
                    Type = "Historic"
                }).ToList());

                return lst;
            }
        }

        private static List<ItemSalesPiSummary> Current7100ItemSalesPi(DateTime startDate, DateTime endDate)
        {
            using (var ctx = new AllocationDSContext())
            {
                var res = ctx.Database.SqlQuery<x7100SalesPi>(
                    $@"SELECT        AllocationsItemNameMapping.ItemNumber, SIM.QtySold AS ItemQuantity, ISNULL(SEX.PiQuantity, 0) AS PiQuantity, SIM.DPQtyAllocated, ISNULL(SEX.DPPi, 0) AS DPPi, SIM.DFQtyAllocated, ISNULL(SEX.DFPi, 0) AS DFPi
FROM            (SELECT        ItemNumber, SUM(SalesQuantity) AS QtySold, SUM(QtyAllocated) AS QtyAllocated, SUM(DPQtyAllocated) AS DPQtyAllocated, SUM(DPSalesQuantity) AS DPSalesQuantity, SUM(DFQtyAllocated) AS DFQtyAllocated, 
                         SUM(DFSalesQuantity) AS DFSalesQuantity
FROM            (SELECT        TOP (100) PERCENT AsycudaItemBasicInfo.ItemNumber, EntryDataDetails.Quantity AS SalesQuantity, AsycudaSalesAllocations.QtyAllocated, EntryDataDetails.EntryDataId AS Invoice, EntryData.EntryDataDate AS InvoiceDate, 
                         EntryDataDetails.EntryDataDetailsId, CASE WHEN taxamount > 0 THEN AsycudaSalesAllocations.qtyallocated ELSE 0 END AS DPQtyAllocated, CASE WHEN taxamount > 0 THEN quantity ELSE 0 END AS DPSalesQuantity, 
                         CASE WHEN taxamount = 0 THEN AsycudaSalesAllocations.qtyallocated ELSE 0 END AS DFQtyAllocated, CASE WHEN taxamount = 0 THEN quantity ELSE 0 END AS DFSalesQuantity
FROM            AsycudaSalesAllocations INNER JOIN
                         AsycudaItemBasicInfo ON AsycudaSalesAllocations.PreviousItem_Id = AsycudaItemBasicInfo.Item_Id INNER JOIN
                         AsycudaDocumentBasicInfo ON AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                         EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                         EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId INNER JOIN
                         EntryData_Sales ON EntryData.EntryDataId = EntryData_Sales.EntryDataId
WHERE        (AsycudaDocumentBasicInfo.Extended_customs_procedure = '7100')
GROUP BY AsycudaItemBasicInfo.ItemNumber, EntryDataDetails.Quantity, EntryData_Sales.TaxAmount, AsycudaSalesAllocations.QtyAllocated, EntryDataDetails.EntryDataId, EntryData.EntryDataDate, 
                         EntryDataDetails.EntryDataDetailsId
ORDER BY InvoiceDate) AS sales
WHERE        (InvoiceDate <= CONVERT(DATETIME, '{endDate.Date.ToShortDateString()}', 102)) AND (InvoiceDate >= CONVERT(DATETIME, '{startDate.Date.ToShortDateString()}', 102))
GROUP BY ItemNumber) AS SIM INNER JOIN
                         AllocationsItemNameMapping ON SIM.ItemNumber = AllocationsItemNameMapping.Precision_4 LEFT OUTER JOIN
                             (SELECT        ItemNumber, SUM(ISNULL(ItemQuantity, 0)) AS PiQuantity, ISNULL(SUM(DPPiQuantity), 0) AS DPPi, ISNULL(SUM(DFPiQuantity), 0) AS DFPi
                               FROM            (SELECT        AsycudaDocumentItem_1.ItemNumber, AsycudaDocumentItem_1.ItemQuantity, CAST(CASE WHEN Extended_customs_procedure = '4071' THEN itemquantity ELSE 0 END AS float) AS DPPiQuantity, 
                                                                                   CAST(CASE WHEN Extended_customs_procedure = '9071' THEN itemquantity ELSE 0 END AS float) AS DFPiQuantity, AsycudaDocument_1.AssessmentDate, AsycudaDocument_1.ExpiryDate, 
                                                                                   AsycudaDocument_1.CNumber
                                                         FROM            AsycudaItemBasicInfo AS AsycudaDocumentItem_1 INNER JOIN
                                                                                   AsycudaDocumentBasicInfo AS AsycudaDocument_1 ON AsycudaDocumentItem_1.ASYCUDA_Id = AsycudaDocument_1.ASYCUDA_Id
                                                         GROUP BY AsycudaDocumentItem_1.ItemNumber, AsycudaDocumentItem_1.ItemQuantity, AsycudaDocument_1.AssessmentDate, AsycudaDocument_1.ExpiryDate, AsycudaDocument_1.CNumber, 
                                                                                   AsycudaDocument_1.Extended_customs_procedure
                                                         HAVING         (AsycudaDocument_1.Extended_customs_procedure = '9071') OR
                                                                                   (AsycudaDocument_1.Extended_customs_procedure = '4071')) AS Ex
                               WHERE        (AssessmentDate <= CONVERT(DATETIME, '{endDate.Date.ToShortDateString()}', 102)) AND (AssessmentDate >= CONVERT(DATETIME, '{startDate.Date.ToShortDateString()}', 102))
                               GROUP BY ItemNumber) AS SEX ON SIM.ItemNumber = SEX.ItemNumber
GROUP BY AllocationsItemNameMapping.ItemNumber, SIM.QtySold, ISNULL(SEX.PiQuantity, 0), SIM.DPQtyAllocated, ISNULL(SEX.DPPi, 0), SIM.DFQtyAllocated, ISNULL(SEX.DFPi, 0)").ToList();

                var lst = res.Where(x => x.DPQtyAllocated > 0).Select(x => new ItemSalesPiSummary()
                {
                    DutyFreePaid = "Duty Paid",
                    ItemNumber = x.ItemNumber,
                    PiQuantity = x.DPPi,
                    QtyAllocated = x.DPQtyAllocated,
                    Type = "Current"
                }).ToList();

                lst.AddRange(res.Where(x => x.DFQtyAllocated > 0).Select(x => new ItemSalesPiSummary()
                {
                    DutyFreePaid = "Duty Free",
                    ItemNumber = x.ItemNumber,
                    PiQuantity = x.DFPi,
                    QtyAllocated = x.DFQtyAllocated,
                    Type = "Current"
                }).ToList());

                return lst;
            }
        }


        //private List<PiSummary> GetPiSummary(DateTime startDate,DateTime endDate)
        //{
            
        //    using (var ctx = new AllocationDSContext())
        //    {
              
        //        var piSummaryHistoric = ctx.xcuda_PreviousItem
        //                        .Where(x => x.xcuda_Item.AsycudaDocument.Cancelled != true
        //                                  //  && x.xcuda_Item.IsAssessed == true --- did not force assesed so that system checks potential ex-warehouse too
        //                                    && (x.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "9070" || x.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070")
        //                                    && x.xcuda_Item.AsycudaDocument.AssessmentDate != null 
        //                                    && x.xcuda_Item.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate ?? DateTime.MinValue)
        //                                    //  && x.xcuda_Item.AsycudaDocument.AssessmentDate >= startDate
        //                                    && x.xcuda_Item.AsycudaDocument.AssessmentDate.Value <= endDate)
        //            .GroupBy(x => new { ItemNumber = x.xcuda_Item.xcuda_Tarification.xcuda_HScode.Precision_4, DutyFreePaid = (x.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "9070" ? "Duty Free" : "Duty Paid"), pCNumber = x.Prev_reg_nbr, pLineNumber = x.Previous_item_number, pOffice = x.Prev_reg_cuo, pAssessmentDate = x.xcuda_Item.AsycudaDocument.AssessmentDate, pItemQuantity = x.Preveious_suplementary_quantity })
        //            .Select(g => new PiSummary
        //            {
        //                ItemNumber = g.Key.ItemNumber,
        //                pItemQuantity = g.Key.pItemQuantity,
        //                DutyFreePaid = g.Key.DutyFreePaid,
        //                TotalQuantity = g.Sum(z => z.Suplementary_Quantity),
        //                Type = "Historic",
        //                pCNumber = g.Key.pCNumber,
        //                pLineNumber = g.Key.pLineNumber,
        //                pOffice = g.Key.pOffice,
        //                pAssessmentDate = g.Key.pAssessmentDate.Value
        //            }).ToList();

        //        var piSummaryCurrent = ctx.xcuda_PreviousItem
        //            .Where(x => x.xcuda_Item.AsycudaDocument.Cancelled != true
        //                        //  && x.xcuda_Item.IsAssessed == true --- did not force assesed so that system checks potential ex-warehouse too
        //                        && (x.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "9070" || x.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070")
        //                        && x.xcuda_Item.AsycudaDocument.AssessmentDate != null
        //                        && x.xcuda_Item.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate ?? DateTime.MinValue)
        //                        && x.xcuda_Item.AsycudaDocument.AssessmentDate >= startDate
        //                        && x.xcuda_Item.AsycudaDocument.AssessmentDate.Value <= endDate)
        //            .GroupBy(x => new { ItemNumber = x.xcuda_Item.xcuda_Tarification.xcuda_HScode.Precision_4, DutyFreePaid = (x.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "9070" ? "Duty Free" : "Duty Paid"), pCNumber = x.Prev_reg_nbr, pLineNumber = x.Previous_item_number, pOffice = x.Prev_reg_cuo, pAssessmentDate = x.xcuda_Item.AsycudaDocument.AssessmentDate, pItemQuantity = x.Preveious_suplementary_quantity })
        //            .Select(g => new PiSummary
        //            {
        //                ItemNumber = g.Key.ItemNumber,
        //                pItemQuantity = g.Key.pItemQuantity,
        //                DutyFreePaid = g.Key.DutyFreePaid,
        //                TotalQuantity = g.Sum(z => z.Suplementary_Quantity),
        //                Type = "Current",
        //                pCNumber = g.Key.pCNumber,
        //                pLineNumber = g.Key.pLineNumber,
        //                pOffice = g.Key.pOffice,
        //                pAssessmentDate = g.Key.pAssessmentDate.Value
        //            }).ToList();
        //        piSummaryHistoric.AddRange(piSummaryCurrent);
        //        return piSummaryHistoric;
        //    }
        //}

        private List<ItemSalesPiSummary> GetItemSalesPiSummary(int applicationSettingsId, DateTime startDate,
            DateTime endDate,
            List<int> alst)
        {
            try
            {
                
                using (var ctx = new AllocationDSContext())
                {
                    var allallocations = ctx.AsycudaSalesAllocations.Where(x => alst.Contains(x.AllocationId)).Select(x => x.EntryDataDetails.ItemNumber).ToList();
                   
                    var resHistoric =   ctx.AsycudaSalesAllocations
                        //.Where(x => x.EntryDataDetails.ItemNumber == "EVC/100508")
                        .Where(x => x.EntryDataDetails.EntryDataDetailsEx.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => allallocations.Contains(x.EntryDataDetails.ItemNumber))//changed from Allocationid to itemnumber because i need all allocations for that item number to do current total
                        .Where(x => x.EntryDataDetails.Sales.EntryDataDate <= endDate 
                                    || (x.EntryDataDetails.Adjustments != null && x.EntryDataDetails.EffectiveDate <= endDate))
                                .Where(x => x.PreviousItem_Id != null)
                                .GroupBy(g => new
                                {
                                    PreviousDocumentItem = new
                                    {
                                        PreviousItem_Id = g.PreviousItem_Id,
                                        PiQuantity = g.PreviousDocumentItem.EntryPreviousItems
                                            .Where(z => z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate <= endDate )
                                            .Where(z => ((g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? "Duty Free" : "Duty Paid") == (z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4074" 
                                                                                                                || z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070"
                                            ? "Duty Paid"
                                            : "Duty Free"))
                                                          .Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                                        QtyAllocated = (g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? g.PreviousDocumentItem.DFQtyAllocated : g.PreviousDocumentItem.DPQtyAllocated ,
                                        
                                        pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
                                        pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
                                        pLineNumber = g.PreviousDocumentItem.LineNumber
                                    },
                                    ItemNumber = g.EntryDataDetails.ItemNumber,
                                    DutyFreePaid = ((g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? "Duty Free" : "Duty Paid")
                                    
                                })
                                                .GroupJoin(ctx.PreviousItemsEx.Where(x => x.AssessmentDate <= endDate)
                                ,a => a.Key.ItemNumber  ,p => p.ItemNumber,
                                (a,p) => new {a,p})
                        .Select(x => new ItemSalesPiSummary
                        {
                            PreviousItem_Id = (int)x.a.Key.PreviousDocumentItem.PreviousItem_Id,
                            ItemNumber = x.a.Key.ItemNumber,
                            QtyAllocated = x.a.Select(z => z.QtyAllocated).DefaultIfEmpty(0).Sum(),
                            pQtyAllocated = x.a.Key.PreviousDocumentItem.QtyAllocated,
                            PiQuantity = x.p.Where(z => z.DutyFreePaid == x.a.Key.DutyFreePaid &&
                                          z.PreviousDocumentItemId == (int)x.a.Key.PreviousDocumentItem.PreviousItem_Id)
                                .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                            pCNumber = x.a.Key.PreviousDocumentItem.pCNumber,
                            pRegistrationDate = x.a.Key.PreviousDocumentItem.pRegistrationDate,
                            pLineNumber = x.a.Key.PreviousDocumentItem.pLineNumber,
                            DutyFreePaid = x.a.Key.DutyFreePaid,
                            Type = "Historic"

                        }).ToList();
                    if (ApplyCurrentChecks)
                    {
                        var resCurrent = ctx.AsycudaSalesAllocations
                            //  .Where(x => x.EntryDataDetails.ItemNumber == "WES/404-45")
                            .Where(x => alst.Contains(x.AllocationId)) // left this as is because pQtyallocated is totaled anyways
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.ApplicationSettingsId == applicationSettingsId)
                            .Where(x => x.EntryDataDetails.Sales.EntryDataDate >= startDate && x.EntryDataDetails.Sales.EntryDataDate <= endDate || (x.EntryDataDetails.Adjustments != null && x.EntryDataDetails.EffectiveDate >= startDate && x.EntryDataDetails.EffectiveDate <= endDate))
                                .Where(x => x.PreviousItem_Id != null)
                                .GroupBy(g => new
                                {
                                    PreviousDocumentItem = new
                                    {
                                        PreviousItem_Id = g.PreviousItem_Id,
                                        PiQuantity = g.PreviousDocumentItem.EntryPreviousItems
                                                        .Where(z => z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate <= endDate)
                                                        .Where(z => ((g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? "Duty Free" : "Duty Paid") == (z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4074"
                                                                                                                                         || z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070"
                                                            ? "Duty Paid"
                                                            : "Duty Free"))
                                                        .Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                                        pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
                                        pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
                                        pLineNumber = g.PreviousDocumentItem.LineNumber
                                    },
                                    ItemNumber = g.EntryDataDetails.ItemNumber,
                                    DutyFreePaid = ((g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? "Duty Free" : "Duty Paid")
                                })
                                                .GroupJoin(ctx.PreviousItemsEx.Where(x => x.AssessmentDate >= startDate && x.AssessmentDate <= endDate)
                                , a => a.Key.ItemNumber, p => p.ItemNumber,
                                (a, p) => new { a, p })
                        .Select(x => new ItemSalesPiSummary
                        {
                            PreviousItem_Id = (int)x.a.Key.PreviousDocumentItem.PreviousItem_Id,
                            ItemNumber = x.a.Key.ItemNumber,
                            QtyAllocated = x.a.Select(z => z.QtyAllocated).DefaultIfEmpty(0).Sum(),
                            PiQuantity = x.p.Where(z => z.DutyFreePaid == x.a.Key.DutyFreePaid 
                                                        && z.PreviousDocumentItemId == (int)x.a.Key.PreviousDocumentItem.PreviousItem_Id)
                                            .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                            pCNumber = x.a.Key.PreviousDocumentItem.pCNumber,
                            pRegistrationDate = x.a.Key.PreviousDocumentItem.pRegistrationDate,
                            pLineNumber = x.a.Key.PreviousDocumentItem.pLineNumber,
                            DutyFreePaid = x.a.Key.DutyFreePaid,
                            Type = "Current"

                        }).ToList();
                        resHistoric.AddRange(resCurrent);
                    }

                    return resHistoric;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            



            //using (var ctx = new AllocationDSContext())
            //{
            //    var resHistoric = ctx.AsycudaSalesAllocations.Where(x => x.EntryDataDetails.Sales.EntryDataDate <= endDate)
            //        .Where(x => x.PreviousItem_Id != null)
            //        .GroupBy(g => new {PreviousDocumentItem = new
            //        {
            //            PreviousItem_Id = g.PreviousItem_Id,
            //            PiQuantity = g.PreviousDocumentItem.EntryPreviousItems.Where(z => z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate <=  endDate).Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
            //            pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
            //            pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
            //            pLineNumber = g.PreviousDocumentItem.LineNumber
            //        }, ItemNumber = g.EntryDataDetails.ItemNumber,
            //            DutyFreePaid = ((g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? "Duty Free" : "Duty Paid")
            //        }).ToList();

            //     var res1 = resHistoric.Select(x => new ItemSalesPiSummary
            //        {
            //         PreviousItem_Id = (int)x.Key.PreviousDocumentItem.PreviousItem_Id,
            //         ItemNumber = x.Key.ItemNumber,
            //            QtyAllocated = x.Select(z => z.QtyAllocated).DefaultIfEmpty(0).Sum(),
            //            PiQuantity = x.Key.PreviousDocumentItem.PiQuantity,
            //            pCNumber = x.Key.PreviousDocumentItem.pCNumber,
            //            pRegistrationDate = x.Key.PreviousDocumentItem.pRegistrationDate,
            //            pLineNumber = x.Key.PreviousDocumentItem.pLineNumber,
            //            DutyFreePaid = x.Key.DutyFreePaid,
            //            Type = "Historic"

            //     }).ToList();
            //    if (ApplyCurrentChecks)
            //    {
            //        var resCurrent = ctx.AsycudaSalesAllocations.Where(x =>
            //                x.EntryDataDetails.Sales.EntryDataDate >= startDate &&
            //                x.EntryDataDetails.Sales.EntryDataDate <= endDate)
            //            .Where(x => x.PreviousItem_Id != null)
            //            .GroupBy(g => new
            //            {
            //                PreviousDocumentItem = new
            //                {
            //                    PreviousItem_Id = g.PreviousItem_Id,
            //                    PiQuantity = g.PreviousDocumentItem.EntryPreviousItems
            //                        .Where(z =>
            //                            z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate >= startDate &&
            //                            z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate <= endDate)
            //                        .Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
            //                    pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
            //                    pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
            //                    pLineNumber = g.PreviousDocumentItem.LineNumber
            //                },
            //                ItemNumber = g.EntryDataDetails.ItemNumber,
            //                DutyFreePaid = ((g.EntryDataDetails.Sales.TaxAmount ?? 0) == 0 ? "Duty Free" : "Duty Paid")
            //            }).ToList();

            //        var res2 = resCurrent.Select(x => new ItemSalesPiSummary
            //        {
            //            PreviousItem_Id = (int) x.Key.PreviousDocumentItem.PreviousItem_Id,
            //            ItemNumber = x.Key.ItemNumber,
            //            QtyAllocated = x.Select(z => z.QtyAllocated).DefaultIfEmpty(0).Sum(),
            //            PiQuantity = x.Key.PreviousDocumentItem.PiQuantity,
            //            pCNumber = x.Key.PreviousDocumentItem.pCNumber,
            //            pRegistrationDate = x.Key.PreviousDocumentItem.pRegistrationDate,
            //            pLineNumber = x.Key.PreviousDocumentItem.pLineNumber,
            //            DutyFreePaid = x.Key.DutyFreePaid,
            //            Type = "Current"

            //        }).ToList();



            //        res1.AddRange(res2);
            //    }
            //    return res1;
            //}
        }

        public class ItemSalesPiSummary
        {
            public string ItemNumber { get; set; }
            public double QtyAllocated { get; set; }
            public double PiQuantity { get; set; }
            public string pCNumber { get; set; }
            public int pLineNumber { get; set; }
            public DateTime? pRegistrationDate { get; set; }
            public string DutyFreePaid { get; set; }
            public string Type { get; set; }
            public int PreviousItem_Id { get; set; }
            public double pQtyAllocated { get; set; }
        }

        private List<SalesSummary> GetSalesSummary(DateTime startDate, DateTime endDate)
        {
           
            using (var ctx = new AllocationDSContext())
            {
                var salesLstHistoric = ctx.EntryDataDetails.Where(x =>  x.Sales.EntryDataDate <= endDate)//x.Sales.EntryDataDate >= startDate &&
                               .GroupBy(x => new { x.ItemNumber, DutyFreePaid =  (x.Sales.TaxAmount == 0?"Duty Free":"Duty Paid") })
                               .Select(g => new SalesSummary
                               {
                                   ItemNumber = g.Key.ItemNumber,
                                   DutyFreePaid = g.Key.DutyFreePaid,
                                   TotalQuantity = g.Sum(z => z.Quantity),
                                   TotalQtyAllocated = g.Sum(z => z.QtyAllocated),
                                   Type = "Historic"
                               }).ToList();

                var salesLstCurrent = ctx.EntryDataDetails.Where(x => x.Sales.EntryDataDate >= startDate && x.Sales.EntryDataDate <= endDate)//
                    .GroupBy(x => new { x.ItemNumber, DutyFreePaid = (x.Sales.TaxAmount == 0 ? "Duty Free" : "Duty Paid") })
                    .Select(g => new SalesSummary
                    {
                        ItemNumber = g.Key.ItemNumber,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        TotalQuantity = g.Sum(z => z.Quantity),
                        TotalQtyAllocated = g.Sum(z => z.QtyAllocated),
                        Type = "Current"
                    }).ToList();
                salesLstHistoric.AddRange(salesLstCurrent);
                return salesLstHistoric;
            }
        }

        public async Task CreateDutyFreePaidDocument(string dfp, IEnumerable<AllocationDataBlock> slst,
            AsycudaDocumentSet docSet, string im7Type, bool isGrouped, List<ItemSalesPiSummary> itemSalesPiSummaryLst,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, string ex9Type,
            bool applyEx9Bucket, bool applyHistoricChecks, bool applyCurrentChecks, bool autoAssess)
        {
            try
            {
                docSetPi.Clear();

                var itmcount = 0;


                Document_Type dt;
                dt = await GetDocumentType(dfp).ConfigureAwait(false);
                if (dt == null)
                {
                    throw new ApplicationException(
                        $"Null Document Type for '{dfp}' Contact your Network Administrator");
                }
                
                if(checkForMultipleMonths)
                if (slst.ToList().SelectMany(x => x.Allocations).Select(x => x.InvoiceDate.Month).Distinct().Count() > 1)
                {
                    throw new ApplicationException(
                        string.Format("Data Contains Multiple Months", dfp));
                }

                

                StatusModel.StatusUpdate($"Creating xBond Entries - {dfp}");

                var cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);

                foreach (var monthyear in slst) //.Where(x => x.DutyFreePaid == dfp)
                {

                    var prevEntryId = "";
                    var prevIM7 = "";
                    var elst = PrepareAllocationsData(monthyear, isGrouped);

                    var effectiveAssessmentDate =
                    monthyear.Allocations.Select(x => x.EffectiveDate == DateTime.MinValue || x.EffectiveDate == null ?  x.InvoiceDate: x.EffectiveDate).Min();

                    foreach (var mypod in elst)
                    {
                        //itmcount = await InitializeDocumentCT(itmcount, prevEntryId, mypod, cdoc, prevIM7, monthyear, dt, dfp).ConfigureAwait(true);
                        if (!(mypod.EntlnData.Quantity > 0)) continue;
                        if (MaxLineCount(itmcount)
                            || InvoicePerEntry(prevEntryId, mypod)
                            || (cdoc.Document == null || itmcount == 0)
                            || IsPerIM7(prevIM7, monthyear))
                        {
                            if (itmcount != 0)
                            {
                                if (cdoc.Document != null)
                                {
                                    
                                    await SaveDocumentCT(cdoc).ConfigureAwait(false);
                                    //}
                                    //else
                                    //{
                                    cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
                                }
                            }
                            Ex9InitializeCdoc(dfp, cdoc, docSet, im7Type, ex9Type);
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
                                    $"EffectiveAssessmentDate:{effectiveAssessmentDate.GetValueOrDefault().ToString("MMM-dd-yyyy")}";
                            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
                                effectiveAssessmentDate;
                            if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                            var curLst = mypod.EntlnData.EntryDataDetails.Select(x => x.Currency).Where(x => x != null).Distinct().ToList();
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

                        
                          var newItms =  await
                                CreateEx9EntryAsync(mypod, cdoc, itmcount, dfp,monthyear.MonthYear, im7Type, itemSalesPiSummaryLst, checkQtyAllocatedGreaterThanPiQuantity, applyEx9Bucket, applyHistoricChecks, applyCurrentChecks)
                                    .ConfigureAwait(false);
                       
                            itmcount += newItms;
                        

                        prevEntryId = mypod.EntlnData.EntryDataDetails.Count() > 0
                            ? mypod.EntlnData.EntryDataDetails[0].EntryDataId
                            : "";
                        prevIM7 = PerIM7 == true ? monthyear.CNumber : "";
                        StatusModel.StatusUpdate();
                    }

                }
                
                await SaveDocumentCT(cdoc).ConfigureAwait(false);
                if (cdoc.Document.ASYCUDA_Id == 0)
                {
                    //clean up
                    docSet.xcuda_ASYCUDA_ExtendedProperties.Remove(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties);
                    cdoc.Document = null;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        private bool IsPerIM7(string prevIM7, AllocationDataBlock monthyear)
        {
            return (PerIM7 == true &&
                    (string.IsNullOrEmpty(prevIM7) || (!string.IsNullOrEmpty(prevIM7) && prevIM7 != monthyear.CNumber)));
        }

        private bool InvoicePerEntry(string prevEntryId, MyPodData mypod)
        {
            return (BaseDataModel.Instance.CurrentApplicationSettings
                .InvoicePerEntry == true &&
                    //prevEntryId != "" &&
                    prevEntryId != mypod.EntlnData.EntryDataDetails[0].EntryDataId);
        }

        public bool MaxLineCount(int itmcount)
        {
            return (itmcount != 0 &&
                    itmcount%
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

                    var xcudaPreviousItems = cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Where(x => x != null).ToList();
                    if (xcudaPreviousItems.Any())
                    {
                        cdoc.Document.xcuda_Valuation.xcuda_Weight.Gross_weight =
                            xcudaPreviousItems.Sum(x => x.Net_weight);
                    }


                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<Document_Type> GetDocumentType(string dfp)
        {
            try
            {

                Document_Type dt;
                using (var ctx = new Document_TypeService())
                {
                    if (dfp == "Duty Free")
                    {
                        dt =
                            (await
                                ctx.GetDocument_TypeByExpression(
                                    "Type_of_declaration + Declaration_gen_procedure_code == \"EX9\"")
                                    .ConfigureAwait(false)).FirstOrDefault();
                    }
                    else
                    {
                        dt =
                            (await
                                ctx.GetDocument_TypeByExpression(
                                    "Type_of_declaration + Declaration_gen_procedure_code == \"IM4\"")
                                    .ConfigureAwait(false)).FirstOrDefault();
                    }
                }
                return dt;

            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        private IEnumerable<string> CreateDutyList(IEnumerable<AllocationDataBlock> slst)
        {
            try
            {
                var dutylst = slst.Where(x => x.Allocations.Count > 0).Select(x => x.DutyFreePaid).Distinct();
                return dutylst;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task<IEnumerable<AllocationDataBlock>> CreateAllocationDataBlocks(string filterExpression)
        {
            try
            {
                StatusModel.Timer("Getting ExBond Data");
                var slstSource = GetEX9Data(filterExpression);
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

        

        private IEnumerable<EX9SalesAllocations> GetEX9Data(string FilterExpression)
        {
            FilterExpression =
                FilterExpression.Replace("&& (pExpiryDate >= \"" + DateTime.Now.Date.ToShortDateString() + "\" || pExpiryDate == null)", "");

            //Regex.Replace(FilterExpression, @"&& \(pExpiryDate >= * \|\| pExpiryDate == null\)", @"");

            FilterExpression += "&& PreviousDocumentItem.DoNotAllocate != true && PreviousDocumentItem.DoNotEX != true && PreviousDocumentItem.WarehouseError == null && (PreviousDocumentItem.AsycudaDocument.DocumentType == \"IM7\" || PreviousDocumentItem.AsycudaDocument.DocumentType == \"OS7\")";

           

            var exp1 = AllocationsModel.Instance.TranslateAllocationWhereExpression(FilterExpression);
            var map = new Dictionary<string, string>()
            {
                {"pIsAssessed", "PreviousDocumentItem.IsAssessed"},
                {"pRegistrationDate", "PreviousDocumentItem.AsycudaDocument.RegistrationDate"},
                {"ApplicationSettingsId", "PreviousDocumentItem.AsycudaDocument.ApplicationSettingsId"},
                {"PiQuantity", "EX9AsycudaSalesAllocations.PiQuantity"},
                {"pQtyAllocated", "EX9AsycudaSalesAllocations.pQtyAllocated"},
                // {"pExpiryDate", "(DbFunctions.AddDays(PreviousDocumentItem.AsycudaDocument.RegistrationDate.GetValueOrDefault(),730))"},
                {"Invalid", "EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCodes.Invalid"},
                {"xBond_Item_Id == 0", "(xEntryItem_Id == null || xEntryItem_Id == 0)"}//xBondAllocations != null  && xBondAllocations.Any() == false

            };
            var exp = map.Aggregate(exp1, (current, m) => current.Replace(m.Key, m.Value));
            var res = new List<EX9SalesAllocations>();
            using (var ctx = new AllocationDSContext())
            {
                try
                {
                   IQueryable<AsycudaSalesAllocations> pres;
                    if (FilterExpression.Contains("xBond_Item_Id == 0"))
                    {
                        //TODO: use expirydate in asycuda document
                        pres = ctx.AsycudaSalesAllocations.OrderBy(x => x.AllocationId)
                            .Where(
                                x =>
                                    (DbFunctions.AddDays(
                                        ((DateTime) x.PreviousDocumentItem.AsycudaDocument.RegistrationDate), 730)) >
                                    DateTime.Now).Where(x => !x.xBondAllocations.Any());
                    }
                    else
                    {
                        pres = ctx.AsycudaSalesAllocations.OrderBy(x => x.AllocationId)
                            .Where(
                                x =>
                                    (DbFunctions.AddDays(
                                        ((DateTime) x.PreviousDocumentItem.AsycudaDocument.RegistrationDate), 730)) >
                                    DateTime.Now);
                            
                    }


                    var ppRes = pres.Where(exp)
                        .Where(x => x.PreviousItem_Id != null && x.PreviousItem_Id != 0)
                        .Where(x => x.EntryDataDetailsId != null && x.EntryDataDetailsId != 0)
                        .Where(
                            x =>
                                x.xEntryItem_Id == null &&
                                x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit.Any(ss => ss.IsFirstRow == true) &&
                                x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(ss => ss.IsFirstRow == true)
                                    .Suppplementary_unit_quantity != 0)

                        
                        .GroupJoin(ctx.xcuda_Weight_itm, x => x.PreviousItem_Id, q => q.Valuation_item_Id,
                            (x, w) => new {x, w})
                        .Where(g => g.x.PreviousDocumentItem != null && g.w.Any() && g.x.EntryDataDetails.Sales != null);
                    res = ppRes
                        .Select(c => new EX9SalesAllocations
                        {
                            AllocationId = c.x.AllocationId,
                            Commercial_Description = c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            DutyFreePaid = c.x.EntryDataDetails.Sales.TaxAmount != 0 ? "Duty Paid" : "Duty Free",
                            EntryDataDetailsId = c.x.EntryDataDetailsId,
                            InvoiceDate = c.x.EntryDataDetails.Sales.EntryDataDate,
                            EffectiveDate =  c.x.EntryDataDetails.EffectiveDate,
                            InvoiceNo = c.x.EntryDataDetails.EntryDataId,
                            ItemDescription = c.x.EntryDataDetails.ItemDescription,
                            ItemNumber = c.x.EntryDataDetails.ItemNumber,
                            pCNumber = c.x.PreviousDocumentItem.AsycudaDocument.CNumber,
                            pItemCost =
                                        (c.x.PreviousDocumentItem.xcuda_Tarification.Item_price/
                                         c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit
                                             .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity),
                            Status = c.x.Status,
                            PreviousItem_Id = c.x.PreviousItem_Id,
                            QtyAllocated = c.x.QtyAllocated,
                            SalesFactor = c.x.PreviousDocumentItem.SalesFactor,
                            SalesQtyAllocated = c.x.EntryDataDetails.QtyAllocated,
                            SalesQuantity = c.x.EntryDataDetails.Quantity,
                            pItemNumber = c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.Precision_4,
                            pItemDescription = c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            pTariffCode = c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.Commodity_code,
                            DFQtyAllocated = c.x.PreviousDocumentItem.DFQtyAllocated,
                            DPQtyAllocated = c.x.PreviousDocumentItem.DPQtyAllocated,
                            pLineNumber = c.x.PreviousDocumentItem.LineNumber,
                            LineNumber = c.x.EntryDataDetails.LineNumber,
                            //not issue for Taking Out of Bond - should depend on Export template
                            //Currency = c.x.EntryDataDetails.Sales.Currency,
                            Customs_clearance_office_code = c.x.PreviousDocumentItem.AsycudaDocument.Customs_clearance_office_code,
                            pQuantity = c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity,
                            pRegistrationDate = (DateTime) c.x.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
                            pAssessmentDate = (DateTime) c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate,
                            Country_of_origin_code =
                                c.x.PreviousDocumentItem.xcuda_Goods_description.Country_of_origin_code,
                            Total_CIF_itm =c.x.PreviousDocumentItem.xcuda_Valuation_item.Total_CIF_itm,
                            Net_weight_itm = c.w.FirstOrDefault().Net_weight_itm ,
                            // Net_weight_itm = c.x.PreviousDocumentItem != null ? ctx.xcuda_Weight_itm.FirstOrDefault(q => q.Valuation_item_Id == x.PreviousItem_Id).Net_weight_itm: 0,
                            previousItems = c.x.PreviousDocumentItem.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                    .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) && y.xcuda_Item.AsycudaDocument.Cancelled != true)
                                    .Select(z => new previousItems()
                                    {
                                        DutyFreePaid =
                                            z.xcuda_Item.AsycudaDocument.DocumentType == "IM4"
                                                ? "Duty Paid"
                                                : "Duty Free",
                                        Net_weight = z.Net_weight,
                                        Suplementary_Quantity = z.Suplementary_Quantity
                                    }).ToList(),
                            TariffSupUnitLkps =
                                c.x.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.Select(x => x.TariffSupUnitLkps).ToList()
                            //.Select(x => (ITariffSupUnitLkp)x)

                        }
                        )
                        //////////// prevent exwarehouse of item whos piQuantity > than AllocatedQuantity//////////
                        
                        .ToList();

                   


                }
                catch (Exception)
                {
                    throw;
                }
            }
            return res;
        }

        

        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(
            IEnumerable<EX9SalesAllocations> slstSource)
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
            IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {

                IEnumerable<AllocationDataBlock> slst;
                var source = slstSource.OrderBy(x => x.pTariffCode).ToList();

                slst = from s in source
                    group s by new {s.DutyFreePaid, MonthYear = "NoMTY"}
                    into g
                    select new AllocationDataBlock
                    {
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
            IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            MonthYear = "NoMTY",
                            CNumber = s.pCNumber
                        }
                    into g
                    select new AllocationDataBlock
                    {
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

        private IEnumerable<AllocationDataBlock> CreateBreakOnMonthYearAllocationDataBlocks(
            IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                if (PerIM7 == true)
                {
                    slst = CreatePerIM7AllocationDataBlocks(slstSource);
                }
                else
                {
                    slst = CreateAllocationDataBlocks(slstSource);
                }
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateAllocationDataBlocks(
            IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            MonthYear =
                                Convert.ToDateTime(
                                    s.InvoiceDate)
                                    .ToString("MMM-yy")
                        }
                    into g
                    select new AllocationDataBlock
                    {
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

        private IEnumerable<AllocationDataBlock> CreatePerIM7AllocationDataBlocks(
            IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {

                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            MonthYear =
                                Convert.ToDateTime(
                                    s.InvoiceDate)
                                    .ToString("MMM-yy"),
                            CNumber = s.pCNumber
                        }
                    into g
                    select new AllocationDataBlock
                    {
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
                              .GroupBy(x => new { x.PreviousItem_Id})
                              .Select(s => new MyPodData
                              {
                                  Allocations = s.OrderByDescending(x => x.AllocationId).Select(x =>
                                     new AsycudaSalesAllocations()
                                     {
                                         AllocationId = x.AllocationId,
                                         PreviousItem_Id = x.PreviousItem_Id,
                                         EntryDataDetailsId = x.EntryDataDetailsId,
                                         Status = x.Status,
                                         QtyAllocated =  x.QtyAllocated,
                                         EntryDataDetails = new EntryDataDetails()
                                         {
                                             EntryDataDetailsId = x.EntryDataDetailsId.GetValueOrDefault(),
                                             EntryDataId = x.InvoiceNo,
                                             Quantity = x.SalesQuantity,
                                             QtyAllocated = x.SalesQtyAllocated,
                                             EffectiveDate = x.EffectiveDate,
                                             LineNumber = x.LineNumber,
                                         }
                                     }).ToList(),
                                  EntlnData = new AlloEntryLineData
                                  {
                                      ItemNumber = s.LastOrDefault().ItemNumber,
                                      ItemDescription = s.LastOrDefault().ItemDescription,
                                      TariffCode = s.LastOrDefault().pTariffCode,
                                      Cost = s.LastOrDefault().pItemCost.GetValueOrDefault(),
                                      Quantity = s.Sum(x => x.QtyAllocated / (x.SalesFactor == 0 ? 1 : x.SalesFactor)),
                                      EntryDataDetails = s.DistinctBy(z => z.EntryDataDetailsId).Select(z =>  
                                                                new EntryDataDetailSummary()
                                                                {
                                                                    EntryDataDetailsId = z.EntryDataDetailsId.GetValueOrDefault(),
                                                                    EntryDataId = z.InvoiceNo,
                                                                    QtyAllocated = z.SalesQuantity,
                                                                    EntryDataDate = z.InvoiceDate,
                                                                    EffectiveDate = z.EffectiveDate.GetValueOrDefault(),
                                                                    Currency = z.Currency,
                                                                }).ToList(),
                                      PreviousDocumentItemId = Convert.ToInt32(s.Key.PreviousItem_Id),
                                      InternalFreight = s.LastOrDefault().InternalFreight,
                                      Freight = s.LastOrDefault().Freight,
                                      Weight = s.LastOrDefault().Weight,
                                      pDocumentItem = new pDocumentItem()
                                      {
                                          DFQtyAllocated = s.LastOrDefault().DFQtyAllocated,
                                          DPQtyAllocated = s.LastOrDefault().DPQtyAllocated,
                                          ItemQuantity = s.LastOrDefault().pQuantity.GetValueOrDefault(),
                                          LineNumber = s.LastOrDefault().pLineNumber,
                                          ItemNumber = s.LastOrDefault().pItemNumber,
                                          Description = s.LastOrDefault().pItemDescription,
                                          xcuda_ItemId = s.LastOrDefault().PreviousItem_Id.GetValueOrDefault(),
                                          AssessmentDate = s.LastOrDefault().pAssessmentDate,
                                          previousItems = s.LastOrDefault().previousItems
                                      },
                                      EX9Allocation = new EX9Allocation()
                                      {
                                          SalesFactor = s.LastOrDefault().SalesFactor,
                                          Net_weight_itm = s.LastOrDefault().Net_weight_itm,
                                          pQuantity = s.LastOrDefault().pQuantity.GetValueOrDefault(),
                                          pCNumber = s.LastOrDefault().pCNumber,
                                          Customs_clearance_office_code = s.LastOrDefault().Customs_clearance_office_code,
                                          Country_of_origin_code = s.LastOrDefault().Country_of_origin_code,
                                          pRegistrationDate = s.LastOrDefault().pRegistrationDate,
                                          pQtyAllocated = s.LastOrDefault().QtyAllocated,
                                          Total_CIF_itm = s.LastOrDefault().Total_CIF_itm,
                                          pTariffCode = s.LastOrDefault().pTariffCode
                                      },
                                      TariffSupUnitLkps = s.LastOrDefault().TariffSupUnitLkps.Select(x => (ITariffSupUnitLkp)x).ToList()

                                  }
                              }).ToList();
               


                return elst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<MyPodData> SingleAllocationPerPreviousItem(AllocationDataBlock monthyear)
        {
            try
            {
                var elst = monthyear.Allocations
                              .OrderBy(x => x.AllocationId)
                              //.GroupBy(x => new { x.PreviousItem_Id })
                              .Select(x => new MyPodData
                              {
                                  Allocations = new List<AsycudaSalesAllocations>() { 
                                     new AsycudaSalesAllocations()
                                     {
                                         AllocationId = x.AllocationId,
                                         PreviousItem_Id = x.PreviousItem_Id,
                                         EntryDataDetailsId = x.EntryDataDetailsId,
                                         Status = x.Status,
                                         QtyAllocated = x.QtyAllocated,
                                         EntryDataDetails = new EntryDataDetails()
                                         {
                                             EntryDataDetailsId = x.EntryDataDetailsId.GetValueOrDefault(),
                                             EntryDataId = x.InvoiceNo,
                                             Quantity = x.SalesQuantity,
                                             QtyAllocated = x.SalesQtyAllocated,
                                             EffectiveDate = x.EffectiveDate,
                                             LineNumber = x.LineNumber,
                                         }
                                     }},
                                  EntlnData = new AlloEntryLineData
                                  {
                                      ItemNumber = x.ItemNumber,
                                      ItemDescription = x.ItemDescription,
                                      TariffCode = x.pTariffCode,
                                      Cost = x.pItemCost.GetValueOrDefault(),
                                      Quantity = x.QtyAllocated / (x.SalesFactor == 0 ? 1: x.SalesFactor),
                                      EntryDataDetails = new List<EntryDataDetailSummary>() { 
                                                                new EntryDataDetailSummary()
                                                                {
                                                                    EntryDataDetailsId = x.EntryDataDetailsId.GetValueOrDefault(),
                                                                    EntryDataId = x.InvoiceNo,
                                                                    QtyAllocated = x.SalesQuantity,
                                                                    EntryDataDate = x.InvoiceDate,
                                                                    EffectiveDate = x.EffectiveDate.GetValueOrDefault(),
                                                                    Currency = x.Currency,
                                                                }},
                                      PreviousDocumentItemId = Convert.ToInt32(x.PreviousItem_Id),
                                      InternalFreight = x.InternalFreight,
                                      Freight = x.Freight,
                                      Weight = x.Weight,
                                      pDocumentItem = new pDocumentItem()
                                      {
                                          DFQtyAllocated = x.DFQtyAllocated,
                                          DPQtyAllocated = x.DPQtyAllocated,
                                          ItemQuantity = x.pQuantity.GetValueOrDefault(),
                                          LineNumber = x.pLineNumber,
                                          ItemNumber = x.pItemNumber,
                                          Description = x.pItemDescription,
                                          xcuda_ItemId = x.PreviousItem_Id.GetValueOrDefault(),
                                          AssessmentDate = x.pAssessmentDate,
                                          previousItems = x.previousItems
                                      },
                                      EX9Allocation = new EX9Allocation()
                                      {
                                          SalesFactor = x.SalesFactor,
                                          Net_weight_itm = x.Net_weight_itm,
                                          pQuantity = x.pQuantity.GetValueOrDefault(),
                                          pCNumber = x.pCNumber,
                                          Customs_clearance_office_code = x.Customs_clearance_office_code,
                                          Country_of_origin_code = x.Country_of_origin_code,
                                          pRegistrationDate = x.pRegistrationDate,
                                          pQtyAllocated = x.QtyAllocated,
                                          Total_CIF_itm = x.Total_CIF_itm,
                                          pTariffCode = x.pTariffCode
                                      },
                                      TariffSupUnitLkps = x.TariffSupUnitLkps.Select(z => (ITariffSupUnitLkp)z).ToList()

                                  
                              }}).ToList();



                return elst;
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
            string im7Type, List<ItemSalesPiSummary> itemSalesPiSummaryLst, bool checkQtyAllocatedGreaterThanPiQuantity,
            bool applyEx9Bucket, bool applyHistoricChecks, bool applyCurrentChecks)
        {
            try
            {

                /////////////// QtyAllocated >= piQuantity cap
                /// 
                if(checkQtyAllocatedGreaterThanPiQuantity)
                if (im7Type == "7000" || im7Type == "7400")
                {
                    if (mypod.EntlnData.pDocumentItem.QtyAllocated <= mypod.EntlnData.pDocumentItem.previousItems
                            .Select(x => x.Suplementary_Quantity).DefaultIfEmpty(0).Sum())
                    {
                        return 0;
                    }
                }
                else
                {
                    if (mypod.EntlnData.pDocumentItem.QtyAllocated < itemSalesPiSummaryLst.Select(x => x.PiQuantity).DefaultIfEmpty(0).Sum())
                    {
                        return 0;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                ///     Sales Cap/ Sales Bucket historic
                //var totalSalesLst = salesSummary.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber && x.Type == "Historic").ToList();
                //var piSummaries = piSummary.Where(x => x.Type == "Historic"
                                                       //&& x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                       //&& x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber.ToString()
                                                       //&& x.pOffice == mypod.EntlnData.EX9Allocation.Customs_clearance_office_code
                                                       //&& x.pItemQuantity.ToString() == mypod.EntlnData.pDocumentItem.ItemQuantity.ToString()
                                                       //&& (x.ItemNumber == mypod.EntlnData.ItemNumber || x.ItemNumber == mypod.EntlnData.pDocumentItem.ItemNumber)).ToList();
                List<ItemSalesPiSummary> salesPiHistoric;
                List<ItemSalesPiSummary> salesPiCurrent;
                List<ItemSalesPiSummary> itemSalesPiHistoric = new List<ItemSalesPiSummary>();
                List<ItemSalesPiSummary> itemSalesPiCurrent = new List<ItemSalesPiSummary>();
                if (im7Type == "7100")
                {
                    //TODO:Include itemSalesPiHistoric & itemSalesPiCurrent
                    salesPiHistoric = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber &&
                                                                       string.IsNullOrEmpty(x.pCNumber) &&
                                                                       x.pLineNumber == 0 &&
                                                                       x.Type == "Historic").ToList();
                    salesPiCurrent = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber &&
                                                                      string.IsNullOrEmpty(x.pCNumber) &&
                                                                      x.pLineNumber == 0 &&
                                                                      x.Type == "Current").ToList();

                }
                else
                {
                    salesPiHistoric = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                       // took this out because of changed allocation can lead to overallocation
                                                                       //eg HS/SAN2, nov 2018 - reallocated to HS/SAN2 but orignally allocated to HS-SAN2

                                                                       //&& x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                                       //&& x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                                                                       && x.Type == "Historic").ToList();
                    salesPiCurrent = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                      && x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                                      && x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                                                                      && x.Type == "Current").ToList();

                    itemSalesPiHistoric = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber

                                                                       && x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                                       && x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                                                                       && x.Type == "Historic").ToList();
                    itemSalesPiCurrent = itemSalesPiSummaryLst.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber
                                                                      && x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber
                                                                      && x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber
                                                                      && x.Type == "Current").ToList();
                }
                Debug.WriteLine($"Create EX9 For {mypod.EntlnData.ItemNumber}:{mypod.EntlnData.EntryDataDetails.First().EntryDataDate:MMM-yy} - {mypod.EntlnData.Quantity} | C#{mypod.EntlnData.EX9Allocation.pCNumber}-{mypod.EntlnData.pDocumentItem.LineNumber}");
                salesPiHistoric.ForEach(x => Debug.WriteLine($"Sales vs Pi History: {x.QtyAllocated} of {x.pQtyAllocated} - {x.PiQuantity} | C#{x.pCNumber}-{x.pLineNumber}"));

                
                var docPi = docSetPi
                    .Where(x => ((x.pCNumber == null && x.ItemNumber == mypod.EntlnData.pDocumentItem.ItemNumber) || (x.pCNumber != null && x.pCNumber == mypod.EntlnData.EX9Allocation.pCNumber))
                                && x.pLineNumber == mypod.EntlnData.pDocumentItem.LineNumber.ToString()).Select(x => x.TotalQuantity)
                    .DefaultIfEmpty(0).Sum();
                if (applyEx9Bucket)
                if (im7Type == "7100")
                {
                    await Ex9Bucket(mypod, dfp, docPi, salesPiCurrent).ConfigureAwait(false);
                }
                else
                {
                    await Ex9Bucket(mypod, dfp, docPi,
                        applyHistoricChecks?salesPiHistoric:itemSalesPiHistoric //i assume once you not doing historic checks especially for adjustments just use the specific item history
                        ).ConfigureAwait(false); //historic = 'HCLAMP/060'
                    }

                var salesFactor = Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                    ? 1
                    : mypod.EntlnData.EX9Allocation.SalesFactor;

                mypod.EntlnData.Quantity = Math.Round(mypod.EntlnData.Quantity, 2);
                Debug.WriteLine($"EX9Bucket Quantity {mypod.EntlnData.ItemNumber} - {mypod.EntlnData.Quantity}");
                if (mypod.EntlnData.Quantity <= 0) return 0;

                ////////////////////////----------------- Cap to prevent xQuantity > Sales Quantity
                double qtyAllocated = 0;
                foreach (var allocation in mypod.Allocations)
                {
                    qtyAllocated += allocation.QtyAllocated /
                                    (Math.Abs(mypod.EntlnData.EX9Allocation.SalesFactor) < 0.0001
                                        ? 1
                                        : mypod.EntlnData.EX9Allocation.SalesFactor);
                }
                // todo: ensure allocations are marked for investigation
                double qty = mypod.EntlnData.Quantity;
                if (Math.Abs(qty - Math.Round(qtyAllocated,2)) > 0.0001)
                {
                    return 0;
                }
                //////////////////////////////////////////////////////////////////////////
                ///     Sales Cap/ Sales Bucket historic

                var totalSalesHistoric = salesPiHistoric.Select(x => x.pQtyAllocated).DefaultIfEmpty(0).Sum(); //salesSummary.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber && x.Type == "Historic").Select(x => x.TotalQuantity).DefaultIfEmpty(0).Sum();
                
                var totalPiHistoric = salesPiHistoric.Select(x => x.PiQuantity).DefaultIfEmpty(0).Sum();//piSummaries.Select(x => x.TotalQuantity).DefaultIfEmpty(0).Sum();
                var itemSalesHistoric = itemSalesPiHistoric.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum(); //salesSummary.Where(x => x.ItemNumber == mypod.EntlnData.ItemNumber && x.Type == "Historic").Select(x => x.TotalQuantity).DefaultIfEmpty(0).Sum();

                var itemPiHistoric = itemSalesPiHistoric.Select(x => x.PiQuantity).DefaultIfEmpty(0).Sum();
                if (applyHistoricChecks)
                {
                    if (totalSalesHistoric == 0) return 0; // no sales found
                    if (Math.Round(totalSalesHistoric, 2) <
                        Math.Round((totalPiHistoric + docPi + mypod.EntlnData.Quantity) * salesFactor, 2))
                    {
                        return 0;
                    }
                }
                ////////////////////////////////////////////////////////////////////////

                if (applyCurrentChecks)
                {
                    //////////////////////////////////////////////////////////////////////////
                    ///     Sales Cap/ Sales Bucket Current
                    var totalSalesCurrent = salesPiCurrent.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum();

                    var totalPiCurrent = salesPiCurrent.Select(x => x.PiQuantity).DefaultIfEmpty(0).Sum();

                    if (totalSalesCurrent == 0) return 0; // no sales found
                    if (Math.Round(totalSalesCurrent, 2) < Math.Round((totalPiCurrent + mypod.EntlnData.Quantity) * salesFactor, 2))
                    {
                        return 0;
                    }
                }
                ////////////////////////////////////////////////////////////////////////
                //// Cap to prevent over creation of ex9 vs Item Quantity espectially if creating Duty paid and Duty Free at same time

                if (mypod.EntlnData.pDocumentItem.ItemQuantity < Math.Round((itemPiHistoric + docPi + mypod.EntlnData.Quantity) ,2))
                {
                    return 0;
                }
                //////////////////// can't delump allocations because of returns and 1kg weights issue too much items wont be able to exwarehouse
                var itmsToBeCreated = 1;
                var itmsCreated = 0;

                //itmsToBeCreated = BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == false && mypod.EntlnData.EX9Allocation.SalesFactor == 1
                //    ? mypod.Allocations.Count()
                //    : 1;

                

                for (int i = 0; i < itmsToBeCreated; i++)
                {

                    var lineData = mypod.EntlnData; ///itmsToBeCreated == 1 ? mypod.EntlnData : CreateLineData(mypod, i);

                    global::DocumentItemDS.Business.Entities.xcuda_PreviousItem pitm = CreatePreviousItem(
                        lineData,
                        itmcount + i, dfp);
                    if (Math.Round(pitm.Net_weight, 2) < 0.01)
                    {
                        return 0;
                    }
                    pitm.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;


                    global::DocumentItemDS.Business.Entities.xcuda_Item itm =
                        BaseDataModel.Instance.CreateItemFromEntryDataDetail(lineData, cdoc);
                    itm.xcuda_Valuation_item.Total_CIF_itm = pitm.Current_value;
                    itm.xcuda_Tarification.xcuda_HScode.Precision_4 = lineData.pDocumentItem.ItemNumber;
                    itm.xcuda_Goods_description.Commercial_Description = BaseDataModel.Instance.CleanText(lineData.pDocumentItem.Description);
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

                            itmcnt = AddFreeText(itmcnt, itm, allo.EntryDataDetails.EntryDataId, allo.EntryDataDetails.LineNumber.GetValueOrDefault());
                        }

                       
                    }


                    if (im7Type == "7000" || im7Type == "7400")
                    {
                        itm.xcuda_PreviousItem = pitm;
                        pitm.xcuda_Item = itm;

                        var ep = new EntryPreviousItems(true) { Item_Id = lineData.PreviousDocumentItemId, PreviousItem_Id = pitm.PreviousItem_Id, TrackingState = TrackingState.Added };
                        pitm.xcuda_Items.Add(ep);





                        if (cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Count() == 1 || itmcount == 0)
                        {
                            pitm.Packages_number = "1"; //(i.Packages.Number_of_packages).ToString();
                            pitm.Previous_Packages_number = pitm.Previous_item_number == "1" ? "1" : "0";

                            
                                itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                                {
                                    Attached_document_code = BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x => x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.DisplayName)?.AttachedDocumentCode ?? "DFS1",
                                    Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                                    Attached_document_reference = lineData.EntryDataDetails.First().EntryDataId,
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
                                    TrackingState = TrackingState.Added
                                };
                                itm.xcuda_Packages.Add(pkg);
                            }
                            pkg.Number_of_packages =
                                Convert.ToDouble(pitm.Previous_Packages_number);
                        }
                    }




                    itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pitm.Hs_code;
                    itm.xcuda_Goods_description.Country_of_origin_code = pitm.Goods_origin;



                    itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
                    {
                        TrackingState = TrackingState.Added
                    };
                    itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = pitm.Net_weight;
                    itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = pitm.Net_weight;
                    // adjusting because not using real statistical value when calculating
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency =
                        Convert.ToDouble(Math.Round((pitm.Current_value * pitm.Suplementary_Quantity), 2));
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency =
                        Convert.ToDouble(Math.Round(pitm.Current_value * pitm.Suplementary_Quantity, 2));
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_code = "XCD";
                    itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_rate = 1;

                    docSetPi.Add(new PiSummary()
                    {
                        ItemNumber = mypod.EntlnData.ItemNumber,
                        DutyFreePaid = dfp,
                        TotalQuantity = mypod.EntlnData.Quantity,
                        pCNumber = mypod.EntlnData.EX9Allocation.pCNumber,
                        pLineNumber = mypod.EntlnData.pDocumentItem.LineNumber.ToString()
                    });

                    itmsCreated += 1;

                }



                return itmsCreated;
            }
            catch (Exception Ex)
            {
                throw;
            }


        }

        private static pDocumentItem oldPDocumentItem = null;
        static List<previousItems> existingPreviousItems = new List<previousItems>(); 
        private List<previousItems> Get7100PreviousItems(AlloEntryLineData entlnDataPDocumentItem, string dfp)
        {
            if(oldPDocumentItem.xcuda_ItemId == entlnDataPDocumentItem.PreviousDocumentItemId) return existingPreviousItems;
            oldPDocumentItem = entlnDataPDocumentItem.pDocumentItem;
            using (var ctx = new AllocationDSContext())
            {

            }
                return existingPreviousItems;
        }

        

        private int AddFreeText(int itmcnt, xcuda_Item itm, string entryDataId, int lineNumber)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 == true) return itmcnt;
            if (itm.Free_text_1 == null) itm.Free_text_1 = "";
            itm.Free_text_1 = $"{entryDataId}|{lineNumber}";
            itmcnt += 1;

            if (itm.Free_text_1 != null && itm.Free_text_1.Length > 1)
            {
                itm.Free_text_1 = itm.Free_text_1.Length < 31 ? itm.Free_text_1.Substring(0) 
                                                              : itm.Free_text_1.Substring(0, 30);
            }


            if (itm.Free_text_2 != null && itm.Free_text_2.Length > 1)
            {
                itm.Free_text_2 = itm.Free_text_2.Length < 21 ? itm.Free_text_2.Substring(0) 
                                                              : itm.Free_text_2.Substring(0, 20);
            }
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
                var allocationSales = itemSalesPiSummaryLst.Select(x => x.QtyAllocated).DefaultIfEmpty(0).Sum();//switch to QtyAllocated - 'CLC/124075'

                var allocationPi = itemSalesPiSummaryLst.Select(x => x.PiQuantity).DefaultIfEmpty(0).Sum();
                //var totalPiQtyHistoric = piSummaries.Where(x => x.DutyFreePaid == dfp).Select(x => x.TotalQuantity).DefaultIfEmpty(0).Sum();
                var dutyFreePaidAllocated = allocationSales;//totalSalesQtyAllocatedHistoric;// itemAllocated < allocationsAllocated ? itemAllocated : allocationsAllocated;


                if (dutyFreePaidAllocated > asycudaLine.ItemQuantity) dutyFreePaidAllocated = asycudaLine.ItemQuantity;
                //if (previousItem.QtyAllocated == 0) return;

                var asycudaTotalQuantity = asycudaLine.ItemQuantity;//PdfpAllocated;//


                //if (asycudaLine.previousItems.Any() == false && entryLine.Quantity > asycudaTotalQuantity)
                //{
                //    entryLine.Quantity = asycudaTotalQuantity;
                //    return;
                //}

                var alreadyTakenOutItemsLst = asycudaLine.previousItems;
                var alreadyTakenOutDFPQty = alreadyTakenOutItemsLst.Any()? alreadyTakenOutItemsLst.Where(x => x.DutyFreePaid == dfp).Sum(xx => xx.Suplementary_Quantity):0;
                var alreadyTakenOutTotalQuantity = alreadyTakenOutItemsLst.Sum(xx => xx.Suplementary_Quantity);

                 var remainingQtyToBeTakenOut = Math.Round(allocationSales - (allocationPi + docPi) , 3); //Math.Round(dutyFreePaidAllocated - alreadyTakenOutDFPQty,3);

                if ((Math.Abs(asycudaTotalQuantity - alreadyTakenOutTotalQuantity) < 0.01) 
                    //|| (Math.Abs(dutyFreePaidAllocated - alreadyTakenOutDFPQty) < 0.01)  //////////////Allow historical adjustment
                    || (Math.Abs(remainingQtyToBeTakenOut) < .01)
                    || allocationSales < allocationPi)
                {
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
                    var totalGottenOut = 0;
                        var salesrow = 0;
                    while (remainingQtyToBeTakenOut < totalAllocatedQty )
                    {

                        if (entryLine.Quantity <= 0.001) break;
                        if (Math.Abs(remainingQtyToBeTakenOut) < 0.001) break;
                        if (salesLst.Any() == false) break;

                        EntryDataDetailSummary saleItm = salesLst.ElementAtOrDefault(salesrow);
                        if (saleItm == null) break;
                        var saleAllocationsLst = allocations.Where(x => x.EntryDataDetailsId == saleItm.EntryDataDetailsId).OrderBy(x => x.AllocationId).ToList();
                        foreach (var allocation in saleAllocationsLst)
                        {
                            if (remainingQtyToBeTakenOut == totalAllocatedQty) break;
                            var wantToTakeOut = totalAllocatedQty - remainingQtyToBeTakenOut;
                            var takeOut = allocation.QtyAllocated / salesFactor;
                            if (takeOut > wantToTakeOut) takeOut = wantToTakeOut;
                            
                            if (remainingQtyToBeTakenOut > totalAllocatedQty) takeOut = totalAllocatedQty;
                            totalAllocatedQty -= takeOut;
                            allocation.QtyAllocated -= takeOut * salesFactor;
                            saleItm.QtyAllocated -= takeOut * salesFactor;
                            entryLine.Quantity -= takeOut;

                            if (Math.Abs(allocation.QtyAllocated) < 0.001)
                            {
                                allocations.Remove(allocation);
                            }
                            else
                            {
                                var sql = "";

                                // Create New allocation
                                sql +=
                                    $@"Insert into AsycudaSalesAllocations(QtyAllocated, EntryDataDetailsId, PreviousItem_Id, EANumber, SANumber)
                                           Values ({takeOut * salesFactor},{allocation.EntryDataDetailsId},{
                                            allocation.PreviousItem_Id
                                        },{allocation.EANumber + 1},{allocation.SANumber + 1})";
                                // update existing allocation
                                sql += $@" UPDATE       AsycudaSalesAllocations
                                                            SET                QtyAllocated =  QtyAllocated{(takeOut >= 0 ? $"-{takeOut * salesFactor}" : $"+{takeOut * -1 * salesFactor}")}
                                                            where	AllocationId = {allocation.AllocationId}";
                                using (var ctx = new AllocationDSContext())
                                {
                                    ctx.Database.CommandTimeout = 5;
                                    ctx.Database
                                        .ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
                                }
                            }
                            

                        }

                        if (Math.Abs(saleItm.QtyAllocated) < 0.001)
                        {
                            entryLine.EntryDataDetails.Remove(saleItm);
                           // salesLst.RemoveAt(0);
                        }
                        salesrow += 1;

                    }


                   // entryLine.Quantity = remainingQtyToBeTakenOut;
                   
                //}
                //if (entryLine.Quantity + alreadyTakenOutTotalQuantity > asycudaTotalQuantity) entryLine.Quantity = asycudaTotalQuantity - alreadyTakenOutTotalQuantity;


            }
            catch (Exception Ex)
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
                pitm.Commodity_code = "00";
                pitm.Current_item_number = (itmcount + 1).ToString(); // piggy back the previous item count
                pitm.Previous_item_number = previousItem.LineNumber.ToString();


                SetWeights(pod, pitm, dfp);


                pitm.Previous_Packages_number = "0";


                pitm.Suplementary_Quantity = Convert.ToDouble(pod.Quantity);
                pitm.Preveious_suplementary_quantity = Convert.ToDouble(pod.EX9Allocation.pQuantity);


                pitm.Goods_origin = pod.EX9Allocation.Country_of_origin_code;
                double pval = pod.EX9Allocation.Total_CIF_itm;//previousItem.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency;
                pitm.Previous_value = Convert.ToDouble((pval / pod.EX9Allocation.pQuantity));
                pitm.Current_value = Convert.ToDouble((pval) / Convert.ToDouble(pod.EX9Allocation.pQuantity));
                pitm.Prev_reg_ser = "C";
                pitm.Prev_reg_nbr = pod.EX9Allocation.pCNumber;
                pitm.Prev_reg_dat = pod.EX9Allocation.pRegistrationDate.Year.ToString();
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
                var plst = previousItem.previousItems;
                var pw = Convert.ToDouble(pod.EX9Allocation.Net_weight_itm);
                
                var iw = Convert.ToDouble((pod.EX9Allocation.Net_weight_itm
                                           / pod.EX9Allocation.pQuantity) * Convert.ToDouble(pod.Quantity));

                //var ppdfpqty =
                //        plst.Where(x => x.DutyFreePaid != dfp)
                //        .Sum(x => x.Suplementary_Quantity);
                //var pi = pod.EX9Allocation.pQuantity -
                //         plst.Where(x => x.DutyFreePaid == dfp)
                //             .Sum(x => x.Suplementary_Quantity);
                //var pdfpqty = (dfp == "Duty Free"
                //    ? previousItem.DPQtyAllocated
                //    : previousItem.DFQtyAllocated);
                var rw = plst.ToList().Sum(x => x.Net_weight);

                if ((pod.EX9Allocation.pQuantity - (plst.Sum(x => x.Suplementary_Quantity) + pod.Quantity))  <= 0)
                {

                    pitm.Net_weight = Math.Round(Convert.ToDouble(pw - rw), 2, MidpointRounding.ToEven);
                }
                else
                {
                    pitm.Net_weight = Convert.ToDouble(Math.Round(iw, 2));
                }

                pitm.Prev_net_weight = pw; //Convert.ToDouble((pw/pod.EX9Allocation.SalesFactor) - rw);
                if (pitm.Net_weight > pitm.Prev_net_weight) pitm.Net_weight = pitm.Prev_net_weight;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Ex9InitializeCdoc(string dfp, DocumentCT cdoc, AsycudaDocumentSet ads, string im7Type, string ex9Type)
        {
            try
            {

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
                BaseDataModel.Instance.IntCdoc(cdoc, ads);
                Customs_Procedure customsProcedure;
                
                switch (ex9Type)
                {
                    case "EX9":


                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Duty Free Entries";
                        customsProcedure =
                            BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                                .FirstOrDefault(x =>
                                    x.DisplayName == (im7Type == "7000" ? "9071-000" : "9072-000") ||
                                    x.DisplayName == (im7Type == "7400" ? "9074-000" : "9075-000"));

                        break;
                    case "IM4":

                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Duty Paid Entries";
                        customsProcedure = BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                            .FirstOrDefault(
                                x =>
                                    x.DisplayName == (im7Type == "7000" ? "4071-000" : "4072-000") ||
                                    x.DisplayName == (im7Type == "7400" ? "4074-000" : "4075-000"));




                        break;
                    case "IM4-801":

                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Duty Paid Entries";
                        customsProcedure = BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                            .FirstOrDefault(
                                x =>
                                    x.DisplayName == (im7Type == "7000" ? "4071-000" : "4072-000") ||
                                    x.DisplayName == (im7Type == "7400" ? "4074-801" : "4075-801"));




                        break;
                    case "IM9":
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Adjustment Short Entries";
                        customsProcedure = BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                            .FirstOrDefault(
                                x =>
                                    x.DisplayName == (im7Type == "7000" ? "9370-EXW" : "9371-EXW") ||
                                    x.DisplayName == (im7Type == "7400" ? "9374-EXW" : "9375-EXW"));
                        break;
                    default:
                        throw new ApplicationException("Unspecified Ex9 Type");


                }

                BaseDataModel.Instance.AttachCustomProcedure(cdoc, customsProcedure);
                AllocationsModel.Instance.AddDutyFreePaidtoRef(cdoc, dfp, ads);

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
        }

        public class EX9Allocation
        {
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
    }

    public class previousItems
    {
        public string DutyFreePaid { get; set; }
        public double Suplementary_Quantity { get; set; }
        public double Net_weight { get; set; }
    }


    public class EX9SalesAllocations
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
        public string Commercial_Description { get; set; }
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
    }

    public class AllocationDataBlock
    {
        public string MonthYear { get; set; }
        public string DutyFreePaid { get; set; }
        public List<EX9SalesAllocations> Allocations { get; set; }
        public string CNumber { get; set; }
    }

    public class MyPodData
    {
        public List<AsycudaSalesAllocations> Allocations { get; set; }
        public CreateEx9Class.AlloEntryLineData EntlnData { get; set; }
    }

    public class AlloEntryLineData
    {
    }
}