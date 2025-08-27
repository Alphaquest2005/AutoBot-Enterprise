// File: OCRCorrectionService/OCRDataModels.cs
using System;
using System.Linq;
using System.Collections.Generic;
using global::EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For DB entities like Invoice, Fields, Lines, etc.
using Serilog;

namespace WaterNut.DataSpace
{
    // Static logger for use within the data models themselves.
    public static class ModelLogger
    {
        public static readonly ILogger Logger = Serilog.Log.ForContext("SourceContext", "DataModels");
    }

    #region Core Correction and Error Models

    public class CorrectionResult
    {
        private string _pattern;
        private string _replacement;

        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string CorrectionType { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public List<string> ContextLinesBefore { get; set; } = new List<string>();
        public List<string> ContextLinesAfter { get; set; } = new List<string>();
        public bool RequiresMultilineRegex { get; set; }
        public string SuggestedRegex { get; set; }
        public string ExistingRegex { get; set; }
        public int? LineId { get; set; }
        public int? PartId { get; set; }
        public int? RegexId { get; set; }
        public string WindowText { get; set; }

        public string Pattern
        {
            get => _pattern;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: CorrectionResult.Pattern = '{Value}'", value);
                _pattern = value;
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: CorrectionResult.Replacement = '{Value}'", value);
                _replacement = value;
            }
        }

        public string FullContext =>
            string.Join(
                "\n",
                ContextLinesBefore.Concat(new[] { $"Line {LineNumber}: {LineText}" }).Concat(ContextLinesAfter));
    }

    public class InvoiceError
    {
        private string _pattern;
        private string _replacement;

        public string Field { get; set; }
        public string ExtractedValue { get; set; }
        public string CorrectValue { get; set; }
        public double Confidence { get; set; }
        public string ErrorType { get; set; }
        public string Reasoning { get; set; }
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public List<string> ContextLinesBefore { get; set; } = new List<string>();
        public List<string> ContextLinesAfter { get; set; } = new List<string>();
        public bool RequiresMultilineRegex { get; set; }
        public string SuggestedRegex { get; set; }
        
        // Multi-field extraction support
        public List<string> CapturedFields { get; set; } = new List<string>();
        public List<FieldCorrection> FieldCorrections { get; set; } = new List<FieldCorrection>();
        
        // Transformation chain support for grouped/linked errors
        public string GroupId { get; set; }  // Links related errors together
        public int SequenceOrder { get; set; } // Order within the group (1, 2, 3...)
        public string TransformationInput { get; set; } // "ocr_text" or "previous_output" or specific field name

