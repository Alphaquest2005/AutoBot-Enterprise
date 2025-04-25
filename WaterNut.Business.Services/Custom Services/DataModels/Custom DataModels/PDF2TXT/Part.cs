#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
using OCR.Business.Entities;
using sun.security.jca; // Note: Consider if this using is necessary or if it's a remnant

namespace WaterNut.DataSpace
{
    public class Part
    {
        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>(); // Holds ALL lines passed to this part within its current parent context
        private readonly StringBuilder _instanceLinesTxt = new StringBuilder(); // Text buffer specific to the current instance being processed

        public Parts OCR_Part;

        public Part(Parts part)
        {
            StartCount = part.Start.Select(x => x.RegularExpressions.RegEx).Count();
            EndCount = part.End.Select(x => x.RegularExpressions.RegEx).Count();
            OCR_Part = part;
            ChildParts = part.ParentParts.Select(x => new Part(x.ChildPart)).ToList();
            Lines = part.Lines.Where(x => x.IsActive ?? true).Select(x => new Line(x)).ToList();
            lastLineRead = 0;
        }

        public List<Part> ChildParts { get; }
        public List<Line> Lines { get; }
        private int EndCount { get; }
        private int StartCount { get; }

        public bool Success => AllRequiredFieldsFilled() && NoFailedLines() && AllChildPartsSucceded();

        private bool AllRequiredFieldsFilled() => Lines.All(x => !x.Values.SelectMany(z => z.Value).Any(z => z.Key.fields.IsRequired && string.IsNullOrEmpty(z.Value?.ToString())));
        private bool NoFailedLines() => !this.FailedLines.Any();
        private bool AllChildPartsSucceded() => ChildParts.All(x => x.Success);

        public List<Line> FailedLines => Lines.Where(x => x.OCR_Lines.Fields.Any(z => z.IsRequired && z.FieldValue?.Value == null) && x.Values.Count == 0)
                                            .Union(ChildParts.SelectMany(x => x.FailedLines)).ToList();

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields => Lines.SelectMany(x => x.FailedFields).ToList();
        public List<Line> AllLines => Lines.Union(ChildParts.SelectMany(x => x.AllLines)).DistinctBy(x => x.OCR_Lines.Id).ToList();
        public bool WasStarted => _startlines.Any();

        private int lastLineRead = 0;
        private int _instance = 1; // Internal instance counter
        private int _lastProcessedParentInstance = 0; // Track the last parent instance processed by this child
        private int _currentInstanceStartLineNumber = -1; // Track the line number where the current instance started

