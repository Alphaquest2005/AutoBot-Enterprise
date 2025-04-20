// Needed for EmailDownloader.EmailDownloader and Client

// Needed for file paths

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class SendEmailStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (!IsRequiredDataMissing(context))
            {
                // Logic from the original CreateEmail method for sending the email
                EmailDownloader.EmailDownloader.SendEmail(context.Client, null, "Invoice Template Not found!",
                    EmailDownloader.EmailDownloader.GetContacts("Developer"), context.EmailBody, new[] { context.FilePath, context.TxtFile });

                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: Sent email for file '{context.FilePath}'.");

                return true; // Indicate success
            }                
            else
            {
                // Required data is missing
                return false;
            }    
        }

        private static bool IsRequiredDataMissing(InvoiceProcessingContext context)
        {
            return context.Client == null || string.IsNullOrEmpty(context.EmailBody) || string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.TxtFile);
        }
    }
}