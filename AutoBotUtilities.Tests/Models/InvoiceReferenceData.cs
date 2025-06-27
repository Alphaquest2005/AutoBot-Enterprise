using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AutoBotUtilities.Tests.Models
{
    /// <summary>
    /// Reference data structure for invoice validation testing
    /// Aligned with production ShipmentInvoice model and Caribbean Customs rules
    /// Used for DeepSeek AI detection comparison
    /// </summary>
    public class InvoiceReferenceData
    {
        [JsonProperty("invoiceHeader")]
        public InvoiceHeader Header { get; set; }

        [JsonProperty("financialFields")]
        public FinancialFields Financial { get; set; }

        [JsonProperty("calculatedValidation")]
        public CalculatedValidation Validation { get; set; }

        [JsonProperty("extractionMetadata")]
        public ExtractionMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Header fields matching ShipmentInvoice model
    /// </summary>
    public class InvoiceHeader
    {
        [JsonProperty("InvoiceNo")]
        public string InvoiceNo { get; set; }

        [JsonProperty("InvoiceDate")]
        public string InvoiceDate { get; set; }

        [JsonProperty("SupplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("SupplierCode")]
        public string SupplierCode { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("EmailId")]
        public string EmailId { get; set; }
    }

    /// <summary>
    /// Financial fields exactly matching ShipmentInvoice model
    /// Using Caribbean Customs field mapping rules
    /// </summary>
    public class FinancialFields
    {
        [JsonProperty("InvoiceTotal")]
        public double? InvoiceTotal { get; set; }

        [JsonProperty("SubTotal")]
        public double? SubTotal { get; set; }

        [JsonProperty("TotalInternalFreight")]
        public double? TotalInternalFreight { get; set; }

        [JsonProperty("TotalOtherCost")]
        public double? TotalOtherCost { get; set; }

        /// <summary>
        /// Customer-caused reductions (stored as NEGATIVE values)
        /// Examples: Gift Cards, Store Credits
        /// </summary>
        [JsonProperty("TotalInsurance")]
        public double? TotalInsurance { get; set; }

        /// <summary>
        /// Supplier-caused reductions (stored as POSITIVE values)
        /// Examples: Free Shipping, Discounts
        /// </summary>
        [JsonProperty("TotalDeduction")]
        public double? TotalDeduction { get; set; }
    }

    public class LineItem
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("lineTotal")]
        public decimal LineTotal { get; set; }
    }

    /// <summary>
    /// Caribbean Customs balance validation using production formula
    /// </summary>
    public class CalculatedValidation
    {
        [JsonProperty("CalculatedTotal")]
        public double? CalculatedTotal { get; set; }

        [JsonProperty("BalanceCheck")]
        public double? BalanceCheck { get; set; }

        [JsonProperty("ValidationPassed")]
        public bool ValidationPassed { get; set; }

        [JsonProperty("Formula")]
        public string Formula { get; set; }
    }

    public class ExtractionMetadata
    {
        [JsonProperty("sourceText")]
        public string SourceText { get; set; }

        [JsonProperty("totalFieldCount")]
        public int TotalFieldCount { get; set; }

        [JsonProperty("extractedFieldCount")]
        public int ExtractedFieldCount { get; set; }

        [JsonProperty("confidenceLevel")]
        public string ConfidenceLevel { get; set; }

        [JsonProperty("processingNotes")]
        public List<string> ProcessingNotes { get; set; } = new List<string>();
    }

    /// <summary>
    /// AI Detection validation result for comparison
    /// </summary>
    public class AIDetectionValidationResult
    {
        public string FileName { get; set; }
        public InvoiceReferenceData ReferenceData { get; set; }
        public List<DetectedField> AIDetectedFields { get; set; } = new List<DetectedField>();
        public ValidationMetrics Metrics { get; set; }
    }

    public class DetectedField
    {
        public string FieldName { get; set; }
        public string DetectedValue { get; set; }
        public string ExpectedValue { get; set; }
        public bool IsMatch { get; set; }
        public string FieldType { get; set; } // "Header", "Financial", "LineItem"
    }

    public class ValidationMetrics
    {
        public int TotalExpectedFields { get; set; }
        public int CorrectlyDetectedFields { get; set; }
        public int MissedFields { get; set; }
        public int FalsePositives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public bool FinancialBalanceCorrect { get; set; }
        public double FinancialAccuracyError { get; set; }
        public bool DeepSeekMoreAccurate { get; set; }
        public string BalanceComparisonDetails { get; set; }
    }

    /// <summary>
    /// Legacy credit detection results - replaced by DetailedErrorDetectionResults
    /// Kept for backwards compatibility during transition
    /// </summary>
    [Obsolete("Use DetailedErrorDetectionResults with ProductionErrorObject instead")]
    public class CreditDetectionResults
    {
        public List<ProductionErrorObject> ActualCreditsFound { get; set; } = new List<ProductionErrorObject>();
        public List<ProductionErrorObject> PaymentMethodsFound { get; set; } = new List<ProductionErrorObject>();
    }

    /// <summary>
    /// Production-format error object matching DeepSeek OCRPromptCreation.cs:156-161
    /// Used for both credit detection and OCR error detection
    /// </summary>
    public class ProductionErrorObject
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("extracted_value")]
        public string ExtractedValue { get; set; }

        [JsonProperty("correct_value")]
        public string CorrectValue { get; set; }

        [JsonProperty("line_text")]
        public string LineText { get; set; }

        [JsonProperty("line_number")]
        public int LineNumber { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("error_type")]
        public string ErrorType { get; set; } // "omission", "format_correction", "inferred", "ocr_error"

        [JsonProperty("suggested_regex")]
        public string SuggestedRegex { get; set; }

        [JsonProperty("reasoning")]
        public string Reasoning { get; set; }
    }

    /// <summary>
    /// Comprehensive error detection results with production-format objects
    /// </summary>
    public class DetailedErrorDetectionResults
    {
        public List<ProductionErrorObject> CreditErrors { get; set; } = new List<ProductionErrorObject>();
        public List<ProductionErrorObject> OCRErrors { get; set; } = new List<ProductionErrorObject>();
        public List<ProductionErrorObject> BalanceErrors { get; set; } = new List<ProductionErrorObject>();
        public List<ProductionErrorObject> PaymentMethodExclusions { get; set; } = new List<ProductionErrorObject>();
    }
}