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
    /// Enhanced OCR metadata for tracking field extraction details and enabling precise database updates
    /// </summary>
    public class OCRFieldMetadata
    {
        // Basic field information
        public string FieldName { get; set; }          // "TotalDeduction" (database destination field)
        public string Value { get; set; }              // "5.99" (extracted value)
        public string RawValue { get; set; }           // Original OCR text value

        // OCR Template Context (THE CRITICAL METADATA!)
        public int LineNumber { get; set; }            // Text line number where found
        public int? FieldId { get; set; }              // OCR-Fields.Id (unique field identifier)
        public int? LineId { get; set; }               // OCR-Lines.Id (line definition)
        public int? RegexId { get; set; }              // OCR-RegularExpressions.Id (regex pattern used)
        public string Key { get; set; }                // "Save" (regex capture group name)
        public string Field { get; set; }              // "TotalDeduction" (database field name)
        public string EntityType { get; set; }         // "Invoice" (target table)
        public string DataType { get; set; }           // "Number", "String", etc.

        // Line Context
        public string LineName { get; set; }           // "Buy More Save" (OCR-Lines.Name)
        public string LineRegex { get; set; }          // The regex pattern that matched this line
        public bool? IsRequired { get; set; }          // Whether field is required

        // Part Context
        public int? PartId { get; set; }               // OCR-Parts.Id
        public string PartName { get; set; }           // "Header", "InvoiceLine", etc.
        public int? PartTypeId { get; set; }           // OCR-PartTypes.Id

        // Invoice Context
        public int? InvoiceId { get; set; }            // OCR-Invoices.Id
        public string InvoiceName { get; set; }        // "Amazon"

        // Processing Context
        public string Section { get; set; }            // "Single", "Ripped", "Sparse"
        public string Instance { get; set; }           // Instance identifier
        public double? Confidence { get; set; }        // Extraction confidence

        // Format Regex Context
        public List<FieldFormatRegexInfo> FormatRegexes { get; set; } = new List<FieldFormatRegexInfo>();
    }

    /// <summary>
    /// Field format regex information for post-processing corrections
    /// </summary>
    public class FieldFormatRegexInfo
    {
        public int? FormatRegexId { get; set; }        // OCR-FieldFormatRegEx.Id
        public int? RegexId { get; set; }              // Pattern regex ID
        public int? ReplacementRegexId { get; set; }   // Replacement regex ID
        public string Pattern { get; set; }            // Regex pattern
        public string Replacement { get; set; }        // Replacement pattern
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

    /// <summary>
    /// Enhanced correction processing result with metadata context
    /// </summary>
    public class EnhancedCorrectionResult
    {
        public int TotalCorrections { get; set; }
        public int SuccessfulUpdates { get; set; }
        public int FailedUpdates { get; set; }
        public List<EnhancedCorrectionDetail> ProcessedCorrections { get; set; } = new List<EnhancedCorrectionDetail>();
        public List<DatabaseUpdateResult> DatabaseUpdates { get; set; } = new List<DatabaseUpdateResult>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan ProcessingDuration { get; set; }
        public bool HasErrors { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Detailed information about processing a single correction with metadata
    /// </summary>
    public class EnhancedCorrectionDetail
    {
        public CorrectionResult Correction { get; set; }
        public bool HasMetadata { get; set; }
        public OCRFieldMetadata OCRMetadata { get; set; }
        public DatabaseUpdateContext UpdateContext { get; set; }
        public DatabaseUpdateResult DatabaseUpdate { get; set; }
        public string SkipReason { get; set; }
        public DateTime ProcessingTime { get; set; }
    }

    /// <summary>
    /// Context classes for metadata extraction
    /// </summary>
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

    public class LineContext
    {
        public int? LineId { get; set; }
        public string LineName { get; set; }
        public string LineRegex { get; set; }
        public int? RegexId { get; set; }
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }
    }

    public class FieldContext
    {
        // Field information
        public int? FieldId { get; set; }
        public string Field { get; set; }
        public string Key { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool? IsRequired { get; set; }
        public string RawValue { get; set; }

        // Line context
        public int LineNumber { get; set; }
        public int? LineId { get; set; }
        public string LineName { get; set; }
        public string LineRegex { get; set; }
        public int? RegexId { get; set; }

        // Part context
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }

        // Processing context
        public string Section { get; set; }
        public string Instance { get; set; }
        public double? Confidence { get; set; }
    }

    /// <summary>
    /// Database update context for field corrections (forward declaration for integration)
    /// </summary>
    public class DatabaseUpdateContext
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public object FieldInfo { get; set; } // Will be EnhancedDatabaseFieldInfo from OCRFieldMapping
        public DatabaseUpdateStrategy UpdateStrategy { get; set; }
        public RequiredDatabaseIds RequiredIds { get; set; }
        public object ValidationRules { get; set; } // Will be FieldValidationInfo from OCRFieldMapping
    }

    /// <summary>
    /// Database update strategies
    /// </summary>
    public enum DatabaseUpdateStrategy
    {
        SkipUpdate,         // No OCR context available
        LogOnly,            // Log correction but don't update database
        UpdateFieldFormat,  // Update field format regex only
        UpdateRegexPattern, // Update existing regex pattern
        CreateNewPattern    // Create new regex pattern
    }

    /// <summary>
    /// Required database IDs for update operations
    /// </summary>
    public class RequiredDatabaseIds
    {
        public int? FieldId { get; set; }
        public int? LineId { get; set; }
        public int? RegexId { get; set; }
        public int? InvoiceId { get; set; }
        public int? PartId { get; set; }
    }

    #endregion
}