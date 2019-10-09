using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using Core.Common.Data;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using System.Linq.Dynamic;
using TrackableEntities;
using WaterNut.DataSpace;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using MoreLinq;

namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentShortService
    {
        private List<AsycudaDocumentItem> _itemCache;
        private List<InventoryItemAliasX> _itemAliaCache;

        public async Task AutoMatch(int applicationSettingsId)
        {



            using (var ctx = new AdjustmentQSContext() {StartTracking = true})
            {
                ctx.Database.CommandTimeout = 0;

                var lst = ctx.AdjustmentDetails
                    .Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    //.Where(x => x.ItemNumber == "LOF/00276D")
                    .Where(x => x.EffectiveDate == null && ((x.InvoiceQty > x.ReceivedQty && !x.ShortAllocations.Any())
                                                            || (x.InvoiceQty < x.ReceivedQty &&
                                                                !x.AsycudaDocumentItemEntryDataDetails.Any())))
                    .OrderBy(x => x.EntryDataDetailsId)
                    .ToList();
                StatusModel.StartStatusUpdate("Matching Shorts To Asycuda Entries", lst.Count());
                foreach (var s in lst)
                {
                    //StatusModel.StatusUpdate("Matching Shorts To Asycuda Entries");
                    var tryCNumber = false;
                    try
                    {
                        ctx.SaveChanges();

                        if (string.IsNullOrEmpty(s.ItemNumber)) continue;
                        var ed = ctx.EntryDataDetails.First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                        if (!string.IsNullOrEmpty(s.PreviousInvoiceNumber))
                        {
                            var aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                                    s.PreviousInvoiceNumber,s.EntryDataId, s.ItemNumber)
                                .ConfigureAwait(false);
                            if (aItem.Any())
                            {
                                MatchToAsycudaItem(s, aItem, ed, ctx);
                                //continue;
                            }
                            else
                            {
                                tryCNumber = true;
                            }

                        }

                        if (( tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber) )&&
                                 s.InvoiceQty.GetValueOrDefault() > 0 && !string.IsNullOrEmpty(s.PreviousCNumber))
                        {
                            var aItem = await GetAsycudaEntriesInCNumber(s.PreviousCNumber, s.ItemNumber)
                                .ConfigureAwait(false);
                            if (!aItem.Any())
                                aItem = await GetAsycudaEntriesInCNumberReference(applicationSettingsId,
                                        s.PreviousCNumber, s.ItemNumber)
                                    .ConfigureAwait(false);
                            MatchToAsycudaItem(s, aItem, ed, ctx);
                            // continue;
                        }

                        else if ((tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber)) &&
                                 s.InvoiceQty.GetValueOrDefault() <= 0 && !string.IsNullOrEmpty(s.PreviousCNumber))
                        {
                            var asycudaDocument = GetAsycudaDocumentInCNumber(applicationSettingsId, s.PreviousCNumber);
                            if (asycudaDocument != null)
                            {
                                ed.EffectiveDate = asycudaDocument.AssessmentDate;
                            }
                            else
                            {
                                ed.Status = "No Asycuda Entry Found";
                            }

                        }
                        else
                        {
                            //Set Overs 1st and Shorts to Last of Month
                            if (ed.EffectiveDate == null) ed.EffectiveDate = s.AdjustmentEx.InvoiceDate;

                        }

                        if (ed.Cost != 0) continue;
                        if (s.InvoiceQty.GetValueOrDefault() > 0)
                            continue; // if invoice > 0 it should have been imported

                        //////////// only apply to Adjustments because they are after shipment... discrepancies have to be provided.
                        if (s.Type != "ADJ") continue;
                        var lastItemCost = ctx.AsycudaDocumentItemLastItemCosts
                            .Where(x => x.assessmentdate <= ed.EffectiveDate)
                            .OrderByDescending(x => x.assessmentdate)
                            .FirstOrDefault(x =>
                                x.ItemNumber == ed.ItemNumber &&
                                x.applicationsettingsid == applicationSettingsId);
                        if (lastItemCost != null)
                            ed.LastCost = (double) lastItemCost.LocalItemCost.GetValueOrDefault();



                        

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                    ctx.SaveChanges();
                StatusModel.StopStatusUpdate();
            }

        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
            string sPreviousInvoiceNumber, string sEntryDataId, string sItemNumber)
        {
            try
            {

                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null &&
                                x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim() &&
                                (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                 || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())));
                var res = aItem.ToList();
                var alias = ItemAliasCache
                    .Where(x => x.InventoryItemsEx.ApplicationSettingsId == applicationSettingsId &&
                                x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim())
                    .Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null && alias.Contains(x.ItemNumber) &&
                                (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                 || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())))
                    .ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task CreateIM9(string filterExpression, bool perInvoice, bool process7100,
            int asycudaDocumentSetId, string ex9Type, string dutyFreePaid)
        {
            try
            {
                var docSet =
                    await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                        .ConfigureAwait(false);

                var exPro =
                    $"&& PreviousDocumentItem.AsycudaDocument.ApplicationSettingsId = {docSet.ApplicationSettingsId}" +
                    "&& (PreviousDocumentItem.AsycudaDocument.Extended_customs_procedure == \"7000\" || PreviousDocumentItem.AsycudaDocument.Extended_customs_procedure == \"7400\")";
                var slst =
                    (await CreateAllocationDataBlocks(perInvoice, filterExpression + exPro).ConfigureAwait(false))
                    .Where(
                        x => x.Allocations.Count > 0);
                if (slst != null && slst.ToList().Any())
                {

                    foreach (var dbBlock in slst)
                    {
                        // must use the month cuz assessment date could be 1 or 30
                        var startDate =
                            new DateTime(dbBlock.Allocations.Min(x => x.EffectiveDate).GetValueOrDefault().Year,
                                dbBlock.Allocations.Min(x => x.EffectiveDate).GetValueOrDefault().Month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);
                        var itemPiSummarylst = CreateEx9Class.Instance.GetItemSalesPiSummary(docSet.ApplicationSettingsId,startDate, endDate, dutyFreePaid);


                        await CreateEx9Class.Instance.CreateDutyFreePaidDocument(dutyFreePaid,
                            new List<AllocationDataBlock>() {dbBlock}, docSet, "7400", false, itemPiSummarylst, false,
                            false, ex9Type, true, "Current", true, false, true).ConfigureAwait(false);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }




        }

        //private List<CreateEx9Class.ItemSalesPiSummary> GetItemPiSummary(int applicationSettingsId,
        //    DateTime startDate, DateTime endDate,
        //    List<string> itmList)
        //{
        //    try
        //    {


        //        using (var ctx = new AllocationDSContext())
        //        {
        //            var resHistoric = ctx.AdjustmentShortAllocations
        //                .Where(x => x.ApplicationSettingsId == applicationSettingsId)
        //                .Where(x => x.EffectiveDate <= endDate)
        //                .Where(x => itmList.Contains(x.ItemNumber))
        //                .Where(x => x.PreviousItem_Id != 0)
        //                .GroupBy(g => new
        //                {
        //                    PreviousItem_Id = g.PreviousItem_Id,
        //                    PreviousDocumentItem = new
        //                    {
        //                        PreviousItem_Id = g.PreviousItem_Id,
        //                        PiQuantity = g.PreviousDocumentItem.EntryPreviousItems
        //                            .Where(z => z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Cancelled != true)
        //                            .Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
        //                        //QtyAllocated = g.PreviousDocumentItem.DFQtyAllocated + g.PreviousDocumentItem.DPQtyAllocated,
        //                        QtyAllocated = g.PreviousDocumentItem.DFQtyAllocated,//assume im9 are duty free
        //                        pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
        //                        pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
        //                        pLineNumber = g.PreviousDocumentItem.LineNumber
        //                    },
        //                    ItemNumber = g.EntryDataDetails.ItemNumber,
        //                    DutyFreePaid = g.DutyFreePaid
        //                }).Select(x => new CreateEx9Class.ItemSalesPiSummary
        //                {
        //                    ItemNumber = x.Key.ItemNumber,
        //                    QtyAllocated = (double) x.Key.PreviousDocumentItem.QtyAllocated,
        //                    pQtyAllocated = x.Key.PreviousDocumentItem.QtyAllocated,
        //                    PiQuantity = (double) x.Key.PreviousDocumentItem.PiQuantity,
        //                    pCNumber = x.Key.PreviousDocumentItem.pCNumber,
        //                    pRegistrationDate = x.Key.PreviousDocumentItem.pRegistrationDate,
        //                    pLineNumber = x.Key.PreviousDocumentItem.pLineNumber,
        //                    DutyFreePaid = x.Key.DutyFreePaid,
        //                    Type = "Historic"

        //                }).ToList();

        //            var resCurrent = ctx.AdjustmentShortAllocations
        //                .Where(x => x.ApplicationSettingsId == applicationSettingsId)
        //                .Where(x => x.EffectiveDate <= endDate)
        //                .Where(x => itmList.Contains(x.ItemNumber))
        //                .Where(x => x.PreviousItem_Id != 0)
        //                .GroupBy(g => new
        //                {
        //                    PreviousItem_Id = g.PreviousItem_Id,
        //                    PreviousDocumentItem = new
        //                    {
        //                        PreviousItem_Id = g.PreviousItem_Id,
        //                        PiQuantity = g.PreviousDocumentItem.EntryPreviousItems
        //                            .Where(z => z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Cancelled != true)
        //                            .Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
        //                        //QtyAllocated = g.PreviousDocumentItem.DFQtyAllocated + g.PreviousDocumentItem.DPQtyAllocated,
        //                        QtyAllocated = g.PreviousDocumentItem.DFQtyAllocated,//assume im9 are duty free
        //                        pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
        //                        pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
        //                        pLineNumber = g.PreviousDocumentItem.LineNumber
        //                    },
        //                    ItemNumber = g.EntryDataDetails.ItemNumber,
        //                    DutyFreePaid = g.DutyFreePaid
        //                }).Select(x => new CreateEx9Class.ItemSalesPiSummary
        //                {
        //                    ItemNumber = x.Key.ItemNumber,
        //                    QtyAllocated = (double)x.Key.PreviousDocumentItem.QtyAllocated,
        //                    pQtyAllocated = x.Key.PreviousDocumentItem.QtyAllocated,
        //                    PiQuantity = (double)x.Key.PreviousDocumentItem.PiQuantity,
        //                    pCNumber = x.Key.PreviousDocumentItem.pCNumber,
        //                    pRegistrationDate = x.Key.PreviousDocumentItem.pRegistrationDate,
        //                    pLineNumber = x.Key.PreviousDocumentItem.pLineNumber,
        //                    DutyFreePaid = x.Key.DutyFreePaid,
        //                    Type = "Current"

        //                }).ToList();

        //            //var res2 = ctx.AdjustmentShortAllocations.Where(x =>
        //            //            x.EffectiveDate >= startDate &&
        //            //            x.EffectiveDate <= endDate)
        //            //        .Where(x => itmList.Contains(x.ItemNumber))
        //            //        .Where(x => x.PreviousItem_Id != 0)
        //            //        .GroupBy(g => new
        //            //        {
        //            //            PreviousDocumentItem = new
        //            //            {
        //            //                PiQuantity = g.PreviousDocumentItem.EntryPreviousItems
        //            //                    .Where(z =>
        //            //                        z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate >= startDate &&
        //            //                        z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.AssessmentDate <= endDate &&
        //            //                        z.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Cancelled != true)
        //            //                    .Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),

        //            //                pCNumber = g.PreviousDocumentItem.AsycudaDocument.CNumber,
        //            //                pRegistrationDate = g.PreviousDocumentItem.AsycudaDocument.RegistrationDate,
        //            //                pLineNumber = g.PreviousDocumentItem.LineNumber
        //            //            },
        //            //            ItemNumber = g.EntryDataDetails.ItemNumber,
        //            //            DutyFreePaid = "Duty Free"
        //            //        }).Select(x => new CreateEx9Class.ItemSalesPiSummary
        //            //        {
        //            //            ItemNumber = x.Key.ItemNumber,
        //            //            QtyAllocated = (double) x.Select(z => z.QtyAllocated).DefaultIfEmpty(0).Sum(),//this has to be current month qty allocated
        //            //            PiQuantity = x.Key.PreviousDocumentItem.PiQuantity,
        //            //            pCNumber = x.Key.PreviousDocumentItem.pCNumber,
        //            //            pRegistrationDate = x.Key.PreviousDocumentItem.pRegistrationDate,
        //            //            pLineNumber = x.Key.PreviousDocumentItem.pLineNumber,
        //            //            DutyFreePaid = x.Key.DutyFreePaid,
        //            //            Type = "Current"

        //            //        }).ToList();

        //            //res1.AddRange(res2);

        //            return resHistoric;
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }

        //}

        private async Task<IEnumerable<AllocationDataBlock>> CreateAllocationDataBlocks(bool perInvoice,string filterExpression)
        {
            try
            {
                StatusModel.Timer("Getting IM9 Data");
                var slstSource = await GetIM9Data(filterExpression).ConfigureAwait(false);
                StatusModel.StartStatusUpdate("Creating IM9 Entries", slstSource.Count());
                IEnumerable<AllocationDataBlock> slst;
                slst = CreateWholeAllocationDataBlocks(perInvoice,slstSource);
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(bool perInvoice,
            IEnumerable<EX9SalesAllocations> slstSource)
        {
            IEnumerable<AllocationDataBlock> slst;
            if (perInvoice == true)
            {
                slst = CreatePerInvoiceAllocationDataBlocks(slstSource);
            }
            else
            {
                slst = CreateNonPerInvoiceAllocationDataBlocks(slstSource);
            }
            return slst;
        }

        private IEnumerable<AllocationDataBlock> CreateNonPerInvoiceAllocationDataBlocks(IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            MonthYear = s.EffectiveDate.GetValueOrDefault().ToString("MMM-yy")
                        }
                    into g
                    select new AllocationDataBlock
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList()
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreatePerInvoiceAllocationDataBlocks(IEnumerable<EX9SalesAllocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            MonthYear = s.EffectiveDate.GetValueOrDefault().ToString("MMM-yy"),
                            s.InvoiceNo
                        }
                    into g
                    select new AllocationDataBlock
                    {
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                        CNumber = g.Key.InvoiceNo
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async  Task<List<EX9SalesAllocations>> GetIM9Data(string FilterExpression)
        {
            FilterExpression =
                FilterExpression.Replace("&& (pExpiryDate >= \"" + DateTime.Now.Date.ToShortDateString() + "\")", "");

            FilterExpression += "&& PreviousDocumentItem.DoNotAllocate != true && PreviousDocumentItem.DoNotEX != true && PreviousDocumentItem.WarehouseError == null && (PreviousDocumentItem.AsycudaDocument.DocumentType == \"IM7\" || PreviousDocumentItem.AsycudaDocument.DocumentType == \"OS7\")";



            var exp1 = AllocationsModel.Instance.TranslateAllocationWhereExpression(FilterExpression);
            var map = new Dictionary<string, string>()
            {
                {"pIsAssessed", "PreviousDocumentItem.IsAssessed"},
                {"pRegistrationDate", "PreviousDocumentItem.AsycudaDocument.RegistrationDate"},
                { "EntryDataId", "InvoiceNo" },
                {"pExpiryDate", "(DbFunctions.AddDays(PreviousDocumentItem.AsycudaDocument.RegistrationDate.GetValueOrDefault(),730))"},
                {"Invalid", "EntryDataDetails.InventoryItem.TariffCodes.Invalid"},
               // {"xBond_Item_Id == 0", "(xBond_Item_Id == null || xBond_Item_Id == 0)"}//xBondAllocations != null  && xBondAllocations.Any() == false

            };
            var exp = map.Aggregate(exp1, (current, m) => current.Replace(m.Key, m.Value));
            var res = new List<EX9SalesAllocations>();
            using (var ctx = new AllocationDSContext())
            {
                try
                {
                    IQueryable<AdjustmentShortAllocations> pres;
                    if (FilterExpression.Contains("xBond_Item_Id == 0"))
                    {
                        //TODO: use expirydate in asycuda document
                        pres = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                            .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                            .Where(x => x.xBond_Item_Id == 0);
                    }
                    else
                    {
                        pres = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                            .Where(x =>x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now);

                    }

                    //var res1 = pres
                    //    .Where(exp)
                    //    .Where(x => x.Status == null || x.Status == "Short Shipped")
                    //    .Where(
                    //        x =>
                    //            x.xBond_Item_Id == 0 &&
                    //            x.pQuantity != 0)
                    //    .GroupJoin(ctx.AsycudaItemPiQuantityData, x => x.PreviousItem_Id, q => q.Item_Id,
                    //        (x, w) => new { x, w })
                    //    .Where(x => !x.w.Any(z => z.PiQuantity == x.x.QtyAllocated))
                    //    .Select(x => x.x)
                    //    .ToList();



                    res = pres
                        .Where(exp)
                        .Where(x => x.Status == null || x.Status == "Short Shipped")
                        .Where(
                            x =>
                                x.xBond_Item_Id == 0 &&
                                x.pQuantity != 0)

                        //.GroupJoin(ctx.AsycudaItemPiQuantityData, x => x.PreviousItem_Id, q => q.Item_Id,
                        //    (x, w) => new { x, w })
                        //.Where(x => x.w.Any(z => z.PiQuantity == x.x.QtyAllocated))
                        //.Select(x => x.x)
                        //.Where(x => !x.EntryDataDetails.AsycudaDocumentItemEntryDataDetails.Any(z =>
                        //    z.DocumentType == "IM9" && z.Quantity == x.QtyAllocated))

                        .GroupJoin(ctx.xcuda_Weight_itm, x => x.PreviousItem_Id, q => q.Valuation_item_Id,
                            (x, w) => new { x, w })
                        // took this out to allow creating entries on is manually assessed entries 
                        //.Where(g => g.x.pCNumber != null) 
                        .Where(g => g.w.Any())
                        .Select(c => new EX9SalesAllocations
                        {
                            AllocationId = c.x.AllocationId,
                            Commercial_Description =
                                c.x.PreviousDocumentItem == null
                                    ? null
                                    : c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            DutyFreePaid = c.x.DutyFreePaid,
                            EntryDataDetailsId = (int)c.x.EntryDataDetailsId,
                            InvoiceDate = c.x.InvoiceDate,
                            EffectiveDate = (DateTime) c.x.EffectiveDate,
                            InvoiceNo = c.x.InvoiceNo,
                            ItemDescription = c.x.ItemDescription,
                            ItemNumber = c.x.ItemNumber,
                            pCNumber = c.x.pCNumber,
                            pItemCost = (double)
                                        (c.x.PreviousDocumentItem.xcuda_Tarification.Item_price /
                                         c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit
                                             .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity),
                            Status = c.x.Status,
                            PreviousItem_Id = c.x.PreviousItem_Id,
                            QtyAllocated = (double)c.x.QtyAllocated,
                            SalesFactor = c.x.PreviousDocumentItem.SalesFactor,
                            SalesQtyAllocated = (double)c.x.SalesQtyAllocated,
                            SalesQuantity = (int)c.x.SalesQuantity,
                            pItemNumber = c.x.pItemNumber,
                            pItemDescription = c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            pTariffCode = c.x.pTariffCode,
                            DFQtyAllocated = c.x.PreviousDocumentItem.DFQtyAllocated,
                            DPQtyAllocated = c.x.PreviousDocumentItem.DPQtyAllocated,
                            LineNumber = (int)c.x.EntryDataDetails.LineNumber,
                            pLineNumber = c.x.PreviousDocumentItem.LineNumber,
                            // Currency don't matter for im9 or exwarehouse
                            Customs_clearance_office_code = c.x.PreviousDocumentItem.AsycudaDocument.Customs_clearance_office_code,
                            pQuantity = (double)c.x.pQuantity,
                            pRegistrationDate = (DateTime)(c.x.pRegistrationDate ?? c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate),
                            pAssessmentDate = (DateTime)c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate,
                            Country_of_origin_code =
                                c.x.PreviousDocumentItem.xcuda_Goods_description.Country_of_origin_code,
                            Total_CIF_itm = c.x.PreviousDocumentItem.xcuda_Valuation_item.Total_CIF_itm,
                            Net_weight_itm = c.w.FirstOrDefault().Net_weight_itm,
                            // Net_weight_itm = c.x.PreviousDocumentItem != null ? ctx.xcuda_Weight_itm.FirstOrDefault(q => q.Valuation_item_Id == x.PreviousItem_Id).Net_weight_itm: 0,
                            previousItems = c.x.PreviousDocumentItem.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                    .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) && y.xcuda_Item.AsycudaDocument.Cancelled != true)
                                    .Select(z => new previousItems()
                                    {
                                        DutyFreePaid =
                                            z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4074" || z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070"
                                                ? "Duty Paid"
                                                : "Duty Free",
                                        Net_weight = z.Net_weight,
                                        Suplementary_Quantity = z.Suplementary_Quantity
                                    }).ToList(),
                            TariffSupUnitLkps =
                                c.x.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.Select(x => x.TariffSupUnitLkps).ToList(),
                            FileTypeId = c.x.FileTypeId,
                            EmailId = c.x.EmailId,
                            //.Select(x => (ITariffSupUnitLkp)x)

                        }
                        )
                        //////////// prevent exwarehouse of item whos piQuantity > than AllocatedQuantity//////////

                        .ToList();
                   
                    return res;

                }
                catch (Exception)
                {
                    throw;
                }
            }
          
        }


        private AsycudaDocument GetAsycudaDocumentInCNumber(int applicationSettingsId, string cNumber)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                try
                {


                    // get document item in cnumber
                    var clst = cNumber.Replace("C", "").Split(',');
                    var res = ctx.AsycudaDocuments
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => (x.CNumber != null || x.IsManuallyAssessed == true) &&
                                    (x.Extended_customs_procedure == "7000" || x.Extended_customs_procedure == "7400" ||
                                     x.Extended_customs_procedure == "7100") &&
                                    // x.WarehouseError == null && 
                                    (x.Cancelled == null || x.Cancelled == false) &&
                                    x.DoNotAllocate != true)
                        .OrderByDescending(x => x.AssessmentDate)
                        .FirstOrDefault(x => clst.Contains(x.CNumber));
                    return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void MatchToAsycudaItem(AdjustmentDetail os, List<AsycudaDocumentItem> alst, EntryDataDetail ed, AdjustmentQSContext ctx)
        {
            //ed.Status = null;
            //ed.QtyAllocated = 0;
            var remainingShortQty = (os.InvoiceQty.GetValueOrDefault() - os.ReceivedQty.GetValueOrDefault());

            if (!alst.Any())
            {
                // took out status comment to allow Allocations to try allocate adjustment if it fails here
              //  ed.Status = "No Asycuda Item Found";
                return;
            }

            /// took PiQuantity out because then entries can exist already and this just preventing it from relinking
            /// 

            

            if (alst.Select(x => x.ItemQuantity.GetValueOrDefault()  - x.DFQtyAllocated.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {
                // took out status comment to allow Allocations to try allocate adjustment if it fails here
                // ed.Status = "Insufficent Quantities";
                return;
            }

            ed.EffectiveDate = alst.FirstOrDefault().AsycudaDocument.AssessmentDate;
            if (remainingShortQty < 0) return;// if overs just mark and return
            foreach (var aItem in alst)
            {
                var pitm = ctx.xcuda_Item.First(x => x.Item_Id == aItem.Item_Id);
                //pitm.DFQtyAllocated = 0;
                var asycudaQty =
                    Convert.ToDouble(aItem.ItemQuantity.GetValueOrDefault() - pitm.DFQtyAllocated);

                if (asycudaQty <= 0) continue;

                    var osa = new AsycudaSalesAllocation(true)
                {
                    PreviousItem_Id = aItem.Item_Id,
                    EntryDataDetailsId = os.EntryDataDetailsId,
                    Status = "Short Shipped",
                    TrackingState = TrackingState.Added,

                };
                ctx.AsycudaSalesAllocations.Add(osa);
                if (asycudaQty >= remainingShortQty)
                {

                    osa.QtyAllocated = remainingShortQty;
                    ed.QtyAllocated += remainingShortQty;
                    pitm.DFQtyAllocated += remainingShortQty;
                    aItem.DFQtyAllocated += remainingShortQty;
                    break;
                }
                else
                {

                    osa.QtyAllocated = asycudaQty;
                    ed.QtyAllocated += asycudaQty;
                    pitm.DFQtyAllocated += asycudaQty;
                    aItem.DFQtyAllocated += asycudaQty;
                    remainingShortQty -= asycudaQty;
                }

            }

            
        }


        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumber(string cNumber, string itemNumber)
        {
           
                try
                {

                
                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.AsycudaDocument.CNumber != null && x.ItemNumber == itemNumber && cNumber.Contains(x.AsycudaDocument.CNumber));
                var res = aItem.ToList();
                var alias = ItemAliasCache.Where (x => x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => x.AsycudaDocument.CNumber != null && alias.Contains(x.ItemNumber) && cNumber.Contains(x.AsycudaDocument.CNumber)).ToList();
                if (ae.Any())res.AddRange(ae);
                


                return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
           

        }

        public List<InventoryItemAliasX> ItemAliasCache
        {
            get
            {
                if(_itemAliaCache != null) return _itemAliaCache;
                using (var ctx = new CoreEntitiesContext())
                {
                    _itemAliaCache = ctx.InventoryItemAliasX
                        .Include(x => x.InventoryItemsEx)
                        .Where(x => x.ApplicationSettingsId ==BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                }

                return _itemAliaCache;
            }
        }

        private List<AsycudaDocumentItem> AsycudaDocumentItemCache
        {
            get
            {
                if (_itemCache != null) return _itemCache;
                using (var ctx = new CoreEntitiesContext())
                {
                    _itemCache = ctx.AsycudaDocumentItems
                        .Include(x => x.AsycudaDocument)
                        .Include(x => x.AsycudaDocumentItemEntryDataDetails)
                        .Where(x => x.AsycudaDocument.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                    (x.AsycudaDocument.Extended_customs_procedure == "7000" || x.AsycudaDocument.Extended_customs_procedure == "7400" || x.AsycudaDocument.Extended_customs_procedure == "7100") &&
                                    // x.WarehouseError == null && 
                                    (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
                                    x.AsycudaDocument.DoNotAllocate != true).ToList();
                }

                return _itemCache;
            }
            
        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumberReference(int applicationSettingsId,
              string cNumber, string itemNumber)
        {
            
                try
                {
                    var doc = AsycudaDocumentItemCache.FirstOrDefault(x => cNumber.Contains(x.CNumber));
                    if(doc == null) return new List<AsycudaDocumentItem>();
                    var docref = doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal) > 0
                        ? doc.ReferenceNumber.Substring(0, doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal))
                        : doc.ReferenceNumber;

                    
                    var aItem = AsycudaDocumentItemCache
                        .Where(x => x.ItemNumber == itemNumber && x.AsycudaDocument.ReferenceNumber.Contains(docref));
                    var res = aItem.ToList();
                    var alias = ItemAliasCache.Where(x => x.ApplicationSettingsId == applicationSettingsId && x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                    if (!alias.Any()) return res;

                    var ae = AsycudaDocumentItemCache
                        .Where(x => alias.Contains(x.ItemNumber) && x.AsycudaDocument.ReferenceNumber.Contains(docref)).ToList();
                    if (ae.Any()) res.AddRange(ae);



                    return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            

        }
    }
}