        public void Read(List<InvoiceLine> newlines, string Section, int textCount, int? parentInstance = null)
        {
            int currentEffectiveInstance = parentInstance ?? _instance;

            // HACK: Reset child state if parent instance has advanced
            if (parentInstance.HasValue && parentInstance.Value > _lastProcessedParentInstance)
            {
                //Console.WriteLine($"[OCR DEBUG HACK] Part.Read: Child Part ID {OCR_Part.Id} detected new parent instance ({parentInstance.Value} > {_lastProcessedParentInstance}). Resetting child state.");
                ResetInternalState();
                _lastProcessedParentInstance = parentInstance.Value;
            }

            Console.WriteLine($"[OCR DEBUG] Part.Read: Entry for Part ID {OCR_Part.Id}, Effective Instance {currentEffectiveInstance} (Internal: {_instance}, Parent: {parentInstance?.ToString() ?? "N/A"}), Section '{Section}'. Of total Lines {textCount}, Processing {newlines.Count} new lines starting from line {newlines.FirstOrDefault()?.LineNumber}. Current buffer size: {_lines.Count}. lastLineRead = {lastLineRead}. WasStarted = {WasStarted}");

            try
            {
                // --- Line Accumulation ---
                var uniqueNewLines = newlines.Where(nl => !_lines.Any(l => l.LineNumber == nl.LineNumber)).ToList();
                _lines.AddRange(uniqueNewLines);
                _lines.Sort((a, b) => a.LineNumber.CompareTo(b.LineNumber));

                var linesForThisStep = newlines.Where(x => lastLineRead < x.LineNumber).ToList();
                if (linesForThisStep.Any())
                {
                    lastLineRead = linesForThisStep.Last().LineNumber;
                }
                // --- End Line Accumulation ---

                // --- Build Instance Text Buffer ---
                var linesForInstanceBuffer = _lines.Where(l => _currentInstanceStartLineNumber == -1 || l.LineNumber >= _currentInstanceStartLineNumber).TakeLast(10).ToList();
                _instanceLinesTxt.Clear();
                linesForInstanceBuffer.ForEach(l => _instanceLinesTxt.AppendLine(l.Line));
                // --- End Instance Text Buffer ---

                // --- Start Detection ---
                // FindStart returns the triggering line or null
                var triggeringLine = FindStart(linesForInstanceBuffer); // Pass relevant lines to FindStart
                bool startFound = triggeringLine != null;
                //Console.WriteLine($"[OCR DEBUG] Part.Read: Part ID {OCR_Part.Id}, Effective Instance {currentEffectiveInstance}: FindStart result = {startFound} {(startFound ? $"(Line: {triggeringLine.LineNumber})" : "")}. WasStarted (before processing start) = {WasStarted}");

                bool justStartedThisCall = false;
                // --- End Start Detection ---

                // --- Reset Logic for RECURRING CHILD Start-to-Start ---
                bool isRecurringChildStartToStart = parentInstance != null && startFound && WasStarted && (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite);

                if (isRecurringChildStartToStart)
                {
                    //Console.WriteLine($"[OCR DEBUG] Part.Read: PARTIAL Resetting state for RECURRING CHILD Part ID {OCR_Part.Id} (Internal Instance {_instance}) within Parent Instance {parentInstance}. Found subsequent start on line {triggeringLine.LineNumber}.");
                    _startlines.Clear();
                    _endlines.Clear();
                    _instanceLinesTxt.Clear();
                    _currentInstanceStartLineNumber = -1;

                    if (triggeringLine != null && StartCount > 0)
                    {
                        _startlines.Add(triggeringLine);
                        _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                        _instanceLinesTxt.AppendLine(triggeringLine.Line);
                        //Console.WriteLine($"[OCR DEBUG] Part.Read: Added triggering line {triggeringLine.LineNumber} as first start line for new recurring child instance.");
                        justStartedThisCall = true;
                    }
                    else if (StartCount > 0)
                    {
                        //Console.WriteLine($"[OCR DEBUG ERROR] Part.Read: Failed to add start line after recurring child reset (triggeringLine was null or StartCount is 0).");
                    }
                }
                // --- End Recurring Child Reset ---

                // --- Initial Start Logic ---
                if (startFound && !WasStarted && !justStartedThisCall)
                {
                    if (triggeringLine != null && StartCount > 0)
                    {
                        _startlines.Add(triggeringLine);
                        _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                        // Ensure buffer has the line
                        if (_instanceLinesTxt.Length == 0 || !_instanceLinesTxt.ToString().Contains(triggeringLine.Line))
                        {
                            _instanceLinesTxt.Clear();
                            _instanceLinesTxt.AppendLine(triggeringLine.Line);
                        }
                        //Console.WriteLine($"[OCR DEBUG] Part.Read: Added triggering line {triggeringLine.LineNumber} as INITIAL start line for instance (Effective: {currentEffectiveInstance}).");
                        justStartedThisCall = true;
                        if (parentInstance != null)
                        {
                            //Console.WriteLine($"[OCR DEBUG] Part.Read: Child Part ID {OCR_Part.Id}: Detected INITIAL start within parent's context (Parent Instance {parentInstance}). Using parent's effective instance {currentEffectiveInstance}.");
                        }
                    }
                    else if (StartCount > 0)
                    {
                        //Console.WriteLine($"[OCR DEBUG ERROR] Part.Read: Failed to add initial start line (triggeringLine was null or StartCount is 0).");
                    }
                }
                // --- End Initial Start Logic ---

                // --- Main Processing Block ---
                bool currentlyStarted = _startlines.Any();
                if (currentlyStarted && _startlines.Count() == StartCount && ((_endlines.Count < EndCount && EndCount > 0) || EndCount == 0))
                {
                    // Process Child Parts
                    ChildParts.ForEach(x => {
                        //Console.WriteLine($"[OCR DEBUG] Part.Read: Part ID {OCR_Part.Id}, Effective Instance {currentEffectiveInstance}: Calling Read() on Child Part ID {x.OCR_Part.Id} with {linesForThisStep.Count} lines for THIS STEP, passing instance {currentEffectiveInstance}.");
                        x.Read(new List<InvoiceLine>(linesForThisStep), Section,linesForThisStep.Count, currentEffectiveInstance);
                    });

                    // Process Own Lines
                    Lines.ForEach(x =>
                    {
                        if (!linesForThisStep.Any()) return;

                        var instanceLines = _lines.Where(l => l.LineNumber >= _currentInstanceStartLineNumber).TakeLast(10).ToList();
                        var instanceText = _instanceLinesTxt.ToString();

                        if (x.OCR_Lines.RegularExpressions.MultiLine == true)
                        {
                            var relevantLines = instanceLines.TakeLast(x.OCR_Lines.RegularExpressions.MaxLines ?? 10).ToList();
                            var lineText = relevantLines.Select(z => z.Line).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
                            //Console.WriteLine($"[OCR DEBUG] Part.Read: Part ID {OCR_Part.Id}, Effective Instance {currentEffectiveInstance}: Reading multi-line Line ID {x.OCR_Lines.Id}");
                            x.Read(lineText, relevantLines.FirstOrDefault()?.LineNumber ?? linesForThisStep.First().LineNumber, Section, currentEffectiveInstance);
                        }
                        else
                        {
                            //Console.WriteLine($"[OCR DEBUG] Part.Read: Part ID {OCR_Part.Id}, Effective Instance {currentEffectiveInstance}: Reading single-line Line ID {x.OCR_Lines.Id}");
                            x.Read(linesForThisStep.Last().Line, linesForThisStep.Last().LineNumber, Section, currentEffectiveInstance);
                        }
                    });
                }
                // --- End Main Processing Block ---

                // --- End Line Detection ---
                currentlyStarted = _startlines.Any();
                if (currentlyStarted && !justStartedThisCall && _startlines.Count() == StartCount && _endlines.Count() < EndCount && EndCount > 0)
                {
                    if (OCR_Part.End.Any(z => Regex.Match(_instanceLinesTxt.ToString(), z.RegularExpressions.RegEx,
                               (z.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline) | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2)).Success))
                    {
                        var instanceLines = _lines.Where(l => l.LineNumber >= _currentInstanceStartLineNumber).ToList();
                        if (instanceLines.Any())
                        {
                            _endlines.Add(instanceLines.Last());
                            //Console.WriteLine($"[OCR DEBUG] Part.Read: Added line {instanceLines.Last().LineNumber} as end line for instance (Effective: {currentEffectiveInstance}).");
                        }
                    }
                }
                // --- End End Line Detection ---

                // --- Reset Logic for TOP-LEVEL Start-to-Start ---
                currentlyStarted = _startlines.Any();
                bool isTopLevelRecurringStartToStart = parentInstance == null && startFound && currentlyStarted && !justStartedThisCall && (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite);

                if (isTopLevelRecurringStartToStart)
                {
                    // Use the triggeringLine captured earlier
                    //Console.WriteLine($"[OCR DEBUG] Part.Read: FULL Resetting state for TOP-LEVEL recurring Part ID {OCR_Part.Id} (Instance {_instance} completed, {_instance + 1} starting). Triggered by line {triggeringLine?.LineNumber}.");

                    ResetInternalState(); // Full reset

                    if (triggeringLine != null && StartCount > 0)
                    {
                        _startlines.Add(triggeringLine);
                        _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                        _instanceLinesTxt.AppendLine(triggeringLine.Line);
                        //Console.WriteLine($"[OCR DEBUG] Part.Read: Added triggering line {triggeringLine.LineNumber} as first start line for new top-level instance.");
                    }
                    else if (StartCount > 0)
                    {
                        //Console.WriteLine($"[OCR DEBUG WARNING] Part.Read: Could not determine triggering start line for top-level reset (triggeringLine was null).");
                    }

                    _instance += 1;
                    //Console.WriteLine($"[OCR DEBUG] Part.Read: Incremented TOP-LEVEL instance counter to {_instance}.");

                    ChildParts.ForEach(child => {
                        if (child.OCR_Part.RecuringPart != null)
                        {
                            //Console.WriteLine($"[OCR DEBUG] Part.Read: Calling Reset() on RECURRING Child/Grandchild Part ID {child.OCR_Part.Id} from TOP-LEVEL Parent Part ID {OCR_Part.Id}");
                            child.Reset();
                        }
                        else
                        {
                            //Console.WriteLine($"[OCR DEBUG] Part.Read: SKIPPING Reset() on NON-RECURRING Child/Grandchild Part ID {child.OCR_Part.Id} from TOP-LEVEL Parent Part ID {OCR_Part.Id}");
                        }
                    });
                }
                // --- End of Reset Logic ---

            }
            catch (Exception e)
            {
                //Console.WriteLine($"[OCR ERROR] Part.Read: Exception in Part ID {OCR_Part.Id}, Effective Instance {currentEffectiveInstance}: {e}");
                // throw;
            }
        }

