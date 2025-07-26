// File: OCRCorrectionService/MetaAI/MetaAIModels.cs
using System;
using System.Collections.Generic;

namespace WaterNut.DataSpace.MetaAI
{
    #region Core Enums

    /// <summary>
    /// Types of changes that can be recommended by Meta AI.
    /// Based on established test patterns and OCR-specific requirements.
    /// </summary>
    public enum ChangeType
    {
        PromptOptimization,
        RegexImprovement,
        InstructionClarification,
        FieldMappingCorrection,
        ExampleAddition,
        ContextualImprovement,
        StructureImprovement,
        PerformanceOptimization
    }

    #endregion

    #region Enhanced Recommendation Models


    #endregion

    #region OCR-Specific Models

    /// <summary>
    /// OCR regex validation result for ensuring patterns meet production requirements.
    /// Based on MANGO test analysis and named capture group requirements.
    /// </summary>
    public class OCRRegexValidationResult
    {
        public string Pattern { get; set; }
        public bool IsValid { get; set; } = true;
        public string PatternType { get; set; } = "General";
        public List<string> Issues { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasNamedGroups { get; set; }
        public bool HasExcessiveEscaping { get; set; }
        public List<string> ExtractedFieldNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// OCR-specific template context for Meta AI optimization.
    /// Includes supplier-specific patterns and test data references.
    /// </summary>
    public class OCRTemplateContext
    {
        public string SupplierName { get; set; }
        public List<string> OCRSections { get; set; } = new List<string>();
        public Dictionary<string, object> TestData { get; set; } = new Dictionary<string, object>();
        public List<string> RequiredFields { get; set; } = new List<string>();
        public string InvoiceType { get; set; }
        public bool RequiresNamedCaptureGroups { get; set; } = true;
        public bool RequiresJSONCompliance { get; set; } = true;
    }

    #endregion

    #region Supporting Models from Interface Analysis

    /// <summary>
    /// Prompt analysis insights from Meta AI.
    /// </summary>
    public class PromptAnalysisInsights
    {
        public List<string> KeyFindings { get; set; } = new List<string>();
        public List<string> StrengthAreas { get; set; } = new List<string>();
        public List<string> ImprovementAreas { get; set; } = new List<string>();
        public List<string> RecommendedActions { get; set; } = new List<string>();
    }

    /// <summary>
    /// Prompt issue identified during analysis.
    /// </summary>
    public class PromptIssue
    {
        public string IssueType { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public string Location { get; set; }
        public string SuggestedFix { get; set; }
    }

    /// <summary>
    /// Prompt strength identified during analysis.
    /// </summary>
    public class PromptStrength
    {
        public string StrengthType { get; set; }
        public string Message { get; set; }
        public string Impact { get; set; }
    }

    /// <summary>
    /// Improvement suggestion from Meta AI.
    /// </summary>
    public class ImprovementSuggestion
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public double ExpectedImprovement { get; set; }
    }

    /// <summary>
    /// Optimization change details.
    /// </summary>
    public class OptimizationChange
    {
        public string ChangeType { get; set; }
        public string BeforeText { get; set; }
        public string AfterText { get; set; }
        public string Rationale { get; set; }
    }

    /// <summary>
    /// Estimated performance improvement from optimization.
    /// </summary>
    public class EstimatedPerformanceImprovement
    {
        public double AccuracyImprovement { get; set; }
        public double ConsistencyImprovement { get; set; }
        public double ClarityImprovement { get; set; }
        public double OverallImprovement { get; set; }
    }

    /// <summary>
    /// Validation issue from prompt validation.
    /// </summary>
    public class ValidationIssue
    {
        public string IssueType { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public string Location { get; set; }
    }

    /// <summary>
    /// Validation warning from prompt validation.
    /// </summary>
    public class ValidationWarning
    {
        public string WarningType { get; set; }
        public string Message { get; set; }
        public string Recommendation { get; set; }
    }

    /// <summary>
    /// Best practice recommendation.
    /// </summary>
    public class BestPracticeRecommendation
    {
        public string Practice { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Impact { get; set; }
    }

    /// <summary>
    /// Test variant for A/B testing.
    /// </summary>
    public class TestVariant
    {
        public string VariantId { get; set; }
        public string VariantName { get; set; }
        public string VariantPrompt { get; set; }
        public string VariantDescription { get; set; }
        public Dictionary<string, object> VariantMetadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Testing strategy for A/B tests.
    /// </summary>
    public class TestingStrategy
    {
        public string StrategyType { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Statistical test plan for A/B testing.
    /// </summary>
    public class StatisticalTestPlan
    {
        public string TestType { get; set; }
        public double SignificanceLevel { get; set; }
        public int MinimumSampleSize { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string PowerAnalysis { get; set; }
    }

    /// <summary>
    /// Metrics to track during A/B testing.
    /// </summary>
    public class MetricsToTrack
    {
        public string MetricName { get; set; }
        public string MetricType { get; set; }
        public string Description { get; set; }
        public string CalculationMethod { get; set; }
    }

    /// <summary>
    /// Implementation step for auto-implementation.
    /// </summary>
    public class ImplementationStep
    {
        public string StepName { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> StepData { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Implementation error during auto-implementation.
    /// </summary>
    public class ImplementationError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public string StepName { get; set; }
        public Dictionary<string, object> ErrorData { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Implementation summary.
    /// </summary>
    public class ImplementationSummary
    {
        public int RecommendationsApplied { get; set; }
        public bool BackupCreated { get; set; }
        public bool ValidationPassed { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public Dictionary<string, object> SummaryMetrics { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Service health metrics.
    /// </summary>
    public class ServiceHealthMetrics
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public double ResponseTimeMs { get; set; }
        public DateTime LastChecked { get; set; }
        public Dictionary<string, object> HealthData { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Auto-implementation options.
    /// </summary>
    public class AutoImplementationOptions
    {
        public bool CreateBackup { get; set; } = true;
        public bool ValidateAfterImplementation { get; set; } = true;
        public bool EnableRollbackOnFailure { get; set; } = true;
        public List<string> ExcludedChangeTypes { get; set; } = new List<string>();
        public Dictionary<string, object> CustomOptions { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Testing objectives for A/B testing.
    /// </summary>
    public class TestingObjectives
    {
        public List<string> PrimaryObjectives { get; set; } = new List<string>();
        public List<string> SecondaryObjectives { get; set; } = new List<string>();
        public Dictionary<string, double> ObjectiveWeights { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Optimization constraint for prompt optimization.
    /// </summary>
    public class OptimizationConstraint
    {
        public string ConstraintType { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
        public string Rationale { get; set; }
    }

    /// <summary>
    /// Validation rule for prompt validation.
    /// </summary>
    public class ValidationRule
    {
        public string RuleName { get; set; }
        public string RuleType { get; set; }
        public string Pattern { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
    }

    #endregion

    #region MANGO-Specific Models

    /// <summary>
    /// MANGO supplier-specific context and requirements.
    /// Based on test case analysis: CanImportMango03152025TotalAmount_AfterLearning.
    /// </summary>
    public class MANGOSpecificContext : OCRTemplateContext
    {
        public MANGOSpecificContext()
        {
            SupplierName = "MANGO";
            OCRSections = new List<string> { "Single Column", "Ripped Text", "SparseText" };
            RequiredFields = new List<string> { "TOTAL AMOUNT", "Currency", "InvoiceDate", "SupplierName" };
            InvoiceType = "Fashion Retailer";
            
            TestData = new Dictionary<string, object>
            {
                ["targetField"] = "TOTAL AMOUNT",
                ["expectedDate"] = "03152025",
                ["expectedTotal"] = 210.08,
                ["currencyFormat"] = "EUR with comma decimals",
                ["specialPatterns"] = new List<string>
                {
                    "European currency formatting",
                    "Fashion item descriptions with ref. codes",
                    "Multi-line address parsing"
                }
            };
        }
    }

    #endregion
}