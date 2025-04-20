using System.Threading.Tasks;
using WaterNut.Business.Services.Utils; // Assuming this is needed for _template.Format

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class FormatPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            // Assuming _template is available in the context or passed in somehow
            // For now, I will assume it's available in the context for simplicity
            // A more robust solution might involve passing it in the constructor or a dedicated property
            if (context.Template == null)
            {
                // Handle the case where the template is not available
                // This might involve logging an error or returning false
                return false;
            }

            context.FormattedPdfText = context.Template.Format(context.PdfText.ToString());
            System.Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: PDF text formatted using template {context.Template.OcrInvoices.Id}.");

            return true; // Indicate success
        }
    }
}