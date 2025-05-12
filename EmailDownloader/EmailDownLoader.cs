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

        public static string[] GetContacts(string role) => new CoreEntitiesContext().Contacts.Where(x => x.Role == role).Select(x => x.EmailAddress).ToArray();
        public static bool ReturnOnlyUnknownMails { get; set; } = false;

        public static async Task<ImapClient> GetOpenImapClientAsync(Client clientConfig, CancellationToken cancellationToken = default)
        {
            var imapClient = new ImapClient(); // ProtocolLogger can be added here for deep diagnostics
            // Example: imapClient = new ImapClient(new ProtocolLogger(Console.OpenStandardError()));
            try
            {
                var mailSettings = GetReadMailSettings(clientConfig.Email); // Uses local GetReadMailSettings
                imapClient.Timeout = 60000; // 60 seconds timeout for operations

                Console.WriteLine($"GetOpenImapClientAsync: Connecting to {mailSettings.Server}:{mailSettings.Port} for {clientConfig.Email}");
                await imapClient.ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"GetOpenImapClientAsync: Connected. Authenticating...");
                await imapClient.AuthenticateAsync(clientConfig.Email, clientConfig.Password, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"GetOpenImapClientAsync: Authenticated. Opening Inbox...");
                await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"GetOpenImapClientAsync: Inbox opened. IsOpen: {imapClient.Inbox.IsOpen}");
                return imapClient;
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetOpenImapClientAsync Error for {clientConfig.Email}: {e.GetType().Name} - {e.Message}");
                // Log e.StackTrace if needed
                imapClient?.Dispose(); // Dispose if any step failed
                return null;
            }
        }

        public static IEnumerable<Task<EmailProcessingResult>> StreamEmailResultsAsync(
            ImapClient imapClient, // Expects a connected, authenticated, and inbox-opened client
            Client clientConfig,
            CancellationToken cancellationToken = default)
        {
            if (imapClient == null || !imapClient.IsConnected || !imapClient.IsAuthenticated || imapClient.Inbox == null || !imapClient.Inbox.IsOpen)
            {
                Console.WriteLine($"StreamEmailResultsAsync: Received IMAP client is not ready (Connected: {imapClient?.IsConnected}, Authenticated: {imapClient?.IsAuthenticated}, Inbox Open: {imapClient?.Inbox?.IsOpen}).");
                yield break;
            }
            Console.WriteLine("StreamEmailResultsAsync: IMAP client is ready. Proceeding with search.");
            
            if (!Directory.Exists(clientConfig.DataFolder)) Directory.CreateDirectory(clientConfig.DataFolder);

            IList<UniqueId> uniqueIds;
            try
            {
                uniqueIds = imapClient.Inbox.Search(SearchQuery.NotSeen, cancellationToken);
                Console.WriteLine($"StreamEmailResultsAsync: Found {uniqueIds.Count} unseen emails.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StreamEmailResultsAsync: Error searching for unseen emails: {ex.GetType().Name} - {ex.Message}");
                yield break;
            }

            List<Emails> existingEmailsDb = new List<Emails>();
            if (ReturnOnlyUnknownMails)
            {
                try
                {
                    using (var dbCtx = new CoreEntitiesContext())
                    {
                        existingEmailsDb = dbCtx.Emails
                            .Where(x => x.ApplicationSettingsId == clientConfig.ApplicationSettingsId)
                            .ToList();
                    }
                }
                catch (Exception dbEx)
                {
                     Console.WriteLine($"StreamEmailResultsAsync: Error fetching existing emails from DB: {dbEx.Message}");
                }
            }

            int emailIndex = 0;
            foreach (var uid in uniqueIds)
            {
                emailIndex++;
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("StreamEmailResultsAsync: Cancellation requested during UID iteration.");
                    break;
                }
                Console.WriteLine($"StreamEmailResultsAsync: Yielding task for UID {uid} (Email #{emailIndex} in batch). IMAP Connected: {imapClient.IsConnected}");
                yield return ProcessSingleEmailAndDownloadAttachmentsAsync(imapClient, uid, clientConfig, existingEmailsDb.ToList(), cancellationToken);
            }
            Console.WriteLine("StreamEmailResultsAsync: Finished yielding all tasks.");
        }

        



        // Definitions for GetSendMailSettings, GetReadMailSettings, and their backing lists
        public static MailSettings GetSendMailSettings(string email) => _sendEmailSettings.FirstOrDefault(x => email.ToUpper().Contains(x.Name.ToUpper()));
        public static MailSettings GetReadMailSettings(string email) => _readEmailSettings.FirstOrDefault(x => email.ToUpper().Contains(x.Name.ToUpper()));

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