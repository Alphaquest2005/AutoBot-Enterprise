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
            int? lineId = lines?.Id; // Safe access
            _logger.Debug("Constructing Line object for OCR_Lines Id: {LineId}", lineId);
            if (lines == null)
            {
                 _logger.Error("Line constructor called with null OCR_Lines object. Cannot initialize.");
                 // Throwing exception as Line is likely invalid without OCR_Lines
                 throw new ArgumentNullException(nameof(lines), "OCR_Lines object cannot be null.");
            }
            OCR_Lines = lines;
             _logger.Verbose("Assigned OCR_Lines (Id: {LineId}) to Line property.", lineId);
        }



        // Modified to accept instance from the calling Part

        // Simple initialization, no logging needed here unless creation logic becomes complex
        public Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> Values { get; } = new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
        //public bool MultiLine => OCR_Lines.MultiLine;

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields
        {
            get
            {
                 int? lineId = this.OCR_Lines?.Id;
                 _logger.Verbose("Evaluating FailedFields property for LineId: {LineId}", lineId);
                 List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> failed = null;
                 try
                 {
                     // Added comprehensive null checks for safety
                     failed = Values? // Check if Values itself is null
                         .Where(x => x.Value != null && // Check inner dictionary not null
                                     x.Value.Any(z => z.Key.fields != null && // Check fields not null
                                                      z.Key.fields.IsRequired &&
                                                      string.IsNullOrEmpty(z.Value))) // Check value is null/empty
                         .SelectMany(x => x.Value.ToList()) // Flatten inner dictionaries' KVPs safely
                         .Where(kvp => kvp.Key.fields != null) // Ensure fields is not null for DistinctBy/GroupBy
                         .DistinctBy(x => x.Key.fields.Id) // Distinct by Field ID
                         .GroupBy(x => $"{x.Key.fields.Field ?? "NULL"}-{x.Key.fields.Key ?? "NULL"}") // Group by FieldName-FieldKey combo safely
                         .Select(g => g.ToDictionary(k => g.Key, v => g.ToList())) // Create dictionary per group
                         .ToList()
                         ?? new List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>(); // Default to empty list if Values was null

                      _logger.Verbose("Found {Count} groups of failed fields for LineId: {LineId}", failed.Count, lineId);
                 }
                 catch (Exception ex)
                 {
                      _logger.Error(ex, "Error evaluating FailedFields property for LineId: {LineId}", lineId);
                      failed = new List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>(); // Return empty list on error
                 }
                 return failed;
            }
        }

    }
}