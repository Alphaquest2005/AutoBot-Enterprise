// File: OCRCorrectionService/OCRDeepSeekIntegration.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Handles direct interactions with the DeepSeek LLM API for OCR correction tasks,
    /// including general response parsing and specific regex creation/correction requests.
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region General DeepSeek Response Processing for Corrections

        public List<CorrectionResult> ProcessDeepSeekCorrectionResponse(
            string deepSeekResponseJson,
            string originalDocumentText)
        {
            var corrections = new List<CorrectionResult>();
            if (string.IsNullOrWhiteSpace(deepSeekResponseJson)) return corrections;

            try
            {
                JsonElement? responseDataRoot = ParseDeepSeekResponseToElement(deepSeekResponseJson);
                if (responseDataRoot != null)
                {
                    corrections = ExtractCorrectionsFromResponseElement(responseDataRoot.Value, originalDocumentText);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception during DeepSeek response processing.");
            }

            return corrections;
        }

        private JsonElement? ParseDeepSeekResponseToElement(string rawResponse)
        {
            try
            {
                var cleanJson = this.CleanJsonResponse(rawResponse);
                if (string.IsNullOrEmpty(cleanJson)) return null;
                using var document = JsonDocument.Parse(cleanJson);
                return document.RootElement.Clone();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to parse DeepSeek response into valid JSON structure.");
                return null;
            }
        }

        private List<CorrectionResult> ExtractCorrectionsFromResponseElement(
            JsonElement responseDataRoot,
            string originalDocumentText)
        {
            var corrections = new List<CorrectionResult>();
            if (responseDataRoot.ValueKind == JsonValueKind.Object
                && responseDataRoot.TryGetProperty("errors", out var errorsArray)
                && errorsArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in errorsArray.EnumerateArray())
                {
                    var cr = CreateCorrectionFromElement(element, originalDocumentText, 0);
                    if (cr != null) corrections.Add(cr);
                }
                
                // **NEW**: Check for explanation when no errors are found
                if (corrections.Count == 0 && responseDataRoot.TryGetProperty("explanation", out var explanationElement))
                {
                    var explanation = explanationElement.GetString();
                    if (!string.IsNullOrEmpty(explanation))
                    {
                        _logger.Information("🔍 **DEEPSEEK_EXPLANATION**: DeepSeek provided explanation for empty errors array: {Explanation}", explanation);
                        // Store explanation for diagnostic capture
                        _lastDeepSeekExplanation = explanation;
                    }
                    else
                    {
                        _logger.Warning("⚠️ **MISSING_EXPLANATION**: DeepSeek returned empty errors array without required explanation");
                    }
                }
                else if (corrections.Count == 0)
                {
                    _logger.Warning("⚠️ **NO_EXPLANATION_PROVIDED**: DeepSeek returned empty errors array without explanation field (prompt compliance issue)");
                }
            }

            return ValidateAndEnrichParsedCorrections(corrections, originalDocumentText);
        }

        // File: OCRCorrectionService/OCRDeepSeekIntegration.cs

        private CorrectionResult CreateCorrectionFromElement(JsonElement element, string originalText, int itemIndex)
        {
            var fieldName = this.GetStringValueWithLogging(element, "field", itemIndex);

            // For format corrections, newValue might be null, but pattern/replacement are key.
            var newValue = this.GetStringValueWithLogging(element, "correct_value", itemIndex, isOptional: true);
            var pattern = this.GetStringValueWithLogging(element, "pattern", itemIndex, isOptional: true);

            if (string.IsNullOrEmpty(fieldName) || (newValue == null && pattern == null)) return null;

            var correctionResult = new CorrectionResult
            {
                FieldName = fieldName,
                OldValue = this.GetStringValueWithLogging(element, "extracted_value", itemIndex, true),
                NewValue = newValue,
                LineText = this.GetStringValueWithLogging(element, "line_text", itemIndex, true),
                LineNumber = this.GetIntValueWithLogging(element, "line_number", itemIndex, 0, true),
                Confidence = this.GetDoubleValueWithLogging(element, "confidence", itemIndex, 0.8, true),
                Reasoning = this.GetStringValueWithLogging(element, "reasoning", itemIndex, true),
                CorrectionType = DetermineCorrectionType(
                               this.GetStringValueWithLogging(element, "extracted_value", itemIndex, true),
                               newValue,
                               this.GetStringValueWithLogging(element, "error_type", itemIndex, true)),
                SuggestedRegex = this.GetStringValueWithLogging(element, "suggested_regex", itemIndex, true),

                // =================================== FIX START ===================================
                Pattern = pattern,
                Replacement = this.GetStringValueWithLogging(element, "replacement", itemIndex, true),
                // ==================================== FIX END ====================================

                Success = true
            };

            // 🚀 **PHASE_2_ENHANCEMENT**: Handle multi-field extraction support
            _logger.Information("🔍 **MULTI_FIELD_PROCESSING**: Checking for multi-field extraction data in element for field {FieldName}", fieldName);
            
            // Check for captured_fields array (multi-field line extraction)
            if (element.TryGetProperty("captured_fields", out var capturedFieldsElement) && 
                capturedFieldsElement.ValueKind == JsonValueKind.Array)
            {
                var capturedFields = new List<string>();
                foreach (var fieldElement in capturedFieldsElement.EnumerateArray())
                {
                    if (fieldElement.ValueKind == JsonValueKind.String)
                    {
                        capturedFields.Add(fieldElement.GetString());
                    }
                }
                
                // Store captured fields as comma-separated string in WindowText for now
                // This preserves the data until we have full multi-field support in CorrectionResult
                correctionResult.WindowText = string.Join(",", capturedFields);
                _logger.Information("   - **CAPTURED_FIELDS**: Found {Count} captured fields: {Fields}", 
                    capturedFields.Count, string.Join(", ", capturedFields));
            }
            
            // Check for field_corrections array (format corrections within multi-field lines)
            if (element.TryGetProperty("field_corrections", out var fieldCorrectionsElement) && 
                fieldCorrectionsElement.ValueKind == JsonValueKind.Array)
            {
                var fieldCorrections = new List<string>();
                foreach (var correctionElement in fieldCorrectionsElement.EnumerateArray())
                {
                    if (correctionElement.TryGetProperty("field_name", out var fieldNameEl) &&
                        correctionElement.TryGetProperty("pattern", out var patternEl) &&
                        correctionElement.TryGetProperty("replacement", out var replacementEl))
                    {
                        var correctionFieldName = fieldNameEl.GetString();
                        var correctionPattern = patternEl.GetString();
                        var correctionReplacement = replacementEl.GetString();
                        fieldCorrections.Add($"{correctionFieldName}:{correctionPattern}→{correctionReplacement}");
                    }
                }
                
                if (fieldCorrections.Any())
                {
                    // Store field corrections in ExistingRegex field for now
                    // This preserves the data until we have full multi-field support
                    correctionResult.ExistingRegex = string.Join("|", fieldCorrections);
                    _logger.Information("   - **FIELD_CORRECTIONS**: Found {Count} field corrections: {Corrections}", 
                        fieldCorrections.Count, string.Join("; ", fieldCorrections));
                }
            }

            return correctionResult;
        }

        #endregion

        #region DeepSeek Regex Creation API Call & Parsing

        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeek(
            CorrectionResult correction,
            LineContext lineContext)
        {
            if (correction == null || lineContext == null) return null;
            try
            {
                var prompt = this.CreateRegexCreationPrompt(correction, lineContext);
                var responseJson = await this._deepSeekApi
                                       .GetCompletionAsync(prompt, this.DefaultTemperature, this.DefaultMaxTokens)
                                       .ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(responseJson)) return null;
                return ParseRegexCreationResponseJson(responseJson);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error requesting new regex from DeepSeek for field {FieldName}",
                    correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// NEW: Requests a CORRECTION for a previously failed regex pattern from DeepSeek.
        /// </summary>
        public async Task<RegexCreationResponse> RequestRegexCorrectionFromDeepSeek(
            CorrectionResult correction,
            LineContext lineContext,
            RegexCreationResponse failedResponse,
            string failureReason)
        {
            if (correction == null || lineContext == null || failedResponse == null) return null;
            try
            {
                _logger.Information("🤖 **DEEPSEEK_REGEX_CORRECTION**: Requesting fix for failed regex for field {FieldName}", correction.FieldName);

                var prompt = this.CreateRegexCorrectionPrompt(correction, lineContext, failedResponse, failureReason);
                var responseJson = await this._deepSeekApi
                                       .GetCompletionAsync(prompt, this.DefaultTemperature, this.DefaultMaxTokens)
                                       .ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(responseJson)) return null;

                return ParseRegexCreationResponseJson(responseJson);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 Error requesting regex CORRECTION from DeepSeek for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        private RegexCreationResponse ParseRegexCreationResponseJson(string responseJson)
        {
            try
            {
                var cleanJson = this.CleanJsonResponse(responseJson);
                if (string.IsNullOrWhiteSpace(cleanJson)) return null;

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;
                int dummyErrorIndex = -1;

                return new RegexCreationResponse
                {
                    Strategy =
                                   this.GetStringValueWithLogging(root, "strategy", dummyErrorIndex)
                                   ?? "create_new_line",
                    RegexPattern =
                                   this.GetStringValueWithLogging(root, "regex_pattern", dummyErrorIndex) ?? "",
                    CompleteLineRegex =
                                   this.GetStringValueWithLogging(
                                       root,
                                       "complete_line_regex",
                                       dummyErrorIndex,
                                       isOptional: true) ?? "",
                    IsMultiline =
                                   this.GetBooleanValueWithLogging(root, "is_multiline", dummyErrorIndex, false),
                    MaxLines = this.GetIntValueWithLogging(root, "max_lines", dummyErrorIndex, 1),
                    TestMatch =
                                   this.GetStringValueWithLogging(root, "test_match", dummyErrorIndex, isOptional: true)
                                   ?? "",
                    Confidence = this.GetDoubleValueWithLogging(root, "confidence", dummyErrorIndex, 0.75),
                    Reasoning =
                                   this.GetStringValueWithLogging(root, "reasoning", dummyErrorIndex, isOptional: true)
                                   ?? "",
                    PreservesExistingGroups = this.GetBooleanValueWithLogging(
                                   root,
                                   "preserves_existing_groups",
                                   dummyErrorIndex,
                                   true),
                };
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(
                    jsonEx,
                    "Error parsing DeepSeek regex creation JSON response. Cleaned snippet: {Snippet}",
                    TruncateForLog(this.CleanJsonResponse(responseJson), 200));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Generic error parsing regex creation response.");
                return null;
            }
        }

        #endregion

        #region Internal Helper Methods for Parsing & Contextualization

        private string FindOriginalValueInText(string fieldName, string text)
        {
            // Implementation can be simple or complex depending on needs.
            return null;
        }

        private int FindLineNumberInTextByFieldName(string fieldName, string text)
        {
            // Placeholder for a more complex implementation.
            return 0;
        }

        private int FindLineNumberInTextByExactLine(string lineToFind, string text)
        {
            if (string.IsNullOrEmpty(lineToFind) || string.IsNullOrEmpty(text))
                return 0;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(lineToFind))
                    return i + 1; // 1-based line number
            }
            return 0;
        }

        private List<CorrectionResult> ValidateAndEnrichParsedCorrections(
            List<CorrectionResult> corrections,
            string originalText)
        {
            foreach (var correction in corrections)
            {
                if (correction.LineNumber == 0 && !string.IsNullOrEmpty(correction.LineText))
                {
                    correction.LineNumber = FindLineNumberInTextByExactLine(correction.LineText, originalText);
                }
                EnrichContextLinesFromText(correction, originalText);
            }
            return corrections;
        }

        private void EnrichContextLinesFromText(CorrectionResult correction, string originalText)
        {
            if (correction.LineNumber <= 0 || string.IsNullOrEmpty(originalText)) return;

            var lines = originalText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int centerIndex = correction.LineNumber - 1;

            int beforeStart = Math.Max(0, centerIndex - 5);
            correction.ContextLinesBefore = lines.Skip(beforeStart).Take(centerIndex - beforeStart).ToList();

            int afterStart = centerIndex + 1;
            if (afterStart < lines.Length)
            {
                correction.ContextLinesAfter = lines.Skip(afterStart).Take(5).ToList();
            }
        }

        private string DetermineCorrectionType(string oldValue, string newValue, string errorTypeFromDeepSeek = null)
        {
            if (!string.IsNullOrEmpty(errorTypeFromDeepSeek)) return errorTypeFromDeepSeek;
            if (string.IsNullOrEmpty(oldValue) && newValue != null) return "omission";

            if (oldValue != null && newValue != null)
            {
                var oldNormalized = Regex.Replace(oldValue, @"[\s\$,€£\-()]", "");
                var newNormalized = Regex.Replace(newValue, @"[\s\$,€£\-()]", "");
                if (string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase)
                    && oldValue != newValue)
                {
                    return "format_error";
                }

                return "value_correction";
            }

            return "unknown";
        }

        #endregion
    }
}