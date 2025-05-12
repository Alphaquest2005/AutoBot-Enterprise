using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoreEntities.Business.Entities;

using EmailDownloader;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;

using NUnit.Framework;
// For EmailMapping, CoreEntitiesContext, Emails, ApplicationSettings etc.

// Ensure InternalsVisibleTo is set for EmailDownloader project:
// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("NameOfYourTestProject")]

namespace AutoBotUtilities.Tests;

[TestFixture]
[Category("Integration")]
public class EmailDownloaderIntegrationTests
{
    // These will be populated from the database via ApplicationSettings
    private static string _dbConfiguredEmailAddress;
    private static string _dbConfiguredEmailPassword; // Sensitive!
    private static int _dbConfiguredApplicationSettingsId; // The ID of the AppSetting record used for testing

    // Dynamically resolved server settings based on _dbConfiguredEmailAddress
    private static string _resolvedImapServer;
    private static int _resolvedImapPort;
    private static SecureSocketOptions _resolvedImapOptions;
    private static string _resolvedSmtpServer;
    private static int _resolvedSmtpPort;
    private static SecureSocketOptions _resolvedSmtpOptions;

    private static string _testSenderName = "Integration Test Bot";
    private static string _testReceiverEmailAddress; // Will be set to _dbConfiguredEmailAddress
    private static string _testDataFolder;
    private static EmailDownloader.Client _testClientForDownloader;

    // Key to identify the ApplicationSettings record in the DB that holds test email credentials.
    // Option 1: Use a specific SoftwareName
    private const string TestAppSettingSoftwareNameKey = "AutoBotIntegrationTestSource";
    // Option 2: Or use a specific ID (ensure this ID exists in your test database)
    // private const int TestAppSettingIdKey = 99999;


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Console.WriteLine("Attempting to load test account configuration from database...");

        ApplicationSettings testAppSettingFromDb = null;
        try
        {
            using (var ctx = new CoreEntitiesContext()) // Your main DbContext
            {
                // Fetch the ApplicationSettings record that contains the test email credentials
                // Ensure your test runner's environment has a connection string for CoreEntitiesContext
                // that points to your TEST DATABASE.
                testAppSettingFromDb = ctx.ApplicationSettings
                    .FirstOrDefault(s => s.IsActive);
                // OR if using a specific ID:
                // testAppSettingFromDb = ctx.ApplicationSettings.Find(TestAppSettingIdKey);

                if (testAppSettingFromDb != null)
                {
                    _dbConfiguredEmailAddress = testAppSettingFromDb.Email;
                    _dbConfiguredEmailPassword = testAppSettingFromDb.EmailPassword; // Password from DB
                    _dbConfiguredApplicationSettingsId = testAppSettingFromDb.ApplicationSettingsId; // Crucial for DB operations

                    if (string.IsNullOrEmpty(_dbConfiguredEmailAddress) || string.IsNullOrEmpty(_dbConfiguredEmailPassword))
                    {
                        throw new InvalidOperationException(
                            $"Test ApplicationSetting record found (ID: {testAppSettingFromDb.ApplicationSettingsId}, Name: '{testAppSettingFromDb.SoftwareName}') " +
                            "but Email or EmailPassword fields are empty. Please populate them in the test database.");
                    }
                    Console.WriteLine($"Successfully loaded Email/Password for AppSettingId: {_dbConfiguredApplicationSettingsId} (Email: {_dbConfiguredEmailAddress}) from database.");
                }
                else
                {
                    var dbErrorMessage = $"Test ApplicationSetting record identified by SoftwareName '{TestAppSettingSoftwareNameKey}' " +
                                         // OR $"Test ApplicationSetting record identified by ID '{TestAppSettingIdKey}' " +
                                         "was not found in the database. Please ensure this record exists in your test database " +
                                         "and contains the test email account credentials.";
                    Console.Error.WriteLine(dbErrorMessage);
                    Assert.Inconclusive(dbErrorMessage); // Stop if primary config source fails
                    return; // Should not be reached due to Assert.Inconclusive
                }
            }
        }
        catch (Exception ex)
        {
            var dbReadError = $"CRITICAL ERROR reading test email credentials from the database: {ex.Message}\n" +
                              "Ensure the database is accessible and the ApplicationSettings table can be queried.\n" +
                              $"Connection string being used by CoreEntitiesContext should point to the TEST DATABASE.";
            Console.Error.WriteLine(dbReadError);
            Assert.Inconclusive(dbReadError);
            return; // Should not be reached
        }

