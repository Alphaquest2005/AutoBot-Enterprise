using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoBot
{
    partial class Program
    {

        static List<Client> Clients = new List<Client>()
        {
            new Client(){DataBase = "Budget-ENTERPRISEDB", Watcher = new FileSystemWatcher(@"C:\Prism\Clients\Budget Marine\Emails\")},
            new Client(){DataBase = "NorthYacht-ENTERPRISEDB", Watcher = new FileSystemWatcher(@"C:\Prism\Clients\North Yacht\Emails\")}
        };
        static void Main(string[] args)
        {
           
        }

        

        
    }
}
