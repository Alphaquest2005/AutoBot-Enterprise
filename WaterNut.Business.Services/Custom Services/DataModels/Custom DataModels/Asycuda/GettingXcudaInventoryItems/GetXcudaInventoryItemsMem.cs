using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using static WaterNut.DataSpace.BuildSalesReportClass;

namespace WaterNut.DataSpace
{
    public class GetXcudaInventoryItemsMem : IGetXcudaInventoryItemsProcessor
    {
        static readonly object Identity = new object();
        private static ConcurrentDictionary<(int Item_Id, int InventoryItemId), xcuda_Inventory_Item> _xcudaInventoryItems = null;
        public GetXcudaInventoryItemsMem()
        {
            lock (Identity)
            {
                if (_xcudaInventoryItems != null) return;
                using (var ctx = new AllocationDSContext { StartTracking = false })
                {
                    ctx.Database.CommandTimeout = 0;
                    var lst = ctx.xcuda_Inventory_Item.AsNoTracking()
                        .ToDictionary(x => (x.Item_Id, x.InventoryItemId), x => x);
                    _xcudaInventoryItems = new ConcurrentDictionary<(int Item_Id, int InventoryItemId), xcuda_Inventory_Item>(lst);
                }
            }
        }
        public List<xcuda_Inventory_Item> Execute(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            
                var lst = _xcudaInventoryItems
                    .Join(itemList.Select(z => z.InventoryItemId).ToList(),
                        x => x.Key.InventoryItemId,
                        i => i,
                        (x, i) => x)
                    .Select(x => x.Value)
                    .ToList();
                return lst;
            
        }
    }
}