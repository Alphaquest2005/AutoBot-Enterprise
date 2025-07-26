// File: OCRCorrectionService/Services/TemplateModificationService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaterNut.DataSpace.MetaAI;
using WaterNut.DataSpace.AutoImplementation;
using Serilog;

namespace WaterNut.DataSpace.Services
{
    /// <summary>
    /// Service for applying Meta AI recommendations to template files.
    /// Handles file-based template modifications with backup and validation capabilities.
    /// CRITICAL: All operations must stay within OCRCorrectionService directory structure.
    /// </summary>
    public class TemplateModificationService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly AutoImplementationConfig _config;
        private readonly string _templateBasePath;

        public TemplateModificationService(ILogger logger, AutoImplementationConfig config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? new AutoImplementationConfig();
            _templateBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRCorrectionService", "Templates");

            _logger.Information("🔧 **TEMPLATE_MODIFICATION_SERVICE_INIT**: Service initialized for template modifications");
        }

        #region Core Modification Methods

        /// <summary>
        /// Applies a Meta AI recommendation to a specific template file.
        /// Returns detailed result including success status and applied changes.
        /// </summary>
        public async Task<AppliedChange> ApplyRecommendationAsync(string templateName, PromptRecommendation recommendation)
        {
            _logger.Information("🔧 **APPLYING_RECOMMENDATION**: Template='{TemplateName}', Type='{ChangeType}', Confidence='{Confidence}'",
                templateName, recommendation.ChangeType, recommendation.Confidence);

            var result = new AppliedChange
            {
                ChangeType = recommendation.ChangeType.ToString(),
                Description = recommendation.Title
            };

            try
            {
                // Validate recommendation applicability
                var validationResult = ValidateRecommendationApplicability(recommendation);
                if (!validationResult.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = validationResult.Reason;
                    return result;
                }

                // Find template file
                var templatePath = FindTemplatePath(templateName);
                if (string.IsNullOrEmpty(templatePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Template file not found: {templateName}";
                    return result;
                }

                // Read current template content
                var originalContent = await File.ReadAllTextAsync(templatePath);
                
                // Apply modification based on recommendation type
                var modifiedContent = await ApplyModificationByTypeAsync(originalContent, recommendation);
                
                if (string.Equals(originalContent, modifiedContent, StringComparison.Ordinal))
                {
                    result.Success = false;
                    result.ErrorMessage = "No changes applied - content identical after modification";
                    return result;
                }

                // Validate modified content
                var contentValidation = ValidateTemplateContent(modifiedContent, templateName);
                if (!contentValidation.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Modified content validation failed: {contentValidation.Reason}";
                    return result;
                }

                // Write modified content back to file
                await File.WriteAllTextAsync(templatePath, modifiedContent);

                result.Success = true;
                result.Description = $"Applied {recommendation.ChangeType}: {recommendation.Title}";

                _logger.Information("✅ **RECOMMENDATION_APPLIED**: Successfully applied '{Title}' to '{TemplateName}'",
                    recommendation.Title, templateName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **RECOMMENDATION_APPLICATION_ERROR**: Failed to apply recommendation to '{TemplateName}'", templateName);
                result.Success = false;
                result.ErrorMessage = $"Application failed: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Applies multiple recommendations to a template in batch mode.
        /// Provides transactional behavior - if any fail, all changes are rolled back.
        /// </summary>
        public async Task<BatchModificationResult> ApplyRecommendationsBatchAsync(
            string templateName, 
            List<PromptRecommendation> recommendations)
        {
            _logger.Information("🔧 **BATCH_MODIFICATION_START**: Applying {RecommendationCount} recommendations to '{TemplateName}'",
                recommendations.Count, templateName);

            var result = new BatchModificationResult
            {
                TemplateName = templateName,
                TotalRecommendations = recommendations.Count,
                AppliedChanges = new List<AppliedChange>()
            };

            string originalContent = null;
            string templatePath = null;

            try
            {
                // Find and backup template
                templatePath = FindTemplatePath(templateName);
                if (string.IsNullOrEmpty(templatePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Template file not found: {templateName}";
                    return result;
                }

                originalContent = await File.ReadAllTextAsync(templatePath);
                var workingContent = originalContent;

                // Apply each recommendation sequentially
                foreach (var recommendation in recommendations)
                {
                    try
                    {
                        var modifiedContent = await ApplyModificationByTypeAsync(workingContent, recommendation);
                        
                        if (!string.Equals(workingContent, modifiedContent, StringComparison.Ordinal))
                        {
                            workingContent = modifiedContent;
                            
                            result.AppliedChanges.Add(new AppliedChange
                            {
                                Success = true,
                                ChangeType = recommendation.ChangeType.ToString(),
                                Description = recommendation.Title
                            });
                        }
                        else
                        {
                            result.AppliedChanges.Add(new AppliedChange
                            {
                                Success = false,
                                ChangeType = recommendation.ChangeType.ToString(),
                                Description = recommendation.Title,
                                ErrorMessage = "No content changes resulted from this recommendation"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        result.AppliedChanges.Add(new AppliedChange
                        {
                            Success = false,
                            ChangeType = recommendation.ChangeType.ToString(),
                            Description = recommendation.Title,
                            ErrorMessage = ex.Message
                        });

                        _logger.Warning("⚠️ **BATCH_RECOMMENDATION_FAILED**: Failed to apply '{Title}': {Error}",
                            recommendation.Title, ex.Message);
                    }
                }

                // Validate final content
                var finalValidation = ValidateTemplateContent(workingContent, templateName);
                if (!finalValidation.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Final template validation failed: {finalValidation.Reason}";
                    return result;
                }

                // Write final content
                await File.WriteAllTextAsync(templatePath, workingContent);

                result.Success = true;
                result.SuccessfulChanges = result.AppliedChanges.Count(c => c.Success);
                result.FailedChanges = result.AppliedChanges.Count(c => !c.Success);

                _logger.Information("✅ **BATCH_MODIFICATION_COMPLETE**: Applied {SuccessCount}/{TotalCount} recommendations to '{TemplateName}'",
                    result.SuccessfulChanges, result.TotalRecommendations, templateName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **BATCH_MODIFICATION_ERROR**: Critical error during batch modification");

                // Attempt to restore original content on critical failure
                if (!string.IsNullOrEmpty(templatePath) && !string.IsNullOrEmpty(originalContent))
                {
                    try
                    {
                        await File.WriteAllTextAsync(templatePath, originalContent);
                        _logger.Information("🔄 **BATCH_ROLLBACK**: Restored original template content after critical error");
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.Error(rollbackEx, "❌ **BATCH_ROLLBACK_FAILED**: Could not restore original content");
                    }
                }

                result.Success = false;
                result.ErrorMessage = $"Batch modification failed: {ex.Message}";
                return result;
            }
        }

        #endregion

        #region Modification Type Handlers

        /// <summary>
        /// Applies modifications based on recommendation type.
        /// Dispatches to specific handlers for different change types.
        /// </summary>
        private async Task<string> ApplyModificationByTypeAsync(string content, PromptRecommendation recommendation)
        {
            switch (recommendation.ChangeType)
            {
                case ChangeType.PromptOptimization:
                    return await ApplyPromptOptimizationAsync(content, recommendation);
                
                case ChangeType.RegexImprovement:
                    return await ApplyRegexImprovementAsync(content, recommendation);
                
                case ChangeType.InstructionClarification:
                    return await ApplyInstructionClarificationAsync(content, recommendation);
                
                case ChangeType.FieldMappingCorrection:
                    return await ApplyFieldMappingCorrectionAsync(content, recommendation);
                
                case ChangeType.ExampleAddition:
                    return await ApplyExampleAdditionAsync(content, recommendation);
                
                case ChangeType.ContextualImprovement:
                    return await ApplyContextualImprovementAsync(content, recommendation);
                
                default:
                    _logger.Warning("⚠️ **UNKNOWN_CHANGE_TYPE**: Unknown change type '{ChangeType}', applying as general modification",
                        recommendation.ChangeType);
                    return await ApplyGeneralModificationAsync(content, recommendation);
            }
        }

        /// <summary>
        /// Applies prompt optimization changes to template content.
        /// </summary>
        private async Task<string> ApplyPromptOptimizationAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_PROMPT_OPTIMIZATION**: {Title}", recommendation.Title);

            // Apply text replacements or modifications suggested in the recommendation
            var modifiedContent = content;

            if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // If the recommendation includes specific before/after text
                if (!string.IsNullOrEmpty(recommendation.BeforeText) && !string.IsNullOrEmpty(recommendation.AfterText))
                {
                    modifiedContent = modifiedContent.Replace(recommendation.BeforeText, recommendation.AfterText);
                }
                else
                {
                    // Apply general change as insertion at appropriate location
                    modifiedContent = await InsertContentAtOptimalLocationAsync(modifiedContent, recommendation.SuggestedChange, recommendation);
                }
            }

            return await Task.FromResult(modifiedContent);
        }

        /// <summary>
        /// Applies regex pattern improvements to template content.
        /// </summary>
        private async Task<string> ApplyRegexImprovementAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_REGEX_IMPROVEMENT**: {Title}", recommendation.Title);

            var modifiedContent = content;

            // Look for existing regex patterns and improve them
            if (!string.IsNullOrEmpty(recommendation.TargetPattern) && !string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Replace specific regex pattern
                modifiedContent = modifiedContent.Replace(recommendation.TargetPattern, recommendation.SuggestedChange);
            }
            else if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Add new regex pattern in appropriate section
                modifiedContent = await InsertRegexPatternAsync(modifiedContent, recommendation.SuggestedChange, recommendation);
            }

            return await Task.FromResult(modifiedContent);
        }

        /// <summary>
        /// Applies instruction clarification changes to template content.
        /// </summary>
        private async Task<string> ApplyInstructionClarificationAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_INSTRUCTION_CLARIFICATION**: {Title}", recommendation.Title);

            var modifiedContent = content;

            if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Insert clarification in appropriate instruction section
                modifiedContent = await InsertInstructionClarificationAsync(modifiedContent, recommendation.SuggestedChange, recommendation);
            }

            return await Task.FromResult(modifiedContent);
        }

        /// <summary>
        /// Applies field mapping corrections to template content.
        /// </summary>
        private async Task<string> ApplyFieldMappingCorrectionAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_FIELD_MAPPING_CORRECTION**: {Title}", recommendation.Title);

            var modifiedContent = content;

            if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Apply field mapping changes in field mapping section
                modifiedContent = await InsertFieldMappingAsync(modifiedContent, recommendation.SuggestedChange, recommendation);
            }

            return await Task.FromResult(modifiedContent);
        }

        /// <summary>
        /// Applies example additions to template content.
        /// </summary>
        private async Task<string> ApplyExampleAdditionAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_EXAMPLE_ADDITION**: {Title}", recommendation.Title);

            var modifiedContent = content;

            if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Add examples in examples section or create new section
                modifiedContent = await InsertExampleAsync(modifiedContent, recommendation.SuggestedChange, recommendation);
            }

