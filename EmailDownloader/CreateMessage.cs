using System; // Added System using
using System.Diagnostics; // Added System.Diagnostics using
using System.IO;
using System.Linq; // Added System.Linq using
using MimeKit;
using Serilog; // Added Serilog using

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static MimeMessage CreateMessage(Client client, string subject, string[] to, string body,
        string[] attachments, ILogger logger) // Add ILogger parameter
    {
        var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
        string methodName = nameof(CreateMessage);
        logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
            methodName, "Create MimeMessage for email", $"ClientEmail: {client?.Email ?? "null"}, Subject: {subject}, ToRecipients: {to?.Length ?? 0}, AttachmentCount: {attachments?.Length ?? 0}");

        try
        {
            // Original CreateMessage logic - remains synchronous
            var message = new MimeMessage();
            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Setting sender address.", methodName, "Setup");
            message.From.Add(new MailboxAddress($"{client?.CompanyName ?? "AutoBot"}-AutoBot", client?.Email ?? "unknown@example.com")); // Added null checks

            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Adding recipients.", methodName, "Setup");
            if (to != null)
            {
                foreach (var recipient in to)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(recipient))
                        {
                            message.To.Add(MailboxAddress.Parse(recipient));
                            logger.Verbose("INTERNAL_STEP ({MethodName} - {Stage}): Added recipient: {Recipient}", methodName, "Setup", recipient);
                        }
                    }
                    catch (Exception recipientEx)
                    {
                        logger.Warning(recipientEx, "INTERNAL_STEP ({MethodName} - {Stage}): Error parsing recipient address '{Recipient}': {ErrorMessage}",
                            methodName, "Setup", recipient, recipientEx.Message);
                    }
                }
            }
            else
            {
                logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): No recipients provided.", methodName, "Setup");
            }

            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Setting subject.", methodName, "Setup");
            message.Subject = subject;

            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Creating BodyBuilder and setting text body.", methodName, "Body");
            var builder = new BodyBuilder { TextBody = body };

            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Adding attachments.", methodName, "Attachments");
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(attachment) && File.Exists(attachment))
                        {
                            builder.Attachments.Add(attachment);
                            logger.Verbose("INTERNAL_STEP ({MethodName} - {Stage}): Added attachment: {AttachmentPath}", methodName, "Attachments", attachment);
                        }
                        else
                        {
                            logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Skipping attachment '{AttachmentPath}' because it is null, empty, or does not exist.",
                                methodName, "Attachments", attachment ?? "null/empty");
                        }
                    }
                    catch (Exception attachEx)
                    {
                        // Log the ignored exception
                        logger.Error(attachEx, "INTERNAL_STEP ({MethodName} - {Stage}): Error adding attachment '{AttachmentPath}': {ErrorMessage}",
                            methodName, "Attachments", attachment ?? "null/empty", attachEx.Message);
                        /* ignore */ // Still ignoring as per original logic, but now logged
                    }
                }
            }
            else
            {
                logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): No attachments provided.", methodName, "Attachments");
            }

            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Building message body.", methodName, "Body");
            message.Body = builder.ToMessageBody();
            logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Message body built.", methodName, "Body");

            methodStopwatch.Stop(); // Stop stopwatch
            logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                methodName, "MimeMessage created successfully", $"Subject: {message.Subject}, RecipientCount: {message.To.Count}, AttachmentCount: {message.Attachments.Count()}", methodStopwatch.ElapsedMilliseconds);
            return message;
        }
        catch (Exception ex)
        {
            methodStopwatch.Stop(); // Stop stopwatch on error
            logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, "Create MimeMessage for email", methodStopwatch.ElapsedMilliseconds, ex.Message);
            throw; // Re-throw the exception
        }
    }
}