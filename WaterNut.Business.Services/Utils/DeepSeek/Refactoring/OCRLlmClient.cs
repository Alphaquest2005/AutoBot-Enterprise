using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using WaterNut.Business.Services.Utils.LlmApi;

namespace WaterNut.Business.Services.Utils.LlmApi
{
    /// <summary>
    /// OCR-compatible LLM client with automatic DeepSeek -> Gemini fallback.
    /// Provides the same interface as the old DeepSeekInvoiceApi but with fallback capability.
    /// </summary>
    public class OCRLlmClient : IDisposable
    {
        private readonly ILogger _logger;
        private DeepSeekStrategy _primaryStrategy;
        private GeminiStrategy _fallbackStrategy;
        private bool _disposed = false;

        public OCRLlmClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeStrategies();
        }

        private void InitializeStrategies()
        {
            try
            {
                _logger.Information("🔄 **OCR_LLM_CLIENT_INIT**: Initializing DeepSeek (primary) and Gemini (fallback) strategies using factory pattern");

                // Create DeepSeek strategy (primary) - get API key directly from environment
                try
                {
                    string deepSeekApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
                    if (string.IsNullOrWhiteSpace(deepSeekApiKey))
                        throw new InvalidOperationException("DEEPSEEK_API_KEY environment variable not set");
                    
                    // Create strategy with null http client and retry policy - they'll be initialized by the strategy
                    _primaryStrategy = new DeepSeekStrategy(deepSeekApiKey, _logger, null, null);
                    _logger.Information("✅ **PRIMARY_STRATEGY_READY**: DeepSeek strategy initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "⚠️ **PRIMARY_STRATEGY_FAILED**: DeepSeek initialization failed, will rely on fallback only");
                }

                // Create Gemini strategy (fallback) - get API key directly from environment
                try
                {
                    string geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                    if (string.IsNullOrWhiteSpace(geminiApiKey))
                        throw new InvalidOperationException("GEMINI_API_KEY environment variable not set");
                    
                    // Create strategy with null http client and retry policy - they'll be initialized by the strategy  
                    _fallbackStrategy = new GeminiStrategy(geminiApiKey, _logger, null, null);
                    _logger.Information("✅ **FALLBACK_STRATEGY_READY**: Gemini strategy initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "❌ **FALLBACK_STRATEGY_FAILED**: Gemini initialization failed - no fallback available");
                }

                if (_primaryStrategy == null && _fallbackStrategy == null)
                {
                    throw new InvalidOperationException("Both DeepSeek and Gemini strategies failed to initialize");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **OCR_LLM_CLIENT_INIT_CRITICAL**: Failed to initialize any LLM strategies");
                throw;
            }
        }

        /// <summary>
        /// Gets response from LLM with automatic fallback.
        /// Matches the interface of the old DeepSeekInvoiceApi.GetResponseAsync.
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            _logger.Information("🎯 **OCR_LLM_REQUEST**: Starting prompt request with fallback capability - Length: {PromptLength}", prompt.Length);

            // Try primary strategy (DeepSeek)
            if (_primaryStrategy != null)
            {
                try
                {
                    _logger.Information("1️⃣ **TRYING_PRIMARY**: Attempting DeepSeek API call");
                    var primaryResponse = await _primaryStrategy.GetResponseAsync(prompt, temperature, maxTokens, cancellationToken);
                    
                    _logger.Information("✅ **PRIMARY_SUCCESS**: DeepSeek responded successfully - Length: {ResponseLength}", primaryResponse?.Length ?? 0);
                    return primaryResponse;
                }
                catch (Exception primaryEx)
                {
                    _logger.Warning(primaryEx, "⚠️ **PRIMARY_FAILED**: DeepSeek strategy failed: {ErrorMessage} - Attempting fallback", primaryEx.Message);
                }
            }
            else
            {
                _logger.Warning("⚠️ **PRIMARY_UNAVAILABLE**: DeepSeek strategy not available, going directly to fallback");
            }

            // Try fallback strategy (Gemini)
            if (_fallbackStrategy != null)
            {
                try
                {
                    _logger.Information("2️⃣ **TRYING_FALLBACK**: Attempting Gemini API call");
                    var fallbackResponse = await _fallbackStrategy.GetResponseAsync(prompt, temperature, maxTokens, cancellationToken);
                    
                    _logger.Information("✅ **FALLBACK_SUCCESS**: Gemini responded successfully - Length: {ResponseLength}", fallbackResponse?.Length ?? 0);
                    return fallbackResponse;
                }
                catch (Exception fallbackEx)
                {
                    _logger.Error(fallbackEx, "❌ **FALLBACK_FAILED**: Gemini strategy also failed: {ErrorMessage}", fallbackEx.Message);
                    throw new InvalidOperationException($"Both DeepSeek and Gemini strategies failed. Last error: {fallbackEx.Message}", fallbackEx);
                }
            }
            else
            {
                throw new InvalidOperationException("No fallback strategy available and primary strategy failed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger?.Information("🔄 **OCR_LLM_CLIENT_DISPOSE**: Disposing OCR LLM client resources");
                    
                    // Dispose strategies if they implement IDisposable
                    if (_primaryStrategy is IDisposable primaryDisposable)
                        primaryDisposable.Dispose();
                    
                    if (_fallbackStrategy is IDisposable fallbackDisposable)
                        fallbackDisposable.Dispose();
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
}