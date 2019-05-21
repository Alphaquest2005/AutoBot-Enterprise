using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MailKit;
using MailKit.Net.Imap;
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
            var res = DownloadAttachment(imapClient, dataFolder);

            imapClient.Disconnect(true);
            return res;
        }

        public static Dictionary<string, List<string>> DownloadAttachment(ImapClient imapClient, string dataFolder )
        {
            var msgFiles = new Dictionary<string, List<string>>();
            foreach (var uid in imapClient.Inbox.Search(SearchQuery.NotSeen))
            {
                var lst = new List<string>();
                var msg = imapClient.Inbox.GetMessage(uid);
                var subject = ReplaceSpecialChar((msg.Subject.Contains(":")?msg.Subject.Split(':')[1]:msg.Subject).Trim(), "-");
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