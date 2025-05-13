// Needed for EmailDownloader.EmailDownloader and Client
using EmailDownloader; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Collections.Generic; // Added for IEnumerable/List

// Needed for file paths

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class SendEmailStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<SendEmailStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing SendEmailStep for File: {FilePath}", filePath);

            // Null check context first
            if (context == null)
            {
                 _logger.Error("SendEmailStep executed with null context.");
                 return false;
            }

            if (IsRequiredDataMissing(context)) // Handles its own logging
            {
                 _logger.Warning("SendEmailStep cannot proceed due to missing required data in context for File: {FilePath}", filePath);
                 return false; // Step fails if required data is missing
            }

            try
            {
                // Logic from the original CreateEmail method for sending the email
                 _logger.Information("Attempting to send error email for File: {FilePath}", filePath);
                 IEnumerable<string> contacts = null;
                 try
                 {
                     contacts = EmailDownloader.EmailDownloader.GetContacts("Developer");
                     _logger.Debug("Sending email to {ContactCount} developer contacts.", contacts?.Count() ?? 0);
                 }
                 catch (Exception contactEx)
                 {
                      _logger.Error(contactEx, "Error getting developer contacts for email.");
                      // Decide if we should proceed without contacts or fail
                      return false; // Fail step if contacts cannot be retrieved
                 }


                 // Filter out null or empty file paths before attaching
                 var attachments = new[] { context.FilePath, context.TextFilePath }
                                     .Where(f => !string.IsNullOrEmpty(f))
                                     .ToArray();
                 _logger.Verbose("Email Subject: {Subject}, Body Length: {BodyLength}, Attachments: {Attachments}",
                    "Template Template Not found!", context.EmailBody?.Length ?? 0, attachments);

                 // Assuming SendEmail is synchronous or handles its own async internally
                 // Running potentially blocking network code in background thread.
                
                   await EmailDownloader.EmailDownloader.SendEmailAsync(
                        context.Client,
                        null, // Assuming null sender is intended
                        "Template Template Not found!",
                        contacts?.ToArray(), // Convert IEnumerable<string> to string[]
                        context.EmailBody,
                        attachments // Use filtered list
                    ).ConfigureAwait(false);

                 _logger.Information("Successfully sent error email for File: {FilePath}", filePath);

                 _logger.Debug("Finished executing SendEmailStep successfully for File: {FilePath}", filePath);
                 return true; // Indicate success
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error sending email for File: {FilePath}", filePath);
                 return false; // Indicate failure
            }
        }

        private static bool IsRequiredDataMissing(InvoiceProcessingContext context)
        {
             _logger.Verbose("Checking for missing required data for sending email.");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (context.Client == null) { _logger.Warning("Missing required data for SendEmail: Client is null."); return true; }
             if (string.IsNullOrEmpty(context.EmailBody)) { _logger.Warning("Missing required data for SendEmail: EmailBody is null or empty."); return true; }
             // FilePath and TextFilePath are used as attachments, but maybe sending without them is acceptable?
             // Let's only require Client and EmailBody for now, attachments are optional.
             // if (string.IsNullOrEmpty(context.FilePath)) { _logger.Warning("Missing required data for SendEmail: FilePath is null or empty."); return true; }
             // if (string.IsNullOrEmpty(context.TextFilePath)) { _logger.Warning("Missing required data for SendEmail: TextFilePath is null or empty."); return true; }

             _logger.Verbose("No missing required data found for sending email (Client, EmailBody).");
             return false; // All strictly required data is present
        }
    }
}