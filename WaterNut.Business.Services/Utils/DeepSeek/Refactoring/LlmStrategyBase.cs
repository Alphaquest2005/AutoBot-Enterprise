#nullable disable
using Serilog; // Added
using Newtonsoft.Json; // Required for serialization in PostRequestAsync
using Newtonsoft.Json.Linq; // Required for parsing in helper methods
using Polly; // Required for AsyncRetryPolicy using
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net; // Required for HttpStatusCode, DecompressionMethods
using System.Net.Http;
using System.Net.Http.Headers; // Required for headers
using System.Text; // Required for Encoding, StringBuilder
using System.Text.RegularExpressions; // Required for Regex
using System.Threading;
using System.Threading.Tasks;

namespace WaterNut.Business.Services.Utils.LlmApi
{
    public abstract class LlmStrategyBase
    {
        // Dependencies injected by concrete class constructors
        protected readonly string ApiKey;
        protected readonly Serilog.ILogger Logger;
        protected readonly HttpClient HttpClient;
        protected readonly AsyncRetryPolicy RetryPolicy;

        // Common Configuration (set by concrete class or properties)
        public string Model { get; set; }
        public string SingleItemPromptTemplate { get; set; }
        public string BatchItemPromptTemplate { get; set; }
        public string GenerateCodePromptTemplate { get; set; } = @"Generate a concise, descriptive product code (max 20 chars) for the item description below. Rules: Use ONLY uppercase letters (A-Z), numbers (0-9), and hyphens (-). Must be between 3 and 20 characters long. Description: {0} Respond ONLY with the generated product code, nothing else.";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 200; // Base default, can be overridden by provider default

        // Common Patterns/Limits (can be overridden by concrete class)
        public virtual int MaxDescriptionLength { get; set; } = 500;
        public virtual string SanitizePattern { get; set; } = @"[^\p{L}\p{N}\p{P}\p{S}\s]";
        public virtual string ItemNumberPattern { get; set; } = @"^[\w-]{1,20}$";
        public virtual string TariffCodePattern { get; set; } = @"^\d{8}$";
        public virtual string HsCodePattern { get; set; } = @"\b(\d{8})\b";

        // Abstract properties/methods to be implemented by concrete strategies
        public abstract LLMProvider ProviderType { get; }
        protected abstract string GetApiUrl(string modelName);
        protected abstract void AddAuthentication(HttpRequestMessage requestMessage, string apiKey);
        protected abstract object BuildRequestBody(string prompt, int maxTokens, double temperature, string modelName);
        protected abstract (string Completion, TokenUsage Usage) ParseProviderResponse(string jsonResponse); // Renamed from ParseResponse
        public abstract ModelPricing? GetPricing(string modelName);
        public abstract string GetDefaultModelName();

