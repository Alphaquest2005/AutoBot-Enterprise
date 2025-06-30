// File: OCRCorrectionService/OCRPatternCreation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Amazon-Specific Pattern Definitions

        private static readonly Dictionary<string, string> AmazonSpecificPatterns = new Dictionary<string, string>
        {
            ["TotalInsurance"] = @"Gift Card Amount:\s*-?\$?(?<TotalInsurance>[\d,]+\.?\d*)",
            ["TotalDeduction"] = @"Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)",
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
        /// Validates a correction result, including its suggested regex, against internal business logic and format rules.
        /// This is an internal check before a pattern is sent for more rigorous validation.
        /// </summary>
        public CorrectionResult ValidatePatternInternal(CorrectionResult correction)
        {
            // REMOVED LogLevelOverride to prevent singleton violations - caller controls logging level
            _logger.Debug("🔍 **INTERNAL_VALIDATION**: Starting internal validation for Field '{FieldName}'", correction?.FieldName);

            if (correction == null)
            {
                _logger.Debug("❌ **INTERNAL_VALIDATION_FAIL**: Correction object is null.");
                return null;
            }

            try
            {
                // Step 1: Check if the field is supported for database learning
                if (!this.IsFieldSupported(correction.FieldName))
                {
                    correction.Success = false;
                    correction.Reasoning = $"Field '{correction.FieldName}' is not supported for database updates.";
                    _logger.Debug("❌ **INTERNAL_VALIDATION_FAIL**: {Reasoning}", correction.Reasoning);
                    return correction;
                }

                // Step 2: Validate the format of the new value against the field's expected data type
                if (!string.IsNullOrEmpty(correction.NewValue))
                {
                    var fieldInfo = this.GetFieldValidationInfo(correction.FieldName);
                    if (fieldInfo.IsValid && !string.IsNullOrEmpty(fieldInfo.ValidationPattern) && !Regex.IsMatch(
                            correction.NewValue, fieldInfo.ValidationPattern))
                    {
                        correction.Success = false;
                        correction.Reasoning = $"New value '{correction.NewValue}' does not match the expected pattern '{fieldInfo.ValidationPattern}' for data type '{fieldInfo.DataType}'.";
                        _logger.Debug("❌ **INTERNAL_VALIDATION_FAIL**: {Reasoning}", correction.Reasoning);
                        return correction;
                    }
                }

                // Step 3: Check the syntax of any suggested regex
                if (!string.IsNullOrEmpty(correction.SuggestedRegex))
                {
                    try
                    {
                        _ = new Regex(correction.SuggestedRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    catch (ArgumentException ex)
                    {
                        correction.Success = false;
                        correction.Reasoning = $"Invalid regex pattern syntax: {ex.Message}";
                        _logger.Debug("❌ **INTERNAL_VALIDATION_FAIL**: {Reasoning}", correction.Reasoning);
                        return correction;
                    }
                }

                _logger.Debug("✅ **INTERNAL_VALIDATION_PASS**: Field '{FieldName}' passed internal checks.", correction.FieldName);
                return correction;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **INTERNAL_VALIDATION_EXCEPTION**: Exception during pattern validation for {FieldName}", correction.FieldName);
                correction.Success = false;
                correction.Reasoning = $"Exception during validation: {ex.Message}";
                return correction;
            }
        }


        /// <summary>
        /// CRITICAL FIX v3: Validates a regex pattern from DeepSeek. This is the gatekeeper for new learnings.
        /// It ensures the pattern is syntactically valid and correctly extracts the expected value from the provided context,
        /// specifically checking that the required NAMED capture group exists and is successful.
        /// </summary>
        public bool ValidateRegexPattern(RegexCreationResponse regexResponse, CorrectionResult correction)
        {
            // REMOVED LogLevelOverride to prevent singleton violations - caller controls logging level
            _logger.Debug("🔬 **REGEX_VALIDATION_START**: Validating new regex pattern for Field '{FieldName}'.", correction.FieldName);

            if (regexResponse == null || string.IsNullOrWhiteSpace(regexResponse.RegexPattern))
            {
                _logger.Debug("    - ❌ **VALIDATION_FAIL**: Regex response or pattern is null or empty.");
                return false;
            }

            _logger.Debug("    - [Input Data Dump]");
            _logger.Debug("      - Correction Field: '{FieldName}', Expected Value: '{NewValue}'", correction.FieldName, correction.NewValue);
            _logger.Debug("      - Regex Pattern: '{RegexPattern}'", regexResponse.RegexPattern);
            _logger.Debug("      - Test Match Text: '{TestMatch}'", regexResponse.TestMatch ?? correction.LineText);

            try
            {
                var regex = new Regex(regexResponse.RegexPattern, RegexOptions.IgnoreCase | (regexResponse.IsMultiline ? RegexOptions.Multiline : RegexOptions.None));
                _logger.Debug("    - ✅ **SYNTAX_CHECK_PASS**: Regex compiled successfully.");

                string textToTest = !string.IsNullOrEmpty(regexResponse.TestMatch) ? regexResponse.TestMatch : correction.LineText;
                if (string.IsNullOrEmpty(textToTest))
                {
                    _logger.Debug("    - ❌ **VALIDATION_FAIL**: No text available (neither TestMatch nor LineText) to test the regex against.");
                    return false;
                }

                var match = regex.Match(textToTest);
                if (!match.Success)
                {
                    _logger.Debug("    - ❌ **VALIDATION_FAIL**: The regex pattern did not find any match in the test text: '{TextToTest}'", textToTest);
                    return false;
                }
                _logger.Debug("    - ✅ **MATCH_SUCCESS**: Regex found a match in the test text: '{MatchValue}'", match.Value);

                // CRITICAL FIX: Check for the NAMED group, not just any group.
                var namedGroup = match.Groups[correction.FieldName];
                if (!namedGroup.Success)
                {
                    _logger.Debug("    - ❌ **VALIDATION_FAIL**: A group named '{FieldName}' was NOT found or did not capture a value. Available groups: {Groups}",
                        correction.FieldName, string.Join(", ", regex.GetGroupNames()));
                    return false;
                }

                // Clean the extracted value for comparison (remove currency, whitespace, and negative signs for absolute comparison).
                string extractedValueClean = Regex.Replace(namedGroup.Value, @"[\s\$,-]", "");
                _logger.Debug("    - [EXTRACTION_RESULT]: Extracted raw value '{Raw}' from named group '{FieldName}', cleaned to '{Cleaned}'", namedGroup.Value, correction.FieldName, extractedValueClean);

                string expectedValueClean = Regex.Replace(correction.NewValue, @"[\s\$,-]", "");
                _logger.Debug("    - [COMPARISON]: Comparing Cleaned Extracted='{ExtractedValue}' vs Cleaned Expected='{ExpectedValue}'", extractedValueClean, expectedValueClean);

                // Use a robust numeric comparison on the absolute values
                if (decimal.TryParse(extractedValueClean, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var extractedDecimal) &&
                    decimal.TryParse(expectedValueClean, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var expectedDecimal))
                {
                    bool isMatch = Math.Abs(extractedDecimal - expectedDecimal) < 0.01m;
                    if (isMatch) _logger.Debug("    - ✅ **VALIDATION_PASS**: Numeric values match."); else _logger.Debug("    - ❌ **VALIDATION_FAIL**: Numeric values do not match.");
                    return isMatch;
                }

                // Fallback to string comparison
                bool stringMatch = string.Equals(extractedValueClean, expectedValueClean, StringComparison.OrdinalIgnoreCase);
                if (stringMatch) _logger.Debug("    - ✅ **VALIDATION_PASS**: String values match."); else _logger.Debug("    - ❌ **VALIDATION_FAIL**: String values do not match.");
                return stringMatch;
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex, "    - ❌ **VALIDATION_FAIL**: The regex pattern is syntactically invalid.");
                return false;
            }
            finally
            {
                _logger.Debug("🔬 **REGEX_VALIDATION_END**");
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