using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Advanced pattern creation logic for OCR corrections
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Pattern Creation Methods

        /// <summary>
        /// Creates advanced format correction patterns for field format fixes
        /// </summary>
        /// <param name="originalValue">Original OCR value</param>
        /// <param name="correctedValue">Corrected value</param>
        /// <returns>Regex pattern and replacement or null if no pattern can be created</returns>
        public (string Pattern, string Replacement)? CreateAdvancedFormatCorrectionPatterns(string originalValue, string correctedValue)
        {
            if (string.IsNullOrWhiteSpace(originalValue) || string.IsNullOrWhiteSpace(correctedValue))
            {
                _logger?.Debug("Cannot create pattern for null/empty values: '{Original}' -> '{Corrected}'",
                    originalValue, correctedValue);
                return null;
            }

            if (originalValue == correctedValue)
            {
                _logger?.Debug("Original and corrected values are identical: '{Value}'", originalValue);
                return null;
            }

            _logger?.Debug("Creating format correction pattern: '{Original}' -> '{Corrected}'",
                originalValue, correctedValue);

            // Try different pattern creation strategies
            var strategies = new Func<(string, string)?>[]
            {
                () => CreateDecimalSeparatorPattern(originalValue, correctedValue),
                () => CreateCurrencySymbolPattern(originalValue, correctedValue),
                () => CreateNegativeNumberPattern(originalValue, correctedValue),
                () => CreateOCRCharacterConfusionPattern(originalValue, correctedValue),
                () => CreateSpaceRemovalPattern(originalValue, correctedValue),
                () => CreateGeneralFormatPattern(originalValue, correctedValue)
            };

            foreach (var strategy in strategies)
            {
                var result = strategy();
                if (result.HasValue && !string.IsNullOrEmpty(result.Value.Item1))
                {
                    _logger?.Debug("Created pattern using strategy: {Pattern} -> {Replacement}",
                        result.Value.Item1, result.Value.Item2);
                    return result;
                }
            }

            _logger?.Warning("No suitable pattern could be created for: '{Original}' -> '{Corrected}'",
                originalValue, correctedValue);
            return null;
        }

        /// <summary>
        /// Creates pattern for decimal comma to point conversion
        /// </summary>
        private (string Pattern, string Replacement)? CreateDecimalSeparatorPattern(string original, string corrected)
        {
            // Pattern: 123,45 -> 123.45
            if (original.Contains(",") && corrected.Contains(".") &&
                original.Replace(",", ".") == corrected)
            {
                // Check if it's a valid decimal format
                var commaIndex = original.LastIndexOf(',');
                if (commaIndex > 0 && commaIndex < original.Length - 1)
                {
                    var afterComma = original.Substring(commaIndex + 1);
                    if (Regex.IsMatch(afterComma, @"^\d{1,4}$")) // 1-4 digits after comma
                    {
                        return (@"(\d+),(\d{1,4})", "$1.$2");
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates pattern for adding currency symbols
        /// </summary>
        private (string Pattern, string Replacement)? CreateCurrencySymbolPattern(string original, string corrected)
        {
            // Pattern: 99.99 -> $99.99
            if (!original.StartsWith("$") && corrected.StartsWith("$") &&
                corrected.Substring(1) == original)
            {
                if (Regex.IsMatch(original, @"^\d+\.?\d*$"))
                {
                    return (@"^(\d+\.?\d*)$", "$$1");
                }
            }

            // Pattern: 99.99 -> €99.99 or other currency symbols
            var currencySymbols = new[] { "€", "£", "¥", "₹" };
            foreach (var symbol in currencySymbols)
            {
                if (!original.StartsWith(symbol) && corrected.StartsWith(symbol) &&
                    corrected.Substring(1) == original)
                {
                    if (Regex.IsMatch(original, @"^\d+\.?\d*$"))
                    {
                        return (@"^(\d+\.?\d*)$", $"{Regex.Escape(symbol)}$1");
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates pattern for negative number format correction
        /// </summary>
        private (string Pattern, string Replacement)? CreateNegativeNumberPattern(string original, string corrected)
        {
            // Pattern: 50.00- -> -50.00 (trailing minus to leading)
            if (original.EndsWith("-") && corrected.StartsWith("-") &&
                original.Substring(0, original.Length - 1) == corrected.Substring(1))
            {
                if (Regex.IsMatch(original.Substring(0, original.Length - 1), @"^\d+\.?\d*$"))
                {
                    return (@"(\d+\.?\d*)-$", "-$1");
                }
            }

            // Pattern: (50.00) -> -50.00 (parentheses to minus)
            if (original.StartsWith("(") && original.EndsWith(")") && corrected.StartsWith("-"))
            {
                var innerValue = original.Substring(1, original.Length - 2);
                if (corrected.Substring(1) == innerValue && Regex.IsMatch(innerValue, @"^\d+\.?\d*$"))
                {
                    return (@"\((\d+\.?\d*)\)", "-$1");
                }
            }

            return null;
        }

        /// <summary>
        /// Creates pattern for OCR character confusion corrections
        /// </summary>
        private (string Pattern, string Replacement)? CreateOCRCharacterConfusionPattern(string original, string corrected)
        {
            // Common OCR character confusions
            var confusions = new Dictionary<char, char[]>
            {
                ['0'] = new[] { 'O', 'o', 'Q' },
                ['1'] = new[] { 'l', 'I', '|' },
                ['5'] = new[] { 'S', 's' },
                ['6'] = new[] { 'G', 'b' },
                ['8'] = new[] { 'B' },
                ['9'] = new[] { 'g', 'q' }
            };

            var pattern = Regex.Escape(original);
            var hasConfusion = false;

            foreach (var kvp in confusions)
            {
                var correctChar = kvp.Key;
                var confusedChars = kvp.Value;

                foreach (var confusedChar in confusedChars)
                {
                    if (original.Contains(confusedChar) && corrected.Contains(correctChar))
                    {
                        // Replace the confused character with a character class
                        var charClass = $"[{confusedChar}{correctChar}]";
                        pattern = pattern.Replace(Regex.Escape(confusedChar.ToString()), charClass);
                        hasConfusion = true;
                    }
                }
            }

            if (hasConfusion)
            {
                return (pattern, corrected);
            }

            return null;
        }

        /// <summary>
        /// Creates pattern for removing unwanted spaces in numbers
        /// </summary>
        private (string Pattern, string Replacement)? CreateSpaceRemovalPattern(string original, string corrected)
        {
            // Pattern: "1 2 3.45" -> "123.45"
            if (original.Contains(" ") && !corrected.Contains(" ") &&
                original.Replace(" ", "") == corrected)
            {
                if (Regex.IsMatch(corrected, @"^\d+\.?\d*$"))
                {
                    // Create pattern that matches numbers with spaces
                    var pattern = Regex.Escape(original).Replace(@"\ ", @"\s*");
                    return (pattern, corrected);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a general format pattern for other cases
        /// </summary>
        private (string Pattern, string Replacement)? CreateGeneralFormatPattern(string original, string corrected)
        {
            // Check if the difference is only in non-alphanumeric characters
            var originalAlphaNum = Regex.Replace(original, @"[^\w]", "");
            var correctedAlphaNum = Regex.Replace(corrected, @"[^\w]", "");

            if (string.Equals(originalAlphaNum, correctedAlphaNum, StringComparison.OrdinalIgnoreCase))
            {
                // Create a flexible pattern that captures the alphanumeric content
                var pattern = @"[^\w]*" + string.Join(@"[^\w]*", originalAlphaNum.ToCharArray().Select(c => Regex.Escape(c.ToString()))) + @"[^\w]*";
                return (pattern, corrected);
            }

            return null;
        }

        #endregion

        #region Advanced Pattern Utilities

        /// <summary>
        /// Creates enhanced regex patterns that can handle variations
        /// </summary>
        /// <param name="basePattern">Base regex pattern</param>
        /// <param name="variations">Known variations to include</param>
        /// <returns>Enhanced pattern</returns>
        public string CreateEnhancedPattern(string basePattern, IEnumerable<string> variations)
        {
            if (string.IsNullOrEmpty(basePattern))
                return basePattern;

            var variationsList = variations?.Where(v => !string.IsNullOrEmpty(v)).ToList() ?? new List<string>();

            if (!variationsList.Any())
                return basePattern;

            // Combine base pattern with variations using OR operator
            var allPatterns = new List<string> { basePattern };
            allPatterns.AddRange(variationsList.Select(Regex.Escape));

            return $"({string.Join("|", allPatterns.Distinct())})";
        }

        /// <summary>
        /// Validates that a regex pattern is safe and functional
        /// </summary>
        /// <param name="pattern">Regex pattern to validate</param>
        /// <returns>True if pattern is valid</returns>
        public bool ValidateRegexPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return false;

            try
            {
                // Test compilation
                var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                // Test with sample text
                var testText = "Sample text 123.45 $100.00 Invoice: INV-001";
                regex.IsMatch(testText);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Warning(ex, "Invalid regex pattern: {Pattern}", pattern);
                return false;
            }
        }

        /// <summary>
        /// Optimizes regex patterns for better performance
        /// </summary>
        /// <param name="pattern">Original pattern</param>
        /// <returns>Optimized pattern</returns>
        public string OptimizeRegexPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return pattern;

            // Remove unnecessary capturing groups
            pattern = Regex.Replace(pattern, @"\(\?\:", "(");

            // Optimize character classes
            pattern = pattern.Replace("[0-9]", @"\d");
            pattern = pattern.Replace("[a-zA-Z]", @"[a-zA-Z]"); // Keep as is for clarity

            // Add word boundaries where appropriate
            if (Regex.IsMatch(pattern, @"^\w"))
            {
                pattern = @"\b" + pattern;
            }

            if (Regex.IsMatch(pattern, @"\w$"))
            {
                pattern = pattern + @"\b";
            }

            return pattern;
        }

        /// <summary>
        /// Creates field-specific extraction patterns
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="sampleValues">Sample values for the field</param>
        /// <returns>List of extraction patterns</returns>
        public List<string> CreateFieldExtractionPatterns(string fieldName, IEnumerable<string> sampleValues)
        {
            var patterns = new List<string>();
            var fieldInfo = MapDeepSeekFieldToDatabase(fieldName);

            if (fieldInfo == null)
                return patterns;

            // Create base patterns based on field type
            switch (fieldInfo.DataType.ToLowerInvariant())
            {
                case "decimal":
                    patterns.AddRange(CreateMonetaryPatterns(fieldName));
                    break;
                case "datetime":
                    patterns.AddRange(CreateDatePatterns(fieldName));
                    break;
                case "string":
                    patterns.AddRange(CreateTextPatterns(fieldName));
                    break;
            }

            // Enhance patterns with sample values
            if (sampleValues != null)
            {
                foreach (var sample in sampleValues.Where(s => !string.IsNullOrEmpty(s)))
                {
                    var samplePattern = CreatePatternFromSample(fieldName, sample);
                    if (!string.IsNullOrEmpty(samplePattern))
                    {
                        patterns.Add(samplePattern);
                    }
                }
            }

            return patterns.Distinct().ToList();
        }

        /// <summary>
        /// Creates monetary field patterns
        /// </summary>
        private List<string> CreateMonetaryPatterns(string fieldName)
        {
            var fieldLower = fieldName.ToLowerInvariant();
            var patterns = new List<string>();

            // Basic monetary patterns
            patterns.Add($@"{Regex.Escape(fieldName)}[:\s]+\$?([0-9,]+\.?[0-9]*)");
            patterns.Add($@"{fieldLower}[:\s]+\$?([0-9,]+\.?[0-9]*)");

            // Alternative field names
            if (fieldLower.Contains("total"))
            {
                patterns.Add(@"total[:\s]+\$?([0-9,]+\.?[0-9]*)");
                patterns.Add(@"amount[:\s]+\$?([0-9,]+\.?[0-9]*)");
            }

            if (fieldLower.Contains("subtotal"))
            {
                patterns.Add(@"subtotal[:\s]+\$?([0-9,]+\.?[0-9]*)");
                patterns.Add(@"sub[:\s]+\$?([0-9,]+\.?[0-9]*)");
            }

            return patterns;
        }

        /// <summary>
        /// Creates date field patterns
        /// </summary>
        private List<string> CreateDatePatterns(string fieldName)
        {
            var patterns = new List<string>
            {
                $@"{Regex.Escape(fieldName)}[:\s]+(\d{{1,2}}[\/\-]\d{{1,2}}[\/\-]\d{{2,4}})",
                $@"date[:\s]+(\d{{1,2}}[\/\-]\d{{1,2}}[\/\-]\d{{2,4}})",
                $@"invoice\s+date[:\s]+(\d{{1,2}}[\/\-]\d{{1,2}}[\/\-]\d{{2,4}})"
            };

            return patterns;
        }

        /// <summary>
        /// Creates text field patterns
        /// </summary>
        private List<string> CreateTextPatterns(string fieldName)
        {
            var patterns = new List<string>
            {
                $@"{Regex.Escape(fieldName)}[:\s]+([^\r\n]+)",
                $@"{fieldName.ToLowerInvariant()}[:\s]+([^\r\n]+)"
            };

            return patterns;
        }

        /// <summary>
        /// Creates a pattern from a sample value
        /// </summary>
        private string CreatePatternFromSample(string fieldName, string sampleValue)
        {
            // Create a pattern that would match this specific sample
            var escapedSample = Regex.Escape(sampleValue);
            return $@"{Regex.Escape(fieldName)}[:\s]+({escapedSample})";
        }

        #endregion
    }
}
