using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.InventoryProcessing;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class InventoryImporter
    {
       

        public async Task ImportInventory(DataFile dataFile)
        {
            try
            {



                var itmlst = InventoryItemDataUtils.CreateItemGroupList(dataFile.Data);


                var inventorySource = InventorySourceFactory.GetInventorySource(dataFile.FileType);

                new InventoryProcessorSelector().Execute(dataFile.DocSet.First().ApplicationSettingsId, itmlst, inventorySource);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}