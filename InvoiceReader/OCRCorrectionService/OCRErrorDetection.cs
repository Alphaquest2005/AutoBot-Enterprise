// File: OCRCorrectionService/OCRErrorDetection.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

using System.Text.RegularExpressions;

// using System.Text.Json; // No longer directly used here for prompt creation
using System.Threading.Tasks;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using Ghostscript.NET.PDFA3Converter.ZUGFeRD;
using Microsoft.Office.Interop.Excel;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Utilities;
using Polly.Caching;
using Serilog; // ILogger is available as this._logger

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Error Detection Orchestration

        /// <summary>
        /// Detects OCR errors and potential omissions in a ShipmentInvoice by comparing it against the original file text
        /// and leveraging DeepSeek analysis. It orchestrates calls to header, product, and omission detection.
        /// </summary>
        /// <param name="invoice">The ShipmentInvoice object populated from OCR.</param>
        /// <param name="fileText">The full original text of the document.</param>
        /// <param name="metadata">Optional pre-extracted OCRFieldMetadata for fields already identified in the invoice.</param>
        /// <returns>A list of InvoiceError objects representing detected discrepancies.</returns>
        private async Task<List<InvoiceError>> DetectInvoiceErrorsAsync(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            var allDetectedErrors = new List<InvoiceError>();
            if (invoice == null)
            {
                _logger.Warning("DetectInvoiceErrorsAsync called with a null invoice.");
                return allDetectedErrors;
            }

            _logger.Information("Starting comprehensive error detection for invoice {InvoiceNo}.", invoice.InvoiceNo);

            try
            {
                // 1. Detect Header-Level Field Errors (including potential omissions based on prompt)
                var headerErrors = await DetectHeaderFieldErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allDetectedErrors.AddRange(headerErrors);
                _logger.Information("Detected {Count} header-level errors/omissions for invoice {InvoiceNo}.", headerErrors.Count, invoice.InvoiceNo);

                // 2. Detect Product-Level (InvoiceDetail) Errors (including potential omissions)
                var productErrors = await DetectProductDetailErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allDetectedErrors.AddRange(productErrors);
                _logger.Information("Detected {Count} product-level errors/omissions for invoice {InvoiceNo}.", productErrors.Count, invoice.InvoiceNo);
                
                // 3. Perform internal consistency validations AFTER LLM-based detection
                // These act as sanity checks or can find errors LLM missed.
                var mathErrors = this.ValidateMathematicalConsistency(invoice); // From OCRValidation.cs
                allDetectedErrors.AddRange(mathErrors);
                _logger.Information("Detected {Count} mathematical consistency errors for invoice {InvoiceNo}.", mathErrors.Count, invoice.InvoiceNo);

                var crossFieldErrors = this.ValidateCrossFieldConsistency(invoice); // From OCRValidation.cs
                allDetectedErrors.AddRange(crossFieldErrors);
                _logger.Information("Detected {Count} cross-field consistency errors for invoice {InvoiceNo}.", crossFieldErrors.Count, invoice.InvoiceNo);

                // 4. Specific Omission Detection (if metadata is available and robust)
                // This can be a more targeted check if the above prompts are too general for omissions.
                // The prompts for header/product already ask for omissions, so this might be redundant or for deeper analysis.
                // For now, let's assume the main prompts cover omissions sufficiently. If a dedicated omission pass is needed:
                /*
                if (metadata != null && metadata.Any())
                {
                    var dedicatedOmissionErrors = await DetectDedicatedFieldOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                    allDetectedErrors.AddRange(dedicatedOmissionErrors);
                    _logger.Information("Detected {Count} dedicated field omissions for invoice {InvoiceNo}", dedicatedOmissionErrors.Count, invoice.InvoiceNo);
                }
                */
                
                // 5. Deduplicate and resolve conflicts from all sources
                // This might happen here or in ApplyCorrectionsAsync. For now, ApplyCorrectionsAsync handles conflict resolution.
                var uniqueErrors = allDetectedErrors
                    .GroupBy(e => new { Field = e.Field?.ToLowerInvariant(), Type = e.ErrorType?.ToLowerInvariant() })
                    .Select(g => g.OrderByDescending(e => e.Confidence).First()) // Simple conflict resolution: take highest confidence
                    .ToList();

                _logger.Information("Total unique errors/omissions detected after consolidation: {ErrorCount} for invoice {InvoiceNo}.", uniqueErrors.Count, invoice.InvoiceNo);
                return uniqueErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical error during DetectInvoiceErrorsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>(); // Return empty list on critical failure
            }
        }

        /// <summary>
        /// Uses DeepSeek to detect errors and omissions in header-level fields.
        /// </summary>
        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAndOmissionsAsync(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            _logger.Debug("Detecting header field errors/omissions for invoice {InvoiceNo} using DeepSeek.", invoice.InvoiceNo);
            try
            {
                // Prompt creation is delegated to OCRPromptCreation.cs
                var prompt = this.CreateHeaderErrorDetectionPrompt(invoice, fileText);
                var deepSeekResponseJson = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
                {
                    _logger.Warning("Received empty response from DeepSeek for header error detection on invoice {InvoiceNo}.", invoice.InvoiceNo);
                    return new List<InvoiceError>();
                }
                
                // Parsing and context enrichment is delegated
                var detectedErrors = this.ProcessDeepSeekCorrectionResponse(deepSeekResponseJson, fileText) // From OCRDeepSeekIntegration.cs
                                         .Select(cr => ConvertCorrectionResultToInvoiceError(cr)) // Convert to InvoiceError
                                         .ToList(); 
                
                foreach (var error in detectedErrors) {
                    EnrichDetectedErrorWithContext(error, metadata, fileText); // Local helper for context
                }
                return detectedErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in DetectHeaderFieldErrorsAndOmissionsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        /// <summary>
        /// Uses DeepSeek to detect errors and omissions in product-level (InvoiceDetail) data.
        /// </summary>
        private async Task<List<InvoiceError>> DetectProductDetailErrorsAndOmissionsAsync(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
            {
                _logger.Information("No invoice details present on invoice {InvoiceNo} to validate for product errors.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
            _logger.Debug("Detecting product detail errors/omissions for invoice {InvoiceNo} using DeepSeek.", invoice.InvoiceNo);

            try
            {
                // Prompt creation is delegated to OCRPromptCreation.cs
                var prompt = this.CreateProductErrorDetectionPrompt(invoice, fileText);
                var deepSeekResponseJson = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
                {
                    _logger.Warning("Received empty response from DeepSeek for product error detection on invoice {InvoiceNo}.", invoice.InvoiceNo);
                    return new List<InvoiceError>();
                }

                var detectedErrors = this.ProcessDeepSeekCorrectionResponse(deepSeekResponseJson, fileText) // From OCRDeepSeekIntegration.cs
                                         .Select(cr => ConvertCorrectionResultToInvoiceError(cr)) // Convert to InvoiceError
                                         .ToList();

                foreach (var error in detectedErrors) {
                     EnrichDetectedErrorWithContext(error, metadata, fileText); // Local helper for context
                }
                return detectedErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in DetectProductDetailErrorsAndOmissionsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }
        
        /// <summary>
        /// A more targeted DeepSeek call specifically for finding omissions if primary prompts are insufficient.
        /// The `CreateOmissionDetectionPrompt` is defined in OCRPromptCreation.cs.
        /// </summary>
        private async Task<List<InvoiceError>> DetectDedicatedFieldOmissionsAsync(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> existingMetadata) // Metadata of fields ALREADY extracted
        {
            _logger.Debug("Performing dedicated omission detection for invoice {InvoiceNo}.", invoice.InvoiceNo);
            try
            {
                // Prompt creation delegated
                var prompt = this.CreateOmissionDetectionPrompt(invoice, fileText, existingMetadata); // From OCRPromptCreation.cs
                var deepSeekResponseJson = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
                {
                    _logger.Warning("Empty response from DeepSeek for dedicated omission detection on invoice {InvoiceNo}.", invoice.InvoiceNo);
                    return new List<InvoiceError>();
                }

                var detectedErrors = this.ProcessDeepSeekCorrectionResponse(deepSeekResponseJson, fileText) // From OCRDeepSeekIntegration.cs
                                         .Select(cr => ConvertCorrectionResultToInvoiceError(cr))
                                         .Where(e => e.ErrorType == "omission" || e.ErrorType == "omitted_line_item") // Ensure only omissions are kept
                                         .ToList();
                
                foreach (var error in detectedErrors) {
                     EnrichDetectedErrorWithContext(error, existingMetadata, fileText);
                }
                return detectedErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during DetectDedicatedFieldOmissionsAsync for invoice {InvoiceNo}", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        #region Omission Detection Prompt Creation

        /// <summary>
        /// Creates a specialized prompt for DeepSeek to analyze the original text and identify
        /// fields or data points that were likely present but not extracted by the primary OCR process.
        /// </summary>
        private string CreateOmissionDetectionPrompt(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> currentMetadata)
        {
            var extractedFieldsSummary = new Dictionary<string, object>();
            if (invoice != null)
            {
                extractedFieldsSummary["InvoiceNo"] = invoice.InvoiceNo;
                extractedFieldsSummary["InvoiceDate"] = invoice.InvoiceDate;
                extractedFieldsSummary["InvoiceTotal"] = invoice.InvoiceTotal;
                extractedFieldsSummary["SubTotal"] = invoice.SubTotal;
                extractedFieldsSummary["TotalDeduction"] = invoice.TotalDeduction;
                extractedFieldsSummary["SupplierName"] = invoice.SupplierName;
                if (invoice.InvoiceDetails != null && invoice.InvoiceDetails.Any())
                {
                    extractedFieldsSummary["LineItemCount"] = invoice.InvoiceDetails.Count;
                }
            }
            if (currentMetadata != null)
            {
                foreach (var metaKey in currentMetadata.Keys.Take(10))
                {
                    if (!extractedFieldsSummary.ContainsKey(metaKey))
                    {
                        extractedFieldsSummary[metaKey + "_extracted_via_metadata"] = true; // Simplified
                    }
                }
            }

            var extractedSummaryJson = JsonSerializer.Serialize(extractedFieldsSummary, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            // Using C# verbatim string literal @"" for easier multi-line strings.
            // Quotes " within the JSON examples must be escaped as ""
            // Braces { and } within the JSON examples must be escaped as {{ and }} when using interpolated strings ($@"...")
            // Since this is not an interpolated string but a direct verbatim string, only " needs to be ""
            return $@"OCR OMISSION DETECTION TASK:
Your goal is to meticulously review the ORIGINAL INVOICE TEXT and identify important financial or identifying fields/data that are present in the text but seem to be MISSING or inadequately represented in the SUMMARY OF EXTRACTED DATA.

SUMMARY OF CURRENTLY EXTRACTED DATA (this is what the OCR system thinks it found):
{extractedSummaryJson}

ORIGINAL INVOICE TEXT (analyze this carefully):
{this.CleanTextForAnalysis(fileText)}

INSTRUCTIONS FOR IDENTIFYING OMISSIONS:
1.  FOCUS AREAS FOR OMISSIONS:
    *   Financial Totals: Any missing grand total, subtotal, tax amounts, shipping fees, or other charges.
    *   Deductions/Discounts: CRITICAL - Look for any mention of gift cards, store credits, coupons, promotional discounts, or other deductions that reduce the payable amount. If ""TotalDeduction"" in extracted data is zero or low, but text mentions such items, report as an omission.
    *   Key Identifiers: Missing invoice number, order number, invoice date, supplier name.
    *   Line Items: Entirely missing product/service lines (description, quantity, price, line total). For these, use `error_type: ""omitted_line_item""`.
    *   Fees/Charges: Any specific fees (e.g., ""Handling Fee"", ""Service Charge"") not captured.

2.  FOR EACH IDENTIFIED OMISSION, PROVIDE:
    *   `""field""`: The canonical name of the field you believe is omitted (e.g., ""TotalDeduction"", ""InvoiceDetail_LineX_ItemDescription"" where X is its text line number). If a whole line item is missing, use a field like ""InvoiceDetail_LineX_OmittedLineItem"".
    *   `""extracted_value""`: This should be an empty string ("""") or reflect the null/missing state from the extracted summary for this field.
    *   `""correct_value""`: The value of the omitted field as found in the ORIGINAL INVOICE TEXT. For an ""omitted_line_item"", summarize the item details here.
    *   `""line_text""`: The EXACT line(s) from ORIGINAL INVOICE TEXT where the omitted data is found or clearly implied.
    *   `""line_number""`: The 1-based *starting* line number in ORIGINAL INVOICE TEXT for ""line_text"".
    *   `""context_lines_before""`: Array of up to 5 full text lines PRECEDING ""line_text"".
    *   `""context_lines_after""`: Array of up to 5 full text lines FOLLOWING ""line_text"".
    *   `""requires_multiline_regex""`: true/false.
    *   `""confidence""`: Your confidence (0.0 to 1.0) that this is a true omission.
    *   `""error_type""`: Must be ""omission"" or ""omitted_line_item"".
    *   `""reasoning""`: Why you identified this as an omission (e.g., ""Text mentions 'Gift Card $10.00', but TotalDeduction is $0.00"").

STRICT JSON RESPONSE FORMAT:
{{
  ""errors"": [ 
    {{
      ""field"": ""TotalDeduction"", 
      ""extracted_value"": ""0.00"", 
      ""correct_value"": ""10.00"",
      ""line_text"": ""Applied Gift Card Balance: -$10.00 USD"", 
      ""line_number"": 35,
      ""context_lines_before"": [""L30: Subtotal: $100.00"", ""L31: Shipping: $5.00"", ""L32: Handling: $0.00"", ""L33: Promo: -$2.00"", ""L34: Tax: $8.00""],
      ""context_lines_after"": [""L36: Total Due: $103.00"", ""L37: Paid By: Mastercard"", ""L38: Card ending **** 1234"", ""L39: Auth: 987654"", ""L40: Thank you!""] ,
      ""requires_multiline_regex"": false,
      ""confidence"": 0.99,
      ""error_type"": ""omission"",
      ""reasoning"": ""Text clearly shows a $10.00 gift card applied, which is missing from extracted deductions.""
    }},
    {{
      ""field"": ""InvoiceDetail_Line42_OmittedLineItem"",
      ""extracted_value"": """",
      ""correct_value"": ""Item: Special Widget, Quantity: 1, UnitPrice: 25.00, LineTotal: 25.00"",
      ""line_text"": ""Special Widget           1     $25.00      $25.00"",
      ""line_number"": 42,
      ""context_lines_before"": [""L37..."", ""L38..."", ""L39..."", ""L40..."", ""L41...""] ,
      ""context_lines_after"": [""L43..."", ""L44..."", ""L45..."", ""L46..."", ""L47...""] ,
      ""requires_multiline_regex"": false,
      ""confidence"": 0.95,
      ""error_type"": ""omitted_line_item"",
      ""reasoning"": ""A complete line item for 'Special Widget' is present in text around line 42 but not in the extracted line items summary.""
    }}
  ]
}}

If NO omissions are found, return: {{""errors"": []}}
Be very precise. Your goal is to help the system learn to capture these missing pieces next time.
Provide rich context (before/after lines) as this is crucial for generating new extraction patterns.
Only report items as omissions if they are truly missing or significantly misrepresented in the EXTRACTED DATA SUMMARY compared to the ORIGINAL INVOICE TEXT.
";
        }

        #endregion

        /// <summary>
        /// Converts a CorrectionResult (typically from parsing a DeepSeek response for corrections)
        /// into an InvoiceError object (used for initial error representation).
        /// </summary>
        private InvoiceError ConvertCorrectionResultToInvoiceError(CorrectionResult cr)
        {
            if (cr == null) return null;
            return new InvoiceError
            {
                Field = cr.FieldName,
                ExtractedValue = cr.OldValue, // In this context, OldValue from CorrectionResult is the ExtractedValue for InvoiceError
                CorrectValue = cr.NewValue,
                Confidence = cr.Confidence,
                ErrorType = cr.CorrectionType, // This might need mapping if CorrectionType and ErrorType have different taxonomies
                Reasoning = cr.Reasoning,
                LineNumber = cr.LineNumber,
                LineText = cr.LineText,
                ContextLinesBefore = cr.ContextLinesBefore,
                ContextLinesAfter = cr.ContextLinesAfter,
                RequiresMultilineRegex = cr.RequiresMultilineRegex
            };
        }

        /// <summary>
        /// Enriches a detected InvoiceError with context from existing OCRFieldMetadata or file text.
        /// </summary>
        private void EnrichDetectedErrorWithContext(InvoiceError error, Dictionary<string, OCRFieldMetadata> allKnownMetadata, string fileText)
        {
            if (error == null) return;

            OCRFieldMetadata fieldSpecificMetadata = null;
            if (allKnownMetadata != null) {
                // Try to find metadata for the specific field in the error.
                // error.Field might be "InvoiceTotal" or "InvoiceDetail_LineX_Quantity".
                // The allKnownMetadata keys would ideally match these formats or be simple like "Quantity".
                if (allKnownMetadata.TryGetValue(error.Field, out var metaEntry))
                {
                    fieldSpecificMetadata = metaEntry;
                } else { 
                    // If error.Field is prefixed (e.g. InvoiceDetail_Line1_Quantity), try to find metadata for "Quantity"
                    // and then align by line number if possible. This is complex without a robust keying strategy for metadata.
                    // For now, we assume if a direct key match fails, specific metadata isn't easily found.
                }
            }

            if (fieldSpecificMetadata != null)
            {
                // If metadata found, it might have a more accurate line number or text for *extracted* values.
                // However, the error's LineNumber/LineText from DeepSeek is for the *correction*.
                // We primarily use existing metadata to fill *gaps* in DeepSeek's error context.
                if (error.LineNumber <= 0 && fieldSpecificMetadata.LineNumber > 0) error.LineNumber = fieldSpecificMetadata.LineNumber;
                if (string.IsNullOrEmpty(error.LineText) && !string.IsNullOrEmpty(fieldSpecificMetadata.LineText)) error.LineText = fieldSpecificMetadata.LineText;
                else if (string.IsNullOrEmpty(error.LineText) && error.LineNumber > 0) error.LineText = this.GetOriginalLineText(fileText, error.LineNumber); // From OCRUtilities
            }
            else // No specific metadata for this field
            {
                 if (string.IsNullOrEmpty(error.LineText) && error.LineNumber > 0) error.LineText = this.GetOriginalLineText(fileText, error.LineNumber);
            }
            
            // Ensure context lines are populated if LineNumber and LineText are known but context arrays are empty
            if (error.LineNumber > 0 && !string.IsNullOrEmpty(error.LineText) &&
                (error.ContextLinesBefore == null || !error.ContextLinesBefore.Any()) &&
                (error.ContextLinesAfter == null || !error.ContextLinesAfter.Any()) )
            {
                var lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
                var lineIndex = error.LineNumber - 1; // 0-based
                int contextSize = 5; // Standard context window

                if(lineIndex >= 0 && lineIndex < lines.Length)
                {
                    error.ContextLinesBefore = new List<string>();
                    for (int i = Math.Max(0, lineIndex - contextSize); i < lineIndex; i++)
                        error.ContextLinesBefore.Add(lines[i]);
                    
                    error.ContextLinesAfter = new List<string>();
                    for (int i = lineIndex + 1; i <= Math.Min(lines.Length - 1, lineIndex + contextSize); i++)
                        error.ContextLinesAfter.Add(lines[i]);
                }
            }
        }
        #endregion
    }
}