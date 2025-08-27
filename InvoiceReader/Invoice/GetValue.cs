using System.Globalization;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Text.RegularExpressions; // Added for Regex.Replace
using Serilog; // Added
using System; // Added
using Core.Common; // Added for BetterExpando if needed elsewhere

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Logger instance is defined in the main Template.cs partial class file.

        // No logging added to this overload as it primarily calls the static GetValue
        private dynamic GetValue(
            KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, string instance), string>> z,
            (Fields fields, string instance) field)
        {
            // Added null check for inner dictionary
            if (z.Value == null)
            {
                _logger.Warning("GetValue called with null inner dictionary for Line: {LineKey}. Returning null.",
                    z.Key);
                return null;
            }

            try
            {
                // Use safe navigation for Key.fields just in case Key itself is default
                var f = z.Value.FirstOrDefault(q => q.Key.fields != null && q.Key == field);
                // Check if field was found before calling GetValue
                if (f.Key.fields == null) return null;
                return GetValue(f, _logger); // Static GetValue handles logging
            }
            catch (Exception e)
            {
                // Log the exception before re-throwing
                var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
                _logger.Error(ex,
                    "Error in GetValue(KeyValuePair, (Fields, int)) for Line: {LineKey}, Field: {FieldKey}", z.Key,
                    field);
                throw ex;
            }
        }

        // No logging added to this overload as it primarily calls the static GetValue
        private dynamic GetValueByKey(
            KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, string instance), string>> z,
            string key)
        {
            // Added null check for inner dictionary
            if (z.Value == null)
            {
                _logger.Warning(
                    "GetValueByKey called with null inner dictionary for Line: {LineKey}, Key: {Key}. Returning null.",
                    z.Key, key);
                return null;
            }

            try
            {
                // Use safe navigation for fields and Key property
                var f = z.Value.FirstOrDefault(q => q.Key.fields?.Key == key);
                // Check if field was found before calling GetValue
                if (f.Key.fields == null) return null;
                return GetValue(f, _logger); // Static GetValue handles logging
            }
            catch (Exception e)
            {
                // Log the exception before re-throwing
                var ex = new ApplicationException($"Error Importing Line:{z.Key}, Key:'{key}' --- {e.Message}", e);
                _logger.Error(ex, "Error in GetValueByKey for Line: {LineKey}, Key: {Key}", z.Key, key);
                throw ex;
            }
        }

        private dynamic GetValue(string field)
        {
            int? invoiceId = this.OcrTemplates?.Id;
            _logger.Debug("Entering GetValue(string field) for Field: '{Field}', InvoiceId: {InvoiceId}", field,
                invoiceId);
            try
            {
                // Added null checks for safety during LINQ traversal
                var f = Lines
                    .Where(x => x?.OCR_Lines?.Parts?.RecuringPart == null &&
                                x.Values != null) // Check line, parts, values
                    .SelectMany(x => x.Values.Values) // Select inner dictionaries
                    .Where(innerDict => innerDict != null) // Check inner dictionary not null
                    .SelectMany(innerDict => innerDict) // Flatten KeyValuePairs from inner dictionaries
                    .FirstOrDefault(kvp => kvp.Key.Fields?.Field == field); // Safe navigation to Field

                // Check if a KeyValuePair with a non-null fields was found
                if (f.Key.Fields == null)
                {
                    _logger.Warning(
                        "Field '{Field}' not found in non-recurring parts for InvoiceId: {InvoiceId}. Returning null.",
                        field, invoiceId);
                    return null;
                }

                _logger.Debug("Found field '{Field}'. Calling static GetValue for conversion. InvoiceId: {InvoiceId}",
                    field, invoiceId);
                // Static GetValue handles its own logging
                return GetValue(f, _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetValue(string field) for Field: '{Field}', InvoiceId: {InvoiceId}", field,
                    invoiceId);
                // Decide whether to re-throw or return null based on requirements
                throw; // Re-throwing to maintain original behavior
            }
        }

        private static dynamic GetValue(KeyValuePair<(Fields fields, string instance), string> f, ILogger logger)
        {
            string methodName = "static " + nameof(GetValue);
            // Extract details for logging, handle nulls safely
            int? fieldId = f.Key.fields?.Id;
            string fieldName = f.Key.fields?.Field ?? "UnknownField";
            string dataType = f.Key.fields?.DataType ?? "UnknownDataType";
            string rawValue = f.Value;
            string instance = f.Key.instance;

            logger.Verbose(
                "Entering {MethodName} for FieldId: {FieldId} ('{FieldName}'), Instance: {Instance}, DataType: {DataType}, RawValue: '{RawValue}'",
                methodName, fieldId, fieldName, instance, dataType, rawValue);

            dynamic result = null; // Initialize result

            try
            {
                // --- Input Validation ---
                if (f.Key.fields == null)
                {
                    logger.Warning(
                        "{MethodName}: Called with null fields in Key for FieldId: {FieldId}. Returning null.",
                        methodName, fieldId);
                    logger.Verbose("Exiting {MethodName} for FieldId: {FieldId} (Null Fields). Returning null.",
                        methodName, fieldId);
                    return null;
                }

                if (rawValue == null)
                {
                    logger.Warning(
                        "{MethodName}: Called with null raw value for FieldId: {FieldId}. Handling based on DataType.",
                        methodName, fieldId);
                    // Allow switch to handle null based on type
                }

                // --- Type Conversion ---
                logger.Verbose("{MethodName}: Performing conversion based on DataType: {DataType}", methodName,
                    dataType);
                switch (dataType)
                {
                    case "String":
                        logger.Verbose("{MethodName}: DataType is String. Returning raw value.", methodName);
                        result = rawValue; // Handles null input correctly
                        break;

                    case "Numeric":
                    case "Number":
                        logger.Verbose("{MethodName}: DataType is Numeric/Number. Attempting to parse.", methodName);
                        if (rawValue == null)
                        {
                            logger.Warning(
                                "{MethodName}: Cannot parse null value as Numeric/Number for Field: {FieldName}. Returning default 0.",
                                methodName, fieldName);
                            result = (double)0; // Default value
                        }
                        else
                        {
                            var cleanedValue =
                                Regex.Replace(rawValue, @"[$\s,]", "").Trim(); // Remove $, whitespace, commas
                            logger.Verbose("{MethodName}: Cleaned numeric value: '{CleanedValue}' (from '{RawValue}')",
                                methodName, cleanedValue, rawValue);
                            if (string.IsNullOrEmpty(cleanedValue))
                            {
                                logger.Warning(
                                    "{MethodName}: Cleaned numeric value is empty for Field: {FieldName}. Returning default 0.",
                                    methodName, fieldName);
                                cleanedValue = "0"; // Treat empty as 0 after cleaning
                            }

                            if (double.TryParse(cleanedValue, NumberStyles.Any, CultureInfo.InvariantCulture,
                                    out double num))
                            {
                                logger.Verbose("{MethodName}: Successfully parsed as double: {Value}", methodName,
                                    num);
                                result = num;
                            }
                            else
                            {
                                string errorMsg =
                                    $"{fieldName} cannot convert to {dataType} for Value:'{rawValue}' (Cleaned:'{cleanedValue}')";
                                logger.Error("{MethodName}: {ErrorMessage}", methodName, errorMsg);
                                throw new ApplicationException(errorMsg); // Maintain original behavior
                            }
                        }

                        break;

                    case "Date":
                        logger.Verbose("{MethodName}: DataType is Date. Attempting DateTime.TryParse.", methodName);
                        if (rawValue == null)
                        {
                            logger.Warning(
                                "{MethodName}: Cannot parse null value as Date for Field: {FieldName}. Returning DateTime.MinValue.",
                                methodName, fieldName);
                            result = DateTime.MinValue; // Consistent with original fallback
                        }
                        else
                        {
                            string trimmedDate = rawValue.Trim();
                            if (DateTime.TryParse(trimmedDate, CultureInfo.InvariantCulture, DateTimeStyles.None,
                                    out DateTime date))
                            {
                                logger.Verbose("{MethodName}: Successfully parsed as DateTime: {Value}", methodName,
                                    date);
                                result = date;
                            }
                            else
                            {
                                logger.Warning(
                                    "{MethodName}: Could not parse '{RawValue}' as standard DateTime for Field: {FieldName}. Returning DateTime.MinValue.",
                                    methodName, rawValue, fieldName);
                                result = DateTime.MinValue; // Maintain original behavior
                            }
                        }

                        break;

                    case "English Date":
                        logger.Verbose(
                            "{MethodName}: DataType is English Date. Attempting DateTime.TryParseExact with multiple formats.",
                            methodName);
                        if (rawValue == null)
                        {
                            string errorMsgNull = $"{fieldName} cannot convert null value to {dataType}.";
                            logger.Error("{MethodName}: {ErrorMessage}", methodName, errorMsgNull);
                            throw new ApplicationException(errorMsgNull); // Maintain original behavior
                        }

                        var formatStrings = new List<string>()
                        {
                            /* ... formats ... */
                            "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy",
                            "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy",
                            "yyyy-MM-dd", "yyyy-M-d",
                            "dd.MM.yyyy", "d.M.yyyy",
                            "MMMM d, yyyy", "MMM d, yyyy", "MMMM d yyyy", "MMM d yyyy",
                            "d MMMM yyyy", "d MMM yyyy",
                            "M/yyyy", "MM/yyyy"
                        };
                        bool parsed = false;
                        string trimmedValue = rawValue.Trim();
                        logger.Verbose(
                            "{MethodName}: Attempting to parse '{TrimmedValue}' using {FormatCount} formats.",
                            methodName, trimmedValue, formatStrings.Count);

                        foreach (String formatString in formatStrings)
                        {
                            if (DateTime.TryParseExact(trimmedValue, formatString, CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out DateTime edate))
                            {
                                logger.Verbose(
                                    "{MethodName}: Successfully parsed as English Date using format '{Format}': {Value}",
                                    methodName, formatString, edate);
                                result = edate;
                                parsed = true;
                                break; // Exit loop on success
                            }
                            else
                            {
                                logger.Verbose("{MethodName}: Failed to parse using format '{Format}'.", methodName,
                                    formatString);
                            }
                        }

                        if (!parsed)
                        {
                            string errorMsg =
                                $"{fieldName} cannot convert to {dataType} for Value:'{rawValue}' using specified formats.";
                            logger.Error("{MethodName}: {ErrorMessage}", methodName, errorMsg);
                            throw new ApplicationException(errorMsg); // Maintain original behavior
                        }

                        break;

                    default:
                        logger.Warning(
                            "{MethodName}: Unknown DataType '{DataType}' for Field: {FieldName}. Returning raw string value.",
                            methodName, dataType, fieldName);
                        result = rawValue;
                        break;
                }

                logger.Verbose(
                    "Exiting {MethodName} for FieldId: {FieldId}. Returning value: {ResultValue} (Type: {ResultType})",
                    methodName, fieldId, result, result?.GetType().Name ?? "null");
                return result;
            }
            catch (Exception e)
            {
                // Log exception before re-throwing
                logger.Error(e,
                    "{MethodName}: Unhandled exception during conversion for FieldId: {FieldId} ('{FieldName}'), RawValue: '{RawValue}'",
                    methodName, fieldId, fieldName, rawValue);
                logger.Verbose("Exiting {MethodName} for FieldId: {FieldId} due to exception.", methodName, fieldId);
                throw; // Re-throw original exception
            }
        }
    }
}