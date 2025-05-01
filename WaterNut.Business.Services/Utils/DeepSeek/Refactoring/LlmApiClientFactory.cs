#nullable disable
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly; // Added
using Polly.Retry; // Added
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WaterNut.Business.Services.Utils.LlmApi
{
    public static class LlmApiClientFactory
    {
        private static HttpClient _sharedHttpClient; // Made non-nullable, ensure created
        private static AsyncRetryPolicy _sharedRetryPolicy; // Share retry policy
        private static readonly object SharedLock = new object();

        static LlmApiClientFactory() // Static constructor to initialize shared resources
        {
            InitializeSharedResources();
        }

        private static void InitializeSharedResources(ILogger logger = null) // Optional logger
        {
            lock (SharedLock)
            {
                if (_sharedHttpClient == null)
                {
                    logger?.LogDebug("Initializing shared HttpClient for LlmApiClientFactory.");
                    var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, MaxConnectionsPerServer = 20 };
                    _sharedHttpClient = new HttpClient(handler, disposeHandler: true) { Timeout = TimeSpan.FromSeconds(300) };
                    _sharedHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _sharedHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    _sharedHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                }

                if (_sharedRetryPolicy == null)
                {
                    logger?.LogDebug("Initializing shared RetryPolicy for LlmApiClientFactory.");
                    // Define the retry policy logic here (copied from LlmApiClient's CreateRetryPolicy)
                    Func<int, TimeSpan> calculateDelay = (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    Action<Exception, TimeSpan, Context> logRetryAction = (exception, calculatedDelay, context) =>
                    {
                        // Use context to get logger if available, otherwise NullLogger
                        var contextLogger = context.Contains("Logger") ? context["Logger"] as ILogger : NullLogger.Instance;
                        string providerName = context.Contains("ProviderType") ? context["ProviderType"].ToString() : "UnknownProvider";

                        if (exception is RateLimitException rle) contextLogger.LogWarning(rle, "[{Strategy}] Retry needed: Rate Limit (HTTP {StatusCode}). Delay: {Delay}s", providerName, rle.StatusCode, calculatedDelay.TotalSeconds);
                        else if (exception is TaskCanceledException tce && !tce.CancellationToken.IsCancellationRequested) contextLogger.LogWarning(tce, "[{Strategy}] Retry needed: Timeout. Delay: {Delay}s", providerName, calculatedDelay.TotalSeconds);
                        else if (exception is HttpRequestException httpEx && httpEx.Data.Contains("StatusCode")) contextLogger.LogWarning(httpEx, "[{Strategy}] Retry needed: Server Error (HTTP {StatusCode}). Delay: {Delay}s", providerName, httpEx.Data["StatusCode"], calculatedDelay.TotalSeconds);
                        else contextLogger.LogWarning(exception, "[{Strategy}] Retry needed: Transient Error ({ExceptionType}). Delay: {Delay}s", providerName, exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
                    };

                    _sharedRetryPolicy = Policy.Handle<RateLimitException>()
                                 .Or<HttpRequestException>(ex => ex.Data.Contains("StatusCode") && ((int)ex.Data["StatusCode"] >= 500 || (int)ex.Data["StatusCode"] == (int)HttpStatusCode.RequestTimeout || (int)ex.Data["StatusCode"] == (int)HttpStatusCode.ServiceUnavailable))
                                 .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested)
                                 .WaitAndRetryAsync(3, calculateDelay, logRetryAction);
                }
            }
        }


        public static LlmApiClient CreateClient(
            LLMProvider provider,
            ILoggerFactory loggerFactory = null,
            HttpClient httpClient = null, // Allow injecting HttpClient
            AsyncRetryPolicy retryPolicy = null) // Allow injecting RetryPolicy
        {
            // Ensure shared resources are initialized (thread-safe due to static constructor)
            // Pass logger factory's logger to initialization if available first time
            InitializeSharedResources(loggerFactory?.CreateLogger("LlmApiClientFactory"));

            ILogger<LlmApiClient> clientLogger = loggerFactory?.CreateLogger<LlmApiClient>() ?? NullLogger<LlmApiClient>.Instance;
            ILogger strategyLogger = loggerFactory?.CreateLogger(GetStrategyLoggerName(provider)) ?? NullLogger.Instance;

            string apiKey = GetApiKeyFromEnv(provider);
            HttpClient clientInstance = httpClient ?? _sharedHttpClient; // Use provided or shared
            AsyncRetryPolicy policyInstance = retryPolicy ?? _sharedRetryPolicy; // Use provided or shared

            // Create strategy with all dependencies
            ILLMProviderStrategy strategy = CreateStrategy(provider, apiKey, strategyLogger, clientInstance, policyInstance);

            // Create client with the configured strategy
            return new LlmApiClient(strategy, clientLogger); // Client only needs strategy and logger now
        }

        // Updated CreateStrategy to accept dependencies
        private static ILLMProviderStrategy CreateStrategy(LLMProvider provider, string apiKey, ILogger strategyLogger, HttpClient httpClient, AsyncRetryPolicy retryPolicy)
        {
            switch (provider)
            {
                case LLMProvider.DeepSeek:
                    // Pass dependencies required by LlmStrategyBase constructor
                    return new DeepSeekStrategy(apiKey, strategyLogger, httpClient, retryPolicy);
                case LLMProvider.Gemini:
                    // Pass dependencies required by LlmStrategyBase constructor
                    return new GeminiStrategy(apiKey, strategyLogger, httpClient, retryPolicy);
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), $"Unsupported LLM provider: {provider}");
            }
        }

        // GetApiKeyFromEnv remains the same
        private static string GetApiKeyFromEnv(LLMProvider provider)
        {
            string key = null; string envVarName = "";
            switch (provider) { case LLMProvider.DeepSeek: envVarName = "DEEPSEEK_API_KEY"; break; case LLMProvider.Gemini: envVarName = "GEMINI_API_KEY"; break; default: throw new ArgumentOutOfRangeException(nameof(provider)); }
            key = Environment.GetEnvironmentVariable(envVarName);
            if (string.IsNullOrWhiteSpace(key)) { throw new InvalidOperationException($"Required API key env var '{envVarName}' for provider '{provider}' not set."); }
            return key;
        }

        // GetStrategyLoggerName remains the same
        private static string GetStrategyLoggerName(LLMProvider provider) => $"WaterNut.Business.Services.Utils.LlmApi.{provider}Strategy";

        // DisposeSharedHttpClient remains the same (optional)
        public static void DisposeSharedHttpClientAndPolicy() { lock (SharedLock) { _sharedHttpClient?.Dispose(); _sharedHttpClient = null; _sharedRetryPolicy = null; } }
    }
}