#nullable disable
using Serilog; // Added
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly.Retry; // Needed for base constructor
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading; // Added
using System.Threading.Tasks; // Added

namespace WaterNut.Business.Services.Utils.LlmApi
{
    // Inherit from base class and implement interface
    public class GeminiStrategy : LlmStrategyBase, ILLMProviderStrategy
    {
        public override LLMProvider ProviderType => LLMProvider.Gemini;

        private const int FALLBACK_MAX_TOKENS = 150; // Max tokens for fallback single item

        private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";
        private const string DefaultModelInternal = "gemini-1.5-flash-latest";

        private static readonly Dictionary<string, ModelPricing> Pricing =
            new Dictionary<string, ModelPricing>(StringComparer.OrdinalIgnoreCase)
            {
                [DefaultModelInternal] = new ModelPricing { InputPricePerMillionTokens = 0.35m, OutputPricePerMillionTokens = 1.05m },
                ["gemini-1.5-pro-latest"] = new ModelPricing { InputPricePerMillionTokens = 3.50m, OutputPricePerMillionTokens = 10.50m }
            };

        // Constructor passes dependencies to base
        public GeminiStrategy(string apiKey, Serilog.ILogger logger, HttpClient httpClient, AsyncRetryPolicy retryPolicy) // Changed ILogger to Serilog.ILogger
            : base(apiKey, logger, httpClient, retryPolicy) // Pass the Serilog logger to base
        {
            SetDefaultPromptsInternal(); // Ensure prompts are set
        }

        // --- Implement Abstract Methods ---
        public override string GetDefaultModelName() => DefaultModelInternal;
        public override ModelPricing? GetPricing(string modelName) => Pricing.TryGetValue(modelName ?? DefaultModelInternal, out var p) ? p : null;

        protected override string GetApiUrl(string modelName) => $"{GeminiBaseUrl}/{modelName ?? GetDefaultModelName()}:generateContent"; // URL *without* key initially

        protected override void AddAuthentication(HttpRequestMessage requestMessage, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            requestMessage.Headers.TryAddWithoutValidation("x-goog-api-key", apiKey);
            // Key is added to URL *here* because it's often mandatory for Gemini REST
            var uriBuilder = new UriBuilder(requestMessage.RequestUri ?? throw new InvalidOperationException("Request URI null"));
            string existingQuery = uriBuilder.Query;
            string queryToAppend = $"key={Uri.EscapeDataString(apiKey)}";
            uriBuilder.Query = (!string.IsNullOrEmpty(existingQuery) && existingQuery.Length > 1) ? existingQuery.Substring(1) + "&" + queryToAppend : queryToAppend;
            requestMessage.RequestUri = uriBuilder.Uri;
        }

        protected override object BuildRequestBody(string prompt, int maxTokens, double temperature, string modelName)
        {
            // Use strongly-typed model defined in LlmApiRequestModels.cs
            var request = new GeminiRequestBody
            {
                Contents = new List<GeminiContent>
                 {
                     new GeminiContent { Parts = new List<GeminiPart> { new GeminiPart { Text = prompt } } }
                     // Add role if needed: new GeminiContent { Role="user", Parts = ... }
                 },
                GenerationConfig = new GeminiGenerationConfig
                {
                    Temperature = temperature,
                    MaxOutputTokens = maxTokens
                    // CandidateCount = 1 // Optional
                }
                // SafetySettings = ... // Optional
            };
            return request;
        }

