// File: OCRCorrectionService/OCRCorrectionPipeline.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OCR.Business.Entities;
using EntryDataDS.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Functional extension methods for correction-centric OCR database update pipeline.
    /// Provides clean, discoverable pipeline operations while maintaining testability through instance methods.
    /// </summary>
    public static class OCRCorrectionPipeline
    {
        #region Correction-Centric Extension Methods

        /// <summary>
        /// Generates regex pattern for a correction using DeepSeek integration.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="correction">The correction to generate pattern for</param>
        /// <param name="service">OCR correction service instance</param>
        /// <param name="lineContext">Context for the line being corrected</param>
        /// <returns>Correction with generated regex pattern</returns>
        public static async Task<CorrectionResult> GenerateRegexPattern(
            this CorrectionResult correction, 
            OCRCorrectionService service, 
            LineContext lineContext)
        {
            return await service.GenerateRegexPatternInternal(correction, lineContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the generated regex pattern against known constraints.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="correction">The correction with pattern to validate</param>
        /// <param name="service">OCR correction service instance</param>
        /// <returns>Correction with validation results</returns>
        public static CorrectionResult ValidatePattern(
            this CorrectionResult correction, 
            OCRCorrectionService service)
        {
            return service.ValidatePatternInternal(correction);
        }

        /// <summary>
        /// Applies correction to database using appropriate strategy.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="correction">The validated correction to apply</param>
        /// <param name="templateContext">Template context for database operations</param>
        /// <param name="service">OCR correction service instance</param>
        /// <returns>Database update result</returns>
        public static async Task<DatabaseUpdateResult> ApplyToDatabase(
            this CorrectionResult correction, 
            TemplateContext templateContext, 
            OCRCorrectionService service)
        {
            return await service.ApplyToDatabaseInternal(correction, templateContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Re-imports template using updated patterns and validates results.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="updateResult">Database update result</param>
        /// <param name="templateContext">Template context for re-import</param>
        /// <param name="service">OCR correction service instance</param>
        /// <param name="fileText">Original file text for re-import</param>
        /// <returns>Re-import validation result</returns>
        public static async Task<ReimportResult> ReimportAndValidate(
            this DatabaseUpdateResult updateResult, 
            TemplateContext templateContext, 
            OCRCorrectionService service, 
            string fileText)
        {
            return await service.ReimportAndValidateInternal(updateResult, templateContext, fileText).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates actual invoice data using corrected values from template re-import.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="reimportResult">Re-import result with corrected data</param>
        /// <param name="invoice">Original invoice to update</param>
        /// <param name="service">OCR correction service instance</param>
        /// <returns>Invoice update result</returns>
        public static async Task<InvoiceUpdateResult> UpdateInvoiceData(
            this ReimportResult reimportResult, 
            ShipmentInvoice invoice, 
            OCRCorrectionService service)
        {
            return await service.UpdateInvoiceDataInternal(reimportResult, invoice).ConfigureAwait(false);
        }

        #endregion

        #region Pipeline Orchestration Extensions

        /// <summary>
        /// Executes complete correction pipeline for a single correction.
        /// Functional composition of all pipeline steps with comprehensive logging.
        /// </summary>
        /// <param name="correction">The correction to process</param>
        /// <param name="service">OCR correction service instance</param>
        /// <param name="templateContext">Template context for operations</param>
        /// <param name="invoice">Invoice to update</param>
        /// <param name="fileText">Original file text</param>
        /// <param name="maxRetries">Maximum retry attempts (default 3)</param>
        /// <returns>Complete pipeline execution result</returns>
        public static async Task<PipelineExecutionResult> ExecuteFullPipeline(
            this CorrectionResult correction,
            OCRCorrectionService service,
            TemplateContext templateContext,
            ShipmentInvoice invoice,
            string fileText,
            int maxRetries = 3)
        {
            return await service.ExecuteFullPipelineInternal(
                correction, templateContext, invoice, fileText, maxRetries).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes batch correction pipeline for multiple corrections with retry logic.
        /// Processes corrections sequentially to avoid database conflicts.
        /// </summary>
        /// <param name="corrections">List of corrections to process</param>
        /// <param name="service">OCR correction service instance</param>
        /// <param name="templateContext">Template context for operations</param>
        /// <param name="invoice">Invoice to update</param>
        /// <param name="fileText">Original file text</param>
        /// <param name="maxRetries">Maximum retry attempts per correction</param>
        /// <returns>Batch pipeline execution result</returns>
        public static async Task<BatchPipelineResult> ExecuteBatchPipeline(
            this IEnumerable<CorrectionResult> corrections,
            OCRCorrectionService service,
            TemplateContext templateContext,
            ShipmentInvoice invoice,
            string fileText,
            int maxRetries = 3)
        {
            return await service.ExecuteBatchPipelineInternal(
                corrections, templateContext, invoice, fileText, maxRetries).ConfigureAwait(false);
        }

        #endregion

        #region Template Context Creation Extensions

        /// <summary>
        /// Creates template context from OCR field metadata for pipeline operations.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="metadata">OCR field metadata dictionary</param>
        /// <param name="service">OCR correction service instance</param>
        /// <param name="fileText">Original file text for context</param>
        /// <returns>Template context for pipeline operations</returns>
        public static TemplateContext CreateTemplateContext(
            this Dictionary<string, OCRFieldMetadata> metadata,
            OCRCorrectionService service,
            string fileText)
        {
            return service.CreateTemplateContextInternal(metadata, fileText);
        }

        /// <summary>
        /// Creates line context from correction result for DeepSeek operations.
        /// Extension method provides discoverability; actual logic in testable instance method.
        /// </summary>
        /// <param name="correction">Correction to create context for</param>
        /// <param name="service">OCR correction service instance</param>
        /// <param name="metadata">OCR field metadata for context</param>
        /// <param name="fileText">Original file text</param>
        /// <returns>Line context for DeepSeek operations</returns>
        public static LineContext CreateLineContext(
            this CorrectionResult correction,
            OCRCorrectionService service,
            Dictionary<string, OCRFieldMetadata> metadata,
            string fileText)
        {
            return service.CreateLineContextInternal(correction, metadata, fileText);
        }

        #endregion
    }

    #region Pipeline Result Classes

    /// <summary>
    /// Result of complete pipeline execution for a single correction.
    /// Contains all intermediate results and final outcome.
    /// </summary>
    public class PipelineExecutionResult
    {
        public CorrectionResult OriginalCorrection { get; set; }
        public CorrectionResult PatternGeneratedCorrection { get; set; }
        public CorrectionResult ValidatedCorrection { get; set; }
        public DatabaseUpdateResult DatabaseResult { get; set; }
        public ReimportResult ReimportResult { get; set; }
        public InvoiceUpdateResult InvoiceResult { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int RetryAttempts { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Indicates if the correction was successful and invoice was updated.
        /// </summary>
        public bool CorrectionApplied => Success && InvoiceResult?.Success == true;

        /// <summary>
        /// Indicates if database patterns were successfully updated.
        /// </summary>
        public bool DatabaseUpdated => DatabaseResult?.IsSuccess == true;

        /// <summary>
        /// Indicates if template re-import was successful.
        /// </summary>
        public bool ReimportSuccessful => ReimportResult?.Success == true;
    }

    /// <summary>
    /// Result of batch pipeline execution for multiple corrections.
    /// Provides summary statistics and individual results.
    /// </summary>
    public class BatchPipelineResult
    {
        public List<PipelineExecutionResult> IndividualResults { get; set; } = new List<PipelineExecutionResult>();
        public int TotalCorrections { get; set; }
        public int SuccessfulCorrections { get; set; }
        public int FailedCorrections { get; set; }
        public int DatabaseUpdates { get; set; }
        public int InvoiceUpdates { get; set; }
        public int RetryAttempts { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Success rate as percentage of successful corrections.
        /// </summary>
        public double SuccessRate => TotalCorrections > 0 ? (SuccessfulCorrections * 100.0) / TotalCorrections : 0.0;

        /// <summary>
        /// Indicates if at least one correction was successfully applied.
        /// </summary>
        public bool AnyCorrectionApplied => IndividualResults.Any(r => r.CorrectionApplied);
    }

    /// <summary>
    /// Result of template re-import operation after database pattern updates.
    /// Contains validation of whether the correction fixed the original issue.
    /// </summary>
    public class ReimportResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> ExtractedValues { get; set; } = new Dictionary<string, object>();
        public double TotalsZero { get; set; }
        public bool IsBalanced => Math.Abs(TotalsZero) <= 0.01;
        public string FieldName { get; set; }
        public object CorrectedValue { get; set; }
        public object OriginalValue { get; set; }
        public bool ValueCorrected => !Equals(CorrectedValue, OriginalValue);
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Result of updating invoice data with corrected values.
    /// Bridges OCRContext changes to EntryDataDSContext.
    /// </summary>
    public class InvoiceUpdateResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string FieldName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public double TotalsZeroBefore { get; set; }
        public double TotalsZeroAfter { get; set; }
        public bool BalanceImproved => Math.Abs(TotalsZeroAfter) < Math.Abs(TotalsZeroBefore);
        public bool IsNowBalanced => Math.Abs(TotalsZeroAfter) <= 0.01;
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Template context for pipeline operations.
    /// Contains all necessary information for database operations.
    /// </summary>
    public class TemplateContext
    {
        public int? InvoiceId { get; set; }
        public string InvoiceName { get; set; }
        public int? FileTypeId { get; set; }
        public Dictionary<string, OCRFieldMetadata> Metadata { get; set; } = new Dictionary<string, OCRFieldMetadata>();
        public string FileText { get; set; }
        public List<int> PartIds { get; set; } = new List<int>();
        public List<int> LineIds { get; set; } = new List<int>();
        public string FilePath { get; set; }
        
        /// <summary>
        /// Indicates if template context has sufficient information for database operations.
        /// </summary>
        public bool IsValid => InvoiceId.HasValue && !string.IsNullOrEmpty(InvoiceName);
        
        /// <summary>
        /// Gets metadata for a specific field, handling both direct and mapped field names.
        /// </summary>
        public OCRFieldMetadata GetFieldMetadata(string fieldName)
        {
            if (Metadata.TryGetValue(fieldName, out var metadata))
                return metadata;
            
            // Try to find by mapped field name
            return Metadata.Values.FirstOrDefault(m => 
                string.Equals(m.FieldName, fieldName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(m.Field, fieldName, StringComparison.OrdinalIgnoreCase));
        }
    }

    #endregion
}