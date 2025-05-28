using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using MoreLinq.Extensions;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class SaveInventoryCodes : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;
 
        public SaveInventoryCodes(FileTypes fileType)
        {
            _fileType = fileType;
            
        }
 
        public async Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data, ILogger log)
        {
            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
            var tasks = data
                .Select(x => (DataItem: x, Code: InventoryCodesProcessor.GetInventoryItemCodes(x.Data, x.Item)))
                .Select(async x =>
                {
                    await InventoryCodesProcessor.SaveInventoryCodes(inventorySource, x.Code, x.DataItem.Item, log).ConfigureAwait(false);
                }).ToList();
 
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return new Result<List<InventoryDataItem>>(data, true, "");
        }
    }
}