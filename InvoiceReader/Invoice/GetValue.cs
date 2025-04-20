using System.Globalization;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Text.RegularExpressions; // Added for Regex.Replace
using Serilog; // Added
using System; // Added
using Core.Common; // Added for BetterExpando if needed elsewhere

namespace WaterNut.DataSpace;

public partial class Invoice
{
    // Logger instance is defined in the main Invoice.cs partial class file.

    // No logging added to this overload as it primarily calls the static GetValue
    private dynamic GetValue(
        KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z,
        (Fields fields, int instance) field)
    {
        // Added null check for inner dictionary
        if (z.Value == null)
        {
             _logger.Warning("GetValue called with null inner dictionary for Line: {LineKey}. Returning null.", z.Key);
             return null;
        }
        try
        {
            // Use safe navigation for Key.fields just in case Key itself is default
            var f = z.Value.FirstOrDefault(q => q.Key.fields != null && q.Key == field);
            // Check if field was found before calling GetValue
            if (f.Key.fields == null) return null;
            return GetValue(f); // Static GetValue handles logging
        }
        catch (Exception e)
        {
            // Log the exception before re-throwing
            var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
             _logger.Error(ex, "Error in GetValue(KeyValuePair, (Fields, int)) for Line: {LineKey}, Field: {FieldKey}", z.Key, field);
            throw ex;
        }
    }

