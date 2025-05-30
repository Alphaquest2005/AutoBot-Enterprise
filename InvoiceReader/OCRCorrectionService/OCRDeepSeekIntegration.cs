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
    /// DeepSeek integration for OCR correction service
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region DeepSeek Response Processing

        /// <summary>
        /// Processes DeepSeek response and creates correction results
        /// </summary>
        /// <param name="deepSeekResponse">Raw DeepSeek response</param>
        /// <param name="originalText">Original OCR text</param>
        /// <returns>List of correction results</returns>
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

                // Extract corrections from response
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
        /// Extracts corrections from parsed DeepSeek response
        /// </summary>
        /// <param name="responseData">Parsed JSON response</param>
        /// <param name="originalText">Original OCR text</param>
        /// <returns>List of corrections</returns>
        private List<CorrectionResult> ExtractCorrectionsFromResponse(JsonElement responseData, string originalText)
        {
            var corrections = new List<CorrectionResult>();

            try
            {
                // Handle different response formats
                if (responseData.TryGetProperty("corrections", out var correctionsArray))
                {
                    corrections.AddRange(ProcessCorrectionsArray(correctionsArray, originalText));
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
        /// Processes corrections array format
        /// </summary>
        /// <param name="correctionsArray">JSON array of corrections</param>
        /// <param name="originalText">Original text</param>
        /// <returns>List of corrections</returns>
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
        /// Creates a correction result from a JSON element
        /// </summary>
        /// <param name="element">JSON element</param>
        /// <param name="originalText">Original text</param>
        /// <returns>Correction result or null</returns>
        private CorrectionResult CreateCorrectionFromElement(JsonElement element, string originalText)
        {
            try
            {
                var fieldName = element.TryGetProperty("field", out var fieldProp) ? fieldProp.GetString() : null;
                var oldValue = element.TryGetProperty("old_value", out var oldProp) ? oldProp.GetString() : null;
                var newValue = element.TryGetProperty("new_value", out var newProp) ? newProp.GetString() : null;
                var confidence = element.TryGetProperty("confidence", out var confProp) ? confProp.GetDouble() : 0.8;
                var reasoning = element.TryGetProperty("reasoning", out var reasonProp) ? reasonProp.GetString() : null;
                var lineNumber = element.TryGetProperty("line_number", out var lineProp) ? lineProp.GetInt32() : 0;

                if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(newValue))
                    return null;

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
                    Success = true,
                    Confidence = confidence,
                    CorrectionType = DetermineCorrectionType(oldValue, newValue),
                    Reasoning = reasoning,
                    LineNumber = lineNumber > 0 ? lineNumber : FindLineNumber(fieldName, originalText)
                };
            }
            catch (Exception ex)
            {
                _logger?.Warning(ex, "Failed to create correction from JSON element");
                return null;
            }
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
        /// <returns>Correction type</returns>
        private string DetermineCorrectionType(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(oldValue))
                return "Extraction";

            if (string.IsNullOrEmpty(newValue))
                return "Removal";

            // Check for format corrections
            var oldNormalized = Regex.Replace(oldValue, @"[\s\$,\-]", "");
            var newNormalized = Regex.Replace(newValue, @"[\s\$,\-]", "");

            if (string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase))
                return "FieldFormat";

            return "ValueCorrection";
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

                validatedCorrections.Add(correction);
            }

            return validatedCorrections;
        }

        #endregion
    }
}
