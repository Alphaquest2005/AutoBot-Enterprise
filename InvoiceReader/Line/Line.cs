using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        // Define logger instance here
        private static readonly ILogger _logger = Log.ForContext<Line>();
        // Define Regex Timeout constant here for use in other partial methods
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        public Lines OCR_Lines { get; }

        public Line(Lines lines)
        {
            string methodName = nameof(Line) + " Constructor";
            int? lineId = lines?.Id; // Safe access
            _logger.Verbose("Entering {MethodName} for OCR_Lines Id: {LineId}", methodName, lineId);

            // --- Input Validation ---
            if (lines == null)
            {
                _logger.Error("{MethodName}: Called with null OCR_Lines object. Cannot initialize.", methodName);
                _logger.Verbose("Exiting {MethodName} due to null input.", methodName);
                // Throwing exception as Line is likely invalid without OCR_Lines
                throw new ArgumentNullException(nameof(lines), "OCR_Lines object cannot be null.");
            }

            OCR_Lines = lines;
            _logger.Verbose("{MethodName}: Assigned OCR_Lines (Id: {LineId}) to Line property.", methodName, lineId);
            _logger.Information("Exiting {MethodName} successfully for LineId: {LineId}", methodName, lineId);
        }



        // Modified to accept instance from the calling Part

        // Simple initialization, no logging needed here unless creation logic becomes complex
        public Dictionary<(int lineNumber, string section), Dictionary<(Fields Fields, string Instance), string>> Values { get; } = new Dictionary<(int lineNumber, string section), Dictionary<(Fields Fields, string Instance), string>>();
        //public bool MultiLine => OCR_Lines.MultiLine;

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, string instance), string>>>> FailedFields
        {
            get
            {
                string propertyName = nameof(FailedFields);
                int? lineId = this.OCR_Lines?.Id;
                _logger.Verbose("Entering {PropertyName} getter for LineId: {LineId}", propertyName, lineId);
                var finalFailedList = new List<Dictionary<string, List<KeyValuePair<(Fields fields, string instance), string>>>>(); // Initialize

                try
                {
                    // --- Input Validation ---
                    if (this.Values == null)
                    {
                        _logger.Warning("{PropertyName}: Main 'Values' dictionary is null for LineId: {LineId}. Returning empty list.", propertyName, lineId);
                        _logger.Verbose("Exiting {PropertyName} getter for LineId: {LineId} (Null Values Dictionary).", propertyName, lineId);
                        return finalFailedList; // Return empty list
                    }

                    // --- LINQ Query for Failed Fields ---
                    _logger.Verbose("{PropertyName}: Evaluating LINQ query to find failed fields for LineId: {LineId}...", propertyName, lineId);
                    finalFailedList = Values
                        .Where(outerKvp => outerKvp.Value != null) // Ensure inner dictionary is not null
                        .SelectMany(outerKvp => outerKvp.Value // Select KVPs from inner dictionary
                            .Where(innerKvp => innerKvp.Key.Fields != null && // Ensure Fields object is not null
                                               innerKvp.Key.Fields.IsRequired && // Check if required
                                               string.IsNullOrEmpty(innerKvp.Value)) // Check if value is missing
                            .Select(innerKvp => new // Project to anonymous type for grouping
                            {
                                Field = innerKvp.Key.Fields,
                                Instance = innerKvp.Key.Instance,
                                Value = innerKvp.Value,
                                GroupKey = $"{innerKvp.Key.Fields.Field ?? "NULL"}-{innerKvp.Key.Fields.Key ?? "NULL"}" // Grouping key
                            }))
                        .GroupBy(failedItem => failedItem.GroupKey) // Group by FieldName-FieldKey combo
                        .Select(group => group.ToDictionary( // Create dictionary per group
                            g => group.Key, // CORRECTED: Use group.Key for the dictionary key
                            // Select from the 'group' itself, not the 'g' parameter which represents the key
                            g => group.Select(item => new KeyValuePair<(Fields fields, string instance), string>((item.Field, item.Instance), item.Value)).ToList() // Value is list of original KVPs
                        ))
                        .ToList();

                    _logger.Information("{PropertyName}: Found {Count} groups of failed fields for LineId: {LineId}", propertyName, finalFailedList.Count, lineId);
                    if (finalFailedList.Any())
                    {
                        // Log details of failed fields if needed (can be verbose)
                        foreach (var groupDict in finalFailedList)
                        {
                            foreach (var kvp in groupDict)
                            {
                                _logger.Debug("{PropertyName}: Failed Field Group '{GroupKey}': {Count} instances failed.", propertyName, kvp.Key, kvp.Value.Count);
                                // foreach (var failedKvp in kvp.Value) {
                                //     _logger.Debug("  - FieldId: {FieldId}, Instance: {Instance}", failedKvp.Key.fields.Id, failedKvp.Key.instance);
                                // }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "{PropertyName}: Unhandled exception during evaluation for LineId: {LineId}. Returning empty list.", propertyName, lineId);
                    finalFailedList = new List<Dictionary<string, List<KeyValuePair<(Fields fields, string instance), string>>>>(); // Ensure empty list on error
                }

                _logger.Verbose("Exiting {PropertyName} getter for LineId: {LineId}", propertyName, lineId);
                return finalFailedList;
            }
        }

    }
}