// File: OCRCorrectionService/TemplateEngine/OCRTemplateService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using Serilog;
using WaterNut.DataSpace.MetaAI;

namespace WaterNut.DataSpace.TemplateEngine
{
    /// <summary>
    /// Integration bridge between the template engine and existing OCR prompt creation system.
    /// Provides seamless transition from hardcoded prompts to file-based templates with fallback support.
    /// </summary>
    public class OCRTemplateService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITemplateEngine _templateEngine;
        private readonly IMetaAIService _metaAIService;
        private readonly OCRTemplateServiceConfig _config;
        private readonly OCRCorrectionService _legacyService;

        // Template name mappings for existing OCR methods
        private readonly Dictionary<string, string> _templateMappings;

        public OCRTemplateService(
            ILogger logger,
            ITemplateEngine templateEngine,
            IMetaAIService metaAIService,
            OCRTemplateServiceConfig config,
            OCRCorrectionService legacyService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
            _metaAIService = metaAIService;
            _config = config ?? new OCRTemplateServiceConfig();
            _legacyService = legacyService ?? throw new ArgumentNullException(nameof(legacyService));

            _templateMappings = InitializeTemplateMappings();

            _logger.Information("🌉 **OCR_TEMPLATE_SERVICE_INITIALIZED**: Bridge service ready with {MappingCount} template mappings", 
                _templateMappings.Count);
        }

        #region Public Template Methods - Drop-in Replacements for Hardcoded Prompts

