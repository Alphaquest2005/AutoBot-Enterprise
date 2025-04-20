// Assuming this is needed for _template.Read

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.Template != null)
            {
                ExtractCsvLines(context);

                ProcessCsvDataExtraction(context);

                return ValidateTemplateSuccess(context);
            }
            else
            {
                // Handle the case where the template is not available
                // This might involve logging an error or returning false
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: Template is not available.");
                         // Handle the case where the template is not available
            return false;

            }       
        }

        private static void ProcessCsvDataExtraction(InvoiceProcessingContext context)
        {
            List<IDictionary<string, object>> list;
            if (ExtractCsvData(context, out list))
            {
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: First data structure contains {list.Count} items.");
            }
            else
            {
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: No data structures found or first data structure is not a list.");
            }
        }

        private static bool ValidateTemplateSuccess(InvoiceProcessingContext context)
        {
            if (context.Template.Success == false)
            {
                // If template.Success is false after reading, it means the template did not match
                // We should stop processing with this template and potentially try the next one
                return false;
            }
            else
            {
                // If template.Success is true, we can proceed with the next steps in the pipeline
                // This might involve further processing of the data or moving to the next step
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: Template read successfully.");
                return true; // Indicate success
            }
        }

        private static bool ExtractCsvData(InvoiceProcessingContext context, out List<IDictionary<string, object>> list)
        {
            list = (List<IDictionary<string, object>>) context.CsvLines[0];
            return (context.CsvLines != null && context.CsvLines.Count > 0 );
        }

        private static void ExtractCsvLines(InvoiceProcessingContext context)
        {
            // Assuming Read method returns List<dynamic>
            context.CsvLines = context.Template.Read(context.FormattedPdfText);
            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: _template.Read returned {context.CsvLines?.Count ?? 0} data structures.");
        }
    }
}