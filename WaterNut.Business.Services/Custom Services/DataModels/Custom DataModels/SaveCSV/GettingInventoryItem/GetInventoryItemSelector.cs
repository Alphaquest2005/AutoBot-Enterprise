using System.Linq;
using InventoryDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.GettingInventoryItem
{
    public class GetInventoryItemSelector : IGetInventoryItemProcessor
    {
        private bool isDBMem = false;

        public InventoryItem Execute(string itemNumber, string description)
        {
            return isDBMem
                ? new GetInventoryItem().Execute(itemNumber, description)
                : new GetInventoryItemMem().Execute(itemNumber, description);
        }
    }
}