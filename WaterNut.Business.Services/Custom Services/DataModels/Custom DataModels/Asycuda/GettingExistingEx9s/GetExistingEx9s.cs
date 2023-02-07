using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class GetExistingEx9s : IGetExistingEx9Processor
    {
        public List<PreAllocations> Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            using (var ctx = new AllocationDSContext() { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;
                return  ctx.ExistingAllocations.AsNoTracking()
                    .Join(itemSetLst.Select(z => z.InventoryItemId).ToList(), x => x.InventoryItemId, i => i, (x,i) => x)
                    .ToList()
                    .Select(x => new PreAllocations() {EntryDataDetailsId = x.EntryDataDetailsId,PItemId = x.pItemId,XItemId = x.xItemId,QtyAllocated = (x.xQuantity <= x.SalesQuantity ? x.xQuantity : x.SalesQuantity) ?? 0,DutyFreePaid = x.DutyFreePaid})
                    .ToList();
            }
        }
    }
}