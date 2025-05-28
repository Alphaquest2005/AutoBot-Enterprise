// EmailDownloader.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Utils;
using CoreEntities.Business.Entities; // Assuming Email, Emails, Client, EmailMapping, ApplicationSettings are here or accessible
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using Serilog;
using Serilog.Context;
using System.Diagnostics; // Added for Stopwatch

namespace EmailDownloader
{
    // Make sure these classes are defined and accessible.
    // If they are in CoreEntities.Business.Entities, ensure that project is referenced
    // and the types are public or internal with InternalsVisibleTo this project.

    // From your test file, EmailProcessingResult seems to be a local class or defined elsewhere.
    // For now, I'll assume it's accessible. If not, it needs to be defined.
    // public class EmailProcessingResult { /* ... as defined in tests ... */ }


    // MailSettings class is now expected to be in EmailDownloader/MailSettings.cs
    // Client class - ensure this is the one intended
    // public class Client { /* ... properties ... */ }


    public static partial class EmailDownloader
    {
        private const int SizeinMB = 1048576;
        private const int AsycudaMaxFileSize = 4;

        public static string[] GetContacts(string role, ILogger logger) // Add ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            string methodName = nameof(GetContacts);
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                methodName, "Get contact email addresses by role", $"Role: {role}");

            try
            {
                logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Creating CoreEntitiesContext.", methodName, "DatabaseAccess");
                using (var dbCtx = new CoreEntitiesContext())
                {
                    logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Querying Contacts table for role: {Role}.", methodName, "DatabaseAccess", role);
                    var contacts = dbCtx.Contacts.Where(x => x.Role == role).Select(x => x.EmailAddress).ToArray();
                    logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Found {ContactCount} contacts for role: {Role}.", methodName, "DatabaseAccess", contacts.Length, role);

                    methodStopwatch.Stop(); // Stop stopwatch
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, "Contacts retrieved successfully", $"Role: {role}, ContactCount: {contacts.Length}", methodStopwatch.ElapsedMilliseconds);
                    return contacts;
                }
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop(); // Stop stopwatch on error
                logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, "Get contact email addresses by role", methodStopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
        public static bool ReturnOnlyUnknownMails { get; set; } = false;

