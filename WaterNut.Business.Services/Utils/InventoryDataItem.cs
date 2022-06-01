using InventoryDS.Business.Entities;

namespace WaterNut.Business.Services.Utils
{
    public class InventoryDataItem
    {
        public InventoryData Data { get; }
        public InventoryItem Item { get; }

        public InventoryDataItem(InventoryData item, InventoryItem inventoryItem)
        {
            Data = item;
            Item = inventoryItem;
            
        }
    }
}