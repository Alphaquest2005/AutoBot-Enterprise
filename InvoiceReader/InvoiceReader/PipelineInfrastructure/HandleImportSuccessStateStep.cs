using CoreEntities.Business.Entities; // Assuming FileTypes is here
// Assuming AsycudaDocumentSet is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager and DataFileProcessor are here

// Assuming DataFile is here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class HandleImportSuccessStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.FileType == null || context.DocSet == null || context.Template == null || context.CsvLines == null || string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.EmailId))
            {
                // Required data is missing
                return false;
            }

            // Logic from the original ImportSuccessState method
            FileTypes fileType = context.FileType;
            if (fileType.Id != context.Template.OcrInvoices.FileTypeId)
                fileType = FileTypeManager.GetFileType(context.Template.OcrInvoices.FileTypeId);


            bool processResult = new DataFileProcessor().Process(new DataFile(fileType, context.DocSet, context.OverWriteExisting,
                context.EmailId,
                context.FilePath, context.CsvLines)).GetAwaiter().GetResult();

            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Handled import success state. Process result: {processResult}.");

            return processResult; // Indicate success based on the result of DataFileProcessor().Process
        }
    }
}