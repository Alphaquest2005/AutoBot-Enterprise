using OCR.Business.Entities;
using System.Collections.Generic; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any()

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        private void SaveLineValues(int lineNumber, string section, string instance,
            Dictionary<(Fields Fields, string Instance), string> values)
        {
            string methodName = nameof(SaveLineValues);
            int? lineId = this.OCR_Lines?.Id;

            LogEnteringMethod(methodName, lineId, lineNumber, section, instance, values);

            if (!ValidateInput(methodName, lineId, instance, values)) return;

            if (!EnsureMainValuesDictionaryInitialized(methodName, lineId)) return;

            var key = (lineNumber, section);
            try
            {
                if (Values.TryGetValue(key, out var existingInnerDict))
                {
                    HandleExistingKey(methodName, lineId, lineNumber, section, instance, values, existingInnerDict);
                }
                else
                {
                    AddNewKey(methodName, lineNumber, section, values);
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
            }

            LogExitingMethod(methodName, lineId, lineNumber, section, instance);
        }

        private void LogEnteringMethod(string methodName, int? lineId, int lineNumber, string section, string instance, Dictionary<(Fields Fields, string Instance), string> values)
        {
            _logger.Verbose(
                "Entering {MethodName} for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}. Input Value Count: {ValueCount}",
                methodName, lineId, lineNumber, section, instance, values?.Count ?? 0);
        }

        private void LogExitingMethod(string methodName, int? lineId, int lineNumber, string section, string instance)
        {
            _logger.Verbose(
                "Exiting {MethodName} for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
                methodName, lineId, lineNumber, section, instance);
        }

        private bool ValidateInput(string methodName, int? lineId, string instance, Dictionary<(Fields Fields, string Instance), string> values)
        {
            if (values == null || !values.Any())
            {
                _logger.Warning(
                    "{MethodName}: Called with null or empty values dictionary for LineId: {LineId}, Instance: {Instance}. Nothing to save.",
                    methodName, lineId, instance);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} (Null/Empty Input Values).", methodName, lineId);
                return false;
            }

            _logger.Verbose("{MethodName}: Input validation passed for LineId: {LineId}.", methodName, lineId);
            return true;
        }

        private bool EnsureMainValuesDictionaryInitialized(string methodName, int? lineId)
        {
            if (this.Values == null)
            {
                _logger.Error(
                    "{MethodName}: Cannot proceed: Main 'Values' dictionary (this.Values) is null for LineId: {LineId}. This indicates an initialization issue.",
                    methodName, lineId);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} (Null Main Values Dictionary).", methodName, lineId);
                return false;
            }

            return true;
        }

        private void HandleExistingKey(string methodName, int? lineId, int lineNumber, string section, string instance,
            Dictionary<(Fields Fields, string Instance), string> values, Dictionary<(Fields Fields, string Instance), string> existingInnerDict)
        {
            _logger.Verbose(
                "{MethodName}: Key ({LineNumber}, {Section}) already exists. Merging/Overwriting values for LineId: {LineId}, Instance: {Instance}",
                methodName, lineNumber, section, lineId, instance);

            if (existingInnerDict == null)
            {
                _logger.Error(
                    "{MethodName}: Existing inner dictionary for Key ({LineNumber}, {Section}) is null! This should not happen. Re-initializing.",
                    methodName, lineNumber, section);
                existingInnerDict = new Dictionary<(Fields Fields, string Instance), string>();
                Values[(lineNumber, section)] = existingInnerDict;
            }

            MergeValues(methodName, lineNumber, section, values, existingInnerDict);
        }

        private void MergeValues(string methodName, int lineNumber, string section,
            Dictionary<(Fields Fields, string Instance), string> values, Dictionary<(Fields Fields, string Instance), string> existingInnerDict)
        {
            int mergedCount = 0;
            int addedCount = 0;

            foreach (var kvp in values)
            {
                var valueKey = kvp.Key;
                string newValue = kvp.Value;
                int? fieldId = valueKey.Fields?.Id;

                if (existingInnerDict.ContainsKey(valueKey))
                {
                    _logger.Warning(
                        "{MethodName}: Duplicate Field/Instance Key ({FieldId}, {ValueInstance}) found for section Key ({LineNumber}, {Section}). Overwriting existing value '{ExistingValue}' with '{NewValue}'.",
                        methodName, fieldId, valueKey.Instance, lineNumber, section,
                        existingInnerDict[valueKey], newValue);
                    existingInnerDict[valueKey] = newValue;
                    mergedCount++;
                }
                else
                {
                    _logger.Verbose(
                        "{MethodName}: Adding new Field/Instance Key: ({FieldId}, {ValueInstance}) with Value: '{NewValue}' to existing section Key: ({LineNumber}, {Section})",
                        methodName, fieldId, valueKey.Instance, newValue, lineNumber, section);
                    existingInnerDict.Add(valueKey, newValue);
                    addedCount++;
                }
            }

            _logger.Debug(
                "{MethodName}: Finished merging values for existing Key ({LineNumber}, {Section}). Added: {AddedCount}, Merged/Overwritten: {MergedCount}. Inner dictionary now has {TotalCount} items.",
                methodName, lineNumber, section, addedCount, mergedCount, existingInnerDict.Count);
        }

        private void AddNewKey(string methodName, int lineNumber, string section, Dictionary<(Fields Fields, string Instance), string> values)
        {
            _logger.Verbose(
                "{MethodName}: Key ({LineNumber}, {Section}) does not exist. Adding new inner dictionary with {Count} values.",
                methodName, lineNumber, section, values.Count);

            foreach (var kvp in values)
            {
                _logger.Verbose(
                    "{MethodName}: Adding Initial Field/Instance Key: ({FieldId}, {ValueInstance}), Value: '{NewValue}' for new section Key: ({LineNumber}, {Section})",
                    methodName, kvp.Key.Fields?.Id, kvp.Key.Instance, kvp.Value, lineNumber, section);
            }

            Values.Add((lineNumber, section), values);
        }
    }
}