using MimeKit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task<bool> ForwardMsgToSenderAsync(int uIDAsInt, Client client, string subject, string body, string[] attachments, CancellationToken cancellationToken = default)
    {
        try
        {
            if (client.Email == null) return false;
            if (uIDAsInt <= 0)
            {
                Console.WriteLine($"ForwardMsgToSenderAsync: Invalid UID {uIDAsInt}.");
                return false;
            }
            uint uID = (uint)uIDAsInt; // Cast to uint for GetMsgAsync

            MimeMessage originalMsg = await GetMsgAsync(uID, client, cancellationToken).ConfigureAwait(false);

            if (originalMsg != null && originalMsg.MessageId != null) // Check if message was actually found and is not empty
            {
                var senderContacts = originalMsg.From.Mailboxes.Select(x => x.Address).ToArray();
                if (senderContacts.Any())
                {
                    await ForwardMsgInternalAsync(originalMsg, client, subject, body, senderContacts, attachments, cancellationToken).ConfigureAwait(false);
                    return true;
                }
                else
                {
                    Console.WriteLine($"ForwardMsgToSenderAsync: Message UID {uID} has no sender address.");
                    // Optionally, forward to an admin or log more verbosely
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"ForwardMsgToSenderAsync: Message UID {uID} not found or is empty.");
                return false;
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ForwardMsgToSenderAsync: {e}");
            // throw; // Or return false
            return false;
        }
    }

    // Ensure other methods like ProcessSingleEmailAndDownloadAttachmentsAsync, MailSettings, SendEmailAsync (all overloads), CreateMessage
    // are present as per the previous complete EmailDownloader.cs answer.
}