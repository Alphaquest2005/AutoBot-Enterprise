#nullable disable
using Serilog; // Added
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
        // Removed ILoggerFactory _serilogLoggerFactory

        static LlmApiClientFactory() // Static constructor to initialize shared resources
        {
            InitializeSharedResources();
        }

        private static void InitializeSharedResources() // Removed optional logger parameter
        {
            lock (SharedLock)
            {
                if (_sharedHttpClient == null)
                {
                    // Use default static logger for factory initialization if needed, or remove
                    // Log.Logger.Debug("Initializing shared HttpClient for LlmApiClientFactory."); // Removed factory logging
                    var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, MaxConnectionsPerServer = 20 };
                    _sharedHttpClient = new HttpClient(handler, disposeHandler: true) { Timeout = TimeSpan.FromSeconds(300) };
                    _sharedHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _sharedHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    _sharedHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                }

                if (_sharedRetryPolicy == null)
                {
                    // Use default static logger for factory initialization if needed, or remove
                    // Log.Logger.Debug("Initializing shared RetryPolicy for LlmApiClientFactory."); // Removed factory logging
                    // Define the retry policy logic here (copied from LlmApiClient's CreateRetryPolicy)
                    Func<int, TimeSpan> calculateDelay = (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    Action<Exception, TimeSpan, Context> logRetryAction = (exception, calculatedDelay, context) =>
                    {
                        // Use context to get logger if available, otherwise use a default static logger
                        var contextLogger = context.Contains("Logger") ? context["Logger"] as ILogger : Log.Logger.ForContext("SourceContext", nameof(LlmApiClientFactory)); // Corrected ForContext usage
                        string providerName = context.Contains("ProviderType") ? context["ProviderType"].ToString() : "UnknownProvider";

                        if (exception is RateLimitException rle) contextLogger.Warning(rle, "[{Strategy}] Retry needed: Rate Limit (HTTP {StatusCode}). Delay: {Delay}s", providerName, rle.StatusCode, calculatedDelay.TotalSeconds);
                        else if (exception is TaskCanceledException tce && !tce.CancellationToken.IsCancellationRequested) contextLogger.Warning(tce, "[{Strategy}] Retry needed: Timeout. Delay: {Delay}s", providerName, calculatedDelay.TotalSeconds);
                        else if (exception is HttpRequestException httpEx && httpEx.Data.Contains("StatusCode")) contextLogger.Warning(httpEx, "[{Strategy}] Retry needed: Server Error (HTTP {StatusCode}). Delay: {Delay}s", providerName, httpEx.Data["StatusCode"], calculatedDelay.TotalSeconds);
                        else contextLogger.Warning(exception, "[{Strategy}] Retry needed: Transient Error ({ExceptionType}). Delay: {Delay}s", providerName, exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
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
            ILogger log, // Takes ILogger as parameter
            HttpClient httpClient = null, // Allow injecting HttpClient
            AsyncRetryPolicy retryPolicy = null) // Allow injecting RetryPolicy
        {
            string methodName = nameof(CreateClient);
            log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
                methodName, new { Provider = provider });

            try
            {
                // Ensure shared resources are initialized (thread-safe due to static constructor)
                InitializeSharedResources();

                // Use the passed-in logger and enrich with context
                ILogger clientLogger = log.ForContext<LlmApiClient>().ForContext("Provider", provider.ToString());
                ILogger strategyLogger = log.ForContext("Provider", provider.ToString()); // Enrich strategy logger with provider

                string apiKey = GetApiKeyFromEnv(provider);
                HttpClient clientInstance = httpClient ?? _sharedHttpClient; // Use provided or shared
                AsyncRetryPolicy policyInstance = retryPolicy ?? _sharedRetryPolicy; // Use provided or shared

                // Create strategy with all dependencies
                ILLMProviderStrategy strategy = CreateStrategy(provider, apiKey, strategyLogger, clientInstance, policyInstance);

                // Create client with the configured strategy
                LlmApiClient createdClient = new LlmApiClient(strategy, clientLogger);

                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Created client for provider: {Provider}",
                    methodName, 0, provider); // Placeholder for duration
                return createdClient;
            }
            catch (Exception ex)
            {
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error creating client for provider {Provider}: {ErrorMessage}",
                    methodName, 0, provider, ex.Message); // Placeholder for duration
                throw; // Re-throw the exception after logging
            }
        }

        // Updated CreateStrategy to accept dependencies
        private static ILLMProviderStrategy CreateStrategy(LLMProvider provider, string apiKey, ILogger strategyLogger, HttpClient httpClient, AsyncRetryPolicy retryPolicy)
        {
            switch (provider)
            {
                case LLMProvider.DeepSeek:
                case LLMProvider.Gemini:
                    // Pass dependencies required by LlmStrategyBase constructor
                    return new DeepSeekStrategy(apiKey, strategyLogger, httpClient, retryPolicy);
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