        protected override (string Completion, TokenUsage Usage) ParseProviderResponse(string jsonResponse)
        {
            // Parses the *provider's* specific JSON structure to get completion text and usage
            TokenUsage usage = new TokenUsage { IsEstimated = true };
            string completion = string.Empty;
            if (string.IsNullOrWhiteSpace(jsonResponse)) { Logger.Warning("[Gemini] ParseProviderResponse received empty JSON."); return (completion, usage); } // Changed LogWarning to Warning
            try
            {
                JObject responseObj = JObject.Parse(jsonResponse);
                var candidate = responseObj?["candidates"]?.FirstOrDefault();
                completion = candidate?["content"]?["parts"]?.FirstOrDefault()?["text"]?.Value<string>() ?? string.Empty;

                if (candidate?["content"]?["parts"] == null && candidate?["finishReason"]?.Value<string>() == "SAFETY") { Logger.Error("[Gemini] Content blocked due to safety (no 'parts'). FinishReason: SAFETY"); throw new LlmApiException("[Gemini] Content blocked (FinishReason: SAFETY)."); } // Changed LogError to Error

                var usageData = responseObj?["usageMetadata"] ?? candidate?["usageMetadata"];
                if (usageData != null)
                {
                    usage.InputTokens = usageData["promptTokenCount"]?.Value<int>() ?? 0;
                    int candidatesTokens = usageData["candidatesTokenCount"]?.Value<int>() ?? 0;
                    int totalTokens = usageData["totalTokenCount"]?.Value<int>() ?? 0;
                    usage.OutputTokens = candidatesTokens > 0 ? candidatesTokens : (totalTokens - usage.InputTokens >= 0 ? totalTokens - usage.InputTokens : EstimateTokenCount(completion));
                    usage.IsEstimated = false;
                    Logger.Verbose("[Gemini] Parsed Usage - Input: {In}, Output: {Out}, Cand: {Cand}, Total: {Total}", usage.InputTokens, usage.OutputTokens, candidatesTokens, totalTokens); // Changed LogTrace to Verbose
                }
                else { Logger.Warning("[Gemini] No 'usageMetadata' found in provider response. Estimating."); usage.InputTokens = 0; usage.OutputTokens = EstimateTokenCount(completion); } // Changed LogWarning to Warning

                var finishReason = candidate?["finishReason"]?.Value<string>();
                if (finishReason != null && finishReason != "STOP" && finishReason != "MAX_TOKENS") { Logger.Warning("[Gemini] API response finishReason: {FinishReason}.", finishReason); if (finishReason == "SAFETY") throw new LlmApiException($"[Gemini] Content blocked (FinishReason: {finishReason})."); } // Changed LogWarning to Warning
                if (string.IsNullOrEmpty(completion) && (finishReason == "STOP" || finishReason == "MAX_TOKENS")) { Logger.Warning("[Gemini] Response indicates success but completion text is empty. Resp: {JsonResponse}", jsonResponse); } // Changed LogWarning to Warning
                else if (string.IsNullOrEmpty(completion)) { CheckAndLogApiError(responseObj); }

                return (completion, usage);
            }
            catch (JsonReaderException jsonEx) { Logger.Error(jsonEx, "[Gemini] Failed to parse provider API response JSON."); throw new LlmApiException("Failed to parse Gemini provider JSON response.", jsonEx); } // Changed LogError to Error
            catch (LlmApiException) { throw; } // Don't re-wrap our own
            catch (Exception ex) { Logger.Error(ex, "[Gemini] Error processing provider response content/usage."); throw new LlmApiException("Error processing Gemini provider response.", ex); } // Changed LogError to Error
        }


        // --- Implement ILLMProviderStrategy High-Level Methods ---

        public async Task<ClassificationResponse> GetSingleClassificationAsync(
            string itemDescription, string productCode,
            double? temperatureOverride, int? maxTokensOverride,
            CancellationToken cancellationToken)
        {
            var response = new ClassificationResponse { IsSuccess = false, Usage = new TokenUsage { IsEstimated = true } };
            try
            {
                string cleanDescription = SanitizeInputText(itemDescription);
                string finalProductCode = string.IsNullOrWhiteSpace(productCode) ? "N/A" : SanitizeProductCode(productCode);
                if (string.IsNullOrEmpty(SingleItemPromptTemplate)) throw new InvalidOperationException("SingleItemPromptTemplate is not set for GeminiStrategy.");
                string prompt = SingleItemPromptTemplate.Replace("__DESCRIPTION_HERE__", cleanDescription).Replace("__PRODUCT_CODE_HERE__", finalProductCode);

                var (completion, usage, error, callSuccess) = await ExecuteProviderApiCallAsync(prompt, temperatureOverride, maxTokensOverride ?? FALLBACK_MAX_TOKENS, cancellationToken).ConfigureAwait(false);
                response.Usage = usage; response.Cost = CalculateCost(usage);

                if (!callSuccess) { response.ErrorMessage = error ?? "API call failed."; return response; }

                var (tariffCode, category, categoryHsCode) = ParseSingleItemResponseFormat(completion);
                if (tariffCode == "ERROR") { response.ErrorMessage = "Failed to parse expected JSON format from LLM response."; response.IsSuccess = false; response.Result = new ClassificationResult { OriginalDescription = itemDescription, ItemNumber = finalProductCode, TariffCode = "ERROR", Category = "ERROR", CategoryHsCode = "ERROR", ParsedSuccessfully = false }; }
                else { response.Result = new ClassificationResult { OriginalDescription = itemDescription, ItemNumber = finalProductCode, TariffCode = tariffCode, Category = category, CategoryHsCode = categoryHsCode, ParsedSuccessfully = true }; response.IsSuccess = true; }
            }
            catch (Exception ex) { Logger.Error(ex, "[Gemini] Unexpected error in GetSingleClassificationAsync for: {ItemDescription}", itemDescription); response.ErrorMessage = $"Unexpected error: {ex.Message}"; response.IsSuccess = false; } // Changed LogError to Error
            return response;
        }

