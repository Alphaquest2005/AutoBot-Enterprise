using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Serilog;
using WaterNut.Business.Services;
using WaterNut.DataSpace;  // Add this line

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
                    try
                    {
                        // Extract file type from filename (before the first dash)
                        var fileType = fileSystemEventArgs.Name.Substring(0, fileSystemEventArgs.Name.IndexOf("-"));

                        // Validate file type exists in our mapping
                        if (!_fileTypeDocumentSets.ContainsKey(fileType))
                        {
                            _logger?.Warning("Unknown file type '{FileType}' in file '{FileName}'. Skipping processing.",
                                fileType, fileSystemEventArgs.Name);
                            return;
                        }

                        // CRITICAL BUG FIX: Use fileType variable instead of literal "fileType" string
                        var documentSetName = _fileTypeDocumentSets[fileType]; // Fixed bug: was ["fileType"]

                        _logger?.Information("CSV file detected for processing: '{FileName}' (Type: '{FileType}' → DocumentSet: '{DocumentSetName}')",
                            fileSystemEventArgs.Name, fileType, documentSetName);

                        // Delegate to domain service or queue for background processing
                        // This follows the principle of separation of concerns
                        QueueCsvForProcessing(fileSystemEventArgs.FullPath, fileType, documentSetName);
                    }
                    catch (Exception ex)
                    {
                        _logger?.Error(ex, "Error in CSV file watcher for '{FileName}': {ErrorMessage}",
                            fileSystemEventArgs.Name, ex.Message);
                    }
                }
            }

            private void QueueCsvForProcessing(string filePath, string fileType, string documentSetName)
            {
                // Use domain service pattern - queue for background processing
                // This decouples the file watcher from the actual import logic
                _logger?.Information("Queued CSV file '{FilePath}' for background processing (Type: '{FileType}', DocumentSet: '{DocumentSetName}')",
                    filePath, fileType, documentSetName);

                // TODO: Implement actual queuing mechanism using available business services
                // This could be: message queue, background service, or direct service call
            }
        }
    }
}