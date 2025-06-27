// File: OCRCorrectionService/OCRErrorDetection.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Error Detection Orchestration

        /// <summary>
        /// FINAL VERSION (V9): Orchestrates a dual-pathway error detection strategy and consolidates results
        /// using a robust, regex-based deduplication key to prevent redundant learning.
        /// </summary>
        private async Task<List<InvoiceError>> DetectInvoiceErrorsAsync(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            var allDetectedErrors = new List<InvoiceError>();
            if (invoice == null) return allDetectedErrors;

            _logger.Information("🚀 **DETECTION_PIPELINE_ENTRY (V9)**: Starting DUAL-PATHWAY error detection for invoice {InvoiceNo}", invoice.InvoiceNo);
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Use both AI and hard-coded rules to find all possible errors, then consolidate them into a unique, actionable list.");

            try
            {
                // --- PATHWAY 1: DEEPSEEK AI-BASED DETECTION ---
                _logger.Information("   - **LOGIC_PATH_1**: Initiating primary AI-based granular error and omission detection.");
                var deepSeekErrors = await DetectHeaderFieldErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allDetectedErrors.AddRange(deepSeekErrors);
                _logger.Information("     - AI pathway found {Count} potential errors.", deepSeekErrors.Count);

                // --- PATHWAY 2: RULE-BASED DETECTION (RELIABILITY BACKSTOP) ---
                // disabled for now, as we are focusing on the AI-based detection.
                //if (fileText?.IndexOf("Amazon.com", StringComparison.OrdinalIgnoreCase) >= 0)
                //{
                //    _logger.Error("   - **LOGIC_PATH_2**: 'Amazon.com' detected. Running secondary rule-based detector as a reliability backstop.");
                //    var amazonErrors = DetectAmazonSpecificErrors(invoice, fileText);
                //    allDetectedErrors.AddRange(amazonErrors);
                //}

                // --- CONSOLIDATION ---
                _logger.Information("   - **CONSOLIDATION_START**: Consolidating {TotalCount} raw errors using a key of (Field, CorrectValue, SuggestedRegex) to find the unique, best-confidence error for each issue.", allDetectedErrors.Count);

                var uniqueErrors = allDetectedErrors
                    .GroupBy(e => new { Field = e.Field?.ToLowerInvariant(), Value = e.CorrectValue, Regex = e.SuggestedRegex })
                    .Select(g => {
                        var bestError = g.OrderByDescending(e => e.Confidence).First();
                        _logger.Information("     - For Key [Field: '{Field}', Value: '{Value}', Regex: '{Regex}'], selected best error with confidence {Confidence:P2} (out of {GroupCount} detections).",
                            g.Key.Field, g.Key.Value, g.Key.Regex, bestError.Confidence, g.Count());
                        return bestError;
                    })
                    .ToList();

                if (!uniqueErrors.Any())
                {
                    _logger.Warning("   - ⚠️ **DETECTION_RESULT**: The detection pipeline found ZERO unique errors. No corrections will be applied.");
                }

                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
                var serializedErrors = JsonSerializer.Serialize(uniqueErrors, options);
                _logger.Information("📊 **DETECTION_PIPELINE_OUTPUT_DUMP**: Final list of {Count} unique InvoiceError objects to be processed: {SerializedErrors}",
                    uniqueErrors.Count, serializedErrors);

                return uniqueErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **CRITICAL_EXCEPTION** during DetectInvoiceErrorsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        // 🗑️ **DEPRECATED_METHOD_REMOVED**: DetectAmazonSpecificErrors() method removed
        // **ARCHITECTURAL_DECISION**: This Amazon-specific rule-based detector was disabled in production
        // (lines 40-47) in favor of the generalized AI-based detection pathway.
        // **BUSINESS_JUSTIFICATION**: OCR correction service needs to work across diverse invoice types,
        // not just Amazon invoices. The AI-based approach provides better generalization.
        // **EVIDENCE**: Method was commented out in DetectInvoiceErrorsAsync() and not being called.
        // **IMPACT**: No production impact - method was already disabled.
        // **GENERALIZATION_BENEFIT**: Removing Amazon-specific logic helps ensure the service
        // works equally well for TEMU, Tropical Vendors, Purchase Orders, BOLs, and other invoice types.

        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAndOmissionsAsync(
            ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            _logger.Information("     - 🤖 **DEEPSEEK_SUB_PROCESS**: Detecting header field errors/omissions for invoice {InvoiceNo} using DeepSeek.", invoice.InvoiceNo);
            try
            {
                // This call now passes the rich metadata required for the V9 prompt.
                var prompt = this.CreateHeaderErrorDetectionPrompt(invoice, fileText, metadata);
                var deepSeekResponseJson = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
                {
                    _logger.Warning("       - ❌ Received empty or null response from DeepSeek for header error detection on invoice {InvoiceNo}.", invoice.InvoiceNo);
                    return new List<InvoiceError>();
                }

                var correctionResults = this.ProcessDeepSeekCorrectionResponse(deepSeekResponseJson, fileText);
                var detectedErrors = correctionResults.Select(cr => ConvertCorrectionResultToInvoiceError(cr)).ToList();

                _logger.Information("       - DeepSeek response processing complete. Found {Count} potential error items.", detectedErrors.Count);
                return detectedErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **EXCEPTION** in DetectHeaderFieldErrorsAndOmissionsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        private InvoiceError ConvertCorrectionResultToInvoiceError(CorrectionResult cr)
        {
            if (cr == null) return null;
            return new InvoiceError
                       {
                           Field = cr.FieldName,
                           ExtractedValue = cr.OldValue,
                           CorrectValue = cr.NewValue,
                           Confidence = cr.Confidence,
                           ErrorType = cr.CorrectionType,
                           Reasoning = cr.Reasoning,
                           LineNumber = cr.LineNumber,
                           LineText = cr.LineText,
                           ContextLinesBefore = cr.ContextLinesBefore,
                           ContextLinesAfter = cr.ContextLinesAfter,
                           RequiresMultilineRegex = cr.RequiresMultilineRegex,
                           SuggestedRegex = cr.SuggestedRegex,
                           // =================================== FIX START ===================================
                           Pattern = cr.Pattern,
                           Replacement = cr.Replacement
                           // ==================================== FIX END ====================================
                       };
        }

        private int GetLineNumberForMatch(string[] lines, Match match)
        {
            if (match == null || !match.Success) return 0;
            int charCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // The character count for a line includes the line itself and its newline characters
                int lineLengthWithNewline = lines[i].Length + Environment.NewLine.Length;
                if (match.Index >= charCount && match.Index < charCount + lineLengthWithNewline)
                {
                    return i + 1; // Return 1-based line number
                }
                charCount += lineLengthWithNewline;
            }
            return 0; // Match not found
        }

        #endregion
    }
}