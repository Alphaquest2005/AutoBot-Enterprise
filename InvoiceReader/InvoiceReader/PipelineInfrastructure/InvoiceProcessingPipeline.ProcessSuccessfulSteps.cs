using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;
    using System.Linq;

    public partial class InvoiceProcessingPipeline
    {
        private async Task<bool> ProcessSuccessfulSteps()
        {
            string filePath = _context?.FilePath ?? "Unknown";
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            _logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ProcessSuccessfulSteps), "Process the success handling pipeline after initial steps succeed", $"FilePath: {filePath}");

            _logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(ProcessSuccessfulSteps), $"Processing success pipeline for file: {filePath}");

            try
            {
                // Success handling pipeline
                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessSuccessfulSteps), "Initialization", "Creating success pipeline steps.", $"FilePath: {filePath}", "");
                var successSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new AddNameSupplierStep(),
                    new AddMissingRequiredFieldValuesStep(),
                    new WriteFormattedTextFileStep(), // Assuming this step exists
                    new HandleImportSuccessStateStep(),
                    new UpdateImportStatusStep() // Update status after success handling
                };
                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessSuccessfulSteps), "Initialization", "Success pipeline steps created.", $"StepCount: {successSteps.Count}", new { Steps = successSteps.Select(step => step.GetType().Name) });

                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessSuccessfulSteps), "Execution", "Running success pipeline steps.", $"FilePath: {filePath}", "");
                bool allStepsSucceeded = true; // Track success across all steps in this sequence
                int stepCounter = 0;

                foreach (var step in successSteps)
                {
                    stepCounter++;
                    var stepName = step.GetType().Name;
                    var stepStopwatch = Stopwatch.StartNew(); // Start stopwatch for step execution
                    try
                    {
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessSuccessfulSteps), "StepExecution", "Executing success pipeline step.", $"StepNumber: {stepCounter}, StepName: {stepName}, FilePath: {filePath}", "");
                        _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            $"Success Step {stepCounter} ({stepName})", "ASYNC_EXPECTED"); // Log before step execution

                        // Correctly await the Execute method from IPipelineStep
                        bool stepResult = await step.Execute(_context).ConfigureAwait(false);
                        stepStopwatch.Stop(); // Stop stopwatch on success or reported failure
                        _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            $"Success Step {stepCounter} ({stepName})", stepStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after step execution returns control

                        if (!stepResult)
                        {
                            // Step explicitly returned false, indicating failure
                            string failureMessage = $"Success step {stepCounter} ({stepName}) reported failure (returned false) for File: {filePath}.";
                            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(ProcessSuccessfulSteps), "StepResult", "Success pipeline step reported failure (returned false).", $"StepNumber: {stepCounter}, StepName: {stepName}, FilePath: {filePath}", "");
                            _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                $"Success Step {stepCounter} ({stepName})", "Execute success pipeline step", stepStopwatch.ElapsedMilliseconds, "Step reported failure (returned false).");
                            _context.AddError(failureMessage); // Add error to context
                            allStepsSucceeded = false; // Mark failure
                            break; // Stop processing further success steps
                        }
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessSuccessfulSteps), "StepResult", "Success pipeline step completed successfully.", $"StepNumber: {stepCounter}, StepName: {stepName}, FilePath: {filePath}", "");
                        _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                            $"Success Step {stepCounter} ({stepName})", "Execute success pipeline step", "", stepStopwatch.ElapsedMilliseconds);
                    }
                    catch (Exception stepEx) // Catch exceptions during step execution
                    {
                        stepStopwatch.Stop(); // Stop stopwatch on exception
                        string errorMessage = $"Error during success step {stepCounter} ({stepName}) for File: {filePath}: {stepEx.Message}";
                        _logger.Error(stepEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            $"Success Step {stepCounter} ({stepName})", "Execute success pipeline step", stepStopwatch.ElapsedMilliseconds, $"Error executing step. Error: {stepEx.Message}");
                        _logger.Error(stepEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            $"{nameof(ProcessSuccessfulSteps)} - Step {stepCounter}", "Step execution", stepStopwatch.ElapsedMilliseconds, $"Error during success step {stepCounter} ({stepName}). Error: {stepEx.Message}");
                        _context.AddError(errorMessage); // Add error to context
                        allStepsSucceeded = false; // Mark failure
                        break; // Stop processing further success steps on error
                    }
                }
                _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessSuccessfulSteps), "Completion", "Success pipeline steps finished processing.", $"FilePath: {filePath}, AllStepsSucceeded: {allStepsSucceeded}", "");

                // Determine overall success based on step execution success AND final import status
                // ImportStatus might be HasErrors even if steps succeeded, which is still considered a form of overall success for the pipeline run.
                bool overallSuccess = allStepsSucceeded && (_context.ImportStatus == ImportStatus.Success ||
                                          _context.ImportStatus == ImportStatus.HasErrors); // Adjusted status check based on typical usage
                _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ProcessSuccessfulSteps), "Success pipeline processed", $"FinalImportStatus: {_context.ImportStatus}, AllSuccessStepsSucceeded: {allStepsSucceeded}", methodStopwatch.ElapsedMilliseconds);
                _logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ProcessSuccessfulSteps), $"Success pipeline processed for file: {filePath}. Final Status: {_context.ImportStatus}", methodStopwatch.ElapsedMilliseconds);
                return overallSuccess; // Return the calculated overall success
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop(); // Stop stopwatch on exception
                // Catch unexpected errors in the ProcessSuccessfulSteps method itself
                string errorMessage = $"Unexpected error during ProcessSuccessfulSteps for File: {filePath}: {ex.Message}";
                _logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ProcessSuccessfulSteps), "Process the success handling pipeline", methodStopwatch.ElapsedMilliseconds, $"Unexpected error. Error: {ex.Message}");
                _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ProcessSuccessfulSteps), "Unexpected error during execution", methodStopwatch.ElapsedMilliseconds, $"Unexpected error during ProcessSuccessfulSteps for File: {filePath}. Error: {ex.Message}");
                _context.AddError(errorMessage); // Add error to context
                return false; // Indicate failure
            }
        }
    }
}