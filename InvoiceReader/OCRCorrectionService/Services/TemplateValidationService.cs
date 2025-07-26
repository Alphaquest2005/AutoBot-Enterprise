// File: OCRCorrectionService/Services/TemplateValidationService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaterNut.DataSpace.TemplateEngine;
using WaterNut.DataSpace.MetaAI;
using WaterNut.DataSpace.AutoImplementation;
using Serilog;

namespace WaterNut.DataSpace.Services
{
    /// <summary>
    /// Service for validating template implementations after applying Meta AI recommendations.
    /// Provides comprehensive validation including syntax, functionality, and business logic checks.
    /// CRITICAL: All validation operates within OCRCorrectionService directory structure.
    /// </summary>
    public class TemplateValidationService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITemplateEngine _templateEngine;
        private readonly TimeSpan _validationTimeout;

        public TemplateValidationService(ILogger logger, ITemplateEngine templateEngine, TimeSpan? validationTimeout = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
            _validationTimeout = validationTimeout ?? TimeSpan.FromMinutes(2);

            _logger.Information("🔍 **TEMPLATE_VALIDATION_SERVICE_INIT**: Service initialized with timeout={TimeoutMinutes}min",
                _validationTimeout.TotalMinutes);
        }

        #region Core Validation Methods

        /// <summary>
        /// Validates a template implementation after applying recommendations.
        /// Performs comprehensive checks including syntax, functionality, and business logic.
        /// </summary>
        public async Task<TemplateImplementationValidationResult> ValidateTemplateImplementationAsync(
            string templateName, 
            List<PromptRecommendation> appliedRecommendations)
        {
            _logger.Information("🔍 **TEMPLATE_VALIDATION_START**: Validating '{TemplateName}' after {RecommendationCount} recommendations",
                templateName, appliedRecommendations?.Count ?? 0);

            var result = new TemplateImplementationValidationResult
            {
                TemplateName = templateName,
                ValidationStartTime = DateTime.UtcNow,
                ValidationErrors = new List<string>(),
                ValidationWarnings = new List<string>(),
                ValidationDetails = new Dictionary<string, object>()
            };

            try
            {
                // Step 1: Syntax Validation
                var syntaxValidation = await ValidateTemplateSyntaxAsync(templateName);
                result.SyntaxValid = syntaxValidation.IsValid;
                if (!syntaxValidation.IsValid)
                {
                    result.ValidationErrors.AddRange(syntaxValidation.Errors);
                }
                result.ValidationWarnings.AddRange(syntaxValidation.Warnings);
                result.ValidationDetails["SyntaxValidation"] = syntaxValidation;

                // Step 2: Compilation Validation
                var compilationValidation = await ValidateTemplateCompilationAsync(templateName);
                result.CompilationValid = compilationValidation.IsValid;
                if (!compilationValidation.IsValid)
                {
                    result.ValidationErrors.AddRange(compilationValidation.Errors);
                }
                result.ValidationWarnings.AddRange(compilationValidation.Warnings);
                result.ValidationDetails["CompilationValidation"] = compilationValidation;

                // Step 3: Functional Validation (if syntax and compilation pass)
                if (result.SyntaxValid && result.CompilationValid)
                {
                    var functionalValidation = await ValidateTemplateFunctionalityAsync(templateName);
                    result.FunctionalValid = functionalValidation.IsValid;
                    if (!functionalValidation.IsValid)
                    {
                        result.ValidationErrors.AddRange(functionalValidation.Errors);
                    }
                    result.ValidationWarnings.AddRange(functionalValidation.Warnings);
                    result.ValidationDetails["FunctionalValidation"] = functionalValidation;
                }

                // Step 4: Business Logic Validation
                var businessValidation = await ValidateBusinessLogicAsync(templateName, appliedRecommendations);
                result.BusinessLogicValid = businessValidation.IsValid;
                if (!businessValidation.IsValid)
                {
                    result.ValidationErrors.AddRange(businessValidation.Errors);
                }
                result.ValidationWarnings.AddRange(businessValidation.Warnings);
                result.ValidationDetails["BusinessValidation"] = businessValidation;

                // Step 5: Performance Validation
                var performanceValidation = await ValidateTemplatePerformanceAsync(templateName);
                result.PerformanceValid = performanceValidation.IsValid;
                if (!performanceValidation.IsValid)
                {
                    result.ValidationErrors.AddRange(performanceValidation.Errors);
                }
                result.ValidationWarnings.AddRange(performanceValidation.Warnings);
                result.ValidationDetails["PerformanceValidation"] = performanceValidation;

                // Overall validation result
                result.IsValid = result.SyntaxValid && 
                                result.CompilationValid && 
                                result.FunctionalValid && 
                                result.BusinessLogicValid && 
                                result.PerformanceValid;

                result.ValidationEndTime = DateTime.UtcNow;
                result.ValidationDuration = result.ValidationEndTime - result.ValidationStartTime;

                if (result.IsValid)
                {
                    _logger.Information("✅ **TEMPLATE_VALIDATION_SUCCESS**: Template '{TemplateName}' passed all validation checks in {Duration}ms",
                        templateName, result.ValidationDuration.TotalMilliseconds);
                }
                else
                {
                    _logger.Warning("⚠️ **TEMPLATE_VALIDATION_FAILED**: Template '{TemplateName}' failed validation with {ErrorCount} errors, {WarningCount} warnings",
                        templateName, result.ValidationErrors.Count, result.ValidationWarnings.Count);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_VALIDATION_ERROR**: Critical error during template validation");
                
                result.IsValid = false;
                result.ValidationErrors.Add($"Validation failed with exception: {ex.Message}");
                result.ValidationEndTime = DateTime.UtcNow;
                result.ValidationDuration = result.ValidationEndTime - result.ValidationStartTime;
                
                return result;
            }
        }

        /// <summary>
        /// Validates a batch of templates for consistency and compatibility.
        /// </summary>
        public async Task<BatchValidationResult> ValidateTemplateBatchAsync(List<string> templateNames)
        {
            _logger.Information("🔍 **BATCH_VALIDATION_START**: Validating {TemplateCount} templates for consistency",
                templateNames.Count);

            var result = new BatchValidationResult
            {
                TotalTemplates = templateNames.Count,
                ValidationResults = new List<TemplateImplementationValidationResult>(),
                BatchValidationErrors = new List<string>(),
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Validate each template individually
                foreach (var templateName in templateNames)
                {
                    var individualResult = await ValidateTemplateImplementationAsync(templateName, new List<PromptRecommendation>());
                    result.ValidationResults.Add(individualResult);
                }

                // Cross-template validation
                var crossValidation = await ValidateCrossTemplateConsistencyAsync(templateNames);
                result.BatchValidationErrors.AddRange(crossValidation.Errors);

                // Summary statistics
                result.ValidTemplates = result.ValidationResults.Count(r => r.IsValid);
                result.InvalidTemplates = result.ValidationResults.Count(r => !r.IsValid);
                result.Success = result.InvalidTemplates == 0 && !result.BatchValidationErrors.Any();

                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime - result.StartTime;

                _logger.Information("✅ **BATCH_VALIDATION_COMPLETE**: {ValidCount}/{TotalCount} templates valid in {Duration}ms",
                    result.ValidTemplates, result.TotalTemplates, result.Duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **BATCH_VALIDATION_ERROR**: Critical error during batch validation");
                
                result.Success = false;
                result.BatchValidationErrors.Add($"Batch validation failed: {ex.Message}");
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime - result.StartTime;
                
                return result;
            }
        }

        #endregion

        #region Individual Validation Methods

        /// <summary>
        /// Validates template syntax including Handlebars expressions and structure.
        /// </summary>
        private async Task<ValidationStepResult> ValidateTemplateSyntaxAsync(string templateName)
        {
            var result = new ValidationStepResult { StepName = "Syntax Validation" };

            try
            {
                // Get template content
                var templateContent = await GetTemplateContentAsync(templateName);
                if (string.IsNullOrEmpty(templateContent))
                {
                    result.Errors.Add("Template content is empty or could not be loaded");
                    result.IsValid = false;
                    return result;
                }

                // Check for balanced Handlebars expressions
                var handlebarsErrors = ValidateHandlebarsExpressions(templateContent);
                result.Errors.AddRange(handlebarsErrors);

                // Check for required template sections
                var structureErrors = ValidateTemplateStructure(templateContent);
                result.Errors.AddRange(structureErrors);

                // Check for proper escaping
                var escapingWarnings = ValidateEscaping(templateContent);
                result.Warnings.AddRange(escapingWarnings);

                // Check for common syntax issues
                var commonIssues = ValidateCommonSyntaxIssues(templateContent);
                result.Errors.AddRange(commonIssues);

                result.IsValid = !result.Errors.Any();
                result.Details = new { ContentLength = templateContent.Length, LineCount = templateContent.Split('\n').Length };

                _logger.Verbose("🔍 **SYNTAX_VALIDATION**: Template '{TemplateName}' syntax validation {Status}",
                    templateName, result.IsValid ? "PASSED" : "FAILED");

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Syntax validation failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Validates template compilation using the template engine.
        /// </summary>
        private async Task<ValidationStepResult> ValidateTemplateCompilationAsync(string templateName)
        {
            var result = new ValidationStepResult { StepName = "Compilation Validation" };

            try
            {
                // Attempt to load and compile template
                var templates = await _templateEngine.GetAvailableTemplatesAsync();
                var template = templates.FirstOrDefault(t => t.Name == templateName);

                if (template == null)
                {
                    result.Errors.Add($"Template '{templateName}' not found in template engine");
                    result.IsValid = false;
                    return result;
                }

                // Try to compile with sample data
                var sampleContext = CreateSampleTemplateContext();
                var compilationSuccess = await TryCompileTemplateAsync(template, sampleContext);
                
                if (!compilationSuccess.Success)
                {
                    result.Errors.Add($"Template compilation failed: {compilationSuccess.ErrorMessage}");
                    result.IsValid = false;
                }
                else
                {
                    result.IsValid = true;
                    result.Details = new { CompiledLength = compilationSuccess.Result?.Length ?? 0 };
                }

                _logger.Verbose("🔍 **COMPILATION_VALIDATION**: Template '{TemplateName}' compilation validation {Status}",
                    templateName, result.IsValid ? "PASSED" : "FAILED");

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Compilation validation failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Validates template functionality by rendering with test data.
        /// </summary>
        private async Task<ValidationStepResult> ValidateTemplateFunctionalityAsync(string templateName)
        {
            var result = new ValidationStepResult { StepName = "Functional Validation" };

            try
            {
                // Get template
                var templates = await _templateEngine.GetAvailableTemplatesAsync();
                var template = templates.FirstOrDefault(t => t.Name == templateName);

                if (template == null)
                {
                    result.Errors.Add($"Template '{templateName}' not found for functional testing");
                    result.IsValid = false;
                    return result;
                }

                // Test with multiple data scenarios
                var testScenarios = CreateTestScenarios();
                var functionalTestResults = new List<FunctionalTestResult>();

                foreach (var scenario in testScenarios)
                {
                    var testResult = await TestTemplateWithScenario(template, scenario);
                    functionalTestResults.Add(testResult);

                    if (!testResult.Success)
                    {
                        result.Errors.Add($"Functional test '{scenario.Name}' failed: {testResult.ErrorMessage}");
                    }
                }

                result.IsValid = functionalTestResults.All(t => t.Success);
                result.Details = new { TestScenarios = functionalTestResults.Count, PassedTests = functionalTestResults.Count(t => t.Success) };

                _logger.Verbose("🔍 **FUNCTIONAL_VALIDATION**: Template '{TemplateName}' functional validation {Status} ({PassedCount}/{TotalCount})",
                    templateName, result.IsValid ? "PASSED" : "FAILED", 
                    functionalTestResults.Count(t => t.Success), functionalTestResults.Count);

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Functional validation failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Validates business logic and OCR-specific requirements.
        /// </summary>
        private async Task<ValidationStepResult> ValidateBusinessLogicAsync(string templateName, List<PromptRecommendation> appliedRecommendations)
        {
            var result = new ValidationStepResult { StepName = "Business Logic Validation" };

            try
            {
                var templateContent = await GetTemplateContentAsync(templateName);
                if (string.IsNullOrEmpty(templateContent))
                {
                    result.Errors.Add("Template content not available for business logic validation");
                    result.IsValid = false;
                    return result;
                }

                // Validate OCR-specific requirements
                var ocrValidation = ValidateOCRRequirements(templateContent);
                result.Errors.AddRange(ocrValidation.Errors);
                result.Warnings.AddRange(ocrValidation.Warnings);

                // Validate applied recommendations don't conflict
                if (appliedRecommendations?.Any() == true)
                {
                    var recommendationValidation = ValidateRecommendationConsistency(templateContent, appliedRecommendations);
                    result.Errors.AddRange(recommendationValidation.Errors);
                    result.Warnings.AddRange(recommendationValidation.Warnings);
                }

                // Validate business rules
                var businessRulesValidation = ValidateBusinessRules(templateContent, templateName);
                result.Errors.AddRange(businessRulesValidation.Errors);
                result.Warnings.AddRange(businessRulesValidation.Warnings);

                result.IsValid = !result.Errors.Any();
                result.Details = new { AppliedRecommendations = appliedRecommendations?.Count ?? 0 };

                _logger.Verbose("🔍 **BUSINESS_LOGIC_VALIDATION**: Template '{TemplateName}' business logic validation {Status}",
                    templateName, result.IsValid ? "PASSED" : "FAILED");

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Business logic validation failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Validates template performance characteristics.
        /// </summary>
        private async Task<ValidationStepResult> ValidateTemplatePerformanceAsync(string templateName)
        {
            var result = new ValidationStepResult { StepName = "Performance Validation" };

            try
            {
                var performanceMetrics = await MeasureTemplatePerformanceAsync(templateName);
                
                // Check performance thresholds
                if (performanceMetrics.RenderTime > TimeSpan.FromSeconds(30))
                {
                    result.Warnings.Add($"Template render time ({performanceMetrics.RenderTime.TotalSeconds:F2}s) exceeds recommended threshold");
                }

                if (performanceMetrics.MemoryUsage > 50 * 1024 * 1024) // 50MB
                {
                    result.Warnings.Add($"Template memory usage ({performanceMetrics.MemoryUsage / 1024 / 1024:F2}MB) is high");
                }

                if (performanceMetrics.OutputSize > 1024 * 1024) // 1MB
                {
                    result.Warnings.Add($"Template output size ({performanceMetrics.OutputSize / 1024:F2}KB) is large");
                }

                result.IsValid = !result.Errors.Any(); // Performance issues are warnings, not errors
                result.Details = new { PerformanceMetrics = performanceMetrics };

                _logger.Verbose("🔍 **PERFORMANCE_VALIDATION**: Template '{TemplateName}' performance validation completed",
                    templateName);

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Performance validation failed: {ex.Message}");
                return result;
            }
        }

        /// <summary>
        /// Validates consistency across multiple templates.
        /// </summary>
        private async Task<ValidationStepResult> ValidateCrossTemplateConsistencyAsync(List<string> templateNames)
        {
            var result = new ValidationStepResult { StepName = "Cross-Template Consistency" };

            try
            {
                // Load all template contents
                var templateContents = new Dictionary<string, string>();
                foreach (var templateName in templateNames)
                {
                    templateContents[templateName] = await GetTemplateContentAsync(templateName);
                }

                // Check for consistent helper usage
                var helperConsistency = ValidateHelperConsistency(templateContents);
                result.Warnings.AddRange(helperConsistency);

                // Check for consistent field naming
                var fieldNamingConsistency = ValidateFieldNamingConsistency(templateContents);
                result.Warnings.AddRange(fieldNamingConsistency);

                // Check for consistent structure patterns
                var structureConsistency = ValidateStructureConsistency(templateContents);
                result.Warnings.AddRange(structureConsistency);

                result.IsValid = !result.Errors.Any();
                result.Details = new { TemplatesAnalyzed = templateNames.Count };

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Cross-template consistency validation failed: {ex.Message}");
                return result;
            }
        }

        #endregion

        #region Validation Helper Methods

        /// <summary>
        /// Validates Handlebars expressions are properly balanced and formed.
        /// </summary>
        private List<string> ValidateHandlebarsExpressions(string content)
        {
            var errors = new List<string>();

            try
            {
                // Check for unmatched opening braces
                var openingBraces = Regex.Matches(content, @"\{\{(?![^}]*\}\})");
                foreach (Match match in openingBraces)
                {
                    errors.Add($"Unmatched opening braces at position {match.Index}");
                }

                // Check for unmatched closing braces
                var closingBraces = Regex.Matches(content, @"(?<!\{\{[^{]*)\}\}");
                foreach (Match match in closingBraces)
                {
                    var precedingText = content.Substring(Math.Max(0, match.Index - 10), Math.Min(10, match.Index));
                    if (!precedingText.Contains("{{"))
                    {
                        errors.Add($"Unmatched closing braces at position {match.Index}");
                    }
                }

                // Check for valid helper syntax
                var helpers = Regex.Matches(content, @"\{\{#?(\w+)[^}]*\}\}");
                foreach (Match match in helpers)
                {
                    var helperName = match.Groups[1].Value;
                    if (!IsValidHelperName(helperName))
                    {
                        errors.Add($"Invalid helper name '{helperName}' at position {match.Index}");
                    }
                }

            }
            catch (Exception ex)
            {
                errors.Add($"Error validating Handlebars expressions: {ex.Message}");
            }

            return errors;
        }

        /// <summary>
        /// Validates the overall structure of the template.
        /// </summary>
        private List<string> ValidateTemplateStructure(string content)
        {
            var errors = new List<string>();

            var requiredSections = new[]
            {
                "CONTEXT",
                "EXTRACTED FIELDS",
                "COMPLETE OCR TEXT",
                "MANDATORY COMPLETION REQUIREMENTS"
            };

            foreach (var section in requiredSections)
            {
                if (!content.Contains(section, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"Required section missing: {section}");
                }
            }

            return errors;
        }

        /// <summary>
        /// Validates proper escaping in template content.
        /// </summary>
        private List<string> ValidateEscaping(string content)
        {
            var warnings = new List<string>();

            // Check for potential JSON injection issues
            if (content.Contains("{{") && content.Contains("toJson") && !content.Contains("escapeForJson"))
            {
                warnings.Add("Consider using escapeForJson helper when outputting JSON to prevent injection");
            }

            return warnings;
        }

        /// <summary>
        /// Validates common syntax issues.
        /// </summary>
        private List<string> ValidateCommonSyntaxIssues(string content)
        {
            var errors = new List<string>();

            // Check for common typos
            var commonTypos = new Dictionary<string, string>
            {
                { "{{toJson", "{{toJson " },
                { "}}}", "}}" },
                { "{{{", "{{" }
            };

            foreach (var typo in commonTypos)
            {
                if (content.Contains(typo.Key))
                {
                    errors.Add($"Potential syntax error: '{typo.Key}' should be '{typo.Value}'");
                }
            }

            return errors;
        }

        /// <summary>
        /// Gets template content for validation.
        /// </summary>
        private async Task<string> GetTemplateContentAsync(string templateName)
        {
            try
            {
                var templates = await _templateEngine.GetAvailableTemplatesAsync();
                var template = templates.FirstOrDefault(t => t.Name == templateName);
                return template?.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **GET_TEMPLATE_CONTENT_ERROR**: Failed to get content for template '{TemplateName}'", templateName);
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates sample context for template testing.
        /// </summary>
        private object CreateSampleTemplateContext()
        {
            return new
            {
                invoice = new
                {
                    InvoiceTotal = "100.00",
                    SubTotal = "85.00",
                    TotalOtherCost = "15.00",
                    InvoiceDate = "07/26/2025",
                    Currency = "USD"
                },
                annotatedContext = "Sample context for validation",
                balanceCheckContext = "Sample balance check",
                fileText = "Sample OCR text for validation testing",
                supplier = new { SupplierName = "Test Supplier" },
                testData = new 
                {
                    targetField = "InvoiceTotal",
                    expectedDate = "07/26/2025",
                    expectedTotal = "100.00"
                }
            };
        }

        /// <summary>
        /// Creates test scenarios for functional validation.
        /// </summary>
        private List<TestScenario> CreateTestScenarios()
        {
            return new List<TestScenario>
            {
                new TestScenario
                {
                    Name = "Basic Invoice Data",
                    Context = CreateSampleTemplateContext()
                },
                new TestScenario
                {
                    Name = "Empty Invoice Data",
                    Context = new { invoice = new { }, fileText = "Empty test case" }
                },
                new TestScenario
                {
                    Name = "Large OCR Text",
                    Context = new { 
                        invoice = CreateSampleTemplateContext(),
                        fileText = new string('x', 10000) // Large text
                    }
                }
            };
        }

        /// <summary>
        /// Tests template rendering with a specific scenario.
        /// </summary>
        private async Task<FunctionalTestResult> TestTemplateWithScenario(TemplateInfo template, TestScenario scenario)
        {
            try
            {
                var renderResult = await _templateEngine.RenderTemplateAsync(template.Name, scenario.Context);
                
                if (renderResult.Success)
                {
                    return new FunctionalTestResult
                    {
                        Success = true,
                        ScenarioName = scenario.Name,
                        OutputLength = renderResult.Content?.Length ?? 0
                    };
                }
                else
                {
                    return new FunctionalTestResult
                    {
                        Success = false,
                        ScenarioName = scenario.Name,
                        ErrorMessage = renderResult.ErrorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                return new FunctionalTestResult
                {
                    Success = false,
                    ScenarioName = scenario.Name,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Tries to compile a template with given context.
        /// </summary>
        private async Task<TemplateCompilationResult> TryCompileTemplateAsync(TemplateInfo template, object context)
        {
            try
            {
                var renderResult = await _templateEngine.RenderTemplateAsync(template.Name, context);
                return new TemplateCompilationResult
                {
                    Success = renderResult.Success,
                    Result = renderResult.Content,
                    ErrorMessage = renderResult.ErrorMessage
                };
            }
            catch (Exception ex)
            {
                return new TemplateCompilationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Validates OCR-specific requirements in template content.
        /// </summary>
        private ValidationStepResult ValidateOCRRequirements(string content)
        {
            var result = new ValidationStepResult();

            // Check for named capture groups requirement
            if (content.Contains("regex") && !content.Contains("(?<"))
            {
                result.Warnings.Add("Template should use named capture groups in regex patterns");
            }

            // Check for critical field mappings
            var requiredMappings = new[] { "InvoiceTotal", "Currency", "InvoiceDate" };
            foreach (var mapping in requiredMappings)
            {
                if (!content.Contains(mapping, StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add($"Template should reference critical field mapping: {mapping}");
                }
            }

            return result;
        }

        /// <summary>
        /// Validates that applied recommendations don't conflict.
        /// </summary>
        private ValidationStepResult ValidateRecommendationConsistency(string content, List<PromptRecommendation> recommendations)
        {
            var result = new ValidationStepResult();

            // Check for conflicting recommendations
            var fieldMappingRecommendations = recommendations.Where(r => r.ChangeType == ChangeType.FieldMappingCorrection).ToList();
            if (fieldMappingRecommendations.Count > 1)
            {
                var fieldNames = fieldMappingRecommendations.Select(r => r.FieldName).Distinct().ToList();
                if (fieldNames.Count != fieldMappingRecommendations.Count)
                {
                    result.Warnings.Add("Multiple field mapping recommendations for same field may conflict");
                }
            }

            return result;
        }

        /// <summary>
        /// Validates business rules specific to the template type.
        /// </summary>
        private ValidationStepResult ValidateBusinessRules(string content, string templateName)
        {
            var result = new ValidationStepResult();

            // Validate based on template type
            if (templateName.Contains("mango", StringComparison.OrdinalIgnoreCase))
            {
                if (!content.Contains("TOTAL AMOUNT", StringComparison.OrdinalIgnoreCase))
                {
                    result.Errors.Add("MANGO template must include TOTAL AMOUNT field detection");
                }
            }

            if (templateName.Contains("header", StringComparison.OrdinalIgnoreCase))
            {
                if (!content.Contains("MANDATORY COMPLETION REQUIREMENTS"))
                {
                    result.Errors.Add("Header detection template must include mandatory completion requirements");
                }
            }

            return result;
        }

        /// <summary>
        /// Measures template performance characteristics.
        /// </summary>
        private async Task<TemplatePerformanceMetrics> MeasureTemplatePerformanceAsync(string templateName)
        {
            var startTime = DateTime.UtcNow;
            var startMemory = GC.GetTotalMemory(false);

            try
            {
                var template = (await _templateEngine.GetAvailableTemplatesAsync()).FirstOrDefault(t => t.Name == templateName);
                if (template == null)
                {
                    return new TemplatePerformanceMetrics { RenderTime = TimeSpan.Zero };
                }

                var context = CreateSampleTemplateContext();
                var renderResult = await _templateEngine.RenderTemplateAsync(templateName, context);

                var endTime = DateTime.UtcNow;
                var endMemory = GC.GetTotalMemory(false);

                return new TemplatePerformanceMetrics
                {
                    RenderTime = endTime - startTime,
                    MemoryUsage = Math.Max(0, endMemory - startMemory),
                    OutputSize = renderResult.Content?.Length ?? 0,
                    Success = renderResult.Success
                };
            }
            catch (Exception ex)
            {
                return new TemplatePerformanceMetrics
                {
                    RenderTime = DateTime.UtcNow - startTime,
                    MemoryUsage = Math.Max(0, GC.GetTotalMemory(false) - startMemory),
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Validates helper usage consistency across templates.
        /// </summary>
        private List<string> ValidateHelperConsistency(Dictionary<string, string> templateContents)
        {
            var warnings = new List<string>();

            var helperUsage = new Dictionary<string, List<string>>();
            foreach (var kvp in templateContents)
            {
                var helpers = Regex.Matches(kvp.Value, @"\{\{(\w+)").Cast<Match>()
                    .Select(m => m.Groups[1].Value).Distinct().ToList();
                
                foreach (var helper in helpers)
                {
                    if (!helperUsage.ContainsKey(helper))
                        helperUsage[helper] = new List<string>();
                    helperUsage[helper].Add(kvp.Key);
                }
            }

            // Check for inconsistent usage patterns
            foreach (var kvp in helperUsage)
            {
                if (kvp.Value.Count == 1 && templateContents.Count > 1)
                {
                    warnings.Add($"Helper '{kvp.Key}' is only used in template '{kvp.Value[0]}' - consider consistency");
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates field naming consistency across templates.
        /// </summary>
        private List<string> ValidateFieldNamingConsistency(Dictionary<string, string> templateContents)
        {
            var warnings = new List<string>();

            var fieldNames = new Dictionary<string, List<string>>();
            foreach (var kvp in templateContents)
            {
                var fields = Regex.Matches(kvp.Value, @"field.*?[""'](\w+)[""']", RegexOptions.IgnoreCase)
                    .Cast<Match>().Select(m => m.Groups[1].Value).Distinct().ToList();
                
                foreach (var field in fields)
                {
                    if (!fieldNames.ContainsKey(field))
                        fieldNames[field] = new List<string>();
                    fieldNames[field].Add(kvp.Key);
                }
            }

            // Check for potential naming inconsistencies
            var potentialVariants = fieldNames.Keys.GroupBy(k => k.ToLower()).Where(g => g.Count() > 1).ToList();
            foreach (var group in potentialVariants)
            {
                warnings.Add($"Potential field naming inconsistency: {string.Join(", ", group.Select(g => g))}");
            }

            return warnings;
        }

        /// <summary>
        /// Validates structure consistency across templates.
        /// </summary>
        private List<string> ValidateStructureConsistency(Dictionary<string, string> templateContents)
        {
            var warnings = new List<string>();

            var sectionPatterns = new[] { "CONTEXT:", "EXTRACTED FIELDS:", "COMPLETE OCR TEXT:" };
            var sectionCounts = new Dictionary<string, int>();

            foreach (var pattern in sectionPatterns)
            {
                sectionCounts[pattern] = templateContents.Values.Count(c => c.Contains(pattern));
            }

            foreach (var kvp in sectionCounts)
            {
                if (kvp.Value > 0 && kvp.Value < templateContents.Count)
                {
                    warnings.Add($"Section '{kvp.Key}' not consistent across all templates ({kvp.Value}/{templateContents.Count})");
                }
            }

            return warnings;
        }

        /// <summary>
        /// Checks if a helper name is valid.
        /// </summary>
        private bool IsValidHelperName(string helperName)
        {
            var validHelpers = new[] { "if", "unless", "each", "with", "toJson", "truncate", "escapeForJson", "escapeForDocumentation" };
            return validHelpers.Contains(helperName, StringComparer.OrdinalIgnoreCase) || 
                   Regex.IsMatch(helperName, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _logger?.Information("🧹 **TEMPLATE_VALIDATION_SERVICE_DISPOSED**: Service disposed successfully");
        }

        #endregion
    }

    #region Supporting Data Models

    /// <summary>
    /// Result of template implementation validation.
    /// </summary>
    public class TemplateImplementationValidationResult
    {
        public string TemplateName { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<string> ValidationWarnings { get; set; } = new List<string>();
        public DateTime ValidationStartTime { get; set; }
        public DateTime ValidationEndTime { get; set; }
        public TimeSpan ValidationDuration { get; set; }
        public bool SyntaxValid { get; set; }
        public bool CompilationValid { get; set; }
        public bool FunctionalValid { get; set; }
        public bool BusinessLogicValid { get; set; }
        public bool PerformanceValid { get; set; }
        public Dictionary<string, object> ValidationDetails { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Result of batch template validation.
    /// </summary>
    public class BatchValidationResult
    {
        public bool Success { get; set; }
        public int TotalTemplates { get; set; }
        public int ValidTemplates { get; set; }
        public int InvalidTemplates { get; set; }
        public List<TemplateImplementationValidationResult> ValidationResults { get; set; } = new List<TemplateImplementationValidationResult>();
        public List<string> BatchValidationErrors { get; set; } = new List<string>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Result of individual validation step.
    /// </summary>
    public class ValidationStepResult
    {
        public string StepName { get; set; }
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public object Details { get; set; }
    }

    /// <summary>
    /// Test scenario for functional validation.
    /// </summary>
    public class TestScenario
    {
        public string Name { get; set; }
        public object Context { get; set; }
    }

    /// <summary>
    /// Result of functional test.
    /// </summary>
    public class FunctionalTestResult
    {
        public bool Success { get; set; }
        public string ScenarioName { get; set; }
        public string ErrorMessage { get; set; }
        public int OutputLength { get; set; }
    }

    /// <summary>
    /// Result of template compilation attempt.
    /// </summary>
    public class TemplateCompilationResult
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Template performance metrics.
    /// </summary>
    public class TemplatePerformanceMetrics
    {
        public TimeSpan RenderTime { get; set; }
        public long MemoryUsage { get; set; }
        public int OutputSize { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    #endregion
}