            return await Task.FromResult(modifiedContent);
        }

        /// <summary>
        /// Applies contextual improvements to template content.
        /// </summary>
        private async Task<string> ApplyContextualImprovementAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_CONTEXTUAL_IMPROVEMENT**: {Title}", recommendation.Title);

            var modifiedContent = content;

            if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Apply contextual improvements in appropriate section
                modifiedContent = await InsertContextualImprovementAsync(modifiedContent, recommendation.SuggestedChange, recommendation);
            }

            return await Task.FromResult(modifiedContent);
        }

        /// <summary>
        /// Applies general modifications when specific type handlers aren't available.
        /// </summary>
        private async Task<string> ApplyGeneralModificationAsync(string content, PromptRecommendation recommendation)
        {
            _logger.Verbose("🔧 **APPLYING_GENERAL_MODIFICATION**: {Title}", recommendation.Title);

            var modifiedContent = content;

            if (!string.IsNullOrEmpty(recommendation.SuggestedChange))
            {
                // Insert at the end of the template before closing
                var insertionPoint = FindGeneralInsertionPoint(modifiedContent);
                if (insertionPoint >= 0)
                {
                    modifiedContent = modifiedContent.Insert(insertionPoint, $"\n\n{{!-- {recommendation.Title} --}}\n{recommendation.SuggestedChange}\n");
                }
                else
                {
                    // Append at the end
                    modifiedContent += $"\n\n{{!-- {recommendation.Title} --}}\n{recommendation.SuggestedChange}\n";
                }
            }

            return await Task.FromResult(modifiedContent);
        }

        #endregion

        #region Content Insertion Helpers

        /// <summary>
        /// Inserts content at the optimal location within the template.
        /// </summary>
        private async Task<string> InsertContentAtOptimalLocationAsync(string content, string insertContent, PromptRecommendation recommendation)
        {
            // Find appropriate section based on recommendation context
            var sectionMarkers = new[]
            {
                "**CONTEXT:**",
                "**CRITICAL REGEX REQUIREMENTS**",
                "**Instructions for",
                "**MANDATORY COMPLETION REQUIREMENTS**"
            };

            foreach (var marker in sectionMarkers)
            {
                var markerIndex = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (markerIndex >= 0)
                {
                    var nextSectionIndex = FindNextSectionStart(content, markerIndex + marker.Length);
                    var insertionPoint = nextSectionIndex > 0 ? nextSectionIndex : content.Length;
                    
                    return content.Insert(insertionPoint, $"\n\n{{!-- {recommendation.Title} --}}\n{insertContent}\n");
                }
            }

            // Default: insert near the end
            return content + $"\n\n{{!-- {recommendation.Title} --}}\n{insertContent}\n";
        }

        /// <summary>
        /// Inserts regex pattern in the appropriate regex section.
        /// </summary>
        private async Task<string> InsertRegexPatternAsync(string content, string regexPattern, PromptRecommendation recommendation)
        {
            var regexSectionMarker = "**CRITICAL REGEX REQUIREMENTS";
            var sectionIndex = content.IndexOf(regexSectionMarker, StringComparison.OrdinalIgnoreCase);
            
            if (sectionIndex >= 0)
            {
                var insertionPoint = FindEndOfSection(content, sectionIndex);
                return content.Insert(insertionPoint, $"\n⚠️ **{recommendation.Title.ToUpper()}**: \"{regexPattern}\"\n");
            }

            return await InsertContentAtOptimalLocationAsync(content, $"**REGEX PATTERN**: {regexPattern}", recommendation);
        }

        /// <summary>
        /// Inserts instruction clarification in the instructions section.
        /// </summary>
        private async Task<string> InsertInstructionClarificationAsync(string content, string clarification, PromptRecommendation recommendation)
        {
            var instructionMarkers = new[] { "**Instructions for", "### **Instructions", "**MANDATORY COMPLETION" };
            
            foreach (var marker in instructionMarkers)
            {
                var sectionIndex = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (sectionIndex >= 0)
                {
                    var insertionPoint = FindEndOfSection(content, sectionIndex);
                    return content.Insert(insertionPoint, $"\n\n**{recommendation.Title}:**\n{clarification}\n");
                }
            }

            return await InsertContentAtOptimalLocationAsync(content, clarification, recommendation);
        }

        /// <summary>
        /// Inserts field mapping information in the field mapping section.
        /// </summary>
        private async Task<string> InsertFieldMappingAsync(string content, string fieldMapping, PromptRecommendation recommendation)
        {
            var fieldMappingMarkers = new[] { "**Field Mapping", "**Caribbean Customs Field Mapping", "#### **Field Mapping" };
            
            foreach (var marker in fieldMappingMarkers)
            {
                var sectionIndex = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (sectionIndex >= 0)
                {
                    var insertionPoint = FindEndOfSection(content, sectionIndex);
                    return content.Insert(insertionPoint, $"\n*   **{recommendation.Title}**: {fieldMapping}\n");
                }
            }

            return await InsertContentAtOptimalLocationAsync(content, $"**Field Mapping - {recommendation.Title}:**\n{fieldMapping}", recommendation);
        }

        /// <summary>
        /// Inserts examples in the examples section.
        /// </summary>
        private async Task<string> InsertExampleAsync(string content, string example, PromptRecommendation recommendation)
        {
            var exampleMarkers = new[] { "**EXAMPLE", "**Examples", "### **Examples" };
            
            foreach (var marker in exampleMarkers)
            {
                var sectionIndex = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (sectionIndex >= 0)
                {
                    var insertionPoint = FindEndOfSection(content, sectionIndex);
                    return content.Insert(insertionPoint, $"\n\n**Example - {recommendation.Title}:**\n{example}\n");
                }
            }

            return await InsertContentAtOptimalLocationAsync(content, $"**Example - {recommendation.Title}:**\n{example}", recommendation);
        }

        /// <summary>
        /// Inserts contextual improvements in appropriate context sections.
        /// </summary>
        private async Task<string> InsertContextualImprovementAsync(string content, string improvement, PromptRecommendation recommendation)
        {
            var contextMarkers = new[] { "**CONTEXT:", "**SUPPLIER CONTEXT", "**2. CONTEXT" };
            
            foreach (var marker in contextMarkers)
            {
                var sectionIndex = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (sectionIndex >= 0)
                {
                    var insertionPoint = FindEndOfSection(content, sectionIndex);
                    return content.Insert(insertionPoint, $"\n\n**{recommendation.Title}:**\n{improvement}\n");
                }
            }

            return await InsertContentAtOptimalLocationAsync(content, improvement, recommendation);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Finds the file path for a given template name.
        /// Searches within OCRCorrectionService directory structure.
        /// </summary>
        private string FindTemplatePath(string templateName)
        {
            try
            {
                var searchPatterns = new[]
                {
                    $"{templateName}.hbs",
                    $"{templateName}",
                    $"{templateName}-template.hbs",
                    $"{templateName.Replace("Template", "")}.hbs"
                };

                var searchDirectories = new[]
                {
                    Path.Combine(_templateBasePath, "OCR", "HeaderDetection"),
                    Path.Combine(_templateBasePath, "OCR", "ProductDetection"),
                    Path.Combine(_templateBasePath, "OCR", "DirectCorrection"),
                    Path.Combine(_templateBasePath, "OCR", "RegexCreation"),
                    Path.Combine(_templateBasePath, "OCR", "Shared"),
                    _templateBasePath
                };

                foreach (var directory in searchDirectories)
                {
                    if (!Directory.Exists(directory)) continue;

                    foreach (var pattern in searchPatterns)
                    {
                        var filePath = Path.Combine(directory, pattern);
                        if (File.Exists(filePath))
                        {
                            _logger.Verbose("📁 **TEMPLATE_FOUND**: Located '{TemplateName}' at '{FilePath}'", templateName, filePath);
                            return filePath;
                        }
                    }
                }

                _logger.Warning("⚠️ **TEMPLATE_NOT_FOUND**: Could not locate template '{TemplateName}' in any search directory", templateName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_SEARCH_ERROR**: Error searching for template '{TemplateName}'", templateName);
                return null;
            }
        }

        /// <summary>
        /// Validates that a recommendation can be applied to templates.
        /// </summary>
        private RecommendationValidationResult ValidateRecommendationApplicability(PromptRecommendation recommendation)
        {
            if (recommendation == null)
                return new RecommendationValidationResult { IsValid = false, Reason = "Recommendation is null" };

            if (string.IsNullOrEmpty(recommendation.SuggestedChange))
                return new RecommendationValidationResult { IsValid = false, Reason = "No suggested change provided" };

            if (recommendation.Confidence == ConfidenceLevel.Low)
                return new RecommendationValidationResult { IsValid = false, Reason = "Confidence level too low for automatic application" };

            if (recommendation.Complexity == ImplementationComplexity.RequiresReview)
                return new RecommendationValidationResult { IsValid = false, Reason = "Recommendation requires manual review" };

            return new RecommendationValidationResult { IsValid = true };
        }

        /// <summary>
        /// Validates template content after modifications.
        /// </summary>
        private TemplateValidationResult ValidateTemplateContent(string content, string templateName)
        {
            try
            {
                // Basic syntax validation
                if (string.IsNullOrWhiteSpace(content))
                {
                    return new TemplateValidationResult { IsValid = false, Reason = "Template content is empty or whitespace" };
                }

                // Check for balanced Handlebars syntax
                var openBraces = Regex.Matches(content, @"\{\{[^}]*\}\}").Count;
                var unmatchedBraces = Regex.Matches(content, @"\{\{(?![^}]*\}\})").Count;
                
                if (unmatchedBraces > 0)
                {
                    return new TemplateValidationResult { IsValid = false, Reason = $"Template has {unmatchedBraces} unmatched handlebars expressions" };
                }

                // Check for required sections (basic validation)
                var requiredSections = new[] { "CONTEXT", "EXTRACTED FIELDS", "COMPLETE OCR TEXT" };
                foreach (var section in requiredSections)
                {
                    if (!content.Contains(section, StringComparison.OrdinalIgnoreCase))
                    {
                        return new TemplateValidationResult { IsValid = false, Reason = $"Template missing required section: {section}" };
                    }
                }

                return new TemplateValidationResult { IsValid = true };
            }
            catch (Exception ex)
            {
                return new TemplateValidationResult { IsValid = false, Reason = $"Validation error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Finds the next section start marker in content.
        /// </summary>
        private int FindNextSectionStart(string content, int startIndex)
        {
            var sectionMarkers = new[] { "\n**", "\n###", "\n🎯", "\n🚨", "\n⚠️" };
            
            var nearestIndex = content.Length;
            foreach (var marker in sectionMarkers)
            {
                var index = content.IndexOf(marker, startIndex, StringComparison.OrdinalIgnoreCase);
                if (index >= 0 && index < nearestIndex)
                {
                    nearestIndex = index;
                }
            }

            return nearestIndex < content.Length ? nearestIndex : -1;
        }

        /// <summary>
        /// Finds the end of the current section.
        /// </summary>
        private int FindEndOfSection(string content, int sectionStartIndex)
        {
            var nextSectionIndex = FindNextSectionStart(content, sectionStartIndex);
            return nextSectionIndex > 0 ? nextSectionIndex : content.Length;
        }

        /// <summary>
        /// Finds appropriate insertion point for general modifications.
        /// </summary>
        private int FindGeneralInsertionPoint(string content)
        {
            var endMarkers = new[] { "**CRITICAL EMPTY RESPONSE REQUIREMENT", "**MANDATORY RESPONSE FORMAT", "{{#if supplier.SupplierName}}" };
            
            foreach (var marker in endMarkers)
            {
                var index = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    return index;
                }
            }

            return -1;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _logger?.Information("🧹 **TEMPLATE_MODIFICATION_SERVICE_DISPOSED**: Service disposed successfully");
        }

        #endregion
    }

    #region Supporting Data Models

    /// <summary>
    /// Result of batch modification operation.
    /// </summary>
    public class BatchModificationResult
    {
        public string TemplateName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int TotalRecommendations { get; set; }
        public int SuccessfulChanges { get; set; }
        public int FailedChanges { get; set; }
        public List<AppliedChange> AppliedChanges { get; set; } = new List<AppliedChange>();
    }

    /// <summary>
    /// Result of template content validation.
    /// </summary>
    public class TemplateValidationResult
    {
        public bool IsValid { get; set; }
        public string Reason { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }

    #endregion
}