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

            _logger.Error("🚀 **DETECTION_PIPELINE_ENTRY (V9)**: Starting DUAL-PATHWAY error detection for invoice {InvoiceNo}", invoice.InvoiceNo);
            _logger.Error("   - **ARCHITECTURAL_INTENT**: Use both AI and hard-coded rules to find all possible errors, then consolidate them into a unique, actionable list.");

            try
            {
                // --- PATHWAY 1: DEEPSEEK AI-BASED DETECTION ---
                _logger.Error("   - **LOGIC_PATH_1**: Initiating primary AI-based granular error and omission detection.");
                var deepSeekErrors = await DetectHeaderFieldErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allDetectedErrors.AddRange(deepSeekErrors);
                _logger.Error("     - AI pathway found {Count} potential errors.", deepSeekErrors.Count);

                // --- PATHWAY 2: RULE-BASED DETECTION (RELIABILITY BACKSTOP) ---
                // disabled for now, as we are focusing on the AI-based detection.
                //if (fileText?.IndexOf("Amazon.com", StringComparison.OrdinalIgnoreCase) >= 0)
                //{
                //    _logger.Error("   - **LOGIC_PATH_2**: 'Amazon.com' detected. Running secondary rule-based detector as a reliability backstop.");
                //    var amazonErrors = DetectAmazonSpecificErrors(invoice, fileText);
                //    allDetectedErrors.AddRange(amazonErrors);
                //}

                // --- CONSOLIDATION ---
                _logger.Error("   - **CONSOLIDATION_START**: Consolidating {TotalCount} raw errors using a key of (Field, CorrectValue, SuggestedRegex) to find the unique, best-confidence error for each issue.", allDetectedErrors.Count);

                var uniqueErrors = allDetectedErrors
                    .GroupBy(e => new { Field = e.Field?.ToLowerInvariant(), Value = e.CorrectValue, Regex = e.SuggestedRegex })
                    .Select(g => {
                        var bestError = g.OrderByDescending(e => e.Confidence).First();
                        _logger.Error("     - For Key [Field: '{Field}', Value: '{Value}', Regex: '{Regex}'], selected best error with confidence {Confidence:P2} (out of {GroupCount} detections).",
                            g.Key.Field, g.Key.Value, g.Key.Regex, bestError.Confidence, g.Count());
                        return bestError;
                    })
                    .ToList();

                if (!uniqueErrors.Any())
                {
                    _logger.Error("   - ⚠️ **DETECTION_RESULT**: The detection pipeline found ZERO unique errors. No corrections will be applied.");
                }

                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
                var serializedErrors = JsonSerializer.Serialize(uniqueErrors, options);
                _logger.Error("📊 **DETECTION_PIPELINE_OUTPUT_DUMP**: Final list of {Count} unique InvoiceError objects to be processed: {SerializedErrors}",
                    uniqueErrors.Count, serializedErrors);

                return uniqueErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **CRITICAL_EXCEPTION** during DetectInvoiceErrorsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        /// <summary>
        /// Rule-based detector that now generates a consistent, pre-defined regex for each finding.
        /// </summary>
        private List<InvoiceError> DetectAmazonSpecificErrors(ShipmentInvoice invoice, string fileText)
        {
            var amazonErrors = new List<InvoiceError>();
            var lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            var giftCardRegex = new Regex(@"(Gift Card Amount:\s*-?\$?)([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
            var giftCardMatch = giftCardRegex.Match(fileText);
            if (giftCardMatch.Success)
            {
                amazonErrors.Add(new InvoiceError
                {
                    Field = "TotalInsurance",
                    ErrorType = "omission",
                    CorrectValue = $"-{giftCardMatch.Groups[2].Value.Trim()}",
                    Confidence = 0.98,
                    Reasoning = "Rule-based: Amazon Gift Card Amount detected.",
                    LineText = giftCardMatch.Value.Trim(),
                    LineNumber = GetLineNumberForMatch(lines, giftCardMatch),
                    SuggestedRegex = @"Gift Card Amount:\s*-?\$?(?<TotalInsurance>[\d,]+\.?\d*)"
                });
            }

            var freeShippingRegex = new Regex(@"(Free Shipping:\s*-?\$?)([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
            foreach (Match match in freeShippingRegex.Matches(fileText))
            {
                amazonErrors.Add(new InvoiceError
                {
                    Field = "TotalDeduction",
                    ErrorType = "omission",
                    CorrectValue = match.Groups[2].Value.Trim(),
                    Confidence = 0.95,
                    Reasoning = "Rule-based: Individual Free Shipping amount detected.",
                    LineText = match.Value.Trim(),
                    LineNumber = GetLineNumberForMatch(lines, match),
                    SuggestedRegex = @"Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)"
                });
            }

            _logger.Error("     - Rule-based Amazon detector found {Count} potential errors.", amazonErrors.Count);
            return amazonErrors;
        }

        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAndOmissionsAsync(
            ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            _logger.Error("     - 🤖 **DEEPSEEK_SUB_PROCESS**: Detecting header field errors/omissions for invoice {InvoiceNo} using DeepSeek.", invoice.InvoiceNo);
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

                _logger.Error("       - DeepSeek response processing complete. Found {Count} potential error items.", detectedErrors.Count);
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