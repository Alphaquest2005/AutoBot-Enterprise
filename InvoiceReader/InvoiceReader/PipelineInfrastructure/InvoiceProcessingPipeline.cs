// Assuming PipelineRunner is here

// Assuming ImportStatus is here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class InvoiceProcessingPipeline
    {
        private readonly InvoiceProcessingContext _context;
        private readonly bool _isLastTemplate;

        public InvoiceProcessingPipeline(InvoiceProcessingContext context, bool isLastTemplate)
        {
            _context = context;
            _isLastTemplate = isLastTemplate;
        }

        public async Task<bool> RunPipeline()
        {
            List<IPipelineStep<InvoiceProcessingContext>> initialSteps = InitializePipelineSteps();

            bool initialRunSuccess = await RunInitialPipelineSteps(initialSteps).ConfigureAwait(false);

            if (IsInitialRunUnsuccessful(initialRunSuccess))
            {
                return await ProcessErrorPipeline().ConfigureAwait(false);
            }
            else
            {
                return await ProcessSuccessfulSteps().ConfigureAwait(false);
            }
        }

        private async Task<bool> ProcessSuccessfulSteps()
        {
            Console.WriteLine(
                                $"[OCR DEBUG] Pipeline: Initial steps successful. Proceeding with Success State Pipeline.");
            // Success handling pipeline
            var successSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new AddNameSupplierStep(),
                    new AddMissingRequiredFieldValuesStep(),
                    new WriteFormattedTextFileStep(),
                    new HandleImportSuccessStateStep(),
                    new UpdateImportStatusStep() // Update status after success handling
                };
            var successRunner = new PipelineRunner<InvoiceProcessingContext>(successSteps);
            await successRunner.Run(_context).ConfigureAwait(false);
            return _context.ImportStatus == ImportStatus.Success || _context.ImportStatus == ImportStatus.HasErrors; // Indicate success if not completely failed
        }

        private async Task<bool> ProcessErrorPipeline()
        {
            Console.WriteLine(
                $"[OCR DEBUG] Pipeline: Initial steps failed or read returned no lines or template success is false. Entering Error State Pipeline.");
            // Error handling pipeline
            var errorSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new HandleErrorStateStep(_isLastTemplate),
                    new UpdateImportStatusStep() // Update status after error handling
                };
            var errorRunner = new PipelineRunner<InvoiceProcessingContext>(errorSteps);
            await errorRunner.Run(_context).ConfigureAwait(false);
            return _context.ImportStatus != ImportStatus.Failed; // Continue if not completely failed
        }

        private bool IsInitialRunUnsuccessful(bool initialRunSuccess)
        {
            return !initialRunSuccess || _context.CsvLines == null || _context.CsvLines.Count < 1 || _context.Template.Success == false;
        }

        private async Task<bool> RunInitialPipelineSteps(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
            var initialRunner = new PipelineRunner<InvoiceProcessingContext>(initialSteps);
            bool initialRunSuccess = await initialRunner.Run(_context).ConfigureAwait(false);
            return initialRunSuccess;
        }

        private static List<IPipelineStep<InvoiceProcessingContext>> InitializePipelineSteps()
        {
            // Initial steps: Format and Read
            return new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new FormatPdfTextStep(),
                new ReadFormattedTextStep()
            };
        }
    }
}