using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{


    public partial class InvoiceProcessingPipeline
    {
        private async Task<bool> RunInitialPipelineSteps(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            string filePath = _context?.FilePath ?? "Unknown";
            LogRunInitialPipelineSteps(filePath);

            try
            {
                var initialRunner = CreatePipelineRunner(initialSteps);

                return await RunPipeline(initialRunner).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during initial pipeline steps execution: {ex.Message}";
                LogPipelineError(ex, filePath); // Log the error with exception details
                _context.AddError(errorMessage); // Add the error to the context's error list
                return false; // Indicate failure
            }
        }

        private void LogRunInitialPipelineSteps(string filePath)
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunInitialPipelineSteps), "Execution", "Starting execution of initial pipeline steps.", $"FilePath: {filePath}", "");
        }

        private PipelineRunner<InvoiceProcessingContext> CreatePipelineRunner(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            var initialRunner = new PipelineRunner<InvoiceProcessingContext>(initialSteps, _logger, "Initial Pipeline"); // Pass the instance logger
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(CreatePipelineRunner), "Creation", "Initial PipelineRunner created.", "", "");
            return initialRunner;
        }

        private async Task<bool> RunPipeline(PipelineRunner<InvoiceProcessingContext> runner)
        {
            
            var res = await runner.Run(_context).ConfigureAwait(false);
            LogPipelineCompletion(res);
            return res;
        }

        private void LogPipelineCompletion(bool success)
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "Completion", "PipelineRunner finished.", $"OverallSuccess: {success}", "");
        }

        private void LogPipelineError(Exception ex, string filePath)
        {
            _logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(RunInitialPipelineSteps), "Run initial pipeline steps", 0, $"Error during initial pipeline steps execution for File: {filePath}. Error: {ex.Message}");
            _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                nameof(RunInitialPipelineSteps), "Initial steps execution", 0, $"Error during initial pipeline steps execution for File: {filePath}. Error: {ex.Message}");
        }
    }
}