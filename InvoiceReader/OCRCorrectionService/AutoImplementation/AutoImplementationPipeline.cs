// File: OCRCorrectionService/AutoImplementation/AutoImplementationPipeline.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaterNut.DataSpace.TemplateEngine;
using WaterNut.DataSpace.MetaAI;
using Serilog;

namespace WaterNut.DataSpace.AutoImplementation
{
    /// <summary>
    /// Automatic implementation pipeline for applying Meta AI recommendations to templates
    /// without requiring compilation. Provides hot-reload template modification with
    /// validation, rollback, and audit capabilities.
    /// </summary>
    public class AutoImplementationPipeline : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITemplateEngine _templateEngine;
        private readonly IMetaAIService _metaAIService;
        private readonly AutoImplementationConfig _config;
        private readonly TemplateModificationService _modificationService;
        private readonly TemplateValidationService _validationService;
        private readonly AuditTrailService _auditService;

        public AutoImplementationPipeline(
            ILogger logger,
            ITemplateEngine templateEngine,
            IMetaAIService metaAIService,
            AutoImplementationConfig config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
            _metaAIService = metaAIService ?? throw new ArgumentNullException(nameof(metaAIService));
            _config = config ?? new AutoImplementationConfig();

            _modificationService = new TemplateModificationService(_logger, _config);
            _validationService = new TemplateValidationService(_logger, _templateEngine);
            _auditService = new AuditTrailService(_logger, _config.AuditTrailPath);

            _logger.Information("🤖 **AUTO_IMPLEMENTATION_PIPELINE_INITIALIZED**: Pipeline ready with validation={Validation}, rollback={Rollback}", 
                _config.EnableValidation, _config.EnableRollback);
        }

        #region Core Pipeline Methods

