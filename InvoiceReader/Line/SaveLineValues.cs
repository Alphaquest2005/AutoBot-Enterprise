using OCR.Business.Entities;
using System.Collections.Generic; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any()

namespace WaterNut.DataSpace;

public partial class Line
{
    // Assuming _logger exists from another partial part
    // private static readonly ILogger _logger = Log.ForContext<Line>();

    private void SaveLineValues(int lineNumber, string section, int instance,
        Dictionary<(Fields Fields, int Instance), string> values)
    {
        int? lineId = this.OCR_Lines?.Id; // For logging context
        _logger.Debug("Entering SaveLineValues for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}. Input Value Count: {ValueCount}",
            lineId, lineNumber, section, instance, values?.Count ?? 0);

        // Null checks
        if (values == null || !values.Any())
        {
             _logger.Warning("SaveLineValues called with null or empty values dictionary for LineId: {LineId}, Instance: {Instance}. Nothing to save.", lineId, instance);
             return;
        }
         // Ensure the main Values dictionary is initialized (should be by property initializer)
         if (this.Values == null)
         {
              _logger.Error("SaveLineValues cannot proceed: Main 'Values' dictionary is null for LineId: {LineId}.", lineId);
              // Cannot recover from this state here as Values is likely readonly after init.
              return;
         }


        var key = (lineNumber, section);
        try
        {
            // Check if key exists before adding/updating using TryGetValue for efficiency
            if (Values.TryGetValue(key, out var existingInnerDict))
            {
                 _logger.Verbose("Key ({LineNumber}, {Section}) already exists. Merging/Overwriting values for LineId: {LineId}, Instance: {Instance}",
                    lineNumber, section, lineId, instance);

                 // Check if the existing inner dictionary is null (shouldn't happen if initialized correctly)
                 if (existingInnerDict == null)
                 {
                      _logger.Error("Existing inner dictionary for Key ({LineNumber}, {Section}) is null! Cannot merge. Re-initializing.", lineNumber, section);
                      existingInnerDict = new Dictionary<(Fields fields, int instance), string>();
                      Values[key] = existingInnerDict; // Assign the new dictionary back
                 }

                 // Merge new values with existing ones for the same line/section
                 foreach (var kvp in values)
                 {
                     // The key in the input 'values' dictionary already contains the correct (Field, Instance) tuple.
                     var valueKey = kvp.Key;

                     // Use indexer for AddOrUpdate semantics
                     if (existingInnerDict.ContainsKey(valueKey))
                     {
                          _logger.Warning("Duplicate Field/Instance Key ({FieldId}, {ValueInstance}) found for section Key ({LineNumber}, {Section}). Overwriting existing value '{ExistingValue}' with '{NewValue}'.",
                             valueKey.Fields?.Id, valueKey.Instance, lineNumber, section, existingInnerDict[valueKey], kvp.Value);
                     } else {
                          _logger.Verbose("Adding new Field/Instance Key: ({FieldId}, {ValueInstance}) to existing section Key: ({LineNumber}, {Section})",
                             valueKey.Fields?.Id, valueKey.Instance, lineNumber, section);
                     }
                     existingInnerDict[valueKey] = kvp.Value; // Add or overwrite
                 }
                  _logger.Debug("Finished merging values for existing Key ({LineNumber}, {Section}). Inner dictionary now has {Count} items.",
                     lineNumber, section, existingInnerDict.Count);
            }
            else // Key doesn't exist, add the whole inner dictionary
            {
                 _logger.Verbose("Key ({LineNumber}, {Section}) does not exist. Adding new inner dictionary with {Count} values.",
                    lineNumber, section, values.Count);
                 Values.Add(key, values);
            }
             _logger.Debug("Finished SaveLineValues for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
                lineId, lineNumber, section, instance);
        }
        catch (Exception ex)
        {
             _logger.Error(ex, "Error during SaveLineValues for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
                lineId, lineNumber, section, instance);
             // Decide if exception should be propagated
        }
    }
}