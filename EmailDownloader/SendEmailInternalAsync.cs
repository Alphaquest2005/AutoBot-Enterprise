using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static  Task SendEmailInternalAsync(Client clientDetails, MimeMessage message,
        CancellationToken cancellationToken = default) // Made async
    {
        try
        {
            if (clientDetails.Email == null) return null;
            var mailSettings = GetSendMailSettings(clientDetails.Email);
            using (var smtpClient = new SmtpClient()) // Renamed variable to avoid conflict
            {
                smtpClient.Connect(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken);

                smtpClient.Authenticate(clientDetails.Email, clientDetails.Password, cancellationToken);
                smtpClient.Send(message, cancellationToken);
                smtpClient.Disconnect(true, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return Task.CompletedTask;
    }
}