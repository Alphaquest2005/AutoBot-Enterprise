using System;
using System.Collections.Generic;
using System.Linq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        public List<Dictionary<string, List<KeyValuePair<(Fields fields, string instance), string>>>> FailedFields
        {
            get
            {
                int? partId = this.OCR_Part?.Id;
                string propertyName = nameof(FailedFields);
                _logger.Verbose("Entering {PropertyName} getter for PartId: {PartId}", propertyName, partId);
                var finalFailedFields =
                    new List<Dictionary<string,
                        List<KeyValuePair<(Fields fields, string instance), string>>>>(); // Initialize

                try
                {
                    // Added null checks and safe navigation
                    _logger.Verbose("{PropertyName}: Getting failed fields from direct lines for PartId: {PartId}...",
                        propertyName, partId);
                    finalFailedFields = this.Lines?
                                            .Where(l => l != null) // Safe check
                                            .SelectMany(x =>
                                                x.FailedFields ?? Enumerable
                                                    .Empty<Dictionary<string,
                                                        List<KeyValuePair<(Fields fields, string instance),
                                                            string>>>>()) // Access property, handle null
                                            .ToList()
                                        ?? new List<Dictionary<string,
                                            List<KeyValuePair<(Fields fields, string instance),
                                                string>>>>(); // Default if Lines is null
                    _logger.Information(
                        "{PropertyName}: Found {Count} groups of failed fields from direct lines for PartId: {PartId}",
                        propertyName, finalFailedFields.Count, partId);
                    // Note: This doesn't currently aggregate failed fields from children. Add if needed.
                }
                catch (Exception ex)
                {
                    _logger.Error(ex,
                        "{PropertyName}: Error during evaluation for PartId: {PartId}. Returning empty list.",
                        propertyName, partId);
                    finalFailedFields =
                        new List<Dictionary<string,
                            List<KeyValuePair<(Fields fields, string instance), string>>>>(); // Ensure empty list on error
                }

                _logger.Verbose("Exiting {PropertyName} getter for PartId: {PartId}", propertyName, partId);
                return finalFailedFields;
            }
        }
    }
}