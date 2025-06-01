// File: OCRCorrectionService/OCRDataModels.cs
using System;
using System.Linq;
using System.Collections.Generic;
using global::EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For DB entities like Invoice, Fields, Lines, etc.

namespace WaterNut.DataSpace
{
    #region Core Correction and Error Models

    /// <summary>
    /// Represents a single correction proposed by the LLM or an internal validation rule.
    /// This is the primary object used to apply changes and learn patterns.
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
        public string LineText { get; set; }
        public List<string> ContextLinesBefore { get; set; } = new List<string>();
        public List<string> ContextLinesAfter { get; set; } = new List<string>();
        public bool RequiresMultilineRegex { get; set; } 
        
        public string FullContext => string.Join("\n", 
            ContextLinesBefore.Concat(new[] { $"Line {LineNumber}: {LineText}" }).Concat(ContextLinesAfter));
    }

    /// <summary>
    /// Represents an error detected in the invoice data before correction.
    /// </summary>
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
    }

    #endregion

    #region Database Interaction Models

    /// <summary>
    /// Result of a database update operation (e.g., creating/updating a regex pattern).
    /// </summary>
    public class DatabaseUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? RecordId { get; set; } 
        public string Operation { get; set; } 
        public Exception Exception { get; set; }

        public static DatabaseUpdateResult Success(int recordId, string operation) =>
            new DatabaseUpdateResult { IsSuccess = true, RecordId = recordId, Operation = operation, Message = $"Successfully {operation} (ID: {recordId})" };
        
        public static DatabaseUpdateResult Failed(string message, Exception ex = null) =>
            new DatabaseUpdateResult { IsSuccess = false, Message = message, Exception = ex };
    }

    /// <summary>
    /// Request object for database update strategies. Contains all necessary context for a strategy to act.
    /// </summary>
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
        public string FilePath { get; set; }
        public string InvoiceType { get; set; }     
        public int? LineId { get; set; }                                                 
        public int? PartId { get; set; }            
        public int? RegexId { get; set; }           
        public string ExistingRegex { get; set; }   
    }
    
    /// <summary>
    /// Enum defining the strategies for database updates.
    /// </summary>
    public enum DatabaseUpdateStrategy
    {
        SkipUpdate,         
        LogOnly,            
        UpdateFieldFormat,  
        UpdateRegexPattern, 
        CreateNewPattern    
    }

    #endregion

    #region OCR Template and Extraction Metadata Models

    /// <summary>
    /// Detailed metadata about how a field was extracted by the OCR template system.
    /// This links runtime extracted values back to the specific OCR template definitions.
    /// </summary>
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
        public string Section { get; set; }            
        public string Instance { get; set; }           
        public double? Confidence { get; set; }        
        public List<FieldFormatRegexInfo> FormatRegexes { get; set; } = new List<FieldFormatRegexInfo>();
    }

    /// <summary>
    /// Information about a specific FieldFormatRegEx rule applied to a field.
    /// </summary>
    public class FieldFormatRegexInfo
    {
        public int? FormatRegexId { get; set; }        
        public int? RegexId { get; set; }              
        public int? ReplacementRegexId { get; set; }   
        public string Pattern { get; set; }            
        public string Replacement { get; set; }        
    }

    /// <summary>
    /// Combines a ShipmentInvoice (runtime data) with its corresponding OCRFieldMetadata.
    /// </summary>
    public class ShipmentInvoiceWithMetadata
    {
        public ShipmentInvoice Invoice { get; set; }
        public Dictionary<string, OCRFieldMetadata> FieldMetadata { get; set; } = new Dictionary<string, OCRFieldMetadata>();

        public OCRFieldMetadata GetFieldMetadata(string fieldName) => 
            FieldMetadata.TryGetValue(fieldName, out var metadata) ? metadata : null;
        
        public void SetFieldMetadata(string fieldName, OCRFieldMetadata metadata) => 
            FieldMetadata[fieldName] = metadata;
    }

    #endregion

    #region Context and Strategy Models for Advanced Processing

    /// <summary>
    /// Represents the textual and structural context of a line being processed for correction, especially omissions.
    /// </summary>
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
        public bool IsOrphaned { get; set; }        
        public bool RequiresNewLineCreation { get; set; } 

        public string FullContextWithLineNumbers => string.Join("\n", 
            ContextLinesBefore.Concat(new[] { $">>> Line {LineNumber}: {LineText} <<<" }).Concat(ContextLinesAfter));
    }

    /// <summary>
    /// Simplified information about a field definition within a line's regex context.
    /// Primarily derived from OCR.Business.Entities.Fields.
    /// </summary>
    public class FieldInfo 
    {
        public int FieldId { get; set; }       
        public string Key { get; set; }         
        public string Field { get; set; }       
        public string EntityType { get; set; }  
        public string DataType { get; set; }    
        public bool? IsRequired { get; set; }   
    }
    
    /// <summary>
    /// Response from DeepSeek specifically for suggesting a new regex pattern.
    /// </summary>
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
        public string ContextLinesUsed { get; set; } // Additional property for test compatibility
    }

    /// <summary>
    /// Alias for RegexCreationResponse for backward compatibility with tests
    /// </summary>
    public class DeepSeekRegexResponse : RegexCreationResponse
    {
        // This is just an alias/inheritance for test compatibility
    }
    
    /// <summary>
    /// Overall result of processing a batch of corrections with enhanced metadata.
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
    /// Details for a single correction processed with enhanced metadata.
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
    /// Context for deciding and executing database updates for a correction.
    /// </summary>
    public class DatabaseUpdateContext 
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public OCRCorrectionService.EnhancedDatabaseFieldInfo FieldInfo { get; set; } 
        public DatabaseUpdateStrategy UpdateStrategy { get; set; }
        public RequiredDatabaseIds RequiredIds { get; set; } 
        public OCRCorrectionService.FieldValidationInfo ValidationRules { get; set; }
    }

    /// <summary>
    /// IDs from OCRFieldMetadata relevant for DB update operations.
    /// </summary>
    public class RequiredDatabaseIds 
    {
        public int? FieldId { get; set; }
        public int? LineId { get; set; }
        public int? RegexId { get; set; }
        public int? InvoiceId { get; set; } 
        public int? PartId { get; set; }
    }
    
    // Context classes used during metadata extraction from OCR Template
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
    public class FieldContext 
    {
        public int? FieldId { get; set; }
        public string Field { get; set; }
        public string Key { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool? IsRequired { get; set; }
        public string RawValue { get; set; }
        public int LineNumber { get; set; } 
        public int? LineId { get; set; } 
        public string LineName { get; set; }
        public string LineRegex { get; set; }
        public int? RegexId { get; set; }
        public int? PartId { get; set; }
        public string PartName { get; set; }
        public int? PartTypeId { get; set; }
        public string Section { get; set; } 
        public string Instance { get; set; } 
        public double? Confidence { get; set; }
    }

    // The following models were present in some files but might be less used after consolidation
    // or their purpose is covered by the above. Review if they are still essential.

    /// <summary>
    /// Enhanced LineInfo with confidence and reasoning (might be superseded by CorrectionResult/InvoiceError context)
    /// </summary>
    public class LineInfo // If this is just for simple line data from DeepSeek, it's fine.
    {
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
    }

    /// <summary>
    /// Represents a regex correction strategy specifically determined by DeepSeek (e.g. for format fixes)
    /// This might be used if DeepSeek returns a specific regex pattern to apply, rather than just a corrected value.
    /// </summary>
    public class RegexCorrectionStrategy // This seems distinct enough to keep if DeepSeek provides explicit regex fix strategies.
    {
        public string StrategyType { get; set; } // e.g., FORMAT_FIX, PATTERN_UPDATE, CHARACTER_MAP, VALIDATION_RULE
        public string RegexPattern { get; set; }
        public string ReplacementPattern { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; }
    }
    
    /// <summary>
    /// Data model for storing learned regex patterns (perhaps in a local cache or simplified DB table, distinct from OCR.Business.Entities.RegularExpressions)
    /// </summary>
    public class RegexPattern // This is for a conceptual local store/cache, not the main DB entity.
    {
        public string FieldName { get; set; }
        public string StrategyType { get; set; } // Type of correction that led to this pattern
        public string Pattern { get; set; }
        public string Replacement { get; set; } // If applicable
        public double Confidence { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UpdateCount { get; set; }
        public string CreatedBy { get; set; }
    }
    
    /// <summary>
    /// Enum for different types of OCR errors, for more granular classification.
    /// </summary>
    public enum OCRErrorType // If used for tagging InvoiceError.ErrorType or CorrectionResult.CorrectionType
    {
        DecimalSeparator,    
        CharacterConfusion,  
        MissingDigit,       
        FormatError,        
        FieldMismatch,      
        CalculationError,   
        UnreasonableValue,
        Omission // Explicitly for missing fields
    }

    #endregion
}