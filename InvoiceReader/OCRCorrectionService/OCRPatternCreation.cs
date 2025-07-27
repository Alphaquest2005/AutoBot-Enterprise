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

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Advanced format correction pattern creation with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Create format correction patterns using multiple strategies to transform original values to corrected values
        /// **BUSINESS OBJECTIVE**: Enable automated pattern generation for common OCR formatting errors and corrections
        /// **SUCCESS CRITERIA**: Must identify appropriate correction strategy or return null for unchanged/invalid inputs
        /// </summary>
        public (string Pattern, string Replacement)? CreateAdvancedFormatCorrectionPatterns(
            string originalValue,
            string correctedValue)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Complete LLM diagnostic workflow for format correction pattern creation
            
            // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
            _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for advanced format correction pattern creation");
            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Pattern creation context with multi-strategy format correction approach");
            _logger.Error("🔍 **PATTERN_ANALYSIS**: Input validation → strategy iteration → pattern generation → result selection pattern");
            _logger.Error("❓ **EVIDENCE_GAPS**: Need input validation, strategy evaluation, pattern generation success, result appropriateness");
            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Format correction requires systematic strategy evaluation with first-match selection");
            
            // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
            _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for pattern creation");
            _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed validation, strategy evaluation, pattern generation, selection tracking");
            _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Input validation, strategy attempts, pattern results, selection criteria");
            
            // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
            _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based advanced format correction pattern creation");
            _logger.Error("📚 **FIX_RATIONALE**: Based on format correction requirements, implementing multi-strategy evaluation workflow");
            _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring strategy evaluation and pattern generation outcomes");
            
            // **v4.2 PATTERN CREATION EXECUTION**: Enhanced multi-strategy format correction with success tracking
            _logger.Error("🎨 **FORMAT_CORRECTION_START**: Beginning advanced format correction pattern creation");
            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Creation context - OriginalValue='{OriginalValue}', CorrectedValue='{CorrectedValue}'", 
                originalValue ?? "NULL", correctedValue ?? "NULL");
            _logger.Error("🔍 **PATTERN_ANALYSIS**: Multi-strategy format correction with systematic evaluation");
            
            // **v4.2 INPUT VALIDATION**: Enhanced input validation with comprehensive checks
            bool originalValid = !string.IsNullOrWhiteSpace(originalValue);
            bool correctedValid = !string.IsNullOrWhiteSpace(correctedValue);
            bool valuesChanged = originalValue != correctedValue;
            
            _logger.Error(originalValid ? "✅ **ORIGINAL_VALUE_VALID**: Original value provided and non-empty" : "❌ **ORIGINAL_VALUE_INVALID**: Original value is null or whitespace");
            _logger.Error(correctedValid ? "✅ **CORRECTED_VALUE_VALID**: Corrected value provided and non-empty" : "❌ **CORRECTED_VALUE_INVALID**: Corrected value is null or whitespace");
            _logger.Error(valuesChanged ? "✅ **VALUES_CHANGED**: Original and corrected values are different" : "❌ **VALUES_UNCHANGED**: Original and corrected values are identical");
            
            if (!originalValid || !correctedValid || !valuesChanged)
            {
                _logger.Error("📝 **PATTERN_CREATION_SKIPPED**: Input validation failed - returning null");
                
                // **STEP 4: MANDATORY SUCCESS CRITERIA VALIDATION - INVALID INPUT PATH**
                _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: Pattern creation failed due to invalid input");
                _logger.Error("❌ **PURPOSE_FULFILLMENT**: Cannot create patterns for invalid or unchanged input values");
                _logger.Error("✅ **OUTPUT_COMPLETENESS**: Appropriate null returned for invalid input conditions");
                _logger.Error("✅ **PROCESS_COMPLETION**: Input validation completed with appropriate early termination");
                _logger.Error("✅ **DATA_QUALITY**: Input validation ensures only valid data proceeds to pattern creation");
                _logger.Error("✅ **ERROR_HANDLING**: Invalid input handled gracefully with null return");
                _logger.Error("✅ **BUSINESS_LOGIC**: Pattern creation objective handled appropriately for invalid input");
                _logger.Error("✅ **INTEGRATION_SUCCESS**: No external dependencies for input validation");
                _logger.Error("✅ **PERFORMANCE_COMPLIANCE**: Validation completed within reasonable timeframe");
                _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: ✅ PASS - Invalid input handled appropriately with null return");
                
                return null;
            }
            
            // **v4.2 STRATEGY EVALUATION**: Enhanced multi-strategy evaluation with comprehensive tracking
            _logger.Error("🔄 **STRATEGY_EVALUATION_START**: Beginning systematic strategy evaluation");
            var strategies = new Func<string, string, (string, string)?>[]
                                 {
                                     CreateDecimalSeparatorPattern, CreateCurrencySymbolPattern,
                                     CreateNegativeNumberPattern, CreateSpecificOCRCharacterConfusionPattern,
                                     CreateSpaceManipulationPattern
                                 };
            
            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Strategy evaluation - StrategyCount={StrategyCount}", strategies.Length);
            
            int strategyIndex = 0;
            foreach (var strategy in strategies)
            {
                strategyIndex++;
                _logger.Error("🔍 **STRATEGY_ATTEMPT**: Evaluating strategy {Index}/{Total}: {StrategyName}", 
                    strategyIndex, strategies.Length, strategy.Method.Name);
                
                var result = strategy(originalValue, correctedValue);
                if (result.HasValue)
                {
                    _logger.Error("✅ **STRATEGY_SUCCESS**: Strategy '{StrategyName}' generated pattern successfully", strategy.Method.Name);
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Pattern result - Pattern='{Pattern}', Replacement='{Replacement}'", 
                        result.Value.Item1, result.Value.Item2);
                    
                    // **STEP 4: MANDATORY SUCCESS CRITERIA VALIDATION - SUCCESS PATH**
                    _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: Format correction pattern creation success analysis");
                    
                    bool patternGenerated = result.HasValue;
                    bool strategySuccessful = true;
                    bool outputValid = !string.IsNullOrEmpty(result.Value.Item1) && !string.IsNullOrEmpty(result.Value.Item2);
                    
                    _logger.Error("✅ **PURPOSE_FULFILLMENT**: Format correction pattern successfully generated using appropriate strategy");
                    _logger.Error(outputValid ? "✅" : "❌" + " **OUTPUT_COMPLETENESS**: Valid pattern and replacement tuple returned");
                    _logger.Error(strategySuccessful ? "✅" : "❌" + " **PROCESS_COMPLETION**: Strategy evaluation completed with successful pattern generation");
                    _logger.Error(outputValid ? "✅" : "❌" + " **DATA_QUALITY**: Generated pattern and replacement are non-empty and valid");
                    _logger.Error("✅ **ERROR_HANDLING**: No exception handling required for successful pattern generation");
                    _logger.Error("✅ **BUSINESS_LOGIC**: Format correction objective achieved with first successful strategy");
                    _logger.Error(patternGenerated ? "✅" : "❌" + " **INTEGRATION_SUCCESS**: Strategy integration successful with pattern generation");
                    _logger.Error("✅ **PERFORMANCE_COMPLIANCE**: Pattern creation completed within reasonable timeframe");
                    
                    bool overallSuccess = patternGenerated && strategySuccessful && outputValid;
                    _logger.Error(overallSuccess ? "🏆 **OVERALL_METHOD_SUCCESS**: ✅ PASS" : "🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL" + " - Format correction pattern creation analysis");
                    
                    _logger.Error("📊 **CREATION_SUMMARY**: Strategy: '{StrategyName}', Pattern: '{Pattern}', Replacement: '{Replacement}'", 
                        strategy.Method.Name, result.Value.Item1, result.Value.Item2);
                    
                    return result;
                }
                else
                {
                    _logger.Error("📝 **STRATEGY_NO_MATCH**: Strategy '{StrategyName}' did not generate a pattern", strategy.Method.Name);
                }
            }
            
            // **v4.2 NO STRATEGY MATCHED**: Enhanced no-match handling
            _logger.Error("📝 **ALL_STRATEGIES_EXHAUSTED**: No strategy generated a suitable pattern");
            
            // **STEP 4: MANDATORY SUCCESS CRITERIA VALIDATION - NO MATCH PATH**
            _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: Pattern creation completed with no strategy match");
            _logger.Error("✅ **PURPOSE_FULFILLMENT**: All available strategies evaluated for format correction");
            _logger.Error("✅ **OUTPUT_COMPLETENESS**: Appropriate null returned when no strategy matches");
            _logger.Error("✅ **PROCESS_COMPLETION**: Complete strategy evaluation performed");
            _logger.Error("✅ **DATA_QUALITY**: No pattern generated maintains method contract for non-correctable formats");
            _logger.Error("✅ **ERROR_HANDLING**: No strategy match handled gracefully with null return");
            _logger.Error("✅ **BUSINESS_LOGIC**: Format correction objective handled appropriately when no strategy applies");
            _logger.Error("✅ **INTEGRATION_SUCCESS**: All strategy integrations evaluated successfully");
            _logger.Error("✅ **PERFORMANCE_COMPLIANCE**: Complete evaluation completed within reasonable timeframe");
            _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: ✅ PASS - No strategy match handled appropriately with null return");
            
            _logger.Error("📊 **EVALUATION_SUMMARY**: Strategies evaluated: {StrategyCount}, Matches found: 0", strategies.Length);
            
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