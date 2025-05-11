using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class InvoiceProcessingPipeline
    {

        public async Task<bool> RunPipeline()
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
                    return await EmailErrors().ConfigureAwait(false);
                    return false;

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
                            await EmailErrors().ConfigureAwait(false); //return false;
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

        private async Task<bool> EmailErrors()
        {
            string aggregatedErrors = string.Join("; ", _context.Errors);
           await InvoiceProcessingUtils.ReportUnimportedFile(_context.DocSet, _context.FilePath,
                _context.EmailId, _context.FileTypeId, _context.Client, _context.PdfText.ToString(), aggregatedErrors, _context.FailedLines).ConfigureAwait(false);
            return false;
          
        }

        private List<IPipelineStep<InvoiceProcessingContext>> InitializePipelineStepsWithLogging(string filePath)
        {
            LogInitializingPipelineSteps(filePath);
            var initialSteps = InitializePipelineSteps();
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
            _logger.Information("Starting main InvoiceProcessingPipeline execution for File: {FilePath}", filePath);

        private void LogContextDetails() =>
            _logger.Verbose("Context details: {@Context}", _context);

        private void LogNullContextError() =>
            _logger.Error("InvoiceProcessingPipeline cannot run with a null context.");

        private void LogInitializingPipelineSteps(string filePath) =>
            _logger.Debug("Initializing initial pipeline steps for File: {FilePath}", filePath);

        private void LogPipelineStepsCreated(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            _logger.Debug("Initial pipeline steps created. Count: {Count}", initialSteps.Count);
            _logger.Verbose("Initial steps: {@InitialSteps}", initialSteps.Select(step => step.GetType().Name));
        }

        private void LogRunningInitialSteps(string filePath) =>
            _logger.Information("Running initial pipeline steps (Format, Read) for File: {FilePath}", filePath);

        private void LogInitialStepsCompleted(string filePath, bool success) =>
            _logger.Information("Initial pipeline steps completed for File: {FilePath}. Success: {Success}", filePath, success);

        private void LogContextAfterInitialSteps() =>
            _logger.Verbose("Context after initial steps: {@Context}", _context);

        private void LogCheckingInitialRunResult(string filePath) =>
            _logger.Debug("Checking if initial run was unsuccessful for File: {FilePath}", filePath);

        private void LogInitialRunResult(string filePath, bool result) =>
            _logger.Debug("IsInitialRunUnsuccessful result for File: {FilePath}: {Result}", filePath, result);

        private void LogProcessingErrorPipeline(string filePath) =>
            _logger.Warning("Initial run deemed unsuccessful for File: {FilePath}. Processing error pipeline.", filePath);

        private void LogErrorPipelineCompleted(string filePath, bool result) =>
            _logger.Information("Error pipeline processing finished for File: {FilePath}. Result (Continue?): {Result}", filePath, result);

        private void LogContextAfterErrorPipeline() =>
            _logger.Verbose("Context after error pipeline: {@Context}", _context);

        private void LogProcessingSuccessPipeline(string filePath) =>
            _logger.Information("Initial run deemed successful for File: {FilePath}. Processing success steps.", filePath);

        private void LogSuccessPipelineCompleted(string filePath, bool result) =>
            _logger.Information("Success pipeline processing finished for File: {FilePath}. Result (Overall Success?): {Result}", filePath, result);

        private void LogContextAfterSuccessPipeline() =>
            _logger.Verbose("Context after success pipeline: {@Context}", _context);

        private void LogFatalError(string filePath, Exception ex) =>
            _logger.Fatal(ex, "Fatal error during main InvoiceProcessingPipeline execution for File: {FilePath}", filePath);

        private void LogContextAfterFatalError() =>
            _logger.Verbose("Context after fatal error: {@Context}", _context);

        private void LogPipelineEnd(string filePath) =>
            _logger.Information("Finished main InvoiceProcessingPipeline execution for File: {FilePath}. Final Import Status: {ImportStatus}", filePath, _context?.ImportStatus);
    }
}