    // No logging added to this overload as it primarily calls the static GetValue
    private dynamic GetValueByKey(
        KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z, string key)
    {
         // Added null check for inner dictionary
         if (z.Value == null)
         {
              _logger.Warning("GetValueByKey called with null inner dictionary for Line: {LineKey}, Key: {Key}. Returning null.", z.Key, key);
              return null;
         }
        try
        {
            // Use safe navigation for fields and Key property
            var f = z.Value.FirstOrDefault(q => q.Key.fields?.Key == key);
            // Check if field was found before calling GetValue
            if (f.Key.fields == null) return null;
            return GetValue(f); // Static GetValue handles logging
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
        int? invoiceId = this.OcrInvoices?.Id;
        _logger.Debug("Entering GetValue(string field) for Field: '{Field}', InvoiceId: {InvoiceId}", field, invoiceId);
        try
        {
            // Added null checks for safety during LINQ traversal
            var f = Lines
                .Where(x => x?.OCR_Lines?.Parts?.RecuringPart == null && x.Values != null) // Check line, parts, values
                .SelectMany(x => x.Values.Values) // Select inner dictionaries
                .Where(innerDict => innerDict != null) // Check inner dictionary not null
                .SelectMany(innerDict => innerDict) // Flatten KeyValuePairs from inner dictionaries
                .FirstOrDefault(kvp => kvp.Key.fields?.Field == field); // Safe navigation to Field

            // Check if a KeyValuePair with a non-null fields was found
            if (f.Key.fields == null)
            {
                 _logger.Warning("Field '{Field}' not found in non-recurring parts for InvoiceId: {InvoiceId}. Returning null.", field, invoiceId);
                 return null;
            }

             _logger.Debug("Found field '{Field}'. Calling static GetValue for conversion. InvoiceId: {InvoiceId}", field, invoiceId);
            // Static GetValue handles its own logging
            return GetValue(f);
        }
        catch (Exception ex)
        {
             _logger.Error(ex, "Error in GetValue(string field) for Field: '{Field}', InvoiceId: {InvoiceId}", field, invoiceId);
             // Decide whether to re-throw or return null based on requirements
             throw; // Re-throwing to maintain original behavior
        }
    }

    private static dynamic GetValue(KeyValuePair<(Fields fields, int instance), string> f)
    {
        // Extract details for logging, handle nulls safely
        int? fieldId = f.Key.fields?.Id;
        string fieldName = f.Key.fields?.Field ?? "UnknownField";
        string dataType = f.Key.fields?.DataType ?? "UnknownDataType";
        string rawValue = f.Value;
        int instance = f.Key.instance;

        _logger.Verbose("Entering static GetValue for FieldId: {FieldId} ('{FieldName}'), Instance: {Instance}, DataType: {DataType}, RawValue: '{RawValue}'",
            fieldId, fieldName, instance, dataType, rawValue);

        try
        {
            // Check fields null again just in case (though caller should prevent this)
            if (f.Key.fields == null)
            {
                 _logger.Warning("Static GetValue called with null fields in Key. Returning null.");
                 return null;
            }
             // Check raw value null - null might be valid for some types, handle in switch
             if (rawValue == null)
             {
                  _logger.Warning("Static GetValue called with null raw value for FieldId: {FieldId}. Handling based on DataType.", fieldId);
                  // Allow switch to handle null based on type (e.g., string is ok, numeric/date might fail or return default)
             }


            dynamic result = null;
            switch (dataType)
            {
                case "String":
                    _logger.Verbose("DataType is String. Returning value (null if input was null).");
                    result = rawValue; // Handles null input correctly
                    break;
                case "Numeric":
                case "Number":
                    _logger.Verbose("DataType is Numeric/Number. Attempting to parse.");
                    if (rawValue == null) {
                         _logger.Warning("Cannot parse null value as Numeric/Number for Field: {FieldName}. Returning 0.", fieldName);
                         result = (double)0; // Or return null if appropriate for numeric type
                         break;
                    }
                    // Clean the value more robustly
                    var val = Regex.Replace(rawValue, @"[$\s,]", "").Trim(); // Remove $, whitespace, commas
                    if (string.IsNullOrEmpty(val)) val = "0"; // Treat empty as 0 after cleaning

                    // Use NumberStyles.Any to handle various formats like currency, decimals, signs
                    if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double num)) // Use InvariantCulture
                    {
                        _logger.Verbose("Successfully parsed as double: {Value}", num);
                        result = num;
                    }
                    else
                    {
                         string errorMsg = $"{fieldName} cannot convert to {dataType} for Value:'{rawValue}' (Cleaned:'{val}')";
                         _logger.Error(errorMsg);
                         // Original code threw exception, maintaining that behavior
                         throw new ApplicationException(errorMsg);
                    }
                    break;

                case "Date":
                     _logger.Verbose("DataType is Date. Attempting DateTime.TryParse.");
                     if (rawValue == null) {
                          _logger.Warning("Cannot parse null value as Date for Field: {FieldName}. Returning DateTime.MinValue.", fieldName);
                          result = DateTime.MinValue; // Consistent with original fallback
                          break;
                     }
                    // Use InvariantCulture and allow default styles, Trim the value
                    if (DateTime.TryParse(rawValue.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                         _logger.Verbose("Successfully parsed as DateTime: {Value}", date);
                         result = date;
                    }
                    else
                    {
                         // Original code returned MinValue, maintaining that behavior
                         _logger.Warning("Could not parse '{RawValue}' as standard DateTime for Field: {FieldName}. Returning DateTime.MinValue.", rawValue, fieldName);
                         result = DateTime.MinValue;
                    }
                    break;
                case "English Date":
                     _logger.Verbose("DataType is English Date. Attempting DateTime.TryParseExact with multiple formats.");
                      if (rawValue == null) {
                          string errorMsgNull = $"{fieldName} cannot convert null value to {dataType}.";
                          _logger.Error(errorMsgNull);
                          throw new ApplicationException(errorMsgNull); // Throw for null on required parse
                     }
                     // Added more formats, including common US and ISO formats, and variations with/without leading zeros
                     var formatStrings = new List<string>()
                     {
                         // Common variations of dd/MM/yyyy
                         "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy",
                         // Common variations of MM/dd/yyyy (US)
                         "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy",
                         // ISO format
                         "yyyy-MM-dd", "yyyy-M-d",
                         // Common variations with dots
                         "dd.MM.yyyy", "d.M.yyyy",
                         // Common variations with month names
                         "MMMM d, yyyy", "MMM d, yyyy", "MMMM d yyyy", "MMM d yyyy",
                         "d MMMM yyyy", "d MMM yyyy",
                         // Month/Year only (if applicable)
                         "M/yyyy", "MM/yyyy"
                     };
                     bool parsed = false;
                     string trimmedValue = rawValue.Trim(); // Trim once
                     foreach (String formatString in formatStrings)
                     {
                         // Use InvariantCulture for consistency
                         if (DateTime.TryParseExact(trimmedValue, formatString, CultureInfo.InvariantCulture,
                                 DateTimeStyles.None,
                                 out DateTime edate))
                         {
                              _logger.Verbose("Successfully parsed as English Date using format '{Format}': {Value}", formatString, edate);
                              result = edate;
                              parsed = true;
                              break; // Exit loop on first successful parse
                         }
                     }

                     if (!parsed)
                     {
                          string errorMsg = $"{fieldName} cannot convert to {dataType} for Value:'{rawValue}' using specified formats.";
                          _logger.Error(errorMsg);
                          // Original code threw exception, maintaining that behavior
                          throw new ApplicationException(errorMsg);
                     }
                    break;
                default:
                     _logger.Warning("Unknown DataType '{DataType}' for Field: {FieldName}. Returning raw string value.", dataType, fieldName);
                     result = rawValue;
                    break;
            }
             _logger.Verbose("Exiting static GetValue for FieldId: {FieldId}. Returning value: {ResultValue} (Type: {ResultType})", fieldId, result, result?.GetType().Name ?? "null");
             return result;
        }
        catch (Exception e)
        {
             // Log exception before re-throwing
             _logger.Error(e, "Error during static GetValue conversion for FieldId: {FieldId} ('{FieldName}'), RawValue: '{RawValue}'", fieldId, fieldName, rawValue);
             throw; // Re-throw original exception
        }
    }
}