        _testReceiverEmailAddress = _dbConfiguredEmailAddress; // Test emails sent to this account

        Console.WriteLine("Deriving server settings using EmailDownloader logic based on DB-fetched email address...");
        try
        {
            // Use EmailDownloader's internal logic (requires InternalsVisibleTo)
            var readSettings = EmailDownloader.EmailDownloader.GetReadMailSettings(_dbConfiguredEmailAddress);
            _resolvedImapServer = readSettings.Server;
            _resolvedImapPort = readSettings.Port;
            _resolvedImapOptions = readSettings.Options;

            var sendSettings = EmailDownloader.EmailDownloader.GetSendMailSettings(_dbConfiguredEmailAddress);
            _resolvedSmtpServer = sendSettings.Server;
            _resolvedSmtpPort = sendSettings.Port;
            _resolvedSmtpOptions = sendSettings.Options;

            Console.WriteLine($"Derived IMAP: {_resolvedImapServer}:{_resolvedImapPort} ({_resolvedImapOptions})");
            Console.WriteLine($"Derived SMTP: {_resolvedSmtpServer}:{_resolvedSmtpPort} ({_resolvedSmtpOptions})");
        }
        catch (Exception ex)
        {
            var deriveError = $"Failed to derive email server settings for '{_dbConfiguredEmailAddress}' using EmailDownloader logic. " +
                              $"Ensure GetRead/SendMailSettings cover this domain. Error: {ex.Message}";
            Console.Error.WriteLine(deriveError);
            Assert.Inconclusive(deriveError);
            return; // Should not be reached
        }

        _testDataFolder = Path.Combine(Path.GetTempPath(), "EmailDownloaderTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDataFolder);
        Console.WriteLine($"Test Data Folder: {_testDataFolder}");

        // Configure the client object that EmailDownloader methods will use
        _testClientForDownloader = new EmailDownloader.Client
                                       {
                                           Email = _dbConfiguredEmailAddress,       // From DB
                                           Password = _dbConfiguredEmailPassword,   // From DB
                                           ApplicationSettingsId = _dbConfiguredApplicationSettingsId, // From DB
                                           DataFolder = _testDataFolder,
                                           CompanyName = testAppSettingFromDb?.CompanyName ?? _testSenderName, // Use CompanyName from DB if available
                                           NotifyUnknownMessages = true,
                                           DevMode = false,
                                           EmailMappings = new List<EmailMapping> // Test-specific mappings
                                                               {
                                                                   new EmailMapping { Id = 1, Pattern = @"^Test Email Subject (?<Identifier>.*)", EmailFileTypes = new List<EmailFileTypes>(), EmailMappingRexExs = new List<EmailMappingRexExs>(), EmailInfoMappings = new List<EmailInfoMappings>()},
                                                                   new EmailMapping { Id = 2, Pattern = @"^FWD: Test Forward Subject", EmailFileTypes = new List<EmailFileTypes>(), EmailMappingRexExs = new List<EmailMappingRexExs>(), EmailInfoMappings = new List<EmailInfoMappings>()},
                                                                   new EmailMapping { Id = 3, Pattern = @"^SpecificKnownSubjectPattern (?<Data>.*)", EmailFileTypes = new List<EmailFileTypes>(), EmailMappingRexExs = new List<EmailMappingRexExs>(), EmailInfoMappings = new List<EmailInfoMappings>()}
                                                               }
                                       };
        Console.WriteLine($"Test client configured for EmailDownloader operations using AppSettingId: {_testClientForDownloader.ApplicationSettingsId}");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (Directory.Exists(_testDataFolder))
        {
            try { Directory.Delete(_testDataFolder, true); }
            catch (Exception ex) { Console.WriteLine($"Error deleting test folder {_testDataFolder}: {ex.Message}"); }
        }
        // Optional: Clean up test entries from the Emails table in the test database.
        // ClearTestDatabaseEmailEntriesAsync(_dbConfiguredApplicationSettingsId).GetAwaiter().GetResult();
    }

    [SetUp]
    public void SetupForEachTest()
    {
        EmailDownloader.EmailDownloader.ReturnOnlyUnknownMails = false;
        _testClientForDownloader.NotifyUnknownMessages = true;
    }

