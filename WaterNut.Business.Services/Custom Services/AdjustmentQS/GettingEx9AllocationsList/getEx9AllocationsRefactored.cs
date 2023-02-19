using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.AdjustmentQS.GettingEx9AllocationsList
{
    public class getEx9AllocationsRefactored : IgetEx9AllocationsListProcessor
    {
        public List<EX9Allocations> Execute(string filterExpression)
        {
            List<EX9Allocations> res;
            using (var ctx = new AllocationDSContext(){StartTracking = false})
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;
                ctx.Database.CommandTimeout = (60 * 30);

                try
                {
                    IQueryable<AdjustmentShort_IM9Data> pres;
                    if (filterExpression.Contains("xBond_Item_Id == 0"))
                    {
                        //TODO: use expirydate in asycuda document
                        pres = ctx.AdjustmentShort_IM9Data
                            .Include(x => x.AsycudaSalesAllocationsPIData)
                            .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
                            .OrderBy(x => x.AllocationId)
                            .Where(x => x.pRegistrationDate == null || x.ExpiryDate > DateTime.Now)
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                            .Where(x => x.xBond_Item_Id == 0);
                    }
                    else
                    {
                        pres = ctx.AdjustmentShort_IM9Data
                            .Include(x => x.AsycudaSalesAllocationsPIData)
                            .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
                            .Where(x => x.pRegistrationDate == null || x.ExpiryDate > DateTime.Now)
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null);

                        //var pres1 = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                        //     .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                        //     .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null).ToList();
                    }

                    // entryDataDetailsIds = entryDataDetailsIds ?? new List<int>();


                    res = pres.AsQueryable()
                        .Where(filterExpression)
                        .Where(x => x.Status == null) //|| x.Status == "Short Shipped"
                        .Where(x => x.xBond_Item_Id == 0)
                        .Select(c => new EX9Allocations
                            {
                                AllocationId = c.AllocationId,
                                EntryData_Id = c.EntryDataDetails.EntryData_Id,
                                DutyFreePaid = c.DutyFreePaid,
                                EntryDataDetailsId = (int)c.EntryDataDetailsId,
                                EntryDataDetails = c.EntryDataDetails,
                                InvoiceDate = c.InvoiceDate,
                                EffectiveDate = (DateTime)c.EffectiveDate,
                                InvoiceNo = c.InvoiceNo,
                                SourceFile = c.SourceFile,
                                ItemDescription = c.ItemDescription,
                                ItemNumber = c.ItemNumber,
                                pCNumber = c.pCNumber,
                                pItemCost = (double)
                                    (c.PreviousDocumentItem.xcuda_Tarification.Item_price /
                                     c.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit
                                         .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity),
                                Status = c.Status,
                                PreviousItem_Id = c.PreviousItem_Id,
                                QtyAllocated = (double)c.QtyAllocated,
                                SalesFactor = c.PreviousDocumentItem.SalesFactor,
                                SalesQtyAllocated = (double)c.SalesQtyAllocated,
                                SalesQuantity = (int)c.SalesQuantity,
                                pItemNumber = c.pItemNumber,
                                pItemDescription = c.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                                pTariffCode = c.pTariffCode,
                                pPrecision1 = c.pPrecision1,
                                DFQtyAllocated = c.PreviousDocumentItem.DFQtyAllocated,
                                DPQtyAllocated = c.PreviousDocumentItem.DPQtyAllocated,
                                LineNumber = (int)c.EntryDataDetails.LineNumber,
                                Comment = c.EntryDataDetails.Comment,
                                pLineNumber = c.PreviousDocumentItem.LineNumber,
                                // Currency don't matter for im9 or exwarehouse
                                Customs_clearance_office_code =
                                    c.PreviousDocumentItem.AsycudaDocument.Customs_clearance_office_code,
                                pQuantity = (double)c.pQuantity,
                                pRegistrationDate = (DateTime)(c.pRegistrationDate ??
                                                               c.PreviousDocumentItem.AsycudaDocument.AssessmentDate),
                                pAssessmentDate = (DateTime)c.PreviousDocumentItem.AsycudaDocument.AssessmentDate,
                                pExpiryDate = (DateTime)c.PreviousDocumentItem.AsycudaDocument.ExpiryDate,
                                Country_of_origin_code =
                                    c.PreviousDocumentItem.xcuda_Goods_description.Country_of_origin_code,
                                Total_CIF_itm = c.PreviousDocumentItem.xcuda_Valuation_item.Total_CIF_itm,
                                Net_weight_itm = c.Net_weight_itm,
                                InventoryItemId = c.EntryDataDetails.InventoryItemId,
                                // Net_weight_itm = c.x.PreviousDocumentItem != null ? ctx.xcuda_Weight_itm.FirstOrDefault(q => q.Valuation_item_Id == x.PreviousItem_Id).Net_weight_itm: 0,
                                PIData = c.AsycudaSalesAllocationsPIData,
                                previousItems = c.PreviousDocumentItem.EntryPreviousItems
                                    .Select(y => y.xcuda_PreviousItem)
                                    .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null ||
                                                 y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) &&
                                                y.xcuda_Item.AsycudaDocument.Cancelled != true)
                                    .Select(z => new PreviousItems()
                                    {
                                        PreviousItem_Id = z.PreviousItem_Id,
                                        DutyFreePaid =
                                            z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4074" ||
                                            z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070" ||
                                            z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4075"
                                                ? "Duty Paid"
                                                : "Duty Free",
                                        Net_weight = (double)z.Net_weight,
                                        Suplementary_Quantity = (double)z.Suplementary_Quantity
                                    }).ToList(),
                                TariffSupUnitLkps =
                                    c.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCodes.TariffCategory
                                        .TariffCategoryCodeSuppUnit.Select(x => x.TariffSupUnitLkps).ToList(),
                                FileTypeId = c.FileTypeId,
                                EmailId = c.EmailId,
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
    }
}