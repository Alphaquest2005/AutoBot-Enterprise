using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class UpdateItemTariffCode : IProcessor<InventoryDataItem>
    {
        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            var inventoryDataItems = data.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.TariffCode))
                .Select(x =>
                {
                    x.Item.TariffCode = x.Data.Key.TariffCode;
                    return x;
                })
                .ToList();
            return new Result<List<InventoryDataItem>>(inventoryDataItems, true, "") ;
        }
    }
}