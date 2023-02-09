using System;
using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;

namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public class SaveInventoryItemsBulk: ISaveInventoryItemsProcessor
    {
        public void Execute(List<InventoryDataItem> processedInventoryItems)
        {
            try
            {
                var inventoryItems = processedInventoryItems.Select(x => x.Item).ToList();
                var newItems = inventoryItems.Where(x => x.Id == 0).ToList();

                var existingItems = inventoryItems.Where(x => x.Id != 0).DistinctBy(x => x.Id).ToList();

                new InventoryDSContext().BulkInsert(newItems);
                new InventoryDSContext().BulkMerge(existingItems);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}