        // FindStart now returns the triggering InvoiceLine or null
        // Takes the lines relevant to the current instance buffer as input
        private InvoiceLine? FindStart(List<InvoiceLine> linesInvolved)
        {
            try
            {
                if (!OCR_Part.Start.Any()) return new InvoiceLine("", 0); // Implicitly started
                if (WasStarted && OCR_Part.RecuringPart?.IsComposite == true) return null; // Composite only starts once

                var instanceLines = _instanceLinesTxt.ToString();
                if (string.IsNullOrEmpty(instanceLines)) return null;

                foreach (var startCondition in OCR_Part.Start)
                {
                    var textToCheck = (startCondition.RegularExpressions.MultiLine ?? false) == true
                        ? GetLastLine(instanceLines.Trim(), 10)
                        : GetLastLine(instanceLines.Trim(), 1);



                    var options = (startCondition.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                                  | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                    Match match = null;
                    try
                    {
                        match = Regex.Match(textToCheck, startCondition.RegularExpressions.RegEx, options, TimeSpan.FromSeconds(2));
                    }
                    catch (RegexMatchTimeoutException ex)
                    {
                        var snippetLength = Math.Min(textToCheck?.Length ?? 0, 200);
                        var inputSnippet = textToCheck?.Substring(0, snippetLength) ?? "[null]";
                        //Console.WriteLine($"[OCR WARNING] Regex Timeout in FindStart: PartID={startCondition?.PartId}, Pattern='{startCondition?.RegularExpressions?.RegEx}', Input Snippet='{inputSnippet}', Error='{ex.Message}'");
                        continue;
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine($"[OCR ERROR] Regex Error in FindStart: PartID={startCondition?.PartId}, Pattern='{startCondition?.RegularExpressions?.RegEx}': {e}");
                        continue;
                    }

                    if (match?.Success ?? false)
                    {
                        return FindMatch(startCondition.RegularExpressions.RegEx, match, linesInvolved.TakeLast(10).ToList(), startCondition.RegularExpressions.MultiLine ?? false);
                    }
                }
                return null; // No start condition matched
            }
            catch (Exception e)
            {
                //Console.WriteLine($"[OCR ERROR] Part.FindStart: Exception in Part ID {OCR_Part.Id}: {e}");
                return null;
            }
        }


        /// <summary>
        /// Accurately finds the line containing the match (not just the start position).
        /// </summary>
        public InvoiceLine? FindMatchStart(
            Match match,
            List<InvoiceLine> linesInvolved,
            string regexPattern,
            bool checkEntireLine = true
        )
        {
            if (!match.Success)
                return null;

            // First, try to find a line that matches the regex *exactly*
            foreach (var line in linesInvolved.TakeLast(10))
            {
                var lineMatch = Regex.Match(line.Line, regexPattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));
                if (lineMatch.Success)
                {
                    //Console.WriteLine($"[DEBUG] Exact match on Line {line.LineNumber}: {line.Line}");
                    return line;
                }
            }

            // Fallback: Find the line containing the match text (original code's logic)
            foreach (var line in linesInvolved.TakeLast(10))
            {
                if (line.Line.Contains(match.Value.Trim()))
                {
                    //Console.WriteLine($"[DEBUG] Fallback match on Line {line.LineNumber}: {line.Line}");
                    return line;
                }
            }

            // Last resort: Check buffer position (less accurate)
            int matchStartIndex = match.Index;
            int currentLength = 0;
            foreach (var line in linesInvolved)
            {
                var lineWithNewline = line.Line + Environment.NewLine;
                if (matchStartIndex >= currentLength && matchStartIndex < currentLength + lineWithNewline.Length)
                {
                    //Console.WriteLine($"[WARNING] Used buffer position fallback for Line {line.LineNumber}");
                    return line;
                }
                currentLength += lineWithNewline.Length;
            }

            return null;
        }


