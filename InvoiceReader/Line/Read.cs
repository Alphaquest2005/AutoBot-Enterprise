using System.Text.RegularExpressions;
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
    // RegexTimeout constant is defined in the main Line.cs partial class file.

    public bool Read(string line, int lineNumber, string section, int instance)
    {
        int? lineId = this.OCR_Lines?.Id;
        _logger.Debug("Entering Line.Read for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}",
            lineId, lineNumber, section, instance);

        // Null checks
        if (line == null)
        {
             _logger.Warning("Line.Read called with null line text for LineId: {LineId}. Returning false.", lineId);
             return false;
        }
         // Safe check for OCR_Lines and RegularExpressions
         if (this.OCR_Lines?.RegularExpressions == null)
         {
              _logger.Warning("Line.Read called with null OCR_Lines or RegularExpressions for LineId: {LineId}. Cannot match. Returning false.", lineId);
              return false;
         }

        try
        {
            string pattern = this.OCR_Lines.RegularExpressions.RegEx;
            if (string.IsNullOrEmpty(pattern))
            {
                 _logger.Warning("Line.Read: Regex pattern is null or empty for LineId: {LineId}. Returning false.", lineId);
                 return false;
            }

            // Determine RegexOptions based on OCR_Lines settings safely
            bool isMultiLine = this.OCR_Lines.RegularExpressions.MultiLine ?? false; // Default to false if null
            RegexOptions options = (isMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                  RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

             _logger.Verbose("Attempting Regex.Matches for LineId: {LineId}, Pattern: '{Pattern}', Options: {Options}", lineId, pattern, options);
             // Execute regex match with timeout
             MatchCollection matches = Regex.Matches(line, pattern, options, RegexTimeout); // Use defined timeout

             _logger.Debug("Regex.Matches found {MatchCount} matches for LineId: {LineId}", matches.Count, lineId);
            if (matches.Count == 0)
            {
                 _logger.Verbose("No regex matches found for LineId: {LineId}. Returning false.", lineId);
                 return false; // No matches, nothing to process
            }

            // Initialize dictionary to store formatted values for this line/instance
            var values = new Dictionary<(Fields Fields, int Instance), string>();

            _logger.Debug("Calling FormatValues for LineId: {LineId}, Instance: {Instance}", lineId, instance);
            // FormatValues handles its own logging
            FormatValues(instance, matches, values);
            _logger.Debug("Finished FormatValues for LineId: {LineId}, Instance: {Instance}. Found {ValueCount} values.", lineId, instance, values.Count);

            // Save the extracted and formatted values only if FormatValues produced something
            if (values.Any())
            {
                _logger.Debug("Calling SaveLineValues for LineId: {LineId}, Instance: {Instance}", lineId, instance);
                // SaveLineValues handles its own logging
                SaveLineValues(lineNumber, section, instance, values);
                _logger.Debug("Finished SaveLineValues for LineId: {LineId}, Instance: {Instance}", lineId, instance);
            } else {
                 _logger.Warning("No values extracted/formatted by FormatValues for LineId: {LineId}, Instance: {Instance}. Skipping SaveLineValues.", lineId, instance);
            }

             _logger.Information("Line.Read completed successfully for LineId: {LineId}, Instance: {Instance}. Found {MatchCount} matches and processed {ValueCount} values.",
                lineId, instance, matches.Count, values.Count);
            return true; // Indicate successful read and processing for this line/instance
        }
        catch (RegexMatchTimeoutException timeoutEx)
        {
             _logger.Error(timeoutEx, "Regex match timed out (>{TimeoutSeconds}s) during Line.Read for LineId: {LineId}, Pattern: '{Pattern}'",
                RegexTimeout.TotalSeconds, lineId, this.OCR_Lines?.RegularExpressions?.RegEx ?? "Unknown");
             return false; // Treat timeout as failure for this line read
        }
        catch (Exception e)
        {
             _logger.Error(e, "Error during Line.Read for LineId: {LineId}, LineNumber: {LineNumber}", lineId, lineNumber);
             throw; // Re-throw original exception
        }
    }

     // Assuming FormatValues and SaveLineValues exist in other partial class parts
     // private void FormatValues(int instance, MatchCollection matches, Dictionary<(Fields Fields, int Instance), string> values) { ... }
     // private void SaveLineValues(int lineNumber, string section, int instance, Dictionary<(Fields Fields, int Instance), string> values) { ... }
}