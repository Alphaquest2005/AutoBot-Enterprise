using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveInventoryItems : IProcessor<InventoryDataItem>
    {
        public Result<List<InventoryDataItem>> Execute(List<InventoryDataItem> data)
        {
            new Utils.SavingInventoryItems.SaveInventoryItemsSelector().Execute(data);
            return new Result<List<InventoryDataItem>>(data, true, "") ;
        }
    }
}