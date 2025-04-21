using System.Text.RegularExpressions;
using OCR.Business.Entities;
using Serilog; // Added
using System; // Added
using System.Linq; // Added

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        // Assuming _logger and RegexTimeout exist from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Part>();
        // private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        private static Match IsMatch(string val, Start z) // Kept static IsMatch for potential reuse?
        {
            // Safe access for logging context
            int? partId = z?.PartId;
            string pattern = z?.RegularExpressions?.RegEx ?? "Unknown Pattern";
            _logger.Verbose(
                "Entering static IsMatch for PartId: {PartId}, Pattern: '{Pattern}'. Input string length: {Length}",
                partId, pattern, val?.Length ?? 0);

            // Null checks for critical inputs
            if (z?.RegularExpressions == null || string.IsNullOrEmpty(pattern))
            {
                _logger.Warning(
                    "Cannot perform IsMatch: Start condition or Regex pattern is null/empty for PartId: {PartId}",
                    partId);
                return System.Text.RegularExpressions.Match.Empty; // Return non-null empty match
            }

            if (val == null)
            {
                _logger.Warning(
                    "Cannot perform IsMatch: Input string 'val' is null for PartId: {PartId}, Pattern: '{Pattern}'",
                    partId, pattern);
                return System.Text.RegularExpressions.Match.Empty; // Return non-null empty match
            }

            try
            {
                // Determine options safely
                var options =
                    (z.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                    | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

                _logger.Verbose("Performing Regex.Match for PartId: {PartId}, Pattern: '{Pattern}', Options: {Options}",
                    partId, pattern, options);
                // Use the defined RegexTimeout constant (defined in Part.cs)
                // Original code used TimeSpan.FromSeconds(2) here - using the consistent timeout instead.
                Match match = Regex.Match(val, pattern, options, RegexTimeout);

                if (match.Success)
                {
                    _logger.Verbose("IsMatch SUCCESS for PartId: {PartId}, Pattern: '{Pattern}'", partId, pattern);
                }
                else
                {
                    _logger.Verbose("IsMatch FAILED for PartId: {PartId}, Pattern: '{Pattern}'", partId, pattern);
                }

                // Always return the Match object; Success property indicates the result.
                return match;
            }
            catch (RegexMatchTimeoutException timeoutEx)
            {
                _logger.Error(timeoutEx,
                    "Regex Timeout (>{Timeout}s) in static IsMatch: PartId={PartId}, Pattern='{Pattern}'",
                    RegexTimeout.TotalSeconds, partId, pattern);
                return System.Text.RegularExpressions.Match.Empty; // Return non-null empty match on timeout
            }
            catch (ArgumentException argEx) // Catch invalid patterns
            {
                _logger.Error(argEx, "Invalid Regex Pattern in static IsMatch: PartId={PartId}, Pattern='{Pattern}'",
                    partId, pattern);
                return System.Text.RegularExpressions.Match.Empty; // Return non-null empty match on invalid pattern
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unexpected error in static IsMatch: PartId={PartId}, Pattern='{Pattern}'", partId,
                    pattern);
                return System.Text.RegularExpressions.Match.Empty; // Return non-null empty match on other errors
            }
        }
    }
}