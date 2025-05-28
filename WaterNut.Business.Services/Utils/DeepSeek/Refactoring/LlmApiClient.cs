#nullable disable
using Serilog; // Added
// Removed unused usings like Newtonsoft.Json, Polly, System.Net.*, System.Text.RegularExpressions etc.
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

namespace WaterNut.Business.Services.Utils.LlmApi
{
    /// <summary>
    /// Client for interacting with configured LLM providers via a strategy pattern.
    /// Orchestrates batching and fallbacks.
    /// </summary>
    public class LlmApiClient : IDisposable // Keep IDisposable if factory manages HttpClient lifetime maybe? Or remove if client is short-lived. Let's keep for now.
    {
        private readonly Serilog.ILogger _logger; // Changed to Serilog.ILogger
        private readonly ILLMProviderStrategy _strategy;

        // Configuration
        private const int MAX_ITEMS_PER_BATCH = 5; // Batching logic remains here

        // --- Public Properties (for potential overrides after creation) ---
        public string Model
        {
            get => _strategy.Model;
            set => _strategy.Model = value; // Pass through to strategy
        }
        public double DefaultTemperature
        {
            get => _strategy.DefaultTemperature;
            set => _strategy.DefaultTemperature = value; // Pass through to strategy
        }
        // Add other config pass-throughs if needed (e.g., templates)

        // --- Constructor ---
        // Expects a fully configured strategy
        public LlmApiClient(ILLMProviderStrategy strategy, Serilog.ILogger logger) // Changed ILogger<LlmApiClient> to Serilog.ILogger
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Assign the Serilog logger
            _logger.Information("LlmApiClient initialized with Strategy: {StrategyType}, Default Model: {Model}", // Changed LogInformation to Information
                _strategy.GetType().Name, _strategy.Model);
        }

        // --- Public Methods ---

        /// <summary>
        /// Gets classification info for a single item using the configured strategy.
        /// </summary>
        public async Task<(string TariffCode, string Category, string CategoryTariffCode, decimal Cost)> GetClassificationInfoAsync(
            string itemDescription, string productCode = null, double? temperature = null,
            int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            _logger.Debug("GetClassificationInfoAsync called for: {Description}", TruncateForLog(itemDescription)); // Changed LogDebug to Debug
            var response = await _strategy.GetSingleClassificationAsync(itemDescription, productCode, temperature, maxTokens, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccess || response.Result == null || !response.Result.ParsedSuccessfully)
            {
                _logger.Warning("GetSingleClassificationAsync failed for '{Description}'. Error: {Error}", itemDescription, response.ErrorMessage ?? "Unknown"); // Changed LogWarning to Warning
                return ("ERROR", "ERROR", "ERROR", response.Cost);
            }

            var result = response.Result;
            return (result.TariffCode, result.Category, result.CategoryHsCode, response.Cost);
        }

