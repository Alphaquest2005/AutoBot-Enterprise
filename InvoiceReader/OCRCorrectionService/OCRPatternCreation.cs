// File: OCRCorrectionService/OCRPatternCreation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog; // ILogger is available as this._logger
using Serilog.Events;
using Core.Common.Extensions;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Amazon-Specific Pattern Definitions (Phase 5)

        private static readonly Dictionary<string, string> AmazonSpecificPatterns = new Dictionary<string, string>
            {
                ["TotalInsurance"] = @"Gift Card Amount:\s*\$?(?<TotalInsurance>-?[\d,]+\.?\d*)",
                ["TotalDeduction"] = @"Free Shipping:\s*\$?(?<TotalDeduction>-?[\d,]+\.?\d*)",
                ["TotalInternalFreight"] = @"Shipping & Handling:\s*\$?(?<TotalInternalFreight>[\d,]+\.?\d*)",
                ["TotalOtherCost"] = @"Estimated tax to be collected:\s*\$?(?<TotalOtherCost>[\d,]+\.?\d*)",
                ["SubTotal"] = @"Item\(s\) Subtotal:\s*\$?(?<SubTotal>[\d,]+\.?\d*)",
                ["InvoiceTotal"] = @"Grand Total:\s*\$?(?<InvoiceTotal>[\d,]+\.?\d*)"
            };

        private string GetAmazonSpecificPattern(string fieldName, string invoiceText)
        {
            if (AmazonSpecificPatterns.ContainsKey(fieldName) && invoiceText?.IndexOf(
                    "Amazon.com",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return AmazonSpecificPatterns[fieldName];
            }

            return null;
        }

        #endregion

        #region Format Correction Pattern Creation

        public (string Pattern, string Replacement)? CreateAdvancedFormatCorrectionPatterns(
            string originalValue,
            string correctedValue)
        {
            if (string.IsNullOrWhiteSpace(originalValue) || string.IsNullOrWhiteSpace(correctedValue)
                                                         || originalValue == correctedValue) return null;
            var strategies = new Func<string, string, (string, string)?>[]
                                 {
                                     CreateDecimalSeparatorPattern, CreateCurrencySymbolPattern,
                                     CreateNegativeNumberPattern, CreateSpecificOCRCharacterConfusionPattern,
                                     CreateSpaceManipulationPattern
                                 };
            foreach (var strategy in strategies)
            {
                var result = strategy(originalValue, correctedValue);
                if (result.HasValue) return result;
            }

            return null;
        }

        private (string, string)? CreateDecimalSeparatorPattern(string o, string c) =>
            o.Contains(",") && c.Contains(".") && o.Replace(",", ".") == c
                ? (@"(\d+),(\d{1,4})", "$1.$2")
                : (o.Contains(".") && c.Contains(",") && o.Replace(".", ",") == c
                       ? (@"(\d+)\.(\d{1,4})", "$1,$2")
                       : ((string, string)?)null);

        private (string, string)? CreateCurrencySymbolPattern(string o, string c)
        {
            var s = c.FirstOrDefault(x => !char.IsLetterOrDigit(x) && !char.IsWhiteSpace(x) && x != '.' && x != ',')
                .ToString();
            return !string.IsNullOrEmpty(s) && !o.Contains(s) && c.StartsWith(s)
                       ? (@"^(-?\d+(\.\d+)?)$", (s == "$" ? "$$" : s) + "$1")
                       : ((string, string)?)null;
        }

        private (string, string)? CreateNegativeNumberPattern(string o, string c) =>
            o.EndsWith("-") && c.StartsWith("-") && o.TrimEnd('-') == c.TrimStart('-')
                ? (@"(\d+(\.\d+)?)-$", "-$1")
                : (o.StartsWith("(") && o.EndsWith(")") && c.StartsWith("-")
                   && o.Substring(1, o.Length - 2).Trim() == c.TrimStart('-')
                       ? (@"\(\s*(\d+(\.\d+)?)\s*\)", "-$1")
                       : ((string, string)?)null);

        private (string, string)? CreateSpecificOCRCharacterConfusionPattern(string o, string c) =>
            o.Length != c.Length ? ((string, string)?)null : null;

        private (string, string)? CreateSpaceManipulationPattern(string o, string c) =>
            o.Contains(" ") && !c.Contains(" ") && o.Replace(" ", "") == c
                ? (Regex.Escape(o).Replace(@"\ ", @"\s*"), c)
                : ((string, string)?)null;

        #endregion

        #region Shared Pattern Utilities & Validation

        /// <summary>
        /// Validates generated regex pattern against known constraints. Now public for testing.
        /// </summary>
        public CorrectionResult ValidatePatternInternal(CorrectionResult correction)
        {
            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                _logger.Error(
                    "🔍 **VALIDATION_SECTION_ENTRY**: ValidatePatternInternal called for validation analysis");

                if (correction == null)
                {
                    _logger.Error("❌ **VALIDATION_NULL_INPUT**: Correction parameter is null - cannot validate");
                    return null;
                }

                try
                {
                    // Step 1: Check if the field is supported
                    if (!this.IsFieldSupported(correction.FieldName))
                    {
                        correction.Success = false;
                        correction.Reasoning = $"Field '{correction.FieldName}' is not supported for database updates";
                        _logger.Error("❌ **VALIDATION_STEP_1_FAILED**: {Reasoning}", correction.Reasoning);
                        return correction;
                    }

                    _logger.Error(
                        "✅ **VALIDATION_STEP_1_PASSED**: Field '{FieldName}' is supported",
                        correction.FieldName);

                    // Step 2: Validate new value format
                    if (!string.IsNullOrEmpty(correction.NewValue))
                    {
                        var fieldInfo = this.GetFieldValidationInfo(correction.FieldName);
                        if (!string.IsNullOrEmpty(fieldInfo.ValidationPattern) && !Regex.IsMatch(
                                correction.NewValue,
                                fieldInfo.ValidationPattern))
                        {
                            correction.Success = false;
                            correction.Reasoning =
                                $"Value '{correction.NewValue}' doesn't match expected pattern for field type '{fieldInfo.DataType}'";
                            _logger.Error("❌ **VALIDATION_STEP_2_FAILED**: {Reasoning}", correction.Reasoning);
                            return correction;
                        }
                    }

                    _logger.Error(
                        "✅ **VALIDATION_STEP_2_PASSED**: NewValue validation passed for '{FieldName}'",
                        correction.FieldName);

                    // Step 3: Validate regex syntax
                    if (!string.IsNullOrEmpty(correction.SuggestedRegex))
                    {
                        try
                        {
                            _ = new Regex(correction.SuggestedRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            _logger.Error(
                                "✅ **VALIDATION_STEP_3_PASSED**: Suggested regex for {FieldName} is syntactically valid",
                                correction.FieldName);
                        }
                        catch (ArgumentException ex)
                        {
                            correction.Success = false;
                            correction.Reasoning = $"Invalid regex pattern: {ex.Message}";
                            _logger.Error("❌ **VALIDATION_STEP_3_FAILED**: {Reasoning}", correction.Reasoning);
                            return correction;
                        }
                    }

                    return correction;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        ex,
                        "🚨 **VALIDATION_EXCEPTION**: Exception during pattern validation for {FieldName}",
                        correction.FieldName);
                    correction.Success = false;
                    correction.Reasoning = $"Exception during validation: {ex.Message}";
                    return correction;
                }
            }
        }

        // OCRPatternCreation.cs

        public bool ValidateRegexPattern(RegexCreationResponse regexResponse, CorrectionResult correction)
        {
            if (regexResponse == null || string.IsNullOrWhiteSpace(regexResponse.RegexPattern)) return false;

            try
            {
                var regex = new Regex(regexResponse.RegexPattern, RegexOptions.IgnoreCase | (regexResponse.IsMultiline ? RegexOptions.Multiline : RegexOptions.None));

                // NEW LOGIC: Prioritize the most specific text for validation.
                string textToTest = !string.IsNullOrEmpty(regexResponse.TestMatch)
                                        ? regexResponse.TestMatch
                                        : correction.LineText;

                if (string.IsNullOrEmpty(textToTest)) return false; // Cannot validate without test text.

                var match = regex.Match(textToTest);
                if (!match.Success) return false;

                // Check if the primary capture group matches the expected corrected value.
                var group = match.Groups.Count > 1 ? match.Groups[1] : match.Groups[0];
                string extractedValue = group.Value.Trim();

                // Simple numeric comparison for values
                if (decimal.TryParse(extractedValue, out var extractedDecimal) &&
                    decimal.TryParse(correction.NewValue, out var expectedDecimal))
                {
                    return Math.Abs(extractedDecimal - expectedDecimal) < 0.01m;
                }

                return string.Equals(extractedValue, correction.NewValue, StringComparison.OrdinalIgnoreCase);
            }
            catch (ArgumentException)
            {
                return false; // Invalid regex syntax
            }
        }

        public List<string> CreateFieldExtractionPatterns(string fieldName, IEnumerable<string> sampleValues)
        {
            var patterns = new List<string>();
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldName);

            var keysToUse = fieldInfo != null
                                ? DeepSeekToDBFieldMapping.Where(kvp => kvp.Value == fieldInfo).Select(kvp => kvp.Key)
                                    .Union(new[] { fieldInfo.DisplayName }).Distinct().ToList()
                                : new List<string> { fieldName };

            var keyGroupPattern = string.Join("|", keysToUse.Select(k => Regex.Escape(k).Replace(" ", "\\s+")));
            patterns.Add($@"(?i)(?:({keyGroupPattern})\s*[:\-=\s]\s*)?(.+)");

            return patterns;
        }

        #endregion
    }

}