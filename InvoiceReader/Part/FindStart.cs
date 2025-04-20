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
        int? partId = this.OCR_Part?.Id;
        _logger.Debug("Entering FindStart for PartId: {PartId}. Lines involved count: {LineCount}", partId, linesInvolved?.Count ?? 0);

        try
        {
            // Check if start conditions exist safely
            if (this.OCR_Part?.Start == null || !this.OCR_Part.Start.Any())
            {
                 _logger.Verbose("PartId: {PartId} has no start conditions defined. Implicitly started. Returning dummy line.", partId);
                 return new InvoiceLine("", 0); // Implicitly started
            }

            // Check if already started (for composite parts) safely
            if (WasStarted && this.OCR_Part.RecuringPart?.IsComposite == true)
            {
                 _logger.Debug("PartId: {PartId} is composite and WasStarted is true. Returning null (already started).", partId);
                 return null; // Composite only starts once
            }

            // Check text buffer safely
            var textToCheck = _instanceLinesTxt?.ToString(); // Safe access
            if (string.IsNullOrEmpty(textToCheck))
            {
                 _logger.Debug("PartId: {PartId} - Text buffer (_instanceLinesTxt) is null or empty. Cannot find start. Returning null.", partId);
                 return null;
            }
             _logger.Verbose("PartId: {PartId} - Text buffer length: {Length}", partId, textToCheck.Length);

             // Check linesInvolved list
             if (linesInvolved == null || !linesInvolved.Any())
             {
                  _logger.Warning("PartId: {PartId} - linesInvolved list is null or empty. Cannot determine start line accurately from list.", partId);
                  // Depending on logic, might return null or proceed with buffer check only
             }


            _logger.Debug("PartId: {PartId} - Iterating through {Count} start conditions.", partId, this.OCR_Part.Start.Count);
            foreach (var startCondition in this.OCR_Part.Start.Where(sc => sc?.RegularExpressions != null)) // Safe iteration
            {
                int startConditionId = startCondition.Id;
                string pattern = startCondition.RegularExpressions.RegEx;
                 _logger.Verbose("PartId: {PartId} - Checking StartConditionId: {StartConditionId}", partId, startConditionId);

                 if (string.IsNullOrEmpty(pattern))
                 {
                      _logger.Warning("PartId: {PartId} - StartConditionId: {StartConditionId} has null or empty regex pattern. Skipping.", partId, startConditionId);
                      continue;
                 }

                // Determine options for buffer match safely
                var options = (startCondition.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                              | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                Match match = null;

                 _logger.Verbose("PartId: {PartId} - Matching buffer text against Pattern: '{Pattern}', Options: {Options}", partId, pattern, options);
                try
                {
                    match = Regex.Match(textToCheck, pattern, options, RegexTimeout); // Use defined timeout
                }
                catch (RegexMatchTimeoutException ex)
                {
                    var snippetLength = Math.Min(textToCheck.Length, 200);
                    var inputSnippet = textToCheck.Substring(0, snippetLength);
                     _logger.Warning(ex, "Regex Timeout (>{Timeout}s) in FindStart buffer match: PartId={PartId}, StartConditionId={StartConditionId}, Pattern='{Pattern}', Input Snippet='{InputSnippet}'",
                        RegexTimeout.TotalSeconds, partId, startConditionId, pattern, inputSnippet);
                    continue; // Try next start condition
                }
                catch (ArgumentException argEx) // Catch invalid patterns
                {
                     _logger.Error(argEx, "Invalid Regex Pattern in FindStart buffer match: PartId={PartId}, StartConditionId={StartConditionId}, Pattern='{Pattern}'",
                        partId, startConditionId, pattern);
                     continue; // Try next start condition
                }
                catch (Exception e) // Catch other regex errors
                {
                     _logger.Error(e, "Regex Error in FindStart buffer match: PartId={PartId}, StartConditionId={StartConditionId}, Pattern='{Pattern}'",
                        partId, startConditionId, pattern);
                    continue; // Try next start condition
                }

                if (match.Success) // Null check removed as Regex.Match never returns null
                {
                     _logger.Information("PartId: {PartId} - Buffer match SUCCESS for StartConditionId: {StartConditionId} at Index: {MatchIndex}", partId, startConditionId, match.Index);

                     // Find the actual InvoiceLine that contains the start of the match
                     int matchStartIndex = match.Index;
                     int currentLength = 0;
                     InvoiceLine bestGuessLine = null; // Keep track of the line containing the start index

                     _logger.Verbose("PartId: {PartId} - Locating line containing buffer match index {MatchIndex}...", partId, matchStartIndex);
                     if (linesInvolved != null) // Check if we have lines to check
                     {
                         foreach (var line in linesInvolved)
                         {
                             if (line == null) continue; // Skip null lines
                             // Use line.Line safely
                             string currentLineText = line.Line ?? "";
                             var lineWithNewline = currentLineText + Environment.NewLine; // Assume Environment.NewLine was used to build buffer
                             int lineEndIndex = currentLength + lineWithNewline.Length;

                             // Check if the match starts within this line's span in the buffer
                             if (bestGuessLine == null && matchStartIndex >= currentLength && matchStartIndex < lineEndIndex)
                             {
                                 bestGuessLine = line; // This line contains the start of the overall match
                                  _logger.Verbose("PartId: {PartId} - Match index {MatchIndex} falls within LineNumber: {LineNumber} (Span {StartIndex}-{EndIndex})",
                                     partId, matchStartIndex, line.LineNumber, currentLength, lineEndIndex -1);
                                 // Don't break yet, continue to check single lines
                             }

                             // Also perform the secondary single-line check
                             var singleLineOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline; // Always use Singleline for individual line check
                             Match lineMatch = null;
                              _logger.Verbose("PartId: {PartId} - Performing single-line check on LineNumber: {LineNumber} using Pattern: '{Pattern}'", partId, line.LineNumber, pattern);
                             try
                             {
                                 // Use the SAME regex pattern, but force Singleline mode for the check
                                 lineMatch = Regex.Match(currentLineText, pattern, singleLineOptions, TimeSpan.FromSeconds(1)); // Shorter timeout for single line
                             }
                             catch (RegexMatchTimeoutException) { _logger.Verbose("Single-line regex check timed out for LineNumber: {LineNumber}", line.LineNumber); /* Ignore timeout on single line check */ }
                             catch (ArgumentException argEx) { _logger.Warning(argEx, "Invalid Regex Pattern during single-line check for LineNumber: {LineNumber}", line.LineNumber); }
                             catch (Exception lineMatchEx) { _logger.Warning(lineMatchEx, "Error during single-line regex check for LineNumber: {LineNumber}", line.LineNumber); }

                             if (lineMatch?.Success ?? false) // Safe check on lineMatch
                             {
                                  _logger.Information("PartId: {PartId} - Found start trigger via single-line check on LineNumber: {LineNumber}. Returning this line.", partId, line.LineNumber);
                                 return line; // Return immediately on single-line match
                             }

                             currentLength = lineEndIndex; // Update position for next line
                         } // End foreach line in linesInvolved
                     } else {
                          _logger.Warning("PartId: {PartId} - Cannot perform line location checks because linesInvolved list is null.", partId);
                     }


                     // If no single line matched, return the line where the match index started (best guess)
                     if (bestGuessLine != null)
                     {
                          _logger.Warning("PartId: {PartId} - No single line matched pattern for StartConditionId: {StartConditionId}. Using best guess line containing buffer match index: {LineNumber}",
                             partId, startConditionId, bestGuessLine.LineNumber);
                         return bestGuessLine;
                     }

                     // Absolute Fallback: Reached if buffer matched but couldn't find line (e.g., linesInvolved was null/empty or logic error)
                     var firstLine = linesInvolved?.FirstOrDefault();
                      _logger.Error("PartId: {PartId} - Could not pinpoint start line from buffer match index or single line check for StartConditionId: {StartConditionId}. Falling back to first line: {LineNumber}",
                         partId, startConditionId, firstLine?.LineNumber ?? -1);
                     return firstLine; // Return first line as fallback (might be null)
                } else {
                     _logger.Verbose("PartId: {PartId} - Buffer match FAILED for StartConditionId: {StartConditionId}", partId, startConditionId);
                }
            } // End foreach startCondition

             _logger.Debug("PartId: {PartId} - No start condition matched the text buffer. Returning null.", partId);
            return null; // No start condition matched
        }
        catch (Exception e)
        {
             _logger.Error(e, "Unexpected error in FindStart for PartId: {PartId}", partId);
             return null; // Return null on error
        }
    }
}