        public string Pattern
        {
            get => _pattern;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: InvoiceError.Pattern = '{Value}'", value);
                _pattern = value;
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: InvoiceError.Replacement = '{Value}'", value);
                _replacement = value;
            }
        }
    }

    /// <summary>
    /// Represents a format correction to be applied to a specific field within a multi-field extraction
    /// </summary>
    public class FieldCorrection
    {
        public string FieldName { get; set; }
        public string Pattern { get; set; }
        public string Replacement { get; set; }
    }

    #endregion

    #region Database Interaction Models

    public partial class DatabaseUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? RecordId { get; set; }
        public int? RelatedRecordId { get; set; }
        public string Operation { get; set; }
        public Exception Exception { get; set; }
        
        // Template creation tracking properties
        public bool TemplateCreated { get; set; }
        public int PartsCreated { get; set; }
        public int LinesCreated { get; set; }
        public int FieldsCreated { get; set; }
        public int FormatCorrectionsCreated { get; set; }
        public int? RegexId { get; set; }

        public static DatabaseUpdateResult Success(int recordId, string operation, int? relatedRecordId = null) =>
            new DatabaseUpdateResult
            {
                IsSuccess = true,
                RecordId = recordId,
                Operation = operation,
                Message = $"Successfully {operation} (ID: {recordId})",
                RelatedRecordId = relatedRecordId
            };

        public static DatabaseUpdateResult Failed(string message, Exception ex = null) =>
            new DatabaseUpdateResult { IsSuccess = false, Message = message, Exception = ex };
    }

    public partial class RegexUpdateRequest
    {
        private string _pattern;
        private string _replacement;

        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string CorrectionType { get; set; }
        public double Confidence { get; set; }
        public string DeepSeekReasoning { get; set; }
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public string WindowText { get; set; }
        public List<string> ContextLinesBefore { get; set; } = new List<string>();
        public List<string> ContextLinesAfter { get; set; } = new List<string>();
        public bool RequiresMultilineRegex { get; set; }
        public string SuggestedRegex { get; set; }
        public string FilePath { get; set; }
        public string InvoiceType { get; set; }
        public int? LineId { get; set; }
        public int? PartId { get; set; }
        public int? FieldId { get; set; } // << NEW PROPERTY
        public int? RegexId { get; set; }
        public string ExistingRegex { get; set; }
        public string PartName { get; set; }
        public int? InvoiceId { get; set; }
        
        // Template creation properties
        public string TemplateName { get; set; }
        public bool CreateNewTemplate { get; set; }
        public string ErrorType { get; set; }
        public string ReasoningContext { get; set; }
        public List<InvoiceError> AllDeepSeekErrors { get; set; } = new List<InvoiceError>();

        public string Pattern
        {
            get => _pattern;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: RegexUpdateRequest.Pattern = '{Value}'", value);
                _pattern = value;
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: RegexUpdateRequest.Replacement = '{Value}'", value);
                _replacement = value;
            }
        }
    }

    #endregion

    #region OCR Template and Extraction Metadata Models

    public class OCRFieldMetadata
    {
        // ... (no changes needed here) ...
        public string FieldName { get; set; }
        public string Value { get; set; }
        public string RawValue { get; set; }
        public int LineNumber { get; set; }
        public int? FieldId { get; set; }
        public int? LineId { get; set; }
        public int? RegexId { get; set; }
        public string Key { get; set; }
        public string Field { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool? IsRequired { get; set; }
        public string LineName { get; set; }
        public string LineRegex { get; set; }
        public string LineText { get; set; }
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }
        public int? InvoiceId { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceType { get; set; }
        public List<FieldFormatRegexInfo> FormatRegexes { get; set; } = new List<FieldFormatRegexInfo>();
    }

    public class FieldFormatRegexInfo
    {
        private string _pattern;
        private string _replacement;

        public int? FormatRegexId { get; set; }
        public int? RegexId { get; set; }
        public int? ReplacementRegexId { get; set; }

        public string Pattern
        {
            get => _pattern;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: FieldFormatRegexInfo.Pattern = '{Value}'", value);
                _pattern = value;
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: FieldFormatRegexInfo.Replacement = '{Value}'", value);
                _replacement = value;
            }
        }
    }

    public class ShipmentInvoiceWithMetadata
    {
        public ShipmentInvoice Invoice { get; set; }
        public Dictionary<string, OCRFieldMetadata> FieldMetadata { get; set; } =
            new Dictionary<string, OCRFieldMetadata>();
    }

    public class EnhancedFieldMapping
    {
        public int LineId { get; set; }
        public int FieldId { get; set; }
        public int PartId { get; set; }
        public string RegexPattern { get; set; }
        public string Key { get; set; }
        public string FieldName { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
    }

    #endregion

    #region Context and Strategy Models for Advanced Processing

    public class LineContext
    {
        // ... (no changes needed here) ...
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public List<string> ContextLinesBefore { get; set; } = new List<string>();
        public List<string> ContextLinesAfter { get; set; } = new List<string>();
        public string WindowText { get; set; }
        public bool RequiresMultilineRegex { get; set; }
        public int? LineId { get; set; }
        public string LineName { get; set; }
        public string RegexPattern { get; set; }
        public int? RegexId { get; set; }
        public List<FieldInfo> FieldsInLine { get; set; } = new List<FieldInfo>();
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }
        public string FullContextWithLineNumbers =>
            string.Join(
                "\n",
                ContextLinesBefore.Concat(new[] { $">>> Line {LineNumber}: {LineText} <<<" })
                    .Concat(ContextLinesAfter));
    }

    public class FieldInfo
    {
        // ... (no changes needed here) ...
        public int FieldId { get; set; }
        public string Key { get; set; }
        public string Field { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool? IsRequired { get; set; }
    }

    public class RegexCreationResponse
    {
        // ... (no changes needed here) ...
        public string Strategy { get; set; }
        public string RegexPattern { get; set; }
        public string CompleteLineRegex { get; set; }
        public bool IsMultiline { get; set; }
        public int MaxLines { get; set; }
        public string TestMatch { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
        public bool PreservesExistingGroups { get; set; } = true;
    }

    public class InvoiceContext
    {
        // ... (no changes needed here) ...
        public int? InvoiceId { get; set; }
        public string InvoiceName { get; set; }
    }

    public class PartContext
    {
        // ... (no changes needed here) ...
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }
    }

    public class RegexPattern
    {
        private string _pattern;
        private string _replacement;

        public string FieldName { get; set; }
        public string StrategyType { get; set; }
        public double Confidence { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UpdateCount { get; set; }
        public string CreatedBy { get; set; }
        public string InvoiceType { get; set; } // Added for learning system

        public string Pattern
        {
            get => _pattern;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: RegexPattern.Pattern = '{Value}'", value);
                _pattern = value;
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                ModelLogger.Logger.Error("📝 **PROPERTY_SET**: RegexPattern.Replacement = '{Value}'", value);
                _replacement = value;
            }
        }
    }

    /// <summary>
    /// Learning analytics data model for OCR correction system insights
    /// Provides comprehensive statistics on OCR accuracy and improvement trends
    /// </summary>
    public class LearningAnalytics
    {
        public int PeriodDays { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public double AverageConfidence { get; set; }
        public Dictionary<string, int> MostCommonFields { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CorrectionTypes { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> InvoiceTypes { get; set; } = new Dictionary<string, int>();
        public List<TrendPoint> SuccessTrend { get; set; } = new List<TrendPoint>();
        public List<TrendPoint> ConfidenceTrend { get; set; } = new List<TrendPoint>();
        
        /// <summary>
        /// Success rate as percentage (0-100)
        /// </summary>
        public double SuccessRate => TotalRecords > 0 ? (double)SuccessfulRecords / TotalRecords * 100 : 0;
        
        /// <summary>
        /// Failure rate as percentage (0-100)
        /// </summary>
        public double FailureRate => TotalRecords > 0 ? (double)FailedRecords / TotalRecords * 100 : 0;
    }

    /// <summary>
    /// Represents a data point in trend analysis for learning analytics
    /// Used for tracking improvements over time
    /// </summary>
    public class TrendPoint
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int Count { get; set; }
        public string Label { get; set; }
    }

    #endregion

    #region Learning System Support Models

    /// <summary>
    /// Field validation information for request validation
    /// Used by OCR correction service to validate incoming requests
    /// </summary>
    public class FieldValidationInfo
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string FieldName { get; set; }
        public string DatabaseFieldName { get; set; }
        public string EntityType { get; set; }
        public bool IsRequired { get; set; }
    }

    /// <summary>
    /// Database field mapping information
    /// Maps DeepSeek field names to database schema
    /// </summary>
    public class DatabaseFieldMapping
    {
        public string DeepSeekFieldName { get; set; }
        public string DatabaseFieldName { get; set; }
        public string DisplayName { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
    }

    #endregion
}