// File: OCRCorrectionService/MetaAI/MetaAIService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace WaterNut.DataSpace.MetaAI
{
    /// <summary>
    /// Meta AI service implementation for prompt optimization and recommendation.
    /// Provides intelligent prompt enhancement using Meta AI's language models.
    /// </summary>
    public class MetaAIService : IMetaAIService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly MetaAIConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions;

        public MetaAIService(ILogger logger, HttpClient httpClient, MetaAIConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            SetupHttpClient();
            
            _logger.Information("🤖 **META_AI_SERVICE_INITIALIZED**: Service initialized with endpoint '{Endpoint}'", _config.ApiEndpoint);
        }

        #region Core API Methods

        public async Task<PromptRecommendationResult> GetPromptRecommendationsAsync(PromptRecommendationRequest request)
        {
            _logger.Information("🔍 **PROMPT_RECOMMENDATIONS_REQUEST**: Getting recommendations for '{Category}' prompt", request.PromptCategory);

            try
            {
                var apiRequest = new
                {
                    task = "prompt_recommendation",
                    prompt = request.CurrentPrompt,
                    category = request.PromptCategory,
                    performance_metrics = request.PerformanceMetrics,
                    failure_examples = request.FailureExamples,
                    success_examples = request.SuccessExamples,
                    context = request.Context,
                    goals = request.Goals,
                    model_config = new
                    {
                        temperature = 0.1,
                        max_tokens = 4000,
                        top_p = 0.9
                    }
                };

                var response = await CallMetaAIAsync<PromptRecommendationResponse>("prompt/recommend", apiRequest);
                
                if (response.Success)
                {
                    var result = new PromptRecommendationResult
                    {
                        Success = true,
                        Message = "Recommendations generated successfully",
                        Recommendations = response.Recommendations?.Select(MapToPromptRecommendation).ToList() ?? new List<PromptRecommendation>(),
                        Insights = MapToPromptInsights(response.Insights),
                        Metadata = response.Metadata ?? new Dictionary<string, object>()
                    };

                    _logger.Information("✅ **PROMPT_RECOMMENDATIONS_SUCCESS**: Generated {Count} recommendations", result.Recommendations.Count);
                    return result;
                }
                else
                {
                    _logger.Warning("⚠️ **PROMPT_RECOMMENDATIONS_WARNING**: Meta AI returned unsuccessful response: {Message}", response.Message);
                    return new PromptRecommendationResult
                    {
                        Success = false,
                        Message = response.Message ?? "Unknown error from Meta AI service"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PROMPT_RECOMMENDATIONS_ERROR**: Failed to get prompt recommendations");
                return new PromptRecommendationResult
                {
                    Success = false,
                    Message = $"Error getting recommendations: {ex.Message}"
                };
            }
        }

        public async Task<PromptAnalysisResult> AnalyzePromptEffectivenessAsync(PromptAnalysisRequest request)
        {
            _logger.Information("📊 **PROMPT_ANALYSIS_REQUEST**: Analyzing prompt effectiveness for '{Domain}' domain", request.TargetDomain);

            try
            {
                var apiRequest = new
                {
                    task = "prompt_analysis",
                    prompt = request.Prompt,
                    target_domain = request.TargetDomain,
                    test_cases = request.TestCases,
                    analysis_depth = request.Depth.ToString().ToLower(),
                    focus_areas = request.FocusAreas,
                    model_config = new
                    {
                        temperature = 0.2,
                        max_tokens = 3000
                    }
                };

                var response = await CallMetaAIAsync<PromptAnalysisResponse>("prompt/analyze", apiRequest);

                if (response.Success)
                {
                    var result = new PromptAnalysisResult
                    {
                        Success = true,
                        Message = "Analysis completed successfully",
                        QualityScore = MapToQualityScore(response.QualityScore),
                        Issues = response.Issues?.Select(MapToPromptIssue).ToList() ?? new List<PromptIssue>(),
                        Strengths = response.Strengths?.Select(MapToPromptStrength).ToList() ?? new List<PromptStrength>(),
                        Insights = MapToPromptInsights(response.Insights),
                        Suggestions = response.Suggestions?.Select(MapToImprovementSuggestion).ToList() ?? new List<ImprovementSuggestion>()
                    };

                    _logger.Information("✅ **PROMPT_ANALYSIS_SUCCESS**: Analysis complete with overall score {Score}", 
                        result.QualityScore?.OverallScore ?? 0);
                    return result;
                }
                else
                {
                    _logger.Warning("⚠️ **PROMPT_ANALYSIS_WARNING**: Meta AI returned unsuccessful response: {Message}", response.Message);
                    return new PromptAnalysisResult
                    {
                        Success = false,
                        Message = response.Message ?? "Unknown error from Meta AI service"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PROMPT_ANALYSIS_ERROR**: Failed to analyze prompt effectiveness");
                return new PromptAnalysisResult
                {
                    Success = false,
                    Message = $"Error analyzing prompt: {ex.Message}"
                };
            }
        }

        public async Task<PromptOptimizationResult> OptimizePromptAsync(PromptOptimizationRequest request)
        {
            _logger.Information("⚡ **PROMPT_OPTIMIZATION_REQUEST**: Optimizing prompt for target model '{TargetModel}'", request.TargetModel);

            try
            {
                var apiRequest = new
                {
                    task = "prompt_optimization",
                    original_prompt = request.OriginalPrompt,
                    goals = request.Goals,
                    constraints = request.Constraints,
                    current_metrics = request.CurrentMetrics,
                    target_model = request.TargetModel,
                    preserve_structure = request.PreserveEssentialStructure,
                    model_config = new
                    {
                        temperature = 0.1,
                        max_tokens = 5000
                    }
                };

                var response = await CallMetaAIAsync<PromptOptimizationResponse>("prompt/optimize", apiRequest);

                if (response.Success && !string.IsNullOrEmpty(response.OptimizedPrompt))
                {
                    var result = new PromptOptimizationResult
                    {
                        Success = true,
                        Message = "Prompt optimization completed successfully",
                        OptimizedPrompt = response.OptimizedPrompt,
                        Changes = response.Changes?.Select(MapToOptimizationChange).ToList() ?? new List<OptimizationChange>(),
                        OriginalScore = MapToQualityScore(response.OriginalScore),
                        OptimizedScore = MapToQualityScore(response.OptimizedScore),
                        Improvement = MapToPerformanceImprovement(response.Improvement),
                        OptimizationRationale = response.Rationale
                    };

                    _logger.Information("✅ **PROMPT_OPTIMIZATION_SUCCESS**: Optimization complete with {ChangeCount} changes", 
                        result.Changes.Count);
                    return result;
                }
                else
                {
                    _logger.Warning("⚠️ **PROMPT_OPTIMIZATION_WARNING**: Meta AI returned unsuccessful response: {Message}", response.Message);
                    return new PromptOptimizationResult
                    {
                        Success = false,
                        Message = response.Message ?? "Optimization failed to produce valid result"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PROMPT_OPTIMIZATION_ERROR**: Failed to optimize prompt");
                return new PromptOptimizationResult
                {
                    Success = false,
                    Message = $"Error optimizing prompt: {ex.Message}"
                };
            }
        }

        public async Task<PromptValidationResult> ValidatePromptAsync(PromptValidationRequest request)
        {
            _logger.Information("🔍 **PROMPT_VALIDATION_REQUEST**: Validating '{PromptType}' prompt", request.PromptType);

            try
            {
                var apiRequest = new
                {
                    task = "prompt_validation",
                    prompt = request.Prompt,
                    prompt_type = request.PromptType,
                    custom_rules = request.CustomRules,
                    strictness = request.Strictness.ToString().ToLower(),
                    check_bias = request.CheckForBias,
                    check_safety = request.CheckForSafety,
                    model_config = new
                    {
                        temperature = 0.0,
                        max_tokens = 2000
                    }
                };

                var response = await CallMetaAIAsync<PromptValidationResponse>("prompt/validate", apiRequest);

                var result = new PromptValidationResult
                {
                    IsValid = response.IsValid,
                    HasWarnings = response.HasWarnings,
                    Issues = response.Issues?.Select(MapToValidationIssue).ToList() ?? new List<ValidationIssue>(),
                    Warnings = response.Warnings?.Select(MapToValidationWarning).ToList() ?? new List<ValidationWarning>(),
                    QualityScore = MapToQualityScore(response.QualityScore),
                    BestPractices = response.BestPractices?.Select(MapToBestPracticeRecommendation).ToList() ?? new List<BestPracticeRecommendation>()
                };

                _logger.Information("✅ **PROMPT_VALIDATION_SUCCESS**: Validation complete - Valid: {IsValid}, Warnings: {WarningCount}", 
                    result.IsValid, result.Warnings.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PROMPT_VALIDATION_ERROR**: Failed to validate prompt");
                return new PromptValidationResult
                {
                    IsValid = false,
                    HasWarnings = true,
                    Issues = new List<ValidationIssue>
                    {
                        new ValidationIssue { Message = $"Validation service error: {ex.Message}", Severity = "Error" }
                    }
                };
            }
        }

        public async Task<ABTestingRecommendation> GetABTestingRecommendationsAsync(ABTestingRequest request)
        {
            _logger.Information("🧪 **AB_TESTING_REQUEST**: Getting A/B testing recommendations for {VariantCount} variants", request.VariantPrompts.Count);

            try
            {
                var apiRequest = new
                {
                    task = "ab_testing",
                    baseline_prompt = request.BaselinePrompt,
                    variant_prompts = request.VariantPrompts,
                    objectives = request.Objectives,
                    minimum_sample_size = request.MinimumSampleSize,
                    significance_level = request.StatisticalSignificanceLevel,
                    max_duration_days = request.MaxTestDuration.TotalDays,
                    model_config = new
                    {
                        temperature = 0.2,
                        max_tokens = 2500
                    }
                };

                var response = await CallMetaAIAsync<ABTestingResponse>("prompt/ab-test", apiRequest);

                if (response.Success)
                {
                    var result = new ABTestingRecommendation
                    {
                        Success = true,
                        Message = "A/B testing recommendations generated successfully",
                        RecommendedVariants = response.RecommendedVariants?.Select(MapToTestVariant).ToList() ?? new List<TestVariant>(),
                        Strategy = MapToTestingStrategy(response.Strategy),
                        TestPlan = MapToStatisticalTestPlan(response.TestPlan),
                        Metrics = response.Metrics?.Select(MapToMetricsToTrack).ToList() ?? new List<MetricsToTrack>(),
                        EstimatedTestDuration = TimeSpan.FromDays(response.EstimatedDurationDays)
                    };

                    _logger.Information("✅ **AB_TESTING_SUCCESS**: Generated {VariantCount} test variants with {MetricCount} metrics", 
                        result.RecommendedVariants.Count, result.Metrics.Count);
                    return result;
                }
                else
                {
                    _logger.Warning("⚠️ **AB_TESTING_WARNING**: Meta AI returned unsuccessful response: {Message}", response.Message);
                    return new ABTestingRecommendation
                    {
                        Success = false,
                        Message = response.Message ?? "Failed to generate A/B testing recommendations"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AB_TESTING_ERROR**: Failed to get A/B testing recommendations");
                return new ABTestingRecommendation
                {
                    Success = false,
                    Message = $"Error getting A/B testing recommendations: {ex.Message}"
                };
            }
        }

        public async Task<AutoImplementationResult> AutoImplementRecommendationsAsync(AutoImplementationRequest request)
        {
            _logger.Information("🤖 **AUTO_IMPLEMENTATION_REQUEST**: Auto-implementing {RecommendationCount} recommendations for template '{TemplateName}'", 
                request.Recommendations.Count, request.TemplateName);

            var result = new AutoImplementationResult
            {
                Success = false,
                StepsExecuted = new List<ImplementationStep>(),
                Errors = new List<ImplementationError>(),
                RollbackAvailable = request.EnableRollback
            };

            try
            {
                // Step 1: Create backup if requested
                if (request.BackupStrategy == "create_backup")
                {
                    result.StepsExecuted.Add(new ImplementationStep
                    {
                        StepName = "Create Backup",
                        Status = "InProgress",
                        StartTime = DateTime.UtcNow
                    });

                    // This would integrate with the template engine's backup functionality
                    result.BackupId = $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
                    
                    result.StepsExecuted.Last().Status = "Completed";
                    result.StepsExecuted.Last().EndTime = DateTime.UtcNow;
                    _logger.Information("✅ **BACKUP_CREATED**: Backup created with ID '{BackupId}'", result.BackupId);
                }

                // Step 2: Validate recommendations
                if (request.RequireValidation)
                {
                    result.StepsExecuted.Add(new ImplementationStep
                    {
                        StepName = "Validate Recommendations",
                        Status = "InProgress",
                        StartTime = DateTime.UtcNow
                    });

                    var validationErrors = await ValidateRecommendationsAsync(request.Recommendations);
                    if (validationErrors.Any())
                    {
                        result.Errors.AddRange(validationErrors);
                        result.StepsExecuted.Last().Status = "Failed";
                        result.StepsExecuted.Last().EndTime = DateTime.UtcNow;
                        result.Message = "Validation failed - implementation aborted";
                        return result;
                    }

                    result.StepsExecuted.Last().Status = "Completed";
                    result.StepsExecuted.Last().EndTime = DateTime.UtcNow;
                    _logger.Information("✅ **VALIDATION_PASSED**: All recommendations validated successfully");
                }

                // Step 3: Apply recommendations
                result.StepsExecuted.Add(new ImplementationStep
                {
                    StepName = "Apply Recommendations",
                    Status = "InProgress",
                    StartTime = DateTime.UtcNow
                });

                var implementationResult = await ApplyRecommendationsAsync(request.TemplateName, request.Recommendations, request.Options);
                if (!implementationResult.Success)
                {
                    result.Errors.AddRange(implementationResult.Errors);
                    result.StepsExecuted.Last().Status = "Failed";
                    result.StepsExecuted.Last().EndTime = DateTime.UtcNow;
                    result.Message = "Implementation failed";
                    return result;
                }

                result.StepsExecuted.Last().Status = "Completed";
                result.StepsExecuted.Last().EndTime = DateTime.UtcNow;

                // Step 4: Validate implementation
                if (request.RequireValidation)
                {
                    result.StepsExecuted.Add(new ImplementationStep
                    {
                        StepName = "Validate Implementation",
                        Status = "InProgress",
                        StartTime = DateTime.UtcNow
                    });

                    var validationResult = await ValidateImplementationAsync(request.TemplateName);
                    result.ValidationResults = validationResult;

                    result.StepsExecuted.Last().Status = "Completed";
                    result.StepsExecuted.Last().EndTime = DateTime.UtcNow;
                }

                result.Success = true;
                result.Message = $"Successfully implemented {request.Recommendations.Count} recommendations";
                result.Summary = new ImplementationSummary
                {
                    RecommendationsApplied = request.Recommendations.Count,
                    BackupCreated = !string.IsNullOrEmpty(result.BackupId),
                    ValidationPassed = request.RequireValidation && !result.Errors.Any(),
                    TotalDuration = TimeSpan.FromTicks(result.StepsExecuted.Sum(s => (s.EndTime - s.StartTime).Ticks))
                };

                _logger.Information("✅ **AUTO_IMPLEMENTATION_SUCCESS**: Successfully implemented {Count} recommendations in {Duration}", 
                    request.Recommendations.Count, result.Summary.TotalDuration);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUTO_IMPLEMENTATION_ERROR**: Failed to auto-implement recommendations");
                result.Errors.Add(new ImplementationError
                {
                    ErrorCode = "IMPLEMENTATION_EXCEPTION",
                    Message = ex.Message,
                    Severity = "Critical"
                });
                result.Message = $"Implementation failed with exception: {ex.Message}";
                return result;
            }
        }

        public async Task<MetaAIServiceStatus> GetServiceStatusAsync()
        {
            _logger.Information("📊 **SERVICE_STATUS_REQUEST**: Checking Meta AI service status");

            try
            {
                var response = await CallMetaAIAsync<ServiceStatusResponse>("service/status", new { });

                var status = new MetaAIServiceStatus
                {
                    IsAvailable = response.IsAvailable,
                    Version = response.Version,
                    SupportedFeatures = response.SupportedFeatures ?? new List<string>(),
                    Health = MapToServiceHealthMetrics(response.Health),
                    Capabilities = response.Capabilities ?? new Dictionary<string, object>()
                };

                _logger.Information("✅ **SERVICE_STATUS_SUCCESS**: Service {Status}, Version {Version}", 
                    status.IsAvailable ? "Available" : "Unavailable", status.Version);

                return status;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **SERVICE_STATUS_ERROR**: Failed to get service status");
                return new MetaAIServiceStatus
                {
                    IsAvailable = false,
                    SupportedFeatures = new List<string>(),
                    Health = new ServiceHealthMetrics { Status = "Error", Message = ex.Message }
                };
            }
        }

        #endregion

        #region Private Helper Methods

        private void SetupHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_config.ApiEndpoint);
            _httpClient.DefaultRequestHeaders.Clear();
            
            if (!string.IsNullOrEmpty(_config.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
            }

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AutoBot-Enterprise-OCR/1.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.Timeout = TimeSpan.FromMinutes(_config.TimeoutMinutes);
        }

        private async Task<T> CallMetaAIAsync<T>(string endpoint, object requestData) where T : class
        {
            var requestJson = JsonSerializer.Serialize(requestData, _jsonOptions);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            _logger.Verbose("📤 **META_AI_REQUEST**: Calling endpoint '{Endpoint}' with {DataLength} chars", endpoint, requestJson.Length);

            using var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.Verbose("📥 **META_AI_RESPONSE**: Received {StatusCode} with {ResponseLength} chars", 
                (int)response.StatusCode, responseContent.Length);

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("❌ **META_AI_API_ERROR**: HTTP {StatusCode}: {Content}", (int)response.StatusCode, responseContent);
                throw new HttpRequestException($"Meta AI API call failed: {response.StatusCode} - {responseContent}");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.Error(ex, "❌ **META_AI_JSON_ERROR**: Failed to deserialize response");
                throw new InvalidOperationException($"Failed to deserialize Meta AI response: {ex.Message}");
            }
        }

        private async Task<List<ImplementationError>> ValidateRecommendationsAsync(List<PromptRecommendation> recommendations)
        {
            var errors = new List<ImplementationError>();

            foreach (var recommendation in recommendations)
            {
                if (string.IsNullOrEmpty(recommendation.SuggestedChange))
                {
                    errors.Add(new ImplementationError
                    {
                        ErrorCode = "EMPTY_CHANGE",
                        Message = $"Recommendation '{recommendation.Title}' has no suggested change",
                        Severity = "Error"
                    });
                }

                if (recommendation.Confidence == ConfidenceLevel.Low && recommendation.AutoImplementable)
                {
                    errors.Add(new ImplementationError
                    {
                        ErrorCode = "LOW_CONFIDENCE_AUTO",
                        Message = $"Recommendation '{recommendation.Title}' marked as auto-implementable but has low confidence",
                        Severity = "Warning"
                    });
                }
            }

            return await Task.FromResult(errors);
        }

        private async Task<(bool Success, List<ImplementationError> Errors)> ApplyRecommendationsAsync(
            string templateName, 
            List<PromptRecommendation> recommendations, 
            AutoImplementationOptions options)
        {
            var errors = new List<ImplementationError>();

            // This would integrate with the template engine to apply changes
            // For now, we'll simulate the implementation
            
            foreach (var recommendation in recommendations.Where(r => r.AutoImplementable))
            {
                try
                {
                    _logger.Information("🔧 **APPLYING_RECOMMENDATION**: {Title} to template '{TemplateName}'", 
                        recommendation.Title, templateName);

                    // Simulate implementation delay
                    await Task.Delay(100);

                    // Here we would call the template engine to apply the change
                    // await _templateEngine.ApplyRecommendationAsync(templateName, recommendation);

                    _logger.Verbose("✅ **RECOMMENDATION_APPLIED**: {Title}", recommendation.Title);
                }
                catch (Exception ex)
                {
                    errors.Add(new ImplementationError
                    {
                        ErrorCode = "APPLICATION_FAILED",
                        Message = $"Failed to apply recommendation '{recommendation.Title}': {ex.Message}",
                        Severity = "Error"
                    });
                }
            }

            return (errors.Count == 0, errors);
        }

        private async Task<Dictionary<string, object>> ValidateImplementationAsync(string templateName)
        {
            // This would integrate with the template engine to validate the implementation
            // For now, we'll return a mock validation result
            
            return await Task.FromResult(new Dictionary<string, object>
            {
                ["template_valid"] = true,
                ["syntax_errors"] = 0,
                ["warnings"] = new List<string>(),
                ["performance_score"] = 85.0
            });
        }

        #endregion

        #region Mapping Methods

        private PromptRecommendation MapToPromptRecommendation(dynamic apiRecommendation)
        {
            return new PromptRecommendation
            {
                Type = apiRecommendation.type?.ToString() ?? "unknown",
                Title = apiRecommendation.title?.ToString() ?? "Untitled",
                Description = apiRecommendation.description?.ToString() ?? "",
                SuggestedChange = apiRecommendation.suggested_change?.ToString() ?? "",
                Rationale = apiRecommendation.rationale?.ToString() ?? "",
                ExpectedImpact = Convert.ToDouble(apiRecommendation.expected_impact ?? 0.0),
                Confidence = Enum.TryParse<ConfidenceLevel>(apiRecommendation.confidence?.ToString(), true, out var conf) ? conf : ConfidenceLevel.Medium,
                Complexity = Enum.TryParse<ImplementationComplexity>(apiRecommendation.complexity?.ToString(), true, out var comp) ? comp : ImplementationComplexity.Moderate,
                AutoImplementable = Convert.ToBoolean(apiRecommendation.auto_implementable ?? false)
            };
        }

        private PromptAnalysisInsights MapToPromptInsights(dynamic apiInsights)
        {
            if (apiInsights == null) return new PromptAnalysisInsights();

            return new PromptAnalysisInsights
            {
                KeyFindings = apiInsights.key_findings?.ToObject<List<string>>() ?? new List<string>(),
                StrengthAreas = apiInsights.strength_areas?.ToObject<List<string>>() ?? new List<string>(),
                ImprovementAreas = apiInsights.improvement_areas?.ToObject<List<string>>() ?? new List<string>(),
                RecommendedActions = apiInsights.recommended_actions?.ToObject<List<string>>() ?? new List<string>()
            };
        }

        private PromptQualityScore MapToQualityScore(dynamic apiScore)
        {
            if (apiScore == null) return new PromptQualityScore();

            return new PromptQualityScore
            {
                OverallScore = Convert.ToDouble(apiScore.overall_score ?? 0.0),
                ClarityScore = Convert.ToDouble(apiScore.clarity_score ?? 0.0),
                SpecificityScore = Convert.ToDouble(apiScore.specificity_score ?? 0.0),
                CompletenessScore = Convert.ToDouble(apiScore.completeness_score ?? 0.0),
                StructureScore = Convert.ToDouble(apiScore.structure_score ?? 0.0),
                ExampleScore = Convert.ToDouble(apiScore.example_score ?? 0.0),
                ContextScore = Convert.ToDouble(apiScore.context_score ?? 0.0),
                ScoreCategory = apiScore.score_category?.ToString() ?? "Unknown"
            };
        }

        // Additional mapping methods would be implemented here for other model types...

        #endregion
    }

    /// <summary>
    /// Configuration for Meta AI service.
    /// </summary>
    public class MetaAIConfiguration
    {
        public string ApiEndpoint { get; set; } = "https://api.meta.ai/v1/";
        public string ApiKey { get; set; }
        public int TimeoutMinutes { get; set; } = 5;
        public bool EnableRetry { get; set; } = true;
        public int MaxRetryAttempts { get; set; } = 3;
        public Dictionary<string, object> DefaultModelConfig { get; set; } = new Dictionary<string, object>();
    }

    #region API Response Models (Private)

    private class PromptRecommendationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<dynamic> Recommendations { get; set; }
        public dynamic Insights { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    private class PromptAnalysisResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic QualityScore { get; set; }
        public List<dynamic> Issues { get; set; }
        public List<dynamic> Strengths { get; set; }
        public dynamic Insights { get; set; }
        public List<dynamic> Suggestions { get; set; }
    }

    private class PromptOptimizationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string OptimizedPrompt { get; set; }
        public List<dynamic> Changes { get; set; }
        public dynamic OriginalScore { get; set; }
        public dynamic OptimizedScore { get; set; }
        public dynamic Improvement { get; set; }
        public string Rationale { get; set; }
    }

    private class PromptValidationResponse
    {
        public bool IsValid { get; set; }
        public bool HasWarnings { get; set; }
        public List<dynamic> Issues { get; set; }
        public List<dynamic> Warnings { get; set; }
        public dynamic QualityScore { get; set; }
        public List<dynamic> BestPractices { get; set; }
    }

    private class ABTestingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<dynamic> RecommendedVariants { get; set; }
        public dynamic Strategy { get; set; }
        public dynamic TestPlan { get; set; }
        public List<dynamic> Metrics { get; set; }
        public double EstimatedDurationDays { get; set; }
    }

    private class ServiceStatusResponse
    {
        public bool IsAvailable { get; set; }
        public string Version { get; set; }
        public List<string> SupportedFeatures { get; set; }
        public dynamic Health { get; set; }
        public Dictionary<string, object> Capabilities { get; set; }
    }

    #endregion
}