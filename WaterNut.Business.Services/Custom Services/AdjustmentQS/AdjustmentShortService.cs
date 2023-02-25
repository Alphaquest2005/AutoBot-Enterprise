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
using TrackableEntities.EF6;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Custom_Services.AdjustmentQS.GettingEx9AllocationsList;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;
using WaterNut.Business.Services.Utils.AutoMatching;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;


namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentShortService
    {
        private readonly AutoMatchUtils _autoMatchUtils = new AutoMatchUtils();

        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting)
        {
            await _autoMatchUtils.AutoMatchProcessor.AutoMatch(applicationSettingsId, overwriteExisting, "").ConfigureAwait(false);
        }
        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting, string lst)
        {
            await _autoMatchUtils.AutoMatchProcessor.AutoMatch(applicationSettingsId, overwriteExisting, lst).ConfigureAwait(false);
        }

        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting, List<List<(string ItemNumber, int InventoryItemId)>> itemSets)
        {
            await new AutoMatchSetBasedProcessor().AutoMatch(applicationSettingsId, overwriteExisting, itemSets).ConfigureAwait(false);
        }


        /// <summary>
        /// /////////////////// Create one entry fro kim

        public async Task CreateIM9(string filterExpression, bool perInvoice,
            int asycudaDocumentSetId, string dutyFreePaid, string adjustmentType, string emailId)
        {
            try
            {
               // AllocationsModel.Instance.CreateEx9.DocSetPiClear();
                var docSet =
                    await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                        .ConfigureAwait(false);

                var cp =
                    BaseDataModel.Instance.Customs_Procedures
                        .Single(x => x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Discrepancy == true);

                docSet.Customs_Procedure = cp;


                var exPro =
                    $" && PreviousDocumentItem.AsycudaDocument.ApplicationSettingsId = {docSet.ApplicationSettingsId}" +
                    $" && EntryDataDetails.EntryDataDetailsEx.AsycudaDocumentSetId == {docSet.AsycudaDocumentSetId}" +
                    $" && (PreviousDocumentItem.AsycudaDocument.CustomsOperationId == {(int)CustomsOperations.Warehouse})";
                var slst =
                    (await CreateAllocationDataBlocks(perInvoice, filterExpression + exPro, adjustmentType).ConfigureAwait(false))
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
                        AllocationsModel.Instance.CreateEx9.GetItemSalesPiSummary(startDate,
                            endDate, dutyFreePaid, adjustmentType);
                    List<DocumentCT> doclst;
                    Dictionary<int, List<PreviousItems>> docPreviousItems = new Dictionary<int, List<PreviousItems>>();
                    if (adjustmentType == "DIS")
                    {

                        doclst = await new CreateDutyFreePaidDocument().Execute(dutyFreePaid,
                                slst, docSet, adjustmentType, false, itemPiSummarylst, false,
                                false, false, "Current", false, false, true, perInvoice, false, false, false,docPreviousItems, false,"S") //ex9bucket = false because sales affect current the piquantity
                            .ConfigureAwait(
                                false);
                    }
                    else
                    {
                        doclst = await new CreateDutyFreePaidDocument().Execute(dutyFreePaid,
                                 slst, docSet, adjustmentType, false, itemPiSummarylst, true,
                                 false, false, "Historic", true, true, true, perInvoice, false, true, true, docPreviousItems, false, "S")
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




        private async Task<IEnumerable<AllocationDataBlock>> CreateAllocationDataBlocks(bool perInvoice, string filterExpression, string adjustmentType)
        {
            try
            {
                StatusModel.Timer("Getting IM9 Data");
                var slstSource = await GetIM9Data(filterExpression, adjustmentType).ConfigureAwait(false);
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

        private async Task<List<EX9Allocations>> GetIM9Data(string FilterExpression, string adjustmentType)
        {
            FilterExpression =
                FilterExpression.Replace("&& (pExpiryDate >= \"" + DateTime.Now.Date.ToShortDateString() + "\")", "");
           // var docsetid = new CoreEntitiesContext().FileTypes.First(x => x.FileImporterInfos.EntryType == adjustmentType).AsycudaDocumentSetId;
           // FilterExpression = Regex.Replace(FilterExpression, "AsycudaDocumentSetId == \"\\d+\"", $"AsycudaDocumentSetId == \"{docsetid}\"");
            FilterExpression = Regex.Replace(FilterExpression, " && AsycudaDocumentSetId == \"\\d+\"", $"");

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

            //var oldData = GetEx9AllocationsListOldway(FilterExpression, exp);
            var newData = new getEx9AllocationsList().Execute(exp);

            return newData;

        }

        //private static List<EX9Allocations> GetEx9AllocationsListOldway(string FilterExpression, string exp)
        //{
        //    List<EX9Allocations> res;
        //    using (var ctx = new AllocationDSContext())
        //    {
        //        ctx.Database.CommandTimeout = (60 * 30);
        //        try
        //        {
        //            IQueryable<AdjustmentShortAllocations> pres;
        //            if (FilterExpression.Contains("xBond_Item_Id == 0"))
        //            {
        //                //TODO: use expirydate in asycuda document
        //                pres = ctx.AdjustmentShortAllocations
        //                    .Include(x => x.AsycudaSalesAllocationsPIData)
        //                    .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
        //                    .OrderBy(x => x.AllocationId)
        //                    .Where(x => x.pRegistrationDate == null || x.pExpiryDate > DateTime.Now)
        //                    .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
        //                    .Where(x => x.xBond_Item_Id == 0);
        //            }
        //            else
        //            {
        //                pres = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
        //                    .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
        //                    .Include(x => x.AsycudaSalesAllocationsPIData)
        //                    .Where(x => x.pRegistrationDate == null || x.pExpiryDate > DateTime.Now)
        //                    .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null);

        //                //var pres1 = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
        //                //     .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
        //                //     .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null).ToList();
        //            }

        //            // entryDataDetailsIds = entryDataDetailsIds ?? new List<int>();

        //            res = pres
        //                .Where(exp)
        //                .Where(x => x.Status == null) //|| x.Status == "Short Shipped"
        //                .Where(x => x.xBond_Item_Id == 0 //&& x.pQuantity != 0
        //                )
        //                // .Where(x => !entryDataDetailsIds.Any()|| entryDataDetailsIds.Contains(x.EntryDataDetailsId))//entryDataDetailsIds.Any(z => z == x.EntryDataDetailsId)
        //                .GroupJoin(ctx.xcuda_Weight_itm, x => x.PreviousItem_Id, q => q.Valuation_item_Id,
        //                    (x, w) => new {x, w})
        //                // took this out to allow creating entries on is manually assessed entries 
        //                //.Where(g => g.x.pCNumber != null) 
        //                .Where(g => g.w.Any())
        //                .Select(c => new EX9Allocations
        //                    {
        //                        AllocationId = c.x.AllocationId,
        //                        EntryData_Id = c.x.EntryDataDetails.EntryData_Id,
        //                        DutyFreePaid = c.x.DutyFreePaid,
        //                        EntryDataDetailsId = (int) c.x.EntryDataDetailsId,
        //                        EntryDataDetails = c.x.EntryDataDetails,
        //                        InvoiceDate = c.x.InvoiceDate,
        //                        EffectiveDate = (DateTime) c.x.EffectiveDate,
        //                        InvoiceNo = c.x.InvoiceNo,
        //                        ItemDescription = c.x.ItemDescription,
        //                        ItemNumber = c.x.ItemNumber,
        //                        pCNumber = c.x.pCNumber,
        //                        pItemCost = (double)
        //                            (c.x.PreviousDocumentItem.xcuda_Tarification.Item_price /
        //                             c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit
        //                                 .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity),
        //                        Status = c.x.Status,
        //                        PreviousItem_Id = c.x.PreviousItem_Id,
        //                        QtyAllocated = (double) c.x.QtyAllocated,
        //                        SalesFactor = c.x.PreviousDocumentItem.SalesFactor,
        //                        SalesQtyAllocated = (double) c.x.SalesQtyAllocated,
        //                        SalesQuantity = (int) c.x.SalesQuantity,
        //                        pItemNumber = c.x.pItemNumber,
        //                        pItemDescription = c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
        //                        pTariffCode = c.x.pTariffCode,
        //                        pPrecision1 = c.x.pPrecision1,
        //                        DFQtyAllocated = c.x.PreviousDocumentItem.DFQtyAllocated,
        //                        DPQtyAllocated = c.x.PreviousDocumentItem.DPQtyAllocated,
        //                        LineNumber = (int) c.x.EntryDataDetails.LineNumber,
        //                        Comment = c.x.EntryDataDetails.Comment,
        //                        pLineNumber = c.x.PreviousDocumentItem.LineNumber,
        //                        // Currency don't matter for im9 or exwarehouse
        //                        Customs_clearance_office_code =
        //                            c.x.PreviousDocumentItem.AsycudaDocument.Customs_clearance_office_code,
        //                        pQuantity = (double) c.x.pQuantity,
        //                        pRegistrationDate = (DateTime) (c.x.pRegistrationDate ??
        //                                                        c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate),
        //                        pAssessmentDate = (DateTime) c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate,
        //                        pExpiryDate = (DateTime) c.x.PreviousDocumentItem.AsycudaDocument.ExpiryDate,
        //                        Country_of_origin_code =
        //                            c.x.PreviousDocumentItem.xcuda_Goods_description.Country_of_origin_code,
        //                        Total_CIF_itm = c.x.PreviousDocumentItem.xcuda_Valuation_item.Total_CIF_itm,
        //                        Net_weight_itm = c.w.FirstOrDefault().Net_weight_itm,
        //                        InventoryItemId = c.x.EntryDataDetails.InventoryItemId,
        //                        // Net_weight_itm = c.x.PreviousDocumentItem != null ? ctx.xcuda_Weight_itm.FirstOrDefault(q => q.Valuation_item_Id == x.PreviousItem_Id).Net_weight_itm: 0,
        //                        PIData = c.x.AsycudaSalesAllocationsPIData,
        //                        previousItems = c.x.PreviousDocumentItem.EntryPreviousItems
        //                            .Select(y => y.xcuda_PreviousItem)
        //                            .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null ||
        //                                         y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) &&
        //                                        y.xcuda_Item.AsycudaDocument.Cancelled != true)
        //                            .Select(z => new previousItems()
        //                            {
        //                                PreviousItem_Id = z.PreviousItem_Id,
        //                                DutyFreePaid =
        //                                    z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4074" ||
        //                                    z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070" ||
        //                                    z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4075"
        //                                        ? "Duty Paid"
        //                                        : "Duty Free",
        //                                Net_weight = (double) z.Net_weight,
        //                                Suplementary_Quantity = (double) z.Suplementary_Quantity
        //                            }).ToList(),
        //                        TariffSupUnitLkps =
        //                            c.x.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCodes.TariffCategory
        //                                .TariffCategoryCodeSuppUnit.Select(x => x.TariffSupUnitLkps).ToList(),
        //                        FileTypeId = c.x.FileTypeId,
        //                        EmailId = c.x.EmailId,
        //                        //.Select(x => (ITariffSupUnitLkp)x)
        //                    }
        //                )
        //                //////////// prevent exwarehouse of item whos piQuantity > than AllocatedQuantity//////////
        //                .ToList();

        //            return res;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}


        public async Task MatchToAsycudaItem(int entryDataDetailId, int itemId)
        {
            await _autoMatchUtils.AutoMatchProcessor.MatchToAsycudaItem(entryDataDetailId, itemId).ConfigureAwait(false);
        }

        public async Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId, string ex9Type, string dutyFreePaid)
        {
            //AllocationsModel.Instance.CreateEx9.SetfreashStart(true);
            await CreateIM9(filterExpression, perInvoice, asycudaDocumentSetId, dutyFreePaid,
                ex9Type, null).ConfigureAwait(false);
        }


        public AutoMatchUtils AutoMatchUtils
        {
            get { return _autoMatchUtils; }
        }
    }
}

