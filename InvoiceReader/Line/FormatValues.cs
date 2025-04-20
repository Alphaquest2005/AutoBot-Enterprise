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
        int? lineId = this.OCR_Lines?.Id; // For logging context
        _logger.Verbose("Entering FormatValues for LineId: {LineId}, Instance: {Instance}. Input Match Count: {MatchCount}",
            lineId, instance, matches?.Count ?? 0);

        // Null checks
        if (matches == null)
        {
            _logger.Warning("FormatValues called with null matches collection for LineId: {LineId}. Exiting.", lineId);
            return;
        }
        if (values == null)
        {
             _logger.Error("FormatValues called with null values dictionary for LineId: {LineId}. Cannot store results. Exiting.", lineId);
             return;
        }
         if (this.OCR_Lines?.Fields == null)
         {
              _logger.Warning("FormatValues called with null OCR_Lines or Fields for LineId: {LineId}. Cannot process fields. Exiting.", lineId);
              return;
         }

        int matchIndex = -1;
        foreach (Match match in matches)
        {
            matchIndex++;
            _logger.Verbose("Processing Match {MatchIndex} for LineId: {LineId}, Instance: {Instance}", matchIndex, lineId, instance);
            if (match == null || !match.Success)
            {
                 _logger.Warning("Skipping null or unsuccessful match at index {MatchIndex} for LineId: {LineId}", matchIndex, lineId);
                 continue;
            }

            // Iterate through top-level fields (ParentField == null)
            foreach (var field in OCR_Lines.Fields.Where(x => x != null && x.ParentField == null)) // Safe iteration
            {
                int? fieldId = field.Id;
                string fieldName = field.Field ?? "UnknownField";
                 _logger.Verbose("Processing FieldId: {FieldId} ('{FieldName}') for Match {MatchIndex}, Instance: {Instance}", fieldId, fieldName, matchIndex, instance);

                 // Determine initial value: FieldValue override or Regex group match
                 string initialValue = field.FieldValue?.Value?.Trim(); // Use override if present
                 bool usedOverride = initialValue != null;
                 if (!usedOverride)
                 {
                     // Safely get value from regex match group using field.Key
                     initialValue = match.Groups.ContainsKey(field.Key) ? match.Groups[field.Key]?.Value?.Trim() : null;
                      _logger.Verbose("FieldId: {FieldId} - Using Regex Group '{GroupKey}'. Initial Value: '{InitialValue}'", fieldId, field.Key, initialValue);
                 } else {
                      _logger.Verbose("FieldId: {FieldId} - Using FieldValue override. Initial Value: '{InitialValue}'", fieldId, initialValue);
                 }

                 // Apply formatting regex patterns
                 string formattedValue = initialValue;
                 if (field.FormatRegEx != null && formattedValue != null) // Only format if value exists
                 {
                      _logger.Verbose("FieldId: {FieldId} - Applying {Count} format regex patterns.", fieldId, field.FormatRegEx.Count);
                      foreach (var reg in field.FormatRegEx.OrderBy(x => x?.Id ?? int.MaxValue)) // Safe ordering
                      {
                          string pattern = reg?.RegEx?.RegEx;
                          string replacement = reg?.ReplacementRegEx?.RegEx ?? string.Empty;
                          int formatRegId = reg?.Id ?? -1;

                          if (reg?.RegEx == null || string.IsNullOrEmpty(pattern))
                          {
                               _logger.Warning("Skipping null/empty format pattern for FormatRegId: {FormatRegId}, FieldId: {FieldId}", formatRegId, fieldId);
                               continue;
                          }
                           if (reg.ReplacementRegEx == null)
                           {
                                _logger.Warning("Format ReplacementRegEx is null for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Replacement will be empty string.", formatRegId, fieldId);
                           }

                           _logger.Verbose("Applying FormatRegId: {FormatRegId} - Pattern: '{Pattern}', Replacement: '{Replacement}'", formatRegId, pattern, replacement);
                           try
                           {
                               // Determine RegexOptions based on OCR_Lines settings safely
                               bool isMultiLine = this.OCR_Lines?.RegularExpressions?.MultiLine ?? false; // Default to false if null
                               RegexOptions options = (isMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                                     RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                               formattedValue = Regex.Replace(formattedValue, pattern, replacement, options, RegexTimeout).Trim();
                                _logger.Verbose("Value after FormatRegId {FormatRegId}: '{FormattedValue}'", formatRegId, formattedValue);
                           }
                           catch (RegexMatchTimeoutException timeoutEx)
                           {
                                _logger.Error(timeoutEx, "Regex format replace timed out (>{TimeoutSeconds}s) for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Skipping this format.", RegexTimeout.TotalSeconds, formatRegId, fieldId);
                           }
                           catch (ArgumentException argEx) // Catch invalid regex patterns
                           {
                                _logger.Error(argEx, "Invalid format regex pattern encountered for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Skipping this format.", formatRegId, fieldId);
                           }
                           catch (Exception formatEx)
                           {
                                _logger.Error(formatEx, "Error applying format regex FormatRegId: {FormatRegId} for FieldId: {FieldId}. Skipping this format.", formatRegId, fieldId);
                           }
                      }
                 } else if (formattedValue == null) {
                      _logger.Verbose("FieldId: {FieldId} - Initial value was null, skipping formatting.", fieldId);
                 } else {
                      _logger.Verbose("FieldId: {FieldId} - No format regex patterns found.", fieldId);
                 }


                 // Skip if the formatted value is empty (it could be null if initialValue was null)
                 if (string.IsNullOrEmpty(formattedValue))
                 {
                      _logger.Verbose("Skipping FieldId: {FieldId} because formatted value is null or empty.", fieldId);
                      continue;
                 }

                 var valueKey = (field, instance); // Key for the dictionary

                 // Check if the field/instance combination already exists
                 if (values.ContainsKey(valueKey))
                 {
                      _logger.Verbose("FieldId: {FieldId}, Instance: {Instance} already exists in values dictionary.", fieldId, instance);
                      // Check distinct values flag (safe navigation)
                      bool onlyDistinct = this.OCR_Lines?.DistinctValues ?? false; // Default to false
                      if (onlyDistinct)
                      {
                           _logger.Verbose("DistinctValues is true. Skipping update for existing FieldId: {FieldId}, Instance: {Instance}.", fieldId, instance);
                           continue; // Skip if only distinct values are needed and key exists
                      }

                      // Handle update/append based on DataType
                      string existingValueStr = values[valueKey]; // Get existing value once
                      switch (field.DataType)
                      {
                          case "String":
                               _logger.Verbose("Appending string value for FieldId: {FieldId}, Instance: {Instance}.", fieldId, instance);
                               values[valueKey] = (existingValueStr + " " + formattedValue).Trim();
                                _logger.Verbose("Appended value for FieldId: {FieldId}, Instance: {Instance}. New Value: '{NewValue}'", fieldId, instance, values[valueKey]);
                              break;
                          case "Number":
                          case "Numeric":
                               _logger.Verbose("Adding numeric value for FieldId: {FieldId}, Instance: {Instance}.", fieldId, instance);
                               double currentNum = 0;
                               double newNum = 0;
                               // Safely parse existing and new values
                               try { if (existingValueStr != null) double.TryParse(existingValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNum); } catch (Exception ex) { _logger.Warning(ex, "Error parsing existing numeric value '{ExistingValue}' for append.", existingValueStr); }
                               try { double.TryParse(formattedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out newNum); } catch (Exception ex) { _logger.Warning(ex, "Error parsing new numeric value '{NewValue}' for append.", formattedValue); }
                               // Store as double, consistent with GetValue parsing
                               values[valueKey] = (currentNum + newNum).ToString(CultureInfo.InvariantCulture); // Convert back to string for dictionary
                                _logger.Verbose("Added value for FieldId: {FieldId}, Instance: {Instance}. New Value: '{NewValue}'", fieldId, instance, values[valueKey]);
                              break;
                          case "Date": // Overwrite Date if key exists? Original code implies overwrite.
                          case "English Date":
                               _logger.Verbose("Overwriting Date/English Date value for existing FieldId: {FieldId}, Instance: {Instance}.", fieldId, instance);
                               values[valueKey] = formattedValue;
                                _logger.Verbose("Overwrote value for FieldId: {FieldId}, Instance: {Instance}. New Value: '{NewValue}'", fieldId, instance, values[valueKey]);
                              break;
                          default:
                               _logger.Warning("Unhandled DataType '{DataType}' for existing FieldId: {FieldId}, Instance: {Instance}. Overwriting with new value.", field.DataType, fieldId, instance);
                               values[valueKey] = formattedValue; // Default overwrite
                                _logger.Verbose("Overwrote value for FieldId: {FieldId}, Instance: {Instance}. New Value: '{NewValue}'", fieldId, instance, values[valueKey]);
                              break;
                      }
                 }
                 else // Key doesn't exist, add new
                 {
                      _logger.Verbose("Adding new value for FieldId: {FieldId}, Instance: {Instance}. Value: '{Value}'", fieldId, instance, formattedValue);
                      values.Add(valueKey, formattedValue);
                 }


                 // Process Child Fields recursively
                 if (field.ChildFields != null && field.ChildFields.Any())
                 {
                      _logger.Verbose("Processing {Count} ChildFields for FieldId: {FieldId}, Instance: {Instance}", field.ChildFields.Count, fieldId, instance);
                      // Aggregate the current value (potentially updated/appended) to pass to child fields
                      // Use TryGetValue for safer access
                      string currentValueForChild = values.TryGetValue(valueKey, out var currentVal) ? currentVal : "";
                      foreach (var childField in field.ChildFields.Where(cf => cf != null)) // Safe iteration
                      {
                           // ReadChildField should handle its own logging
                           ReadChildField(childField, values, currentValueForChild);
                      }
                 }
            } // End foreach field
        } // End foreach match
         _logger.Verbose("Exiting FormatValues for LineId: {LineId}, Instance: {Instance}", lineId, instance);
    }

     // Assuming ReadChildField exists in another partial class part
     // private void ReadChildField(Fields childField, Dictionary<(Fields Fields, int Instance), string> values, string value) { ... }
}