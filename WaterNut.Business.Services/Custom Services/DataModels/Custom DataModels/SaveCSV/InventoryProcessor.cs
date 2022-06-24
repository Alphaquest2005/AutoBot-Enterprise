using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class InventoryProcessor
    {
        public static void ProcessInventoryItemLst(int applicationSettingsId,
            List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {

            var existingInventoryItem = InventoryItemDataUtils.GetExistingInventoryItemFromData(inventoryDataList, inventorySource);

            existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.TariffCode))
                .ForEach(x => x.Item.TariffCode = x.Data.Key.TariffCode);

            existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Data.Data.ForEach(z => z.ItemDescription = x.Item.Description));

            existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Item.Description = x.Data.Key.ItemDescription);


            existingInventoryItem
                .Where(i => i.Item.InventoryItemSources.All(x => x.InventorySourceId != inventorySource.Id))
                .ForEach(x =>
                    x.Item.InventoryItemSources.Add(InventorySourceProcessor.CreateItemSource(inventorySource, x.Item)));

            InventoryItemDataUtils.SaveInventoryItems(existingInventoryItem);
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

        }
    }
}