        protected LlmStrategyBase(string apiKey, Serilog.ILogger logger, HttpClient httpClient, AsyncRetryPolicy retryPolicy)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Assign the Serilog logger
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient)); // Assign the injected client
            RetryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
            Model = GetDefaultModelName(); // Initialize model
            SetDefaultPromptsInternal(); // Set base prompts
        }

        // --- Core Logic Moved from LlmApiClient ---

        /// <summary>
        /// Makes the actual API call, handling retries and basic response validation.
        /// Returns raw completion string, usage info, error message, and success status.
        /// </summary>
        protected async Task<(string RawCompletion, TokenUsage Usage, string Error, bool CallSuccess)> ExecuteProviderApiCallAsync(
            string prompt, double? temperatureOverride, int? maxTokensOverride,
            CancellationToken cancellationToken)
        {
            TokenUsage usage = new TokenUsage { IsEstimated = true, InputTokens = EstimateTokenCount(prompt) };
            string rawCompletion = string.Empty;

            if (string.IsNullOrWhiteSpace(prompt))
            {
                Logger.Warning("[{Provider}] ExecuteProviderApiCallAsync called with empty prompt.", ProviderType); // Changed LogWarning to Warning
                return (string.Empty, usage, "Empty prompt provided.", false);
            }

            int safeMaxTokens = CalculateSafeMaxTokens(prompt, maxTokensOverride);
            double temp = temperatureOverride ?? DefaultTemperature;

            Logger.Verbose("[{Provider}] Sending API request. Model: {Model}, Temp: {Temperature}, MaxTokens: {MaxTokens}", ProviderType, Model, temp, safeMaxTokens); // Changed LogTrace to Trace
            Logger.Debug("[{Provider}] Prompt Length: {PromptLength} chars, Est. Input Tokens: {InputTokens}", ProviderType, prompt.Length, usage.InputTokens); // Changed LogDebug to Debug

            string jsonResponse = null;
            try
            {
                string apiUrl = GetApiUrl(Model);
                object requestBody = BuildRequestBody(prompt, safeMaxTokens, temp, Model);
                jsonResponse = await PostRequestAsync(apiUrl, requestBody, AddAuthentication, cancellationToken).ConfigureAwait(false);

                // Parse provider response - gets completion string and actual usage
                (rawCompletion, usage) = ParseProviderResponse(jsonResponse); // Use strategy's implementation

                // Basic sanitization of the raw completion text
                string sanitizedCompletion = SanitizeApiResponse(rawCompletion);

                return (sanitizedCompletion, usage, null, true); // Success
            }
            // Catch specific exceptions from PostRequestAsync or ParseProviderResponse
            catch (RateLimitException rle) { Logger.Error(rle, "[{Provider}] API rate limit exceeded after retries. Status: {StatusCode}", ProviderType, rle.StatusCode); return (string.Empty, usage, $"Rate Limit Exceeded (HTTP {rle.StatusCode})", false); } // Changed LogError to Error
            catch (HttpRequestException httpEx) { var sc = httpEx.Data.Contains("StatusCode") ? httpEx.Data["StatusCode"] : "N/A"; Logger.Error(httpEx, "[{Provider}] HTTP request failed after retries. Status: {StatusCode}", ProviderType, sc); return (string.Empty, usage, $"HTTP Request Failed (Status: {sc})", false); } // Changed LogError to Error
            catch (TaskCanceledException tcEx) { if (cancellationToken.IsCancellationRequested) { Logger.Warning("[{Provider}] API request cancelled by user.", ProviderType); return (string.Empty, usage, "Request Cancelled", false); } else { Logger.Error(tcEx, "[{Provider}] API request timed out after retries.", ProviderType); return (string.Empty, usage, "Request Timeout", false); } } // Changed LogWarning to Warning and LogError to Error
            catch (JsonException jsonEx) { Logger.Error(jsonEx, "[{Provider}] Failed to parse JSON content from API response. Response: {JsonResponse}", ProviderType, TruncateForLog(jsonResponse ?? "N/A", 500)); return (string.Empty, usage, "Invalid JSON response from API.", false); } // Changed LogError to Error
            catch (LlmApiException apiEx) { Logger.Error(apiEx, "[{Provider}] Provider-specific API Exception.", ProviderType); return (string.Empty, usage, $"API Error: {apiEx.Message}", false); } // Catch errors thrown by ParseProviderResponse (like safety blocks) // Changed LogError to Error
            catch (Exception ex) { Logger.Error(ex, "[{Provider}] An unexpected error occurred during API call.", ProviderType); return (string.Empty, usage, $"Unexpected Error: {ex.Message}", false); } // Changed LogError to Error
        }


        protected async Task<string> PostRequestAsync(string apiUrl, object requestBody, Action<HttpRequestMessage, string> addAuthAction, CancellationToken cancellationToken = default)
        {
            // Using Polly retry policy defined in RetryPolicy field
            Context pollyContext = new Context($"PostRequest-{Guid.NewGuid()}");
            pollyContext["Logger"] = Logger; // Pass logger via context if needed by policy
            pollyContext["ProviderType"] = ProviderType;

            return await RetryPolicy.ExecuteAsync(async (ctx, ct) => {
                string jsonRequest = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                HttpResponseMessage response = null;
                using (var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
                {
                    var initialUri = new Uri(apiUrl);
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, initialUri) { Content = content })
                    {
                        addAuthAction(requestMessage, ApiKey); // Apply auth using delegate

                        Logger.Debug("[{Provider}] Attempting HTTP POST to {Url}. Request size: {Size} bytes.", ProviderType, requestMessage.RequestUri, jsonRequest.Length); // Changed LogDebug to Debug
                        try
                        {
                            response = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
                            string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            // --- Status Code Handling ---
                            // Check for retryable status codes first to throw exceptions Polly will handle
                            if (response.StatusCode == (HttpStatusCode)429) throw new RateLimitException((int)response.StatusCode, responseContent);
                            if (response.StatusCode >= HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.GatewayTimeout)
                            { var httpEx = new HttpRequestException($"[{ProviderType}] API req failed (retryable) status {(int)response.StatusCode}.", null); httpEx.Data["StatusCode"] = (int)response.StatusCode; throw httpEx; }

                            // Check for other non-success codes that shouldn't be retried
                            if (!response.IsSuccessStatusCode)
                            { var finalHttpEx = new HttpRequestException($"[{ProviderType}] API req failed (non-retryable) status {(int)response.StatusCode}: {TruncateForLog(responseContent)}"); finalHttpEx.Data["StatusCode"] = (int)response.StatusCode; throw finalHttpEx; }

                            Logger.Debug("[{Provider}] PostRequestAsync successful (Status {StatusCode}). Resp size: {Size} bytes.", ProviderType, (int)response.StatusCode, responseContent.Length); // Changed LogDebug to Debug
                            return responseContent;
                        }
                        // Ensure response is disposed even if ReadAsStringAsync fails
                        finally { response?.Dispose(); }
                    }
                }
            }, pollyContext, cancellationToken).ConfigureAwait(false); // Pass context and cancellation token
        }

        /// <summary>
        /// Estimates the number of tokens in a given text using a basic character count method.
        /// Can be overridden by concrete strategies.
        /// </summary>
        public virtual int EstimateTokenCount(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            // ~3 chars per token is a rough general estimate
            int estimatedTokens = (int)Math.Ceiling(text.Length / 3.0);
            // Logger?.LogTrace("[{Provider}] Estimated tokens for text length {Length}: {Tokens}", ProviderType, text.Length, estimatedTokens);
            return estimatedTokens;
        }


        // --- Common Helper Methods ---
        protected decimal CalculateCost(TokenUsage usage)
        {
            if (usage == null) return 0m;
            var modelPricing = GetPricing(Model); // Get pricing via strategy's implementation

            if (modelPricing == null) { Logger.Warning("[{Provider}] Pricing info not found for Model '{Model}'. Cannot calculate cost.", ProviderType, Model); return 0m; } // Changed LogWarning to Warning
            decimal inputCost = ((decimal)usage.InputTokens / 1_000_000m) * modelPricing.InputPricePerMillionTokens;
            decimal outputCost = ((decimal)usage.OutputTokens / 1_000_000m) * modelPricing.OutputPricePerMillionTokens;
            decimal totalCost = inputCost + outputCost;
            Logger.Debug("[{Provider}] Cost Calc - Input: {In}, Output: {Out}, Estimated: {IsEst}, Total Cost: {TotalCost:C}", ProviderType, usage.InputTokens, usage.OutputTokens, usage.IsEstimated, totalCost); // Changed LogDebug to Debug
            return totalCost;
        }

        protected int CalculateSafeMaxTokens(string prompt, int? requestedMax)
        {
            // Constants defined within LlmStrategyBase scope now
            const int MAX_TOKENS_PER_REQUEST_LOCAL = 4096; // Use a local constant name
            const int TOKEN_BUFFER_LOCAL = 600;
            const int MIN_RESPONSE_TOKENS_LOCAL = 50;
            const int ABSOLUTE_MAX_RESPONSE_LOCAL = MAX_TOKENS_PER_REQUEST_LOCAL - TOKEN_BUFFER_LOCAL - 100;

            var promptTokenEstimate = EstimateTokenCount(prompt);
            var availableForResponse = MAX_TOKENS_PER_REQUEST_LOCAL - promptTokenEstimate - TOKEN_BUFFER_LOCAL;

            if (availableForResponse < MIN_RESPONSE_TOKENS_LOCAL) { Logger.Warning("[{Provider}] Prompt tokens ({PromptTokens}) + buffer ({TokenBuffer}) leaves less than MIN_RESPONSE_TOKENS for response. Forcing minimum.", ProviderType, promptTokenEstimate, TOKEN_BUFFER_LOCAL); availableForResponse = MIN_RESPONSE_TOKENS_LOCAL; } // Changed LogWarning to Warning

            int finalMaxTokens = availableForResponse;
            if (requestedMax.HasValue && requestedMax.Value < finalMaxTokens) { finalMaxTokens = requestedMax.Value; Logger.Verbose("[{Provider}] CalculateSafeMaxTokens: Capping available ({Available}) by requested ({Requested}).", ProviderType, availableForResponse, requestedMax.Value); } // Changed LogTrace to Trace
            else if (requestedMax.HasValue) { Logger.Verbose("[{Provider}] CalculateSafeMaxTokens: Requested ({Requested}) >= available ({Available}). Using available.", ProviderType, requestedMax.Value, availableForResponse); } // Changed LogTrace to Trace
            else { Logger.Verbose("[{Provider}] CalculateSafeMaxTokens: No requested max. Using available ({Available}).", ProviderType, availableForResponse); } // Changed LogTrace to Trace

            finalMaxTokens = Math.Min(finalMaxTokens, ABSOLUTE_MAX_RESPONSE_LOCAL);
            finalMaxTokens = Math.Max(MIN_RESPONSE_TOKENS_LOCAL, finalMaxTokens);
            Logger.Verbose("[{Provider}] CalculateSafeMaxTokens: PromptEst={PromptEst}, Requested={Req}, Available={Avail}, Final={Final}", ProviderType, promptTokenEstimate, requestedMax?.ToString() ?? "null", availableForResponse, finalMaxTokens); // Changed LogTrace to Trace
            return finalMaxTokens;
        }

        protected virtual void SetDefaultPromptsInternal()
        {
            // Base prompts - concrete classes can override these in their constructor or properties
            SingleItemPromptTemplate = @"Analyze the product description provided below. Return ONLY a single, valid JSON object.
Follow these rules strictly:
1. The JSON object must match this exact format:
{{
    ""items"": [
        {{
            ""original_description"": ""EXACT_INPUT_DESCRIPTION"",
            ""product_code"": ""INPUT_CODE_OR_GENERATED"",
            ""category"": ""BEST_GUESS_BROAD_CATEGORY"",
            ""category_hs_code"": ""00000000"",
            ""hs_code"": ""00000000""
        }}
    ]
}}
2. Preserve ""original_description"" EXACTLY.
3. ""hs_code"" MUST be specific 8-digit HS code (0-9). Use '00000000' if unsure.
4. ""category"" broad classification. Use 'N/A' if unsure.
5. ""category_hs_code"" MUST be 8-digit HS code for category (0-9). Use '00000000' if unsure.
6. Respond *only* with valid JSON. No extra text/markdown.

Product Information:
Description: __DESCRIPTION_HERE__
Product Code (Optional): __PRODUCT_CODE_HERE__";

            BatchItemPromptTemplate = @"Analyze list below. For each: 1. Broad category. 2. 8-digit category HS code. 3. Specific 8-digit item HS code. 4. Use/generate product code.
Return ONLY single valid JSON object 'items' array. Format/rules strictly:
JSON Format: { ""items"": [ { ""original_description"": ""EXACT_INPUT"", ""product_code"": ""PROVIDED_OR_GENERATED"", ""category"": ""BROAD_CATEGORY"", ""category_hs_code"": ""8_DIGIT_CAT_HS"", ""hs_code"": ""8_DIGIT_ITEM_HS"" } ] }
Rules:
- ONLY JSON object. No extra text/markdown.
- Preserve ""original_description"" EXACTLY.
- ""product_code"": Use input. If 'NEW'/invalid, generate concise code (A-Z, 0-9, -, 1-20 chars).
- ""category"": Concise name. Use 'N/A' if unknown.
- ""category_hs_code"": Exactly 8 digits (0-9). Use '00000000' if unknown.
- ""hs_code"": Exactly 8 digits (0-9). Use '00000000' if unknown.
- TOKEN LIMIT: If close, finish CURRENT item, close 'items' array ']' & main '}'. Ensure VALID JSON.
- No trailing commas.
Product List:";
            // GenerateCodePromptTemplate is already initialized with a default
        }

        // --- Domain-Specific Parsing ---
        protected (string TariffCode, string Category, string CategoryTariffCode) ParseSingleItemResponseFormat(string jsonContent)
        {
            string tariffCode = "00000000"; string category = "N/A"; string categoryHsCode = "00000000";
            if (string.IsNullOrWhiteSpace(jsonContent)) { Logger.Warning("[{Provider}] ParseSingleItemResponseFormat received empty content.", ProviderType); return (tariffCode, category, categoryHsCode); } // Changed LogWarning to Warning
            bool fixApplied; string jsonToParse = TryFixJson(jsonContent.Trim(), out fixApplied);
            try
            {
                JObject parsedObject = JObject.Parse(jsonToParse);
                if (parsedObject["items"] is JArray itemsArray && itemsArray.Count > 0 && itemsArray[0] is JObject itemObj)
                {
                    string hsCodeRaw = itemObj.TryGetValue("hs_code", StringComparison.OrdinalIgnoreCase, out var hsToken) ? hsToken.Value<string>() : null;
                    string categoryRaw = itemObj.TryGetValue("category", StringComparison.OrdinalIgnoreCase, out var catToken) ? catToken.Value<string>() : null;
                    string categoryHsCodeRaw = itemObj.TryGetValue("category_hs_code", StringComparison.OrdinalIgnoreCase, out var catHsToken) ? catHsToken.Value<string>() : null;
                    tariffCode = SanitizeTariffCode("SingleItemParse", hsCodeRaw, "ItemHS");
                    category = string.IsNullOrWhiteSpace(categoryRaw) ? "N/A" : categoryRaw.Trim();
                    categoryHsCode = SanitizeTariffCode("SingleItemParse", categoryHsCodeRaw, "CategoryHS");
                    Logger.Debug("[{Provider}] Parsed single item response format (Fix Applied: {FixApplied}): HS={HS}, Cat={Cat}, CatHS={CatHS}", ProviderType, fixApplied, tariffCode, category, categoryHsCode); // Changed LogDebug to Debug
                }
                else { Logger.Warning("[{Provider}] Could not find expected 'items' array structure in single item content (Fix Applied: {FixApplied}). JSON: {Json}. Attempting Regex fallback.", ProviderType, fixApplied, TruncateForLog(jsonToParse)); tariffCode = ParseHsCodeFromText(jsonToParse); } // Changed LogWarning to Warning
            }
            catch (JsonReaderException jsonEx) { Logger.Warning(jsonEx, "[{Provider}] Failed to parse JSON for single item format (Fix Applied: {FixApplied}), falling back to regex for HS code only. Content: {JsonContent}", ProviderType, fixApplied, TruncateForLog(jsonToParse)); tariffCode = ParseHsCodeFromText(jsonToParse); } // Changed LogWarning to Warning
            // IMPORTANT: Catch specific exceptions first, then general Exception
            catch (LlmApiException) { throw; } // Don't re-wrap our own specific exceptions
            catch (FormatException fx) { Logger.Error(fx, "[{Provider}] Formatting error parsing single item format (Fix Applied: {FixApplied}). Content: {JsonContent}", ProviderType, fixApplied, TruncateForLog(jsonToParse)); return ("ERROR", "ERROR", "ERROR"); } // Return ERROR on format issue // Changed LogError to Error
            catch (Exception ex) { Logger.Error(ex, "[{Provider}] Unexpected error parsing single item format (Fix Applied: {FixApplied}). Content: {JsonContent}", ProviderType, fixApplied, TruncateForLog(jsonToParse)); return ("ERROR", "ERROR", "ERROR"); } // Changed LogError to Error
            return (tariffCode, category, categoryHsCode);
        }

        protected Dictionary<string, ClassificationResult> ParseBatchResponseFormat(string jsonContent)
        {
            var results = new Dictionary<string, ClassificationResult>();
            string originalTrimmed = jsonContent?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(originalTrimmed)) { Logger.Warning("[{Provider}] ParseBatchResponseFormat received empty or null JSON content.", ProviderType); return results; } // Changed LogWarning to Warning

            bool fixApplied; string jsonToParse = TryFixJson(originalTrimmed, out fixApplied);
            try
            {
                JObject parsedObject = JObject.Parse(jsonToParse);
                if (!(parsedObject["items"] is JArray itemsArray)) { Logger.Warning("[{Provider}] Batch response content JSON does not contain 'items' array (Fix Applied: {FixApplied}). Content: {JsonContent}", ProviderType, fixApplied, TruncateForLog(jsonToParse)); return results; } // Changed LogWarning to Warning

                int itemCount = 0;
                foreach (var itemToken in itemsArray)
                {
                    itemCount++;
                    ClassificationResult itemResult = new ClassificationResult { ParsedSuccessfully = false };
                    if (!(itemToken is JObject itemObj)) { Logger.Warning("[{Provider}] Item #{ItemIndex} in batch 'items' array was not a JSON object: {ItemToken}", ProviderType, itemCount, TruncateForLog(itemToken.ToString())); continue; } // Changed LogWarning to Warning

                    string originalDescription = itemObj.TryGetValue("original_description", StringComparison.OrdinalIgnoreCase, out var descToken) ? descToken.Value<string>() : null;
                    if (string.IsNullOrWhiteSpace(originalDescription)) { Logger.Warning("[{Provider}] Skipping item #{ItemIndex} in batch response format due to missing 'original_description'. Item JSON: {ItemJson}", ProviderType, itemCount, TruncateForLog(itemToken.ToString())); continue; } // Changed LogWarning to Warning
                    itemResult.OriginalDescription = originalDescription;

                    string productCode = itemObj.TryGetValue("product_code", StringComparison.OrdinalIgnoreCase, out var codeToken) ? codeToken.Value<string>() : null;
                    string hsCode = itemObj.TryGetValue("hs_code", StringComparison.OrdinalIgnoreCase, out var hsToken) ? hsToken.Value<string>() : null;
                    string category = itemObj.TryGetValue("category", StringComparison.OrdinalIgnoreCase, out var catToken) ? catToken.Value<string>() : null;
                    string categoryHsCode = itemObj.TryGetValue("category_hs_code", StringComparison.OrdinalIgnoreCase, out var catHsToken) ? catHsToken.Value<string>() : null;

                    itemResult.ItemNumber = SanitizeProductCode(productCode ?? "MISSING");
                    itemResult.TariffCode = SanitizeTariffCode(originalDescription, hsCode, "ItemHS");
                    itemResult.Category = string.IsNullOrWhiteSpace(category) ? "N/A" : category.Trim();
                    itemResult.CategoryHsCode = SanitizeTariffCode(originalDescription, categoryHsCode, "CategoryHS");
                    itemResult.ParsedSuccessfully = true;

                    if (!results.ContainsKey(originalDescription)) { results.Add(originalDescription, itemResult); Logger.Verbose("[{Provider}] Parsed batch item #{ItemIndex}: Desc='{Desc}'", ProviderType, itemCount, TruncateForLog(originalDescription, 50)); } // Changed LogTrace to Trace
                    else { Logger.Warning("[{Provider}] Duplicate original_description '{Description}' found in batch response format (Item #{ItemIndex}). Keeping first.", ProviderType, TruncateForLog(originalDescription, 50), itemCount); } // Changed LogWarning to Warning
                }
                if (fixApplied && results.Any()) Logger.Information("[{Provider}] Successfully parsed {ItemCount} items from format after applying JSON fix.", ProviderType, results.Count); // Changed LogInformation to Information
                else if (results.Any()) Logger.Debug("[{Provider}] Successfully parsed {ItemCount} items from batch response format (Fix applied: {FixApplied}).", ProviderType, results.Count, fixApplied); // Changed LogDebug to Debug
                else Logger.Warning("[{Provider}] Parsed batch format JSON but found no valid items in 'items' array (Fix Applied: {FixApplied}).", ProviderType, fixApplied); // Changed LogWarning to Warning
            }
            catch (JsonReaderException jsonEx) { string errorContext = fixApplied ? $"OrigTrim: {TruncateForLog(originalTrimmed)}, Fixed: {TruncateForLog(jsonToParse)}" : $"Content: {TruncateForLog(originalTrimmed)}"; Logger.Error(jsonEx, "[{Provider}] Failed to parse batch response format JSON (Fix Applied: {FixApplied}). {ErrorContext}", ProviderType, fixApplied, errorContext); throw new FormatException("Failed to parse batch response format JSON.", jsonEx); } // Changed LogError to Error
            catch (FormatException) { throw; }
            catch (Exception ex) { string errorContext = fixApplied ? $"OrigTrim: {TruncateForLog(originalTrimmed)}, Fixed: {TruncateForLog(jsonToParse)}" : $"Content: {TruncateForLog(originalTrimmed)}"; Logger.Error(ex, "[{Provider}] Unexpected error parsing batch response format content (Fix Applied: {FixApplied}). {ErrorContext}", ProviderType, fixApplied, errorContext); throw new LlmApiException("Unexpected error parsing batch response format content.", ex); } // Changed LogError to Error
            return results;
        }


        // --- Sanitization Helpers ---
        protected string TryFixJson(string jsonContent, out bool fixApplied)
        {
            fixApplied = false; if (string.IsNullOrWhiteSpace(jsonContent)) return jsonContent; string trimmedJson = jsonContent.Trim();
            try { JObject.Parse(trimmedJson); return trimmedJson; } catch (JsonReaderException) { Logger.Debug("[{Provider}] Original JSON failed to parse. Attempting fixes...", ProviderType); } catch (Exception ex) { Logger.Error(ex, "[{Provider}] Unexpected error during initial JSON parse check. Returning original.", ProviderType); return trimmedJson; } // Changed LogDebug to Debug and LogError to Error
            string fixedJson = trimmedJson; bool currentFixApplied = false; int lastValidCharIndex = -1; int balance = 0; bool inString = false; char lastNonWhite = ' ';
            for (int i = 0; i < trimmedJson.Length; i++) { char c = trimmedJson[i]; if (c == '"' && (i == 0 || trimmedJson[i - 1] != '\\')) inString = !inString; if (!inString) { if (c == '{' || c == '[') balance++; else if (c == '}' || c == ']') balance--; if (!char.IsWhiteSpace(c)) lastNonWhite = c; if (balance == 0 && (c == '}' || c == ']')) lastValidCharIndex = i; } }
            if (lastValidCharIndex != -1 && lastValidCharIndex < trimmedJson.Length - 1) { fixedJson = trimmedJson.Substring(0, lastValidCharIndex + 1); Logger.Information("[{Provider}] Attempting JSON Fix 1 (Removed trailing chars)...", ProviderType); currentFixApplied = true; } // Changed LogInformation to Information
            else if (trimmedJson.Contains("\"items\": [") && (!trimmedJson.EndsWith("]}") && !trimmedJson.EndsWith("]}"))) { string fixAttempt = trimmedJson; int openBrackets = fixAttempt.Count(c => c == '['); int closeBrackets = fixAttempt.Count(c => c == ']'); int openBraces = fixAttempt.Count(c => c == '{'); int closeBraces = fixAttempt.Count(c => c == '}'); if (lastNonWhite == ',') { fixAttempt = fixAttempt.TrimEnd().TrimEnd(','); if (openBraces > closeBraces) closeBraces++; } else if (lastNonWhite == '{') { if (openBraces > closeBraces) openBraces--; /*Fix potential mismatch*/} else if (lastNonWhite == '[') { if (openBrackets > closeBrackets) openBrackets--; /*Fix potential mismatch*/ } while (closeBraces < openBraces) { fixAttempt += '}'; closeBraces++; } while (closeBrackets < openBrackets) { fixAttempt += ']'; closeBrackets++; } if (fixAttempt.Count(c => c == '{') > fixAttempt.Count(c => c == '}')) fixAttempt += '}'; fixedJson = fixAttempt; Logger.Information("[{Provider}] Attempting JSON Fix 2 (Appended missing brackets/braces)...", ProviderType); currentFixApplied = true; } // Changed LogInformation to Information
            if (currentFixApplied) { try { JObject.Parse(fixedJson); Logger.Information("[{Provider}] JSON Fix successful.", ProviderType); fixApplied = true; return fixedJson; } catch (JsonReaderException) { Logger.Warning("[{Provider}] JSON Fix attempt failed validation. Returning original potentially invalid JSON.", ProviderType); return trimmedJson; } catch (Exception ex) { Logger.Error(ex, "[{Provider}] Unexpected error validating fixed JSON. Returning original.", ProviderType); return trimmedJson; } } // Changed LogInformation to Information, LogWarning to Warning, and LogError to Error
            Logger.Warning("[{Provider}] No applicable JSON fix found or fix failed. Returning original potentially invalid JSON: {JsonContent}", ProviderType, TruncateForLog(trimmedJson)); return trimmedJson; // Changed LogWarning to Warning
        }

        protected string SanitizeApiResponse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent)) return string.Empty;
            return Regex.Replace(responseContent, @"^```(json)?\s*|\s*```$", "", RegexOptions.IgnoreCase).Trim();
        }

        protected string SanitizeInputText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            string truncated = input.Length > MaxDescriptionLength ? input.Substring(0, MaxDescriptionLength) : input;
            string sanitized = Regex.Replace(truncated, SanitizePattern, "");
            sanitized = sanitized.Replace("\"", "'").Replace("\\", "\\\\").Replace("\n", " ").Replace("\r", " ").Trim();
            return string.IsNullOrWhiteSpace(sanitized) ? "SANITIZED_EMPTY" : sanitized;
        }

        protected string SanitizeItemNumber(string itemNumber)
        {
            if (string.IsNullOrWhiteSpace(itemNumber)) return "NEW";
            var cleaned = Regex.Replace(itemNumber, @"[^\w-]", "").Trim();
            if (string.IsNullOrEmpty(cleaned)) { Logger.Verbose("[{Provider}] Sanitizing ItemNumber: Input '{Input}' became empty.", ProviderType, itemNumber); return "NEW"; } // Changed LogTrace to Trace
            if (cleaned.Length > 20) { cleaned = cleaned.Substring(0, 20); Logger.Verbose("[{Provider}] Sanitizing ItemNumber: Input '{Input}' truncated to '{Cleaned}'.", ProviderType, itemNumber, cleaned); } // Changed LogTrace to Trace
            if (!Regex.IsMatch(cleaned, ItemNumberPattern)) { Logger.Verbose("[{Provider}] Sanitizing ItemNumber: Cleaned '{Cleaned}' failed pattern '{Pattern}'.", ProviderType, cleaned, ItemNumberPattern); return "NEW"; } // Changed LogTrace to Trace
            return cleaned.ToUpperInvariant();
        }

        protected string SanitizeTariffCode(string contextIdentifier, string tariffCode, string fieldName = "HSCode")
        {
            if (string.IsNullOrWhiteSpace(tariffCode)) return "00000000";
            var cleaned = Regex.Replace(tariffCode, @"\D", "");
            if (string.IsNullOrEmpty(cleaned)) { Logger.Verbose("[{Provider}] Invalid {FieldName} (empty after removing non-digits: '{TariffCode}') for Context '{Context}'. Returning default.", ProviderType, fieldName, tariffCode, contextIdentifier); return "00000000"; } // Changed LogTrace to Trace
            if (cleaned.Length == 8) { if (cleaned != tariffCode.Trim()) Logger.Verbose("[{Provider}] Sanitized {FieldName} (removed non-digits from '{TariffCode}') for Context '{Context}'. Using '{CleanedValue}'.", ProviderType, fieldName, tariffCode, contextIdentifier, cleaned); else Logger.Verbose("[{Provider}] Valid {FieldName} format '{TariffCode}' for Context '{Context}'.", ProviderType, fieldName, tariffCode, contextIdentifier); return cleaned; } // Changed LogTrace to Trace
            else if (cleaned.Length < 8) { Logger.Warning("[{Provider}] Invalid {FieldName} format (too short: '{TariffCode}', cleaned: '{Cleaned}') for Context '{Context}'. Padding.", ProviderType, fieldName, tariffCode, cleaned, contextIdentifier); return cleaned.PadRight(8, '0'); } // Changed LogWarning to Warning
            else { Logger.Warning("[{Provider}] Invalid {FieldName} format (too long: '{TariffCode}', cleaned: '{Cleaned}') for Context '{Context}'. Truncating.", ProviderType, fieldName, tariffCode, cleaned, contextIdentifier); return cleaned.Substring(0, 8); } // Changed LogWarning to Warning
        }

        protected string SanitizeProductCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "MISSING-CODE";
            var sanitized = Regex.Replace(code.Trim(), @"[^a-zA-Z0-9-]", "");
            sanitized = Regex.Replace(sanitized, @"-+", "-").Trim('-');
            sanitized = sanitized.ToUpperInvariant();
            if (sanitized.Length == 0) return "INVALID-CODE";
            if (sanitized.Length < 3) return "SHORT-CODE";
            if (sanitized.Length > 20) sanitized = sanitized.Substring(0, 20).TrimEnd('-');
            if (sanitized.Length < 3) return "SHORT-CODE";
            if (!Regex.IsMatch(sanitized, ItemNumberPattern)) return "INVALID-CODE";
            return sanitized;
        }

        protected string ParseHsCodeFromText(string textContent) // Fallback if domain parsing fails
        {
            if (string.IsNullOrWhiteSpace(textContent)) return "00000000";
            try { var pattern = @"""?(\d{8})""?"; var match = Regex.Match(textContent, pattern); if (match.Success && match.Groups.Count > 1) { var code = match.Groups[1].Value; Logger.Debug("[{Provider}] Extracted HS code using Regex fallback: {HsCode}", ProviderType, code); return SanitizeTariffCode("RegexFallback", code, "ItemHS_Regex"); } Logger.Warning("[{Provider}] Could not find 8-digit HS code using Regex pattern '{Pattern}' in text: {Text}", ProviderType, pattern, TruncateForLog(textContent, 100)); return "00000000"; } // Changed LogDebug to Debug and LogWarning to Warning
            catch (RegexMatchTimeoutException ex) { Logger.Error(ex, "[{Provider}] Regex timed out parsing HS code from text.", ProviderType); return "00000000"; } // Changed LogError to Error
            catch (Exception ex) { Logger.Error(ex, "[{Provider}] Error during Regex HS code parsing from text.", ProviderType); return "00000000"; } // Changed LogError to Error
        }

        protected string TruncateForLog(string text, int maxLength = 250)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...(truncated)";
        }
    }
}