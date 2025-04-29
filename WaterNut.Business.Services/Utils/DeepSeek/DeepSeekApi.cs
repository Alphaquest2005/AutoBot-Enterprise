using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Logging.Abstractions; // Not strictly needed if using LoggerFactory.Create
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WaterNut.Business.Services.Utils
{
    public partial class DeepSeekApi : IDisposable
    {
        // Configuration constants
        private const int MAX_TOKENS_PER_REQUEST = 4096;
        private const int MAX_ITEMS_PER_BATCH = 5; // Keep batch size small for complex requests
        private const int TOKEN_BUFFER = 512; // Larger buffer due to potentially longer responses
        private const int FALLBACK_MAX_TOKENS = 100; // Fallback for single item max_tokens if calculation fails

        private readonly HttpClient _httpClient;
        private readonly ILogger<DeepSeekApi> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        // --- Prompts ---
        // Prompt for single item classification (less likely to get category well)
        public string SingleItemPromptTemplate { get; set; }
        // Prompt for batch classification (primary method for getting category)
        public string BatchItemPromptTemplate { get; set; }

        public string Model { get; set; } = "deepseek-chat";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 150; // Default for single item, batch calculates dynamically
        public string HsCodePattern { get; set; } = @"\b(\d{8})\b";
        public int MaxDescriptionLength { get; set; } = 500;
        public string SanitizePattern { get; set; } = @"[^\p{L}\p{N}\p{P}\p{S}\s]"; // Allow letters, numbers, punctuation, symbols, whitespace
        public string ItemNumberPattern { get; set; } = @"^[\w-]{1,20}$"; // Alphanumeric + hyphen, 1-20 chars
        public string TariffCodePattern { get; set; } = @"^\d{8}$"; // Exactly 8 digits

        private readonly AsyncRetryPolicy _retryPolicy;

        // --- Constructors ---

        public DeepSeekApi()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("WaterNut", LogLevel.Debug) // Set your desired log level here
                    .AddConsole()
                    .AddDebug();
            });
            _logger = loggerFactory.CreateLogger<DeepSeekApi>();
            _logger.LogDebug("Initializing DeepSeekApi using default constructor...");

            _apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
                      ?? throw new InvalidOperationException("API key 'DEEPSEEK_API_KEY' not found in environment variables.");

            _retryPolicy = CreateRetryPolicy();
            _baseUrl = "https://api.deepseek.com/v1";
            _httpClient = CreateHttpClient();

            SetupHttpClient();
            SetDefaultPrompts(); // Initialize both prompts
            _logger.LogInformation("DeepSeekApi initialized successfully.");
        }

        public DeepSeekApi(ILogger<DeepSeekApi> logger, string apiKey)
        {
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DeepSeekApi>.Instance;
            _apiKey = !string.IsNullOrWhiteSpace(apiKey) ? apiKey
                      : throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");
            _logger.LogDebug("Initializing DeepSeekApi with provided logger and API key...");

            _retryPolicy = CreateRetryPolicy();
            _baseUrl = "https://api.deepseek.com/v1";
            _httpClient = CreateHttpClient();

            SetupHttpClient();
            SetDefaultPrompts(); // Initialize both prompts
            _logger.LogInformation("DeepSeekApi initialized successfully with provided logger.");
        }

        // --- Retry Policy ---
        private AsyncRetryPolicy CreateRetryPolicy()
        {
            Func<int, TimeSpan> calculateDelay = (retryAttempt) =>
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                _logger.LogTrace("Retry attempt {RetryCount}: Calculated delay {DelaySeconds}s", retryAttempt, delay.TotalSeconds);
                return delay;
            };

            Action<Exception, TimeSpan> logRetryAction = (exception, calculatedDelay) =>
            {
                if (exception is RateLimitException rle)
                {
                    _logger.LogWarning(rle, "Retry needed due to Rate Limit (HTTP {StatusCode}). Delaying for {DelaySeconds}s...", rle.StatusCode, calculatedDelay.TotalSeconds);
                }
                else if (exception is TaskCanceledException tce)
                {
                    if (tce.CancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning(tce, "Retry triggered for Task Cancellation (possibly user initiated). Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                    else
                    {
                        _logger.LogWarning(tce, "Retry needed due to operation Timeout. Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                }
                else if (exception is HttpRequestException httpEx && httpEx.Data.Contains("StatusCode"))
                {
                    _logger.LogWarning(httpEx, "Retry needed due to Server Error (HTTP {StatusCode}). Delaying for {DelaySeconds}s...", httpEx.Data["StatusCode"], calculatedDelay.TotalSeconds);
                }
                else
                {
                    _logger.LogWarning(exception, "Retry needed due to handled Transient Error ({ExceptionType}). Delaying for {DelaySeconds}s...", exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
                }
            };

            return Policy
               .Handle<RateLimitException>()
               .Or<HttpRequestException>(ex => ex.Data.Contains("StatusCode") && ((int)ex.Data["StatusCode"] >= 500 || (int)ex.Data["StatusCode"] == (int)HttpStatusCode.RequestTimeout))
               .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested) // Only retry timeouts, not user cancellations
               .WaitAndRetryAsync(
                   retryCount: 3,
                   sleepDurationProvider: calculateDelay,
                   onRetry: logRetryAction
               );
        }

        // --- HTTP Client Setup ---
        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                MaxConnectionsPerServer = 20, // Allow more concurrent connections
                UseProxy = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromSeconds(300) // 5-minute timeout for potentially long requests
            };
            return client;
        }

        private void SetupHttpClient()
        {
            if (_httpClient == null) throw new InvalidOperationException("HttpClient not initialized.");
            if (string.IsNullOrWhiteSpace(_apiKey)) throw new InvalidOperationException("API Key not initialized.");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        }

        // --- Prompt Setup ---
        private void SetDefaultPrompts()
        {
            // Updated Single Item Prompt (Focus: HS code, Best guess category if possible)
            SingleItemPromptTemplate = @"Analyze the product description provided below. Return ONLY a single, valid JSON object.
Follow these rules strictly:
1. The JSON object must match this exact format:
{{
    ""items"": [
        {{
            ""original_description"": ""EXACT_INPUT_DESCRIPTION"",
            ""product_code"": ""INPUT_CODE_OR_GENERATED"",
            ""category"": ""BEST_GUESS_BROAD_CATEGORY"", // Provide a broad category guess
            ""category_hs_code"": ""00000000"", // Provide 8-digit category HS code guess (or 00000000 if unsure)
            ""hs_code"": ""00000000"" // Exactly 8 digits for the specific item
        }}
    ]
}}
2. Preserve the ""original_description"" EXACTLY as provided in the input.
3. The ""hs_code"" MUST be the specific 8-digit HS code for the item. No dots, dashes, or letters. Provide the best estimate.
4. The ""category"" should be a broad classification (e.g., 'women clothing', 'electronics').
5. The ""category_hs_code"" MUST be the 8-digit HS code for the broad category. Provide the best 8-digit estimate (0-9 only). If unsure about category or its code, use ""N/A"" for category and ""00000000"" for category_hs_code.
6. Ensure the entire response is *only* the valid JSON structure specified. No introductory text, explanations, apologies, or markdown formatting (like ```json).

Product Information:
Description: __DESCRIPTION_HERE__
Product Code (Optional): __PRODUCT_CODE_HERE__";

            // Updated Batch Item Prompt (Explicitly asking for Category and Category HS Code)
            BatchItemPromptTemplate = @"Analyze the list of products below. For each product, determine:
1. A broad product category (e.g., 'electronics', 'women clothing', 'office supplies').
2. The 8-digit HS code for that broad category (Category Tariff Code).
3. The specific 8-digit HS code for the item description itself (Tariff Code).
4. Use the provided product code. If 'NEW', invalid, or missing, generate a suitable one.

Return ONLY a single, valid JSON object containing an 'items' array. Follow the format and rules strictly.

JSON Format:
{
    ""items"": [
        {
            ""original_description"": ""EXACT_DESCRIPTION_FROM_INPUT"",
            ""product_code"": ""PROVIDED_OR_GENERATED_CODE"",
            ""category"": ""BROAD_CATEGORY_NAME"",
            ""category_hs_code"": ""8_DIGIT_CATEGORY_HS"", // Tariff code for the category
            ""hs_code"": ""8_DIGIT_ITEM_HS"" // Tariff code for the specific item
        }
        // ... more items
    ]
}

Rules:
- The entire response MUST be only the JSON object. No extra text, explanations, apologies, or markdown.
- Preserve ""original_description"" EXACTLY as provided in the input list.
- ""product_code"" should be the code provided in the input. If 'NEW' or invalid (not alphanumeric/hyphen, 1-20 chars), generate a concise code (uppercase alphanumeric, hyphens, max 20 chars).
- ""category"" MUST be a concise, broad category name (e.g., 'home goods', 'mens footwear'). Use 'N/A' if truly unclassifiable.
- ""category_hs_code"" MUST be exactly 8 digits (0-9) representing the HS code for the BROAD CATEGORY. Use '00000000' if the category HS code cannot be determined.
- ""hs_code"" MUST be exactly 8 digits (0-9) representing the SPECIFIC HS code for the item described. Use '00000000' if it cannot be determined.
- IMPORTANT: If you are about to exceed the token limit, STOP adding new items. Finish the CURRENT item object completely, then properly CLOSE the 'items' array with ']' and the main JSON object with '}'. Ensure the final output is ALWAYS valid JSON.
- Ensure no trailing commas in the JSON arrays or objects.

Product List:";
            // The actual product list will be appended in CreateBatchPrompt
        }

        // --- Public Methods ---

        /// <summary>
        /// Attempts to get the specific HS code, category, and category HS code for a single item description.
        /// Note: Category information might be less reliable from single-item requests compared to batch requests.
        /// </summary>
        /// <param name="itemDescription">The description of the item.</param>
        /// <param name="productCode">Optional product code.</param>
        /// <param name="temperature">Optional temperature override.</param>
        /// <param name="maxTokens">Optional max tokens override.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A tuple containing (TariffCode, Category, CategoryTariffCode). Returns empty strings or defaults if unsuccessful.</returns>
        public async Task<(string TariffCode, string Category, string CategoryTariffCode)> GetClassificationInfoAsync(
            string itemDescription,
            string productCode = null,
            double? temperature = null,
            int? maxTokens = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(itemDescription))
            {
                _logger.LogWarning("GetClassificationInfoAsync called with empty description.");
                return ("", "N/A", "00000000"); // Return default/empty values
            }

            try
            {
                var cleanDescription = SanitizeInputText(itemDescription);
                var finalProductCode = string.IsNullOrWhiteSpace(productCode) ? "N/A" : SanitizeProductCode(productCode);

                // Construct the prompt using the SingleItemPromptTemplate
                var prompt = SingleItemPromptTemplate
                                .Replace("__DESCRIPTION_HERE__", cleanDescription)
                                .Replace("__PRODUCT_CODE_HERE__", finalProductCode);

                _logger.LogDebug("Generated Prompt for GetClassificationInfoAsync: {Prompt}", TruncateForLog(prompt, 200)); // Log truncated prompt

                var jsonResponseContent = await GetCompletionAsync(prompt, temperature, maxTokens ?? DefaultMaxTokens, cancellationToken)
                                                .ConfigureAwait(false);

                return ParseSingleItemResponse(jsonResponseContent);
            }
            catch (Exception ex) when (!(ex is HSCodeRequestException)) // Catch exceptions not already wrapped
            {
                _logger.LogError(ex, "Failed to retrieve classification info for Description: {ItemDescription}", itemDescription);
                // Throw a specific exception type if needed, or return defaults
                // throw new HSCodeRequestException($"Failed to retrieve classification info for '{itemDescription}'", ex);
                return ("", "ERROR", "ERROR"); // Indicate error in return values
            }
        }


        /// <summary>
        /// Processes a list of items to classify them, retrieving specific HS code, category, and category HS code using batching.
        /// </summary>
        /// <param name="items">List of items to classify (ItemNumber, ItemDescription, existing TariffCode - optional).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A dictionary mapping the original item description to its classification results.</returns>
        public async Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>>
            ClassifyItemsAsync(
                List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
                CancellationToken cancellationToken = default)
        {
            // Result dictionary holds the final structured data including Category and CategoryTariffCode
            var result = new Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>();
            if (items == null || !items.Any()) return result;

            // Pre-process and sanitize inputs
            var processedItems = items
                .Where(i => !string.IsNullOrWhiteSpace(i.ItemDescription))
                .Select(i => (
                    ItemNumber: SanitizeItemNumber(i.ItemNumber), // Ensure ItemNumber is sanitized early
                    ItemDescription: i.ItemDescription, // Keep original description for lookup key
                    TariffCode: SanitizeTariffCode(i.ItemNumber, i.TariffCode, "InputHS") // Sanitize input HS if provided
                 )).ToList();

            var chunks = ChunkBy(processedItems, MAX_ITEMS_PER_BATCH).ToList();
            _logger.LogInformation("Processing {ItemCount} valid items in {ChunkCount} chunks (Batch Size: {BatchSize}).", processedItems.Count, chunks.Count, MAX_ITEMS_PER_BATCH);

            int chunkIndex = 0;
            foreach (var chunk in chunks)
            {
                chunkIndex++;
                var chunkList = chunk.ToList(); // Materialize chunk
                _logger.LogDebug("Processing chunk {ChunkIndex}/{ChunkCount} with {ItemCount} items.", chunkIndex, chunks.Count, chunkList.Count);


                try
                {
                    // Process the chunk using the batch prompt
                    var batchResult = await ProcessChunk(chunkList, cancellationToken);

                    // Merge results from the batch back into the main result dictionary
                    foreach (var item in chunkList)
                    {
                        var originalDescription = item.ItemDescription; // Use the original description as the key

                        if (batchResult.TryGetValue(originalDescription, out var batchValues))
                        {
                            // Prioritize results from the LLM batch processing
                            var finalItemNumber = (item.ItemNumber == "NEW" || !Regex.IsMatch(item.ItemNumber, ItemNumberPattern))
                                                    ? batchValues.ItemNumber // Use generated if input was NEW or invalid
                                                    : item.ItemNumber;      // Use original sanitized if it was valid

                            var finalTariffCode = (string.IsNullOrWhiteSpace(item.TariffCode) || item.TariffCode == "00000000")
                                                    ? batchValues.TariffCode // Use LLM result if input was empty/default
                                                    : item.TariffCode;       // Use sanitized input if it was provided

                            // Add the complete result (including category info from batch) to the dictionary
                            result[originalDescription] = (finalItemNumber, originalDescription, finalTariffCode, batchValues.Category, batchValues.CategoryTariffCode);
                            _logger.LogTrace("Batch success for: '{Desc}' -> Code: {Code}, HS: {HS}, Cat: {Cat}, CatHS: {CatHS}",
                                TruncateForLog(originalDescription, 50), finalItemNumber, finalTariffCode, batchValues.Category, batchValues.CategoryTariffCode);
                        }
                        else
                        {
                            // Item was in the chunk sent, but not in the response (maybe LLM truncated?)
                            _logger.LogWarning("Item '{Description}' not found in batch result for chunk {ChunkIndex}. Attempting individual fallback.", TruncateForLog(originalDescription, 50), chunkIndex);
                            await ProcessSingleItemFallback(item, result, cancellationToken);
                        }
                    }
                }
                catch (Exception chunkEx)
                {
                    _logger.LogWarning(chunkEx, "Chunk {ChunkIndex} processing failed entirely. Falling back to individual item processing for this chunk.", chunkIndex);
                    // If the whole chunk fails (e.g., API error, parsing error), process each item individually
                    foreach (var item in chunkList)
                    {
                        // Check if already processed by a previous successful batch (shouldn't happen often here, but safe check)
                        if (!result.ContainsKey(item.ItemDescription))
                        {
                            await ProcessSingleItemFallback(item, result, cancellationToken);
                        }
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Cancellation requested during batch processing. Aborting further chunks.");
                    break; // Exit the loop if cancellation is requested
                }
            } // End foreach chunk

            _logger.LogInformation("Finished processing. Total items processed: {ResultCount}/{InitialCount}", result.Count, items.Count);
            return result;
        }


        // --- Private Helper Methods ---

        /// <summary>
        /// Fallback mechanism to process a single item if batch processing fails or misses an item.
        /// Uses the single-item prompt/parsing logic.
        /// </summary>
        private async Task ProcessSingleItemFallback(
            (string ItemNumber, string ItemDescription, string TariffCode) item,
            Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> result,
            CancellationToken cancellationToken)
        {
            var itemNumber = item.ItemNumber;
            var description = item.ItemDescription;
            var originalInputTariff = item.TariffCode; // Keep the originally provided (but sanitized) tariff code

            // Avoid reprocessing if already present
            if (result.ContainsKey(description))
            {
                _logger.LogDebug("Skipping fallback for '{Description}' as it already exists in results.", TruncateForLog(description, 50));
                return;
            }

            _logger.LogDebug("Processing fallback for item: '{Description}'", TruncateForLog(description, 50));

            try
            {
                // Generate Item Number if needed (e.g., was 'NEW')
                // Check if the original item number was 'NEW' or invalid before sanitization
                if (itemNumber == "NEW" || string.IsNullOrWhiteSpace(itemNumber) || !Regex.IsMatch(itemNumber, ItemNumberPattern))
                {
                    itemNumber = await GenerateProductCode(description, cancellationToken);
                    _logger.LogDebug("Generated product code via fallback for '{Desc}': {Code}", TruncateForLog(description, 50), itemNumber);
                }

                // Attempt to get classification info using the single-item method
                // Only call API if the input TariffCode was missing or default '00000000'
                string finalTariffCode = originalInputTariff;
                string category = "N/A_Fallback";
                string categoryHsCode = "00000000";

                if (string.IsNullOrWhiteSpace(finalTariffCode) || finalTariffCode == "00000000")
                {
                    var (retrievedTariff, retrievedCategory, retrievedCategoryHs) = await GetClassificationInfoAsync(description, itemNumber, cancellationToken: cancellationToken);
                    // Use retrieved values if they are valid, otherwise keep defaults/original
                    finalTariffCode = !string.IsNullOrWhiteSpace(retrievedTariff) && retrievedTariff != "00000000" ? retrievedTariff : "00000000"; // Fallback requires 8 zeros if failed
                    category = !string.IsNullOrWhiteSpace(retrievedCategory) ? retrievedCategory : category;
                    categoryHsCode = !string.IsNullOrWhiteSpace(retrievedCategoryHs) ? retrievedCategoryHs : categoryHsCode;
                    _logger.LogDebug("Retrieved classification via fallback for '{Desc}': HS={HS}, Cat={Cat}, CatHS={CatHS}", TruncateForLog(description, 50), finalTariffCode, category, categoryHsCode);
                }
                else
                {
                    _logger.LogDebug("Skipping API call in fallback for '{Desc}' as valid input HS '{InputHS}' was provided.", TruncateForLog(description, 50), finalTariffCode);
                }


                // Add result (or defaults/errors) to the main dictionary
                result[description] = (itemNumber, description, finalTariffCode, category, categoryHsCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process individual item fallback for: {Description}", description);
                // Add error placeholders if fallback fails
                result[description] = (item.ItemNumber == "NEW" ? "ERROR" : item.ItemNumber, // Use original item number if not 'NEW' on error
                                       description,
                                       "ERROR", "ERROR", "ERROR");
            }
        }

        /// <summary>
        /// Processes a single chunk of items using the batch prompt.
        /// </summary>
        /// <returns>Dictionary mapping original description to batch results.</returns>
        private async Task<Dictionary<string, (string ItemNumber, string TariffCode, string Category, string CategoryTariffCode)>> ProcessChunk(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
            CancellationToken cancellationToken)
        {
            if (items == null || !items.Any()) return new Dictionary<string, (string ItemNumber, string TariffCode, string Category, string CategoryTariffCode)>();

            var batchPrompt = CreateBatchPrompt(items);
            // Estimate response size based on items + expected JSON overhead per item
            var estimatedResponseTokensPerItem = 50; // Guess: description, code, cat, caths, hs, json syntax
            var estimatedTotalResponseTokens = items.Count * estimatedResponseTokensPerItem;
            var responseMaxTokens = CalculateSafeMaxTokens(batchPrompt, estimatedTotalResponseTokens); // Calculate max_tokens for the response

            _logger.LogDebug("Processing batch with estimated prompt tokens: {EstimatedTokens}, calculated response max_tokens: {ResponseMaxTokens}", EstimateTokenCount(batchPrompt), responseMaxTokens);

            var jsonResponseContent = await GetCompletionAsync(batchPrompt, DefaultTemperature, responseMaxTokens, cancellationToken);

            return ParseBatchResponse(jsonResponseContent); // Parse the batch response JSON
        }

        /// <summary>
        /// Estimates the number of tokens based on approximate character count.
        /// </summary>
        private int EstimateTokenCount(string text)
        {
            // Very rough estimate: ~3-4 chars per token on average for English text/code
            return string.IsNullOrEmpty(text) ? 0 : (int)Math.Ceiling(text.Length / 3.0);
        }

        /// <summary>
        /// Creates the full prompt string for a batch of items.
        /// </summary>
        private string CreateBatchPrompt(List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine(BatchItemPromptTemplate); // Start with the base template and instructions

            // Append the list of items to the prompt
            foreach (var item in items)
            {
                // Sanitize description specifically for the prompt to avoid breaking JSON/API rules
                var safeDesc = SanitizeInputText(item.ItemDescription);
                // Use sanitized item number and input tariff code
                sb.AppendLine($"- DESC: \"{safeDesc}\" | CODE: {item.ItemNumber} | HS_INPUT: {item.TariffCode}");
            }

            sb.AppendLine("Respond ONLY with the complete, valid JSON object containing the 'items' array.");
            return sb.ToString();
        }

        /// <summary>
        /// Attempts to fix common JSON issues like truncation or trailing characters.
        /// </summary>
        /// <param name="jsonContent">The potentially broken JSON string.</param>
        /// <param name="fixApplied">Output parameter indicating if a fix was attempted and potentially successful.</param>
        /// <returns>The potentially fixed JSON string.</returns>
        private string TryFixJson(string jsonContent, out bool fixApplied)
        {
            fixApplied = false;
            if (string.IsNullOrWhiteSpace(jsonContent)) return jsonContent;

            string trimmedJson = jsonContent.Trim();

            // 1. Check if it's already valid JSON
            try
            {
                JObject.Parse(trimmedJson);
                // _logger.LogTrace("Original JSON parsed successfully. No fix needed."); // Too verbose for normal operation
                return trimmedJson; // It's valid, return as is
            }
            catch (JsonReaderException)
            {
                _logger.LogDebug("Original JSON failed to parse. Attempting fixes...");
            }
            catch (Exception ex) // Catch other potential parsing errors
            {
                _logger.LogError(ex, "Unexpected error during initial JSON parse check. Returning original.");
                return trimmedJson;
            }

            // --- Attempt Fixes ---
            string fixedJson = trimmedJson;
            bool currentFixApplied = false;

            // Strategy: Find the logical end of the JSON structure

            int lastValidCharIndex = -1;
            int balance = 0;
            bool inString = false;
            char lastNonWhite = ' ';

            for (int i = 0; i < trimmedJson.Length; i++)
            {
                char c = trimmedJson[i];
                if (c == '"' && (i == 0 || trimmedJson[i - 1] != '\\')) inString = !inString;

                if (!inString)
                {
                    if (c == '{' || c == '[') balance++;
                    else if (c == '}' || c == ']') balance--;

                    if (!char.IsWhiteSpace(c)) lastNonWhite = c;

                    // Potential valid end: balance is 0, not in string, and last char was } or ]
                    if (balance == 0 && (c == '}' || c == ']'))
                    {
                        lastValidCharIndex = i;
                        // We might have multiple points where balance is 0 (e.g., after each item in array).
                        // We want the *last* one corresponding to the main object closing.
                        // Keep updating lastValidCharIndex as long as balance is 0.
                    }
                }
            }

            // If we found a potential end point before the actual end of the string
            if (lastValidCharIndex != -1 && lastValidCharIndex < trimmedJson.Length - 1)
            {
                fixedJson = trimmedJson.Substring(0, lastValidCharIndex + 1);
                _logger.LogInformation("Attempting JSON Fix 1 (Removed trailing characters): Original end: '...{OriginalEnd}', Fixed end: '...{FixedEnd}'", TruncateForLog(trimmedJson, 30), TruncateForLog(fixedJson, 30));
                currentFixApplied = true;
            }
            // If still not valid, check if it seems truncated (missing closing brackets/braces)
            else if (trimmedJson.Contains("\"items\": [") && (!trimmedJson.EndsWith("]}") && !trimmedJson.EndsWith("]}"))) // Common truncation patterns
            {
                // Try adding the most common missing sequence
                string fixAttempt = trimmedJson;
                int openBrackets = fixAttempt.Count(c => c == '[');
                int closeBrackets = fixAttempt.Count(c => c == ']');
                int openBraces = fixAttempt.Count(c => c == '{');
                int closeBraces = fixAttempt.Count(c => c == '}');

                // Add necessary closing brackets/braces intelligently
                if (lastNonWhite == ',')
                { // Remove trailing comma inside last object
                    fixAttempt = fixAttempt.TrimEnd().TrimEnd(',');
                    openBraces--; // Adjust count as we assume the object was trying to close
                }

                // Add missing '}' for items
                while (closeBraces < openBraces) { fixAttempt += '}'; closeBraces++; }
                // Add missing ']' for items array
                while (closeBrackets < openBrackets) { fixAttempt += ']'; closeBrackets++; }
                // Add final '}' for root object if needed (should balance now ideally)
                if (fixAttempt.Count(c => c == '{') > fixAttempt.Count(c => c == '}')) fixAttempt += '}';

                fixedJson = fixAttempt;
                _logger.LogInformation("Attempting JSON Fix 2 (Appended missing brackets/braces): Original end: '...{OriginalEnd}', Fixed end: '...{FixedEnd}'", TruncateForLog(trimmedJson, 30), TruncateForLog(fixedJson, 30));
                currentFixApplied = true;
            }


            // --- Validate the fix attempt ---
            if (currentFixApplied)
            {
                try
                {
                    JObject.Parse(fixedJson);
                    _logger.LogInformation("JSON Fix successful.");
                    fixApplied = true;
                    return fixedJson;
                }
                catch (JsonReaderException)
                {
                    _logger.LogWarning("JSON Fix attempt failed validation. Returning original potentially invalid JSON.");
                    return trimmedJson; // Fix failed, return original
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error validating fixed JSON. Returning original.");
                    return trimmedJson;
                }
            }

            _logger.LogWarning("No applicable JSON fix found or fix failed. Returning original potentially invalid JSON: {JsonContent}", TruncateForLog(trimmedJson));
            return trimmedJson; // Return original if no fixes applied or needed
        }


        /// <summary>
        /// Parses the JSON response from a batch request.
        /// </summary>
        /// <returns>Dictionary mapping original description to parsed results.</returns>
        private Dictionary<string, (string ItemNumber, string TariffCode, string Category, string CategoryTariffCode)> ParseBatchResponse(string jsonContent)
        {
            var result = new Dictionary<string, (string ItemNumber, string TariffCode, string Category, string CategoryTariffCode)>();
            string originalTrimmed = jsonContent?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(originalTrimmed)) { _logger.LogWarning("ParseBatchResponse received empty or null JSON content."); return result; }

            bool fixApplied;
            string jsonToParse = TryFixJson(originalTrimmed, out fixApplied);

            try
            {
                JObject parsedObject = JObject.Parse(jsonToParse);

                if (!(parsedObject["items"] is JArray itemsArray))
                {
                    _logger.LogWarning("Batch response JSON does not contain 'items' array (Fix Applied: {FixApplied}). Content: {JsonContent}", fixApplied, TruncateForLog(jsonToParse));
                    // Attempt fallback regex extraction if primary structure fails but content exists
                    // This is less ideal as it won't get categories reliably
                    // Consider if this fallback is desired or if failure is better
                    return result; // Return empty if structure is wrong
                }

                int itemCount = 0;
                foreach (var itemToken in itemsArray)
                {
                    itemCount++;
                    if (!(itemToken is JObject itemObj))
                    {
                        _logger.LogWarning("Item #{ItemIndex} in batch 'items' array was not a JSON object: {ItemToken}", itemCount, TruncateForLog(itemToken.ToString()));
                        continue; // Skip non-object items
                    }

                    // Extract all fields, including new Category and CategoryTariffCode
                    var originalDescription = itemObj["original_description"]?.Value<string>();
                    var productCode = itemObj["product_code"]?.Value<string>();
                    var hsCode = itemObj["hs_code"]?.Value<string>();
                    var category = itemObj["category"]?.Value<string>();
                    var categoryHsCode = itemObj["category_hs_code"]?.Value<string>();

                    if (string.IsNullOrWhiteSpace(originalDescription))
                    {
                        _logger.LogWarning("Skipping item #{ItemIndex} in batch response due to missing 'original_description'. Item JSON: {ItemJson}", itemCount, TruncateForLog(itemToken.ToString()));
                        continue;
                    }

                    // Sanitize and Validate ALL extracted fields
                    // Use originalDescription as a key for logging context in sanitizers
                    string finalProductCode = SanitizeProductCode(productCode ?? "MISSING"); // Sanitize or mark as missing
                    string finalHsCode = SanitizeTariffCode(originalDescription, hsCode, "ItemHS"); // Sanitize item HS
                    string finalCategory = string.IsNullOrWhiteSpace(category) ? "N/A" : category.Trim(); // Provide default category if missing
                    string finalCategoryHsCode = SanitizeTariffCode(originalDescription, categoryHsCode, "CategoryHS"); // Sanitize category HS

                    // Add to result dictionary using original description as key
                    if (!result.ContainsKey(originalDescription))
                    {
                        result.Add(originalDescription, (finalProductCode, finalHsCode, finalCategory, finalCategoryHsCode));
                        _logger.LogTrace("Parsed item #{ItemIndex}: Desc='{Desc}', Code='{Code}', HS='{HS}', Cat='{Cat}', CatHS='{CatHS}'",
                                itemCount, TruncateForLog(originalDescription, 50), finalProductCode, finalHsCode, finalCategory, finalCategoryHsCode);
                    }
                    else
                    {
                        _logger.LogWarning("Duplicate original_description '{Description}' found in batch response JSON (Item #{ItemIndex}). Keeping the first occurrence.", TruncateForLog(originalDescription, 50), itemCount);
                    }
                } // end foreach item

                if (fixApplied && result.Any()) _logger.LogInformation("Successfully parsed {ItemCount} items after applying JSON fix.", result.Count);
                else if (result.Any()) _logger.LogDebug("Successfully parsed {ItemCount} items from batch response (Fix applied: {FixApplied}).", result.Count, fixApplied);
                else _logger.LogWarning("Parsed JSON but found no valid items in the 'items' array (Fix Applied: {FixApplied}).", fixApplied);

            }
            catch (JsonReaderException jsonEx)
            {
                string errorContext = fixApplied ? $"Original Trimmed: {TruncateForLog(originalTrimmed)}, Attempted Fix: {TruncateForLog(jsonToParse)}" : $"Content: {TruncateForLog(originalTrimmed)}";
                _logger.LogError(jsonEx, "Failed to parse batch response JSON (Fix Applied: {FixApplied}). {ErrorContext}", fixApplied, errorContext);
                // Throw a more specific exception that the calling method (ClassifyItemsAsync) can catch to trigger fallback
                throw new FormatException("Failed to parse batch response JSON from API.", jsonEx);
            }
            catch (Exception ex)
            {
                string errorContext = fixApplied ? $"Original Trimmed: {TruncateForLog(originalTrimmed)}, Attempted Fix: {TruncateForLog(jsonToParse)}" : $"Content: {TruncateForLog(originalTrimmed)}";
                _logger.LogError(ex, "Unexpected error parsing batch response content (Fix Applied: {FixApplied}). {ErrorContext}", fixApplied, errorContext);
                throw; // Re-throw unexpected errors
            }
            return result;
        }

        /// <summary>
        /// Parses the JSON response for a single item request.
        /// </summary>
        /// <returns>Tuple containing (TariffCode, Category, CategoryTariffCode).</returns>
        private (string TariffCode, string Category, string CategoryTariffCode) ParseSingleItemResponse(string jsonContent)
        {
            string tariffCode = "00000000";
            string category = "N/A";
            string categoryHsCode = "00000000";

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                _logger.LogWarning("ParseSingleItemResponse received empty content.");
                return (tariffCode, category, categoryHsCode);
            }

            // Attempt to fix JSON before parsing
            bool fixApplied;
            string jsonToParse = TryFixJson(jsonContent.Trim(), out fixApplied);


            try
            {
                JObject parsedObject = JObject.Parse(jsonToParse);
                if (parsedObject["items"] is JArray itemsArray && itemsArray.Count > 0 && itemsArray[0] is JObject itemObj)
                {
                    var hsCodeRaw = itemObj["hs_code"]?.Value<string>();
                    var categoryRaw = itemObj["category"]?.Value<string>();
                    var categoryHsCodeRaw = itemObj["category_hs_code"]?.Value<string>();

                    // Use the first item found
                    // Sanitize results (using placeholder "SingleItem" for context item number)
                    tariffCode = SanitizeTariffCode("SingleItem", hsCodeRaw, "ItemHS");
                    category = string.IsNullOrWhiteSpace(categoryRaw) ? "N/A" : categoryRaw.Trim();
                    categoryHsCode = SanitizeTariffCode("SingleItem", categoryHsCodeRaw, "CategoryHS");

                    _logger.LogDebug("Parsed single item response (Fix Applied: {FixApplied}): HS={HS}, Cat={Cat}, CatHS={CatHS}", fixApplied, tariffCode, category, categoryHsCode);
                }
                else
                {
                    _logger.LogWarning("Could not find valid 'items' array structure in single item JSON response (Fix Applied: {FixApplied}). JSON: {Json}", fixApplied, TruncateForLog(jsonToParse));
                    // Fallback: Try regex only for the specific HS code if JSON parsing fails structurally
                    tariffCode = ParseHsCodeFromText(jsonToParse); // Keep category info as default/NA
                }
            }
            catch (JsonReaderException jsonEx)
            {
                _logger.LogWarning(jsonEx, "Failed to parse JSON for single item (Fix Applied: {FixApplied}), falling back to regex for HS code only. Content: {JsonContent}", fixApplied, TruncateForLog(jsonToParse));
                tariffCode = ParseHsCodeFromText(jsonToParse); // Fallback to regex only for HS code
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error parsing single item JSON (Fix Applied: {FixApplied}). Content: {JsonContent}", fixApplied, TruncateForLog(jsonToParse));
                // Return error indicators
                return ("ERROR", "ERROR", "ERROR");
            }

            return (tariffCode, category, categoryHsCode);
        }


        /// <summary>
        /// Sanitizes text input for use in prompts or descriptions.
        /// </summary>
        private string SanitizeInputText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Truncate first to avoid processing huge strings
            string truncated = input.Length > MaxDescriptionLength
                                ? input.Substring(0, MaxDescriptionLength)
                                : input;

            // Replace characters potentially problematic for JSON or API, normalize whitespace
            string sanitized = truncated.Replace("\"", "'")       // Replace double quotes with single
                                      .Replace("\\", "\\\\")   // Escape backslashes
                                      .Replace("\n", " ")      // Replace newlines with space
                                      .Replace("\r", " ")      // Replace carriage returns with space
                                      .Trim();                // Trim leading/trailing whitespace

            // Remove characters not explicitly allowed by SanitizePattern
            // (Allows letters, numbers, punctuation, symbols, whitespace)
            sanitized = Regex.Replace(sanitized, SanitizePattern, "");

            // Ensure it's not completely empty after sanitization
            return string.IsNullOrWhiteSpace(sanitized) ? "SANITIZED_EMPTY" : sanitized;
        }

        /// <summary>
        /// Sanitizes an item number to meet specific criteria (alphanumeric, hyphen, max length).
        /// Returns "NEW" if input is null, empty, or invalid after cleaning.
        /// </summary>
        private string SanitizeItemNumber(string itemNumber)
        {
            if (string.IsNullOrWhiteSpace(itemNumber)) return "NEW";

            // Remove invalid characters (anything not alphanumeric or hyphen)
            var cleaned = Regex.Replace(itemNumber, @"[^\w-]", "").Trim();

            // Check length constraints
            if (cleaned.Length == 0 || cleaned.Length > 20)
            {
                if (cleaned.Length > 20)
                    _logger.LogTrace("Sanitizing ItemNumber: Input '{Input}' was too long, truncated.", itemNumber);
                else
                    _logger.LogTrace("Sanitizing ItemNumber: Input '{Input}' became empty after cleaning.", itemNumber);
                return "NEW"; // Return NEW if empty or too long after cleaning
            }

            // Final check against the regex pattern (redundant if cleaning is correct, but safe)
            if (!Regex.IsMatch(cleaned, ItemNumberPattern))
            {
                _logger.LogTrace("Sanitizing ItemNumber: Input '{Input}' failed pattern match after cleaning ('{Cleaned}').", itemNumber, cleaned);
                return "NEW";
            }

            return cleaned.ToUpperInvariant(); // Return valid, cleaned, uppercase code
        }

        /// <summary>
        /// Sanitizes a tariff code (HS code) to be exactly 8 digits.
        /// Logs warnings if modifications are needed.
        /// </summary>
        /// <param name="contextIdentifier">Identifier for logging (e.g., ItemNumber or Description).</param>
        /// <param name="tariffCode">The input tariff code string.</param>
        /// <param name="fieldName">Name of the field being sanitized (for logging).</param>
        /// <returns>An 8-digit string or "00000000".</returns>
        private string SanitizeTariffCode(string contextIdentifier, string tariffCode, string fieldName = "HSCode")
        {
            if (string.IsNullOrWhiteSpace(tariffCode)) return "00000000";

            var cleaned = Regex.Replace(tariffCode, @"\D", ""); // Remove all non-digits

            if (string.IsNullOrEmpty(cleaned))
            {
                _logger.LogTrace("Invalid {FieldName} (empty after removing non-digits: '{TariffCode}') for Context '{Context}'. Returning default.", fieldName, tariffCode, contextIdentifier);
                return "00000000";
            }

            if (cleaned.Length == 8)
            {
                // If it's 8 digits, check if the original *was different* (meaning it contained non-digits)
                if (cleaned != tariffCode.Trim())
                {
                    _logger.LogTrace("Sanitized {FieldName} (removed non-digits from '{TariffCode}') for Context '{Context}'. Using '{CleanedValue}'.", fieldName, tariffCode, contextIdentifier, cleaned);
                }
                else
                {
                    _logger.LogTrace("Valid {FieldName} format '{TariffCode}' for Context '{Context}'.", fieldName, tariffCode, contextIdentifier);
                }
                return cleaned; // Return the valid 8-digit code
            }
            else if (cleaned.Length < 8)
            {
                _logger.LogWarning("Invalid {FieldName} format (too short: '{TariffCode}', cleaned: '{Cleaned}') for Context '{Context}'. Padding with zeros.", fieldName, tariffCode, cleaned, contextIdentifier);
                return cleaned.PadRight(8, '0');
            }
            else // cleaned.Length > 8
            {
                _logger.LogWarning("Invalid {FieldName} format (too long: '{TariffCode}', cleaned: '{Cleaned}') for Context '{Context}'. Truncating.", fieldName, tariffCode, cleaned, contextIdentifier);
                return cleaned.Substring(0, 8);
            }
        }


        /// <summary>
        /// Generates a product code based on the description using the LLM (simple request).
        /// </summary>
        private async Task<string> GenerateProductCode(string description, CancellationToken cancellationToken)
        {
            const string promptTemplate = @"Generate a concise, descriptive product code (max 20 chars) for the item description below.
Rules: Use ONLY uppercase letters (A-Z), numbers (0-9), and hyphens (-). Must be between 3 and 20 characters long.
Description: {0}
Respond ONLY with the generated product code, nothing else.";

            if (string.IsNullOrWhiteSpace(description))
            {
                _logger.LogWarning("GenerateProductCode called with empty description.");
                return "GENERATED-CODE"; // Return a default placeholder
            }

            try
            {
                var cleanDesc = SanitizeInputText(description); // Sanitize description for the prompt
                var prompt = string.Format(promptTemplate, cleanDesc);

                // Use lower temp for more predictable code generation, low max tokens
                var response = await GetCompletionAsync(prompt, maxTokens: 30, temperature: 0.2, cancellationToken: cancellationToken);

                // Sanitize the LLM's response to fit the rules
                return SanitizeProductCode(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate product code via LLM for Description: {Description}", description);
                return "ERROR-CODE"; // Indicate failure
            }
        }

        /// <summary>
        /// Sanitizes a generated or input product code string to meet the required format.
        /// </summary>
        private string SanitizeProductCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "MISSING-CODE";

            // Aggressive cleaning: remove anything not letter, number, or hyphen
            var sanitized = Regex.Replace(code.Trim(), @"[^a-zA-Z0-9-]", "");
            // Remove leading/trailing hyphens and consecutive hyphens
            sanitized = Regex.Replace(sanitized, @"-+", "-").Trim('-');
            sanitized = sanitized.ToUpperInvariant(); // Convert to uppercase

            // Check length after cleaning
            if (sanitized.Length < 3) return "SHORT-CODE"; // Too short
            if (sanitized.Length > 20) sanitized = sanitized.Substring(0, 20).TrimEnd('-'); // Truncate if too long

            // Final check if truncation made it too short
            if (sanitized.Length < 3) return "SHORT-CODE";

            // Final pattern check (should pass if logic above is correct)
            if (!Regex.IsMatch(sanitized, ItemNumberPattern)) return "INVALID-CODE";

            return sanitized;
        }

        /// <summary>
        /// Core method to send a request to the DeepSeek API completion endpoint.
        /// </summary>
        private async Task<string> GetCompletionAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("GetCompletionAsync called with empty prompt.");
                return string.Empty;
            }

            // Ensure maxTokens is calculated safely
            int safeMaxTokens = CalculateSafeMaxTokens(prompt, maxTokens);

            var requestBody = new
            {
                model = Model,
                messages = new[] { new { role = "user", content = prompt } },
                temperature = temperature ?? DefaultTemperature,
                max_tokens = safeMaxTokens,
                stream = false // Not using streaming responses here
            };

            _logger.LogTrace("Sending API request. Model: {Model}, Temp: {Temperature}, MaxTokens: {MaxTokens}", Model, requestBody.temperature, requestBody.max_tokens);
            // Log prompt hash or length instead of full prompt for production to avoid leaking data
            _logger.LogDebug("Prompt Length: {PromptLength} chars", prompt.Length);

            try
            {
                // Execute the POST request using the retry policy
                var jsonResponse = await PostRequestAsync(requestBody, cancellationToken).ConfigureAwait(false);

                // Parse the 'content' field from the response JSON
                return ParseCompletionResponse(jsonResponse);
            }
            // Catch specific exceptions that might be thrown by PostRequestAsync or ParseCompletionResponse
            catch (RateLimitException rle)
            {
                // Already logged in retry policy, just rethrow if needed by caller
                _logger.LogError(rle, "API rate limit exceeded even after retries. Status: {StatusCode}", rle.StatusCode);
                throw; // Rethrow to indicate failure
            }
            catch (HttpRequestException httpEx)
            {
                var sc = httpEx.Data.Contains("StatusCode") ? httpEx.Data["StatusCode"] : "N/A";
                _logger.LogError(httpEx, "HTTP request failed even after retries. Status: {StatusCode}", sc);
                throw new HSCodeRequestException($"API request failed after retries.", httpEx);
            }
            catch (TaskCanceledException tcEx)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("API request cancelled by user.");
                    throw; // Propagate cancellation
                }
                else
                {
                    _logger.LogError(tcEx, "API request timed out even after retries.");
                    throw new TimeoutException("API request timed out after retries.", tcEx);
                }
            }
            catch (JsonException jsonEx)
            { // Catch parsing errors from ParseCompletionResponse
                _logger.LogError(jsonEx, "Failed to parse JSON content from API response.");
                throw new FormatException("Invalid JSON content received from API.", jsonEx);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors during the process
                _logger.LogError(ex, "An unexpected error occurred during API completion request.");
                throw new HSCodeRequestException("An unexpected error occurred during API completion.", ex);
            }
        }

        /// <summary>
        /// Calculates a safe value for max_tokens based on prompt length and limits.
        /// </summary>
        private int CalculateSafeMaxTokens(string prompt, int? requestedMax)
        {
            const int MIN_RESPONSE_TOKENS = 50; // Minimum tokens to allow for a basic response
            const int ABSOLUTE_MAX_RESPONSE = MAX_TOKENS_PER_REQUEST - TOKEN_BUFFER - 100; // Hard cap on response size

            var promptTokenEstimate = EstimateTokenCount(prompt);

            // Calculate tokens available for the response
            var availableForResponse = MAX_TOKENS_PER_REQUEST - promptTokenEstimate - TOKEN_BUFFER;

            // Ensure we allow at least a minimum number of tokens for the response
            if (availableForResponse < MIN_RESPONSE_TOKENS)
            {
                _logger.LogWarning("Prompt tokens ({PromptTokens}) + buffer ({TokenBuffer}) leaves less than minimum ({MinTokens}) for response. Forcing minimum.",
                    promptTokenEstimate, TOKEN_BUFFER, MIN_RESPONSE_TOKENS);
                availableForResponse = MIN_RESPONSE_TOKENS;
            }

            // Determine the desired max tokens: either the requested value or a default/calculated one
            // If requestedMax is null, use a reasonable default like DefaultMaxTokens or a larger value based on available space.
            // Let's use a larger default for batch-calculated scenarios if not specified.
            int targetMaxTokens = requestedMax ?? Math.Max(DefaultMaxTokens, availableForResponse / 2); // Use half of available if nothing else specified

            // Final value is the minimum of the target, the available space, and the absolute max response size
            int finalMaxTokens = Math.Min(targetMaxTokens, availableForResponse);
            finalMaxTokens = Math.Min(finalMaxTokens, ABSOLUTE_MAX_RESPONSE); // Apply absolute cap

            // Ensure it's never negative or zero
            finalMaxTokens = Math.Max(MIN_RESPONSE_TOKENS, finalMaxTokens);


            _logger.LogTrace("CalculateSafeMaxTokens: PromptEst={PromptEst}, Requested={Req}, Available={Avail}, Target={Tgt}, Final={Final}",
                promptTokenEstimate, requestedMax?.ToString() ?? "null", availableForResponse, targetMaxTokens, finalMaxTokens);

            return finalMaxTokens;
        }

        /// <summary>
        /// Extracts the main 'content' string from the API's JSON response.
        /// </summary>
        private string ParseCompletionResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogWarning("ParseCompletionResponse received empty JSON.");
                return string.Empty;
            }
            try
            {
                JObject responseObj = JObject.Parse(jsonResponse);

                // Navigate safely through the expected JSON structure
                var content = responseObj?["choices"]?.FirstOrDefault()?["message"]?["content"]?.Value<string>();

                if (content == null)
                {
                    _logger.LogWarning("Could not find 'choices[0].message.content' in API response JSON.");
                    _logger.LogDebug("Response JSON structure: {JsonResponse}", TruncateForLog(jsonResponse));
                    // Check for error messages in the response
                    var error = responseObj?["error"]?["message"]?.Value<string>();
                    if (!string.IsNullOrEmpty(error))
                    {
                        _logger.LogError("API returned an error message: {ErrorMessage}", error);
                        throw new HSCodeRequestException($"API returned an error: {error}");
                    }
                    return string.Empty; // Return empty if content not found
                }

                // Basic sanitization: remove markdown code fences if present
                return SanitizeApiResponse(content);
            }
            // Let JsonReaderException propagate up to GetCompletionAsync for specific handling
            catch (JsonReaderException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to parse API response JSON. Response: {JsonResponse}", TruncateForLog(jsonResponse));
                throw; // Rethrow to be caught by GetCompletionAsync
            }
            catch (Exception ex)
            {
                // Catch other potential errors during parsing/accessing fields
                _logger.LogError(ex, "Error accessing content in API response JSON. Response: {JsonResponse}", TruncateForLog(jsonResponse));
                throw; // Rethrow unexpected errors
            }
        }

        /// <summary>
        /// Performs basic cleaning on the raw content string from the API response.
        /// (e.g., removing markdown code fences).
        /// </summary>
        private string SanitizeApiResponse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent)) return string.Empty;

            // Remove ```json and ``` markers, trim whitespace
            var sanitized = Regex.Replace(responseContent, @"^```(json)?\s*|\s*```$", "", RegexOptions.IgnoreCase).Trim();

            // Simple check for obviously truncated JSON - this is less reliable than TryFixJson
            // TryFixJson is called later during specific JSON parsing (ParseBatchResponse/ParseSingleItemResponse)
            // if (sanitized.StartsWith("{") && !sanitized.EndsWith("}"))
            // {
            //     _logger.LogDebug("API response content appears truncated (starts with '{{' but doesn't end with '}}'). Full fixing will be attempted during parsing.");
            // }

            return sanitized;
        }


        /// <summary>
        /// Executes the HTTP POST request with retry logic.
        /// Throws specific exceptions on failure conditions handled by the retry policy.
        /// </summary>
        private async Task<string> PostRequestAsync(object requestBody, CancellationToken cancellationToken = default)
        {
            // Execute the request within the Polly retry policy
            // Pass the CancellationToken to the ExecuteAsync lambda
            return await _retryPolicy.ExecuteAsync(async (ct) => {
                string jsonRequest = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                HttpResponseMessage response = null; // Declare here to ensure disposal in finally
                try
                {
                    using (var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions") { Content = content })
                    {
                        _logger.LogDebug("Executing HTTP POST to {Url}. Request size: {Size} bytes.", requestMessage.RequestUri, jsonRequest.Length);
                        // Pass CancellationToken (ct from Polly context) to SendAsync
                        response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

                        string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false); // Read content regardless of status for potential error details

                        // --- Check status codes for triggering Polly retries ---
                        if (response.StatusCode == (HttpStatusCode)429)
                        { // Too Many Requests
                            _logger.LogDebug("PostRequestAsync: Throwing RateLimitException for status 429.");
                            throw new RateLimitException((int)response.StatusCode, responseContent);
                        }
                        // Server errors or specific client errors like timeout
                        if (response.StatusCode >= HttpStatusCode.InternalServerError ||
                            response.StatusCode == HttpStatusCode.RequestTimeout ||
                            response.StatusCode == HttpStatusCode.GatewayTimeout ||
                            response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            var httpEx = new HttpRequestException($"API request failed with retryable status {(int)response.StatusCode}. Response: {TruncateForLog(responseContent)}", null); // Inner exception is null here
                            httpEx.Data["StatusCode"] = (int)response.StatusCode;
                            _logger.LogDebug(httpEx, "PostRequestAsync: Throwing HttpRequestException for retryable status {StatusCode}.", (int)response.StatusCode);
                            throw httpEx;
                        }

                        // --- Handle non-retryable errors ---
                        if (!response.IsSuccessStatusCode)
                        {
                            // Log the error details and throw an exception that won't be retried by the current policy
                            var finalHttpEx = new HttpRequestException($"API request failed with non-retryable status {(int)response.StatusCode}: {TruncateForLog(responseContent)}");
                            finalHttpEx.Data["StatusCode"] = (int)response.StatusCode;
                            _logger.LogError(finalHttpEx, "PostRequestAsync: Unhandled non-success status code {StatusCode}. Not retrying.", (int)response.StatusCode);
                            throw finalHttpEx; // This exception type is NOT handled by the retry policy, so it surfaces immediately
                        }

                        // --- Success ---
                        _logger.LogDebug("PostRequestAsync: API request successful (Status {StatusCode}). Response size: {Size} bytes.", (int)response.StatusCode, responseContent.Length);
                        return responseContent; // Return the successful response content
                    }
                }
                finally
                {
                    // Ensure the HttpResponseMessage is disposed even if exceptions occur before reading content
                    response?.Dispose();
                }
            }, cancellationToken).ConfigureAwait(false); // Pass the original CancellationToken here for overall operation cancellation
        }

        /// <summary>
        /// Fallback HS code extraction using Regex if JSON parsing fails.
        /// Less reliable than JSON parsing.
        /// </summary>
        private string ParseHsCodeFromText(string textContent)
        {
            if (string.IsNullOrWhiteSpace(textContent)) return "00000000"; // Return default if empty
            try
            {
                // Look for an 8-digit number, possibly surrounded by quotes or whitespace
                var pattern = @"""?(\d{8})""?"; // Relaxed pattern to catch codes within quotes potentially
                var match = Regex.Match(textContent, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    var code = match.Groups[1].Value;
                    _logger.LogDebug("Extracted HS code using Regex fallback: {HsCode}", code);
                    // Final validation just to be sure
                    return SanitizeTariffCode("RegexFallback", code, "ItemHS_Regex");
                }
                _logger.LogWarning("Could not find 8-digit HS code using Regex pattern '{Pattern}' in text: {Text}", pattern, TruncateForLog(textContent, 100));
                return "00000000"; // Return default if no match
            }
            catch (RegexMatchTimeoutException ex)
            {
                _logger.LogError(ex, "Regex timed out parsing HS code from text.");
                return "00000000";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Regex HS code parsing from text.");
                return "00000000";
            }
        }

        /// <summary>
        /// Helper to truncate strings for logging.
        /// </summary>
        private string TruncateForLog(string text, int maxLength = 250) // Reduced default length for logs
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...(truncated)";
        }

        // --- Custom Exception Classes ---
        public class RateLimitException : HttpRequestException
        {
            public int StatusCode { get; }
            public RateLimitException(int statusCode, string message) : base($"Rate limit exceeded (Status {statusCode}): {message}") { StatusCode = statusCode; }
            public RateLimitException(int statusCode, string message, Exception inner) : base($"Rate limit exceeded (Status {statusCode}): {message}", inner) { StatusCode = statusCode; }
        }
        public class HSCodeRequestException : Exception
        {
            public HSCodeRequestException(string message) : base(message) { }
            public HSCodeRequestException(string message, Exception inner) : base(message, inner) { }
        }

        // --- Chunking Helper ---
        private static IEnumerable<IEnumerable<T>> ChunkBy<T>(IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be positive.");
            if (source == null) throw new ArgumentNullException(nameof(source));
            return ChunkByIterator(source, chunkSize);
        }
        private static IEnumerable<IEnumerable<T>> ChunkByIterator<T>(IEnumerable<T> source, int chunkSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext()) { yield return GetChunk(enumerator, chunkSize); }
            }
        }
        private static IEnumerable<T> GetChunk<T>(IEnumerator<T> enumerator, int chunkSize)
        {
            var chunk = new List<T>(chunkSize); // Pre-allocate list capacity
            chunk.Add(enumerator.Current); // Add the first item that MoveNext() already found
            for (int i = 1; i < chunkSize && enumerator.MoveNext(); i++) { chunk.Add(enumerator.Current); }
            return chunk;
        }

        // --- Dispose Pattern ---
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    _httpClient?.Dispose();
                    _logger.LogDebug("DeepSeekApi HttpClient disposed.");
                }
                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null
                disposedValue = true;
            }
        }
        // Do not need a finalizer (~DeepSeekApi()) unless you have unmanaged resources

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this); // Suppress finalization as Dispose does the cleanup
        }
    }
}