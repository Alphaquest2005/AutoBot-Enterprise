using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.Business.Services.Utils; // Assuming PipelineRunner is here

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
            // Define the email creation pipeline steps
            var emailPipelineSteps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new ConstructEmailBodyStep(),
                new SendEmailStep()
            };

            // Create and run the email pipeline
            var emailPipelineRunner = new PipelineRunner<InvoiceProcessingContext>(emailPipelineSteps);
            bool success = await emailPipelineRunner.Run(_context).ConfigureAwait(false);

            System.Console.WriteLine(
                $"[OCR DEBUG] CreateEmailPipeline: Finished. Success: {success}.");

            return success; // Indicate if the email pipeline completed successfully
        }
    }
}