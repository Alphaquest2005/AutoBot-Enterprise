using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here

// Assuming ImportStatus is here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class UpdateImportStatusStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (!IsImportDataValid(context)) // Need to figure out how ImportStatus is passed
            {
                var importStatus = ProcessImportFile(context);
                return LogImportStatusUpdate(importStatus);
            }
            else
            {
                // Handle the case where required data is missing
                // This might involve logging an error or returning false
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: Required data is missing for import status update.");
                     // Required data is missing
                return false;
            }          
        }

        private static bool IsImportDataValid(InvoiceProcessingContext context)
        {
            return context.Template == null || string.IsNullOrEmpty(context.FilePath) || context.Imports == null /*|| context.ImportStatus == null*/;
        }

        private static bool LogImportStatusUpdate(ImportStatus importStatus)
        {
            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Updated import status to {importStatus}.");

            // This step always succeeds in updating the status if it reaches here
            return true;
        }

        private static ImportStatus ProcessImportFile(InvoiceProcessingContext context)
        {

            var fileDescription = FileTypeManager.GetFileType(context.Template.OcrInvoices.FileTypeId).Description;
            // Assuming ImportStatus is available in the context based on previous steps
            ImportStatus importStatus = context.ImportStatus; // This property needs to be added to the context
            switch (importStatus)
            {
                case ImportStatus.Success:
                    context.Imports.Add($"{context.FilePath}-{context.Template.OcrInvoices.Name}-{context.Template.OcrInvoices.Id}",
                        (context.FilePath, FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.Success));
                    break;
                case ImportStatus.HasErrors:
                    context.Imports.Add($"{context.FilePath}-{context.Template.OcrInvoices.Name}-{context.Template.OcrInvoices.Id}",
                        (context.FilePath, FileTypeManager.EntryTypes.GetEntryType(fileDescription),
                            ImportStatus.HasErrors));
                    break;
                case ImportStatus.Failed:
                    // Assuming ReportUnImportedFile is handled elsewhere or in a previous step if needed for this case
                    context.Imports.Add($"{context.FilePath}-{context.Template.OcrInvoices.Name}-{context.Template.OcrInvoices.Id}",
                        (context.FilePath, FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.Failed));
                    break;
            }

            return importStatus;
        }
    }
}