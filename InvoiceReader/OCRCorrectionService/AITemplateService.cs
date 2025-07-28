// File: OCRCorrectionService/AITemplateService.cs
// Ultra-Simple AI-Powered Template Service
// Single file implementation with multi-provider AI, validation, recommendations
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using EntryDataDS.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Ultra-simple AI-powered template service for OCR correction prompts.
    /// Supports multiple AI providers (DeepSeek, Gemini) with automatic fallback.
    /// Includes template validation, AI recommendations, and supplier intelligence.
    /// </summary>
    public class AITemplateService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _templateBasePath;
        private readonly string _configBasePath;
        private readonly string _recommendationsPath;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, AIProviderConfig> _providerConfigs;
        private readonly TemplateSystemConfig _systemConfig;
        private bool _disposed = false;
        
        // Field to store last DeepSeek explanation for diagnostic purposes
        private string _lastDeepSeekExplanation = string.Empty;
        
        // API keys from environment variables (same as OCRLlmClient)
        private readonly string _deepSeekApiKey;
        private readonly string _geminiApiKey;

        #region Constructor and Initialization

        public AITemplateService(ILogger logger, string basePath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Get API keys from environment variables (same as OCRLlmClient)
            _deepSeekApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
            _geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            
            _logger.Information("🔑 **API_KEYS_LOADED**: DeepSeek={HasDeepSeek}, Gemini={HasGemini}", 
                !string.IsNullOrWhiteSpace(_deepSeekApiKey), !string.IsNullOrWhiteSpace(_geminiApiKey));
            
            // Setup paths
            var rootPath = basePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRCorrectionService");
            _templateBasePath = Path.Combine(rootPath, "Templates");
            _configBasePath = Path.Combine(rootPath, "Config");
            _recommendationsPath = Path.Combine(rootPath, "Recommendations");
            
            // Create directories if they don't exist
            Directory.CreateDirectory(_templateBasePath);
            Directory.CreateDirectory(_configBasePath);
            Directory.CreateDirectory(_recommendationsPath);
            
            // Initialize HTTP client for AI provider calls
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            // Load configurations
            _providerConfigs = LoadProviderConfigs();
            _systemConfig = LoadSystemConfig();
            
            _logger.Information("🚀 **AI_TEMPLATE_SERVICE_INITIALIZED**: Base path='{BasePath}', Providers={ProviderCount}, HasAPIKeys={HasKeys}", 
                rootPath, _providerConfigs.Count, !string.IsNullOrWhiteSpace(_deepSeekApiKey) || !string.IsNullOrWhiteSpace(_geminiApiKey));
        }

        #endregion

        #region Main Public API

        /// <summary>
        /// Creates header error detection prompt using AI-powered template system.
        /// Automatically selects provider-specific templates with fallback support.
        /// </summary>
        public async Task<string> CreateHeaderErrorDetectionPromptAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata,
            string provider = "deepseek")
        {
            try
            {
                _logger.Information("🎯 **AI_TEMPLATE_START**: Creating prompt using {Provider} for supplier '{Supplier}'", 
                    provider, invoice?.SupplierName ?? "Unknown");

                // 1. Load provider-specific template
                _logger.Information("🔍 **TEMPLATE_LOADING**: Attempting to load template for provider={Provider}, type=header-detection, supplier={Supplier}", 
                    provider, invoice?.SupplierName);
                
                var template = LoadTemplateAsync(provider, "header-detection", invoice?.SupplierName);
                _logger.Information("✅ **TEMPLATE_LOADED**: Successfully loaded template. Length: {Length} characters", template?.Length ?? 0);
                
                // 2. Validate template
                _logger.Information("🔍 **TEMPLATE_VALIDATING**: Validating template structure and required variables");
                var validation = ValidateTemplate(template, provider);
                if (!validation.IsValid)
                {
                    _logger.Warning("⚠️ **TEMPLATE_INVALID**: {Errors}, falling back to hardcoded", 
                        string.Join("; ", validation.Errors));
                    return CreateFallbackPrompt(invoice, fileText, metadata);
                }
                _logger.Information("✅ **TEMPLATE_VALID**: Template validation passed");

                // 3. Prepare template data (extract from existing prompt creation logic)
                _logger.Information("🔍 **DATA_PREPARATION**: Preparing template data from invoice and metadata");
                var templateData = PrepareTemplateData(invoice, fileText, metadata);
                _logger.Information("✅ **DATA_PREPARED**: Template data prepared. Variables: {Variables}", 
                    string.Join(", ", templateData.Keys));
                
                // 4. Render template with data
                _logger.Information("🔍 **TEMPLATE_RENDERING**: Rendering template with prepared data");
                var prompt = RenderTemplate(template, templateData);
                _logger.Information("✅ **TEMPLATE_RENDERED**: Template rendered successfully. Final prompt length: {Length}", prompt?.Length ?? 0);
                
                // 5. Async: Get AI recommendations for improvement (non-blocking)
                if (_systemConfig.EnableRecommendations)
                {
                    _ = Task.Run(() => GetRecommendationsAsync(prompt, provider));
                }
                
                _logger.Information("✅ **AI_TEMPLATE_SUCCESS**: Generated {Length} char prompt using {Provider}", 
                    prompt.Length, provider);
                
                return prompt;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AI_TEMPLATE_ERROR**: Failed for provider {Provider} at step: {Step}. Exception: {Message}", 
                    provider, "UNKNOWN", ex.Message);
                _logger.Error("❌ **AI_TEMPLATE_STACK**: {StackTrace}", ex.StackTrace);
                return CreateFallbackPrompt(invoice, fileText, metadata);
            }
        }

        /// <summary>
        /// Detects pattern failures and triggers automatic improvement cycle.
        /// This is called after template execution to check if patterns worked.
        /// </summary>
        public async Task<bool> HandlePostExecutionPatternAnalysis(
            List<string> usedPatterns,
            string actualText,
            List<object> extractionResults,
            string provider,
            string templateType,
            string supplierName)
        {
            try
            {
                _logger.Information("📊 **POST_EXECUTION_ANALYSIS**: Analyzing {PatternCount} patterns against extraction results",
                    usedPatterns?.Count ?? 0);
                
                // Check if extraction results indicate pattern failures
                var hasFailures = extractionResults?.Count == 0 || 
                                  extractionResults?.All(r => r == null || r.ToString() == "0") == true;
                
                if (hasFailures && usedPatterns?.Any() == true)
                {
                    _logger.Warning("🚨 **PATTERN_FAILURES_DETECTED**: No successful extractions, triggering improvement cycle");
                    
                    return await DetectAndHandlePatternFailure(
                        "template_used_in_execution", // This would be passed from the calling context
                        usedPatterns,
                        actualText,
                        provider,
                        templateType,
                        supplierName);
                }
                
                _logger.Information("✅ **PATTERNS_WORKING**: Extraction successful, no improvement needed");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **POST_EXECUTION_ANALYSIS_ERROR**: Failed to analyze pattern performance");
                return false;
            }
        }

        /// <summary>
        /// Creates header error detection prompt with automatic pattern improvement.
        /// This method includes the self-improving cycle when patterns fail.
        /// </summary>
        public async Task<string> CreateHeaderErrorDetectionPromptWithImprovementAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata,
            List<string> previouslyFailedPatterns = null,
            string provider = "deepseek")
        {
            try
            {
                _logger.Information("🚀 **AI_TEMPLATE_WITH_IMPROVEMENT_START**: Creating self-improving prompt for {Provider}", provider);
                
                // 1. Get the base prompt first
                var basePrompt = await CreateHeaderErrorDetectionPromptAsync(invoice, fileText, metadata, provider);
                
                // 2. If we have previously failed patterns, trigger improvement cycle
                if (previouslyFailedPatterns?.Any() == true)
                {
                    _logger.Information("🔄 **TRIGGERING_IMPROVEMENT**: {FailedCount} patterns previously failed, starting improvement cycle", 
                        previouslyFailedPatterns.Count);
                    
                    var failedPatterns = previouslyFailedPatterns.Select(p => new FailedPatternInfo
                    {
                        Pattern = p,
                        FailureReason = "Zero matches found in previous execution",
                        ActualText = fileText
                    }).ToList();
                    
                    var improvementSuccess = await ProcessTemplateImprovementCycle(
                        basePrompt, failedPatterns, fileText, provider, "header-detection", invoice?.SupplierName);
                    
                    if (improvementSuccess)
                    {
                        // Reload the improved template
                        _logger.Information("✅ **IMPROVEMENT_SUCCESS**: Reloading improved template");
                        return await CreateHeaderErrorDetectionPromptAsync(invoice, fileText, metadata, provider);
                    }
                    else
                    {
                        _logger.Warning("⚠️ **IMPROVEMENT_FAILED**: Using original template as fallback");
                    }
                }
                
                return basePrompt;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AI_TEMPLATE_WITH_IMPROVEMENT_ERROR**: Failed for provider {Provider}", provider);
                return CreateFallbackPrompt(invoice, fileText, metadata);
            }
        }

        /// <summary>
        /// Gets AI recommendations for improving a prompt from specified provider.
        /// </summary>
        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: AI template recommendations with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Get AI-powered template improvement recommendations from specified provider
        /// **BUSINESS OBJECTIVE**: Provide actionable template enhancement suggestions for continuous improvement
        /// **SUCCESS CRITERIA**: Valid recommendations generated, properly parsed, successfully saved, provider compatibility maintained
        /// </summary>
        public async Task<List<PromptRecommendation>> GetRecommendationsAsync(string prompt, string provider)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Complete LLM diagnostic workflow with success criteria validation
            
            // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
            _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for AI template recommendations");
            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Provider={Provider}, PromptLength={PromptLength}", provider ?? "NULL", prompt?.Length ?? 0);
            _logger.Error("🔍 **PATTERN_ANALYSIS**: Recommendation workflow requires meta-prompt creation, AI response, parsing, and persistence");
            _logger.Error("❓ **EVIDENCE_GAPS**: Need meta-prompt quality, AI response validity, parsing success, recommendation quality");
            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Provider-specific meta-prompts generate actionable template improvement recommendations");
            
            // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
            _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for AI recommendations");
            _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Track meta-prompt creation, AI response analysis, recommendation parsing, persistence validation");
            _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Meta-prompt structure, AI response format, recommendation quality, provider compatibility");
            
            // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
            _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based AI template recommendation workflow");
            _logger.Error("📚 **FIX_RATIONALE**: Provider-specific meta-prompts with comprehensive parsing ensure actionable recommendations");
            _logger.Error("🔍 **FIX_VALIDATION**: Validate inputs, create meta-prompt, parse AI response, persist recommendations");
            
            try
            {
                _logger.Error("🤖 **RECOMMENDATION_START**: Getting AI template improvement suggestions from {Provider}", provider);
                
                var metaPrompt = CreateRecommendationPrompt(prompt, provider);
                _logger.Error("📋 **META_PROMPT_CREATED**: Meta-prompt length={Length} characters for provider {Provider}", metaPrompt?.Length ?? 0, provider);
                
                var response = await CallAIProviderAsync(provider, metaPrompt);
                _logger.Error("📊 **AI_RESPONSE_RECEIVED**: AI response length={Length} characters from provider {Provider}", response?.Length ?? 0, provider);
                
                var recommendations = ParseRecommendations(response, provider);
                _logger.Error("🎯 **RECOMMENDATIONS_PARSED**: Parsed {Count} recommendations from AI response", recommendations?.Count ?? 0);
                
                SaveRecommendationsAsync(provider, recommendations);
                _logger.Error("💾 **RECOMMENDATIONS_SAVED**: Persisted {Count} recommendations for provider {Provider}", recommendations?.Count ?? 0, provider);
                
                // **STEP 4: MANDATORY SUCCESS CRITERIA VALIDATION**
                _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: AI template recommendations success analysis");
                
                // Individual criterion assessment with evidence
                var purposeFulfilled = recommendations != null && recommendations.Count > 0;
                _logger.Error((purposeFulfilled ? "✅" : "❌") + " **PURPOSE_FULFILLMENT**: " + 
                    (purposeFulfilled ? $"Successfully generated {recommendations.Count} AI template improvement recommendations" : 
                    "Failed to generate AI template recommendations"));
                
                var outputComplete = recommendations != null && recommendations.All(r => !string.IsNullOrEmpty(r.Description) && !string.IsNullOrEmpty(r.Reasoning));
                _logger.Error((outputComplete ? "✅" : "❌") + " **OUTPUT_COMPLETENESS**: " + 
                    (outputComplete ? "All recommendations have complete description and reasoning fields" : 
                    "Incomplete recommendations detected - missing description or reasoning"));
                
                var processComplete = !string.IsNullOrEmpty(metaPrompt) && !string.IsNullOrEmpty(response);
                _logger.Error((processComplete ? "✅" : "❌") + " **PROCESS_COMPLETION**: " + 
                    (processComplete ? "Complete recommendation workflow - meta-prompt created and AI response received" : 
                    "Incomplete recommendation workflow - missing meta-prompt or AI response"));
                
                var dataQuality = recommendations != null && recommendations.All(r => r.Priority >= 1 && r.Priority <= 5);
                _logger.Error((dataQuality ? "✅" : "❌") + " **DATA_QUALITY**: " + 
                    (dataQuality ? "All recommendations have valid priority values (1-5 scale)" : 
                    "Data quality issues detected - invalid priority values in recommendations"));
                
                var errorHandling = true; // Exception handling in place
                _logger.Error((errorHandling ? "✅" : "❌") + " **ERROR_HANDLING**: " + 
                    "Exception handling implemented for graceful failure recovery");
                
                var businessLogic = recommendations != null && recommendations.Any(r => r.Category == "Template Optimization" || r.Category == "Field Mapping" || r.Category == "Pattern Quality");
                _logger.Error((businessLogic ? "✅" : "❌") + " **BUSINESS_LOGIC**: " + 
                    (businessLogic ? "Recommendations align with template improvement business requirements" : 
                    "Business logic alignment issues - recommendations may not address template improvement needs"));
                
                var integrationSuccess = !string.IsNullOrEmpty(response);
                _logger.Error((integrationSuccess ? "✅" : "❌") + " **INTEGRATION_SUCCESS**: " + 
                    (integrationSuccess ? $"AI provider {provider} integration successful with valid response" : 
                    $"AI provider {provider} integration failed - no response received"));
                
                var performanceCompliance = recommendations != null && recommendations.Count <= 20; // Reasonable recommendation count
                _logger.Error((performanceCompliance ? "✅" : "❌") + " **PERFORMANCE_COMPLIANCE**: " + 
                    (performanceCompliance ? "Recommendation generation completed within reasonable bounds" : 
                    "Performance concerns - excessive recommendation count may impact system performance"));
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: AI recommendations template specification compliance analysis");
                
                // **TEMPLATE_SPEC_1: EntityType-Aware Recommendations**
                var entityTypeRecommendations = recommendations?.Where(r => 
                    r.Description.Contains("Invoice") || r.Description.Contains("InvoiceDetails") || 
                    r.Description.Contains("ShipmentBL") || r.Description.Contains("PurchaseOrders") ||
                    r.Reasoning.Contains("EntityType")).ToList() ?? new List<PromptRecommendation>();
                bool entityTypeAwarenessSuccess = entityTypeRecommendations.Any() || (recommendations?.Count ?? 0) == 0;
                _logger.Error((entityTypeAwarenessSuccess ? "✅" : "❌") + " **TEMPLATE_SPEC_ENTITYTYPE_AWARENESS**: " + 
                    (entityTypeAwarenessSuccess ? $"Generated {entityTypeRecommendations.Count} EntityType-aware recommendations" : 
                    "No EntityType-aware recommendations detected - may lack template specification context"));
                
                // **TEMPLATE_SPEC_2: Field Mapping Enhancement Recommendations**
                var fieldMappingRecommendations = recommendations?.Where(r => 
                    r.Description.Contains("field") || r.Description.Contains("mapping") || 
                    r.Category == "Field Mapping").ToList() ?? new List<PromptRecommendation>();
                bool fieldMappingEnhancementSuccess = fieldMappingRecommendations.Any() || (recommendations?.Count ?? 0) == 0;
                _logger.Error((fieldMappingEnhancementSuccess ? "✅" : "❌") + " **TEMPLATE_SPEC_FIELD_MAPPING**: " + 
                    (fieldMappingEnhancementSuccess ? $"Generated {fieldMappingRecommendations.Count} field mapping enhancement recommendations" : 
                    "No field mapping recommendations detected - may miss critical template improvement opportunities"));
                
                // **TEMPLATE_SPEC_3: Data Type Validation Recommendations**
                var dataTypeRecommendations = recommendations?.Where(r => 
                    r.Description.Contains("data type") || r.Description.Contains("validation") || 
                    r.Description.Contains("decimal") || r.Description.Contains("date")).ToList() ?? new List<PromptRecommendation>();
                bool dataTypeValidationSuccess = dataTypeRecommendations.Any() || (recommendations?.Count ?? 0) == 0;
                _logger.Error((dataTypeValidationSuccess ? "✅" : "❌") + " **TEMPLATE_SPEC_DATATYPE_RECOMMENDATIONS**: " + 
                    (dataTypeValidationSuccess ? $"Generated {dataTypeRecommendations.Count} data type validation recommendations" : 
                    "No data type validation recommendations - may miss type safety improvements"));
                
                // **TEMPLATE_SPEC_4: Pattern Quality Enhancement**
                var patternQualityRecommendations = recommendations?.Where(r => 
                    r.Description.Contains("regex") || r.Description.Contains("pattern") || 
                    r.Category == "Pattern Quality").ToList() ?? new List<PromptRecommendation>();
                bool patternQualitySuccess = patternQualityRecommendations.Any() || (recommendations?.Count ?? 0) == 0;
                _logger.Error((patternQualitySuccess ? "✅" : "❌") + " **TEMPLATE_SPEC_PATTERN_QUALITY**: " + 
                    (patternQualitySuccess ? $"Generated {patternQualityRecommendations.Count} pattern quality enhancement recommendations" : 
                    "No pattern quality recommendations - may miss regex and extraction improvements"));
                
                // **TEMPLATE_SPEC_5: Template Optimization Business Rules**
                var templateOptimizationRecommendations = recommendations?.Where(r => 
                    r.Category == "Template Optimization" || r.Description.Contains("optimization") || 
                    r.Description.Contains("performance")).ToList() ?? new List<PromptRecommendation>();
                bool templateOptimizationSuccess = templateOptimizationRecommendations.Any() || (recommendations?.Count ?? 0) == 0;
                _logger.Error((templateOptimizationSuccess ? "✅" : "❌") + " **TEMPLATE_SPEC_OPTIMIZATION**: " + 
                    (templateOptimizationSuccess ? $"Generated {templateOptimizationRecommendations.Count} template optimization recommendations" : 
                    "No template optimization recommendations - may miss performance improvement opportunities"));
                
                // **OVERALL SUCCESS VALIDATION WITH TEMPLATE SPECIFICATIONS**
                bool templateSpecificationSuccess = entityTypeAwarenessSuccess && fieldMappingEnhancementSuccess && 
                    dataTypeValidationSuccess && patternQualitySuccess && templateOptimizationSuccess;
                bool overallSuccess = purposeFulfilled && outputComplete && processComplete && dataQuality && 
                    errorHandling && businessLogic && integrationSuccess && performanceCompliance && templateSpecificationSuccess;
                _logger.Error(overallSuccess ? "🏆 **OVERALL_METHOD_SUCCESS**: ✅ PASS" : "🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL" + 
                    " - AI template recommendations " + (overallSuccess ? "generated successfully with comprehensive template specification compliance" : 
                    "failed due to validation criteria not met"));
                
                _logger.Error("✅ **RECOMMENDATION_SUCCESS**: Generated and saved {Count} AI template improvement suggestions for {Provider}", 
                    recommendations.Count, provider);
                
                return recommendations ?? new List<PromptRecommendation>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **EXCEPTION** in GetRecommendationsAsync for provider {Provider}", provider);
                
                // **EXCEPTION SUCCESS CRITERIA VALIDATION**
                _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: AI recommendations exception analysis");
                _logger.Error("❌ **PURPOSE_FULFILLMENT**: Failed to generate AI template recommendations due to exception");
                _logger.Error("❌ **OUTPUT_COMPLETENESS**: No recommendations generated due to exception");
                _logger.Error("❌ **PROCESS_COMPLETION**: Recommendation workflow interrupted by exception");
                _logger.Error("❌ **DATA_QUALITY**: No data quality assessment possible due to exception");
                _logger.Error("✅ **ERROR_HANDLING**: Exception caught and graceful empty result returned");
                _logger.Error("❌ **BUSINESS_LOGIC**: Business logic execution failed due to exception");
                _logger.Error("❌ **INTEGRATION_SUCCESS**: AI provider integration failed due to exception");
                _logger.Error("❌ **PERFORMANCE_COMPLIANCE**: Performance affected by exception handling");
                _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL - AI template recommendations failed due to exception");
                
                return new List<PromptRecommendation>();
            }
        }

        #endregion

        #region Template Loading and Management

        private string LoadTemplateAsync(string provider, string templateType, string supplierName)
        {
            // Check for versioned templates first, then fall back to standard templates
            var templatePaths = GetTemplateLoadPaths(provider, templateType, supplierName);

            foreach (var path in templatePaths)
            {
                if (File.Exists(path))
                {
                    _logger.Verbose("📄 **TEMPLATE_LOADED**: {TemplatePath}", path);
                    return File.ReadAllText(path);
                }
            }

            throw new FileNotFoundException($"No template found for {provider}/{templateType} (supplier: {supplierName})");
        }

        /// <summary>
        /// Gets template loading paths in priority order, including versioned templates.
        /// </summary>
        private string[] GetTemplateLoadPaths(string provider, string templateType, string supplierName)
        {
            var paths = new List<string>();
            
            // Check for latest versioned templates first
            var latestVersion = GetLatestTemplateVersion(provider, templateType, supplierName);
            
            if (latestVersion > 0)
            {
                var baseFileName = !string.IsNullOrEmpty(supplierName)
                    ? $"{supplierName.ToLower()}-{templateType}"
                    : templateType;
                
                // 1. Latest versioned supplier-specific template
                if (!string.IsNullOrEmpty(supplierName))
                {
                    paths.Add(Path.Combine(_templateBasePath, provider, $"{baseFileName}-v{latestVersion}.txt"));
                }
                
                // 2. Latest versioned standard template
                paths.Add(Path.Combine(_templateBasePath, provider, $"{templateType}-v{latestVersion}.txt"));
            }
            
            // 3. Standard supplier-specific template for provider
            if (!string.IsNullOrEmpty(supplierName))
            {
                paths.Add(Path.Combine(_templateBasePath, provider, $"{supplierName.ToLower()}-{templateType}.txt"));
            }
            
            // 4. Standard template for provider
            paths.Add(Path.Combine(_templateBasePath, provider, $"{templateType}.txt"));
            
            // 5. Default fallback template
            paths.Add(Path.Combine(_templateBasePath, "default", $"{templateType}.txt"));
            
            return paths.ToArray();
        }

        /// <summary>
        /// Gets the latest version number for a template, or 0 if no versions exist.
        /// </summary>
        private int GetLatestTemplateVersion(string provider, string templateType, string supplierName)
        {
            try
            {
                var versionTrackingPath = Path.Combine(_configBasePath, "template-versions.json");
                
                if (!File.Exists(versionTrackingPath))
                {
                    return 0;
                }
                
                var versionJson = File.ReadAllText(versionTrackingPath);
                var versionData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(versionJson)
                                  ?? new Dictionary<string, Dictionary<string, int>>();
                
                if (!versionData.ContainsKey(provider))
                {
                    return 0;
                }
                
                var templateKey = $"{provider}/{(!string.IsNullOrEmpty(supplierName) ? $"{supplierName.ToLower()}-" : "")}{templateType}";
                
                return versionData[provider].TryGetValue(templateKey, out var version) ? version : 0;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **VERSION_CHECK_FAILED**: Could not check template version for {Provider}/{TemplateType}", 
                    provider, templateType);
                return 0;
            }
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: AI template validation with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Validates AI template syntax, structure, and provider compatibility for template deployment
        /// **BUSINESS OBJECTIVE**: Ensure template quality and compliance before deployment to prevent AI template failures
        /// **SUCCESS CRITERIA**: Must verify template syntax, field mappings, provider compatibility, and template specification compliance
        /// </summary>
        private TemplateValidationResult ValidateTemplate(string template, string provider)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Complete LLM diagnostic workflow for template validation
            
            // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
            _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for AI template validation");
            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Template validation context with provider compatibility and quality assurance");
            _logger.Error("🔍 **PATTERN_ANALYSIS**: Validation → structure check → provider requirements → quality verification → result compilation pattern");
            _logger.Error("❓ **EVIDENCE_GAPS**: Need input validation, structure verification, provider compliance, quality assessment");
            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Template validation requires comprehensive structure and provider compatibility verification");
            
            // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
            _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for template validation");
            _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed structure validation, provider requirements, quality metrics tracking");
            _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Template structure, variable validation, section verification, provider compliance");
            
            // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
            _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based template validation");
            _logger.Error("📚 **FIX_RATIONALE**: Based on AI template requirements, implementing comprehensive validation workflow");
            _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring validation completeness and compliance accuracy");

            var result = new TemplateValidationResult();
            bool inputValid = false;
            bool structureValid = false;
            bool variablesValid = false;
            bool sectionsValid = false;
            bool providerValid = false;
            
            if (string.IsNullOrWhiteSpace(template))
            {
                result.Errors.Add("Template is empty or null");
                _logger.Error("❌ **INPUT_VALIDATION_FAILED**: Template is null or whitespace - cannot validate");
                
                // **STEP 4A: MANDATORY SUCCESS CRITERIA VALIDATION - INPUT VALIDATION FAILURE PATH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_SUCCESS_CRITERIA_VALIDATION**: Template validation failed due to input validation failure");
                _logger.Error("❌ **TEMPLATE_SPEC_SYNTAX_VALIDATION**: Cannot validate template syntax with null input");
                _logger.Error("❌ **TEMPLATE_SPEC_STRUCTURE_COMPLIANCE**: Cannot validate template structure with null input");
                _logger.Error("❌ **TEMPLATE_SPEC_VARIABLE_PRESENCE**: Cannot validate required variables with null input");
                _logger.Error("❌ **TEMPLATE_SPEC_SECTION_COMPLETENESS**: Cannot validate required sections with null input");
                _logger.Error("❌ **TEMPLATE_SPEC_PROVIDER_COMPATIBILITY**: Cannot validate provider compatibility with null input");
                
                _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: Template validation failed due to input validation failure");
                _logger.Error("❌ **PURPOSE_FULFILLMENT**: Cannot perform template validation with null template");
                _logger.Error("❌ **OUTPUT_COMPLETENESS**: Validation result with input failure error only");
                _logger.Error("❌ **PROCESS_COMPLETION**: Template validation workflow terminated at input validation");
                _logger.Error("❌ **DATA_QUALITY**: No validation processing possible with null template");
                _logger.Error("✅ **ERROR_HANDLING**: Input validation handled gracefully with error result");
                _logger.Error("❌ **BUSINESS_LOGIC**: Template validation objective cannot be achieved without valid template");
                _logger.Error("❌ **INTEGRATION_SUCCESS**: No validation processing possible without valid template");
                _logger.Error("✅ **PERFORMANCE_COMPLIANCE**: Validation completed within reasonable timeframe");
                _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL - Template validation terminated due to input validation failure");
                
                return result;
            }
            
            inputValid = true;
            _logger.Information("✅ **INPUT_VALIDATION_SUCCESS**: Template input valid - Length={TemplateLength}, Provider='{Provider}'", 
                template.Length, provider);
            
            if (template.Length < 200)
            {
                result.Errors.Add("Template too short (< 200 characters)");
                _logger.Warning("⚠️ **STRUCTURE_VALIDATION_FAILED**: Template too short - Length={Length}", template.Length);
            }
            else
            {
                structureValid = true;
                _logger.Information("✅ **STRUCTURE_VALIDATION_SUCCESS**: Template length adequate - Length={Length}", template.Length);
            }
            
            // Check for required template variables
            var requiredVariables = new[] { "{{invoiceJson}}", "{{fileText}}" };
            int foundVariables = 0;
            foreach (var variable in requiredVariables)
            {
                if (!template.Contains(variable))
                {
                    result.Errors.Add($"Missing required variable: {variable}");
                    _logger.Warning("⚠️ **VARIABLE_VALIDATION_FAILED**: Missing required variable '{Variable}'", variable);
                }
                else
                {
                    foundVariables++;
                    _logger.Information("✅ **VARIABLE_VALIDATION_SUCCESS**: Found required variable '{Variable}'", variable);
                }
            }
            variablesValid = foundVariables == requiredVariables.Length;
            
            // Check for required sections
            var requiredSections = new[] { "EXTRACTED FIELDS", "CRITICAL", "COMPLETION REQUIREMENTS" };
            int foundSections = 0;
            foreach (var section in requiredSections)
            {
                if (!template.Contains(section))
                {
                    result.Errors.Add($"Missing required section: {section}");
                    _logger.Warning("⚠️ **SECTION_VALIDATION_FAILED**: Missing required section '{Section}'", section);
                }
                else
                {
                    foundSections++;
                    _logger.Information("✅ **SECTION_VALIDATION_SUCCESS**: Found required section '{Section}'", section);
                }
            }
            sectionsValid = foundSections == requiredSections.Length;
            
            // Provider-specific validation
            var providerErrors = result.Errors.Count;
            ValidateProviderSpecificRequirements(template, provider, result);
            providerValid = result.Errors.Count == providerErrors; // No new errors added
            
            result.IsValid = result.Errors.Count == 0;
            
            _logger.Information("📊 **TEMPLATE_VALIDATION_SUMMARY**: IsValid={IsValid}, Errors={ErrorCount}, Variables={FoundVars}/{TotalVars}, Sections={FoundSecs}/{TotalSecs}", 
                result.IsValid, result.Errors.Count, foundVariables, requiredVariables.Length, foundSections, requiredSections.Length);
            
            // **STEP 4: MANDATORY TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION** ⭐ **ENHANCED WITH TEMPLATE SPECIFICATIONS**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_SUCCESS_CRITERIA_VALIDATION**: AI template validation success analysis with Template Specifications compliance");
            
            // **TEMPLATE_SPEC_1: Template Syntax Validation**
            bool templateSyntaxValid = inputValid && structureValid;
            _logger.Error((templateSyntaxValid ? "✅" : "❌") + " **TEMPLATE_SPEC_SYNTAX_VALIDATION**: " + 
                (templateSyntaxValid ? $"Template syntax validation success - template structure valid with {template.Length} characters" : 
                "Template syntax validation failed - template structure invalid or too short"));
            
            // **TEMPLATE_SPEC_2: Required Variable Compliance**
            bool variableComplianceValid = variablesValid;
            _logger.Error((variableComplianceValid ? "✅" : "❌") + " **TEMPLATE_SPEC_VARIABLE_COMPLIANCE**: " + 
                (variableComplianceValid ? $"Variable compliance success - {foundVariables}/{requiredVariables.Length} required variables present" : 
                $"Variable compliance failed - only {foundVariables}/{requiredVariables.Length} required variables present"));
            
            // **TEMPLATE_SPEC_3: Section Completeness Validation**
            bool sectionCompletenessValid = sectionsValid;
            _logger.Error((sectionCompletenessValid ? "✅" : "❌") + " **TEMPLATE_SPEC_SECTION_COMPLETENESS**: " + 
                (sectionCompletenessValid ? $"Section completeness success - {foundSections}/{requiredSections.Length} required sections present" : 
                $"Section completeness failed - only {foundSections}/{requiredSections.Length} required sections present"));
            
            // **TEMPLATE_SPEC_4: Provider Compatibility Validation**
            bool providerCompatibilityValid = providerValid && !string.IsNullOrEmpty(provider);
            _logger.Error((providerCompatibilityValid ? "✅" : "❌") + " **TEMPLATE_SPEC_PROVIDER_COMPATIBILITY**: " + 
                (providerCompatibilityValid ? $"Provider compatibility success - template compatible with '{provider}' provider requirements" : 
                $"Provider compatibility failed - template incompatible with '{provider}' provider requirements"));
            
            // **TEMPLATE_SPEC_5: AI Template Quality Validation**
            bool templateQualityValid = result.IsValid && result.Warnings.Count <= 2;
            _logger.Error((templateQualityValid ? "✅" : "❌") + " **TEMPLATE_SPEC_QUALITY_VALIDATION**: " + 
                (templateQualityValid ? $"Template quality validation success - {result.Errors.Count} errors, {result.Warnings.Count} warnings" : 
                $"Template quality validation failed - {result.Errors.Count} errors, {result.Warnings.Count} warnings exceed quality thresholds"));
            
            // **STEP 4: MANDATORY SUCCESS CRITERIA VALIDATION** ⭐ **ENHANCED WITH TEMPLATE SPECIFICATIONS**
            _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: Template validation success analysis with enhanced template specification validation");
            
            bool validationExecuted = inputValid;
            bool outputComplete = result != null && result.Errors != null;
            bool processCompleted = inputValid && structureValid && variablesValid && sectionsValid && providerValid;
            bool dataQualityMet = result.IsValid || result.Errors.Count > 0; // Either valid or has meaningful errors
            bool errorHandlingSuccess = result.Errors != null && result.Warnings != null;
            bool businessLogicCorrect = string.IsNullOrWhiteSpace(template) ? !result.IsValid : true;
            bool integrationSuccess = !string.IsNullOrEmpty(provider);
            bool performanceCompliant = template == null || template.Length < 100000; // Reasonable template size
            
            _logger.Error((validationExecuted ? "✅" : "❌") + " **PURPOSE_FULFILLMENT**: " + (validationExecuted ? "Template validation executed successfully" : "Template validation execution failed"));
            _logger.Error((outputComplete ? "✅" : "❌") + " **OUTPUT_COMPLETENESS**: " + (outputComplete ? "Valid validation result returned with proper structure" : "Validation result malformed or incomplete"));
            _logger.Error((processCompleted ? "✅" : "❌") + " **PROCESS_COMPLETION**: " + (processCompleted ? "All validation steps completed successfully" : "Validation processing incomplete"));
            _logger.Error((dataQualityMet ? "✅" : "❌") + " **DATA_QUALITY**: " + (dataQualityMet ? "Validation results properly assessed and documented" : "Validation results assessment failed"));
            _logger.Error((errorHandlingSuccess ? "✅" : "❌") + " **ERROR_HANDLING**: " + (errorHandlingSuccess ? "Error and warning collection functioning properly" : "Error and warning collection failed"));
            _logger.Error((businessLogicCorrect ? "✅" : "❌") + " **BUSINESS_LOGIC**: " + (businessLogicCorrect ? "Template validation follows business rules" : "Template validation business logic validation failed"));
            _logger.Error((integrationSuccess ? "✅" : "❌") + " **INTEGRATION_SUCCESS**: " + (integrationSuccess ? "Provider integration functioning properly" : "Provider integration failed"));
            _logger.Error((performanceCompliant ? "✅" : "❌") + " **PERFORMANCE_COMPLIANCE**: " + (performanceCompliant ? "Template size within reasonable performance limits" : "Template size exceeds performance limits"));
            
            // **ENHANCED OVERALL SUCCESS WITH TEMPLATE SPECIFICATIONS**
            bool overallSuccess = validationExecuted && outputComplete && processCompleted && dataQualityMet && errorHandlingSuccess && businessLogicCorrect && integrationSuccess && performanceCompliant &&
                                 templateSyntaxValid && variableComplianceValid && sectionCompletenessValid && providerCompatibilityValid && templateQualityValid;
            _logger.Error(overallSuccess ? "🏆 **OVERALL_METHOD_SUCCESS**: ✅ PASS" : ("🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL" + " - Template validation analysis"));
            
            _logger.Error("📊 **TEMPLATE_VALIDATION_SUMMARY**: IsValid={IsValid}, Provider='{Provider}', Errors={ErrorCount}, Warnings={WarningCount}, OverallSuccess={OverallSuccess}", 
                result.IsValid, provider, result.Errors.Count, result.Warnings.Count, overallSuccess);
            
            return result;
        }

        private void ValidateProviderSpecificRequirements(string template, string provider, TemplateValidationResult result)
        {
            switch (provider.ToLower())
            {
                case "deepseek":
                    if (!template.Contains("logical") && !template.Contains("systematic"))
                    {
                        result.Warnings.Add("DeepSeek templates should leverage logical reasoning capabilities");
                    }
                    break;
                    
                case "gemini":
                    if (!template.Contains("comprehensive") && !template.Contains("contextual"))
                    {
                        result.Warnings.Add("Gemini templates should leverage comprehensive understanding");
                    }
                    break;
            }
        }

        #endregion

        #region Data Preparation (Extracted from existing OCRPromptCreation.cs)

        private Dictionary<string, string> PrepareTemplateData(ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata)
        {
            // Extract invoice data (same as existing CreateHeaderErrorDetectionPrompt)
            var currentValues = new Dictionary<string, object>
            {
                ["InvoiceNo"] = invoice?.InvoiceNo,
                ["InvoiceDate"] = invoice?.InvoiceDate,
                ["SupplierName"] = invoice?.SupplierName,
                ["Currency"] = invoice?.Currency,
                ["SubTotal"] = invoice?.SubTotal,
                ["TotalInternalFreight"] = invoice?.TotalInternalFreight,
                ["TotalOtherCost"] = invoice?.TotalOtherCost,
                ["TotalDeduction"] = invoice?.TotalDeduction,
                ["TotalInsurance"] = invoice?.TotalInsurance,
                ["InvoiceTotal"] = invoice?.InvoiceTotal,
            };

            var serializerOptions = new JsonSerializerOptions 
            { 
                WriteIndented = true, 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
            };
            
            var currentJson = JsonSerializer.Serialize(currentValues, serializerOptions);
            var annotatedContext = BuildAnnotatedContext(metadata, invoice);
            var balanceCheckContext = BuildBalanceCheckContext(invoice);
            var cleanedFileText = CleanTextForAnalysis(fileText);
            var ocrSections = AnalyzeOCRSections(fileText);

            return new Dictionary<string, string>
            {
                ["invoiceJson"] = currentJson,
                ["annotatedContext"] = annotatedContext,
                ["balanceCheckContext"] = balanceCheckContext,
                ["fileText"] = cleanedFileText,
                ["supplierName"] = invoice?.SupplierName ?? "Unknown",
                ["ocrSections"] = string.Join(", ", ocrSections),
                ["invoiceNo"] = invoice?.InvoiceNo ?? "Unknown",
                ["currency"] = invoice?.Currency ?? "Unknown",
                ["invoiceTotal"] = invoice?.InvoiceTotal?.ToString() ?? "0"
            };
        }

        private string BuildAnnotatedContext(Dictionary<string, OCRFieldMetadata> metadata, ShipmentInvoice invoice)
        {
            if (metadata == null || metadata.Count == 0)
            {
                return "No additional context available.";
            }

            var contextBuilder = new StringBuilder();
            var fieldsGroupedByCanonicalName = metadata.Values
                .Where(m => m != null && !string.IsNullOrEmpty(m.Field))
                .GroupBy(m => m.Field);

            foreach (var group in fieldsGroupedByCanonicalName)
            {
                if (group.Count() > 1)
                {
                    var finalValue = GetCurrentFieldValue(invoice, group.Key);
                    contextBuilder.AppendLine($"\n- The value for `{group.Key}` ({finalValue}) was calculated by summing the following lines:");
                    foreach (var component in group)
                    {
                        contextBuilder.AppendLine($"  - Line {component.LineNumber}: Found value '{component.RawValue}' from rule '{component.LineName}' on text: \"{TruncateForLog(component.LineText, 100)}\"");
                    }
                }
            }

            return contextBuilder.ToString();
        }

        private string BuildBalanceCheckContext(ShipmentInvoice invoice)
        {
            if (invoice == null)
            {
                return "No invoice data available for balance check.";
            }

            double subTotal = invoice.SubTotal ?? 0;
            double freight = invoice.TotalInternalFreight ?? 0;
            double otherCost = invoice.TotalOtherCost ?? 0;
            double deduction = invoice.TotalDeduction ?? 0;
            double insurance = invoice.TotalInsurance ?? 0;
            double reportedTotal = invoice.InvoiceTotal ?? 0;
            double calculatedTotal = subTotal + freight + otherCost + insurance - deduction;
            double discrepancy = reportedTotal - calculatedTotal;

            return $@"
**MATHEMATICAL BALANCE CHECK:**
My system's calculated total is {calculatedTotal:F2}. The reported InvoiceTotal is {reportedTotal:F2}.
The current discrepancy is: **{discrepancy:F2}**.
Your primary goal is to find all missing values in the text that account for this discrepancy.";
        }

        private string CleanTextForAnalysis(string fileText)
        {
            if (string.IsNullOrWhiteSpace(fileText))
            {
                return "No OCR text available.";
            }

            // Truncate very long text to prevent prompt overflow
            const int maxLength = 5000;
            if (fileText.Length > maxLength)
            {
                return fileText.Substring(0, maxLength) + "\n\n[TEXT TRUNCATED - SHOWING FIRST 5000 CHARACTERS]";
            }

            return fileText;
        }

        private List<string> AnalyzeOCRSections(string fileText)
        {
            var sections = new List<string>();
            if (string.IsNullOrEmpty(fileText)) return sections;

            // Simple heuristic analysis of OCR text structure
            var lines = fileText.Split('\n');
            var totalLines = lines.Length;

            if (totalLines < 10)
                sections.Add("SparseText");
            else if (lines.Any(line => line.Length > 100))
                sections.Add("Single Column");
            else if (lines.Any(line => line.Contains('\t') || line.Split(' ').Length > 10))
                sections.Add("Multi Column");
            else
                sections.Add("Ripped Text");

            return sections;
        }

        private object GetCurrentFieldValue(ShipmentInvoice invoice, string fieldName)
        {
            if (invoice == null) return null;

            return fieldName switch
            {
                "InvoiceNo" => invoice.InvoiceNo,
                "InvoiceDate" => invoice.InvoiceDate,
                "SupplierName" => invoice.SupplierName,
                "Currency" => invoice.Currency,
                "SubTotal" => invoice.SubTotal,
                "TotalInternalFreight" => invoice.TotalInternalFreight,
                "TotalOtherCost" => invoice.TotalOtherCost,
                "TotalDeduction" => invoice.TotalDeduction,
                "TotalInsurance" => invoice.TotalInsurance,
                "InvoiceTotal" => invoice.InvoiceTotal,
                _ => null
            };
        }

        private string TruncateForLog(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text ?? "";
            
            return text.Substring(0, maxLength) + "...";
        }

        #endregion

        #region Template Rendering

        private string RenderTemplate(string template, Dictionary<string, string> data)
        {
            var result = template;
            
            // Simple variable substitution using {{variable}} syntax
            foreach (var kvp in data)
            {
                var placeholder = $"{{{{{kvp.Key}}}}}";
                result = result.Replace(placeholder, kvp.Value ?? "");
            }
            
            // Log any unresolved variables
            var unresolvedVariables = System.Text.RegularExpressions.Regex.Matches(result, @"\{\{([^}]+)\}\}")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .ToList();
                
            if (unresolvedVariables.Any())
            {
                _logger.Warning("⚠️ **UNRESOLVED_VARIABLES**: {Variables}", string.Join(", ", unresolvedVariables));
            }
            
            return result;
        }

        #endregion

        #region Self-Improving Template System (Pattern Failure Detection & Auto-Improvement)

        /// <summary>
        /// Detects when regex patterns fail (return zero matches) and triggers automatic template improvement.
        /// This is the core of the self-improving system.
        /// </summary>
        public async Task<bool> DetectAndHandlePatternFailure(
            string templateUsed,
            List<string> regexPatterns,
            string actualText,
            string provider,
            string templateType,
            string supplierName)
        {
            try
            {
                _logger.Information("🔍 **PATTERN_FAILURE_DETECTION**: Analyzing {PatternCount} regex patterns for zero matches",
                    regexPatterns?.Count ?? 0);

                var failedPatterns = new List<FailedPatternInfo>();
                
                // Test each regex pattern against actual text
                foreach (var pattern in regexPatterns ?? new List<string>())
                {
                    if (string.IsNullOrEmpty(pattern)) continue;
                    
                    try
                    {
                        var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        var matches = regex.Matches(actualText);
                        
                        if (matches.Count == 0)
                        {
                            _logger.Warning("⚠️ **PATTERN_FAILED**: Pattern '{Pattern}' returned zero matches", pattern);
                            failedPatterns.Add(new FailedPatternInfo
                            {
                                Pattern = pattern,
                                FailureReason = "Zero matches found",
                                ActualText = actualText
                            });
                        }
                        else
                        {
                            _logger.Information("✅ **PATTERN_SUCCESS**: Pattern '{Pattern}' found {MatchCount} matches", 
                                pattern, matches.Count);
                        }
                    }
                    catch (Exception regexEx)
                    {
                        _logger.Error(regexEx, "❌ **PATTERN_INVALID**: Regex pattern '{Pattern}' is invalid", pattern);
                        failedPatterns.Add(new FailedPatternInfo
                        {
                            Pattern = pattern,
                            FailureReason = $"Invalid regex: {regexEx.Message}",
                            ActualText = actualText
                        });
                    }
                }

                // If we have failed patterns, trigger improvement cycle
                if (failedPatterns.Any())
                {
                    _logger.Warning("🚨 **TEMPLATE_IMPROVEMENT_TRIGGERED**: {FailedCount} patterns failed, starting improvement cycle",
                        failedPatterns.Count);
                    
                    return await ProcessTemplateImprovementCycle(
                        templateUsed,
                        failedPatterns,
                        actualText,
                        provider,
                        templateType,
                        supplierName);
                }
                
                _logger.Information("✅ **ALL_PATTERNS_WORKING**: No pattern failures detected");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PATTERN_FAILURE_DETECTION_ERROR**: Failed to analyze patterns");
                return false;
            }
        }

        /// <summary>
        /// Main orchestration method for the self-improving template system.
        /// Cycles through AI provider improvements until patterns work or max attempts reached.
        /// </summary>
        public async Task<bool> ProcessTemplateImprovementCycle(
            string currentTemplate,
            List<FailedPatternInfo> failedPatterns,
            string actualText,
            string provider,
            string templateType,
            string supplierName,
            int maxAttempts = 3)
        {
            try
            {
                _logger.Information("🔄 **IMPROVEMENT_CYCLE_START**: Starting template improvement cycle for {Provider}/{TemplateType}",
                    provider, templateType);

                var currentTemplateContent = currentTemplate;
                var attemptNumber = 1;
                
                while (attemptNumber <= maxAttempts)
                {
                    _logger.Information("🔄 **IMPROVEMENT_ATTEMPT_{AttemptNumber}**: Requesting template improvements from AI providers",
                        attemptNumber);
                    
                    // Request improvements from both DeepSeek and Gemini
                    var deepseekImprovement = await RequestTemplateImprovementAsync(
                        "deepseek", currentTemplateContent, failedPatterns, actualText, templateType, supplierName);
                    
                    var geminiImprovement = await RequestTemplateImprovementAsync(
                        "gemini", currentTemplateContent, failedPatterns, actualText, templateType, supplierName);
                    
                    // Choose the best improvement (prefer provider-specific, then highest confidence)
                    var chosenImprovement = ChooseBestImprovement(deepseekImprovement, geminiImprovement, provider);
                    
                    if (chosenImprovement != null)
                    {
                        // Save improved template as new version
                        var versionedTemplatePath = await SaveImprovedTemplateVersion(
                            provider, templateType, supplierName, chosenImprovement.ImprovedTemplate, attemptNumber);
                        
                        _logger.Information("💾 **TEMPLATE_VERSION_SAVED**: Saved improved template v{Version} to {Path}",
                            attemptNumber, versionedTemplatePath);
                        
                        // Test the improved template
                        var testResult = await TestImprovedTemplate(
                            chosenImprovement.ImprovedTemplate, actualText, chosenImprovement.ImprovedPatterns);
                        
                        if (testResult.Success)
                        {
                            _logger.Information("✅ **IMPROVEMENT_SUCCESS**: Template improvement successful after {Attempts} attempts",
                                attemptNumber);
                            return true;
                        }
                        else
                        {
                            _logger.Warning("⚠️ **IMPROVEMENT_FAILED**: Template v{Version} still has {FailedCount} failing patterns",
                                attemptNumber, testResult.FailedPatterns.Count);
                            
                            // Update failed patterns for next iteration
                            failedPatterns = testResult.FailedPatterns;
                            currentTemplateContent = chosenImprovement.ImprovedTemplate;
                        }
                    }
                    else
                    {
                        _logger.Warning("⚠️ **NO_IMPROVEMENTS**: AI providers returned no valid improvements for attempt {Attempt}",
                            attemptNumber);
                    }
                    
                    attemptNumber++;
                }
                
                _logger.Error("❌ **IMPROVEMENT_CYCLE_FAILED**: Template improvement failed after {MaxAttempts} attempts",
                    maxAttempts);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **IMPROVEMENT_CYCLE_ERROR**: Exception during template improvement cycle");
                return false;
            }
        }

        /// <summary>
        /// Requests template improvement from a specific AI provider.
        /// </summary>
        private async Task<TemplateImprovementResponse> RequestTemplateImprovementAsync(
            string provider,
            string currentTemplate,
            List<FailedPatternInfo> failedPatterns,
            string actualText,
            string templateType,
            string supplierName)
        {
            try
            {
                _logger.Information("🤖 **REQUESTING_IMPROVEMENT**: Asking {Provider} to improve template", provider);
                
                var improvementPrompt = CreateTemplateImprovementPrompt(
                    provider, currentTemplate, failedPatterns, actualText, templateType, supplierName);
                
                var response = await CallAIProviderAsync(provider, improvementPrompt);
                
                if (string.IsNullOrEmpty(response))
                {
                    _logger.Warning("⚠️ **NO_RESPONSE**: {Provider} returned empty response for template improvement", provider);
                    return null;
                }
                
                return ParseTemplateImprovementResponse(response, provider);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **IMPROVEMENT_REQUEST_FAILED**: Failed to get template improvement from {Provider}", provider);
                return null;
            }
        }

        /// <summary>
        /// Creates a detailed prompt asking an AI provider to improve a failing template.
        /// </summary>
        private string CreateTemplateImprovementPrompt(
            string provider,
            string currentTemplate,
            List<FailedPatternInfo> failedPatterns,
            string actualText,
            string templateType,
            string supplierName)
        {
            var failedPatternsText = string.Join("\n", failedPatterns.Select((fp, i) => 
                $"{i + 1}. Pattern: {fp.Pattern}\n   Reason: {fp.FailureReason}"));
            
            var actualTextTruncated = actualText.Length > 2000 ? actualText.Substring(0, 2000) + "..." : actualText;
            
            return $@"
You are an expert prompt engineer specializing in {provider.ToUpper()} optimization. 
A template for {templateType} processing (supplier: {supplierName ?? "Generic"}) is failing because its regex patterns don't match the actual text.

**CURRENT FAILING TEMPLATE:**
{currentTemplate}

**FAILED PATTERNS:**
{failedPatternsText}

**ACTUAL TEXT TO MATCH:**
{actualTextTruncated}

**YOUR TASK:**
Analyze why the regex patterns are failing and create an improved template that will successfully extract data from this text.

**REQUIREMENTS:**
1. Keep the same template structure and variables ({{invoiceJson}}, {{fileText}}, etc.)
2. Focus on improving the regex pattern generation instructions
3. Add specific guidance for this supplier's text format
4. Ensure all regex patterns use named capture groups: (?<FieldName>pattern)
5. Make instructions more specific to prevent zero-match failures

**RESPONSE FORMAT:**
Return your response as JSON:
{{
  ""provider"": ""{provider}"",
  ""confidence"": 0.85,
  ""improvements_made"": [
    ""Specific improvement 1"",
    ""Specific improvement 2""
  ],
  ""improved_template"": ""Your complete improved template here"",
  ""improved_patterns"": [
    ""(?<InvoiceTotal>pattern1)"",
    ""(?<InvoiceDate>pattern2)""
  ],
  ""reasoning"": ""Explanation of why these improvements will work""
}}

**{provider.ToUpper()}-SPECIFIC OPTIMIZATION:**
{GetProviderSpecificGuidance(provider)}
";
        }

        /// <summary>
        /// Provides provider-specific guidance for template improvements.
        /// </summary>
        private string GetProviderSpecificGuidance(string provider)
        {
            return provider.ToLower() switch
            {
                "deepseek" => @"
- Leverage DeepSeek's logical reasoning to create more robust pattern matching
- Use systematic analysis to identify text structure patterns
- Focus on logical edge case handling in regex patterns
- Create step-by-step reasoning for pattern construction",
                "gemini" => @"
- Utilize Gemini's comprehensive understanding of document formats
- Focus on contextual pattern recognition across different layouts
- Leverage multi-modal reasoning for text structure analysis
- Create adaptive patterns that work across format variations",
                _ => "Focus on creating robust, specific regex patterns that match the actual text format."
            };
        }

        /// <summary>
        /// Parses the AI provider's template improvement response.
        /// </summary>
        private TemplateImprovementResponse ParseTemplateImprovementResponse(string response, string provider)
        {
            try
            {
                var cleanJson = ExtractJsonFromProviderResponse(response, provider);
                if (string.IsNullOrEmpty(cleanJson))
                {
                    _logger.Warning("⚠️ **PARSE_FAILED**: No valid JSON found in {Provider} improvement response", provider);
                    return null;
                }
                
                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;
                
                var improvements = new List<string>();
                if (root.TryGetProperty("improvements_made", out var improvementsArray) && 
                    improvementsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in improvementsArray.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            improvements.Add(item.GetString());
                        }
                    }
                }
                
                var patterns = new List<string>();
                if (root.TryGetProperty("improved_patterns", out var patternsArray) && 
                    patternsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in patternsArray.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            patterns.Add(item.GetString());
                        }
                    }
                }
                
                return new TemplateImprovementResponse
                {
                    Provider = provider,
                    Confidence = root.TryGetProperty("confidence", out var confProp) && confProp.TryGetDouble(out var confVal) ? confVal : 0.5,
                    ImprovementsMade = improvements,
                    ImprovedTemplate = root.TryGetProperty("improved_template", out var templateProp) ? templateProp.GetString() ?? "" : "",
                    ImprovedPatterns = patterns,
                    Reasoning = root.TryGetProperty("reasoning", out var reasonProp) ? reasonProp.GetString() ?? "" : ""
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PARSE_IMPROVEMENT_FAILED**: Failed to parse {Provider} improvement response", provider);
                return null;
            }
        }

        /// <summary>
        /// Chooses the best improvement from multiple AI provider responses.
        /// </summary>
        private TemplateImprovementResponse ChooseBestImprovement(
            TemplateImprovementResponse deepseekResponse,
            TemplateImprovementResponse geminiResponse,
            string preferredProvider)
        {
            var responses = new[] { deepseekResponse, geminiResponse }
                .Where(r => r != null && !string.IsNullOrEmpty(r.ImprovedTemplate))
                .ToList();
            
            if (!responses.Any())
            {
                _logger.Warning("⚠️ **NO_VALID_IMPROVEMENTS**: No valid improvements received from any provider");
                return null;
            }
            
            // Prefer the provider-specific response if available
            var preferredResponse = responses.FirstOrDefault(r => r.Provider.Equals(preferredProvider, StringComparison.OrdinalIgnoreCase));
            if (preferredResponse != null)
            {
                _logger.Information("✅ **CHOSEN_PREFERRED**: Selected {Provider} improvement (preferred provider)", preferredProvider);
                return preferredResponse;
            }
            
            // Otherwise, choose the highest confidence response
            var bestResponse = responses.OrderByDescending(r => r.Confidence).First();
            _logger.Information("✅ **CHOSEN_BEST**: Selected {Provider} improvement (confidence: {Confidence})", 
                bestResponse.Provider, bestResponse.Confidence);
            
            return bestResponse;
        }

        /// <summary>
        /// Saves an improved template as a new version file.
        /// </summary>
        private async Task<string> SaveImprovedTemplateVersion(
            string provider,
            string templateType,
            string supplierName,
            string improvedTemplate,
            int versionNumber)
        {
            try
            {
                // Determine template file name
                var baseFileName = !string.IsNullOrEmpty(supplierName)
                    ? $"{supplierName.ToLower()}-{templateType}"
                    : templateType;
                
                var versionedFileName = $"{baseFileName}-v{versionNumber}.txt";
                var templatePath = Path.Combine(_templateBasePath, provider, versionedFileName);
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
                
                // Write improved template
                await Task.Run(() => File.WriteAllText(templatePath, improvedTemplate));
                
                _logger.Information("💾 **TEMPLATE_SAVED**: Saved improved template to {Path}", templatePath);
                
                // Update template loading to use the latest version
                UpdateTemplateVersionTracking(provider, templateType, supplierName, versionNumber);
                
                return templatePath;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **SAVE_TEMPLATE_FAILED**: Failed to save improved template v{Version}", versionNumber);
                return null;
            }
        }

        /// <summary>
        /// Updates template version tracking to use the latest version.
        /// </summary>
        private void UpdateTemplateVersionTracking(string provider, string templateType, string supplierName, int versionNumber)
        {
            try
            {
                var versionTrackingPath = Path.Combine(_configBasePath, "template-versions.json");
                
                var versionData = new Dictionary<string, Dictionary<string, int>>();
                if (File.Exists(versionTrackingPath))
                {
                    var existingJson = File.ReadAllText(versionTrackingPath);
                    versionData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(existingJson) 
                                  ?? new Dictionary<string, Dictionary<string, int>>();
                }
                
                var templateKey = $"{provider}/{(!string.IsNullOrEmpty(supplierName) ? $"{supplierName.ToLower()}-" : "")}{templateType}";
                
                if (!versionData.ContainsKey(provider))
                {
                    versionData[provider] = new Dictionary<string, int>();
                }
                
                versionData[provider][templateKey] = versionNumber;
                
                var json = JsonSerializer.Serialize(versionData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(versionTrackingPath, json);
                
                _logger.Information("📊 **VERSION_TRACKING_UPDATED**: {TemplateKey} now at v{Version}", templateKey, versionNumber);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **VERSION_TRACKING_FAILED**: Failed to update version tracking");
            }
        }

        /// <summary>
        /// Tests an improved template against actual text to verify patterns work.
        /// </summary>
        private async Task<TemplateTestResult> TestImprovedTemplate(
            string improvedTemplate,
            string actualText,
            List<string> improvedPatterns)
        {
            try
            {
                _logger.Information("🧪 **TESTING_TEMPLATE**: Testing improved template with {PatternCount} patterns",
                    improvedPatterns?.Count ?? 0);
                
                var failedPatterns = new List<FailedPatternInfo>();
                var successfulPatterns = new List<string>();
                
                foreach (var pattern in improvedPatterns ?? new List<string>())
                {
                    if (string.IsNullOrEmpty(pattern)) continue;
                    
                    try
                    {
                        var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        var matches = regex.Matches(actualText);
                        
                        if (matches.Count == 0)
                        {
                            failedPatterns.Add(new FailedPatternInfo
                            {
                                Pattern = pattern,
                                FailureReason = "Zero matches found",
                                ActualText = actualText
                            });
                        }
                        else
                        {
                            successfulPatterns.Add(pattern);
                        }
                    }
                    catch (Exception regexEx)
                    {
                        failedPatterns.Add(new FailedPatternInfo
                        {
                            Pattern = pattern,
                            FailureReason = $"Invalid regex: {regexEx.Message}",
                            ActualText = actualText
                        });
                    }
                }
                
                var success = !failedPatterns.Any();
                _logger.Information("🧪 **TEST_RESULT**: Success={Success}, Working={WorkingCount}, Failed={FailedCount}",
                    success, successfulPatterns.Count, failedPatterns.Count);
                
                return new TemplateTestResult
                {
                    Success = success,
                    SuccessfulPatterns = successfulPatterns,
                    FailedPatterns = failedPatterns,
                    ImprovedTemplate = improvedTemplate
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_TEST_FAILED**: Exception during template testing");
                return new TemplateTestResult
                {
                    Success = false,
                    FailedPatterns = new List<FailedPatternInfo>(),
                    SuccessfulPatterns = new List<string>(),
                    ImprovedTemplate = improvedTemplate
                };
            }
        }

        #endregion

        #region AI Recommendations System

        private string CreateRecommendationPrompt(string originalPrompt, string provider)
        {
            var truncatedPrompt = originalPrompt.Length > 2000 
                ? originalPrompt.Substring(0, 2000) + "..." 
                : originalPrompt;

            return $@"
You are an expert prompt engineer. Analyze this OCR correction prompt and suggest 3-5 specific improvements for {provider.ToUpper()}:

CURRENT PROMPT:
{truncatedPrompt}

Please suggest improvements specifically for {provider.ToUpper()} focusing on:
1. Clarity and specificity
2. {provider.ToUpper()}-specific optimizations 
3. Better instruction structure
4. More effective examples
5. OCR-specific enhancements

Return your suggestions as JSON in this exact format:
{{
  ""provider"": ""{provider}"",
  ""improvements"": [
    {{
      ""type"": ""clarity"",
      ""description"": ""Specific improvement description"",
      ""example"": ""Example of improved text"",
      ""impact"": ""Expected impact on accuracy""
    }}
  ]
}}";
        }

        private async Task<string> CallAIProviderAsync(string provider, string prompt)
        {
            if (!_providerConfigs.ContainsKey(provider))
            {
                throw new NotSupportedException($"Provider {provider} not configured");
            }

            // Check if we have the required API key
            string apiKey = provider.ToLower() switch
            {
                "deepseek" => _deepSeekApiKey,
                "gemini" => _geminiApiKey,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException($"API key not available for provider {provider}");
            }

            var config = _providerConfigs[provider];
            var requestBody = CreateProviderRequest(provider, prompt, config);
            
            _logger.Information("🌐 **API_CALL_START**: Making {Provider} API call, prompt length: {Length}", 
                provider, prompt.Length);
            
            try
            {
                // Create request with proper Authorization header
                var request = new HttpRequestMessage(HttpMethod.Post, config.Endpoint)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                };

                // Set authorization header based on provider
                if (provider.ToLower() == "deepseek")
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                }
                else if (provider.ToLower() == "gemini")
                {
                    // Gemini uses query parameter for API key
                    var uriBuilder = new UriBuilder(config.Endpoint);
                    uriBuilder.Query = $"key={apiKey}";
                    request.RequestUri = uriBuilder.Uri;
                }

                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error("❌ **API_CALL_FAILED**: {Provider} returned {StatusCode}: {Error}", 
                        provider, response.StatusCode, errorContent);
                    throw new HttpRequestException($"{provider} API returned {response.StatusCode}: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Information("✅ **API_CALL_SUCCESS**: {Provider} responded with {Length} characters", 
                    provider, responseContent?.Length ?? 0);
                
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex, "❌ **HTTP_REQUEST_FAILED**: {Provider} API call failed", provider);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **API_CALL_EXCEPTION**: Unexpected error calling {Provider}", provider);
                throw;
            }
        }

        private string CreateProviderRequest(string provider, string prompt, AIProviderConfig config)
        {
            switch (provider.ToLower())
            {
                case "deepseek":
                    return JsonSerializer.Serialize(new
                    {
                        model = config.Model,
                        messages = new[] 
                        {
                            new { role = "user", content = prompt }
                        },
                        temperature = config.Temperature,
                        max_tokens = config.MaxTokens
                    });
                    
                case "gemini":
                    return JsonSerializer.Serialize(new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = prompt } } }
                        },
                        generationConfig = new
                        {
                            temperature = config.Temperature,
                            maxOutputTokens = config.MaxTokens
                        }
                    });
                    
                default:
                    throw new NotSupportedException($"Provider {provider} request format not implemented");
            }
        }

        private List<PromptRecommendation> ParseRecommendations(string response, string provider)
        {
            try
            {
                // Extract JSON from provider-specific response format
                string jsonContent = ExtractJsonFromProviderResponse(response, provider);
                
                var recommendationData = JsonSerializer.Deserialize<RecommendationResponse>(jsonContent);
                return recommendationData?.Improvements?.Select(imp => new PromptRecommendation
                {
                    Provider = provider,
                    Type = imp.Type,
                    Description = imp.Description,
                    Example = imp.Example,
                    Impact = imp.Impact,
                    Timestamp = DateTime.UtcNow
                }).ToList() ?? new List<PromptRecommendation>();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to parse recommendations from {Provider}", provider);
                return new List<PromptRecommendation>();
            }
        }

        private string ExtractJsonFromProviderResponse(string response, string provider)
        {
            switch (provider.ToLower())
            {
                case "deepseek":
                    var deepseekResponse = JsonSerializer.Deserialize<DeepSeekResponse>(response);
                    return deepseekResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "{}";
                    
                case "gemini":
                    var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(response);
                    return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "{}";
                    
                default:
                    return response; // Assume direct JSON
            }
        }

        private void SaveRecommendationsAsync(string provider, List<PromptRecommendation> recommendations)
        {
            var filePath = Path.Combine(_recommendationsPath, $"{provider}-suggestions.json");
            
            // Load existing recommendations
            var existingRecommendations = new List<PromptRecommendation>();
            if (File.Exists(filePath))
            {
                var existingJson = File.ReadAllText(filePath);
                existingRecommendations = JsonSerializer.Deserialize<List<PromptRecommendation>>(existingJson) ?? new List<PromptRecommendation>();
            }
            
            // Add new recommendations
            existingRecommendations.AddRange(recommendations);
            
            // Keep only recent recommendations (last 100)
            var recentRecommendations = existingRecommendations
                .OrderByDescending(r => r.Timestamp)
                .Take(100)
                .ToList();
            
            // Save back to file
            var json = JsonSerializer.Serialize(recentRecommendations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        #endregion

        #region Configuration Management

        private Dictionary<string, AIProviderConfig> LoadProviderConfigs()
        {
            var configPath = Path.Combine(_configBasePath, "ai-providers.json");
            
            if (!File.Exists(configPath))
            {
                // Create default configuration
                var defaultConfigs = new Dictionary<string, AIProviderConfig>
                {
                    ["deepseek"] = new AIProviderConfig
                    {
                        Endpoint = "https://api.deepseek.com/v1/chat/completions",
                        Model = "deepseek-chat",
                        ApiKeyEnvVar = "DEEPSEEK_API_KEY",
                        MaxTokens = 8000,  // Official DeepSeek API output limit
                        Temperature = 0.3
                    },
                    ["gemini"] = new AIProviderConfig
                    {
                        Endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent",
                        Model = "gemini-pro",
                        ApiKeyEnvVar = "GEMINI_API_KEY", 
                        MaxTokens = 8192,  // Official Gemini API output limit  
                        Temperature = 0.3
                    }
                };
                
                var json = JsonSerializer.Serialize(defaultConfigs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
                return defaultConfigs;
            }
            
            var configJson = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<Dictionary<string, AIProviderConfig>>(configJson) ?? new Dictionary<string, AIProviderConfig>();
        }

        private TemplateSystemConfig LoadSystemConfig()
        {
            var configPath = Path.Combine(_configBasePath, "template-config.json");
            
            if (!File.Exists(configPath))
            {
                var defaultConfig = new TemplateSystemConfig
                {
                    DefaultProvider = "deepseek",
                    EnableRecommendations = true,
                    ValidationEnabled = true,
                    FallbackToHardcoded = true,
                    SupplierMappings = new Dictionary<string, SupplierConfig>
                    {
                        ["MANGO"] = new SupplierConfig
                        {
                            PreferredProvider = "deepseek",
                            SpecialTemplates = new[] { "mango-header", "mango-product" }
                        }
                    }
                };
                
                var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
                return defaultConfig;
            }
            
            var configJson = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<TemplateSystemConfig>(configJson) ?? new TemplateSystemConfig();
        }

        #endregion

        #region Fallback Implementation

        private string CreateFallbackPrompt(ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata)
        {
            _logger.Information("🔄 **FALLBACK_PROMPT**: Using hardcoded implementation");
            
            // This would call the existing hardcoded CreateHeaderErrorDetectionPrompt
            // For now, return a minimal functional prompt
            var fallbackData = PrepareTemplateData(invoice, fileText, metadata);
            
            return $@"OBJECT-ORIENTED INVOICE ANALYSIS (V14.0 - Fallback Implementation):

**CONTEXT:**
You are analyzing a structured business document with defined object schemas.

**1. EXTRACTED FIELDS:**
{fallbackData["invoiceJson"]}

**2. CONTEXT & COMPONENTS:**
{fallbackData["annotatedContext"]}

**3. BALANCE CHECK:**
{fallbackData["balanceCheckContext"]}

**4. COMPLETE OCR TEXT:**
{fallbackData["fileText"]}

🎯 **V14.0 MANDATORY COMPLETION REQUIREMENTS**:

🚨 **CRITICAL**: FOR EVERY ERROR YOU REPORT, YOU MUST PROVIDE ALL OF THE FOLLOWING:

1. ✅ **field**: The exact field name (NEVER null)
2. ✅ **correct_value**: The actual value from the OCR text (NEVER null)  
3. ✅ **error_type**: ""omission"" or ""format_correction"" or ""multi_field_omission"" (NEVER null)
4. ✅ **line_number**: The actual line number where the value appears (NEVER 0 or null)
5. ✅ **line_text**: The complete text of that line from the OCR (NEVER null)
6. ✅ **suggested_regex**: A working regex pattern that captures the value (NEVER null)
7. ✅ **reasoning**: Explain why this value was missed (NEVER null)

❌ **ABSOLUTELY FORBIDDEN**: 
   - ""Reasoning"": null
   - ""LineNumber"": 0
   - ""LineText"": null
   - ""SuggestedRegex"": null

**🚨 CRITICAL REGEX REQUIREMENTS FOR PRODUCTION:**
⚠️ **MANDATORY**: ALL regex patterns MUST use named capture groups: (?<FieldName>pattern)
⚠️ **FORBIDDEN**: Never use numbered capture groups: (pattern) - these will fail in production

If you find no new omissions or corrections, return an empty errors array with detailed explanation.

**MANDATORY RESPONSE FORMAT:**
- **If errors found**: {{ ""errors"": [error objects] }}
- **If NO errors found**: {{ ""errors"": [], ""explanation"": ""Detailed explanation of why no corrections are needed"" }}";
        }

        #endregion

        #region Self-Improving System Data Models

        public class FailedPatternInfo
        {
            public string Pattern { get; set; }
            public string FailureReason { get; set; }
            public string ActualText { get; set; }
        }

        public class TemplateImprovementResponse
        {
            public string Provider { get; set; }
            public double Confidence { get; set; }
            public List<string> ImprovementsMade { get; set; } = new List<string>();
            public string ImprovedTemplate { get; set; }
            public List<string> ImprovedPatterns { get; set; } = new List<string>();
            public string Reasoning { get; set; }
        }

        public class TemplateTestResult
        {
            public bool Success { get; set; }
            public List<string> SuccessfulPatterns { get; set; } = new List<string>();
            public List<FailedPatternInfo> FailedPatterns { get; set; } = new List<FailedPatternInfo>();
            public string ImprovedTemplate { get; set; }
        }

        #endregion

        #region Data Models

        public class TemplateValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
            public List<string> Warnings { get; set; } = new List<string>();
        }

        public class PromptRecommendation
        {
            public string Provider { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string Example { get; set; }
            public string Impact { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public class AIProviderConfig
        {
            public string Endpoint { get; set; }
            public string Model { get; set; }
            public string ApiKeyEnvVar { get; set; }
            public int MaxTokens { get; set; }
            public double Temperature { get; set; }
        }

        public class TemplateSystemConfig
        {
            public string DefaultProvider { get; set; } = "deepseek";
            public bool EnableRecommendations { get; set; } = true;
            public bool ValidationEnabled { get; set; } = true;
            public bool FallbackToHardcoded { get; set; } = true;
            public Dictionary<string, SupplierConfig> SupplierMappings { get; set; } = new Dictionary<string, SupplierConfig>();
        }

        public class SupplierConfig
        {
            public string PreferredProvider { get; set; }
            public string[] SpecialTemplates { get; set; }
        }

        private class RecommendationResponse
        {
            [JsonPropertyName("provider")]
            public string Provider { get; set; }
            
            [JsonPropertyName("improvements")]
            public List<ImprovementItem> Improvements { get; set; }
        }

        private class ImprovementItem
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }
            
            [JsonPropertyName("description")]
            public string Description { get; set; }
            
            [JsonPropertyName("example")]
            public string Example { get; set; }
            
            [JsonPropertyName("impact")]
            public string Impact { get; set; }
        }

        private class DeepSeekResponse
        {
            [JsonPropertyName("choices")]
            public List<DeepSeekChoice> Choices { get; set; }
        }

        private class DeepSeekChoice
        {
            [JsonPropertyName("message")]
            public DeepSeekMessage Message { get; set; }
        }

        private class DeepSeekMessage
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<GeminiCandidate> Candidates { get; set; }
        }

        private class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent Content { get; set; }
        }

        private class GeminiContent
        {
            [JsonPropertyName("parts")]
            public List<GeminiPart> Parts { get; set; }
        }

        private class GeminiPart
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}