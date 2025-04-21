using OCR.Business.Entities;
using System.Collections.Generic; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any()

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        // Assuming _logger exists from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Line>();

        private void SaveLineValues(int lineNumber, string section, int instance,
            Dictionary<(Fields Fields, int Instance), string> values)
        {
            string methodName = nameof(SaveLineValues);
            int? lineId = this.OCR_Lines?.Id; // For logging context
            _logger.Verbose(
                "Entering {MethodName} for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}. Input Value Count: {ValueCount}",
                methodName, lineId, lineNumber, section, instance, values?.Count ?? 0);

            // --- Input Validation ---
            if (values == null || !values.Any())
            {
                _logger.Warning(
                    "{MethodName}: Called with null or empty values dictionary for LineId: {LineId}, Instance: {Instance}. Nothing to save.",
                    methodName, lineId, instance);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} (Null/Empty Input Values).", methodName,
                    lineId);
                return;
            }

            if (this.Values == null)
            {
                // This should ideally not happen if the property initializer works correctly.
                _logger.Error(
                    "{MethodName}: Cannot proceed: Main 'Values' dictionary (this.Values) is null for LineId: {LineId}. This indicates an initialization issue.",
                    methodName, lineId);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} (Null Main Values Dictionary).", methodName,
                    lineId);
                return;
            }

            _logger.Verbose("{MethodName}: Input validation passed for LineId: {LineId}.", methodName, lineId);


            var key = (lineNumber, section); // Key for the outer dictionary
            try
            {
                // --- Check if Line/Section Key Exists ---
                if (Values.TryGetValue(key, out var existingInnerDict))
                {
                    _logger.Verbose(
                        "{MethodName}: Key ({LineNumber}, {Section}) already exists. Merging/Overwriting values for LineId: {LineId}, Instance: {Instance}",
                        methodName, lineNumber, section, lineId, instance);

                    // --- Ensure Inner Dictionary Exists ---
                    if (existingInnerDict == null)
                    {
                        // This is unexpected if the outer dictionary contains the key. Log an error and fix.
                        _logger.Error(
                            "{MethodName}: Existing inner dictionary for Key ({LineNumber}, {Section}) is null! This should not happen. Re-initializing.",
                            methodName, lineNumber, section);
                        existingInnerDict = new Dictionary<(Fields fields, int instance), string>();
                        Values[key] = existingInnerDict; // Assign the new dictionary back
                    }

                    // --- Merge/Overwrite Values in Inner Dictionary ---
                    _logger.Verbose(
                        "{MethodName}: Merging {Count} new values into existing inner dictionary for Key ({LineNumber}, {Section})...",
                        methodName, values.Count, lineNumber, section);
                    int mergedCount = 0;
                    int addedCount = 0;
                    foreach (var kvp in values)
                    {
                        var valueKey = kvp.Key; // (Field, Instance) tuple
                        string newValue = kvp.Value;
                        int? fieldId = valueKey.Fields?.Id; // Safe access for logging

                        if (existingInnerDict.ContainsKey(valueKey))
                        {
                            // Log overwrite clearly
                            _logger.Warning(
                                "{MethodName}: Duplicate Field/Instance Key ({FieldId}, {ValueInstance}) found for section Key ({LineNumber}, {Section}). Overwriting existing value '{ExistingValue}' with '{NewValue}'.",
                                methodName, fieldId, valueKey.Instance, lineNumber, section,
                                existingInnerDict[valueKey], newValue);
                            existingInnerDict[valueKey] = newValue; // Overwrite
                            mergedCount++;
                        }
                        else
                        {
                            _logger.Verbose(
                                "{MethodName}: Adding new Field/Instance Key: ({FieldId}, {ValueInstance}) with Value: '{NewValue}' to existing section Key: ({LineNumber}, {Section})",
                                methodName, fieldId, valueKey.Instance, newValue, lineNumber, section);
                            existingInnerDict.Add(valueKey, newValue); // Add new
                            addedCount++;
                        }
                    }

                    _logger.Debug(
                        "{MethodName}: Finished merging values for existing Key ({LineNumber}, {Section}). Added: {AddedCount}, Merged/Overwritten: {MergedCount}. Inner dictionary now has {TotalCount} items.",
                        methodName, lineNumber, section, addedCount, mergedCount, existingInnerDict.Count);
                }
                else // Key doesn't exist, add the new inner dictionary
                {
                    _logger.Verbose(
                        "{MethodName}: Key ({LineNumber}, {Section}) does not exist. Adding new inner dictionary with {Count} values.",
                        methodName, lineNumber, section, values.Count);
                    // Log the values being added for the first time
                    foreach (var kvp in values)
                    {
                        _logger.Verbose(
                            "{MethodName}: Adding Initial Field/Instance Key: ({FieldId}, {ValueInstance}), Value: '{NewValue}' for new section Key: ({LineNumber}, {Section})",
                            methodName, kvp.Key.Fields?.Id, kvp.Key.Instance, kvp.Value, lineNumber, section);
                    }

                    Values.Add(key, values); // Add the dictionary passed in
                }

                _logger.Information(
                    "{MethodName}: Completed successfully for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
                    methodName, lineId, lineNumber, section, instance);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "{MethodName}: Unhandled exception for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
                    methodName, lineId, lineNumber, section, instance);
                // Decide if exception should be propagated based on overall error handling strategy
                // throw;
            }

            _logger.Verbose(
                "Exiting {MethodName} for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
                methodName, lineId, lineNumber, section, instance);
        }
    }
}