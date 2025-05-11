using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class UpdateItemDescription : IProcessor<InventoryDataItem>
    {
        public async Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data)
        {
            var inventoryDataItems = data
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .Select(x =>
                {
                    x.Item.Description = x.Data.Key.ItemDescription;
                    return x;
                })
                .ToList();
            return Task.FromResult(new Result<List<InventoryDataItem>>(inventoryDataItems, true, "")).Result; // Wrap in Task.FromResult
        }
    }
}