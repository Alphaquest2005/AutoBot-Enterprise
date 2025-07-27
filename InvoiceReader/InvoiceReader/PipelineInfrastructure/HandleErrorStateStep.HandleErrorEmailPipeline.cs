using System;
using System.IO;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Serilog;

    public partial class HandleErrorStateStep
    {
        // Added InvoiceProcessingContext context parameter
        // Added InvoiceProcessingContext context parameter
        private static async Task<bool> HandleErrorEmailPipeline(ILogger logger, InvoiceProcessingContext context, Template template, string filePath) // Add logger parameter
        {
             filePath = filePath ?? context?.FilePath ?? "Unknown"; // Get filePath from context if null
            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HandleErrorEmailPipeline), "Processing", "Starting HandleErrorEmailPipeline.", $"File: {filePath}", "");

            // Populate FileInfo and TextFilePath in template for email pipeline
            try
            {
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorEmailPipeline), "Setup", "Creating FileInfo.", $"File: {filePath}", "");
                

                // Assuming TextFilePath was set in a previous step (e.g., WriteFormattedTextFileStep)
                if (string.IsNullOrEmpty(template?.FormattedPdfText))
                {
                    logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(HandleErrorEmailPipeline), "Validation", "TextFilePath is missing in template. Email attachment might be incomplete.", $"File: {filePath}", "");
                    // Decide if this is fatal for the email process
                }
                else
                {
                    logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(HandleErrorEmailPipeline), "Setup", "Using TextFilePath from template.", $"TextFilePath: {template.FormattedPdfText}", "");
                }

                // Add FailedLines to template if not already there (needed by CreateEmailPipeline/ConstructEmailBodyStep)
                if (template?.FailedLines == null)
                {
                    logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(HandleErrorEmailPipeline), "Setup", "Template.FailedLines is null before calling CreateEmailPipeline. Attempting to populate.", $"File: {filePath}", "");
                    // Re-calculate failed lines specifically for the email body generation
                    List<Line> failedLines = GetFailedLines(logger, template); // Use the same logic, pass logger
                    AddExistingFailedLines(logger, template, failedLines); // Add existing ones too, pass logger
                    template.FailedLines = failedLines;
                    logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(HandleErrorEmailPipeline), "Setup", "Populated Template.FailedLines for email generation.", $"Count: {template.FailedLines.Count}", "");
                }


                // Create and run the CreateEmailPipeline
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorEmailPipeline), "PipelineExecution", "Creating CreateEmailPipeline instance.", $"File: {filePath}", "");
                var createEmailPipeline = new CreateEmailPipeline(logger, context); // Pass the logger and context parameters

                logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"CreateEmailPipeline.RunPipeline for File: {filePath}", "ASYNC_EXPECTED"); // Log before running email pipeline
                var emailPipelineStopwatch = Stopwatch.StartNew(); // Start stopwatch
                bool emailPipelineSuccess =
                    await createEmailPipeline.RunPipeline().ConfigureAwait(false); // Handles its own logging
                emailPipelineStopwatch.Stop(); // Stop stopwatch
                logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"CreateEmailPipeline.RunPipeline for File: {filePath}", emailPipelineStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after running email pipeline

                logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorEmailPipeline), "PipelineExecution", "CreateEmailPipeline finished.", $"File: {filePath}, Success: {emailPipelineSuccess}", "");
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorEmailPipeline), "Completion", "Exiting HandleErrorEmailPipeline.", $"File: {filePath}, Result: {emailPipelineSuccess}", "");
                return emailPipelineSuccess; // Indicate success/failure of the email pipeline run
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(HandleErrorEmailPipeline), "Handle error email pipeline", 0, $"Error during HandleErrorEmailPipeline for File: {filePath}. Error: {ex.Message}");
                return false; // Indicate failure in setting up or running the email pipeline
            }
        }
    }
}