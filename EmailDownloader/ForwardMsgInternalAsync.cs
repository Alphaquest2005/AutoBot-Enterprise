using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using Serilog; // Added

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task ForwardMsgInternalAsync(MimeMessage originalMsg, Client clientDetails, string subject, string body,
               string[] contacts, string[] attachments, CancellationToken cancellationToken, ILogger logger) // Added ILogger parameter
    {
        if (clientDetails.Email == null) return;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));

        if (!clientDetails.DevMode)
        {
            if (originalMsg != null && originalMsg.From.Mailboxes.Any())
            {
                message.ReplyTo.Add(new MailboxAddress(originalMsg.From.First().Name ?? "Original Sender", originalMsg.From.Mailboxes.First().Address));
            }
            else
            {
                // Fallback if original sender is unknown
                var devContacts = GetContacts("Developer", logger); // Pass the logger
                if (devContacts.Any()) message.ReplyTo.Add(MailboxAddress.Parse(devContacts.First()));
            }
            foreach (var recipent in contacts.Distinct()) { message.To.Add(MailboxAddress.Parse(recipent)); }
        }
        else
        {
           // DevMode logic for recipients(e.g., send to a specific test address)

            var devContacts = GetContacts("Developer", logger);
             if (devContacts.Any()) message.To.Add(MailboxAddress.Parse(devContacts.First()));
        }

        message.Subject = subject;
        var builder = new BodyBuilder { TextBody = body };

        // Only add original message as attachment if it's not null and has content (e.g., MessageId is set)
        if (originalMsg != null && originalMsg.MessageId != null)
        {
            builder.Attachments.Add(new MessagePart { Message = originalMsg });
        }

        foreach (var attachmentPath in attachments)
        {
            if (File.Exists(attachmentPath)) builder.Attachments.Add(attachmentPath);
        }
        message.Body = builder.ToMessageBody();
        // Use the public SendEmailAsync (MimeMessage overload)
        await SendEmailInternalAsync(clientDetails, message, cancellationToken, logger).ConfigureAwait(false); // Pass the logger
    }

}