using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using MoreLinq;
using static sun.awt.SunHints;
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
                            .Include(x => x.InventoryItemAliasEx_NoReverseMappings)
                            .Where(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance
                                .CurrentApplicationSettings.ApplicationSettingsId)
                            .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                            ///.Where(x => x.ItemNumber.Substring(3, 1) == "/")
                            .ToList()
                            .GroupBy(x => (ItemNumber: x.ItemNumber.ToUpper().Trim(), x.InventoryItemId)).ToList();

                        //var dupitemsets1 = groups.Where(x => x.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
                        //var t = groups.Where(z => z.Key.ItemNumber == "MMM/62556752301").ToList();

                        var lst1 = groups.Select(g =>
                        {
                            return new KeyValuePair<(string ItemNumber, int InventoryItemId),
                                List<(string ItemNumber, int InventoryItemId)>>(g.Key,
                                new List<(string ItemNumber, int InventoryItemId)>(g.SelectMany(x =>
                                {
                                    var res = new List<(string ItemNumber, int InventoryItemId)>
                                        { (x.ItemNumber.ToUpper().Trim(), x.InventoryItemId) };
                                    res.AddRange(x.InventoryItemAliasEx_NoReverseMappings
                                        .Select(a => (a.AliasName.ToUpper().Trim(), a.AliasItemId)).ToList());
                                    return res;
                                })));
                        }).OrderBy(x => x.Key.InventoryItemId).ToList();

                        //var dupitemsets2 = lst1.Where(x => x.Value.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
                        //var t2 = lst1.Where(z => z.Key.ItemNumber == "MMM/62556752301").ToList();

                        var clst = GroupLst(lst1);

                        var lst = clst
                            .Where(x => x.Value.All(z => z.InventoryItemId >= x.Key.InventoryItemId))
                            .ToDictionary(x => x.Key, x => x.Value);
                        _itemSets =
                            new Dictionary<(string ItemNumber, int InventoryItemId),
                                List<(string ItemNumber, int InventoryItemId)>>(lst);

                        //var dupitemsets3 = _itemSets.Where(x => x.Value.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
                        //var t3 = _itemSets.Where(z => z.Key.ItemNumber == "MMM/62556752301").ToList();
                    }

            }
        }

        private List<KeyValuePair<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>> GroupLst(List<KeyValuePair<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>> lst1)
        {
            var res = new List<KeyValuePair<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>>();
            // var numlst = new List<int>() { 10436, 57399, 54940, 48631, 48630 };
            var slut = lst1
                //.Where(x => x.Value.Any(z => z.ItemNumber == "MMM/62556752301"))
                // .Where(x => x.Value.Any(z =>numlst.Contains(z.InventoryItemId)))
                .ToList();
            while (slut.Any())
            {
                var itm = slut.First();
                var others = slut.Skip(1)
                    .Where(x => x.Value.Any(z => z.InventoryItemId == itm.Key.InventoryItemId) || x.Value
                        .Select(q => q.InventoryItemId).Intersect(itm.Value.Select(q => q.InventoryItemId)).Any())
                    .ToList();
                var resValue = itm.Value.ToList();
                resValue.AddRange(others.SelectMany(x => x.Value).ToList().Except(resValue).ToList());
                res.Add(new KeyValuePair<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>(itm.Key, resValue));
                slut.Remove(itm);
                slut = slut.Except(others).ToList();

            }

            var rres = res.Where(x => x.Value.Any()).ToList();

            return (res.Count == lst1.Count ? rres : GroupLst(rres));
        }

        //private Dictionary<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>> GroupLst(List<KeyValuePair<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>> inputList)
        //{
        //    var resultList = new Dictionary<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>();
        //    var toProcessList = new List<KeyValuePair<(string ItemNumber, int InventoryItemId), List<(string ItemNumber, int InventoryItemId)>>>(inputList);
        //    var processedList = new HashSet<(string ItemNumber, int InventoryItemId)>();
        //    //var numList = new HashSet<int>() { 10436, 57399, 54940, 48631, 48630 };

        //    while (toProcessList.Any())
        //    {
        //        var item = toProcessList.First();
        //        var others = toProcessList.Skip(1)
        //            .Where(x => x.Value.Any(z => z.InventoryItemId == item.Key.InventoryItemId
        //            //|| numList.Contains(z.InventoryItemId))
        //            || x.Value.Select(q => q.InventoryItemId).Intersect(item.Value.Select(q => q.InventoryItemId)).Any()))
        //            .ToList();
        //        var resultValue = item.Value.ToList();
        //        resultValue.AddRange(others.SelectMany(x => x.Value).ToList().Except(resultValue).ToList());
        //        resultList.Add(item.Key, resultValue);
        //        processedList.Add(item.Key);
        //        toProcessList = toProcessList.Skip(1).Except(others).ToList();
        //    }

        //    // Remove empty values
        //    resultList = resultList.Where(x => x.Value.Any()).ToDictionary(x => x.Key, x => x.Value);

        //    // Process remaining items
        //    if (processedList.Count < inputList.Count)
        //    {
        //        var remainingItems = inputList.Where(x => !processedList.Contains(x.Key)).ToList();
        //        var remainingResult = GroupLst(remainingItems);
        //        foreach (var item in remainingResult)
        //        {
        //            if (!resultList.ContainsKey(item.Key))
        //            {
        //                resultList.Add(item.Key, item.Value);
        //            }
        //        }
        //    }

        //    return resultList;
        //}



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
                var returnItems = new List<List<(string ItemNumber, int InventoryItemId)>>();
                foreach (var itm in res)
                {
                    var items = _itemSets
                        //.Join(res, x => x.Key.ItemNumber, i => i, (x, i) => x)
                        .Where(x => x.Key.ItemNumber == itm)
                        .SelectMany(x => x.Value)
                        .ToList();
                    var reverseAlias = _itemSets
                        .Where(x => x.Key.ItemNumber != itm)
                        .Where(x => x.Value.Any(z => z.ItemNumber == itm))
                        .SelectMany(x => x.Value)
                        .ToList();
                    var comb = items.Union(reverseAlias);
                    returnItems.Add(comb.ToList());
                }

                return returnItems.Where(x => x.Any()).ToList();


            }

        }
    }
}