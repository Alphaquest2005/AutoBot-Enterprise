using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public class SaveInventoryItemsSelector: ISaveInventoryItemsProcessor
    {
        private bool isDBMem = false;
 
        public async Task Execute(List<InventoryDataItem> processedInventoryItems)
        {
            if (isDBMem)
                await new SaveInventoryItems().Execute(processedInventoryItems).ConfigureAwait(false);
            else
                await new SaveInventoryItemsBulk().Execute(processedInventoryItems).ConfigureAwait(false);
        }
    }
}