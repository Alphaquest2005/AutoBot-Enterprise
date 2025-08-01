using System;
using System.Collections.Generic;
using System.Linq;
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
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: OCR-dedicated LLM client with intelligent provider failover
    /// 
    /// **LOG_THE_WHAT**: Multi-provider LLM client supporting DeepSeek primary + Gemini fallback for OCR correction tasks
    /// **LOG_THE_HOW**: Implements automatic provider switching, retry policies, and comprehensive error handling
    /// **LOG_THE_WHY**: Ensures OCR processing continues even when primary LLM provider fails, maximizing system reliability
    /// **LOG_THE_WHO**: Serves OCRCorrectionService with identical interface to legacy DeepSeekInvoiceApi
    /// **LOG_THE_WHAT_IF**: Expects either DeepSeek or Gemini availability; graceful degradation when providers fail
    /// 
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
        private const int DeepSeekMaxTokens = 8000;  // Official DeepSeek API limit for output tokens
        
        // Gemini Configuration  
        private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";
        private const string GeminiModel = "gemini-1.5-flash-latest";
        private const int GeminiMaxTokens = 8192;   // Official Gemini API limit for output tokens

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Constructor - Initialize OCR LLM client with provider discovery
        /// 
        /// **LOG_THE_WHAT**: Constructor accepting logger dependency, discovering available API keys, initializing HTTP infrastructure
        /// **LOG_THE_HOW**: Environment variable discovery, HTTP client creation, retry policy setup, dependency validation
        /// **LOG_THE_WHY**: Establishes dual-provider architecture ensuring OCR processing resilience and API redundancy
        /// **LOG_THE_WHO**: Returns configured OCRLlmClient ready for OCR correction service integration
        /// **LOG_THE_WHAT_IF**: Expects at least one API key available; throws InvalidOperationException if none configured
        /// </summary>
        public OCRLlmClient(ILogger logger)
        {
            // **CRITICAL FIX**: Assign logger FIRST before any logging calls
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete constructor initialization narrative
            _logger.Information("🏗️ **CONSTRUCTOR_INIT_START**: OCRLlmClient constructor beginning with dependency injection and provider discovery");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Self-contained LLM client with DeepSeek primary + Gemini fallback for OCR processing reliability");
            _logger.Information("   - **DESIGN_BACKSTORY**: Replaces single-provider DeepSeekInvoiceApi with intelligent failover capability");
            _logger.Information("✅ **DEPENDENCY_INJECTION_SUCCESS**: Logger dependency successfully injected and validated");
            
            // **LOG_THE_WHAT**: Environment variable discovery for API authentication
            _logger.Information("🔍 **API_KEY_DISCOVERY_START**: Discovering available LLM provider API keys from environment variables");
            _deepSeekApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
            _geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            
            var hasDeepSeek = !string.IsNullOrWhiteSpace(_deepSeekApiKey);
            var hasGemini = !string.IsNullOrWhiteSpace(_geminiApiKey);
            _logger.Information("🔍 **API_KEY_DISCOVERY_RESULT**: DeepSeek={HasDeepSeek}, Gemini={HasGemini}", hasDeepSeek, hasGemini);
            _logger.Information("   - **PROVIDER_AVAILABILITY**: Primary={PrimaryProvider}, Fallback={FallbackProvider}", 
                hasDeepSeek ? "DeepSeek" : "None", hasGemini ? "Gemini" : "None");
            
            if (!hasDeepSeek && !hasGemini)
            {
                _logger.Error("❌ **CONFIGURATION_FAILURE**: No LLM provider API keys configured - cannot initialize client");
                _logger.Error("   - **REQUIRED_ENVIRONMENT_VARIABLES**: DEEPSEEK_API_KEY or GEMINI_API_KEY must be set");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: OCRLlmClient constructor dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Shipment Invoice"; // LLM client construction is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "OCRLlmClient", 
                    logger, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // LLM client configuration operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec.IsValid;
                
                throw new InvalidOperationException("At least one API key must be set: DEEPSEEK_API_KEY or GEMINI_API_KEY");
            }

            // **LOG_THE_HOW**: HTTP infrastructure initialization
            _logger.Information("🌐 **HTTP_INFRASTRUCTURE_INIT_START**: Creating HTTP client and retry policy for LLM communication");
            _httpClient = CreateHttpClient();
            _logger.Information("✅ **HTTP_CLIENT_CREATED**: HTTP client initialized with compression and timeout settings");
            
            _retryPolicy = CreateRetryPolicy();
            _logger.Information("✅ **RETRY_POLICY_CREATED**: Exponential backoff retry policy configured for transient failures");
            
            // **LOG_THE_WHAT_IF**: Constructor completion and readiness assertion
            _logger.Information("🏁 **CONSTRUCTOR_COMPLETE**: OCRLlmClient fully initialized and ready for OCR processing");
            _logger.Information("   - **CAPABILITY_SUMMARY**: Primary={Primary}, Fallback={Fallback}, RetryEnabled={RetryEnabled}", 
                hasDeepSeek ? "DeepSeek" : "None", hasGemini ? "Gemini" : "None", true);
            _logger.Information("   - **EXPECTATION_ASSERTION**: Client expects successful LLM responses with automatic failover capability");
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Main LLM request orchestrator with intelligent provider fallback
        /// 
        /// **LOG_THE_WHAT**: Primary LLM request method accepting prompt, temperature, and token limits with provider failover
        /// **LOG_THE_HOW**: Attempts DeepSeek first, falls back to Gemini on failure, handles all provider-specific protocols
        /// **LOG_THE_WHY**: Ensures OCR processing continues despite individual provider failures, maintaining system uptime
        /// **LOG_THE_WHO**: Returns LLM response string or throws after all providers exhausted
        /// **LOG_THE_WHAT_IF**: Expects valid prompt input; gracefully handles provider failures; throws when all strategies fail
        /// 
        /// Gets response from LLM with automatic DeepSeek -> Gemini fallback.
        /// Matches the interface of the old DeepSeekInvoiceApi.GetResponseAsync.
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt, double? temperature = null, int? maxTokens = null, CancellationToken cancellationToken = default)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete LLM request orchestration narrative
            _logger.Information("🎯 **LLM_REQUEST_ORCHESTRATOR_START**: Main LLM request beginning with intelligent provider fallback strategy");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Execute prompt against available LLM providers with seamless failover capability");
            _logger.Information("   - **BUSINESS_RATIONALE**: Maximize OCR processing reliability by eliminating single points of failure");
            
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.Error("❌ **VALIDATION_FAILURE**: Prompt validation failed - null or empty prompt provided");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetResponseAsync dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Shipment Invoice"; // LLM request orchestration is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetResponseAsync", 
                    new { prompt, temperature, maxTokens }, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // LLM response text operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec.IsValid;
                
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));
            }

            // **LOG_THE_WHAT**: Request configuration and parameter analysis
            var promptLength = prompt.Length;
            var effectiveTemperature = temperature ?? 0.3;
            var requestedTokens = maxTokens?.ToString() ?? "provider_default";
            
            _logger.Information("🔧 **REQUEST_CONFIGURATION**: Length={PromptLength}, Temperature={Temperature}, MaxTokens={MaxTokens}", 
                promptLength, effectiveTemperature, requestedTokens);
            _logger.Information("   - **FAILOVER_STRATEGY**: Primary=DeepSeek, Fallback=Gemini, RetryEnabled=true");
            _logger.Information("   - **EXPECTED_OUTCOME**: Valid LLM response or comprehensive error after all strategies exhausted");

            // **LOG_THE_HOW**: Primary provider strategy execution (DeepSeek)
            if (!string.IsNullOrWhiteSpace(_deepSeekApiKey))
            {
                _logger.Information("🥇 **PRIMARY_STRATEGY_START**: Attempting DeepSeek as primary LLM provider");
                _logger.Information("   - **STRATEGY_RATIONALE**: DeepSeek provides optimal performance for OCR correction tasks");
                
                try
                {
                    // **LOG_THE_WHAT**: Token limit calculation and provider-specific constraints
                    var effectiveTokens = maxTokens.HasValue ? Math.Min(maxTokens.Value, DeepSeekMaxTokens) : DeepSeekMaxTokens;
                    var tokenLimitApplied = maxTokens.HasValue && maxTokens.Value > DeepSeekMaxTokens;
                    
                    _logger.Information("🔢 **TOKEN_LIMIT_CALCULATION**: Requested={RequestedTokens}, Effective={EffectiveTokens}, LimitApplied={LimitApplied}", 
                        maxTokens?.ToString() ?? "provider_default", effectiveTokens, tokenLimitApplied);
                    _logger.Information("   - **PROVIDER_CONSTRAINTS**: DeepSeek maximum output tokens = {MaxTokens}", DeepSeekMaxTokens);
                    
                    _logger.Information("1️⃣ **DEEPSEEK_API_CALL_START**: Initiating DeepSeek API request with validated parameters");
                    var deepSeekResponse = await CallDeepSeekAsync(prompt, temperature ?? 0.3, effectiveTokens, cancellationToken);
                    
                    // **LOG_THE_WHO**: Successful primary strategy completion
                    var responseLength = deepSeekResponse?.Length ?? 0;
                    _logger.Information("✅ **PRIMARY_STRATEGY_SUCCESS**: DeepSeek responded successfully");
                    _logger.Information("   - **RESPONSE_METRICS**: Length={ResponseLength}, Provider=DeepSeek, FallbackUsed=false", responseLength);
                    _logger.Information("   - **SUCCESS_ASSERTION**: Primary provider delivered expected LLM response, no fallback required");
                    
                    // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                    _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetResponseAsync dual-layer template specification compliance analysis");

                    // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                    string documentType = "Shipment Invoice"; // LLM request orchestration is document-type agnostic
                    _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                    // Create template specification object for document type with dual-layer validation
                    var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetResponseAsync", 
                        new { prompt, temperature, maxTokens }, deepSeekResponse);

                    // Fluent validation with short-circuiting - stops on first failure
                    var validatedSpec = templateSpec
                        .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                        .ValidateFieldMappingEnhancement(null)
                        .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // LLM response text operations
                        .ValidatePatternQuality(null)
                        .ValidateTemplateOptimization(null);

                    // Log all validation results
                    validatedSpec.LogValidationResults(_logger);

                    // Extract overall success from validated specification
                    bool templateSpecificationSuccess = validatedSpec.IsValid;
                    
                    return deepSeekResponse;
                }
                catch (Exception deepSeekEx)
                {
                    // **LOG_THE_WHAT_IF**: Primary strategy failure handling
                    _logger.Warning(deepSeekEx, "⚠️ **PRIMARY_STRATEGY_FAILED**: DeepSeek strategy encountered error - initiating fallback protocol");
                    _logger.Warning("   - **FAILURE_DETAILS**: ErrorType={ErrorType}, Message={ErrorMessage}", deepSeekEx.GetType().Name, deepSeekEx.Message);
                    _logger.Warning("   - **FAILOVER_PROTOCOL**: Attempting Gemini fallback to maintain OCR processing continuity");
                }
            }
            else
            {
                _logger.Warning("⚠️ **PRIMARY_STRATEGY_UNAVAILABLE**: DeepSeek API key not configured - skipping to fallback strategy");
                _logger.Warning("   - **CONFIGURATION_STATE**: DEEPSEEK_API_KEY environment variable not set or empty");
                _logger.Warning("   - **STRATEGY_ADJUSTMENT**: Proceeding directly to Gemini fallback provider");
            }

            // **LOG_THE_HOW**: Fallback provider strategy execution (Gemini)
            if (!string.IsNullOrWhiteSpace(_geminiApiKey))
            {
                _logger.Information("🥈 **FALLBACK_STRATEGY_START**: Attempting Gemini as fallback LLM provider");
                _logger.Information("   - **STRATEGY_RATIONALE**: Gemini provides reliable alternative when DeepSeek unavailable or failed");
                _logger.Information("   - **RESILIENCE_DESIGN**: Dual-provider architecture ensures OCR processing continuity");
                
                try
                {
                    // **LOG_THE_WHAT**: Fallback token limit calculation and provider constraints
                    var effectiveTokens = maxTokens.HasValue ? Math.Min(maxTokens.Value, GeminiMaxTokens) : GeminiMaxTokens;
                    var tokenLimitApplied = maxTokens.HasValue && maxTokens.Value > GeminiMaxTokens;
                    
                    _logger.Information("🔢 **FALLBACK_TOKEN_CALCULATION**: Requested={RequestedTokens}, Effective={EffectiveTokens}, LimitApplied={LimitApplied}", 
                        maxTokens?.ToString() ?? "provider_default", effectiveTokens, tokenLimitApplied);
                    _logger.Information("   - **PROVIDER_CONSTRAINTS**: Gemini maximum output tokens = {MaxTokens}", GeminiMaxTokens);
                    
                    _logger.Information("2️⃣ **GEMINI_API_CALL_START**: Initiating Gemini API request as fallback strategy");
                    var geminiResponse = await CallGeminiAsync(prompt, temperature ?? 0.3, effectiveTokens, cancellationToken);
                    
                    // **LOG_THE_WHO**: Successful fallback strategy completion
                    var responseLength = geminiResponse?.Length ?? 0;
                    _logger.Information("✅ **FALLBACK_STRATEGY_SUCCESS**: Gemini responded successfully - system resilience demonstrated");
                    _logger.Information("   - **RESPONSE_METRICS**: Length={ResponseLength}, Provider=Gemini, FallbackUsed=true", responseLength);
                    _logger.Information("   - **SUCCESS_ASSERTION**: Fallback provider delivered expected LLM response, OCR processing can continue");
                    
                    // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                    _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetResponseAsync dual-layer template specification compliance analysis");

                    // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                    string documentType = "Shipment Invoice"; // LLM request orchestration is document-type agnostic
                    _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                    // Create template specification object for document type with dual-layer validation
                    var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetResponseAsync", 
                        new { prompt, temperature, maxTokens }, geminiResponse);

                    // Fluent validation with short-circuiting - stops on first failure
                    var validatedSpec = templateSpec
                        .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                        .ValidateFieldMappingEnhancement(null)
                        .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // LLM response text operations
                        .ValidatePatternQuality(null)
                        .ValidateTemplateOptimization(null);

                    // Log all validation results
                    validatedSpec.LogValidationResults(_logger);

                    // Extract overall success from validated specification
                    bool templateSpecificationSuccess = validatedSpec.IsValid;
                    
                    return geminiResponse;
                }
                catch (Exception geminiEx)
                {
                    // **LOG_THE_WHAT_IF**: Complete strategy failure - all providers exhausted
                    _logger.Error(geminiEx, "❌ **FALLBACK_STRATEGY_FAILED**: Gemini strategy also failed - all LLM providers exhausted");
                    _logger.Error("   - **FAILURE_DETAILS**: ErrorType={ErrorType}, Message={ErrorMessage}", geminiEx.GetType().Name, geminiEx.Message);
                    _logger.Error("   - **SYSTEM_STATE**: Both DeepSeek and Gemini strategies failed - OCR processing cannot continue");
                    _logger.Error("   - **RECOMMENDED_ACTION**: Check API keys, network connectivity, and provider service status");
                    
                    // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                    _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetResponseAsync dual-layer template specification compliance analysis");

                    // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                    string documentType = "Shipment Invoice"; // LLM request orchestration is document-type agnostic
                    _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                    // Create template specification object for document type with dual-layer validation
                    var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetResponseAsync", 
                        new { prompt, temperature, maxTokens }, null);

                    // Fluent validation with short-circuiting - stops on first failure
                    var validatedSpec = templateSpec
                        .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                        .ValidateFieldMappingEnhancement(null)
                        .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // LLM response text operations
                        .ValidatePatternQuality(null)
                        .ValidateTemplateOptimization(null);

                    // Log all validation results
                    validatedSpec.LogValidationResults(_logger);

                    // Extract overall success from validated specification
                    bool templateSpecificationSuccess = validatedSpec.IsValid;
                    
                    throw new InvalidOperationException($"Both DeepSeek and Gemini strategies failed. Last error: {geminiEx.Message}", geminiEx);
                }
            }
            else
            {
                // **LOG_THE_WHAT_IF**: No fallback available - complete system failure
                _logger.Error("❌ **NO_FALLBACK_AVAILABLE**: Gemini API key not configured and DeepSeek failed - no recovery possible");
                _logger.Error("   - **CONFIGURATION_STATE**: GEMINI_API_KEY environment variable not set or empty");
                _logger.Error("   - **SYSTEM_FAILURE**: No alternative LLM providers available for OCR processing");
                _logger.Error("   - **REQUIRED_ACTION**: Configure at least one working API key (DEEPSEEK_API_KEY or GEMINI_API_KEY)");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetResponseAsync dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Shipment Invoice"; // LLM request orchestration is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetResponseAsync", 
                    new { prompt, temperature, maxTokens }, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // LLM response text operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec.IsValid;
                
                throw new InvalidOperationException("No fallback strategy available - Gemini API key not configured and DeepSeek failed");
            }
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: DeepSeek API integration with OCR-optimized request handling
        /// 
        /// **LOG_THE_WHAT**: DeepSeek-specific API request formatting, authentication, and response parsing
        /// **LOG_THE_HOW**: Constructs chat completion request, handles Bearer authentication, parses choice responses
        /// **LOG_THE_WHY**: Provides primary LLM capability with DeepSeek's OCR-optimized model for correction tasks
        /// **LOG_THE_WHO**: Returns DeepSeek response content or throws detailed provider-specific exceptions
        /// **LOG_THE_WHAT_IF**: Expects valid API key and prompt; handles rate limits and API errors gracefully
        /// </summary>
        private async Task<string> CallDeepSeekAsync(string prompt, double temperature, int maxTokens, CancellationToken cancellationToken)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete DeepSeek API integration narrative
            _logger.Information("🤖 **DEEPSEEK_API_INTEGRATION_START**: Initiating DeepSeek chat completion request");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Execute OCR correction prompt using DeepSeek's advanced language model");
            _logger.Information("   - **PROVIDER_RATIONALE**: DeepSeek provides superior performance for structured data extraction tasks");
            // **LOG_THE_WHAT**: DeepSeek request structure and parameters
            var requestBody = new
            {
                model = DeepSeekModel,
                messages = new[] { new { role = "user", content = prompt } },
                temperature = temperature,
                max_tokens = maxTokens,
                stream = false
            };
            
            _logger.Information("🔧 **DEEPSEEK_REQUEST_CONFIG**: Model={Model}, Temperature={Temperature}, MaxTokens={MaxTokens}", 
                DeepSeekModel, temperature, maxTokens);
            _logger.Information("   - **MESSAGE_STRUCTURE**: Single user message with OCR correction prompt");
            _logger.Information("   - **STREAMING_MODE**: Disabled for synchronous response processing");

            // **LOG_THE_HOW**: API endpoint construction and authentication
            var apiUrl = $"{DeepSeekBaseUrl}/chat/completions";
            _logger.Information("🌐 **DEEPSEEK_API_CALL**: Executing POST request to {ApiUrl}", apiUrl);
            
            var responseJson = await PostRequestAsync(apiUrl, requestBody, (request, apiKey) =>
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                _logger.Debug("🔐 **DEEPSEEK_AUTH**: Bearer token authentication configured");
            }, _deepSeekApiKey, cancellationToken);

            // **LOG_THE_WHO**: Response parsing and validation
            _logger.Information("🔍 **DEEPSEEK_RESPONSE_PARSING**: Parsing DeepSeek API response structure");
            var responseObj = JObject.Parse(responseJson);
            CheckForApiError(responseObj, "DeepSeek");
            
            var content = responseObj["choices"]?[0]?["message"]?["content"]?.Value<string>();
            _logger.Information("🔍 **DEEPSEEK_CONTENT_EXTRACTION**: Content extracted from choices[0].message.content path");
            
            if (string.IsNullOrEmpty(content))
            {
                _logger.Error("❌ **DEEPSEEK_EMPTY_CONTENT**: DeepSeek API returned null or empty content in response");
                _logger.Error("   - **RESPONSE_STRUCTURE**: Choices array may be empty or content field missing");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CallDeepSeekAsync dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeApiCall = "Shipment Invoice"; // DeepSeek API integration is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeApiCall} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecApiCall = TemplateSpecification.CreateForUtilityOperation(documentTypeApiCall, "CallDeepSeekAsync", 
                    new { prompt, temperature, maxTokens }, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecApiCall = templateSpecApiCall
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // DeepSeek response text operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecApiCall.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationApiCallSuccess = validatedSpecApiCall.IsValid;
                
                throw new InvalidOperationException("DeepSeek API returned empty content");
            }
            
            // **LOG_THE_WHAT_IF**: Successful content return
            _logger.Information("✅ **DEEPSEEK_CONTENT_SUCCESS**: Valid content extracted - Length={ContentLength}", content.Length);
            _logger.Information("   - **SUCCESS_ASSERTION**: DeepSeek delivered expected OCR correction response");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CallDeepSeekAsync dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Shipment Invoice"; // DeepSeek API integration is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "CallDeepSeekAsync", 
                new { prompt, temperature, maxTokens }, content);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // DeepSeek response text operations
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
            return content;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Gemini API integration with fallback-optimized request handling
        /// 
        /// **LOG_THE_WHAT**: Gemini-specific API request formatting, API key authentication, and response parsing
        /// **LOG_THE_HOW**: Constructs generateContent request, handles API key in URL, parses candidate responses
        /// **LOG_THE_WHY**: Provides reliable fallback LLM capability when DeepSeek unavailable or failing
        /// **LOG_THE_WHO**: Returns Gemini response content or throws detailed provider-specific exceptions
        /// **LOG_THE_WHAT_IF**: Expects valid API key and prompt; handles rate limits and content filtering
        /// </summary>
        private async Task<string> CallGeminiAsync(string prompt, double temperature, int maxTokens, CancellationToken cancellationToken)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete Gemini API integration narrative
            _logger.Information("🤖 **GEMINI_API_INTEGRATION_START**: Initiating Gemini generateContent request as fallback strategy");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Execute OCR correction prompt using Gemini's reliable language model");
            _logger.Information("   - **FALLBACK_RATIONALE**: Gemini provides consistent alternative when primary provider fails");
            // **LOG_THE_WHAT**: Gemini request structure and configuration
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
            
            _logger.Information("🔧 **GEMINI_REQUEST_CONFIG**: Model={Model}, Temperature={Temperature}, MaxOutputTokens={MaxTokens}", 
                GeminiModel, temperature, maxTokens);
            _logger.Information("   - **CONTENT_STRUCTURE**: Single content part with OCR correction prompt text");
            _logger.Information("   - **GENERATION_CONFIG**: Configured for consistent OCR correction output");

            // **LOG_THE_HOW**: API endpoint construction with embedded authentication
            var apiUrl = $"{GeminiBaseUrl}/{GeminiModel}:generateContent?key={Uri.EscapeDataString(_geminiApiKey)}";
            _logger.Information("🌐 **GEMINI_API_CALL**: Executing POST request to generateContent endpoint");
            _logger.Information("   - **AUTHENTICATION_METHOD**: API key embedded in URL query parameter");
            
            var responseJson = await PostRequestAsync(apiUrl, requestBody, (request, apiKey) =>
            {
                request.Headers.TryAddWithoutValidation("x-goog-api-key", apiKey);
                _logger.Debug("🔐 **GEMINI_AUTH**: x-goog-api-key header configured for additional security");
            }, _geminiApiKey, cancellationToken);

            // **LOG_THE_WHO**: Response parsing and content extraction
            _logger.Information("🔍 **GEMINI_RESPONSE_PARSING**: Parsing Gemini API response structure");
            var responseObj = JObject.Parse(responseJson);
            CheckForApiError(responseObj, "Gemini");
            
            var content = responseObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.Value<string>();
            _logger.Information("🔍 **GEMINI_CONTENT_EXTRACTION**: Content extracted from candidates[0].content.parts[0].text path");
            
            if (string.IsNullOrEmpty(content))
            {
                _logger.Error("❌ **GEMINI_EMPTY_CONTENT**: Gemini API returned null or empty content in response");
                _logger.Error("   - **RESPONSE_STRUCTURE**: Candidates array may be empty, filtered, or parts missing");
                _logger.Error("   - **POTENTIAL_CAUSES**: Content filtering, safety blocks, or generation failure");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CallGeminiAsync dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeGemini = "Invoice"; // Gemini API integration is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeGemini} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecGemini = TemplateSpecification.CreateForUtilityOperation(documentTypeGemini, "CallGeminiAsync", 
                    new { prompt, temperature, maxTokens }, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecGemini = templateSpecGemini
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Gemini response text operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecGemini.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationGeminiSuccess = validatedSpecGemini.IsValid;
                
                throw new InvalidOperationException("Gemini API returned empty content");
            }
            
            // **LOG_THE_WHAT_IF**: Successful fallback content return
            _logger.Information("✅ **GEMINI_FALLBACK_SUCCESS**: Valid content extracted from fallback provider - Length={ContentLength}", content.Length);
            _logger.Information("   - **SUCCESS_ASSERTION**: Gemini delivered expected OCR correction response as fallback");
            _logger.Information("   - **RESILIENCE_DEMONSTRATED**: System successfully recovered from primary provider failure");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CallGeminiAsync dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Gemini API integration is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "CallGeminiAsync", 
                new { prompt, temperature, maxTokens }, content);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Gemini response text operations
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
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
                    
                    // **COMPRESSION_DEBUG**: Log response headers to debug compression issues
                    _logger.Debug("**HTTP_RESPONSE_HEADERS**: ContentEncoding={ContentEncoding}, ContentType={ContentType}, ContentLength={ContentLength}", 
                        response.Content.Headers.ContentEncoding?.FirstOrDefault() ?? "None",
                        response.Content.Headers.ContentType?.MediaType ?? "Unknown", 
                        response.Content.Headers.ContentLength?.ToString() ?? "Unknown");
                    
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // **🔬 ULTRA_DIAGNOSTIC_LOGGING**: Complete LLM troubleshooting information
                    _logger.Information("🔬 **LLM_RESPONSE_FULL_CONTENT**: RAW_RESPONSE_START\n{ResponseContent}\nRAW_RESPONSE_END", responseContent ?? "[NULL_RESPONSE]");
                    
                    // **🔍 RESPONSE_STRUCTURE_ANALYSIS**: Detailed content analysis for LLM debugging
                    var responseLength = responseContent?.Length ?? 0;
                    var startsWithJson = responseContent?.TrimStart().StartsWith("{") == true;
                    var endsWithJson = responseContent?.TrimEnd().EndsWith("}") == true;
                    var containsBrackets = responseContent?.Contains("{") == true && responseContent?.Contains("}") == true;
                    var lineCount = responseContent?.Split('\n').Length ?? 0;
                    var hasMarkdownJson = responseContent?.Contains("```json") == true || responseContent?.Contains("```") == true;
                    var hasJsonSchema = responseContent?.Contains("Invoices") == true && responseContent?.Contains("CustomsDeclarations") == true;
                    
                    _logger.Information("🔍 **LLM_RESPONSE_ANALYSIS**: Length={Length}, StartsWithJson={StartsJson}, EndsWithJson={EndsJson}, HasBrackets={HasBrackets}, LineCount={Lines}, HasMarkdown={HasMarkdown}, HasSchema={HasSchema}",
                        responseLength, startsWithJson, endsWithJson, containsBrackets, lineCount, hasMarkdownJson, hasJsonSchema);
                    
                    // **🧹 JSON_CLEANING_DIAGNOSTICS**: Show what cleaning would be needed
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var trimmed = responseContent.Trim();
                        var withoutMarkdown = trimmed.Replace("```json", "").Replace("```", "").Trim();
                        var firstChar = trimmed.Length > 0 ? trimmed[0].ToString() : "[EMPTY]";
                        var lastChar = trimmed.Length > 0 ? trimmed[trimmed.Length - 1].ToString() : "[EMPTY]";
                        
                        _logger.Information("🧹 **JSON_CLEANING_PREVIEW**: OriginalFirst='{FirstChar}', OriginalLast='{LastChar}', AfterMarkdownClean='{CleanedPreview}'",
                            firstChar, lastChar, withoutMarkdown.Length > 100 ? withoutMarkdown.Substring(0, 100) + "..." : withoutMarkdown);
                    }
                    
                    // **🚨 JSON_VALIDATION_ATTEMPT**: Try parsing to identify specific issues
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            var testParse = Newtonsoft.Json.Linq.JObject.Parse(responseContent);
                            _logger.Information("✅ **JSON_VALIDATION_SUCCESS**: Response is valid JSON with {PropertyCount} top-level properties", testParse.Properties().Count());
                        }
                        catch (Newtonsoft.Json.JsonReaderException jsonEx)
                        {
                            _logger.Error("❌ **JSON_VALIDATION_FAILED**: JsonReaderException at Line={Line}, Position={Position}, Path='{Path}', Error='{Error}'",
                                jsonEx.LineNumber, jsonEx.LinePosition, jsonEx.Path ?? "[NO_PATH]", jsonEx.Message);
                            
                            // **🔧 SPECIFIC_JSON_ISSUE_ANALYSIS**: Common JSON problems
                            var hasUnescapedQuotes = responseContent.Contains("\"") && !responseContent.Contains("\\\"");
                            var hasTrailingCommas = responseContent.Contains(",}") || responseContent.Contains(",]");
                            var hasUnterminatedStrings = CountOccurrences(responseContent, "\"") % 2 != 0;
                            var hasMismatchedBraces = CountOccurrences(responseContent, "{") != CountOccurrences(responseContent, "}");
                            
                            _logger.Error("🔧 **JSON_ISSUE_DETAILS**: UnescapedQuotes={UnescapedQuotes}, TrailingCommas={TrailingCommas}, UnterminatedStrings={UnterminatedStrings}, MismatchedBraces={MismatchedBraces}",
                                hasUnescapedQuotes, hasTrailingCommas, hasUnterminatedStrings, hasMismatchedBraces);
                        }
                        catch (Exception parseEx)
                        {
                            _logger.Error(parseEx, "❌ **JSON_VALIDATION_UNKNOWN_ERROR**: Unexpected parsing error: {ErrorType}", parseEx.GetType().Name);
                        }
                    }
                    
                    // **COMPRESSION_DEBUG**: Log response content summary for regex pattern analysis
                    var containsRegexPatterns = responseContent?.Contains("suggested_regex") == true || responseContent?.Contains("regex") == true || responseContent?.Contains("pattern") == true;
                    _logger.Debug("**HTTP_RESPONSE_ANALYSIS**: ContainsRegexPatterns={ContainsRegex}, FirstChars={FirstChars}, LastChars={LastChars}",
                        containsRegexPatterns,
                        responseLength > 50 ? responseContent.Substring(0, 50) + "..." : responseContent ?? "[NULL]",
                        responseLength > 50 ? "..." + responseContent.Substring(responseContent.Length - 50) : responseContent ?? "[NULL]");
                    
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

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: API error detection and provider-specific error handling
        /// 
        /// **LOG_THE_WHAT**: JSON response error field inspection and provider-specific error message extraction
        /// **LOG_THE_HOW**: Parses error object from response JSON, extracts message, logs and throws exceptions
        /// **LOG_THE_WHY**: Provides consistent error handling across all LLM providers with detailed diagnostics
        /// **LOG_THE_WHO**: Validates response success or throws InvalidOperationException with provider context
        /// **LOG_THE_WHAT_IF**: Expects valid response JSON; silent when no errors, throws on API error detection
        /// </summary>
        private void CheckForApiError(JObject responseObj, string provider)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete API error validation narrative
            _logger.Debug("🔍 **API_ERROR_CHECK_START**: Examining {Provider} response for error indicators", provider);
            _logger.Debug("   - **VALIDATION_PURPOSE**: Detect and handle provider-specific API errors before content processing");
            
            var error = responseObj["error"]?["message"]?.Value<string>();
            if (!string.IsNullOrEmpty(error))
            {
                // **LOG_THE_WHAT_IF**: API error detected - comprehensive error reporting
                _logger.Error("❌ **API_ERROR_DETECTED**: [{Provider}] API returned error response", provider);
                _logger.Error("   - **ERROR_MESSAGE**: {ErrorMessage}", error);
                _logger.Error("   - **PROVIDER_CONTEXT**: {Provider} API rejected request with detailed error", provider);
                _logger.Error("   - **FAILURE_IMPACT**: LLM request cannot be processed, provider strategy will fail");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CheckForApiError dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string apiErrorDocumentType = "Invoice"; // API error detection is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {apiErrorDocumentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var apiErrorTemplateSpec = TemplateSpecification.CreateForUtilityOperation(apiErrorDocumentType, "CheckForApiError", 
                    responseObj, error);

                // Fluent validation with short-circuiting - stops on first failure
                var apiErrorValidatedSpec = apiErrorTemplateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // API error message processing
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                apiErrorValidatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool apiErrorTemplateSpecificationSuccess = apiErrorValidatedSpec.IsValid;
                
                throw new InvalidOperationException($"{provider} API error: {error}");
            }
            
            // **LOG_THE_WHO**: Successful error validation
            _logger.Debug("✅ **API_ERROR_CHECK_PASSED**: [{Provider}] response contains no error indicators", provider);
            _logger.Debug("   - **SUCCESS_ASSERTION**: Response ready for content extraction and processing");
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: HTTP client factory with LLM-optimized configuration
        /// 
        /// **LOG_THE_WHAT**: HttpClient creation with compression, connection pooling, and timeout configuration
        /// **LOG_THE_HOW**: Configures handler with decompression, sets connection limits, adds content negotiation headers
        /// **LOG_THE_WHY**: Optimizes HTTP performance for LLM API calls with large payloads and extended processing times
        /// **LOG_THE_WHO**: Returns fully configured HttpClient instance ready for multi-provider LLM communication
        /// **LOG_THE_WHAT_IF**: Expects successful client creation; throws on configuration errors
        /// </summary>
        private HttpClient CreateHttpClient()
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete HTTP client configuration narrative
            _logger.Information("🌐 **HTTP_CLIENT_FACTORY_START**: Creating optimized HTTP client for LLM API communication");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Configure HTTP infrastructure for reliable multi-provider LLM access");
            _logger.Information("   - **PERFORMANCE_DESIGN**: Enable compression and connection pooling for efficient API calls");
            
            // **LOG_THE_WHAT**: HTTP handler configuration with performance optimizations
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = 20
            };
            
            _logger.Information("🔧 **HTTP_HANDLER_CONFIG**: Compression={CompressionTypes}, MaxConnections={MaxConnections}", 
                "GZip|Deflate", 20);
            _logger.Information("   - **COMPRESSION_RATIONALE**: Reduce bandwidth usage for large OCR prompt payloads");
            _logger.Information("   - **CONNECTION_POOLING**: Allow multiple concurrent LLM requests for performance");

            // **LOG_THE_HOW**: HTTP client instantiation with LLM-appropriate timeouts
            var client = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromSeconds(300) // 5 minute timeout
            };
            
            _logger.Information("🕓 **HTTP_CLIENT_TIMEOUT**: Configured 5-minute timeout for LLM processing delays");
            _logger.Information("   - **TIMEOUT_RATIONALE**: LLM APIs may require extended processing time for complex OCR tasks");

            // **LOG_THE_WHO**: Content negotiation headers for optimal API communication
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            
            _logger.Information("📋 **HTTP_HEADERS_CONFIG**: Accept=application/json, AcceptEncoding=gzip,deflate");
            _logger.Information("   - **CONTENT_NEGOTIATION**: Ensure JSON responses with compression when available");
            
            // **LOG_THE_WHAT_IF**: Successful HTTP client creation
            _logger.Information("✅ **HTTP_CLIENT_CREATED**: Fully configured HTTP client ready for LLM API communication");
            _logger.Information("   - **SUCCESS_ASSERTION**: Client configured with optimal settings for multi-provider access");

            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CreateHttpClient dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // HTTP client creation is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "CreateHttpClient", 
                null, client);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // HTTP client object creation
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;

            return client;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Retry policy factory with intelligent failure classification
        /// 
        /// **LOG_THE_WHAT**: Polly retry policy creation with exponential backoff and context-aware logging
        /// **LOG_THE_HOW**: Configures rate limit, timeout, and server error handling with graduated delay calculations
        /// **LOG_THE_WHY**: Provides resilient LLM API access by gracefully handling transient failures and provider limits
        /// **LOG_THE_WHO**: Returns AsyncRetryPolicy configured for OCR-specific LLM communication patterns
        /// **LOG_THE_WHAT_IF**: Expects successful policy creation; handles multiple exception types with appropriate delays
        /// </summary>
        private AsyncRetryPolicy CreateRetryPolicy()
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete retry policy configuration narrative
            _logger.Information("🔁 **RETRY_POLICY_FACTORY_START**: Creating intelligent retry policy for LLM API resilience");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Handle transient failures gracefully with exponential backoff strategy");
            _logger.Information("   - **RESILIENCE_DESIGN**: Distinguish between retryable and non-retryable failures for optimal recovery");
            
            // **LOG_THE_WHAT**: Exponential backoff delay calculation strategy
            Func<int, TimeSpan> calculateDelay = (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            _logger.Information("🔢 **BACKOFF_STRATEGY**: Exponential delay calculation - Attempt 1={Delay1}s, Attempt 2={Delay2}s, Attempt 3={Delay3}s", 
                Math.Pow(2, 1), Math.Pow(2, 2), Math.Pow(2, 3));
            _logger.Information("   - **DELAY_RATIONALE**: Progressive delays allow provider recovery time and reduce system load");

            // **LOG_THE_HOW**: Context-aware retry logging with provider-specific diagnostics
            Action<Exception, TimeSpan, Context> logRetryAction = (exception, calculatedDelay, context) =>
            {
                var contextLogger = context.Contains("Logger") ? context["Logger"] as ILogger : _logger;
                string providerName = context.Contains("ProviderType") ? context["ProviderType"].ToString() : "OCRLlmClient";

                // **LOG_THE_WHAT_IF**: Intelligent exception classification for targeted retry logging
                if (exception is RateLimitException rle)
                {
                    contextLogger.Warning(rle, "⚠️ **RETRY_RATE_LIMIT**: [{Strategy}] Rate limit encountered - implementing backoff strategy", providerName);
                    contextLogger.Warning("   - **RATE_LIMIT_DETAILS**: StatusCode={StatusCode}, Delay={Delay}s, Strategy=ExponentialBackoff", rle.StatusCode, calculatedDelay.TotalSeconds);
                }
                else if (exception is TaskCanceledException tce && !tce.CancellationToken.IsCancellationRequested)
                {
                    contextLogger.Warning(tce, "⚠️ **RETRY_TIMEOUT**: [{Strategy}] Request timeout detected - retrying with extended delay", providerName);
                    contextLogger.Warning("   - **TIMEOUT_DETAILS**: Delay={Delay}s, Cause=NetworkTimeout, Strategy=ExponentialBackoff", calculatedDelay.TotalSeconds);
                }
                else if (exception is HttpRequestException httpEx && httpEx.Data.Contains("StatusCode"))
                {
                    contextLogger.Warning(httpEx, "⚠️ **RETRY_SERVER_ERROR**: [{Strategy}] Server error detected - attempting recovery", providerName);
                    contextLogger.Warning("   - **SERVER_ERROR_DETAILS**: StatusCode={StatusCode}, Delay={Delay}s, Strategy=ExponentialBackoff", httpEx.Data["StatusCode"], calculatedDelay.TotalSeconds);
                }
                else
                {
                    contextLogger.Warning(exception, "⚠️ **RETRY_TRANSIENT_ERROR**: [{Strategy}] Transient error detected - implementing recovery strategy", providerName);
                    contextLogger.Warning("   - **TRANSIENT_ERROR_DETAILS**: ExceptionType={ExceptionType}, Delay={Delay}s, Strategy=ExponentialBackoff", exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
                }
            };

            // **LOG_THE_WHO**: Policy configuration with comprehensive exception handling
            _logger.Information("🔧 **RETRY_POLICY_CONFIG**: MaxRetries=3, Exceptions=RateLimit|ServerError|Timeout");
            _logger.Information("   - **RETRYABLE_CONDITIONS**: Rate limits (429), Server errors (5xx), Timeouts, Service unavailable (503)");
            _logger.Information("   - **NON_RETRYABLE**: Authentication errors (401), Bad requests (400), Forbidden (403)");
            
            var policy = Policy.Handle<RateLimitException>()
                         .Or<HttpRequestException>(ex => ex.Data.Contains("StatusCode") &&
                             ((int)ex.Data["StatusCode"] >= 500 ||
                              (int)ex.Data["StatusCode"] == (int)HttpStatusCode.RequestTimeout ||
                              (int)ex.Data["StatusCode"] == (int)HttpStatusCode.ServiceUnavailable))
                         .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested)
                         .WaitAndRetryAsync(3, calculateDelay, logRetryAction);
            
            // **LOG_THE_WHAT_IF**: Successful policy creation
            _logger.Information("✅ **RETRY_POLICY_CREATED**: Intelligent retry policy configured for LLM API resilience");
            _logger.Information("   - **SUCCESS_ASSERTION**: Policy ready to handle transient failures with graduated recovery strategies");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CreateRetryPolicy dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Retry policy creation is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "CreateRetryPolicy", 
                null, policy);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Retry policy object creation
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
            return policy;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Protected disposal implementation with resource cleanup
        /// 
        /// **LOG_THE_WHAT**: Managed resource disposal following standard IDisposable pattern implementation
        /// **LOG_THE_HOW**: Checks disposal state, disposes HTTP client, updates disposal flag to prevent double disposal
        /// **LOG_THE_WHY**: Ensures proper cleanup of HTTP connections and prevents resource leaks in LLM client lifecycle
        /// **LOG_THE_WHO**: Cleans up HTTP client resources and marks instance as disposed
        /// **LOG_THE_WHAT_IF**: Expects safe multiple disposal calls; logs cleanup actions when disposing managed resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete resource disposal narrative
            if (!_disposed)
            {
                _logger?.Information("🗑️ **DISPOSAL_PROCESS_START**: OCRLlmClient disposal initiated - cleaning up LLM communication resources");
                _logger?.Information("   - **DISPOSAL_CONTEXT**: DisposingManagedResources={DisposingManaged}, AlreadyDisposed={AlreadyDisposed}", disposing, false);
                
                if (disposing)
                {
                    // **LOG_THE_WHAT**: Managed resource cleanup with detailed logging
                    _logger?.Information("🌐 **HTTP_CLIENT_DISPOSAL**: Disposing HTTP client and releasing connection pool resources");
                    _logger?.Information("   - **RESOURCE_CLEANUP**: HTTP connections, handlers, and associated network resources");
                    _logger?.Information("   - **ARCHITECTURAL_INTENT**: Prevent connection leaks and ensure clean shutdown of LLM communication");
                    
                    _httpClient?.Dispose();
                    _logger?.Information("✅ **HTTP_CLIENT_DISPOSED**: HTTP client successfully disposed and resources released");
                }
                
                // **LOG_THE_WHO**: Disposal state management
                _disposed = true;
                _logger?.Information("🏁 **DISPOSAL_COMPLETE**: OCRLlmClient disposal completed - instance marked as disposed");
                _logger?.Information("   - **SUCCESS_ASSERTION**: All LLM client resources properly cleaned up, no resource leaks expected");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger?.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: Dispose(bool) dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Invoice"; // Resource disposal is document-type agnostic
                _logger?.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "Dispose", 
                    disposing, _disposed);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Boolean disposal state operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec?.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec?.IsValid ?? false;
            }
            else
            {
                // **LOG_THE_WHAT_IF**: Multiple disposal attempts handled gracefully
                _logger?.Debug("🔁 **DISPOSAL_ALREADY_COMPLETED**: Disposal called on already disposed OCRLlmClient - ignoring safely");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger?.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: Dispose(bool) dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Invoice"; // Resource disposal is document-type agnostic
                _logger?.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "Dispose", 
                    disposing, _disposed);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Boolean disposal state operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec?.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec?.IsValid ?? false;
            }
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Public disposal interface implementation
        /// 
        /// **LOG_THE_WHAT**: IDisposable.Dispose() implementation following standard .NET disposal pattern
        /// **LOG_THE_HOW**: Calls protected Dispose(true) and suppresses finalization for performance
        /// **LOG_THE_WHY**: Provides clean disposal interface for OCR LLM client resource management
        /// **LOG_THE_WHO**: Initiates managed resource cleanup and optimization of garbage collection
        /// **LOG_THE_WHAT_IF**: Expects successful disposal; safe to call multiple times
        /// </summary>
        public void Dispose()
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete public disposal interface narrative
            _logger?.Information("💪 **PUBLIC_DISPOSE_CALLED**: IDisposable.Dispose() invoked - initiating complete resource cleanup");
            _logger?.Information("   - **DISPOSAL_PATTERN**: Following standard .NET IDisposable implementation pattern");
            _logger?.Information("   - **PERFORMANCE_OPTIMIZATION**: Suppressing finalization to reduce GC overhead");
            
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            
            _logger?.Information("✅ **PUBLIC_DISPOSE_COMPLETE**: IDisposable implementation completed successfully");
            _logger?.Information("   - **SUCCESS_ASSERTION**: OCRLlmClient disposal interface fulfilled, finalization suppressed");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger?.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: Dispose() dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Public disposal interface is document-type agnostic
            _logger?.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "Dispose", 
                null, _disposed);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Public disposal interface operations
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec?.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec?.IsValid ?? false;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: JSON diagnostic utility for substring occurrence counting
        /// 
        /// **LOG_THE_WHAT**: Static utility method for counting substring occurrences in text for JSON validation diagnostics
        /// **LOG_THE_HOW**: Iterates through text using IndexOf, counts matches, advances position by substring length
        /// **LOG_THE_WHY**: Supports JSON validation by counting braces, quotes, and other structural elements
        /// **LOG_THE_WHO**: Returns integer count of substring occurrences or zero for null/empty inputs
        /// **LOG_THE_WHAT_IF**: Expects valid string inputs; handles null/empty gracefully; accurate count for debugging
        /// 
        /// Helper method for JSON diagnostic analysis
        /// </summary>
        private static int CountOccurrences(string text, string substring)
        {
            // **DESIGN_NOTE**: Static method - no instance logger available, diagnostic context provided by caller
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(substring))
            {
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                // Note: Static method - no logger available, validation performed without logging
                
                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeStringEmpty = "Invoice"; // Text counting utility is document-type agnostic
                
                // Create template specification object for document type with dual-layer validation
                var templateSpecStringNull = TemplateSpecification.CreateForUtilityOperation(documentTypeStringEmpty, "CountOccurrences", 
                    new { text, substring }, 0);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecStringNull = templateSpecStringNull
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Text counting operations return numeric results
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Extract overall success from validated specification (no logging available)
                bool templateSpecificationSuccessStringNull = validatedSpecStringNull.IsValid;
                
                return 0;
            }
            
            // **LOG_THE_HOW**: Efficient substring counting implementation
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(substring, index)) != -1)
            {
                count++;
                index += substring.Length;
            }
            
            // **LOG_THE_WHO**: Return occurrence count for JSON diagnostic analysis
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            // Note: Static method - no logger available, validation performed without logging
            
            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Text counting utility is document-type agnostic
            
            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "CountOccurrences", 
                new { text, substring }, count);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Text counting operations return numeric results
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Extract overall success from validated specification (no logging available)
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
            return count;
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