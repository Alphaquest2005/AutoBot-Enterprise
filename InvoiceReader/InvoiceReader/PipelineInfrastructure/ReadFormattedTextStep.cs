using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.Business.Services.Utils; // Assuming this is needed for _template.Read

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.Template == null)
            {
                // Handle the case where the template is not available
                return false;
            }

            // Assuming Read method returns List<dynamic>
            context.CsvLines = context.Template.Read(context.FormattedPdfText);
            System.Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: _template.Read returned {context.CsvLines?.Count ?? 0} data structures.");

            if (context.CsvLines != null && context.CsvLines.Count > 0 && context.CsvLines[0] is List<IDictionary<string, object>> list)
            {
                System.Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: First data structure contains {list.Count} items.");
            }
             if (context.Template.Success == false)
            {
                // If template.Success is false after reading, it means the template did not match
                // We should stop processing with this template and potentially try the next one
                return false;
            }


            return true; // Indicate success
        }
    }
}