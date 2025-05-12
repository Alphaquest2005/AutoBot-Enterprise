using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Utils; // For StringExtensions.CleanFileName if EmailDownloader.CleanFileName is not used
using CoreEntities.Business.Entities; // For Email, Emails, EmailMapping, FileTypes, Client etc.
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
// Ensure EmailDownloader.EmailProcessingResult is defined (as per previous answers)
// Ensure EmailDownloader.Client is defined (as per previous answers)
// Ensure EmailDownloader.MailSettings is defined (as per previous answers)


namespace EmailDownloader
{
    public static partial class EmailDownloader
    {
        // ... (Keep other static fields, GetContacts, ReturnOnlyUnknownMails, StreamEmailResultsAsync, etc.)
        // ... (Keep GetOpenImapClientAsync, SendEmailAsync, CreateMessage, GetRead/SendMailSettings, _emailSettings lists)
        // ... (Keep SaveAttachmentPartAsync, GetNextAvailableFileName, CleanFileName, SaveBodyPartAsync, WriteAllTextAsync)
        // ... (Keep GetSubject, CheckFileSizeLimitAsync, SendBackMsgAsync)

        public static async Task<EmailProcessingResult> ProcessSingleEmailAndDownloadAttachmentsAsync(
                ImapClient imapClient,
                UniqueId uid,
                Client clientConfig, // This is EmailDownloader.Client
                List<Emails> existingEmails, // This is List<CoreEntities.Business.Entities.Emails>
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"ProcessSingle: Entered for UID {uid}. IMAP Client IsConnected: {imapClient.IsConnected}, IsAuthenticated: {imapClient.IsAuthenticated}, Inbox IsOpen: {imapClient.Inbox?.IsOpen}");

            if (!imapClient.IsConnected || !imapClient.IsAuthenticated || (imapClient.Inbox == null || !imapClient.Inbox.IsOpen))
            {
                Console.WriteLine($"ProcessSingle: UID {uid} - IMAP client not in ready state. Connected: {imapClient.IsConnected}, Auth: {imapClient.IsAuthenticated}, Inbox Open: {imapClient.Inbox?.IsOpen}");
                return null;
            }

            var lst = new List<FileInfo>(); // This will hold FileInfo for attachments AND Info.txt
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
                Console.WriteLine($"ProcessSingle: UID {uid} - Error fetching message: {ex.GetType().Name} - {ex.Message}."); // Stack removed for brevity here
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

            if (ReturnOnlyUnknownMails && existingEmails.Any(x => x.EmailDate == msg.Date.DateTime && x.Subject == msg.Subject && x.ApplicationSettingsId == clientConfig.ApplicationSettingsId))
            {
                Console.WriteLine($"ProcessSingle: UID {uid} - Email subject '{msg.Subject}' dated '{msg.Date.DateTime}' already processed and ReturnOnlyUnknownMails is true. Skipping.");
                // Optionally mark as seen if it was unseen but already known by DB
                // await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                return null;
            }

