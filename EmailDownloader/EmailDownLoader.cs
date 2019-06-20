using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static Dictionary<string, List<string>> CheckEmails(Client client)
        {
            var imapClient = new ImapClient();
            imapClient.Connect("ez-brokerage-services.com", 993, SecureSocketOptions.SslOnConnect);
            imapClient.Authenticate(client.Email, client.Password);
            var dataFolder = client.DataFolder;
            imapClient.Inbox.Open(FolderAccess.ReadWrite);
            var res = DownloadAttachment(imapClient, dataFolder, client.EmailMappings, client);

            imapClient.Disconnect(true);
            return res;
        }

        public static void SendEmail(Client client, string directory, string subject, string[] To, string body,
            string[] attachments)
        {
            try
            {
                
                MimeMessage msg = CreateMessage(client, subject, To, body, attachments);
                SendEmail(client, msg);
                if(directory != null)
                File.AppendAllLines(Path.Combine(directory, "EmailResults.txt"), attachments);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static MimeMessage CreateMessage(Client client, string subject, string[] to, string body, string[] attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("AutoBot",client.Email));
            foreach (var recipent in to)
            {
               message.To.Add(MailboxAddress.Parse(recipent)); 
            }
            
            message.Subject = subject;

            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.TextBody = body;

            foreach (var attachment in attachments)
            {
               builder.Attachments.Add(attachment);
            }


            // Now we just need to set the message body and we're done
            message.Body = builder.ToMessageBody();
            return message;
        }


        public static Dictionary<string, List<string>> DownloadAttachment(ImapClient imapClient, string dataFolder,
            List<string> emailMappings, Client client)
        {
            var msgFiles = new Dictionary<string, List<string>>();
            foreach (var uid in imapClient.Inbox.Search(SearchQuery.NotSeen))
            {
                var lst = new List<string>();
                var msg = imapClient.Inbox.GetMessage(uid);

                if(!emailMappings.Any(x => Regex.IsMatch(msg.Subject, x)))
                {
                    imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                    SendBackMsg(msg, client);
                    continue;
                }

                var subject = GetSubject(msg);
                

                var desFolder = Path.Combine(dataFolder, subject);
                Directory.CreateDirectory(desFolder);
                foreach (var a in msg.Attachments)
                {
                    if (!a.IsAttachment) continue;
                    SaveAttachmentPart(desFolder,  a, lst);
                }

                SaveBodyPart(desFolder, msg, lst);

                imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                if (msgFiles.ContainsKey(subject))
                {
                    msgFiles[subject].AddRange(lst);
                }
                else
                {
                   msgFiles.Add(subject, lst); 
                }
            }

            return msgFiles;
        }

        private static string GetSubject(MimeMessage msg)
        {
            var patterns = new string[]
            {
                @"(\b\d{1,2}\D{0,3})?\b(?<Month>Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|(Nov|Dec)(?:ember)?)[a-zA-Z\s]*(?<Year>(19[7-9]\d|20\d{2})|\d{2})?",
                @"Shipment: (?<Subject>.+)"
            };

            foreach (var pattern in patterns)
            {
                var mat = Regex.Match(msg.Subject,
                    pattern,
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (!mat.Success) continue;
                var subject = "";
                for (int i = 1; i < mat.Groups.Count; i++)
                {
                    var g = mat.Groups[i];
                    subject += " " + g;
                }
                return subject.Trim();

            }

            return null;
        }
        
        private static void SendBackMsg(MimeMessage msg, Client clientDetails)
        {
            // construct a new message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("AutoBot", clientDetails.Email));
            message.ReplyTo.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
            message.To.Add(new MailboxAddress(msg.From.First().Name, msg.From.Mailboxes.FirstOrDefault().Address));
            message.Subject = "FWD: " + msg.Subject;

            // now to create our body...
            var builder = new BodyBuilder();
            builder.TextBody = "Hey,\r\n\r\n The System is not configured for this message.\r\n" +
                               "Check the Subject again or Check Joseph Barthoomew at josephbartholomew@outlook.com to make the necessary changes.\r\n" +
                               "Thanks\r\n" +
                               "Ez-Asycuda-Toolkit";
            
            builder.Attachments.Add(new MessagePart { Message = msg });

            message.Body = builder.ToMessageBody();

            SendEmail(clientDetails, message);
        }

        private static void SendEmail(Client clientDetails, MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Connect("ez-brokerage-services.com", 465, true);
                client.Authenticate(clientDetails.Email, clientDetails.Password);

                client.Send(message);

                client.Disconnect(true);
            }
        }

        private static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z]+", rstring);
        }

        private static void SaveAttachmentPart(string dataFolder,  MimeEntity a, List<string> lst)
        {
            var part = (MimePart) a;
            var fileName = part.FileName;

            using (var stream = File.Create(Path.Combine(dataFolder, fileName)))
                part.Content.DecodeTo(stream);
            lst.Add(fileName);
        }

        private static void SaveBodyPart(string dataFolder, MimeMessage a, List<string> lst)
        {
            var part = a.BodyParts.OfType<TextPart>().FirstOrDefault();
            var fileName = "Info.txt";

            if (part != null) System.IO.File.AppendAllText(Path.Combine(dataFolder, fileName), part.Text);
            lst.Add(fileName);
        }
    }
}