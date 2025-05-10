using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using InventoryDS.Business.Entities;
using InventoryQS.Business.Entities;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Omu.ValueInjecter;
using WaterNut.Business.Services.Utils;
using WaterNut.Business.Services.Utils.LlmApi;
using WaterNut.DataSpace;

namespace InventoryQS.Business.Services
{
   
   
    public partial class InventoryItemsExService 
    {
        public async Task AssignTariffToItms(List<int> list, string tariffCodes)
        {
            await
                WaterNut.DataSpace.NullTarifInventoryItemsModel.Instance.AssignTariffToItms(list, tariffCodes)
                    .ConfigureAwait(false);
        }

        public async Task ValidateExistingTariffCodes(int docSetId)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.ValidateExistingTariffCodes(docSet).ConfigureAwait(false);
        }

        //public async Task MapInventoryToAsycuda()
        //{
        //    await WaterNut.DataSpace.NullTarifInventoryItemsModel.Instance.MapInventoryToAsycuda().ConfigureAwait(false);
        //}

        public async Task SaveInventoryItemsEx(InventoryItemsEx olditm)
        {
            var itm = new InventoryDSContext().InventoryItems.First(x => x.Id == olditm.InventoryItemId && x.ApplicationSettingsId == olditm.ApplicationSettingsId);
            //itm.ApplicationSettingsId = olditm.ApplicationSettingsId;
            itm.TariffCode = olditm.TariffCode;

            await WaterNut.DataSpace.InventoryDS.DataModels.BaseDataModel.Instance.SaveInventoryItem(itm).ConfigureAwait(false);
        }


        public static async Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>>
            ClassifiedItms(List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms)
        {
            try
            {
                //var
                var itms =
                BaseDataModel.Instance.CurrentApplicationSettings.UseAIClassification ?? false
                    ? (await GetClassifyItems(Itms).ConfigureAwait(false))
                        .Where(x => x.Value.ItemNumber != null)
                        .ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.ItemNumber, kvp.Value.ItemDescription, kvp.Value.TariffCode, kvp.Value.Category, kvp.Value.CategoryTariffCode))
                    : Itms.DistinctBy(x => x.ItemNumber)
                        .ToDictionary(x => x.ItemNumber, x => (x.ItemNumber, x.ItemDescription, x.TariffCode, string.Empty, string.Empty));


                var res = new Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>();


                foreach (var itm in itms)
                {
                    
                    res.Add(itm.Key, await UpdateTariffValue(itm).ConfigureAwait(false));
                }


                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static async Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>> GetClassifyItems(List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms)
        {
            // Assuming:
            // - Itms is some input for ClassifyItemsAsync
            // - ClassifyItemsAsync returns something like IEnumerable<KeyValuePair<string, YourOriginalValueType>>
            //   where YourOriginalValueType has properties ItemNumber, ItemDescription, TariffCode, Category.
            // - GetCategoryTariffCode is an async method:
            //   public async Task<string> GetCategoryTariffCode(KeyValuePair<string, YourOriginalValueType> itemKeyValuePair)
            //   or
            //   public async Task<string> GetCategoryTariffCode(YourOriginalValueType itemValue)
            //   (adjust the call to GetCategoryTariffCode(x) or GetCategoryTariffCode(x.Value) accordingly)

            var classifiedItems = await ClassifyItemsAsync(Itms).ConfigureAwait(false);
            if (BaseDataModel.Instance.CurrentApplicationSettings.GroupIM4ByCategory == true)
            {


                // Directly await Task.WhenAll on the LINQ Select expression
                var transformedItems = await Task.WhenAll(
                    classifiedItems.Select(async x => // x is a KeyValuePair<string, YourOriginalValueType>
                        new KeyValuePair<string, (string ItemNumber, string ItemDescription, string TariffCode, string
                            Category, string CategoryTariffCode)>(
                            x.Key,
                            ( // Start of the tuple for the Value
                                x.Value.ItemNumber,
                                x.Value.ItemDescription,
                                x.Value.TariffCode,
                                x.Value.Category,
                                await GetCategoryTariffCode(x).ConfigureAwait(false) // Await directly here
                            ) // End of the tuple
                        )
                    )
                ).ConfigureAwait(false);

                // Convert the array of KeyValuePairs to a Dictionary
                // This assumes that x.Key values are unique across the items.
                var resultDictionary = transformedItems.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value
                );

                return resultDictionary;
            }

            return classifiedItems;
        }
        

        private static async Task<(string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> UpdateTariffValue( KeyValuePair<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> itm)
        {
            var tariffCode = await GetTariffCode(itm.Value.TariffCode).ConfigureAwait(false);

            var categoryTariffCode = await GetCategoryTariffCode(itm).ConfigureAwait(false);

            return tariffCode == itm.Value.TariffCode
                ? itm.Value
                : (itm.Value.ItemNumber, itm.Value.ItemDescription, tariffCode, itm.Value.Category, categoryTariffCode);
        }

        private static async Task<string> GetCategoryTariffCode(KeyValuePair<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> itm)
        {
            var dbCategoryTariff = BaseDataModel.Instance.CategoryTariffs.FirstOrDefault(x => x.Category == itm.Value.Category);
            var categoryTariffCode = dbCategoryTariff == null
                ? await GetTariffCode(itm.Value.CategoryTariffCode).ConfigureAwait(false)
                : dbCategoryTariff.TariffCode;
            return categoryTariffCode;
        }

        public static async Task<string> GetCategoryTariffCode(string Category, string CategoryTariffCode)
        {
            var dbCategoryTariff = BaseDataModel.Instance.CategoryTariffs.FirstOrDefault(x => x.Category == Category);
            var categoryTariffCode = dbCategoryTariff == null
                ? await GetTariffCode(CategoryTariffCode).ConfigureAwait(false)
                : dbCategoryTariff.TariffCode;
            return categoryTariffCode;
        }

        private static async
            Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category,
                string CategoryTariffCode)>> ClassifyItemsAsync(
                List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms)
        {
            try
            {
                var desiredProvider = LLMProvider.DeepSeek;
                using var apiClient = LlmApiClientFactory.CreateClient(desiredProvider);
                var res = await apiClient.ClassifyItemsAsync(Itms).ConfigureAwait(false);
                Console.WriteLine($"Call cost: {res.TotalCost.ToString("C")}");
                return res.Results;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task<string> GetTariffCode(string suspectedTariffCode)
        {
            if (string.IsNullOrEmpty(suspectedTariffCode))
                return suspectedTariffCode;

            var partialCode = suspectedTariffCode.Length >= 6
                ? suspectedTariffCode.Substring(0, 6)
                : suspectedTariffCode;

            var code90 = partialCode + "90";
            var code00 = partialCode + "00";


            var context = new InventoryDSContext();
            return await context.TariffCodes
                .Where(x => x.RateofDuty != null && !string.IsNullOrEmpty(x.RateofDuty))
                .Where(x => x.TariffCodeName == suspectedTariffCode
                            || x.TariffCodeName == code90
                            || x.TariffCodeName == code00
                            || x.TariffCodeName.StartsWith(partialCode))
                .OrderByDescending(x => x.TariffCodeName.Length)
                .ThenBy(x => x.TariffCodeName)
                .Select(x => x.TariffCodeName)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false) ?? suspectedTariffCode;
        }


    }
}
