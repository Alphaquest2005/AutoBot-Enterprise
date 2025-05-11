using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveInventoryItems : IProcessor<InventoryDataItem>
    {
        public async Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data)
        {
            await new Utils.SavingInventoryItems.SaveInventoryItemsSelector().Execute(data).ConfigureAwait(false);
            return new Result<List<InventoryDataItem>>(data, true, "") ;
        }
    }
}