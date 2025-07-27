using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Logger instance is defined in the main Template.cs partial class file.

        private void AddMissingRequiredFieldValues()
        {
            // Use instance property for logging context if available (e.g., OcrInvoices.Id)
            int? invoiceId = this.OcrTemplates?.Id;
            _logger.Debug("Starting AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}", invoiceId);

            try
            {
                // Get required fields safely
                var requiredFieldsList = Lines
                    .Where(l => l?.OCR_Lines?.Fields != null) // Safe navigation
                    .SelectMany(x => x.OCR_Lines.Fields)
                    .Where(z => z != null && z.IsRequired && z.FieldValue != null &&
                                !string.IsNullOrEmpty(z.FieldValue.Value)) // Safe checks
                    .ToList();
                _logger.Debug("Found {Count} required fields with values for InvoiceId: {InvoiceId}",
                    requiredFieldsList.Count, invoiceId);
                if (!requiredFieldsList.Any())
                {
                    _logger.Debug(
                        "No required fields with values found, exiting AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}",
                        invoiceId);
                    return; // Nothing to do
                }

                // Get distinct instances from "Single" section lines safely
                var instances = Lines
                    .Where(l => l?.Values != null) // Safe navigation
                    .SelectMany(x => x.Values)
                    .Where(x => x.Key.section == "Single" && x.Value != null) // Ensure inner dictionary is not null
                    .SelectMany(x => x.Value.Keys.Select(k => (Instance: k.Instance, LineNumber: x.Key.lineNumber)))
                    .DistinctBy(x => x.Instance) // Requires MoreLinq
                    .ToList();
                _logger.Debug("Found {Count} distinct instances from 'Single' sections for InvoiceId: {InvoiceId}",
                    instances.Count, invoiceId);
                if (!instances.Any())
                {
                    _logger.Debug(
                        "No instances found in 'Single' sections, exiting AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}",
                        invoiceId);
                    return; // Nothing to do
                }

                // Iterate through required fields
                foreach (var field in requiredFieldsList)
                {
                    int? fieldId = field?.Id; // Safe access
                    _logger.Verbose("Processing Required FieldId: {FieldId} ('{FieldName}') for InvoiceId: {InvoiceId}",
                        fieldId, field?.Field, invoiceId);

                    // Find lines matching criteria for this field
                    // Assuming helper methods handle their own logging/null checks
                    var lines = Lines.Where(x => x != null && // Ensure line is not null
                                                 LineHasField(x, field) &&
                                                 ValueContainsRequiredField(x, field) &&
                                                 ValueForAllInstances(x,
                                                     instances) // Assuming this checks if the value exists for *all* specified instances
                    ).ToList();
                    _logger.Verbose("Found {Count} lines matching criteria for FieldId: {FieldId}", lines.Count,
                        fieldId);

                    // Iterate through matching lines
                    foreach (var line in lines)
                    {
                        int? lineId = line?.OCR_Lines?.Id; // Safe access
                        _logger.Verbose("Processing LineId: {LineId} for FieldId: {FieldId}", lineId, fieldId);

                        // Get instances already present in this line's values safely
                        var lineInstances =
                            line?.Values
                                ?.SelectMany(z => z.Value?.Keys.Select(k => k.Instance) ?? Enumerable.Empty<string>())
                                .Distinct().ToList()
                            ?? new List<string>(); // Safe navigation and default
                        _logger.Verbose("LineId: {LineId} already has values for instances: [{Instances}]", lineId,
                            string.Join(",", lineInstances));

                        // Iterate through all distinct instances found earlier
                        foreach (var instance in instances)
                        {
                            // If this line *doesn't* have a value for the current instance
                            if (!lineInstances.Contains(instance.Instance))
                            {
                                _logger.Debug(
                                    "Adding missing value for FieldId: {FieldId}, Instance: {Instance}, LineNumber: {LineNumber} to LineId: {LineId}",
                                    fieldId, instance.Instance, instance.LineNumber, lineId);

                                // Prepare keys and value
                                var key = (instance.LineNumber, "Single");
                                var innerValueKey = (field, instance.Instance);
                                var valueToAdd =
                                    field.FieldValue.Value; // Already checked FieldValue/Value is not null/empty

                                // Safely add to the dictionary
                                // Check if the main Values dictionary exists FIRST
                                if (line.Values == null)
                                {
                                    _logger.Warning(
                                        "Cannot add missing value for FieldId: {FieldId}, Instance: {Instance} because Line.Values dictionary is null for LineId: {LineId}. Skipping.",
                                        fieldId, instance.Instance, lineId);
                                    continue; // Skip to the next instance for this line
                                }

                                if (line.Values.TryGetValue(key, out var innerDict))
                                {
                                    // Key exists, add to inner dictionary if the specific field/instance doesn't exist yet
                                    if (innerDict == null) // Check if the inner dictionary itself is null
                                    {
                                        _logger.Warning(
                                            "Inner dictionary for Key ({LineNumber}, {Section}) is null for LineId: {LineId}. Initializing.",
                                            instance.LineNumber, "Single", lineId);
                                        innerDict = new Dictionary<(Fields fields, string instance), string>();
                                        line.Values[key] = innerDict; // Assign the new dictionary back
                                    }

                                    if (!innerDict.ContainsKey(innerValueKey))
                                    {
                                        innerDict.Add(innerValueKey, valueToAdd);
                                        _logger.Verbose(
                                            "Added value to existing inner dictionary for LineId: {LineId}, Key: {Key}, InnerKey: (FieldId:{FieldId}, Instance:{Instance})",
                                            lineId, key, fieldId, instance.Instance);
                                    }
                                    else
                                    {
                                        _logger.Warning(
                                            "Attempted to add value for FieldId: {FieldId}, Instance: {Instance} to LineId: {LineId}, but it already exists. Ignoring.",
                                            fieldId, instance.Instance, lineId);
                                    }
                                }
                                else
                                {
                                    // Key doesn't exist, add new entry with the inner dictionary
                                    _logger.Verbose(
                                        "Adding new key and inner dictionary for LineId: {LineId}, Key: {Key}, InnerKey: (FieldId:{FieldId}, Instance:{Instance})",
                                        lineId, key, fieldId, instance.Instance);
                                    line.Values.Add(key,
                                        new Dictionary<(Fields fields, string instance), string>
                                            { { innerValueKey, valueToAdd } });
                                }
                            }
                            else
                            {
                                _logger.Verbose(
                                    "LineId: {LineId} already contains instance {Instance} for FieldId: {FieldId}. Skipping.",
                                    lineId, instance.Instance, fieldId);
                            }
                        }
                    }
                }

                _logger.Debug("Finished AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}", invoiceId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}", invoiceId);
                // Decide if exception should be propagated
            }
        }

        // Assuming these helper methods exist in another partial class part or need logging added if defined here.
        // These need to be robust against nulls passed from the main loop.
        // private bool LineHasField(Line line, Fields field) { ... }
        // private bool ValueContainsRequiredField(Line line, Fields field) { ... }
        // private bool ValueForAllInstances(Line line, List<(int Instance, int LineNumber)> instances) { ... }
    }
}