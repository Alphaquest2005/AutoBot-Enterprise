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
using CoreEntities.Business.Enums;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.DataSpace;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using MoreLinq;
using WaterNut.Business.Entities;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;


namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentShortService
    {
        private List<AsycudaDocumentItem> _itemCache;
        private List<InventoryItemAliasX> _itemAliaCache;

        public async Task AutoMatch(int applicationSettingsId)
        {



            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 0;



                var lst = ctx.AdjustmentDetails
                    .Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => x.SystemDocumentSet != null)
                    // .Where(x => x.ItemNumber == "HEL/003361001")
                    //.Where(x => x.EntryDataId == "120902")
                    //.Where( x => x.EntryDataDetailsId == 545303)
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .OrderBy(x => x.EntryDataDetailsId)
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();

                await DoAutoMatch(applicationSettingsId, lst, ctx);
            }

        }


        public async Task AutoMatchDocSet(int applicationSettingsId, int docSetId)
        {



            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 0;



                var lst = ctx.AdjustmentDetails
                    .Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => x.AsycudaDocumentSetId == docSetId)
                    //.Where(x => x.SystemDocumentSet != null)
                    // .Where(x => x.ItemNumber == "HEL/003361001")
                    //.Where(x => x.EntryDataId == "120902")
                    //.Where( x => x.EntryDataDetailsId == 545303)
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .OrderBy(x => x.EntryDataDetailsId)
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();

                await DoAutoMatch(applicationSettingsId, lst, ctx);
            }

        }

        private async Task DoAutoMatch(int applicationSettingsId, List<AdjustmentDetail> lst, AdjustmentQSContext ctx)
        {
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
                        List<AsycudaDocumentItem> aItem;
                        if (s.InvoiceQty > 0)
                        {
                            aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                                    s.PreviousInvoiceNumber, s.EntryDataId, s.ItemNumber)
                                .ConfigureAwait(false);
                            if (aItem.Any()) MatchToAsycudaItem(s, aItem, ed, ctx);
                        }
                        else
                        {
                            aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                                    s.PreviousInvoiceNumber, s.EntryDataId)
                                .ConfigureAwait(false);
                            if (aItem.Any()) MatchToAsycudaDocument(aItem.First().AsycudaDocument, ed);
                        }

                        if (!aItem.Any())
                        {
                            tryCNumber = true;
                        }
                    }

                    if ((tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber)) &&
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
                        MatchToAsycudaDocument(asycudaDocument, ed);
                    }

                    if (ed.EffectiveDate == null)
                    {
                        //Set Overs 1st and Shorts to Last of Month
                        //Try match effective date if i find the invoice if not leave it blank

                        // if (ed.EffectiveDate == null) ed.EffectiveDate = s.AdjustmentEx.InvoiceDate;
                        if (s.Type == "DIS")
                            ed.EffectiveDate = new EntryDataDSContext().EntryData.OfType<PurchaseOrders>().FirstOrDefault(x =>
                                x.EntryDataId == ed.EntryDataId ||
                                ed.PreviousInvoiceNumber.Contains(x.EntryDataId))?.EntryDataDate;
                        else
                        {
                            ed.EffectiveDate = s.AdjustmentEx.InvoiceDate;
                        }
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
                        ed.LastCost = (double)lastItemCost.LocalItemCost.GetValueOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            ctx.SaveChanges();
            StatusModel.StopStatusUpdate();
        }


        private static void MatchToAsycudaDocument(AsycudaDocument asycudaDocument, EntryDataDetail ed)
        {
            if (asycudaDocument != null)
            {
                ed.EffectiveDate = asycudaDocument.AssessmentDate;
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    //PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = asycudaDocument.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                });
            }
            else
            {
                ed.Status = "No Asycuda Entry Found";
            }
        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
    string sPreviousInvoiceNumber, string sEntryDataId)
        {
            try
            {

                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.ApplicationSettingsId == applicationSettingsId &&
                                (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                 || sEntryDataId.ToUpper().Trim().Contains(x.PreviousInvoiceNumber.ToUpper().Trim())));
                var res = aItem.ToList();


                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
            string sPreviousInvoiceNumber, string sEntryDataId, string sItemNumber)
        {
            try
            {

                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim() &&
                                (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                 || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())));
                var res = aItem.ToList();
                var alias = ItemAliasCache
                    .Where(x => x.InventoryItemsEx.ApplicationSettingsId == applicationSettingsId &&
                                x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim())
                    .Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null
                                && alias.Contains(x.ItemNumber)
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
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

        /// <summary>
        /// /////////////////// Create one entry fro kim

        public async Task CreateIM9(string filterExpression, bool perInvoice,
            int asycudaDocumentSetId, string dutyFreePaid, string adjustmentType, List<int> entryDataDetailsIds, string emailId)
        {
            try
            {
                CreateEx9Class.DocSetPi.Clear();
                var docSet =
                    await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                        .ConfigureAwait(false);

                var cp =
                    BaseDataModel.Instance.Customs_Procedures
                        .Single(x => x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Discrepancy == true);

                docSet.Customs_Procedure = cp;


                var exPro =
                    $" && PreviousDocumentItem.AsycudaDocument.ApplicationSettingsId = {docSet.ApplicationSettingsId}" +
                    //"&& PreviousDocumentItem.AsycudaDocument.SystemDocumentSet == null" +
                    $" && (PreviousDocumentItem.AsycudaDocument.CustomsOperationId == {(int)CustomsOperations.Warehouse})";
                var slst =
                    (await CreateAllocationDataBlocks(perInvoice, filterExpression + exPro, entryDataDetailsIds, adjustmentType).ConfigureAwait(false))
                    .Where(
                        x => x.Allocations.Count > 0).ToList();
                if (slst.Any())
                {
                    // must use the month cuz assessment date could be 1 or 30
                    var startDate =
                        new DateTime(slst.SelectMany(x => x.Allocations).Min(x => x.EffectiveDate).GetValueOrDefault().Year,
                            slst.SelectMany(x => x.Allocations).Min(x => x.EffectiveDate).GetValueOrDefault().Month, 1);
                    var endDate = new DateTime(slst.SelectMany(x => x.Allocations).Max(x => x.EffectiveDate).GetValueOrDefault().Year,
                        slst.SelectMany(x => x.Allocations).Max(x => x.EffectiveDate).GetValueOrDefault().Month, 1).AddMonths(1).AddDays(-1);


                    var itemPiSummarylst =
                        CreateEx9Class.Instance.GetItemSalesPiSummary(docSet.ApplicationSettingsId, startDate,
                            endDate, dutyFreePaid, adjustmentType);
                    List<DocumentCT> doclst;
                    if (adjustmentType == "DIS")
                    {

                        doclst = await CreateEx9Class.Instance.CreateDutyFreePaidDocument(dutyFreePaid,
                                slst, docSet, adjustmentType, false, itemPiSummarylst, false,
                                false, false, "Current", false, false, true, perInvoice, false, false, "S") //ex9bucket = false because sales affect current the piquantity
                            .ConfigureAwait(
                                false);
                    }
                    else
                    {
                        doclst = await CreateEx9Class.Instance.CreateDutyFreePaidDocument(dutyFreePaid,
                                 slst, docSet, adjustmentType, false, itemPiSummarylst, true,
                                 false, false, "Historic", true, true, true, perInvoice, false, true, "S")
                             .ConfigureAwait(
                                 false);
                    }
                    
                    BaseDataModel.StripAttachments(doclst, emailId);
                    BaseDataModel.AttachEmailPDF(asycudaDocumentSetId, emailId);
                    BaseDataModel.SetInvoicePerline(doclst.Select(x => x.Document.ASYCUDA_Id).ToList());
                    BaseDataModel.RenameDuplicateDocumentCodes(doclst.Select(x => x.Document.ASYCUDA_Id).ToList());

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }




        }




        private async Task<IEnumerable<AllocationDataBlock>> CreateAllocationDataBlocks(bool perInvoice, string filterExpression, List<int> entryDataDetailsIds, string adjustmentType)
        {
            try
            {
                StatusModel.Timer("Getting IM9 Data");
                var slstSource = await GetIM9Data(filterExpression, entryDataDetailsIds, adjustmentType).ConfigureAwait(false);
                StatusModel.StartStatusUpdate("Creating IM9 Entries", slstSource.Count());
                IEnumerable<AllocationDataBlock> slst;
                slst = CreateWholeAllocationDataBlocks(perInvoice, slstSource);
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(bool perInvoice,
            IEnumerable<EX9Allocations> slstSource)
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

        private IEnumerable<AllocationDataBlock> CreateNonPerInvoiceAllocationDataBlocks(IEnumerable<EX9Allocations> slstSource)
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

        private IEnumerable<AllocationDataBlock> CreatePerInvoiceAllocationDataBlocks(IEnumerable<EX9Allocations> slstSource)
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

        private async Task<List<EX9Allocations>> GetIM9Data(string FilterExpression, List<int> entryDataDetailsIds, string adjustmentType)
        {
            FilterExpression =
                FilterExpression.Replace("&& (pExpiryDate >= \"" + DateTime.Now.Date.ToShortDateString() + "\")", "");
            var docsetid = new CoreEntitiesContext().FileTypes.First(x => x.Type == adjustmentType).AsycudaDocumentSetId;
            FilterExpression =
                Regex.Replace(FilterExpression, "AsycudaDocumentSetId == \"\\d+\"", $"AsycudaDocumentSetId == \"{docsetid}\"");

            FilterExpression += "&& (PreviousDocumentItem.DoNotAllocate != true && PreviousDocumentItem.DoNotEX != true && PreviousDocumentItem.WarehouseError == null)";



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
            var res = new List<EX9Allocations>();
            using (var ctx = new AllocationDSContext())
            {
                ctx.Database.CommandTimeout = (60 * 30);
                try
                {
                    IQueryable<AdjustmentShortAllocations> pres;
                    if (FilterExpression.Contains("xBond_Item_Id == 0"))
                    {
                        //TODO: use expirydate in asycuda document
                        pres = ctx.AdjustmentShortAllocations

                            .OrderBy(x => x.AllocationId)
                            .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                            .Where(x => x.xBond_Item_Id == 0);
                    }
                    else
                    {
                        pres = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                            .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null);

                        //var pres1 = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                        //     .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                        //     .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null).ToList();

                    }

                    entryDataDetailsIds = entryDataDetailsIds ?? new List<int>();

                    res = pres
                        .Where(exp)
                        .Where(x => x.Status == null)//|| x.Status == "Short Shipped"
                        .Where(x => x.xBond_Item_Id == 0 //&& x.pQuantity != 0
                            )
                        // .Where(x => !entryDataDetailsIds.Any()|| entryDataDetailsIds.Contains(x.EntryDataDetailsId))//entryDataDetailsIds.Any(z => z == x.EntryDataDetailsId)

                        .GroupJoin(ctx.xcuda_Weight_itm, x => x.PreviousItem_Id, q => q.Valuation_item_Id,
                            (x, w) => new { x, w })
                        // took this out to allow creating entries on is manually assessed entries 
                        //.Where(g => g.x.pCNumber != null) 
                        .Where(g => g.w.Any())
                        .Select(c => new EX9Allocations
                        {
                            AllocationId = c.x.AllocationId,
                            EntryData_Id = c.x.EntryDataDetails.EntryData_Id,
                            Commercial_Description =
                                c.x.PreviousDocumentItem == null
                                    ? null
                                    : c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            DutyFreePaid = c.x.DutyFreePaid,
                            EntryDataDetailsId = (int)c.x.EntryDataDetailsId,
                            InvoiceDate = c.x.InvoiceDate,
                            EffectiveDate = (DateTime)c.x.EffectiveDate,
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
                            Comment = c.x.EntryDataDetails.Comment,
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
                            InventoryItemId = c.x.EntryDataDetails.InventoryItemId,
                            // Net_weight_itm = c.x.PreviousDocumentItem != null ? ctx.xcuda_Weight_itm.FirstOrDefault(q => q.Valuation_item_Id == x.PreviousItem_Id).Net_weight_itm: 0,
                            previousItems = c.x.PreviousDocumentItem.EntryPreviousItems
                                    .Select(y => y.xcuda_PreviousItem)
                                    .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) && y.xcuda_Item.AsycudaDocument.Cancelled != true)
                                    .Select(z => new previousItems()
                                    {
                                        PreviousItem_Id = z.PreviousItem_Id,
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
                                    (x.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Warehouse) &&
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

        public async Task MatchToAsycudaItem(int entryDataDetailId, int itemId)
        {
            try
            {
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    var os = ctx.AdjustmentDetails.First(x => x.EntryDataDetailsId == entryDataDetailId);
                    var ed = ctx.EntryDataDetails.First(x => x.EntryDataDetailsId == entryDataDetailId);
                    var item = AsycudaDocumentItemCache.FirstOrDefault(x => x.Item_Id == itemId);
                    MatchToAsycudaItem(os, new List<AsycudaDocumentItem>() { item }, ed, ctx);
                    ed.Status = null;
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId, string ex9Type, string dutyFreePaid)
        {
            await CreateIM9(filterExpression, perInvoice, asycudaDocumentSetId, dutyFreePaid,
                ex9Type, null, null).ConfigureAwait(false);
        }

        private void MatchToAsycudaItem(AdjustmentDetail os, List<AsycudaDocumentItem> alst, EntryDataDetail ed, AdjustmentQSContext ctx)
        {
            //ed.Status = null;
            //ed.QtyAllocated = 0;
            var remainingShortQty = (os.InvoiceQty.GetValueOrDefault() - os.ReceivedQty.GetValueOrDefault());

            if (!alst.Any())
            {
                //Mark here cuz Allocations don't do Discrepancies only Adjustments
                ed.Status = "No Asycuda Item Found";
                return;
            }

            /// took PiQuantity out because then entries can exist already and this just preventing it from relinking
            /// 

            ed.EffectiveDate = alst.FirstOrDefault().AsycudaDocument.AssessmentDate;

            if (alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.DFQtyAllocated.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error
                ed.Status = "Insufficent Quantities";
                return;
            }


            if (remainingShortQty < 0)
            {
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = alst.First().AsycudaDocumentId.GetValueOrDefault(),
                    TrackingState = TrackingState.Added

                });
                return;
            } // if overs just mark and return

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
                    //Status = "Short Shipped",
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
                    .Where(x => x.AsycudaDocument.CNumber != null
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.ItemNumber == itemNumber && cNumber.Contains(x.AsycudaDocument.CNumber));
                var res = aItem.ToList();
                var alias = ItemAliasCache.Where(x => x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => x.AsycudaDocument.CNumber != null
                                && x.AsycudaDocument.DocumentType == "IM7"
                                && x.AsycudaDocument.ImportComplete == true
                                && alias.Contains(x.ItemNumber) && cNumber.Contains(x.AsycudaDocument.CNumber)).ToList();
                if (ae.Any()) res.AddRange(ae);



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
                if (_itemAliaCache != null) return _itemAliaCache;
                using (var ctx = new CoreEntitiesContext())
                {
                    _itemAliaCache = ctx.InventoryItemAliasX
                        .Include(x => x.InventoryItemsEx)
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                }

                return _itemAliaCache;
            }
        }

        private List<AsycudaDocumentItem> AsycudaDocumentItemCache
        {
            get
            {
                try
                {
                    if (_itemCache != null) return _itemCache;
                    using (var ctx = new CoreEntitiesContext())
                    {
                        ctx.Database.CommandTimeout = 60;
                        _itemCache = ctx.AsycudaDocumentItems
                            .Include(x => x.AsycudaDocument)
                            .Include(x => x.AsycudaDocumentItemEntryDataDetails)
                            .Where(x => x.AsycudaDocument.ApplicationSettingsId == BaseDataModel.Instance
                                            .CurrentApplicationSettings.ApplicationSettingsId)
                            .Where(x => (x.AsycudaDocument.CNumber != null ||
                                         x.AsycudaDocument.IsManuallyAssessed == true) &&
                                        (x.AsycudaDocument.Extended_customs_procedure == "7000" ||
                                         x.AsycudaDocument.Extended_customs_procedure == "7400" ||
                                         x.AsycudaDocument.Extended_customs_procedure == "7100") &&
                                        // x.WarehouseError == null && 
                                        (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
                                        x.AsycudaDocument.DoNotAllocate != true).ToList();
                    }

                    return _itemCache;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumberReference(int applicationSettingsId,
              string cNumber, string itemNumber)
        {

            try
            {
                var doc = AsycudaDocumentItemCache.FirstOrDefault(x => x.CNumber != null && cNumber.Contains(x.CNumber));
                if (doc == null) return new List<AsycudaDocumentItem>();
                var docref = doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal) > 0
                    ? doc.ReferenceNumber.Substring(0, doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal))
                    : doc.ReferenceNumber;


                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.ItemNumber == itemNumber
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref));
                var res = aItem.ToList();
                var alias = ItemAliasCache.Where(x => x.ApplicationSettingsId == applicationSettingsId && x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => alias.Contains(x.ItemNumber)
                                && x.AsycudaDocument.DocumentType == "IM7"
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref)).ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public async Task ProcessDISErrorsForAllocation(int applicationSettingsId)
        {
            // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 0;



                var lst = ctx.TODO_PreDiscrepancyErrors
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    //.Where(x => x.ItemNumber == "HEL/003361001")
                    .OrderBy(x => x.EntryDataDetailsId)
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();
                // looking for 'INT/YBA473/3GL'

                ProcessDISErrorsForAllocation(lst, ctx);
            }
        }

        public async Task ProcessDISErrorsForAllocation(int applicationSettingsId, string res)
        {
            // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 0;



                var lst = ctx.TODO_PreDiscrepancyErrors
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                && res.Contains(x.EntryDataDetailsId.ToString()))
                    //.Where(x => x.ItemNumber == "HEL/003361001")
                    .OrderBy(x => x.EntryDataDetailsId)
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();
                // looking for 'INT/YBA473/3GL'

                ProcessDISErrorsForAllocation(lst, ctx);
            }
        }

        private static void ProcessDISErrorsForAllocation(List<TODO_PreDiscrepancyErrors> lst, AdjustmentQSContext ctx)
        {
            StatusModel.StartStatusUpdate("Preparing Discrepancy Errors for Re-Allocation", lst.Count());
            foreach (var s in lst)
            {
                var ed = ctx.EntryDataDetails.First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                ed.EffectiveDate = BaseDataModel.CurrentSalesInfo().Item2;
                ed.QtyAllocated = 0;
                ed.Comment = $@"DISERROR:{s.Status}";
                ed.Status = null;
                ctx.Database.ExecuteSqlCommand(
                    $"delete from AsycudaSalesAllocations where EntryDataDetailsId = {ed.EntryDataDetailsId}");
            }

            ctx.SaveChanges();
        }
    }
}

