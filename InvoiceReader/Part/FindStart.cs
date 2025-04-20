using System.Text.RegularExpressions;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for InvoiceLine

namespace WaterNut.DataSpace;

public partial class Part
{
    // Logger and RegexTimeout are defined in the main Part.cs partial class file.

    private InvoiceLine FindStart(List<InvoiceLine> linesInvolved)
    {
        string methodName = nameof(FindStart);
        int? partId = this.OCR_Part?.Id;
        int linesInvolvedCount = linesInvolved?.Count ?? 0;
        _logger.Verbose("Entering {MethodName} for PartId: {PartId}. Lines involved count: {LineCount}", methodName, partId, linesInvolvedCount);

        InvoiceLine resultLine = null; // Initialize result to null

        try
        {
            // --- Pre-checks ---
            if (this.OCR_Part?.Start == null || !this.OCR_Part.Start.Any())
            {
                _logger.Information("{MethodName}: PartId: {PartId} has no start conditions defined. Implicitly started.", methodName, partId);
                resultLine = new InvoiceLine("", 0); // Implicitly started
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Implicit Start). Returning dummy line.", methodName, partId);
                return resultLine;
            }

            if (WasStarted && this.OCR_Part.RecuringPart?.IsComposite == true)
            {
                _logger.Debug("{MethodName}: PartId: {PartId} is composite and WasStarted is true. Returning null (already started).", methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Composite Already Started). Returning null.", methodName, partId);
                return null; // Composite only starts once
            }

            var textToCheck = _instanceLinesTxt?.ToString();
            if (string.IsNullOrEmpty(textToCheck))
            {
                _logger.Debug("{MethodName}: PartId: {PartId} - Text buffer (_instanceLinesTxt) is null or empty. Cannot find start. Returning null.", methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Empty Buffer). Returning null.", methodName, partId);
                return null;
            }
            _logger.Verbose("{MethodName}: PartId: {PartId} - Text buffer length: {Length}", methodName, partId, textToCheck.Length);

            if (linesInvolved == null || !linesInvolved.Any())
            {
                _logger.Warning("{MethodName}: PartId: {PartId} - linesInvolved list is null or empty. Buffer check will proceed, but line location might be inaccurate.", methodName, partId);
            }

            // --- Iterate Through Start Conditions ---
            _logger.Debug("{MethodName}: PartId: {PartId} - Iterating through {Count} start conditions.", methodName, partId, this.OCR_Part.Start.Count);
            int conditionIndex = 0;
            foreach (var startCondition in this.OCR_Part.Start.Where(sc => sc?.RegularExpressions != null)) // Safe iteration
            {
                conditionIndex++;
                int startConditionId = startCondition.Id;
                string pattern = startCondition.RegularExpressions.RegEx;
                _logger.Verbose("{MethodName}: PartId: {PartId} - Checking StartCondition {Index}/{Total} (Id: {StartConditionId})", methodName, partId, conditionIndex, this.OCR_Part.Start.Count, startConditionId);

                if (string.IsNullOrEmpty(pattern))
                {
                    _logger.Warning("{MethodName}: PartId: {PartId} - StartConditionId: {StartConditionId} has null or empty regex pattern. Skipping.", methodName, partId, startConditionId);
                    continue;
                }

                // --- Buffer Match ---
                var options = (startCondition.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                              | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                Match bufferMatch = null;
                _logger.Verbose("{MethodName}: PartId: {PartId} - Matching buffer text against Pattern: '{Pattern}', Options: {Options}", methodName, partId, pattern, options);
                try
                {
                    bufferMatch = Regex.Match(textToCheck, pattern, options, RegexTimeout);
                }
                catch (RegexMatchTimeoutException ex)
                {
                    var snippetLength = Math.Min(textToCheck.Length, 200);
                    var inputSnippet = textToCheck.Substring(0, snippetLength);
                    _logger.Warning(ex, "{MethodName}: Regex Timeout (>{Timeout}s) in buffer match: PartId={PartId}, StartConditionId={StartConditionId}, Pattern='{Pattern}', Input Snippet='{InputSnippet}'",
                       methodName, RegexTimeout.TotalSeconds, partId, startConditionId, pattern, inputSnippet);
                    continue; // Try next start condition
                }
                catch (ArgumentException argEx)
                {
                    _logger.Error(argEx, "{MethodName}: Invalid Regex Pattern in buffer match: PartId={PartId}, StartConditionId={StartConditionId}, Pattern='{Pattern}'",
                       methodName, partId, startConditionId, pattern);
                    continue;
                }
                catch (Exception e)
                {
                    _logger.Error(e, "{MethodName}: Regex Error in buffer match: PartId={PartId}, StartConditionId={StartConditionId}, Pattern='{Pattern}'",
                       methodName, partId, startConditionId, pattern);
                    continue;
                }

                if (bufferMatch.Success)
                {
                    _logger.Information("{MethodName}: PartId: {PartId} - Buffer match SUCCESS for StartConditionId: {StartConditionId} at Index: {MatchIndex}", methodName, partId, startConditionId, bufferMatch.Index);

                    // --- Locate Triggering Line ---
                    int matchStartIndex = bufferMatch.Index;
                    int currentLength = 0;
                    InvoiceLine bestGuessLine = null; // Line containing the start index of the buffer match

                    _logger.Verbose("{MethodName}: PartId: {PartId} - Locating line containing buffer match index {MatchIndex}...", methodName, partId, matchStartIndex);
                    if (linesInvolved != null)
                    {
                        int lineIndex = 0;
                        foreach (var line in linesInvolved)
                        {
                            lineIndex++;
                            if (line == null) continue;
                            string currentLineText = line.Line ?? "";
                            var lineWithNewline = currentLineText + Environment.NewLine;
                            int lineEndIndex = currentLength + lineWithNewline.Length;

                            // Check if match starts within this line's span
                            if (bestGuessLine == null && matchStartIndex >= currentLength && matchStartIndex < lineEndIndex)
                            {
                                bestGuessLine = line;
                                _logger.Verbose("{MethodName}: PartId: {PartId} - Buffer match index {MatchIndex} falls within Line {Index}/{Total} (Num: {LineNumber}, Span {StartIndex}-{EndIndex})",
                                   methodName, partId, matchStartIndex, lineIndex, linesInvolvedCount, line.LineNumber, currentLength, lineEndIndex - 1);
                            }

                            // --- Secondary Single-Line Check ---
                            var singleLineOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;
                            Match lineMatch = null;
                            _logger.Verbose("{MethodName}: PartId: {PartId} - Performing single-line check on Line {Index}/{Total} (Num: {LineNumber}) using Pattern: '{Pattern}'", methodName, partId, lineIndex, linesInvolvedCount, line.LineNumber, pattern);
                            try
                            {
                                lineMatch = Regex.Match(currentLineText, pattern, singleLineOptions, TimeSpan.FromSeconds(1)); // Shorter timeout
                            }
                            catch (RegexMatchTimeoutException) { _logger.Verbose("{MethodName}: Single-line regex check timed out for LineNumber: {LineNumber}", methodName, line.LineNumber); }
                            catch (ArgumentException argEx) { _logger.Warning(argEx, "{MethodName}: Invalid Regex Pattern during single-line check for LineNumber: {LineNumber}", methodName, line.LineNumber); }
                            catch (Exception lineMatchEx) { _logger.Warning(lineMatchEx, "{MethodName}: Error during single-line regex check for LineNumber: {LineNumber}", methodName, line.LineNumber); }

                            if (lineMatch?.Success ?? false)
                            {
                                _logger.Information("{MethodName}: PartId: {PartId} - Found start trigger via single-line check on LineNumber: {LineNumber}. Returning this line.", methodName, partId, line.LineNumber);
                                resultLine = line; // Set result
                                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Single Line Match). Returning LineNumber: {LineNumber}", methodName, partId, resultLine.LineNumber);
                                return resultLine; // Return immediately
                            }
                            // --- End Secondary Single-Line Check ---

                            currentLength = lineEndIndex; // Update position
                        } // End foreach line
                    }
                    else
                    {
                        _logger.Warning("{MethodName}: PartId: {PartId} - Cannot perform line location checks because linesInvolved list is null.", methodName, partId);
                    }

                    // --- Post-Loop Decision ---
                    if (bestGuessLine != null)
                    {
                        _logger.Warning("{MethodName}: PartId: {PartId} - No single line matched pattern for StartConditionId: {StartConditionId}. Using best guess line containing buffer match index: {LineNumber}",
                           methodName, partId, startConditionId, bestGuessLine.LineNumber);
                        resultLine = bestGuessLine; // Set result
                        _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Best Guess Line). Returning LineNumber: {LineNumber}", methodName, partId, resultLine.LineNumber);
                        return resultLine;
                    }

                    // Absolute Fallback
                    var firstLine = linesInvolved?.FirstOrDefault();
                    _logger.Error("{MethodName}: PartId: {PartId} - Could not pinpoint start line from buffer match index or single line check for StartConditionId: {StartConditionId}. Falling back to first line: {LineNumber}",
                       methodName, partId, startConditionId, firstLine?.LineNumber ?? -1);
                    resultLine = firstLine; // Set result (might be null)
                    _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (Fallback Line). Returning LineNumber: {LineNumber}", methodName, partId, resultLine?.LineNumber ?? -1);
                    return resultLine;
                }
                else
                {
                    _logger.Verbose("{MethodName}: PartId: {PartId} - Buffer match FAILED for StartConditionId: {StartConditionId}", methodName, partId, startConditionId);
                }
            } // End foreach startCondition

            _logger.Debug("{MethodName}: PartId: {PartId} - No start condition matched the text buffer. Returning null.", methodName, partId);
            resultLine = null; // Explicitly set result to null
        }
        catch (Exception e)
        {
            _logger.Error(e, "{MethodName}: Unhandled exception for PartId: {PartId}", methodName, partId);
            resultLine = null; // Ensure null on error
        }

        _logger.Verbose("Exiting {MethodName} for PartId: {PartId}. Returning LineNumber: {LineNumber}", methodName, partId, resultLine?.LineNumber ?? -1);
        return resultLine;
    }
}