        /// <summary>
        /// Classifies a list of items using batching and fallbacks via the configured strategy.
        /// </summary>
        public async Task<(Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> Results, decimal TotalCost)>
            ClassifyItemsAsync(List<(string ItemNumber, string ItemDescription, string TariffCode)> items, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            // Use a Dictionary keyed by ORIGINAL description for the final output format
            var finalResults = new Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>();
            decimal totalCost = 0m;
            if (items == null || !items.Any()) return (finalResults, totalCost);

            // Use a temporary lookup for processing status (original description -> input tuple)
            // Group by description to handle potential duplicates, taking the first item for each description.
            var itemsToProcess = items.Where(i => !string.IsNullOrWhiteSpace(i.ItemDescription))
                                     .GroupBy(i => i.ItemDescription)
                                     .ToDictionary(g => g.Key, g => g.First());

            // NEW: Create a mapping from cleaned descriptions back to original descriptions
            var cleanedToOriginalMapping = new Dictionary<string, string>();

            var chunks = ChunkBy(itemsToProcess.Values, MAX_ITEMS_PER_BATCH).ToList();
            _logger.Information("Processing {ItemCount} items in {ChunkCount} chunks using {Strategy}",
                itemsToProcess.Count, chunks.Count, _strategy.ProviderType);

            int chunkIndex = 0;
            foreach (var chunk in chunks)
            {
                chunkIndex++;
                var chunkList = chunk.ToList(); // List of (string, string, string) tuples
                var chunkDescriptions = chunkList.Select(i => i.ItemDescription).ToList();
                _logger.Debug("Processing chunk {ChunkIndex}/{ChunkCount} with {ItemCount} items.", chunkIndex, chunks.Count, chunkList.Count);

                // NEW: Pre-populate the mapping for this chunk
                foreach (var item in chunkList)
                {
                    var cleanedDescription = CleanDescription(item.ItemDescription);
                    if (!cleanedToOriginalMapping.ContainsKey(cleanedDescription))
                    {
                        cleanedToOriginalMapping[cleanedDescription] = item.ItemDescription;
                    }
                }

                // --- Call Strategy for Batch ---
                var batchResponse = await _strategy.GetBatchClassificationAsync(chunkList, temperature, maxTokens, cancellationToken).ConfigureAwait(false);
                totalCost += batchResponse.TotalCost;

                if (batchResponse.IsSuccess)
                {
                    _logger.Information("Chunk {ChunkIndex} processed via batch. Cost: {Cost:C}, Success Count: {SuccessCount}, Failed Count: {FailCount}",
                        chunkIndex, batchResponse.TotalCost, batchResponse.Results.Count, batchResponse.FailedDescriptions.Count);

                    // Process successful results from the batch
                    foreach (var kvp in batchResponse.Results)
                    {
                        string returnedDescription = kvp.Key;
                        ClassificationResult classifiedItem = kvp.Value;

                        // NEW: Try to find the original description using the mapping
                        string originalDescription = returnedDescription;
                        if (cleanedToOriginalMapping.TryGetValue(returnedDescription, out string mappedOriginal))
                        {
                            originalDescription = mappedOriginal;
                        }
                        else
                        {
                            // Fallback: try to find by exact match in itemsToProcess
                            if (!itemsToProcess.ContainsKey(returnedDescription))
                            {
                                // Try to find by similarity or partial match
                                var possibleMatch = itemsToProcess.Keys.FirstOrDefault(k =>
                                    CleanDescription(k) == returnedDescription ||
                                    k.Contains(returnedDescription) ||
                                    returnedDescription.Contains(CleanDescription(k)));

                                if (possibleMatch != null)
                                {
                                    originalDescription = possibleMatch;
                                    _logger.Debug("Mapped cleaned description '{Cleaned}' to original '{Original}' via similarity match.",
                                        returnedDescription, originalDescription);
                                }
                                else
                                {
                                    _logger.Warning("Could not map returned description '{Returned}' to any original description.", returnedDescription);
                                    continue; // Skip this result
                                }
                            }
                        }

                        // Use the original description as the key
                        finalResults[originalDescription] = (
                            classifiedItem.ItemNumber,
                            originalDescription, // Use original description, not the cleaned one
                            classifiedItem.TariffCode,
                            classifiedItem.Category,
                            classifiedItem.CategoryHsCode
                        );
                        itemsToProcess.Remove(originalDescription); // Mark as processed
                    }

                    // Handle items that failed *within* the successful batch call
                    foreach (string failedDesc in batchResponse.FailedDescriptions)
                    {
                        // NEW: Map failed description back to original
                        string originalFailedDesc = failedDesc;
                        if (cleanedToOriginalMapping.TryGetValue(failedDesc, out string mappedOriginalFailed))
                        {
                            originalFailedDesc = mappedOriginalFailed;
                        }

                        if (itemsToProcess.TryGetValue(originalFailedDesc, out var itemTuple))
                        {
                            _logger.Warning("Item '{Description}' failed within successful batch {ChunkIndex}. Attempting fallback.", originalFailedDesc, chunkIndex);
                            await ProcessSingleItemFallbackAndAddCost(itemTuple, finalResults, totalCost, cancellationToken).ConfigureAwait(false);
                            itemsToProcess.Remove(originalFailedDesc);
                        }
                    }
                }
                else // The entire batch API call failed
                {
                    _logger.Warning("Batch API call failed for chunk {ChunkIndex}. Error: {Error}. Attempting fallback for all items in chunk.", chunkIndex, batchResponse.ErrorMessage ?? "Unknown");
                    foreach (var itemTuple in chunkList)
                    {
                        if (itemsToProcess.ContainsKey(itemTuple.ItemDescription))
                        {
                            await ProcessSingleItemFallbackAndAddCost(itemTuple, finalResults, totalCost, cancellationToken).ConfigureAwait(false);
                            itemsToProcess.Remove(itemTuple.ItemDescription);
                        }
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Warning("Cancellation requested during batch processing.");
                    break;
                }
            }

            // Safety check for any items missed
            if (itemsToProcess.Any())
            {
                _logger.Warning("Found {Count} items remaining after batch/fallback loop. Processing individually.", itemsToProcess.Count);
                foreach (var itemTuple in itemsToProcess.Values.ToList())
                {
                    await ProcessSingleItemFallbackAndAddCost(itemTuple, finalResults, totalCost, cancellationToken).ConfigureAwait(false);
                }
            }

            _logger.Information("Finished processing using {Strategy}. Total Estimated Cost: {TotalCost:C}. Items Processed: {ResultCount}/{InitialCount}",
                 _strategy.ProviderType, totalCost, finalResults.Count, items.Count);
            return (finalResults, totalCost);
        }

        // NEW: Helper method to clean descriptions (should match what the LLM API does)
        private string CleanDescription(string description)
        {
            if (string.IsNullOrEmpty(description)) return description;

            // Common cleaning operations that LLM APIs might perform:
            // - Remove backslashes
            // - Remove or escape quotes
            // - Normalize whitespace
            // - Remove other problematic characters

            return description
                .Replace("\\", "")           // Remove backslashes
                .Replace("\"", "'")          // Replace double quotes with single quotes
                .Replace("\t", " ")          // Replace tabs with spaces
                .Replace("\r", "")           // Remove carriage returns
                .Replace("\n", " ")          // Replace newlines with spaces
                .Trim()                      // Remove leading/trailing whitespace
                .Replace("  ", " ");         // Normalize multiple spaces to single space
        }

        // --- Private Helper Methods ---

        // Combined Fallback call and cost addition
        private async Task ProcessSingleItemFallbackAndAddCost(
             (string ItemNumber, string ItemDescription, string TariffCode) itemTuple,
             Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> finalResults, decimal totalCost,
             CancellationToken cancellationToken)
        {
            // Fallback using single item method
            var (fbTariff, fbCat, fbCatHs, fbCost) = await GetClassificationInfoAsync(itemTuple.ItemDescription, itemTuple.ItemNumber, temperature: null, maxTokens: null, cancellationToken: cancellationToken).ConfigureAwait(false); // Use default temp/tokens for fallback? Or specific ones?
            totalCost += fbCost; // Add cost of this fallback attempt

            // Add result (even if it's "ERROR") to the final dictionary
            finalResults[itemTuple.ItemDescription] = (
                 itemTuple.ItemNumber == "NEW" && fbTariff != "ERROR" ? fbTariff : itemTuple.ItemNumber, // Need item number from ClassificationResult DTO ideally
                 itemTuple.ItemDescription,
                 fbTariff,
                 fbCat,
                 fbCatHs
             );

            if (fbTariff == "ERROR")
            {
                _logger.Warning("Fallback processing failed for '{Description}'.", itemTuple.ItemDescription); // Changed LogWarning to Warning
            }
            else
            {
                _logger.Information("Fallback processing successful for '{Description}'. Cost: {Cost:C}", itemTuple.ItemDescription, fbCost); // Changed LogInformation to Information
            }
        }

        // --- Simple Helpers ---
        private string TruncateForLog(string text, int maxLength = 250)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...(truncated)";
        }

