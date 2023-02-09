using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;
using WaterNut.Business.Services.Utils.SavingInventoryItems;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.InventoryProcessing
{
    public class InventoryProcessorSelector : IInventoryProcessor
    {
        private bool isDbMem = false;

        public void Execute(int applicationSettingsId,
            List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {
            if(isDbMem)
                new InventoryProcessor().Execute(applicationSettingsId, inventoryDataList, inventorySource);
            else
                new InventoryProcessorSet().Execute(applicationSettingsId, inventoryDataList, inventorySource);

        }
    }
}