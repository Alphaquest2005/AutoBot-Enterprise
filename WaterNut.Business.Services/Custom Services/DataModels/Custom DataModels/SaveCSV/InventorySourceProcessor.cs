using System.Collections.Generic;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;

namespace WaterNut.DataSpace
{
    public class InventorySourceProcessor
    {
        public static InventoryItemSource CreateItemSource(InventorySource inventorySource, InventoryItem i)
        {
            var inventoryItemSource = CreateInventoryItemSource(inventorySource, i);
            return SaveInventoryItemSource(inventoryItemSource);
        }

        public static InventoryItemSource SaveInventoryItemSource(InventoryItemSource inventoryItemSource)
        {
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                ctx.InventoryItemSources.Add(inventoryItemSource);
                ctx.SaveChanges();
                return inventoryItemSource;
            }
        }

        public List<InventoryItemSource> SaveInventoryItemSource(List<InventoryItemSource> inventoryItemSource)
        {
            new InventoryDSContext().BulkMerge(inventoryItemSource);
            inventoryItemSource.ForEach(x => x.AcceptChanges());
            return inventoryItemSource;
        }

        public static InventoryItemSource CreateInventoryItemSource(InventorySource inventorySource, InventoryItem i)
        {
            var inventoryItemSource = new InventoryItemSource(true)
            {
                InventorySourceId = inventorySource.Id,
                TrackingState = TrackingState.Added,
                InventoryId = i.Id
            };
            i.InventoryItemSources.Add(inventoryItemSource);
            return inventoryItemSource;
        }
    }
}