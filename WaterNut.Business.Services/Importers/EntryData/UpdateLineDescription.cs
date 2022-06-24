using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class UpdateLineDescription : IProcessor<InventoryDataItem>
    {
        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            var inventoryDataItems = data.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .Select(x =>
                {
                    x.Data.Data.ForEach(z => z.ItemDescription = x.Item.Description);
                    return x;
                })
                .ToList();
            return new Result<List<InventoryDataItem>>(inventoryDataItems, true, "");
        }
    }
}