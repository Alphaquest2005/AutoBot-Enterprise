using System; // Added for Type and Convert
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllocationDS.Business.Services;
using Core.Common.Extensions;
using Core.Common.Utils;
using InventoryDS.Business.Entities;
using InventoryQS.Business.Services;
using MoreLinq.Extensions;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Utils.LlmApi;
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


        public static async Task<List<InventoryDataItem>> GetNewInventoryItemFromData(List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {
            var inventoryItems =
                InventoryItemUtils.GetInventoryItems(inventoryDataList.Select(x => (string) x.Key.ItemNumber).ToList());


            var newInventoryItems = await CreateNewInventoryData(inventorySource, inventoryDataList, inventoryItems).ConfigureAwait(false);


            return newInventoryItems;
        }

        public static async Task<List<InventoryDataItem>> CreateNewInventoryData(InventorySource inventorySource,
            List<InventoryData> validItems, List<InventoryItem> inventoryItems)
        {
            var newInventoryItemData = GetNewInventoryItems(inventorySource, validItems, inventoryItems);
            var newInventoryItems = new List<InventoryDataItem>();
            foreach (var itemData in newInventoryItemData)
            {
                newInventoryItems.Add(await CreateInventoryItem(inventorySource, itemData).ConfigureAwait(false));
            }

            await new SaveInventoryItemsSelector().Execute(newInventoryItems).ConfigureAwait(false);

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

        public static async Task<InventoryDataItem> CreateInventoryItem(InventorySource inventorySource,
            InventoryData item)
        {
            try
            {

                //create a function that call deepseek api with item description and return tariff code
                
                string keyTariffCode = inventorySource.Name != "POS" && string.IsNullOrEmpty(item.Key.TariffCode)
                    ? await GetTariffCode(item).ConfigureAwait(false)//GetTariffCodeValue(item)
                    : item.Key.TariffCode;

                // Use helper method to safely get values
                string description = await GetDynamicStringValueAsync(item.Key.ItemDescription).ConfigureAwait(false);
                string itemNumber = await GetDynamicStringValueAsync(item.Key.ItemNumber).ConfigureAwait(false);

                var i = new InventoryItem(true)
                {
                    ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                    Description = ((string)description).Truncate(255), // Do not Remove explicit cast as it is needed for extension call
                    ItemNumber =
                        ((string)itemNumber)
                        .Truncate(20), // Do not Remove explicit cast as it is needed for extension call
                    TariffCode =
                        keyTariffCode, // Already awaited or string
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
                if (string.IsNullOrEmpty(item.Key.ItemDescription))
                    foreach (var line in item.Data)
                    {
                        line.ItemDescription = i.Description.ToString().Truncate(255); // Removed explicit cast
                    }

                return new InventoryDataItem(item, i);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Helper to safely get string from dynamic property (might be Task<string> or string)
        private static async Task<string> GetDynamicStringValueAsync(dynamic dynamicValue)
        {
            if (dynamicValue == null) return string.Empty;

            // Explicitly check if it's already a Task<string>
            if (dynamicValue is Task<string> taskString)
            {
                return await taskString.ConfigureAwait(false) ?? string.Empty;
            }

            // Check if it's some other awaitable Task<T>
            var type = (Type)dynamicValue.GetType();
            if (type.GetMethod("GetAwaiter") != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                 // It's likely a Task<T>, await it
                await dynamicValue.ConfigureAwait(false);
                // Now get the result (which we hope is string or convertible)
                 var result = await dynamicValue.ConfigureAwait(false);
                 return Convert.ToString(result) ?? string.Empty;
            }

            // Otherwise, assume it's directly convertible
            return Convert.ToString(dynamicValue) ?? string.Empty;
        }


        private static async Task<string> GetTariffCode(InventoryData item)
        {
            // Use the helper for the description passed to the API as well
            var description = await GetDynamicStringValueAsync(item.Key.ItemDescription).ConfigureAwait(false);
            var suspectedTariffCode = await GetClassificationInfo(description);
            return await InventoryItemsExService.GetTariffCode(suspectedTariffCode.Item1).ConfigureAwait(false);
        }

        private static async Task<dynamic> GetClassificationInfo(dynamic description)
        {
            var desiredProvider = LLMProvider.DeepSeek;
            var client = LlmApiClientFactory.CreateClient(desiredProvider);
            var classificationInfo = await client.GetClassificationInfoAsync(description).ConfigureAwait(false);
            return classificationInfo; //await new DeepSeekApi().GetClassificationInfoAsync(description).ConfigureAwait(false);
        }
    }
}
