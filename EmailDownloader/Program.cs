using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;

namespace EmailDownloader
{
    class Program
    {
        public class Client
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string DataFolder { get; set; }

        }

        static List<Client> Clients = new List<Client>()
        {
            new Client(){Email = "budgetmarine@ez-brokerage-services.com", DataFolder = @"C:\Prism\Clients\Budget Marine\Emails\", Password = "budgetQuest!1"},
            new Client(){Email = "northyachtshop@ez-brokerage-services.com", DataFolder = @"C:\Prism\Clients\North Yacht\Emails\", Password = "northQuest!1"}
        };
        static void Main(string[] args)
        {
            foreach (var client in Clients)
            {
                CheckEmails(client);
            }
        }

        private static void CheckEmails(Client client)
        {
            var imapClient = new ImapClient();
            imapClient.Connect("ez-brokerage-services.com", 993, SecureSocketOptions.SslOnConnect);
            imapClient.Authenticate(client.Email, client.Password);
            var dataFolder = client.DataFolder;
            imapClient.Inbox.Open(FolderAccess.ReadWrite);
            DowloadAttachment(imapClient, dataFolder);

            imapClient.Disconnect(true);
        }

        private static void DowloadAttachment(ImapClient imapClient, string dataFolder)
        {
            foreach (var uid in imapClient.Inbox.Search(SearchQuery.NotSeen))
            {
                var msg = imapClient.Inbox.GetMessage(uid);
                foreach (var a in msg.Attachments)
                {
                    if (!a.IsAttachment) continue;
                    var part = (MimePart) a;
                    var fileName = part.FileName;

                    using (var stream = File.Create(dataFolder + fileName))
                        part.Content.DecodeTo(stream);
                }

                imapClient.Inbox.AddFlags(uid, MessageFlags.Seen, true);
            }
        }
    }
}
