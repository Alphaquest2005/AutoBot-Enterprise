using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using System.Globalization; // Added for CultureInfo if needed by GetValue

namespace WaterNut.DataSpace;

public partial class Line
{
    // Assuming _logger and RegexTimeout exist from other partial parts
    // private static readonly ILogger _logger = Log.ForContext<Line>();
    // private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

    private void ReadChildField(Fields childField, Dictionary<(Fields fields, int instance), string> values,
        string strValue)
    {
        int? lineId = this.OCR_Lines?.Id; // For logging context
        int? childFieldId = childField?.Id;
        string childFieldName = childField?.Field ?? "UnknownChildField";
        _logger.Verbose("Entering ReadChildField for LineId: {LineId}, ChildFieldId: {ChildFieldId} ('{ChildFieldName}') based on ParentValue: '{ParentValue}'",
            lineId, childFieldId, childFieldName, strValue);

        // Null checks for critical inputs
        if (childField?.Lines?.RegularExpressions == null)
        {
             _logger.Warning("Cannot read child field {ChildFieldId}: ChildField, Lines, or RegularExpressions is null.", childFieldId);
             return;
        }
         if (values == null)
         {
              _logger.Error("Cannot read child field {ChildFieldId}: values dictionary is null.", childFieldId);
              return;
         }
          if (strValue == null) // Check parent value
         {
              _logger.Warning("Cannot read child field {ChildFieldId}: Input string value (strValue from parent) is null.", childFieldId);
              return;
         }


        try
        {
            string pattern = childField.Lines.RegularExpressions.RegEx;
            if (string.IsNullOrEmpty(pattern))
            {
                 _logger.Warning("ChildField {ChildFieldId} regex pattern is null or empty. Skipping match.", childFieldId);
                 return;
            }

            // Determine RegexOptions based on child field's Lines settings
            bool isMultiLine = childField.Lines.RegularExpressions.MultiLine ?? false; // Default to false
            RegexOptions options = (isMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                  RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

             _logger.Verbose("Attempting Regex.Match for ChildFieldId: {ChildFieldId}, Pattern: '{Pattern}', Options: {Options}", childFieldId, pattern, options);
             // Execute regex match with timeout
             Match match = Regex.Match(strValue.Trim(), pattern, options, RegexTimeout);

            if (!match.Success)
            {
                 _logger.Verbose("No regex match found for ChildFieldId: {ChildFieldId}. Exiting ReadChildField.", childFieldId);
                 return; // No match, nothing to process
            }
             _logger.Debug("Regex match successful for ChildFieldId: {ChildFieldId}.", childFieldId);

             // Iterate through the fields defined within the child field's Lines object
             if (childField.Lines.Fields == null)
             {
                  _logger.Warning("ChildFieldId: {ChildFieldId} has null Fields collection within its Lines object. Cannot extract values.", childFieldId);
                  return;
             }

             foreach (var field in childField.Lines.Fields.Where(f => f != null)) // Safe iteration
             {
                 int? innerFieldId = field.Id;
                 string innerFieldName = field.Field ?? "UnknownInnerField";
                  _logger.Verbose("Processing Inner FieldId: {InnerFieldId} ('{InnerFieldName}') for ChildFieldId: {ChildFieldId}", innerFieldId, innerFieldName, childFieldId);

                 // Determine initial value (override or group match)
                 string initialValue = field.FieldValue?.Value?.Trim();
                 bool usedOverride = initialValue != null;
                 if (!usedOverride)
                 {
                     // Use field.Key (the regex group name) to extract value
                     initialValue = match.Groups.ContainsKey(field.Key) ? match.Groups[field.Key]?.Value?.Trim() : null;
                      _logger.Verbose("InnerFieldId: {InnerFieldId} - Using Regex Group '{GroupKey}'. Initial Value: '{InitialValue}'", innerFieldId, field.Key, initialValue);
                 } else {
                      _logger.Verbose("InnerFieldId: {InnerFieldId} - Using FieldValue override. Initial Value: '{InitialValue}'", innerFieldId, initialValue);
                 }

                 // Apply formatting regex
                 string formattedValue = initialValue;
                 if (field.FormatRegEx != null && formattedValue != null)
                 {
                      _logger.Verbose("InnerFieldId: {InnerFieldId} - Applying {Count} format regex patterns.", innerFieldId, field.FormatRegEx.Count);
                      foreach (var reg in field.FormatRegEx.OrderBy(x => x?.Id ?? int.MaxValue)) // Safe ordering
                      {
                          string fmtPattern = reg?.RegEx?.RegEx;
                          string fmtReplacement = reg?.ReplacementRegEx?.RegEx ?? string.Empty;
                          int fmtRegId = reg?.Id ?? -1;

                          if (reg?.RegEx == null || string.IsNullOrEmpty(fmtPattern)) {
                               _logger.Warning("Skipping null/empty format pattern for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}", fmtRegId, innerFieldId);
                               continue;
                          }
                           if (reg.ReplacementRegEx == null) {
                                _logger.Warning("Format ReplacementRegEx is null for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}. Replacement is empty string.", fmtRegId, innerFieldId);
                           }

                           _logger.Verbose("Applying FormatRegId: {FormatRegId} - Pattern: '{Pattern}', Replacement: '{Replacement}'", fmtRegId, fmtPattern, fmtReplacement);
                           try
                           {
                               // Use parent line's multiline setting for format regex? Original code did this.
                               bool parentIsMultiLine = this.OCR_Lines?.RegularExpressions?.MultiLine ?? false;
                               RegexOptions fmtOptions = (parentIsMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                                         RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                               formattedValue = Regex.Replace(formattedValue, fmtPattern, fmtReplacement, fmtOptions, RegexTimeout).Trim();
                                _logger.Verbose("Value after FormatRegId {FormatRegId}: '{FormattedValue}'", fmtRegId, formattedValue);
                           }
                           catch (RegexMatchTimeoutException timeoutEx) {
                                _logger.Error(timeoutEx, "Regex format replace timed out (>{TimeoutSeconds}s) for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}. Skipping.", RegexTimeout.TotalSeconds, fmtRegId, innerFieldId);
                           }
                           catch (ArgumentException argEx) { // Catch invalid regex patterns
                                _logger.Error(argEx, "Invalid format regex pattern for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}. Skipping.", fmtRegId, innerFieldId);
                           }
                           catch (Exception formatEx) {
                                _logger.Error(formatEx, "Error applying format regex FormatRegId: {FormatRegId} for InnerFieldId: {InnerFieldId}. Skipping.", fmtRegId, innerFieldId);
                           }
                      }
                 } else if (formattedValue == null) {
                      _logger.Verbose("InnerFieldId: {InnerFieldId} - Initial value was null, skipping formatting.", innerFieldId);
                 } else {
                      _logger.Verbose("InnerFieldId: {InnerFieldId} - No format regex patterns found.", innerFieldId);
                 }


                 // Add to dictionary if value is not empty, always using instance 1 as per original code
                 if (!string.IsNullOrEmpty(formattedValue))
                 {
                     // Use the inner field ('field') and hardcoded instance 1 for the key
                     var valueKey = (field, 1);
                      _logger.Verbose("Adding/Updating value for InnerFieldId: {InnerFieldId}, Instance: 1. Value: '{Value}'", innerFieldId, formattedValue);
                      // Use AddOrUpdate semantics - if key exists, update; otherwise, add.
                      values[valueKey] = formattedValue.Trim(); // Trim final value
                 } else {
                      _logger.Verbose("Skipping InnerFieldId: {InnerFieldId} because formatted value is null or empty.", innerFieldId);
                 }
             } // End foreach inner field
        }
        catch (RegexMatchTimeoutException timeoutEx)
        {
             _logger.Error(timeoutEx, "Regex match timed out (>{TimeoutSeconds}s) during ReadChildField for ChildFieldId: {ChildFieldId}, Pattern: '{Pattern}'",
                RegexTimeout.TotalSeconds, childFieldId, childField?.Lines?.RegularExpressions?.RegEx ?? "Unknown");
             // Don't throw, just log and exit method for this child field
        }
        catch (Exception e)
        {
             _logger.Error(e, "Error during ReadChildField for ChildFieldId: {ChildFieldId} ('{ChildFieldName}')", childFieldId, childFieldName);
             // Don't re-throw, allow processing of other fields/lines to continue if possible
        }
         _logger.Verbose("Exiting ReadChildField for ChildFieldId: {ChildFieldId}", childFieldId);
    }
}