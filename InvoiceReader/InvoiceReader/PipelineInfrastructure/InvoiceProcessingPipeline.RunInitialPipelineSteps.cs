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
                LogPipelineError(ex, filePath);
                return false;
            }
        }

        private void LogRunInitialPipelineSteps(string filePath)
        {
            _logger.Debug("Starting RunInitialPipelineSteps for File: {FilePath}", filePath);
        }

        private PipelineRunner<InvoiceProcessingContext> CreatePipelineRunner(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            var initialRunner = new PipelineRunner<InvoiceProcessingContext>(initialSteps, "Initial Pipeline");
            _logger.Verbose("Initial PipelineRunner created.");
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
            _logger.Debug("Initial PipelineRunner finished. Overall Success: {Success}", success);
        }

        private void LogPipelineError(Exception ex, string filePath)
        {
            _logger.Error(ex, "Error during RunInitialPipelineSteps for File: {FilePath}", filePath);
        }
    }
}