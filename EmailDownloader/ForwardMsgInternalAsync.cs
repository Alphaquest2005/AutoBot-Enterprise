using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task ForwardMsgInternalAsync(MimeMessage msg, Client clientDetails, string subject,
        string body,
        string[] contacts, string[] attachments, CancellationToken cancellationToken) // Made async
    {
        if (clientDetails.Email == null) return;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
        if (!clientDetails.DevMode)
        {
            if (msg.From.Mailboxes.Any())
            {
                message.ReplyTo.Add(new MailboxAddress(msg.From?.FirstOrDefault()?.Name ?? "No Sender Found",
                    msg.From?.Mailboxes?.FirstOrDefault()?.Address ?? GetContacts("Developer").FirstOrDefault()));
            }

            foreach (var recipent in contacts.Distinct())
            {
                message.To.Add(MailboxAddress.Parse(recipent));
            }
        }

        message.Subject = subject;
        var builder = new BodyBuilder { TextBody = body };
        if (msg.MessageId != null)
            builder.Attachments.Add(new MessagePart { Message = msg }); // Check if msg is not a new/empty one
        foreach (var attachment in attachments)
        {
            if (File.Exists(attachment)) builder.Attachments.Add(attachment);
        }

        message.Body = builder.ToMessageBody();
        await SendEmailInternalAsync(clientDetails, message, cancellationToken).ConfigureAwait(false);
    }
}