        // --- Chunking Helper ---
        private static IEnumerable<IEnumerable<T>> ChunkBy<T>(IEnumerable<T> source, int chunkSize) { if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize)); if (source == null) throw new ArgumentNullException(nameof(source)); return ChunkByIterator(source, chunkSize); }
        private static IEnumerable<IEnumerable<T>> ChunkByIterator<T>(IEnumerable<T> source, int chunkSize) { using (var enumerator = source.GetEnumerator()) { while (enumerator.MoveNext()) { yield return GetChunk(enumerator, chunkSize); } } }
        private static IEnumerable<T> GetChunk<T>(IEnumerator<T> enumerator, int chunkSize) { var chunk = new List<T>(chunkSize); chunk.Add(enumerator.Current); for (int i = 1; i < chunkSize && enumerator.MoveNext(); i++) { chunk.Add(enumerator.Current); } return chunk; }

        // --- CORRECT IDisposable IMPLEMENTATION ---
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    // Since LlmApiClient doesn't directly own disposable managed objects anymore
                    // (like HttpClient), this section might be empty.
                    // Log the disposal action.
                    _logger?.Debug("Disposing LlmApiClient (managed resources)."); // Changed LogDebug to Debug
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                // This class likely doesn't have unmanaged resources.

                disposedValue = true;
            }
        }

        // Optional: uncomment finalizer only if you have unmanaged resources directly in this class
        // ~LlmApiClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            // Suppress finalization because Dispose has done the work.
            GC.SuppressFinalize(this);
        }
        // --- END CORRECTED IDisposable IMPLEMENTATION ---
    }
}