using System;
using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;
using WaterNut.Business.Services.Utils.SavingInventoryItems;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.InventoryProcessing
{
    public class InventoryProcessor : IInventoryProcessor
    {
        public bool Execute(int applicationSettingsId,
            List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {
            try
            {

            var existingInventoryItem = InventoryItemDataUtils.GetExistingInventoryItemFromData(inventoryDataList, inventorySource);

            existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.TariffCode))
                .ForEach(x =>
                {
                    x.Item.StartTracking();
                    x.Item.TariffCode = x.Data.Key.TariffCode;
                });

            existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Data.Data.ForEach(z =>
                {
                    x.Item.StartTracking();
                    z.ItemDescription = x.Item.Description;
                }));

            existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x =>
                {
                    x.Item.StartTracking();
                    x.Item.Description = x.Data.Key.ItemDescription;
                });


            existingInventoryItem
                .Where(i => i.Item.InventoryItemSources.All(x => x.InventorySourceId != inventorySource.Id))
                .ForEach(x =>
                    x.Item.InventoryItemSources.Add(InventorySourceProcessor.CreateItemSource(inventorySource, x.Item)));

           new SaveInventoryItemsSelector().Execute(existingInventoryItem);
            ///////////////

            existingInventoryItem.ForEach(x => x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id));

            existingInventoryItem
                .Select(x => (DataItem: x, Code: InventoryCodesProcessor.GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => InventoryCodesProcessor.SaveInventoryCodes( inventorySource, x.Code, x.DataItem.Item));


            existingInventoryItem
                .Select(x => (DataItem: x, Code: InventoryAliasCodesProcessor.GetInventoryAliasCodes(x.Data, x.Item)))
                .ForEach(x => InventoryCodesProcessor.SaveInventoryCodes( inventorySource, x.Code, x.DataItem.Item));



            var newInventoryItems = InventoryItemDataUtils.GetNewInventoryItemFromData(inventoryDataList, inventorySource);



            newInventoryItems.ForEach(x => x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id));

            newInventoryItems
                .Select(x => (DataItem: x, Code: InventoryCodesProcessor.GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => InventoryCodesProcessor.SaveInventoryCodes( inventorySource, x.Code, x.DataItem.Item));


            newInventoryItems
                .Select(x => (DataItem: x, Code: InventoryAliasCodesProcessor.GetInventoryAliasCodes(x.Data, x.Item)))
                .ForEach(x => InventoryCodesProcessor.SaveInventoryCodes( inventorySource, x.Code, x.DataItem.Item));
            return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }
    }
}