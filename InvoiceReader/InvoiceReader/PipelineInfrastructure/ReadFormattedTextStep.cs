// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using Serilog.Events; // Added for LogEventLevel
using System;
using System.Runtime.InteropServices; // Added
using OCR.Business.Entities; // Added for Template
using Core.Common.Extensions; // Added for BetterExpando

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    using CoreEntities.Business.Entities;

    using WaterNut.Business.Services.Utils;

    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<ReadFormattedTextStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // Ensure verbose logging for this method
            {
                var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
                string filePath = context?.FilePath ?? "Unknown";
                context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                    nameof(Execute), "Read formatted PDF text based on template structure", $"FilePath: {filePath}");

                context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ReadFormattedTextStep), $"Reading formatted text for file: {filePath}");


                 if (!context.MatchedTemplates.Any())
                {
                     context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "Validation", "Skipping ReadFormattedTextStep: No Templates found in context.", $"FilePath: {filePath}", "Expected templates for reading.");
                     // Not necessarily an error, but nothing to process. Consider if this should be true or false based on pipeline logic.
                     // Returning true as no processing *failed*, just skipped.
                     methodStopwatch.Stop(); // Stop stopwatch on skip
                     context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                         nameof(Execute), "Skipped due to no templates", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                     context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                         nameof(ReadFormattedTextStep), $"Skipped reading formatted text for file: {filePath} (no templates)", methodStopwatch.ElapsedMilliseconds);
                     return Task.FromResult(true);
                }

                bool overallSuccess = true; // Track if at least one template was read successfully

              foreach (var template in context.MatchedTemplates)
                {
                     int? templateId = template?.OcrInvoices?.Id; // Get template ID safely
                     string templateName = template?.OcrInvoices?.Name ?? "Unknown";

                    try
                    {
                        // --- Validation ---
                        if (!ExecutionValidation(context.Logger, template, filePath)) // Pass logger
                        {
                            // ExecutionValidation logs the specific reason
                            string errorMsg = $"Validation failed for TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}.";
                            context.AddError(errorMsg); // Add error to context
                            overallSuccess = false; // Mark that this template failed
                            context.AddError(errorMsg); // Add error to context
                            methodStopwatch.Stop(); // Stop stopwatch immediately
                            context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                nameof(Execute), "Read formatted PDF text based on template structure", methodStopwatch.ElapsedMilliseconds, "Validation failed for a template. Terminating early.");
                            context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(ReadFormattedTextStep), "Validation", methodStopwatch.ElapsedMilliseconds, "Validation failed for a template. Terminating early.");
                            continue;
                        }
                        // --- End Validation ---

                        var textLines = GetTextLinesFromFormattedPdfText(context.Logger, template, filePath); // Pass logger

                        // --- Template Read Execution ---
                        try
                        {
                             LogCallingTemplateRead(context.Logger, textLines.Count, filePath, templateId); // Pass logger
                             context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                 $"Template.Read for Template {templateId}", "SYNC_EXPECTED"); // Log before Read call
                             var readStopwatch = Stopwatch.StartNew(); // Start stopwatch

                             context.Logger?.Verbose("Template Parts for TemplateId: {TemplateId}: {@Parts}",
                                 templateId, template.OcrInvoices?.Parts); // Log template parts
                             context.Logger?.Verbose("Template RegEx for TemplateId: {TemplateId}: {@RegEx}",
                                 templateId, template.OcrInvoices?.RegEx); // Log template regex

                             context.Logger?.Verbose("Calling template.Read() for TemplateId: {TemplateId}. Input textLines: {@TextLines}",
                                 templateId, textLines); // Log input textLines

                             template.CsvLines = template.Read(textLines); // The core operation

                            // --- Resolve File Type ---
                            context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(Execute), "FileTypeResolution", "Resolving file type.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                            FileTypes fileType = HandleImportSuccessStateStep.ResolveFileType(context.Logger, template); // Handles its own logging, pass logger
                            if (fileType == null)
                            {
                                string errorMsg = $"ResolveFileType returned null for File: {filePath}, TemplateId: {templateId}. Cannot proceed.";
                                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                    nameof(Execute), "Resolve file type", 0, errorMsg);
                                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                    $"{nameof(HandleImportSuccessStateStep)} - Template {templateId}", "File type resolution", 0, errorMsg);
                                context.AddError(errorMsg); // Add error to context
                                continue;
                            }
                            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(Execute), "FileTypeResolution", "Resolved FileType.", $"FileTypeId: {fileType.Id}, FilePath: {filePath}", "");
                            // --- End Resolve File Type ---

                            template.FileType = fileType;

                            if (template.FileType.FileImporterInfos.EntryType
                                 == FileTypeManager.EntryTypes.ShipmentInvoice)
                             {
                                 var res = template.CsvLines;

                                 // Log the initial CsvLines result for totals calculation debugging
                                 context.Logger?.Information("OCR_CORRECTION_DEBUG: Initial CsvLines result from template.Read()");
                                 if (res != null)
                                 {
                                     context.Logger?.Information("OCR_CORRECTION_DEBUG: Initial CsvLines Count: {Count}", res.Count);
                                     for (int i = 0; i < res.Count; i++)
                                     {
                                         context.Logger?.Information("OCR_CORRECTION_DEBUG: Initial CsvLine[{Index}]: {@CsvLineData}", i, res[i]);
                                     }
                                 }
                                 else
                                 {
                                     context.Logger?.Warning("OCR_CORRECTION_DEBUG: Initial CsvLines is null");
                                 }

                                 // Calculate and log TotalsZero using the CsvLines result
                                 var totalsZero = OCRCorrectionService.TotalsZero(res);
                                 context.Logger?.Information("OCR_CORRECTION_DEBUG: TotalsZero calculation from CsvLines = {TotalsZero}", totalsZero);

                                 // Log line values for DeepSeek mapping
                                 context.Logger?.Information("OCR_CORRECTION_DEBUG: Template line values for DeepSeek mapping");
                                 if (template.Lines != null)
                                 {
                                     context.Logger?.Information("OCR_CORRECTION_DEBUG: Template Lines Count: {Count}", template.Lines.Count);
                                     for (int lineIndex = 0; lineIndex < template.Lines.Count; lineIndex++)
                                     {
                                         var line = template.Lines[lineIndex];
                                         if (line?.Values != null && line.Values.Any())
                                         {
                                             context.Logger?.Information("OCR_CORRECTION_DEBUG: Template Line[{LineIndex}] Values: {@LineValues}", lineIndex, line.Values);
                                         }
                                     }
                                 }
                                 else
                                 {
                                     context.Logger?.Warning("OCR_CORRECTION_DEBUG: Template Lines is null");
                                 }

                                 int correctionAttempts = 0;
                                 const int maxCorrectionAttempts = 1; // Circuit breaker: only 1 attempt to prevent infinite loops

                                 while (Math.Abs(OCRCorrectionService.TotalsZero(res)) > 0.01 && correctionAttempts < maxCorrectionAttempts)
                                 {
                                     correctionAttempts++;
                                     context.Logger?.Information("OCR_CORRECTION_DEBUG: Starting correction attempt {Attempt} of {MaxAttempts}. Current TotalsZero = {TotalsZero}", correctionAttempts, maxCorrectionAttempts, OCRCorrectionService.TotalsZero(res));

                                     // Apply OCR correction using the CsvLines result and template
                                     OCRCorrectionService.CorrectInvoices(res, template);

                                     // Clear and re-read to get updated values
                                     template.CsvLines = null;
                                     template.Lines.ForEach(x => x.Values.Clear());
                                     res = template.Read(textLines); // Re-read after correction

                                     var newTotalsZero = OCRCorrectionService.TotalsZero(res);
                                     context.Logger?.Information("OCR_CORRECTION_DEBUG: After correction attempt {Attempt}, new TotalsZero = {TotalsZero}", correctionAttempts, newTotalsZero);
                                 }

                                 if (correctionAttempts >= maxCorrectionAttempts)
                                 {
                                     var finalTotalsZero = OCRCorrectionService.TotalsZero(res);
                                     context.Logger?.Warning("OCR_CORRECTION_DEBUG: Circuit breaker triggered - maximum correction attempts ({MaxAttempts}) reached. Final TotalsZero = {TotalsZero}", maxCorrectionAttempts, finalTotalsZero);
                                 }
                                 else if (correctionAttempts > 0)
                                 {
                                     var finalTotalsZero = OCRCorrectionService.TotalsZero(res);
                                     context.Logger?.Information("OCR_CORRECTION_DEBUG: OCR correction completed successfully after {Attempts} attempts. Final TotalsZero = {TotalsZero}", correctionAttempts, finalTotalsZero);
                                 }
                                 else
                                 {
                                     context.Logger?.Information("OCR_CORRECTION_DEBUG: No OCR correction needed. TotalsZero = {TotalsZero}", totalsZero);
                                 }

                                 template.CsvLines = res;
                             }


                             readStopwatch.Stop(); // Stop stopwatch

                             context.Logger?.Verbose("template.Read() returned. TemplateId: {TemplateId}. CsvLines: {@CsvLines}",
                                 templateId, template.CsvLines); // Log output CsvLines

                             context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                 $"Template.Read for Template {templateId}", readStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after Read call
                             LogTemplateReadFinished(context.Logger, filePath, templateId, template.CsvLines?.Count ?? 0); // Pass logger
                         }
                         catch (Exception readEx) // Catch errors specifically from template.Read()
                         {
                             string errorMsg = $"Error executing template.Read() for TemplateId: {templateId}, File: {filePath}: {readEx.Message}";
                             LogExecutionError(context.Logger, readEx, filePath, templateId); // Log detailed error, pass logger
                             context.AddError(errorMsg); // Add error to context
                             template.CsvLines = null; // Ensure CsvLines is null after failure
                             overallSuccess = false;
                            methodStopwatch.Stop(); // Stop stopwatch immediately
                             context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                 nameof(Execute), "Read formatted PDF text based on template structure", methodStopwatch.ElapsedMilliseconds, "Template.Read() failed. Terminating early.");
                             context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                 nameof(ReadFormattedTextStep), "Template reading", methodStopwatch.ElapsedMilliseconds, "Template.Read() failed. Terminating early.");

                             continue;
                         }
                         // --- End Template Read Execution ---


                        // --- Result Check ---
                        if (!ExecutionSuccess(context.Logger, template, filePath)) // Checks if CsvLines is null or empty, pass logger
                        {
                             // ExecutionSuccess logs the specific reason (empty CsvLines)
                             string errorMsg = $"No CsvLines generated after read for TemplateId: {templateId}, File: {filePath}.";
                             context.AddError(errorMsg); // Add error to context
                             methodStopwatch.Stop(); // Stop stopwatch immediately
                             context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                 nameof(Execute), "Read formatted PDF text based on template structure", methodStopwatch.ElapsedMilliseconds, "No CsvLines generated. Terminating early.");
                             context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                 nameof(ReadFormattedTextStep), "Result check", methodStopwatch.ElapsedMilliseconds, "No CsvLines generated. Terminating early.");
                             return Task.FromResult(false); // Terminate pipeline on first empty CsvLines result
                        }
                         // --- End Result Check ---

                         // If we reach here, this template was processed successfully.
                         LogExecutionSuccess(context.Logger, filePath, templateId); // Log individual template success, pass logger
                         // If a template is successful, we assume this is the correct one and stop processing others.
                         methodStopwatch.Stop(); // Stop stopwatch
                         context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                             nameof(Execute), "Successfully read formatted text for a template. Terminating early as successful.", $"OverallSuccess: {true}, TemplateId: {templateId}", methodStopwatch.ElapsedMilliseconds);
                         context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                             nameof(ReadFormattedTextStep), $"Successfully read formatted text for file: {filePath} using TemplateId: {templateId}. Terminating early.", methodStopwatch.ElapsedMilliseconds);
                         return Task.FromResult(true); // Indicate success and stop processing further templates
                     }
                     catch (Exception ex) // Catch unexpected errors within the loop but outside template.Read()
                     {
                         string errorMsg = $"Unexpected error processing TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}: {ex.Message}";
                         LogExecutionError(context.Logger, ex, filePath, templateId); // Log detailed error, pass logger
                         context.AddError(errorMsg); // Add error to context
                         template.CsvLines = null; // Ensure CsvLines is null
                         methodStopwatch.Stop(); // Stop stopwatch immediately
                         context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                             nameof(Execute), "Read formatted PDF text based on template structure", methodStopwatch.ElapsedMilliseconds, "Unexpected error processing template. Terminating early.");
                         context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                             nameof(ReadFormattedTextStep), "Unexpected error during template processing", methodStopwatch.ElapsedMilliseconds, "Unexpected error processing template. Terminating early.");
                         return Task.FromResult(false); // Terminate pipeline on first unexpected error
                     }
                 }

             // If the loop completes without finding a successful template, or if it was empty initially
             methodStopwatch.Stop(); // Stop stopwatch
             if (overallSuccess) // This branch will only be hit if context.Templates was empty initially
             {
                  context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                      nameof(Execute), "Skipped due to no templates or all templates failed but no early exit triggered (should not happen with new logic).", $"OverallSuccess: {overallSuccess}", methodStopwatch.ElapsedMilliseconds);
                  context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                      nameof(ReadFormattedTextStep), $"Skipped reading formatted text for file: {filePath} (no templates or all templates failed)", methodStopwatch.ElapsedMilliseconds);
             }
             else // This branch will be hit if all templates failed and no early exit was triggered (should not happen with new logic)
             {
                  context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                      nameof(Execute), "Read formatted PDF text based on template structure", methodStopwatch.ElapsedMilliseconds, "Reading formatted text failed for all templates.");
                  context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                      nameof(ReadFormattedTextStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Reading formatted text failed for all templates.");
             }

             return Task.FromResult(overallSuccess); // Return overall success status

            // If the loop completes, check overallSuccess
            methodStopwatch.Stop(); // Stop stopwatch
            if (overallSuccess)
            {
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Successfully read formatted text for at least one template", $"OverallSuccess: {overallSuccess}", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(ReadFormattedTextStep), $"Successfully read formatted text for file: {filePath} using at least one template", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                 context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Read formatted PDF text based on template structure", methodStopwatch.ElapsedMilliseconds, "Reading formatted text failed for all templates.");
                 context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(ReadFormattedTextStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Reading formatted text failed for all templates.");
            }

            return Task.FromResult(overallSuccess); // Return overall success status
        } // Closing brace for the 'using' block
        } // Closing brace for the 'Execute' method

        // Validation specific to one template instance
        private bool ExecutionValidation(ILogger logger, Invoice template, string filePath) // Add logger parameter
        {
             if (template == null || template.OcrInvoices == null)
             {
                  LogNullTemplateWarning(logger, filePath); // Logs appropriate message, pass logger
                  return false;
             }

             int? templateId = template.OcrInvoices.Id; // Safe now
             string templateName = template.OcrInvoices.Name; // Safe now
             LogExecutionStart(logger, filePath, templateId, templateName); // Pass logger

            if (string.IsNullOrEmpty(template.FormattedPdfText))
            {
                LogEmptyFormattedPdfTextWarning(logger, filePath, templateId); // Pass logger
                return false;
            }

            return true;
        }

        // Checks if the result of template.Read() is valid (not null/empty)
        private bool ExecutionSuccess(ILogger logger, Invoice template, string filePath) // Add logger parameter
        {
            // Note: Logging for finish/counts moved to main Execute method for better flow control view

            if (template.CsvLines == null || !template.CsvLines.Any())
            {
                LogEmptyCsvLinesWarning(logger, filePath, template?.OcrInvoices?.Id); // Log the specific issue, pass logger
                return false; // Indicate failure
            }

            // Logging for success moved to main Execute method after this check passes
            return true; // Indicate success
        }

        private List<string> GetTextLinesFromFormattedPdfText(ILogger logger, Invoice template, string filePath) // Add logger parameter
        {
            LogDataExtractionStart(logger, filePath, template.OcrInvoices.Id); // Pass logger
            LogFormattedPdfText(logger, template.FormattedPdfText); // Pass logger

            if (template?.OcrInvoices?.Parts != null)
            {
                LogTemplateRegexPatterns(logger, template.OcrInvoices.Parts); // Pass logger
            }

            var textLines = template.FormattedPdfText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            LogSplitTextLines(logger, textLines.Count); // Pass logger

            var topLevelParts = template.OcrInvoices.Parts
                .Where(p => (p.ParentParts.Any() && !p.ChildParts.Any()) ||
                            (!p.ParentParts.Any() && !p.ChildParts.Any()))
                .ToList();

            LogTopLevelPartsIdentified(logger, topLevelParts.Count); // Pass logger
            // Logging moved to main Execute method just before the call
            return textLines;
        }

        private void LogExecutionStart(ILogger logger, string filePath, int? templateId, string templateName) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "Execution", "Executing ReadFormattedTextStep for template.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");
        }

        private void LogNullContextError()
        {
            // This is logged before context is validated, so cannot use context.Logger
            Log.ForContext<ReadFormattedTextStep>().Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "ContextValidation", "ReadFormattedTextStep executed with null context.", "", "");
        }

        private void LogNullTemplateWarning(ILogger logger, string filePath) // Add logger parameter
        {
            logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ExecutionValidation), "Validation", "Skipping template: Template is null.", $"FilePath: {filePath}", "Expected a valid template object.");
        }

        private void LogEmptyFormattedPdfTextWarning(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ExecutionValidation), "Validation", "Skipping template: FormattedPdfText is null or empty.", $"FilePath: {filePath}, TemplateId: {templateId}", "Expected formatted text for reading.");
        }

        private void LogDataExtractionStart(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Starting data extraction using template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
        }

        private void LogFormattedPdfText(ILogger logger, string formattedPdfText) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "FormattedPdfText content.", "", new { FormattedPdfText = formattedPdfText });
        }

        private void LogTemplateRegexPatterns(ILogger logger, List<Parts> parts) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Template Regex Patterns.", "", new { Parts = parts.Select(p => new { PartId = p.Id, Lines = p.Lines?.Select(l => new { LineName = l.Name, Regex = l.RegularExpressions?.RegEx }) }) });
        }

        private void LogSplitTextLines(ILogger logger, int lineCount) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Split FormattedPdfText into lines.", $"LineCount: {lineCount}", "");
        }

        private void LogTopLevelPartsIdentified(ILogger logger, int topLevelPartCount) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Identified top-level parts from template.", $"TopLevelPartCount: {topLevelPartCount}", "");
        }

        private void LogCallingTemplateRead(ILogger logger, int lineCount, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateRead", "Calling template.Read().", $"LineCount: {lineCount}, FilePath: {filePath}, TemplateId: {templateId}", "");
        }

        // Log message updated slightly for clarity
        private void LogTemplateReadFinished(ILogger logger, string filePath, int? templateId, int resultCount) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateReadResult", "template.Read() finished.", $"FilePath: {filePath}, TemplateId: {templateId}, ResultingCsvLinesCount: {resultCount}", "");
        }

        private void LogEmptyCsvLinesWarning(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ExecutionSuccess), "ResultCheck", "CsvLines is null or empty after extraction attempt.", $"FilePath: {filePath}, TemplateId: {templateId}", "Step fails for this template.");
        }

        private void LogExecutionSuccess(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateCompletion", "ReadFormattedTextStep finished successfully for template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
        }

        private void LogExecutionError(ILogger logger, Exception ex, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(Execute), "Read formatted PDF text based on template structure", 0, $"Error during ReadFormattedTextStep for File: {filePath}, TemplateId: {templateId}. Error: {ex.Message}");
            logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{nameof(ReadFormattedTextStep)} - Template {templateId}", "Processing template", 0, $"Error during ReadFormattedTextStep for File: {filePath}, TemplateId: {templateId}. Error: {ex.Message}");
        }
    }
}