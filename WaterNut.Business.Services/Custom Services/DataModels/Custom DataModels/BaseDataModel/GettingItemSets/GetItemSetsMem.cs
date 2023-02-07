using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets
{
    public class GetItemSetsMem : IGetItemSetsProcessor
    {
        static readonly object Identity = new object();

        private static
            ConcurrentDictionary<(string ItemNumber, int InventoryItemId),
                List<(string ItemNumber, int InventoryItemId)>> _itemSets = null;

        static GetItemSetsMem()
        {
            lock (Identity)
            {
                using (var ctx = new AllocationDSContext() { StartTracking = false })
                {
                    var lst = ctx.InventoryItems
                        .AsNoTracking()
                        .Include(x => x.InventoryItemAliasEx)
                        .Where(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance
                            .CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .Where(x => x.ItemNumber.Substring(3, 1) == "/")
                        .ToList()
                        .GroupBy(x => (ItemNumber: x.ItemNumber.ToUpper().Trim(), x.InventoryItemId))
                        .Select(g =>
                        {
                            return new KeyValuePair<(string ItemNumber, int InventoryItemId),
                                List<(string ItemNumber, int InventoryItemId)>>(g.Key,
                                new List<(string ItemNumber, int InventoryItemId)>(g.SelectMany(x =>
                                {
                                    var res = new List<(string ItemNumber, int InventoryItemId)>
                                        { (x.ItemNumber.ToUpper().Trim(), x.InventoryItemId) };
                                    res.AddRange(x.InventoryItemAliasEx
                                        .Select(a => (a.AliasName.ToUpper().Trim(), a.AliasItemId)).ToList());
                                    return res;
                                })));
                        })
                        .ToDictionary(x => x.Key, x => x.Value);
                    _itemSets =
                        new ConcurrentDictionary<(string ItemNumber, int InventoryItemId),
                            List<(string ItemNumber, int InventoryItemId)>>(lst);
                }
            }
        }



        public List<List<(string ItemNumber, int InventoryItemId)>> Execute(string lst)
        {
            if (lst == null)
            {
                return _itemSets
                    .Select(x => x.Value)
                    .ToList();
            }
            else
            {
                var res = lst.ToUpper().Trim().Split(',').ToList();
                return _itemSets
                    .Join(res, x => x.Key.ItemNumber, i => i, (x, i) => x)
                    .Select(x => x.Value)
                    .ToList();
            }

        }
    }
}