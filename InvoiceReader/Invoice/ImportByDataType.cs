using OCR.Business.Entities;
using System.Collections.Generic; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for FirstOrDefault
using System.Globalization; // Added for CultureInfo

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Logger instance is defined in the main Template.cs partial class file.

        private void ImportByDataType(KeyValuePair<(Fields fields, int instance), string> field,
            IDictionary<string, object> ditm,
            KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, string instance), string>> value)
        {
            // Extract details for logging, handle nulls safely
            int? fieldId = field.Key.fields?.Id;
            string fieldName = field.Key.fields?.Field ?? "UnknownField";
            string fieldKey = field.Key.fields?.Key ?? "UnknownKey";
            string dataType = field.Key.fields?.DataType ?? "UnknownDataType";
            bool appendValues = field.Key.fields?.AppendValues ?? false;
            int instance = field.Key.instance;
            var lineKey = value.Key; // (lineNumber, section)

            _logger.Verbose(
                "Entering ImportByDataType for FieldId: {FieldId} ('{FieldName}'), Instance: {Instance}, DataType: {DataType}, Append: {Append}, LineKey: {LineKey}",
                fieldId, fieldName, instance, dataType, appendValues, lineKey);

            // Null checks for critical inputs
            if (field.Key.fields == null || string.IsNullOrEmpty(fieldName)) // Also check FieldName as it's used as key
            {
                _logger.Error("ImportByDataType cannot proceed: fields object or Field name in key is null/empty.");
                return;
            }

            if (ditm == null)
            {
                _logger.Error("ImportByDataType cannot proceed: target dictionary 'ditm' is null. FieldId: {FieldId}",
                    fieldId);
                return;
            }

            // value (KeyValuePair) itself is unlikely null if called from loop, but inner dictionary could be
            if (value.Value == null)
            {
                _logger.Warning(
                    "ImportByDataType called with null inner dictionary in 'value' parameter for FieldId: {FieldId}, LineKey: {LineKey}. GetValueByKey will likely return null.",
                    fieldId, lineKey);
            }


            try
            {
                // Ensure the key exists in ditm before trying to access/update it, initialize if necessary
                if (!ditm.ContainsKey(fieldName))
                {
                    _logger.Debug(
                        "Field '{FieldName}' not found in target dictionary 'ditm'. Initializing based on DataType: {DataType}.",
                        fieldName, dataType);
                    // Initialize based on type to avoid errors later
                    switch (dataType)
                    {
                        case "String":
                            ditm[fieldName] = ""; // Initialize as empty string
                            break;
                        case "Number":
                        case "Numeric":
                            ditm[fieldName] = (double)0; // Initialize as 0.0
                            break;
                        case "Date":
                        case "English Date":
                            ditm[fieldName] = null; // Or DateTime.MinValue;
                            break;
                        default:
                            ditm[fieldName] = null; // Initialize as null for other types
                            break;
                    }
                }

                // Get the value using the helper (which handles its own logging)
                _logger.Verbose("Calling GetValueByKey for FieldKey: {FieldKey}, LineKey: {LineKey}", fieldKey,
                    lineKey);
                dynamic retrievedValue = GetValueByKey(value, fieldKey); // GetValueByKey logs its own details
                _logger.Verbose("GetValueByKey returned: {Value} (Type: {Type}) for FieldKey: {FieldKey}",
                    retrievedValue, retrievedValue?.GetType().Name ?? "null", fieldKey);


                switch (dataType)
                {
                    case "String":
                        _logger.Verbose("DataType is String. Concatenating value.");
                        string existingString = (ditm[fieldName]?.ToString() ?? "").Trim(); // Safe access and trim
                        string newString = (retrievedValue?.ToString() ?? "").Trim(); // Safe access and trim
                        // Only concatenate if newString is not empty, handle initial empty state
                        ditm[fieldName] = string.IsNullOrEmpty(existingString)
                            ? newString
                            : (existingString + " " + newString).Trim();
                        _logger.Verbose("Updated ditm['{FieldName}'] to: '{Value}'", fieldName, ditm[fieldName]);
                        break;
                    case "Number":
                    case "Numeric":
                        _logger.Verbose("DataType is Numeric/Number. AppendValues: {Append}", appendValues);
                        double currentValue = 0;
                        double newValue = 0;
                        bool currentParsed = false;
                        bool newParsed = false;

                        // Safely convert current value in dictionary
                        try
                        {
                            if (ditm[fieldName] is double currentDbl)
                            {
                                currentValue = currentDbl;
                                currentParsed = true;
                            }
                            else if (ditm[fieldName] != null)
                            {
                                // Use invariant culture for reliable parsing regardless of system settings
                                currentParsed = double.TryParse(ditm[fieldName].ToString(), NumberStyles.Any,
                                    CultureInfo.InvariantCulture, out currentValue);
                            }
                            else
                            {
                                currentParsed = true; // Treat null as 0.0
                            }
                        }
                        catch (Exception convEx)
                        {
                            _logger.Warning(convEx,
                                "Could not convert existing ditm value '{ExistingValue}' to double for Field '{FieldName}'. Using 0.",
                                ditm[fieldName], fieldName);
                        }

                        if (!currentParsed) currentValue = 0; // Ensure it's 0 if parse failed

                        // Safely convert retrieved value
                        try
                        {
                            if (retrievedValue is double newDbl)
                            {
                                newValue = newDbl;
                                newParsed = true;
                            }
                            else if (retrievedValue != null)
                            {
                                newParsed = double.TryParse(retrievedValue.ToString(), NumberStyles.Any,
                                    CultureInfo.InvariantCulture, out newValue);
                            }
                            else
                            {
                                newParsed = true; // Treat null as 0.0
                            }
                        }
                        catch (Exception convEx)
                        {
                            _logger.Warning(convEx,
                                "Could not convert retrieved value '{RetrievedValue}' to double for Field '{FieldName}'. Using 0.",
                                retrievedValue, fieldName);
                        }

                        if (!newParsed) newValue = 0; // Ensure it's 0 if parse failed


                        // Apply logic based on AppendValues flag
                        if (appendValues)
                        {
                            // The original code had complex logic involving string comparison which seemed incorrect.
                            // Assuming the intent is always to ADD when append is true.
                            _logger.Verbose(
                                "AppendValues is true. Adding new value {NewValue} to existing {CurrentValue} for Field '{FieldName}'.",
                                newValue, currentValue, fieldName);
                            ditm[fieldName] = currentValue + newValue;
                        }
                        else // appendValues is false
                        {
                            // Corrected logic: Replace the value if append is false.
                            _logger.Verbose(
                                "AppendValues is false. Replacing existing value {CurrentValue} with new value {NewValue} for Field '{FieldName}'.",
                                currentValue, newValue, fieldName);
                            ditm[fieldName] = newValue; // Replace
                        }

                        _logger.Verbose("Updated ditm['{FieldName}'] to: {Value}", fieldName, ditm[fieldName]);
                        break;
                    default: // Includes Date, English Date, and any unknown types
                        _logger.Verbose("DataType is '{DataType}'. Assigning retrieved value directly.", dataType);
                        // Assign directly. If retrievedValue is null, the dictionary value becomes null.
                        // If it's a specific type like DateTime, it remains that type.
                        ditm[fieldName] = retrievedValue;
                        _logger.Verbose("Updated ditm['{FieldName}'] to: {Value} (Type: {Type})", fieldName,
                            ditm[fieldName], ditm[fieldName]?.GetType().Name ?? "null");
                        break;
                }

                _logger.Verbose("Exiting ImportByDataType for FieldId: {FieldId}", fieldId);
            }
            catch (Exception e)
            {
                // Log exception before re-throwing
                _logger.Error(e,
                    "Error during ImportByDataType for FieldId: {FieldId} ('{FieldName}'), Instance: {Instance}, LineKey: {LineKey}",
                    fieldId, fieldName, instance, lineKey);
                throw; // Re-throw original exception
            }
        }
    }
}