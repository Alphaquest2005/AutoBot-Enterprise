// Assuming this is needed for _template.Format

using System.Text; // Added for StringBuilder
using System.Threading.Tasks;
using Serilog; // Added
using System; // Added for Exception
using System.Linq; // Added for OrderBy

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class FormatPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<FormatPdfTextStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Format extracted PDF text using identified templates", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(FormatPdfTextStep), $"Formatting PDF text for file: {filePath}");

            if (!ValidateContext(context, filePath))
            {
                methodStopwatch.Stop(); // Stop stopwatch on validation failure
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Format extracted PDF text using identified templates", methodStopwatch.ElapsedMilliseconds, "Context validation failed.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(FormatPdfTextStep), "Context validation", methodStopwatch.ElapsedMilliseconds, "Context validation failed.");
                return Task.FromResult(false);
            }

            bool overallSuccess = true; // Track if formatting succeeded for at least one template

            if (context?.MatchedTemplates != null)
            {
                foreach (var template in context.MatchedTemplates)
                {
                    int templateId = template.OcrTemplates?.Id ?? 0;
                    LogTemplateDetails(context.Logger, templateId, filePath, context.PdfText.Length); // Pass logger

                    try
                    {
                        string pdfTextString = context.PdfText.ToString();
                        LogFormattingStart(context.Logger, templateId); // Pass logger

                        context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            $"Template.Format for Template {templateId}", "SYNC_EXPECTED"); // Log before Format call
                        var formatStopwatch = Stopwatch.StartNew(); // Start stopwatch
                        ////////////////////////////////////////
                        template.FormattedPdfText = template.Format(pdfTextString);
                        ////////////////////////////////////////
                        formatStopwatch.Stop(); // Stop stopwatch
                        context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            $"Template.Format for Template {templateId}", formatStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after Format call

                        LogFormattedTextDetails(context.Logger, template.FormattedPdfText, templateId); // Pass logger

                        LogExecutionSuccess(context.Logger, templateId, filePath); // Pass logger
                        overallSuccess = true; // At least one template formatted successfully
                    }
                    catch (Exception ex) // Catch errors during formatting for a specific template
                    {
                        string errorMsg = $"Error formatting PDF text using TemplateId: {templateId} for File: {filePath}: {ex.Message}";
                        LogExecutionError(context.Logger, ex, templateId, filePath); // Log the error with details, pass logger
                        context.AddError(errorMsg); // Add the specific error to the context
                        overallSuccess = false; // Mark that at least one template failed formatting
                        // Do NOT return false here, continue trying other templates
                    }
                }
            }

            methodStopwatch.Stop(); // Stop stopwatch
            if (overallSuccess)
            {
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Successfully formatted PDF text using at least one template", $"OverallSuccess: {overallSuccess}", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(FormatPdfTextStep), $"Successfully formatted PDF text for file: {filePath} using at least one template", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Format extracted PDF text using identified templates", methodStopwatch.ElapsedMilliseconds, "Formatting failed for all templates.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(FormatPdfTextStep), "Formatting templates", methodStopwatch.ElapsedMilliseconds, "Formatting failed for all templates.");
            }

            return Task.FromResult(overallSuccess); // Return overall success status

        }

        private void LogExecutionStart(ILogger logger, string filePath) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "Execution", "Starting FormatPdfTextStep.", $"FilePath: {filePath}", "");
        }

        private bool ValidateContext(InvoiceProcessingContext context, string filePath)
        {
            if (context == null)
            {
                // Cannot use context.Logger if context is null - throw exception instead
                throw new ArgumentNullException(nameof(context), "FormatPdfTextStep executed with null context.");
            }
            if (!context.MatchedTemplates.Any())
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ValidateContext), "Validation", "Skipping FormatPdfTextStep: No Templates.", $"FilePath: {filePath}", "Expected templates for formatting.");
                // This is not a critical failure, just means no formatting will occur.
                return true; // Treat as successful validation but no work done
            }
            if (context.PdfText == null)
            {
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ValidateContext), "Validate pipeline context", 0, $"Skipping FormatPdfTextStep: PdfText (StringBuilder) is null for File: {filePath}.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(FormatPdfTextStep), "Context validation", 0, $"Skipping FormatPdfTextStep: PdfText (StringBuilder) is null for File: {filePath}.");
                context.AddError($"PdfText is null for file: {filePath}");
                return false; // Indicate validation failure
            }
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ValidateContext), "Validation", "Context validation successful.", $"FilePath: {filePath}", "");
            return true;
        }

        private void LogTemplateDetails(ILogger logger, int templateId, string filePath, int pdfTextLength) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateProcessing", "Formatting PDF text using template.", $"TemplateId: {templateId}, FilePath: {filePath}, OriginalPdfTextLength: {pdfTextLength}", "");
        }

        private void LogFormattingStart(ILogger logger, int templateId) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "Formatting", "Starting formatting using formatters.", $"TemplateId: {templateId}", "");
        }

        private void LogFormattedTextDetails(ILogger logger, string formattedPdfText, int templateId) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "FormattingResult", "Formatted PdfText details.", $"TemplateId: {templateId}, FormattedPdfTextLength: {formattedPdfText?.Length ?? 0}", "");
            if (!string.IsNullOrEmpty(formattedPdfText))
            {
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FormattingResult", "Formatted PdfText snippet.", $"TemplateId: {templateId}", new { Snippet = formattedPdfText.Substring(0, Math.Min(formattedPdfText.Length, 500)) });
            }
        }

        private void LogExecutionSuccess(ILogger logger, int templateId, string filePath) // Add logger parameter
        {
            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateCompletion", "PDF text formatted successfully using template.", $"TemplateId: {templateId}, FilePath: {filePath}", "");
        }

        private void LogExecutionError(ILogger logger, Exception ex, int templateId, string filePath) // Add logger parameter
        {
            logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(Execute), "Format PDF text using template", 0, $"Error formatting PDF text using TemplateId: {templateId} for File: {filePath}. Error: {ex.Message}");
            logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{nameof(FormatPdfTextStep)} - Template {templateId}", "Formatting template", 0, $"Error formatting PDF text using TemplateId: {templateId} for File: {filePath}. Error: {ex.Message}");
        }
    }
}