using System;
using System.Collections.Generic;
using System.Data.Entity; // Keep if needed for older EF interactions, check necessity
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoBot; // For EmailProcessor - Ensure visibility/access
using CoreEntities.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities; // Added for EntryDataDSContext
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.NUnit; // **** Ensure this using directive is present ****

// For EmailMapping, CoreEntitiesContext, Emails, ApplicationSettings etc.

// Ensure InternalsVisibleTo is set for EmailDownloader project:
// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("NameOfYourTestProject")]
// Ensure InternalsVisibleTo is set for AutoBot project (if Program.ProcessEmailsAsync is internal):
// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AutoBotUtilities.Tests")] // In AutoBot AssemblyInfo.cs or .csproj

namespace AutoBotUtilities.Tests;

[TestFixture]
[Category("Integration")]
public class EmailDownloaderIntegrationTests
{
    private static Logger _log; // Serilog logger instance

    // ... (Keep other fields as they are) ...
    private static string _dbConfiguredEmailAddress;
    private static string _dbConfiguredEmailPassword; // Sensitive!
    private static int _dbConfiguredApplicationSettingsId; // The ID of the AppSetting record used for testing
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
    private const string TestAppSettingSoftwareNameKey = "AutoBotIntegrationTestSource";
    private const string TestPdfFilePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf";


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Configure Serilog - Ensure NUnitOutput sink is included
        _log = new LoggerConfiguration()
            .MinimumLevel.Verbose() // Adjust level as needed for debugging
            .WriteTo.Console() // Optional: keep if you want console output too
            .WriteTo.NUnitOutput() // **** NUnit Sink ****
            .WriteTo.File("EmailDownloaderIntegrationTests.log", // File logging
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}") // Example template
            .CreateLogger();
        Log.Logger = _log; // Assign to the static Log.Logger for global access

        Log.Information("=== EmailDownloaderIntegrationTests OneTimeSetup Starting ===");
        Log.Information("Attempting to load test account configuration from database...");

