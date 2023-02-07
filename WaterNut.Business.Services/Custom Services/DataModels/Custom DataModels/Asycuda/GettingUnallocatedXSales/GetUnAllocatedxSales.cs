using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingUnallocatedXSales
{
    public class GetUnAllocatedxSales : IGetUnAllocatedxSalesProcessor
    {
        public List<PreAllocations> Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            using (var ctx = new AllocationDSContext() { StartTracking = false })
            {
                return ctx.XSales_UnAllocated
                    .AsNoTracking()
                    .Join(itemSetLst.Select(z => z.InventoryItemId).ToList(), x => x.InventoryItemId, i => i, (x,i) => x)
                    .ToList()
                    .Select(x => new PreAllocations() {EntryDataDetailsId = x.EntryDataDetailsId,PItemId = x.pItemId,XItemId = x.xItemId,QtyAllocated = x.SalesQuantity ?? 0.0,DutyFreePaid = x.DutyFreePaid})
                    .ToList();
            }
        }
    }
}