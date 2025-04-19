using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class GetXcudaInventoryItems : IGetXcudaInventoryItemsProcessor
    {
        public List<xcuda_Inventory_Item> Execute(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;
                var lst = ctx.xcuda_Inventory_Item.AsNoTracking()
                    .Join(itemList.Select(z => z.InventoryItemId).ToList(),
                        x => x.InventoryItemId,
                        i => i,
                        (x, i) => x)
                    .ToList();
                return lst;
            }
        }
    }
}