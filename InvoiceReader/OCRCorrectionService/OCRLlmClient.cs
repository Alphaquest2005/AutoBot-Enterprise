using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Serilog;
using Polly;
using Polly.Retry;
using Newtonsoft.Json.Linq;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// OCR-dedicated LLM client with automatic DeepSeek -> Gemini fallback capability.
    /// Self-contained implementation specifically for OCR correction service needs.
    /// Provides the same interface as the old DeepSeekInvoiceApi but with fallback support.
    /// </summary>
    public class OCRLlmClient : IDisposable
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly string _deepSeekApiKey;
        private readonly string _geminiApiKey;
        private bool _disposed = false;

        // DeepSeek Configuration
        private const string DeepSeekBaseUrl = "https://api.deepseek.com/v1";
        private const string DeepSeekModel = "deepseek-chat";
        
        // Gemini Configuration  
        private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";
        private const string GeminiModel = "gemini-1.5-flash-latest";

        public OCRLlmClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Get API keys from environment
            _deepSeekApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
            _geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            
            if (string.IsNullOrWhiteSpace(_deepSeekApiKey) && string.IsNullOrWhiteSpace(_geminiApiKey))
            {
                throw new InvalidOperationException("At least one API key must be set: DEEPSEEK_API_KEY or GEMINI_API_KEY");
            }

            // Initialize HTTP client and retry policy
            _httpClient = CreateHttpClient();
            _retryPolicy = CreateRetryPolicy();
            
            _logger.Information("🔄 **OCR_LLM_CLIENT_INIT**: Initialized with DeepSeek={HasDeepSeek}, Gemini={HasGemini}", 
                !string.IsNullOrWhiteSpace(_deepSeekApiKey), !string.IsNullOrWhiteSpace(_geminiApiKey));
        }

        /// <summary>
        /// Gets response from LLM with automatic DeepSeek -> Gemini fallback.
        /// Matches the interface of the old DeepSeekInvoiceApi.GetResponseAsync.
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            _logger.Information("🎯 **OCR_LLM_REQUEST**: Starting prompt request with fallback capability - Length: {PromptLength}", prompt.Length);

            // Try DeepSeek first (primary strategy)
            if (!string.IsNullOrWhiteSpace(_deepSeekApiKey))
            {
                try
                {
                    _logger.Information("1️⃣ **TRYING_DEEPSEEK**: Attempting DeepSeek API call");
                    var deepSeekResponse = await CallDeepSeekAsync(prompt, temperature ?? 0.3, maxTokens ?? 4096, cancellationToken);
                    
                    _logger.Information("✅ **DEEPSEEK_SUCCESS**: DeepSeek responded successfully - Length: {ResponseLength}", deepSeekResponse?.Length ?? 0);
                    return deepSeekResponse;
                }
                catch (Exception deepSeekEx)
                {
                    _logger.Warning(deepSeekEx, "⚠️ **DEEPSEEK_FAILED**: DeepSeek strategy failed: {ErrorMessage} - Attempting Gemini fallback", deepSeekEx.Message);
                }
            }
            else
            {
                _logger.Warning("⚠️ **DEEPSEEK_UNAVAILABLE**: DeepSeek API key not configured, going directly to Gemini fallback");
            }

            // Try Gemini fallback
            if (!string.IsNullOrWhiteSpace(_geminiApiKey))
            {
                try
                {
                    _logger.Information("2️⃣ **TRYING_GEMINI**: Attempting Gemini API call (fallback)");
                    var geminiResponse = await CallGeminiAsync(prompt, temperature ?? 0.3, maxTokens ?? 4096, cancellationToken);
                    
                    _logger.Information("✅ **GEMINI_SUCCESS**: Gemini responded successfully (FALLBACK SUCCESS) - Length: {ResponseLength}", geminiResponse?.Length ?? 0);
                    return geminiResponse;
                }
                catch (Exception geminiEx)
                {
                    _logger.Error(geminiEx, "❌ **GEMINI_FAILED**: Gemini strategy also failed: {ErrorMessage}", geminiEx.Message);
                    throw new InvalidOperationException($"Both DeepSeek and Gemini strategies failed. Last error: {geminiEx.Message}", geminiEx);
                }
            }
            else
            {
                throw new InvalidOperationException("No fallback strategy available - Gemini API key not configured and DeepSeek failed");
            }
        }

        private async Task<string> CallDeepSeekAsync(string prompt, double temperature, int maxTokens, CancellationToken cancellationToken)
        {
            var requestBody = new
            {
                model = DeepSeekModel,
                messages = new[] { new { role = "user", content = prompt } },
                temperature = temperature,
                max_tokens = maxTokens,
                stream = false
            };

            var apiUrl = $"{DeepSeekBaseUrl}/chat/completions";
            var responseJson = await PostRequestAsync(apiUrl, requestBody, (request, apiKey) =>
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }, _deepSeekApiKey, cancellationToken);

            // Parse DeepSeek response
            var responseObj = JObject.Parse(responseJson);
            CheckForApiError(responseObj, "DeepSeek");
            
            var content = responseObj["choices"]?[0]?["message"]?["content"]?.Value<string>();
            if (string.IsNullOrEmpty(content))
            {
                throw new InvalidOperationException("DeepSeek API returned empty content");
            }
            
            return content;
        }

        private async Task<string> CallGeminiAsync(string prompt, double temperature, int maxTokens, CancellationToken cancellationToken)
        {
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
                    temperature = temperature,
                    maxOutputTokens = maxTokens
                }
            };

            var apiUrl = $"{GeminiBaseUrl}/{GeminiModel}:generateContent?key={Uri.EscapeDataString(_geminiApiKey)}";
            var responseJson = await PostRequestAsync(apiUrl, requestBody, (request, apiKey) =>
            {
                request.Headers.TryAddWithoutValidation("x-goog-api-key", apiKey);
            }, _geminiApiKey, cancellationToken);

            // Parse Gemini response
            var responseObj = JObject.Parse(responseJson);
            CheckForApiError(responseObj, "Gemini");
            
            var content = responseObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.Value<string>();
            if (string.IsNullOrEmpty(content))
            {
                throw new InvalidOperationException("Gemini API returned empty content");
            }
            
            return content;
        }

        private async Task<string> PostRequestAsync(string apiUrl, object requestBody, Action<HttpRequestMessage, string> addAuth, string apiKey, CancellationToken cancellationToken)
        {
            var context = new Context($"OCRLlmRequest-{Guid.NewGuid()}");
            context["Logger"] = _logger;
            context["ProviderType"] = apiUrl.Contains("deepseek") ? "DeepSeek" : "Gemini";

            return await _retryPolicy.ExecuteAsync(async (ctx, ct) =>
            {
                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                
                using (var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl) { Content = content })
                {
                    addAuth(requestMessage, apiKey);
                    
                    _logger.Debug("**HTTP_REQUEST**: POST to {Url}, Size: {Size} bytes", requestMessage.RequestUri, jsonRequest.Length);
                    
                    var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, ct);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // Handle HTTP status codes with retry logic
                    if (response.StatusCode == (HttpStatusCode)429)
                        throw new RateLimitException((int)response.StatusCode, responseContent);
                    
                    if (response.StatusCode >= HttpStatusCode.InternalServerError || 
                        response.StatusCode == HttpStatusCode.RequestTimeout || 
                        response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        var httpEx = new HttpRequestException($"API request failed with retryable status {(int)response.StatusCode}");
                        httpEx.Data["StatusCode"] = (int)response.StatusCode;
                        throw httpEx;
                    }
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var httpEx = new HttpRequestException($"API request failed with non-retryable status {(int)response.StatusCode}: {responseContent}");
                        httpEx.Data["StatusCode"] = (int)response.StatusCode;
                        throw httpEx;
                    }
                    
                    _logger.Debug("**HTTP_SUCCESS**: Status {StatusCode}, Response size: {Size} bytes", (int)response.StatusCode, responseContent.Length);
                    return responseContent;
                }
            }, context, cancellationToken);
        }

        private void CheckForApiError(JObject responseObj, string provider)
        {
            var error = responseObj["error"]?["message"]?.Value<string>();
            if (!string.IsNullOrEmpty(error))
            {
                _logger.Error("[{Provider}] API returned error: {ErrorMessage}", provider, error);
                throw new InvalidOperationException($"{provider} API error: {error}");
            }
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = 20
            };

            var client = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromSeconds(300) // 5 minute timeout
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return client;
        }

        private AsyncRetryPolicy CreateRetryPolicy()
        {
            Func<int, TimeSpan> calculateDelay = (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

            Action<Exception, TimeSpan, Context> logRetryAction = (exception, calculatedDelay, context) =>
            {
                var contextLogger = context.Contains("Logger") ? context["Logger"] as ILogger : _logger;
                string providerName = context.Contains("ProviderType") ? context["ProviderType"].ToString() : "OCRLlmClient";

                if (exception is RateLimitException rle)
                    contextLogger.Warning(rle, "[{Strategy}] OCR Retry needed: Rate Limit (HTTP {StatusCode}). Delay: {Delay}s", providerName, rle.StatusCode, calculatedDelay.TotalSeconds);
                else if (exception is TaskCanceledException tce && !tce.CancellationToken.IsCancellationRequested)
                    contextLogger.Warning(tce, "[{Strategy}] OCR Retry needed: Timeout. Delay: {Delay}s", providerName, calculatedDelay.TotalSeconds);
                else if (exception is HttpRequestException httpEx && httpEx.Data.Contains("StatusCode"))
                    contextLogger.Warning(httpEx, "[{Strategy}] OCR Retry needed: Server Error (HTTP {StatusCode}). Delay: {Delay}s", providerName, httpEx.Data["StatusCode"], calculatedDelay.TotalSeconds);
                else
                    contextLogger.Warning(exception, "[{Strategy}] OCR Retry needed: Transient Error ({ExceptionType}). Delay: {Delay}s", providerName, exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
            };

            return Policy.Handle<RateLimitException>()
                         .Or<HttpRequestException>(ex => ex.Data.Contains("StatusCode") &&
                             ((int)ex.Data["StatusCode"] >= 500 ||
                              (int)ex.Data["StatusCode"] == (int)HttpStatusCode.RequestTimeout ||
                              (int)ex.Data["StatusCode"] == (int)HttpStatusCode.ServiceUnavailable))
                         .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested)
                         .WaitAndRetryAsync(3, calculateDelay, logRetryAction);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger?.Information("🔄 **OCR_LLM_CLIENT_DISPOSE**: Disposing OCR LLM client resources");
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Rate limit exception for retry policy handling
    /// </summary>
    public class RateLimitException : Exception
    {
        public int StatusCode { get; }

        public RateLimitException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}