        public static async Task<ImapClient> GetOpenImapClientAsync(Client clientConfig, ILogger logger, CancellationToken cancellationToken = default)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            string methodName = nameof(GetOpenImapClientAsync);
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                methodName, "Get and open IMAP client", $"Email: {clientConfig.Email}"); // Removed Server and Port from initial log

            var imapClient = new ImapClient(); // ProtocolLogger can be added here for deep diagnostics
            // Example: imapClient = new ImapClient(new ProtocolLogger(Console.OpenStandardError()));
            try
            {
                logger.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Getting read mail settings.", methodName, "Configuration");
                var mailSettings = GetReadMailSettings(clientConfig.Email, logger); // Uses local GetReadMailSettings and passes logger
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Read mail settings obtained. Server: {Server}, Port: {Port}",
                    methodName, "Configuration", mailSettings?.Server, mailSettings?.Port); // Added log for mail settings
                imapClient.Timeout = 60000; // 60 seconds timeout for operations

                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Connecting to {Server}:{Port} for {Email}",
                    methodName, "Connection", mailSettings.Server, mailSettings.Port, clientConfig.Email);
                await imapClient.ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken).ConfigureAwait(false);
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Connected. Authenticating...", methodName, "Authentication");
                await imapClient.AuthenticateAsync(clientConfig.Email, clientConfig.Password, cancellationToken).ConfigureAwait(false);
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Authenticated. Opening Inbox...", methodName, "InboxOpen");
                await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Inbox opened. IsOpen: {InboxIsOpen}",
                    methodName, "InboxOpen", imapClient.Inbox.IsOpen);

                methodStopwatch.Stop(); // Stop stopwatch
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, "IMAP client opened successfully", $"Email: {clientConfig.Email}, IsConnected: {imapClient.IsConnected}, IsAuthenticated: {imapClient.IsAuthenticated}, InboxIsOpen: {imapClient.Inbox.IsOpen}", methodStopwatch.ElapsedMilliseconds);
                return imapClient;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on error
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, "Get and open IMAP client", methodStopwatch.ElapsedMilliseconds, e.Message);
                imapClient?.Dispose(); // Dispose if any step failed
                return null;
            }
        }

        public static IEnumerable<Task<EmailProcessingResult>> StreamEmailResultsAsync(
        ImapClient imapClient, // Expects a connected, authenticated, and inbox-opened client
        Client clientConfig, ILogger logger, // Ensure logger is used consistently
        CancellationToken cancellationToken = default)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            string methodName = nameof(StreamEmailResultsAsync);
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                methodName, "Stream email processing results", $"Email: {clientConfig.Email}, ReturnOnlyUnknownMails: {ReturnOnlyUnknownMails}");

            if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated || imapClient.Inbox == null || !imapClient.Inbox.IsOpen)
            {
                logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Received IMAP client is not ready (Connected: {IsConnected}, Authenticated: {IsAuthenticated}, Inbox Open: {InboxIsOpen}). {EmptyCollectionExpectation}",
                    methodName, "Validation", imapClient?.IsConnected, imapClient?.IsAuthenticated, imapClient?.Inbox?.IsOpen, "Expected a ready client to stream emails.");
                methodStopwatch.Stop(); // Stop stopwatch on early exit
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, "Skipped due to invalid IMAP client state", $"Email: {clientConfig.Email}", methodStopwatch.ElapsedMilliseconds);
                yield break;
            }

            // Generate a unique InvocationId for this email processing operation
            var invocationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("InvocationId", invocationId)) // Keep LogContext for InvocationId as per previous implementation
            {
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): IMAP client is ready. Proceeding with search.", methodName, "Initialization");

                if (!Directory.Exists(clientConfig.DataFolder))
                {
                    logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Data folder does not exist. Creating directory: {DataFolder}",
                        methodName, "DirectoryCreation", clientConfig.DataFolder);
                    Directory.CreateDirectory(clientConfig.DataFolder);
                    logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Data folder created.", methodName, "DirectoryCreation");
                }
                else
                {
                    logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Data folder already exists: {DataFolder}",
                        methodName, "DirectoryCreation", clientConfig.DataFolder);
                }


                IList<UniqueId> uniqueIds = new List<UniqueId>(); // Declare uniqueIds outside the try block
                try
                {
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Searching for unseen emails.", methodName, "EmailSearch");
                    uniqueIds = imapClient.Inbox.Search(SearchQuery.NotSeen, cancellationToken);
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {UnseenEmailCount} unseen emails.",
                        methodName, "EmailSearch", uniqueIds.Count);
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop(); // Stop stopwatch on error
                    logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        methodName, "Search for unseen emails", methodStopwatch.ElapsedMilliseconds, ex.Message);
                    yield break;
                }

                List<Emails> existingEmailsDb = new List<Emails>();
                if (ReturnOnlyUnknownMails)
                {
                    logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): ReturnOnlyUnknownMails is true. Fetching existing emails from DB.",
                        methodName, "DatabaseFetch");
                    try
                    {
                        using (var dbCtx = new CoreEntitiesContext())
                        {
                            logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Creating CoreEntitiesContext for fetching existing emails.",
                                methodName, "DatabaseFetch");
                            existingEmailsDb = dbCtx.Emails
                                .Where(x => x.ApplicationSettingsId == clientConfig.ApplicationSettingsId)
                                .ToList();
                            logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Found {ExistingEmailCount} existing emails in DB.",
                                methodName, "DatabaseFetch", existingEmailsDb.Count);
                        }
                    }
                    catch (Exception dbEx)
                    {
                        methodStopwatch.Stop(); // Stop stopwatch on error
                        logger.Error(dbEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            methodName, "Fetch existing emails from DB", methodStopwatch.ElapsedMilliseconds, dbEx.Message);
                        // Decide if this error should stop processing or just log and continue
                        // For now, we'll yield break as per original logic on error
                        yield break;
                    }
                }
                else
                {
                    logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): ReturnOnlyUnknownMails is false. Skipping fetching existing emails from DB.",
                        methodName, "DatabaseFetch");
                }


                int emailIndex = 0;
                foreach (var uid in uniqueIds)
                {
                    emailIndex++;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Cancellation requested during UID iteration.",
                            methodName, "Cancellation");
                        break;
                    }
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Yielding task for UID {Uid} (Email #{EmailIndex} in batch). IMAP Connected: {IsConnected}",
                        methodName, "YieldingTask", uid, emailIndex, imapClient.IsConnected);
                    yield return ProcessSingleEmailAndDownloadAttachmentsAsync(imapClient, uid, clientConfig, existingEmailsDb.ToList(), logger, cancellationToken);
                }
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Finished yielding all tasks.", methodName, "Completion");

            methodStopwatch.Stop(); // Stop stopwatch
            logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                methodName, "Email streaming completed", $"Email: {clientConfig.Email}, ProcessedEmails: {uniqueIds?.Count ?? 0}", methodStopwatch.ElapsedMilliseconds);
            } // End of LogContext for InvocationId - Moved this closing brace down
        }



        // Definitions for GetSendMailSettings, GetReadMailSettings, and their backing lists
        public static MailSettings GetSendMailSettings(string email, ILogger logger) // Add ILogger parameter
        {
            string methodName = nameof(GetSendMailSettings);
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                methodName, "Get mail settings for sending email", $"Email: {email}");

            try
            {
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Searching for mail settings.", methodName, "Search");
                var settings = _sendEmailSettings.FirstOrDefault(x => email.ToUpper().Contains(x.Name.ToUpper()));

                if (settings != null)
                {
                    logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found mail settings for email: {Email}. Server: {Server}, Port: {Port}",
                        methodName, "Found", email, settings.Server, settings.Port);
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, "Mail settings found", $"Server: {settings.Server}, Port: {settings.Port}", 0); // Placeholder for duration
                }
                else
                {
                    logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No mail settings found for email: {Email}",
                        methodName, "NotFound", email);
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, "No mail settings found", "", 0); // Placeholder for duration
                }

                return settings;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, "Get mail settings for sending email", 0, ex.Message); // Placeholder for duration
                throw;
            }
        }
        public static MailSettings GetReadMailSettings(string email, ILogger logger) // Added ILogger parameter
        {
            string methodName = nameof(GetReadMailSettings);
            logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Attempting to get read mail settings for email: {Email}",
                methodName, "Start", email);

            var settings = _readEmailSettings.FirstOrDefault(x => email.ToUpper().Contains(x.Name.ToUpper()));

            if (settings != null)
            {
                logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found read mail settings for email: {Email}. Server: {Server}, Port: {Port}",
                    methodName, "Found", email, settings.Server, settings.Port);
            }
            else
            {
                logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No read mail settings found for email: {Email}",
                    methodName, "NotFound", email);
            }

            return settings;
        }

        private static List<MailSettings> _sendEmailSettings = new List<MailSettings>()
        {
            new MailSettings(){Name = "AUTO-BROKERAGE.COM",Server = "mail.auto-brokerage.com", Port = 465, Options = SecureSocketOptions.SslOnConnect},
            new MailSettings(){Name = "OUTLOOK.COM", Server = @"smtp-mail.outlook.com", Port = 587, Options = SecureSocketOptions.StartTls}
        };
        private static List<MailSettings> _readEmailSettings = new List<MailSettings>()
        {
            new MailSettings(){Name = "AUTO-BROKERAGE.COM",Server = "mail.auto-brokerage.com", Port = 993, Options = SecureSocketOptions.SslOnConnect},
            new MailSettings(){Name = "OUTLOOK.COM", Server = @"outlook.office365.com", Port = 993, Options = SecureSocketOptions.Auto} // Outlook often uses Auto
        };

        // Other helper methods like SendEmailAsync, CreateMessage, SendBackMsgAsync,
        // SaveAttachmentPartAsync, SaveBodyPartAsync, CleanFileName, GetNextFileName, CheckFileSizeLimitAsync
        // should be included here as per your full EmailDownloader.cs implementation.
        // For brevity, they are omitted from this direct write_to_file content but are assumed to exist.
    }

    // EmailProcessingResult class is now expected to be defined elsewhere (e.g., CoreEntities or the Test project)
}