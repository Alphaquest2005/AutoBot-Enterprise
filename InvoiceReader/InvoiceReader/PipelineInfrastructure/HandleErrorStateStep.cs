using OCR.Business.Entities; // Added
using System.Collections.Generic; // Added
using System.IO; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class HandleErrorStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<HandleErrorStateStep>();
        private readonly bool _isLastTemplate;
        // Removed static InvoiceProcessingContext _context; - Use context passed to Execute

        public HandleErrorStateStep(bool isLastTemplate)
        {
            _isLastTemplate = isLastTemplate;
            // Removed static logger usage in constructor
            // _logger.Debug("HandleErrorStateStep initialized with IsLastTemplate: {IsLastTemplate}", _isLastTemplate);
            // _logger.Warning("Note: _isLastTemplate field is initialized but currently commented out in IsValidErrorState check.");
        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Handle error state during invoice processing", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(HandleErrorStateStep), $"Handling error state for file: {filePath}");

             // Basic context validation
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "HandleErrorStateStep executed with null context.");
            }
             // Removed static context assignment: _context = context;
             
             // Check for templates - if none, step succeeds vacuously.
             if (!context.MatchedTemplates.Any())
             {
                  context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                      nameof(Execute), "Validation", "Skipping HandleErrorStateStep: No Templates found in context.", $"FilePath: {filePath}", "Expected templates for error handling.");
                  methodStopwatch.Stop(); // Stop stopwatch on skip
                  context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                      nameof(Execute), "Skipped due to no templates", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                  context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                      nameof(HandleErrorStateStep), $"Skipped handling error state for file: {filePath} (no templates)", methodStopwatch.ElapsedMilliseconds);
                  return true;
             }

            bool overallStepSuccess = true; // Track success across all templates

            foreach (var template in context.MatchedTemplates)
            {
                 int? templateId = template?.OcrTemplates?.Id; // Safe access
                 string templateName = template?.OcrTemplates?.Name ?? "Unknown";
                 context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "TemplateProcessing", "Processing template for error handling.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");
                 context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "TemplateProcessing", "Context details at start of template error handling.", $"FilePath: {filePath}, TemplateId: {templateId}", new { Context = context });

                 try // Wrap processing for each template
                 {
                     // --- Validation for this template/context ---
                     // Assuming HasMissingRequiredData checks the *overall* context, not just the template
                     if (HasMissingRequiredData(context.Logger, context)) // Handles its own logging, pass logger
                     {
                         string errorMsg = $"HandleErrorStateStep cannot proceed due to missing required data in context for File: {filePath}, TemplateId: {templateId}";
                         // Logging is handled by helper
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false; // Mark step as failed
                         break; // Stop processing other templates
                     }
                     // --- End Validation ---

                     // --- Process Template Logic ---
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateProcessing", "Starting template processing logic.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     // Pass the full context, not just the template
                     bool templateProcessResult = await ProcessTemplate(context.Logger, context, template, filePath, templateId).ConfigureAwait(false); // Pass logger
                     
                     if (!templateProcessResult)
                     {
                          // ProcessTemplate or its helpers should have logged and added the specific error to context.Errors
                          context.Logger?.Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                              nameof(Execute), "TemplateProcessing", "ProcessTemplate failed.", $"FilePath: {filePath}, TemplateId: {templateId}", "See previous errors.");
                          overallStepSuccess = false;
                          break; // Stop processing other templates
                     }
                     context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateProcessing", "Finished processing template successfully.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     // --- End Process Template Logic ---

                     context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateCompletion", "Finished executing HandleErrorStateStep successfully for template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Unexpected error during HandleErrorStateStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Execute), "Handle error state during invoice processing", 0, errorMsg);
                     context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         $"{nameof(HandleErrorStateStep)} - Template {templateId}", "Unexpected error during template processing", 0, errorMsg);
                     context.AddError(errorMsg); // Add error to context
                     overallStepSuccess = false; // Mark the overall step as failed
                     break; // Stop processing immediately on error
                 }
            }

            // Log final status based on whether all templates were processed without error
            methodStopwatch.Stop(); // Stop stopwatch
            if (overallStepSuccess)
            {
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Successfully handled error state for all applicable templates", $"OverallSuccess: true", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(HandleErrorStateStep), $"Successfully handled error state for file: {filePath} for all applicable templates", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                 // This case is hit if any template processing resulted in an error.
                 // The specific failure reason is logged within the loop.
                 context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Handle error state during invoice processing", methodStopwatch.ElapsedMilliseconds, "Handling error state failed for at least one template.");
                 context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(HandleErrorStateStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Handling error state failed for at least one template.");
            }
            
            return overallStepSuccess;
        }

        // Corrected signature: Takes InvoiceProcessingContext and the specific Template template
        private async Task<bool> ProcessTemplate(ILogger logger, InvoiceProcessingContext context, Template template, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ProcessTemplate), "Processing", "Entering ProcessTemplate.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            // Assuming helper methods now correctly use 'template' where appropriate,
            // but might need 'context' for broader info (like Client, EmailId etc.)
            
            List<Line> failedLines = GetFailedLinesWithLogging(logger, template, filePath, templateId); // Pass logger and template
            AddExistingFailedLinesWithLogging(logger, template, failedLines, filePath, templateId); // Pass logger and template

            List<Line> allRequired = GetDistinctRequiredLinesWithLogging(logger, template, filePath, templateId); // Pass logger and template

            if (allRequired.Any() && failedLines.Count >= allRequired.Count)
            {
                string errorMsg = $"All {allRequired.Count} required lines appear to have failed for File: {filePath}, TemplateId: {templateId}.";
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessTemplate), "Validation", "All required lines appear to have failed.", $"FilePath: {filePath}, TemplateId: {templateId}, RequiredLineCount: {allRequired.Count}, FailedLineCount: {failedLines.Count}", "");
                context.AddError(errorMsg); // Add specific error
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessTemplate), "Completion", "Exiting ProcessTemplate with failure.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                return false; // Indicate failure for this template
            }

            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ProcessTemplate), "Processing", "Calling HandleErrorState.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            // Pass the full context to HandleErrorState as it likely needs it for email/reporting
            bool handleErrorResult = await HandleErrorState(logger, context, template, failedLines, filePath, templateId).ConfigureAwait(false); // Pass logger
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ProcessTemplate), "Completion", "Exiting ProcessTemplate.", $"FilePath: {filePath}, TemplateId: {templateId}, Result: {handleErrorResult}", "");
            return handleErrorResult;
        }

        // Corrected signature: Takes Template template
        private List<Line> GetFailedLinesWithLogging(ILogger logger, Template template, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetFailedLinesWithLogging), "Processing", "Getting failed lines.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            List<Line> failedLines = GetFailedLines(logger, template); // Pass template
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetFailedLinesWithLogging), "Completion", "Initial failed lines retrieved.", $"Count: {failedLines.Count}", new { FailedLines = failedLines });
            return failedLines;
        }

        // Corrected signature: Takes Template template
        private void AddExistingFailedLinesWithLogging(ILogger logger, Template template, List<Line> failedLines, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(AddExistingFailedLinesWithLogging), "Processing", "Adding existing failed lines from template parts.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            AddExistingFailedLines(logger, template, failedLines); // Pass template
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(AddExistingFailedLinesWithLogging), "Completion", "Total failed lines count after adding existing.", $"Count: {failedLines.Count}", new { FailedLines = failedLines });
        }

        // Corrected signature: Takes Template template
        private List<Line> GetDistinctRequiredLinesWithLogging(ILogger logger, Template template, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetDistinctRequiredLinesWithLogging), "Processing", "Getting distinct required lines.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            List<Line> allRequired = GetDistinctRequiredLines(logger, template); // Pass template
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetDistinctRequiredLinesWithLogging), "Completion", "Distinct required lines retrieved.", $"Count: {allRequired.Count}", new { RequiredLines = allRequired });
            return allRequired;
        }

        // Corrected signature: Takes InvoiceProcessingContext and Template template
        private async Task<bool> HandleErrorState(ILogger logger, InvoiceProcessingContext context, Template template, List<Line> failedLines, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HandleErrorState), "Processing", "Checking if current state is a valid error state for email notification.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            bool isValidErrorState = IsValidErrorState(logger, template, failedLines, filePath); // Pass logger, template and filePath
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HandleErrorState), "Processing", "IsValidErrorState check result.", $"Result: {isValidErrorState}", "");

            if (isValidErrorState)
            {
                logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorState), "EmailHandling", "Valid error state detected. Handling error email pipeline.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                // Pass context, template, and filePath to HandleErrorEmailPipeline
                logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"HandleErrorEmailPipeline for File: {filePath}, TemplateId: {templateId}", "ASYNC_EXPECTED"); // Log before email pipeline
                var emailPipelineStopwatch = Stopwatch.StartNew(); // Start stopwatch
                bool emailResult = await HandleErrorEmailPipeline(logger, context, template, filePath).ConfigureAwait(false); // Pass logger
                emailPipelineStopwatch.Stop(); // Stop stopwatch
                logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"HandleErrorEmailPipeline for File: {filePath}, TemplateId: {templateId}", emailPipelineStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after email pipeline

                logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorState), "EmailHandling", "HandleErrorEmailPipeline finished.", $"FilePath: {filePath}, Result: {emailResult}", "");
                // If email sending fails, should the step fail? Assuming yes for now.
                if (!emailResult)
                {
                     context.AddError($"HandleErrorEmailPipeline failed for File: {filePath}, TemplateId: {templateId}.");
                }
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HandleErrorState), "Completion", "Exiting HandleErrorState after email handling.", $"FilePath: {filePath}, TemplateId: {templateId}, Result: {emailResult}", "");
                return emailResult; // Return success/failure of email handling
            }

            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HandleErrorState), "UnsuccessfulImportHandling", "Not a valid error state for email notification. Handling as standard unsuccessful import.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
            // HandleUnsuccessfulImport only needs the file path
            logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                $"HandleUnsuccessfulImport for File: {filePath}", "SYNC_EXPECTED"); // Log before HandleUnsuccessfulImport
            var unsuccessfulImportStopwatch = Stopwatch.StartNew(); // Start stopwatch
            bool importHandled = HandleUnsuccessfulImport(logger, filePath); // Assuming sync or handle async if needed, pass logger
            unsuccessfulImportStopwatch.Stop(); // Stop stopwatch
            logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                $"HandleUnsuccessfulImport for File: {filePath}", unsuccessfulImportStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after HandleUnsuccessfulImport

             if (!importHandled)
             {
                  context.AddError($"HandleUnsuccessfulImport failed for File: {filePath}, TemplateId: {templateId}.");
             }
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HandleErrorState), "Completion", "Exiting HandleErrorState after unsuccessful import handling.", $"FilePath: {filePath}, TemplateId: {templateId}, Result: {importHandled}", "");
            return importHandled; // Return success/failure of handling
        }
    }
}