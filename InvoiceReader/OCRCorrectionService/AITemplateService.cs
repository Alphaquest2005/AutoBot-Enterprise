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

        #region Constructor and Initialization

        public AITemplateService(ILogger logger, string basePath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
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
            
            _logger.Information("🚀 **AI_TEMPLATE_SERVICE_INITIALIZED**: Base path='{BasePath}', Providers={ProviderCount}", 
                rootPath, _providerConfigs.Count);
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
                var template = await LoadTemplateAsync(provider, "header-detection", invoice?.SupplierName);
                
                // 2. Validate template
                var validation = ValidateTemplate(template, provider);
                if (!validation.IsValid)
                {
                    _logger.Warning("⚠️ **TEMPLATE_INVALID**: {Errors}, falling back to hardcoded", 
                        string.Join("; ", validation.Errors));
                    return CreateFallbackPrompt(invoice, fileText, metadata);
                }

                // 3. Prepare template data (extract from existing prompt creation logic)
                var templateData = PrepareTemplateData(invoice, fileText, metadata);
                
                // 4. Render template with data
                var prompt = RenderTemplate(template, templateData);
                
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
                _logger.Error(ex, "❌ **AI_TEMPLATE_ERROR**: Failed for provider {Provider}, using fallback", provider);
                return CreateFallbackPrompt(invoice, fileText, metadata);
            }
        }

        /// <summary>
        /// Gets AI recommendations for improving a prompt from specified provider.
        /// </summary>
        public async Task<List<PromptRecommendation>> GetRecommendationsAsync(string prompt, string provider)
        {
            try
            {
                _logger.Information("🤖 **RECOMMENDATION_START**: Getting suggestions from {Provider}", provider);
                
                var metaPrompt = CreateRecommendationPrompt(prompt, provider);
                var response = await CallAIProviderAsync(provider, metaPrompt);
                var recommendations = ParseRecommendations(response, provider);
                
                await SaveRecommendationsAsync(provider, recommendations);
                
                _logger.Information("✅ **RECOMMENDATION_SUCCESS**: Saved {Count} suggestions for {Provider}", 
                    recommendations.Count, provider);
                
                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **RECOMMENDATION_FAIL**: Could not get suggestions from {Provider}", provider);
                return new List<PromptRecommendation>();
            }
        }

        #endregion

        #region Template Loading and Management

        private async Task<string> LoadTemplateAsync(string provider, string templateType, string supplierName)
        {
            // Provider-specific + supplier-specific template selection
            var templatePaths = new[]
            {
                // 1. Supplier-specific template for provider (e.g., deepseek/mango-header.txt)
                Path.Combine(_templateBasePath, provider, $"{supplierName?.ToLower()}-{templateType}.txt"),
                
                // 2. Standard template for provider (e.g., deepseek/header-detection.txt)
                Path.Combine(_templateBasePath, provider, $"{templateType}.txt"),
                
                // 3. Default fallback template (e.g., default/header-detection.txt)
                Path.Combine(_templateBasePath, "default", $"{templateType}.txt")
            };

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

        private TemplateValidationResult ValidateTemplate(string template, string provider)
        {
            var result = new TemplateValidationResult();
            
            if (string.IsNullOrWhiteSpace(template))
            {
                result.Errors.Add("Template is empty or null");
                return result;
            }
            
            if (template.Length < 200)
            {
                result.Errors.Add("Template too short (< 200 characters)");
                return result;
            }
            
            // Check for required template variables
            var requiredVariables = new[] { "{{invoiceJson}}", "{{fileText}}" };
            foreach (var variable in requiredVariables)
            {
                if (!template.Contains(variable))
                {
                    result.Errors.Add($"Missing required variable: {variable}");
                }
            }
            
            // Check for required sections
            var requiredSections = new[] { "EXTRACTED FIELDS", "CRITICAL", "COMPLETION REQUIREMENTS" };
            foreach (var section in requiredSections)
            {
                if (!template.Contains(section))
                {
                    result.Errors.Add($"Missing required section: {section}");
                }
            }
            
            // Provider-specific validation
            ValidateProviderSpecificRequirements(template, provider, result);
            
            result.IsValid = result.Errors.Count == 0;
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

            var config = _providerConfigs[provider];
            var requestBody = CreateProviderRequest(provider, prompt, config);
            
            try
            {
                var response = await _httpClient.PostAsync(config.Endpoint, 
                    new StringContent(requestBody, Encoding.UTF8, "application/json"));
                
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.Warning(ex, "HTTP request failed for provider {Provider}", provider);
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

        private async Task SaveRecommendationsAsync(string provider, List<PromptRecommendation> recommendations)
        {
            var filePath = Path.Combine(_recommendationsPath, $"{provider}-suggestions.json");
            
            // Load existing recommendations
            var existingRecommendations = new List<PromptRecommendation>();
            if (File.Exists(filePath))
            {
                var existingJson = await File.ReadAllTextAsync(filePath);
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
            await File.WriteAllTextAsync(filePath, json);
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
                        MaxTokens = 8192,
                        Temperature = 0.3
                    },
                    ["gemini"] = new AIProviderConfig
                    {
                        Endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent",
                        Model = "gemini-pro",
                        ApiKeyEnvVar = "GEMINI_API_KEY", 
                        MaxTokens = 8192,
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