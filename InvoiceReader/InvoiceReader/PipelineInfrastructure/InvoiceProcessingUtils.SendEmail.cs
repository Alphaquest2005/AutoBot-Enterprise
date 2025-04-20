namespace WaterNut.DataSpace.PipelineInfrastructure;

public static partial class InvoiceProcessingUtils
{
    private static void SendEmail(string file, EmailDownloader.Client client, string txtFile, string body)
    {
        _utilsLogger.Debug("Attempting to send email for File: {FilePath}", file);
        try
        {
            var contacts = EmailDownloader.EmailDownloader.GetContacts("Developer");
            _utilsLogger.Debug("Sending email to {ContactCount} developer contacts for File: {FilePath}",
                contacts?.Count() ?? 0, file);
            // Consider logging subject and body length for debugging, but avoid logging full body unless necessary (PII)
            _utilsLogger.Verbose(
                "Email Subject: {Subject}, Body Length: {BodyLength}, Attachment Count: {AttachmentCount}",
                "Invoice Template Not found!", body?.Length ?? 0, (file != null ? 1 : 0) + (txtFile != null ? 1 : 0));

            EmailDownloader.EmailDownloader.SendEmail(client, null, "Invoice Template Not found!",
                contacts, body, new[] { file, txtFile }.Where(f => f != null).ToArray()); // Filter null attachments

            _utilsLogger.Information("Successfully sent email regarding File: {FilePath}", file);
        }
        catch (Exception ex)
        {
            _utilsLogger.Error(ex, "Failed to send email for File: {FilePath}", file);
            // Decide if the exception should be propagated
        }
    }
}