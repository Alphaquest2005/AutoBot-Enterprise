using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Logger instance is defined in the main Template.cs partial class file.

        private Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>
            DistinctValues(
                Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>
                    lineValues)
        {
            int? invoiceId = this.OcrTemplates?.Id; // For logging context
            _logger.Debug("Entering DistinctValues for InvoiceId: {InvoiceId}. Input dictionary count: {InputCount}",
                invoiceId, lineValues?.Count ?? 0);

            // Null check for input
            if (lineValues == null)
            {
                _logger.Warning(
                    "DistinctValues called with null input dictionary for InvoiceId: {InvoiceId}. Returning new empty dictionary.",
                    invoiceId);
                return new Dictionary<(int lineNumber, string section),
                    Dictionary<(Fields fields, int instance), string>>();
            }

            var res =
                new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
            int processedCount = 0;
            int addedCount = 0;

            // Use a HashSet for efficient lookup of existing values encountered so far
            var existingValues = new HashSet<string>();

            try
            {
                foreach (var val in lineValues)
                {
                    processedCount++;
                    var currentKey = val.Key;
                    var currentValueDict = val.Value; // The inner dictionary
                    _logger.Verbose(
                        "Processing item {ProcessedCount}/{TotalCount}: Key=({LineNumber}, {Section}) for InvoiceId: {InvoiceId}",
                        processedCount, lineValues.Count, currentKey.lineNumber, currentKey.section, invoiceId);

                    // Check if the inner dictionary is null or empty first
                    if (currentValueDict == null || !currentValueDict.Any())
                    {
                        _logger.Verbose(
                            "Skipping item {ProcessedCount} because its inner dictionary (Value) is null or empty. Key: {Key}",
                            processedCount, currentKey);
                        continue;
                    }

                    // Check if any value in the current inner dictionary already exists in our collected set
                    bool alreadyExists = currentValueDict.Values.Any(v => existingValues.Contains(v));

                    if (!alreadyExists)
                    {
                        _logger.Verbose(
                            "Adding item with Key: {Key} because its values [{Values}] are distinct from previously added items.",
                            currentKey, string.Join(",", currentValueDict.Values));
                        res.Add(currentKey, currentValueDict);
                        addedCount++;
                        // Add the values from the newly added dictionary to our set for future checks
                        foreach (var valueToAdd in currentValueDict.Values)
                        {
                            if (valueToAdd != null) // Avoid adding nulls to the HashSet
                            {
                                existingValues.Add(valueToAdd);
                            }
                        }
                    }
                    else
                    {
                        _logger.Verbose(
                            "Skipping item with Key: {Key} because at least one of its values [{Values}] already exists in the results.",
                            currentKey, string.Join(",", currentValueDict.Values));
                    }
                }

                _logger.Debug(
                    "Finished DistinctValues for InvoiceId: {InvoiceId}. Processed: {ProcessedCount}, Added: {AddedCount}, Result Count: {ResultCount}",
                    invoiceId, processedCount, addedCount, res.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during DistinctValues processing for InvoiceId: {InvoiceId}", invoiceId);
                // Return potentially partially filled dictionary or rethrow depending on requirements
            }

            return res;
        }
    }
}