        public async Task<BatchClassificationResponse> GetBatchClassificationAsync(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
            double? temperatureOverride, int? maxTokensOverride,
            CancellationToken cancellationToken)
        {
            var response = new BatchClassificationResponse { IsSuccess = false, AggregatedUsage = new TokenUsage { IsEstimated = true } };
            if (items == null || !items.Any()) { response.IsSuccess = true; return response; }
            try
            {
                if (string.IsNullOrEmpty(BatchItemPromptTemplate)) throw new InvalidOperationException("BatchItemPromptTemplate is not set for GeminiStrategy.");
                string batchPrompt = CreateBatchPromptInternal(items);

                int? finalMaxTokens = maxTokensOverride;
                if (!finalMaxTokens.HasValue) { int estimatedResponseTokensPerItem = 150; int estimatedTotalResponseTokens = items.Count * estimatedResponseTokensPerItem; finalMaxTokens = CalculateSafeMaxTokens(batchPrompt, estimatedTotalResponseTokens); }

                var (completion, usage, error, callSuccess) = await ExecuteProviderApiCallAsync(batchPrompt, temperatureOverride, finalMaxTokens, cancellationToken).ConfigureAwait(false);
                response.AggregatedUsage = usage; response.TotalCost = CalculateCost(usage);

                if (!callSuccess) { response.ErrorMessage = error ?? "Batch API call failed."; response.FailedDescriptions.AddRange(items.Select(i => i.ItemDescription)); return response; }

                var parsedResults = ParseBatchResponseFormat(completion);
                response.Results = parsedResults; response.IsSuccess = true;
                var returnedDescriptions = new HashSet<string>(parsedResults.Keys);
                foreach (var item in items) { if (!returnedDescriptions.Contains(item.ItemDescription)) { response.FailedDescriptions.Add(item.ItemDescription); Logger.Warning("[Gemini] Item '{Description}' requested but not in parsed batch response.", item.ItemDescription); } } // Changed LogWarning to Warning
                if (response.FailedDescriptions.Any()) { Logger.Warning("[Gemini] {FailCount} items failed processing within the successful batch call.", response.FailedDescriptions.Count); } // Changed LogWarning to Warning
            }
            catch (FormatException formatEx) { Logger.Error(formatEx, "[Gemini] Failed to parse the format of the batch response content."); response.ErrorMessage = "Failed to parse batch response format."; response.IsSuccess = false; response.FailedDescriptions.AddRange(items.Select(i => i.ItemDescription)); } // Changed LogError to Error
            catch (Exception ex) { Logger.Error(ex, "[Gemini] Unexpected error in GetBatchClassificationAsync."); response.ErrorMessage = $"Unexpected error: {ex.Message}"; response.IsSuccess = false; response.FailedDescriptions.AddRange(items.Select(i => i.ItemDescription)); } // Changed LogError to Error
            return response;
        }

