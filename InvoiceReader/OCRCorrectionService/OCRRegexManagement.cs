// File: OCRCorrectionService/OCRRegexManagement.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region DeepSeek Regex Creation

        /// <summary>
        /// Requests new regex pattern from DeepSeek for omission correction
        /// </summary>
        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeek(
            CorrectionResult correction,
            LineContext lineContext)
        {
            try
            {
                var existingNamedGroups = lineContext.FieldsInLine?.Select(f => f.Key).ToList() ?? new List<string>();

                var prompt = $@"CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:

A field '{correction.FieldName}' with value '{correction.NewValue}' was found but not extracted by current OCR processing.

CURRENT LINE REGEX: {lineContext.RegexPattern ?? "None"}
EXISTING NAMED GROUPS: {string.Join(", ", existingNamedGroups)}

TARGET LINE:
{correction.LineText}

FULL CONTEXT:
{string.Join("\n", correction.ContextLinesBefore)}
>>> TARGET LINE {correction.LineNumber}: {correction.LineText} <<<
{string.Join("\n", correction.ContextLinesAfter)}

REQUIREMENTS:
1. Create a regex pattern that extracts the value '{correction.NewValue}' using named group (?<{correction.FieldName}>pattern)
2. If updating existing regex, ensure you don't break existing named groups: {string.Join(", ", existingNamedGroups)}
3. Pattern should work with the provided context
4. Decide if this should be a separate line or modify existing line regex

RESPONSE FORMAT:
{{
  ""strategy"": ""modify_existing_line"" OR ""create_new_line"",
  ""regex_pattern"": ""(?<{correction.FieldName}>your_pattern_here)"",
  ""complete_line_regex"": ""full regex if modifying existing line"",
  ""is_multiline"": true/false,
  ""max_lines"": your_determined_number,
  ""test_match"": ""exact text from context that should be matched"",
  ""confidence"": 0.95,
  ""reasoning"": ""why you chose this approach and pattern"",
  ""preserves_existing_groups"": true/false
}}

EXAMPLES:
- Add to existing: Combine patterns preserving existing named groups
- New line: Create completely separate regex with single named group

Choose the safest approach that won't break existing extractions.";

                var response = await _deepSeekApi.GetResponseAsync(prompt);
                return ParseRegexCreationResponse(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error requesting regex from DeepSeek for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Parses DeepSeek response for regex creation
        /// </summary>
        private RegexCreationResponse ParseRegexCreationResponse(string response)
        {
            try
            {
                var cleanJson = CleanJsonResponse(response);
                if (string.IsNullOrWhiteSpace(cleanJson))
                {
                    _logger.Warning("No valid JSON found in DeepSeek regex creation response");
                    return null;
                }

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                return new RegexCreationResponse
                {
                    Strategy = GetStringValue(root, "strategy") ?? "create_new_line",
                    RegexPattern = GetStringValue(root, "regex_pattern") ?? "",
                    CompleteLineRegex = GetStringValue(root, "complete_line_regex") ?? "",
                    IsMultiline = GetBooleanValue(root, "is_multiline"),
                    MaxLines = GetIntValue(root, "max_lines", 1),
                    TestMatch = GetStringValue(root, "test_match") ?? "",
                    Confidence = GetDoubleValue(root, "confidence"),
                    Reasoning = GetStringValue(root, "reasoning") ?? "",
                    PreservesExistingGroups = GetBooleanValue(root, "preserves_existing_groups", true)
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error parsing regex creation response");
                return null;
            }
        }

        /// <summary>
        /// Validates DeepSeek-generated regex pattern
        /// </summary>
        public bool ValidateRegexPattern(RegexCreationResponse regexResponse, CorrectionResult correction)
        {
            try
            {
                if (regexResponse == null || string.IsNullOrEmpty(regexResponse.RegexPattern))
                {
                    _logger.Warning("Invalid regex response for field {FieldName}", correction.FieldName);
                    return false;
                }

                var regex = new Regex(regexResponse.RegexPattern,
                    regexResponse.IsMultiline ? RegexOptions.Multiline : RegexOptions.None);

                // Test against the full context that DeepSeek provided
                var fullContext = correction.FullContext;
                var match = regex.Match(fullContext);

                if (!match.Success)
                {
                    _logger.Warning("Regex pattern failed to match in provided context for field {FieldName}",
                        correction.FieldName);
                    return false;
                }

                // Verify the captured value matches what DeepSeek expected
                if (match.Groups[correction.FieldName]?.Value != correction.NewValue)
                {
                    _logger.Warning("Regex captured '{Captured}' but expected '{Expected}' for field {FieldName}",
                        match.Groups[correction.FieldName]?.Value, correction.NewValue, correction.FieldName);
                    return false;
                }

                // Verify the test match works if provided
                if (!string.IsNullOrEmpty(regexResponse.TestMatch))
                {
                    var testMatch = regex.Match(regexResponse.TestMatch);
                    if (!testMatch.Success)
                    {
                        _logger.Warning("Regex failed to match against provided test_match for field {FieldName}",
                            correction.FieldName);
                        return false;
                    }
                }

                _logger.Information("Regex validation successful for field {FieldName}. Pattern: {Pattern}, Multiline: {IsMultiline}, MaxLines: {MaxLines}",
                    correction.FieldName, regexResponse.RegexPattern, regexResponse.IsMultiline, regexResponse.MaxLines);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating regex pattern for field {FieldName}: {Pattern}",
                    correction.FieldName, regexResponse?.RegexPattern);
                return false;
            }
        }

        /// <summary>
        /// Creates database entries for new omission field
        /// </summary>
        public async Task<bool> CreateDatabaseEntriesForOmission(
            RegexCreationResponse regexResponse,
            LineContext lineContext,
            CorrectionResult correction)
        {
            try
            {
                // Initialize strategy factory if not already done
                _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger, this);

                using var context = new OCRContext();

                // Create update request
                var request = new RegexUpdateRequest
                {
                    FieldName = correction.FieldName,
                    OldValue = correction.OldValue,
                    NewValue = correction.NewValue,
                    CorrectionType = "omission",
                    LineId = lineContext.LineId,
                    PartId = lineContext.PartId,
                    RegexId = GetRegexIdFromLineContext(lineContext),
                    ExistingRegex = lineContext.RegexPattern,
                    LineText = correction.LineText,
                    ContextBefore = string.Join("\n", correction.ContextLinesBefore),
                    ContextAfter = string.Join("\n", correction.ContextLinesAfter)
                };

                // Get omission strategy and execute
                var strategy = _strategyFactory.GetStrategy(correction);
                var result = await strategy.ExecuteAsync(context, request);

                if (result.IsSuccess)
                {
                    _logger.Information("Successfully created database entries for omission field {FieldName}: {Operation}",
                        correction.FieldName, result.Operation);
                    return true;
                }
                else
                {
                    _logger.Warning("Failed to create database entries for omission field {FieldName}: {Message}",
                        correction.FieldName, result.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating database entries for omission field {FieldName}", correction.FieldName);
                return false;
            }
        }

        /// <summary>
        /// Gets regex ID from line context
        /// </summary>
        private int? GetRegexIdFromLineContext(LineContext lineContext)
        {
            if (!lineContext.LineId.HasValue) return null;

            try
            {
                using var context = new OCRContext();
                var line = context.Lines.FirstOrDefault(l => l.Id == lineContext.LineId);
                return line?.RegExId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting regex ID from line context");
                return null;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets string value from JSON element
        /// </summary>
        private string GetStringValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return prop.GetString();
            return null;
        }

        /// <summary>
        /// Gets double value from JSON element
        /// </summary>
        private double GetDoubleValue(JsonElement element, string propertyName, double defaultValue = 0.0)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetDouble(out var value)) return value;
                if (prop.TryGetDecimal(out var decimalValue)) return (double)decimalValue;
                if (prop.TryGetInt32(out var intValue)) return intValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets integer value from JSON element
        /// </summary>
        private int GetIntValue(JsonElement element, string propertyName, int defaultValue = 0)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetInt32(out var value)) return value;
                if (prop.TryGetDouble(out var doubleValue)) return (int)doubleValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets boolean value from JSON element
        /// </summary>
        private bool GetBooleanValue(JsonElement element, string propertyName, bool defaultValue = false)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;

                // Try parsing string values
                var stringValue = prop.GetString();
                if (bool.TryParse(stringValue, out var boolValue)) return boolValue;
            }
            return defaultValue;
        }

        #endregion

        #region Response Models

        /// <summary>
        /// Response from DeepSeek for regex creation
        /// </summary>
        public class RegexCreationResponse
        {
            public string Strategy { get; set; }  // "modify_existing_line" or "create_new_line"
            public string RegexPattern { get; set; }
            public string CompleteLineRegex { get; set; }  // Full regex for line updates
            public bool IsMultiline { get; set; }
            public int MaxLines { get; set; }
            public string TestMatch { get; set; }
            public double Confidence { get; set; }
            public string Reasoning { get; set; }
            public bool PreservesExistingGroups { get; set; }  // Safety check
        }

        #endregion
    }
}