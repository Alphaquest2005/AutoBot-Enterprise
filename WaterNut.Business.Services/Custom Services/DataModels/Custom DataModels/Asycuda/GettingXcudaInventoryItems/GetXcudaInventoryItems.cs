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
                        dbItem => dbItem.InventoryItemId, // From xcuda_Inventory_Item table
                        inputItemId => inputItemId,       // From in-memory itemList
                        (dbItem, inputItemId) => new xcuda_Inventory_Item { // Project to new objects
                            Item_Id = dbItem.Item_Id,
                            InventoryItemId = dbItem.InventoryItemId
                            // Add any other properties of xcuda_Inventory_Item that are ABSOLUTELY essential
                            // for the object's validity or for other potential callers not found.
                        })
                    .ToList();
                return lst;
            }
        }
    }
}