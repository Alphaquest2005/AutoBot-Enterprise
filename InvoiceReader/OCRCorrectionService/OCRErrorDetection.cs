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
        /// FINAL VERSION: Orchestrates a dual-pathway error detection strategy and consolidates results
        /// using a robust, regex-based deduplication key to prevent redundant learning.
        /// </summary>
        private async Task<List<InvoiceError>> DetectInvoiceErrorsAsync(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            var allDetectedErrors = new List<InvoiceError>();
            if (invoice == null) return allDetectedErrors;

            _logger.Error("🚀 **DETECTION_PIPELINE_ENTRY_V2**: Starting DUAL-PATHWAY error detection for invoice {InvoiceNo}", invoice.InvoiceNo);

            try
            {
                // --- PATHWAY 1: DEEPSEEK AI-BASED DETECTION ---
                _logger.Error("🤖 **DEEPSEEK_DETECTION_START**: Initiating primary AI-based granular error and omission detection.");
                var deepSeekErrors = await DetectHeaderFieldErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                allDetectedErrors.AddRange(deepSeekErrors);

                // --- PATHWAY 2: RULE-BASED DETECTION (RELIABILITY BACKSTOP) ---
                if (fileText?.IndexOf("Amazon.com", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _logger.Error("🎯 **RULE_BASED_TRIGGER**: Amazon.com detected. Running secondary rule-based detector.");
                    var amazonErrors = DetectAmazonSpecificErrors(invoice, fileText);
                    allDetectedErrors.AddRange(amazonErrors);
                }

                // --- CONSOLIDATION (THE CRITICAL FIX) ---
                _logger.Information("  **CONSOLIDATION_START**: Consolidating {TotalCount} raw errors using robust regex-based key.", allDetectedErrors.Count);

                // The new, correct key for a duplicate is: Same Field, Same Value, and a Regex that describes the line.
                // We will use the SuggestedRegex for this.
                var uniqueErrors = allDetectedErrors
                    .GroupBy(e => new { Field = e.Field?.ToLowerInvariant(), Value = e.CorrectValue, Regex = e.SuggestedRegex })
                    .Select(g => {
                        var bestError = g.OrderByDescending(e => e.Confidence).First();
                        _logger.Debug("    - For Key [Field: '{Field}', Value: '{Value}', Regex: '{Regex}'], selected best error with confidence {Confidence:P2}",
                            g.Key.Field, g.Key.Value, g.Key.Regex, bestError.Confidence);
                        return bestError;
                    })
                    .ToList();

                if (!uniqueErrors.Any())
                {
                    _logger.Error("❌ **INTENTION_FAILED**: The detection pipeline found ZERO unique errors. This is the root cause of the correction failure.");
                }

                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
                var serializedErrors = JsonSerializer.Serialize(uniqueErrors, options);
                _logger.Error("✅ **DETECTION_PIPELINE_OUTPUT_DUMP**: Final list of {Count} unique InvoiceError objects: {SerializedErrors}",
                    uniqueErrors.Count, serializedErrors);

                return uniqueErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 CRITICAL ERROR during DetectInvoiceErrorsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
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

            // --- Check for Gift Card Amount (TotalInsurance) ---
            var giftCardRegex = new Regex(@"(Gift Card Amount:\s*-?\$?)([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
            var giftCardMatch = giftCardRegex.Match(fileText);
            if (giftCardMatch.Success)
            {
                amazonErrors.Add(new InvoiceError
                {
                    Field = "TotalInsurance",
                    ErrorType = "omission",
                    CorrectValue = $"-{giftCardMatch.Groups[2].Value.Trim()}", // Always negative
                    Confidence = 0.98,
                    Reasoning = "Rule-based: Amazon Gift Card Amount detected.",
                    LineText = giftCardMatch.Value.Trim(),
                    LineNumber = GetLineNumberForMatch(lines, giftCardMatch),
                    SuggestedRegex = @"Gift Card Amount:\s*-?\$?(?<TotalInsurance>[\d,]+\.?\d*)" // CANONICAL REGEX
                });
            }

            // --- Check for Free Shipping (TotalDeduction) ---
            var freeShippingRegex = new Regex(@"(Free Shipping:\s*-?\$?)([\d,]+\.?\d*)", RegexOptions.IgnoreCase);
            foreach (Match match in freeShippingRegex.Matches(fileText))
            {
                amazonErrors.Add(new InvoiceError
                {
                    Field = "TotalDeduction",
                    ErrorType = "omission",
                    CorrectValue = match.Groups[2].Value.Trim(), // Value is positive
                    Confidence = 0.95,
                    Reasoning = "Rule-based: Individual Free Shipping amount detected.",
                    LineText = match.Value.Trim(),
                    LineNumber = GetLineNumberForMatch(lines, match),
                    SuggestedRegex = @"Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)" // CANONICAL REGEX
                });
            }

            _logger.Error("🎯 **RULE_BASED_RESULT**: Rule-based Amazon detector found {Count} potential errors.", amazonErrors.Count);
            return amazonErrors;
        }

        /// <summary>
        /// Calls the DeepSeek API using the granular error detection prompt.
        /// </summary>
        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAndOmissionsAsync(
            ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            _logger.Information("🤖 **DEEPSEEK_SUB_PROCESS**: Detecting header field errors/omissions for invoice {InvoiceNo} using DeepSeek.", invoice.InvoiceNo);
            try
            {
                var prompt = this.CreateHeaderErrorDetectionPrompt(invoice, fileText);
                var deepSeekResponseJson = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
                {
                    _logger.Warning("   -> ❌ Received empty or null response from DeepSeek for header error detection on invoice {InvoiceNo}.", invoice.InvoiceNo);
                    return new List<InvoiceError>();
                }

                var correctionResults = this.ProcessDeepSeekCorrectionResponse(deepSeekResponseJson, fileText);
                var detectedErrors = correctionResults.Select(cr => ConvertCorrectionResultToInvoiceError(cr)).ToList();

                _logger.Information("   -> DeepSeek response processing complete. Found {Count} items.", detectedErrors.Count);
                return detectedErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 Error in DetectHeaderFieldErrorsAndOmissionsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
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
            };
        }

        private int GetLineNumberForMatch(string[] lines, Match match)
        {
            if (match == null || !match.Success) return 0;
            int charCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (match.Index >= charCount && match.Index < charCount + lines[i].Length + Environment.NewLine.Length)
                {
                    return i + 1;
                }
                charCount += lines[i].Length + Environment.NewLine.Length;
            }
            return 0;
        }

        #endregion
    }
}