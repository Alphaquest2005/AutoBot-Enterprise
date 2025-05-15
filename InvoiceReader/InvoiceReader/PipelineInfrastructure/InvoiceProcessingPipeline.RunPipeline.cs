using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class InvoiceProcessingPipeline
    {

        public async Task<bool> RunPipeline(ILogger log)
        {
            string filePath = _context?.FilePath ?? "Unknown";
            LogPipelineStart(filePath);
            LogContextDetails();

            // Guard Clause: Check for null context
            if (_context == null)
            {
                // Log the error, but cannot add to context as it's null.
                LogNullContextError();
                // Removed: _context.Error = errorMessage; // This would throw NullReferenceException
                return false; // Exit early
            }

            try
            {
                var initialSteps = InitializePipelineStepsWithLogging(filePath);
                ///////////////////////////////////////////////
                var initialRunSuccess = await RunInitialPipelineStepsWithLogging(filePath, initialSteps).ConfigureAwait(false);
                //////////////////////////////////////////////
                ///
               

                var isInitialRunUnsuccessful = CheckInitialRunResultWithLogging(filePath, initialRunSuccess);

                if (isInitialRunUnsuccessful)
                {
                    // Report unimported file, joining accumulated errors
                    return await EmailErrors(log).ConfigureAwait(false);
                }
                else
                {
                    if (_context.Templates.Any(x => x.CsvLines == null || !x.CsvLines.Any() || x.Success == false))
                    {
                        LogProcessingErrorPipeline(filePath);
                        var errorPipelineResult = await ProcessErrorPipeline().ConfigureAwait(false);
                        LogErrorPipelineCompleted(filePath, errorPipelineResult);
                        LogContextAfterErrorPipeline();
                        if (!errorPipelineResult && _context.Errors.Any()) //cuz the error pipeline could fail
                            await EmailErrors(log).ConfigureAwait(false); //return false;
                        // return errorPipelineResult;
                    }

                    LogProcessingSuccessPipeline(filePath);
                    var successPipelineResult = await ProcessSuccessfulSteps().ConfigureAwait(false);
                    LogSuccessPipelineCompleted(filePath, successPipelineResult);
                    LogContextAfterSuccessPipeline();
                    return successPipelineResult;
                }
                
            }
            catch (Exception ex)
            {
                string fatalErrorMessage = $"Fatal error during pipeline execution: {ex.Message}";
                LogFatalError(filePath, ex); // Log the fatal error with exception details
                _context.AddError(fatalErrorMessage); // Add the fatal error to the context's error list
                LogContextAfterFatalError(); // Log context state after fatal error
                return false; // Indicate pipeline failure
            }
            finally
            {
                LogPipelineEnd(filePath);
            }
        }

        private async Task<bool> EmailErrors(ILogger log)
        {
            string aggregatedErrors = string.Join("; ", _context.Errors);
           await InvoiceProcessingUtils.ReportUnimportedFile(_context.DocSet, _context.FilePath,
                _context.EmailId, _context.FileTypeId, _context.Client, _context.PdfText.ToString(), aggregatedErrors, _context.FailedLines, log).ConfigureAwait(false);
            return false;
          
        }

        private List<IPipelineStep<InvoiceProcessingContext>> InitializePipelineStepsWithLogging(string filePath)
        {
            LogInitializingPipelineSteps(filePath);
            var initialSteps = InitializePipelineSteps(); // InitializePipelineSteps now uses _logger internally
            LogPipelineStepsCreated(initialSteps);
            return initialSteps;
        }

        private async Task<bool> RunInitialPipelineStepsWithLogging(string filePath, List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            LogRunningInitialSteps(filePath);
            var success = await RunInitialPipelineSteps(initialSteps).ConfigureAwait(false);
            LogInitialStepsCompleted(filePath, success);
            LogContextAfterInitialSteps();
            return success;
        }

        private bool CheckInitialRunResultWithLogging(string filePath, bool initialRunSuccess)
        {
            LogCheckingInitialRunResult(filePath);
            var isUnsuccessful = IsInitialRunUnsuccessful(initialRunSuccess);
            LogInitialRunResult(filePath, isUnsuccessful);
            return isUnsuccessful;
        }

        private void LogPipelineStart(string filePath) =>
            _logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(RunPipeline), "Execute the invoice processing pipeline", $"FilePath: {filePath}");

        private void LogContextDetails() =>
            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ContextSnapshot", "Current pipeline context details.", "", new { Context = _context });

        private void LogNullContextError() =>
            _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(RunPipeline), "Execute the invoice processing pipeline", 0, "InvoiceProcessingPipeline cannot run with a null context.");

        private void LogInitializingPipelineSteps(string filePath) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "Initialization", "Initializing initial pipeline steps.", $"FilePath: {filePath}", "");

        private void LogPipelineStepsCreated(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "Initialization", "Initial pipeline steps created.", $"StepCount: {initialSteps.Count}", new { Steps = initialSteps.Select(step => step.GetType().Name) });
        }

        private void LogRunningInitialSteps(string filePath) =>
            _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "RunInitialPipelineSteps", "ASYNC_EXPECTED");

        private void LogInitialStepsCompleted(string filePath, bool success) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "InitialStepsCompletion", "Initial pipeline steps completed.", $"FilePath: {filePath}, Success: {success}", "");

        private void LogContextAfterInitialSteps() =>
            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ContextSnapshot", "Context after initial steps.", "", new { Context = _context });

        private void LogCheckingInitialRunResult(string filePath) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ResultCheck", "Checking if initial run was unsuccessful.", $"FilePath: {filePath}", "");

        private void LogInitialRunResult(string filePath, bool result) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ResultCheck", "Initial run unsuccessful check complete.", $"FilePath: {filePath}, IsUnsuccessful: {result}", "");

        private void LogProcessingErrorPipeline(string filePath) =>
            _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ErrorHandling", "Initial run deemed unsuccessful. Processing error pipeline.", $"FilePath: {filePath}", "");

        private void LogErrorPipelineCompleted(string filePath, bool result) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ErrorHandling", "Error pipeline processing finished.", $"FilePath: {filePath}, Result (Continue?): {result}", "");

        private void LogContextAfterErrorPipeline() =>
            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ContextSnapshot", "Context after error pipeline.", "", new { Context = _context });

        private void LogProcessingSuccessPipeline(string filePath) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "SuccessProcessing", "Initial run deemed successful. Processing success steps.", $"FilePath: {filePath}", "");

        private void LogSuccessPipelineCompleted(string filePath, bool result) =>
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "SuccessProcessing", "Success pipeline processing finished.", $"FilePath: {filePath}, Result (Overall Success?): {result}", "");

        private void LogContextAfterSuccessPipeline() =>
            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ContextSnapshot", "Context after success pipeline.", "", new { Context = _context });

        private void LogFatalError(string filePath, Exception ex) =>
           _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                nameof(RunPipeline), "Fatal error during execution", 0, $"Fatal error during pipeline execution for File: {filePath}. Error: {ex.Message}");
            

        private void LogContextAfterFatalError() =>
            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(RunPipeline), "ContextSnapshot", "Context after fatal error.", "", new { Context = _context });

        private void LogPipelineEnd(string filePath) =>
            _logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                nameof(RunPipeline), $"Pipeline execution completed for File: {filePath}. Final Status: {_context?.ImportStatus}", 0); // Duration will be added in RunPipeline
    }
}