        public async Task<ProductCodeResponse> GenerateProductCodeAsync(
           string description,
           CancellationToken cancellationToken)
        {
            var response = new ProductCodeResponse { IsSuccess = false, Usage = new TokenUsage { IsEstimated = true }, ProductCode = "ERROR-CODE" };
            if (string.IsNullOrWhiteSpace(description)) { response.ErrorMessage = "Empty description."; return response; }
            try
            {
                string cleanDesc = SanitizeInputText(description);
                if (string.IsNullOrEmpty(GenerateCodePromptTemplate)) throw new InvalidOperationException("GenerateCodePromptTemplate is not set.");
                string prompt = string.Format(GenerateCodePromptTemplate, cleanDesc);

                var (completion, usage, error, callSuccess) = await ExecuteProviderApiCallAsync(prompt, 0.2, 30, cancellationToken).ConfigureAwait(false);
                response.Usage = usage; response.Cost = CalculateCost(usage);

                if (!callSuccess) { response.ErrorMessage = error ?? "Code generation API call failed."; return response; }

                string productCode = SanitizeProductCode(completion);
                if (productCode.Contains("ERROR") || productCode.Contains("INVALID") || productCode.Contains("SHORT") || productCode.Contains("MISSING"))
                { response.ErrorMessage = $"LLM returned invalid code format: '{productCode}'"; response.ProductCode = "ERROR-CODE"; }
                else { response.ProductCode = productCode; response.IsSuccess = true; }
            }
            catch (Exception ex) { Logger.Error(ex, "[Gemini] Failed to generate product code for Description: {Description}", description); response.ErrorMessage = $"Unexpected error: {ex.Message}"; } // Changed LogError to Error
            return response;
        }


        // --- Internal Helpers ---
        private string CreateBatchPromptInternal(List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            if (string.IsNullOrEmpty(BatchItemPromptTemplate)) throw new InvalidOperationException("BatchItemPromptTemplate is not set.");
            var sb = new StringBuilder(); sb.AppendLine(BatchItemPromptTemplate);
            foreach (var item in items) { var safeDesc = SanitizeInputText(item.ItemDescription); sb.AppendLine($"- DESC: \"{safeDesc}\" | CODE: {item.ItemNumber} | HS_INPUT: {item.TariffCode}"); }
            sb.AppendLine("Respond ONLY with the complete, valid JSON object containing the 'items' array."); return sb.ToString();
        }

        // Base class helpers are available: Sanitize*, TryFixJson, CalculateCost, EstimateTokenCount etc.
        private void CheckAndLogApiError(JObject responseObj)
        {
            var error = responseObj?["error"]?["message"]?.Value<string>();
            if (!string.IsNullOrEmpty(error)) { Logger.Error("[Gemini] API returned an error message in response body: {ErrorMessage}", error); } // Changed LogError to Error
        }

        /// <summary>
        /// Raw prompt method for OCR correction integration.
        /// Provides Gemini fallback capability when DeepSeek fails.
        /// Matches the interface for seamless provider switching.
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            Logger.Information("[{Provider}] Raw prompt request (FALLBACK) - Length: {PromptLength}, Temperature: {Temperature}, MaxTokens: {MaxTokens}", 
                ProviderType, prompt.Length, temperature ?? DefaultTemperature, maxTokens ?? DefaultMaxTokens);

            try
            {
                // Use provided values or defaults
                var temp = temperature ?? DefaultTemperature;
                var tokens = maxTokens ?? DefaultMaxTokens;
                
                // Create request body matching Gemini API format
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = prompt } }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = temp,
                        maxOutputTokens = tokens
                    }
                };
                
                // Use base class PostRequestAsync with proper error handling and retry
                var apiUrl = GetApiUrl(Model);
                var responseJson = await PostRequestAsync(apiUrl, requestBody, AddAuthentication, cancellationToken);
                
                // Parse Gemini response format and extract content
                var responseObj = JObject.Parse(responseJson);
                CheckAndLogApiError(responseObj); // Check for API errors
                
                var content = responseObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.Value<string>();
                
                if (string.IsNullOrEmpty(content))
                {
                    Logger.Error("[{Provider}] No content found in API response", ProviderType);
                    throw new InvalidOperationException($"{ProviderType} API returned empty content");
                }
                
                Logger.Information("[{Provider}] Raw prompt response received (FALLBACK SUCCESS) - Length: {ResponseLength}", ProviderType, content.Length);
                return content;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[{Provider}] Raw prompt request failed: {ErrorMessage}", ProviderType, ex.Message);
                throw; // Re-throw for retry policy handling
            }
        }
    }
}