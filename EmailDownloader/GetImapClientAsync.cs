using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task<ImapClient> GetImapClientAsync(Client client, CancellationToken cancellationToken)
    {
        try
        {
            var imapClient = new ImapClient();
            var mailSettings = GetReadMailSettings(client.Email);
            await imapClient
                .ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken)
                .ConfigureAwait(false);
            await imapClient.AuthenticateAsync(client.Email, client.Password, cancellationToken).ConfigureAwait(false);
            await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
            return imapClient;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetImapClientAsync Error for {client.Email}: {e}");
            return null;
        }
    }
}