namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class InvoiceProcessingPipeline
{
    public async Task<bool> RunPipeline()
    {
        string filePath = _context?.FilePath ?? "Unknown";
        _logger.Information("Starting main InvoiceProcessingPipeline execution for File: {FilePath}", filePath);
        _logger.Verbose("Context details: {@Context}", _context); // Log context details

        // Null check context early
        if (_context == null)
        {
            _logger.Error("InvoiceProcessingPipeline cannot run with a null context.");
            return false;
        }

        try
        {
            _logger.Debug("Initializing initial pipeline steps for File: {FilePath}", filePath);
            List<IPipelineStep<InvoiceProcessingContext>>
                initialSteps = InitializePipelineSteps(); // Handles its own logging
            _logger.Debug("Initial pipeline steps created. Count: {Count}", initialSteps.Count);
            _logger.Verbose("Initial steps: {@InitialSteps}", initialSteps.Select(step => step.GetType().Name)); // Log step types

            _logger.Information("Running initial pipeline steps (Format, Read) for File: {FilePath}", filePath);
            bool initialRunSuccess =
                await RunInitialPipelineSteps(initialSteps).ConfigureAwait(false); // Handles its own logging
            _logger.Information("Initial pipeline steps completed for File: {FilePath}. Success: {Success}", filePath,
                initialRunSuccess);
            _logger.Verbose("Context after initial steps: {@Context}", _context); // Log context after initial steps


            _logger.Debug("Checking if initial run was unsuccessful for File: {FilePath}", filePath);
            bool isInitialRunUnsuccessful = IsInitialRunUnsuccessful(initialRunSuccess); // Handles its own logging
            _logger.Debug("IsInitialRunUnsuccessful result for File: {FilePath}: {Result}", filePath, isInitialRunUnsuccessful);

            if (isInitialRunUnsuccessful)
            {
                _logger.Warning("Initial run deemed unsuccessful for File: {FilePath}. Processing error pipeline.",
                    filePath);
                // ProcessErrorPipeline handles its own logging
                bool errorPipelineResult = await ProcessErrorPipeline().ConfigureAwait(false);
                _logger.Information(
                    "Error pipeline processing finished for File: {FilePath}. Result (Continue?): {Result}", filePath,
                    errorPipelineResult);
                _logger.Verbose("Context after error pipeline: {@Context}", _context); // Log context after error pipeline
                return errorPipelineResult; // Return result from error pipeline
            }
            else
            {
                _logger.Information("Initial run deemed successful for File: {FilePath}. Processing success steps.",
                    filePath);
                // ProcessSuccessfulSteps handles its own logging
                bool successPipelineResult = await ProcessSuccessfulSteps().ConfigureAwait(false);
                _logger.Information(
                    "Success pipeline processing finished for File: {FilePath}. Result (Overall Success?): {Result}",
                    filePath, successPipelineResult);
                _logger.Verbose("Context after success pipeline: {@Context}", _context); // Log context after success pipeline
                return successPipelineResult; // Return result from success pipeline
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Fatal error during main InvoiceProcessingPipeline execution for File: {FilePath}",
                filePath);
            // Ensure status reflects failure before returning
            if (_context != null) _context.ImportStatus = ImportStatus.Failed;
            _logger.Verbose("Context after fatal error: {@Context}", _context); // Log context after fatal error
            return false; // Indicate overall failure
        }
        finally
        {
             _logger.Information("Finished main InvoiceProcessingPipeline execution for File: {FilePath}. Final Import Status: {ImportStatus}",
                 filePath, _context?.ImportStatus); // Log final status
        }
    }
}