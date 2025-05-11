using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task SendEmailInternalAsync(Client clientDetails, MimeMessage message,
        CancellationToken cancellationToken = default) // Made async
    {
        try
        {
            if (clientDetails.Email == null) return;
            var mailSettings = GetSendMailSettings(clientDetails.Email);
            using (var smtpClient = new SmtpClient()) // Renamed variable to avoid conflict
            {
                await smtpClient
                    .ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken)
                    .ConfigureAwait(false);
                await smtpClient.AuthenticateAsync(clientDetails.Email, clientDetails.Password, cancellationToken)
                    .ConfigureAwait(false);
                await smtpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);
                await smtpClient.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}