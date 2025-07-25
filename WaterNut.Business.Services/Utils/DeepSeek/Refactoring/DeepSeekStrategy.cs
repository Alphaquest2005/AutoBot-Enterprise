#nullable disable
using Serilog; // Added
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly.Retry; // Needed for base constructor
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading; // Added
using System.Threading.Tasks; // Added

namespace WaterNut.Business.Services.Utils.LlmApi
{
    // Inherit from base class and implement interface
    public class DeepSeekStrategy : LlmStrategyBase, ILLMProviderStrategy
    {
        public override LLMProvider ProviderType => LLMProvider.DeepSeek;

        private const int FALLBACK_MAX_TOKENS = 150; // Max tokens for fallback single item

        private const string DeepSeekBaseUrl = "https://api.deepseek.com/v1";
        private const string DefaultModelInternal = "deepseek-chat"; // Internal constant

        private static readonly Dictionary<string, ModelPricing> Pricing =
            new Dictionary<string, ModelPricing>(StringComparer.OrdinalIgnoreCase)
            { [DefaultModelInternal] = new ModelPricing { InputPricePerMillionTokens = 0.14m, OutputPricePerMillionTokens = 0.28m } };

        // Constructor passes dependencies to base
        public DeepSeekStrategy(string apiKey, Serilog.ILogger logger, HttpClient httpClient, AsyncRetryPolicy retryPolicy) // Changed ILogger to Serilog.ILogger
            : base(apiKey, logger, httpClient, retryPolicy) // Pass the Serilog logger to base
        {
            // Set provider-specific defaults if different from base
            // e.g., this.DefaultTemperature = 0.2;
            SetDefaultPromptsInternal(); // Ensure prompts are set
        }

        // --- Implement Abstract Methods ---
        public override string GetDefaultModelName() => DefaultModelInternal;
        public override ModelPricing? GetPricing(string modelName) => Pricing.TryGetValue(modelName ?? DefaultModelInternal, out var p) ? p : null;
        protected override string GetApiUrl(string modelName) => $"{DeepSeekBaseUrl}/chat/completions";

