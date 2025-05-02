
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailDownloader;
using OCR.Business.Entities;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using WaterNut.DataSpace.PipelineInfrastructure;
using Serilog;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace InvoiceReader
{
    public class InvoiceReader
    {
        private static ILogger _logger = Log.ForContext<InvoiceReader>();

        static InvoiceReader()
        {
             string logFilePath = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Logs", "AutoBot.log");
            // Ensure log directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()// Set default level
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Override specific namespaces
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("InvoiceReaderPipelineTests", Serilog.Events.LogEventLevel.Verbose) // Ensure test utilities logs are captured
                .MinimumLevel.Override("WaterNut.DataSpace.PipelineInfrastructure.PipelineRunner", Serilog.Events.LogEventLevel.Verbose) // Explicitly set level for PipelineRunner
                .Enrich.FromLogContext() // Enrichers
                //.Enrich.WithMachineName()
                //.Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}") // Console Sink - Added output template
                .WriteTo.File(logFilePath, // File Sink
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 3,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            _logger = Log.ForContext<InvoiceReader>(); // Get logger instance for this class
        }

        public static Client Client { get; set; } = new Client
        {
            CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
            DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
            Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
            Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
            EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList(),
            DevMode = true
        };

        public static string CommandsTxt => InvoiceProcessingUtils.CommandsTxt;


        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)
        {
            // Guard Clause: Check for null input array
            if (pdfFiles == null)
            {
                _logger.Error("ImportPDF called with null pdfFiles array. Aborting.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>(); // Return empty list
            }
             // Guard Clause: Check for null fileType
            if (fileType == null)
            {
                _logger.Error("ImportPDF called with null fileType. Aborting.");
                 // Cannot determine FileTypeId, critical failure.
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>(); // Return empty list
            }


            LogStartPDFImport(pdfFiles.Length, fileType);

            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> allResults = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            int fileCounter = 0;

            foreach (var file in pdfFiles.Where(f => f != null && f.Extension.ToLower() == ".pdf")) // Added null check for file
            {
                fileCounter++;
                string currentFileName = file.FullName; // Store for logging in case of error
                try
                {
                    string emailId = null;
                    int? resolvedFileTypeId = null; // Use nullable int
                    LogProcessingFile(currentFileName);

                    // --- Database Interaction ---
                    try
                    {
                        using (var ctx = new CoreEntitiesContext())
                        {
                            LogQueryingDatabase();
                            var res = ctx.AsycudaDocumentSet_Attachments
                                .Where(x => x.Attachments.FilePath == currentFileName)
                                .Select(x => new { x.EmailId, x.FileTypeId })
                                .FirstOrDefault();
                            
                            emailId = res?.EmailId ?? fileType.EmailId ?? file.Name;
                            resolvedFileTypeId = res?.FileTypeId ?? fileType.Id; // Assign to nullable int

                            LogResolvedEmailAndFileType(emailId, resolvedFileTypeId);
                        }
                    }
                    catch (Exception dbEx)
                    {
                        _logger.Error(dbEx, "Error querying database for file metadata: {FileName}. Skipping this file.", currentFileName);
                        // Add a result indicating failure for this specific file
                         allResults.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                             currentFileName, (currentFileName, "Unknown", ImportStatus.Failed)));
                        continue; // Move to the next file
                    }
                    // --- End Database Interaction ---

                    // Ensure we have a valid FileTypeId before calling Import
                    if (!resolvedFileTypeId.HasValue)
                    {
                         _logger.Error("Could not resolve FileTypeId for file: {FileName}. Skipping this file.", currentFileName);
                         allResults.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                             currentFileName, (currentFileName, "Unknown", ImportStatus.Failed)));
                         continue; // Move to the next file
                    }


                    // --- Call Import ---
                    // Import method now handles its own errors and returns results including status
                    var importResults = await Import(currentFileName, resolvedFileTypeId.Value, emailId, true, null, fileType, Client).ConfigureAwait(false);
                    allResults.AddRange(importResults);
                    // --- End Call Import ---

                }
                catch (Exception ex)
                {
                    // Catch unexpected errors during the processing of a single file
                    _logger.Error(ex, "Unexpected error processing file {FileNumber}: {FileName} in ImportPDF loop. Skipping this file.", fileCounter, currentFileName);
                     allResults.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                             currentFileName, (currentFileName, "Unknown", ImportStatus.Failed)));
                    // Continue to the next file
                }
            }

            LogPDFImportCompleted(allResults.Count(r => r.Value.Success == ImportStatus.Success)); // Log count of *successful* imports
            return allResults; // Return all results, including failures
        }

        public static async Task<List<KeyValuePair<string, (string file, string, ImportStatus Success)>>> Import(string fileFullName, int fileTypeId, string emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSets, FileTypes fileType, Client client)
        {
             // --- Guard Clauses ---
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                _logger.Error("Import called with null or empty fileFullName. Aborting.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            }
             if (fileType == null)
            {
                _logger.Error("Import called with null fileType for file: {FileName}. Aborting.", fileFullName);
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            }
             if (client == null)
            {
                _logger.Error("Import called with null client for file: {FileName}. Aborting.", fileFullName);
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            }
            // --- End Guard Clauses ---

            InvoiceProcessingContext context = null; // Declare context outside try block

            try
            {
                 FileInfo fileInfo;
                 try
                 {
                     fileInfo = new FileInfo(fileFullName);
                     if (!fileInfo.Exists)
                     {
                         _logger.Error("File does not exist: {FileName}. Aborting import for this file.", fileFullName);
                         // Return a result indicating failure due to non-existent file
                         return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>> {
                             new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                 fileFullName, (fileFullName, "Unknown", ImportStatus.Failed))
                         };
                     }
                 }
                 catch (Exception ex) when (ex is ArgumentException || ex is PathTooLongException || ex is NotSupportedException || ex is System.Security.SecurityException || ex is UnauthorizedAccessException || ex is IOException)
                 {
                     _logger.Error(ex, "Error creating FileInfo or checking existence for: {FileName}. Aborting import for this file.", fileFullName);
                      return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>> {
                             new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                 fileFullName, (fileFullName, "Unknown", ImportStatus.Failed))
                         };
                 }


                LogCreatingInvoiceProcessingContext();
                // Context creation is less likely to fail, but keep in outer try
                context = new InvoiceProcessingContext
                {
                    FilePath = fileFullName,
                    FileInfo = fileInfo, // Use validated FileInfo
                    FileTypeId = fileTypeId,
                    EmailId = emailId,
                    OverWriteExisting = overWriteExisting,
                    DocSet = docSets,
                    FileType = fileType,
                    Client = client,
                    PdfText = new StringBuilder(),
                    Imports = new Dictionary<string, (string file, string, ImportStatus Success)>()
                    // Errors list initialized automatically
                };

                LogStartingPipeline(fileFullName);
                var pipe = new InvoiceProcessingPipeline(context, false);
                bool pipeResult = false; // Default to false

                // --- Pipeline Execution ---
                try
                {
                    pipeResult = await pipe.RunPipeline().ConfigureAwait(false);
                }
                catch (Exception pipeEx)
                {
                    // Catch errors specifically from RunPipeline itself (should be rare if PipelineRunner catches step errors)
                    string errorMsg = $"Unexpected error during pipeline execution for file: {fileFullName}";
                    _logger.Error(pipeEx, errorMsg);
                    context.AddError($"{errorMsg}: {pipeEx.Message}"); // Add error to context
                    pipeResult = false; // Ensure result indicates failure
                }
                // --- End Pipeline Execution ---


                if (!pipeResult)
                {
                    // Pipeline reported failure (either returned false or caught exception)
                     _logger.Warning("Pipeline execution failed or reported unsuccessful for file: {FileName}. Errors: {Errors}", fileFullName, string.Join("; ", context.Errors));
                     // Ensure the Imports dictionary reflects the failure if it's empty
                     if (!context.Imports.Any())
                     {
                         context.Imports.Add(fileFullName, (fileFullName, context.FileType?.Description ?? "Unknown", ImportStatus.Failed));
                     }
                }

                LogPipelineCompleted(context.Imports.Count);
                return context.Imports.ToList(); // Return results, including failures recorded in context.Imports
            }
            catch (Exception ex)
            {
                // Catch broader errors in the Import method setup (e.g., context creation)
                 _logger.Error(ex, "Unexpected error during Import setup for file: {FileName}. Aborting import for this file.", fileFullName);
                 // If context exists, add the error.
                 context?.AddError($"Unexpected error during Import setup: {ex.Message}");
                 // Return failure indication
                 var failureResult = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
                 if (context?.Imports != null && context.Imports.Any())
                 {
                     // If pipeline ran partially and added imports before this outer error, return them.
                     failureResult = context.Imports.ToList();
                 }
                 else
                 {
                      // Otherwise, return a generic failure entry.
                      failureResult.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                 fileFullName, (fileFullName, fileType?.Description ?? "Unknown", ImportStatus.Failed)));
                 }
                 return failureResult;
            }
        }

        public static async Task<string> GetPdftxt(string fileFullName)
        {
             // Guard Clause: Check for null/empty input
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                _logger.Error("GetPdftxt called with null or empty fileFullName.");
                return null; // Indicate failure
            }

            InvoiceProcessingContext context = null;
            try
            {
                 FileInfo fileInfo;
                 try
                 {
                     fileInfo = new FileInfo(fileFullName);
                      if (!fileInfo.Exists) // Check if file exists
                     {
                         _logger.Error("File does not exist in GetPdftxt: {FileName}", fileFullName);
                         return null; // Indicate failure
                     }
                 }
                 catch (Exception ex) when (ex is ArgumentException || ex is PathTooLongException || ex is NotSupportedException || ex is System.Security.SecurityException || ex is UnauthorizedAccessException || ex is IOException)
                 {
                     _logger.Error(ex, "Error creating FileInfo or checking existence in GetPdftxt for: {FileName}", fileFullName);
                     return null; // Indicate failure
                 }


                // Create a minimal context for the step
                context = new InvoiceProcessingContext
                {
                    FilePath = fileFullName,
                    FileInfo = fileInfo, // Use validated FileInfo
                    PdfText = new StringBuilder()
                    // No need for other properties like FileType, Client etc. for this specific task
                };

                var pdfStep = new GetPdfTextStep();
                bool stepResult = false;

                // --- Step Execution ---
                try
                {
                     _logger.Debug("Executing GetPdfTextStep for file: {FileName}", fileFullName);
                    stepResult = await pdfStep.Execute(context).ConfigureAwait(false);
                }
                catch (Exception stepEx)
                {
                    _logger.Error(stepEx, "Error executing GetPdfTextStep for file: {FileName}", fileFullName);
                    // Cannot add to context.Errors as it's local and not returned. Logging is key.
                    stepResult = false; // Indicate failure
                }
                 // --- End Step Execution ---

                if (!stepResult)
                {
                    _logger.Warning("GetPdfTextStep reported failure (returned false) for file: {FileName}", fileFullName);
                    return null; // Indicate failure
                }

                _logger.Information("Successfully extracted PDF text for file: {FileName}", fileFullName);
                return context.PdfText.ToString();
            }
            catch (Exception ex)
            {
                // Catch broader errors (e.g., context creation - unlikely here)
                _logger.Error(ex, "Unexpected error during GetPdftxt for file: {FileName}", fileFullName);
                return null; // Indicate failure
            }
        }

        public static bool IsInvoiceDocument(Invoices invoice, string fileText, string fileName)
        {
            return GetPossibleInvoicesStep.IsInvoiceDocument(invoice, fileText, fileName);
        }

        private static void LogStartPDFImport(int fileCount, FileTypes fileType)
        {
            _logger.Information("Starting PDF import for {FileCount} files with FileType: {FileTypeName} (ID: {FileTypeId})",
                fileCount, fileType.Description, fileType.Id);
        }

        private static void LogProcessingFile(string fileName)
        {
            _logger.Debug("Processing file: {FileName}", fileName);
        }

        private static void LogQueryingDatabase()
        {
            _logger.Debug("Querying database for existing attachments matching file path");
        }

        private static void LogResolvedEmailAndFileType(string emailId, int? fileTypeId)
        {
            _logger.Debug("Resolved EmailId: {EmailId}, FileTypeId: {FileTypeId}", emailId, fileTypeId);
        }

        private static void LogCreatingInvoiceProcessingContext()
        {
            _logger.Debug("Creating InvoiceProcessingContext");
        }

        private static void LogStartingPipeline(string fileName)
        {
            _logger.Information("Starting invoice processing pipeline for file: {FileName}", fileName);
        }

        private static void LogPipelineCompleted(int importCount)
        {
            _logger.Information("Pipeline completed with {ImportCount} imports", importCount);
        }

        private static void LogPDFImportCompleted(int successCount)
        {
            _logger.Information("PDF import completed with {SuccessCount} successful imports", successCount);
        }


  
    }
}
