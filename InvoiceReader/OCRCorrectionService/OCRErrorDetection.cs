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
        /// ENHANCED v3: Orchestrates a dual-pathway error detection strategy (AI + Rule-Based)
        /// to produce a consolidated, granular list of potential invoice errors for the learning pipeline.
        /// </summary>
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

            _logger.Error("🚀 **DETECTION_PIPELINE_ENTRY**: Starting DUAL-PATHWAY error detection for invoice {InvoiceNo}", invoice.InvoiceNo);
            _logger.Information("    **ARCHITECTURAL_INTENT**: Employ a dual-pathway strategy (AI + Rule-Based) and consolidate results to produce the most accurate set of granular corrections.");

            try
            {
                // --- PATHWAY 1: DEEPSEEK AI-BASED DETECTION (GRANULAR) ---
                _logger.Error("🤖 **DEEPSEEK_DETECTION_START**: Initiating primary AI-based granular error and omission detection.");
                var deepSeekErrors = await DetectHeaderFieldErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                _logger.Error("🤖 **DEEPSEEK_DETECTION_RESULT**: DeepSeek pathway returned {Count} potential granular errors.", deepSeekErrors.Count);
                allDetectedErrors.AddRange(deepSeekErrors);

                // --- PATHWAY 2: RULE-BASED DETECTION (RELIABILITY BACKSTOP) ---
                if (fileText?.IndexOf("Amazon.com", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _logger.Error("🎯 **RULE_BASED_TRIGGER**: Amazon.com detected. Running secondary rule-based detector for reliability.");
                    var amazonErrors = DetectAmazonSpecificErrors(invoice, fileText);
                    _logger.Error("🎯 **RULE_BASED_RESULT**: Rule-based Amazon detector returned {Count} potential errors.", amazonErrors.Count);
                    allDetectedErrors.AddRange(amazonErrors);
                }

                // --- CONSOLIDATION & DEDUPLICATION ---
                _logger.Information("  **CONSOLIDATION_START**: Consolidating {TotalCount} raw errors from all detection pathways.", allDetectedErrors.Count);
                var uniqueErrors = allDetectedErrors
                    .GroupBy(e => new { Field = e.Field?.ToLowerInvariant(), Line = e.LineNumber, Value = e.CorrectValue })
                    .Select(g => {
                        var bestError = g.OrderByDescending(e => e.Confidence).First();
                        _logger.Debug("    - For Field '{Field}' on Line {Line}, selected best error with confidence {Confidence:P2} (Reason: {Reasoning})", bestError.Field, bestError.LineNumber, bestError.Confidence, bestError.Reasoning);
                        return bestError;
                    })
                    .ToList();

                if (uniqueErrors.Any())
                {
                    _logger.Error("✅ **INTENTION_MET**: As expected, the detection pipeline found {Count} unique errors. The correction process will now proceed.", uniqueErrors.Count);
                }
                else
                {
                    _logger.Error("❌ **INTENTION_FAILED**: The detection pipeline found ZERO errors. If the invoice is unbalanced, this is the root cause of the correction failure.");
                }

                // --- MANDATE LOG: Serialize the final, consolidated output ---
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
                var serializedErrors = System.Text.Json.JsonSerializer.Serialize(uniqueErrors, options);
                _logger.Error("✅ **DETECTION_PIPELINE_OUTPUT_DUMP**: Final list of {Count} unique InvoiceError objects: {SerializedErrors}", uniqueErrors.Count, serializedErrors);

                return uniqueErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 CRITICAL ERROR during DetectInvoiceErrorsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        /// <summary>
        /// Rule-based detector for common, high-volume Amazon invoice omissions. Provides reliable, granular data.
        /// </summary>
        private List<InvoiceError> DetectAmazonSpecificErrors(ShipmentInvoice invoice, string fileText)
        {
            var amazonErrors = new List<InvoiceError>();
            var lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var uniqueAmounts = new HashSet<string>();

            _logger.Error("🔍 **AMAZON_DETECTOR_START**: Running rule-based detection for Amazon invoice.");

            // --- Check for Gift Card Amount (TotalInsurance) ---
            var giftCardMatch = new Regex(@"Gift Card Amount:\s*(-?\$?[\d,]+\.?\d*)", RegexOptions.IgnoreCase).Match(fileText);
            if (giftCardMatch.Success)
            {
                string rawValue = giftCardMatch.Groups[1].Value;
                if (uniqueAmounts.Add($"giftcard_{rawValue}"))
                {
                    var giftCardValue = rawValue.Replace("$", "").Trim();
                    amazonErrors.Add(new InvoiceError
                    {
                        Field = "TotalInsurance",
                        ErrorType = "omission",
                        CorrectValue = giftCardValue,
                        ExtractedValue = invoice.TotalInsurance?.ToString() ?? "null",
                        Confidence = 0.98,
                        Reasoning = "Rule-based: Amazon Gift Card Amount detected. Maps to TotalInsurance per Caribbean customs.",
                        LineText = giftCardMatch.Value.Trim(),
                        LineNumber = GetLineNumberForMatch(lines, giftCardMatch),
                    });
                }
            }

            // --- Check for Free Shipping (TotalDeduction) ---
            var freeShippingMatches = new Regex(@"Free Shipping:\s*(-?\$?[\d,]+\.?\d*)", RegexOptions.IgnoreCase).Matches(fileText);
            foreach (Match match in freeShippingMatches)
            {
                string rawValue = match.Groups[1].Value;
                if (uniqueAmounts.Add($"freeship_{rawValue}"))
                {
                    string cleanValueStr = rawValue.Replace("$", "").Replace("-", "").Trim();
                    amazonErrors.Add(new InvoiceError
                    {
                        Field = "TotalDeduction",
                        ErrorType = "omission",
                        CorrectValue = cleanValueStr,
                        ExtractedValue = "0",
                        Confidence = 0.95,
                        Reasoning = "Rule-based: Individual Free Shipping amount detected for pattern learning.",
                        LineText = match.Value.Trim(),
                        LineNumber = GetLineNumberForMatch(lines, match),
                    });
                }
            }

            _logger.Error("🔍 **AMAZON_DETECTOR_END**: Finished rule-based detection. Found {Count} unique errors.", amazonErrors.Count);
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