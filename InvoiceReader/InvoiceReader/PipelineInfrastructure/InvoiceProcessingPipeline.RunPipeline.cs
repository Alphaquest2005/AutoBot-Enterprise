namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class InvoiceProcessingPipeline
{
    public async Task<bool> RunPipeline()
    {
        string filePath = _context?.FilePath ?? "Unknown";
        _logger.Information("Starting main InvoiceProcessingPipeline execution for File: {FilePath}", filePath);

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

            _logger.Information("Running initial pipeline steps (Format, Read) for File: {FilePath}", filePath);
            bool initialRunSuccess =
                await RunInitialPipelineSteps(initialSteps).ConfigureAwait(false); // Handles its own logging
            _logger.Information("Initial pipeline steps completed for File: {FilePath}. Success: {Success}", filePath,
                initialRunSuccess);

            _logger.Debug("Checking if initial run was unsuccessful for File: {FilePath}", filePath);
            if (IsInitialRunUnsuccessful(initialRunSuccess)) // Handles its own logging
            {
                _logger.Warning("Initial run deemed unsuccessful for File: {FilePath}. Processing error pipeline.",
                    filePath);
                // ProcessErrorPipeline handles its own logging
                bool errorPipelineResult = await ProcessErrorPipeline().ConfigureAwait(false);
                _logger.Information(
                    "Error pipeline processing finished for File: {FilePath}. Result (Continue?): {Result}", filePath,
                    errorPipelineResult);
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
                return successPipelineResult; // Return result from success pipeline
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Fatal error during main InvoiceProcessingPipeline execution for File: {FilePath}",
                filePath);
            // Ensure status reflects failure before returning
            if (_context != null) _context.ImportStatus = ImportStatus.Failed;
            return false; // Indicate overall failure
        }
    }
}