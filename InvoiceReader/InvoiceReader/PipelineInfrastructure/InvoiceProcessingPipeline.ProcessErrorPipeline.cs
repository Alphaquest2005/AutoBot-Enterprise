namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class InvoiceProcessingPipeline
{
    private async Task<bool> ProcessErrorPipeline()
    {
        string filePath = _context?.FilePath ?? "Unknown";
        _logger.Information("Starting ProcessErrorPipeline for File: {FilePath}", filePath);
        // Replace Console.WriteLine

        try
        {
            // Error handling pipeline
            _logger.Debug("Creating error pipeline steps for File: {FilePath}", filePath);
            var errorSteps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new HandleErrorStateStep(_isLastTemplate), // Handles email logic
                new UpdateImportStatusStep() // Update status after error handling
            };
            _logger.Debug("Error pipeline steps created. Count: {Count}", errorSteps.Count);

            // Pass pipeline name to runner constructor
            var errorRunner = new PipelineRunner<InvoiceProcessingContext>(errorSteps, "Error Pipeline");
            _logger.Information("Running error pipeline steps for File: {FilePath}", filePath);
            // Assuming PipelineRunner handles internal step logging/errors and context updates
            await errorRunner.Run(_context).ConfigureAwait(false);
            _logger.Information("Error pipeline runner finished for File: {FilePath}", filePath);

            // Determine if processing should continue based on final status
            bool continueProcessing = _context.ImportStatus != ImportStatus.Failed;
            _logger.Information(
                "Final ImportStatus after error steps: {ImportStatus}. Continue Processing considered: {ContinueProcessing}",
                _context.ImportStatus, continueProcessing);
            return continueProcessing;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during ProcessErrorPipeline for File: {FilePath}", filePath);
            if (_context != null) _context.ImportStatus = ImportStatus.Failed; // Ensure status reflects failure
            return false; // Indicate failure to continue
        }
    }
}