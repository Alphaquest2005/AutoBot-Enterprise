using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using System.Globalization; // Added for CultureInfo if needed by GetValue

namespace WaterNut.DataSpace
{

    public partial class Line
    {
        // Assuming _logger and RegexTimeout exist from other partial parts
        // private static readonly ILogger _logger = Log.ForContext<Line>();
        // private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        private void ReadChildField(Fields childField, Dictionary<(Fields field, string instance), string> values,
            string strValue)
        {
            string methodName = nameof(ReadChildField);
            int? lineId = this.OCR_Lines?.Id; // For logging context
            int? childFieldId = childField?.Id;
            string childFieldName = childField?.Field ?? "UnknownChildField";
            _logger.Verbose(
                "Entering {MethodName} for LineId: {LineId}, ChildFieldId: {ChildFieldId} ('{ChildFieldName}') based on ParentValue: '{ParentValue}'",
                methodName, lineId, childFieldId, childFieldName, strValue);

            // --- Input Validation ---
            if (childField?.Lines?.RegularExpressions == null)
            {
                _logger.Warning(
                    "{MethodName}: Cannot read child field {ChildFieldId}: ChildField, Lines, or RegularExpressions is null. Exiting.",
                    methodName, childFieldId);
                _logger.Verbose(
                    "Exiting {MethodName} for ChildFieldId: {ChildFieldId} due to null ChildField/Lines/Regex.",
                    methodName, childFieldId);
                return;
            }

            if (values == null)
            {
                _logger.Error(
                    "{MethodName}: Cannot read child field {ChildFieldId}: values dictionary is null. Exiting.",
                    methodName, childFieldId);
                _logger.Verbose("Exiting {MethodName} for ChildFieldId: {ChildFieldId} due to null values dictionary.",
                    methodName, childFieldId);
                return;
            }

            if (strValue == null) // Check parent value
            {
                _logger.Warning(
                    "{MethodName}: Cannot read child field {ChildFieldId}: Input string value (strValue from parent) is null. Exiting.",
                    methodName, childFieldId);
                _logger.Verbose("Exiting {MethodName} for ChildFieldId: {ChildFieldId} due to null parent value.",
                    methodName, childFieldId);
                return;
            }

            _logger.Verbose("{MethodName}: Input validation passed for ChildFieldId: {ChildFieldId}.", methodName,
                childFieldId);


            try
            {
                // --- Regex Matching on Parent Value ---
                string pattern = childField.Lines.RegularExpressions.RegEx;
                if (string.IsNullOrEmpty(pattern))
                {
                    _logger.Warning(
                        "{MethodName}: ChildField {ChildFieldId} regex pattern is null or empty. Skipping match.",
                        methodName, childFieldId);
                    _logger.Verbose("Exiting {MethodName} for ChildFieldId: {ChildFieldId} due to null/empty pattern.",
                        methodName, childFieldId);
                    return;
                }

                bool isMultiLine = childField.Lines.RegularExpressions.MultiLine ?? false;
                RegexOptions options = (isMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                       RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

                _logger.Verbose(
                    "{MethodName}: Attempting Regex.Match for ChildFieldId: {ChildFieldId}, Pattern: '{Pattern}', Options: {Options} on ParentValue: '{ParentValue}'",
                    methodName, childFieldId, pattern, options, strValue.Trim());
                Match match = Regex.Match(strValue.Trim(), pattern, options, RegexTimeout);

                if (!match.Success)
                {
                    _logger.Verbose("{MethodName}: No regex match found for ChildFieldId: {ChildFieldId}. Exiting.",
                        methodName, childFieldId);
                    _logger.Verbose("Exiting {MethodName} for ChildFieldId: {ChildFieldId} (No Match).", methodName,
                        childFieldId);
                    return; // No match, nothing to process
                }

                _logger.Debug("{MethodName}: Regex match successful for ChildFieldId: {ChildFieldId}.", methodName,
                    childFieldId);

                // --- Process Inner Fields defined in ChildField.Lines.Fields ---
                if (childField.Lines.Fields == null)
                {
                    _logger.Warning(
                        "{MethodName}: ChildFieldId: {ChildFieldId} has null Fields collection within its Lines object. Cannot extract values.",
                        methodName, childFieldId);
                    _logger.Verbose(
                        "Exiting {MethodName} for ChildFieldId: {ChildFieldId} due to null inner Fields collection.",
                        methodName, childFieldId);
                    return;
                }

                _logger.Verbose("{MethodName}: Processing {Count} inner fields for ChildFieldId: {ChildFieldId}...",
                    methodName, childField.Lines.Fields.Count, childFieldId);
                int innerFieldIndex = 0;
                foreach (var field in childField.Lines.Fields.Where(f => f != null)) // Safe iteration
                {
                    innerFieldIndex++;
                    int? innerFieldId = field.Id;
                    string innerFieldName = field.Field ?? "UnknownInnerField";
                    _logger.Verbose(
                        "{MethodName}: Processing Inner Field {Index}/{Total} (Id: {InnerFieldId}, Name: '{InnerFieldName}') for ChildFieldId: {ChildFieldId}",
                        methodName, innerFieldIndex, childField.Lines.Fields.Count, innerFieldId, innerFieldName,
                        childFieldId);

                    // --- Determine Initial Value ---
                    string initialValue = field.FieldValue?.Value?.Trim();
                    bool usedOverride = initialValue != null;
                    if (!usedOverride)
                    {
                        string groupKey = field.Key;
                        if (match.Groups[groupKey] != null)
                        {
                            initialValue = match.Groups[groupKey]?.Value?.Trim();
                            _logger.Verbose(
                                "{MethodName}: InnerFieldId: {InnerFieldId} - Using Regex Group '{GroupKey}'. Initial Value: '{InitialValue}'",
                                methodName, innerFieldId, groupKey, initialValue);
                        }
                        else
                        {
                            initialValue = null;
                            _logger.Warning(
                                "{MethodName}: InnerFieldId: {InnerFieldId} - Regex Group Key '{GroupKey}' not found in match. Initial Value set to null.",
                                methodName, innerFieldId, groupKey);
                        }
                    }
                    else
                    {
                        _logger.Verbose(
                            "{MethodName}: InnerFieldId: {InnerFieldId} - Using FieldValue override. Initial Value: '{InitialValue}'",
                            methodName, innerFieldId, initialValue);
                    }

                    // --- Apply Formatting Regex ---
                    string formattedValue = initialValue;
                    if (field.FormatRegEx != null && formattedValue != null)
                    {
                        _logger.Verbose(
                            "{MethodName}: InnerFieldId: {InnerFieldId} - Applying {Count} format regex patterns to value '{CurrentValue}'...",
                            methodName, innerFieldId, field.FormatRegEx.Count, formattedValue);
                        int formatIndex = 0;
                        foreach (var reg in field.FormatRegEx.OrderBy(x => x?.Id ?? int.MaxValue))
                        {
                            formatIndex++;
                            string fmtPattern = reg?.RegEx?.RegEx;
                            string fmtReplacement = reg?.ReplacementRegEx?.RegEx ?? string.Empty;
                            int fmtRegId = reg?.Id ?? -1;

                            if (reg?.RegEx == null || string.IsNullOrEmpty(fmtPattern))
                            {
                                _logger.Warning(
                                    "{MethodName}: Skipping null/empty format pattern {Index}/{Total} (FormatRegId: {FormatRegId}) for InnerFieldId: {InnerFieldId}",
                                    methodName, formatIndex, field.FormatRegEx.Count, fmtRegId, innerFieldId);
                                continue;
                            }

                            if (reg.ReplacementRegEx == null)
                            {
                                _logger.Warning(
                                    "{MethodName}: Format ReplacementRegEx is null for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}. Replacement is empty string.",
                                    methodName, fmtRegId, innerFieldId);
                            }

                            _logger.Verbose(
                                "{MethodName}: Applying Format {Index}/{Total} (Id: {FormatRegId}) - Pattern: '{Pattern}', Replacement: '{Replacement}'",
                                methodName, formatIndex, field.FormatRegEx.Count, fmtRegId, fmtPattern, fmtReplacement);
                            try
                            {
                                // Use parent line's multiline setting?
                                bool parentIsMultiLine = this.OCR_Lines?.RegularExpressions?.MultiLine ?? false;
                                RegexOptions fmtOptions =
                                    (parentIsMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                                string valueBeforeFormat = formattedValue;
                                formattedValue = Regex.Replace(formattedValue, fmtPattern, fmtReplacement, fmtOptions,
                                    RegexTimeout).Trim();
                                _logger.Verbose(
                                    "{MethodName}: Value after FormatRegId {FormatRegId}: '{FormattedValue}' (was: '{OriginalValue}')",
                                    methodName, fmtRegId, formattedValue, valueBeforeFormat);
                            }
                            catch (RegexMatchTimeoutException timeoutEx)
                            {
                                _logger.Error(timeoutEx,
                                    "{MethodName}: Regex format replace timed out (>{TimeoutSeconds}s) for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}. Skipping.",
                                    methodName, RegexTimeout.TotalSeconds, fmtRegId, innerFieldId);
                            }
                            catch (ArgumentException argEx)
                            {
                                _logger.Error(argEx,
                                    "{MethodName}: Invalid format regex pattern for FormatRegId: {FormatRegId}, InnerFieldId: {InnerFieldId}. Skipping.",
                                    methodName, fmtRegId, innerFieldId);
                            }
                            catch (Exception formatEx)
                            {
                                _logger.Error(formatEx,
                                    "{MethodName}: Error applying format regex FormatRegId: {FormatRegId} for InnerFieldId: {InnerFieldId}. Skipping.",
                                    methodName, fmtRegId, innerFieldId);
                            }
                        }

                        _logger.Verbose(
                            "{MethodName}: InnerFieldId: {InnerFieldId} - Finished applying format regex patterns. Final formatted value: '{FinalValue}'",
                            methodName, innerFieldId, formattedValue);
                    }
                    else if (formattedValue == null)
                    {
                        _logger.Verbose(
                            "{MethodName}: InnerFieldId: {InnerFieldId} - Initial value was null, skipping formatting.",
                            methodName, innerFieldId);
                    }
                    else
                    {
                        _logger.Verbose("{MethodName}: InnerFieldId: {InnerFieldId} - No format regex patterns found.",
                            methodName, innerFieldId);
                    }


                    // --- Store Value ---
                    if (!string.IsNullOrEmpty(formattedValue))
                    {
                        // Use the inner field ('field') and hardcoded instance 1 for the key
                        (Fields field, string instance) valueKey = (field, "0");
                        _logger.Verbose(
                            "{MethodName}: Adding/Updating value for InnerFieldId: {InnerFieldId}, Instance: 1. Value: '{Value}'",
                            methodName, innerFieldId, formattedValue);
                        values[valueKey] = formattedValue.Trim(); // Add or update
                    }
                    else
                    {
                        _logger.Verbose(
                            "{MethodName}: Skipping InnerFieldId: {InnerFieldId} because formatted value is null or empty.",
                            methodName, innerFieldId);
                    }

                    _logger.Verbose("{MethodName}: Finished processing Inner FieldId: {InnerFieldId}", methodName,
                        innerFieldId);
                } // End foreach inner field

                _logger.Verbose("{MethodName}: Finished processing inner fields for ChildFieldId: {ChildFieldId}.",
                    methodName, childFieldId);
            }
            catch (RegexMatchTimeoutException timeoutEx)
            {
                _logger.Error(timeoutEx,
                    "{MethodName}: Regex match timed out (>{TimeoutSeconds}s) for ChildFieldId: {ChildFieldId}, Pattern: '{Pattern}'",
                    methodName, RegexTimeout.TotalSeconds, childFieldId,
                    childField?.Lines?.RegularExpressions?.RegEx ?? "Unknown");
                // Don't throw, just log and exit method for this child field
            }
            catch (Exception e)
            {
                _logger.Error(e,
                    "{MethodName}: Unhandled exception for ChildFieldId: {ChildFieldId} ('{ChildFieldName}')",
                    methodName, childFieldId, childFieldName);
                // Don't re-throw
            }

            _logger.Verbose("Exiting {MethodName} for ChildFieldId: {ChildFieldId}", methodName, childFieldId);
        }
    }
}