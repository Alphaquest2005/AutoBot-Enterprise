using System.Collections.Generic;
using InventoryDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.InventoryProcessing
{
    public interface IInventoryProcessor
    {
        bool Execute(int applicationSettingsId,
            List<InventoryData> inventoryDataList,
            InventorySource inventorySource);
    }
}