        protected override void AddAuthentication(HttpRequestMessage requestMessage, string apiKey)
        {
            // Base class ApiKey is used implicitly by PostRequestAsync if needed,
            // but AddAuthentication delegate pattern requires passing it.
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        protected override object BuildRequestBody(string prompt, int maxTokens, double temperature, string modelName)
        {
            // Use strongly-typed model defined in LlmApiRequestModels.cs
            var request = new DeepSeekRequestBody
            {
                Model = modelName ?? GetDefaultModelName(), // Use property or default
                Messages = new List<DeepSeekMessage>
                {
                    new DeepSeekMessage { Role = "user", Content = prompt }
                },
                Temperature = temperature,
                MaxTokens = maxTokens,
                Stream = false
            };
            return request;
        }

        protected override (string Completion, TokenUsage Usage) ParseProviderResponse(string jsonResponse)
        {
            // Parses the *provider's* specific JSON structure to get completion text and usage
            TokenUsage usage = new TokenUsage { IsEstimated = true };
            string completion = string.Empty;
            if (string.IsNullOrWhiteSpace(jsonResponse)) { Logger.Warning("[DeepSeek] ParseProviderResponse received empty JSON."); return (completion, usage); } // Changed LogWarning to Warning
            try
            {
                JObject responseObj = JObject.Parse(jsonResponse);
                completion = responseObj?["choices"]?.FirstOrDefault()?["message"]?["content"]?.Value<string>() ?? string.Empty;
                var usageData = responseObj?["usage"];
                if (usageData != null)
                {
                    usage.InputTokens = usageData["prompt_tokens"]?.Value<int>() ?? 0;
                    usage.OutputTokens = usageData["completion_tokens"]?.Value<int>() ?? 0;
                    usage.IsEstimated = false;
                    Logger.Verbose("[DeepSeek] Parsed Usage - Input: {InputTokens}, Output: {OutputTokens}", usage.InputTokens, usage.OutputTokens); // Changed LogTrace to Trace
                 }
                 else { Logger.Warning("[DeepSeek] No 'usage' data found in provider response. Estimating."); usage.InputTokens = 0; usage.OutputTokens = EstimateTokenCount(completion); } // Changed LogWarning to Warning
                 if (string.IsNullOrEmpty(completion)) { CheckAndLogApiError(responseObj); } // Use helper from base
                return (completion, usage);
            }
            catch (JsonReaderException jsonEx) { Logger.Error(jsonEx, "[DeepSeek] Failed to parse provider API response JSON."); throw new LlmApiException("Failed to parse DeepSeek provider JSON response.", jsonEx); } // Changed LogError to Error
            catch (Exception ex) { Logger.Error(ex, "[DeepSeek] Error processing provider response content/usage."); throw new LlmApiException("Error processing DeepSeek provider response.", ex); } // Changed LogError to Error
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
                if (string.IsNullOrEmpty(SingleItemPromptTemplate)) throw new InvalidOperationException("SingleItemPromptTemplate is not set for DeepSeekStrategy.");
                string prompt = SingleItemPromptTemplate.Replace("__DESCRIPTION_HERE__", cleanDescription).Replace("__PRODUCT_CODE_HERE__", finalProductCode);

                // Execute API call using base helper
                var (completion, usage, error, callSuccess) = await ExecuteProviderApiCallAsync(prompt, temperatureOverride, maxTokensOverride ?? FALLBACK_MAX_TOKENS, cancellationToken).ConfigureAwait(false);

                response.Usage = usage;
                response.Cost = CalculateCost(usage);

                if (!callSuccess)
                {
                    response.ErrorMessage = error ?? "API call failed.";
                    return response;
                }

                // Parse the domain-specific format from the completion string
                var (tariffCode, category, categoryHsCode) = ParseSingleItemResponseFormat(completion);

                // Check if domain parsing failed (indicated by ERROR return)
                if (tariffCode == "ERROR")
                {
                    response.ErrorMessage = "Failed to parse expected JSON format from LLM response.";
                    response.IsSuccess = false; // Mark as overall failure if parsing failed
                                                // Optionally include the raw completion in the error message for debugging
                    response.Result = new ClassificationResult { OriginalDescription = itemDescription, ItemNumber = finalProductCode, TariffCode = "ERROR", Category = "ERROR", CategoryHsCode = "ERROR", ParsedSuccessfully = false };
                }
                else
                {
                    response.Result = new ClassificationResult
                    {
                        OriginalDescription = itemDescription, // Add context
                        ItemNumber = finalProductCode,
                        TariffCode = tariffCode,
                        Category = category,
                        CategoryHsCode = categoryHsCode,
                        ParsedSuccessfully = true
                    };
                    response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[DeepSeek] Unexpected error in GetSingleClassificationAsync for: {ItemDescription}", itemDescription); // Changed LogError to Error
                response.ErrorMessage = $"Unexpected error: {ex.Message}";
                response.IsSuccess = false;
                // Ensure usage/cost calculated so far are returned
            }
            return response;
        }

        public async Task<BatchClassificationResponse> GetBatchClassificationAsync(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
            double? temperatureOverride, int? maxTokensOverride,
            CancellationToken cancellationToken)
        {
            var response = new BatchClassificationResponse { IsSuccess = false, AggregatedUsage = new TokenUsage { IsEstimated = true } };
            if (items == null || !items.Any()) { response.IsSuccess = true; return response; } // Empty input is success

            try
            {
                if (string.IsNullOrEmpty(BatchItemPromptTemplate)) throw new InvalidOperationException("BatchItemPromptTemplate is not set for DeepSeekStrategy.");
                string batchPrompt = CreateBatchPromptInternal(items); // Use internal helper to build prompt

                // Estimate required tokens for the response dynamically if not provided
                int? finalMaxTokens = maxTokensOverride;
                if (!finalMaxTokens.HasValue)
                {
                    int estimatedResponseTokensPerItem = 150; // Example estimate
                    int estimatedTotalResponseTokens = items.Count * estimatedResponseTokensPerItem;
                    finalMaxTokens = CalculateSafeMaxTokens(batchPrompt, estimatedTotalResponseTokens);
                }

                // Execute API call
                var (completion, usage, error, callSuccess) = await ExecuteProviderApiCallAsync(batchPrompt, temperatureOverride, finalMaxTokens, cancellationToken).ConfigureAwait(false);

                response.AggregatedUsage = usage; // Store potentially estimated usage
                response.TotalCost = CalculateCost(usage); // Calculate cost based on usage

                if (!callSuccess)
                {
                    response.ErrorMessage = error ?? "Batch API call failed.";
                    // Add all items to failed list if the whole call fails
                    response.FailedDescriptions.AddRange(items.Select(i => i.ItemDescription));
                    return response;
                }

                // Parse the domain-specific batch format from the completion string
                var parsedResults = ParseBatchResponseFormat(completion); // Can throw FormatException

                response.Results = parsedResults;
                response.IsSuccess = true; // Mark API call success, individual items might have failed parsing inside ParseBatchResponseFormat

                // Identify items that were sent but not successfully parsed/returned
                var returnedDescriptions = new HashSet<string>(parsedResults.Keys);
                foreach (var item in items)
                {
                    if (!returnedDescriptions.Contains(item.ItemDescription))
                    {
                        response.FailedDescriptions.Add(item.ItemDescription);
                        Logger.Warning("[DeepSeek] Item '{Description}' was in the batch request but not found or parsed in the response.", item.ItemDescription); // Changed LogWarning to Warning
                    }
                }
                if (response.FailedDescriptions.Any())
                {
                    // If some items failed, maybe the overall success is partial? Decide business logic.
                    // For now, IsSuccess reflects the API call success.
                    Logger.Warning("[DeepSeek] {FailCount} items failed to be processed correctly within the successful batch call.", response.FailedDescriptions.Count); // Changed LogWarning to Warning
                }

            }
            // Catch FormatException specifically from ParseBatchResponseFormat
            catch (FormatException formatEx)
            {
                Logger.Error(formatEx, "[DeepSeek] Failed to parse the format of the batch response content."); // Changed LogError to Error
                response.ErrorMessage = "Failed to parse batch response format.";
                response.IsSuccess = false;
                response.FailedDescriptions.AddRange(items.Select(i => i.ItemDescription)); // All failed if format is wrong
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[DeepSeek] Unexpected error in GetBatchClassificationAsync."); // Changed LogError to Error
                response.ErrorMessage = $"Unexpected error: {ex.Message}";
                response.IsSuccess = false;
                response.FailedDescriptions.AddRange(items.Select(i => i.ItemDescription)); // Mark all as failed
            }
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

                // Execute API call - use low temp, low max tokens
                var (completion, usage, error, callSuccess) = await ExecuteProviderApiCallAsync(prompt, 0.2, 30, cancellationToken).ConfigureAwait(false);

                response.Usage = usage;
                response.Cost = CalculateCost(usage);

                if (!callSuccess) { response.ErrorMessage = error ?? "Code generation API call failed."; return response; }

                string productCode = SanitizeProductCode(completion);
                if (productCode.Contains("ERROR") || productCode.Contains("INVALID") || productCode.Contains("SHORT") || productCode.Contains("MISSING"))
                {
                    response.ErrorMessage = $"LLM returned invalid code format: '{productCode}'";
                    response.ProductCode = "ERROR-CODE"; // Standardize error
                }
                else
                {
                    response.ProductCode = productCode;
                    response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[DeepSeek] Failed to generate product code for Description: {Description}", description); // Changed LogError to Error
                response.ErrorMessage = $"Unexpected error: {ex.Message}";
            }
            return response;
        }

        // --- Internal Helpers ---
        private string CreateBatchPromptInternal(List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            if (string.IsNullOrEmpty(BatchItemPromptTemplate)) throw new InvalidOperationException("BatchItemPromptTemplate is not set.");
            var sb = new StringBuilder();
            sb.AppendLine(BatchItemPromptTemplate);
            foreach (var item in items) { var safeDesc = SanitizeInputText(item.ItemDescription); sb.AppendLine($"- DESC: \"{safeDesc}\" | CODE: {item.ItemNumber} | HS_INPUT: {item.TariffCode}"); }
            sb.AppendLine("Respond ONLY with the complete, valid JSON object containing the 'items' array.");
            return sb.ToString();
        }

        // Base class helpers are available: Sanitize*, TryFixJson, CalculateCost, EstimateTokenCount etc.
        // Provider specific helpers if needed:
        private void CheckAndLogApiError(JObject responseObj)
        {
            var error = responseObj?["error"]?["message"]?.Value<string>();
            if (!string.IsNullOrEmpty(error)) { Logger.Error("[DeepSeek] API returned an error message in response body: {ErrorMessage}", error); } // Changed LogError to Error
        }

        /// <summary>
        /// Raw prompt method for OCR correction integration.
        /// Matches the interface of the old DeepSeekInvoiceApi.GetResponseAsync.
        /// Provides direct prompt/response capability with proper retry and fallback support.
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            Logger.Information("[{Provider}] Raw prompt request - Length: {PromptLength}, Temperature: {Temperature}, MaxTokens: {MaxTokens}", 
                ProviderType, prompt.Length, temperature ?? DefaultTemperature, maxTokens ?? DefaultMaxTokens);

            try
            {
                // Use provided values or defaults
                var temp = temperature ?? DefaultTemperature;
                var tokens = maxTokens ?? DefaultMaxTokens;
                
                // Create request body matching DeepSeek API format
                var requestBody = new
                {
                    model = Model,
                    messages = new[] { new { role = "user", content = prompt } },
                    temperature = temp,
                    max_tokens = tokens,
                    stream = false
                };
                
                // Use base class PostRequestAsync with proper error handling and retry
                var apiUrl = GetApiUrl(Model);
                var responseJson = await PostRequestAsync(apiUrl, requestBody, AddAuthentication, cancellationToken);
                
                // Parse response and extract content
                var responseObj = JObject.Parse(responseJson);
                CheckAndLogApiError(responseObj); // Check for API errors
                
                var content = responseObj["choices"]?[0]?["message"]?["content"]?.Value<string>();
                
                if (string.IsNullOrEmpty(content))
                {
                    Logger.Error("[{Provider}] No content found in API response", ProviderType);
                    throw new InvalidOperationException($"{ProviderType} API returned empty content");
                }
                
                Logger.Information("[{Provider}] Raw prompt response received - Length: {ResponseLength}", ProviderType, content.Length);
                return content;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[{Provider}] Raw prompt request failed: {ErrorMessage}", ProviderType, ex.Message);
                throw; // Re-throw for retry policy or fallback handling
            }
        }

    }
}