using System;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class GetXcudaInventoryItems : IGetXcudaInventoryItemsProcessor
    {
        public List<xcuda_Inventory_Item> Execute(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            if (itemList == null || itemList.Count == 0)
            {
                return new List<xcuda_Inventory_Item>();
            }

            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;
                
                // Extract inventory IDs and remove duplicates for efficiency
                var inventoryIds = itemList.Select(z => z.InventoryItemId).Distinct().ToList();
                
                // Use Contains instead of Join - EF translates this much better
                var lst = ctx.xcuda_Inventory_Item.AsNoTracking()
                    .Where(x => inventoryIds.Contains(x.InventoryItemId))
                    .ToList();
                    
                return lst;
            }
        }
    }
}