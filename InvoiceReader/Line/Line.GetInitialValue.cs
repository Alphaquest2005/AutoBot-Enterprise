using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        private static string GetInitialValue(Match match, Fields field, string methodName, int? fieldId)
        {
            // --- Determine Initial Value ---
            string initialValue = field.FieldValue?.Value?.Trim(); // Use override if present
            bool usedOverride = initialValue != null;
            if (!usedOverride)
            {
                string groupKey = field.Key;
                if (match.Groups[groupKey] != null)
                {
                    initialValue = match.Groups[groupKey]?.Value?.Trim();
                    _logger.Verbose(
                        "{MethodName}: FieldId: {FieldId} - Using Regex Group '{GroupKey}'. Initial Value: '{InitialValue}'",
                        methodName, fieldId, groupKey, initialValue);
                }
                else
                {
                    initialValue = null; // Explicitly null if group key doesn't exist
                    _logger.Warning(
                        "{MethodName}: FieldId: {FieldId} - Regex Group Key '{GroupKey}' not found in match. Initial Value set to null.",
                        methodName, fieldId, groupKey);
                }
            }
            else
            {
                _logger.Verbose(
                    "{MethodName}: FieldId: {FieldId} - Using FieldValue override. Initial Value: '{InitialValue}'",
                    methodName, fieldId, initialValue);
            }

            return initialValue;
        }
    }
}