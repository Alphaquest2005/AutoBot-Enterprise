using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class PipelineRunner<TContext>
    {
        private readonly ILogger _logger; // Use instance logger

        private readonly IReadOnlyList<IPipelineStep<TContext>> _steps;
        private readonly string _pipelineName;

        public PipelineRunner(IEnumerable<IPipelineStep<TContext>> steps, ILogger logger, string pipelineName = "Unnamed Pipeline")
        {
            _steps = steps?.ToList() ?? new List<IPipelineStep<TContext>>();
            _pipelineName = pipelineName;
            _logger = logger; // Assign the passed logger
            LogPipelineInitialization();
        }

        // Updated Run method to be async
        public async Task<bool> Run(TContext context)
        {
            if (!ValidateContext(context))
                return false;

            // Cast context once for error reporting (assuming InvoiceProcessingContext)
            var invoiceContext = context as InvoiceProcessingContext;
            if (invoiceContext == null && typeof(TContext) == typeof(InvoiceProcessingContext))
            {
                 // Log an error if the context is expected to be InvoiceProcessingContext but isn't.
                 // This shouldn't happen if used correctly but is a safeguard.
                 _logger.Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Run), "ContextValidation", "Pipeline received unexpected context type for error reporting.", $"Expected: {typeof(InvoiceProcessingContext).Name}, Received: {context?.GetType().Name ?? "null"}", new { PipelineName = _pipelineName });
                 // We might not be able to add to context.Errors here, depending on the actual type.
                 // Consider throwing an exception or returning false based on desired strictness.
                 return false; // Stop if context type is wrong for error handling
            }


            _logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                $"{_pipelineName}Execution", $"Starting execution of pipeline: {_pipelineName}");

            int stepCounter = 0;
            bool overallSuccess = true; // Tracks if all steps succeeded

            foreach (var step in _steps)
            {
                stepCounter++;
                string stepName = GetStepName(step, stepCounter);
                LogStepProcessing(stepCounter, stepName); // Moved logging here

                // **CRITICAL_DEBUG**: Special logging for GetPossibleInvoicesStep
                if (stepName.Contains("GetPossibleInvoicesStep"))
                {
                    _logger.Error("🎯 **CRITICAL_DEBUG_GETPOSSIBLEINVOICESSTEP**: About to execute GetPossibleInvoicesStep");
                    _logger.Error("   - **STEP_COUNTER**: {StepCounter}", stepCounter);
                    _logger.Error("   - **STEP_NAME**: {StepName}", stepName);
                    _logger.Error("   - **STEP_TYPE**: {StepType}", step?.GetType()?.FullName ?? "NULL");
                    _logger.Error("   - **CONTEXT_TYPE**: {ContextType}", context?.GetType()?.FullName ?? "NULL");
                }

                if (step == null)
                {
                    LogNullStepWarning(stepCounter);
                    continue; // Skip null step
                }

                // Execute step asynchronously and check result
                bool stepSuccess = await ExecuteStepAsync(step, context, invoiceContext, stepName, stepCounter).ConfigureAwait(false);

                // **CRITICAL_DEBUG**: Special logging for GetPossibleInvoicesStep result
                if (stepName.Contains("GetPossibleInvoicesStep"))
                {
                    _logger.Error("🔍 **CRITICAL_DEBUG_GETPOSSIBLEINVOICESSTEP_RESULT**: GetPossibleInvoicesStep execution completed");
                    _logger.Error("   - **STEP_SUCCESS**: {StepSuccess}", stepSuccess);
                    _logger.Error("   - **STEP_COUNTER**: {StepCounter}", stepCounter);
                    
                    var invoiceCtx = context as InvoiceProcessingContext;
                    if (invoiceCtx != null)
                    {
                        _logger.Error("   - **MATCHED_TEMPLATES_COUNT**: {Count}", invoiceCtx.MatchedTemplates?.Count() ?? 0);
                        _logger.Error("   - **MATCHED_TEMPLATES**: {Templates}", 
                            string.Join(", ", invoiceCtx.MatchedTemplates?.Select(t => $"{t.OcrTemplates?.Name}({t.FileType?.FileImporterInfos?.EntryType})") ?? new string[0]));
                    }
                }

                if (!stepSuccess)
                {
                    overallSuccess = false;
                    LogPipelinePrematureStop(stepCounter); // Log that pipeline stopped due to this step
                    break; // Stop processing further steps
                }
            }

            LogPipelineCompletion(overallSuccess, stepCounter);
            return overallSuccess;
        }

        // Removed ProcessStep as its logic is merged into the loop in Run and ExecuteStepAsync

        private void LogPipelineInitialization()
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(PipelineRunner<TContext>), "Initialization", "Pipeline runner initialized.", $"PipelineName: {_pipelineName}, StepCount: {_steps.Count}", "");
            if (_steps.Count == 0)
            {
                LogZeroStepsWarning();
            }
        }

        private void LogZeroStepsWarning()
        {
            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(PipelineRunner<TContext>), "Initialization", "Pipeline runner initialized with zero steps.", $"PipelineName: {_pipelineName}", "Expected steps for execution.");
        }

        private bool ValidateContext(TContext context)
        {
            if (context == null)
            {
                LogNullContextError();
                return false;
            }
            return true;
        }

        private void LogNullContextError()
        {
            _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(Run), "Execute pipeline steps", 0, $"{_pipelineName} cannot run with a null context.");
            _logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{_pipelineName}Execution", "Context validation", 0, $"{_pipelineName} cannot run with a null context.");
        }



        private void LogNullStepWarning(int stepCounter)
        {
            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Run), "StepExecution", "Skipping null step.", $"PipelineName: {_pipelineName}, StepNumber: {stepCounter}", "Null step encountered in pipeline definition.");
        }

        // Renamed and made async, includes error handling and context update
        private async Task<bool> ExecuteStepAsync(IPipelineStep<TContext> step, TContext context, InvoiceProcessingContext invoiceContext, string stepName, int stepCounter)
        {
            var stepStopwatch = Stopwatch.StartNew(); // Start stopwatch for step execution
            try
            {
                LogStepExecutionStart(stepName, stepCounter);
                _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"Step {stepCounter} ({stepName})", "ASYNC_EXPECTED"); // Log before step execution
                bool stepResult = await step.Execute(context).ConfigureAwait(false); // Use await
                stepStopwatch.Stop(); // Stop stopwatch on success or reported failure
                _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"Step {stepCounter} ({stepName})", stepStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after step execution returns control

                if (!stepResult)
                {
                    // Step indicated failure without throwing an exception
                    string failureMessage = $"Step {stepCounter} ({stepName}) in {_pipelineName} reported failure (returned false).";
                    LogStepFailure(stepName, stepCounter, stepStopwatch.ElapsedMilliseconds); // Log the failure with duration
                    invoiceContext?.AddError(failureMessage); // Add error to context if possible
                    return false; // Indicate failure, stop pipeline
                }

                LogStepSuccess(stepName, stepCounter, stepStopwatch.ElapsedMilliseconds);
                return true; // Indicate success, continue pipeline
            }
            catch (Exception ex)
            {
                stepStopwatch.Stop(); // Stop stopwatch on exception
                // Exception occurred during step execution
                string errorMessage = $"Error executing Step {stepCounter} ({stepName}) in {_pipelineName}: {ex.Message}";
                LogStepExecutionError(ex, stepName, stepCounter, stepStopwatch.ElapsedMilliseconds); // Log the error with exception details and duration
                invoiceContext?.AddError(errorMessage); // Add error to context if possible
                return false; // Indicate failure, stop pipeline
            }
        }

        private void LogStepExecutionStart(string stepName, int stepCounter)
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Run), "StepExecution", "Executing pipeline step.", $"PipelineName: {_pipelineName}, StepNumber: {stepCounter}, StepName: {stepName}", "");
        }

        private void LogStepExecutionError(Exception ex, string stepName, int stepCounter, long durationMs)
        {
            _logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                $"Step {stepCounter} ({stepName})", "Execute pipeline step", durationMs, $"Error executing step. Error: {ex.Message}");
            _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{_pipelineName}Execution - Step {stepCounter}", "Step execution", durationMs, $"Error executing Step {stepCounter} ({stepName}). Stopping pipeline execution. Error: {ex.Message}");
        }

        // GetStepName remains largely the same, just logging call moved inside Run loop
        private string GetStepName(IPipelineStep<TContext> step, int stepCounter)
        {
             return step?.GetType().Name ?? $"Unnamed Step {stepCounter}";
             // LogStepProcessing call moved to the loop in Run for clarity
        }

        private void LogStepProcessing(int stepCounter, string stepName)
        {
            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Run), "StepProcessing", "Processing pipeline step.", $"PipelineName: {_pipelineName}, StepNumber: {stepCounter}, StepName: {stepName}", "");
        }

        // Removed HandleStepResult as its logic is now integrated into ExecuteStepAsync

        private void LogStepFailure(string stepName, int stepCounter, long durationMs)
        {
            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Run), "StepResult", "Pipeline step reported failure (returned false).", $"PipelineName: {_pipelineName}, StepNumber: {stepCounter}, StepName: {stepName}", "");
            _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                $"Step {stepCounter} ({stepName})", "Execute pipeline step", durationMs, "Step reported failure (returned false).");
            _logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{_pipelineName}Execution - Step {stepCounter}", "Step result check", durationMs, $"Step {stepCounter} ({stepName}) reported failure. Stopping pipeline execution.");
        }

        private void LogStepSuccess(string stepName, int stepCounter, long durationMs)
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Run), "StepResult", "Pipeline step completed successfully.", $"PipelineName: {_pipelineName}, StepNumber: {stepCounter}, StepName: {stepName}", "");
            _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                $"Step {stepCounter} ({stepName})", "Execute pipeline step", "", durationMs);
        }

        // Updated LogPipelineCompletion to use overallSuccess flag
        private void LogPipelineCompletion(bool overallSuccess, int stepCounter)
        {
            if (overallSuccess)
            {
                LogPipelineSuccess(stepCounter);
            }
            else
            {
                // LogPipelinePrematureStop is called within the loop when a step fails
                // Add a final log indicating pipeline finished with errors
                LogPipelineFinishedWithErrors(stepCounter);
            }
             _logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                 $"{_pipelineName}Execution", $"Pipeline finished execution. Overall Success: {overallSuccess}", 0); // Duration will be added in the calling method
        }

        private void LogPipelineSuccess(int stepCount)
        {
            _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                nameof(Run), "Execute pipeline steps", $"OverallSuccess: true, StepsCompleted: {stepCount}", 0); // Duration will be added in the calling method
        }

        private void LogPipelinePrematureStop(int stepCounter)
        {
            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Run), "PipelineControlFlow", "Pipeline execution stopped prematurely.", $"PipelineName: {_pipelineName}, StoppedAfterStep: {stepCounter}", "");
        }

        private void LogPipelineFinishedWithErrors(int stepCounter)
        {
             _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                 nameof(Run), "Execute pipeline steps", 0, $"Pipeline finished with errors after step {stepCounter}."); // Duration will be added in the calling method
             _logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                 $"{_pipelineName}Execution", "Pipeline execution with errors", 0, $"Pipeline finished with errors after step {stepCounter}."); // Duration will be added in the calling method
        }
    }
}
