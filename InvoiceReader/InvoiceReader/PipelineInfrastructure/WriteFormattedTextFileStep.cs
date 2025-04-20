namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class WriteFormattedTextFileStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.FormattedPdfText))
            {
                // Required data is missing
                return false;
            }

            // Logic from the original WriteTextFile method
            var txtFile = context.FilePath + ".txt";
            //if (File.Exists(txtFile)) return; // Original code had this commented out
            File.WriteAllText(txtFile, context.FormattedPdfText);

           context.TxtFile = txtFile; // Set TxtFile in context

           Console.WriteLine(
               $"[OCR DEBUG] Pipeline Step: Wrote formatted text to {txtFile}.");

           // The original method returned the file path, but pipeline steps return bool
           // If the write operation was successful, we return true.
           return true; // Indicate success
       }
   }
}