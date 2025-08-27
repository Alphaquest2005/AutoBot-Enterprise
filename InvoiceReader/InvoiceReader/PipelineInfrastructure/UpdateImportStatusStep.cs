using System.Threading.Tasks;
// Assuming FileTypeManager is here

// Added for ImportStatus enum
// Added
using Serilog; // Added
using System; // Added
// Added for Dictionary access
using System.Linq; // Added for Any()

// Added for FileTypes

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class UpdateImportStatusStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<UpdateImportStatusStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Update import status based on pipeline outcome", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(UpdateImportStatusStep), $"Updating import status for file: {filePath}");

             // Basic context validation
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "UpdateImportStatusStep executed with null context.");
            }
             if (!context.MatchedTemplates.Any())
            {
                 context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "Validation", "Skipping UpdateImportStatusStep: No Templates found in context.", $"FilePath: {filePath}", "Expected templates for status update.");
                 methodStopwatch.Stop(); // Stop stopwatch on skip
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Skipped due to no templates", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(UpdateImportStatusStep), $"Skipped updating import status for file: {filePath} (no templates)", methodStopwatch.ElapsedMilliseconds);
                 return Task.FromResult(true); // No templates to process, not a failure of the step itself.
            }

            bool overallStepSuccess = true; // Track success across all templates

            foreach (var template in context.MatchedTemplates)
            {
                 int? templateId = template?.OcrTemplates?.Id; // Safe access
                 string templateName = template?.OcrTemplates?.Name ?? "Unknown";
                 context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "TemplateProcessing", "Processing template for status update.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");

                 try
                 {
                     // --- Data Presence Check ---
                     if (!IsImportDataPresent(context.Logger, context)) // Handles its own logging, pass logger
                     {
                         string errorMsg = $"UpdateImportStatusStep cannot proceed due to missing required data for File: {filePath}, TemplateId: {templateId}";
                         // Logging is handled by IsImportDataPresent
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false; // Mark step as failed
                         continue; // Continue to the next template
                     }
                     // --- End Data Presence Check ---

                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "StatusUpdate", "Required data is present. Processing import status update.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     
                     // --- Process and Log Status ---
                     // Assuming ProcessImportFile and LogImportStatusUpdate might throw exceptions
                     context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                         $"ProcessImportFile for File: {filePath}", "SYNC_EXPECTED"); // Log before ProcessImportFile
                     var processFileStopwatch = Stopwatch.StartNew(); // Start stopwatch
                     ImportStatus finalStatus = ProcessImportFile(context.Logger, context); // Handles its own logging, pass logger
                     processFileStopwatch.Stop(); // Stop stopwatch
                     context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                         $"ProcessImportFile for File: {filePath}", processFileStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after ProcessImportFile

                     context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                         $"LogImportStatusUpdate for File: {filePath}, Status: {finalStatus}", "SYNC_EXPECTED"); // Log before LogImportStatusUpdate
                     var logStatusStopwatch = Stopwatch.StartNew(); // Start stopwatch
                     bool logSuccess = LogImportStatusUpdate(context.Logger, finalStatus, filePath, templateId); // Handles its own logging, pass logger
                     logStatusStopwatch.Stop(); // Stop stopwatch
                     context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                         $"LogImportStatusUpdate for File: {filePath}, Status: {finalStatus}", logStatusStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after LogImportStatusUpdate
                     
                     if (!logSuccess) // If LogImportStatusUpdate indicates an issue (e.g., couldn't save status)
                     {
                          string errorMsg = $"LogImportStatusUpdate reported failure for File: {filePath}, TemplateId: {templateId}, Status: {finalStatus}";
                          context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                              nameof(Execute), "StatusUpdateLogging", "LogImportStatusUpdate reported failure.", $"FilePath: {filePath}, TemplateId: {templateId}, Status: {finalStatus}", "");
                           context.AddError(errorMsg); // Add error to context
                           overallStepSuccess = false; // Mark step as failed
                           continue; // Continue to the next template
                     }
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "StatusUpdateLogging", "Import status update logged successfully.", $"FilePath: {filePath}, TemplateId: {templateId}, Status: {finalStatus}", "");
                     // --- End Process and Log Status ---

                     // If we reach here, status update for this template was successful.
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateCompletion", "Finished processing template for status update successfully.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Error during UpdateImportStatusStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Execute), "Update import status based on pipeline outcome", 0, errorMsg);
                     context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         $"{nameof(UpdateImportStatusStep)} - Template {templateId}", "Unexpected error during template processing", 0, errorMsg);
                     context.AddError(errorMsg); // Add error to context
                     overallStepSuccess = false; // Mark the overall step as failed
                     continue; // Continue to next template
                 }
            }

            // Log final status based on whether all templates were processed without error
            methodStopwatch.Stop(); // Stop stopwatch
            if (overallStepSuccess)
            {
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Successfully updated import status for all applicable templates", $"OverallSuccess: true", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(UpdateImportStatusStep), $"Successfully updated import status for file: {filePath} for all applicable templates", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                 // This case is hit if any template processing resulted in an error.
                 // The specific failure reason is logged within the loop.
                 context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Update import status based on pipeline outcome", methodStopwatch.ElapsedMilliseconds, "Updating import status failed for at least one template.");
                 context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(UpdateImportStatusStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Updating import status failed for at least one template.");
            }
            
            return Task.FromResult(overallStepSuccess);
        }

        // Renamed and corrected logic: Returns true if data needed for this step is PRESENT

        // Added context parameters for logging
    }
}