using System.Collections.Generic;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public interface IGetXcudaInventoryItemsProcessor
    {
        List<xcuda_Inventory_Item> Execute(List<(string ItemNumber, int InventoryItemId)> itemList);
    }
}