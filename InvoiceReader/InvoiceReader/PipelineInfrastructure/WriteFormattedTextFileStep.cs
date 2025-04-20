namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class WriteFormattedTextFileStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (!IsContextDataIncomplete(context))
            {
                string txtFile = CreateFormattedTextFile(context);

                context.TxtFile = txtFile; // Set TxtFile in context

                return LogFileWriteSuccess(txtFile);
            }
            else
            {
                // Required data is missing
                return false;
            }     
        }

        private static bool IsContextDataIncomplete(InvoiceProcessingContext context)
        {
            return string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.FormattedPdfText);
        }

        private static string CreateFormattedTextFile(InvoiceProcessingContext context)
        {
            // Logic from the original WriteTextFile method
            var txtFile = context.FilePath + ".txt";
            //if (File.Exists(txtFile)) return; // Original code had this commented out
            File.WriteAllText(txtFile, context.FormattedPdfText);
            return txtFile;
        }

        private static bool LogFileWriteSuccess(string txtFile)
        {
            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Wrote formatted text to {txtFile}.");

            // The original method returned the file path, but pipeline steps return bool
            // If the write operation was successful, we return true.
            return true; // Indicate success
        }
    }
}