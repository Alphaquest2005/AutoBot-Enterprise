using System;
using System.Linq;
using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        private string ApplyFormating(string initialValue, Fields field, string methodName, int? fieldId)
        {
            // --- Apply Formatting Regex ---
            string formattedValue = initialValue;
            if (field.FormatRegEx != null &&
                formattedValue != null) // Only format if value exists and formats defined
            {
                _logger.Verbose(
                    "{MethodName}: FieldId: {FieldId} - Applying {Count} format regex patterns to value '{CurrentValue}'...",
                    methodName, fieldId, field.FormatRegEx.Count, formattedValue);
                int formatIndex = 0;
                foreach (var reg in field.FormatRegEx.OrderBy(x => x?.Id ?? int.MaxValue)) // Safe ordering
                {
                    formatIndex++;
                    string pattern = reg?.RegEx?.RegEx;
                    string replacement = reg?.ReplacementRegEx?.RegEx ?? string.Empty;
                    int formatRegId = reg?.Id ?? -1;

                    if (reg?.RegEx == null || string.IsNullOrEmpty(pattern))
                    {
                        _logger.Warning(
                            "{MethodName}: Skipping null/empty format pattern {Index}/{Total} (FormatRegId: {FormatRegId}) for FieldId: {FieldId}",
                            methodName, formatIndex, field.FormatRegEx.Count, formatRegId, fieldId);
                        continue;
                    }

                    if (reg.ReplacementRegEx == null)
                    {
                        _logger.Warning(
                            "{MethodName}: Format ReplacementRegEx is null for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Replacement will be empty string.",
                            methodName, formatRegId, fieldId);
                    }

                    _logger.Verbose(
                        "{MethodName}: Applying Format {Index}/{Total} (Id: {FormatRegId}) - Pattern: '{Pattern}', Replacement: '{Replacement}'",
                        methodName, formatIndex, field.FormatRegEx.Count, formatRegId, pattern, replacement);
                    try
                    {
                        bool isMultiLineFormat =
                            this.OCR_Lines?.RegularExpressions?.MultiLine ??
                            false; // Use line's multiline setting? Or format's? Assuming line's for now.
                        RegexOptions options =
                            (isMultiLineFormat ? RegexOptions.Multiline : RegexOptions.Singleline) |
                            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                        string valueBeforeFormat = formattedValue;
                        formattedValue = Regex.Replace(formattedValue, pattern, replacement, options,
                            RegexTimeout).Trim();
                        _logger.Verbose(
                            "{MethodName}: Value after FormatRegId {FormatRegId}: '{FormattedValue}' (was: '{OriginalValue}')",
                            methodName, formatRegId, formattedValue, valueBeforeFormat);
                    }
                    catch (RegexMatchTimeoutException timeoutEx)
                    {
                        _logger.Error(timeoutEx,
                            "{MethodName}: Regex format replace timed out (>{TimeoutSeconds}s) for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Skipping this format.",
                            methodName, RegexTimeout.TotalSeconds, formatRegId, fieldId);
                    }
                    catch (ArgumentException argEx)
                    {
                        _logger.Error(argEx,
                            "{MethodName}: Invalid format regex pattern encountered for FormatRegId: {FormatRegId}, FieldId: {FieldId}. Skipping this format.",
                            methodName, formatRegId, fieldId);
                    }
                    catch (Exception formatEx)
                    {
                        _logger.Error(formatEx,
                            "{MethodName}: Error applying format regex FormatRegId: {FormatRegId} for FieldId: {FieldId}. Skipping this format.",
                            methodName, formatRegId, fieldId);
                    }
                }
            }
            else
            {
                if (formattedValue == null)
                {
                    _logger.Verbose(
                        "{MethodName}: FieldId: {FieldId} - Initial value was null, skipping formatting.",
                        methodName, fieldId);
                }
                else
                {
                    _logger.Verbose("{MethodName}: FieldId: {FieldId} - No format regex patterns defined.",
                        methodName, fieldId);
                }
            }

            return formattedValue;
        }
    }
}