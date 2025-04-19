﻿﻿﻿using Microsoft.Extensions.Logging;
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
        private const int MAX_ITEMS_PER_BATCH = 5;
        private const int TOKEN_BUFFER = 512;
        private const int FALLBACK_MAX_TOKENS = 100;

        private readonly HttpClient _httpClient;
        private readonly ILogger<DeepSeekApi> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public string PromptTemplate { get; set; }
        public string Model { get; set; } = "deepseek-chat";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 150;
        public string HsCodePattern { get; set; } = @"\b(\d{8})\b";
        public int MaxDescriptionLength { get; set; } = 500;
        public string SanitizePattern { get; set; } = @"[^\p{L}\p{N}\p{P}\p{S}\s]";
        public string ItemNumberPattern { get; set; } = @"^[\w-]{1,20}$";
        public string TariffCodePattern { get; set; } = @"^\d{8}$";

        private readonly AsyncRetryPolicy _retryPolicy;

        public DeepSeekApi()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("WaterNut", LogLevel.Debug)
                    .AddConsole()
                    .AddDebug();
            });
            _logger = loggerFactory.CreateLogger<DeepSeekApi>();
            _logger.LogDebug("Initializing DeepSeekApi...");

            _apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
                      ?? throw new InvalidOperationException("API key 'DEEPSEEK_API_KEY' not found in environment variables.");

            _retryPolicy = CreateRetryPolicy(); // Called here
            _baseUrl = "https://api.deepseek.com/v1";
            _httpClient = CreateHttpClient();

            SetupHttpClient();
            SetDefaultPrompt();
            _logger.LogInformation("DeepSeekApi initialized successfully.");
        }

        public DeepSeekApi(ILogger<DeepSeekApi> logger, string apiKey)
        {
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DeepSeekApi>.Instance;
            _apiKey = !string.IsNullOrWhiteSpace(apiKey) ? apiKey
                      : throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");
            _logger.LogDebug("Initializing DeepSeekApi with provided logger and API key...");

            _retryPolicy = CreateRetryPolicy(); // Called here
            _baseUrl = "https://api.deepseek.com/v1";
            _httpClient = CreateHttpClient();

            SetupHttpClient();
            SetDefaultPrompt();
            _logger.LogInformation("DeepSeekApi initialized successfully with provided logger.");
        }

        // --- FINAL CreateRetryPolicy version based on compiler errors ---
        private AsyncRetryPolicy CreateRetryPolicy()
        {
            // Standard Exponential Backoff (only depends on retry count)
            Func<int, TimeSpan> calculateDelay = (retryAttempt) =>
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                // Simple trace logging for delay calculation is fine here
                _logger.LogTrace("Retry attempt {RetryCount}: Calculated delay {DelaySeconds}s", retryAttempt, delay.TotalSeconds);
                return delay;
            };

            // Exception-Specific Logging (performed in onRetry)
            Action<Exception, TimeSpan> logRetryAction = (exception, calculatedDelay) =>
            {
                // Log *based on the exception type* received here.
                // The logging level (Warning) indicates a recoverable issue is happening.
                if (exception is RateLimitException rle)
                {
                    _logger.LogWarning(rle, // Pass the specific exception
                        "Retry needed due to Rate Limit (HTTP {StatusCode}). Delaying for {DelaySeconds}s...",
                        rle.StatusCode,
                        calculatedDelay.TotalSeconds);
                }
                else if (exception is TaskCanceledException tce)
                {
                    // Distinguish timeout from user cancellation if possible
                    if (tce.CancellationToken.IsCancellationRequested)
                    {
                        // Logged as Warning because a retry is happening, though maybe unexpected
                        _logger.LogWarning(tce, "Retry triggered for Task Cancellation (possibly user initiated). Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                    else
                    {
                        // This is likely a timeout, which is expected to be retried
                        _logger.LogWarning(tce, "Retry needed due to operation Timeout. Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                }
                else if (exception is HttpRequestException httpEx && httpEx.Data.Contains("StatusCode"))
                {
                    _logger.LogWarning(httpEx, // Pass the specific exception
                        "Retry needed due to Server Error (HTTP {StatusCode}). Delaying for {DelaySeconds}s...",
                        httpEx.Data["StatusCode"],
                        calculatedDelay.TotalSeconds);
                }
                else // Default logging for other handled exceptions
                {
                    _logger.LogWarning(exception, // Pass the general exception
                        "Retry needed due to handled Transient Error ({ExceptionType}). Delaying for {DelaySeconds}s...",
                        exception?.GetType().Name ?? "Unknown",
                        calculatedDelay.TotalSeconds);
                }
            };

            // Build the Policy using the simpler overload signatures
            return Policy
               .Handle<RateLimitException>()
               .Or<HttpRequestException>(ex => ex.Data.Contains("StatusCode") && ((int)ex.Data["StatusCode"] >= 500 || (int)ex.Data["StatusCode"] == (int)HttpStatusCode.RequestTimeout))
               // Important: Let's ONLY retry TaskCanceledException if it's NOT due to user cancellation.
               .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested)
               .WaitAndRetryAsync(
                   retryCount: 3,
                   sleepDurationProvider: calculateDelay, // Use Func<int, TimeSpan>
                   onRetry: logRetryAction               // Use Action<Exception, TimeSpan>
               );
        }
        // --- End CreateRetryPolicy ---

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                MaxConnectionsPerServer = 20,
                UseProxy = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromSeconds(300)
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

        private void SetDefaultPrompt()
        {
            // REMOVED {0} and {1} placeholders
            PromptTemplate = @"Analyze the product description provided below and return ONLY a single, valid JSON object containing the HS code.
Follow these rules strictly:
1. The JSON object must match this exact format:
{{ // Escape literal braces for clarity if needed, though not strictly required if not using string.Format
    ""items"": [
        {{
            ""original_description"": ""EXACT_INPUT_DESCRIPTION"",
            ""product_code"": ""INPUT_CODE_OR_GENERATED"",
            ""hs_code"": ""00000000"" // Exactly 8 digits, no punctuation.
        }}
    ]
}} // Escaped brace
2. Preserve the ""original_description"" EXACTLY as provided in the input. Do not modify it.
3. The ""hs_code"" MUST be exactly 8 digits (0-9). No dots, dashes, or letters. If unsure, provide the best estimate.
4. Ensure the entire response is *only* the valid JSON structure specified. No introductory text, explanations, apologies, or markdown formatting (like ```json).

Product Information:
Description: __DESCRIPTION_HERE__
Product Code (Optional): __PRODUCT_CODE_HERE__";
            // Using placeholders like __DESCRIPTION_HERE__ is optional, just for readability in the template.
            // You could also just construct the final string part directly in GetTariffCode.
            // Escaping the JSON braces {{ }} isn't strictly needed now but doesn't hurt.
        }

        public async Task<string> GetTariffCode(string itemDescription, string productCode = null,
            double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(itemDescription))
            {
                _logger.LogWarning("GetTariffCode called with empty description.");
                return "";
            }

            try
            {
                var cleanDescription = SanitizeInputText(itemDescription);
                var finalProductCode = productCode ?? "N/A"; // Handle null productCode

                // --- Use String Interpolation ---
                // Construct the final prompt string.
                // NOTE: Assumes PromptTemplate has NO {0}, {1} anymore.
                // We manually append the specific product info part.
                var prompt = $@"{PromptTemplate}

Product Information:
Description: {cleanDescription}
Product Code (Optional): {finalProductCode}";
                // --- End String Interpolation ---


                // --- Alternative Construction (if you removed the "Product Information" section from the template): ---
                /*
                var prompt = $@"{PromptTemplate} // Template contains only general instructions and JSON format now
Product Information:
Description: {cleanDescription}
Product Code (Optional): {finalProductCode}";
                */
                // --- End Alternative ---


                // Log the final prompt for debugging if needed (be careful with sensitive data)
                // _logger.LogDebug("Generated Prompt for GetTariffCode: {Prompt}", prompt);

                var jsonResponseContent =
                    await GetCompletionAsync(prompt, temperature, maxTokens ?? DefaultMaxTokens, cancellationToken)
                        .ConfigureAwait(false);

                return ParseSingleHsCodeFromJson(jsonResponseContent);
            }
            catch (Exception ex) when (!(ex is HSCodeRequestException))
            {
                _logger.LogError(ex, "Failed to retrieve HS code for Description: {ItemDescription}", itemDescription);
                throw new HSCodeRequestException($"Failed to retrieve HS code for '{itemDescription}'", ex);
            }
            // No need for explicit catch/rethrow of HSCodeRequestException
        }

        public async Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode)>>
            ClassifyItemsAsync(
                List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
                CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<string, (string, string, string)>();
            if (items == null || !items.Any()) return result;

            var processedItems = items
                .Where(i => !string.IsNullOrWhiteSpace(i.ItemDescription))
                .Select(i => (
                    ItemNumber: SanitizeItemNumber(i.ItemNumber),
                    ItemDescription: i.ItemDescription,
                    TariffCode: SanitizeTariffCode(i.ItemNumber, i.TariffCode)
                 )).ToList();

            var chunks = ChunkBy(processedItems, MAX_ITEMS_PER_BATCH).ToList();
            _logger.LogInformation("Processing {ItemCount} items in {ChunkCount} chunks.", processedItems.Count, chunks.Count);

            int chunkIndex = 0;
            foreach (var chunk in chunks)
            {
                chunkIndex++;
                _logger.LogDebug("Processing chunk {ChunkIndex}/{ChunkCount} with {ItemCount} items.", chunkIndex, chunks.Count, chunk.Count());
                var chunkList = chunk.ToList();

                try
                {
                    var batchResult = await ProcessChunk(chunkList, cancellationToken);
                    foreach (var item in chunkList)
                    {
                        var originalDescription = item.ItemDescription;
                        if (batchResult.TryGetValue(originalDescription, out var batchValues))
                        {
                            var finalItemNumber = item.ItemNumber == "NEW" ? batchValues.ItemNumber : item.ItemNumber;
                            var finalTariffCode = string.IsNullOrWhiteSpace(item.TariffCode) || item.TariffCode == "00000000" ? batchValues.TariffCode : item.TariffCode;
                            result[originalDescription] = (finalItemNumber, originalDescription, finalTariffCode);
                            _logger.LogDebug("Batch success for: {Description} -> HS: {HSCode}, Code: {ProdCode}", originalDescription, finalTariffCode, finalItemNumber);
                        }
                        else
                        {
                            _logger.LogWarning("Item '{Description}' not found in batch result for chunk {ChunkIndex}, processing individually.", originalDescription, chunkIndex);
                            await ProcessSingleItemFallback(item, result, cancellationToken);
                        }
                    }
                }
                catch (Exception chunkEx)
                {
                    _logger.LogWarning(chunkEx, "Chunk {ChunkIndex} processing failed. Falling back to individual item processing for this chunk.", chunkIndex);
                    foreach (var item in chunkList)
                    {
                        await ProcessSingleItemFallback(item, result, cancellationToken);
                    }
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Cancellation requested during batch processing.");
                    break;
                }
            }
            _logger.LogInformation("Finished processing all items. Results count: {ResultCount}", result.Count);
            return result;
        }

        private async Task ProcessSingleItemFallback(
            (string ItemNumber, string ItemDescription, string TariffCode) item,
            Dictionary<string, (string, string, string)> result,
            CancellationToken cancellationToken)
        {
            var itemNumber = item.ItemNumber;
            var description = item.ItemDescription;
            var tariffCode = item.TariffCode;
            try
            {
                if (itemNumber == "NEW")
                {
                    itemNumber = await GenerateProductCode(description, cancellationToken);
                    _logger.LogDebug("Generated product code for '{Description}': {ProductCode}", description, itemNumber);
                }
                if (string.IsNullOrWhiteSpace(tariffCode) || tariffCode == "00000000")
                {
                    tariffCode = await GetTariffCode(description, itemNumber, cancellationToken: cancellationToken);
                    _logger.LogDebug("Retrieved tariff code for '{Description}': {TariffCode}", description, tariffCode);
                }
                result[description] = (itemNumber, description, tariffCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process individual item fallback: {Description}", description);
                result[description] = (item.ItemNumber == "NEW" ? "ERROR" : item.ItemNumber, description, "ERROR");
            }
        }

        private async Task<Dictionary<string, (string ItemNumber, string TariffCode)>> ProcessChunk(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
            CancellationToken cancellationToken)
        {
            var batchPrompt = CreateBatchPrompt(items);
            var responseMaxTokens = CalculateSafeMaxTokens(batchPrompt, null);
            _logger.LogDebug("Processing batch with estimated prompt tokens: {EstimatedTokens}, calculated response max_tokens: {ResponseMaxTokens}", EstimateTokenCount(batchPrompt), responseMaxTokens);
            var jsonResponseContent = await GetCompletionAsync(batchPrompt, DefaultTemperature, responseMaxTokens, cancellationToken);
            return ParseBatchResponse(jsonResponseContent);
        }

        private int EstimateTokenCount(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : (int)Math.Ceiling(text.Length / 2.5);
        }

        private string CreateBatchPrompt(List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Analyze the list of products below. For each product, determine the 8-digit HS code and use the provided product code (or generate one if missing/invalid).");
            sb.AppendLine("Return ONLY a single, valid JSON object containing an 'items' array. Follow the format and rules strictly.");
            sb.AppendLine("JSON Format:");
            sb.AppendLine(@"{ ""items"": [ { ""original_description"": ""EXACT_DESCRIPTION_FROM_INPUT"", ""product_code"": ""PROVIDED_OR_GENERATED_CODE"", ""hs_code"": ""00000000"" } ] }"); // Compacted
            sb.AppendLine(@"Rules:");
            sb.AppendLine(@"- The entire response MUST be only the JSON object. No extra text, explanations, or markdown.");
            sb.AppendLine(@"- Preserve ""original_description"" EXACTLY as provided in the input list.");
            sb.AppendLine(@"- ""product_code"" should be the code provided in the input. If 'NEW' or invalid, generate a concise code (alphanumeric, hyphens, max 20 chars).");
            sb.AppendLine(@"- ""hs_code"" MUST be exactly 8 digits (0-9). No punctuation.");
            // --- ADD THIS RULE ---
            sb.AppendLine(@"- IMPORTANT: If you are about to exceed the token limit, STOP adding new items. Finish the CURRENT item object completely, then properly CLOSE the 'items' array with ']' and the main JSON object with '}'. Ensure the final output is ALWAYS valid JSON.");
            // --------------------
            sb.AppendLine(@"- No trailing commas.");
            sb.AppendLine(@"Product List:");
            foreach (var item in items)
            {
                var safeDesc = SanitizeInputText(item.ItemDescription);
                sb.AppendLine($"- DESC: \"{safeDesc}\" | CODE: {item.ItemNumber} | HS_INPUT: {item.TariffCode}");
            }
            sb.AppendLine("Respond ONLY with the complete, valid JSON object.");
            return sb.ToString();
        }

        // --- Helper Function to Fix Common JSON Issues (Truncation, Trailing Chars) ---
        private string TryFixJson(string jsonContent, out bool fixApplied)
        {
            fixApplied = false;
            if (string.IsNullOrWhiteSpace(jsonContent)) return jsonContent;

            string trimmedJson = jsonContent.Trim();

            // 1. Check if it's already valid JSON
            try
            {
                JObject.Parse(trimmedJson);
                _logger.LogTrace("Original JSON parsed successfully. No fix needed.");
                return trimmedJson; // It's valid, return as is
            }
            catch (JsonReaderException)
            {
                _logger.LogDebug("Original JSON failed to parse. Attempting fixes...");
                // Continue to fix attempts
            }
            catch (Exception ex) // Catch other potential parsing errors
            {
                _logger.LogError(ex, "Unexpected error parsing original JSON. Returning original.");
                return trimmedJson;
            }

            // 2. Attempt to fix common truncation: Missing closing "]}"
            if (trimmedJson.EndsWith("}") && trimmedJson.Contains("\"items\": [") && !trimmedJson.EndsWith("]}"))
            {
                // Likely missing the array/object close
                string fixAttempt1 = trimmedJson + "]}";
                try
                {
                    JObject.Parse(fixAttempt1);
                    _logger.LogInformation("Applied JSON Fix 1 (Appended ']}}'): Original end: '...{OriginalEnd}', Fixed end: '...]}'", TruncateForLog(trimmedJson, 30));
                    fixApplied = true;
                    return fixAttempt1;
                }
                catch (JsonReaderException) { /* Fix 1 failed, try next */ }
            }

            // 3. Attempt to fix truncation: Missing part of last object AND closing "]}"
            int lastObjectStart = trimmedJson.LastIndexOf('{');
            int itemsArrayStart = trimmedJson.IndexOf("\"items\": [");
            if (lastObjectStart > 0 && itemsArrayStart >= 0 && lastObjectStart > itemsArrayStart)
            {
                string fixAttempt2 = trimmedJson.Substring(0, lastObjectStart) + /* Potential missing object data */ "}]}"; // Close object, array, main object
                try
                {
                    JObject.Parse(fixAttempt2);
                     _logger.LogInformation("Applied JSON Fix 2 (Closed last object and appended ']}}'): Original end: '...{OriginalEnd}', Fixed end: '...}}]}}'", TruncateForLog(trimmedJson, 30));
                    fixApplied = true;
                    return fixAttempt2;
                }
                catch (JsonReaderException) { /* Fix 2 failed, try next */ }
            }

            // 4. Attempt to fix trailing characters (like the extra ']')
            // Find the *correct* end of the JSON structure
            int balance = 0;
            int lastValidCharIndex = -1;
            bool inString = false;
            for (int i = 0; i < trimmedJson.Length; i++)
            {
                char c = trimmedJson[i];
                if (c == '"' && (i == 0 || trimmedJson[i - 1] != '\\')) // Handle escaped quotes
                {
                    inString = !inString;
                }
                if (!inString)
                {
                    if (c == '{' || c == '[') balance++;
                    else if (c == '}' || c == ']') balance--;
                }
                // If balance is 0 and we are not inside a string, this *could* be the end of a valid JSON structure
                if (balance == 0 && !inString && (c == '}' || c == ']'))
                {
                    lastValidCharIndex = i;
                }
            }

            if (lastValidCharIndex != -1 && lastValidCharIndex < trimmedJson.Length - 1)
            {
                string fixAttempt3 = trimmedJson.Substring(0, lastValidCharIndex + 1);
                try
                {
                    JObject.Parse(fixAttempt3);
                    _logger.LogInformation("Applied JSON Fix 3 (Removed trailing characters): Original end: '...{OriginalEnd}', Fixed end: '...{FixedEnd}'", TruncateForLog(trimmedJson, 30), TruncateForLog(fixAttempt3, 30));
                    fixApplied = true;
                    return fixAttempt3;
                }
                catch (JsonReaderException) { /* Fix 3 failed */ }
            }


            _logger.LogWarning("All JSON fix attempts failed. Returning original potentially invalid JSON: {JsonContent}", TruncateForLog(trimmedJson));
            return trimmedJson; // Return original if all fixes fail
        }


        // --- Modified ParseBatchResponse to use the helper correctly ---
        private Dictionary<string, (string ItemNumber, string TariffCode)> ParseBatchResponse(string jsonContent)
        {
            var result = new Dictionary<string, (string, string)>();
            // Handle null/whitespace input early
            string originalTrimmed = jsonContent?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(originalTrimmed)) { _logger.LogWarning("ParseBatchResponse received empty or null JSON content."); return result; }

            // --- Attempt to fix potential truncation ---
            bool fixApplied = false; // Declare fixApplied first
            string jsonToParse = TryFixJson(originalTrimmed, out fixApplied); // TryFixJson now returns bool indicating if fix was applied
            // bool fixApplied = (jsonToParse != originalTrimmed); // This line is redundant now as TryFixJson uses 'out'
            // -----------------------------------------

            try
            {
                // --- Parse the potentially fixed JSON ---
                JObject parsedObject = JObject.Parse(jsonToParse);
                // ------------------------------------

                if (!(parsedObject["items"] is JArray itemsArray))
                {
                    // Log details including whether fix was applied
                    _logger.LogWarning("Batch response JSON does not contain 'items' array (Fix Applied: {FixApplied}). Content: {JsonContent}", fixApplied, TruncateForLog(jsonToParse));
                    return result;
                }

                int itemCount = 0;
                foreach (var itemToken in itemsArray)
                { // Safely iterate
                    itemCount++;
                    // Rest of the parsing logic... (as before)
                    if (!(itemToken is JObject itemObj))
                    {
                        _logger.LogWarning("Item #{ItemIndex} in batch 'items' array was not a JSON object: {ItemToken}", itemCount, itemToken.ToString());
                        continue; // Skip non-object items
                    }
                    string description = itemObj.Value<string>("original_description");
                    string productCode = itemObj.Value<string>("product_code");
                    string hsCode = itemObj.Value<string>("hs_code");
                    if (!string.IsNullOrWhiteSpace(description) && !string.IsNullOrWhiteSpace(hsCode) && hsCode.Length == 8 && hsCode.All(char.IsDigit))
                    {
                        string finalProductCode = SanitizeProductCode(productCode);
                        if (!result.ContainsKey(description)) { result.Add(description, (finalProductCode, hsCode)); }
                        else { _logger.LogWarning("Duplicate original description '{Description}' found in batch response JSON. Overwriting.", description); result[description] = (finalProductCode, hsCode); }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid/incomplete data for item #{ItemIndex} in batch response. Desc: '{Desc}', HS: '{HS}', Code: '{Code}'. Skipping item.", itemCount, description ?? "NULL", hsCode ?? "NULL", productCode ?? "NULL");
                    }
                } // end foreach item

                // Log successful parsing, especially if fixed
                if (fixApplied && result.Any())
                {
                    _logger.LogInformation("Successfully parsed {ItemCount} items after applying JSON fix.", result.Count);
                }
                else if (result.Any())
                {
                    _logger.LogDebug("Successfully parsed {ItemCount} items (no fix applied or needed).", result.Count);
                }
                else
                {
                    _logger.LogWarning("Parsed JSON but found no valid items (Fix Applied: {FixApplied}).", fixApplied);
                }

            }
            catch (JsonReaderException jsonEx)
            {
                // Log the specific error and include original/fixed strings if fix was attempted
                if (fixApplied)
                {
                    _logger.LogError(jsonEx, "Failed to parse batch response JSON even after attempting fix. Original Trimmed: {OriginalJson}, Attempted Fix: {AttemptedJson}", TruncateForLog(originalTrimmed), TruncateForLog(jsonToParse));
                }
                else
                {
                    _logger.LogError(jsonEx, "Failed to parse batch response JSON (no fix applied). Content: {JsonContent}", TruncateForLog(originalTrimmed));
                }
                // Throw FormatException so the upstream ClassifyItemsAsync catch block triggers fallback
                throw new FormatException("Failed to parse batch response JSON from API.", jsonEx);
            }
            catch (Exception ex)
            { // Catch other potential errors
              // Log the error and include original/fixed strings if fix was attempted
                if (fixApplied)
                {
                    _logger.LogError(ex, "Unexpected error parsing batch response content after attempting fix. Original Trimmed: {OriginalJson}, Attempted Fix: {AttemptedJson}", TruncateForLog(originalTrimmed), TruncateForLog(jsonToParse));
                }
                else
                {
                    _logger.LogError(ex, "Unexpected error parsing batch response content (no fix applied). Content: {JsonContent}", TruncateForLog(originalTrimmed));
                }
                throw; // Re-throw unexpected errors
            }
            return result;
        }

        private string SanitizeInputText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            string truncated = input.Length > MaxDescriptionLength ? input.Substring(0, MaxDescriptionLength) : input;
            string sanitized = truncated.Replace("\"", "'").Replace("\\", "\\\\").Replace("\n", " ").Replace("\r", " ").Trim();
            sanitized = Regex.Replace(sanitized, SanitizePattern, "");
            return string.IsNullOrWhiteSpace(sanitized) ? "SANITIZED_EMPTY" : sanitized;
        }

        private string SanitizeItemNumber(string itemNumber)
        {
            if (string.IsNullOrWhiteSpace(itemNumber)) return "NEW";
            var cleaned = Regex.Replace(itemNumber, @"[^\w-]", "").Trim();
            if (string.IsNullOrEmpty(cleaned)) return "NEW";
            if (cleaned.Length > 20) cleaned = cleaned.Substring(0, 20);
            return string.IsNullOrEmpty(cleaned) ? "NEW" : cleaned.ToUpperInvariant();
        }

        private string SanitizeTariffCode(string itemNumber, string tariffCode)
        {
            if (string.IsNullOrWhiteSpace(tariffCode)) return "00000000";
            var cleaned = Regex.Replace(tariffCode, @"\D", "");
            if (string.IsNullOrEmpty(cleaned)) return "00000000";
            if (cleaned.Length < 8) { _logger.LogTrace("Tariff code for Item '{ItemNumber}' was short ('{OriginalCode}'). Padding.", itemNumber, tariffCode); return cleaned.PadRight(8, '0'); }
            else if (cleaned.Length > 8) { _logger.LogTrace("Tariff code for Item '{ItemNumber}' was long ('{OriginalCode}'). Truncating.", itemNumber, tariffCode); return cleaned.Substring(0, 8); }
            else { return cleaned; }
        }

        private async Task<string> GenerateProductCode(string description, CancellationToken cancellationToken)
        {
            const string promptTemplate = @"Generate a concise, descriptive product code (max 20 chars) for: {0}. Rules: Uppercase A-Z, 0-9, hyphens ONLY. 3-20 chars. Respond ONLY with code."; // Shortened prompt
            if (string.IsNullOrWhiteSpace(description)) { _logger.LogWarning("GenerateProductCode called with empty description."); return "GENERATED-CODE"; }
            try
            {
                var cleanDesc = SanitizeInputText(description);
                var prompt = string.Format(promptTemplate, cleanDesc);
                var response = await GetCompletionAsync(prompt, maxTokens: 30, temperature: 0.2, cancellationToken: cancellationToken);
                return SanitizeProductCode(response);
            }
            catch (Exception ex) { _logger.LogError(ex, "Failed to generate product code for Description: {Description}", description); return "ERROR-CODE"; }
        }

        private string SanitizeProductCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "NEW-CODE";
            var sanitized = Regex.Replace(code.Trim(), @"[^\w-]", "-").Replace("--", "-").Trim('-');
            sanitized = sanitized.ToUpperInvariant();
            if (sanitized.Length == 0) return "NEW-CODE";
            if (sanitized.Length > 20) sanitized = sanitized.Substring(0, 20).TrimEnd('-');
            return sanitized.Length == 0 ? "NEW-CODE" : sanitized;
        }

        private async Task<string> GetCompletionAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt)) { _logger.LogWarning("GetCompletionAsync called with empty prompt."); return string.Empty; }
            int safeMaxTokens = CalculateSafeMaxTokens(prompt, maxTokens);
            var requestBody = new { model = Model, messages = new[] { new { role = "user", content = prompt } }, temperature = temperature ?? DefaultTemperature, max_tokens = safeMaxTokens, stream = false };
            _logger.LogTrace("Sending API request. Model: {Model}, Temp: {Temperature}, MaxTokens: {MaxTokens}", Model, requestBody.temperature, requestBody.max_tokens);
            try
            {
                var jsonResponse = await PostRequestAsync(requestBody, cancellationToken).ConfigureAwait(false);
                return ParseCompletionResponse(jsonResponse);
            }
            catch (RateLimitException rle) { _logger.LogError(rle, "API rate limit exceeded after retries. Status: {StatusCode}", rle.StatusCode); throw; }
            catch (HttpRequestException httpEx) { var sc = httpEx.Data.Contains("StatusCode") ? httpEx.Data["StatusCode"] : "N/A"; _logger.LogError(httpEx, "HTTP request failed after retries. Status: {StatusCode}", sc); throw new HSCodeRequestException("API request failed after retries.", httpEx); }
            catch (TaskCanceledException tcEx) { if (cancellationToken.IsCancellationRequested) { _logger.LogWarning("API request cancelled by user."); throw; } else { _logger.LogError(tcEx, "API request timed out after retries."); throw new TimeoutException("API request timed out after retries.", tcEx); } }
            catch (JsonException jsonEx) { _logger.LogError(jsonEx, "Failed to parse JSON content from API response."); throw new FormatException("Invalid JSON content received from API.", jsonEx); }
            catch (Exception ex) { _logger.LogError(ex, "An unexpected error occurred during API completion request."); throw new HSCodeRequestException("An unexpected error occurred.", ex); }
        }

        private int CalculateSafeMaxTokens(string prompt, int? requestedMax)
        {
            const int MIN_RESPONSE_TOKENS = 50;
            var promptTokenEstimate = EstimateTokenCount(prompt);
            var availableForResponse = MAX_TOKENS_PER_REQUEST - promptTokenEstimate - TOKEN_BUFFER;
            if (availableForResponse < MIN_RESPONSE_TOKENS) { _logger.LogWarning("Prompt tokens ({PromptTokens}) leaves less than minimum ({MinTokens}) for response. Using minimum.", promptTokenEstimate, MIN_RESPONSE_TOKENS); availableForResponse = MIN_RESPONSE_TOKENS; }
            int finalMaxTokens = Math.Min(requestedMax ?? FALLBACK_MAX_TOKENS, availableForResponse);
            return Math.Min(finalMaxTokens, MAX_TOKENS_PER_REQUEST);
        }

        private string ParseCompletionResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) { _logger.LogWarning("ParseCompletionResponse received empty JSON."); return string.Empty; }
            try
            {
                JObject responseObj = JObject.Parse(jsonResponse);
                var content = responseObj["choices"]?[0]?["message"]?["content"]?.Value<string>();
                if (content == null) { _logger.LogWarning("Could not find 'content' in API response JSON."); _logger.LogDebug("Response JSON structure: {JsonResponse}", TruncateForLog(jsonResponse)); return string.Empty; }
                return SanitizeApiResponse(content);
            }
            catch (JsonReaderException jsonEx) { _logger.LogError(jsonEx, "Failed to parse API response JSON. Response: {JsonResponse}", TruncateForLog(jsonResponse)); throw new FormatException("Invalid JSON structure from API.", jsonEx); }
            catch (Exception ex) { _logger.LogError(ex, "Error accessing content in API response JSON. Response: {JsonResponse}", TruncateForLog(jsonResponse)); throw; }
        }

        private string SanitizeApiResponse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent)) return string.Empty;
            var sanitized = Regex.Replace(responseContent, @"^```(json)?\s*|\s*```$", "", RegexOptions.IgnoreCase).Trim();
            if (sanitized.StartsWith("{") && !sanitized.EndsWith("}"))
            {
                int lastBrace = sanitized.LastIndexOf('}'); int lastBracket = sanitized.LastIndexOf(']'); int lastValid = Math.Max(lastBrace, lastBracket);
                if (lastValid > 0) { sanitized = sanitized.Substring(0, lastValid + 1); _logger.LogDebug("Attempted to fix truncated JSON response ending."); }
            }
            return sanitized;
        }

        // PostRequestAsync uses the _retryPolicy defined earlier
        private async Task<string> PostRequestAsync(object requestBody, CancellationToken cancellationToken = default)
        {
            // Note: The ExecuteAsync overload taking Func<Context, CancellationToken, Task<TResult>>
            // might be needed if you want to pass context *into* the execution.
            // For now, we pass the cancellation token directly.
            return await _retryPolicy.ExecuteAsync(async (ct) => {
                string jsonRequest = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                using (var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions") { Content = content })
                {
                    HttpResponseMessage response = null;
                    try
                    {
                        // Pass CancellationToken (ct) from Polly context to SendAsync
                        response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
                        string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        // Check status codes for throwing exceptions handled by Polly
                        if (response.StatusCode == (HttpStatusCode)429) { _logger.LogDebug("PostRequestAsync: Throwing RateLimitException for status 429."); throw new RateLimitException((int)response.StatusCode, responseContent); }
                        if (response.StatusCode >= HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
                        {
                            var httpEx = new HttpRequestException($"API request failed with status {(int)response.StatusCode}. Response: {responseContent}");
                            httpEx.Data["StatusCode"] = (int)response.StatusCode;
                            _logger.LogDebug(httpEx, "PostRequestAsync: Throwing HttpRequestException for status {StatusCode}.", (int)response.StatusCode);
                            throw httpEx;
                        }
                        // If not handled by Polly but still an error, throw normally
                        if (!response.IsSuccessStatusCode)
                        {
                            var finalHttpEx = new HttpRequestException($"API request failed with status {(int)response.StatusCode}: {responseContent}");
                            finalHttpEx.Data["StatusCode"] = (int)response.StatusCode;
                            _logger.LogError(finalHttpEx, "PostRequestAsync: Unhandled non-success status code {StatusCode}.", (int)response.StatusCode);
                            throw finalHttpEx;
                        }
                        _logger.LogTrace("PostRequestAsync: API request successful (Status {StatusCode}).", (int)response.StatusCode);
                        return responseContent;
                    }
                    finally { response?.Dispose(); }
                }
            }, cancellationToken).ConfigureAwait(false); // Pass the original CancellationToken here
        }

        private string ParseSingleHsCodeFromJson(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent)) { _logger.LogWarning("ParseSingleHsCodeFromJson received empty content."); return ""; }
            try
            {
                JObject parsedObject = JObject.Parse(jsonContent);
                var hsCode = parsedObject["items"]?[0]?["hs_code"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(hsCode) && hsCode.Length == 8 && hsCode.All(char.IsDigit)) { return hsCode; }
                else { _logger.LogWarning("Could not extract valid HS code from JSON content. Found: '{HsCode}'. Falling back to regex. JSON: {Json}", hsCode, TruncateForLog(jsonContent)); return ParseHsCodeFromText(jsonContent); }
            }
            catch (JsonReaderException jsonEx) { _logger.LogWarning(jsonEx, "Failed to parse JSON for single HS code, falling back to regex. Content: {JsonContent}", TruncateForLog(jsonContent)); return ParseHsCodeFromText(jsonContent); }
            catch (Exception ex) { _logger.LogError(ex, "Unexpected error parsing single HS code from JSON. Content: {JsonContent}", TruncateForLog(jsonContent)); return ""; }
        }

        private string ParseHsCodeFromText(string textContent)
        {
            if (string.IsNullOrWhiteSpace(textContent)) return "";
            try
            {
                var match = Regex.Match(textContent, HsCodePattern); // HsCodePattern = @"\b(\d{8})\b";
                if (match.Success) { var code = match.Groups[1].Value; _logger.LogDebug("Extracted HS code using Regex fallback: {HsCode}", code); return code; }
                _logger.LogWarning("Could not find HS code using Regex '{Pattern}' in text: {Text}", HsCodePattern, TruncateForLog(textContent, 100)); return "";
            }
            catch (RegexMatchTimeoutException ex) { _logger.LogError(ex, "Regex timed out parsing HS code."); return ""; }
            catch (Exception ex) { _logger.LogError(ex, "Error during Regex HS code parsing."); return ""; }
        }

        // --- Helper for logging ---
        private string TruncateForLog(string text, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
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
            var chunk = new List<T>(chunkSize); chunk.Add(enumerator.Current);
            for (int i = 1; i < chunkSize && enumerator.MoveNext(); i++) { chunk.Add(enumerator.Current); }
            return chunk;
        }

        // --- Dispose Pattern ---
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { _httpClient?.Dispose(); }
                disposedValue = true;
            }
        }
        public void Dispose() { Dispose(disposing: true); GC.SuppressFinalize(this); }
    }
}