        /// <summary>
        /// Reconstructs all lines that contributed to a multi-line match.
        /// </summary>
        public List<InvoiceLine>? ReconstructMultiLineMatch(string regularExpressionsRegEx, Match match,
            List<InvoiceLine> linesInvolved,
            InvoiceLine? startLine = null // Optional: pass pre-found start line
        )
        {
            if (!match.Success)
                return null;

            // If startLine isn't provided, find it first
            startLine ??= FindMatchStart(match, linesInvolved, regularExpressionsRegEx);
            if (startLine == null)
                return null;

            var matchLines = match.Value.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            var candidateLines = new List<InvoiceLine>();
            

            // Scan backward to include all relevant lines
            foreach (var line in linesInvolved.OrderByDescending(x => x.LineNumber))
            {
                // Start collecting when we hit the start line
              

              
                    candidateLines.Add(line);
                    // Stop if we've captured enough lines
                    if (candidateLines.Count >= matchLines.Length + 2)
                        break;
                
                if (line.LineNumber == startLine.LineNumber)
                    break;
            }

            // Reorder lines (original order) and validate
            var orderedLines = candidateLines.OrderBy(x => x.LineNumber).ToList();
            var reconstructedText = string.Join(Environment.NewLine, orderedLines.Select(x => x.Line));

            return Regex.IsMatch(reconstructedText, regularExpressionsRegEx)
                ? orderedLines
                : null;
        }

