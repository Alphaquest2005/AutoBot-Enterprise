using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class AddInventorySource : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;
 
        public AddInventorySource(FileTypes fileType)
        {
            _fileType = fileType;
            
        }
 
        public Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data, ILogger log)
        {
            var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
 
            var inventoryDataItems = data
                //Becareful not to add filtering operations that unintentionally reduce the return values
                .Select(x =>
                {
                    if(x.Item.InventoryItemSources.All(x => x.InventorySourceId != inventorySource.Id))
                      x.Item.InventoryItemSources.Add(InventorySourceProcessor.CreateItemSource(inventorySource, x.Item));
 
                    return x;
                })
                .ToList();
            return Task.FromResult(Task.FromResult(new Result<List<InventoryDataItem>>(inventoryDataItems,true,"")).Result); // Wrap in Task.FromResult
        }
    }
}