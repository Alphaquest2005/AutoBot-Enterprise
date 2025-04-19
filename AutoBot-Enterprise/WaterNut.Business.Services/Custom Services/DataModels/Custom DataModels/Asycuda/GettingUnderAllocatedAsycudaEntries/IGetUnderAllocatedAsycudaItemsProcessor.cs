using System.Collections.Generic;

namespace WaterNut.DataSpace
{
    public interface IGetUnderAllocatedAsycudaItemsProcessor
    {
        List<int> Execute(List<(string ItemNumber, int InventoryItemId)> itemList);
    }
}