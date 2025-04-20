using System.Text.RegularExpressions;
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Text; // Added for StringBuilder
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for InvoiceLine
using Core.Common.Extensions; // Added for ForEach

namespace WaterNut.DataSpace;

public partial class Part
{
    // Assuming _logger and RegexTimeout exist from another partial part
    // private static readonly ILogger _logger = Log.ForContext<Part>();
    // private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

    public void Read(List<InvoiceLine> newlines, string Section, int? parentInstance = null)
    {
        int? partId = this.OCR_Part?.Id;
        int currentEffectiveInstance = parentInstance ?? _instance;
        int newLineCount = newlines?.Count ?? 0;
        int firstNewLineNum = newlines?.FirstOrDefault()?.LineNumber ?? -1;

        _logger.Debug("Entering Part.Read for PartId: {PartId}, EffectiveInstance: {EffectiveInstance} (Internal: {InternalInstance}, Parent: {ParentInstance}), Section: '{Section}'. Processing {NewLineCount} new lines starting from LineNumber: {FirstNewLineNum}. Current buffer size: {BufferLineCount}. lastLineRead: {LastLineRead}. WasStarted: {WasStarted}",
            partId, currentEffectiveInstance, _instance, parentInstance?.ToString() ?? "N/A", Section, newLineCount, _lines?.Count ?? 0, lastLineRead, WasStarted);

        // Null check inputs
        if (newlines == null)
        {
             _logger.Warning("Part.Read called with null newlines list for PartId: {PartId}. Exiting.", partId);
             return;
        }
         if (this.OCR_Part == null)
         {
              _logger.Error("Part.Read cannot proceed: OCR_Part is null.");
              return;
         }


        try
        {
            // --- HACK: Reset child state ---
            if (parentInstance.HasValue && parentInstance.Value > _lastProcessedParentInstance)
            {
                 _logger.Warning("Child PartId: {PartId} detected new parent instance ({ParentInstance} > {LastProcessedParentInstance}). Resetting child state (HACK).",
                    partId, parentInstance.Value, _lastProcessedParentInstance);
                ResetInternalState(); // Assuming this method exists and handles logging
                _lastProcessedParentInstance = parentInstance.Value;
                 _logger.Debug("Updated _lastProcessedParentInstance to {ParentInstance}", parentInstance.Value);
            }
            // --- End HACK ---


            // --- Line Accumulation ---
             _logger.Verbose("PartId: {PartId} - Accumulating lines...", partId);
            var uniqueNewLines = newlines.Where(nl => nl != null && !_lines.Any(l => l != null && l.LineNumber == nl.LineNumber)).ToList();
             _logger.Verbose("PartId: {PartId} - Added {Count} unique new lines to internal buffer.", partId, uniqueNewLines.Count);
            _lines.AddRange(uniqueNewLines);
            _lines.Sort((a, b) => a.LineNumber.CompareTo(b.LineNumber)); // Ensure sorted

            var linesForThisStep = newlines.Where(x => x != null && lastLineRead < x.LineNumber).ToList();
            if (linesForThisStep.Any())
            {
                int newLastLineRead = linesForThisStep.Last().LineNumber;
                 _logger.Verbose("PartId: {PartId} - Updating lastLineRead from {OldLastLineRead} to {NewLastLineRead}", partId, lastLineRead, newLastLineRead);
                 lastLineRead = newLastLineRead;
            } else {
                 _logger.Verbose("PartId: {PartId} - No new lines found beyond lastLineRead ({LastLineRead}).", partId, lastLineRead);
            }
            // --- End Line Accumulation ---

            // --- Build Instance Text Buffer ---
             _logger.Verbose("PartId: {PartId} - Rebuilding instance text buffer...", partId);
            var linesForInstanceBuffer = _lines.Where(l => l != null && (_currentInstanceStartLineNumber == -1 || l.LineNumber >= _currentInstanceStartLineNumber)).ToList();
            _instanceLinesTxt.Clear();
            linesForInstanceBuffer.ForEach(l => _instanceLinesTxt.AppendLine(l.Line)); // Use ForEach extension
             _logger.Verbose("PartId: {PartId} - Instance text buffer rebuilt. Length: {Length}, Line Count: {LineCount}", partId, _instanceLinesTxt.Length, linesForInstanceBuffer.Count);
            // --- End Instance Text Buffer ---

            // --- Start Detection ---
             _logger.Verbose("PartId: {PartId} - Calling FindStart...", partId);
            var triggeringLine = FindStart(linesForInstanceBuffer); // FindStart handles its own logging
            bool startFound = triggeringLine != null;
            _logger.Information("PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - FindStart result: {StartFound} {TriggerLineInfo}. WasStarted (before): {WasStarted}",
                partId, currentEffectiveInstance, startFound, startFound ? $"(Line: {triggeringLine.LineNumber})" : "", WasStarted);

            bool justStartedThisCall = false;
            // --- End Start Detection ---

            // --- Reset Logic for RECURRING CHILD Start-to-Start ---
            bool isRecurringChildStartToStart = parentInstance != null && startFound && WasStarted &&
                                                (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite);
             _logger.Verbose("PartId: {PartId} - Checking Recurring Child Start-to-Start reset condition: {ConditionResult} (ParentInstance={ParentInstance}, StartFound={StartFound}, WasStarted={WasStarted}, IsRecurringNonComposite={IsRecurringNonComposite})",
                partId, isRecurringChildStartToStart, parentInstance, startFound, WasStarted, (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite));

            if (isRecurringChildStartToStart)
            {
                 _logger.Information("PARTIAL Resetting state for RECURRING CHILD PartId: {PartId} (Internal Instance: {InternalInstance}) within ParentInstance: {ParentInstance}. Found subsequent start on LineNumber: {TriggerLineNumber}.",
                    partId, _instance, parentInstance, triggeringLine?.LineNumber ?? -1);

                // Partial Reset for recurring child finding a new start within the same parent instance
                _startlines.Clear();
                _endlines.Clear();
                _instanceLinesTxt.Clear();
                _currentInstanceStartLineNumber = -1;
                 _logger.Verbose("PartId: {PartId} - Cleared startlines, endlines, instance buffer. Reset _currentInstanceStartLineNumber.", partId);

                if (triggeringLine != null && StartCount > 0)
                {
                    _startlines.Add(triggeringLine);
                    _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                    _instanceLinesTxt.AppendLine(triggeringLine.Line ?? ""); // Re-add the triggering line to buffer safely
                     _logger.Debug("Added triggering LineNumber: {TriggerLineNumber} as first start line for new recurring child instance.", triggeringLine.LineNumber);
                    justStartedThisCall = true;
                }
                else if (StartCount > 0)
                {
                     _logger.Error("Failed to add start line after recurring child reset (triggeringLine was null or StartCount is 0) for PartId: {PartId}.", partId);
                }
            }
            // --- End Recurring Child Reset ---

            // --- Initial Start Logic ---
             _logger.Verbose("PartId: {PartId} - Checking Initial Start condition: StartFound={StartFound}, WasStarted={WasStarted}, JustStartedThisCall={JustStarted}", partId, startFound, WasStarted, justStartedThisCall);
            if (startFound && !WasStarted && !justStartedThisCall)
            {
                if (triggeringLine != null && StartCount > 0)
                {
                    _startlines.Add(triggeringLine);
                    _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                    // Ensure buffer has the line
                    if (_instanceLinesTxt.Length == 0 || !_instanceLinesTxt.ToString().Contains(triggeringLine.Line ?? Guid.NewGuid().ToString())) // Check against non-null line
                    {
                         _logger.Warning("Triggering line text not found in instance buffer after FindStart. Rebuilding buffer with triggering line for PartId: {PartId}, LineNumber: {LineNumber}", partId, triggeringLine.LineNumber);
                         _instanceLinesTxt.Clear();
                         _instanceLinesTxt.AppendLine(triggeringLine.Line ?? ""); // Add safely
                    }

                     _logger.Information("PartId: {PartId} - Detected INITIAL start at LineNumber: {LineNumber} for EffectiveInstance: {EffectiveInstance}", partId, triggeringLine.LineNumber, currentEffectiveInstance);
                    justStartedThisCall = true;
                    if (parentInstance != null)
                    {
                         _logger.Debug("Child PartId: {PartId}: Detected INITIAL start within ParentInstance: {ParentInstance}. Using EffectiveInstance: {EffectiveInstance}.", partId, parentInstance, currentEffectiveInstance);
                    }
                }
                else if (StartCount > 0)
                {
                     _logger.Error("Failed to add initial start line (triggeringLine was null or StartCount is 0) for PartId: {PartId}.", partId);
                }
            }
            // --- End Initial Start Logic ---

            // --- Main Processing Block ---
            bool currentlyStarted = _startlines.Any(); // Re-evaluate after potential start logic
             _logger.Verbose("PartId: {PartId} - Checking Main Processing Block conditions: CurrentlyStarted={CurrentlyStarted}, StartLinesCount={StartCountActual}/{StartCountRequired}, EndLinesCount={EndCountActual}/{EndCountRequired}",
                partId, currentlyStarted, _startlines.Count, StartCount, _endlines.Count, EndCount);

            if (currentlyStarted && _startlines.Count >= StartCount &&
                ((_endlines.Count < EndCount && EndCount > 0) || EndCount == 0))
            {
                 _logger.Debug("PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Conditions met for processing lines and child parts.", partId, currentEffectiveInstance);
                 // Process Child Parts
                 if (ChildParts != null)
                 {
                      _logger.Verbose("PartId: {PartId} - Processing {Count} child parts.", partId, ChildParts.Count);
                      ChildParts.ForEach(x => // Use ForEach extension
                      {
                          if (x != null)
                          {
                               _logger.Debug("PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Calling Read() on Child PartId: {ChildPartId} with {LineCount} lines, passing ParentInstance: {ParentInstance}.",
                                  partId, currentEffectiveInstance, x.OCR_Part?.Id, linesForThisStep.Count, currentEffectiveInstance);
                               x.Read(new List<InvoiceLine>(linesForThisStep), Section, currentEffectiveInstance); // Pass copy? Original passed same list.
                          } else {
                               _logger.Warning("Skipping null ChildPart object during processing for PartId: {PartId}.", partId);
                          }
                      });
                 } else {
                      _logger.Verbose("PartId: {PartId} - No child parts to process.", partId);
                 }


                 // Process Own Lines
                 if (Lines != null)
                 {
                      _logger.Verbose("PartId: {PartId} - Processing {Count} own lines.", partId, Lines.Count);
                      Lines.ForEach(x => // Use ForEach extension
                      {
                          if (x == null || x.OCR_Lines == null) {
                               _logger.Warning("Skipping null Line or Line with null OCR_Lines during processing for PartId: {PartId}.", partId);
                               return; // Continue ForEach
                          }
                          if (!linesForThisStep.Any()) {
                               _logger.Verbose("Skipping LineId: {LineId} processing as linesForThisStep is empty.", x.OCR_Lines.Id);
                               return; // Continue ForEach if no new lines relevant to this step
                          }

                          // Rebuild buffer and text specific to this line's potential multiline read
                          var instanceLines = _lines.Where(l => l != null && l.LineNumber >= _currentInstanceStartLineNumber).ToList();

                          if (x.OCR_Lines.RegularExpressions?.MultiLine == true) // Safe check
                          {
                              var relevantLines = instanceLines.TakeLast(x.OCR_Lines.RegularExpressions.MaxLines ?? 10).ToList(); // Safe check for MaxLines
                              var lineText = relevantLines.Select(z => z.Line).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
                               _logger.Debug("PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Reading multi-line LineId: {LineId} using {RelevantLineCount} lines.",
                                  partId, currentEffectiveInstance, x.OCR_Lines.Id, relevantLines.Count);
                               // Line.Read handles its own logging
                               x.Read(lineText, relevantLines.FirstOrDefault()?.LineNumber ?? linesForThisStep.First().LineNumber, Section, currentEffectiveInstance);
                          }
                          else // Single line read
                          {
                               _logger.Debug("PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Reading single-line LineId: {LineId} using last line of step ({LastLineNum}).",
                                  partId, currentEffectiveInstance, x.OCR_Lines.Id, linesForThisStep.Last().LineNumber);
                               // Line.Read handles its own logging
                               x.Read(linesForThisStep.Last().Line, linesForThisStep.Last().LineNumber, Section, currentEffectiveInstance);
                          }
                      });
                 } else {
                      _logger.Verbose("PartId: {PartId} - No own lines to process.", partId);
                 }

            } else {
                 _logger.Debug("PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Conditions NOT met for processing lines/children (CurrentlyStarted={CurrentlyStarted}, StartCount={StartCountActual}/{StartCountRequired}, EndCount={EndCountActual}/{EndCountRequired})",
                    partId, currentEffectiveInstance, currentlyStarted, _startlines.Count, StartCount, _endlines.Count, EndCount);
            }
            // --- End Main Processing Block ---

            // --- End Line Detection ---
            // Re-evaluate currentlyStarted as state might have changed if start was found this call
            currentlyStarted = _startlines.Any();
             _logger.Verbose("PartId: {PartId} - Checking End condition: CurrentlyStarted={CurrentlyStarted}, JustStartedThisCall={JustStarted}, StartLinesCount={StartCountActual}/{StartCountRequired}, EndLinesCount={EndCountActual}/{EndCountRequired}",
                partId, currentlyStarted, justStartedThisCall, _startlines.Count, StartCount, _endlines.Count, EndCount);

            // Check end condition only if started, not just started this call, start conditions met, and end conditions not yet met (and required)
            if (currentlyStarted && !justStartedThisCall && _startlines.Count >= StartCount &&
                _endlines.Count < EndCount && EndCount > 0)
            {
                 _logger.Debug("PartId: {PartId} - Checking end conditions against instance buffer.", partId);
                 // Check if any end condition regex matches the current instance buffer
                 bool endFound = false;
                 if (this.OCR_Part.End != null) // Safe check
                 {
                     foreach (var endCondition in this.OCR_Part.End.Where(ec => ec?.RegularExpressions != null)) // Safe iteration
                     {
                          string endPattern = endCondition.RegularExpressions.RegEx;
                          int endConditionId = endCondition.Id;
                          if (string.IsNullOrEmpty(endPattern)) {
                               _logger.Warning("Skipping end condition check for EndConditionId: {EndConditionId} due to null/empty pattern.", endConditionId);
                               continue;
                          }
                          var endOptions = (endCondition.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline) |
                                           RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
                           _logger.Verbose("PartId: {PartId} - Checking EndConditionId: {EndConditionId}, Pattern: '{Pattern}', Options: {Options}", partId, endConditionId, endPattern, endOptions);
                          try
                          {
                              if (Regex.IsMatch(_instanceLinesTxt.ToString(), endPattern, endOptions, RegexTimeout)) // Use IsMatch for efficiency
                              {
                                   _logger.Information("PartId: {PartId} - End condition MET by EndConditionId: {EndConditionId}", partId, endConditionId);
                                   endFound = true;
                                   break; // Found an end condition
                              } else {
                                   _logger.Verbose("PartId: {PartId} - EndConditionId: {EndConditionId} did not match.", partId, endConditionId);
                              }
                          }
                          catch (RegexMatchTimeoutException) { _logger.Warning("Regex timeout checking end condition EndConditionId: {EndConditionId}", endConditionId); }
                          catch (ArgumentException argEx) { _logger.Error(argEx, "Invalid regex pattern for end condition EndConditionId: {EndConditionId}", endConditionId); }
                          catch (Exception endEx) { _logger.Error(endEx, "Error checking end condition EndConditionId: {EndConditionId}", endConditionId); }
                     }
                 } else {
                      _logger.Warning("PartId: {PartId} - OCR_Part.End collection is null. Cannot check end conditions.", partId);
                 }


                 if (endFound)
                 {
                     // Find the last line within the current instance buffer to mark as the end line
                     var instanceLines = _lines.Where(l => l != null && l.LineNumber >= _currentInstanceStartLineNumber).ToList();
                     if (instanceLines.Any())
                     {
                         var endLine = instanceLines.Last();
                         _endlines.Add(endLine);
                          _logger.Information("PartId: {PartId} - Added LineNumber: {LineNumber} as end line for EffectiveInstance: {EffectiveInstance}.", partId, endLine.LineNumber, currentEffectiveInstance);
                     } else {
                          _logger.Warning("PartId: {PartId} - End condition met, but could not determine last line of instance buffer to add to _endlines.", partId);
                     }
                 }
            }
            // --- End End Line Detection ---

            // --- Reset Logic for TOP-LEVEL Start-to-Start ---
            // Re-evaluate currentlyStarted as state might have changed
            currentlyStarted = _startlines.Any();
            bool isTopLevelRecurringStartToStart = parentInstance == null && startFound && currentlyStarted &&
                                                   !justStartedThisCall && (OCR_Part.RecuringPart != null &&
                                                                            !OCR_Part.RecuringPart.IsComposite);
             _logger.Verbose("PartId: {PartId} - Checking Top-Level Start-to-Start reset condition: {ConditionResult} (ParentInstance={ParentInstance}, StartFound={StartFound}, CurrentlyStarted={CurrentlyStarted}, JustStartedThisCall={JustStarted}, IsRecurringNonComposite={IsRecurringNonComposite})",
                partId, isTopLevelRecurringStartToStart, parentInstance, startFound, currentlyStarted, justStartedThisCall, (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite));

            if (isTopLevelRecurringStartToStart)
            {
                 _logger.Information("FULL Resetting state for TOP-LEVEL recurring PartId: {PartId} (Instance {CompletedInstance} completed, {NextInstance} starting). Triggered by LineNumber: {TriggerLineNumber}.",
                    partId, _instance, _instance + 1, triggeringLine?.LineNumber ?? -1);

                ResetInternalState(); // Full reset (assuming this handles logging)

                if (triggeringLine != null && StartCount > 0)
                {
                    _startlines.Add(triggeringLine);
                    _currentInstanceStartLineNumber = triggeringLine.LineNumber;
                    _instanceLinesTxt.AppendLine(triggeringLine.Line ?? ""); // Re-add triggering line to buffer safely
                     _logger.Debug("Added triggering LineNumber: {TriggerLineNumber} as first start line for new top-level instance.", triggeringLine.LineNumber);
                }
                else if (StartCount > 0)
                {
                     _logger.Warning("Could not determine triggering start line for top-level reset (triggeringLine was null) for PartId: {PartId}.", partId);
                }

                _instance += 1; // Increment instance counter *after* reset
                 _logger.Debug("Incremented TOP-LEVEL instance counter to {InstanceCount}.", _instance);

                 // Reset recurring children
                 if (ChildParts != null)
                 {
                     ChildParts.ForEach(child => // Use ForEach extension
                     {
                         if (child?.OCR_Part?.RecuringPart != null) // Safe check
                         {
                              _logger.Debug("Calling Reset() on RECURRING Child PartId: {ChildPartId} from TOP-LEVEL Parent PartId: {ParentPartId}", child.OCR_Part.Id, partId);
                              child.Reset(); // Assuming Reset handles logging
                         }
                         else
                         {
                              _logger.Debug("SKIPPING Reset() on NON-RECURRING or null Child PartId: {ChildPartId} from TOP-LEVEL Parent PartId: {ParentPartId}", child?.OCR_Part?.Id, partId);
                         }
                     });
                 }
            }
            // --- End of Reset Logic ---
        }
        catch (Exception e)
        {
             _logger.Error(e, "Error during Part.Read for PartId: {PartId}, EffectiveInstance: {EffectiveInstance}", partId, currentEffectiveInstance);
             // throw; // Original code commented out throw, maintaining that behavior
        }
         _logger.Debug("Exiting Part.Read for PartId: {PartId}, EffectiveInstance: {EffectiveInstance}", partId, currentEffectiveInstance);
    }

     // Assuming ResetInternalState, FindStart, FormatValues, SaveLineValues exist in other partial class parts
     // private void ResetInternalState() { ... }
     // private InvoiceLine FindStart(List<InvoiceLine> linesInvolved) { ... }
     // private void FormatValues(int instance, MatchCollection matches, Dictionary<(Fields Fields, int Instance), string> values) { ... }
     // private void SaveLineValues(int lineNumber, string section, int instance, Dictionary<(Fields Fields, int Instance), string> values) { ... }
}