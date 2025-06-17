using System.Collections.Generic;
using InvoiceReader.PipelineInfrastructure;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Linq;

    public partial class InvoiceProcessingPipeline
    {
        private List<IPipelineStep<InvoiceProcessingContext>> InitializePipelineSteps()
        {
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(InitializePipelineSteps), "Initialization", "Initializing initial pipeline steps.", "", "");
            // Initial steps: GetTemplates, GetPdfText, GetPossibleInvoices, Format and Read
            var steps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                //new DatabaseValidationStep(this._logger),
                new GetTemplatesStep(),
                new GetPdfTextStep(),
                new GetPossibleInvoicesStep(),
                
                new FormatPdfTextStep(),
                new ReadFormattedTextStep() // Assuming this step exists

            };
            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(InitializePipelineSteps), "StepsCreated", "Initial pipeline steps created.", $"StepCount: {steps.Count}", new { Steps = steps.Select(step => step.GetType().Name) });
            return steps;
        }
    }
}