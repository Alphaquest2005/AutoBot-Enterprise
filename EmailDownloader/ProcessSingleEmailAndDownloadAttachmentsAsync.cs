using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace EmailDownloader;

public static partial class EmailDownloader
{
    private static async Task<EmailProcessingResult> ProcessSingleEmailAndDownloadAttachmentsAsync(
        ImapClient imapClient,
        UniqueId uid,
        Client client,
        List<Emails> existingEmails, // Pass pre-fetched list
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var lst = new List<FileInfo>();
        MimeMessage msg;

        try
        {
            msg = await imapClient.Inbox.GetMessageAsync(uid, cancellationToken).ConfigureAwait(false);
        }
        catch (MessageNotFoundException)
        {
            Console.WriteLine($"Message UID {uid} not found. Marking seen.");
            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            return null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching UID {uid}: {ex.Message}. Marking seen.");
            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            return null;
        }


        if (ReturnOnlyUnknownMails)
        {
            if (existingEmails.Any(x => x.EmailDate == msg.Date.DateTime && x.Subject == msg.Subject))
            {
                // Optionally mark as seen if it was unseen but known
                // await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                return null; // Skip
            }
        }

        var emailsFound = client.EmailMappings // Use client.EmailMappings directly
            .Where(x => Regex.IsMatch(msg.Subject, x.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
            .OrderByDescending(x => x.Pattern.Length)
            .ToList();

        if (!emailsFound.Any())
        {
            if (client.NotifyUnknownMessages)
            {
                await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken)
                    .ConfigureAwait(false);
                var errTxt = "Hey,\r\n\r\n The System is not configured for this message.\r\n" +
                             "Check the Subject again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                             "Thanks\r\n" +
                             "Ez-Asycuda-Toolkit";
                await SendBackMsgAsync(msg, client, errTxt, cancellationToken)
                    .ConfigureAwait(false); // Make SendBackMsg async
            }

            return null; // Skip
        }

        // Taking first one because i think there should only be one real match but multiple matches possible
        var emailMapping = emailsFound.First();
        var subjectInfo = GetSubject(msg, uid, new List<EmailMapping>() { emailMapping });

        if (subjectInfo == null || string.IsNullOrEmpty(subjectInfo.Item1))
        {
            if (client.NotifyUnknownMessages)
            {
                await SendEmailAsync(client, null, $"Bug Found", GetContacts("Developer"),
                        $"Subject not configured for Regex: '{msg.Subject}'", Array.Empty<string>(), cancellationToken)
                    .ConfigureAwait(false); // Make SendEmail async
            }

            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            return null; // Skip
        }

        var desFolder = Path.Combine(client.DataFolder, subjectInfo.Item1, uid.ToString());
        if (Directory.Exists(desFolder))
            Directory.Delete(desFolder, true); // Sync, consider async if becomes bottleneck
        Directory.CreateDirectory(desFolder);

        foreach (var a in msg.Attachments.Where(x => x.ContentType.MediaType != "message"))
        {
            if (!a.IsAttachment) continue;
            await SaveAttachmentPartAsync(desFolder, a, lst, cancellationToken)
                .ConfigureAwait(false); // Make SaveAttachmentPart async
        }

        await SaveBodyPartAsync(desFolder, msg, lst, cancellationToken)
            .ConfigureAwait(false); // Make SaveBodyPart async

        var fileTypes = emailMapping.EmailFileTypes.Select(x => x.FileTypes)
            .Where(x => lst.Any(z => Regex.IsMatch(z.Name, x.FilePattern, RegexOptions.IgnoreCase)))
            .Where(x => x.FileImporterInfos != null)
            .ToList();

        if (lst.Any(x => x.Name != "Info.txt") && fileTypes.All(x => x.FileImporterInfos.EntryType == "Info"))
        {
            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            if (client.NotifyUnknownMessages)
            {
                var errTxt = "Hey,\r\n\r\n The System is not configured for none of the Attachments in this mail.\r\n" +
                             "Check the file Name of attachments again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                             "Thanks\r\n" +
                             "AutoBot";
                await SendBackMsgAsync(msg, client, errTxt, cancellationToken).ConfigureAwait(false);
            }

            return null; // Skip
        }

        if (!await CheckFileSizeLimitAsync(client, fileTypes, lst, msg, cancellationToken)
                .ConfigureAwait(false)) // Make CheckFileSizeLimit async
        {
            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            return null; // Skip
        }

        subjectInfo.Item2.FileTypes = fileTypes;
        await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);

        return new EmailProcessingResult(subjectInfo, lst);
    }
}