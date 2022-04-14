using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;
using Core.Common.Converters;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using xcuda_ItemService = DocumentItemDS.Business.Services.xcuda_ItemService;

namespace WaterNut.DataSpace
{
    public class AllocationsModel
    {
        // private readonly CreateIncompOPSClass _createIncompOpsClass = new CreateIncompOPSClass();
        //private readonly BuildSalesReportClass _buildSalesReportClass = new BuildSalesReportClass();


        static AllocationsModel()
        {
            Instance = new AllocationsModel();
        }

        public static AllocationsModel Instance { get; }


   

        public CreateEx9Class CreateEX9Class => CreateEx9Class.Instance;

        public CreateOPSClass CreateOpsClassClass { get; } = new CreateOPSClass();


        //TODO: Refactor this
        private string CleanText(string p)
        {
            p = Regex.Replace(p, @"[\-0]+", "");
            return p;
        }


        public void AddDutyFreePaidtoRef(DocumentCT cdoc, string dfp, AsycudaDocumentSet docSet)
        {
            switch (dfp)
            {
                case "Duty Free":

                    cdoc.Document.xcuda_Declarant.Number = docSet.Declarant_Reference_Number
                                                           + "-F" + //+ "-DF"
                                                           cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber;
                    break;
                case "Duty Paid":
                    cdoc.Document.xcuda_Declarant.Number = docSet.Declarant_Reference_Number
                                                           + "-P" + // + "-DP"
                                                           cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber;
                    break;
            }
        }

        public async Task<IEnumerable<AsycudaSalesAllocations>> GetAsycudaSalesAllocations(string FilterExpression)
        {
            // create dictionary map and run replace
            var exp = TranslateAllocationWhereExpression(FilterExpression);


            using (var ctx = new AsycudaSalesAllocationsService())
            {
                var res = await ctx.GetAsycudaSalesAllocationsByExpression(exp,
                    new List<string>
                    {
                        "xBondAllocations",
                        // "EntryDataDetails",
                         "EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx",
                        // "EntryDataDetails.InventoryItem",
                        //"EntryDataDetails.InventoryItem.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps",
                        "EntryDataDetails.Sales",
                        // "PreviousDocumentItem",
                        //"PreviousDocumentItem.EX",
                        "PreviousDocumentItem.xcuda_Goods_description",
                        "PreviousDocumentItem.xcuda_Tarification.xcuda_HScode",
                        "PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit",
                        "PreviousDocumentItem.xcuda_Taxation.xcuda_Taxation_line",
                        "PreviousDocumentItem.xcuda_Valuation_item.xcuda_Item_Invoice",
                        "PreviousDocumentItem.xcuda_Valuation_item.xcuda_Weight_itm",
                        "PreviousDocumentItem.AsycudaDocument",
                        //"PreviousDocumentItem.xcuda_PreviousItems",
                        "PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument"
                        // "PreviousDocumentItem.xcuda_PreviousItems.xcuda_Item"
                    }).ConfigureAwait(false);

                return res;
            }
        }


        public string TranslateAllocationWhereExpression(string FilterExpression)
        {
            var map = new Dictionary<string, string>
            {
                {"InvoiceDate", "EntryDataDetails.Sales.EntryDataDate"},
                {"InvoiceNo", "EntryDataDetails.EntryDataId"},
                {"SalesQtyAllocated", "EntryDataDetails.QtyAllocated"},
                {"SalesQuantity", "EntryDataDetails.Quantity"},
                {"Cost", "EntryDataDetails.Cost"},
                {"TaxAmount", "EntryDataDetails.TaxAmount"},
                {"ItemNumber", "EntryDataDetails.ItemNumber"},
                {"ItemDescription", "EntryDataDetails.ItemDescription"},
                {"TariffCode", "EntryDataDetails.InventoryItem.TariffCode"},
                {"pCNumber", "PreviousDocumentItem.AsycudaDocument.pCNumber"},
                {"pLineNumber", "PreviousDocumentItem.LineNumber"},
                {"PreviousItem_Id == 0", "PreviousItem_Id == null"},
                {"(ApplicationSettingsId", "(EntryDataDetails.Sales.ApplicationSettingsId"},
            };


            var exp = map.Aggregate(FilterExpression, (current, m) => current.Replace(m.Key, m.Value));
            return exp;
        }


