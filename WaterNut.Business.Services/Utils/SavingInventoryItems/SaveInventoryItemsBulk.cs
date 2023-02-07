using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;

namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public class SaveInventoryItemsBulk: ISaveInventoryItemsProcessor
    {
        public void Execute(List<InventoryDataItem> processedInventoryItems)
        {
          //var res =  processedInventoryItems
          //      .Where(x =>
          //          x.Item.TrackingState != TrackingState.Unchanged)
          //      .Select(x => x.Item.ChangeTracker.GetChanges().FirstOrDefault())
          //      .ToList();
           new InventoryDSContext().BulkMerge(processedInventoryItems.Select(x => x.Item).ToList());
           processedInventoryItems.ForEach(x => x.Item.AcceptChanges());
        }
    }
}