        /// <summary>
        /// Creates header error detection prompt using template engine with fallback to original implementation.
        /// Drop-in replacement for CreateHeaderErrorDetectionPrompt in OCRPromptCreation.cs.
        /// </summary>
        public async Task<string> CreateHeaderErrorDetectionPromptAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata)
        {
            _logger.Information("🏗️ **HEADER_PROMPT_REQUEST**: Creating header detection prompt for invoice '{InvoiceNo}' using template engine", 
                invoice?.InvoiceNo ?? "NULL");

            try
            {
                // Determine template based on supplier and invoice characteristics
                var templateName = DetermineHeaderTemplateForInvoice(invoice, fileText);
                
                if (await IsTemplateAvailableAsync(templateName))
                {
                    var context = CreateHeaderTemplateContext(invoice, fileText, metadata);
                    var templateResult = await _templateEngine.RenderAsync(templateName, context);

                    // Validate template result
                    if (ValidatePromptResult(templateResult, "header"))
                    {
                        _logger.Information("✅ **HEADER_TEMPLATE_SUCCESS**: Template '{TemplateName}' rendered {Length} characters", 
                            templateName, templateResult.Length);

                        // Optionally enhance with Meta AI if configured
                        if (_config.EnableMetaAIOptimization && _metaAIService != null)
                        {
                            return await OptimizePromptWithMetaAI(templateResult, "header_detection", invoice);
                        }

                        return templateResult;
                    }
                    else
                    {
                        _logger.Warning("⚠️ **HEADER_TEMPLATE_VALIDATION_FAILED**: Template result invalid, falling back to hardcoded");
                    }
                }
                else
                {
                    _logger.Information("📝 **HEADER_TEMPLATE_FALLBACK**: Template '{TemplateName}' not available, using hardcoded prompt", templateName);
                }

                // Fallback to original hardcoded implementation
                return await FallbackToHardcodedHeaderPrompt(invoice, fileText, metadata);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **HEADER_TEMPLATE_ERROR**: Template engine failed, falling back to hardcoded implementation");
                return await FallbackToHardcodedHeaderPrompt(invoice, fileText, metadata);
            }
        }

        /// <summary>
        /// Creates product error detection prompt using template engine with fallback.
        /// Drop-in replacement for CreateProductErrorDetectionPrompt in OCRPromptCreation.cs.
        /// </summary>
        public async Task<string> CreateProductErrorDetectionPromptAsync(
            ShipmentInvoice invoice,
            string fileText)
        {
            _logger.Information("🛒 **PRODUCT_PROMPT_REQUEST**: Creating product detection prompt for invoice '{InvoiceNo}' using template engine", 
                invoice?.InvoiceNo ?? "NULL");

            try
            {
                var templateName = DetermineProductTemplateForInvoice(invoice, fileText);
                
                if (await IsTemplateAvailableAsync(templateName))
                {
                    var context = CreateProductTemplateContext(invoice, fileText);
                    var templateResult = await _templateEngine.RenderAsync(templateName, context);

                    if (ValidatePromptResult(templateResult, "product"))
                    {
                        _logger.Information("✅ **PRODUCT_TEMPLATE_SUCCESS**: Template '{TemplateName}' rendered {Length} characters", 
                            templateName, templateResult.Length);

                        if (_config.EnableMetaAIOptimization && _metaAIService != null)
                        {
                            return await OptimizePromptWithMetaAI(templateResult, "product_detection", invoice);
                        }

                        return templateResult;
                    }
                    else
                    {
                        _logger.Warning("⚠️ **PRODUCT_TEMPLATE_VALIDATION_FAILED**: Template result invalid, falling back to hardcoded");
                    }
                }
                else
                {
                    _logger.Information("📝 **PRODUCT_TEMPLATE_FALLBACK**: Template '{TemplateName}' not available, using hardcoded prompt", templateName);
                }

                // Fallback to original hardcoded implementation
                return await FallbackToHardcodedProductPrompt(invoice, fileText);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PRODUCT_TEMPLATE_ERROR**: Template engine failed, falling back to hardcoded implementation");
                return await FallbackToHardcodedProductPrompt(invoice, fileText);
            }
        }

        /// <summary>
        /// Creates direct data correction prompt using template engine with fallback.
        /// Drop-in replacement for CreateDirectDataCorrectionPrompt in OCRPromptCreation.cs.
        /// </summary>
        public async Task<string> CreateDirectDataCorrectionPromptAsync(
            List<dynamic> invoiceDataList,
            string originalText)
        {
            _logger.Information("🔧 **CORRECTION_PROMPT_REQUEST**: Creating direct correction prompt using template engine");

            try
            {
                var templateName = "direct-correction-base";
                
                if (await IsTemplateAvailableAsync(templateName))
                {
                    var context = CreateCorrectionTemplateContext(invoiceDataList, originalText);
                    var templateResult = await _templateEngine.RenderAsync(templateName, context);

                    if (ValidatePromptResult(templateResult, "correction"))
                    {
                        _logger.Information("✅ **CORRECTION_TEMPLATE_SUCCESS**: Template rendered {Length} characters", templateResult.Length);

                        if (_config.EnableMetaAIOptimization && _metaAIService != null)
                        {
                            return await OptimizePromptWithMetaAI(templateResult, "direct_correction", invoiceDataList?.FirstOrDefault());
                        }

                        return templateResult;
                    }
                    else
                    {
                        _logger.Warning("⚠️ **CORRECTION_TEMPLATE_VALIDATION_FAILED**: Template result invalid, falling back to hardcoded");
                    }
                }

                // Fallback to original hardcoded implementation
                return await FallbackToHardcodedCorrectionPrompt(invoiceDataList, originalText);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CORRECTION_TEMPLATE_ERROR**: Template engine failed, falling back to hardcoded implementation");
                return await FallbackToHardcodedCorrectionPrompt(invoiceDataList, originalText);
            }
        }

        #endregion

        #region Template Management and Meta AI Integration

        /// <summary>
        /// Optimizes existing template using Meta AI recommendations and applies changes automatically.
        /// </summary>
        public async Task<TemplateOptimizationResult> OptimizeTemplateWithMetaAI(
            string templateName,
            PromptPerformanceMetrics performanceMetrics,
            List<string> failureExamples = null,
            List<string> successExamples = null)
        {
            _logger.Information("🤖 **META_AI_OPTIMIZATION_REQUEST**: Optimizing template '{TemplateName}' with Meta AI", templateName);

            var result = new TemplateOptimizationResult
            {
                TemplateName = templateName,
                Success = false
            };

            try
            {
                if (_metaAIService == null)
                {
                    result.Message = "Meta AI service not available";
                    return result;
                }

                // Get current template content
                var availableTemplates = await _templateEngine.GetAvailableTemplatesAsync();
                var currentTemplate = availableTemplates.FirstOrDefault(t => t.Name == templateName);
                
                if (currentTemplate == null)
                {
                    result.Message = $"Template '{templateName}' not found";
                    return result;
                }

                var currentPrompt = await File.ReadAllTextAsync(currentTemplate.Path);

                // Request recommendations from Meta AI
                var recommendationRequest = new PromptRecommendationRequest
                {
                    CurrentPrompt = currentPrompt,
                    PromptCategory = DeterminePromptCategory(templateName),
                    PerformanceMetrics = performanceMetrics,
                    FailureExamples = failureExamples ?? new List<string>(),
                    SuccessExamples = successExamples ?? new List<string>(),
                    Goals = new PromptOptimizationGoals
                    {
                        ImproveAccuracy = true,
                        IncreaseConsistency = true,
                        ImproveClarity = true,
                        ReduceHallucination = true
                    }
                };

                var recommendations = await _metaAIService.GetPromptRecommendationsAsync(recommendationRequest);

                if (recommendations.Success && recommendations.Recommendations.Any())
                {
                    // Apply auto-implementable recommendations
                    var autoRecommendations = recommendations.Recommendations
                        .Where(r => r.AutoImplementable && r.Confidence >= ConfidenceLevel.Medium)
                        .ToList();

                    if (autoRecommendations.Any())
                    {
                        // Create backup before modification
                        var backupId = await _templateEngine.BackupTemplateAsync(templateName);
                        result.BackupId = backupId;

                        // Apply recommendations automatically
                        var implementationRequest = new AutoImplementationRequest
                        {
                            TemplateName = templateName,
                            Recommendations = autoRecommendations,
                            BackupStrategy = "create_backup",
                            RequireValidation = true,
                            EnableRollback = true
                        };

                        var implementationResult = await _metaAIService.AutoImplementRecommendationsAsync(implementationRequest);

                        if (implementationResult.Success)
                        {
                            result.Success = true;
                            result.Message = $"Successfully applied {autoRecommendations.Count} Meta AI recommendations";
                            result.RecommendationsApplied = autoRecommendations.Count;
                            result.AppliedRecommendations = autoRecommendations;

                            _logger.Information("✅ **META_AI_OPTIMIZATION_SUCCESS**: Applied {Count} recommendations to template '{TemplateName}'", 
                                autoRecommendations.Count, templateName);
                        }
                        else
                        {
                            result.Message = $"Implementation failed: {implementationResult.Message}";
                            result.Errors = implementationResult.Errors?.Select(e => e.Message).ToList();

                            // Attempt rollback
                            if (!string.IsNullOrEmpty(backupId))
                            {
                                await _templateEngine.RestoreTemplateAsync(templateName, backupId);
                                _logger.Warning("🔄 **ROLLBACK_APPLIED**: Restored template '{TemplateName}' from backup '{BackupId}'", templateName, backupId);
                            }
                        }
                    }
                    else
                    {
                        result.Message = "No auto-implementable recommendations available";
                        result.AllRecommendations = recommendations.Recommendations;
                    }
                }
                else
                {
                    result.Message = recommendations.Message ?? "No recommendations received from Meta AI";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **META_AI_OPTIMIZATION_ERROR**: Failed to optimize template '{TemplateName}' with Meta AI", templateName);
                result.Message = $"Optimization failed: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Gets template usage statistics and performance metrics for Meta AI analysis.
        /// </summary>
        public async Task<TemplatePerformanceReport> GetTemplatePerformanceReportAsync(
            string templateName,
            TimeSpan reportPeriod)
        {
            _logger.Information("📊 **PERFORMANCE_REPORT_REQUEST**: Generating performance report for template '{TemplateName}'", templateName);

            var report = new TemplatePerformanceReport
            {
                TemplateName = templateName,
                ReportPeriod = reportPeriod,
                GeneratedAt = DateTime.UtcNow
            };

            try
            {
                // This would integrate with actual usage tracking
                // For now, we'll provide a framework for collecting metrics
                
                report.UsageCount = await GetTemplateUsageCountAsync(templateName, reportPeriod);
                report.AverageRenderTime = await GetAverageRenderTimeAsync(templateName, reportPeriod);
                report.SuccessRate = await GetTemplateSuccessRateAsync(templateName, reportPeriod);
                report.CommonFailurePatterns = await GetCommonFailurePatternsAsync(templateName, reportPeriod);
                
                _logger.Information("📊 **PERFORMANCE_REPORT_SUCCESS**: Report generated for template '{TemplateName}' - Usage: {Usage}, Success Rate: {SuccessRate}%", 
                    templateName, report.UsageCount, report.SuccessRate * 100);

                return report;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PERFORMANCE_REPORT_ERROR**: Failed to generate performance report for template '{TemplateName}'", templateName);
                report.Error = ex.Message;
                return report;
            }
        }

        #endregion

        #region Template Context Creation

        private TemplateContext CreateHeaderTemplateContext(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata)
        {
            var context = new TemplateContext();
            
            // Core OCR data
            context.SetInvoiceData(invoice);
            context.SetFileText(fileText);
            context.SetMetadata(metadata);

            // Additional context from original implementation
            context.Variables["ocrSections"] = AnalyzeOCRSections(fileText);
            context.Variables["balanceCheckContext"] = CreateBalanceCheckContext(invoice);
            context.Variables["annotatedContext"] = CreateAnnotatedContext(metadata);
            context.Variables["currentValues"] = CreateCurrentValuesDict(invoice);

            // Template-specific helpers
            context.Variables["supplier"] = new { SupplierName = invoice?.SupplierName ?? "Unknown" };
            context.Variables["timestamp"] = DateTime.UtcNow;

            return context;
        }

        private TemplateContext CreateProductTemplateContext(
            ShipmentInvoice invoice,
            string fileText)
        {
            var context = new TemplateContext();
            
            context.SetInvoiceData(invoice);
            context.SetFileText(fileText);

            // Product-specific context
            var productData = invoice?.InvoiceDetails?.Select(d => new
            {
                InputLineNumber = d.LineNumber,
                d.ItemDescription,
                d.Quantity,
                UnitCost = d.Cost,
                LineTotal = d.TotalCost,
                d.Discount,
                d.Units
            }).ToList();

            context.Variables["productData"] = productData;
            context.Variables["ocrSections"] = AnalyzeOCRSections(fileText);
            context.Variables["supplier"] = new { SupplierName = invoice?.SupplierName ?? "Unknown" };

            return context;
        }

        private TemplateContext CreateCorrectionTemplateContext(
            List<dynamic> invoiceDataList,
            string originalText)
        {
            var context = new TemplateContext();
            
            var invoiceDataToSerialize = invoiceDataList?.FirstOrDefault() ?? new Dictionary<string, object>();
            context.Variables["invoiceData"] = invoiceDataToSerialize;
            context.Variables["originalText"] = originalText;
            context.Variables["timestamp"] = DateTime.UtcNow;

            return context;
        }

        #endregion

        #region Template Selection Logic

        private string DetermineHeaderTemplateForInvoice(ShipmentInvoice invoice, string fileText)
        {
            var supplierName = invoice?.SupplierName?.ToUpper() ?? "";
            
            // Supplier-specific templates
            if (supplierName.Contains("MANGO"))
            {
                return "mango-header-detection";
            }
            else if (supplierName.Contains("AMAZON"))
            {
                return "amazon-header-detection";
            }
            else if (IsEuropeanSupplier(supplierName, fileText))
            {
                return "european-header-detection";
            }
            
            // Default template
            return "base-header-detection";
        }

        private string DetermineProductTemplateForInvoice(ShipmentInvoice invoice, string fileText)
        {
            var supplierName = invoice?.SupplierName?.ToUpper() ?? "";
            
            // Check for multi-field requirements
            if (HasMultiFieldProducts(fileText))
            {
                if (supplierName.Contains("MANGO"))
                {
                    return "mango-product-detection";
                }
                return "multi-field-product-detection";
            }
            
            return "base-product-detection";
        }

        private bool IsEuropeanSupplier(string supplierName, string fileText)
        {
            var europeanIndicators = new[] { "EUR", "€", "VAT", "TVA", "IVA" };
            return europeanIndicators.Any(indicator => fileText.Contains(indicator));
        }

        private bool HasMultiFieldProducts(string fileText)
        {
            // Detect if product lines span multiple lines or have complex structure
            return fileText.Contains("ref.") || fileText.Contains("SKU:") || fileText.Contains("\n") && fileText.Length > 1000;
        }

        #endregion

        #region Fallback Methods

        private async Task<string> FallbackToHardcodedHeaderPrompt(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata)
        {
            _logger.Information("🔄 **FALLBACK_HEADER**: Using hardcoded header prompt creation");
            return await Task.Run(() => _legacyService.CreateHeaderErrorDetectionPrompt(invoice, fileText, metadata));
        }

        private async Task<string> FallbackToHardcodedProductPrompt(
            ShipmentInvoice invoice,
            string fileText)
        {
            _logger.Information("🔄 **FALLBACK_PRODUCT**: Using hardcoded product prompt creation");
            return await Task.Run(() => _legacyService.CreateProductErrorDetectionPrompt(invoice, fileText));
        }

        private async Task<string> FallbackToHardcodedCorrectionPrompt(
            List<dynamic> invoiceDataList,
            string originalText)
        {
            _logger.Information("🔄 **FALLBACK_CORRECTION**: Using hardcoded correction prompt creation");
            return await Task.Run(() => _legacyService.CreateDirectDataCorrectionPrompt(invoiceDataList, originalText));
        }

        #endregion

        #region Helper Methods

        private Dictionary<string, string> InitializeTemplateMappings()
        {
            return new Dictionary<string, string>
            {
                ["CreateHeaderErrorDetectionPrompt"] = "base-header-detection",
                ["CreateProductErrorDetectionPrompt"] = "base-product-detection", 
                ["CreateDirectDataCorrectionPrompt"] = "direct-correction-base",
                ["CreateRegexCreationPrompt"] = "regex-creation-base",
                ["CreateRegexCorrectionPrompt"] = "regex-correction-base"
            };
        }

        private async Task<bool> IsTemplateAvailableAsync(string templateName)
        {
            try
            {
                var availableTemplates = await _templateEngine.GetAvailableTemplatesAsync();
                return availableTemplates.Any(t => t.Name == templateName && t.IsActive);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **TEMPLATE_AVAILABILITY_CHECK_FAILED**: Could not check availability of template '{TemplateName}'", templateName);
                return false;
            }
        }

        private bool ValidatePromptResult(string prompt, string promptType)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return false;

            // Basic validation requirements
            if (prompt.Length < 100)
                return false;

            // OCR-specific validation
            if (promptType == "header" || promptType == "product")
            {
                if (!prompt.Contains("field") || !prompt.Contains("correct_value"))
                    return false;
            }

            // Check for unresolved template variables
            if (prompt.Contains("{{") || prompt.Contains(" is undefined"))
                return false;

            return true;
        }

        private async Task<string> OptimizePromptWithMetaAI(string prompt, string category, object contextData)
        {
            if (_metaAIService == null || !_config.EnableMetaAIOptimization)
                return prompt;

            try
            {
                var optimizationRequest = new PromptOptimizationRequest
                {
                    OriginalPrompt = prompt,
                    TargetModel = "DeepSeek",
                    Goals = new PromptOptimizationGoals
                    {
                        ImproveAccuracy = true,
                        IncreaseConsistency = true,
                        ImproveClarity = true
                    }
                };

                var result = await _metaAIService.OptimizePromptAsync(optimizationRequest);
                
                if (result.Success && !string.IsNullOrEmpty(result.OptimizedPrompt))
                {
                    _logger.Information("🤖 **META_AI_OPTIMIZED**: Prompt optimized from {OriginalLength} to {OptimizedLength} characters", 
                        prompt.Length, result.OptimizedPrompt.Length);
                    return result.OptimizedPrompt;
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **META_AI_OPTIMIZATION_FAILED**: Using original prompt");
            }

            return prompt;
        }

        private string DeterminePromptCategory(string templateName)
        {
            if (templateName.Contains("header"))
                return "header_detection";
            else if (templateName.Contains("product"))
                return "product_detection";
            else if (templateName.Contains("correction"))
                return "direct_correction";
            else
                return "general";
        }

        // Helper methods from original implementation
        private List<string> AnalyzeOCRSections(string fileText)
        {
            var sections = new List<string>();
            if (string.IsNullOrEmpty(fileText)) return sections;

            if (fileText.IndexOf("Single Column", StringComparison.OrdinalIgnoreCase) >= 0) sections.Add("Single Column");
            if (fileText.IndexOf("Ripped", StringComparison.OrdinalIgnoreCase) >= 0) sections.Add("Ripped");
            if (fileText.IndexOf("SparseText", StringComparison.OrdinalIgnoreCase) >= 0) sections.Add("SparseText");
            if (sections.Count == 0) sections.Add("Multiple OCR Methods");

            return sections;
        }

        private string CreateBalanceCheckContext(ShipmentInvoice invoice)
        {
            if (invoice == null) return "";

            double subTotal = invoice.SubTotal ?? 0;
            double freight = invoice.TotalInternalFreight ?? 0;
            double otherCost = invoice.TotalOtherCost ?? 0;
            double deduction = invoice.TotalDeduction ?? 0;
            double insurance = invoice.TotalInsurance ?? 0;
            double reportedTotal = invoice.InvoiceTotal ?? 0;
            double calculatedTotal = subTotal + freight + otherCost + insurance - deduction;
            double discrepancy = reportedTotal - calculatedTotal;

            return $"Calculated total: {calculatedTotal:F2}, Reported total: {reportedTotal:F2}, Discrepancy: {discrepancy:F2}";
        }

        private string CreateAnnotatedContext(Dictionary<string, OCRFieldMetadata> metadata)
        {
            if (metadata == null || !metadata.Any())
                return "";

            var annotated = new System.Text.StringBuilder();
            var groupedFields = metadata.Values
                .Where(m => m != null && !string.IsNullOrEmpty(m.Field))
                .GroupBy(m => m.Field);

            foreach (var group in groupedFields)
            {
                if (group.Count() > 1)
                {
                    annotated.AppendLine($"Field '{group.Key}' found in {group.Count()} locations");
                }
            }

            return annotated.ToString();
        }

        private Dictionary<string, object> CreateCurrentValuesDict(ShipmentInvoice invoice)
        {
            if (invoice == null) return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                ["InvoiceNo"] = invoice.InvoiceNo,
                ["InvoiceDate"] = invoice.InvoiceDate,
                ["SupplierName"] = invoice.SupplierName,
                ["Currency"] = invoice.Currency,
                ["SubTotal"] = invoice.SubTotal,
                ["InvoiceTotal"] = invoice.InvoiceTotal
            };
        }

        // Performance tracking methods (framework for future implementation)
        private async Task<int> GetTemplateUsageCountAsync(string templateName, TimeSpan period)
        {
            // This would integrate with actual usage tracking
            return await Task.FromResult(0);
        }

        private async Task<double> GetAverageRenderTimeAsync(string templateName, TimeSpan period)
        {
            // This would integrate with actual performance tracking
            return await Task.FromResult(0.0);
        }

        private async Task<double> GetTemplateSuccessRateAsync(string templateName, TimeSpan period)
        {
            // This would integrate with actual success tracking
            return await Task.FromResult(1.0);
        }

        private async Task<List<string>> GetCommonFailurePatternsAsync(string templateName, TimeSpan period)
        {
            // This would integrate with actual failure pattern analysis
            return await Task.FromResult(new List<string>());
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _templateEngine?.Dispose();
            _logger?.Information("🧹 **OCR_TEMPLATE_SERVICE_DISPOSED**: Service disposed successfully");
        }

        #endregion
    }

    #region Configuration and Result Classes

    /// <summary>
    /// Configuration for OCR template service.
    /// </summary>
    public class OCRTemplateServiceConfig
    {
        public bool EnableMetaAIOptimization { get; set; } = false;
        public bool EnableFallbackToHardcoded { get; set; } = true;
        public bool EnableTemplateValidation { get; set; } = true;
        public bool EnablePerformanceTracking { get; set; } = true;
        public TimeSpan TemplateLoadTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan PromptRenderTimeout { get; set; } = TimeSpan.FromMinutes(2);
    }

    /// <summary>
    /// Result of template optimization with Meta AI.
    /// </summary>
    public class TemplateOptimizationResult
    {
        public string TemplateName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string BackupId { get; set; }
        public int RecommendationsApplied { get; set; }
        public List<PromptRecommendation> AppliedRecommendations { get; set; } = new List<PromptRecommendation>();
        public List<PromptRecommendation> AllRecommendations { get; set; } = new List<PromptRecommendation>();
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime OptimizedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Performance report for template usage analysis.
    /// </summary>
    public class TemplatePerformanceReport
    {
        public string TemplateName { get; set; }
        public TimeSpan ReportPeriod { get; set; }
        public DateTime GeneratedAt { get; set; }
        public int UsageCount { get; set; }
        public double AverageRenderTime { get; set; }
        public double SuccessRate { get; set; }
        public List<string> CommonFailurePatterns { get; set; } = new List<string>();
        public string Error { get; set; }
    }

    #endregion
}