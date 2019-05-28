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

namespace EmailDownloader
{
    public static class EmailDownloader
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

                var monthYear = Regex.Match(msg.Subject, @"(\b\d{1,2}\D{0,3})?\b(?<Month>Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|(Nov|Dec)(?:ember)?)[a-zA-Z\s]*(?<Year>(19[7-9]\d|20\d{2})|\d{2})?", RegexOptions.IgnoreCase);

                var subject =  (monthYear.Groups["Month"].Success? monthYear.Groups["Month"].Value: msg.Date.ToString("MMMM")) + " " + (monthYear.Groups["Year"].Success? monthYear.Groups["Year"].Value : DateTime.Now.Year.ToString());
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