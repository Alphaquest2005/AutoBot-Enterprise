using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class UpdateItemTariffCode : IProcessor<InventoryDataItem>
    {
        public Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data, ILogger log)
        {
            var inventoryDataItems = data
                //.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode) took this out because it reduces the return data along the pipeline
                //.Where(x => !string.IsNullOrEmpty(x.Data.Key.TariffCode))
                .Select(x =>
                {
                    if(x.Item.TariffCode != x.Data.Key.TariffCode)
                    if (!string.IsNullOrEmpty(x.Data.Key.TariffCode))
                    {
                        x.Item.TariffCode = x.Data.Key.TariffCode;
                    }
                    
                    return x;
                })
                .ToList();
       
            return Task.FromResult(Task.FromResult(new Result<List<InventoryDataItem>>(inventoryDataItems, true, "")).Result); // Wrap in Task.FromResult
        }
    }
}