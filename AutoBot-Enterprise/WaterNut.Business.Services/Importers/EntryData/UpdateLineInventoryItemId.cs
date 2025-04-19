using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class UpdateLineInventoryItemId : IProcessor<InventoryDataItem>
    {
        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            var inventoryDataItems = data.Select(x =>
                {
                    x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id);
                    return x;
                })
                .ToList();
            return new Result<List<InventoryDataItem>>(inventoryDataItems, true, "");
        }
    }
}