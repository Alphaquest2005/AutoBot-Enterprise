using System.Diagnostics;
﻿using MoreLinq;
using Core.Common.Extensions; // Added for ForEach
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using Core.Common; // Added for BetterExpando if needed by SetPartLineValues
using OCR.Business.Entities; // Added for InvoiceLine

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Assuming _logger exists from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Template>();

        // Overload for string input - simply splits and calls the main Read method

        // Primary Read method accepting list of lines
        public List<dynamic> Read(List<string> text)
        {
            // **CRITICAL TEST**: This should ALWAYS appear if this Read method is called
            _logger.Debug("**CRITICAL_READ_ENTRY**: Invoice.Read(List<string>) method called with {LineCount} lines", text?.Count ?? 0);
            _logger.Debug("**CRITICAL_READ_ENTRY**: Invoice.Read(List<string>) method called with {LineCount} lines", text?.Count ?? 0);
            // Console.WriteLine($"**CRITICAL_READ_ENTRY**: Invoice.Read(List<string>) method called with {text?.Count ?? 0} lines");
            
            var methodStopwatch = Stopwatch.StartNew();
            int? invoiceId = this.OcrTemplates?.Id;
            string invoiceName = this.OcrTemplates?.Name ?? "Unknown";
            int inputLineCount = text?.Count ?? 0;
            string methodName = nameof(Read) + "(List<string>)";
            _logger.Information("ACTION_START: {ActionName}. Context: [InvoiceId: {InvoiceId}, Name: '{InvoiceName}', InputLineCount: {InputLineCount}]",
                methodName, invoiceId, invoiceName, inputLineCount);

            // --- Input Validation ---
            if (ValidateInput(text, methodName, invoiceId, out var read))
            {
                methodStopwatch.Stop();
                _logger.Information("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: Input validation failed.",
                    methodName, methodStopwatch.ElapsedMilliseconds);
                return read;
            }

            // --- Main Processing ---
            List<dynamic>
                finalResult = new List<dynamic> { new List<IDictionary<string, object>>() }; // Default empty structure
            try
            {
                ExtractValues(text, methodName, inputLineCount, invoiceId);
                ////////////////////////

                if (ProcessValues(methodName, invoiceId, finalResult, out var finalResultList, out var list))
                {
                    methodStopwatch.Stop();
                    _logger.Information("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: No lines extracted.",
                        methodName, methodStopwatch.ElapsedMilliseconds);
                    return list;
                }

                ////////////////////

                var result = ReturnFinalResults(finalResultList, methodName, invoiceId, finalResult, _logger);

                methodStopwatch.Stop();
                _logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. ResultInstanceCount: {ResultInstanceCount}",
                    methodName, methodStopwatch.ElapsedMilliseconds, (result?.Count > 0 && result[0] is List<IDictionary<string, object>> resultList) ? resultList.Count : 0);
                return result;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop();
                _logger.Error(e, "ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Unhandled exception during processing for InvoiceId: {InvoiceId}, Name: '{InvoiceName}'.",
                    methodName, methodStopwatch.ElapsedMilliseconds, invoiceId, invoiceName);
                throw; // Re-throw original exception to signal failure
            }
        }

        // Removed LogReadError as its logic is integrated into the catch block above.

        private bool ValidateInput(List<string> text, string methodName, int? invoiceId, out List<dynamic> read)
        {
            var validationStopwatch = Stopwatch.StartNew();
            _logger.Information("ACTION_START: {ActionName}. Context: [InvoiceId: {InvoiceId}]",
                nameof(ValidateInput), invoiceId);

            var emptyResult = new List<dynamic> { new List<IDictionary<string, object>>() };
            read = emptyResult;
            if (text == null)
            {
                validationStopwatch.Stop();
                _logger.Error("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: Called with null text list. Cannot proceed.",
                    nameof(ValidateInput), validationStopwatch.ElapsedMilliseconds);
                return true;
            }

            if (this.Parts == null)
            {
                validationStopwatch.Stop();
                _logger.Error("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: Template Parts collection is null. Cannot proceed.",
                    nameof(ValidateInput), validationStopwatch.ElapsedMilliseconds);
                return true;
            }

            validationStopwatch.Stop();
            _logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Result: Input validation passed.",
                nameof(ValidateInput), validationStopwatch.ElapsedMilliseconds);
            return false;
        }

        // Removed LogValidationPassed, LogNullPartsIssue, LogNullTextIssue as their logic is integrated into ValidateInput.

        private static List<dynamic> ReturnFinalResults(List<IDictionary<string, object>> finalResultList, string methodName, int? invoiceId, List<dynamic> finalResult, ILogger logger)
        {
            var methodStopwatch = Stopwatch.StartNew();
            logger.Information("ACTION_START: {ActionName}. Context: [InvoiceId: {InvoiceId}, FinalResultListCount: {FinalResultListCount}]",
                nameof(ReturnFinalResults), invoiceId, finalResultList?.Count ?? 0);

            if (!finalResultList.Any())
            {
                methodStopwatch.Stop();
                logger.Warning("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: No instances assembled after SetPartLineValues. Returning empty list structure.",
                    nameof(ReturnFinalResults), methodStopwatch.ElapsedMilliseconds);
                // finalResult is already the empty structure
                return finalResult;
            }

            // --- Success ---
            finalResult = new List<dynamic> { finalResultList }; // Wrap the final list
            methodStopwatch.Stop();
            logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Returning {InstanceCount} assembled instances.",
                nameof(ReturnFinalResults), methodStopwatch.ElapsedMilliseconds, finalResultList.Count);
            return finalResult;
        }

        private bool ProcessValues(string methodName, int? invoiceId, List<dynamic> finalResult, out List<IDictionary<string, object>> finalResultList, out List<dynamic> list)
        {
            var methodStopwatch = Stopwatch.StartNew();
            _logger.Information("ACTION_START: {ActionName}. Context: [InvoiceId: {InvoiceId}]",
                nameof(ProcessValues), invoiceId);

            list = new List<dynamic>();
            finalResultList = new List<IDictionary<string, object>>();
            // --- Post-Iteration Processing ---
            CallAddMissingRequiredFieldValues(methodName, invoiceId);

            // Check if any values were extracted (using the Lines property which has logging)
            if (WereLinesExtracted(methodName, invoiceId, finalResult, ref list))
            {
                methodStopwatch.Stop();
                _logger.Information("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: No lines extracted.",
                    nameof(ProcessValues), methodStopwatch.ElapsedMilliseconds);
                return true;
            }


            // --- Result Assembly ---
            var ores = AssembleResults(methodName, invoiceId);

            var result = FlattenResults(methodName, out finalResultList, ores, _logger);

            methodStopwatch.Stop();
            _logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                nameof(ProcessValues), methodStopwatch.ElapsedMilliseconds);
            return result;
        }

        private static bool FlattenResults(string methodName, out List<IDictionary<string, object>> finalResultList, List<List<IDictionary<string, object>>> ores, ILogger logger)
        {
            var methodStopwatch = Stopwatch.StartNew();
            logger.Information("ACTION_START: {ActionName}. Context: [OresCount: {OresCount}]",
                nameof(FlattenResults), ores?.Count ?? 0);

            // Flatten the results from all parts safely
            finalResultList = ores
                .SelectMany(x => x ?? Enumerable.Empty<IDictionary<string, object>>()) // Safe SelectMany
                .Where(d => d != null) // Filter out potential null dictionaries
                .ToList();
            logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Flattened results. Final instance count: {FinalCount}",
                nameof(FlattenResults), "Flattening", finalResultList.Count);
            // Log the content of finalResultList
            logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Content of finalResultList before final check: {@FinalResultList}",
                nameof(FlattenResults), "ContentCheck", finalResultList);

            methodStopwatch.Stop();
            logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. ResultCount: {ResultCount}",
                nameof(FlattenResults), methodStopwatch.ElapsedMilliseconds, finalResultList.Count);
            return false; // This method doesn't indicate failure, just returns the flattened list
        }

        private List<List<IDictionary<string, object>>> AssembleResults(string methodName, int? invoiceId)
        {
            _logger.Debug(
                "{MethodName}: Calling SetPartLineValues for {PartCount} top-level parts to assemble results for InvoiceId: {InvoiceId}...",
                methodName, this.Parts.Count, invoiceId);
            var ores = Parts.Select(part =>
            {
                if (part == null)
                {
                    _logger.Warning("{MethodName}: Skipping null Part during final result assembly.", methodName);
                    return new List<IDictionary<string, object>>(); // Return empty list for null part
                }

                // Explicitly qualify Part type due to potential namespace conflicts (CS0436 warning)
                int? partId = ((WaterNut.DataSpace.Part)part).OCR_Part?.Id;
                _logger.Verbose(
                    "{MethodName}: Calling SetPartLineValues for top-level PartId: {PartId} (Instance: null)...",
                    methodName, partId);
                // Pass null for top-level calls, indicating no instance filtering yet
                // SetPartLineValues should handle its own logging
                _logger.Debug("**READ_CS_DEBUG**: About to call SetPartLineValues for PartId: {PartId}", partId);
                var partResultList = SetPartLineValues(part, null);
                _logger.Debug("**READ_CS_DEBUG**: SetPartLineValues returned {Count} items for PartId: {PartId}", partResultList?.Count ?? 0, partId);
                _logger.Verbose(
                    "{MethodName}: Finished SetPartLineValues for PartId: {PartId}. Returned {Count} items.",
                    methodName, partId, partResultList?.Count ?? 0);
                return partResultList;
            }).ToList();
            _logger.Debug(
                "{MethodName}: Finished calling SetPartLineValues for all {PartCount} top-level parts. Raw result list count: {OresCount}",
                methodName, this.Parts.Count, ores.Count);
            // Log the content of ores
            _logger.Verbose("{MethodName}: Content of ores after Part.Select: {@Ores}", methodName, ores);
            return ores;
        }

        private bool WereLinesExtracted(string methodName, int? invoiceId, List<dynamic> finalResult, ref List<dynamic> list)
        {
            _logger.Verbose("{MethodName}: Checking if any lines were extracted via Lines property access...",
                methodName);
            if (!Lines.Any()) // Accessing Lines property triggers its getter logic and logging
            {
                _logger.Warning(
                    "{MethodName}: No values found in any lines after processing for InvoiceId: {InvoiceId}. Returning empty list structure.",
                    methodName, invoiceId);
                // finalResult is already the empty structure
                _logger.Information(
                    "Exiting {MethodName} for InvoiceId: {InvoiceId} because no lines were extracted.", methodName,
                    invoiceId);
                list = finalResult;
                return true;
            }

            _logger.Verbose("{MethodName}: Lines property indicates values were extracted.", methodName);
            return false;
        }

        private void CallAddMissingRequiredFieldValues(string methodName, int? invoiceId)
        {
            _logger.Debug("{MethodName}: Calling AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}...",
                methodName, invoiceId);
            AddMissingRequiredFieldValues(); // Assumes this method handles its own logging
            _logger.Debug("{MethodName}: Finished AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}.",
                methodName, invoiceId);
        }

        private void ExtractValues(List<string> text, string methodName, int inputLineCount, int? invoiceId)
        {
            _logger.Verbose("{MethodName}: ExtractValues called. Input text: {@Text}", methodName, text); // Log input text
            var lineCount = 0;
            var section = ""; // Current section name
            LogStartIteration(methodName, inputLineCount, invoiceId);

            // --- Line Iteration ---
            foreach (var lineText in text)
            {
                lineCount++;
                section = GetSection(methodName, inputLineCount, section, lineCount, lineText);

                var iLine = CreateInvoiceLines(methodName, lineText, lineCount);

                // Call Read on each Part

                ////////////////////////////////
                ProcessParts(methodName, lineCount, section, iLine);
                ///////////////////////////////

            }

            LogEndIteration(methodName, inputLineCount, invoiceId);
        }

        private void ProcessParts(string methodName, int lineCount, string section, List<InvoiceLine> iLine)
        {
            _logger.Verbose("{MethodName}: ProcessParts called. Input iLine: {@ILine}", methodName, iLine); // Log iLine
            LogCallingPartRead(methodName, lineCount, this.Parts.Count, section);
            Parts.ForEach(part =>
            {
                if (part != null)
                {
                    ////////////////////////////////
                    ReadLine(methodName, lineCount, section, iLine, part);
                    /////////////////////////////
                }
                else
                {
                    LogSkippingNullPart(methodName, lineCount);
                }
            });
        }

        private void ReadLine(string methodName, int lineCount, string section, List<InvoiceLine> iLine, Part part)
        {
            int? partId = ((WaterNut.DataSpace.Part)part).OCR_Part?.Id;
            LogPartReadStart(methodName, lineCount, partId, section);

            // Log inputs to part.Read()
            _logger.Verbose("{MethodName}: Calling part.Read() for PartId: {PartId}. Input iLine: {@ILine}, Section: '{Section}', Instance: {Instance}",
                methodName, partId, iLine, section, part.Instance);

            part.Read(iLine, section, part.Instance);// need to call itself passing its own instance again to set the currenteffectiveinstance

            // Log outputs from part.Read()
            _logger.Verbose("{MethodName}: part.Read() returned for PartId: {PartId}. Part Values: {@PartValues}",
                methodName, partId, ((WaterNut.DataSpace.Part)part).Values);

            LogPartReadEnd(methodName, lineCount, partId);
            LogPartValues(methodName, lineCount, partId, ((WaterNut.DataSpace.Part)part).Values);
        }

        private List<InvoiceLine> CreateInvoiceLines(string methodName, string lineText, int lineCount)
        {
            // Create InvoiceLine object
            var iLine = new List<InvoiceLine>() { new InvoiceLine(lineText, lineCount) };

            LogInvoiceLineCreated(methodName, lineCount, iLine);
            return iLine;
        }

        private string GetSection(string methodName, int inputLineCount, string section, int lineCount, string lineText)
        {
            string previousSection = section; // Store previous section for logging change

            LogProcessingLine(methodName, lineCount, inputLineCount, lineText);

            // Determine current section based on predefined markers
            string detectedSectionKey = Sections.FirstOrDefault(s =>
                lineText != null && s.Value != null &&
                lineText.Contains(s.Value)).Key;
            if (detectedSectionKey != null)
            {
                section = detectedSectionKey;
                if (section != previousSection)
                {
                    LogSectionChange(methodName, lineCount, previousSection, section);
                }
            }

            LogCurrentSection(methodName, lineCount, section);
            return section;
        }

        private void LogStartIteration(string methodName, int inputLineCount, int? invoiceId)
        {
            _logger.Debug(
                "{MethodName}: Starting iteration through {LineCount} input text lines for InvoiceId: {InvoiceId}...",
                methodName, inputLineCount, invoiceId);
        }

        private void LogProcessingLine(string methodName, int lineCount, int inputLineCount, string lineText)
        {
            _logger.Verbose(
                "{MethodName}: Processing Line {LineNum}/{TotalLines}. Text: '{LineText}'",
                methodName, lineCount, inputLineCount, lineText);
        }

        private void LogSectionChange(string methodName, int lineCount, string previousSection, string currentSection)
        {
            _logger.Debug(
                "{MethodName}: Line {LineNum}: Section changed from '{PreviousSection}' to '{CurrentSection}'.",
                methodName, lineCount, previousSection, currentSection);
        }

        private void LogCurrentSection(string methodName, int lineCount, string section)
        {
            _logger.Verbose(
                "{MethodName}: Line {LineNum}: Current Section='{CurrentSection}'.",
                methodName, lineCount, section);
        }

        private void LogInvoiceLineCreated(string methodName, int lineCount, List<InvoiceLine> iLine)
        {
            _logger.Verbose("{MethodName}: Line {LineNum}: Created InvoiceLine object.", methodName, lineCount);
            _logger.Verbose("{MethodName}: Line {LineNum}: InvoiceLine content: {@InvoiceLine}", methodName, lineCount, iLine);
        }

        private void LogCallingPartRead(string methodName, int lineCount, int partCount, string section)
        {
            _logger.Verbose("{MethodName}: Line {LineNum}: Calling Part.Read for {PartCount} parts with section '{CurrentSection}'.",
                methodName, lineCount, partCount, section);
        }

        private void LogPartReadStart(string methodName, int lineCount, int? partId, string section)
        {
            _logger.Verbose("{MethodName}: Line {LineNum}: Calling Read() on PartId: {PartId} with section '{CurrentSection}'...",
                methodName, lineCount, partId, section);
        }

        private void LogPartReadEnd(string methodName, int lineCount, int? partId)
        {
            _logger.Verbose("{MethodName}: Line {LineNum}: Finished Read() on PartId: {PartId}.",
                methodName, lineCount, partId);
        }

        private void LogPartValues(string methodName, int lineCount, int? partId, object partValues)
        {
            _logger.Verbose("{MethodName}: Line {LineNum}: PartId: {PartId} values after Read: {@PartValues}",
                methodName, lineCount, partId, partValues);
        }

        private void LogSkippingNullPart(string methodName, int lineCount)
        {
            _logger.Warning(
                "{MethodName}: Line {LineNum}: Skipping null Part object during Read iteration.",
                methodName, lineCount);
        }

        private void LogEndIteration(string methodName, int inputLineCount, int? invoiceId)
        {
            _logger.Debug(
                "{MethodName}: Finished iterating through {LineCount} text lines for InvoiceId: {InvoiceId}.",
                methodName, inputLineCount, invoiceId);
        }

        // Assuming SetPartLineValues exists in another partial class part
        // private List<IDictionary<string, object>> SetPartLineValues(Part part, int? instance) { ... }
    }
}