        //System.Windows.Forms.MessageBox.


        public async Task ManuallyAllocate(AsycudaSalesAllocations currentAsycudaSalesAllocation,
            xcuda_Item PreviousItemEx)
        {
            double aqty;
            using (var ctx = new AllocationDSContext {StartTracking = true})
            {
                var entryDataDetails =
                    ctx.EntryDataDetails.Include(x => x.Sales).Include(x => x.Adjustments)
                        .First(
                            x => x.EntryDataDetailsId == currentAsycudaSalesAllocation.EntryDataDetailsId.Value);

                var asycudaItem = ctx.xcuda_Item.Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                    .First(x => x.Item_Id == PreviousItemEx.Item_Id);
                ctx.AsycudaSalesAllocations.Attach(currentAsycudaSalesAllocation);

                if (entryDataDetails.Quantity >=
                    asycudaItem.ItemQuantity - asycudaItem.QtyAllocated)
                    aqty = asycudaItem.ItemQuantity - asycudaItem.QtyAllocated;
                else
                    aqty = entryDataDetails.Quantity;
                currentAsycudaSalesAllocation.PreviousItem_Id = asycudaItem.Item_Id;

                currentAsycudaSalesAllocation.QtyAllocated = aqty;
                entryDataDetails.QtyAllocated += aqty;
                if (entryDataDetails.Sales == null && entryDataDetails.Adjustments != null ||
                    entryDataDetails.DutyFreePaid == "Duty Free")
                    asycudaItem.DFQtyAllocated += aqty;
                else
                    asycudaItem.DPQtyAllocated += aqty;

                currentAsycudaSalesAllocation.Status = "Manual Allocation";

                ctx.SaveChanges();
                currentAsycudaSalesAllocation.AcceptChanges();
            }
            // SaveAsycudaSalesAllocation(currentAsycudaSalesAllocation);
        }

        
        public async Task ClearAllocations(string filterExpression)
        {
            var lst = await GetAsycudaSalesAllocations(filterExpression).ConfigureAwait(false);
            await ClearAllocations(lst).ConfigureAwait(false);
        }

