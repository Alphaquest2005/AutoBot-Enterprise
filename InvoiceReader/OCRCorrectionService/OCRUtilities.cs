// File: OCRCorrectionService/OCRUtilities.cs
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Helper Methods

        /// <summary>
        /// Cleans text for analysis
        /// </summary>
        internal string CleanTextForAnalysis(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var cleaned = Regex.Replace(text, @"-{30,}[^-]*-{30,}", "", RegexOptions.Multiline);

            if (cleaned.Length > 8000)
                cleaned = cleaned.Substring(0, 8000) + "...[truncated]";

            return cleaned;
        }

        /// <summary>
        /// Cleans JSON response
        /// </summary>
        internal string CleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            var cleaned = Regex.Replace(jsonResponse, @"```json|```|'''|\uFEFF", string.Empty, RegexOptions.IgnoreCase);

            var startIndex = cleaned.IndexOf('{');
            var endIndex = cleaned.LastIndexOf('}');

            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                _logger.Warning("No valid JSON boundaries detected in response");
                return string.Empty;
            }

            return cleaned.Substring(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>
        /// Parses corrected value to appropriate type
        /// </summary>
        internal object ParseCorrectedValue(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var cleanValue = value.Replace("$", "").Replace(",", "").Trim();

            if (IsNumericField(fieldName))
            {
                if (decimal.TryParse(cleanValue, out var decimalValue))
                    return decimalValue;
            }

            return value;
        }

        /// <summary>
        /// Determines if a field should contain numeric values
        /// </summary>
        internal bool IsNumericField(string fieldName)
        {
            var numericFields = new[] {
                "invoicetotal", "subtotal", "totalinternalfreight",
                "totalothercost", "totalinsurance", "totaldeduction",
                "quantity", "cost", "totalcost", "discount"
            };
            return Array.Exists(numericFields, f => f.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets current field value for logging
        /// </summary>
        internal object GetCurrentFieldValue(ShipmentInvoice invoice, string fieldName)
        {
            return fieldName.ToLower() switch
            {
                "invoicetotal" => invoice.InvoiceTotal,
                "subtotal" => invoice.SubTotal,
                "totalinternalfreight" => invoice.TotalInternalFreight,
                "totalothercost" => invoice.TotalOtherCost,
                "totalinsurance" => invoice.TotalInsurance,
                "totaldeduction" => invoice.TotalDeduction,
                "invoiceno" => invoice.InvoiceNo,
                "suppliername" => invoice.SupplierName,
                "currency" => invoice.Currency,
                _ => null
            };
        }

        /// <summary>
        /// Truncates text for logging
        /// </summary>
        internal string TruncateForLog(string text, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// Enhanced error detection response parsing (no duplicates)
        /// </summary>
        internal List<InvoiceError> ParseErrorDetectionResponse(string response)
        {
            try
            {
                var cleanJson = CleanJsonResponse(response);
                if (string.IsNullOrWhiteSpace(cleanJson))
                {
                    _logger.Warning("No valid JSON found in error detection response");
                    return new List<InvoiceError>();
                }

                _logger.Debug("Parsing error detection response JSON: {Json}", TruncateForLog(cleanJson, 1000));

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                var errors = new List<InvoiceError>();

                if (root.TryGetProperty("errors", out var errorsElement))
                {
                    _logger.Information("Found errors array with {Count} elements", errorsElement.GetArrayLength());

                    int errorIndex = 0;
                    foreach (var errorElement in errorsElement.EnumerateArray())
                    {
                        errorIndex++;
                        try
                        {
                            _logger.Debug("Processing error element {Index}: {Element}", errorIndex, errorElement.GetRawText());

                            var error = new InvoiceError();

                            // Parse each field with detailed logging
                            error.Field = GetStringValueWithLogging(errorElement, "field", errorIndex);
                            error.ExtractedValue = GetStringValueWithLogging(errorElement, "extracted_value", errorIndex);
                            error.CorrectValue = GetStringValueWithLogging(errorElement, "correct_value", errorIndex);
                            error.Confidence = GetDoubleValueWithLogging(errorElement, "confidence", errorIndex);
                            error.ErrorType = GetStringValueWithLogging(errorElement, "error_type", errorIndex);
                            error.Reasoning = GetStringValueWithLogging(errorElement, "reasoning", errorIndex);
                            error.LineText = GetStringValueWithLogging(errorElement, "line_text", errorIndex);
                            error.LineNumber = GetIntValueWithLogging(errorElement, "line_number", errorIndex);
                            error.RequiresMultilineRegex = GetBooleanValueWithLogging(errorElement, "requires_multiline_regex", errorIndex);

                            // Parse context lines
                            error.ContextLinesBefore = ParseContextLinesArray(errorElement, "context_lines_before", errorIndex);
                            error.ContextLinesAfter = ParseContextLinesArray(errorElement, "context_lines_after", errorIndex);

                            // Validate required fields
                            if (!string.IsNullOrEmpty(error.Field) && !string.IsNullOrEmpty(error.CorrectValue))
                            {
                                errors.Add(error);
                                _logger.Information("Successfully parsed error {Index}: Field={Field}, Type={ErrorType}, Confidence={Confidence:P0}",
                                    errorIndex, error.Field, error.ErrorType, error.Confidence);
                            }
                            else
                            {
                                _logger.Warning("Skipping incomplete error {Index}: Field='{Field}', CorrectValue='{CorrectValue}'",
                                    errorIndex, error.Field ?? "NULL", error.CorrectValue ?? "NULL");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Failed to parse error element {Index}. Raw JSON: {Element}",
                                errorIndex, errorElement.GetRawText());
                        }
                    }
                }
                else
                {
                    _logger.Warning("No 'errors' property found in response. Available properties: {Properties}",
                        string.Join(", ", root.EnumerateObject().Select(p => p.Name)));
                }

                _logger.Debug("Successfully parsed {Count} errors from response", errors.Count);
                return errors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error parsing error detection response. Raw response: {Response}", TruncateForLog(response));
                return new List<InvoiceError>();
            }
        }

        /// <summary>
        /// Gets string value with detailed logging of data types and conversion issues
        /// </summary>
        private string GetStringValueWithLogging(JsonElement element, string propertyName, int errorIndex)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    _logger.Debug("Property '{PropertyName}' not found in error {Index}", propertyName, errorIndex);
                    return null;
                }

                _logger.Debug("Error {Index}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}",
                    errorIndex, propertyName, prop.ValueKind, TruncateForLog(prop.GetRawText(), 100));

                switch (prop.ValueKind)
                {
                    case JsonValueKind.String:
                        var stringValue = prop.GetString();
                        _logger.Debug("Extracted string value for {PropertyName}: '{Value}'", propertyName, stringValue);
                        return stringValue;

                    case JsonValueKind.Number:
                        var numberAsString = prop.GetRawText();
                        _logger.Warning("Expected string for '{PropertyName}' but got number: {Value}. Converting to string.",
                            propertyName, numberAsString);
                        return numberAsString;

                    case JsonValueKind.True:
                        _logger.Warning("Expected string for '{PropertyName}' but got boolean: true. Converting to string.", propertyName);
                        return "true";

                    case JsonValueKind.False:
                        _logger.Warning("Expected string for '{PropertyName}' but got boolean: false. Converting to string.", propertyName);
                        return "false";

                    case JsonValueKind.Null:
                        _logger.Debug("Property '{PropertyName}' is null", propertyName);
                        return null;

                    case JsonValueKind.Array:
                        var arrayValue = prop.GetRawText();
                        _logger.Warning("Expected string for '{PropertyName}' but got array: {Value}. Using raw JSON.",
                            propertyName, TruncateForLog(arrayValue, 100));
                        return arrayValue;

                    case JsonValueKind.Object:
                        var objectValue = prop.GetRawText();
                        _logger.Warning("Expected string for '{PropertyName}' but got object: {Value}. Using raw JSON.",
                            propertyName, TruncateForLog(objectValue, 100));
                        return objectValue;

                    default:
                        var defaultValue = prop.GetRawText();
                        _logger.Warning("Unexpected JSON type {ValueKind} for '{PropertyName}': {Value}. Using raw text.",
                            prop.ValueKind, propertyName, defaultValue);
                        return defaultValue;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting string value for property '{PropertyName}' in error {Index}",
                    propertyName, errorIndex);
                return null;
            }
        }

        /// <summary>
        /// Gets double value with detailed logging of data types and conversion issues
        /// </summary>
        private double GetDoubleValueWithLogging(JsonElement element, string propertyName, int errorIndex)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    _logger.Debug("Property '{PropertyName}' not found in error {Index}", propertyName, errorIndex);
                    return 0.0;
                }

                _logger.Debug("Error {Index}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}",
                    errorIndex, propertyName, prop.ValueKind, prop.GetRawText());

                switch (prop.ValueKind)
                {
                    case JsonValueKind.Number:
                        if (prop.TryGetDouble(out var doubleValue))
                        {
                            _logger.Debug("Extracted double value for {PropertyName}: {Value}", propertyName, doubleValue);
                            return doubleValue;
                        }
                        if (prop.TryGetDecimal(out var decimalValue))
                        {
                            var convertedValue = (double)decimalValue;
                            _logger.Debug("Converted decimal to double for {PropertyName}: {Value}", propertyName, convertedValue);
                            return convertedValue;
                        }
                        if (prop.TryGetInt32(out var intValue))
                        {
                            _logger.Debug("Converted int to double for {PropertyName}: {Value}", propertyName, intValue);
                            return intValue;
                        }
                        if (prop.TryGetInt64(out var longValue))
                        {
                            _logger.Debug("Converted long to double for {PropertyName}: {Value}", propertyName, longValue);
                            return longValue;
                        }
                        break;

                    case JsonValueKind.String:
                        var stringValue = prop.GetString();
                        if (double.TryParse(stringValue, out var parsedValue))
                        {
                            _logger.Warning("Expected number for '{PropertyName}' but got string: '{Value}'. Successfully parsed as {ParsedValue}.",
                                propertyName, stringValue, parsedValue);
                            return parsedValue;
                        }
                        _logger.Error("Expected number for '{PropertyName}' but got unparseable string: '{Value}'. Using 0.0.",
                            propertyName, stringValue);
                        break;

                    case JsonValueKind.Null:
                        _logger.Debug("Property '{PropertyName}' is null, using 0.0", propertyName);
                        break;

                    default:
                        _logger.Warning("Expected number for '{PropertyName}' but got {ValueKind}: {Value}. Using 0.0.",
                            propertyName, prop.ValueKind, prop.GetRawText());
                        break;
                }

                return 0.0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting double value for property '{PropertyName}' in error {Index}",
                    propertyName, errorIndex);
                return 0.0;
            }
        }

        /// <summary>
        /// Gets integer value with logging
        /// </summary>
        private int GetIntValueWithLogging(JsonElement element, string propertyName, int errorIndex)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    _logger.Debug("Property '{PropertyName}' not found in error {Index}", propertyName, errorIndex);
                    return 0;
                }

                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var intValue))
                {
                    return intValue;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting int value for property '{PropertyName}' in error {Index}",
                    propertyName, errorIndex);
                return 0;
            }
        }

        /// <summary>
        /// Gets boolean value with logging
        /// </summary>
        private bool GetBooleanValueWithLogging(JsonElement element, string propertyName, int errorIndex)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    _logger.Debug("Property '{PropertyName}' not found in error {Index}", propertyName, errorIndex);
                    return false;
                }

                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error extracting boolean value for property '{PropertyName}' in error {Index}",
                    propertyName, errorIndex);
                return false;
            }
        }

        /// <summary>
        /// Parses context lines array from JSON
        /// </summary>
        private List<string> ParseContextLinesArray(JsonElement element, string propertyName, int errorIndex)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop) || prop.ValueKind != JsonValueKind.Array)
                {
                    return new List<string>();
                }

                var contextLines = new List<string>();
                foreach (var item in prop.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        contextLines.Add(item.GetString());
                    }
                }

                _logger.Debug("Parsed {Count} context lines for {PropertyName} in error {Index}",
                    contextLines.Count, propertyName, errorIndex);

                return contextLines;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error parsing context lines for property '{PropertyName}' in error {Index}",
                    propertyName, errorIndex);
                return new List<string>();
            }
        }

        #endregion
    }
}