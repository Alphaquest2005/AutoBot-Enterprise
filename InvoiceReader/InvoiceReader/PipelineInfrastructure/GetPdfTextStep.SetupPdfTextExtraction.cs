using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class GetPdfTextStep
    {
        private static void SetupPdfTextExtraction(InvoiceProcessingContext context, string filePath, out Task<string> ripTask, // Add filePath parameter
            out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            // Logging happens within the async methods themselves
            ripTask = GetRippedTextAsync(context);
            singleColumnTask = GetSingleColumnPdfText(context);
            sparseTextTask = GetPdfSparseTextAsync(context);
        }
    }
}