using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task<bool> ForwardMsgToSenderAsync(int uID, Client client, string subject, string body,
        string[] attachments, CancellationToken cancellationToken = default) // Made async
    {
        try
        {
            if (client.Email == null) return false;
            if (uID <= 0) return false; // Basic check for valid UID
            var msg = await GetMsgAsync((uint)uID, client, cancellationToken).ConfigureAwait(false);
            if (msg != null && msg.MessageId != null) // Check if message was actually found
            {
                var contacts = msg.From.Mailboxes.Select(x => x.Address).ToArray();
                if (contacts.Any())
                {
                    await ForwardMsgInternalAsync(msg, client, subject, body, contacts, attachments, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}