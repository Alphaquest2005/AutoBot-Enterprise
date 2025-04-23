using System.Collections.Generic;
using InvoiceReader.PipelineInfrastructure;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class InvoiceProcessingPipeline
    {
        private static List<IPipelineStep<InvoiceProcessingContext>> InitializePipelineSteps()
        {
            _logger.Debug("Initializing initial pipeline steps (FormatPdfTextStep, ReadFormattedTextStep).");
            // Initial steps: Format and Read
            var steps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new GetTemplatesStep(),
                new GetPdfTextStep(),
                new GetPossibleInvoicesStep(),
                
                new FormatPdfTextStep(),
                new ReadFormattedTextStep() // Assuming this step exists

            };
            _logger.Verbose("Initial pipeline steps created. Count: {Count}", steps.Count);
            return steps;
        }
    }
}