        public async Task ClearAllAllocations(int appSettingsId)
        {
            try
            {
                StatusModel.Timer("Clear All Existing Allocations");

                using (var ctx = new AllocationDSContext())
                {
                    await ctx.Database.ExecuteSqlCommandAsync(TransactionalBehavior.EnsureTransaction,
                        $@"
                            DELETE FROM AdjustmentOversAllocations
                            FROM    AdjustmentOversAllocations INNER JOIN
                                             EntryDataDetails ON AdjustmentOversAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId
                            WHERE (EntryData.ApplicationSettingsId = {appSettingsId})

                            DELETE FROM AsycudaSalesAllocations
                            FROM    AsycudaSalesAllocations INNER JOIN
                                             EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId
                            WHERE (EntryData.ApplicationSettingsId = {appSettingsId})

                            UPDATE xcuda_Item
                            SET         DFQtyAllocated = 0, DPQtyAllocated = 0
                            FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                                             AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                                             xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id
                            WHERE (AsycudaDocumentSet.ApplicationSettingsId = {appSettingsId})


                            UPDATE EntryDataDetails
                            SET         QtyAllocated = 0, Status = NULL--, EffectiveDate = NULL
                            FROM    EntryDataDetails INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId
                            WHERE (EntryData.ApplicationSettingsId = {appSettingsId})

                            UPDATE xcuda_PreviousItem
                            SET         QtyAllocated = 0
                            FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                                             AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                                             xcuda_PreviousItem ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_PreviousItem.ASYCUDA_Id
                            WHERE (AsycudaDocumentSet.ApplicationSettingsId = {appSettingsId})

                            UPDATE SubItems
                            SET         QtyAllocated = 0
                            FROM    xcuda_Item INNER JOIN
                                             SubItems ON xcuda_Item.Item_Id = SubItems.Item_Id INNER JOIN
                                             AsycudaDocumentSet INNER JOIN
                                             xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentSet.AsycudaDocumentSetId = xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ON 
                                             xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
                            WHERE (AsycudaDocumentSet.ApplicationSettingsId = {appSettingsId})").ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public async Task ClearDocSetAllocations(string lst)
        {
            try
            {
                StatusModel.Timer("Clear DocSet Existing Allocations");

                using (var ctx = new AllocationDSContext())
                {
                    var sql = $@" DELETE FROM AdjustmentOversAllocations
							FROM    AdjustmentOversAllocations INNER JOIN
											 EntryDataDetails ON AdjustmentOversAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
											 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId 
                            WHERE (EntryDataDetails.ItemNumber in ({lst}))

                            DELETE FROM AsycudaSalesAllocations
                            FROM    AsycudaSalesAllocations INNER JOIN
                                             EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId 
                            WHERE (EntryDataDetails.ItemNumber in ({lst}))

                            UPDATE xcuda_Item
                            SET         DFQtyAllocated = 0, DPQtyAllocated = 0
                            FROM    xcuda_Item INNER JOIN
                                             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                                                 (SELECT InventoryItemsWithAlias.ItemNumber
                                                  FROM     InventoryItems INNER JOIN
                                                                   InventoryItemsWithAlias ON InventoryItems.Id = InventoryItemsWithAlias.Id
                                                  WHERE  (InventoryItems.ItemNumber IN ({lst}))) AS items ON xcuda_HScode.Precision_4 = items.ItemNumber


                            UPDATE EntryDataDetails
                            SET         QtyAllocated = 0, Status = NULL--, EffectiveDate = NULL
                            FROM    EntryDataDetails INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId 
                            WHERE (EntryDataDetails.ItemNumber in ({lst}))


                            UPDATE xcuda_PreviousItem
                            SET         QtyAllocated = 0
                            FROM  xcuda_PreviousItem inner join  xcuda_Item on xcuda_Item.Item_Id = xcuda_PreviousItem.PreviousItem_Id INNER JOIN
                                             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                                                 (SELECT InventoryItemsWithAlias.ItemNumber
                                                  FROM     InventoryItems INNER JOIN
                                                                   InventoryItemsWithAlias ON InventoryItems.Id = InventoryItemsWithAlias.Id
                                                  WHERE  (InventoryItems.ItemNumber IN ({lst}))) AS items ON xcuda_HScode.Precision_4 = items.ItemNumber

                            UPDATE SubItems
                            SET         QtyAllocated = 0
                            FROM    xcuda_Item INNER JOIN
                                             SubItems ON xcuda_Item.Item_Id = SubItems.Item_Id INNER JOIN
                                             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                                                 (SELECT InventoryItemsWithAlias.ItemNumber
                                                  FROM     InventoryItems INNER JOIN
                                                                   InventoryItemsWithAlias ON InventoryItems.Id = InventoryItemsWithAlias.Id
                                                  WHERE  (InventoryItems.ItemNumber IN ({lst}))) AS items ON xcuda_HScode.Precision_4 = items.ItemNumber";
                    await ctx.Database.ExecuteSqlCommandAsync(TransactionalBehavior.EnsureTransaction, sql)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public async Task ClearAllocations(IEnumerable<AsycudaSalesAllocations> alst)
        {
            foreach (var allo in alst)
            {
                await ClearAllocation(allo).ConfigureAwait(false);
                StatusModel.StatusUpdate();
            }


            var neAllo = alst.Where(x => x.EntryDataDetails == null && x.PreviousDocumentItem != null).ToList();
            StatusModel.StartStatusUpdate("Deleting Null EntryData Allocations", neAllo.Count());

            foreach (var allo in neAllo)
            {
                await ClearAllocation(allo).ConfigureAwait(false);
                StatusModel.StatusUpdate();
            }

            StatusModel.Timer("ReLoading Data..");


            StatusModel.StopStatusUpdate();
        }


        public void Send2Excel(List<AsycudaSalesAllocations> lst)
        {
            var s = new ExportToCSV<AllocationsExcelLine, List<AllocationsExcelLine>>
            {
                dataToPrint = (from sa in lst
                    let sales = sa.EntryDataDetails.Sales
                    let prevEntry = sa.PreviousDocumentItem
                    select new AllocationsExcelLine
                    {
                        DutyFreePaid = sa.EntryDataDetails.DutyFreePaid,
                        InvoiceNo = sales?.INVNumber,
                        InvoiceDate = sales == null ? DateTime.MinValue : sales.EntryDataDate,
                        SalesAllocationNo = sales == null ? 0 : Convert.ToInt32(sa.SANumber),
                        SalesQty = sales == null ? 0 : Convert.ToDouble(sa.EntryDataDetails.Quantity),
                        SalesQtyAllocated = sales == null ? 0 : Convert.ToDouble(sa.EntryDataDetails.QtyAllocated),
                        ItemNumber = sales == null ? null : sa.EntryDataDetails.ItemNumber,
                        ItemDescription = sales == null ? null : sa.EntryDataDetails.ItemDescription,
                        UnitCost = sales == null ? 0 : Convert.ToDouble(sa.EntryDataDetails.Cost),
                        SalesValue = sales == null
                            ? 0
                            : Convert.ToDouble(sa.EntryDataDetails.Cost) * Convert.ToDouble(sa.EntryDataDetails.Quantity),
                        AllocatedValue = sales == null
                            ? 0
                            : Convert.ToDouble(sa.EntryDataDetails.Cost) * Convert.ToDouble(sa.QtyAllocated),
                        TariffCode = prevEntry?.TariffCode,
                        CIF = prevEntry == null
                            ? 0
                            : sa.PreviousDocumentItem.xcuda_Valuation_item.Total_CIF_itm /
                              Convert.ToDouble(sa.PreviousDocumentItem.ItemQuantity),
                        DutyLiability = prevEntry == null ? 0 : Convert.ToDouble(sa.PreviousDocumentItem.DutyLiability),
                        ItemQuantity = prevEntry == null ? 0 : Convert.ToDouble(sa.PreviousDocumentItem.ItemQuantity),
                        AllocatedQty = sales == null ? 0 : Convert.ToDouble(sa.QtyAllocated),
                        PreviousLineNumber = prevEntry == null ? 0 : sa.PreviousDocumentItem.LineNumber,
                        PreviousCNumber = prevEntry == null ? null : sa.PreviousDocumentItem.AsycudaDocument.CNumber,
                        PreviousRegDate = prevEntry == null
                            ? DateTime.MinValue
                            : Convert.ToDateTime(sa.PreviousDocumentItem.AsycudaDocument.RegistrationDate),
                        AllocationStatus = sa.Status,
                        PiQuantity = prevEntry == null
                            ? 0
                            : Convert.ToDouble(sa.PreviousDocumentItem.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
                                .Sum(x => x.Suplementary_Quantity)),
                        DutyFreePi =
                            (double) (prevEntry == null ||
                                      sa.PreviousDocumentItem.EntryPreviousItems.Select(p => p.xcuda_PreviousItem).Any() ==
                                      false
                                ? 0
                                : sa.PreviousDocumentItem.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
                                    .Where(x => x.DutyFreePaid == "Duty Free").Sum(x => x.Suplementary_Quantity)),
                        DutyPaidPi =
                            (double) (prevEntry == null ||
                                      sa.PreviousDocumentItem.EntryPreviousItems.Select(p => p.xcuda_PreviousItem).Any() ==
                                      false
                                ? 0
                                : sa.PreviousDocumentItem.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
                                    .Where(x => x.DutyFreePaid == "Duty Paid").Sum(x => x.Suplementary_Quantity)),
                        DFQtyAllocated = prevEntry == null ? 0 : Convert.ToDouble(sa.PreviousDocumentItem.DFQtyAllocated),
                        DPQtyAllocated = prevEntry == null ? 0 : Convert.ToDouble(sa.PreviousDocumentItem.DPQtyAllocated),
                        Ex9Doc = sa.xBondEntry == null
                            ? ""
                            : BaseDataModel.Instance.GetDocument(sa.xBondEntry.ASYCUDA_Id).Result.ReferenceNumber,
                        Ex9DocLine = sa.xBondEntry == null ? 0 : sa.xBondEntry.LineNumber
                    }).ToList()
            };
            s.GenerateReport();
        }

        //public CreateErrOPS CreateErrOps
        //{
        //    get { return CreateErrOPS.Instance; }
        //}

        //public CreateIncompOPSClass CreateIncompOpsClass
        //{
        //    get { return _createIncompOpsClass; }
        //}

        public async Task ClearAllocation(AsycudaSalesAllocations allo)
        {
            /////////// took out entrydatadetails update

            if (allo.EntryDataDetails != null && allo.EntryDataDetails.TrackingState != TrackingState.Deleted)
            {
                allo.EntryDataDetails.QtyAllocated = 0;
                allo.EntryDataDetails = null;
            }

            if (allo.PreviousDocumentItem != null)
            {
                using (var ctx = new xcuda_ItemService())
                {
                    var res = await ctx.Getxcuda_ItemByKey(allo.PreviousItem_Id.ToString()).ConfigureAwait(false);
                    res.DFQtyAllocated = 0;
                    res.DPQtyAllocated = 0;

                    foreach (var sitm in res.SubItems) sitm.QtyAllocated = 0;

                    foreach (var ed in res.xcuda_PreviousItems.Select(x => x.xcuda_PreviousItem)) ed.QtyAllocated = 0;
                    await ctx.Updatexcuda_Item(res).ConfigureAwait(false);
                }

                allo.PreviousDocumentItem = null;
            }


            using (var ctx = new AsycudaSalesAllocationsService())
            {
                await ctx.DeleteAsycudaSalesAllocations(allo.AllocationId.ToString()).ConfigureAwait(false);
            }
        }


        public class MyPodData
        {
            public List<AsycudaSalesAllocations> Allocations { get; set; }
            public AlloEntryLineData EntlnData { get; set; }
        }


        public class AllocationsExcelLine
        {
            public string DutyFreePaid { get; set; }
            public string InvoiceNo { get; set; }
            public DateTime InvoiceDate { get; set; }
            public int SalesAllocationNo { get; set; }
            public double SalesQty { get; set; }
            public double SalesQtyAllocated { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public double UnitCost { get; set; }
            public double SalesValue { get; set; }
            public double AllocatedValue { get; set; }
            public string TariffCode { get; set; }
            public double CIF { get; set; }
            public double DutyLiability { get; set; }
            public double ItemQuantity { get; set; }
            public double AllocatedQty { get; set; }
            public int PreviousLineNumber { get; set; }
            public string PreviousCNumber { get; set; }
            public DateTime PreviousRegDate { get; set; }
            public string AllocationStatus { get; set; }
            public double PiQuantity { get; set; }
            public double DFQtyAllocated { get; set; }
            public double DutyFreePi { get; set; }
            public double DPQtyAllocated { get; set; }
            public double DutyPaidPi { get; set; }
            public string Ex9Doc { get; set; }
            public int Ex9DocLine { get; set; }
        }


        public class AlloEntryLineData : BaseDataModel.IEntryLineData
        {
            // public IDocumentItem PreviousDocumentItem { get; set; }
            public IInventoryItem InventoryItem { get; set; }
            // public InventoryItem InventoryItem { get; set; }

            public EX9AsycudaSalesAllocations EX9Allocation { get; set; }
            public string MonthYear { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double Cost { get; set; }
            public int PreviousDocumentItemId { get; set; }
            public double Quantity { get; set; }
            public List<EntryDataDetailSummary> EntryDataDetails { get; set; }
            public double Weight { get; set; }
            public double InternalFreight { get; set; }
            public double Freight { get; set; }
            public List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }
            public DateTime EntryDataDate { get; set; }
            public int InventoryItemId { get; set; }
        }
    }
}