        ApplicationSettings testAppSettingFromDb = null;
        try
        {
            // Ensure CoreEntitiesContext connection string points to TEST database
            using (var ctx = new CoreEntitiesContext())
            {
                // Fetch the ApplicationSettings record...
                testAppSettingFromDb = ctx.ApplicationSettings
                    .Include(x => x.FileTypes)
                    .Include(x => x.Declarants)
                    .Include("FileTypes.FileTypeReplaceRegex")
                    .Include("FileTypes.FileImporterInfos")
                    .Include(x => x.EmailMapping)
                    .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos")
                    .Include("EmailMapping.EmailMappingRexExs")
                    .Include("EmailMapping.EmailMappingActions.Actions")
                    .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                    .FirstOrDefault(s => s.IsActive); // Assuming only one active test setting

                if (testAppSettingFromDb != null)
                {
                    _dbConfiguredEmailAddress = testAppSettingFromDb.Email;
                    _dbConfiguredEmailPassword = testAppSettingFromDb.EmailPassword; // Password from DB
                    _dbConfiguredApplicationSettingsId = testAppSettingFromDb.ApplicationSettingsId;

                    if (string.IsNullOrEmpty(_dbConfiguredEmailAddress) || string.IsNullOrEmpty(_dbConfiguredEmailPassword))
                    {
                        var configError = $"Test ApplicationSetting record found (ID: {testAppSettingFromDb.ApplicationSettingsId}, Name: '{testAppSettingFromDb.SoftwareName}') " +
                                          "but Email or EmailPassword fields are empty.";
                        Log.Error(configError);
                        Assert.Inconclusive(configError);
                        return;
                    }
                    Log.Information("Successfully loaded Email/Password for AppSettingId: {AppSettingId} (Email: {EmailAddress}) from database.", _dbConfiguredApplicationSettingsId, _dbConfiguredEmailAddress);
                }
                else
                {
                    var dbErrorMessage = $"Test ApplicationSetting record identified by IsActive=true " +
                                         "was not found in the database. Please ensure this record exists and is active in your test database.";
                    Log.Error(dbErrorMessage);
                    Assert.Inconclusive(dbErrorMessage);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            var dbReadError = $"CRITICAL ERROR reading test email credentials from the database: {ex.Message}\n" +
                              "Ensure the database is accessible and the ApplicationSettings table can be queried.\n" +
                              $"Connection string being used by CoreEntitiesContext should point to the TEST DATABASE.";
            Log.Error(ex, dbReadError);
            Assert.Inconclusive(dbReadError);
            return;
        }

        _testReceiverEmailAddress = _dbConfiguredEmailAddress;

        Log.Information("Deriving server settings using EmailDownloader logic...");
        try
        {
            var readSettings = EmailDownloader.EmailDownloader.GetReadMailSettings(_dbConfiguredEmailAddress, _log);
            _resolvedImapServer = readSettings.Server;
            _resolvedImapPort = readSettings.Port;
            _resolvedImapOptions = readSettings.Options;

            var sendSettings = EmailDownloader.EmailDownloader.GetSendMailSettings(_dbConfiguredEmailAddress, _log);
            _resolvedSmtpServer = sendSettings.Server;
            _resolvedSmtpPort = sendSettings.Port;
            _resolvedSmtpOptions = sendSettings.Options;

            Log.Information("Derived IMAP: {ImapServer}:{ImapPort} ({ImapOptions})", _resolvedImapServer, _resolvedImapPort, _resolvedImapOptions);
            Log.Information("Derived SMTP: {SmtpServer}:{SmtpPort} ({SmtpOptions})", _resolvedSmtpServer, _resolvedSmtpPort, _resolvedSmtpOptions);
        }
        catch (Exception ex)
        {
            var deriveError = $"Failed to derive email server settings for '{_dbConfiguredEmailAddress}'.";
            Log.Error(ex, deriveError);
            Assert.Inconclusive($"{deriveError} Error: {ex.Message}");
            return;
        }

        _testDataFolder = Path.Combine(Path.GetTempPath(), "EmailDownloaderTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDataFolder);
        Log.Information("Test Data Folder: {TestDataFolder}", _testDataFolder);

        string companyName = testAppSettingFromDb?.CompanyName ?? _testSenderName;

        _testClientForDownloader = new EmailDownloader.Client
        {
            Email = _dbConfiguredEmailAddress,
            Password = _dbConfiguredEmailPassword,
            ApplicationSettingsId = _dbConfiguredApplicationSettingsId,
            DataFolder = _testDataFolder,
            CompanyName = companyName,
            NotifyUnknownMessages = true,
            DevMode = false,
            // Load mappings from DB below, keep placeholder patterns for clarity if needed
            EmailMappings = new List<EmailMapping>()
        };

        // Add mappings retrieved from the database
        if (testAppSettingFromDb?.EmailMapping != null)
        {
            _testClientForDownloader.EmailMappings.AddRange(testAppSettingFromDb.EmailMapping);
            Log.Information("Added {Count} EmailMappings from database AppSettingId: {AppSettingId}",
                testAppSettingFromDb.EmailMapping.Count, _dbConfiguredApplicationSettingsId);

            // Ensure the specific mapping for the PDF test exists or add a default if needed
            if (!_testClientForDownloader.EmailMappings.Any(em => em.Pattern.Contains("PDF Import Test")))
            {
                Log.Warning("Did not find 'PDF Import Test' pattern in DB mappings. Adding a default for the test.");
                _testClientForDownloader.EmailMappings.Add(new EmailMapping { Id = -1, Pattern = @"^PDF Import Test (?<Identifier>.*)" }); // Use temporary ID
            }
        }
        else
        {
            Log.Warning("No EmailMappings found in the loaded ApplicationSettings (ID: {AppSettingId}). Adding default mappings for tests.", _dbConfiguredApplicationSettingsId);
            // Add minimal mappings needed for other tests if they rely on specific patterns
            _testClientForDownloader.EmailMappings.AddRange(new List<EmailMapping>
             {
                 new EmailMapping { Id = -2, Pattern = @"^Test Email Subject (?<Identifier>.*)"},
                 new EmailMapping { Id = -3, Pattern = @"^FWD: (?<OriginalSubject>.*)"},
                 new EmailMapping { Id = -4, Pattern = @"^PDF Import Test (?<Identifier>.*)"}
             });
        }


        Log.Information("Test client configured for EmailDownloader operations using AppSettingId: {AppSettingId}", _testClientForDownloader.ApplicationSettingsId);
        Log.Information("=== EmailDownloaderIntegrationTests OneTimeSetup Complete ===");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Log.Information("=== EmailDownloaderIntegrationTests OneTimeTearDown Starting ===");
        if (Directory.Exists(_testDataFolder))
        {
            try
            {
                Directory.Delete(_testDataFolder, true);
                Log.Information("Deleted test data folder: {TestDataFolder}", _testDataFolder);
            }
            catch (Exception ex) { Log.Warning(ex, "Error deleting test folder {TestDataFolder}", _testDataFolder); }
        }
        Log.CloseAndFlush(); // Flush Serilog
        Console.WriteLine("=== EmailDownloaderIntegrationTests OneTimeTearDown Complete ===");
    }

    [SetUp]
    public void SetupForEachTest()
    {
        Log.Information("--- Test Setup Starting: {TestName} ---", TestContext.CurrentContext.Test.Name);
        EmailDownloader.EmailDownloader.ReturnOnlyUnknownMails = false; // Default state
        _testClientForDownloader.NotifyUnknownMessages = true; // Default state
        Log.Information("--- Test Setup Complete: {TestName} ---", TestContext.CurrentContext.Test.Name);
    }

    // --- Send/Receive Helpers ---
    private async Task SendTestEmailAsync(string subject, string body, string[] attachments = null, string toAddress = null)
    {
        toAddress = toAddress ?? _dbConfiguredEmailAddress;
        attachments = attachments ?? Array.Empty<string>();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_testSenderName, _dbConfiguredEmailAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { TextBody = body };
        foreach (var attPath in attachments)
        {
            if (File.Exists(attPath)) { bodyBuilder.Attachments.Add(attPath); Log.Debug("Adding attachment: {AttachmentPath}", attPath); }
            else Log.Warning("Test setup warning: Attachment not found, skipping: {AttachmentPath}", attPath);
        }
        message.Body = bodyBuilder.ToMessageBody();

        using (var smtpClient = new SmtpClient())
        {
            try
            {
                Log.Debug("Connecting SMTP: {SmtpServer}:{SmtpPort} ({SmtpOptions})", _resolvedSmtpServer, _resolvedSmtpPort, _resolvedSmtpOptions);
                await smtpClient.ConnectAsync(_resolvedSmtpServer, _resolvedSmtpPort, _resolvedSmtpOptions).ConfigureAwait(false);
                Log.Debug("Authenticating SMTP: {EmailAddress}", _dbConfiguredEmailAddress);
                await smtpClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword).ConfigureAwait(false);
                Log.Debug("Sending SMTP: To={ToAddress}, Subject={Subject}", toAddress, subject);
                await smtpClient.SendAsync(message).ConfigureAwait(false);
                await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
                Log.Information("Sent test email. Subject: '{Subject}' To: '{ToAddress}'", subject, toAddress);
            }
            catch (Exception ex) { Log.Error(ex, "Failed to send test email. Subject: {Subject}, To: {ToAddress}", subject, toAddress); throw; }
        }
        Log.Debug("Waiting 5 seconds for email server processing...");
        await Task.Delay(5000).ConfigureAwait(false);
    }

    private async Task<ImapClient> GetTestImapClientAsync(CancellationToken cancellationToken = default)
    {
        var imapClient = new ImapClient();
        try
        {
            Log.Debug("Connecting IMAP: {ImapServer}:{ImapPort} ({ImapOptions})", _resolvedImapServer, _resolvedImapPort, _resolvedImapOptions);
            await imapClient.ConnectAsync(_resolvedImapServer, _resolvedImapPort, _resolvedImapOptions, cancellationToken).ConfigureAwait(false);
            Log.Debug("Authenticating IMAP: {EmailAddress}", _dbConfiguredEmailAddress);
            await imapClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword, cancellationToken).ConfigureAwait(false);
            Log.Debug("Opening IMAP Inbox (ReadWrite)"); // Ensure default is ReadWrite if needed later
            await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
            Log.Information("IMAP client connected & Inbox opened: {EmailAddress}", _dbConfiguredEmailAddress);
            return imapClient;
        }
        catch (Exception e)
        {
            Log.Error(e, "GetTestImapClientAsync Error for {EmailAddress} on {ImapServer}", _dbConfiguredEmailAddress, _resolvedImapServer);
            try { await imapClient.DisconnectAsync(false, CancellationToken.None).ConfigureAwait(false); } catch { /* Ignored */ }
            imapClient?.Dispose();
            return null;
        }
    }

