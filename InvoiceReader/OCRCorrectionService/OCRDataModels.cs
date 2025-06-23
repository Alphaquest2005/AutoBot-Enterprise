// File: OCRCorrectionService/OCRDataModels.cs
using System;
using System.Linq;
using System.Collections.Generic;
using global::EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For DB entities like Invoice, Fields, Lines, etc.

namespace WaterNut.DataSpace
{
    #region Core Correction and Error Models

    public class CorrectionResult
    {
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

        // =================================== FIX START ===================================
        /// <summary>
        /// For format corrections, this is the regex pattern to FIND the incorrect format.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// For format corrections, this is the string to REPLACE the found pattern with.
        /// </summary>
        public string Replacement { get; set; }
        // ==================================== FIX END ====================================

        public string FullContext =>
            string.Join(
                "\n",
                ContextLinesBefore.Concat(new[] { $"Line {LineNumber}: {LineText}" }).Concat(ContextLinesAfter));
    }


    public class InvoiceError
    {
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

        /// <summary>
        /// A regex pattern suggested by the detection phase that can be used for de-duplication and learning.
        /// </summary>
        public string SuggestedRegex { get; set; }

        // =================================== FIX START ===================================
        /// <summary>
        /// For format corrections, this is the regex pattern to FIND the incorrect format.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// For format corrections, this is the string to REPLACE the found pattern with.
        /// </summary>
        public string Replacement { get; set; }
        // ==================================== FIX END ====================================
    }
    #endregion

    #region Database Interaction Models

    public class DatabaseUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? RecordId { get; set; }
        public int? RelatedRecordId { get; set; } // ADD THIS LINE
        public string Operation { get; set; }
        public Exception Exception { get; set; }

        public static DatabaseUpdateResult Success(int recordId, string operation, int? relatedRecordId = null) => // MODIFY THIS LINE
            new DatabaseUpdateResult
                {
                    IsSuccess = true,
                    RecordId = recordId,
                    Operation = operation,
                    Message = $"Successfully {operation} (ID: {recordId})",
                    RelatedRecordId = relatedRecordId // ADD THIS LINE
                };

        public static DatabaseUpdateResult Failed(string message, Exception ex = null) =>
            new DatabaseUpdateResult { IsSuccess = false, Message = message, Exception = ex };
    }


    public class RegexUpdateRequest
    {
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
        public int? RegexId { get; set; }
        public string ExistingRegex { get; set; }
        public string PartName { get; set; }
        public int? InvoiceId { get; set; }

        // =================================== FIX START ===================================
        /// <summary>
        /// For format corrections, this is the regex pattern to FIND the incorrect format.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// For format corrections, this is the string to REPLACE the found pattern with.
        /// </summary>
        public string Replacement { get; set; }
        // ==================================== FIX END ====================================
    }

    #endregion

    #region OCR Template and Extraction Metadata Models

    public class OCRFieldMetadata
    {
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
        public int? FormatRegexId { get; set; }
        public int? RegexId { get; set; }
        public int? ReplacementRegexId { get; set; }
        public string Pattern { get; set; }
        public string Replacement { get; set; }
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
        public int FieldId { get; set; }
        public string Key { get; set; }
        public string Field { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool? IsRequired { get; set; }
    }

    public class RegexCreationResponse
    {
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
        public int? InvoiceId { get; set; }
        public string InvoiceName { get; set; }
    }

    public class PartContext
    {
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }
    }

    public class RegexPattern
    {
        public string FieldName { get; set; }
        public string StrategyType { get; set; }
        public string Pattern { get; set; }
        public string Replacement { get; set; }
        public double Confidence { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UpdateCount { get; set; }
        public string CreatedBy { get; set; }
    }

    #endregion
}