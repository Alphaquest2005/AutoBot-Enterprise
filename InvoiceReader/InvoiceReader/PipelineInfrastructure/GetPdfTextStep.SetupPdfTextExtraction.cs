namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class GetPdfTextStep
{
    private static void SetupPdfTextExtraction(InvoiceProcessingContext context, out Task<string> ripTask,
        out Task<string> singleColumnTask, out Task<string> sparseTextTask)
    {
        // Logging happens within the async methods themselves
        ripTask = GetRippedTextAsync(context);
        singleColumnTask = GetSingleColumnPdfText(context);
        sparseTextTask = GetPdfSparseTextAsync(context);
    }
}