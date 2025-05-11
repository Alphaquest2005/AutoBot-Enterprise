using System;
using System.Collections.Generic;
using System.Data.Entity;

// using System.Diagnostics.Eventing.Reader; // Not used in the snippet provided for refactoring
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Utils;
using CoreEntities.Business.Entities; // Assuming this has Email, EmailMapping, FileTypes, CoreEntitiesContext etc.
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;

namespace EmailDownloader
{
    public static partial class EmailDownloader
    {
        private const int SizeinMB = 1048576;
        private const int AsycudaMaxFileSize = 4;

        // GetContacts remains the same (make async if EF Core is ever used)
        public static string[] GetContacts(string role) => new CoreEntitiesContext().Contacts.Where(x => x.Role == role).Select(x => x.EmailAddress).ToArray();

        public static bool ReturnOnlyUnknownMails { get; set; } = false;

        // OLD CheckEmails - to be replaced
        // public static Dictionary<Tuple<string, Email, string>, List<FileInfo>> CheckEmails(Client client) ...

        /// <summary>
        /// Streams email processing results one by one.
        /// The ImapClient connection is managed within this method.
        /// Consumer MUST await tasks sequentially if parallel processing is not intended for the IMAP client.
        /// </summary>
        public static IEnumerable<Task<EmailProcessingResult>> StreamEmailResultsAsync(Client client, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(client.Email))
            {
                yield break;
            }

