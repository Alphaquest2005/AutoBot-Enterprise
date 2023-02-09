using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets
{
    public class GetItemSets : IGetItemSetsProcessor
    {
        

        static GetItemSets()
        {
        }

        public GetItemSets()
        {
            
        }

        public List<List<(string ItemNumber, int InventoryItemId)>> Execute(string lst)
        {
            using (var ctx = new AllocationDSContext() { StartTracking = false })
            {
                return ctx.InventoryItems
                    .AsNoTracking()
                    .Include(x => x.InventoryItemAliasEx)
                    .Where(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                    .Where(x => x.ItemNumber.Substring(3, 1) == "/")
                    .Where(x => lst == null || lst.ToUpper().Trim().Contains(x.ItemNumber.ToUpper().Trim()))
                    .ToList()
                    .Select(x =>
                    {
                        var res = new List<(string ItemNumber, int InventoryItemId)> { (x.ItemNumber.ToUpper().Trim(), x.InventoryItemId) };
                        res.AddRange(x.InventoryItemAliasEx.Select(a => (a.AliasName.ToUpper().Trim(), a.AliasItemId)).ToList());
                        return res;
                    })
                    .ToList();

            }
        }
    }
}