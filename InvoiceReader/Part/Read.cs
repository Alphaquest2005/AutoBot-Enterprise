﻿using System.Text.RegularExpressions;
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
        private List<InvoiceLine> _linesForInstanceBuffer;

        public void Read(List<InvoiceLine> newlines, string Section, int? parentInstance)
        {
            string methodName = nameof(Read);
            int? partId = this.OCR_Part?.Id;
            string currentEffectiveInstance = $"{parentInstance}-{(ParentPart == null && parentInstance == _instance ? 1 :_instance )}";//
            int newLineCount = newlines?.Count ?? 0;
            int firstNewLineNum = newlines?.FirstOrDefault()?.LineNumber ?? -1;

            LogEnteringMethod(methodName, partId, currentEffectiveInstance, parentInstance, Section, newLineCount, firstNewLineNum);

            if (!CanRead(newlines, methodName, partId)) return;

            try
            {
                //SetLastProcessedParentInstance(parentInstance, methodName, partId);

                var linesForThisStep = LinesForThisStep(newlines, methodName, partId);

                var triggerline = ProcessStart(parentInstance, methodName, partId, currentEffectiveInstance);

                var importLineSuccess = ProcessLine(Section, methodName, partId, currentEffectiveInstance, linesForThisStep, parentInstance, triggerline);

                PostProcessLine(parentInstance, methodName, partId,triggerline, importLineSuccess) ;

                

                LogExitingMethod(methodName, partId);
            }
            catch (Exception e)
            {
                LogUnhandledException(methodName, partId, Section, e);
                throw;
            }
        }

        private void PostProcessLine(int? parentInstance, string methodName, int? partId, List<InvoiceLine> triggerline,
            bool importLineSuccess)
        {

            if (ParentPart != null && WasStarted && importLineSuccess &&
                (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite))
            {
                LogRecurringChildReset(methodName, partId, parentInstance, triggerline?.FirstOrDefault());
                ResetInternalState();
            }
        }

        private List<InvoiceLine> LinesForThisStep(List<InvoiceLine> newlines, string methodName, int? partId)
        {
            SetLines(newlines, methodName, partId);

            var linesForThisStep = GetLastLinesRead(newlines, methodName, partId);
            return linesForThisStep;
        }

        private List<InvoiceLine> GetLastLinesRead(List<InvoiceLine> newlines, string methodName, int? partId)
        {
            var linesForThisStep = GetLinesForThisStep(newlines, methodName, partId);

            var lastLinesRead = GetlastLineRead(linesForThisStep, methodName, partId);
            return lastLinesRead;
        }

        private List<InvoiceLine> GetlastLineRead(List<InvoiceLine> linesForThisStep1, string methodName, int? partId)
        {
            var linesForThisStep2 = linesForThisStep1;

            if (linesForThisStep2.Any())
            {
                int newLastLineRead = linesForThisStep2.Last().LineNumber;
                LogUpdatingLastLineRead(methodName, partId, lastLineRead, newLastLineRead);
                lastLineRead = newLastLineRead;
            }
            else
            {
                LogNoNewLinesFound(methodName, partId, lastLineRead);
            }

               
            var linesForThisStep = linesForThisStep2;
            return linesForThisStep;
        }

        private List<InvoiceLine> GetLinesForThisStep(List<InvoiceLine> newlines, string methodName, int? partId)
        {
            var linesForThisStep1 = newlines.Where(x => x != null && lastLineRead < x.LineNumber).ToList();
            LogLinesForThisStep(methodName, partId, linesForThisStep1.Count);
            return linesForThisStep1;
        }

        private List<InvoiceLine> ProcessStart(int? parentInstance, string methodName, int? partId,
            string currentEffectiveInstance)
        {
            var linesForInstanceBuffer = GetLinesForInstanceBuffer(methodName, partId);

            var triggeringLine = GetTriggeringLine(linesForInstanceBuffer, methodName, partId, currentEffectiveInstance, out var startFound);

               

            var justStartedThisCall = JustStartedThisCall(parentInstance, startFound, methodName, partId, triggeringLine);

            StartFoundButNotStarted(startFound, justStartedThisCall, triggeringLine, methodName, partId, currentEffectiveInstance);

            ProcessEnd(justStartedThisCall);

            return triggeringLine;
        }

        private void ProcessEnd(bool justStartedThisCall)
        {
            // --- End Line Detection ---
            var currentlyStarted = _startlines.Any();
            if (currentlyStarted && !justStartedThisCall && _startlines.Count() == StartCount && _endlines.Count() < EndCount && EndCount > 0)
            {
                if (OCR_Part.End.Any(z => Regex.Match(_instanceLinesTxt.ToString(), z.RegularExpressions.RegEx,
                        (z.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline) | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2)).Success))
                {
                   
                        _endlines.Add(_linesForInstanceBuffer.Last());
                   
                }
            }
            // --- End End Line Detection ---
        }

        private bool ProcessLine(string Section, string methodName, int? partId, string currentEffectiveInstance,
            List<InvoiceLine> linesForThisStep, int? parentInstance, List<InvoiceLine> triggerLiner)
        {
            bool currentlyStarted = _startlines.Any();
            var importedLineSuccess = false;
            LogMainProcessingBlockConditions(methodName, partId, currentlyStarted, _startlines.Count, StartCount, _endlines.Count, EndCount);

            if (currentlyStarted && _startlines.Count >= StartCount && ((_endlines.Count < EndCount && EndCount > 0) || EndCount == 0))
            {
                LogEnteringMainProcessingBlock(methodName, partId, currentEffectiveInstance);

                if (ChildParts != null && ChildParts.Any())
                {
                    LogProcessingChildParts(methodName, partId, ChildParts.Count);
                    int childIndex = 0;
                    ChildParts.ForEach(childPart =>
                    {
                        childIndex++;
                        if (childPart != null)
                        {
                            int? childPartId = childPart.OCR_Part?.Id;
                            LogCallingChildRead(methodName, partId, currentEffectiveInstance, childIndex, ChildParts.Count, childPartId, linesForThisStep.Count);
                            childPart.Read(new List<InvoiceLine>(linesForThisStep), Section, Instance);
                            LogFinishedChildRead(methodName, partId, currentEffectiveInstance, childPartId);
                        }
                        else
                        {
                            LogSkippingNullChildPart(methodName, partId, childIndex - 1);
                        }
                    });
                    LogFinishedProcessingChildParts(methodName, partId);
                }
                else
                {
                    LogNoChildPartsToProcess(methodName, partId);
                }

                if (Lines != null && Lines.Any())
                {
                    LogProcessingOwnLines(methodName, partId, Lines.Count);
                    int lineIndex = 0;
                    Lines.ForEach(line =>
                    {
                        lineIndex++;
                        if (line == null || line.OCR_Lines == null)
                        {
                            LogSkippingNullLine(methodName, partId, lineIndex - 1);
                            return;
                        }

                        int lineId = line.OCR_Lines.Id;

                        if (!linesForThisStep.Any())
                        {
                            LogSkippingLineProcessing(methodName, partId, lineId);
                            return;
                        }

                        var instanceLines = _lines
                            .Where(l => l != null && l.LineNumber >= _currentInstanceStartLineNumber).TakeLast(10)
                            .ToList();
                        if((!instanceLines.Any() || instanceLines?.Count() < triggerLiner?.Count() ) && triggerLiner != null && triggerLiner.Any())
                            instanceLines.AddRange(_linesForInstanceBuffer);// using the instancebuffer lines because startline is also dataline problem
                       

                        if (line.OCR_Lines.RegularExpressions?.MultiLine == true)
                        {
                            var relevantLines = instanceLines.TakeLast(line.OCR_Lines.RegularExpressions.MaxLines ?? 10).ToList();
                            var lineText = relevantLines.Select(z => z.Line).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
                            LogReadingMultiLine(methodName, partId, currentEffectiveInstance, lineIndex, Lines.Count, lineId, relevantLines.Count);
                            if (line.Read(lineText,
                                    relevantLines.FirstOrDefault()?.LineNumber ?? linesForThisStep.First().LineNumber,
                                    Section, currentEffectiveInstance))
                            {
                                importedLineSuccess = true; //ResetStartlinesifNoEndsDefined(instanceLines.Count());
                            }
                            LogLineValuesAfterRead(methodName, partId, currentEffectiveInstance, lineId, line.Values);
                        }
                        else
                        {
                            var lastLineOfStep = linesForThisStep.Last();
                            LogReadingSingleLine(methodName, partId, currentEffectiveInstance, lineIndex, Lines.Count, lineId, lastLineOfStep.LineNumber);
                            if(line.Read(lastLineOfStep.Line, lastLineOfStep.LineNumber, Section, currentEffectiveInstance))
                            {
                                importedLineSuccess = true; //ResetStartlinesifNoEndsDefined(1);// 1 last line of step count
                            }
                            LogLineValuesAfterRead(methodName, partId, currentEffectiveInstance, lineId, line.Values);
                        }
                    });
                    LogFinishedProcessingOwnLines(methodName, partId);
                }
                else
                {
                    LogNoOwnLinesToProcess(methodName, partId);
                }

                LogFinishedMainProcessingBlock(methodName, partId, currentEffectiveInstance);
            }
            else
            {
                LogMainProcessingBlockConditionsNotMet(methodName, partId, currentEffectiveInstance, currentlyStarted, _startlines.Count, StartCount, _endlines.Count, EndCount);
            }

            return importedLineSuccess;
        }

  

        private void StartFoundButNotStarted(bool startFound, bool justStartedThisCall, List<InvoiceLine> triggeringLine,
            string methodName, int? partId, string currentEffectiveInstance)
        {
            if (startFound && !WasStarted && !justStartedThisCall)
            {
                if (triggeringLine.Any() && StartCount > 0)
                {
                    _startlines.AddRange(triggeringLine);
                    _currentInstanceStartLineNumber = triggeringLine.FirstOrDefault()?.LineNumber+1?? -1;
                    if (CheckIfAllTriggerLinesInBuffer(triggeringLine))
                    {
                        LogRebuildingBufferWithTriggeringLine(methodName, partId, triggeringLine.FirstOrDefault()?.LineNumber ?? -1);
                        SyncInstanceBuffer(triggeringLine);
                    }

                    LogInitialStartDetected(methodName, partId, triggeringLine.FirstOrDefault()?.LineNumber?? -1, currentEffectiveInstance);
                    justStartedThisCall = true;
                }
                else if (StartCount > 0)
                {
                    LogFailedToAddInitialStartLine(methodName, partId);
                }
            }
        }

        private bool JustStartedThisCall(int? parentInstance, bool startFound, string methodName, int? partId,
            List<InvoiceLine> triggeringLine)
        {
            bool justStartedThisCall = false;
            if (startFound && WasStarted && (OCR_Part.RecuringPart != null && !OCR_Part.RecuringPart.IsComposite))
            {
                LogRecurringChildReset(methodName, partId, parentInstance, triggeringLine?.FirstOrDefault());
                Reset();
                _lastProcessedParentInstance = parentInstance.Value;
                LogUpdatedLastProcessedParentInstance(methodName, partId, parentInstance.Value);

                if (triggeringLine.Any() && StartCount > 0)
                {
                    _startlines.AddRange(triggeringLine);
                    _currentInstanceStartLineNumber = triggeringLine.FirstOrDefault()?.LineNumber + 1 ?? -1;//+1 to prevent startfound true on cycle
                    //_instanceLinesTxt.AppendLine(triggeringLine.Line ?? "");
                    SyncInstanceBuffer(triggeringLine);
                    LogAddedTriggeringLine(methodName, partId, triggeringLine.FirstOrDefault()?.LineNumber ?? -1);
                    justStartedThisCall = true;
                }
                else if (StartCount > 0)
                {
                    LogFailedToAddStartLine(methodName, partId);
                }
            }

            return justStartedThisCall;
        }

        private List<InvoiceLine> GetTriggeringLine(List<InvoiceLine> linesForInstanceBuffer, string methodName, int? partId,
            string currentEffectiveInstance, out bool startFound)
        {
            var triggeringLine = FindStart(linesForInstanceBuffer);
            startFound = (triggeringLine != null && triggeringLine.Any());
            if(startFound && EverStarted == null) this.EverStarted = true;
            LogFindStartResult(methodName, partId, currentEffectiveInstance, startFound, triggeringLine?.FirstOrDefault(), WasStarted);
            return triggeringLine;
        }

        private List<InvoiceLine> GetLinesForInstanceBuffer(string methodName, int? partId)
        {
            var linesForInstanceBuffer = _lines.Where(l => l != null && (_currentInstanceStartLineNumber == -1 || l.LineNumber >= _currentInstanceStartLineNumber)).TakeLast(10).ToList();
            SyncInstanceBuffer(linesForInstanceBuffer);
            LogInstanceTextBufferRebuilt(methodName, partId, _instanceLinesTxt.Length, _linesForInstanceBuffer.Count, _instanceLinesTxt.ToString());
            return _linesForInstanceBuffer;
        }

        private void SyncInstanceBuffer(List<InvoiceLine> linesForInstanceBuffer)
        {
            _linesForInstanceBuffer = linesForInstanceBuffer;
            _instanceLinesTxt.Clear();
            _linesForInstanceBuffer.ForEach(l => _instanceLinesTxt.AppendLine(l.Line));
        }

        private void SetLines(List<InvoiceLine> newlines, string methodName, int? partId)
        {
            var uniqueNewLines = newlines.Where(nl => nl != null && !_lines.Any(l => l != null && l.LineNumber == nl.LineNumber)).ToList();
            LogAccumulatedLines(methodName, partId, uniqueNewLines.Count);
            _lines.AddRange(uniqueNewLines);
            _lines.Sort((a, b) => a.LineNumber.CompareTo(b.LineNumber));
            LogInternalBufferState(methodName, partId, _lines.Count, _lines);
        }

        private void SetLastProcessedParentInstance(int? parentInstance, string methodName, int? partId)
        {
            if (parentInstance.HasValue && parentInstance.Value > _lastProcessedParentInstance)
            {
                LogResetChildState(methodName, partId, parentInstance.Value);

                ResetInternalState();
                _lastProcessedParentInstance = parentInstance.Value;
                LogUpdatedLastProcessedParentInstance(methodName, partId, parentInstance.Value);
            }
        }

        private bool CanRead(List<InvoiceLine> newlines, string methodName, int? partId)
        {
            if (newlines == null)
            {
                LogNullNewlinesWarning(methodName, partId);
                return false;
            }

            if (this.OCR_Part == null)
            {
                LogNullOCRPartError(methodName, partId);
                return false;
            }

            LogInputValidationPassed(methodName, partId);
            return true;
        }


        public bool CheckIfAllTriggerLinesInBuffer(List<InvoiceLine> triggerLines) // Assuming this is within a method
        {
            // 1. Handle nulls first
            if (triggerLines == null || _linesForInstanceBuffer == null)
            {
                return false;
            }


            // 3. Find trigger lines whose key is NOT present in the buffer keys
            //    ExceptBy likely uses a HashSet internally for efficiency.
            var linesNotInBuffer = triggerLines.ExceptBy(_linesForInstanceBuffer, tl => (tl.LineNumber, tl.Line)
);

            // 4. If there are NO lines that are not in the buffer, it means ALL lines ARE in the buffer.
            return !linesNotInBuffer.Any();
        }

        private void LogEnteringMethod(string methodName, int? partId, string currentEffectiveInstance, int? parentInstance, string Section, int newLineCount, int firstNewLineNum)
        {
            _logger.Verbose("Entering {MethodName} for PartId: {PartId}, EffectiveInstance: {EffectiveInstance} (Internal: {InternalInstance}, Parent: {ParentInstance}), Section: '{Section}'. Processing {NewLineCount} new lines starting from LineNumber: {FirstNewLineNum}. Current buffer size: {BufferLineCount}. lastLineRead: {LastLineRead}. WasStarted: {WasStarted}", methodName, partId, currentEffectiveInstance, _instance, parentInstance?.ToString() ?? "N/A", Section, newLineCount, _lines?.Count ?? 0, lastLineRead, WasStarted);
        }

        private void LogNullNewlinesWarning(string methodName, int? partId)
        {
            _logger.Warning("{MethodName}: Called with null newlines list for PartId: {PartId}. Exiting.", methodName, partId);
        }

        private void LogNullOCRPartError(string methodName, int? partId)
        {
            _logger.Error("{MethodName}: Cannot proceed for PartId: {PartId}: OCR_Part is null. Exiting.", methodName, partId);
        }

        private void LogInputValidationPassed(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: Input validation passed for PartId: {PartId}.", methodName, partId);
        }

        private void LogResetChildState(string methodName, int? partId, int parentInstanceValue)
        {
            _logger.Warning("{MethodName}: Child PartId: {PartId} detected new parent instance ({ParentInstance} > {LastProcessedParentInstance}). Resetting child state (HACK).", methodName, partId, parentInstanceValue, _lastProcessedParentInstance);
        }

        private void LogUpdatedLastProcessedParentInstance(string methodName, int? partId, int parentInstanceValue)
        {
            _logger.Debug("{MethodName}: Updated _lastProcessedParentInstance to {ParentInstance} for PartId: {PartId}", methodName, parentInstanceValue, partId);
        }

        private void LogAccumulatedLines(string methodName, int? partId, int uniqueNewLinesCount)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Added {Count} unique new lines to internal buffer.", methodName, partId, uniqueNewLinesCount);
        }

        private void LogInternalBufferState(string methodName, int? partId, int totalCount, List<InvoiceLine> lines)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Internal buffer now contains {TotalCount} lines. Content: {@Lines}", methodName, partId, totalCount, lines);
        }

        private void LogLinesForThisStep(string methodName, int? partId, int count)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - linesForThisStep count: {Count}", methodName, partId, count);
        }

        private void LogUpdatingLastLineRead(string methodName, int? partId, int oldLastLineRead, int newLastLineRead)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Updating lastLineRead from {OldLastLineRead} to {NewLastLineRead}", methodName, partId, oldLastLineRead, newLastLineRead);
        }

        private void LogNoNewLinesFound(string methodName, int? partId, int lastLineRead)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - No new lines found beyond lastLineRead ({LastLineRead}). linesForThisStep is empty.", methodName, partId, lastLineRead);
        }

        private void LogInstanceTextBufferRebuilt(string methodName, int? partId, int length, int lineCount, string content)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Instance text buffer rebuilt. Length: {Length}, Line Count: {LineCount}. Content: '{Content}'", methodName, partId, length, lineCount, content);
        }

        private void LogFindStartResult(string methodName, int? partId, string currentEffectiveInstance, bool startFound, InvoiceLine triggeringLine, bool wasStarted)
        {
            _logger.Information("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - FindStart result: {StartFound} {TriggerLineInfo}. WasStarted (before this call): {WasStarted}", methodName, partId, currentEffectiveInstance, startFound, startFound ? $"(Line: {triggeringLine?.LineNumber})" : "", wasStarted);
        }

        private void LogRecurringChildReset(string methodName, int? partId, int? parentInstance, InvoiceLine triggeringLine)
        {
            _logger.Information("{MethodName}: PARTIAL Resetting state for RECURRING CHILD PartId: {PartId} (Internal Instance: {InternalInstance}) within ParentInstance: {ParentInstance}. Found subsequent start on LineNumber: {TriggerLineNumber}.", methodName, partId, _instance, parentInstance, triggeringLine?.LineNumber ?? -1);
        }

        private void LogAddedTriggeringLine(string methodName, int? partId, int lineNumber)
        {
            _logger.Debug("{MethodName}: Added triggering LineNumber: {TriggerLineNumber} as first start line for new recurring child instance.", methodName, lineNumber);
        }

        private void LogFailedToAddStartLine(string methodName, int? partId)
        {
            _logger.Error("{MethodName}: Failed to add start line after recurring child reset (triggeringLine was null or StartCount is 0) for PartId: {PartId}.", methodName, partId);
        }

        private void LogRebuildingBufferWithTriggeringLine(string methodName, int? partId, int lineNumber)
        {
            _logger.Warning("{MethodName}: Triggering line text not found in instance buffer after FindStart. Rebuilding buffer with triggering line for PartId: {PartId}, LineNumber: {LineNumber}", methodName, partId, lineNumber);
        }

        private void LogInitialStartDetected(string methodName, int? partId, int lineNumber, string currentEffectiveInstance)
        {
            _logger.Information("{MethodName}: PartId: {PartId} - Detected INITIAL start at LineNumber: {LineNumber} for EffectiveInstance: {EffectiveInstance}", methodName, partId, lineNumber, currentEffectiveInstance);
        }

        private void LogFailedToAddInitialStartLine(string methodName, int? partId)
        {
            _logger.Error("{MethodName}: Failed to add initial start line (triggeringLine was null or StartCount is 0) for PartId: {PartId}.", methodName, partId);
        }

        private void LogMainProcessingBlockConditions(string methodName, int? partId, bool currentlyStarted, int startCountActual, int startCountRequired, int endCountActual, int endCountRequired)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Checking Main Processing Block conditions: CurrentlyStarted={CurrentlyStarted}, StartLinesCount={StartCountActual}/{StartCountRequired}, EndLinesCount={EndCountActual}/{EndCountRequired}", methodName, partId, currentlyStarted, startCountActual, startCountRequired, endCountActual, endCountRequired);
        }

        private void LogEnteringMainProcessingBlock(string methodName, int? partId, string currentEffectiveInstance)
        {
            _logger.Debug("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Conditions met. Entering main processing block.", methodName, partId, currentEffectiveInstance);
        }

        private void LogProcessingChildParts(string methodName, int? partId, int count)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Processing {Count} child parts...", methodName, partId, count);
        }

        private void LogCallingChildRead(string methodName, int? partId, string currentEffectiveInstance, int index, int total, int? childPartId, int lineCount)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Calling Read() on Child Part {Index}/{Total} (Id: {ChildPartId}) with {LineCount} lines, passing ParentInstance: {ParentInstance}.", methodName, partId, currentEffectiveInstance, index, total, childPartId, lineCount, currentEffectiveInstance);
        }

        private void LogFinishedChildRead(string methodName, int? partId, string currentEffectiveInstance, int? childPartId)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Finished Read() on Child PartId: {ChildPartId}.", methodName, partId, currentEffectiveInstance, childPartId);
        }

        private void LogSkippingNullChildPart(string methodName, int? partId, int index)
        {
            _logger.Warning("{MethodName}: Skipping null ChildPart object at index {Index} during processing for PartId: {PartId}.", methodName, index, partId);
        }

        private void LogFinishedProcessingChildParts(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Finished processing child parts.", methodName, partId);
        }

        private void LogNoChildPartsToProcess(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - No child parts to process.", methodName, partId);
        }

        private void LogProcessingOwnLines(string methodName, int? partId, int count)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Processing {Count} own lines...", methodName, partId, count);
        }

        private void LogSkippingNullLine(string methodName, int? partId, int index)
        {
            _logger.Warning("{MethodName}: Skipping null Line or Line with null OCR_Lines at index {Index} during processing for PartId: {PartId}.", methodName, index, partId);
        }

        private void LogSkippingLineProcessing(string methodName, int? partId, int lineId)
        {
            _logger.Verbose("{MethodName}: Skipping LineId: {LineId} processing as linesForThisStep is empty.", methodName, lineId);
        }

        private void LogReadingMultiLine(string methodName, int? partId, string currentEffectiveInstance, int index, int total, int lineId, int relevantLineCount)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Reading multi-line Line {Index}/{Total} (Id: {LineId}) using {RelevantLineCount} lines.", methodName, partId, currentEffectiveInstance, index, total, lineId, relevantLineCount);
        }

        private void LogLineValuesAfterRead(string methodName, int? partId, string currentEffectiveInstance, int lineId, object values)
        {
            //_logger.Verbose("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - LineId: {LineId} values after Read: {@LineValues}", methodName, partId, currentEffectiveInstance, lineId, values);
        }

        private void LogReadingSingleLine(string methodName, int? partId, string currentEffectiveInstance, int index, int total, int lineId, int lastLineNum)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Reading single-line Line {Index}/{Total} (Id: {LineId}) using last line of step ({LastLineNum}).", methodName, partId, currentEffectiveInstance, index, total, lineId, lastLineNum);
        }

        private void LogFinishedProcessingOwnLines(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - Finished processing own lines.", methodName, partId);
        }

        private void LogNoOwnLinesToProcess(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - No own lines to process.", methodName, partId);
        }

        private void LogFinishedMainProcessingBlock(string methodName, int? partId, string currentEffectiveInstance)
        {
            _logger.Debug("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Finished main processing block.", methodName, partId, currentEffectiveInstance);
        }

        private void LogMainProcessingBlockConditionsNotMet(string methodName, int? partId, string currentEffectiveInstance, bool currentlyStarted, int startCountActual, int startCountRequired, int endCountActual, int endCountRequired)
        {
            _logger.Debug("{MethodName}: PartId: {PartId}, EffectiveInstance: {EffectiveInstance} - Conditions NOT met for main processing block (CurrentlyStarted={CurrentlyStarted}, StartCount={StartCountActual}/{StartCountRequired}, EndCount={EndCountActual}/{EndCountRequired})", methodName, partId, currentEffectiveInstance, currentlyStarted, startCountActual, startCountRequired, endCountActual, endCountRequired);
        }

        private void LogExitingMethod(string methodName, int? partId)
        {
            _logger.Verbose("Exiting {MethodName} for PartId: {PartId}", methodName, partId);
        }

        private void LogUnhandledException(string methodName, int? partId, string Section, Exception e)
        {
            _logger.Error(e, "{MethodName}: Unhandled exception for PartId: {PartId}, Section: '{Section}'", methodName, partId, Section);
        }
    }
}