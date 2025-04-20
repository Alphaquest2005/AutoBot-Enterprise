// Assuming PipelineRunner is here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class CreateEmailPipeline
    {
        private readonly InvoiceProcessingContext _context;

        public CreateEmailPipeline(InvoiceProcessingContext context)
        {
            _context = context;
        }

        public async Task<bool> RunPipeline()
        {
            List<IPipelineStep<InvoiceProcessingContext>> emailPipelineSteps = CreateEmailPipelineSteps();

            // Create and run the email pipeline
            var emailPipelineRunner = new PipelineRunner<InvoiceProcessingContext>(emailPipelineSteps);

            bool success = await emailPipelineRunner.Run(_context).ConfigureAwait(false);
            
            return LogPipelineResult(success);
        }

        private static bool LogPipelineResult(bool success)
        {
            Console.WriteLine(
                            $"[OCR DEBUG] CreateEmailPipeline: Finished. Success: {success}.");

            return success; // Indicate if the email pipeline completed successfully
        }

        private static List<IPipelineStep<InvoiceProcessingContext>> CreateEmailPipelineSteps()
        {
            // Define the email creation pipeline steps
            return new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new ConstructEmailBodyStep(),
                new SendEmailStep()
            };
        }
    }
}