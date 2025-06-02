using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;

namespace EmailStructureTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var imapServer = "mail.auto-brokerage.com";
            var imapPort = 993;
            var username = "autobot@auto-brokerage.com";
            var password = "AutoBot";

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine($"Connecting to {imapServer}:{imapPort}...");
                    await client.ConnectAsync(imapServer, imapPort, SecureSocketOptions.SslOnConnect);
                    
                    Console.WriteLine($"Authenticating as {username}...");
                    await client.AuthenticateAsync(username, password);
                    
                    Console.WriteLine("Opening INBOX...");
                    await client.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);
                    
                    Console.WriteLine($"Total messages: {client.Inbox.Count}");
                    Console.WriteLine($"Unread messages: {client.Inbox.Unread}");
                    
                    // Get recent emails
                    var recentUids = await client.Inbox.SearchAsync(SearchQuery.Recent);
                    Console.WriteLine($"Recent messages: {recentUids.Count}");
                    
                    // Get last 10 emails
                    var lastUids = await client.Inbox.SearchAsync(SearchQuery.All);
                    var last10 = lastUids.TakeLast(10).ToList();
                    
                    Console.WriteLine("\n=== LAST 10 EMAILS ===");
                    foreach (var uid in last10)
                    {
                        try
                        {
                            var message = await client.Inbox.GetMessageAsync(uid);
                            Console.WriteLine($"\nUID: {uid}");
                            Console.WriteLine($"Subject: {message.Subject}");
                            Console.WriteLine($"From: {message.From}");
                            Console.WriteLine($"Date: {message.Date}");
                            Console.WriteLine($"Attachments: {message.Attachments.Count()}");
                            
                            foreach (var attachment in message.Attachments)
                            {
                                if (attachment is MimePart part)
                                {
                                    Console.WriteLine($"  - {part.FileName} ({part.ContentType})");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading message {uid}: {ex.Message}");
                        }
                    }
                    
                    // Look for emails with "Invoice Template" in subject
                    Console.WriteLine("\n=== SEARCHING FOR 'Invoice Template' EMAILS ===");
                    var invoiceTemplateUids = await client.Inbox.SearchAsync(SearchQuery.SubjectContains("Invoice Template"));
                    Console.WriteLine($"Found {invoiceTemplateUids.Count} emails with 'Invoice Template' in subject");
                    
                    foreach (var uid in invoiceTemplateUids.Take(5))
                    {
                        try
                        {
                            var message = await client.Inbox.GetMessageAsync(uid);
                            Console.WriteLine($"\nInvoice Template Email - UID: {uid}");
                            Console.WriteLine($"Subject: {message.Subject}");
                            Console.WriteLine($"From: {message.From}");
                            Console.WriteLine($"Date: {message.Date}");
                            Console.WriteLine($"Attachments: {message.Attachments.Count()}");
                            
                            foreach (var attachment in message.Attachments)
                            {
                                if (attachment is MimePart part)
                                {
                                    Console.WriteLine($"  - {part.FileName} ({part.ContentType})");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading invoice template message {uid}: {ex.Message}");
                        }
                    }
                    
                    // Look for emails with PDF attachments
                    Console.WriteLine("\n=== SEARCHING FOR EMAILS WITH PDF ATTACHMENTS ===");
                    var allUids = await client.Inbox.SearchAsync(SearchQuery.All);
                    var pdfEmails = 0;
                    
                    foreach (var uid in allUids.TakeLast(20)) // Check last 20 emails
                    {
                        try
                        {
                            var message = await client.Inbox.GetMessageAsync(uid);
                            var hasPdf = message.Attachments.OfType<MimePart>()
                                .Any(part => part.FileName?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) == true);
                            
                            if (hasPdf)
                            {
                                pdfEmails++;
                                Console.WriteLine($"\nPDF Email - UID: {uid}");
                                Console.WriteLine($"Subject: {message.Subject}");
                                Console.WriteLine($"From: {message.From}");
                                Console.WriteLine($"Date: {message.Date}");
                                
                                var pdfAttachments = message.Attachments.OfType<MimePart>()
                                    .Where(part => part.FileName?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) == true);
                                
                                foreach (var pdf in pdfAttachments)
                                {
                                    Console.WriteLine($"  - PDF: {pdf.FileName} ({pdf.ContentType})");
                                }
                                
                                if (pdfEmails >= 5) break; // Limit to first 5 PDF emails
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error checking message {uid} for PDFs: {ex.Message}");
                        }
                    }
                    
                    Console.WriteLine($"\nTotal emails with PDF attachments found: {pdfEmails}");
                    
                    await client.DisconnectAsync(true);
                    Console.WriteLine("\nDisconnected successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }
    }
}
