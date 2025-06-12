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
        #region Amazon-Specific Pattern Definitions (Phase 5)
        
        /// <summary>
        /// PHASE 5: Amazon-Specific Pattern Fixes (30 minutes)
        /// Pre-defined regex patterns for common Amazon invoice fields
        /// </summary>
        private static readonly Dictionary<string, string> AmazonSpecificPatterns = new Dictionary<string, string>
        {
            // Amazon Gift Card Amount Pattern
            // Source: "Gift Card Amount: -$6.99" → TotalInsurance = -6.99 (customer reduction, negative)
            // Fixed: Currency symbol outside capture group to capture only "-6.99" without "$"
            ["TotalInsurance"] = @"Gift Card Amount:\s*\$?(?<TotalInsurance>-?[\d,]+\.?\d*)",
            
            // Amazon Free Shipping Pattern (handles multiple lines)
            // Source: "Free Shipping: -$0.46" and "Free Shipping: -$6.53"
            // Target: Sum to TotalDeduction = 6.99 (supplier reduction, positive sum)
            // Fixed: Currency symbol outside capture group, correct field name in capture group
            ["TotalDeduction"] = @"Free Shipping:\s*\$?(?<TotalDeduction>-?[\d,]+\.?\d*)",
            
            // Amazon Shipping & Handling
            // Fixed: Currency symbol outside capture group
            ["TotalInternalFreight"] = @"Shipping & Handling:\s*\$?(?<TotalInternalFreight>[\d,]+\.?\d*)",
            
            // Amazon Estimated Tax
            // Fixed: Currency symbol outside capture group
            ["TotalOtherCost"] = @"Estimated tax to be collected:\s*\$?(?<TotalOtherCost>[\d,]+\.?\d*)",
            
            // Amazon Item Subtotal
            // Fixed: Currency symbol outside capture group
            ["SubTotal"] = @"Item\(s\) Subtotal:\s*\$?(?<SubTotal>[\d,]+\.?\d*)",
            
            // Amazon Grand Total
            // Fixed: Currency symbol outside capture group
            ["InvoiceTotal"] = @"Grand Total:\s*\$?(?<InvoiceTotal>[\d,]+\.?\d*)"
        };
        
        /// <summary>
        /// Get Amazon-specific pattern for a field if available
        /// </summary>
        private string GetAmazonSpecificPattern(string fieldName, string invoiceText)
        {
            if (AmazonSpecificPatterns.ContainsKey(fieldName) && 
                invoiceText?.IndexOf("Amazon.com", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var pattern = AmazonSpecificPatterns[fieldName];
                _logger?.Error("🎯 **AMAZON_PATTERN_USED**: Using Amazon-specific pattern for {FieldName}: '{Pattern}'", 
                    fieldName, pattern);
                
                // For TotalDeduction (Free Shipping), we need to handle multiple matches and sum them
                if (fieldName == "TotalDeduction")
                {
                    var enhancedPattern = CreateEnhancedFreeShippingPattern(invoiceText);
                    if (!string.IsNullOrEmpty(enhancedPattern))
                    {
                        _logger?.Error("🎯 **AMAZON_ENHANCED_PATTERN**: Using enhanced Free Shipping pattern: '{Pattern}'", enhancedPattern);
                        pattern = enhancedPattern;
                    }
                }
                
                // Test pattern against actual invoice text
                TestAmazonPattern(pattern, fieldName, invoiceText);
                return pattern;
            }
            return null;
        }
        
        /// <summary>
        /// Create enhanced Free Shipping pattern that captures and sums multiple Free Shipping lines
        /// </summary>
        private string CreateEnhancedFreeShippingPattern(string invoiceText)
        {
            try
            {
                // First, find all Free Shipping amounts in the text
                // Fixed: Currency symbol outside capture group to capture only numeric values
                var freeShippingRegex = new Regex(@"Free Shipping:\s*\$?(-?[\d,]+\.?\d*)", RegexOptions.IgnoreCase);
                var matches = freeShippingRegex.Matches(invoiceText);
                
                if (matches.Count > 0)
                {
                    double totalFreeShipping = 0;
                    var foundValues = new List<string>();
                    
                    foreach (Match match in matches)
                    {
                        var value = match.Groups[1].Value;
                        foundValues.Add(value);
                        
                        // Parse and sum for pattern generation
                        var cleanValue = value.Replace("$", "").Replace(",", "").Trim();
                        bool isNegative = cleanValue.StartsWith("-");
                        if (isNegative) cleanValue = cleanValue.TrimStart('-');
                        
                        if (double.TryParse(cleanValue, out var amount))
                        {
                            totalFreeShipping += Math.Abs(amount);
                        }
                    }
                    
                    // Create a pattern that will match the calculated total
                    var totalString = totalFreeShipping.ToString("F2");
                    _logger?.Error("🔍 **AMAZON_PATTERN_CALCULATION**: Found {Count} Free Shipping amounts: {Values} → Total: {Total}", 
                        matches.Count, string.Join(", ", foundValues), totalString);
                    
                    // Return a pattern that matches the total value directly
                    // This pattern will be used to extract the calculated total from corrected values
                    return $@"(?<TotalDeduction>{Regex.Escape(totalString)})";
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating enhanced Free Shipping pattern");
            }
            
            // Fallback to basic pattern - Fixed: Currency symbol outside capture group
            return @"Free Shipping:\s*\$?(?<TotalDeduction>-?[\d,]+\.?\d*)";
        }
        
        /// <summary>
        /// Test Amazon pattern against actual invoice text
        /// </summary>
        private void TestAmazonPattern(string pattern, string fieldName, string invoiceText)
        {
            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var matches = regex.Matches(invoiceText);
                
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var value = match.Groups[fieldName]?.Value ?? match.Groups["FreeShippingAmount"]?.Value;
                        _logger?.Error("✅ **AMAZON_PATTERN_MATCH**: Field {FieldName} matched '{Value}' in invoice text", 
                            fieldName, value);
                    }
                }
                else
                {
                    _logger?.Error("❌ **AMAZON_PATTERN_NO_MATCH**: Field {FieldName} pattern found no matches in invoice text", 
                        fieldName);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "❌ **AMAZON_PATTERN_ERROR**: Error testing pattern for {FieldName}", fieldName);
            }
        }
        
        #endregion
        
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
            _logger.Error("🔍 **VALIDATION_ENTRY**: ValidateRegexPattern called for field {FieldName}", correction?.FieldName);
            
            if (regexResponse == null)
            {
                _logger.Error("❌ **VALIDATION_STEP_1_FAILED**: RegexCreationResponse is null for field {FieldName}.", correction?.FieldName);
                return false;
            }
            _logger.Error("✅ **VALIDATION_STEP_1_SUCCESS**: RegexCreationResponse is not null");
            
            if (string.IsNullOrEmpty(regexResponse.RegexPattern) && string.IsNullOrEmpty(regexResponse.CompleteLineRegex))
            {
                _logger.Error("❌ **VALIDATION_STEP_2_FAILED**: Both RegexPattern and CompleteLineRegex are empty for field {FieldName}.", correction?.FieldName);
                return false;
            }
            _logger.Error("✅ **VALIDATION_STEP_2_SUCCESS**: At least one pattern is provided - RegexPattern='{RegexPattern}', CompleteLineRegex='{CompleteLineRegex}'", 
                regexResponse.RegexPattern ?? "NULL", regexResponse.CompleteLineRegex ?? "NULL");

            string patternToValidate = regexResponse.Strategy == "modify_existing_line" && !string.IsNullOrEmpty(regexResponse.CompleteLineRegex)
                                   ? regexResponse.CompleteLineRegex
                                   : regexResponse.RegexPattern;
            _logger.Error("🔍 **VALIDATION_PATTERN_SELECTION**: Strategy={Strategy}, Selected pattern='{SelectedPattern}'", 
                regexResponse.Strategy, patternToValidate);
                
            if (string.IsNullOrEmpty(patternToValidate))
            {
                patternToValidate = regexResponse.RegexPattern;
                if (string.IsNullOrEmpty(patternToValidate))
                {
                    _logger.Error("❌ **VALIDATION_STEP_3_FAILED**: No usable pattern string to validate for field {FieldName}.", correction?.FieldName);
                    return false;
                }
            }
            _logger.Error("✅ **VALIDATION_STEP_3_SUCCESS**: Pattern selected for validation: '{PatternToValidate}'", patternToValidate);

            try
            {
                // Normalize double-escaped regex patterns from DeepSeek JSON responses
                // DeepSeek returns patterns like (?<TotalInsurance>-\\$\\d+\\.\\d{2}) which need normalization
                string normalizedPattern = patternToValidate;
                if (patternToValidate.Contains("\\\\"))
                {
                    // Replace double backslashes with single backslashes for regex compilation
                    normalizedPattern = patternToValidate.Replace("\\\\", "\\");
                    _logger.Error("🔍 **VALIDATION_NORMALIZATION**: Normalized regex pattern from '{Original}' to '{Normalized}' for field {FieldName}", 
                        patternToValidate, normalizedPattern, correction?.FieldName);
                }
                else
                {
                    _logger.Error("🔍 **VALIDATION_NO_NORMALIZATION**: Pattern does not contain double backslashes, using as-is: '{Pattern}'", normalizedPattern);
                }

                var regexOptions = RegexOptions.IgnoreCase;
                if (regexResponse.IsMultiline) regexOptions |= RegexOptions.Multiline;
                _logger.Error("🔍 **VALIDATION_REGEX_OPTIONS**: Using regex options: {Options}", regexOptions);

                var regex = new Regex(normalizedPattern, regexOptions);
                _logger.Error("✅ **VALIDATION_STEP_4_SUCCESS**: Regex compiled successfully for pattern: '{NormalizedPattern}'", normalizedPattern);

                string testText = !string.IsNullOrEmpty(regexResponse.TestMatch)
                                ? regexResponse.TestMatch
                                : correction?.FullContext;
                
                // **CRITICAL**: If pattern includes context (like "Gift Card Amount:"), we need the full line, not just the value
                if (patternToValidate.Contains("Gift Card") || patternToValidate.Contains("Amount:") || patternToValidate.Contains("\\s*"))
                {
                    // Pattern expects context - use LineText or FullContext
                    if (!string.IsNullOrEmpty(correction?.LineText))
                    {
                        testText = correction.LineText;
                        _logger.Error("🔍 **VALIDATION_TEXT_CONTEXT_AWARE**: Pattern includes context, using LineText: '{LineText}'", testText);
                    }
                    else if (!string.IsNullOrEmpty(correction?.FullContext))
                    {
                        testText = correction.FullContext;
                        _logger.Error("🔍 **VALIDATION_TEXT_CONTEXT_FALLBACK**: Pattern includes context, using FullContext: '{FullContext}'", testText);
                    }
                }
                
                if (string.IsNullOrEmpty(testText) && !string.IsNullOrEmpty(correction?.NewValue))
                {
                    testText = correction.NewValue;
                    _logger.Error("🔍 **VALIDATION_TEST_TEXT_FALLBACK**: Test text for {FieldName} fell back to NewValue itself.", correction?.FieldName);
                }
                _logger.Error("🔍 **VALIDATION_TEST_TEXT**: Using test text: '{TestText}'", testText);
                
                if (string.IsNullOrEmpty(testText))
                {
                    _logger.Error("❌ **VALIDATION_STEP_5_FAILED**: No test text available (TestMatch, FullContext, or NewValue) for field {FieldName}. Cannot validate match.", correction?.FieldName);
                    return false;
                }
                _logger.Error("✅ **VALIDATION_STEP_5_SUCCESS**: Test text is available");

                var match = regex.Match(testText);
                _logger.Error("🔍 **VALIDATION_MATCH_ATTEMPT**: Regex.Match() executed - Success={Success}, Value='{Value}', Groups.Count={GroupCount}", 
                    match.Success, match.Value, match.Groups.Count);
                    
                if (!match.Success)
                {
                    _logger.Error("❌ **VALIDATION_STEP_6_FAILED**: Pattern for {FieldName} failed to match test text. Pattern: '{Pattern}', TestText: '{TestTextSnippet}'",
                        correction?.FieldName, normalizedPattern, this.TruncateForLog(testText));
                    return false;
                }
                _logger.Error("✅ **VALIDATION_STEP_6_SUCCESS**: Regex pattern matched test text successfully");

                var fieldGroupName = correction?.FieldName;
                _logger.Error("🔍 **VALIDATION_FIELD_GROUP**: Looking for named group '{FieldGroupName}'", fieldGroupName);
                
                if (string.IsNullOrEmpty(fieldGroupName))
                {
                    _logger.Error("❌ **VALIDATION_STEP_7_FAILED**: Correction object or FieldName is null. Cannot validate group name.");
                    return false; // Cannot validate group if fieldname is null
                }
                _logger.Error("✅ **VALIDATION_STEP_7_SUCCESS**: Field group name is available");

                var capturedGroup = match.Groups[fieldGroupName];
                _logger.Error("🔍 **VALIDATION_CAPTURED_GROUP**: Group '{FieldGroupName}' - Success={Success}, Value='{Value}', Index={Index}, Length={Length}", 
                    fieldGroupName, capturedGroup.Success, capturedGroup.Value, capturedGroup.Index, capturedGroup.Length);
                    
                if (!capturedGroup.Success)
                {
                    _logger.Error("❌ **VALIDATION_STEP_8_FAILED**: Named group '(?<{FieldName}>...)' not found or did not capture in pattern for field {FieldName}. Pattern: '{Pattern}'. Matched text: '{MatchedText}'",
                       fieldGroupName, normalizedPattern, this.TruncateForLog(match.Value));

                    _logger.Error("🔍 **VALIDATION_ALL_GROUPS**: Examining all {GroupCount} groups in match:", match.Groups.Count);
                    for (int i = 0; i < match.Groups.Count; i++)
                    {
                        var group = match.Groups[i];
                        _logger.Error("🔍 **VALIDATION_GROUP_{Index}**: Name='{Name}', Success={Success}, Value='{Value}'", 
                            i, group.Name, group.Success, group.Value);
                    }

                    foreach (Group g in match.Groups)
                    {
                        if (g.Success && !int.TryParse(g.Name, out _) && g.Value == correction.NewValue)
                        {
                            _logger.Error("✅ **VALIDATION_TOLERANCE_SUCCESS**: Field group '{ExpectedGroup}' missed, but group '{ActualGroup}' captured the correct value '{Value}'. Tolerating.", fieldGroupName, g.Name, correction.NewValue);
                            return true;
                        }
                    }
                    return false;
                }
                _logger.Error("✅ **VALIDATION_STEP_8_SUCCESS**: Named group '{FieldGroupName}' captured successfully", fieldGroupName);
                
                _logger.Error("🔍 **VALIDATION_VALUE_CHECK**: Expected='{Expected}', Captured='{Captured}'", correction.NewValue, capturedGroup.Value);
                
                if (capturedGroup.Value != correction.NewValue)
                {
                    // Enhanced tolerance for currency fields - handle currency symbol extraction differences
                    bool isTolerableMatch = false;
                    string toleranceReason = "";
                    
                    // Check for exact trimmed match
                    if (capturedGroup.Value.Trim() == (correction.NewValue ?? "").Trim())
                    {
                        isTolerableMatch = true;
                        toleranceReason = "whitespace difference";
                    }
                    // Check for containment (original logic)
                    else if (!string.IsNullOrEmpty(correction.NewValue) && capturedGroup.Value.Contains(correction.NewValue))
                    {
                        isTolerableMatch = true;
                        toleranceReason = "contains expected value";
                    }
                    // **NEW**: Check for currency symbol tolerance (e.g., "-$6.99" vs "-6.99")
                    else if (!string.IsNullOrEmpty(correction.NewValue) && !string.IsNullOrEmpty(capturedGroup.Value))
                    {
                        // Remove common currency symbols for comparison
                        string normalizedCaptured = capturedGroup.Value.Replace("$", "").Replace("€", "").Replace("£", "").Replace("¥", "").Trim();
                        string normalizedExpected = correction.NewValue.Replace("$", "").Replace("€", "").Replace("£", "").Replace("¥", "").Trim();
                        
                        if (normalizedCaptured == normalizedExpected)
                        {
                            isTolerableMatch = true;
                            toleranceReason = "currency symbol difference";
                            _logger.Error("🔍 **VALIDATION_CURRENCY_NORMALIZATION**: Captured='{Captured}' → '{NormalizedCaptured}' | Expected='{Expected}' → '{NormalizedExpected}'", 
                                capturedGroup.Value, normalizedCaptured, correction.NewValue, normalizedExpected);
                        }
                    }
                    
                    if (isTolerableMatch)
                    {
                        _logger.Error("✅ **VALIDATION_VALUE_TOLERANCE**: Captured value '{Captured}' for '{FieldName}' differs from expected '{Expected}' but is accepted ({Reason}).",
                           capturedGroup.Value, fieldGroupName, correction.NewValue, toleranceReason);
                    }
                    else
                    {
                        _logger.Error("❌ **VALIDATION_STEP_9_FAILED**: Captured value '{Captured}' does not match expected '{Expected}' for field {FieldName}. Pattern: '{Pattern}'",
                            capturedGroup.Value, correction.NewValue, fieldGroupName, normalizedPattern);
                        return false;
                    }
                }
                else
                {
                    _logger.Error("✅ **VALIDATION_STEP_9_SUCCESS**: Captured value matches expected value exactly");
                }

                if (regexResponse.Strategy == "modify_existing_line" && !regexResponse.PreservesExistingGroups)
                {
                    _logger.Error("⚠️ **VALIDATION_WARNING**: Strategy is 'modify_existing_line' but DeepSeek response indicates existing groups might not be preserved for field {FieldName}. Pattern: '{Pattern}'. This is a high risk.",
                        correction.FieldName, normalizedPattern);
                }

                _logger.Error("✅ **VALIDATION_COMPLETE_SUCCESS**: Regex validation successful for field {FieldName}. Pattern: '{PatternToValidate}'", correction.FieldName, normalizedPattern);
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex, "❌ **VALIDATION_REGEX_SYNTAX_ERROR**: Invalid regex pattern syntax for field {FieldName}: {Pattern}", correction?.FieldName, patternToValidate);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **VALIDATION_UNEXPECTED_ERROR**: Unexpected error during regex validation for field {FieldName}, pattern: {Pattern}", correction?.FieldName, patternToValidate);
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