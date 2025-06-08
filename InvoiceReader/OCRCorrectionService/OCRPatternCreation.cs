// File: OCRCorrectionService/OCRPatternCreation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog; // ILogger is available as this._logger

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Format Correction Pattern Creation

        public (string Pattern, string Replacement)? CreateAdvancedFormatCorrectionPatterns(string originalValue, string correctedValue)
        {
            if (string.IsNullOrWhiteSpace(originalValue) && string.IsNullOrWhiteSpace(correctedValue))
            {
                _logger?.Debug("CreateAdvancedFormatCorrectionPatterns: Both original and corrected values are empty/null. No pattern generated.");
                return null;
            }
            if (originalValue == correctedValue)
            {
                _logger?.Debug("CreateAdvancedFormatCorrectionPatterns: Original and corrected values are identical ('{Value}'). No pattern needed.", originalValue);
                return null;
            }

            _logger?.Debug("Attempting to create format correction pattern for: '{Original}' -> '{Corrected}'", originalValue, correctedValue);

            var strategies = new Func<string, string, (string, string)?>[]
            {
                CreateDecimalSeparatorPattern,
                CreateCurrencySymbolPattern,
                CreateNegativeNumberPattern,
                CreateSpecificOCRCharacterConfusionPattern,
                CreateSpaceManipulationPattern,
                CreateGeneralSymbolOrCasePattern,
                CreateLooseAlphanumericMatchPattern
            };

            foreach (var strategy in strategies)
            {
                var result = strategy(originalValue, correctedValue);
                if (result.HasValue && !string.IsNullOrEmpty(result.Value.Item1)) // Use Item1 for pattern
                {
                    _logger?.Information("Successfully created format correction pattern via strategy '{StrategyName}': Pattern='{Pattern}', Replacement='{Replacement}' for '{Original}' -> '{Corrected}'",
                        strategy.Method.Name, result.Value.Item1, result.Value.Item2, originalValue, correctedValue); // Use Item1 and Item2
                    return result;
                }
            }

            _logger?.Warning("No suitable advanced format correction pattern could be automatically generated for: '{Original}' -> '{Corrected}'", originalValue, correctedValue);
            return null;
        }

        // --- Private Helper Strategies for CreateAdvancedFormatCorrectionPatterns ---

        private (string Pattern, string Replacement)? CreateDecimalSeparatorPattern(string original, string corrected)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(corrected)) return null;
            // Handles "123,45" -> "123.45" (common European to US)
            if (original.Contains(",") && corrected.Contains(".") && original.Replace(",", ".") == corrected)
            {
                // Ensure it's likely a number format, e.g., digits around the comma.
                if (Regex.IsMatch(original, @"^\s*-?\d+(,\d{1,4})\s*$"))
                    return (@"(\d+),(\d{1,4})", "$1.$2"); // Captures digits before and after comma
            }
            // Handles "123.45" -> "123,45" (common US to European)
            if (original.Contains(".") && corrected.Contains(",") && original.Replace(".", ",") == corrected)
            {
                if (Regex.IsMatch(original, @"^\s*-?\d+(\.\d{1,4})\s*$"))
                    return (@"(\d+)\.(\d{1,4})", "$1,$2");
            }
            return null;
        }

        private (string Pattern, string Replacement)? CreateCurrencySymbolPattern(string original, string corrected)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(corrected)) return null;
            string[] currencySymbols = { "$", "€", "£", "¥", "₹" }; // Extend as needed
            foreach (var symbol in currencySymbols)
            {
                string escSymbolForPattern = Regex.Escape(symbol);

                // Adding symbol: "99.99" -> "$99.99"
                if (!original.Contains(symbol) && corrected.StartsWith(symbol) && corrected.Substring(symbol.Length).Trim() == original.Trim())
                {
                    if (Regex.IsMatch(original.Trim(), @"^-?\d+(\.\d+)?$")) // Ensure original is a number
                    {
                        // In replacement strings, a literal '$' is '$$'. For other symbols, they are literal.
                        string replacementSymbol = symbol == "$" ? "$$" : symbol;
                        return (@"^(-?\d+(\.\d+)?)$", replacementSymbol + "$1"); // Correctly produces "$$$1" for dollar sign.
                    }
                }
                // Removing symbol: "$99.99" -> "99.99"
                if (original.StartsWith(symbol) && !corrected.Contains(symbol) && original.Substring(symbol.Length).Trim() == corrected.Trim())
                {
                    if (Regex.IsMatch(corrected.Trim(), @"^-?\d+(\.\d+)?$"))
                        return (escSymbolForPattern + @"\s*(-?\d+(\.\d+)?)", "$1");
                }
            }
            return null;
        }

        private (string Pattern, string Replacement)? CreateNegativeNumberPattern(string original, string corrected)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(corrected)) return null;
            // Trailing minus to leading: "50.00-" -> "-50.00"
            if (original.EndsWith("-") && corrected.StartsWith("-") && original.TrimEnd('-') == corrected.TrimStart('-'))
            {
                if (Regex.IsMatch(original.TrimEnd('-'), @"^\d+(\.\d+)?$"))
                    return (@"(\d+(\.\d+)?)-$", "-$1");
            }
            // Parentheses to leading minus: "(50.00)" -> "-50.00"
            if (original.StartsWith("(") && original.EndsWith(")") && corrected.StartsWith("-"))
            {
                var valInParen = original.Substring(1, original.Length - 2).Trim();
                if (valInParen == corrected.TrimStart('-') && Regex.IsMatch(valInParen, @"^\d+(\.\d+)?$"))
                    return (@"\(\s*(\d+(\.\d+)?)\s*\)", "-$1"); // Allow spaces inside parens
            }
            return null;
        }

        private (string Pattern, string Replacement)? CreateSpecificOCRCharacterConfusionPattern(string original, string corrected)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(corrected) || original.Length != corrected.Length) return null;

            var diffs = new List<(char o, char c, int i)>();
            for (int i = 0; i < original.Length; ++i)
            {
                if (original[i] != corrected[i]) diffs.Add((original[i], corrected[i], i));
            }

            if (diffs.Count == 1)
            {
                var d = diffs[0];
                bool isKnownConfusion = (d.o == 'O' && d.c == '0') || (d.o == '0' && d.c == 'O') ||
                                        (d.o == 'l' && d.c == '1') || (d.o == '1' && d.c == 'l') || // Note: 'I' can also be confused with '1' or 'l'
                                        (d.o == 'I' && d.c == '1') || (d.o == '1' && d.c == 'I') ||
                                        (d.o == 'S' && d.c == '5') || (d.o == '5' && d.c == 'S') ||
                                        (d.o == 'B' && d.c == '8') || (d.o == '8' && d.c == 'B') ||
                                        (d.o == 'Z' && d.c == '2') || (d.o == '2' && d.c == 'Z') ||
                                        (d.o == 'G' && d.c == '6') || (d.o == '6' && d.c == 'G');
                if (isKnownConfusion)
                {
                    string pattern = Regex.Escape(original.Substring(0, d.i)) +
                                     $"[{Regex.Escape(d.o.ToString())}{Regex.Escape(d.c.ToString())}]" + // Create a character class e.g. [O0]
                                     Regex.Escape(original.Substring(d.i + 1));
                    return (pattern, corrected);
                }
            }
            return null;
        }

        private (string Pattern, string Replacement)? CreateSpaceManipulationPattern(string original, string corrected)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(corrected)) return null;
            if (original.Contains(" ") && !corrected.Contains(" ") && original.Replace(" ", "") == corrected)
            {
                if (Regex.IsMatch(corrected, @"^-?[\d.,]+$"))
                    return (Regex.Escape(original).Replace(@"\ ", @"\s*"), corrected); // \s* allows zero or more spaces
            }
            return null;
        }

        private (string Pattern, string Replacement)? CreateGeneralSymbolOrCasePattern(string original, string corrected)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(corrected)) return null;
            if (original.Equals(corrected, StringComparison.OrdinalIgnoreCase) && original != corrected)
            {
                // Only case difference, pattern is exact original, replacement is corrected.
                // RegexOptions.IgnoreCase would make this pattern redundant if used globally,
                // but for FieldFormatRegEx, this specific rule might be useful.
                return (Regex.Escape(original), corrected);
            }
            var originalAlphaNum = Regex.Replace(original, @"[^\w]", "");
            var correctedAlphaNum = Regex.Replace(corrected, @"[^\w]", "");
            if (originalAlphaNum.Length > 0 && originalAlphaNum == correctedAlphaNum) // Alphanumeric parts are the same
            {
                // Only non-alphanumeric characters differ.
                // Example: "INV-123" -> "INV/123" or "Order # 123" -> "Order: 123"
                // This creates a specific rule for this exact transformation.
                return (Regex.Escape(original), corrected);
            }
            return null;
        }

        private (string Pattern, string Replacement)? CreateLooseAlphanumericMatchPattern(string original, string corrected)
        {
            // This strategy is intended as a last resort and can be risky if too loose.
            // It's generally better to rely on more specific strategies or DeepSeek for complex transformations.
            // For now, this will not generate a pattern to avoid overly broad or incorrect rules.
            return null;
        }
        #endregion

        #region Omission Pattern Creation (Local Heuristics - DeepSeek is Primary)

        public RegexCreationResponse CreateLocalOmissionExtractionPattern(CorrectionResult correction, LineContext lineContext)
        {
            if (correction == null || lineContext == null || !(correction.CorrectionType == "omission" || correction.CorrectionType == "omitted_line_item"))
            {
                _logger.Warning("CreateLocalOmissionExtractionPattern: Invalid input or not an omission type. Field: {FieldName}, Type: {CorrectionType}", correction?.FieldName, correction?.CorrectionType);
                return null;
            }

            try
            {
                _logger.Information("Attempting local heuristic pattern creation for omitted field: {FieldName}", correction.FieldName);
                var patternStrategy = AnalyzeLocalOmissionContext(correction, lineContext);
                string newFieldPatternPart = CreateLocalOmissionRegexPatternPart(correction, patternStrategy, lineContext);

                if (string.IsNullOrEmpty(newFieldPatternPart))
                {
                    _logger.Warning("Local heuristic: Could not create regex part for omitted field {FieldName}", correction.FieldName);
                    return null;
                }

                string determinedStrategy = DetermineLocalOmissionStrategy(correction, lineContext);
                string completeLineRegex = $"(?<{correction.FieldName}>{newFieldPatternPart})";

                if (determinedStrategy == "modify_existing_line" && !string.IsNullOrEmpty(lineContext.RegexPattern))
                {
                    string combinedRegex = CombineLocalOmissionWithExistingRegex(lineContext.RegexPattern, newFieldPatternPart, correction.FieldName);
                    if (combinedRegex != lineContext.RegexPattern)
                    {
                        completeLineRegex = combinedRegex;
                    }
                    else
                    {
                        _logger.Warning("Local heuristic: Failed to meaningfully combine new pattern part with existing regex. Defaulting to new line strategy for {FieldName}.", correction.FieldName);
                        determinedStrategy = "create_new_line";
                        completeLineRegex = $"(?<{correction.FieldName}>{newFieldPatternPart})";
                    }
                }

                var response = new RegexCreationResponse
                {
                    Strategy = determinedStrategy,
                    RegexPattern = $"(?<{correction.FieldName}>{newFieldPatternPart})",
                    CompleteLineRegex = (determinedStrategy == "modify_existing_line") ? completeLineRegex : null, // Only provide if modifying
                    IsMultiline = patternStrategy.RequiresMultiline,
                    MaxLines = patternStrategy.MaxLines,
                    Confidence = CalculateLocalPatternConfidence(correction, newFieldPatternPart),
                    Reasoning = $"Locally generated heuristic pattern for omitted field: {correction.FieldName}",
                    TestMatch = ExtractTestMatchFromContext(correction)
                };

                _logger.Information("Local heuristic: Created omission pattern for {FieldName}. Strategy={Strategy}, FieldPattern='{PatternPart}'",
                    correction.FieldName, response.Strategy, newFieldPatternPart);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in local CreateOmissionExtractionPattern for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        private OmissionPatternStrategy AnalyzeLocalOmissionContext(CorrectionResult correction, LineContext lineContext)
        {
            var strategy = new OmissionPatternStrategy();
            var fieldTypeInfo = this.MapDeepSeekFieldToDatabase(correction.FieldName);

            strategy.PatternType = fieldTypeInfo?.DataType?.ToLowerInvariant() ?? "string";

            switch (strategy.PatternType)
            {
                case "decimal":
                case "double":
                case "currency":
                case "int":
                case "integer":
                    strategy.BasePattern = IsPotentiallySignedCurrencyOrNumber(correction.NewValue)
                                           ? @"-?\$?€?£?\s*(?:[0-9]{1,3}(?:[,.]\d{3})*|[0-9]+)(?:[.,]\d{1,4})?"
                                           : Regex.Escape(correction.NewValue ?? "");
                    break;
                case "datetime":
                    strategy.BasePattern = @"(\d{1,2}[/\-.]\d{1,2}[/\-.]\d{2,4}|\d{4}[/\-.]\d{1,2}[/\-.]\d{1,2}|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\.?\s+\d{1,2}(?:st|nd|rd|th)?(?:,)?\s+\d{2,4})";
                    break;
                default:
                    strategy.BasePattern = Regex.Escape(correction.NewValue ?? "");
                    break;
            }

            strategy.RequiresMultiline = correction.RequiresMultilineRegex ||
                                       (correction.LineText != null && correction.LineText.Length > 80 && (correction.NewValue?.Length ?? 0) < correction.LineText.Length / 2 && !(correction.NewValue?.Contains("\n") ?? false)) ||
                                       (correction.NewValue?.Contains("\n") ?? false);
            strategy.MaxLines = strategy.RequiresMultiline ? Math.Min(3, 1 + (lineContext?.ContextLinesBefore.Count ?? 0) + (lineContext?.ContextLinesAfter.Count ?? 0)) : 1;
            if (strategy.MaxLines <= 0) strategy.MaxLines = 1;

            return strategy;
        }

        private bool IsPotentiallySignedCurrencyOrNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            string temp = value.Trim().Replace(" ", "");
            temp = Regex.Replace(temp, @"[\$€£]", "");
            if (temp.StartsWith("(") && temp.EndsWith(")")) temp = "-" + temp.Substring(1, temp.Length - 2);
            if (temp.EndsWith("-")) temp = "-" + temp.TrimEnd('-');
            return Regex.IsMatch(temp, @"^-?\d+([.,]\d+)?$");
        }


        private string CreateLocalOmissionRegexPatternPart(CorrectionResult correction, OmissionPatternStrategy analysis, LineContext lineContext)
        {
            string valuePattern = analysis.BasePattern;
            string fieldKeyForPattern = this.MapDeepSeekFieldToDatabase(correction.FieldName)?.DisplayName ?? correction.FieldName;

            string bestContextText = correction.LineText ?? "";
            if (lineContext != null)
            {
                string effectiveLineText = lineContext.LineText ?? correction.LineText ?? ""; // Prefer lineContext's lineText if available
                if (!string.IsNullOrEmpty(effectiveLineText) && effectiveLineText.Contains(correction.NewValue ?? ""))
                {
                    bestContextText = effectiveLineText;
                }
                else if (!bestContextText.Contains(correction.NewValue ?? "") && (lineContext.ContextLinesAfter?.Any() ?? false) && lineContext.ContextLinesAfter[0].Contains(correction.NewValue ?? ""))
                    bestContextText = lineContext.ContextLinesAfter[0];
                else if (!bestContextText.Contains(correction.NewValue ?? "") && (lineContext.ContextLinesBefore?.Any() ?? false) && lineContext.ContextLinesBefore.Last().Contains(correction.NewValue ?? ""))
                    bestContextText = lineContext.ContextLinesBefore.Last();
            }


            if (!string.IsNullOrEmpty(bestContextText) && !string.IsNullOrEmpty(correction.NewValue) && bestContextText.Contains(correction.NewValue))
            {
                int valIndex = bestContextText.IndexOf(correction.NewValue);
                string prefix = bestContextText.Substring(0, valIndex);
                string suffix = bestContextText.Substring(valIndex + correction.NewValue.Length);

                string prefixPattern = CreateFlexibleContextRegex(prefix, true);
                string suffixPattern = CreateFlexibleContextRegex(suffix, false);

                if (!string.IsNullOrEmpty(prefixPattern) || !string.IsNullOrEmpty(suffixPattern))
                {
                    string finalPattern = "";
                    if (!string.IsNullOrEmpty(prefixPattern))
                    {
                        if (!string.IsNullOrEmpty(fieldKeyForPattern) &&
                            (Regex.Escape(fieldKeyForPattern).Equals(prefixPattern, StringComparison.OrdinalIgnoreCase) ||
                             prefixPattern.StartsWith("(?i)" + Regex.Escape(fieldKeyForPattern), StringComparison.OrdinalIgnoreCase)))
                        {
                            finalPattern += $"(?:{prefixPattern}\\s*[:\\-=\\s]\\s*)?";
                        }
                        else
                        {
                            finalPattern += prefixPattern;
                        }
                    }
                    finalPattern += valuePattern; // This is already the pattern for the value, not the value itself
                    if (!string.IsNullOrEmpty(suffixPattern)) finalPattern += suffixPattern;

                    return finalPattern;
                }
            }

            if (!string.IsNullOrEmpty(fieldKeyForPattern) && (fieldKeyForPattern.Length < 4 && !Regex.IsMatch(fieldKeyForPattern, @"\d")))
                return valuePattern;

            return $@"(?i)(?:{Regex.Escape(fieldKeyForPattern ?? "")}\s*[:\-=\s]\s*)?{valuePattern}";
        }

        private string CreateFlexibleContextRegex(string contextStr, bool isPrefix)
        {
            if (string.IsNullOrWhiteSpace(contextStr)) return "";
            contextStr = contextStr.Trim();
            if (contextStr.Length == 0) return "";

            int maxLength = 20;
            string relevantContext = isPrefix ?
                (contextStr.Length > maxLength ? contextStr.Substring(contextStr.Length - maxLength) : contextStr) :
                (contextStr.Length > maxLength ? contextStr.Substring(0, maxLength) : contextStr);

            string pattern = Regex.Escape(relevantContext);
            pattern = pattern.Replace(@"\ ", @"\s+");

            if (relevantContext.Length > 1)
            { // Avoid applying boundaries to single char context
                if (isPrefix && char.IsLetterOrDigit(relevantContext.Last())) pattern = pattern + @"\b\s*"; // Word boundary after prefix
                else if (!isPrefix && char.IsLetterOrDigit(relevantContext.First())) pattern = @"\s*\b" + pattern; // Word boundary before suffix
            }
            else if (char.IsLetterOrDigit(relevantContext.FirstOrDefault()))
            { // Single char context
                pattern = @"\b" + pattern + @"\b"; // Boundary on both sides if it's a word char
            }

            if (Regex.IsMatch(relevantContext, @"^[^\w\s]+$"))
            {
                return isPrefix ? pattern + @"\s*" : @"\s*" + pattern;
            }

            return pattern;
        }


        private string DetermineLocalOmissionStrategy(CorrectionResult correction, LineContext lineContext)
        {
            if (string.IsNullOrEmpty(lineContext?.RegexPattern) || !lineContext.LineId.HasValue) return "create_new_line";

            string effectiveLineText = lineContext.LineText ?? correction.LineText ?? "";
            if (!string.IsNullOrEmpty(effectiveLineText) && !string.IsNullOrEmpty(correction.NewValue) && effectiveLineText.Contains(correction.NewValue))
            {
                if ((lineContext.FieldsInLine?.Count ?? 0) < 4 && (lineContext.RegexPattern?.Length ?? 0) < 200)
                    return "modify_existing_line";
            }
            return "create_new_line";
        }

        private string CombineLocalOmissionWithExistingRegex(string existingRegex, string newFieldPatternPart, string newFieldName)
        {
            if (string.IsNullOrEmpty(existingRegex)) return $"(?<{newFieldName}>{newFieldPatternPart})";
            if (Regex.IsMatch(existingRegex, $@"\(\?<{Regex.Escape(newFieldName)}>")) return existingRegex;

            return $"{existingRegex}(?:\\s*(?<{newFieldName}>{newFieldPatternPart}))?";
        }

        private double CalculateLocalPatternConfidence(CorrectionResult correction, string patternPart)
        {
            double confidence = 0.5;
            if (!string.IsNullOrEmpty(correction.LineText) && !string.IsNullOrEmpty(correction.NewValue) && correction.LineText.Contains(correction.NewValue)) confidence += 0.15;

            string escapedNewValue = Regex.Escape(correction.NewValue ?? "");
            if (!string.IsNullOrEmpty(patternPart) && !patternPart.Equals(escapedNewValue, StringComparison.OrdinalIgnoreCase) && patternPart.Length > escapedNewValue.Length + 3) confidence += 0.1;

            string keyForPattern = this.MapDeepSeekFieldToDatabase(correction.FieldName)?.DisplayName ?? correction.FieldName;
            if (!string.IsNullOrEmpty(patternPart) && !string.IsNullOrEmpty(keyForPattern) && patternPart.IndexOf(Regex.Escape(keyForPattern), StringComparison.OrdinalIgnoreCase) >= 0) confidence += 0.1;
            return Math.Min(0.85, confidence);
        }

        private string ExtractTestMatchFromContext(CorrectionResult correction)
        {
            string lineWithMatch = null;
            if (!string.IsNullOrEmpty(correction.LineText) && !string.IsNullOrEmpty(correction.NewValue) && correction.LineText.Contains(correction.NewValue))
            {
                lineWithMatch = correction.LineText;
            }
            else
            {
                var searchLines = (correction.ContextLinesBefore ?? Enumerable.Empty<string>())
                                .Concat(new[] { correction.LineText ?? "" })
                                .Concat(correction.ContextLinesAfter ?? Enumerable.Empty<string>());
                lineWithMatch = searchLines.FirstOrDefault(l => !string.IsNullOrEmpty(l) && !string.IsNullOrEmpty(correction.NewValue) && l.Contains(correction.NewValue));
            }

            // If a line containing the new value is found, return it as the test match.
            // Otherwise, fallback to the new value itself, assuming the pattern should at least match that.
            return lineWithMatch ?? correction.NewValue ?? "";
        }

        private int DetermineMaxLines(CorrectionResult correction)
        {
            if (!correction.RequiresMultilineRegex) return 1;
            return Math.Max(1, Math.Min(5, 1 + (correction.ContextLinesBefore?.Count ?? 0) + (correction.ContextLinesAfter?.Count ?? 0)));
        }
        #endregion

        #region Shared Pattern Utilities & Validation

        public bool ValidateRegexPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                _logger?.Verbose("ValidateRegexPattern (syntax): Pattern is null or whitespace.");
                return false;
            }
            try
            {
                _ = new Regex(pattern);
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger?.Warning(ex, "ValidateRegexPattern (syntax): Invalid regex pattern syntax: {Pattern}", pattern);
                return false;
            }
        }

        public bool ValidateRegexPattern(RegexCreationResponse regexResponse, CorrectionResult correction)
        {
            if (regexResponse == null)
            {
                _logger.Warning("ValidateRegexPattern: RegexCreationResponse is null for field {FieldName}.", correction?.FieldName);
                return false;
            }
            if (string.IsNullOrEmpty(regexResponse.RegexPattern) && string.IsNullOrEmpty(regexResponse.CompleteLineRegex))
            {
                _logger.Warning("ValidateRegexPattern: Both RegexPattern and CompleteLineRegex are empty for field {FieldName}.", correction?.FieldName);
                return false;
            }

            string patternToValidate = regexResponse.Strategy == "modify_existing_line" && !string.IsNullOrEmpty(regexResponse.CompleteLineRegex)
                                   ? regexResponse.CompleteLineRegex
                                   : regexResponse.RegexPattern;
            if (string.IsNullOrEmpty(patternToValidate))
            {
                patternToValidate = regexResponse.RegexPattern;
                if (string.IsNullOrEmpty(patternToValidate))
                {
                    _logger.Warning("ValidateRegexPattern: No usable pattern string to validate for field {FieldName}.", correction?.FieldName);
                    return false;
                }
            }

            try
            {
                var regexOptions = RegexOptions.IgnoreCase;
                if (regexResponse.IsMultiline) regexOptions |= RegexOptions.Multiline;

                var regex = new Regex(patternToValidate, regexOptions);

                string testText = !string.IsNullOrEmpty(regexResponse.TestMatch)
                                ? regexResponse.TestMatch
                                : correction?.FullContext;
                if (string.IsNullOrEmpty(testText) && !string.IsNullOrEmpty(correction?.NewValue))
                {
                    testText = correction.NewValue;
                    _logger.Verbose("ValidateRegexPattern: Test text for {FieldName} fell back to NewValue itself.", correction?.FieldName);
                }
                if (string.IsNullOrEmpty(testText))
                {
                    _logger.Warning("ValidateRegexPattern: No test text available (TestMatch, FullContext, or NewValue) for field {FieldName}. Cannot validate match.", correction?.FieldName);
                    return false;
                }

                var match = regex.Match(testText);
                if (!match.Success)
                {
                    _logger.Warning("Regex validation: Pattern for {FieldName} failed to match test text. Pattern: '{Pattern}', TestText: '{TestTextSnippet}'",
                        correction?.FieldName, patternToValidate, this.TruncateForLog(testText));
                    return false;
                }

                var fieldGroupName = correction?.FieldName;
                if (string.IsNullOrEmpty(fieldGroupName))
                {
                    _logger.Warning("Regex validation: Correction object or FieldName is null. Cannot validate group name.");
                    return false; // Cannot validate group if fieldname is null
                }

                var capturedGroup = match.Groups[fieldGroupName];
                if (!capturedGroup.Success)
                {
                    _logger.Warning("Regex validation: Named group '(?<{FieldName}>...)' not found or did not capture in pattern for field {FieldName}. Pattern: '{Pattern}'. Matched text: '{MatchedText}'",
                       fieldGroupName, patternToValidate, this.TruncateForLog(match.Value));

                    foreach (Group g in match.Groups)
                    {
                        if (g.Success && !int.TryParse(g.Name, out _) && g.Value == correction.NewValue)
                        {
                            _logger.Information("Regex validation: Field group '{ExpectedGroup}' missed, but group '{ActualGroup}' captured the correct value '{Value}'. Tolerating.", fieldGroupName, g.Name, correction.NewValue);
                            return true;
                        }
                    }
                    return false;
                }
                if (capturedGroup.Value != correction.NewValue)
                {
                    if (capturedGroup.Value.Trim() == (correction.NewValue ?? "").Trim() ||
                        (!string.IsNullOrEmpty(correction.NewValue) && capturedGroup.Value.Contains(correction.NewValue)))
                    {
                        _logger.Information("Regex validation: Captured value '{Captured}' for '{FieldName}' differs slightly from expected '{Expected}' but is accepted (e.g. whitespace/contains).",
                           capturedGroup.Value, fieldGroupName, correction.NewValue);
                    }
                    else
                    {
                        _logger.Warning("Regex validation: Captured value '{Captured}' does not match expected '{Expected}' for field {FieldName}. Pattern: '{Pattern}'",
                            capturedGroup.Value, correction.NewValue, fieldGroupName, patternToValidate);
                        return false;
                    }
                }

                if (regexResponse.Strategy == "modify_existing_line" && !regexResponse.PreservesExistingGroups)
                {
                    _logger.Warning("Regex validation: Strategy is 'modify_existing_line' but DeepSeek response indicates existing groups might not be preserved for field {FieldName}. Pattern: '{Pattern}'. This is a high risk.",
                        correction.FieldName, patternToValidate);
                }

                _logger.Information("Regex validation successful for field {FieldName}. Pattern: '{PatternToValidate}'", correction.FieldName, patternToValidate);
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.Warning(ex, "Regex validation: Invalid regex pattern syntax for field {FieldName}: {Pattern}", correction?.FieldName, patternToValidate);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error during regex validation for field {FieldName}, pattern: {Pattern}", correction?.FieldName, patternToValidate);
                return false;
            }
        }

        public List<string> CreateFieldExtractionPatterns(string fieldName, IEnumerable<string> sampleValues)
        {
            var patterns = new List<string>();
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldName);

            // Default to just the provided field name if no mapping exists
            var keysToUse = new List<string> { fieldName };

            if (fieldInfo != null)
            {
                // If a mapping exists, gather all aliases that point to the same DatabaseFieldInfo
                keysToUse = DeepSeekToDBFieldMapping
                    .Where(kvp => kvp.Value == fieldInfo)
                    .Select(kvp => kvp.Key)
                    .Union(new[] { fieldInfo.DatabaseFieldName, fieldInfo.DisplayName }) // Also include the canonical name and display name
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(s => s.Length) // Process longer aliases first to avoid partial matches
                    .ToList();
                _logger?.Debug("Generating patterns for field {FieldName} using keys: {Keys}", fieldName, string.Join(", ", keysToUse));
            }
            else
            {
                _logger?.Debug("CreateFieldExtractionPatterns: No DB mapping for '{FieldName}', using generic text patterns for the name itself.", fieldName);
            }

            // Create a single regex group of all possible keys, e.g., (Invoice Total|Grand Total|Total)
            var keyGroupPattern = string.Join("|", keysToUse.Select(k => Regex.Escape(k).Replace(" ", "\\s+")));

            // Determine pattern type based on fieldInfo or guess from name
            string dataType = fieldInfo?.DataType?.ToLowerInvariant() ?? "string";

            switch (dataType)
            {
                case "decimal":
                case "double":
                case "currency":
                case "int":
                case "integer":
                    patterns.AddRange(CreateMonetaryOrNumericPatternsFromKeyName(keyGroupPattern, true));
                    break;
                case "datetime":
                    patterns.AddRange(CreateDatePatternsFromKeyName(keyGroupPattern, true));
                    break;
                case "string":
                default:
                    patterns.AddRange(CreateTextPatternsFromKeyName(keyGroupPattern, true));
                    break;
            }

            if (sampleValues != null)
            {
                foreach (var sample in sampleValues.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    patterns.Add($@"(?i)(?:({keyGroupPattern})\s*[:\-=\s]\s*)?({Regex.Escape(sample)})");
                }
            }
            return patterns.Distinct().ToList();
        }

        private List<string> CreateMonetaryOrNumericPatternsFromKeyName(string keyPattern, bool isGroup = false)
        {
            // If it's a group, wrap it in non-capturing group (?:...) so we don't mess up the value capture group index
            string keyRegex = isGroup ? $"(?:{keyPattern})" : $@"\b{Regex.Escape(keyPattern)}\b";
            string valuePattern = @"-?\$?€?£?\s*(?:[0-9]{1,3}(?:[,.]\d{3})*|[0-9]+)(?:[.,]\d{1,4})?";
            return new List<string> {
                $@"(?i)\b{keyRegex}\b\s*[:\-=\s]\s*({valuePattern})",
                $@"(?i)({valuePattern})\s*(?:associated with|for|is)?\s*\b{keyRegex}\b",
                $@"(?i)\b{keyRegex}\b[^\w\s]*\s*({valuePattern})"
            };
        }
        private List<string> CreateDatePatternsFromKeyName(string keyPattern, bool isGroup = false)
        {
            string keyRegex = isGroup ? $"(?:{keyPattern})" : $@"\b{Regex.Escape(keyPattern)}\b";
            string datePattern = @"(\d{1,2}[/\-.]\d{1,2}[/\-.]\d{2,4}|\d{4}[/\-.]\d{1,2}[/\-.]\d{1,2}|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\.?\s+\d{1,2}(?:st|nd|rd|th)?(?:,)?\s+\d{2,4})";
            return new List<string> {
                $@"(?i)\b{keyRegex}\b\s*[:\-=\s]\s*{datePattern}",
                $@"{datePattern}\s*(?:as of|for|is)?\s*\b{keyRegex}\b"
            };
        }
        private List<string> CreateTextPatternsFromKeyName(string keyPattern, bool isGroup = false)
        {
            string keyRegex = isGroup ? $"(?:{keyPattern})" : $@"\b{Regex.Escape(keyPattern)}\b";
            string valuePattern = @"(.+?)(?:\n|\r|$|\s{2,}(?:\b(?:Amount|Total|Date|Due|Item|Product|Price|Qty)\b)|--|==)";
            return new List<string> {
                $@"(?i)\b{keyRegex}\b\s*[:\-=\s]\s*{valuePattern}",
                $@"(?i)\b{keyRegex}\b\s*[:\-=\s]\s*""([^""]+)""",
                $@"(?i)\b{keyRegex}\b\s*[:\-=\s]\s*'([^']*)'"
            };
        }

        #endregion

        #region Helper Class for Omission Pattern Strategy Analysis (Local Heuristics)
        private class OmissionPatternStrategy
        {
            public string PatternType { get; set; }
            public string BasePattern { get; set; }
            public bool RequiresMultiline { get; set; } = false;
            public int MaxLines { get; set; } = 1;
        }
        #endregion
    }
}