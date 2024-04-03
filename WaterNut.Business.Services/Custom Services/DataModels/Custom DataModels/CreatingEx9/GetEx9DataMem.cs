using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9DataByDateRange;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9
{
    public class GetEx9DataMem
    {
        private readonly GetEx9DataByDateRangeMem _getEx9DataByDateRangeMem;

        public GetEx9DataMem(string filterExp, string rdateFilter)
        {
            _getEx9DataByDateRangeMem = new GetEx9DataByDateRangeMem(filterExp, rdateFilter);
        }


        public async Task<List<EX9Allocations>> Execute(string filterExpression, string dateFilter)
        {
            try
            {
                
                var data = await _getEx9DataByDateRangeMem.Execute(dateFilter).ConfigureAwait(false);

                return CreateEx9Allocations(filterExpression, data.AsQueryable());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

      

        private List<EX9Allocations> CreateEx9Allocations(string filterExpression, IQueryable<EX9AsycudaSalesAllocations> tres)
        {
            var res = new List<EX9Allocations>();
            filterExpression = CreateFilterExpression(filterExpression);

            //var test = tres.Where(x => x.AllocationId == 4597648).ToList();

            var rres = tres
                .Where(filterExpression)
                .ToList();
            foreach (var x in rres)
            {
                try
                {
                    var allocations = CreateEx9Allocations(x);
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

        private string CreateFilterExpression(string filterExpression)
        {
            if ((DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ExportExpiredEntries ?? true))
                filterExpression =
                    filterExpression.Replace(
                        $"&& (pExpiryDate >= \"{DateTime.Now.Date.ToShortDateString()}\" || pExpiryDate == null)",
                        "");
            filterExpression =
                filterExpression.Replace("TariffCode.Contains", "TariffCode != null && TariffCode.Contains");

            return filterExpression += "&& DoNotAllocateSales != true " +
                                       "&& DoNotAllocatePreviousEntry != true " +
                                       "&& DoNotEX != true " +
                                       "&& Status == null " + // force no error execution
                                       //"&& AllocationErrors == null" +
                                       "&& WarehouseError == null " +
                                       $"&& (CustomsOperationId == {(int)CustomsOperations.Warehouse})";
        }

        private EX9Allocations CreateEx9Allocations(EX9AsycudaSalesAllocations x)
        {
            var allocations = new EX9Allocations
            {
                Type = x.Type,
                AllocationId = x.AllocationId,
                EntryData_Id = (int) x.EntryData_Id,
                DutyFreePaid = x.DutyFreePaid,
                EntryDataDetailsId = x.EntryDataDetailsId,
                InvoiceDate = (DateTime) (x.EffectiveDate == null || x.EffectiveDate == DateTime.MinValue
                    ? x.InvoiceDate
                    : x.EffectiveDate.Value),
                EffectiveDate = x.EffectiveDate,
                InvoiceNo = x.InvoiceNo,
                SourceFile = x.SourceFile,
                ItemDescription = x.ItemDescription,
                ItemNumber = x.ItemNumber,
                pCNumber = x.pCNumber,
                pItemCost = x.pItemCost,
                Status = x.Status,
                EntryDataDetails = x.EntryDataDetails,
                PreviousItem_Id = x.PreviousItem_Id,
                QtyAllocated = x.QtyAllocated,
                SalesFactor = x.SalesFactor,
                SalesQtyAllocated = x.SalesQtyAllocated,
                SalesQuantity = x.SalesQuantity,
                pItemNumber = x.pItemNumber,
                pItemDescription = x.Commercial_Description,
                pTariffCode = x.pTariffCode,
                pPrecision1 = x.pPrecision1,
                DFQtyAllocated = x.DFQtyAllocated,
                DPQtyAllocated = x.DPQtyAllocated,
                pLineNumber = x.pLineNumber,
                LineNumber = x.SalesLineNumber,
                Comment = x.Comment,
                Customs_clearance_office_code = x.Customs_clearance_office_code,
                pQuantity = x.pQuantity,
                pRegistrationDate = x.pRegistrationDate,
                pAssessmentDate = x.AssessmentDate,
                CustomsProcedure = x.CustomsProcedure,
                pExpiryDate = (DateTime) x.pExpiryDate.GetValueOrDefault(),
                Country_of_origin_code = x.Country_of_origin_code,
                Total_CIF_itm = x.Total_CIF_itm,
                Net_weight_itm = x.Net_weight_itm,
                InventoryItemId = x.InventoryItemId ,
                PIData = x.AsycudaSalesAllocationsPIData,
                previousItems = x?.PreviousDocumentItem.EntryPreviousItems
                    .Select(y => y.xcuda_PreviousItem)
                    .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null ||
                                 y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true)
                                && y.xcuda_Item.AsycudaDocument.Cancelled != true)
                    .Select(z => new PreviousItems()
                    {
                        PreviousItem_Id = z.PreviousItem_Id,
                        DutyFreePaid =
                            z.xcuda_Item.AsycudaDocument.Customs_Procedure.CustomsOperationId ==
                            (int) CustomsOperations.Exwarehouse &&
                            z.xcuda_Item.AsycudaDocument.Customs_Procedure.IsPaid == true
                                ? "Duty Paid"
                                : "Duty Free",
                        Net_weight = (double) z.Net_weight,
                        Suplementary_Quantity = (double) z.Suplementary_Quantity
                    }).ToList(),
                TariffSupUnitLkps = x.PreviousDocumentItem?.xcuda_Tarification.xcuda_HScode.TariffCodes == null
                    ? new List<TariffSupUnitLkps>()
                    : x.PreviousDocumentItem?.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory
                        ?.TariffCategoryCodeSuppUnit?.Select(z => z.TariffSupUnitLkps)
                        .ToList() //.Select(x => (ITariffSupUnitLkp)x)
            };
            return allocations;
        }
    }
}