using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Advanced pattern creation logic for OCR corrections
    /// Enhanced with omission handling and DeepSeek integration
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
        /// Creates extraction patterns for omissions based on DeepSeek analysis and context
        /// </summary>
        /// <param name="correction">Correction result with context</param>
        /// <param name="existingRegex">Current line regex pattern if available</param>
        /// <param name="existingNamedGroups">Existing named groups to preserve</param>
        /// <returns>Regex creation response for omission handling</returns>
        public RegexCreationResponse CreateOmissionExtractionPattern(CorrectionResult correction, 
            string existingRegex = null, List<string> existingNamedGroups = null)
        {
            try
            {
                _logger?.Information("Creating omission extraction pattern for field {FieldName}", correction.FieldName);

                // Analyze the context to determine pattern strategy
                var patternStrategy = AnalyzeOmissionContext(correction);
                
                // Create the regex pattern based on analysis
                var regexPattern = CreateOmissionRegexPattern(correction, patternStrategy);
                
                if (string.IsNullOrEmpty(regexPattern))
                {
                    _logger?.Warning("Could not create omission regex pattern for field {FieldName}", correction.FieldName);
                    return null;
                }

                // Determine if we should modify existing line or create new one
                var strategy = DetermineOmissionStrategy(correction, existingRegex, existingNamedGroups);

                var response = new RegexCreationResponse
                {
                    Strategy = strategy,
                    RegexPattern = regexPattern,
                    CompleteLineRegex = strategy == "modify_existing_line" ? 
                        CombineWithExistingRegex(existingRegex, regexPattern) : regexPattern,
                    IsMultiline = correction.RequiresMultilineRegex,
                    MaxLines = DetermineMaxLines(correction),
                    Confidence = CalculatePatternConfidence(correction, regexPattern),
                    Reasoning = $"Created {strategy} pattern for {correction.FieldName} based on context analysis",
                    TestMatch = ExtractTestMatch(correction)
                };

                _logger?.Information("Created omission pattern for {FieldName}: Strategy={Strategy}, Pattern={Pattern}",
                    correction.FieldName, response.Strategy, response.RegexPattern);

                return response;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating omission extraction pattern for field {FieldName}", correction.FieldName);
                return null;
            }
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

        #region Omission Pattern Creation

        /// <summary>
        /// Analyzes omission context to determine the best pattern creation strategy
        /// </summary>
        private OmissionPatternStrategy AnalyzeOmissionContext(CorrectionResult correction)
        {
            var strategy = new OmissionPatternStrategy();

            // Analyze the target value to determine pattern type
            if (IsMonetaryValue(correction.NewValue))
            {
                strategy.PatternType = "monetary";
                strategy.BasePattern = @"-?\$?([0-9,]+\.?[0-9]*)";
            }
            else if (IsDateValue(correction.NewValue))
            {
                strategy.PatternType = "date";
                strategy.BasePattern = @"(\d{1,2}[\/\-]\d{1,2}[\/\-]\d{2,4})";
            }
            else if (IsNumericValue(correction.NewValue))
            {
                strategy.PatternType = "numeric";
                strategy.BasePattern = @"([0-9]+\.?[0-9]*)";
            }
            else
            {
                strategy.PatternType = "text";
                strategy.BasePattern = @"([^\r\n\t]+)";
            }

            // Analyze context for multiline requirements
            var fullContext = correction.FullContext;
            if (correction.RequiresMultilineRegex || ContainsMultilinePattern(fullContext))
            {
                strategy.RequiresMultiline = true;
                strategy.MaxLines = DetermineMaxLines(correction);
            }

            _logger?.Debug("Analyzed omission context for {FieldName}: Type={PatternType}, Multiline={RequiresMultiline}",
                correction.FieldName, strategy.PatternType, strategy.RequiresMultiline);

            return strategy;
        }

        /// <summary>
        /// Creates regex pattern for omission based on strategy
        /// </summary>
        private string CreateOmissionRegexPattern(CorrectionResult correction, OmissionPatternStrategy strategy)
        {
            try
            {
                var fieldName = correction.FieldName;
                var targetValue = correction.NewValue;
                var lineText = correction.LineText;

                // Create field-specific pattern based on context
                string pattern;

                if (strategy.RequiresMultiline)
                {
                    pattern = CreateMultilineOmissionPattern(correction, strategy);
                }
                else
                {
                    pattern = CreateSingleLineOmissionPattern(correction, strategy);
                }

                // Wrap in named group
                if (!string.IsNullOrEmpty(pattern))
                {
                    pattern = $"(?<{fieldName}>{pattern})";
                }

                _logger?.Debug("Created omission regex pattern for {FieldName}: {Pattern}", fieldName, pattern);
                return pattern;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating omission regex pattern for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Creates single-line pattern for omission
        /// </summary>
        private string CreateSingleLineOmissionPattern(CorrectionResult correction, OmissionPatternStrategy strategy)
        {
            var fieldName = correction.FieldName;
            var targetValue = correction.NewValue;
            var lineText = correction.LineText;

            // Try to find the target value in the line text and create pattern around it
            var valueIndex = lineText.IndexOf(targetValue, StringComparison.OrdinalIgnoreCase);
            if (valueIndex >= 0)
            {
                // Extract context around the value
                var beforeValue = lineText.Substring(0, valueIndex).Trim();
                var afterValue = lineText.Substring(valueIndex + targetValue.Length).Trim();

                // Create pattern with context
                var beforePattern = CreateContextPattern(beforeValue, 20); // Last 20 chars
                var afterPattern = CreateContextPattern(afterValue, 10);   // First 10 chars

                return $"{beforePattern}\\s*{strategy.BasePattern}\\s*{afterPattern}";
            }
            else
            {
                // Fallback: create generic pattern for field type
                return CreateGenericFieldPattern(fieldName, strategy.PatternType);
            }
        }

        /// <summary>
        /// Creates multi-line pattern for omission
        /// </summary>
        private string CreateMultilineOmissionPattern(CorrectionResult correction, OmissionPatternStrategy strategy)
        {
            var fieldName = correction.FieldName;
            var contextLines = new List<string>();
            
            contextLines.AddRange(correction.ContextLinesBefore);
            contextLines.Add(correction.LineText);
            contextLines.AddRange(correction.ContextLinesAfter);

            // Find the line containing the target value
            var targetLineIndex = -1;
            for (int i = 0; i < contextLines.Count; i++)
            {
                if (contextLines[i].Contains(correction.NewValue))
                {
                    targetLineIndex = i;
                    break;
                }
            }

            if (targetLineIndex >= 0)
            {
                // Create pattern that spans the necessary lines
                var linesBefore = Math.Min(2, targetLineIndex);
                var linesAfter = Math.Min(2, contextLines.Count - targetLineIndex - 1);

                var patternLines = new List<string>();
                
                for (int i = targetLineIndex - linesBefore; i <= targetLineIndex + linesAfter; i++)
                {
                    if (i >= 0 && i < contextLines.Count)
                    {
                        if (i == targetLineIndex)
                        {
                            // This is the line with our value
                            var line = contextLines[i];
                            var valueIndex = line.IndexOf(correction.NewValue);
                            if (valueIndex >= 0)
                            {
                                var beforeValue = line.Substring(0, valueIndex);
                                var afterValue = line.Substring(valueIndex + correction.NewValue.Length);
                                patternLines.Add($"{Regex.Escape(beforeValue)}{strategy.BasePattern}{Regex.Escape(afterValue)}");
                            }
                            else
                            {
                                patternLines.Add($".*{strategy.BasePattern}.*");
                            }
                        }
                        else
                        {
                            // Context line - make it flexible
                            patternLines.Add(".*");
                        }
                    }
                }

                return string.Join("\\s*\\n\\s*", patternLines);
            }

            // Fallback for multiline
            return $"(?:.*\\n){{0,{strategy.MaxLines}}}{strategy.BasePattern}(?:.*\\n){{0,{strategy.MaxLines}}}";
        }

        /// <summary>
        /// Creates context pattern from text
        /// </summary>
        private string CreateContextPattern(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Take last/first characters and escape them
            var contextText = text.Length > maxLength ? text.Substring(text.Length - maxLength) : text;
            return Regex.Escape(contextText.Trim());
        }

        /// <summary>
        /// Creates generic field pattern based on field type
        /// </summary>
        private string CreateGenericFieldPattern(string fieldName, string patternType)
        {
            var fieldLower = fieldName.ToLowerInvariant();

            return patternType switch
            {
                "monetary" => $@"{Regex.Escape(fieldName)}[:\s]*\$?([0-9,]+\.?[0-9]*)",
                "numeric" => $@"{Regex.Escape(fieldName)}[:\s]*([0-9]+\.?[0-9]*)",
                "date" => $@"{Regex.Escape(fieldName)}[:\s]*(\d{{1,2}}[\/\-]\d{{1,2}}[\/\-]\d{{2,4}})",
                "text" => $@"{Regex.Escape(fieldName)}[:\s]*([^\r\n\t]+)",
                _ => $@"{Regex.Escape(fieldName)}[:\s]*([^\r\n\t]+)"
            };
        }

        #endregion

        #region Pattern Utilities

        /// <summary>
        /// Determines if we should modify existing line or create new one
        /// </summary>
        private string DetermineOmissionStrategy(CorrectionResult correction, string existingRegex, List<string> existingNamedGroups)
        {
            // If no existing regex, must create new line
            if (string.IsNullOrEmpty(existingRegex))
                return "create_new_line";

            // If field already exists in named groups, this shouldn't be an omission
            if (existingNamedGroups?.Contains(correction.FieldName) == true)
            {
                _logger?.Warning("Field {FieldName} already exists in named groups but marked as omission", correction.FieldName);
                return "create_new_line";
            }

            // If the existing regex is very complex or long, safer to create new line
            if (existingRegex.Length > 500 || CountRegexGroups(existingRegex) > 10)
            {
                _logger?.Debug("Existing regex too complex for modification, creating new line for {FieldName}", correction.FieldName);
                return "create_new_line";
            }

            // If the correction is for a different type of content, create new line
            if (IsDifferentContentType(correction, existingRegex))
            {
                return "create_new_line";
            }

            // Otherwise, try to modify existing line
            return "modify_existing_line";
        }

        /// <summary>
        /// Combines new pattern with existing regex
        /// </summary>
        private string CombineWithExistingRegex(string existingRegex, string newPattern)
        {
            if (string.IsNullOrEmpty(existingRegex))
                return newPattern;

            // Simple combination - add new pattern to end
            return $"{existingRegex}.*{newPattern}";
        }

        /// <summary>
        /// Calculates confidence for pattern based on context analysis
        /// </summary>
        private double CalculatePatternConfidence(CorrectionResult correction, string pattern)
        {
            var confidence = 0.7; // Base confidence

            // Increase confidence if we found exact value in context
            if (!string.IsNullOrEmpty(correction.LineText) && 
                correction.LineText.Contains(correction.NewValue))
            {
                confidence += 0.2;
            }

            // Increase confidence for specific field types
            if (IsMonetaryValue(correction.NewValue) || IsNumericValue(correction.NewValue))
            {
                confidence += 0.1;
            }

            // Decrease confidence for very complex patterns
            if (pattern.Length > 200)
            {
                confidence -= 0.1;
            }

            return Math.Max(0.5, Math.Min(0.95, confidence));
        }

        /// <summary>
        /// Extracts test match from correction context
        /// </summary>
        private string ExtractTestMatch(CorrectionResult correction)
        {
            // Return the line containing the target value
            if (!string.IsNullOrEmpty(correction.LineText) && 
                correction.LineText.Contains(correction.NewValue))
            {
                return correction.LineText;
            }

            // Search in context lines
            foreach (var line in correction.ContextLinesBefore.Concat(correction.ContextLinesAfter))
            {
                if (line.Contains(correction.NewValue))
                {
                    return line;
                }
            }

            return correction.LineText ?? "";
        }

        /// <summary>
        /// Determines maximum lines for multiline pattern
        /// </summary>
        private int DetermineMaxLines(CorrectionResult correction)
        {
            if (!correction.RequiresMultilineRegex)
                return 1;

            // Base on available context
            var totalContextLines = correction.ContextLinesBefore.Count + correction.ContextLinesAfter.Count + 1;
            return Math.Min(10, Math.Max(3, totalContextLines / 2));
        }

        /// <summary>
        /// Checks if context contains multiline pattern indicators
        /// </summary>
        private bool ContainsMultilinePattern(string context)
        {
            if (string.IsNullOrEmpty(context))
                return false;

            // Look for multiline indicators
            var multilineIndicators = new[] { "Address", "Description", "Notes", "Comments" };
            return multilineIndicators.Any(indicator => context.Contains(indicator, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if value appears to be monetary
        /// </summary>
        private bool IsMonetaryValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return Regex.IsMatch(value, @"^\$?-?[0-9,]+\.?[0-9]*$") ||
                   value.Contains("$") || value.Contains("€") || value.Contains("£");
        }

        /// <summary>
        /// Checks if value appears to be a date
        /// </summary>
        private bool IsDateValue(string value)
        {
            return !string.IsNullOrEmpty(value) && 
                   Regex.IsMatch(value, @"\d{1,2}[\/\-]\d{1,2}[\/\-]\d{2,4}");
        }

        /// <summary>
        /// Checks if value appears to be numeric
        /// </summary>
        private bool IsNumericValue(string value)
        {
            return !string.IsNullOrEmpty(value) && 
                   Regex.IsMatch(value, @"^-?[0-9]+\.?[0-9]*$");
        }

        /// <summary>
        /// Counts regex groups in pattern
        /// </summary>
        private int CountRegexGroups(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return 0;

            return Regex.Matches(pattern, @"\([^)]*\)").Count;
        }

        /// <summary>
        /// Checks if correction is for different content type than existing regex
        /// </summary>
        private bool IsDifferentContentType(CorrectionResult correction, string existingRegex)
        {
            // Simple heuristic - if existing regex has monetary patterns and correction is not monetary
            var existingIsMonetary = existingRegex.Contains(@"\$") || existingRegex.Contains("price") || existingRegex.Contains("total");
            var correctionIsMonetary = IsMonetaryValue(correction.NewValue);

            return existingIsMonetary != correctionIsMonetary;
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Strategy for creating omission patterns
        /// </summary>
        private class OmissionPatternStrategy
        {
            public string PatternType { get; set; } = "text";
            public string BasePattern { get; set; } = @"([^\r\n\t]+)";
            public bool RequiresMultiline { get; set; } = false;
            public int MaxLines { get; set; } = 1;
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