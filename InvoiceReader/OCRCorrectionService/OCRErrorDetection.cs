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

            _logger.Information("    **ARCHITECTURAL_INTENT**: This service employs a dual-pathway error detection strategy to maximize both flexibility and reliability.");
            _logger.Information("    - **Pathway 1 (AI - DeepSeek):** Provides intelligent, adaptive detection for new or unknown invoice formats.");
            _logger.Information("    - **Pathway 2 (Rule-Based):** Provides fast, deterministic, and guaranteed detection for known, high-volume formats like Amazon.");
            _logger.Information("    - **Consolidation:** Results from both pathways are collected and deduplicated to produce the most accurate set of corrections.");

            try
            {
                _logger.Error("    **INTENTION_ASSERTION**: For unbalanced invoice {InvoiceNo}, we EXPECT to detect at least one error (e.g., the missing TotalDeduction) to trigger a correction.", invoice.InvoiceNo);

                // --- PATHWAY 1: DEEPSEEK AI-BASED DETECTION ---
                _logger.Error("🤖 **DEEPSEEK_DETECTION_START**: Initiating primary AI-based error and omission detection.");
                var deepSeekErrors = await DetectHeaderFieldErrorsAndOmissionsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                _logger.Error("🤖 **DEEPSEEK_DETECTION_RESULT**: DeepSeek pathway returned {Count} potential errors.", deepSeekErrors.Count);
                allDetectedErrors.AddRange(deepSeekErrors);

                // --- PATHWAY 2: RULE-BASED DETECTION ---
                if (fileText?.IndexOf("Amazon.com", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _logger.Error("🎯 **RULE_BASED_TRIGGER**: Amazon.com detected. Running secondary rule-based detector as a reliability backstop.");
                    var amazonErrors = DetectAmazonSpecificErrors(invoice, fileText);
                    _logger.Error("🎯 **RULE_BASED_RESULT**: Rule-based Amazon detector returned {Count} potential errors.", amazonErrors.Count);
                    allDetectedErrors.AddRange(amazonErrors);
                }

                // --- CONSOLIDATION ---
                _logger.Information("  **CONSOLIDATION_START**: Consolidating {TotalCount} raw errors from all detection pathways.", allDetectedErrors.Count);
                var uniqueErrors = allDetectedErrors
                    .GroupBy(e => new { Field = e.Field?.ToLowerInvariant(), Line = e.LineNumber })
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
                    _logger.Error("❌ **INTENTION_FAILED**: The detection pipeline found ZERO errors.");
                    _logger.Error("   **DIAGNOSTIC_GUIDANCE**: If you are trying to figure out why the invoice was not corrected or why no database entries were created, THIS IS THE ROOT CAUSE.");
                    _logger.Error("   **NEXT_STEP**: Investigate the output of the 'DEEPSEEK_DETECTION_RESULT' and 'RULE_BASED_RESULT' logs above to see why neither pathway found any omissions or errors.");
                }

                _logger.Error("✅ **DETECTION_PIPELINE_COMPLETE**: Total unique errors after consolidation: {ErrorCount} for invoice {InvoiceNo}.", uniqueErrors.Count, invoice.InvoiceNo);
                return uniqueErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 CRITICAL ERROR during DetectInvoiceErrorsAsync for invoice {InvoiceNo}.", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAndOmissionsAsync(
            ShipmentInvoice invoice,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            _logger.Information("🤖 **DEEPSEEK_SUB_PROCESS**: Detecting header field errors/omissions for invoice {InvoiceNo} using DeepSeek.", invoice.InvoiceNo);

            try
            {
                var prompt = this.CreateHeaderErrorDetectionPrompt(invoice, fileText);
                _logger.Debug("   -> Sending prompt of length {Length} to DeepSeek API.", prompt.Length);

                var deepSeekResponseJson = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
                {
                    _logger.Warning("   -> ❌ Received empty or null response from DeepSeek for header error detection on invoice {InvoiceNo}.", invoice.InvoiceNo);
                    return new List<InvoiceError>();
                }

                _logger.Information("   -> ✅ Received non-empty response from DeepSeek. Processing...");
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

        private List<InvoiceError> DetectAmazonSpecificErrors(ShipmentInvoice invoice, string fileText)
        {
            var amazonErrors = new List<InvoiceError>();
            var lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            _logger.Error("🔍 **AMAZON_DETECTOR_START**: Running rule-based detection for Amazon invoice.");
            _logger.Information("    **DESIGN_BACKSTORY**: The existing template's 'FreeShipping' regex is intentionally strict to handle a specific Amazon format that includes a currency code. For the current invoice format which lacks this code, a failed match by the template is **EXPECTED BEHAVIOR**. This rule-based detector's purpose is to act as the fallback to correctly capture this omission.");

            // --- Check for Gift Card Amount (TotalInsurance) ---
            _logger.Information("   -> Rule 1: Checking for omitted 'TotalInsurance' (Gift Card).");
            _logger.Information("      - Current Invoice.TotalInsurance value: {Value}", invoice.TotalInsurance?.ToString("F2") ?? "null");
            if (!invoice.TotalInsurance.HasValue || invoice.TotalInsurance.Value == 0)
            {
                _logger.Information("      - Condition MET: TotalInsurance is missing or zero.");
                string giftCardPattern = @"Gift Card Amount:\s*(-?\$[\d,]+\.?\d*)";
                _logger.Information("      - Regex Pattern: {Pattern}", giftCardPattern);

                var giftCardRegex = new Regex(giftCardPattern, RegexOptions.IgnoreCase);
                var giftCardMatch = giftCardRegex.Match(fileText);

                if (giftCardMatch.Success)
                {
                    _logger.Error("      - ✅ MATCH FOUND! Text: '{MatchText}'", giftCardMatch.Value.Trim());
                    var giftCardValue = giftCardMatch.Groups[1].Value.Replace("$", "").Trim();
                    amazonErrors.Add(new InvoiceError
                    {
                        Field = "TotalInsurance",
                        ErrorType = "omission",
                        ExtractedValue = invoice.TotalInsurance?.ToString() ?? "null",
                        CorrectValue = giftCardValue,
                        Confidence = 0.98,
                        Reasoning = "Rule-based detection: Amazon Gift Card Amount detected. Maps to TotalInsurance per Caribbean customs rules.",
                        LineText = giftCardMatch.Value.Trim(),
                        LineNumber = GetLineNumberForMatch(lines, giftCardMatch),
                    });
                }
                else
                {
                    _logger.Warning("      - ❌ NO MATCH FOUND for Gift Card pattern in the provided text.");
                }
            }
            else
            {
                _logger.Information("      - Condition NOT MET. Skipping rule, TotalInsurance already has a non-zero value.");
            }

            // --- Check for Free Shipping (TotalDeduction) ---
            _logger.Information("   -> Rule 2: Checking for omitted 'TotalDeduction' (Free Shipping).");
            _logger.Information("      - Current Invoice.TotalDeduction value: {Value}", invoice.TotalDeduction?.ToString("F2") ?? "null");
            if (!invoice.TotalDeduction.HasValue || invoice.TotalDeduction.Value == 0)
            {
                _logger.Information("      - Condition MET: TotalDeduction is missing or zero.");
                string freeShippingPattern = @"Free Shipping:\s*(-?\$[\d,]+\.?\d*)";
                _logger.Information("      - Regex Pattern: {Pattern}", freeShippingPattern);

                var freeShippingRegex = new Regex(freeShippingPattern, RegexOptions.IgnoreCase);
                var freeShippingMatches = freeShippingRegex.Matches(fileText);

                _logger.Information("      - Regex search complete. Found {Count} matches.", freeShippingMatches.Count);

                if (freeShippingMatches.Count > 0)
                {
                    foreach (Match match in freeShippingMatches)
                    {
                        _logger.Error("      - ✅ MATCH FOUND! Text: '{MatchText}'", match.Value.Trim());
                        var valueStr = match.Groups[1].Value;
                        string cleanValueStr = valueStr.Replace("$", "").Replace(",", "").Trim();
                        if (double.TryParse(cleanValueStr, out var amount))
                        {
                            amazonErrors.Add(new InvoiceError
                            {
                                Field = "TotalDeduction",
                                ErrorType = "omission_aggregate_part",
                                CorrectValue = Math.Abs(amount).ToString("F2"),
                                ExtractedValue = "0",
                                Confidence = 0.95,
                                Reasoning = $"Part of an aggregate sum for supplier reduction (Free Shipping).",
                                LineText = match.Value.Trim(),
                                LineNumber = GetLineNumberForMatch(lines, match),
                            });
                        }
                    }
                }
                else
                {
                    _logger.Warning("      - ❌ NO MATCH FOUND for Free Shipping pattern in the provided text.");
                }
            }
            else
            {
                _logger.Information("      - Condition NOT MET. Skipping rule, TotalDeduction already has a non-zero value.");
            }

            _logger.Error("🔍 **AMAZON_DETECTOR_END**: Finished rule-based detection. Found {Count} total errors.", amazonErrors.Count);
            return amazonErrors;
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
                if (match.Index >= charCount && match.Index < charCount + lines[i].Length + 1)
                {
                    return i + 1;
                }
                charCount += lines[i].Length + 1;
            }
            return 0;
        }

        #endregion
    }
}