            // Find the first matching EmailMapping (original code took the first one from an ordered list)
            var emailsFound = clientConfig.EmailMappings
                .Where(x => !string.IsNullOrEmpty(x.Pattern) && Regex.IsMatch(msg.Subject, x.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                .OrderByDescending(x => x.Pattern?.Length ?? 0) // Handle null Pattern defensively
                .ToList();

            if (!emailsFound.Any())
            {
                Console.WriteLine($"ProcessSingle: UID {uid} - No email mapping found for subject '{msg.Subject}'.");
                if (clientConfig.NotifyUnknownMessages)
                {
                    var errTxt = "Hey,\r\n\r\n The System is not configured for this message.\r\n" + // Using original error text
                                 "Check the Subject again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                                 "Thanks\r\nEz-Asycuda-Toolkit";
                    Console.WriteLine($"ProcessSingle: UID {uid} - NotifyUnknownMessages is true. Attempting to send notification.");
                    await SendBackMsgAsync(msg, clientConfig, errTxt, cancellationToken).ConfigureAwait(false);
                }
                if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                {
                    Console.WriteLine($"ProcessSingle: UID {uid} - Marking as seen (no mapping).");
                    await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                }
                return null;
            }

            var emailMapping = emailsFound.First(); // Taking the first one as per original logic's intent
            Console.WriteLine($"ProcessSingle: UID {uid} - Using EmailMapping Id: {emailMapping.Id}, Pattern: '{emailMapping.Pattern}' for subject '{msg.Subject}'.");

            var subjectInfoTuple = GetSubject(msg, uid, new List<EmailMapping>() { emailMapping }); // GetSubject is sync
            if (subjectInfoTuple == null || string.IsNullOrEmpty(subjectInfoTuple.Item1))
            {
                Console.WriteLine($"ProcessSingle: UID {uid} - GetSubject returned null or empty key for subject '{msg.Subject}'.");
                if (clientConfig.NotifyUnknownMessages) // As per original logic on empty subject
                {
                    await SendEmailAsync(clientConfig, null, $"Bug Found", GetContacts("Developer"), // SendEmailAsync exists
                        $"Subject not configured for Regex: '{msg.Subject}'", Array.Empty<string>(), cancellationToken).ConfigureAwait(false);
                }
                if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                {
                    await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                }
                return null;
            }

            // subjectInfoTuple.Item1 is the string key for the folder
            // subjectInfoTuple.Item2 is the CoreEntities.Business.Entities.Email object (pre-populated by GetSubject)
            // subjectInfoTuple.Item3 is the UID string

            var desFolder = Path.Combine(clientConfig.DataFolder, CleanFileName(subjectInfoTuple.Item1), uid.ToString()); // Used EmailDownloader.CleanFileName
            Console.WriteLine($"ProcessSingle: UID {uid} - Destination folder: '{desFolder}'");
            if (Directory.Exists(desFolder)) Directory.Delete(desFolder, true); // Sync delete
            Directory.CreateDirectory(desFolder);

            Console.WriteLine($"ProcessSingle: UID {uid} - Processing attachments. Total MimeEntities in msg.Attachments: {msg.Attachments.Count()}.");
            var filteredAttachments = msg.Attachments.Where(x => x.ContentType.MediaType != "message").ToList();
            Console.WriteLine($"ProcessSingle: UID {uid} - Found {filteredAttachments.Count} attachments after filtering 'message' MediaType.");
            foreach (var attachmentEntity in filteredAttachments)
            {
                cancellationToken.ThrowIfCancellationRequested();
                // SaveAttachmentPartAsync already checks if it's a MimePart and IsAttachment
                await SaveAttachmentPartAsync(desFolder, attachmentEntity, lst, cancellationToken).ConfigureAwait(false);
            }
            Console.WriteLine($"ProcessSingle: UID {uid} - Finished processing attachments. lst count: {lst.Count}");

            Console.WriteLine($"ProcessSingle: UID {uid} - Calling SaveBodyPartAsync. Current lst count: {lst.Count}");
            await SaveBodyPartAsync(desFolder, msg, lst, cancellationToken).ConfigureAwait(false);
            Console.WriteLine($"ProcessSingle: UID {uid} - After SaveBodyPartAsync. Current lst count: {lst.Count}. Files in lst: {string.Join(", ", lst.Select(f => f.Name))}");

            // --- This is the critical part for subjectInfoTuple.Item2.FileTypes ---
            // This logic comes directly from your original DownloadAttachment method.
            var relevantFileTypesForEmailEntity = emailMapping.EmailFileTypes // EmailFileTypes is List<CoreEntities.Business.Entities.EmailFileType>
                .Select(eft => eft.FileTypes) // eft.FileTypes is CoreEntities.Business.Entities.FileTypes
                .Where(coreBusFileType => coreBusFileType != null && // Ensure FileTypes object itself is not null
                                          lst.Any(downloadedFile => Regex.IsMatch(downloadedFile.Name, coreBusFileType.FilePattern ?? "", RegexOptions.IgnoreCase))) // Match downloaded files against pattern
                .Where(coreBusFileType => coreBusFileType.FileImporterInfos != null) // Original condition
                .ToList(); // Results in List<CoreEntities.Business.Entities.FileTypes>

            // The Email object created by GetSubject is subjectInfoTuple.Item2
           Email emailEntityForResult = subjectInfoTuple.Item2;
            emailEntityForResult.FileTypes = relevantFileTypesForEmailEntity; // Assign the filtered list
            Console.WriteLine($"ProcessSingle: UID {uid} - Assigned {emailEntityForResult.FileTypes.Count} relevant FileTypes to Email object. Patterns: {string.Join(", ", emailEntityForResult.FileTypes.Select(ft => ft.FilePattern))}");


            // Check file size limits (this was in your original DownloadAttachment)
            if (!await CheckFileSizeLimitAsync(clientConfig, relevantFileTypesForEmailEntity, lst, msg, cancellationToken).ConfigureAwait(false))
            {
                Console.WriteLine($"ProcessSingle: UID {uid} - File size limit check failed. Marking as seen and returning null.");
                if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                {
                    await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                }
                return null;
            }

            // Check for unconfigured attachments (this was in your original DownloadAttachment)
            // If there are downloaded files (other than Info.txt) but NONE of them match any configured FileTypes that have FileImporterInfos
            if (lst.Any(x => x.Name != "Info.txt") &&
                relevantFileTypesForEmailEntity.All(configuredFt => configuredFt.FileImporterInfos.EntryType == "Info")) // This condition might need refinement based on original intent
            {
                // The original code checked fileTypes (which was relevantFileTypesForEmailEntity) against FileImporterInfos.EntryType == "Info"
                // And if sendNotifications was true, it sent a message.
                // This implies: if we have attachments but they only map to "Info" type importers, it's an issue.
                Console.WriteLine($"ProcessSingle: UID {uid} - Attachments present but seem unconfigured (only map to 'Info' importers).");
                if (clientConfig.NotifyUnknownMessages)
                {
                    var errTxt = "Hey,\r\n\r\n The System is not configured for none of the Attachments in this mail.\r\n" + // Original text
                                 "Check the file Name of attachments again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                                 "Thanks\r\nAutoBot";
                    await SendBackMsgAsync(msg, clientConfig, errTxt, cancellationToken).ConfigureAwait(false);
                }
                if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                {
                    await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                }
                return null;
            }


            // Mark as seen after successful processing of everything
            if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
            {
                Console.WriteLine($"ProcessSingle: UID {uid} - Successfully processed all checks. Marking as seen.");
                await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
            }

            Console.WriteLine($"ProcessSingle: UID {uid} - Successfully processed. Returning result for subject '{emailEntityForResult.Subject}'.");
            return new EmailProcessingResult(
                (SubjectIdentifier: subjectInfoTuple.Item1, EmailMessage: emailEntityForResult, UidString: subjectInfoTuple.Item3),
                lst // lst contains FileInfo for all saved parts (attachments + Info.txt)
            );
        }

        // Ensure ALL other helper methods are present and correct:
        // GetContacts, GetReadMailSettings, GetSendMailSettings, _sendEmailSettings, _readEmailSettings, MailSettings class
        // Client class, EmailProcessingResult class
        // SendEmailAsync (multiple overloads), CreateMessage
        // SendBackMsgAsync (multiple overloads)
        // GetSubject
        // CleanFileName (ensure this is the robust one)
        // GetNextAvailableFileName
        // SaveAttachmentPartAsync
        // SaveBodyPartAsync
        // WriteAllTextAsync
        // CheckFileSizeLimitAsync
    }
}