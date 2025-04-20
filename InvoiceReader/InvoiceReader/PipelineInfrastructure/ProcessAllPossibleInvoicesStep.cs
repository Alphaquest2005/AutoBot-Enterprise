namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ProcessAllPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.PossibleInvoices == null)
            {
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: No possible invoices found. Skipping template processing.");
                return true; // Not an error if no possible invoices
            }

            var possibleInvoicesList = context.PossibleInvoices.ToList();

            for (int i = 0; i < possibleInvoicesList.Count; i++)
            {
                var template = possibleInvoicesList[i];
                bool isLastTemplate = (i == possibleInvoicesList.Count - 1);

                // Create a new context for each template processing pipeline to avoid interference
                // Copy necessary properties from the original context
                var templateContext = new InvoiceProcessingContext
                {
                    FilePath = context.FilePath,
                    FileTypeId = context.FileTypeId,
                    EmailId = context.EmailId,
                    OverWriteExisting = context.OverWriteExisting,
                    DocSet = context.DocSet,
                    FileType = context.FileType,
                    Client = context.Client,
                    PdfText = context.PdfText, // Pass the original PdfText
                    Template = template, // Pass the current template
                    Templates = context.Templates, // Pass the list of all templates
                    PossibleInvoices = context.PossibleInvoices, // Pass the list of possible invoices
                    Imports = context.Imports // Pass the shared Imports dictionary
                };

                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: Running InvoiceProcessingPipeline for template '{template.OcrInvoices.Name}' (ID: {template.OcrInvoices.Id}).");

                var invoiceProcessingPipeline = new InvoiceProcessingPipeline(templateContext, isLastTemplate);
                await invoiceProcessingPipeline.RunPipeline().ConfigureAwait(false);

                // The result of the sub-pipeline (success or failure for this template)
                // is reflected in templateContext.ImportStatus and added to context.Imports
            }

            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Finished processing all possible invoices.");

            return true; // Indicate that this step completed its iteration
        }
    }
}