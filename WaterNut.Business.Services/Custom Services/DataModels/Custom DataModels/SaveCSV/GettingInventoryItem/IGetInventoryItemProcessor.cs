using InventoryDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.GettingInventoryItem
{
    public interface IGetInventoryItemProcessor
    {
        InventoryItem Execute( string itemNumber);
    }
}