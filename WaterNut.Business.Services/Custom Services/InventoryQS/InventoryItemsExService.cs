using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics; // Added for Stopwatch
using InventoryDS.Business.Entities;
using InventoryQS.Business.Entities;
using Serilog;
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
            ClassifiedItms(List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ClassifiedItms), "Classify inventory items using AI or existing data", $"ItemCount: {Itms?.Count ?? 0}");

            try
            {
                logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ClassifiedItms), $"Classifying {Itms?.Count ?? 0} items");

                //var
                var itms =
                BaseDataModel.Instance.CurrentApplicationSettings.UseAIClassification ?? false
                    ? (await GetClassifyItems(Itms, logger).ConfigureAwait(false)) // Pass logger
                        .Where(x => x.Value.ItemNumber != null)
                        .ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.ItemNumber, kvp.Value.ItemDescription, kvp.Value.TariffCode, kvp.Value.Category, kvp.Value.CategoryTariffCode))
                    : Itms.DistinctBy(x => x.ItemNumber)
                        .ToDictionary(x => x.ItemNumber, x => (x.ItemNumber, x.ItemDescription, x.TariffCode, string.Empty, string.Empty));
 
 
                var res = new Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>();
 
 
                foreach (var itm in itms)
                {
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(ClassifiedItms), "UpdateTariffValueLoop", "Processing item for tariff value update.", $"ItemKey: {itm.Key}");
                    res.Add(itm.Key, await UpdateTariffValue(itm).ConfigureAwait(false));
                }
 
                methodStopwatch.Stop(); // Stop stopwatch on success
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ClassifiedItms), "Items classified successfully", $"ClassifiedItemCount: {res.Count}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ClassifiedItms), $"Successfully classified {res.Count} items", methodStopwatch.ElapsedMilliseconds);

                return res;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ClassifiedItms), "Classify inventory items using AI or existing data", methodStopwatch.ElapsedMilliseconds, $"Error classifying items: {e.Message}");
                logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ClassifiedItms), "Overall classification process", methodStopwatch.ElapsedMilliseconds, $"Error classifying items: {e.Message}");
                Console.WriteLine(e); // Keep Console.WriteLine for now as per original code
                throw;
            }
 
        }
 
        private static async Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>> GetClassifyItems(List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(GetClassifyItems), "Get classified items, potentially using AI", $"ItemCount: {Itms?.Count ?? 0}");

            try
            {
                logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(GetClassifyItems), $"Getting classified items for {Itms?.Count ?? 0} items");

                // Assuming:
                // - Itms is some input for ClassifyItemsAsync
                // - ClassifyItemsAsync returns something like IEnumerable<KeyValuePair<string, YourOriginalValueType>>
                //   where YourOriginalValueType has properties ItemNumber, ItemDescription, TariffCode, Category.
                // - GetCategoryTariffCode is an async method:
                //   public async Task<string> GetCategoryTariffCode(KeyValuePair<string, YourOriginalValueType> itemKeyValuePair)
                //   or
                //   public async Task<string> GetCategoryTariffCode(YourOriginalValueType itemValue)
                //   (adjust the call to GetCategoryTariffCode(x) or GetCategoryTariffCode(x.Value) accordingly)
 
                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ClassifyItemsAsync", "ASYNC_EXPECTED");
                var classifyStopwatch = Stopwatch.StartNew();
                var classifiedItems = await ClassifyItemsAsync(Itms, logger).ConfigureAwait(false); // Pass logger
                classifyStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ClassifyItemsAsync", classifyStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                if (BaseDataModel.Instance.CurrentApplicationSettings.GroupIM4ByCategory == true)
                {
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(GetClassifyItems), "CategoryGrouping", "Grouping items by category and getting category tariff codes.", $"ClassifiedItemCount: {classifiedItems.Count()}");

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

                    methodStopwatch.Stop(); // Stop stopwatch on success
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(GetClassifyItems), "Items classified and categorized successfully", $"ResultCount: {resultDictionary.Count}", methodStopwatch.ElapsedMilliseconds);
                    logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(GetClassifyItems), $"Successfully classified and categorized {resultDictionary.Count} items", methodStopwatch.ElapsedMilliseconds);

                    return resultDictionary;
                }

                methodStopwatch.Stop(); // Stop stopwatch on success
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(GetClassifyItems), "Items classified successfully (no category grouping)", $"ResultCount: {classifiedItems.Count()}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(GetClassifyItems), $"Successfully classified {classifiedItems.Count()} items (no category grouping)", methodStopwatch.ElapsedMilliseconds);

                return classifiedItems;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetClassifyItems), "Get classified items, potentially using AI", methodStopwatch.ElapsedMilliseconds, $"Error getting classified items: {e.Message}");
                logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetClassifyItems), "Classification process", methodStopwatch.ElapsedMilliseconds, $"Error getting classified items: {e.Message}");
                Console.WriteLine(e); // Keep Console.WriteLine for now
                throw;
            }
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
                List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ClassifyItemsAsync), "Asynchronously classify items using LLM API", $"ItemCount: {Itms?.Count ?? 0}");

            try
            {
                logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ClassifyItemsAsync), $"Classifying {Itms?.Count ?? 0} items using LLM API");

                var desiredProvider = LLMProvider.DeepSeek;
                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "LlmApiClientFactory.CreateClient", "SYNC_EXPECTED");
                var createClientStopwatch = Stopwatch.StartNew();
                using var apiClient = LlmApiClientFactory.CreateClient(desiredProvider, logger); // Pass logger
                createClientStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "LlmApiClientFactory.CreateClient", createClientStopwatch.ElapsedMilliseconds, "Sync call returned");

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "apiClient.ClassifyItemsAsync", "ASYNC_EXPECTED");
                var classifyStopwatch = Stopwatch.StartNew();
                var res = await apiClient.ClassifyItemsAsync(Itms).ConfigureAwait(false);
                classifyStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "apiClient.ClassifyItemsAsync", classifyStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ClassifyItemsAsync), "ResultSummary", "LLM API classification completed.", $"ResultCount: {res.Results?.Count ?? 0}, TotalCost: {res.TotalCost.ToString("C")}", new { res.TotalCost });

                Console.WriteLine($"Call cost: {res.TotalCost.ToString("C")}"); // Keep Console.WriteLine for now

                methodStopwatch.Stop(); // Stop stopwatch on success
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ClassifyItemsAsync), "Items classified successfully by LLM API", $"ResultCount: {res.Results?.Count ?? 0}, TotalCost: {res.TotalCost.ToString("C")}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ClassifyItemsAsync), $"Successfully classified {res.Results?.Count ?? 0} items using LLM API", methodStopwatch.ElapsedMilliseconds);

                return res.Results;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ClassifyItemsAsync), "Asynchronously classify items using LLM API", methodStopwatch.ElapsedMilliseconds, $"Error classifying items with LLM API: {e.Message}");
                logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ClassifyItemsAsync), "LLM API classification process", methodStopwatch.ElapsedMilliseconds, $"Error classifying items with LLM API: {e.Message}");
                Console.WriteLine(e); // Keep Console.WriteLine for now
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
