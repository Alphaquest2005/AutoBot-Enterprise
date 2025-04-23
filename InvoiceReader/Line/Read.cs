using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any()

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        public bool Read(string line, int lineNumber, string section, string instance)
        {
            string methodName = nameof(Read);
            int? lineId = this.OCR_Lines?.Id;
            string lineName = this.OCR_Lines?.Name ?? "Unknown";

            LogEntry(methodName, lineId, lineName, line, lineNumber, section, instance);

            if (!ValidateInput(line, methodName, lineId)) return false;

            string pattern = this.OCR_Lines.RegularExpressions.RegEx;
            if (!ValidatePattern(pattern, methodName, lineId)) return false;

            bool success = false;
            try
            {
                Match match = PerformRegexMatching(line, pattern, methodName, lineId);
                if (match == null) return false;

                var values = new Dictionary<(Fields field, string instance), string>();
                FormatValues(instance, match, values);

                SaveValuesIfAny(values, lineNumber, section, instance, methodName, lineId);

                success = true;
                LogCompletion(methodName, lineId, instance, match.Success, values.Count);
            }
            catch (RegexMatchTimeoutException timeoutEx)
            {
                LogRegexTimeout(timeoutEx, methodName, lineId, pattern);
            }
            catch (Exception e)
            {
                LogUnhandledException(e, methodName, lineId, lineNumber);
            }

            LogExit(methodName, lineId, success);
            return success;
        }

        private void LogEntry(string methodName, int? lineId, string lineName, string line, int lineNumber, string section, string instance)
        {
            _logger.Verbose(
                "Entering {MethodName} for LineId: {LineId}, Name: '{LineName}', LineNumber: {LineNumber}, Section: '{Section}', Instance: {Instance}. Input line length: {Length}. Input line content (first 100 chars): '{LineContent}'",
                methodName, lineId, lineName, lineNumber, section, instance, line?.Length ?? 0, line != null ? line.Substring(0, Math.Min(line.Length, 100)) : "null");
        }

        private bool ValidateInput(string line, string methodName, int? lineId)
        {
            if (line == null)
            {
                _logger.Warning("{MethodName}: Called with null line text for LineId: {LineId}. Returning false.", methodName, lineId);
                return false;
            }

            if (this.OCR_Lines?.RegularExpressions == null)
            {
                _logger.Warning("{MethodName}: Called with null OCR_Lines or RegularExpressions for LineId: {LineId}. Returning false.", methodName, lineId);
                return false;
            }

            return true;
        }

        private bool ValidatePattern(string pattern, string methodName, int? lineId)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                _logger.Warning("{MethodName}: Regex pattern is null or empty for LineId: {LineId}. Returning false.", methodName, lineId);
                return false;
            }

            _logger.Verbose("{MethodName}: Input validation passed for LineId: {LineId}. Pattern: '{Pattern}'", methodName, lineId, pattern);
            return true;
        }

        private Match PerformRegexMatching(string line, string pattern, string methodName, int? lineId)
        {
            bool isMultiLine = this.OCR_Lines.RegularExpressions.MultiLine ?? false;
            RegexOptions options = (isMultiLine ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                   RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

            _logger.Verbose(
                "{MethodName}: Attempting Regex.Match for LineId: {LineId}, Pattern: '{Pattern}', Options: {Options}. Input line (first 100 chars): '{LineContent}'",
                methodName, lineId, pattern, options, line != null ? line.Substring(0, Math.Min(line.Length, 100)) : "null");

            Match match = Regex.Match(line, pattern, options, RegexTimeout);
            _logger.Debug("{MethodName}: Regex.Match result for LineId: {LineId}: {MatchSuccess}", methodName, lineId, match.Success);

            if (!match.Success)
            {
                _logger.Verbose("{MethodName}: No regex match found for LineId: {LineId}. Returning false.", methodName, lineId);
                return null;
            }

            LogMatchDetails(match, methodName, lineId);
            return match;
        }

        private void LogMatchDetails(Match match, string methodName, int? lineId)
        {
            _logger.Verbose("{MethodName}: LineId: {LineId} - Match found: '{MatchValue}' at position {MatchPosition}", methodName, lineId, match.Value, match.Index);
            foreach (Group group in match.Groups)
            {
                if (group.Name != "0")
                {
                    _logger.Verbose("{MethodName}: LineId: {LineId} - Group '{GroupName}': '{GroupValue}' (Success: {GroupSuccess})", methodName, lineId, group.Name, group.Value, group.Success);
                }
            }
        }

        private void SaveValuesIfAny(Dictionary<(Fields Fields, string Instance), string> values, int lineNumber, string section, string instance, string methodName, int? lineId)
        {
            if (values.Any())
            {
                SaveLineValues(lineNumber, section, instance, values);
                _logger.Debug("{MethodName}: Finished SaveLineValues for LineId: {LineId}, Instance: {Instance}", methodName, lineId, instance);
            }
            else
            {
                _logger.Warning("{MethodName}: No values extracted/formatted by FormatValues for LineId: {LineId}, Instance: {Instance}. Skipping SaveLineValues.", methodName, lineId, instance);
            }
        }

        private void LogCompletion(string methodName, int? lineId, string instance, bool matchSuccess, int valueCount)
        {
            _logger.Information(
                "{MethodName}: Completed successfully for LineId: {LineId}, Instance: {Instance}. Found {MatchCount} matches and processed {ValueCount} values.",
                methodName, lineId, instance, matchSuccess ? 1 : 0, valueCount);
        }

        private void LogRegexTimeout(RegexMatchTimeoutException timeoutEx, string methodName, int? lineId, string pattern)
        {
            _logger.Error(timeoutEx, "{MethodName}: Regex match timed out (>{TimeoutSeconds}s) for LineId: {LineId}, Pattern: '{Pattern}'", methodName, RegexTimeout.TotalSeconds, lineId, pattern);
        }

        private void LogUnhandledException(Exception e, string methodName, int? lineId, int lineNumber)
        {
            _logger.Error(e, "{MethodName}: Unhandled exception for LineId: {LineId}, LineNumber: {LineNumber}", methodName, lineId, lineNumber);
        }

        private void LogExit(string methodName, int? lineId, bool success)
        {
            _logger.Verbose("Exiting {MethodName} for LineId: {LineId}. Returning {SuccessFlag}", methodName, lineId, success);
        }
    }
}