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
        string methodName = nameof(Read);
        int? lineId = this.OCR_Lines?.Id;
        _logger.Verbose("Entering {MethodName} for LineId: {LineId}, LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}. Input line length: {Length}",
            methodName, lineId, lineNumber, section, instance, line?.Length ?? 0);

        // --- Input Validation ---
        if (line == null)
        {
            _logger.Warning("{MethodName}: Called with null line text for LineId: {LineId}. Returning false.", methodName, lineId);
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null line text.", methodName, lineId);
            return false;
        }
        if (this.OCR_Lines?.RegularExpressions == null)
        {
            _logger.Warning("{MethodName}: Called with null OCR_Lines or RegularExpressions for LineId: {LineId}. Cannot match. Returning false.", methodName, lineId);
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null OCR_Lines/RegularExpressions.", methodName, lineId);
            return false;
        }
        string pattern = this.OCR_Lines.RegularExpressions.RegEx;
        if (string.IsNullOrEmpty(pattern))
        {
            _logger.Warning("{MethodName}: Regex pattern is null or empty for LineId: {LineId}. Returning false.", methodName, lineId);
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null/empty pattern.", methodName, lineId);
            return false;
        }
        _logger.Verbose("{MethodName}: Input validation passed for LineId: {LineId}.", methodName, lineId);

        bool success = false; // Default to false
        try
        {
            // --- Regex Matching ---
            bool isMultiLine = this.OCR_Lines.RegularExpressions.MultiLine ?? false;
            RegexOptions options = (isMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                  RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

            _logger.Verbose("{MethodName}: Attempting Regex.Matches for LineId: {LineId}, Pattern: '{Pattern}', Options: {Options}", methodName, lineId, pattern, options);
            MatchCollection matches = Regex.Matches(line, pattern, options, RegexTimeout); // Use defined timeout
            _logger.Debug("{MethodName}: Regex.Matches found {MatchCount} matches for LineId: {LineId}", methodName, matches.Count, lineId);

            if (matches.Count == 0)
            {
                _logger.Verbose("{MethodName}: No regex matches found for LineId: {LineId}. Returning false.", methodName, lineId);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} (No Matches). Returning false.", methodName, lineId);
                return false; // No matches, nothing to process
            }

            // --- Value Formatting ---
            var values = new Dictionary<(Fields Fields, int Instance), string>();
            _logger.Verbose("{MethodName}: Calling FormatValues for LineId: {LineId}, Instance: {Instance} with {MatchCount} matches...", methodName, lineId, instance, matches.Count);
            // FormatValues should handle its own logging
            FormatValues(instance, matches, values);
            _logger.Debug("{MethodName}: Finished FormatValues for LineId: {LineId}, Instance: {Instance}. Extracted {ValueCount} values.", methodName, lineId, instance, values.Count);

            // --- Value Saving ---
            if (values.Any())
            {
                _logger.Verbose("{MethodName}: Calling SaveLineValues for LineId: {LineId}, Instance: {Instance} with {ValueCount} values...", methodName, lineId, instance, values.Count);
                // SaveLineValues should handle its own logging
                SaveLineValues(lineNumber, section, instance, values);
                _logger.Debug("{MethodName}: Finished SaveLineValues for LineId: {LineId}, Instance: {Instance}", methodName, lineId, instance);
            }
            else
            {
                _logger.Warning("{MethodName}: No values extracted/formatted by FormatValues for LineId: {LineId}, Instance: {Instance}. Skipping SaveLineValues.", methodName, lineId, instance);
            }

            _logger.Information("{MethodName}: Completed successfully for LineId: {LineId}, Instance: {Instance}. Found {MatchCount} matches and processed {ValueCount} values.",
               methodName, lineId, instance, matches.Count, values.Count);
            success = true; // Mark as successful
        }
        catch (RegexMatchTimeoutException timeoutEx)
        {
            _logger.Error(timeoutEx, "{MethodName}: Regex match timed out (>{TimeoutSeconds}s) for LineId: {LineId}, Pattern: '{Pattern}'",
               methodName, RegexTimeout.TotalSeconds, lineId, pattern);
            success = false; // Treat timeout as failure
        }
        catch (Exception e)
        {
            _logger.Error(e, "{MethodName}: Unhandled exception for LineId: {LineId}, LineNumber: {LineNumber}", methodName, lineId, lineNumber);
            success = false; // Treat other exceptions as failure for this read attempt
            // Consider if re-throwing is appropriate depending on overall error handling strategy
            // throw;
        }

        _logger.Verbose("Exiting {MethodName} for LineId: {LineId}. Returning {SuccessFlag}", methodName, lineId, success);
        return success;
    }

     // Assuming FormatValues and SaveLineValues exist in other partial class parts
     // private void FormatValues(int instance, MatchCollection matches, Dictionary<(Fields Fields, int Instance), string> values) { ... }
     // private void SaveLineValues(int lineNumber, string section, int instance, Dictionary<(Fields Fields, int Instance), string> values) { ... }
}