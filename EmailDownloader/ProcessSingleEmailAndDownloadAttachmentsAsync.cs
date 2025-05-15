using Serilog;
using Serilog.Context;
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
                ILogger logger, // Added ILogger parameter
                CancellationToken cancellationToken)
        {
            string methodName = nameof(ProcessSingleEmailAndDownloadAttachmentsAsync);
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
                methodName, new { Uid = uid, ClientEmail = clientConfig.Email });

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Entered for UID {Uid}. IMAP Client IsConnected: {IsConnected}, IsAuthenticated: {IsAuthenticated}, Inbox IsOpen: {InboxIsOpen}",
                    methodName, "EntryCheck", uid, imapClient.IsConnected, imapClient.IsAuthenticated, imapClient.Inbox?.IsOpen);

                if (!imapClient.IsConnected || !imapClient.IsAuthenticated || (imapClient.Inbox == null || !imapClient.Inbox.IsOpen))
                {
                    logger.Warning("METHOD_EXIT_FAILURE: {MethodName}. Reason: IMAP client not in ready state. Connected: {Connected}, Auth: {Authenticated}, Inbox Open: {InboxOpen}",
                        methodName, imapClient.IsConnected, imapClient.IsAuthenticated, imapClient.Inbox?.IsOpen);
                    return null;
                }

                var lst = new List<FileInfo>(); // This will hold FileInfo for attachments AND Info.txt
                MimeMessage msg;

                try
                {
                    logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        $"IMAP Inbox GetMessageAsync for UID {uid}", "ASYNC_EXPECTED");
                    msg = await imapClient.Inbox.GetMessageAsync(uid, cancellationToken).ConfigureAwait(false);
                    logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        $"IMAP Inbox GetMessageAsync for UID {uid}", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                    logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Successfully got message. Subject: {Subject}",
                        methodName, "GetMessage", msg.Subject);
                }
                catch (ServiceNotConnectedException snce)
                {
                    logger.Error(snce, "INTERNAL_STEP ({MethodName} - {Stage}): ServiceNotConnectedException: {ErrorMessage}. IMAP State: Connected={Connected}, Auth={Authenticated}",
                        methodName, "GetMessageError", snce.Message, imapClient.IsConnected, imapClient.IsAuthenticated);
                    throw;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        methodName, 0, ex.Message); // Placeholder for duration
                    if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                    {
                        try
                        {
                            logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Marking as seen due to fetch error.",
                                methodName, "MarkSeenOnError");
                            await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception flagEx)
                        {
                            logger.Error(flagEx, "INTERNAL_STEP ({MethodName} - {Stage}): Error marking as seen after fetch error: {ErrorMessage}",
                                methodName, "MarkSeenOnError", flagEx.Message);
                        }
                    }
                    return null;
                }

                if (ReturnOnlyUnknownMails && existingEmails.Any(x => x.EmailDate == msg.Date.DateTime && x.Subject == msg.Subject && x.ApplicationSettingsId == clientConfig.ApplicationSettingsId))
                {
                    logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Email subject '{Subject}' dated '{Date}' already processed and ReturnOnlyUnknownMails is true. Skipping.",
                        methodName, "CheckExisting", msg.Subject, msg.Date.DateTime);
                    // Optionally mark as seen if it was unseen but already known by DB
                    // await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Reason: Already processed.",
                        methodName, 0); // Placeholder for duration
                    return null;
                }

                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Finding email mappings for subject '{Subject}'.",
                    methodName, "FindMapping", msg.Subject);
                // Find the first matching EmailMapping (original code took the first one from an ordered list)
                var emailsFound = clientConfig.EmailMappings
                    .Where(x => !string.IsNullOrEmpty(x.Pattern) && Regex.IsMatch(msg.Subject, x.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                    .OrderByDescending(x => x.Pattern?.Length ?? 0) // Handle null Pattern defensively
                    .ToList();
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found {MappingCount} potential email mappings.",
                    methodName, "FindMapping", emailsFound.Count);


                if (!emailsFound.Any())
                {
                    logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No email mapping found for subject '{Subject}'.",
                        methodName, "NoMapping", msg.Subject);
                    if (clientConfig.NotifyUnknownMessages)
                    {
                        var errTxt = "Hey,\r\n\r\n The System is not configured for this message.\r\n" + // Using original error text
                                     "Check the Subject again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                                     "Thanks\r\nEz-Asycuda-Toolkit";
                        logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            "SendBackMsgAsync (NotifyUnknownMessages)", "ASYNC_EXPECTED");
                        await SendBackMsgAsync(msg, clientConfig, errTxt, logger, cancellationToken).ConfigureAwait(false); // Pass the logger
                        logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            "SendBackMsgAsync (NotifyUnknownMessages)", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                    }
                    if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                    {
                        logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Marking as seen (no mapping).",
                            methodName, "MarkSeenNoMapping");
                        await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                    }
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Reason: No mapping found.",
                        methodName, 0); // Placeholder for duration
                    return null;
                }

                var emailMapping = emailsFound.First(); // Taking the first one as per original logic's intent
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Using EmailMapping Id: {MappingId}, Pattern: '{Pattern}' for subject '{Subject}'.",
                    methodName, "UsingMapping", emailMapping.Id, emailMapping.Pattern, msg.Subject);

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    "GetSubject", "SYNC_EXPECTED");
                var subjectInfoTuple = GetSubject(msg, uid, new List<EmailMapping>() { emailMapping }, logger); // GetSubject is sync, Pass the logger
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "GetSubject", 0, "Sync call returned"); // Placeholder for duration

                if (subjectInfoTuple == null || string.IsNullOrEmpty(subjectInfoTuple.Item1))
                {
                    logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): GetSubject returned null or empty key for subject '{Subject}'.",
                        methodName, "GetSubjectError", msg.Subject);
                    if (clientConfig.NotifyUnknownMessages) // As per original logic on empty subject
                    {
                        logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            "SendEmailAsync (Bug Found)", "ASYNC_EXPECTED");
                        await SendEmailAsync(clientConfig, null, $"Bug Found", GetContacts("Developer", logger), // Pass the logger to GetContacts
                            $"Subject not configured for Regex: '{msg.Subject}'", Array.Empty<string>(), logger, cancellationToken).ConfigureAwait(false); // Pass the logger
                        logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            "SendEmailAsync (Bug Found)", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                    }
                    if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                    {
                        logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Marking as seen (GetSubject error).",
                            methodName, "MarkSeenGetSubjectError");
                        await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                    }
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Reason: GetSubject error.",
                        methodName, 0); // Placeholder for duration
                    return null;
                }

                // subjectInfoTuple.Item1 is the string key for the folder
                // subjectInfoTuple.Item2 is the CoreEntities.Business.Entities.Email object (pre-populated by GetSubject)
                // subjectInfoTuple.Item3 is the UID string

                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Determining destination folder.",
                    methodName, "DestinationFolder");
                var desFolder = Path.Combine(clientConfig.DataFolder, CleanFileName(subjectInfoTuple.Item1, logger), uid.ToString()); // Used EmailDownloader.CleanFileName, Pass the logger
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Destination folder: '{DestinationFolder}'",
                    methodName, "DestinationFolder", desFolder);

                if (Directory.Exists(desFolder))
                {
                    logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Destination folder already exists, deleting: '{DestinationFolder}'",
                        methodName, "DeleteExistingFolder", desFolder);
                    Directory.Delete(desFolder, true); // Sync delete
                }
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Creating destination directory: '{DestinationFolder}'",
                    methodName, "CreateDestinationFolder", desFolder);
                Directory.CreateDirectory(desFolder);

                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Processing attachments. Total MimeEntities in msg.Attachments: {AttachmentCount}.",
                    methodName, "ProcessAttachments", msg.Attachments.Count());
                var filteredAttachments = msg.Attachments.Where(x => x.ContentType.MediaType != "message").ToList();
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found {FilteredAttachmentCount} attachments after filtering 'message' MediaType.",
                    methodName, "ProcessAttachments", filteredAttachments.Count);

                foreach (var attachmentEntity in filteredAttachments)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Saving attachment: {AttachmentFileName}",
                        methodName, "SaveAttachment", attachmentEntity.ContentDisposition?.FileName ?? attachmentEntity.ContentType?.Name ?? "Unknown");
                    // SaveAttachmentPartAsync already checks if it's a MimePart and IsAttachment
                    await SaveAttachmentPartAsync(desFolder, attachmentEntity, lst, logger, cancellationToken).ConfigureAwait(false); // Pass the logger
                }
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Finished processing attachments. Saved {SavedAttachmentCount} files.",
                    methodName, "ProcessAttachments", lst.Count);

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    "SaveBodyPartAsync", "ASYNC_EXPECTED");
                await SaveBodyPartAsync(desFolder, msg, lst, logger, cancellationToken).ConfigureAwait(false); // Pass the logger
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "SaveBodyPartAsync", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): After SaveBodyPartAsync. Current lst count: {LstCount}. Files in lst: {Files}",
                    methodName, "SaveBody", lst.Count, string.Join(", ", lst.Select(f => f.Name)));


                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Determining relevant FileTypes for Email object.",
                    methodName, "DetermineFileTypes");
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
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Assigned {FileTypeCount} relevant FileTypes to Email object. Patterns: {Patterns}",
                    methodName, "DetermineFileTypes", emailEntityForResult.FileTypes.Count, string.Join(", ", emailEntityForResult.FileTypes.Select(ft => ft.FilePattern)));


                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    "CheckFileSizeLimitAsync", "ASYNC_EXPECTED");
                // Check file size limits (this was in your original DownloadAttachment)
                if (!await CheckFileSizeLimitAsync(clientConfig, relevantFileTypesForEmailEntity, lst, msg, logger, cancellationToken).ConfigureAwait(false)) // Pass the logger
                {
                    logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): File size limit check failed. Marking as seen and returning null.",
                        methodName, "FileSizeLimit");
                    logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "CheckFileSizeLimitAsync", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                    if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                    {
                        logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Marking as seen (file size limit).",
                            methodName, "MarkSeenFileSizeLimit");
                        await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                    }
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Reason: File size limit exceeded.",
                        methodName, 0); // Placeholder for duration
                    return null;
                }
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "CheckFileSizeLimitAsync", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration


                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for unconfigured attachments.",
                    methodName, "CheckUnconfiguredAttachments");
                // Check for unconfigured attachments (this was in your original DownloadAttachment)
                // If there are downloaded files (other than Info.txt) but NONE of them match any configured FileTypes that have FileImporterInfos
                if (lst.Any(x => x.Name != "Info.txt") &&
                    relevantFileTypesForEmailEntity.All(configuredFt => configuredFt.FileImporterInfos.EntryType == "Info")) // This condition might need refinement based on original intent
                {
                    // The original code checked fileTypes (which was relevantFileTypesForEmailEntity) against FileImporterInfos.EntryType == "Info"
                    // And if sendNotifications was true, it sent a message.
                    // This implies: if we have attachments but they only map to "Info" type importers, it's an issue.
                    logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Attachments present but seem unconfigured (only map to 'Info' importers).",
                        methodName, "UnconfiguredAttachments");
                    if (clientConfig.NotifyUnknownMessages)
                    {
                        var errTxt = "Hey,\r\n\r\n The System is not configured for none of the Attachments in this mail.\r\n" + // Original text
                                     "Check the file Name of attachments again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                                     "Thanks\r\nAutoBot";
                        logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                            "SendBackMsgAsync (Unconfigured Attachments)", "ASYNC_EXPECTED");
                        await SendBackMsgAsync(msg, clientConfig, errTxt, logger, cancellationToken).ConfigureAwait(false); // Pass the logger
                        logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call/await took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            "SendBackMsgAsync (Unconfigured Attachments)", 0, "If ASYNC_EXPECTED, this is pre-await return"); // Placeholder for duration
                    }
                    if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                    {
                        logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Marking as seen (unconfigured attachments).",
                            methodName, "MarkSeenUnconfigured");
                        await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                    }
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Reason: Unconfigured attachments.",
                        methodName, 0); // Placeholder for duration
                    return null;
                }


                // Mark as seen after successful processing of everything
                if (imapClient.IsConnected && imapClient.Inbox != null && imapClient.Inbox.IsOpen)
                {
                    logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Successfully processed all checks. Marking as seen.",
                        methodName, "MarkSeenSuccess");
                    await imapClient.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken).ConfigureAwait(false);
                }

                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Successfully processed email.",
                    methodName, 0); // Placeholder for duration
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Returning result for subject '{Subject}'.",
                    methodName, "ReturnResult", emailEntityForResult.Subject);
                return new EmailProcessingResult(
                    (SubjectIdentifier: subjectInfoTuple.Item1, EmailMessage: emailEntityForResult, UidString: subjectInfoTuple.Item3),
                    lst // lst contains FileInfo for all saved parts (attachments + Info.txt)
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, 0, ex.Message); // Placeholder for duration
                throw; // Re-throw the exception after logging
            }
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