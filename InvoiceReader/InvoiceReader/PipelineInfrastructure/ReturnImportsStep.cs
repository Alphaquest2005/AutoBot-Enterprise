// Assuming ImportStatus is here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ReturnImportsStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            Console.WriteLine($"[OCR DEBUG] Pipeline Step: Finalizing import process.");

            // Determine overall success based on the ImportStatus of processed files
            bool overallSuccess = context.Imports != null &&
                                  context.Imports.Any(x => x.Value.Success == ImportStatus.Success ||
                                                           x.Value.Success == ImportStatus.HasErrors);

            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Overall import success: {overallSuccess}.");

            return overallSuccess; // Indicate overall success or failure
        }
    }
}