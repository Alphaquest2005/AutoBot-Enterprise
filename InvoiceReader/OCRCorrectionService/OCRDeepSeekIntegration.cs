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
    /// Enhanced DeepSeek integration for OCR correction service with omission handling and context line support
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Enhanced DeepSeek Response Processing

        /// <summary>
        /// Processes DeepSeek response and creates correction results with enhanced context support
        /// </summary>
        /// <param name="deepSeekResponse">Raw DeepSeek response</param>
        /// <param name="originalText">Original OCR text</param>
        /// <returns>List of correction results with context lines</returns>
        public List<CorrectionResult> ProcessDeepSeekResponse(string deepSeekResponse, string originalText)
        {
            var corrections = new List<CorrectionResult>();

            try
            {
                _logger?.Information("Processing DeepSeek response for OCR corrections");

                // Parse JSON response
                var responseData = ParseDeepSeekResponse(deepSeekResponse);
                if (responseData == null)
                {
                    _logger?.Warning("Failed to parse DeepSeek response as JSON");
                    return corrections;
                }

                // Extract corrections from response with enhanced context support
                corrections = ExtractCorrectionsFromResponse(responseData.Value, originalText);

                _logger?.Information("Extracted {CorrectionCount} corrections from DeepSeek response", corrections.Count);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error processing DeepSeek response");
            }

            return corrections;
        }

        /// <summary>
        /// Parses DeepSeek JSON response
        /// </summary>
        /// <param name="response">Raw response string</param>
        /// <returns>Parsed JSON data or null if failed</returns>
        private JsonElement? ParseDeepSeekResponse(string response)
        {
            try
            {
                // Clean up response if needed
                var cleanResponse = CleanDeepSeekResponse(response);
                
                using var document = JsonDocument.Parse(cleanResponse);
                return document.RootElement.Clone();
            }
            catch (JsonException ex)
            {
                _logger?.Warning(ex, "Failed to parse DeepSeek response as JSON: {Response}", response.Substring(0, Math.Min(200, response.Length)));
                return null;
            }
        }

        /// <summary>
        /// Cleans up DeepSeek response for JSON parsing
        /// </summary>
        /// <param name="response">Raw response</param>
        /// <returns>Cleaned response</returns>
        private string CleanDeepSeekResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return "{}";

            // Remove markdown code blocks if present
            response = Regex.Replace(response, @"```json\s*", "", RegexOptions.IgnoreCase);
            response = Regex.Replace(response, @"```\s*$", "", RegexOptions.IgnoreCase);

            // Remove any leading/trailing whitespace
            response = response.Trim();

            // If response doesn't start with {, try to find JSON content
            if (!response.StartsWith("{"))
            {
                var jsonMatch = Regex.Match(response, @"\{.*\}", RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    response = jsonMatch.Value;
                }
                else
                {
                    _logger?.Warning("No JSON content found in DeepSeek response");
                    return "{}";
                }
            }

            return response;
        }

        /// <summary>
        /// Extracts corrections from parsed DeepSeek response with enhanced context support
        /// </summary>
        /// <param name="responseData">Parsed JSON response</param>
        /// <param name="originalText">Original OCR text</param>
        /// <returns>List of corrections with context lines</returns>
        private List<CorrectionResult> ExtractCorrectionsFromResponse(JsonElement responseData, string originalText)
        {
            var corrections = new List<CorrectionResult>();

            try
            {
                // Handle different response formats
                if (responseData.TryGetProperty("errors", out var errorsElement))
                {
                    corrections.AddRange(ProcessCorrectionsArray(errorsElement, originalText));
                }
                else if (responseData.TryGetProperty("corrections", out var correctionsElement))
                {
                    corrections.AddRange(ProcessCorrectionsArray(correctionsElement, originalText));
                }
                else if (responseData.TryGetProperty("fields", out var fieldsObject))
                {
                    corrections.AddRange(ProcessFieldsObject(fieldsObject, originalText));
                }
                else
                {
                    // Try to process as direct field corrections
                    corrections.AddRange(ProcessDirectFieldCorrections(responseData, originalText));
                }

                // Validate and enrich corrections
                corrections = ValidateAndEnrichCorrections(corrections, originalText);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting corrections from DeepSeek response");
            }

            return corrections;
        }

        /// <summary>
        /// Processes corrections array format with enhanced context support
        /// </summary>
        /// <param name="correctionsArray">JSON array of corrections</param>
        /// <param name="originalText">Original text</param>
        /// <returns>List of corrections with context</returns>
        private List<CorrectionResult> ProcessCorrectionsArray(JsonElement correctionsArray, string originalText)
        {
            var corrections = new List<CorrectionResult>();

            if (correctionsArray.ValueKind != JsonValueKind.Array)
                return corrections;

            foreach (var correctionElement in correctionsArray.EnumerateArray())
            {
                var correction = CreateCorrectionFromElement(correctionElement, originalText);
                if (correction != null)
                {
                    corrections.Add(correction);
                }
            }

            return corrections;
        }

        /// <summary>
        /// Creates a correction result from a JSON element with enhanced context support
        /// </summary>
        /// <param name="element">JSON element</param>
        /// <param name="originalText">Original text</param>
        /// <returns>Correction result or null</returns>
        private CorrectionResult CreateCorrectionFromElement(JsonElement element, string originalText)
        {
            try
            {
                var fieldName = element.TryGetProperty("field", out var fieldProp) ? fieldProp.GetString() : null;
                var oldValue = element.TryGetProperty("extracted_value", out var oldProp) ? oldProp.GetString() : null;
                var newValue = element.TryGetProperty("correct_value", out var newProp) ? newProp.GetString() : null;
                var lineText = element.TryGetProperty("line_text", out var lineProp) ? lineProp.GetString() : null;
                var lineNumber = element.TryGetProperty("line_number", out var lineNumProp) ? lineNumProp.GetInt32() : 0;
                var confidence = element.TryGetProperty("confidence", out var confProp) ? confProp.GetDouble() : 0.8;
                var reasoning = element.TryGetProperty("reasoning", out var reasonProp) ? reasonProp.GetString() : null;
                var errorType = element.TryGetProperty("error_type", out var errorProp) ? errorProp.GetString() : null;
                var requiresMultiline = element.TryGetProperty("requires_multiline_regex", out var multiProp) ? multiProp.GetBoolean() : false;

                // Parse context lines - NEW ENHANCED FUNCTIONALITY
                var contextBefore = new List<string>();
                var contextAfter = new List<string>();
                
                if (element.TryGetProperty("context_lines_before", out var beforeArray) && beforeArray.ValueKind == JsonValueKind.Array)
                {
                    contextBefore = beforeArray.EnumerateArray()
                        .Select(e => e.GetString())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                }
                
                if (element.TryGetProperty("context_lines_after", out var afterArray) && afterArray.ValueKind == JsonValueKind.Array)
                {
                    contextAfter = afterArray.EnumerateArray()
                        .Select(e => e.GetString())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                }

                _logger?.Debug("Parsed correction for {FieldName}: {BeforeCount} lines before, {AfterCount} lines after, multiline: {Multiline}", 
                    fieldName, contextBefore.Count, contextAfter.Count, requiresMultiline);

                if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(newValue))
                {
                    _logger?.Warning("Skipping correction with missing field name or new value");
                    return null;
                }

                // If old value not provided, try to find it in original text
                if (string.IsNullOrEmpty(oldValue))
                {
                    oldValue = FindOriginalValue(fieldName, originalText);
                }

                return new CorrectionResult
                {
                    FieldName = MapDeepSeekFieldToDatabase(fieldName)?.DatabaseFieldName ?? fieldName,
                    OldValue = oldValue ?? "",
                    NewValue = newValue,
                    LineText = lineText,
                    LineNumber = lineNumber > 0 ? lineNumber : FindLineNumber(fieldName, originalText),
                    ContextLinesBefore = contextBefore,
                    ContextLinesAfter = contextAfter,
                    RequiresMultilineRegex = requiresMultiline,
                    Success = true,
                    Confidence = confidence,
                    CorrectionType = DetermineCorrectionType(oldValue, newValue, errorType),
                    Reasoning = reasoning
                };
            }
            catch (Exception ex)
            {
                _logger?.Warning(ex, "Failed to create correction from JSON element");
                return null;
            }
        }

        /// <summary>
        /// Processes fields object format
        /// </summary>
        /// <param name="fieldsObject">JSON object with field corrections</param>
        /// <param name="originalText">Original text</param>
        /// <returns>List of corrections</returns>
        private List<CorrectionResult> ProcessFieldsObject(JsonElement fieldsObject, string originalText)
        {
            var corrections = new List<CorrectionResult>();

            if (fieldsObject.ValueKind != JsonValueKind.Object)
                return corrections;

            foreach (var property in fieldsObject.EnumerateObject())
            {
                var fieldName = property.Name;
                var fieldValue = property.Value;

                if (fieldValue.ValueKind == JsonValueKind.Object)
                {
                    var correction = CreateCorrectionFromFieldObject(fieldName, fieldValue, originalText);
                    if (correction != null)
                    {
                        corrections.Add(correction);
                    }
                }
                else if (fieldValue.ValueKind == JsonValueKind.String)
                {
                    // Simple field value correction
                    var correction = CreateSimpleFieldCorrection(fieldName, fieldValue.GetString(), originalText);
                    if (correction != null)
                    {
                        corrections.Add(correction);
                    }
                }
            }

            return corrections;
        }

        /// <summary>
        /// Processes direct field corrections format
        /// </summary>
        /// <param name="responseData">Response data</param>
        /// <param name="originalText">Original text</param>
        /// <returns>List of corrections</returns>
        private List<CorrectionResult> ProcessDirectFieldCorrections(JsonElement responseData, string originalText)
        {
            var corrections = new List<CorrectionResult>();

            // Look for common field names directly in the response
            var commonFields = new[] { "InvoiceTotal", "SubTotal", "InvoiceNo", "InvoiceDate", "SupplierName" };

            foreach (var fieldName in commonFields)
            {
                if (responseData.TryGetProperty(fieldName, out var fieldValue))
                {
                    var correction = CreateSimpleFieldCorrection(fieldName, fieldValue.GetString(), originalText);
                    if (correction != null)
                    {
                        corrections.Add(correction);
                    }
                }
            }

            return corrections;
        }

        /// <summary>
        /// Creates a correction from a field object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldObject">Field object</param>
        /// <param name="originalText">Original text</param>
        /// <returns>Correction result or null</returns>
        private CorrectionResult CreateCorrectionFromFieldObject(string fieldName, JsonElement fieldObject, string originalText)
        {
            var oldValue = fieldObject.TryGetProperty("original", out var origProp) ? origProp.GetString() : null;
            var newValue = fieldObject.TryGetProperty("corrected", out var corrProp) ? corrProp.GetString() : null;
            var confidence = fieldObject.TryGetProperty("confidence", out var confProp) ? confProp.GetDouble() : 0.8;

            if (string.IsNullOrEmpty(newValue))
                return null;

            if (string.IsNullOrEmpty(oldValue))
            {
                oldValue = FindOriginalValue(fieldName, originalText);
            }

            return new CorrectionResult
            {
                FieldName = MapDeepSeekFieldToDatabase(fieldName)?.DatabaseFieldName ?? fieldName,
                OldValue = oldValue ?? "",
                NewValue = newValue,
                Success = true,
                Confidence = confidence,
                CorrectionType = DetermineCorrectionType(oldValue, newValue),
                LineNumber = FindLineNumber(fieldName, originalText)
            };
        }

        /// <summary>
        /// Creates a simple field correction
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="newValue">New value</param>
        /// <param name="originalText">Original text</param>
        /// <returns>Correction result or null</returns>
        private CorrectionResult CreateSimpleFieldCorrection(string fieldName, string newValue, string originalText)
        {
            if (string.IsNullOrEmpty(newValue))
                return null;

            var oldValue = FindOriginalValue(fieldName, originalText);

            return new CorrectionResult
            {
                FieldName = MapDeepSeekFieldToDatabase(fieldName)?.DatabaseFieldName ?? fieldName,
                OldValue = oldValue ?? "",
                NewValue = newValue,
                Success = true,
                Confidence = 0.8,
                CorrectionType = DetermineCorrectionType(oldValue, newValue),
                LineNumber = FindLineNumber(fieldName, originalText)
            };
        }

        #endregion

        #region Enhanced Regex Creation for Omissions

        /// <summary>
        /// Requests new regex pattern from DeepSeek for omission corrections
        /// </summary>
        /// <param name="correction">Correction with omission details</param>
        /// <param name="existingLineRegex">Current line regex pattern</param>
        /// <param name="existingNamedGroups">Existing named groups in line</param>
        /// <returns>Regex creation response</returns>
        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeek(
            CorrectionResult correction, 
            string existingLineRegex, 
            List<string> existingNamedGroups)
        {
            try
            {
                var prompt = CreateRegexCreationPrompt(correction, existingLineRegex, existingNamedGroups);
                var response = await _deepSeekApi.GetResponseAsync(prompt);
                return ParseRegexCreationResponse(response);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error requesting new regex from DeepSeek for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Creates prompt for DeepSeek regex pattern creation
        /// </summary>
        private string CreateRegexCreationPrompt(CorrectionResult correction, string existingLineRegex, List<string> existingNamedGroups)
        {
            return $@"CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:

A field '{correction.FieldName}' with value '{correction.NewValue}' was found but not extracted by current OCR processing.

CURRENT LINE REGEX: {existingLineRegex ?? "None"}
EXISTING NAMED GROUPS: {string.Join(", ", existingNamedGroups ?? new List<string>())}

TARGET LINE:
{correction.LineText}

FULL CONTEXT:
{string.Join("\n", correction.ContextLinesBefore)}
>>> TARGET LINE {correction.LineNumber}: {correction.LineText} <<<
{string.Join("\n", correction.ContextLinesAfter)}

REQUIREMENTS:
1. Create a regex pattern that extracts the value '{correction.NewValue}' using named group (?<{correction.FieldName}>pattern)
2. If updating existing regex, ensure you don't break existing named groups: {string.Join(", ", existingNamedGroups ?? new List<string>())}
3. Pattern should work with the provided context
4. Decide if this should be a separate line or modify existing line regex

RESPONSE FORMAT:
{{
  ""strategy"": ""modify_existing_line"" OR ""create_new_line"",
  ""regex_pattern"": ""(?<{correction.FieldName}>your_pattern_here)"",
  ""complete_line_regex"": ""full regex if modifying existing line"",
  ""is_multiline"": {correction.RequiresMultilineRegex.ToString().ToLower()},
  ""max_lines"": {(correction.RequiresMultilineRegex ? "5" : "1")},
  ""test_match"": ""exact text from context that should be matched"",
  ""confidence"": 0.95,
  ""reasoning"": ""why you chose this approach and pattern"",
  ""preserves_existing_groups"": true
}}

EXAMPLES:
- Single line currency: ""(?<TotalDeduction>Gift Card Applied:\s*-?\$?([0-9]+\.?[0-9]*))""
- Multi-line address: ""(?<Address>(?:.*\n){{1,3}}.*(?:Street|Ave|Blvd|Road))""

Choose the safest approach that won't break existing extractions.";
        }

        /// <summary>
        /// Parses DeepSeek regex creation response
        /// </summary>
        /// <param name="response">Raw DeepSeek response</param>
        /// <returns>Parsed regex creation response</returns>
        private RegexCreationResponse ParseRegexCreationResponse(string response)
        {
            try
            {
                var cleanJson = CleanDeepSeekResponse(response);
                if (string.IsNullOrWhiteSpace(cleanJson))
                {
                    _logger?.Warning("Empty or invalid regex creation response from DeepSeek");
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
                    MaxLines = GetIntValue(root, "max_lines"),
                    TestMatch = GetStringValue(root, "test_match") ?? "",
                    Confidence = GetDoubleValue(root, "confidence"),
                    Reasoning = GetStringValue(root, "reasoning") ?? "",
                    PreservesExistingGroups = GetBooleanValue(root, "preserves_existing_groups")
                };
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error parsing regex creation response");
                return null;
            }
        }

        #endregion

        #region Helper Methods for DeepSeek Processing

        /// <summary>
        /// Finds the original value for a field in the text
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="text">Original text</param>
        /// <returns>Original value or null</returns>
        private string FindOriginalValue(string fieldName, string text)
        {
            // Implementation would use existing OCR extraction logic
            // This is a simplified version
            var patterns = GetFieldExtractionPatterns(fieldName);
            
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets extraction patterns for a field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>List of regex patterns</returns>
        private List<string> GetFieldExtractionPatterns(string fieldName)
        {
            var patterns = new List<string>();

            switch (fieldName.ToLowerInvariant())
            {
                case "invoicetotal":
                case "total":
                    patterns.Add(@"total[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    patterns.Add(@"amount[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    break;
                case "subtotal":
                    patterns.Add(@"subtotal[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    patterns.Add(@"sub[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    break;
                case "invoiceno":
                case "invoice":
                    patterns.Add(@"invoice[:\s#]+([A-Z0-9\-]+)");
                    patterns.Add(@"order[:\s#]+([A-Z0-9\-]+)");
                    break;
                case "totaldeduction":
                case "deduction":
                    patterns.Add(@"gift\s+card[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    patterns.Add(@"deduction[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    patterns.Add(@"discount[:\s]+\$?([0-9,]+\.?[0-9]*)");
                    break;
                default:
                    patterns.Add($@"{fieldName}[:\s]+([^\r\n]+)");
                    break;
            }

            return patterns;
        }

        /// <summary>
        /// Finds the line number where a field appears
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="text">Text to search</param>
        /// <returns>Line number (1-based) or 0 if not found</returns>
        private int FindLineNumber(string fieldName, string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var patterns = GetFieldExtractionPatterns(fieldName);

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (var pattern in patterns)
                {
                    if (Regex.IsMatch(lines[i], pattern, RegexOptions.IgnoreCase))
                    {
                        return i + 1; // Return 1-based line number
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Determines the type of correction based on old and new values
        /// </summary>
        /// <param name="oldValue">Original value</param>
        /// <param name="newValue">Corrected value</param>
        /// <param name="errorType">Error type from DeepSeek</param>
        /// <returns>Correction type</returns>
        private string DetermineCorrectionType(string oldValue, string newValue, string errorType = null)
        {
            // Use explicit error type from DeepSeek if provided
            if (!string.IsNullOrEmpty(errorType))
                return errorType;

            // Omission: no old value, has new value
            if (string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(newValue))
                return "omission";

            // Removal: has old value, no new value
            if (!string.IsNullOrEmpty(oldValue) && string.IsNullOrEmpty(newValue))
                return "removal";

            // Check for format corrections
            var oldNormalized = Regex.Replace(oldValue ?? "", @"[\s\$,\-]", "");
            var newNormalized = Regex.Replace(newValue ?? "", @"[\s\$,\-]", "");

            if (string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase))
                return "format_error";

            return "value_correction";
        }

        /// <summary>
        /// Validates and enriches corrections
        /// </summary>
        /// <param name="corrections">Raw corrections</param>
        /// <param name="originalText">Original text</param>
        /// <returns>Validated corrections</returns>
        private List<CorrectionResult> ValidateAndEnrichCorrections(List<CorrectionResult> corrections, string originalText)
        {
            var validatedCorrections = new List<CorrectionResult>();

            foreach (var correction in corrections)
            {
                // Validate field name
                if (!IsFieldSupported(correction.FieldName))
                {
                    _logger?.Warning("Unsupported field name in correction: {FieldName}", correction.FieldName);
                    continue;
                }

                // Validate values
                if (string.IsNullOrEmpty(correction.NewValue))
                {
                    _logger?.Warning("Empty new value in correction for field: {FieldName}", correction.FieldName);
                    continue;
                }

                // Enrich with line number if missing
                if (correction.LineNumber <= 0)
                {
                    correction.LineNumber = FindLineNumber(correction.FieldName, originalText);
                }

                // Enrich context if missing but we have line text
                if (!correction.ContextLinesBefore.Any() && !correction.ContextLinesAfter.Any() && 
                    !string.IsNullOrEmpty(correction.LineText) && correction.LineNumber > 0)
                {
                    EnrichContextFromLineNumber(correction, originalText);
                }

                validatedCorrections.Add(correction);
            }

            return validatedCorrections;
        }

        /// <summary>
        /// Enriches correction with context lines based on line number
        /// </summary>
        /// <param name="correction">Correction to enrich</param>
        /// <param name="originalText">Original text</param>
        private void EnrichContextFromLineNumber(CorrectionResult correction, string originalText)
        {
            try
            {
                var lines = originalText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var lineIndex = correction.LineNumber - 1; // Convert to 0-based

                if (lineIndex >= 0 && lineIndex < lines.Length)
                {
                    // Add context lines before
                    var startIndex = Math.Max(0, lineIndex - 5);
                    for (int i = startIndex; i < lineIndex; i++)
                    {
                        correction.ContextLinesBefore.Add($"Line {i + 1}: {lines[i]}");
                    }

                    // Add context lines after
                    var endIndex = Math.Min(lines.Length - 1, lineIndex + 5);
                    for (int i = lineIndex + 1; i <= endIndex; i++)
                    {
                        correction.ContextLinesAfter.Add($"Line {i + 1}: {lines[i]}");
                    }

                    _logger?.Debug("Enriched correction for {FieldName} with {BeforeCount} before and {AfterCount} after context lines",
                        correction.FieldName, correction.ContextLinesBefore.Count, correction.ContextLinesAfter.Count);
                }
            }
            catch (Exception ex)
            {
                _logger?.Warning(ex, "Failed to enrich context for correction on field {FieldName}", correction.FieldName);
            }
        }

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
        private double GetDoubleValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetDouble(out var value)) return value;
                if (prop.TryGetDecimal(out var decimalValue)) return (double)decimalValue;
                if (prop.TryGetInt32(out var intValue)) return intValue;
            }
            return 0.0;
        }

        /// <summary>
        /// Gets boolean value from JSON element
        /// </summary>
        private bool GetBooleanValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;
                if (prop.TryGetBoolean(out var boolValue)) return boolValue;
            }
            return false;
        }

        /// <summary>
        /// Gets integer value from JSON element
        /// </summary>
        private int GetIntValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetInt32(out var value)) return value;
                if (prop.TryGetInt64(out var longValue)) return (int)longValue;
            }
            return 1; // Default for max_lines
        }

        #endregion
    }
}