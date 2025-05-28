using System;
using System.Collections.Generic;
using System.Diagnostics; // Added for Stopwatch
using Serilog.Context; // Added for LogContext
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoBot;
using CoreEntities.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.NUnit;
using Core.Common.Extensions; // Assuming this is where LogCategory and TypedLoggerExtensions reside
using Serilog.Events;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class EmailDownloaderIntegrationTests
    {
        private static ILogger _log;
        private string invocationId;

        private static string _dbConfiguredEmailAddress;
        private static string _dbConfiguredEmailPassword;
        private static int _dbConfiguredApplicationSettingsId;
        private static string _resolvedImapServer;
        private static int _resolvedImapPort;
        private static SecureSocketOptions _resolvedImapOptions;
        private static string _resolvedSmtpServer;
        private static int _resolvedSmtpPort;
        private static SecureSocketOptions _resolvedSmtpOptions;
        private static string _testSenderName = "Integration Test Bot";
        private static string _testReceiverEmailAddress;
        private static string _testDataFolder;
        private static EmailDownloader.Client _testClientForDownloader;
        private const string TestAppSettingSoftwareNameKey = "AutoBotIntegrationTestSource";
        private const string TestPdfFilePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogFilterState.TargetSourceContextForDetails = "WaterNut.DataSpace.PipelineInfrastructure.ReadFormattedTextStep";
            LogFilterState.TargetMethodNameForDetails = "Execute";
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose; // Set to Verbose for detailed debugging
            LogFilterState.EnabledCategoryLevels = new Dictionary<LogCategory, LogEventLevel>
            {
                { LogCategory.MethodBoundary, LogEventLevel.Information },
                { LogCategory.ActionBoundary, LogEventLevel.Information },
                { LogCategory.ExternalCall, LogEventLevel.Information },
                { LogCategory.StateChange, LogEventLevel.Information },
                { LogCategory.Security, LogEventLevel.Information },
                { LogCategory.MetaLog, LogEventLevel.Warning },
                { LogCategory.InternalStep, LogEventLevel.Information }, // Changed from Warning to Information
                { LogCategory.DiagnosticDetail, LogEventLevel.Information }, // Changed from Warning to Information
                { LogCategory.Performance, LogEventLevel.Warning },
                { LogCategory.Undefined, LogEventLevel.Information }
            };

            _log = new LoggerConfiguration()
                .MinimumLevel.Error() // Changed to Verbose to allow all logs to pass to the filter
                .Enrich.FromLogContext().Enrich.WithProperty("TestFixture", nameof(EmailDownloaderIntegrationTests))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.NUnitOutput(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}")
                .WriteTo.File("EmailDownloaderIntegrationTests.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}")
                .Filter.ByIncludingOnly(evt =>
                {
                    LogCategory category = LogCategory.Undefined;
                    string sourceContext = evt.Properties.TryGetValue("SourceContext", out var scP) && scP is ScalarValue scV && scV.Value != null ? scV.Value.ToString().Trim('"') : "";
                    string memberName = evt.Properties.TryGetValue("MemberName", out var mnP) && mnP is ScalarValue mnV && mnV.Value != null ? mnV.Value.ToString().Trim('"') : "";
                    if (evt.Properties.TryGetValue("LogCategory", out var categoryProp) && categoryProp is ScalarValue svCat && svCat.Value is LogCategory catVal) { category = catVal; }
                    else if (evt.Properties.TryGetValue("LogCategory", out var categoryPropStr) && categoryPropStr is ScalarValue svCatStr && svCatStr.Value != null && Enum.TryParse<LogCategory>(svCatStr.Value.ToString().Trim('"'), out var catValStr)) { category = catValStr; }
                    if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails) && !string.IsNullOrEmpty(sourceContext) && sourceContext.Contains(LogFilterState.TargetSourceContextForDetails))
                    {
                        bool methodMatch = string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) || (!string.IsNullOrEmpty(memberName) && memberName.Equals(LogFilterState.TargetMethodNameForDetails, StringComparison.OrdinalIgnoreCase));
                        if (methodMatch) { return evt.Level >= LogFilterState.DetailTargetMinimumLevel; }
                    }
                    if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var enabledMinLevelForCategory)) { return evt.Level >= enabledMinLevelForCategory; }
                    return false;
                })
                .CreateLogger();
            Log.Logger = _log;

            _log.LogInfoCategorized(LogCategory.Undefined, "=== EmailDownloaderIntegrationTests OneTimeSetup Starting ===", invocationId: null);
            _log.LogInfoCategorized(LogCategory.Undefined, "Attempting to load test account configuration from database...", invocationId: null);

            ApplicationSettings testAppSettingFromDb = null;
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    testAppSettingFromDb = ctx.ApplicationSettings
                        .Include(x => x.FileTypes).Include(x => x.Declarants)
                        .Include("FileTypes.FileTypeReplaceRegex").Include("FileTypes.FileImporterInfos")
                        .Include(x => x.EmailMapping)
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos")
                        .Include("EmailMapping.EmailMappingRexExs").Include("EmailMapping.EmailMappingActions.Actions")
                        .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                        .FirstOrDefault(s => s.IsActive);

                    if (testAppSettingFromDb != null)
                    {
                        _dbConfiguredEmailAddress = testAppSettingFromDb.Email; _dbConfiguredEmailPassword = testAppSettingFromDb.EmailPassword; _dbConfiguredApplicationSettingsId = testAppSettingFromDb.ApplicationSettingsId;
                        if (string.IsNullOrEmpty(_dbConfiguredEmailAddress) || string.IsNullOrEmpty(_dbConfiguredEmailPassword))
                        {
                            var configError = $"Test ApplicationSetting record found (ID: {testAppSettingFromDb.ApplicationSettingsId}, Name: '{testAppSettingFromDb.SoftwareName}') but Email or EmailPassword fields are empty.";
                            _log.LogErrorCategorized(LogCategory.Undefined, configError, invocationId: null);
                            Assert.Inconclusive(configError); return;
                        }
                        _log.LogInfoCategorized(LogCategory.Undefined, "Successfully loaded Email/Password for AppSettingId: {AppSettingId} (Email: {EmailAddress}) from database.", invocationId: null, propertyValues: new object[] { _dbConfiguredApplicationSettingsId, _dbConfiguredEmailAddress });
                    }
                    else
                    {
                        var dbErrorMessage = $"Test ApplicationSetting record identified by IsActive=true was not found in the database. Please ensure this record exists and is active in your test database.";
                        _log.LogErrorCategorized(LogCategory.Undefined, dbErrorMessage, invocationId: null);
                        Assert.Inconclusive(dbErrorMessage); return;
                    }
                }
            }
            catch (Exception ex)
            {
                var dbReadError = $"CRITICAL ERROR reading test email credentials from the database: {ex.Message}\nEnsure the database is accessible and the ApplicationSettings table can be queried.\nConnection string being used by CoreEntitiesContext should point to the TEST DATABASE.";
                _log.LogErrorCategorized(LogCategory.Undefined, ex, dbReadError, invocationId: null);
                Assert.Inconclusive(dbReadError); return;
            }

            _testReceiverEmailAddress = _dbConfiguredEmailAddress;
            _log.LogInfoCategorized(LogCategory.Undefined, "Deriving server settings using EmailDownloader logic...", invocationId: null);
            try
            {
                var readSettings = EmailDownloader.EmailDownloader.GetReadMailSettings(_dbConfiguredEmailAddress, _log);
                _resolvedImapServer = readSettings.Server; _resolvedImapPort = readSettings.Port; _resolvedImapOptions = readSettings.Options;
                var sendSettings = EmailDownloader.EmailDownloader.GetSendMailSettings(_dbConfiguredEmailAddress, _log);
                _resolvedSmtpServer = sendSettings.Server; _resolvedSmtpPort = sendSettings.Port; _resolvedSmtpOptions = sendSettings.Options;
                _log.LogInfoCategorized(LogCategory.Undefined, "Derived IMAP: {ImapServer}:{ImapPort} ({ImapOptions})", invocationId: null, propertyValues: new object[] { _resolvedImapServer, _resolvedImapPort, _resolvedImapOptions });
                _log.LogInfoCategorized(LogCategory.Undefined, "Derived SMTP: {SmtpServer}:{SmtpPort} ({SmtpOptions})", invocationId: null, propertyValues: new object[] { _resolvedSmtpServer, _resolvedSmtpPort, _resolvedSmtpOptions });
            }
            catch (Exception ex)
            {
                var deriveError = $"Failed to derive email server settings for '{_dbConfiguredEmailAddress}'.";
                _log.LogErrorCategorized(LogCategory.Undefined, ex, deriveError, invocationId: null);
                Assert.Inconclusive($"{deriveError} Error: {ex.Message}"); return;
            }

            _testDataFolder = Path.Combine(Path.GetTempPath(), "EmailDownloaderTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDataFolder);
            _log.LogInfoCategorized(LogCategory.Undefined, "Test Data Folder: {TestDataFolder}", invocationId: null, propertyValues: new object[] { _testDataFolder });

            string companyName = testAppSettingFromDb?.CompanyName ?? _testSenderName;
            _testClientForDownloader = new EmailDownloader.Client { Email = _dbConfiguredEmailAddress, Password = _dbConfiguredEmailPassword, ApplicationSettingsId = _dbConfiguredApplicationSettingsId, DataFolder = _testDataFolder, CompanyName = companyName, NotifyUnknownMessages = true, DevMode = false, EmailMappings = new List<EmailMapping>() };

            if (testAppSettingFromDb?.EmailMapping != null)
            {
                _testClientForDownloader.EmailMappings.AddRange(testAppSettingFromDb.EmailMapping);
                _log.LogInfoCategorized(LogCategory.Undefined, "Added {Count} EmailMappings from database AppSettingId: {AppSettingId}", invocationId: null, propertyValues: new object[] { testAppSettingFromDb.EmailMapping.Count, _dbConfiguredApplicationSettingsId });
                if (!_testClientForDownloader.EmailMappings.Any(em => em.Pattern.Contains("PDF Import Test")))
                {
                    _log.LogWarningCategorized(LogCategory.Undefined, "Did not find 'PDF Import Test' pattern in DB mappings. Adding a default for the test.", invocationId: null);
                    _testClientForDownloader.EmailMappings.Add(new EmailMapping { Id = -1, Pattern = @"^PDF Import Test (?<Identifier>.*)" });
                }
            }
            else
            {
                _log.LogWarningCategorized(LogCategory.Undefined, "No EmailMappings found in the loaded ApplicationSettings (ID: {AppSettingId}). Adding default mappings for tests.", invocationId: null, propertyValues: new object[] { _dbConfiguredApplicationSettingsId });
                _testClientForDownloader.EmailMappings.AddRange(new List<EmailMapping> { new EmailMapping { Id = -2, Pattern = @"^Test Email Subject (?<Identifier>.*)" }, new EmailMapping { Id = -3, Pattern = @"^FWD: (?<OriginalSubject>.*)" }, new EmailMapping { Id = -4, Pattern = @"^PDF Import Test (?<Identifier>.*)" } });
            }
            _log.LogInfoCategorized(LogCategory.Undefined, "Test client configured for EmailDownloader operations using AppSettingId: {AppSettingId}", invocationId: null, propertyValues: new object[] { _testClientForDownloader.ApplicationSettingsId });
            _log.LogInfoCategorized(LogCategory.Undefined, "=== EmailDownloaderIntegrationTests OneTimeSetup Complete ===", invocationId: null);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _log.LogInfoCategorized(LogCategory.Undefined, "=== EmailDownloaderIntegrationTests OneTimeTearDown Starting ===", invocationId: null);
            if (Directory.Exists(_testDataFolder))
            {
                try { Directory.Delete(_testDataFolder, true); _log.LogInfoCategorized(LogCategory.Undefined, "Deleted test data folder: {TestDataFolder}", invocationId: null, propertyValues: new object[] { _testDataFolder }); }
                catch (Exception ex) { _log.LogWarningCategorized(LogCategory.Undefined, ex, "Error deleting test folder {TestDataFolder}", invocationId: null, propertyValues: new object[] { _testDataFolder }); }
            }
            Log.CloseAndFlush();
            Console.WriteLine("=== EmailDownloaderIntegrationTests OneTimeTearDown Complete ===");
        }

        [SetUp]
        public void SetupForEachTest()
        {
            invocationId = Guid.NewGuid().ToString();
            _log.LogInfoCategorized(LogCategory.InternalStep, "--- Test Setup Starting: {TestName} ---", invocationId: this.invocationId, propertyValues: new object[] { TestContext.CurrentContext.Test.Name });
            EmailDownloader.EmailDownloader.ReturnOnlyUnknownMails = false; _testClientForDownloader.NotifyUnknownMessages = true;
            _log.LogInfoCategorized(LogCategory.InternalStep, "--- Test Setup Complete: {TestName} ---", invocationId: this.invocationId, propertyValues: new object[] { TestContext.CurrentContext.Test.Name });
        }

        private async Task SendTestEmailAsync(string subject, string body, string[] attachments = null, string toAddress = null)
        {
            toAddress = toAddress ?? _dbConfiguredEmailAddress; attachments = attachments ?? Array.Empty<string>();
            var message = new MimeMessage(); message.From.Add(new MailboxAddress(_testSenderName, _dbConfiguredEmailAddress)); message.To.Add(MailboxAddress.Parse(toAddress)); message.Subject = subject;
            var bodyBuilder = new BodyBuilder { TextBody = body };
            foreach (var attPath in attachments)
            {
                if (File.Exists(attPath)) { bodyBuilder.Attachments.Add(attPath); _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Adding attachment: {AttachmentPath}", this.invocationId, propertyValues: new object[] { attPath }); }
                else _log.LogWarningCategorized(LogCategory.Undefined, "Test setup warning: Attachment not found, skipping: {AttachmentPath}", this.invocationId, propertyValues: new object[] { attPath });
            }
            message.Body = bodyBuilder.ToMessageBody();
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Connecting SMTP: {SmtpServer}:{SmtpPort} ({SmtpOptions})", this.invocationId, propertyValues: new object[] { _resolvedSmtpServer, _resolvedImapPort, _resolvedImapOptions });
                    await smtpClient.ConnectAsync(_resolvedSmtpServer, _resolvedSmtpPort, _resolvedSmtpOptions).ConfigureAwait(false);
                    _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Authenticating SMTP: {EmailAddress}", this.invocationId, propertyValues: new object[] { _dbConfiguredEmailAddress });
                    await smtpClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword).ConfigureAwait(false);
                    _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Sending SMTP: To={ToAddress}, Subject={Subject}", this.invocationId, propertyValues: new object[] { toAddress, subject });
                    await smtpClient.SendAsync(message).ConfigureAwait(false); await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
                    _log.LogInfoCategorized(LogCategory.Undefined, "Sent test email. Subject: '{Subject}' To: '{ToAddress}'", this.invocationId, propertyValues: new object[] { subject, toAddress });
                }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Failed to send test email. Subject: {Subject}, To: {ToAddress}", this.invocationId, propertyValues: new object[] { subject, toAddress }); throw; }
            }
            _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Waiting 5 seconds for email server processing...", this.invocationId);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        private async Task<ImapClient> GetTestImapClientAsync(CancellationToken cancellationToken = default)
        {
            var imapClient = new ImapClient();
            try
            {
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Connecting IMAP: {ImapServer}:{ImapPort} ({ImapOptions})", this.invocationId, propertyValues: new object[] { _resolvedImapServer, _resolvedImapPort, _resolvedImapOptions });
                await imapClient.ConnectAsync(_resolvedImapServer, _resolvedImapPort, _resolvedImapOptions, cancellationToken).ConfigureAwait(false);
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Authenticating IMAP: {EmailAddress}", this.invocationId, propertyValues: new object[] { _dbConfiguredEmailAddress });
                await imapClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword, cancellationToken).ConfigureAwait(false);
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Opening IMAP Inbox (ReadWrite)", this.invocationId);
                await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
                _log.LogInfoCategorized(LogCategory.Undefined, "IMAP client connected & Inbox opened: {EmailAddress}", this.invocationId, propertyValues: new object[] { _dbConfiguredEmailAddress });
                return imapClient;
            }
            catch (Exception e)
            {
                _log.LogErrorCategorized(LogCategory.Undefined, e, "GetTestImapClientAsync Error for {EmailAddress} on {ImapServer}", this.invocationId, propertyValues: new object[] { _dbConfiguredEmailAddress, _resolvedImapServer });
                try { await imapClient.DisconnectAsync(false, CancellationToken.None).ConfigureAwait(false); } catch { /* Ignored */ }
                imapClient?.Dispose(); return null;
            }
        }

        [Test, Order(1)]
        public async Task A_CanSendAndReceiveBasicEmail_StreamEmailResultsAsync()
        {
            _log.LogMethodEntry(this.invocationId);
            var stopwatch = Stopwatch.StartNew();
            _log.LogInfoCategorized(LogCategory.Undefined, "Executing Test: A_CanSendAndReceiveBasicEmail_StreamEmailResultsAsync", this.invocationId);
            string uniqueContent = Guid.NewGuid().ToString(); string testSubject = $"Test Email Subject {uniqueContent}"; string testBody = $"This is a test email body. Content: {uniqueContent}";
            await this.SendTestEmailAsync(testSubject, testBody).ConfigureAwait(false);
            var receivedEmails = new List<EmailProcessingResult>(); ImapClient imapClientForTest = null;
            try
            {
                imapClientForTest = await this.GetTestImapClientAsync(CancellationToken.None).ConfigureAwait(false);
                Assert.That(imapClientForTest, Is.Not.Null, "IMAP client for test should be connected and authenticated.");
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Starting StreamEmailResultsAsync to find email: {Subject}", this.invocationId, propertyValues: new object[] { testSubject });
                var streamEmailResultsAsync = EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClientForTest, _testClientForDownloader, _log, CancellationToken.None);
                foreach (var emailTask in streamEmailResultsAsync)
                {
                    var result = await emailTask.ConfigureAwait(false);
                    if (result != null)
                    {
                        _log.LogVerboseCategorized(LogCategory.DiagnosticDetail, "StreamEmailResultsAsync yielded: Subject='{Subject}', UID={Uid}", this.invocationId, propertyValues: new object[] { result.EmailKey.EmailMessage?.Subject, result.EmailKey.UidString });
                        if (result.EmailKey.EmailMessage?.Subject == testSubject)
                        {
                            _log.LogInfoCategorized(LogCategory.Undefined, "Found matching email: {Subject}", this.invocationId, propertyValues: new object[] { testSubject });
                            receivedEmails.Add(result);
                        }
                    }
                    else _log.LogVerboseCategorized(LogCategory.DiagnosticDetail, "StreamEmailResultsAsync yielded null.", this.invocationId);
                }
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Finished StreamEmailResultsAsync loop.", this.invocationId);
            }
            catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during StreamEmailResultsAsync processing in test A.", this.invocationId); Assert.Fail($"Test failed during email processing: {ex.Message}"); }
            finally
            {
                if (imapClientForTest != null && imapClientForTest.IsConnected) { _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Disconnecting test IMAP client.", this.invocationId); await imapClientForTest.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false); }
                imapClientForTest?.Dispose();
            }
            Assert.That(receivedEmails, Is.Not.Empty, $"Expected to find email with subject '{testSubject}'.");
            var foundEmail = receivedEmails.First();
            Assert.That(foundEmail.EmailKey, Is.Not.Null); Assert.That(foundEmail.EmailKey.EmailMessage, Is.Not.Null); Assert.That(foundEmail.EmailKey.UidString, Is.Not.Null.Or.Empty); Assert.That(foundEmail.EmailKey.SubjectIdentifier, Is.Not.Null.Or.Empty);
            Assert.That(foundEmail.EmailKey.SubjectIdentifier.Trim(), Is.EqualTo(uniqueContent.Trim()), "Subject key (SubjectIdentifier) mismatch.");
            string subjectIdTrimmed = foundEmail.EmailKey.SubjectIdentifier?.Trim(); string uidString = foundEmail.EmailKey.UidString ?? "NULL_UID";
            Assert.That(!string.IsNullOrEmpty(subjectIdTrimmed), "SubjectIdentifier was empty after trimming.");
            string expectedDir = Path.Combine(_testDataFolder, subjectIdTrimmed, uidString); string infoTxtPath = Path.Combine(expectedDir, "Info.txt");
            _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Checking for Info.txt at: {InfoTxtPath}", this.invocationId, propertyValues: new object[] { infoTxtPath });
            Assert.That(File.Exists(infoTxtPath), Is.True, $"Info.txt not found at expected path: {infoTxtPath}");
            _log.LogInfoCategorized(LogCategory.Undefined, "Test A verification successful.", this.invocationId);
            stopwatch.Stop(); _log.LogMethodExitSuccess(this.invocationId, stopwatch.ElapsedMilliseconds);
        }

        [Test, Order(6)]
        public async Task ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()
        {
            var stopwatch = Stopwatch.StartNew();
            _log.LogMetaDirective(LogEventLevel.Warning, "ObjectiveConfirmation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest", "User confirmed the iteration objective: Make this test pass.", "LLM_Iter_0.1", invocationId: this.invocationId);
            _log.LogMetaDirective(LogEventLevel.Warning, "Analysis", "PlanStep_1.1", "Reading EmailDownloaderIntegrationTests.cs for initial instrumentation planning.", "LLM_Iter_0.1_PlanStep_4.6", invocationId: this.invocationId);
            _log.LogActionStart(this.invocationId, "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Execution");
            _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Entry", "Added InvocationId setup and ACTION_START log.", "LLM_Iter_0.1_PlanStep_4.6", expectedChange: "InvocationId and ACTION_START logged at test start.", invocationId: this.invocationId);
            _log.LogMethodEntry(this.invocationId);
            _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Entry", "Added METHOD_ENTRY log.", "LLM_Iter_0.1_PlanStep_4.6", expectedChange: "METHOD_ENTRY logged.", invocationId: this.invocationId);
            _log.LogInfoCategorized(LogCategory.InternalStep, "Test execution commencing for ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest", this.invocationId);
            try
            {
                _log.LogActionStart(this.invocationId, "ArrangePhase_ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest", parentAction: "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Execution");
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_Start", "Added ACTION_START for Arrange phase.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.1", expectedChange: "ACTION_START for ArrangePhase logged.", invocationId: this.invocationId);
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Verifying test PDF file exists at {FilePath}", this.invocationId, propertyValues: new object[] { TestPdfFilePath });
                Assert.That(File.Exists(TestPdfFilePath), Is.True, $"Test PDF file not found at: {TestPdfFilePath}");
                _log.LogInfoCategorized(LogCategory.InternalStep, "Verified test PDF file existence; FileName: {FileName}; FileExists: {FileExists}", this.invocationId, propertyValues: new object[] { TestPdfFilePath, File.Exists(TestPdfFilePath) });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_PdfFileVerification", "Added INTERNAL_STEP for PDF file verification.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.2", expectedChange: "INTERNAL_STEP for PDF file verification logged.", invocationId: this.invocationId);
                string uniqueContent = Guid.NewGuid().ToString(); string testSubject = $"PDF Import Test {uniqueContent}"; string testBody = "Integration test email with PDF attachment for ProcessEmailsAsync."; string invoiceNumberToVerify = "114-7827932-2029910";
                _log.LogInfoCategorized(LogCategory.InternalStep, "Starting database cleanup for EntryDataDS; TargetInvoiceNumber: {TargetInvoiceNumber}", this.invocationId, propertyValues: new object[] { invoiceNumberToVerify });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_DbCleanupStart", "Added INTERNAL_STEP for database cleanup start.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.3", expectedChange: "INTERNAL_STEP for database cleanup start logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.Undefined, "Cleaning up previous test data for InvoiceNo: {InvoiceNumber} in EntryDataDS...", this.invocationId, propertyValues: new object[] { invoiceNumberToVerify });
                try { using (var entryCtx = new EntryDataDSContext()) { var existingDetails = await entryCtx.ShipmentInvoiceDetails.Where(d => d.Invoice.InvoiceNo == invoiceNumberToVerify).ToListAsync().ConfigureAwait(false); if (existingDetails.Any()) { _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Removing {Count} existing ShipmentInvoiceDetails.", this.invocationId, propertyValues: new object[] { existingDetails.Count }); entryCtx.ShipmentInvoiceDetails.RemoveRange(existingDetails); } var existingInvoice = await entryCtx.ShipmentInvoice.FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNumberToVerify).ConfigureAwait(false); if (existingInvoice != null) { _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Removing existing ShipmentInvoice.", this.invocationId); entryCtx.ShipmentInvoice.Remove(existingInvoice); } await entryCtx.SaveChangesAsync().ConfigureAwait(false); _log.LogInfoCategorized(LogCategory.Undefined, "EntryDataDS cleanup complete.", this.invocationId); } }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during EntryDataDS database cleanup.", this.invocationId); Assert.Fail($"Database cleanup failed: {ex.Message}"); }
                _log.LogInfoCategorized(LogCategory.InternalStep, "Finished database cleanup for EntryDataDS; TargetInvoiceNumber: {TargetInvoiceNumber}", this.invocationId, propertyValues: new object[] { invoiceNumberToVerify });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_DbCleanupEnd", "Added INTERNAL_STEP for database cleanup end.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.3", expectedChange: "INTERNAL_STEP for database cleanup end logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.InternalStep, "Preparing to send test email with PDF attachment; EmailSubject: {EmailSubject}; AttachmentsCount: {AttachmentsCount}", this.invocationId, propertyValues: new object[] { testSubject, 1 });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_SendEmailStart", "Added INTERNAL_STEP for starting send test email.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.4", expectedChange: "INTERNAL_STEP for send test email start logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.Undefined, "Sending test email with PDF attachment...", this.invocationId);
                await this.SendTestEmailAsync(testSubject, testBody, new[] { TestPdfFilePath }).ConfigureAwait(false);
                _log.LogInfoCategorized(LogCategory.Undefined, "Simulating initial email download to get UID and verify attachment presence...", this.invocationId);
                EmailProcessingResult processedEmailResult = null; ImapClient imapClientForDownload = null;
                _log.LogInfoCategorized(LogCategory.InternalStep, "Starting simulation of initial email download to get UID and verify attachment presence", this.invocationId);
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_SimulateDownloadStart", "Added INTERNAL_STEP for starting initial email download simulation.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.5", expectedChange: "INTERNAL_STEP for initial email download simulation start logged.", invocationId: this.invocationId);
                string downloadedPdfPath = null; UniqueId targetEmailUid = UniqueId.Invalid;
                try
                {
                    imapClientForDownload = await this.GetTestImapClientAsync(CancellationToken.None).ConfigureAwait(false); Assert.That(imapClientForDownload, Is.Not.Null, "IMAP client for initial download failed.");
                    var streamEmailResultsAsync = EmailDownloader.EmailDownloader.StreamEmailResultsAsync(imapClientForDownload, _testClientForDownloader, _log, CancellationToken.None);
                    foreach (var emailTask in streamEmailResultsAsync) { var result = await emailTask.ConfigureAwait(false); if (result != null && result.EmailKey.EmailMessage?.Subject == testSubject) { processedEmailResult = result; _log.LogInfoCategorized(LogCategory.Undefined, "Found and processed test email during initial download simulation: Subject='{Subject}', UID={Uid}", this.invocationId, propertyValues: new object[] { testSubject, result.EmailKey.UidString }); if (!UniqueId.TryParse(result.EmailKey.UidString, out targetEmailUid)) { _log.LogErrorCategorized(LogCategory.Undefined, "Failed to parse UID string '{UidString}' from processed email result.", this.invocationId, propertyValues: new object[] { result.EmailKey.UidString }); Assert.Fail($"Could not parse UID '{result.EmailKey.UidString}' needed for the test."); } _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Successfully parsed target email UID: {Uid}", this.invocationId, propertyValues: new object[] { targetEmailUid }); if (result.EmailKey.EmailMessage != null && !string.IsNullOrEmpty(result.EmailKey.UidString)) { await this.SimulateStoringEmailInDb(result.EmailKey.EmailMessage, result.EmailKey.UidString, _testClientForDownloader.ApplicationSettingsId).ConfigureAwait(false); } else _log.LogWarningCategorized(LogCategory.Undefined, "Could not simulate storing email in DB due to missing EmailMessage or UidString.", this.invocationId); Assert.That(processedEmailResult.AttachedFiles, Is.Not.Null.And.Not.Empty, "No attached files found in processed result."); var pdfAttachmentInfo = processedEmailResult.AttachedFiles.FirstOrDefault(f => f.Name?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ?? false); Assert.That(pdfAttachmentInfo, Is.Not.Null, "PDF attachment info not found in result."); string subjectIdTrimmed = processedEmailResult.EmailKey.SubjectIdentifier?.Trim(); string uidString = processedEmailResult.EmailKey.UidString; Assert.That(!string.IsNullOrEmpty(subjectIdTrimmed), "SubjectIdentifier is empty."); Assert.That(!string.IsNullOrEmpty(uidString), "UID string is empty."); string expectedDir = Path.Combine(_testDataFolder, subjectIdTrimmed, uidString); downloadedPdfPath = Path.Combine(expectedDir, pdfAttachmentInfo.Name); _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Checking for downloaded PDF at: {PdfPath}", this.invocationId, propertyValues: new object[] { downloadedPdfPath }); Assert.That(File.Exists(downloadedPdfPath), Is.True, $"Downloaded PDF not found at expected path: {downloadedPdfPath}."); _log.LogInfoCategorized(LogCategory.Undefined, "Verified downloaded PDF exists at: {PdfPath}", this.invocationId, propertyValues: new object[] { downloadedPdfPath }); break; } }
                    Assert.That(processedEmailResult, Is.Not.Null, $"Test email '{testSubject}' not found/processed during initial download simulation."); Assert.That(targetEmailUid.IsValid, Is.True, "Target Email UID was not successfully obtained.");
                }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during initial email download simulation phase.", this.invocationId); Assert.Fail($"Initial email download simulation failed: {ex.Message}"); }
                finally { if (imapClientForDownload != null && imapClientForDownload.IsConnected) { _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Disconnecting IMAP client used for initial download simulation.", this.invocationId); await imapClientForDownload.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false); } imapClientForDownload?.Dispose(); }
                _log.LogInfoCategorized(LogCategory.InternalStep, "Finished simulation of initial email download; EmailSubject: {EmailSubject}; TargetEmailUid: {TargetEmailUid}; DownloadedPdfPath: {DownloadedPdfPath}", this.invocationId, propertyValues: new object[] { testSubject, targetEmailUid.ToString(), downloadedPdfPath });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_SimulateDownloadEnd", "Added INTERNAL_STEP for finishing initial email download simulation.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.5", expectedChange: "INTERNAL_STEP for initial email download simulation end logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.InternalStep, "Starting to mark email as UNREAD; TargetEmailUid: {TargetEmailUid}", this.invocationId, propertyValues: new object[] { targetEmailUid.ToString() });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_MarkUnreadStart", "Added INTERNAL_STEP for starting to mark email as UNREAD.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.6", expectedChange: "INTERNAL_STEP for marking email as UNREAD start logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.Undefined, "Attempting to mark email UID {Uid} as UNREAD before calling ProcessEmailsAsync.", this.invocationId, propertyValues: new object[] { targetEmailUid });
                ImapClient markUnreadClient = null;
                try { markUnreadClient = new ImapClient(); await markUnreadClient.ConnectAsync(_resolvedImapServer, _resolvedImapPort, _resolvedImapOptions, CancellationToken.None).ConfigureAwait(false); await markUnreadClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword, CancellationToken.None).ConfigureAwait(false); await markUnreadClient.Inbox.OpenAsync(FolderAccess.ReadWrite, CancellationToken.None).ConfigureAwait(false); await markUnreadClient.Inbox.RemoveFlagsAsync(targetEmailUid, MessageFlags.Seen, true, CancellationToken.None).ConfigureAwait(false); _log.LogInfoCategorized(LogCategory.Undefined, "Successfully marked email UID {Uid} as UNREAD.", this.invocationId, propertyValues: new object[] { targetEmailUid }); await markUnreadClient.DisconnectAsync(true, CancellationToken.None).ConfigureAwait(false); }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Failed to mark email UID {Uid} as UNREAD.", this.invocationId, propertyValues: new object[] { targetEmailUid }); if (markUnreadClient != null && markUnreadClient.IsConnected) { try { await markUnreadClient.DisconnectAsync(false, CancellationToken.None).ConfigureAwait(false); } catch { /* Ignore */ } } Assert.Fail($"Failed to mark email as unread before ProcessEmailsAsync call: {ex.Message}"); }
                finally { markUnreadClient?.Dispose(); }
                _log.LogInfoCategorized(LogCategory.InternalStep, "Finished marking email as UNREAD; TargetEmailUid: {TargetEmailUid}; MarkedAsUnread: {MarkedAsUnread}", this.invocationId, propertyValues: new object[] { targetEmailUid.ToString(), true });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_MarkUnreadEnd", "Added INTERNAL_STEP for finishing marking email as UNREAD.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.6", expectedChange: "INTERNAL_STEP for marking email as UNREAD end logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.InternalStep, "Starting preparation for ProcessEmailsAsync call (loading ApplicationSettings); ApplicationSettingsId: {ApplicationSettingsId}", this.invocationId, propertyValues: new object[] { _dbConfiguredApplicationSettingsId });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_PrepareProcessEmailsStart", "Added INTERNAL_STEP for starting preparation for ProcessEmailsAsync call.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.7", expectedChange: "INTERNAL_STEP for ProcessEmailsAsync preparation start logged.", invocationId: this.invocationId);
                _log.LogInfoCategorized(LogCategory.Undefined, "Preparing to call EmailProcessor.ProcessEmailsAsync...", this.invocationId);
                ApplicationSettings appSetting = null; CoreEntitiesContext ctxForProcess = null;
                try
                {
                    ctxForProcess = new CoreEntitiesContext();
                    appSetting = await ctxForProcess.ApplicationSettings.Include(x => x.FileTypes).Include(x => x.Declarants).Include("FileTypes.FileTypeReplaceRegex").Include("FileTypes.FileImporterInfos").Include(x => x.EmailMapping).Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos").Include("EmailMapping.EmailMappingRexExs").Include("EmailMapping.EmailMappingActions.Actions").Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx").FirstOrDefaultAsync(a => a.ApplicationSettingsId == _dbConfiguredApplicationSettingsId).ConfigureAwait(false);
                    _log.LogInfoCategorized(LogCategory.InternalStep, "Finished preparation for ProcessEmailsAsync call; ApplicationSettingsId: {ApplicationSettingsId}; AppSettingLoaded: {AppSettingLoaded}", this.invocationId, propertyValues: new object[] { _dbConfiguredApplicationSettingsId, appSetting != null });
                    _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ArrangePhase_PrepareProcessEmailsEnd", "Added INTERNAL_STEP for finishing preparation for ProcessEmailsAsync call.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.1.7", expectedChange: "INTERNAL_STEP for ProcessEmailsAsync preparation end logged.", invocationId: this.invocationId);
                    Assert.That(appSetting, Is.Not.Null, $"Failed to retrieve ApplicationSettings ID: {_dbConfiguredApplicationSettingsId}");
                    _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Retrieved ApplicationSettings: ID={AppSettingId}, Name='{SoftwareName}'", this.invocationId, propertyValues: new object[] { appSetting.ApplicationSettingsId, appSetting.SoftwareName });
                }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error retrieving ApplicationSettings for ProcessEmailsAsync.", this.invocationId); ctxForProcess?.Dispose(); Assert.Fail($"Failed to prepare for ProcessEmailsAsync: {ex.Message}"); }
                _log.LogActionStart(this.invocationId, "ActPhase_ProcessEmailsAsync", parentAction: "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Execution");
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ActPhase_Start", "Added ACTION_START for Act phase (ProcessEmailsAsync call).", "LLM_Iter_0.1_PlanStep_4.6_Phase2.2.1", expectedChange: "ACTION_START for ActPhase logged.", invocationId: this.invocationId);
                DateTime beforeImport = DateTime.Now.AddMinutes(-1);
                _log.LogInfoCategorized(LogCategory.InternalStep, "Preparing to call EmailProcessor.ProcessEmailsAsync; AppSettingId: {AppSettingId}; BeforeImportTime: {BeforeImportTime}", this.invocationId, propertyValues: new object[] { appSetting.ApplicationSettingsId, beforeImport });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ActPhase_ProcessEmailsAsync_PreCall", "Added INTERNAL_STEP before ProcessEmailsAsync call.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.2.2", expectedChange: "INTERNAL_STEP logged.", invocationId: this.invocationId);
                _log.LogExternalCallStart(this.invocationId, "EmailProcessingLogic", "ProcessEmailsAsync", parameters: new { AppSettingId = appSetting.ApplicationSettingsId });
                _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ActPhase_ProcessEmailsAsync_ExternalCallStart", "Added EXTERNAL_CALL_START for ProcessEmailsAsync.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.2.3", expectedChange: "EXTERNAL_CALL_START logged.", invocationId: this.invocationId);
                CancellationToken cancellationToken = CancellationToken.None;
                _log.LogInfoCategorized(LogCategory.Undefined, "Calling EmailProcessor.ProcessEmailsAsync for AppSetting ID: {AppSettingId}...", this.invocationId, propertyValues: new object[] { appSetting.ApplicationSettingsId });
                try
                {
                    await EmailProcessor.ProcessEmailsAsync(appSetting, beforeImport, ctxForProcess, cancellationToken, _log).ConfigureAwait(false);
                    _log.LogInfoCategorized(LogCategory.Undefined, "EmailProcessor.ProcessEmailsAsync call completed.", this.invocationId);
                    _log.LogExternalCallEnd(this.invocationId, "EmailProcessingLogic", "ProcessEmailsAsync", true, stopwatch.ElapsedMilliseconds, result: "CallCompleted");
                    _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ActPhase_ProcessEmailsAsync_ExternalCallEnd", "Added EXTERNAL_CALL_END for ProcessEmailsAsync.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.2.4", expectedChange: "EXTERNAL_CALL_END logged.", invocationId: this.invocationId);
                    _log.LogInfoCategorized(LogCategory.InternalStep, "EmailProcessor.ProcessEmailsAsync finished; AppSettingId: {AppSettingId}", this.invocationId, propertyValues: new object[] { appSetting.ApplicationSettingsId });
                    _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ActPhase_ProcessEmailsAsync_PostCall", "Added INTERNAL_STEP after ProcessEmailsAsync call.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.2.5", expectedChange: "INTERNAL_STEP logged.", invocationId: this.invocationId);
                    _log.LogActionEndSuccess(this.invocationId, "ActPhase_ProcessEmailsAsync", stopwatch.ElapsedMilliseconds);
                    _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ActPhase_EndSuccess", "Added ACTION_END_SUCCESS for Act phase.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.2.6", expectedChange: "ACTION_END_SUCCESS for ActPhase logged.", invocationId: this.invocationId);
                }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Exception during EmailProcessor.ProcessEmailsAsync execution.", this.invocationId); _log.LogActionStart(this.invocationId, "AssertPhase_ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest", parentAction: "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Execution"); _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "AssertPhase_Start", "Added ACTION_START for Assert phase.", "LLM_Iter_0.1_PlanStep_4.6_Phase2.3.1", expectedChange: "ACTION_START for AssertPhase logged.", invocationId: this.invocationId); Assert.Fail($"ProcessEmailsAsync threw an exception: {ex.Message}\nStackTrace: {ex.StackTrace}"); }
                finally { ctxForProcess?.Dispose(); }
                _log.LogInfoCategorized(LogCategory.Undefined, "Verifying import results in database (EntryDataDSContext)...", this.invocationId);
                try
                {
                    using (var verifyCtx = new EntryDataDSContext())
                    {
                        _log.LogVerboseCategorized(LogCategory.DiagnosticDetail, "Checking for ShipmentInvoice '{InvoiceNumber}'", this.invocationId, propertyValues: new object[] { invoiceNumberToVerify });
                        bool invoiceExists = await verifyCtx.ShipmentInvoice.AnyAsync(x => x.InvoiceNo == invoiceNumberToVerify, cancellationToken).ConfigureAwait(false); Assert.That(invoiceExists, Is.True, $"ShipmentInvoice '{invoiceNumberToVerify}' not created.");
                        _log.LogVerboseCategorized(LogCategory.DiagnosticDetail, "ShipmentInvoice found: {Exists}", this.invocationId, propertyValues: new object[] { invoiceExists });
                        _log.LogVerboseCategorized(LogCategory.DiagnosticDetail, "Checking for ShipmentInvoiceDetails count > 2 for '{InvoiceNumber}'", this.invocationId, propertyValues: new object[] { invoiceNumberToVerify });
                        int detailCount = await verifyCtx.ShipmentInvoiceDetails.CountAsync(x => x.Invoice.InvoiceNo == invoiceNumberToVerify, cancellationToken).ConfigureAwait(false); Assert.That(detailCount > 2, Is.True, $"Expected > 2 ShipmentInvoiceDetails for '{invoiceNumberToVerify}', found {detailCount}.");
                        _log.LogVerboseCategorized(LogCategory.DiagnosticDetail, "ShipmentInvoiceDetails count: {Count}", this.invocationId, propertyValues: new object[] { detailCount });
                        int totalInvoices = await verifyCtx.ShipmentInvoice.CountAsync(cancellationToken).ConfigureAwait(false); int totalDetails = await verifyCtx.ShipmentInvoiceDetails.CountAsync(cancellationToken).ConfigureAwait(false);
                        _log.LogInfoCategorized(LogCategory.Undefined, "Verification successful. Total Invoices: {InvoiceCount}, Total Details: {DetailCount}", this.invocationId, propertyValues: new object[] { totalInvoices, totalDetails });
                    }
                }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during database verification in EntryDataDSContext.", this.invocationId); Assert.Fail($"Database verification failed: {ex.Message}"); }
                _log.LogInfoCategorized(LogCategory.InternalStep, "=== Test Completed Successfully: ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest ===", this.invocationId);
            }
            catch (Exception ex) { stopwatch.Stop(); _log.LogMethodExitFailure(this.invocationId, stopwatch.ElapsedMilliseconds, ex); _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Exit", "Added METHOD_EXIT_FAILURE log.", "LLM_Iter_0.1_PlanStep_4.6", expectedChange: "METHOD_EXIT_FAILURE logged on exception.", invocationId: this.invocationId); _log.LogActionEndFailure(this.invocationId, "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Execution", stopwatch.ElapsedMilliseconds, ex); _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Exit", "Added ACTION_END_FAILURE log.", "LLM_Iter_0.1_PlanStep_4.6", expectedChange: "ACTION_END_FAILURE logged on exception.", invocationId: this.invocationId); throw; }
            finally { if (stopwatch.IsRunning) stopwatch.Stop(); _log.LogMethodExitSuccess(this.invocationId, stopwatch.ElapsedMilliseconds); _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Exit", "Added METHOD_EXIT_SUCCESS log.", "LLM_Iter_0.1_PlanStep_4.6", expectedChange: "METHOD_EXIT_SUCCESS logged on normal completion.", invocationId: this.invocationId); _log.LogActionEndSuccess(this.invocationId, "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Execution", stopwatch.ElapsedMilliseconds); _log.LogMetaDirective(LogEventLevel.Warning, "Instrumentation", "ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest_Exit", "Added ACTION_END_SUCCESS log.", "LLM_Iter_0.1_PlanStep_4.6", expectedChange: "ACTION_END_SUCCESS logged on normal completion.", invocationId: this.invocationId); }
        }

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
            // Assuming 'this.invocationId' is available from the calling test context
            if (emailData == null || string.IsNullOrEmpty(emailData.Subject)) { _log.LogWarningCategorized(LogCategory.Undefined, "SimulateStoringEmailInDb: emailData or Subject is null/empty.", invocationId: this.invocationId); return; }
            if (!uint.TryParse(mailkitUidString, out var uidUint)) { _log.LogWarningCategorized(LogCategory.Undefined, "SimulateStoringEmailInDb: Failed to parse MailKit UID '{MailkitUid}' to uint. Using 0.", invocationId: this.invocationId, mailkitUidString); uidUint = 0; }

            using (var dbCtx = new CoreEntitiesContext())
            {
                try
                {
                    bool exists = await dbCtx.Emails.AnyAsync(e =>
                                          e.Subject == emailData.Subject &&
                                          e.ApplicationSettingsId == appSettingsId &&
                                          e.EmailUniqueId == (int)uidUint
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
                        };
                        dbCtx.Emails.Add(emailEntity);
                        await dbCtx.SaveChangesAsync().ConfigureAwait(false);
                        _log.LogInfoCategorized(
                            LogCategory.InternalStep,
                            "Simulated storing email in DB: Subject='{Subject}', AppSettingId={AppSettingId}, UID={Uid}",
                            invocationId: this.invocationId,
                            propertyValues: new object[] { emailData.Subject, appSettingsId, uidUint }
                        );
                    }
                    else _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Email already simulated in DB: Subject='{Subject}', AppSettingId={AppSettingId}, UID={Uid}", invocationId: this.invocationId, propertyValues: new object[] { emailData.Subject, appSettingsId, uidUint });
                }
                catch (Exception ex) { _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error simulating storing email in DB for Subject: {Subject}", this.invocationId, emailData.Subject); }
            }
        }
    }
}