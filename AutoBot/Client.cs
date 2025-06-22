using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreEntities.Client.Repositories;
using EntryDataQS.Client.Repositories;
using Serilog;

namespace AutoBot
{
    internal class Program
    {
        public class Client
        {
            private readonly Dictionary<string, string> _fileTypeDocumentSets = new Dictionary<string, string>
            {
                {"PO", "Purchase Orders"},
                {"Sales", "Sales"}
            };

            private readonly ILogger _logger;
            private FileSystemWatcher _watcher;
            public string DataBase { get; set; }
            public string Password { get; set; }
            public string DataFolder { get; set; }

            public Client(ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public FileSystemWatcher Watcher
            {
                get => _watcher;
                set
                {
                    _watcher = value;
                    _watcher.Created += WatcherOnCreatedAsync;
                }
            }

            private void WatcherOnCreatedAsync(object sender, FileSystemEventArgs fileSystemEventArgs)
            {
                if (fileSystemEventArgs.Name.EndsWith(".csv"))
                {
                    var fileType = fileSystemEventArgs.Name.Substring(0, fileSystemEventArgs.Name.IndexOf("-"));
                    var docSet = AsycudaDocumentSetExRepository.Instance
                        .GetAsycudaDocumentSetExsByExpression(
                            $"Declarant_Reference_Number == \"{_fileTypeDocumentSets["fileType"]}\"").Result.First();
                    var t = EntryDataExRepository.Instance.SaveCSV(fileSystemEventArgs.FullPath, fileType,
                        docSet.AsycudaDocumentSetId, true, _logger);
                }
            }
        }
    }
}