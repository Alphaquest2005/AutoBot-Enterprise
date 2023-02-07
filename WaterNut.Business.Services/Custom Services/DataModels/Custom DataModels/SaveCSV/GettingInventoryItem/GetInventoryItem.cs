using System.Linq;
using InventoryDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.GettingInventoryItem
{
    public class GetInventoryItem : IGetInventoryItemProcessor
    {
        public InventoryItem Execute( string itemNumber)
        {
            return new InventoryDSContext().InventoryItems
                .Include("InventoryItemAlias")
                .First(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.ItemNumber == itemNumber);
        }
    }
}