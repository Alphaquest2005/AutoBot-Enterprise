using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;

namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public interface ISaveInventoryItemsProcessor
    {
        void Execute(List<InventoryDataItem> processedInventoryItems);
    }
}