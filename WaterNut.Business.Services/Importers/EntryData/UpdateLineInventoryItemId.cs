using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class UpdateLineInventoryItemId : IProcessor<InventoryDataItem>
    {
        public Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data, ILogger log)
        {
            var inventoryDataItems = data.Select(x =>
                {
                    x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id);
                    return x;
                })
                .ToList();
            return Task.FromResult(Task.FromResult(new Result<List<InventoryDataItem>>(inventoryDataItems, true, "")).Result); // Wrap in Task.FromResult
        }
    }
}