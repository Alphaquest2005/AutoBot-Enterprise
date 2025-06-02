using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using EmailDownloader;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using Serilog;
using Core.Common.Extensions;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Test helper for examining email structure and content in the autobot@auto-brokerage.com mailbox
    /// Based on the proven EmailDownloaderIntegrationTests pattern
    /// </summary>
    public class EmailStructureTestHelper
    {
        private readonly ILogger _log;
        private readonly string _invocationId;
        
        // Email configuration from database
        private string _dbConfiguredEmailAddress;
        private string _dbConfiguredEmailPassword;
        private int _dbConfiguredApplicationSettingsId;
        private string _resolvedImapServer;
        private int _resolvedImapPort;
        private SecureSocketOptions _resolvedImapOptions;

        public EmailStructureTestHelper(ILogger log, string invocationId = null)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _invocationId = invocationId ?? Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialize email configuration from database (same pattern as EmailDownloaderIntegrationTests)
        /// </summary>
        public async Task InitializeAsync()
        {
            _log.LogInfoCategorized(LogCategory.InternalStep, "=== EmailStructureTestHelper Initialization Starting ===", _invocationId);
            
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    // Load active ApplicationSettings with all email-related includes
                    var testAppSettingFromDb = await ctx.ApplicationSettings
                        .Include(x => x.FileTypes).Include(x => x.Declarants)
                        .Include("FileTypes.FileTypeReplaceRegex").Include("FileTypes.FileImporterInfos")
                        .Include(x => x.EmailMapping)
                        .Include("EmailMapping.EmailFileTypes.FileTypes.FileImporterInfos")
                        .Include("EmailMapping.EmailMappingRexExs").Include("EmailMapping.EmailMappingActions.Actions")
                        .Include("EmailMapping.EmailInfoMappings.InfoMapping.InfoMappingRegEx")
                        .FirstOrDefaultAsync(s => s.IsActive);

                    if (testAppSettingFromDb != null)
                    {
                        _dbConfiguredEmailAddress = testAppSettingFromDb.Email;
                        _dbConfiguredEmailPassword = testAppSettingFromDb.EmailPassword;
                        _dbConfiguredApplicationSettingsId = testAppSettingFromDb.ApplicationSettingsId;
                        
                        if (string.IsNullOrEmpty(_dbConfiguredEmailAddress) || string.IsNullOrEmpty(_dbConfiguredEmailPassword))
                        {
                            throw new InvalidOperationException($"ApplicationSetting record found (ID: {testAppSettingFromDb.ApplicationSettingsId}) but Email or EmailPassword fields are empty.");
                        }
                        
                        _log.LogInfoCategorized(LogCategory.InternalStep, "Loaded email configuration: AppSettingId={AppSettingId}, Email={EmailAddress}", 
                            _invocationId, propertyValues: new object[] { _dbConfiguredApplicationSettingsId, _dbConfiguredEmailAddress });
                    }
                    else
                    {
                        throw new InvalidOperationException("No active ApplicationSettings record found in database.");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error reading email configuration from database", _invocationId);
                throw;
            }

            // Derive email server settings using EmailDownloader logic
            try
            {
                var readSettings = EmailDownloader.EmailDownloader.GetReadMailSettings(_dbConfiguredEmailAddress, _log);
                _resolvedImapServer = readSettings.Server;
                _resolvedImapPort = readSettings.Port;
                _resolvedImapOptions = readSettings.Options;
                
                _log.LogInfoCategorized(LogCategory.InternalStep, "Derived IMAP settings: {ImapServer}:{ImapPort} ({ImapOptions})", 
                    _invocationId, propertyValues: new object[] { _resolvedImapServer, _resolvedImapPort, _resolvedImapOptions });
            }
            catch (Exception ex)
            {
                _log.LogErrorCategorized(LogCategory.Undefined, ex, "Failed to derive email server settings", _invocationId);
                throw;
            }
            
            _log.LogInfoCategorized(LogCategory.InternalStep, "=== EmailStructureTestHelper Initialization Complete ===", _invocationId);
        }

        /// <summary>
        /// Get IMAP client connection (same pattern as EmailDownloaderIntegrationTests)
        /// </summary>
        private async Task<ImapClient> GetTestImapClientAsync(CancellationToken cancellationToken = default)
        {
            var imapClient = new ImapClient();
            try
            {
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Connecting IMAP: {ImapServer}:{ImapPort} ({ImapOptions})", 
                    _invocationId, propertyValues: new object[] { _resolvedImapServer, _resolvedImapPort, _resolvedImapOptions });
                
                await imapClient.ConnectAsync(_resolvedImapServer, _resolvedImapPort, _resolvedImapOptions, cancellationToken);
                
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Authenticating IMAP: {EmailAddress}", 
                    _invocationId, propertyValues: new object[] { _dbConfiguredEmailAddress });
                
                await imapClient.AuthenticateAsync(_dbConfiguredEmailAddress, _dbConfiguredEmailPassword, cancellationToken);
                
                _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Opening IMAP Inbox (ReadOnly)", _invocationId);
                await imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
                
                _log.LogInfoCategorized(LogCategory.InternalStep, "IMAP client connected & Inbox opened: {EmailAddress}", 
                    _invocationId, propertyValues: new object[] { _dbConfiguredEmailAddress });
                
                return imapClient;
            }
            catch (Exception ex)
            {
                _log.LogErrorCategorized(LogCategory.Undefined, ex, "Failed to connect IMAP client", _invocationId);
                imapClient?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Examine email structure and content in the mailbox
        /// </summary>
        public async Task<EmailStructureReport> ExamineEmailStructureAsync()
        {
            _log.LogInfoCategorized(LogCategory.InternalStep, "=== Starting Email Structure Examination ===", _invocationId);
            
            var report = new EmailStructureReport();
            ImapClient imapClient = null;
            
            try
            {
                imapClient = await GetTestImapClientAsync();
                
                // Get basic mailbox statistics
                report.TotalMessages = imapClient.Inbox.Count;
                report.UnreadMessages = imapClient.Inbox.Unread;
                
                _log.LogInfoCategorized(LogCategory.InternalStep, "Mailbox stats: Total={TotalMessages}, Unread={UnreadMessages}", 
                    _invocationId, propertyValues: new object[] { report.TotalMessages, report.UnreadMessages });

                // Get recent emails
                var recentUids = await imapClient.Inbox.SearchAsync(SearchQuery.Recent);
                report.RecentMessages = recentUids.Count;
                
                // Get last 20 emails for analysis
                var allUids = await imapClient.Inbox.SearchAsync(SearchQuery.All);
                var last20 = allUids.Skip(Math.Max(0, allUids.Count - 20)).ToList();

                _log.LogInfoCategorized(LogCategory.InternalStep, "Analyzing last {EmailCount} emails",
                    _invocationId, propertyValues: new object[] { last20.Count });

                foreach (var uid in last20)
                {
                    try
                    {
                        var message = await imapClient.Inbox.GetMessageAsync(uid);
                        var emailInfo = new EmailInfo
                        {
                            Uid = uid.ToString(),
                            Subject = message.Subject,
                            From = message.From.ToString(),
                            Date = message.Date,
                            AttachmentCount = message.Attachments.Count()
                        };

                        // Analyze attachments
                        foreach (var attachment in message.Attachments.OfType<MimePart>())
                        {
                            var attachmentInfo = new AttachmentInfo
                            {
                                FileName = attachment.FileName,
                                ContentType = attachment.ContentType.ToString(),
                                IsPdf = attachment.FileName?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) == true
                            };
                            emailInfo.Attachments.Add(attachmentInfo);
                            
                            if (attachmentInfo.IsPdf)
                            {
                                report.EmailsWithPdfAttachments++;
                            }
                        }

                        // Check for specific patterns
                        if (message.Subject != null && message.Subject.ToLowerInvariant().Contains("invoice template"))
                        {
                            report.InvoiceTemplateEmails++;
                        }

                        if (message.Subject != null && message.Subject.ToLowerInvariant().Contains("shipment"))
                        {
                            report.ShipmentEmails++;
                        }

                        report.Emails.Add(emailInfo);
                        
                        _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Analyzed email: UID={Uid}, Subject='{Subject}', Attachments={AttachmentCount}", 
                            _invocationId, propertyValues: new object[] { uid, message.Subject, emailInfo.AttachmentCount });
                    }
                    catch (Exception ex)
                    {
                        _log.LogWarningCategorized(LogCategory.Undefined, ex, "Error analyzing email UID: {Uid}", 
                            _invocationId, propertyValues: new object[] { uid });
                    }
                }

                // Search for specific email patterns
                await SearchForSpecificPatternsAsync(imapClient, report);
                
                _log.LogInfoCategorized(LogCategory.InternalStep, "Email structure examination complete: TotalEmails={TotalEmails}, PDFEmails={PDFEmails}, InvoiceTemplateEmails={InvoiceTemplateEmails}", 
                    _invocationId, propertyValues: new object[] { report.Emails.Count, report.EmailsWithPdfAttachments, report.InvoiceTemplateEmails });
            }
            catch (Exception ex)
            {
                _log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during email structure examination", _invocationId);
                throw;
            }
            finally
            {
                if (imapClient?.IsConnected == true)
                {
                    await imapClient.DisconnectAsync(true);
                }
                imapClient?.Dispose();
            }
            
            return report;
        }

        private async Task SearchForSpecificPatternsAsync(ImapClient imapClient, EmailStructureReport report)
        {
            // Search for "Invoice Template" emails
            var invoiceTemplateUids = await imapClient.Inbox.SearchAsync(SearchQuery.SubjectContains("Invoice Template"));
            _log.LogInfoCategorized(LogCategory.InternalStep, "Found {Count} emails with 'Invoice Template' in subject", 
                _invocationId, propertyValues: new object[] { invoiceTemplateUids.Count });

            // Search for "Shipment" emails  
            var shipmentUids = await imapClient.Inbox.SearchAsync(SearchQuery.SubjectContains("Shipment"));
            _log.LogInfoCategorized(LogCategory.InternalStep, "Found {Count} emails with 'Shipment' in subject", 
                _invocationId, propertyValues: new object[] { shipmentUids.Count });

            // Search for emails with PDF attachments (by examining recent emails)
            var pdfEmailCount = 0;
            var recentUids = await imapClient.Inbox.SearchAsync(SearchQuery.Recent);
            
            foreach (var uid in recentUids.Take(10)) // Check last 10 recent emails
            {
                try
                {
                    var message = await imapClient.Inbox.GetMessageAsync(uid);
                    var hasPdf = message.Attachments.OfType<MimePart>()
                        .Any(part => part.FileName?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) == true);
                    
                    if (hasPdf)
                    {
                        pdfEmailCount++;
                        _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Recent email with PDF: UID={Uid}, Subject='{Subject}'", 
                            _invocationId, propertyValues: new object[] { uid, message.Subject });
                    }
                }
                catch (Exception ex)
                {
                    _log.LogWarningCategorized(LogCategory.Undefined, ex, "Error checking email UID {Uid} for PDFs", 
                        _invocationId, propertyValues: new object[] { uid });
                }
            }
            
            _log.LogInfoCategorized(LogCategory.InternalStep, "Found {Count} recent emails with PDF attachments", 
                _invocationId, propertyValues: new object[] { pdfEmailCount });
        }
    }

    public class EmailStructureReport
    {
        public int TotalMessages { get; set; }
        public int UnreadMessages { get; set; }
        public int RecentMessages { get; set; }
        public int EmailsWithPdfAttachments { get; set; }
        public int InvoiceTemplateEmails { get; set; }
        public int ShipmentEmails { get; set; }
        public List<EmailInfo> Emails { get; set; } = new List<EmailInfo>();
    }

    public class EmailInfo
    {
        public string Uid { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public DateTimeOffset Date { get; set; }
        public int AttachmentCount { get; set; }
        public List<AttachmentInfo> Attachments { get; set; } = new List<AttachmentInfo>();
    }

    public class AttachmentInfo
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public bool IsPdf { get; set; }
    }
}
