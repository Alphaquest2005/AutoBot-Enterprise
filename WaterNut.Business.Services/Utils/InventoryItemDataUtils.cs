using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Core.Common.Utils;
using InventoryDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Utils.SavingInventoryItems;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Utils
{
    public static class InventoryItemDataUtils
    {

        public static
            List<InventoryDataItem>
            GetExistingInventoryItemFromData(List<InventoryData> inventoryDataList,
                InventorySource inventorySource)
        {
            var inventoryItems =
                InventoryItemUtils.GetInventoryItems(inventoryDataList.Select(x => (string) x.Key.ItemNumber).ToList());


            var existingInventoryItem = CreateExistingInventoryData(inventorySource, inventoryDataList, inventoryItems);


            return existingInventoryItem;
        }


        public static List<InventoryDataItem> GetNewInventoryItemFromData(List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {
            var inventoryItems =
                InventoryItemUtils.GetInventoryItems(inventoryDataList.Select(x => (string) x.Key.ItemNumber).ToList());


            var newInventoryItems = CreateNewInventoryData(inventorySource, inventoryDataList, inventoryItems);


            return newInventoryItems;
        }

        public static List<InventoryDataItem> CreateNewInventoryData(InventorySource inventorySource,
            List<InventoryData> validItems, List<InventoryItem> inventoryItems)
        {
            var newInventoryItem = GetNewInventoryItems(inventorySource, validItems, inventoryItems);
            var newInventoryItems = newInventoryItem
                .Select(item => CreateInventoryItem(inventorySource, item))
                .ToList();

            new SaveInventoryItemsSelector().Execute(newInventoryItems);

            return newInventoryItems;
        }

        public static List<InventoryData> GetNewInventoryItems(InventorySource inventorySource,
            List<InventoryData> validItems, List<InventoryItem> inventoryItems)
        {
            var newInventoryItem = validItems.Where(d =>
                !ItemInData(inventorySource, inventoryItems, d.Key.ItemNumber)).ToList();
            return newInventoryItem;
        }

        public static List<InventoryDataItem> CreateExistingInventoryData(InventorySource inventorySource,
            List<InventoryData> validItems, List<InventoryItem> inventoryItems)
        {
            List<InventoryDataItem> existingInventoryItem = validItems.Where(d =>
                    ItemInData(inventorySource, inventoryItems, d.Key.ItemNumber))
                .Join(inventoryItems, d => d.Key.ItemNumber, i => i.ItemNumber,
                    (d, i) => new InventoryDataItem(d, i)).ToList();
            return existingInventoryItem;
        }

        private static bool ItemInData(InventorySource inventorySource, List<InventoryItem> inventoryItems,
            string itemNumber)
        {
            return inventoryItems.Any(i =>
                    i.ItemNumber == itemNumber
                // no need to compare cau duplicate item when importing shipment xlsx
                // &&  i.InventoryItemSources.Any(z => z.InventorySourceId == inventorySource.Id)
            );
        }

        public static List<InventoryData> CreateItemGroupList(List<dynamic> eslst)
        {
            return eslst.Where(x => x.ItemNumber != null)
                .GroupBy(g => (g.ItemNumber.ToUpper(), g.ItemDescription, g.TariffCode))
                .Select(g => new InventoryData(g.Key, g.ToList()))
                .Where(x => !string.IsNullOrEmpty(x.Key.ItemDescription))
                .ToList();
        }

        public static InventoryDataItem CreateInventoryItem(InventorySource inventorySource,
            InventoryData item)
        {


            var i = new InventoryItem(true)
            {
                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                Description = ((string)item.Key.ItemDescription).Truncate(255), // quicker trust database than file
                ItemNumber = ((string) item.Key.ItemNumber).Truncate(20),
                TariffCode = //create a function that call deepseek api with item description and return tariff code
                    string.IsNullOrEmpty(item.Key.TariffCode)
                        ? DeepSeekApi.GetTariffCode(item.Key.ItemDescription)
                        : item.Key.TariffCode,
                InventoryItemSources = new List<InventoryItemSource>()
                {
                    new InventoryItemSource(true)
                    {
                        InventorySourceId = inventorySource.Id,
                        TrackingState = TrackingState.Added
                    }
                },
                TrackingState = TrackingState.Added
            };
            if (!string.IsNullOrEmpty(item.Key.TariffCode)) i.TariffCode = item.Key.TariffCode;
            if (string.IsNullOrEmpty(item.Key.ItemDescription))
                foreach (var line in item.Data)
                {
                    line.ItemDescription = i.Description.Truncate(255);
                }

            return new InventoryDataItem(item, i);
        }
    }
}
