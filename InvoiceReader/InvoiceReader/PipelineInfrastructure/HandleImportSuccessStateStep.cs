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
            if (!IsRequiredDataMissing(context))
            {
                FileTypes fileType = ResolveFileType(context);

                bool processResult = new DataFileProcessor()
                                    .Process(CreateDataFile(context, fileType))
                                    .GetAwaiter()
                                    .GetResult();

                return LogImportProcessingOutcome(processResult);
            }
            else
            {
                 // Required data is missing
                return false;
            }           
        }

        private static bool IsRequiredDataMissing(InvoiceProcessingContext context)
        {
            return context.FileType == null || context.DocSet == null || context.Template == null || context.CsvLines == null || string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.EmailId);
        }

        private static FileTypes ResolveFileType(InvoiceProcessingContext context)
        {
            // Logic from the original ImportSuccessState method
            FileTypes fileType = context.FileType;
            if (fileType.Id != context.Template.OcrInvoices.FileTypeId)
                fileType = FileTypeManager.GetFileType(context.Template.OcrInvoices.FileTypeId);
            return fileType;
        }

        private static bool LogImportProcessingOutcome(bool processResult)
        {
            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Handled import success state. Process result: {processResult}.");

            return processResult; // Indicate success based on the result of DataFileProcessor().Process
        }

        private static DataFile CreateDataFile(InvoiceProcessingContext context, FileTypes fileType)
        {
            return new DataFile(fileType, context.DocSet, context.OverWriteExisting,
                                                context.EmailId,
                                                context.FilePath, context.CsvLines);
        }
    }
}