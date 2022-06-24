using InventoryDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace
{
    public class InventorySourceProcessor
    {
        public static InventoryItemSource CreateItemSource(InventorySource inventorySource, InventoryItem i)
        {
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                var inventoryItemSource = new InventoryItemSource(true)
                {
                    InventorySourceId = inventorySource.Id,
                    TrackingState = TrackingState.Added,
                    InventoryId = i.Id,
                };
                ctx.InventoryItemSources.Add(inventoryItemSource);
                ctx.SaveChanges();
                return inventoryItemSource;
            }
        }
    }
}