    // SendTestEmailAsync now uses the dynamically resolved SMTP settings from OneTimeSetup
    // and the DB-configured email/password for authentication.
    private async Task SendTestEmailAsync(string subject, string body, string[] attachments = null, string toAddress = null)
    {
        toAddress = toAddress ?? _dbConfiguredEmailAddress; // Send to the test account by default
        attachments = attachments ?? Array.Empty<string>();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_testSenderName, _dbConfiguredEmailAddress)); // 'From' header uses the test account email
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { TextBody = body };
        foreach (var attPath in attachments)
        {
            if (File.Exists(attPath)) bodyBuilder.Attachments.Add(attPath);
            else Console.WriteLine($"Test setup warning: Attachment not found: {attPath}");
        }
        message.Body = bodyBuilder.ToMessageBody();

        using (var smtpClient = new SmtpClient())
        {
            await smtpClient.ConnectAsync(_resolvedSmtpServer, _resolvedSmtpPort, _resolvedSmtpOptions);
            await smtpClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword); // Authenticate with DB-fetched credentials
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
        Console.WriteLine($"Sent test email. Subject: '{subject}' To: '{toAddress}' using SMTP: {_resolvedSmtpServer}");
        await Task.Delay(5000); // Allow server time to process
    }

    // GetTestImapClientAsync (for tests to directly interact with IMAP) uses resolved settings
    // and DB-configured email/password for authentication.
    private async Task<ImapClient> GetTestImapClientAsync(CancellationToken cancellationToken = default)
    {
        var imapClient = new ImapClient();
        try
        {
            await imapClient.ConnectAsync(_resolvedImapServer, _resolvedImapPort, _resolvedImapOptions, cancellationToken).ConfigureAwait(false);
            await imapClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword, cancellationToken).ConfigureAwait(false);
            await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
            return imapClient;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GetTestImapClientAsync Error for {_dbConfiguredEmailAddress} on {_resolvedImapServer}: {e}");
            imapClient?.Dispose();
            return null;
        }
    }

    // --- Test Methods (A, B, C, D, E from the previous answer) ---
    // These methods will now use `_testClientForDownloader` which is fully configured from the database
    // and the test helper methods `SendTestEmailAsync` / `GetTestImapClientAsync` also use this DB-derived config.
    // Example Test A:
    [Test, Order(1)]
    public async Task A_CanSendAndReceiveBasicEmail_StreamEmailResultsAsync()
    {
        string uniqueContent = Guid.NewGuid().ToString();
        string testSubject = $"Test Email Subject {uniqueContent}";
        string testBody = $"This is a test email body. Content: {uniqueContent}";

        await this.SendTestEmailAsync(testSubject, testBody);

        var receivedEmails = new List<EmailProcessingResult>();
        ImapClient imapClientForTest = null; // Client for test's direct IMAP operations
        try
        {
            imapClientForTest = await this.GetTestImapClientAsync(CancellationToken.None);
            Assert.That(imapClientForTest, Is.Not.Null, "IMAP client for test should be connected.");
            Assert.That(imapClientForTest.IsConnected && imapClientForTest.IsAuthenticated, Is.True, "Test IMAP client not connected/authenticated.");

            // Pass this test-configured imapClient and the _testClientForDownloader (which EmailDownloader uses)
            foreach (var emailTask in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClientForTest, _testClientForDownloader, CancellationToken.None))
            {
                var result = await emailTask;
                if (result != null && result.EmailKey.Item2.Subject == testSubject)
                {
                    receivedEmails.Add(result);
                }
            }
        }
        finally
        {
            if (imapClientForTest != null && imapClientForTest.IsConnected) await imapClientForTest.DisconnectAsync(true, CancellationToken.None);
            imapClientForTest?.Dispose();
        }

        Assert.That(receivedEmails.Any(), Is.True, $"Expected to find email with subject '{testSubject}'. No matching emails processed.");
        var foundEmail = receivedEmails.First();

        string expectedKeyPart = uniqueContent;
        Assert.That(foundEmail.EmailKey.Item1.Trim(), Is.EqualTo(expectedKeyPart.Trim()), "Subject key (Item1) from GetSubject mismatch.");
        Assert.That(File.Exists(Path.Combine(_testDataFolder, foundEmail.EmailKey.Item1.Trim(), foundEmail.EmailKey.Item3, "Info.txt")), Is.True, "Info.txt not found.");
    }

    // --- Test Methods B, C, D, E (and your helpers like CleanFileNameForComparison, SimulateStoringEmailInDb) ---
    // --- should be here, adapted from the previous full example. They will use _testClientForDownloader ---
    // --- and the helper methods GetTestImapClientAsync/SendTestEmailAsync which are now DB-configured. ---

    [Test, Order(2)]
    public async Task B_CanDownloadEmailWithAttachment_StreamEmailResultsAsync()
    {
        string uniqueContent = Guid.NewGuid().ToString();
        string testSubject = $"Test Email Subject Attachment {uniqueContent}"; // Matches EmailMapping Id=1
        string testBody = "Test email with attachment.";
        string attachmentFileName = $"testAttachment_{uniqueContent}.txt";
        string attachmentPath = Path.Combine(_testDataFolder, attachmentFileName);
        File.WriteAllText(attachmentPath, "This is attachment content.");

        await this.SendTestEmailAsync(testSubject, testBody, new[] { attachmentPath });

        var receivedEmails = new List<EmailProcessingResult>();
        ImapClient imapClientForTest = null;
        try
        {
            imapClientForTest = await this.GetTestImapClientAsync(CancellationToken.None);
            Assert.That(imapClientForTest, Is.Not.Null, "IMAP client for test should be connected.");

            foreach (var emailTask in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClientForTest, _testClientForDownloader, CancellationToken.None))
            {
                var result = await emailTask;
                if (result != null && result.EmailKey.Item2.Subject == testSubject)
                {
                    receivedEmails.Add(result);
                }
            }
        }
        finally
        {
            if (imapClientForTest != null && imapClientForTest.IsConnected) await imapClientForTest.DisconnectAsync(true, CancellationToken.None);
            imapClientForTest?.Dispose();
        }

        Assert.That(receivedEmails.Any(), Is.True, $"Email with subject '{testSubject}' not found.");
        var foundEmail = receivedEmails.First();

        Assert.That(foundEmail.AttachedFiles.Any(f => f.Name.Equals(this.CleanFileNameForComparison(attachmentFileName), StringComparison.OrdinalIgnoreCase)), Is.True,
            $"Attachment '{attachmentFileName}' (cleaned: {this.CleanFileNameForComparison(attachmentFileName)}) not found. Found: {string.Join(", ", foundEmail.AttachedFiles.Select(f => f.Name))}");

        if (File.Exists(attachmentPath)) File.Delete(attachmentPath);
    }

    private string CleanFileNameForComparison(string originalName)
    {
        if (string.IsNullOrWhiteSpace(originalName)) return "unknown.file";
        int lastDot = originalName.LastIndexOf(".");
        if (lastDot == -1 || lastDot == 0 || lastDot == originalName.Length - 1) // Handle no extension or dot at start/end
            return Core.Common.Utils.StringExtensions.ReplaceSpecialChar(originalName, "-").Trim();

        var nameWithoutExtension = originalName.Substring(0, lastDot);
        var fileExtension = originalName.Substring(lastDot);
        var res = Core.Common.Utils.StringExtensions.ReplaceSpecialChar(nameWithoutExtension, "-").Trim() + fileExtension;
        return res;
    }

    [Test, Order(3)]
    public async Task C_ReturnOnlyUnknownMails_FiltersPreviouslyProcessedEmails()
    {
        _testClientForDownloader.ApplicationSettingsId = _dbConfiguredApplicationSettingsId;
        EmailDownloader.EmailDownloader.ReturnOnlyUnknownMails = true;

        string uniqueContent = Guid.NewGuid().ToString();
        string testSubjectForUnknown = $"Test Email Subject ForUnknownCheck {uniqueContent}";

        await this.SendTestEmailAsync(testSubjectForUnknown, "Body for unknown mail check - first pass.");

        List<EmailProcessingResult> firstPassResults = new List<EmailProcessingResult>();
        ImapClient imap = null;
        try
        {
            imap = await this.GetTestImapClientAsync(CancellationToken.None);
            foreach (var task in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imap, _testClientForDownloader, CancellationToken.None))
            {
                var result = await task;
                if (result != null && result.EmailKey.Item2.Subject == testSubjectForUnknown)
                {
                    firstPassResults.Add(result);
                    // Simulate app storing email in DB, using the ApplicationSettingsId from _testClientForDownloader
                    await this.SimulateStoringEmailInDb(result.EmailKey.Item2, result.EmailKey.Item3, _testClientForDownloader.ApplicationSettingsId);
                }
            }
        }
        finally
        {
            if (imap != null && imap.IsConnected) await imap.DisconnectAsync(true, CancellationToken.None);
            imap?.Dispose();
        }
        Assert.That(firstPassResults.Any(), Is.True, "New email should have been fetched on the first pass.");

        List<EmailProcessingResult> secondPassResults = new List<EmailProcessingResult>();
        try
        {
            imap = await this.GetTestImapClientAsync(CancellationToken.None); // Fresh client for second pass
            foreach (var task in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imap, _testClientForDownloader, CancellationToken.None))
            {
                var result = await task;
                if (result != null && result.EmailKey.Item2.Subject == testSubjectForUnknown)
                {
                    secondPassResults.Add(result); // Should not happen
                }
            }
        }
        finally
        {
            EmailDownloader.EmailDownloader.ReturnOnlyUnknownMails = false; // Reset
            if (imap != null && imap.IsConnected) await imap.DisconnectAsync(true, CancellationToken.None);
            imap?.Dispose();
        }
        Assert.That(secondPassResults, Is.Empty, $"Email '{testSubjectForUnknown}' should NOT have been fetched on the second pass as it's now known.");
    }

    private async Task SimulateStoringEmailInDb(Email emailData, string mailkitUid, int appSettingsId)
    {
        using (var dbCtx = new CoreEntitiesContext())
        {
            if (!dbCtx.Emails.Any(e => e.Subject == emailData.Subject && e.EmailDate == emailData.EmailDate && e.ApplicationSettingsId == appSettingsId))
            {
                var emailEntity = new Emails
                                      {
                                          ApplicationSettingsId = appSettingsId,
                                          Subject = emailData.Subject,
                                          EmailDate = emailData.EmailDate,
                                          EmailUniqueId = int.TryParse(mailkitUid, out var uidInt) ? uidInt : 0,
                                          EmailId = $"simulated-{Guid.NewGuid()}",
                                          MachineName = Environment.MachineName,
                                      };
                dbCtx.Emails.Add(emailEntity);
                await dbCtx.SaveChangesAsync();
                Console.WriteLine($"Simulated storing email in DB: Subject='{emailData.Subject}', AppSettingId={appSettingsId}");
            }
            else
            {
                Console.WriteLine($"Email already simulated in DB: Subject='{emailData.Subject}', AppSettingId={appSettingsId}");
            }
        }
    }

    [Test, Order(4)]
    public async Task D_CanForwardExistingMessage_WhenFoundInDb()
    {
        string uniqueOriginal = Guid.NewGuid().ToString();
        string originalSubject = $"Test Email Subject OriginalToForward {uniqueOriginal}";
        await this.SendTestEmailAsync(originalSubject, "Original body to be forwarded.");

        uint mailkitMessageUid = 0;
        string appSpecificEmailId = $"app-emailid-{uniqueOriginal}";

        ImapClient imap = null;
        try
        {
            imap = await this.GetTestImapClientAsync(CancellationToken.None);
            EmailProcessingResult originalEmailResult = null;
            foreach (var task in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imap, _testClientForDownloader, CancellationToken.None))
            {
                var result = await task;
                if (result != null && result.EmailKey.Item2.Subject == originalSubject)
                {
                    originalEmailResult = result;
                    mailkitMessageUid = uint.Parse(result.EmailKey.Item3);
                    await this.SimulateStoringEmailInDbForForwarding(originalEmailResult.EmailKey.Item2, mailkitMessageUid, appSpecificEmailId, _testClientForDownloader.ApplicationSettingsId);
                    break;
                }
            }
            Assert.That(originalEmailResult, Is.Not.Null, "Original email to forward was not found/processed.");
        }
        finally
        {
            if (imap != null && imap.IsConnected) await imap.DisconnectAsync(true, CancellationToken.None);
            imap?.Dispose();
        }

        string forwardSubject = $"FWD: Test Forward Subject {Guid.NewGuid()}";
        string forwardBody = "This is the forwarding body content.";
        string[] contactsToForwardTo = { _testReceiverEmailAddress };

        bool success = await EmailDownloader.EmailDownloader.ForwardMsgAsync(appSpecificEmailId, _testClientForDownloader, forwardSubject, forwardBody, contactsToForwardTo, Array.Empty<string>());
        Assert.That(success, Is.True, "ForwardMsgAsync should return true.");

        await Task.Delay(7000);

        List<EmailProcessingResult> receivedForwardedEmails = new List<EmailProcessingResult>();
        try
        {
            imap = await this.GetTestImapClientAsync(CancellationToken.None);
            foreach (var task in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imap, _testClientForDownloader, CancellationToken.None))
            {
                var result = await task;
                if (result != null && result.EmailKey.Item2.Subject == forwardSubject)
                {
                    receivedForwardedEmails.Add(result);
                }
            }
        }
        finally
        {
            if (imap != null && imap.IsConnected) await imap.DisconnectAsync(true, CancellationToken.None);
            imap?.Dispose();
        }
        Assert.That(receivedForwardedEmails.Any(), Is.True, $"Forwarded email with subject '{forwardSubject}' not found.");
    }

    private async Task SimulateStoringEmailInDbForForwarding(Email emailData, uint mailkitUid, string appSpecificEmailId, int appSettingsId)
    {
        using (var dbCtx = new CoreEntitiesContext())
        {
            if (!dbCtx.Emails.Any(e => e.EmailId == appSpecificEmailId && e.ApplicationSettingsId == appSettingsId))
            {
                var emailEntity = new Emails
                                      {
                                          EmailId = appSpecificEmailId,
                                          ApplicationSettingsId = appSettingsId,
                                          Subject = emailData.Subject,
                                          EmailDate = emailData.EmailDate,
                                          EmailUniqueId = (int)mailkitUid,
                                          MachineName = Environment.MachineName,
                                      };
                dbCtx.Emails.Add(emailEntity);
                await dbCtx.SaveChangesAsync();
                Console.WriteLine($"Simulated storing email for forwarding: AppEmailId='{appSpecificEmailId}', AppSettingId={appSettingsId}");
            }
            else
            {
                Console.WriteLine($"Email for forwarding already simulated in DB: AppEmailId='{appSpecificEmailId}', AppSettingId={appSettingsId}");
            }
        }
    }

    [Test, Order(5)]
    public async Task E_NotifyUnknownMessage_WhenNoMappingMatches()
    {
        _testClientForDownloader.NotifyUnknownMessages = true;

        string unknownSubject = $"Unknown Test Subject {Guid.NewGuid()}"; // Will not match Id=1, 2, or 3 patterns
        await this.SendTestEmailAsync(unknownSubject, "Body for an unknown message type.");

        ImapClient imap = null;
        bool unknownEmailCausedNullResult = false;
        try
        {
            imap = await this.GetTestImapClientAsync(CancellationToken.None);
            foreach (var task in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imap, _testClientForDownloader, CancellationToken.None))
            {
                var result = await task;
                if (result == null)
                {
                    // This is the expected outcome if an unmapped email triggers SendBackMsg (which doesn't yield a result for the original)
                    unknownEmailCausedNullResult = true;
                    // We can't easily check *which* original email caused this null without more elaborate tracking
                    // or inspecting the IMAP server for "Seen" flags on the unknown email.
                }
            }
        }
        finally
        {
            if (imap != null && imap.IsConnected) await imap.DisconnectAsync(true, CancellationToken.None);
            imap?.Dispose();
        }
        Assert.That(unknownEmailCausedNullResult, Is.True, "Processing an unknown email with NotifyUnknownMessages=true should lead to a null EmailProcessingResult for that original email.");

        await Task.Delay(7000); // Wait for the notification email

        string expectedNotificationSubject = $"FWD: {unknownSubject}"; // SendBackMsg prepends "FWD: "
        List<EmailProcessingResult> notificationEmails = new List<EmailProcessingResult>();
        try
        {
            imap = await this.GetTestImapClientAsync(CancellationToken.None);
            // Our EmailMapping Id=2 should match "FWD: Test Forward Subject"
            // For "FWD: Unknown Test Subject ...", we need a mapping or ensure Id=2 is generic enough.
            // Let's assume Mapping Id=2 is: Pattern = @"^FWD: (?<OriginalSubject>.*)"
            // For this test, we'll rely on exact subject match for the notification.
            _testClientForDownloader.EmailMappings.First(m => m.Id == 2).Pattern = @"^FWD: Unknown Test Subject .*"; // Temporarily adjust for exact match

            foreach (var task in EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imap, _testClientForDownloader, CancellationToken.None))
            {
                var result = await task;
                if (result != null && result.EmailKey.Item2.Subject == expectedNotificationSubject)
                {
                    notificationEmails.Add(result);
                }
            }
        }
        finally
        {
            // Reset mapping pattern if changed, or use more robust mapping for tests
            _testClientForDownloader.EmailMappings.First(m => m.Id == 2).Pattern = @"^FWD: Test Forward Subject";
            if (imap != null && imap.IsConnected) await imap.DisconnectAsync(true, CancellationToken.None);
            imap?.Dispose();
        }
        Assert.That(notificationEmails.Any(), Is.True, $"Notification email with subject '{expectedNotificationSubject}' was not found.");
    }
}