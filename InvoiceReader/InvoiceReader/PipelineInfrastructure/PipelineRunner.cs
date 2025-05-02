using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class PipelineRunner<TContext>
    {
        private static readonly ILogger _logger = Log.ForContext<PipelineRunner<TContext>>();

        private readonly IReadOnlyList<IPipelineStep<TContext>> _steps;
        private readonly string _pipelineName;

        public PipelineRunner(IEnumerable<IPipelineStep<TContext>> steps, string pipelineName = "Unnamed Pipeline")
        {
            _steps = steps?.ToList() ?? new List<IPipelineStep<TContext>>();
            _pipelineName = pipelineName;
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
                 _logger.Error("{PipelineName} received context of type {ContextType} but expected InvoiceProcessingContext for error reporting.", _pipelineName, context?.GetType().Name ?? "null");
                 // We might not be able to add to context.Errors here, depending on the actual type.
                 // Consider throwing an exception or returning false based on desired strictness.
                 return false; // Stop if context type is wrong for error handling
            }


            _logger.Information("Starting execution of {PipelineName}.", _pipelineName);

            int stepCounter = 0;
            bool overallSuccess = true; // Tracks if all steps succeeded

            foreach (var step in _steps)
            {
                stepCounter++;
                string stepName = GetStepName(step, stepCounter);
                 LogStepProcessing(stepCounter, stepName); // Moved logging here

                if (step == null)
                {
                    LogNullStepWarning(stepCounter);
                    continue; // Skip null step
                }

                // Execute step asynchronously and check result
                bool stepSuccess = await ExecuteStepAsync(step, context, invoiceContext, stepName, stepCounter);

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
            _logger.Debug("{PipelineName} runner initialized with {StepCount} steps.", _pipelineName, _steps.Count);
            if (_steps.Count == 0)
            {
                LogZeroStepsWarning();
            }
        }

        private void LogZeroStepsWarning()
        {
            _logger.Warning("{PipelineName} runner initialized with zero steps.", _pipelineName);
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
            _logger.Error("{PipelineName} cannot run with a null context.", _pipelineName);
        }



        private void LogNullStepWarning(int stepCounter)
        {
            _logger.Warning("Skipping null step at position {StepNumber} in {PipelineName}.", stepCounter, _pipelineName);
        }

        // Renamed and made async, includes error handling and context update
        private async Task<bool> ExecuteStepAsync(IPipelineStep<TContext> step, TContext context, InvoiceProcessingContext invoiceContext, string stepName, int stepCounter)
        {
            try
            {
                LogStepExecutionStart(stepName);
                bool stepResult = await step.Execute(context).ConfigureAwait(false); // Use await

                if (!stepResult)
                {
                    // Step indicated failure without throwing an exception
                    string failureMessage = $"Step {stepCounter} ({stepName}) in {_pipelineName} reported failure (returned false).";
                    LogStepFailure(stepName, stepCounter); // Log the failure
                    invoiceContext?.AddError(failureMessage); // Add error to context if possible
                    return false; // Indicate failure, stop pipeline
                }

                LogStepSuccess(stepName, stepCounter);
                return true; // Indicate success, continue pipeline
            }
            catch (Exception ex)
            {
                // Exception occurred during step execution
                string errorMessage = $"Error executing Step {stepCounter} ({stepName}) in {_pipelineName}: {ex.Message}";
                LogStepExecutionError(ex, stepName, stepCounter); // Log the error with exception details
                invoiceContext?.AddError(errorMessage); // Add error to context if possible
                return false; // Indicate failure, stop pipeline
            }
        }

        private void LogStepExecutionStart(string stepName)
        {
            _logger.Verbose("Executing step {StepName}...", stepName);
        }

        private void LogStepExecutionError(Exception ex, string stepName, int stepCounter)
        {
            _logger.Error(ex, "Error executing Step {StepNumber} ({StepName}) in {PipelineName}. Stopping pipeline execution.",
                stepCounter, stepName, _pipelineName);
        }

        // GetStepName remains largely the same, just logging call moved inside Run loop
        private string GetStepName(IPipelineStep<TContext> step, int stepCounter)
        {
             return step?.GetType().Name ?? $"Unnamed Step {stepCounter}";
             // LogStepProcessing call moved to the loop in Run for clarity
        }

        private void LogStepProcessing(int stepCounter, string stepName)
        {
            _logger.Verbose("Processing step {StepNumber}: {StepName} in {PipelineName}.", stepCounter, stepName, _pipelineName);
        }

        // Removed HandleStepResult as its logic is now integrated into ExecuteStepAsync

        private void LogStepFailure(string stepName, int stepCounter)
        {
            _logger.Warning("Step {StepNumber} ({StepName}) in {PipelineName} returned false. Stopping pipeline execution.",
                stepCounter, stepName, _pipelineName);
        }

        private void LogStepSuccess(string stepName, int stepCounter)
        {
            _logger.Information("Step {StepNumber} ({StepName}) in {PipelineName} completed successfully.",
                stepCounter, stepName, _pipelineName);
        }

        // Updated LogPipelineCompletion to use overallSuccess flag
        private void LogPipelineCompletion(bool overallSuccess, int stepCounter)
        {
            if (overallSuccess)
            {
                LogPipelineSuccess();
            }
            // else case is handled by LogPipelinePrematureStop called within the loop
            // We could add a final "Pipeline finished with errors" log here if needed.
             _logger.Information("{PipelineName} finished execution. Overall Success: {OverallSuccess}", _pipelineName, overallSuccess);
        }

        private void LogPipelineSuccess()
        {
            _logger.Information("{PipelineName} completed all {StepCount} steps successfully.", _pipelineName, _steps.Count);
        }

        private void LogPipelinePrematureStop(int stepCounter)
        {
            _logger.Warning("{PipelineName} execution stopped prematurely after Step {StepNumber}.", _pipelineName, stepCounter);
        }
    }
}
