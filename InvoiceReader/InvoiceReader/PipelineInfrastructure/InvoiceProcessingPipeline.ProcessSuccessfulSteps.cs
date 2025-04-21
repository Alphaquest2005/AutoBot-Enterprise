using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class InvoiceProcessingPipeline
    {
        private async Task<bool> ProcessSuccessfulSteps()
        {
            string filePath = _context?.FilePath ?? "Unknown";
            _logger.Information("Starting ProcessSuccessfulSteps for File: {FilePath}", filePath);
            // Replace Console.WriteLine

            try
            {
                // Success handling pipeline
                _logger.Debug("Creating success pipeline steps for File: {FilePath}", filePath);
                var successSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new AddNameSupplierStep(),
                    new AddMissingRequiredFieldValuesStep(),
                    new WriteFormattedTextFileStep(), // Assuming this step exists
                    new HandleImportSuccessStateStep(),
                    new UpdateImportStatusStep() // Update status after success handling
                };
                _logger.Debug("Success pipeline steps created. Count: {Count}", successSteps.Count);

                // Pass pipeline name to runner constructor
                var successRunner = new PipelineRunner<InvoiceProcessingContext>(successSteps, "Success Pipeline");
                _logger.Information("Running success pipeline steps for File: {FilePath}", filePath);
                // Assuming PipelineRunner handles internal step logging/errors and context updates
                await successRunner.Run(_context).ConfigureAwait(false);
                _logger.Information("Success pipeline runner finished for File: {FilePath}", filePath);

                // Determine overall success based on final import status
                bool overallSuccess = _context.ImportStatus == ImportStatus.Success ||
                                      _context.ImportStatus == ImportStatus.HasErrors;
                _logger.Information(
                    "Final ImportStatus after success steps: {ImportStatus}. Overall Success considered: {OverallSuccess}",
                    _context.ImportStatus, overallSuccess);
                return overallSuccess;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during ProcessSuccessfulSteps for File: {FilePath}", filePath);
                if (_context != null) _context.ImportStatus = ImportStatus.Failed; // Ensure status reflects failure
                return false;
            }
        }
    }
}