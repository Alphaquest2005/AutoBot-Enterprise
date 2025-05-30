// File: OCRCorrectionService/OCRDataModels.cs
using System;

namespace WaterNut.DataSpace
{
    using System.Collections.Generic;

    using global::EntryDataDS.Business.Entities;

    #region Data Models

    /// <summary>
    /// Result of applying an OCR correction
    /// </summary>
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
    }

    /// <summary>
    /// Result of a database update operation
    /// </summary>
    public class DatabaseUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? RecordId { get; set; }
        public string Operation { get; set; }
        public Exception Exception { get; set; }

        public static DatabaseUpdateResult Success(int recordId, string operation)
        {
            return new DatabaseUpdateResult
            {
                IsSuccess = true,
                RecordId = recordId,
                Operation = operation,
                Message = $"Successfully {operation.ToLower()} record {recordId}"
            };
        }

        public static DatabaseUpdateResult Failed(string message, Exception exception = null)
        {
            return new DatabaseUpdateResult
            {
                IsSuccess = false,
                Message = message,
                Exception = exception
            };
        }
    }

    /// <summary>
    /// Represents an OCR error detected in invoice data
    /// </summary>
    public class InvoiceError
    {
        public string Field { get; set; }
        public string ExtractedValue { get; set; }
        public string CorrectValue { get; set; }
        public double Confidence { get; set; }
        public string ErrorType { get; set; }
        public string Reasoning { get; set; }
    }

    /// <summary>
    /// Enhanced LineInfo with confidence and reasoning
    /// </summary>
    public class LineInfo
    {
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
    }

    /// <summary>
    /// Regex correction strategy determined by DeepSeek
    /// </summary>
    public class RegexCorrectionStrategy
    {
        public string StrategyType { get; set; } // FORMAT_FIX, PATTERN_UPDATE, CHARACTER_MAP, VALIDATION_RULE
        public string RegexPattern { get; set; }
        public string ReplacementPattern { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
    }

    /// <summary>
    /// Request for updating regex patterns
    /// </summary>
    public class RegexUpdateRequest
    {
        public string FieldName { get; set; }
        public string CorrectionType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public RegexCorrectionStrategy Strategy { get; set; }
        public double Confidence { get; set; }
    }

    /// <summary>
    /// Data model for regex pattern storage
    /// </summary>
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

    /// <summary>
    /// OCR metadata for tracking field extraction details
    /// </summary>
    public class OCRFieldMetadata
    {
        public int LineNumber { get; set; }
        public int? FieldId { get; set; }
        public int? LineId { get; set; }
        public int? RegexId { get; set; }
        public string Section { get; set; }
        public string Instance { get; set; }
        public string RawValue { get; set; }
        public string FieldName { get; set; }
    }

    /// <summary>
    /// Enhanced ShipmentInvoice with OCR metadata for database updates
    /// </summary>
    public class ShipmentInvoiceWithMetadata
    {
        public ShipmentInvoice Invoice { get; set; }
        public Dictionary<string, OCRFieldMetadata> FieldMetadata { get; set; } = new Dictionary<string, OCRFieldMetadata>();

        /// <summary>
        /// Gets OCR metadata for a specific field
        /// </summary>
        public OCRFieldMetadata GetFieldMetadata(string fieldName)
        {
            return FieldMetadata.TryGetValue(fieldName, out var metadata) ? metadata : null;
        }

        /// <summary>
        /// Sets OCR metadata for a specific field
        /// </summary>
        public void SetFieldMetadata(string fieldName, OCRFieldMetadata metadata)
        {
            FieldMetadata[fieldName] = metadata;
        }
    }

    /// <summary>
    /// Enumeration of OCR error types
    /// </summary>
    public enum OCRErrorType
    {
        DecimalSeparator,    // Comma vs period confusion
        CharacterConfusion,  // 1/l, 0/O, etc.
        MissingDigit,       // Missing or extra digits
        FormatError,        // General formatting issues
        FieldMismatch,      // Wrong field mapping
        CalculationError,   // Mathematical inconsistencies
        UnreasonableValue   // Values that don't make sense
    }

    #endregion
}