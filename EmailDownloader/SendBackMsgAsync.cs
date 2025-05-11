using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using MailKit;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task<bool> SendBackMsgAsync(string emailId, Client clientDetails, string errtxt,
        CancellationToken cancellationToken = default) // Made async
    {
        try
        {
            if (clientDetails.Email == null) return false;
            uint? uID = null;
            using (var ctx = new CoreEntitiesContext()) // Assuming EF6 with async support
            {
                var emailEntity = await ctx.Emails
                    .FirstOrDefaultAsync(
                        x => x.EmailId == emailId && x.MachineName == Environment.MachineName /*, cancellationToken */)
                    .ConfigureAwait(false);
                if (emailEntity?.EmailUniqueId != null &&
                    uint.TryParse(emailEntity.EmailUniqueId.ToString(), out uint parsedUid))
                {
                    uID = parsedUid;
                }
            }

            if (uID == null) return false;

            MimeMessage msg;
            using (var imapClient = await GetImapClientAsync(clientDetails, cancellationToken).ConfigureAwait(false))
            {
                if (imapClient == null) return false;
                msg = await imapClient.Inbox.GetMessageAsync(new UniqueId(uID.Value), cancellationToken)
                    .ConfigureAwait(false);
            }

            if (msg != null)
            {
                await SendBackMsgAsync(msg, clientDetails, errtxt, cancellationToken)
                    .ConfigureAwait(false); // Call the MimeMessage overload
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Consider rethrowing or returning false based on policy
            return false;
        }
    }

    private static async Task SendBackMsgAsync(MimeMessage msg, Client clientDetails, string errtxt,
        CancellationToken cancellationToken = default) // Made async
    {
        if (clientDetails.Email == null) return;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress($"{clientDetails.CompanyName}-AutoBot", clientDetails.Email));
        if (!clientDetails.DevMode)
        {
            if (msg.From.Mailboxes.Any())
            {
                message.ReplyTo.Add(new MailboxAddress(msg.From.First().Name,
                    msg.From.Mailboxes.FirstOrDefault().Address));
                message.To.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
            }
        }

        message.Subject = "FWD: " + msg.Subject;
        var builder = new BodyBuilder { TextBody = errtxt };
        builder.Attachments.Add(new MessagePart { Message = msg });
        message.Body = builder.ToMessageBody();
        await SendEmailInternalAsync(clientDetails, message, cancellationToken)
            .ConfigureAwait(false); // Use internal async version
    }
}