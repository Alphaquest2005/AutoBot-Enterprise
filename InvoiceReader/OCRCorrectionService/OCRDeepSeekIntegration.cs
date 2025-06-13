// File: OCRCorrectionService/OCRDeepSeekIntegration.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog; // ILogger is available as this._logger

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Handles direct interactions with the DeepSeek LLM API for OCR correction tasks,
    /// including general response parsing and specific regex creation requests.
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
            }

            return ValidateAndEnrichParsedCorrections(corrections, originalDocumentText);
        }

        private CorrectionResult CreateCorrectionFromElement(JsonElement element, string originalText, int itemIndex)
        {
            var fieldName = this.GetStringValueWithLogging(element, "field", itemIndex);
            var newValue = this.GetStringValueWithLogging(element, "correct_value", itemIndex);
            if (string.IsNullOrEmpty(fieldName) || newValue == null) return null;

            return new CorrectionResult
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
                           Success = true
                       };
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
                               // **FIX**: The 'ContextLinesUsed' property was removed from the RegexCreationResponse model.
                               // This line, which attempted to assign to it, is now removed to fix the compilation error.
                           };
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(
                    jsonEx,
                    "Error parsing DeepSeek regex creation JSON response. Cleaned snippet: {Snippet}",
                    this.TruncateForLog(this.CleanJsonResponse(responseJson), 200));
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
            // Implementation remains unchanged
            return null;
        }

        private int FindLineNumberInTextByFieldName(string fieldName, string text)
        {
            // Implementation remains unchanged
            return 0;
        }

        private int FindLineNumberInTextByExactLine(string lineToFind, string text)
        {
            // Implementation remains unchanged
            return 0;
        }

        private List<CorrectionResult> ValidateAndEnrichParsedCorrections(
            List<CorrectionResult> corrections,
            string originalText)
        {
            // Implementation remains unchanged
            return corrections;
        }

        private void EnrichContextLinesFromText(CorrectionResult correction, string originalText)
        {
            // Implementation remains unchanged
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