using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using System.Globalization; // Added for TryParse

namespace WaterNut.DataSpace;

public partial class Line
{
    // Logger instance is defined in the main Line.cs partial class file.
    // RegexTimeout constant is defined in the main Line.cs partial class file.

    private void FormatValues(int instance, MatchCollection matches,
        Dictionary<(Fields Fields, int Instance), string> values)
    {
        string methodName = nameof(FormatValues);
        int? lineId = this.OCR_Lines?.Id; // For logging context
        _logger.Verbose("Entering {MethodName} for LineId: {LineId}, Instance: {Instance}. Input Match Count: {MatchCount}",
            methodName, lineId, instance, matches?.Count ?? 0);

        // --- Input Validation ---
        if (matches == null)
        {
            _logger.Warning("{MethodName}: Called with null matches collection for LineId: {LineId}. Exiting.", methodName, lineId);
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null matches.", methodName, lineId);
            return;
        }
        if (values == null)
        {
            _logger.Error("{MethodName}: Called with null values dictionary for LineId: {LineId}. Cannot store results. Exiting.", methodName, lineId);
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null values dictionary.", methodName, lineId);
            return;
        }
        if (this.OCR_Lines?.Fields == null)
        {
            _logger.Warning("{MethodName}: Called with null OCR_Lines or Fields for LineId: {LineId}. Cannot process fields. Exiting.", methodName, lineId);
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null OCR_Lines/Fields.", methodName, lineId);
            return;
        }
        _logger.Verbose("{MethodName}: Input validation passed for LineId: {LineId}.", methodName, lineId);


        // --- Process Matches ---
        int matchIndex = -1;
        foreach (Match match in matches)
        {
            matchIndex++;
            _logger.Verbose("{MethodName}: Processing Match {MatchIndex}/{TotalMatches} for LineId: {LineId}, Instance: {Instance}", methodName, matchIndex + 1, matches.Count, lineId, instance);
            if (match == null || !match.Success)
            {
                _logger.Warning("{MethodName}: Skipping null or unsuccessful match at index {MatchIndex} for LineId: {LineId}", methodName, matchIndex, lineId);
                continue;
            }

            // --- Process Top-Level Fields for this Match ---
            int fieldIndex = -1;
            var topLevelFields = OCR_Lines.Fields.Where(x => x != null && x.ParentField == null).ToList();
            _logger.Verbose("{MethodName}: Match {MatchIndex}: Processing {FieldCount} top-level fields.", methodName, matchIndex + 1, topLevelFields.Count);

            foreach (var field in topLevelFields)
            {
                fieldIndex++;
                int? fieldId = field.Id;
                string fieldName = field.Field ?? "UnknownField";
                _logger.Verbose("{MethodName}: Match {MatchIndex}: Processing Field {FieldIndex}/{TotalFields} (Id: {FieldId}, Name: '{FieldName}') for Instance: {Instance}",
                    methodName, matchIndex + 1, fieldIndex + 1, topLevelFields.Count, fieldId, fieldName, instance);

                // --- Determine Initial Value ---
                string initialValue = field.FieldValue?.Value?.Trim(); // Use override if present
                bool usedOverride = initialValue != null;
                if (!usedOverride)
                {
                    string groupKey = field.Key;
                    if (match.Groups.ContainsKey(groupKey))
                    {
                        initialValue = match.Groups[groupKey]?.Value?.Trim();
                        _logger.Verbose("{MethodName}: FieldId: {FieldId} - Using Regex Group '{GroupKey}'. Initial Value: '{InitialValue}'", methodName, fieldId, groupKey, initialValue);
                    }
                    else
                    {
                        initialValue = null; // Explicitly null if group key doesn't exist
                        _logger.Warning("{MethodName}: FieldId: {FieldId} - Regex Group Key '{GroupKey}' not found in match. Initial Value set to null.", methodName, fieldId, groupKey);
                    }
                }
                else
                {
                    _logger.Verbose("{MethodName}: FieldId: {FieldId} - Using FieldValue override. Initial Value: '{InitialValue}'", methodName, fieldId, initialValue);
                }

                // --- Apply Formatting Regex ---
                string formattedValue = initialValue;
                if (field.FormatRegEx != null && formattedValue != null) // Only format if value exists and formats defined
                {
                    _logger.Verbose("{MethodName}: FieldId: {FieldId} - Applying {Count} format regex patterns to value '{CurrentValue}'...", methodName, fieldId, field.FormatRegEx.Count, formattedValue);
                    int formatIndex = 0;
                    foreach (var reg in field.FormatRegEx.OrderBy(x => x?.Id ?? int.MaxValue)) // Safe ordering
                    {
                        formatIndex++;
                        string pattern = reg?.RegEx?.RegEx;
                        string replacement = reg?.ReplacementRegEx?.RegEx ?? string.Empty;
                        int formatRegId = reg?.Id ?? -1;

                        if (reg?.RegEx == null || string.IsNullOrEmpty(pattern))
                        {
                            _logger.Warning("{MethodName}: Skipping null/empty format pattern {Index}/{Total} (FormatRegId: {FormatRegId}) for FieldId: {FieldId}", methodName, formatIndex, field.FormatRegEx.Count, formatRegId, fieldId);
                            continue;
                        }
                        if (reg.ReplacementRegEx == null)
                        {
                            _logger.Warning("{MethodName}: Format ReplacementRegEx is null for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Replacement will be empty string.", methodName, formatRegId, fieldId);
                        }

                        _logger.Verbose("{MethodName}: Applying Format {Index}/{Total} (Id: {FormatRegId}) - Pattern: '{Pattern}', Replacement: '{Replacement}'", methodName, formatIndex, field.FormatRegEx.Count, formatRegId, pattern, replacement);
                        try
                        {
                            bool isMultiLineFormat = this.OCR_Lines?.RegularExpressions?.MultiLine ?? false; // Use line's multiline setting? Or format's? Assuming line's for now.
                            RegexOptions options = (isMultiLineFormat ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                                  RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                            string valueBeforeFormat = formattedValue;
                            formattedValue = Regex.Replace(formattedValue, pattern, replacement, options, RegexTimeout).Trim();
                            _logger.Verbose("{MethodName}: Value after FormatRegId {FormatRegId}: '{FormattedValue}' (was: '{OriginalValue}')", methodName, formatRegId, formattedValue, valueBeforeFormat);
                        }
                        catch (RegexMatchTimeoutException timeoutEx)
                        {
                            _logger.Error(timeoutEx, "{MethodName}: Regex format replace timed out (>{TimeoutSeconds}s) for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Skipping this format.", methodName, RegexTimeout.TotalSeconds, formatRegId, fieldId);
                        }
                        catch (ArgumentException argEx)
                        {
                            _logger.Error(argEx, "{MethodName}: Invalid format regex pattern encountered for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Skipping this format.", methodName, formatRegId, fieldId);
                        }
                        catch (Exception formatEx)
                        {
                            _logger.Error(formatEx, "{MethodName}: Error applying format regex FormatRegId: {FormatRegId} for FieldId: {FieldId}. Skipping this format.", methodName, formatRegId, fieldId);
                        }
                    }
                    _logger.Verbose("{MethodName}: FieldId: {FieldId} - Finished applying format regex patterns. Final formatted value: '{FinalValue}'", methodName, fieldId, formattedValue);
                }
                else if (formattedValue == null)
                {
                    _logger.Verbose("{MethodName}: FieldId: {FieldId} - Initial value was null, skipping formatting.", methodName, fieldId);
                }
                else
                {
                    _logger.Verbose("{MethodName}: FieldId: {FieldId} - No format regex patterns defined.", methodName, fieldId);
                }


                // --- Store Value (Add/Update/Append) ---
                if (string.IsNullOrEmpty(formattedValue))
                {
                    _logger.Verbose("{MethodName}: Skipping FieldId: {FieldId} because formatted value is null or empty.", methodName, fieldId);
                    continue; // Skip to next field
                }

                var valueKey = (field, instance); // Key for the dictionary
                if (values.ContainsKey(valueKey))
                {
                    _logger.Verbose("{MethodName}: FieldId: {FieldId}, Instance: {Instance} already exists in values dictionary.", methodName, fieldId, instance);
                    bool onlyDistinct = this.OCR_Lines?.DistinctValues ?? false;
                    if (onlyDistinct)
                    {
                        _logger.Verbose("{MethodName}: DistinctValues is true. Skipping update for existing FieldId: {FieldId}, Instance: {Instance}.", methodName, fieldId, instance);
                        continue; // Skip update
                    }

                    // --- Handle Update/Append ---
                    string existingValueStr = values[valueKey];
                    string newValueStr = formattedValue; // Use current formatted value
                    string finalValueStr = existingValueStr; // Default to existing

                    switch (field.DataType)
                    {
                        case "String":
                            _logger.Verbose("{MethodName}: Appending string value for FieldId: {FieldId}, Instance: {Instance}. Existing: '{Existing}', New: '{New}'", methodName, fieldId, instance, existingValueStr, newValueStr);
                            finalValueStr = (existingValueStr + " " + newValueStr).Trim();
                            break;
                        case "Number":
                        case "Numeric":
                            _logger.Verbose("{MethodName}: Adding numeric value for FieldId: {FieldId}, Instance: {Instance}. Existing: '{Existing}', New: '{New}'", methodName, fieldId, instance, existingValueStr, newValueStr);
                            double currentNum = 0, newNum = 0;
                            try { if (existingValueStr != null) double.TryParse(existingValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNum); } catch (Exception ex) { _logger.Warning(ex, "{MethodName}: Error parsing existing numeric value '{ExistingValue}' for append.", methodName, existingValueStr); }
                            try { double.TryParse(newValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out newNum); } catch (Exception ex) { _logger.Warning(ex, "{MethodName}: Error parsing new numeric value '{NewValue}' for append.", methodName, newValueStr); }
                            finalValueStr = (currentNum + newNum).ToString(CultureInfo.InvariantCulture);
                            break;
                        case "Date":
                        case "English Date":
                            _logger.Verbose("{MethodName}: Overwriting Date/English Date value for existing FieldId: {FieldId}, Instance: {Instance}. Old: '{Existing}', New: '{New}'", methodName, fieldId, instance, existingValueStr, newValueStr);
                            finalValueStr = newValueStr;
                            break;
                        default:
                            _logger.Warning("{MethodName}: Unhandled DataType '{DataType}' for existing FieldId: {FieldId}, Instance: {Instance}. Overwriting. Old: '{Existing}', New: '{New}'", methodName, field.DataType, fieldId, instance, existingValueStr, newValueStr);
                            finalValueStr = newValueStr; // Default overwrite
                            break;
                    }
                    values[valueKey] = finalValueStr;
                    _logger.Verbose("{MethodName}: Updated value for FieldId: {FieldId}, Instance: {Instance}. Final Value: '{FinalValue}'", methodName, fieldId, instance, finalValueStr);
                }
                else // Key doesn't exist, add new
                {
                    _logger.Verbose("{MethodName}: Adding new value for FieldId: {FieldId}, Instance: {Instance}. Value: '{Value}'", methodName, fieldId, instance, formattedValue);
                    values.Add(valueKey, formattedValue);
                }


                // --- Process Child Fields Recursively ---
                if (field.ChildFields != null && field.ChildFields.Any())
                {
                    _logger.Verbose("{MethodName}: Processing {Count} ChildFields for FieldId: {FieldId}, Instance: {Instance}...", methodName, field.ChildFields.Count, fieldId, instance);
                    string currentValueForChild = values.TryGetValue(valueKey, out var currentVal) ? currentVal : ""; // Use potentially updated value
                    int childFieldIndex = 0;
                    foreach (var childField in field.ChildFields.Where(cf => cf != null)) // Safe iteration
                    {
                        childFieldIndex++;
                        _logger.Verbose("{MethodName}: Calling ReadChildField for ChildField {Index}/{Total} (Id: {ChildFieldId}) of Parent FieldId: {ParentFieldId}...", methodName, childFieldIndex, field.ChildFields.Count, childField.Id, fieldId);
                        // ReadChildField should handle its own logging
                        ReadChildField(childField, values, currentValueForChild);
                    }
                    _logger.Verbose("{MethodName}: Finished processing ChildFields for FieldId: {FieldId}, Instance: {Instance}", methodName, fieldId, instance);
                }
                _logger.Verbose("{MethodName}: Finished processing FieldId: {FieldId} for Match {MatchIndex}, Instance: {Instance}", methodName, fieldId, matchIndex + 1, instance);
            } // End foreach field
            _logger.Verbose("{MethodName}: Finished processing Match {MatchIndex} for LineId: {LineId}, Instance: {Instance}", methodName, matchIndex + 1, lineId, instance);
        } // End foreach match
        _logger.Verbose("Exiting {MethodName} for LineId: {LineId}, Instance: {Instance}. Final values count: {ValuesCount}", methodName, lineId, instance, values.Count);
    }

     // Assuming ReadChildField exists in another partial class part
     // private void ReadChildField(Fields childField, Dictionary<(Fields Fields, int Instance), string> values, string value) { ... }
}