// File: OCRCorrectionService/MetaAI/IMetaAIService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.MetaAI
{
    /// <summary>
    /// Meta AI service interface for prompt optimization and recommendation.
    /// Provides intelligent prompt enhancement and automatic template improvements.
    /// </summary>
    public interface IMetaAIService
    {
        /// <summary>
        /// Gets prompt recommendations from Meta AI based on current template performance.
        /// </summary>
        Task<PromptRecommendationResult> GetPromptRecommendationsAsync(PromptRecommendationRequest request);

        /// <summary>
        /// Analyzes prompt effectiveness and suggests improvements.
        /// </summary>
        Task<PromptAnalysisResult> AnalyzePromptEffectivenessAsync(PromptAnalysisRequest request);

        /// <summary>
        /// Optimizes existing prompt template using Meta AI insights.
        /// </summary>
        Task<PromptOptimizationResult> OptimizePromptAsync(PromptOptimizationRequest request);

        /// <summary>
        /// Validates prompt template against Meta AI best practices.
        /// </summary>
        Task<PromptValidationResult> ValidatePromptAsync(PromptValidationRequest request);

        /// <summary>
        /// Gets A/B testing recommendations for prompt variations.
        /// </summary>
        Task<ABTestingRecommendation> GetABTestingRecommendationsAsync(ABTestingRequest request);

        /// <summary>
        /// Automatically applies recommended improvements to template.
        /// </summary>
        Task<AutoImplementationResult> AutoImplementRecommendationsAsync(AutoImplementationRequest request);

        /// <summary>
        /// Gets service health and capabilities.
        /// </summary>
        Task<MetaAIServiceStatus> GetServiceStatusAsync();
    }

    #region Request Models

    /// <summary>
    /// Request for prompt recommendations from Meta AI.
    /// </summary>
    public class PromptRecommendationRequest
    {
        public string CurrentPrompt { get; set; }
        public string PromptCategory { get; set; } // "header_detection", "product_detection", "direct_correction"
        public PromptPerformanceMetrics PerformanceMetrics { get; set; }
        public List<string> FailureExamples { get; set; } = new List<string>();
        public List<string> SuccessExamples { get; set; } = new List<string>();
        public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
        public PromptOptimizationGoals Goals { get; set; } = new PromptOptimizationGoals();
    }

    /// <summary>
    /// Request for prompt analysis and effectiveness assessment.
    /// </summary>
    public class PromptAnalysisRequest
    {
        public string Prompt { get; set; }
        public string TargetDomain { get; set; } // "OCR", "invoice_processing", "template_creation"
        public List<TestCase> TestCases { get; set; } = new List<TestCase>();
        public AnalysisDepth Depth { get; set; } = AnalysisDepth.Standard;
        public List<string> FocusAreas { get; set; } = new List<string>(); // "clarity", "specificity", "examples", "structure"
    }

    /// <summary>
    /// Request for prompt optimization using Meta AI.
    /// </summary>
    public class PromptOptimizationRequest
    {
        public string OriginalPrompt { get; set; }
        public PromptOptimizationGoals Goals { get; set; }
        public List<OptimizationConstraint> Constraints { get; set; } = new List<OptimizationConstraint>();
        public PromptPerformanceMetrics CurrentMetrics { get; set; }
        public string TargetModel { get; set; } = "DeepSeek"; // "DeepSeek", "Gemini", "Generic"
        public bool PreserveEssentialStructure { get; set; } = true;
    }

    /// <summary>
    /// Request for prompt validation against best practices.
    /// </summary>
    public class PromptValidationRequest
    {
        public string Prompt { get; set; }
        public string PromptType { get; set; } // "instruction", "few_shot", "chain_of_thought"
        public List<ValidationRule> CustomRules { get; set; } = new List<ValidationRule>();
        public ValidationStrictness Strictness { get; set; } = ValidationStrictness.Standard;
        public bool CheckForBias { get; set; } = true;
        public bool CheckForSafety { get; set; } = true;
    }

    /// <summary>
    /// Request for A/B testing recommendations.
    /// </summary>
    public class ABTestingRequest
    {
        public string BaselinePrompt { get; set; }
        public List<string> VariantPrompts { get; set; } = new List<string>();
        public TestingObjectives Objectives { get; set; }
        public int MinimumSampleSize { get; set; } = 100;
        public double StatisticalSignificanceLevel { get; set; } = 0.05;
        public TimeSpan MaxTestDuration { get; set; } = TimeSpan.FromDays(7);
    }

    /// <summary>
    /// Request for automatic implementation of recommendations.
    /// </summary>
    public class AutoImplementationRequest
    {
        public string TemplateName { get; set; }
        public List<PromptRecommendation> Recommendations { get; set; } = new List<PromptRecommendation>();
        public AutoImplementationOptions Options { get; set; } = new AutoImplementationOptions();
        public string BackupStrategy { get; set; } = "create_backup"; // "create_backup", "version_control", "none"
        public bool RequireValidation { get; set; } = true;
        public bool EnableRollback { get; set; } = true;
    }

    #endregion

    #region Response Models

    /// <summary>
    /// Result of prompt recommendation request.
    /// </summary>
    public class PromptRecommendationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<PromptRecommendation> Recommendations { get; set; } = new List<PromptRecommendation>();
        public PromptAnalysisInsights Insights { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string RecommendationId { get; set; } = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Result of prompt analysis.
    /// </summary>
    public class PromptAnalysisResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PromptQualityScore QualityScore { get; set; }
        public List<PromptIssue> Issues { get; set; } = new List<PromptIssue>();
        public List<PromptStrength> Strengths { get; set; } = new List<PromptStrength>();
        public PromptAnalysisInsights Insights { get; set; }
        public List<ImprovementSuggestion> Suggestions { get; set; } = new List<ImprovementSuggestion>();
    }

    /// <summary>
    /// Result of prompt optimization.
    /// </summary>
    public class PromptOptimizationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string OptimizedPrompt { get; set; }
        public List<OptimizationChange> Changes { get; set; } = new List<OptimizationChange>();
        public PromptQualityScore OriginalScore { get; set; }
        public PromptQualityScore OptimizedScore { get; set; }
        public EstimatedPerformanceImprovement Improvement { get; set; }
        public string OptimizationRationale { get; set; }
    }

    /// <summary>
    /// Result of prompt validation.
    /// </summary>
    public class PromptValidationResult
    {
        public bool IsValid { get; set; }
        public bool HasWarnings { get; set; }
        public List<ValidationIssue> Issues { get; set; } = new List<ValidationIssue>();
        public List<ValidationWarning> Warnings { get; set; } = new List<ValidationWarning>();
        public PromptQualityScore QualityScore { get; set; }
        public List<BestPracticeRecommendation> BestPractices { get; set; } = new List<BestPracticeRecommendation>();
    }

    /// <summary>
    /// A/B testing recommendations.
    /// </summary>
    public class ABTestingRecommendation
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<TestVariant> RecommendedVariants { get; set; } = new List<TestVariant>();
        public TestingStrategy Strategy { get; set; }
        public StatisticalTestPlan TestPlan { get; set; }
        public List<MetricsToTrack> Metrics { get; set; } = new List<MetricsToTrack>();
        public TimeSpan EstimatedTestDuration { get; set; }
    }

    /// <summary>
    /// Result of automatic implementation.
    /// </summary>
    public class AutoImplementationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ImplementationStep> StepsExecuted { get; set; } = new List<ImplementationStep>();
        public List<ImplementationError> Errors { get; set; } = new List<ImplementationError>();
        public string BackupId { get; set; }
        public bool RollbackAvailable { get; set; }
        public ImplementationSummary Summary { get; set; }
        public Dictionary<string, object> ValidationResults { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Meta AI service status and capabilities.
    /// </summary>
    public class MetaAIServiceStatus
    {
        public bool IsAvailable { get; set; }
        public string Version { get; set; }
        public List<string> SupportedFeatures { get; set; } = new List<string>();
        public ServiceHealthMetrics Health { get; set; }
        public Dictionary<string, object> Capabilities { get; set; } = new Dictionary<string, object>();
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region Supporting Data Models

    /// <summary>
    /// Performance metrics for prompt evaluation.
    /// </summary>
    public class PromptPerformanceMetrics
    {
        public double SuccessRate { get; set; }
        public double AverageResponseTime { get; set; }
        public double AccuracyScore { get; set; }
        public double ConsistencyScore { get; set; }
        public int TotalExecutions { get; set; }
        public int SuccessfulExecutions { get; set; }
        public List<string> CommonFailurePatterns { get; set; } = new List<string>();
        public Dictionary<string, double> MetricBreakdown { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Optimization goals for prompt improvement.
    /// </summary>
    public class PromptOptimizationGoals
    {
        public bool ImproveAccuracy { get; set; } = true;
        public bool ReduceResponseTime { get; set; } = false;
        public bool IncreaseConsistency { get; set; } = true;
        public bool ReduceTokenUsage { get; set; } = false;
        public bool ImproveClarity { get; set; } = true;
        public bool ReduceHallucination { get; set; } = true;
        public List<string> CustomGoals { get; set; } = new List<string>();
        public Dictionary<string, double> GoalWeights { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Individual prompt recommendation.
    /// </summary>
    public class PromptRecommendation
    {
        public string RecommendationId { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } // "structure", "content", "examples", "formatting"
        public string Title { get; set; }
        public string Description { get; set; }
        public string SuggestedChange { get; set; }
        public string Rationale { get; set; }
        public double ExpectedImpact { get; set; } // 0.0 to 1.0
        public ConfidenceLevel Confidence { get; set; }
        public ImplementationComplexity Complexity { get; set; }
        public bool AutoImplementable { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Test case for prompt analysis.
    /// </summary>
    public class TestCase
    {
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
        public string ActualOutput { get; set; }
        public bool Passed { get; set; }
        public string Category { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Prompt quality scoring.
    /// </summary>
    public class PromptQualityScore
    {
        public double OverallScore { get; set; } // 0.0 to 100.0
        public double ClarityScore { get; set; }
        public double SpecificityScore { get; set; }
        public double CompletenessScore { get; set; }
        public double StructureScore { get; set; }
        public double ExampleScore { get; set; }
        public double ContextScore { get; set; }
        public Dictionary<string, double> DetailedScores { get; set; } = new Dictionary<string, double>();
        public string ScoreCategory { get; set; } // "Poor", "Fair", "Good", "Excellent"
    }

    #endregion

    #region Enums

    public enum AnalysisDepth
    {
        Quick,
        Standard,
        Comprehensive,
        Deep
    }

    public enum ValidationStrictness
    {
        Relaxed,
        Standard,
        Strict,
        Rigorous
    }

    public enum ConfidenceLevel
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    public enum ImplementationComplexity
    {
        Simple,
        Moderate,
        Complex,
        RequiresReview
    }

    #endregion
}