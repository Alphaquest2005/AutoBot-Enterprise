using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.Business.Services.Utils; // Assuming PipelineRunner is here
using WaterNut.DataSpace; // Assuming ImportStatus is here

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
            // Initial steps: Format and Read
            var initialSteps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new FormatPdfTextStep(),
                new ReadFormattedTextStep()
            };

            var initialRunner = new PipelineRunner<InvoiceProcessingContext>(initialSteps);
            bool initialRunSuccess = await initialRunner.Run(_context).ConfigureAwait(false);

            if (!initialRunSuccess || _context.CsvLines == null || _context.CsvLines.Count < 1 || _context.Template.Success == false)
            {
                System.Console.WriteLine(
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
            else
            {
                System.Console.WriteLine(
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
        }
    }
}