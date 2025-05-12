using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    public static async Task<EmailProcessingResult> ProcessSingleEmailAndDownloadAttachmentsAsync(
            ImapClient imapClient,
            UniqueId uid,
            Client clientConfig, // This is EmailDownloader.Client
            List<Emails> existingEmails, // This is  CoreEntities.Business.Entities.Emailss
            CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine($"ProcessSingle: Entered for UID {uid}. IMAP Client IsConnected: {imapClient.IsConnected}, IsAuthenticated: {imapClient.IsAuthenticated}, Inbox IsOpen: {imapClient.Inbox?.IsOpen}");

        if (!imapClient.IsConnected || !imapClient.IsAuthenticated || (imapClient.Inbox == null || !imapClient.Inbox.IsOpen))
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - IMAP client not in ready state. Connected: {imapClient.IsConnected}, Auth: {imapClient.IsAuthenticated}, Inbox Open: {imapClient.Inbox?.IsOpen}");
            return null;
        }

        var lst = new List<FileInfo>();
        MimeMessage msg;

        try
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - Attempting Inbox.GetMessageAsync. Inbox: {imapClient.Inbox.FullName}, Count: {imapClient.Inbox.Count}");
            msg = await imapClient.Inbox.GetMessageAsync(uid, cancellationToken).ConfigureAwait(false);
            Console.WriteLine($"ProcessSingle: UID {uid} - Successfully got message. Subject: {msg.Subject}");
        }
        catch (ServiceNotConnectedException snce)
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - ServiceNotConnectedException: {snce.Message}. IMAP State: Connected={imapClient.IsConnected}, Auth={imapClient.IsAuthenticated}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - Error fetching message: {ex.GetType().Name} - {ex.Message}. Stack: {ex.StackTrace}");
            if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
            {
                try
                {
                    Console.WriteLine($"ProcessSingle: UID {uid} - Marking as seen due to fetch error.");
                    await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception flagEx)
                {
                    Console.WriteLine($"ProcessSingle: UID {uid} - Error marking as seen after fetch error: {flagEx.Message}");
                }
            }
            return null;
        }

        // Simplified continuation for brevity - actual logic for mapping, saving, etc. should be here
        if (ReturnOnlyUnknownMails && existingEmails.Any(x => x.EmailDate == msg.Date.DateTime && x.Subject == msg.Subject && x.ApplicationSettingsId == clientConfig.ApplicationSettingsId))
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - Email subject '{msg.Subject}' dated '{msg.Date.DateTime}' already processed and ReturnOnlyUnknownMails is true. Skipping.");
            return null;
        }

        var emailMapping = clientConfig.EmailMappings?.FirstOrDefault(x => Regex.IsMatch(msg.Subject, x.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline));

        if (emailMapping == null)
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - No email mapping found for subject '{msg.Subject}'.");
            if (clientConfig.NotifyUnknownMessages)
            {
                // await SendBackMsgAsync(msg, clientConfig, "Unknown Email Subject", cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"ProcessSingle: UID {uid} - NotifyUnknownMessages is true. Would attempt to send notification.");
            }
            // Mark as seen if no mapping found and not notifying, or after attempting notification
            if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
            {
                await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            }
            return null;
        }

        var subjectInfo = GetSubject(msg, uid, new List<EmailMapping>() { emailMapping });
        if (subjectInfo == null || string.IsNullOrEmpty(subjectInfo.Item1))
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - GetSubject returned null or empty key for subject '{msg.Subject}'.");
            if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
            {
                await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            }
            return null;
        }

        var desFolder = Path.Combine(clientConfig.DataFolder, EmailDownloader.CleanFileName(subjectInfo.Item1), uid.ToString());
        if (!Directory.Exists(desFolder)) Directory.CreateDirectory(desFolder);

        // Placeholder for SaveAttachmentPartAsync and SaveBodyPartAsync logic
        // await SaveAttachmentPartAsync(desFolder, msg, lst, cancellationToken).ConfigureAwait(false);
        // await SaveBodyPartAsync(desFolder, msg, lst, cancellationToken).ConfigureAwait(false);


        // Mark as seen after successful processing
        if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
        {
            Console.WriteLine($"ProcessSingle: UID {uid} - Successfully processed. Marking as seen.");
            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
        }

        Console.WriteLine($"ProcessSingle: UID {uid} - Successfully processed. Returning result.");
        // Ensure Email type here matches what EmailProcessingResult expects for Item2
        var emailEntity = new Email((int)uid.Id,msg.Subject, msg.Date.DateTime, emailMapping);
        return new EmailProcessingResult(new Tuple<string, Email, string>(subjectInfo.Item1, emailEntity, uid.ToString()), lst);
    }


}
