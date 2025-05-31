// File: OCRCorrectionService/OCRErrorDetection.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Error Detection

        /// <summary>
        /// Detects OCR errors using comprehensive 4-stage validation with omission detection
        /// </summary>
        private async Task<List<InvoiceError>> DetectInvoiceErrorsAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            try
            {
                var allErrors = new List<InvoiceError>();

                // 1. Header-level field validation (totals, supplier info) - ENHANCED
                _logger.Information("Detecting header field errors and omissions for invoice {InvoiceNo}", invoice.InvoiceNo);
                var headerErrors = await this.DetectHeaderFieldErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allErrors.AddRange(headerErrors);

                // 2. Product-level validation (prices, quantities, descriptions) - ENHANCED
                _logger.Information("Detecting product-level errors and omissions for invoice {InvoiceNo}", invoice.InvoiceNo);
                var productErrors = await this.DetectProductErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allErrors.AddRange(productErrors);

                // 3. Mathematical consistency validation
                _logger.Information("Validating mathematical consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
                var mathErrors = ValidateMathematicalConsistency(invoice);
                allErrors.AddRange(mathErrors);

                // 4. Cross-field validation (totals vs details)
                _logger.Information("Validating cross-field consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
                var crossFieldErrors = ValidateCrossFieldConsistency(invoice);
                allErrors.AddRange(crossFieldErrors);

                // 5. NEW: Omission detection using metadata comparison
                if (metadata != null)
                {
                    _logger.Information("Detecting field omissions using metadata for invoice {InvoiceNo}", invoice.InvoiceNo);
                    var omissionErrors = await DetectFieldOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                    allErrors.AddRange(omissionErrors);
                }

                _logger.Information("Total errors detected: {ErrorCount} for invoice {InvoiceNo}",
                    allErrors.Count, invoice.InvoiceNo);

                return allErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error detecting invoice errors for {InvoiceNo}", invoice?.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        /// <summary>
        /// NEW: Detects field omissions by comparing extracted data against DeepSeek analysis
        /// </summary>
        private async Task<List<InvoiceError>> DetectFieldOmissionsAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata)
        {
            var omissionErrors = new List<InvoiceError>();

            try
            {
                // Create omission detection prompt
                var prompt = CreateOmissionDetectionPrompt(invoice, fileText, metadata);
                var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.Warning("Empty response from DeepSeek for omission detection");
                    return omissionErrors;
                }

                // Parse response for omissions
                var errors = ParseErrorDetectionResponse(response);
                
                // Filter for omission-type errors and add metadata context
                foreach (var error in errors.Where(e => e.ErrorType == "omission"))
                {
                    // Enrich with metadata context if available
                    EnrichErrorWithMetadata(error, metadata);
                    omissionErrors.Add(error);
                }

                _logger.Information("Detected {OmissionCount} field omissions", omissionErrors.Count);
                return omissionErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error detecting field omissions");
                return omissionErrors;
            }
        }

        /// <summary>
        /// Detects errors in header-level fields using specialized prompt with context
        /// </summary>
        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            var prompt = CreateHeaderErrorDetectionPrompt(invoice, fileText);
            var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.Warning("Empty response from DeepSeek for header error detection");
                return new List<InvoiceError>();
            }

            var errors = ParseErrorDetectionResponse(response);
            
            // Enrich errors with metadata context
            foreach (var error in errors)
            {
                EnrichErrorWithMetadata(error, metadata);
            }

            return errors;
        }

        /// <summary>
        /// Detects errors in product-level data using specialized prompt with context
        /// </summary>
        private async Task<List<InvoiceError>> DetectProductErrorsAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
            {
                _logger.Information("No invoice details to validate for invoice {InvoiceNo}", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }

            var prompt = CreateProductErrorDetectionPrompt(invoice, fileText);
            var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.Warning("Empty response from DeepSeek for product error detection");
                return new List<InvoiceError>();
            }

            var errors = ParseErrorDetectionResponse(response);
            
            // Enrich errors with metadata context
            foreach (var error in errors)
            {
                EnrichErrorWithMetadata(error, metadata);
            }

            return errors;
        }

        /// <summary>
        /// Creates omission detection prompt for missing fields
        /// </summary>
        private string CreateOmissionDetectionPrompt(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata)
        {
            var extractedFields = string.Join(", ", metadata.Keys);
            
            return $@"OCR OMISSION DETECTION:

Analyze the original text and identify fields that contain data but were not extracted.

EXTRACTED FIELDS: {extractedFields}

ORIGINAL TEXT:
{CleanTextForAnalysis(fileText)}

CRITICAL REQUIREMENTS:
1. Find fields with values in the text that are missing from extracted data
2. Focus on invoice totals, deductions, fees, and line item details
3. For each missing field, provide the EXACT line text where it appears
4. Include 5 lines before and after for context

RESPONSE FORMAT (JSON only):
{{
  ""errors"": [
    {{
      ""field"": ""TotalDeduction"",
      ""extracted_value"": """",
      ""correct_value"": ""6.99"",
      ""line_text"": ""Gift Card Applied: -$6.99"",
      ""line_number"": 28,
      ""context_lines_before"": [""Line 23: text"", ""Line 24: text"", ...],
      ""context_lines_after"": [""Line 29: text"", ""Line 30: text"", ...],
      ""requires_multiline_regex"": false,
      ""confidence"": 0.95,
      ""error_type"": ""omission"",
      ""reasoning"": ""Gift card deduction found in text but not extracted""
    }}
  ]
}}

Return empty array if no omissions: {{""errors"": []}}";
        }

        /// <summary>
        /// Enriches error with metadata context from template processing
        /// </summary>
        private void EnrichErrorWithMetadata(InvoiceError error, Dictionary<string, OCRFieldMetadata> metadata)
        {
            if (metadata == null) return;

            // Try to find matching metadata for this field
            var fieldMetadata = metadata.Values.FirstOrDefault(m => 
                m.FieldName.Equals(error.Field, StringComparison.OrdinalIgnoreCase));

            if (fieldMetadata != null)
            {
                error.LineNumber = fieldMetadata.LineNumber;
                // Additional metadata enrichment can be added here
            }
        }

        #endregion
    }
}