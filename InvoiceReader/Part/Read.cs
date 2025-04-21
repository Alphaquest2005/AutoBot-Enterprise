using System.Text.RegularExpressions;
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Text; // Added for StringBuilder
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for InvoiceLine
using Core.Common.Extensions;
using MoreLinq; // Added for ForEach

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        // Assuming _logger and RegexTimeout exist from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Part>();
        // private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        public void Read(List<InvoiceLine> newlines, string Section, int? parentInstance = null)
        {
            string methodName = nameof(Read);
            int? partId = this.OCR_Part?.Id;
            int currentEffectiveInstance =
                parentInstance ?? _instance; // Use parent's instance if provided, otherwise internal
            int newLineCount = newlines?.Count ?? 0;
            int firstNewLineNum = newlines?.FirstOrDefault()?.LineNumber ?? -1;

            _logger.Verbose(
                "Entering {MethodName} for PartId: {PartId}, EffectiveInstance: {EffectiveInstance} (Internal: {InternalInstance}, Parent: {ParentInstance}), Section: '{Section}'. Processing {NewLineCount} new lines starting from LineNumber: {FirstNewLineNum}. Current buffer size: {BufferLineCount}. lastLineRead: {LastLineRead}. WasStarted: {WasStarted}",
                methodName, partId, currentEffectiveInstance, _instance, parentInstance?.ToString() ?? "N/A", Section,
                newLineCount, _lines?.Count ?? 0, lastLineRead, WasStarted);

            // --- Input Validation ---
            if (newlines == null)
            {
                _logger.Warning("{MethodName}: Called with null newlines list for PartId: {PartId}. Exiting.",
                    methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} due to null newlines.", methodName, partId);
                return;
            }

            if (this.OCR_Part == null)
            {
                _logger.Error("{MethodName}: Cannot proceed for PartId: {PartId}: OCR_Part is null. Exiting.",
                    methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} due to null OCR_Part.", methodName, partId);
                return;
            }

            _logger.Verbose("{MethodName}: Input validation passed for PartId: {PartId}.", methodName, partId);


            try
            {
                // --- HACK: Reset child state if new parent instance detected ---
                if (parentInstance.HasValue && parentInstance.Value > _lastProcessedParentInstance)
                {
                    _logger.Warning(
                        "{MethodName}: Child PartId: {PartId} detected new parent instance ({ParentInstance} > {LastProcessedParentInstance}). Resetting child state (HACK).",
                        methodName, partId, parentInstance.Value, _lastProcessedParentInstance);
                    ResetInternalState(); // Assuming this method exists and handles logging
                    _lastProcessedParentInstance = parentInstance.Value;
                    _logger.Debug(
                        "{MethodName}: Updated _lastProcessedParentInstance to {ParentInstance} for PartId: {PartId}",
                        methodName, parentInstance.Value, partId);
                }
                // --- End HACK ---


                // --- Line Accumulation ---
                _logger.Verbose("{MethodName}: PartId: {PartId} - Accumulating lines...", methodName, partId);
                var uniqueNewLines = newlines
                    .Where(nl => nl != null && !_lines.Any(l => l != null && l.LineNumber == nl.LineNumber)).ToList();
                _logger.Verbose("{MethodName}: PartId: {PartId} - Added {Count} unique new lines to internal buffer.",
                    methodName, partId, uniqueNewLines.Count);
                _lines.AddRange(uniqueNewLines);
                _lines.Sort((a, b) => a.LineNumber.CompareTo(b.LineNumber)); // Ensure sorted
                _logger.Verbose("{MethodName}: PartId: {PartId} - Internal buffer now contains {TotalCount} lines.",
                    methodName, partId, _lines.Count);


                // --- Update lastLineRead ---
                var linesForThisStep = newlines.Where(x => x != null && lastLineRead < x.LineNumber).ToList();
                if (linesForThisStep.Any())
                {
                    int newLastLineRead = linesForThisStep.Last().LineNumber;
                    _logger.Verbose(
                        "{MethodName}: PartId: {PartId} - Updating lastLineRead from {OldLastLineRead} to {NewLastLineRead}",
                        methodName, partId, lastLineRead, newLastLineRead);
                    lastLineRead = newLastLineRead;
                }
                else
                {
                    _logger.Verbose(
                        "{MethodName}: PartId: {PartId} - No new lines found beyond lastLineRead ({LastLineRead}). linesForThisStep is empty.",
                        methodName, partId, lastLineRead);
                }
                // --- End Line Accumulation ---


                // --- Build Instance Text Buffer ---
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Rebuilding instance text buffer (lines >= {StartLineNum})...",
                    methodName, partId, _currentInstanceStartLineNumber);
                var linesForInstanceBuffer = _lines.Where(l =>
                    l != null && (_currentInstanceStartLineNumber == -1 ||
                                  l.LineNumber >= _currentInstanceStartLineNumber)).ToList();
                _instanceLinesTxt.Clear();
                linesForInstanceBuffer.ForEach(l => _instanceLinesTxt.AppendLine(l.Line)); // Use ForEach extension
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Instance text buffer rebuilt. Length: {Length}, Line Count: {LineCount}",
                    methodName, partId, _instanceLinesTxt.Length, linesForInstanceBuffer.Count);
                // --- End Instance Text Buffer ---


                // --- Start Detection ---
                _logger.Verbose("{MethodName}: PartId: {PartId} - Calling FindStart using instance buffer...",
                    methodName, partId);
                var triggeringLine = FindStart(linesForInstanceBuffer); // FindStart handles its own logging
                bool startFound = triggeringLine != null;
                _logger.Information(
                    "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - FindStart result: {StartFound} {TriggerLineInfo}. WasStarted (before this call): {WasStarted}",
                    methodName, partId, currentEffectiveInstance, startFound,
                    startFound ? $"(Line: {triggeringLine.LineNumber})" : "", WasStarted);

                bool justStartedThisCall = false; // Track if start was detected *in this specific call*
                // --- End Start Detection ---


                // --- Reset Logic for RECURRING CHILD Start-to-Start ---
                bool isRecurringChildStartToStart = parentInstance != null && startFound && WasStarted &&
                                                    (OCR_Part.RecuringPart != null &&
                                                     !OCR_Part.RecuringPart.IsComposite);
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Checking Recurring Child Start-to-Start reset condition: {ConditionResult} (ParentInstance={ParentInstance}, StartFound={StartFound}, WasStarted={WasStarted}, IsRecurringNonComposite={IsRecurringNonComposite})",
                    methodName, partId, isRecurringChildStartToStart, parentInstance, startFound, WasStarted,
                    (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite));

                if (isRecurringChildStartToStart)
                {
                    _logger.Information(
                        "{MethodName}: PARTIAL Resetting state for RECURRING CHILD PartId: {PartId} (Internal Instance: {InternalInstance}) within ParentInstance: {ParentInstance}. Found subsequent start on LineNumber: {TriggerLineNumber}.",
                        methodName, partId, _instance, parentInstance, triggeringLine?.LineNumber ?? -1);

                    // Partial Reset for recurring child finding a new start within the same parent instance
                    _startlines.Clear();
                    _endlines.Clear();
                    _instanceLinesTxt.Clear();
                    _currentInstanceStartLineNumber = -1;
                    _logger.Verbose(
                        "{MethodName}: PartId: {PartId} - Cleared startlines, endlines, instance buffer. Reset _currentInstanceStartLineNumber.",
                        methodName, partId);

                    if (triggeringLine != null && StartCount > 0)
                    {
                        _startlines.Add(triggeringLine);
                        _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                        _instanceLinesTxt.AppendLine(triggeringLine.Line ??
                                                     ""); // Re-add the triggering line to buffer safely
                        _logger.Debug(
                            "{MethodName}: Added triggering LineNumber: {TriggerLineNumber} as first start line for new recurring child instance.",
                            methodName, triggeringLine.LineNumber);
                        justStartedThisCall = true; // Mark that we started a new instance *now*
                    }
                    else if (StartCount > 0)
                    {
                        _logger.Error(
                            "{MethodName}: Failed to add start line after recurring child reset (triggeringLine was null or StartCount is 0) for PartId: {PartId}.",
                            methodName, partId);
                    }
                }
                // --- End Recurring Child Reset ---


                // --- Initial Start Logic ---
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Checking Initial Start condition: StartFound={StartFound}, WasStarted={WasStarted}, JustStartedThisCall={JustStarted}",
                    methodName, partId, startFound, WasStarted, justStartedThisCall);
                if (startFound && !WasStarted &&
                    !justStartedThisCall) // Only trigger if not already started and didn't just start via recurring child reset
                {
                    if (triggeringLine != null && StartCount > 0)
                    {
                        _startlines.Add(triggeringLine);
                        _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                        // Ensure buffer has the line
                        if (_instanceLinesTxt.Length == 0 || !_instanceLinesTxt.ToString()
                                .Contains(triggeringLine.Line ?? Guid.NewGuid().ToString()))
                        {
                            _logger.Warning(
                                "{MethodName}: Triggering line text not found in instance buffer after FindStart. Rebuilding buffer with triggering line for PartId: {PartId}, LineNumber: {LineNumber}",
                                methodName, partId, triggeringLine.LineNumber);
                            _instanceLinesTxt.Clear();
                            _instanceLinesTxt.AppendLine(triggeringLine.Line ?? ""); // Add safely
                        }

                        _logger.Information(
                            "{MethodName}: PartId: {PartId} - Detected INITIAL start at LineNumber: {LineNumber} for EffectiveInstance: {EffectiveInstance}",
                            methodName, partId, triggeringLine.LineNumber, currentEffectiveInstance);
                        justStartedThisCall = true; // Mark that we started a new instance *now*
                        if (parentInstance != null)
                        {
                            _logger.Debug(
                                "{MethodName}: Child PartId: {PartId}: Detected INITIAL start within ParentInstance: {ParentInstance}. Using EffectiveInstance: {EffectiveInstance}.",
                                methodName, partId, parentInstance, currentEffectiveInstance);
                        }
                    }
                    else if (StartCount > 0)
                    {
                        _logger.Error(
                            "{MethodName}: Failed to add initial start line (triggeringLine was null or StartCount is 0) for PartId: {PartId}.",
                            methodName, partId);
                    }
                }
                // --- End Initial Start Logic ---


                // --- Main Processing Block ---
                bool currentlyStarted = _startlines.Any(); // Re-evaluate after potential start logic
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Checking Main Processing Block conditions: CurrentlyStarted={CurrentlyStarted}, StartLinesCount={StartCountActual}/{StartCountRequired}, EndLinesCount={EndCountActual}/{EndCountRequired}",
                    methodName, partId, currentlyStarted, _startlines.Count, StartCount, _endlines.Count, EndCount);

                // Conditions: Must be started, have enough start lines, and either not need end lines or not have enough end lines yet.
                if (currentlyStarted && _startlines.Count >= StartCount &&
                    ((_endlines.Count < EndCount && EndCount > 0) || EndCount == 0))
                {
                    _logger.Debug(
                        "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Conditions met. Entering main processing block.",
                        methodName, partId, currentEffectiveInstance);

                    // --- Process Child Parts ---
                    if (ChildParts != null)
                    {
                        _logger.Verbose("{MethodName}: PartId: {PartId} - Processing {Count} child parts...",
                            methodName, partId, ChildParts.Count);
                        int childIndex = 0;
                        ChildParts.ForEach(childPart => // Use ForEach extension
                        {
                            childIndex++;
                            if (childPart != null)
                            {
                                int? childPartId = childPart.OCR_Part?.Id;
                                _logger.Verbose(
                                    "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Calling Read() on Child Part {Index}/{Total} (Id: {ChildPartId}) with {LineCount} lines, passing ParentInstance: {ParentInstance}.",
                                    methodName, partId, currentEffectiveInstance, childIndex, ChildParts.Count,
                                    childPartId, linesForThisStep.Count, currentEffectiveInstance);
                                // Child Read handles its own logging
                                childPart.Read(new List<InvoiceLine>(linesForThisStep), Section,
                                    currentEffectiveInstance); // Pass copy? Original passed same list.
                                _logger.Verbose(
                                    "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Finished Read() on Child PartId: {ChildPartId}.",
                                    methodName, partId, currentEffectiveInstance, childPartId);
                            }
                            else
                            {
                                _logger.Warning(
                                    "{MethodName}: Skipping null ChildPart object at index {Index} during processing for PartId: {PartId}.",
                                    methodName, childIndex - 1, partId);
                            }
                        });
                        _logger.Verbose("{MethodName}: PartId: {PartId} - Finished processing child parts.", methodName,
                            partId);
                    }
                    else
                    {
                        _logger.Verbose("{MethodName}: PartId: {PartId} - No child parts to process.", methodName,
                            partId);
                    }


                    // --- Process Own Lines ---
                    if (Lines != null)
                    {
                        _logger.Verbose("{MethodName}: PartId: {PartId} - Processing {Count} own lines...", methodName,
                            partId, Lines.Count);
                        int lineIndex = 0;
                        Lines.ForEach(line => // Use ForEach extension
                        {
                            lineIndex++;
                            if (line == null || line.OCR_Lines == null)
                            {
                                _logger.Warning(
                                    "{MethodName}: Skipping null Line or Line with null OCR_Lines at index {Index} during processing for PartId: {PartId}.",
                                    methodName, lineIndex - 1, partId);
                                return; // Continue ForEach
                            }

                            int lineId = line.OCR_Lines.Id;

                            if (!linesForThisStep.Any())
                            {
                                _logger.Verbose(
                                    "{MethodName}: Skipping LineId: {LineId} processing as linesForThisStep is empty.",
                                    methodName, lineId);
                                return; // Continue ForEach if no new lines relevant to this step
                            }

                            // Rebuild buffer and text specific to this line's potential multiline read
                            var instanceLines = _lines
                                .Where(l => l != null && l.LineNumber >= _currentInstanceStartLineNumber).ToList();

                            if (line.OCR_Lines.RegularExpressions?.MultiLine == true) // Safe check
                            {
                                var relevantLines = instanceLines
                                    .TakeLast(line.OCR_Lines.RegularExpressions.MaxLines ?? 10)
                                    .ToList(); // Safe check for MaxLines
                                var lineText = relevantLines.Select(z => z.Line).DefaultIfEmpty("")
                                    .Aggregate((o, n) => $"{o}\r\n{n}");
                                _logger.Verbose(
                                    "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Reading multi-line Line {Index}/{Total} (Id: {LineId}) using {RelevantLineCount} lines.",
                                    methodName, partId, currentEffectiveInstance, lineIndex, Lines.Count, lineId,
                                    relevantLines.Count);
                                // Line.Read handles its own logging
                                line.Read(lineText,
                                    relevantLines.FirstOrDefault()?.LineNumber ?? linesForThisStep.First().LineNumber,
                                    Section, currentEffectiveInstance);
                            }
                            else // Single line read
                            {
                                var lastLineOfStep = linesForThisStep.Last();
                                _logger.Verbose(
                                    "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Reading single-line Line {Index}/{Total} (Id: {LineId}) using last line of step ({LastLineNum}).",
                                    methodName, partId, currentEffectiveInstance, lineIndex, Lines.Count, lineId,
                                    lastLineOfStep.LineNumber);
                                // Line.Read handles its own logging
                                line.Read(lastLineOfStep.Line, lastLineOfStep.LineNumber, Section,
                                    currentEffectiveInstance);
                            }
                        });
                        _logger.Verbose("{MethodName}: PartId: {PartId} - Finished processing own lines.", methodName,
                            partId);
                    }
                    else
                    {
                        _logger.Verbose("{MethodName}: PartId: {PartId} - No own lines to process.", methodName,
                            partId);
                    }

                    _logger.Debug(
                        "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Finished main processing block.",
                        methodName, partId, currentEffectiveInstance);

                }
                else
                {
                    _logger.Debug(
                        "{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Conditions NOT met for main processing block (CurrentlyStarted={CurrentlyStarted}, StartCount={StartCountActual}/{StartCountRequired}, EndCount={EndCountActual}/{EndCountRequired})",
                        methodName, partId, currentEffectiveInstance, currentlyStarted, _startlines.Count, StartCount,
                        _endlines.Count, EndCount);
                }
                // --- End Main Processing Block ---


                // --- End Line Detection ---
                // Re-evaluate currentlyStarted as state might have changed if start was found this call
                currentlyStarted = _startlines.Any();
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Checking End condition: CurrentlyStarted={CurrentlyStarted}, JustStartedThisCall={JustStarted}, StartLinesCount={StartCountActual}/{StartCountRequired}, EndLinesCount={EndCountActual}/{EndCountRequired}",
                    methodName, partId, currentlyStarted, justStartedThisCall, _startlines.Count, StartCount,
                    _endlines.Count, EndCount);

                // Check end condition only if started, not just started this call, start conditions met, and end conditions not yet met (and required)
                if (currentlyStarted && !justStartedThisCall && _startlines.Count >= StartCount &&
                    _endlines.Count < EndCount && EndCount > 0)
                {
                    _logger.Debug(
                        "{MethodName}: PartId: {PartId} - Checking end conditions against instance buffer (Length: {BufferLength})...",
                        methodName, partId, _instanceLinesTxt.Length);
                    bool endFound = false;
                    if (this.OCR_Part.End != null) // Safe check
                    {
                        int endConditionIndex = 0;
                        foreach (var endCondition in
                                 this.OCR_Part.End.Where(ec => ec?.RegularExpressions != null)) // Safe iteration
                        {
                            endConditionIndex++;
                            string endPattern = endCondition.RegularExpressions.RegEx;
                            int endConditionId = endCondition.Id;
                            if (string.IsNullOrEmpty(endPattern))
                            {
                                _logger.Warning(
                                    "{MethodName}: Skipping end condition check {Index}/{Total} (Id: {EndConditionId}) due to null/empty pattern.",
                                    methodName, endConditionIndex, this.OCR_Part.End.Count(), endConditionId);
                                continue;
                            }

                            var endOptions = (endCondition.RegularExpressions.MultiLine == true
                                                 ? RegexOptions.Multiline
                                                 : RegexOptions.Singleline) |
                                             RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                            _logger.Verbose(
                                "{MethodName}: PartId: {PartId} - Checking EndCondition {Index}/{Total} (Id: {EndConditionId}), Pattern: '{Pattern}', Options: {Options}",
                                methodName, partId, endConditionIndex, this.OCR_Part.End.Count(), endConditionId,
                                endPattern, endOptions);
                            try
                            {
                                if (Regex.IsMatch(_instanceLinesTxt.ToString(), endPattern, endOptions,
                                        RegexTimeout)) // Use IsMatch for efficiency
                                {
                                    _logger.Information(
                                        "{MethodName}: PartId: {PartId} - End condition MET by EndConditionId: {EndConditionId}",
                                        methodName, partId, endConditionId);
                                    endFound = true;
                                    break; // Found an end condition
                                }
                                else
                                {
                                    _logger.Verbose(
                                        "{MethodName}: PartId: {PartId} - EndConditionId: {EndConditionId} did not match.",
                                        methodName, partId, endConditionId);
                                }
                            }
                            catch (RegexMatchTimeoutException)
                            {
                                _logger.Warning(
                                    "{MethodName}: Regex timeout checking end condition EndConditionId: {EndConditionId}",
                                    methodName, endConditionId);
                            }
                            catch (ArgumentException argEx)
                            {
                                _logger.Error(argEx,
                                    "{MethodName}: Invalid regex pattern for end condition EndConditionId: {EndConditionId}",
                                    methodName, endConditionId);
                            }
                            catch (Exception endEx)
                            {
                                _logger.Error(endEx,
                                    "{MethodName}: Error checking end condition EndConditionId: {EndConditionId}",
                                    methodName, endConditionId);
                            }
                        }
                    }
                    else
                    {
                        _logger.Warning(
                            "{MethodName}: PartId: {PartId} - OCR_Part.End collection is null. Cannot check end conditions.",
                            methodName, partId);
                    }


                    if (endFound)
                    {
                        // Find the last line within the current instance buffer to mark as the end line
                        var instanceLines = _lines
                            .Where(l => l != null && l.LineNumber >= _currentInstanceStartLineNumber).ToList();
                        if (instanceLines.Any())
                        {
                            var endLine = instanceLines.Last();
                            _endlines.Add(endLine);
                            _logger.Information(
                                "{MethodName}: PartId: {PartId} - Added LineNumber: {LineNumber} as end line for EffectiveInstance: {EffectiveInstance}.",
                                methodName, partId, endLine.LineNumber, currentEffectiveInstance);
                        }
                        else
                        {
                            _logger.Warning(
                                "{MethodName}: PartId: {PartId} - End condition met, but could not determine last line of instance buffer to add to _endlines.",
                                methodName, partId);
                        }
                    }
                    else
                    {
                        _logger.Debug("{MethodName}: PartId: {PartId} - No end condition met.", methodName, partId);
                    }
                }
                else
                {
                    _logger.Verbose(
                        "{MethodName}: PartId: {PartId} - Skipping end condition check (Conditions not met).",
                        methodName, partId);
                }
                // --- End End Line Detection ---


                // --- Reset Logic for TOP-LEVEL Start-to-Start ---
                // Re-evaluate currentlyStarted as state might have changed
                currentlyStarted = _startlines.Any();
                bool isTopLevelRecurringStartToStart = parentInstance == null && startFound && currentlyStarted &&
                                                       !justStartedThisCall && (OCR_Part.RecuringPart != null &&
                                                           !OCR_Part.RecuringPart.IsComposite);
                _logger.Verbose(
                    "{MethodName}: PartId: {PartId} - Checking Top-Level Start-to-Start reset condition: {ConditionResult} (ParentInstance={ParentInstance}, StartFound={StartFound}, CurrentlyStarted={CurrentlyStarted}, JustStartedThisCall={JustStarted}, IsRecurringNonComposite={IsRecurringNonComposite})",
                    methodName, partId, isTopLevelRecurringStartToStart, parentInstance, startFound, currentlyStarted,
                    justStartedThisCall, (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite));

                if (isTopLevelRecurringStartToStart)
                {
                    _logger.Information(
                        "{MethodName}: FULL Resetting state for TOP-LEVEL recurring PartId: {PartId} (Instance {CompletedInstance} completed, {NextInstance} starting). Triggered by LineNumber: {TriggerLineNumber}.",
                        methodName, partId, _instance, _instance + 1, triggeringLine?.LineNumber ?? -1);

                    ResetInternalState(); // Full reset (assuming this handles logging)

                    if (triggeringLine != null && StartCount > 0)
                    {
                        _startlines.Add(triggeringLine);
                        _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                        _instanceLinesTxt.AppendLine(triggeringLine.Line ??
                                                     ""); // Re-add triggering line to buffer safely
                        _logger.Debug(
                            "{MethodName}: Added triggering LineNumber: {TriggerLineNumber} as first start line for new top-level instance.",
                            methodName, triggeringLine.LineNumber);
                    }
                    else if (StartCount > 0)
                    {
                        _logger.Warning(
                            "{MethodName}: Could not determine triggering start line for top-level reset (triggeringLine was null) for PartId: {PartId}.",
                            methodName, partId);
                    }

                    _instance += 1; // Increment instance counter *after* reset
                    _logger.Debug(
                        "{MethodName}: Incremented TOP-LEVEL instance counter to {InstanceCount} for PartId: {PartId}.",
                        methodName, _instance, partId);

                    // Reset recurring children
                    if (ChildParts != null)
                    {
                        _logger.Verbose("{MethodName}: Resetting recurring children for PartId: {PartId}...",
                            methodName, partId);
                        ChildParts.ForEach(child => // Use ForEach extension
                        {
                            if (child?.OCR_Part?.RecuringPart != null) // Safe check
                            {
                                _logger.Debug(
                                    "{MethodName}: Calling Reset() on RECURRING Child PartId: {ChildPartId} from TOP-LEVEL Parent PartId: {ParentPartId}",
                                    methodName, child.OCR_Part.Id, partId);
                                child.Reset(); // Assuming Reset handles logging
                            }
                            else
                            {
                                _logger.Verbose(
                                    "{MethodName}: SKIPPING Reset() on NON-RECURRING or null Child PartId: {ChildPartId} from TOP-LEVEL Parent PartId: {ParentPartId}",
                                    methodName, child?.OCR_Part?.Id, partId);
                            }
                        });
                        _logger.Verbose("{MethodName}: Finished resetting recurring children for PartId: {PartId}.",
                            methodName, partId);
                    }
                }
                // --- End of Reset Logic ---
            }
            catch (Exception e)
            {
                _logger.Error(e,
                    "{MethodName}: Unhandled exception during processing for PartId: {PartId}, EffectiveInstance: {EffectiveInstance}",
                    methodName, partId, currentEffectiveInstance);
                // throw; // Original code commented out throw, maintaining that behavior
            }

            _logger.Verbose("Exiting {MethodName} for PartId: {PartId}, EffectiveInstance: {EffectiveInstance}",
                methodName, partId, currentEffectiveInstance);
        }

        // Assuming ResetInternalState, FindStart, FormatValues, SaveLineValues exist in other partial class parts
        // private void ResetInternalState() { ... }
        // private InvoiceLine FindStart(List<InvoiceLine> linesInvolved) { ... }
        // private void FormatValues(int instance, MatchCollection matches, Dictionary<(Fields Fields, int Instance), string> values) { ... }
        // private void SaveLineValues(int lineNumber, string section, int instance, Dictionary<(Fields Fields, int Instance), string> values) { ... }
    }
}