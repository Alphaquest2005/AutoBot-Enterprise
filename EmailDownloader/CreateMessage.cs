using System.IO;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static MimeMessage CreateMessage(Client client, string subject, string[] to, string body,
        string[] attachments)
    {
        // Original CreateMessage logic - remains synchronous
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress($"{client.CompanyName}-AutoBot", client.Email));
        foreach (var recipent in to)
        {
            message.To.Add(MailboxAddress.Parse(recipent));
        }

        message.Subject = subject;
        var builder = new BodyBuilder { TextBody = body };
        foreach (var attachment in attachments)
        {
            try
            {
                if (File.Exists(attachment)) builder.Attachments.Add(attachment);
            }
            catch
            {
                /* ignore */
            }
        }

        message.Body = builder.ToMessageBody();
        return message;
    }
}