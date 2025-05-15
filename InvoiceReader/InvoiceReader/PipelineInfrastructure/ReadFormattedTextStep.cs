// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System;
using System.Runtime.InteropServices; // Added
using OCR.Business.Entities; // Added for Template
using Core.Common.Extensions; // Added for BetterExpando

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<ReadFormattedTextStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Read formatted PDF text based on template structure", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(ReadFormattedTextStep), $"Reading formatted text for file: {filePath}");

            // Basic context validation (null check)
            if (context == null)
            {
                // Cannot use context.Logger if context is null
                Log.ForContext<ReadFormattedTextStep>().Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Read formatted PDF text based on template structure", 0, "ReadFormattedTextStep executed with null context.");
                Log.ForContext<ReadFormattedTextStep>().Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ReadFormattedTextStep), "Context validation", 0, "ReadFormattedTextStep executed with null context.");
                // Cannot add error as context is null
                return Task.FromResult(false);
            }
             if (context.Templates == null || !context.Templates.Any())
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

            string filePath = context.FilePath ?? "Unknown"; // Safe now due to null check above

            foreach (var template in context.Templates)
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
                        continue; // Continue to the next template
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
                         template.CsvLines = template.Read(textLines); // The core operation
                         readStopwatch.Stop(); // Stop stopwatch
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
                         overallSuccess = false; // Mark that this template failed
                         continue; // Continue to the next template
                    }
                    // --- End Template Read Execution ---


                    // --- Result Check ---
                    if (!ExecutionSuccess(context.Logger, template, filePath)) // Checks if CsvLines is null or empty, pass logger
                    {
                         // ExecutionSuccess logs the specific reason (empty CsvLines)
                         string errorMsg = $"No CsvLines generated after read for TemplateId: {templateId}, File: {filePath}.";
                         context.AddError(errorMsg); // Add error to context
                         overallSuccess = false; // Mark that this template failed
                         continue; // Continue to the next template
                    }
                     // --- End Result Check ---

                     // If we reach here, this template was processed successfully. Continue to the next if any.
                     LogExecutionSuccess(context.Logger, filePath, templateId); // Log individual template success, pass logger
                     overallSuccess = true; // At least one template was successful

                }
                catch (Exception ex) // Catch unexpected errors within the loop but outside template.Read()
                {
                    string errorMsg = $"Unexpected error processing TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}: {ex.Message}";
                    LogExecutionError(context.Logger, ex, filePath, templateId); // Log detailed error, pass logger
                    context.AddError(errorMsg); // Add error to context
                    template.CsvLines = null; // Ensure CsvLines is null
                    overallSuccess = false; // Mark that this template failed
                    continue; // Continue to the next template
                }
            }

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
        }

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