    // --- Test Methods ---

    [Test, Order(1)]
    public async Task A_CanSendAndReceiveBasicEmail_StreamEmailResultsAsync()
    {
        Log.Information("Executing Test: A_CanSendAndReceiveBasicEmail_StreamEmailResultsAsync");
        string uniqueContent = Guid.NewGuid().ToString();
        string testSubject = $"Test Email Subject {uniqueContent}"; // Matches default Mapping Id=-2
        string testBody = $"This is a test email body. Content: {uniqueContent}";

        await this.SendTestEmailAsync(testSubject, testBody).ConfigureAwait(false);

        var receivedEmails = new List<EmailProcessingResult>();
        ImapClient imapClientForTest = null;
        try
        {
            imapClientForTest = await this.GetTestImapClientAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.That(imapClientForTest, Is.Not.Null, "IMAP client for test should be connected and authenticated.");

            Log.Debug("Starting StreamEmailResultsAsync to find email: {Subject}", testSubject);
            var streamEmailResultsAsync = EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClientForTest, _testClientForDownloader, _log, CancellationToken.None);
            foreach (var emailTask in streamEmailResultsAsync) // Use await foreach
            {
                var result = await emailTask.ConfigureAwait(false); // Materialize the result
                if (result != null)
                {
                    Log.Verbose("StreamEmailResultsAsync yielded: Subject='{Subject}', UID={Uid}", result.EmailKey.EmailMessage?.Subject, result.EmailKey.UidString);
                    if (result.EmailKey.EmailMessage?.Subject == testSubject)
                    {
                        Log.Information("Found matching email: {Subject}", testSubject);
                        receivedEmails.Add(result);
                        // break; // Removed break to process all matches if needed, add back if only one expected
                    }
                }
                else Log.Verbose("StreamEmailResultsAsync yielded null.");
            }
            Log.Debug("Finished StreamEmailResultsAsync loop.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during StreamEmailResultsAsync processing in test A.");
            Assert.Fail($"Test failed during email processing: {ex.Message}");
        }
        finally
        {
            if (imapClientForTest != null && imapClientForTest.IsConnected)
            {
                Log.Debug("Disconnecting test IMAP client.");
                await imapClientForTest.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false);
            }
            imapClientForTest?.Dispose();
        }

        Assert.That(receivedEmails, Is.Not.Empty, $"Expected to find email with subject '{testSubject}'.");
        var foundEmail = receivedEmails.First(); // Assuming first match is the one

        Assert.That(foundEmail.EmailKey, Is.Not.Null);
        Assert.That(foundEmail.EmailKey.EmailMessage, Is.Not.Null);
        Assert.That(foundEmail.EmailKey.UidString, Is.Not.Null.Or.Empty);
        Assert.That(foundEmail.EmailKey.SubjectIdentifier, Is.Not.Null.Or.Empty);
        Assert.That(foundEmail.EmailKey.SubjectIdentifier.Trim(), Is.EqualTo(uniqueContent.Trim()), "Subject key (SubjectIdentifier) mismatch.");

        string subjectIdTrimmed = foundEmail.EmailKey.SubjectIdentifier?.Trim();
        string uidString = foundEmail.EmailKey.UidString ?? "NULL_UID";
        Assert.That(!string.IsNullOrEmpty(subjectIdTrimmed), "SubjectIdentifier was empty after trimming.");

        string expectedDir = Path.Combine(_testDataFolder, subjectIdTrimmed, uidString);
        string infoTxtPath = Path.Combine(expectedDir, "Info.txt");

        Log.Debug("Checking for Info.txt at: {InfoTxtPath}", infoTxtPath);
        Assert.That(File.Exists(infoTxtPath), Is.True, $"Info.txt not found at expected path: {infoTxtPath}");
        Log.Information("Test A verification successful.");
    }


    [Test, Order(6)]
    public async Task ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()
    {
        Log.Information("=== Test Started: ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest ===");

        // --- Arrange ---
        Log.Debug("Verifying test PDF file exists at {FilePath}", TestPdfFilePath);
        Assert.That(File.Exists(TestPdfFilePath), Is.True, $"Test PDF file not found at: {TestPdfFilePath}");

        string uniqueContent = Guid.NewGuid().ToString();
        string testSubject = $"PDF Import Test {uniqueContent}"; // Matches mapping pattern
        string testBody = "Integration test email with PDF attachment for ProcessEmailsAsync.";
        string invoiceNumberToVerify = "114-7827932-2029910"; // Example invoice number from the PDF

        // 1. Database Cleanup (EntryDataDSContext - Target DB)
        Log.Information("Cleaning up previous test data for InvoiceNo: {InvoiceNumber} in EntryDataDS...", invoiceNumberToVerify);
        try
        {
            using (var entryCtx = new EntryDataDSContext()) // Ensure connection string is correct
            {
                // Use explicit loading or disable lazy loading if needed for related entities
                var existingDetails = await entryCtx.ShipmentInvoiceDetails
                                          .Where(d => d.Invoice.InvoiceNo == invoiceNumberToVerify)
                                          .ToListAsync().ConfigureAwait(false);
                if (existingDetails.Any())
                {
                    Log.Debug("Removing {Count} existing ShipmentInvoiceDetails.", existingDetails.Count);
                    entryCtx.ShipmentInvoiceDetails.RemoveRange(existingDetails);
                }

                var existingInvoice = await entryCtx.ShipmentInvoice
                                          .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNumberToVerify).ConfigureAwait(false);
                if (existingInvoice != null)
                {
                    Log.Debug("Removing existing ShipmentInvoice.");
                    entryCtx.ShipmentInvoice.Remove(existingInvoice);
                }
                await entryCtx.SaveChangesAsync().ConfigureAwait(false);
                Log.Information("EntryDataDS cleanup complete.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during EntryDataDS database cleanup.");
            Assert.Fail($"Database cleanup failed: {ex.Message}");
        }

        // 2. Send Test Email
        Log.Information("Sending test email with PDF attachment...");
        await this.SendTestEmailAsync(testSubject, testBody, new[] { TestPdfFilePath }).ConfigureAwait(false);

        // 3. Simulate Initial Download & Get Email UID
        Log.Information("Simulating initial email download to get UID and verify attachment presence...");
        EmailProcessingResult processedEmailResult = null;
        ImapClient imapClientForDownload = null;
        string downloadedPdfPath = null;
        UniqueId targetEmailUid = UniqueId.Invalid; // Store the UID here

        try
        {
            imapClientForDownload = await this.GetTestImapClientAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.That(imapClientForDownload, Is.Not.Null, "IMAP client for initial download failed.");

            // Use StreamEmailResultsAsync to find the email and simulate download logic
            var streamEmailResultsAsync = EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClientForDownload, _testClientForDownloader, _log, CancellationToken.None);
            foreach (var emailTask in streamEmailResultsAsync) // Use await foreach
            {
                var result = await emailTask.ConfigureAwait(false);
                if (result != null && result.EmailKey.EmailMessage?.Subject == testSubject)
                {
                    processedEmailResult = result;
                    Log.Information("Found and processed test email during initial download simulation: Subject='{Subject}', UID={Uid}", testSubject, result.EmailKey.UidString);

                    // *** Store the UID ***
                    if (!UniqueId.TryParse(result.EmailKey.UidString, out targetEmailUid))
                    {
                        Log.Error("Failed to parse UID string '{UidString}' from processed email result.", result.EmailKey.UidString);
                        Assert.Fail($"Could not parse UID '{result.EmailKey.UidString}' needed for the test.");
                    }
                    Log.Debug("Successfully parsed target email UID: {Uid}", targetEmailUid);

                    // Simulate storing the email record (optional but good practice for test completeness)
                    if (result.EmailKey.EmailMessage != null && !string.IsNullOrEmpty(result.EmailKey.UidString))
                    {
                        await this.SimulateStoringEmailInDb(result.EmailKey.EmailMessage, result.EmailKey.UidString, _testClientForDownloader.ApplicationSettingsId).ConfigureAwait(false);
                    }
                    else Log.Warning("Could not simulate storing email in DB due to missing EmailMessage or UidString.");

                    // Verify attachment info
                    Assert.That(processedEmailResult.AttachedFiles, Is.Not.Null.And.Not.Empty, "No attached files found in processed result.");
                    var pdfAttachmentInfo = processedEmailResult.AttachedFiles.FirstOrDefault(f => f.Name?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ?? false);
                    Assert.That(pdfAttachmentInfo, Is.Not.Null, "PDF attachment info not found in result.");

                    // Verify file exists on disk (assuming StreamEmailResultsAsync saved it)
                    string subjectIdTrimmed = processedEmailResult.EmailKey.SubjectIdentifier?.Trim();
                    string uidString = processedEmailResult.EmailKey.UidString;
                    Assert.That(!string.IsNullOrEmpty(subjectIdTrimmed), "SubjectIdentifier is empty.");
                    Assert.That(!string.IsNullOrEmpty(uidString), "UID string is empty.");

                    string expectedDir = Path.Combine(_testDataFolder, subjectIdTrimmed, uidString);
                    downloadedPdfPath = Path.Combine(expectedDir, pdfAttachmentInfo.Name);
                    Log.Debug("Checking for downloaded PDF at: {PdfPath}", downloadedPdfPath);
                    Assert.That(File.Exists(downloadedPdfPath), Is.True, $"Downloaded PDF not found at expected path: {downloadedPdfPath}.");
                    Log.Information("Verified downloaded PDF exists at: {PdfPath}", downloadedPdfPath);

                    break; // Found the target email
                }
            }
            Assert.That(processedEmailResult, Is.Not.Null, $"Test email '{testSubject}' not found/processed during initial download simulation.");
            Assert.That(targetEmailUid.IsValid, Is.True, "Target Email UID was not successfully obtained.");

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during initial email download simulation phase.");
            Assert.Fail($"Initial email download simulation failed: {ex.Message}");
        }
        finally
        {
            // Disconnect the client used for the initial download simulation
            if (imapClientForDownload != null && imapClientForDownload.IsConnected)
            {
                Log.Debug("Disconnecting IMAP client used for initial download simulation.");
                await imapClientForDownload.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false);
            }
            imapClientForDownload?.Dispose();
        }

        // **** NEW STEP: Mark the Email as UNREAD before calling ProcessEmailsAsync ****
        Log.Information("Attempting to mark email UID {Uid} as UNREAD before calling ProcessEmailsAsync.", targetEmailUid);
        ImapClient markUnreadClient = null;
        try
        {
            // Create a new client instance for this specific operation
            markUnreadClient = new ImapClient();
            await markUnreadClient.ConnectAsync(_resolvedImapServer, _resolvedImapPort, _resolvedImapOptions, CancellationToken.None).ConfigureAwait(false);
            await markUnreadClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword, CancellationToken.None).ConfigureAwait(false);

            // Open Inbox in ReadWrite mode to modify flags
            await markUnreadClient.Inbox.OpenAsync(FolderAccess.ReadWrite, CancellationToken.None).ConfigureAwait(false);

            // Remove the Seen flag (mark as unread)
            await markUnreadClient.Inbox.RemoveFlagsAsync(targetEmailUid, MessageFlags.Seen, true, CancellationToken.None).ConfigureAwait(false); // silent=true

            Log.Information("Successfully marked email UID {Uid} as UNREAD.", targetEmailUid);

            await markUnreadClient.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log the error and fail the test if marking unread fails
            Log.Error(ex, "Failed to mark email UID {Uid} as UNREAD.", targetEmailUid);
            if (markUnreadClient != null && markUnreadClient.IsConnected)
            {
                try { await markUnreadClient.DisconnectAsync(false, CancellationToken.None).ConfigureAwait(false); } catch { /* Ignore */ }
            }
            Assert.Fail($"Failed to mark email as unread before ProcessEmailsAsync call: {ex.Message}");
        }
        finally
        {
            markUnreadClient?.Dispose(); // Ensure disposal
        }


        // 4. Prepare for ProcessEmailsAsync call
        Log.Information("Preparing to call EmailProcessor.ProcessEmailsAsync...");
        ApplicationSettings appSetting = null;
        CoreEntitiesContext ctxForProcess = null;
        try
        {
            ctxForProcess = new CoreEntitiesContext(); // Fresh context
            
            appSetting = await ctxForProcess.ApplicationSettings
                             .Include(x => x.FileTypes)
                             .Include(x => x.Declarants)
                             .Include("FileTypes.FileTypeReplaceRegex")
                             .Include("FileTypes.FileImporterInfos")
                             .Include(x => x.EmailMapping)
                             .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos")
                             .Include("EmailMapping.EmailMappingRexExs")
                             .Include("EmailMapping.EmailMappingActions.Actions")
                             .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                             .FirstOrDefaultAsync(a => a.ApplicationSettingsId == _dbConfiguredApplicationSettingsId).ConfigureAwait(false);
            
            Assert.That(appSetting, Is.Not.Null, $"Failed to retrieve ApplicationSettings ID: {_dbConfiguredApplicationSettingsId}");
            Log.Debug("Retrieved ApplicationSettings: ID={AppSettingId}, Name='{SoftwareName}'", appSetting.ApplicationSettingsId, appSetting.SoftwareName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving ApplicationSettings for ProcessEmailsAsync.");
            ctxForProcess?.Dispose();
            Assert.Fail($"Failed to prepare for ProcessEmailsAsync: {ex.Message}");
        }

        DateTime beforeImport = DateTime.Now.AddMinutes(-1); // Time before the processor runs
        CancellationToken cancellationToken = CancellationToken.None;

        // --- Act ---
        Log.Information("Calling EmailProcessor.ProcessEmailsAsync for AppSetting ID: {AppSettingId}...", appSetting.ApplicationSettingsId);
        try
        {
            // Ensure EmailProcessor.ProcessEmailsAsync is accessible (public/internal+InternalsVisibleTo)
            await EmailProcessor.ProcessEmailsAsync(appSetting, beforeImport, ctxForProcess, cancellationToken, Log.Logger).ConfigureAwait(false); // Assuming EmailProcessor class
            Log.Information("EmailProcessor.ProcessEmailsAsync call completed.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception during EmailProcessor.ProcessEmailsAsync execution.");
            Assert.Fail($"ProcessEmailsAsync threw an exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
        finally
        {
            ctxForProcess?.Dispose(); // Dispose context used by ProcessEmailsAsync
        }

        // --- Assert ---
        Log.Information("Verifying import results in database (EntryDataDSContext)...");
        try
        {
            using (var verifyCtx = new EntryDataDSContext()) // Ensure connection string is correct
            {
                Log.Verbose("Checking for ShipmentInvoice '{InvoiceNumber}'", invoiceNumberToVerify);
                bool invoiceExists = await verifyCtx.ShipmentInvoice.AnyAsync(x => x.InvoiceNo == invoiceNumberToVerify, cancellationToken).ConfigureAwait(false);
                Assert.That(invoiceExists, Is.True, $"ShipmentInvoice '{invoiceNumberToVerify}' not created.");
                Log.Verbose("ShipmentInvoice found: {Exists}", invoiceExists);

                Log.Verbose("Checking for ShipmentInvoiceDetails count > 2 for '{InvoiceNumber}'", invoiceNumberToVerify);
                int detailCount = await verifyCtx.ShipmentInvoiceDetails.CountAsync(x => x.Invoice.InvoiceNo == invoiceNumberToVerify, cancellationToken).ConfigureAwait(false);
                Assert.That(detailCount > 2, Is.True, $"Expected > 2 ShipmentInvoiceDetails for '{invoiceNumberToVerify}', found {detailCount}.");
                Log.Verbose("ShipmentInvoiceDetails count: {Count}", detailCount);

                int totalInvoices = await verifyCtx.ShipmentInvoice.CountAsync(cancellationToken).ConfigureAwait(false);
                int totalDetails = await verifyCtx.ShipmentInvoiceDetails.CountAsync(cancellationToken).ConfigureAwait(false);
                Log.Information("Verification successful. Total Invoices: {InvoiceCount}, Total Details: {DetailCount}", totalInvoices, totalDetails);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during database verification in EntryDataDSContext.");
            Assert.Fail($"Database verification failed: {ex.Message}");
        }

        Log.Information("=== Test Completed Successfully: ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest ===");
    }


    // --- Helper Methods ---
    private string CleanFileNameForComparison(string originalName)
    {
        if (string.IsNullOrWhiteSpace(originalName)) return "unknown.file";
        string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        string cleanedName = originalName;
        foreach (char c in invalidChars) { cleanedName = cleanedName.Replace(c.ToString(), "-"); }
        cleanedName = System.Text.RegularExpressions.Regex.Replace(cleanedName, @"\s+", " ").Trim().Trim('-');
        return cleanedName;
    }

    private async Task SimulateStoringEmailInDb(Email emailData, string mailkitUidString, int appSettingsId)
    {
        if (emailData == null || string.IsNullOrEmpty(emailData.Subject)) { Log.Warning("SimulateStoringEmailInDb: emailData or Subject is null/empty."); return; }
        if (!uint.TryParse(mailkitUidString, out var uidUint)) { Log.Warning("SimulateStoringEmailInDb: Failed to parse MailKit UID '{MailkitUid}' to uint. Using 0.", mailkitUidString); uidUint = 0; }

        using (var dbCtx = new CoreEntitiesContext()) // Ensure connection string is correct
        {
            try
            {
                bool exists = await dbCtx.Emails.AnyAsync(e =>
                                      e.Subject == emailData.Subject &&
                                      e.ApplicationSettingsId == appSettingsId &&
                                      e.EmailUniqueId == (int)uidUint // Cast uint to int for DB
                                  ).ConfigureAwait(false);

                if (!exists)
                {
                    var emailEntity = new Emails
                    {
                        ApplicationSettingsId = appSettingsId,
                        Subject = emailData.Subject,
                        EmailDate = emailData.EmailDate.ToUniversalTime(),
                        EmailUniqueId = (int)uidUint,
                        EmailId = $"simulated-{Guid.NewGuid()}",
                        MachineName = Environment.MachineName,
                        // TimeStamp = DateTime.Now // Set if required by DB
                    };
                    dbCtx.Emails.Add(emailEntity);
                    await dbCtx.SaveChangesAsync().ConfigureAwait(false);
                    Log.Information("Simulated storing email in DB: Subject='{Subject}', AppSettingId={AppSettingId}, UID={Uid}", emailData.Subject, appSettingsId, uidUint);
                }
                else Log.Debug("Email already simulated in DB: Subject='{Subject}', AppSettingId={AppSettingId}, UID={Uid}", emailData.Subject, appSettingsId, uidUint);
            }
            catch (Exception ex) { Log.Error(ex, "Error simulating storing email in DB for Subject: {Subject}", emailData.Subject); }
        }
    }

    // SimulateStoringEmailInDbForForwarding (Keep as is if used by other tests)
    private async Task SimulateStoringEmailInDbForForwarding(Email emailData, uint mailkitUid, string appSpecificEmailId, int appSettingsId)
    {
        if (emailData == null || string.IsNullOrEmpty(emailData.Subject)) { Log.Warning("SimulateStoringEmailInDbForForwarding: emailData or Subject is null/empty."); return; }
        if (string.IsNullOrEmpty(appSpecificEmailId)) { Log.Warning("SimulateStoringEmailInDbForForwarding: appSpecificEmailId is null or empty."); return; }

        using (var dbCtx = new CoreEntitiesContext()) // Ensure connection string is correct
        {
            try
            {
                bool exists = await dbCtx.Emails.AnyAsync(e => e.EmailId == appSpecificEmailId && e.ApplicationSettingsId == appSettingsId).ConfigureAwait(false);
                if (!exists)
                {
                    var emailEntity = new Emails
                    {
                        EmailId = appSpecificEmailId,
                        ApplicationSettingsId = appSettingsId,
                        Subject = emailData.Subject,
                        EmailDate = emailData.EmailDate.ToUniversalTime(),
                        EmailUniqueId = (int)mailkitUid,
                        MachineName = Environment.MachineName,
                        // TimeStamp = DateTime.Now // Set if required by DB
                    };
                    dbCtx.Emails.Add(emailEntity);
                    await dbCtx.SaveChangesAsync().ConfigureAwait(false);
                    Log.Information("Simulated storing email for forwarding: AppEmailId='{AppEmailId}', AppSettingId={AppSettingId}, UID={Uid}", appSpecificEmailId, appSettingsId, mailkitUid);
                }
                else Log.Debug("Email for forwarding already simulated in DB: AppEmailId='{AppEmailId}', AppSettingId={AppSettingId}", appSpecificEmailId, appSettingsId);
            }
            catch (Exception ex) { Log.Error(ex, "Error simulating storing email for forwarding with AppEmailId: {AppEmailId}", appSpecificEmailId); }
        }
    }

} // End Class