        /// <summary>
        /// Executes the complete automatic implementation pipeline for template optimization.
        /// </summary>
        public async Task<PipelineExecutionResult> ExecuteAsync(PipelineExecutionRequest request)
        {
            _logger.Information("🚀 **PIPELINE_EXECUTION_START**: Executing auto-implementation for template '{TemplateName}' with {RecommendationCount} recommendations",
                request.TemplateName, request.Recommendations?.Count ?? 0);

            var result = new PipelineExecutionResult
            {
                TemplateName = request.TemplateName,
                ExecutionId = Guid.NewGuid().ToString(),
                StartTime = DateTime.UtcNow,
                Steps = new List<PipelineStep>()
            };

            try
            {
                // Step 1: Validate Prerequisites
                var prerequisiteStep = await ValidatePrerequisitesAsync(request);
                result.Steps.Add(prerequisiteStep);
                
                if (!prerequisiteStep.Success)
                {
                    result.Success = false;
                    result.Message = "Prerequisites validation failed";
                    return result;
                }

                // Step 2: Create Backup
                var backupStep = await CreateBackupAsync(request.TemplateName);
                result.Steps.Add(backupStep);
                result.BackupId = backupStep.BackupId;

                if (!backupStep.Success && _config.RequireBackup)
                {
                    result.Success = false;
                    result.Message = "Backup creation failed";
                    return result;
                }

                // Step 3: Filter and Validate Recommendations
                var filterStep = await FilterRecommendationsAsync(request.Recommendations);
                result.Steps.Add(filterStep);
                
                var validRecommendations = filterStep.ValidRecommendations;
                if (!validRecommendations.Any())
                {
                    result.Success = false;
                    result.Message = "No valid recommendations to implement";
                    return result;
                }

                // Step 4: Apply Recommendations
                var implementationStep = await ApplyRecommendationsAsync(request.TemplateName, validRecommendations);
                result.Steps.Add(implementationStep);

                if (!implementationStep.Success)
                {
                    // Step 5a: Rollback on Failure
                    if (_config.EnableRollback && !string.IsNullOrEmpty(result.BackupId))
                    {
                        var rollbackStep = await PerformRollbackAsync(request.TemplateName, result.BackupId);
                        result.Steps.Add(rollbackStep);
                    }
                    
                    result.Success = false;
                    result.Message = $"Implementation failed: {implementationStep.Message}";
                    return result;
                }

                // Step 5b: Validate Implementation
                if (_config.EnableValidation)
                {
                    var validationStep = await ValidateImplementationAsync(request.TemplateName, validRecommendations);
                    result.Steps.Add(validationStep);

                    if (!validationStep.Success && _config.RollbackOnValidationFailure)
                    {
                        var rollbackStep = await PerformRollbackAsync(request.TemplateName, result.BackupId);
                        result.Steps.Add(rollbackStep);
                        
                        result.Success = false;
                        result.Message = "Implementation validation failed, rolled back";
                        return result;
                    }
                }

                // Step 6: Create Audit Trail
                var auditStep = await CreateAuditTrailAsync(request, result);
                result.Steps.Add(auditStep);

                // Step 7: Hot Reload Templates
                var reloadStep = await TriggerHotReloadAsync(request.TemplateName);
                result.Steps.Add(reloadStep);

                result.Success = true;
                result.Message = $"Successfully implemented {validRecommendations.Count} recommendations";
                result.RecommendationsImplemented = validRecommendations.Count;
                result.EndTime = DateTime.UtcNow;

                _logger.Information("✅ **PIPELINE_EXECUTION_SUCCESS**: Template '{TemplateName}' updated with {Count} recommendations in {Duration}ms",
                    request.TemplateName, validRecommendations.Count, (result.EndTime - result.StartTime).TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PIPELINE_EXECUTION_ERROR**: Critical error during auto-implementation pipeline");
                
                result.Success = false;
                result.Message = $"Pipeline execution failed: {ex.Message}";
                result.Exception = ex;
                result.EndTime = DateTime.UtcNow;

                // Attempt emergency rollback
                if (_config.EnableRollback && !string.IsNullOrEmpty(result.BackupId))
                {
                    try
                    {
                        var emergencyRollback = await PerformRollbackAsync(request.TemplateName, result.BackupId);
                        result.Steps.Add(emergencyRollback);
                        _logger.Information("🔄 **EMERGENCY_ROLLBACK**: Performed emergency rollback after critical error");
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.Error(rollbackEx, "❌ **EMERGENCY_ROLLBACK_FAILED**: Emergency rollback also failed");
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Monitors template performance and automatically optimizes using Meta AI.
        /// </summary>
        public async Task<ContinuousOptimizationResult> StartContinuousOptimizationAsync(ContinuousOptimizationConfig optimizationConfig)
        {
            _logger.Information("📊 **CONTINUOUS_OPTIMIZATION_START**: Starting continuous optimization for {TemplateCount} templates",
                optimizationConfig.TemplateNames?.Count ?? 0);

            var result = new ContinuousOptimizationResult
            {
                StartTime = DateTime.UtcNow,
                OptimizationResults = new List<TemplateOptimizationSummary>()
            };

            try
            {
                foreach (var templateName in optimizationConfig.TemplateNames ?? new List<string>())
                {
                    var templateResult = await OptimizeTemplateBasedOnPerformanceAsync(templateName, optimizationConfig);
                    result.OptimizationResults.Add(templateResult);
                }

                result.Success = result.OptimizationResults.Any(r => r.Success);
                result.EndTime = DateTime.UtcNow;

                _logger.Information("📊 **CONTINUOUS_OPTIMIZATION_COMPLETE**: Optimized {SuccessCount}/{TotalCount} templates",
                    result.OptimizationResults.Count(r => r.Success), result.OptimizationResults.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CONTINUOUS_OPTIMIZATION_ERROR**: Continuous optimization failed");
                result.Success = false;
                result.Error = ex.Message;
                result.EndTime = DateTime.UtcNow;
                return result;
            }
        }

        #endregion

        #region Pipeline Step Implementation

        private async Task<PipelineStep> ValidatePrerequisitesAsync(PipelineExecutionRequest request)
        {
            var step = new PipelineStep
            {
                StepName = "Validate Prerequisites",
                StartTime = DateTime.UtcNow
            };

            try
            {
                var validationErrors = new List<string>();

                // Validate template exists
                var availableTemplates = await _templateEngine.GetAvailableTemplatesAsync();
                if (!availableTemplates.Any(t => t.Name == request.TemplateName))
                {
                    validationErrors.Add($"Template '{request.TemplateName}' not found");
                }

                // Validate recommendations
                if (request.Recommendations == null || !request.Recommendations.Any())
                {
                    validationErrors.Add("No recommendations provided");
                }

                // Validate Meta AI service availability
                if (_metaAIService != null)
                {
                    var serviceStatus = await _metaAIService.GetServiceStatusAsync();
                    if (!serviceStatus.IsAvailable)
                    {
                        validationErrors.Add("Meta AI service not available");
                    }
                }

                step.Success = !validationErrors.Any();
                step.Message = step.Success ? "Prerequisites validated" : string.Join("; ", validationErrors);
                step.Details = new { ValidationErrors = validationErrors };
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Prerequisites validation failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> CreateBackupAsync(string templateName)
        {
            var step = new PipelineStep
            {
                StepName = "Create Backup",
                StartTime = DateTime.UtcNow
            };

            try
            {
                var backupId = await _templateEngine.BackupTemplateAsync(templateName);
                
                step.Success = !string.IsNullOrEmpty(backupId);
                step.Message = step.Success ? $"Backup created: {backupId}" : "Backup creation failed";
                step.BackupId = backupId;
                step.Details = new { BackupId = backupId };

                _logger.Information("💾 **BACKUP_CREATED**: Template '{TemplateName}' backed up with ID '{BackupId}'", templateName, backupId);
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Backup creation failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> FilterRecommendationsAsync(List<PromptRecommendation> recommendations)
        {
            var step = new PipelineStep
            {
                StepName = "Filter Recommendations",
                StartTime = DateTime.UtcNow
            };

            try
            {
                var validRecommendations = new List<PromptRecommendation>();
                var rejectedRecommendations = new List<(PromptRecommendation, string)>();

                foreach (var recommendation in recommendations ?? new List<PromptRecommendation>())
                {
                    var validationResult = ValidateRecommendation(recommendation);
                    if (validationResult.IsValid)
                    {
                        validRecommendations.Add(recommendation);
                    }
                    else
                    {
                        rejectedRecommendations.Add((recommendation, validationResult.Reason));
                    }
                }

                step.ValidRecommendations = validRecommendations;
                step.Success = validRecommendations.Any();
                step.Message = $"Filtered {validRecommendations.Count} valid recommendations from {recommendations?.Count ?? 0} total";
                step.Details = new 
                { 
                    ValidCount = validRecommendations.Count,
                    RejectedCount = rejectedRecommendations.Count,
                    RejectedReasons = rejectedRecommendations.Select(r => new { r.Item1.Title, Reason = r.Item2 })
                };

                _logger.Information("🔍 **RECOMMENDATIONS_FILTERED**: {ValidCount} valid, {RejectedCount} rejected", 
                    validRecommendations.Count, rejectedRecommendations.Count);
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Recommendation filtering failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> ApplyRecommendationsAsync(string templateName, List<PromptRecommendation> recommendations)
        {
            var step = new PipelineStep
            {
                StepName = "Apply Recommendations",
                StartTime = DateTime.UtcNow
            };

            try
            {
                var appliedChanges = new List<AppliedChange>();

                foreach (var recommendation in recommendations)
                {
                    var changeResult = await _modificationService.ApplyRecommendationAsync(templateName, recommendation);
                    appliedChanges.Add(changeResult);

                    if (!changeResult.Success)
                    {
                        _logger.Warning("⚠️ **RECOMMENDATION_APPLICATION_FAILED**: Failed to apply '{Title}': {Reason}", 
                            recommendation.Title, changeResult.ErrorMessage);
                    }
                }

                var successfulChanges = appliedChanges.Where(c => c.Success).ToList();
                
                step.Success = successfulChanges.Any();
                step.Message = $"Applied {successfulChanges.Count}/{appliedChanges.Count} recommendations";
                step.Details = new 
                { 
                    AppliedChanges = appliedChanges,
                    SuccessCount = successfulChanges.Count,
                    FailureCount = appliedChanges.Count - successfulChanges.Count
                };

                _logger.Information("🔧 **RECOMMENDATIONS_APPLIED**: {SuccessCount}/{TotalCount} recommendations applied to '{TemplateName}'",
                    successfulChanges.Count, appliedChanges.Count, templateName);
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Recommendation application failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> ValidateImplementationAsync(string templateName, List<PromptRecommendation> recommendations)
        {
            var step = new PipelineStep
            {
                StepName = "Validate Implementation",
                StartTime = DateTime.UtcNow
            };

            try
            {
                var validationResult = await _validationService.ValidateTemplateImplementationAsync(templateName, recommendations);
                
                step.Success = validationResult.IsValid;
                step.Message = validationResult.IsValid ? "Implementation validation passed" : $"Validation failed: {validationResult.ErrorMessage}";
                step.Details = new 
                { 
                    ValidationResult = validationResult,
                    ValidationErrors = validationResult.ValidationErrors,
                    ValidationWarnings = validationResult.ValidationWarnings
                };

                _logger.Information("🔍 **IMPLEMENTATION_VALIDATED**: Template '{TemplateName}' validation {Status}", 
                    templateName, validationResult.IsValid ? "PASSED" : "FAILED");
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Implementation validation failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> PerformRollbackAsync(string templateName, string backupId)
        {
            var step = new PipelineStep
            {
                StepName = "Perform Rollback",
                StartTime = DateTime.UtcNow
            };

            try
            {
                await _templateEngine.RestoreTemplateAsync(templateName, backupId);
                
                step.Success = true;
                step.Message = $"Template rolled back to backup {backupId}";
                step.Details = new { BackupId = backupId };

                _logger.Information("🔄 **ROLLBACK_PERFORMED**: Template '{TemplateName}' restored from backup '{BackupId}'", templateName, backupId);
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Rollback failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> CreateAuditTrailAsync(PipelineExecutionRequest request, PipelineExecutionResult result)
        {
            var step = new PipelineStep
            {
                StepName = "Create Audit Trail",
                StartTime = DateTime.UtcNow
            };

            try
            {
                var auditEntry = new AuditTrailEntry
                {
                    TemplateName = request.TemplateName,
                    ExecutionId = result.ExecutionId,
                    Timestamp = DateTime.UtcNow,
                    Action = "AutoImplementation",
                    Success = result.Success,
                    RecommendationsCount = request.Recommendations?.Count ?? 0,
                    ImplementedCount = result.RecommendationsImplemented,
                    BackupId = result.BackupId,
                    Steps = result.Steps.Select(s => new { s.StepName, s.Success, s.Message }).ToList()
                };

                await _auditService.CreateAuditEntryAsync(auditEntry);
                
                step.Success = true;
                step.Message = "Audit trail created";
                step.Details = new { AuditEntryId = auditEntry.Id };
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Audit trail creation failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        private async Task<PipelineStep> TriggerHotReloadAsync(string templateName)
        {
            var step = new PipelineStep
            {
                StepName = "Trigger Hot Reload",
                StartTime = DateTime.UtcNow
            };

            try
            {
                await _templateEngine.ReloadTemplatesAsync();
                
                step.Success = true;
                step.Message = "Hot reload triggered successfully";
                step.Details = new { TemplateName = templateName };

                _logger.Information("🔥 **HOT_RELOAD_TRIGGERED**: Templates reloaded after auto-implementation");
            }
            catch (Exception ex)
            {
                step.Success = false;
                step.Message = $"Hot reload failed: {ex.Message}";
                step.Exception = ex;
            }

            step.EndTime = DateTime.UtcNow;
            return step;
        }

        #endregion

        #region Continuous Optimization

        private async Task<TemplateOptimizationSummary> OptimizeTemplateBasedOnPerformanceAsync(
            string templateName, 
            ContinuousOptimizationConfig config)
        {
            _logger.Information("📈 **TEMPLATE_OPTIMIZATION_START**: Analyzing performance for template '{TemplateName}'", templateName);

            var summary = new TemplateOptimizationSummary
            {
                TemplateName = templateName,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Collect performance metrics (this would integrate with actual metrics collection)
                var performanceMetrics = await CollectPerformanceMetricsAsync(templateName, config.AnalysisPeriod);

                // Check if optimization is needed
                if (!IsOptimizationNeeded(performanceMetrics, config.OptimizationThresholds))
                {
                    summary.Success = true;
                    summary.Message = "No optimization needed - performance within acceptable thresholds";
                    summary.OptimizationApplied = false;
                    return summary;
                }

                // Request recommendations from Meta AI
                var recommendationRequest = new PromptRecommendationRequest
                {
                    PromptCategory = DeterminePromptCategory(templateName),
                    PerformanceMetrics = performanceMetrics,
                    Goals = config.OptimizationGoals
                };

                var recommendations = await _metaAIService.GetPromptRecommendationsAsync(recommendationRequest);

                if (recommendations.Success && recommendations.Recommendations.Any())
                {
                    // Apply auto-implementable recommendations
                    var pipelineRequest = new PipelineExecutionRequest
                    {
                        TemplateName = templateName,
                        Recommendations = recommendations.Recommendations
                            .Where(r => r.AutoImplementable && r.Confidence >= ConfidenceLevel.Medium)
                            .ToList()
                    };

                    var pipelineResult = await ExecuteAsync(pipelineRequest);

                    summary.Success = pipelineResult.Success;
                    summary.Message = pipelineResult.Message;
                    summary.OptimizationApplied = pipelineResult.Success && pipelineResult.RecommendationsImplemented > 0;
                    summary.RecommendationsApplied = pipelineResult.RecommendationsImplemented;
                    summary.BackupId = pipelineResult.BackupId;
                }
                else
                {
                    summary.Success = false;
                    summary.Message = "No applicable recommendations received from Meta AI";
                    summary.OptimizationApplied = false;
                }

                summary.EndTime = DateTime.UtcNow;
                return summary;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_OPTIMIZATION_ERROR**: Failed to optimize template '{TemplateName}'", templateName);
                summary.Success = false;
                summary.Message = $"Optimization failed: {ex.Message}";
                summary.OptimizationApplied = false;
                summary.EndTime = DateTime.UtcNow;
                return summary;
            }
        }

        #endregion

        #region Helper Methods

        private RecommendationValidationResult ValidateRecommendation(PromptRecommendation recommendation)
        {
            if (recommendation == null)
                return new RecommendationValidationResult { IsValid = false, Reason = "Recommendation is null" };

            if (string.IsNullOrEmpty(recommendation.SuggestedChange))
                return new RecommendationValidationResult { IsValid = false, Reason = "No suggested change provided" };

            if (recommendation.Confidence == ConfidenceLevel.Low && recommendation.AutoImplementable)
                return new RecommendationValidationResult { IsValid = false, Reason = "Low confidence recommendation marked as auto-implementable" };

            if (recommendation.Complexity == ImplementationComplexity.RequiresReview)
                return new RecommendationValidationResult { IsValid = false, Reason = "Recommendation requires manual review" };

            return new RecommendationValidationResult { IsValid = true };
        }

        private async Task<PromptPerformanceMetrics> CollectPerformanceMetricsAsync(string templateName, TimeSpan analysisPeriod)
        {
            // This would integrate with actual performance tracking
            // For now, return mock metrics
            return await Task.FromResult(new PromptPerformanceMetrics
            {
                SuccessRate = 0.85,
                AverageResponseTime = 2.5,
                AccuracyScore = 0.90,
                ConsistencyScore = 0.88,
                TotalExecutions = 100,
                SuccessfulExecutions = 85
            });
        }

        private bool IsOptimizationNeeded(PromptPerformanceMetrics metrics, OptimizationThresholds thresholds)
        {
            return metrics.SuccessRate < thresholds.MinSuccessRate ||
                   metrics.AccuracyScore < thresholds.MinAccuracyScore ||
                   metrics.ConsistencyScore < thresholds.MinConsistencyScore ||
                   metrics.AverageResponseTime > thresholds.MaxResponseTime;
        }

        private string DeterminePromptCategory(string templateName)
        {
            if (templateName.Contains("header", StringComparison.OrdinalIgnoreCase))
                return "header_detection";
            if (templateName.Contains("product", StringComparison.OrdinalIgnoreCase))
                return "product_detection";
            if (templateName.Contains("correction", StringComparison.OrdinalIgnoreCase))
                return "direct_correction";
            return "general";
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _modificationService?.Dispose();
            _validationService?.Dispose();
            _auditService?.Dispose();
            _logger?.Information("🧹 **AUTO_IMPLEMENTATION_PIPELINE_DISPOSED**: Pipeline disposed successfully");
        }

        #endregion
    }

    #region Supporting Classes and Data Models

    /// <summary>
    /// Configuration for auto-implementation pipeline.
    /// </summary>
    public class AutoImplementationConfig
    {
        public bool EnableValidation { get; set; } = true;
        public bool EnableRollback { get; set; } = true;
        public bool RequireBackup { get; set; } = true;
        public bool RollbackOnValidationFailure { get; set; } = true;
        public string AuditTrailPath { get; set; } = "System/audit";
        public TimeSpan ValidationTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public List<string> RestrictedOperations { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request for pipeline execution.
    /// </summary>
    public class PipelineExecutionRequest
    {
        public string TemplateName { get; set; }
        public List<PromptRecommendation> Recommendations { get; set; } = new List<PromptRecommendation>();
        public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
        public bool ForceExecution { get; set; } = false;
        public string RequestedBy { get; set; } = "System";
    }

    /// <summary>
    /// Result of pipeline execution.
    /// </summary>
    public class PipelineExecutionResult
    {
        public string TemplateName { get; set; }
        public string ExecutionId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string BackupId { get; set; }
        public int RecommendationsImplemented { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<PipelineStep> Steps { get; set; } = new List<PipelineStep>();
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Individual pipeline step with execution details.
    /// </summary>
    public class PipelineStep
    {
        public string StepName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string BackupId { get; set; }
        public List<PromptRecommendation> ValidRecommendations { get; set; }
        public object Details { get; set; }
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Configuration for continuous optimization.
    /// </summary>
    public class ContinuousOptimizationConfig
    {
        public List<string> TemplateNames { get; set; } = new List<string>();
        public TimeSpan AnalysisPeriod { get; set; } = TimeSpan.FromDays(7);
        public OptimizationThresholds OptimizationThresholds { get; set; } = new OptimizationThresholds();
        public PromptOptimizationGoals OptimizationGoals { get; set; } = new PromptOptimizationGoals();
        public TimeSpan OptimizationInterval { get; set; } = TimeSpan.FromHours(24);
    }

    /// <summary>
    /// Thresholds for determining when optimization is needed.
    /// </summary>
    public class OptimizationThresholds
    {
        public double MinSuccessRate { get; set; } = 0.80;
        public double MinAccuracyScore { get; set; } = 0.85;
        public double MinConsistencyScore { get; set; } = 0.75;
        public double MaxResponseTime { get; set; } = 5.0;
    }

    /// <summary>
    /// Result of continuous optimization process.
    /// </summary>
    public class ContinuousOptimizationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<TemplateOptimizationSummary> OptimizationResults { get; set; } = new List<TemplateOptimizationSummary>();
    }

    /// <summary>
    /// Summary of individual template optimization.
    /// </summary>
    public class TemplateOptimizationSummary
    {
        public string TemplateName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool OptimizationApplied { get; set; }
        public int RecommendationsApplied { get; set; }
        public string BackupId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// Validation result for recommendations.
    /// </summary>
    public class RecommendationValidationResult
    {
        public bool IsValid { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// Applied change result.
    /// </summary>
    public class AppliedChange
    {
        public bool Success { get; set; }
        public string ChangeType { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
    }

    #endregion
}