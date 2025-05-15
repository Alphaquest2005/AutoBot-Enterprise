using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;

namespace EmailDownloader;

using Serilog;

public static partial class EmailDownloader
{
    // GetMessageOrDefaultAsync was an internal helper, GetMsgAsync now encapsulates its logic
    // If GetMessageOrDefaultAsync was used elsewhere, it would also need this short-lived client pattern or accept a client.

    public static async Task<bool> ForwardMsgAsync(string emailId, Client clientDetails, string subject, string body, string[] contacts, string[] attachments, ILogger log, CancellationToken cancellationToken = default)
    {
        try
        {
            if (clientDetails.Email == null) return false;
            uint? uIDNumeric = null; // Changed from int to uint? to match GetMsgAsync
            using (var dbCtx = new CoreEntitiesContext())
            {
                // Assuming EmailUniqueId in DB is stored in a way that can be parsed to uint
                var emailEntity = await dbCtx.Emails
                    .FirstOrDefaultAsync(x => x.EmailId == emailId && x.MachineName == Environment.MachineName)
                    .ConfigureAwait(false);
                if (emailEntity?.EmailUniqueId != null && uint.TryParse(emailEntity.EmailUniqueId.ToString(), out uint parsedUid))
                {
                    uIDNumeric = parsedUid;
                }
            }

            if (uIDNumeric == null || uIDNumeric.Value == 0)
            {
                // Email not found in DB or UID is invalid, send a new email
                await SendEmailAsync(clientDetails, null, subject, contacts, body, attachments,log,  cancellationToken).ConfigureAwait(false);
                return true;
            }

            // uIDNumeric.Value is now uint
            MimeMessage originalMsg = await GetMsgAsync(uIDNumeric.Value, clientDetails,log, cancellationToken).ConfigureAwait(false);

            if (originalMsg == null)
            {
                Console.WriteLine($"ForwardMsgAsync: Original message for EmailId '{emailId}' (UID {uIDNumeric.Value}) not found. Sending as new email.");
                // Optionally, send as a new email if the original can't be fetched
                await SendEmailAsync(clientDetails, null, subject, contacts, body, attachments,log, cancellationToken).ConfigureAwait(false);
                return false; // Or true depending on desired behavior
            }

            await ForwardMsgInternalAsync(originalMsg, clientDetails, subject, body, contacts, attachments, cancellationToken, log).ConfigureAwait(false); // Pass the logger
            return true;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ForwardMsgAsync: {e}");
            // Consider rethrowing or specific error handling
            throw; // Or return false
        }
    }


    /// <summary>
    /// Fetches a specific MimeMessage by its UID using a new, short-lived ImapClient connection.
    /// </summary>
    private static async Task<MimeMessage> GetMsgAsync(uint uID, Client clientDetails, ILogger logger, CancellationToken cancellationToken = default)
    {
        // Create a new client instance for this specific operation
        ImapClient imapClient = null;
        try
        {
            // GetOpenImapClientAsync connects, authenticates, and opens Inbox
            imapClient = await GetOpenImapClientAsync(clientDetails, logger, cancellationToken).ConfigureAwait(false);
            if (imapClient == null)
            {
                Console.WriteLine($"GetMsgAsync: Failed to get open IMAP client for UID {uID}.");
                return null; // Or throw an exception
            }

            return await imapClient.Inbox.GetMessageAsync(new UniqueId(uID), cancellationToken).ConfigureAwait(false);
        }
        catch (MessageNotFoundException)
        {
            Console.WriteLine($"GetMsgAsync: Message with UID {uID} not found.");
            return null;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            Console.WriteLine($"GetMsgAsync: Error fetching message UID {uID}: {ex}");
            return null; // Or throw
        }
        finally
        {
            if (imapClient != null)
            {
                if (imapClient.IsConnected)
                {
                    await imapClient.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false); // Use CancellationToken.None for cleanup
                }
                imapClient.Dispose();
            }
        }
    }
}