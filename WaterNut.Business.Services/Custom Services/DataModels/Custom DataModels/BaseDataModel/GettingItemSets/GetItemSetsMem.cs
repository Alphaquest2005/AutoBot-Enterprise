using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using static WaterNut.DataSpace.AllocationsBaseModel;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets
{
    public class GetItemSetsMem : IGetItemSetsProcessor
    {
        static readonly object Identity = new object();

        private static
            Dictionary<(string ItemNumber, int InventoryItemId),
                List<(string ItemNumber, int InventoryItemId)>> _itemSets = null;

        public GetItemSetsMem()
        {
            lock (Identity)
            {
                if (_itemSets == null)
                    using (var ctx = new AllocationDSContext() { StartTracking = false })
                    {
                        var groups = ctx.InventoryItems
                            .AsNoTracking()
                            .Include(x => x.InventoryItemAliasEx)
                            .Where(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance
                                .CurrentApplicationSettings.ApplicationSettingsId)
                            .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                            ///.Where(x => x.ItemNumber.Substring(3, 1) == "/")
                            .ToList()
                            .GroupBy(x => (ItemNumber: x.ItemNumber.ToUpper().Trim(), x.InventoryItemId)).ToList();

                        var t = groups.Where(z => z.Key.ItemNumber == "CRB/HIF-SR290BL").ToList();

                        var lst1 = groups.Select(g =>
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
                        }).ToList();

                         var t2 = lst1.Where(z => z.Key.ItemNumber == "CRB/HIF-SR290BL").ToList();


                        var lst = lst1
                            .Where(x => x.Value.All(z => z.InventoryItemId >= x.Key.InventoryItemId))
                            .ToDictionary(x => x.Key, x => x.Value);
                        _itemSets =
                            new Dictionary<(string ItemNumber, int InventoryItemId),
                                List<(string ItemNumber, int InventoryItemId)>>(lst);

                        var t3 = _itemSets.Where(z => z.Key.ItemNumber == "CRB/HIF-SR290BL").ToList();
                    }

            }
        }



        public List<List<(string ItemNumber, int InventoryItemId)>> Execute(string lst)
        {
            if (string.IsNullOrEmpty(lst))
            {
               return _itemSets
                    .Select(x => x.Value)
                    .ToList();
            }
            else
            {
                var res = lst.ToUpper().Trim().Split(',').ToList();

                var t = _itemSets.Where(z => z.Key.InventoryItemId == 1706).ToList();

                return _itemSets
                    .Join(res, x => x.Key.ItemNumber, i => i, (x, i) => x)
                    .Select(x => x.Value)
                    .ToList();
            }

        }
    }
}