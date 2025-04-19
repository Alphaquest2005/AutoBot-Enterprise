using System.Text.RegularExpressions;

namespace WaterNut.DataSpace;

public partial class Part
{
    private InvoiceLine FindStart(List<InvoiceLine> linesInvolved)
    {
        try
        {
            if (!OCR_Part.Start.Any()) return new InvoiceLine("", 0); // Implicitly started
            if (WasStarted && OCR_Part.RecuringPart?.IsComposite == true) return null; // Composite only starts once

            var textToCheck = _instanceLinesTxt.ToString();
            if (string.IsNullOrEmpty(textToCheck)) return null;

            foreach (var startCondition in OCR_Part.Start)
            {
                var options = (startCondition.RegularExpressions.MultiLine == true
                                  ? RegexOptions.Multiline
                                  : RegexOptions.Singleline)
                              | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                Match match = null;
                try
                {
                    match = Regex.Match(textToCheck, startCondition.RegularExpressions.RegEx, options,
                        TimeSpan.FromSeconds(5));
                }
                catch (RegexMatchTimeoutException ex)
                {
                    var snippetLength = Math.Min(textToCheck?.Length ?? 0, 200);
                    var inputSnippet = textToCheck?.Substring(0, snippetLength) ?? "[null]";
                    Console.WriteLine(
                        $"[OCR WARNING] Regex Timeout in FindStart: PartID={startCondition?.PartId}, Pattern='{startCondition?.RegularExpressions?.RegEx}', Input Snippet='{inputSnippet}', Error='{ex.Message}'");
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        $"[OCR ERROR] Regex Error in FindStart: PartID={startCondition?.PartId}, Pattern='{startCondition?.RegularExpressions?.RegEx}': {e}");
                    continue;
                }

                if (match?.Success ?? false)
                {
                    // Find the actual InvoiceLine that contains the start of the match
                    // Iterate through the lines provided (linesInvolved)
                    int matchStartIndex = match.Index;
                    int currentLength = 0;
                    InvoiceLine bestGuessLine = null; // Keep track of the line containing the start index

                    foreach (var line in linesInvolved)
                    {
                        var lineWithNewline = line.Line + Environment.NewLine;
                        // Check if the match starts within this line's span in the buffer
                        if (bestGuessLine == null && matchStartIndex >= currentLength &&
                            matchStartIndex < currentLength + lineWithNewline.Length)
                        {
                            bestGuessLine = line; // This line contains the start of the overall match
                            Console.WriteLine(
                                $"[OCR DEBUG] FindStart: PartID={startCondition?.PartId} - Match index {matchStartIndex} falls within Line: {line.LineNumber}");
                        }

                        // Also perform the secondary single-line check
                        var singleLineOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                        Match lineMatch = null;
                        try
                        {
                            // Use the SAME regex pattern, but force Singleline mode for the check
                            lineMatch = Regex.Match(line.Line, startCondition.RegularExpressions.RegEx,
                                singleLineOptions, TimeSpan.FromSeconds(1));
                        }
                        catch
                        {
                        } // Ignore errors/timeouts on this simpler check

                        if (lineMatch?.Success ?? false)
                        {
                            // If the single line matches, this is a strong candidate, prefer it.
                            Console.WriteLine(
                                $"[OCR DEBUG] FindStart: PartID={startCondition?.PartId} - Found trigger via single-line check on Line: {line.LineNumber}");
                            return line;
                        }

                        currentLength += lineWithNewline.Length;
                    }

                    // If no single line matched, return the line where the match index started
                    if (bestGuessLine != null)
                    {
                        Console.WriteLine(
                            $"[OCR DEBUG WARNING] FindStart: PartID={startCondition?.PartId} - No single line matched pattern. Using line containing match index: {bestGuessLine.LineNumber}");
                        return bestGuessLine;
                    }

                    // Absolute Fallback: Should ideally not be reached if match.Success was true
                    Console.WriteLine(
                        $"[OCR DEBUG ERROR] FindStart: PartID={startCondition?.PartId} - Could not pinpoint line from match index or single line check. Falling back to first line ({linesInvolved.FirstOrDefault()?.LineNumber}).");
                    return linesInvolved.FirstOrDefault();
                }
            }

            return null; // No start condition matched
        }
        catch (Exception e)
        {
            Console.WriteLine($"[OCR ERROR] Part.FindStart: Exception in Part ID {OCR_Part.Id}: {e}");
            return null;
        }
    }
}