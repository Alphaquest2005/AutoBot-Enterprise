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
            _logger.Debug("Starting RunInitialPipelineSteps for File: {FilePath}", filePath);
            try
            {
                // Pass pipeline name to runner constructor
                var initialRunner = new PipelineRunner<InvoiceProcessingContext>(initialSteps, "Initial Pipeline");
                _logger.Verbose("Initial PipelineRunner created.");
                // Assuming PipelineRunner internally logs start/end of each step and handles context updates
                bool initialRunSuccess = await initialRunner.Run(_context).ConfigureAwait(false);
                _logger.Debug("Initial PipelineRunner finished. Overall Success: {Success}", initialRunSuccess);
                return initialRunSuccess;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during RunInitialPipelineSteps for File: {FilePath}", filePath);
                return false; // Indicate failure of initial steps
            }
        }
    }
}