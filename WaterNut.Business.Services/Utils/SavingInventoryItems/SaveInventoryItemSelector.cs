using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;

namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public class SaveInventoryItemsSelector: ISaveInventoryItemsProcessor
    {
        private bool isDBMem = false;

        public void Execute(List<InventoryDataItem> processedInventoryItems)
        {
            if (isDBMem)
                new SaveInventoryItems().Execute(processedInventoryItems);
            else
                new SaveInventoryItemsBulk().Execute(processedInventoryItems);
        }
    }
}