            ImapClient imapClient = null;
            try
            {
                // Blocking get client for the setup of the IEnumerable.
                // The actual email processing tasks will be async.
                // Use synchronous MailKit calls for setup in this IEnumerable method
                imapClient = new ImapClient();
                var mailSettings = GetReadMailSettings(client.Email);
                imapClient.Connect(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken);
                imapClient.Authenticate(client.Email, client.Password, cancellationToken);
                imapClient.Inbox.Open(FolderAccess.ReadWrite, cancellationToken);

                if (!imapClient.IsConnected || !imapClient.IsAuthenticated)
                {
                    Console.WriteLine($"Failed to connect or authenticate IMAP client for {client.Email}.");
                    yield break;
                }

                if (!Directory.Exists(client.DataFolder)) Directory.CreateDirectory(client.DataFolder);

                // Fetch UIDs and existing emails (synchronous for setup)
                IList<UniqueId> uniqueIds = imapClient.Inbox.Search(SearchQuery.NotSeen, cancellationToken);

                List<Emails> existingEmailsDb = new List<Emails>(); // From CoreEntities.Business.Entities
                if (ReturnOnlyUnknownMails)
                {
                    using (var dbCtx = new CoreEntitiesContext())
                    {
                        // Use ToListAsync if EF6 supports it here, otherwise ToList is sync
                        existingEmailsDb = dbCtx.Emails
                            .Where(x => x.ApplicationSettingsId == client.ApplicationSettingsId)
                            .ToList(); // Or await ToListAsync(cancellationToken) if method was async Task<IEnumerable<...>>
                    }
                }

                // The core loop now yields tasks
                foreach (var uid in uniqueIds)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    // Yield a task that will process a single email.
                    // The imapClient is captured. Consumer should await sequentially.
                    yield return ProcessSingleEmailAndDownloadAttachmentsAsync(imapClient, uid, client, existingEmailsDb.ToList(), cancellationToken);
                }
            }
            finally
            {
                if (imapClient != null)
                {
                    if (imapClient.IsConnected)
                    {
                        // Disconnect synchronously as we are in a finally block of a non-async method.
                        imapClient.Disconnect(true, CancellationToken.None); // Use CancellationToken.None for cleanup
                    }
                    imapClient.Dispose();
                }
            }
        }


        // --- Methods below need to be converted to async Task or async Task<T> ---

        // Renamed to avoid conflict with any existing public SendEmail that might be synchronous

        // GetSendMailSettings and GetReadMailSettings remain synchronous
        private static MailSettings GetSendMailSettings(string email) => _sendEmailSettings.First(x => email.ToUpper().Contains(x.Name.ToUpper()));
        private static MailSettings GetReadMailSettings(string email) => _readEmailSettings.First(x => email.ToUpper().Contains(x.Name.ToUpper()));

        private static string GetNextFileName(string file)
        {
            // Original GetNextFileName logic - remains synchronous
            var fileinfo = new FileInfo(file);
            for (int i = 1; i < 1000; i++)
            {
                var nfileName = Path.Combine(fileinfo.DirectoryName, $"{Path.GetFileNameWithoutExtension(fileinfo.Name)}({i}){fileinfo.Extension.ToLower()}");
                if (!File.Exists(nfileName)) return nfileName;
            }
            return null;
        }

        private static string CleanFileName(string partFileName)
        {
            // Original CleanFileName logic - remains synchronous
            if (string.IsNullOrWhiteSpace(partFileName)) partFileName = "unknown.file";
            // Ensure ReplaceSpecialChar is an extension method available on string, or implement it.
            var newFileName = partFileName.Substring(0, partFileName.LastIndexOf("."));
            var fileExtention = partFileName.Substring(partFileName.LastIndexOf("."));
            var res = StringExtensions.ReplaceSpecialChar(newFileName, "-") + fileExtention; // Assuming Core.Common.Utils.StringExtensions
            return res;
        }

        private static async Task SaveBodyPartAsync(string dataFolder, MimeMessage a, List<FileInfo> lst, CancellationToken cancellationToken) // Made async
        {
            var part = a.BodyParts.OfType<TextPart>().FirstOrDefault();
            if (part == null) return;

            var fileName = "Info.txt";
            var file = Path.Combine(dataFolder, fileName);

            await WriteAllTextAsync(file, part.Text, false, cancellationToken).ConfigureAwait(false); // false for overwrite
            lst.Add(new FileInfo(file));
        }

        // Helper for async file writing in .NET 4.x
        private static async Task WriteAllTextAsync(string path, string contents, bool append, CancellationToken cancellationToken)
        {
            byte[] encodedText = System.Text.Encoding.UTF8.GetBytes(contents);
            FileMode mode = append ? FileMode.Append : FileMode.Create;
            using (var stream = new FileStream(path, mode, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await stream.WriteAsync(encodedText, 0, encodedText.Length, cancellationToken).ConfigureAwait(false);
                if (append) // Ensure newline if appending lines
                    await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(Environment.NewLine), 0, System.Text.Encoding.UTF8.GetBytes(Environment.NewLine).Length, cancellationToken).ConfigureAwait(false);
            }
        }


        private static async Task<MimeMessage> GetMsgAsync(uint uID, Client clientDetails, CancellationToken cancellationToken) // Made async
        {
            using (var imapClient = await GetImapClientAsync(clientDetails, cancellationToken).ConfigureAwait(false))
            {
                if (imapClient == null) return new MimeMessage(); // Or throw
                return await GetMessageOrDefaultAsync(uID, imapClient, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<MimeMessage> GetMessageOrDefaultAsync(uint uID, ImapClient imapClient, CancellationToken cancellationToken) // Made async
        {
            try
            {
                return await imapClient.Inbox.GetMessageAsync(new UniqueId(uID), cancellationToken).ConfigureAwait(false);
            }
            catch (MessageNotFoundException) { /* Log or ignore */ }
            catch (Exception) { /* Log general errors */ }
            return new MimeMessage(); // Return empty message if not found or error
        }

        // Ensure MailSettings class definition from your original code is here
        // It was at the end of your EmailDownloader.cs file.
        // If it's not, the GetSendMailSettings/GetReadMailSettings will cause errors.
        // static _sendEmailSettings and _readEmailSettings lists need to be here too.
        private static List<MailSettings> _sendEmailSettings = new List<MailSettings>()
            {
                new MailSettings(){Name = "auto-brokerage.com",Server = "mail.auto-brokerage.com", Port = 465, Options = SecureSocketOptions.SslOnConnect},
                new MailSettings(){Name = "outlook.com", Server = @"smtp-mail.outlook.com", Port = 587, Options = SecureSocketOptions.StartTls}
            };

        private static List<MailSettings> _readEmailSettings = new List<MailSettings>()
        {
            new MailSettings(){Name = "auto-brokerage.com",Server = "mail.auto-brokerage.com", Port = 993, Options = SecureSocketOptions.SslOnConnect},
            new MailSettings(){Name = "outlook.com", Server = @"outlook.office365.com", Port = 993, Options = SecureSocketOptions.Auto}
        };

        public static async Task SendEmailAsync(Client clientDetails, MimeMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                if (clientDetails.Email == null) return;
                var mailSettings = GetSendMailSettings(clientDetails.Email);
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.Options, cancellationToken).ConfigureAwait(false);
                    await client.AuthenticateAsync(clientDetails.Email, clientDetails.Password, cancellationToken).ConfigureAwait(false);

                    await client.SendAsync(message, cancellationToken).ConfigureAwait(false);

                    await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending email: {e.Message}");
                // Consider more robust logging or re-throwing specific exceptions
            }
        }
    }
    // This was at the end of your original EmailDownloader.cs file.
    // If it's defined elsewhere, remove this and ensure the namespace is correct.
    // If it's only used internally by EmailDownloader, keeping it here is fine.
}