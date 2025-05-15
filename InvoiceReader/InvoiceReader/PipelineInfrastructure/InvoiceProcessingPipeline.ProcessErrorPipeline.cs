using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;
    using System.Linq;

    public partial class InvoiceProcessingPipeline
    {
        private async Task<bool> ProcessErrorPipeline()
        {
            string filePath = _context?.FilePath ?? "Unknown";
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            _logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ProcessErrorPipeline), "Process the error handling pipeline after initial steps fail", $"FilePath: {filePath}");

            _logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(ProcessErrorPipeline), $"Processing error pipeline for file: {filePath}");

            try
            {
                // Error handling pipeline
                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessErrorPipeline), "Initialization", "Creating error pipeline steps.", $"FilePath: {filePath}", "");
                var errorSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new HandleErrorStateStep(_isLastTemplate), // Handles email logic
                    new UpdateImportStatusStep() // Update status after error handling
                };
                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessErrorPipeline), "Initialization", "Error pipeline steps created.", $"StepCount: {errorSteps.Count}", new { Steps = errorSteps.Select(step => step.GetType().Name) });

                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessErrorPipeline), "Execution", "Running error pipeline steps.", $"FilePath: {filePath}", "");
                bool allStepsAttemptedSuccessfully = true; // Track if steps executed without *new* issues
                int stepCounter = 0;

                foreach (var step in errorSteps)
                {
                    stepCounter++;
                    var stepName = step.GetType().Name;
                    var stepStopwatch = Stopwatch.StartNew(); // Start stopwatch for step execution
                    try
                    {
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessErrorPipeline), "StepExecution", "Executing error pipeline step.", $"StepNumber: {stepCounter}, StepName: {stepName}, FilePath: {filePath}", "");
                        _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            $"Error Step {stepCounter} ({stepName})", "ASYNC_EXPECTED"); // Log before step execution

                        // Correctly await the Execute method from IPipelineStep
                        bool stepResult = await step.Execute(_context).ConfigureAwait(false);
                        stepStopwatch.Stop(); // Stop stopwatch on success or reported failure
                        _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            $"Error Step {stepCounter} ({stepName})", stepStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after step execution returns control

                        if (!stepResult)
                        {
                            // Step explicitly returned false, indicating failure within the error handling itself
                            string failureMessage = $"Error step {stepCounter} ({stepName}) reported failure (returned false) for File: {filePath}.";
                            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(ProcessErrorPipeline), "StepResult", "Error pipeline step reported failure (returned false).", $"StepNumber: {stepCounter}, StepName: {stepName}, FilePath: {filePath}", "");
                            _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                $"Error Step {stepCounter} ({stepName})", "Execute error pipeline step", stepStopwatch.ElapsedMilliseconds, "Step reported failure (returned false).");
                            _context.AddError(failureMessage); // Add this *new* error to context
                            allStepsAttemptedSuccessfully = false; // Mark that an error step itself failed
                            // Do NOT break - allow subsequent error steps to run if possible
                        }
                        else
                        {
                             _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                 nameof(ProcessErrorPipeline), "StepResult", "Error pipeline step completed successfully.", $"StepNumber: {stepCounter}, StepName: {stepName}, FilePath: {filePath}", "");
                             _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                                 $"Error Step {stepCounter} ({stepName})", "Execute error pipeline step", "", stepStopwatch.ElapsedMilliseconds);
                        }
                    }
                    catch (Exception stepEx) // Catch exceptions during error step execution
                    {
                        stepStopwatch.Stop(); // Stop stopwatch on exception
                        string errorMessage = $"Error during error step {stepCounter} ({stepName}) for File: {filePath}: {stepEx.Message}";
                        _logger.Error(stepEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            $"Error Step {stepCounter} ({stepName})", "Execute error pipeline step", stepStopwatch.ElapsedMilliseconds, $"Error executing step. Error: {stepEx.Message}");
                        _logger.Error(stepEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            $"{nameof(ProcessErrorPipeline)} - Step {stepCounter}", "Step execution", stepStopwatch.ElapsedMilliseconds, $"Error during error step {stepCounter} ({stepName}). Error: {stepEx.Message}");
                        _context.AddError(errorMessage); // Add this *new* error to context
                        allStepsAttemptedSuccessfully = false; // Mark that an error step itself failed
                        // Do NOT break - allow subsequent error steps to run if possible
                    }
                }
                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessErrorPipeline), "Completion", "Error pipeline steps finished processing.", $"FilePath: {filePath}, AllStepsAttemptedSuccessfully: {allStepsAttemptedSuccessfully}", "");

                // The return value indicates if the *overall* pipeline run should be considered successful enough
                // to potentially proceed with other actions outside the pipeline, despite the initial error.
                // This logic might depend on the final ImportStatus set by the error steps.
                // For now, we assume that if the error pipeline ran (even with internal failures), the overall result is still 'failed'.
                // The context.Errors list contains the details.
                bool overallPipelineSuccess = false; // Error path always results in overall failure
                 _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(ProcessErrorPipeline), "Error pipeline processed", $"FinalImportStatus: {_context.ImportStatus}, AllErrorStepsAttemptedSuccessfully: {allStepsAttemptedSuccessfully}", methodStopwatch.ElapsedMilliseconds);
                 _logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(ProcessErrorPipeline), $"Error pipeline processed for file: {filePath}. Final Status: {_context.ImportStatus}", methodStopwatch.ElapsedMilliseconds);
                return overallPipelineSuccess; // Return false as the initial steps failed
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop(); // Stop stopwatch on exception
                // Catch unexpected errors in the ProcessErrorPipeline method itself
                string errorMessage = $"Unexpected error during ProcessErrorPipeline for File: {filePath}: {ex.Message}";
                _logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ProcessErrorPipeline), "Process the error handling pipeline", methodStopwatch.ElapsedMilliseconds, $"Unexpected error. Error: {ex.Message}");
                _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ProcessErrorPipeline), "Unexpected error during execution", methodStopwatch.ElapsedMilliseconds, $"Unexpected error during ProcessErrorPipeline for File: {filePath}. Error: {ex.Message}");
                _context.AddError(errorMessage); // Add this *new* error to context
                return false; // Indicate overall pipeline failure
            }
        }
    }
}