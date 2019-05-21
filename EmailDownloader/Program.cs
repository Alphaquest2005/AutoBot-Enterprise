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
        static List<Client> Clients = new List<Client>()
        {
            new Client(){Email = "budgetmarine@ez-brokerage-services.com", DataFolder = @"C:\Prism\Clients\Budget Marine\Emails\", Password = "budgetQuest!1"},
            new Client(){Email = "northyachtshop@ez-brokerage-services.com", DataFolder = @"C:\Prism\Clients\North Yacht\Emails\", Password = "northQuest!1"}
        };
        static void Main(string[] args)
        {
            foreach (var client in Clients)
            {
                EmailDownloader.CheckEmails(client);
            }
        }
    }
}
