// File: OCRCorrectionService/OCRErrorDetection.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Error Detection

        /// <summary>
        /// Detects OCR errors using comprehensive 4-stage validation
        /// </summary>
        private async Task<List<InvoiceError>> DetectInvoiceErrorsAsync(ShipmentInvoice invoice, string fileText)
        {
            try
            {
                var allErrors = new List<InvoiceError>();

                // 1. Header-level field validation (totals, supplier info)
                _logger.Information("Detecting header field errors for invoice {InvoiceNo}", invoice.InvoiceNo);
                var headerErrors = await this.DetectHeaderFieldErrorsAsync(invoice, fileText).ConfigureAwait(false);
                allErrors.AddRange(headerErrors);

                // 2. Product-level validation (prices, quantities, descriptions)
                _logger.Information("Detecting product-level errors for invoice {InvoiceNo}", invoice.InvoiceNo);
                var productErrors = await this.DetectProductErrorsAsync(invoice, fileText).ConfigureAwait(false);
                allErrors.AddRange(productErrors);

                // 3. Mathematical consistency validation
                _logger.Information("Validating mathematical consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
                var mathErrors = ValidateMathematicalConsistency(invoice);
                allErrors.AddRange(mathErrors);

                // 4. Cross-field validation (totals vs details)
                _logger.Information("Validating cross-field consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
                var crossFieldErrors = ValidateCrossFieldConsistency(invoice);
                allErrors.AddRange(crossFieldErrors);

                _logger.Information("Total errors detected: {ErrorCount} for invoice {InvoiceNo}",
                    allErrors.Count, invoice.InvoiceNo);

                return allErrors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error detecting invoice errors for {InvoiceNo}", invoice?.InvoiceNo);
                return new List<InvoiceError>();
            }
        }

        /// <summary>
        /// Detects errors in header-level fields using specialized prompt
        /// </summary>
        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAsync(ShipmentInvoice invoice, string fileText)
        {
            var prompt = CreateHeaderErrorDetectionPrompt(invoice, fileText);
            var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.Warning("Empty response from DeepSeek for header error detection");
                return new List<InvoiceError>();
            }

            return ParseErrorDetectionResponse(response);
        }

        /// <summary>
        /// Detects errors in product-level data using specialized prompt
        /// </summary>
        private async Task<List<InvoiceError>> DetectProductErrorsAsync(ShipmentInvoice invoice, string fileText)
        {
            if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
            {
                _logger.Information("No invoice details to validate for invoice {InvoiceNo}", invoice.InvoiceNo);
                return new List<InvoiceError>();
            }

            var prompt = CreateProductErrorDetectionPrompt(invoice, fileText);
            var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.Warning("Empty response from DeepSeek for product error detection");
                return new List<InvoiceError>();
            }

            return ParseErrorDetectionResponse(response);
        }

        /// <summary>
        /// Enhanced error detection response parsing with comprehensive logging and type handling
        /// </summary>
        private List<InvoiceError> ParseErrorDetectionResponse(string response)
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
                            // Continue processing other errors
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

        #endregion
    }
}