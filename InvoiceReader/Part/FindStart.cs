using System.Text.RegularExpressions;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for InvoiceLine

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        private InvoiceLine FindStart(List<InvoiceLine> linesInvolved)
        {
            string methodName = nameof(FindStart);
            int? partId = this.OCR_Part?.Id;
            int linesInvolvedCount = linesInvolved?.Count ?? 0;
            _logger.Verbose("Entering {MethodName} for PartId: {PartId}. Lines involved count: {LineCount}", methodName, partId, linesInvolvedCount);

            InvoiceLine resultLine = null;

            try
            {
                if (IsImplicitStart(methodName, partId, out resultLine) || IsCompositeAlreadyStarted(methodName, partId))
                {
                    return resultLine;
                }

                var textToCheck = _instanceLinesTxt?.ToString();
                if (IsBufferEmpty(methodName, partId, textToCheck))
                {
                    return null;
                }

                if (linesInvolved == null || !linesInvolved.Any())
                {
                    _logger.Warning("{MethodName}: PartId: {PartId} - linesInvolved list is null or empty. Buffer check will proceed, but line location might be inaccurate.", methodName, partId);
                }

                resultLine = ProcessStartConditions(methodName, partId, textToCheck, linesInvolved);
            }
            catch (Exception e)
            {
                _logger.Error(e, "{MethodName}: Unhandled exception for PartId: {PartId}", methodName, partId);
            }

            _logger.Verbose("Exiting {MethodName} for PartId: {PartId}. Returning LineNumber: {LineNumber}", methodName, partId, resultLine?.LineNumber ?? -1);
            return resultLine;
        }

        private bool IsImplicitStart(string methodName, int? partId, out InvoiceLine resultLine)
        {
            resultLine = null;
            if (this.OCR_Part?.Start == null || !this.OCR_Part.Start.Any())
            {
                _logger.Information("{MethodName}: PartId: {PartId} has no start conditions defined. Implicitly started.", methodName, partId);
                resultLine = new InvoiceLine("", 0);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Implicit Start). Returning dummy line.", methodName, partId);
                return true;
            }
            return false;
        }

        private bool IsCompositeAlreadyStarted(string methodName, int? partId)
        {
            if (WasStarted && this.OCR_Part.RecuringPart?.IsComposite == true)
            {
                _logger.Debug("{MethodName}: PartId: {PartId} is composite and WasStarted is true. Returning null (already started).", methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Composite Already Started). Returning null.", methodName, partId);
                return true;
            }
            return false;
        }

        private bool IsBufferEmpty(string methodName, int? partId, string textToCheck)
        {
            if (string.IsNullOrEmpty(textToCheck))
            {
                _logger.Debug("{MethodName}: PartId: {PartId} - Text buffer (_instanceLinesTxt) is null or empty. Cannot find start. Returning null.", methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Empty Buffer). Returning null.", methodName, partId);
                return true;
            }

            _logger.Verbose("{MethodName}: PartId: {PartId} - Text buffer length: {Length}", methodName, partId, textToCheck.Length);
            return false;
        }

        private InvoiceLine ProcessStartConditions(string methodName, int? partId, string textToCheck, List<InvoiceLine> linesInvolved)
        {
            _logger.Debug("{MethodName}: PartId: {PartId} - Iterating through {Count} start conditions.", methodName, partId, this.OCR_Part.Start.Count);

            foreach (var startCondition in this.OCR_Part.Start.Where(sc => sc?.RegularExpressions != null))
            {
                if (TryMatchStartCondition(methodName, partId, textToCheck, linesInvolved, startCondition, out var resultLine))
                {
                    return resultLine;
                }
            }

            _logger.Debug("{MethodName}: PartId: {PartId} - No start condition matched the text buffer. Returning null.", methodName, partId);
            return null;
        }

        private bool TryMatchStartCondition(string methodName, int? partId, string textToCheck, List<InvoiceLine> linesInvolved, dynamic startCondition, out InvoiceLine resultLine)
        {
            resultLine = null;
            string pattern = startCondition.RegularExpressions.RegEx;
            if (string.IsNullOrEmpty(pattern))
            {
                _logger.Warning("{MethodName}: PartId: {PartId} - StartConditionId: {StartConditionId} has null or empty regex pattern. Skipping.", methodName, partId, startCondition.Id);
                return false;
            }

            var options = (startCondition.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                          | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

            if (TryMatchBuffer(methodName, partId, textToCheck, pattern, options, out var bufferMatch))
            {
                resultLine = LocateTriggeringLine(methodName, partId, bufferMatch, linesInvolved, pattern);
                return true;
            }

            return false;
        }

        private bool TryMatchBuffer(string methodName, int? partId, string textToCheck, string pattern, RegexOptions options, out Match bufferMatch)
        {
            bufferMatch = null;
            try
            {
                bufferMatch = Regex.Match(textToCheck, pattern, options, RegexTimeout);
                if (bufferMatch.Success)
                {
                    _logger.Information("{MethodName}: PartId: {PartId} - Buffer match SUCCESS for Pattern: '{Pattern}' at Index: {MatchIndex}", methodName, partId, pattern, bufferMatch.Index);
                    return true;
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                LogRegexTimeout(methodName, partId, textToCheck, pattern, ex);
            }
            catch (ArgumentException argEx)
            {
                _logger.Error(argEx, "{MethodName}: Invalid Regex Pattern in buffer match: PartId={PartId}, Pattern='{Pattern}'", methodName, partId, pattern);
            }
            catch (Exception e)
            {
                _logger.Error(e, "{MethodName}: Regex Error in buffer match: PartId={PartId}, Pattern='{Pattern}'", methodName, partId, pattern);
            }

            return false;
        }

        private void LogRegexTimeout(string methodName, int? partId, string textToCheck, string pattern, RegexMatchTimeoutException ex)
        {
            var snippetLength = Math.Min(textToCheck.Length, 200);
            var inputSnippet = textToCheck.Substring(0, snippetLength);
            _logger.Warning(ex, "{MethodName}: Regex Timeout (>{Timeout}s) in buffer match: PartId={PartId}, Pattern='{Pattern}', Input Snippet='{InputSnippet}'", methodName, RegexTimeout.TotalSeconds, partId, pattern, inputSnippet);
        }

        private InvoiceLine LocateTriggeringLine(string methodName, int? partId, Match bufferMatch, List<InvoiceLine> linesInvolved, string pattern)
        {
            int matchStartIndex = bufferMatch.Index;
            int currentLength = 0;
            InvoiceLine bestGuessLine = null;

            if (linesInvolved != null && linesInvolved.Any())
            {
                foreach (var line in linesInvolved)
                {
                    if (line == null) continue;

                    string currentLineText = line.Line ?? "";
                    var lineWithNewline = currentLineText + Environment.NewLine;
                    int lineEndIndex = currentLength + lineWithNewline.Length;

                    if (bestGuessLine == null && matchStartIndex >= currentLength && matchStartIndex < lineEndIndex)
                    {
                        bestGuessLine = line;
                    }

                    if (TrySingleLineMatch(methodName, partId, line, pattern, out var singleLineMatch))
                    {
                        return line;
                    }

                    currentLength = lineEndIndex;
                }
            }

            return bestGuessLine ?? linesInvolved?.FirstOrDefault();
        }

        private bool TrySingleLineMatch(string methodName, int? partId, InvoiceLine line, string pattern, out Match singleLineMatch)
        {
            singleLineMatch = null;
            try
            {
                singleLineMatch = Regex.Match(line.Line ?? "", pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
                if (singleLineMatch.Success)
                {
                    _logger.Information("{MethodName}: PartId: {PartId} - Found start trigger via single-line check on LineNumber: {LineNumber}.", methodName, partId, line.LineNumber);
                    return true;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                _logger.Verbose("{MethodName}: Single-line regex check timed out for LineNumber: {LineNumber}", methodName, line.LineNumber);
            }
            catch (ArgumentException argEx)
            {
                _logger.Warning(argEx, "{MethodName}: Invalid Regex Pattern during single-line check for LineNumber: {LineNumber}", methodName, line.LineNumber);
            }
            catch (Exception lineMatchEx)
            {
                _logger.Warning(lineMatchEx, "{MethodName}: Error during single-line regex check for LineNumber: {LineNumber}", methodName, line.LineNumber);
            }

            return false;
        }
    }
}