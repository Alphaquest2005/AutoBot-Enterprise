using System;
using System.Collections.Generic;
using System.Globalization;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        private bool GetValue(string instance, Dictionary<(Fields field, string instance), string> values, Fields field,
            string methodName, int? fieldId,
            string formattedValue, out (Fields field, string instance) valueKey)
        {
            valueKey = (field, instance);
            if (values.ContainsKey(valueKey))
            {
                _logger.Verbose(
                    "{MethodName}: FieldId: {FieldId}, Instance: {Instance} already exists in values dictionary.",
                    methodName, fieldId, instance);
                bool onlyDistinct = this.OCR_Lines?.DistinctValues ?? false;
                if (onlyDistinct)
                {
                    _logger.Verbose(
                        "{MethodName}: DistinctValues is true. Skipping update for existing FieldId: {FieldId}, Instance: {Instance}.",
                        methodName, fieldId, instance);
                    return true;
                }

                // --- Handle Update/Append ---
                string existingValueStr = values[valueKey];
                string newValueStr = formattedValue; // Use current formatted value
                string finalValueStr = existingValueStr; // Default to existing

                switch (field.DataType)
                {
                    case "String":
                        _logger.Verbose(
                            "{MethodName}: Appending string value for FieldId: {FieldId}, Instance: {Instance}. Existing: '{Existing}', New: '{New}'",
                            methodName, fieldId, instance, existingValueStr, newValueStr);
                        finalValueStr = (existingValueStr + " " + newValueStr).Trim();
                        break;
                    case "Number":
                    case "Numeric":
                        _logger.Verbose(
                            "{MethodName}: Adding numeric value for FieldId: {FieldId}, Instance: {Instance}. Existing: '{Existing}', New: '{New}'",
                            methodName, fieldId, instance, existingValueStr, newValueStr);
                        double currentNum = 0, newNum = 0;
                        try
                        {
                            if (existingValueStr != null)
                                double.TryParse(existingValueStr, NumberStyles.Any,
                                    CultureInfo.InvariantCulture, out currentNum);
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning(ex,
                                "{MethodName}: Error parsing existing numeric value '{ExistingValue}' for append.",
                                methodName, existingValueStr);
                        }

                        try
                        {
                            double.TryParse(newValueStr, NumberStyles.Any, CultureInfo.InvariantCulture,
                                out newNum);
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning(ex,
                                "{MethodName}: Error parsing new numeric value '{NewValue}' for append.",
                                methodName, newValueStr);
                        }

                        finalValueStr = (currentNum + newNum).ToString(CultureInfo.InvariantCulture);
                        break;
                    case "Date":
                    case "English Date":
                        _logger.Verbose(
                            "{MethodName}: Overwriting Date/English Date value for existing FieldId: {FieldId}, Instance: {Instance}. Old: '{Existing}', New: '{New}'",
                            methodName, fieldId, instance, existingValueStr, newValueStr);
                        finalValueStr = newValueStr;
                        break;
                    default:
                        _logger.Warning(
                            "{MethodName}: Unhandled DataType '{DataType}' for existing FieldId: {FieldId}, Instance: {Instance}. Overwriting. Old: '{Existing}', New: '{New}'",
                            methodName, field.DataType, fieldId, instance, existingValueStr, newValueStr);
                        finalValueStr = newValueStr; // Default overwrite
                        break;
                }

                values[valueKey] = finalValueStr;
                _logger.Verbose(
                    "{MethodName}: Updated value for FieldId: {FieldId}, Instance: {Instance}. Final Value: '{FinalValue}'",
                    methodName, fieldId, instance, finalValueStr);
            }
            else // Key doesn't exist, add new
            {
                _logger.Verbose(
                    "{MethodName}: Adding new value for FieldId: {FieldId}, Instance: {Instance}. Value: '{Value}'",
                    methodName, fieldId, instance, formattedValue);
                values.Add(valueKey, formattedValue);
            }

            return false;
        }
    }
}