        /// <summary>
        /// Flexible finder: toggle GetWholeMatchOrFirstLine=true to reconstruct all lines.
        /// </summary>
        public InvoiceLine? FindMatch(string regularExpressionsRegEx, Match match,
            List<InvoiceLine> linesInvolved,
            bool GetWholeMatchOrFirstLine = false)
        {
            if (!match.Success)
                return null;

            // Always find the start line first (efficient)
            var startLine = FindMatchStart(match, linesInvolved, regularExpressionsRegEx);
            if (startLine == null)
                return null;

            // Return just the start line if not in multi-line mode
            if (!GetWholeMatchOrFirstLine)
                return startLine;

            // Otherwise, reconstruct full match
            var matchLines = ReconstructMultiLineMatch(regularExpressionsRegEx, match, linesInvolved, startLine);

            UpdateLinesField(matchLines);

            return matchLines.First();
        }

        /// <summary>
        /// Updates _lines field with reconstructed match if valid.
        /// Returns true if update succeeded.
        /// </summary>
        public bool UpdateLinesField(List<InvoiceLine>? newLines)
        {
            if (newLines == null || newLines.Count == 0)
                return false;

            _lines.Clear();
            _lines.AddRange(newLines);
            Console.WriteLine($"[UPDATED] _lines now contains {newLines.Count} matched lines");
            return true;
        }

        private static Match IsMatch(string val, Start z) // Kept static IsMatch for potential reuse?
        {
            try
            {
                var options = (z.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                              | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                return Regex.Match(val, z.RegularExpressions.RegEx, options, TimeSpan.FromSeconds(2));
            }
            catch (Exception e)
            {
                //Console.WriteLine($"[OCR ERROR] IsMatch (Static): Exception for PartID={z?.PartId}, Pattern='{z?.RegularExpressions?.RegEx}': {e}");
                return null;
            }
        }

        private void ResetInternalState()
        {
            _startlines.Clear();
            _endlines.Clear();
            _lines.Clear();
            _instanceLinesTxt.Clear();
            lastLineRead = 0;
            _currentInstanceStartLineNumber = -1;
        }

        public void Reset()
        {
            ResetInternalState();
            _instance = 1;
            _lastProcessedParentInstance = 0;

            //Console.WriteLine($"[OCR DEBUG] Part.Reset: FULL Resetting Part ID {OCR_Part.Id}"); 
            ChildParts.ForEach(child => child.Reset());
        }
        // Method to get the last line from a string
        private string GetLastLine(string input, int noOfLastLines)
        {
            if (string.IsNullOrEmpty(input) || noOfLastLines <= 0)
                return string.Empty;

            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Skip(Math.Max(0, lines.Length - noOfLastLines)));
        }
    }
}