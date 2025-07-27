using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Serilog.Context;
using System.Text;
using System.Threading.Tasks;
using EmailDownloader;
using OCR.Business.Entities;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using Serilog;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace InvoiceReader
{
    public class InvoiceReader
    {
        private readonly ILogger _logger;
        
        public InvoiceReader(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
            // Static logger configuration removed - instances now require injected logger
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


        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes fileType, ILogger logger)
        {
            var methodStopwatch = Stopwatch.StartNew();
            var invocationId = Guid.NewGuid(); // Generate unique InvocationId
            var pdfLogger = logger.ForContext("InvocationId", invocationId); // Create enriched logger

            pdfLogger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                            nameof(ImportPDF), "Import multiple PDF files and process them", $"FileCount: {pdfFiles?.Length ?? 0}, FileTypeId: {fileType?.Id}, FileTypeName: {fileType?.Description}");

            pdfLogger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                            nameof(ImportPDF), $"Processing {pdfFiles?.Length ?? 0} PDF files for import with FileType: {fileType?.Description}");

            // Guard Clause: Check for null input array
            if (pdfFiles == null)
            {
                methodStopwatch.Stop();
                pdfLogger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ImportPDF), "Import multiple PDF files and process them", methodStopwatch.ElapsedMilliseconds, "Input pdfFiles array is null.");
                pdfLogger.Error(new ArgumentNullException(nameof(pdfFiles)), "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ImportPDF), "Input validation", methodStopwatch.ElapsedMilliseconds, "Input pdfFiles array is null.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>(); // Return empty list
             }
              // Guard Clause: Check for null fileType
             if (fileType == null)
             {
                methodStopwatch.Stop();
                pdfLogger.Error(new ArgumentNullException(nameof(fileType)), "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ImportPDF), "Import multiple PDF files and process them", methodStopwatch.ElapsedMilliseconds, "Input fileType is null.");
                 pdfLogger.Error(new ArgumentNullException(nameof(fileType)), "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(ImportPDF), "Input validation", methodStopwatch.ElapsedMilliseconds, "Input fileType is null.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>(); // Return empty list
             }


            pdfLogger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ImportPDF), "Initialization", "Starting PDF import process.", $"FileCount: {pdfFiles.Length}, FileTypeId: {fileType.Id}, FileTypeName: {fileType.Description}", "");

            List<KeyValuePair<string, (string file, string, ImportStatus Success)>> allResults = new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            int fileCounter = 0;

            foreach (var file in pdfFiles.Where(f => f != null && f.Extension.ToLower() == ".pdf")) // Added null check for file
            {
                fileCounter++;
                string currentFileName = file.FullName; // Store for logging in case of error
                {
                    var fileProcessingStopwatch = Stopwatch.StartNew();
                    pdfLogger.Information("ACTION_START: {ActionName}. Context: {ActionContext}",
                        "ProcessSinglePDFFile", $"FileName: {currentFileName}, FileNumber: {fileCounter}/{pdfFiles.Length}");

                    try
                    {
                        string emailId = null;
                        int? resolvedFileTypeId = null; // Use nullable int
                        LogProcessingFile(currentFileName, pdfLogger); // Pass logger

                        // --- Database Interaction ---
                        var dbQueryStopwatch = Stopwatch.StartNew();
                        try
                        {
                            using (var ctx = new CoreEntitiesContext())
                            {
                                LogQueryingDatabase(pdfLogger); // Pass logger
                                var res = ctx.AsycudaDocumentSet_Attachments
                                    .Where(x => x.Attachments.FilePath == currentFileName)
                                    .Select(x => new { x.EmailId, x.FileTypeId })
                                    .FirstOrDefault();
                                dbQueryStopwatch.Stop();
                                pdfLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                                                    "Query database for existing attachments", dbQueryStopwatch.ElapsedMilliseconds, "Sync call returned");

                                emailId = res?.EmailId ?? fileType.EmailId ?? file.Name;
                                resolvedFileTypeId = res?.FileTypeId ?? fileType.Id; // Assign to nullable int

                                 LogResolvedEmailAndFileType(emailId, resolvedFileTypeId, pdfLogger); // Pass logger
                             }
                         }
                         catch (Exception dbEx)
                         {
                             dbQueryStopwatch.Stop();
                             pdfLogger.Error(dbEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                 "ProcessSinglePDFFile - DBQuery", "Database query for metadata", dbQueryStopwatch.ElapsedMilliseconds, $"Error querying database for file metadata: {currentFileName}. Skipping this file.");
                             // Add a result indicating failure for this specific file
                              allResults.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                   currentFileName, (currentFileName, "Unknown", ImportStatus.Failed)));
                             fileProcessingStopwatch.Stop();
                             pdfLogger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                 "ProcessSinglePDFFile", "Process individual PDF file", fileProcessingStopwatch.ElapsedMilliseconds, $"Database query failed for file: {currentFileName}");
                             pdfLogger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                 "ProcessSinglePDFFile", "Database query", fileProcessingStopwatch.ElapsedMilliseconds, $"Database query failed for file: {currentFileName}");
                             continue; // Move to the next file
                         }
                         // --- End Database Interaction ---

                         // Ensure we have a valid FileTypeId before calling Import
                         if (!resolvedFileTypeId.HasValue)
                         {
                              pdfLogger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                                                  "ProcessSinglePDFFile", "ResolveMetadata", "Could not resolve FileTypeId.", $"FileName: {currentFileName}", "");
                              allResults.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                   currentFileName, (currentFileName, "Unknown", ImportStatus.Failed)));
                              fileProcessingStopwatch.Stop();
                              pdfLogger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                  "ProcessSinglePDFFile", "Process individual PDF file", fileProcessingStopwatch.ElapsedMilliseconds, $"Could not resolve FileTypeId for file: {currentFileName}");
                              pdfLogger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                   "ProcessSinglePDFFile", "Metadata resolution", fileProcessingStopwatch.ElapsedMilliseconds, $"Could not resolve FileTypeId for file: {currentFileName}");
                              continue; // Move to the next file
                         }


                         // --- Call Import ---
                         var importCallStopwatch = Stopwatch.StartNew();
                         pdfLogger.Information("INVOKING_OPERATION: {OperationDescription}. Async Expectation: {AsyncExpectation}",
                             "InvoiceReader.Import", "Yes");
                         var importResults = await Import(currentFileName, resolvedFileTypeId.Value, emailId, true, null, fileType, Client, pdfLogger).ConfigureAwait(false); // Pass enriched logger
                         importCallStopwatch.Stop();
                         pdfLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                                        "InvoiceReader.Import", importCallStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                        allResults.AddRange(importResults);
                        // --- End Call Import ---

                        fileProcessingStopwatch.Stop();
                        pdfLogger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                            "ProcessSinglePDFFile", $"Successfully processed file: {currentFileName}", fileProcessingStopwatch.ElapsedMilliseconds);

                    }
                    catch (Exception ex)
                    {
                        // Catch unexpected errors during the processing of a single file
                        fileProcessingStopwatch.Stop();
                        pdfLogger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            "ProcessSinglePDFFile", "Process individual PDF file", fileProcessingStopwatch.ElapsedMilliseconds, $"Unexpected error processing file {fileCounter}: {currentFileName}.");
                        pdfLogger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            "ProcessSinglePDFFile", "Unexpected error during file processing", fileProcessingStopwatch.ElapsedMilliseconds, $"Unexpected error processing file {fileCounter}: {currentFileName}. Error: {ex.Message}");
                        allResults.Add(new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                  currentFileName, (currentFileName, "Unknown", ImportStatus.Failed)));
                        // Continue to the next file
                    }
                } // End of the logical scope for the enriched logger
            }

            int successCount = allResults.Count(r => r.Value.Success == ImportStatus.Success);
            LogPDFImportCompleted(successCount, pdfLogger); // Pass logger

            methodStopwatch.Stop();
            pdfLogger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                nameof(ImportPDF), $"Processed {pdfFiles.Length} files, {successCount} successful.", $"SuccessfulImports: {successCount}, TotalFilesProcessed: {pdfFiles.Length}", methodStopwatch.ElapsedMilliseconds);
            pdfLogger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                nameof(ImportPDF), $"Processed {pdfFiles.Length} files, {successCount} successful.", methodStopwatch.ElapsedMilliseconds);

            return allResults; // Return all results, including failures
        }

        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> Import(string fileFullName, int fileTypeId, string emailId, bool overWriteExisting, List<AsycudaDocumentSet> docSets, FileTypes fileType, Client client, ILogger logger)
        {
            // Static logger removed - using injected logger parameter
            
            var methodStopwatch = Stopwatch.StartNew();
            // InvocationId is now passed via the enriched logger from ImportPDF
            // var invocationId = Guid.NewGuid(); // Generate unique InvocationId
            // using (LogContext.PushProperty("InvocationId", invocationId)) // Scope InvocationId for this operation
            // {
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                            nameof(Import), "Import a single file and process it through the pipeline", $"FileName: {fileFullName}, FileTypeId: {fileTypeId}, EmailId: {emailId}, Overwrite: {overWriteExisting}");

            logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                            nameof(Import), $"Processing single file: {fileFullName} with FileType ID: {fileTypeId}");

            // --- Guard Clauses ---
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                methodStopwatch.Stop();
                logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Import), "Import a single file and process it through the pipeline", methodStopwatch.ElapsedMilliseconds, "Input fileFullName is null or empty.");
                logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Import), "Input validation", methodStopwatch.ElapsedMilliseconds, "Input fileFullName is null or empty.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            }
             if (fileType == null)
            {
                methodStopwatch.Stop();
                logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Import), "Import a single file and process it through the pipeline", methodStopwatch.ElapsedMilliseconds, $"Input fileType is null for file: {fileFullName}.");
                logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Import), "Input validation", methodStopwatch.ElapsedMilliseconds, $"Input fileType is null for file: {fileFullName}.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            }
             if (client == null)
            {
                methodStopwatch.Stop();
                logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Import), "Import a single file and process it through the pipeline", methodStopwatch.ElapsedMilliseconds, $"Input client is null for file: {fileFullName}.");
                logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Import), "Input validation", methodStopwatch.ElapsedMilliseconds, $"Input client is null for file: {fileFullName}.");
                return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>>();
            }
            // --- End Guard Clauses ---

            InvoiceProcessingContext context = null; // Declare context outside try block

            try
            {
                 FileInfo fileInfo;
                 try
                 {
                     var fileInfoStopwatch = Stopwatch.StartNew();
                     logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(Import), "FileInfoCreation", "Creating FileInfo object.", $"FileName: {fileFullName}", "");
                     fileInfo = new FileInfo(fileFullName);
                     fileInfoStopwatch.Stop();
                     logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                              nameof(Import), "FileInfoCreation", "FileInfo object created.", $"Duration: {fileInfoStopwatch.ElapsedMilliseconds}ms, Exists: {fileInfo.Exists}", "");

                     if (!fileInfo.Exists)
                     {
                         methodStopwatch.Stop();
                         logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                             nameof(Import), "Import a single file and process it through the pipeline", methodStopwatch.ElapsedMilliseconds, $"File does not exist: {fileFullName}.");
                         logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                             nameof(Import), "FileInfo check", methodStopwatch.ElapsedMilliseconds, $"File does not exist: {fileFullName}.");
                         return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>> {
                             new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                 fileFullName, (fileFullName, "Unknown", ImportStatus.Failed))
                         };
                     }
                 }
                 catch (Exception ex) when (ex is ArgumentException || ex is PathTooLongException || ex is NotSupportedException || ex is System.Security.SecurityException || ex is UnauthorizedAccessException || ex is IOException)
                 {
                     var fileInfoStopwatch = Stopwatch.StartNew(); // Start a new stopwatch for the catch block duration
                     methodStopwatch.Stop();
                     logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Import), "Import a single file and process it through the pipeline", methodStopwatch.ElapsedMilliseconds, $"Error creating FileInfo or checking existence for: {fileFullName}. Error: {ex.Message}");
                     logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Import) + " - FileInfo", methodStopwatch.ElapsedMilliseconds, $"Error creating FileInfo or checking existence for: {fileFullName}. Error: {ex.Message}");
                      return new List<KeyValuePair<string, (string file, string, ImportStatus Success)>> {
                             new KeyValuePair<string, (string file, string, ImportStatus Success)>(
                                 fileFullName, (fileFullName, "Unknown", ImportStatus.Failed))
                         };
                 }


                LogCreatingInvoiceProcessingContext(logger); // Pass logger
                // Context creation is less likely to fail, but keep in outer try
                context = new InvoiceProcessingContext(logger)
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
                    Imports = new Dictionary<string, (string file, string, ImportStatus Success)>(),
                    
                    // Errors list initialized automatically
                };
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(Import), "ContextCreation", "InvoiceProcessingContext created.", $"ContextHashCode: {context.GetHashCode()}", "");


                var pipelineSetupStopwatch = Stopwatch.StartNew();
                LogStartingPipeline(fileFullName, logger); // Pass logger
                var pipe = new InvoiceProcessingPipeline(context, false, logger); // Pass logger to pipeline constructor
                pipelineSetupStopwatch.Stop();
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                    nameof(Import), "PipelineSetup", "InvoiceProcessingPipeline setup complete.", $"Duration: {pipelineSetupStopwatch.ElapsedMilliseconds}ms", "");

                bool pipeResult = false; // Default to false

                // --- Pipeline Execution ---
                var pipelineExecutionStopwatch = Stopwatch.StartNew();
                logger.Information("INVOKING_OPERATION: {OperationDescription}. Async Expectation: {AsyncExpectation}",
                    "InvoiceProcessingPipeline.RunPipeline", "Yes");
                try
                {
                    pipeResult = await pipe.RunPipeline(logger).ConfigureAwait(false);
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                        nameof(Import), "PipelineExecution", "InvoiceProcessingPipeline.RunPipeline completed.", $"Result: {pipeResult}", "");
                    pipelineExecutionStopwatch.Stop();
                    logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                            "InvoiceProcessingPipeline.RunPipeline", pipelineExecutionStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");
                }
                catch (Exception pipeEx)
                {
                    pipelineExecutionStopwatch.Stop();
                    // Catch errors specifically from RunPipeline itself (should be rare if PipelineRunner catches step errors)
                    string errorMsg = $"Unexpected error during pipeline execution for file: {fileFullName}";
                    logger.Error(pipeEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        "InvoiceProcessingPipeline.RunPipeline", "Pipeline execution", pipelineExecutionStopwatch.ElapsedMilliseconds, errorMsg);
                    context.AddError($"{errorMsg}: {pipeEx.Message}"); // Add error to context
                    pipeResult = false; // Ensure result indicates failure
                }
                // --- End Pipeline Execution ---


                if (!pipeResult)
                {
                    // Pipeline reported failure (either returned false or caught exception)
                     logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                                nameof(Import), "PipelineResultCheck", "Pipeline execution failed or reported unsuccessful.", $"FileName: {fileFullName}, Errors: {string.Join("; ", context.Errors)}", "");
                     // Ensure the Imports dictionary reflects the failure if it's empty
                     if (!context.Imports.Any())
                     {
                         context.Imports.Add(fileFullName, (fileFullName, context.FileType?.Description ?? "Unknown", ImportStatus.Failed));
                     }
                }

                LogPipelineCompleted(context.Imports.Count, logger); // Pass logger

                methodStopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Import), pipeResult ? "File imported successfully through pipeline" : "File import pipeline completed with failures", $"ImportedFileCount: {context.Imports.Count(i => i.Value.Success == ImportStatus.Success)}, FailedFileCount: {context.Imports.Count(i => i.Value.Success == ImportStatus.Failed)}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(Import), pipeResult ? $"Successfully imported file: {fileFullName}" : $"Failed to import file: {fileFullName}", methodStopwatch.ElapsedMilliseconds);

                return context.Imports.ToList(); // Return results, including failures recorded in context.Imports
            }
            catch (Exception ex)
            {
                // Catch broader errors in the Import method setup (e.g., context creation)
                 methodStopwatch.Stop();
                 logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Import), "Import a single file and process it through the pipeline", methodStopwatch.ElapsedMilliseconds, $"Unexpected error during Import setup for file: {fileFullName}. Error: {ex.Message}");
                 logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Import), "Import setup", methodStopwatch.ElapsedMilliseconds, $"Unexpected error during Import setup for file: {fileFullName}. Error: {ex.Message}");
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

         public static async Task<string> GetPdftxt(string fileFullName, ILogger logger)
         {
             var methodStopwatch = Stopwatch.StartNew();
             // InvocationId is pushed in the calling method (ImportPDF or Import)
             // using (LogContext.PushProperty("InvocationId", methodInvocationId)) // New InvocationId for each GetPdftxt operation
             // {
             logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                              nameof(GetPdftxt), "Extract text content from a PDF file", $"FileName: {fileFullName}");

             logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                              nameof(GetPdftxt), $"Extracting text from PDF file: {fileFullName}");

             // Guard Clause: Check for null/empty input
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                methodStopwatch.Stop();
                logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetPdftxt), "Extract text content from a PDF file", methodStopwatch.ElapsedMilliseconds, "Input fileFullName is null or empty.");
                logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetPdftxt), "Input validation", methodStopwatch.ElapsedMilliseconds, "Input fileFullName is null or empty.");
                return null; // Indicate failure
            }

            InvoiceProcessingContext context = null;
            try
            {
                 FileInfo fileInfo;
                 try
                 {
                     var fileInfoStopwatch = Stopwatch.StartNew();
                     logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(GetPdftxt), "FileInfoCreation", "Creating FileInfo object.", $"FileName: {fileFullName}", "");
                     fileInfo = new FileInfo(fileFullName);
                     fileInfoStopwatch.Stop();
                     logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                              nameof(GetPdftxt), "FileInfoCreation", "FileInfo object created.", $"Duration: {fileInfoStopwatch.ElapsedMilliseconds}ms, Exists: {fileInfo.Exists}", "");

                   if (!fileInfo.Exists) // Check if file exists
                  {
                      methodStopwatch.Stop();
                      logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                          nameof(GetPdftxt), "Extract text content from a PDF file", methodStopwatch.ElapsedMilliseconds, $"File does not exist in GetPdftxt: {fileFullName}");
                      logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                          nameof(GetPdftxt), "FileInfo check", methodStopwatch.ElapsedMilliseconds, $"File does not exist in GetPdftxt: {fileFullName}");
                      return null; // Indicate failure
                  }
              }
              catch (Exception ex) when (ex is ArgumentException || ex is PathTooLongException || ex is NotSupportedException || ex is System.Security.SecurityException || ex is UnauthorizedAccessException || ex is IOException)
              {
                  var fileInfoStopwatch = Stopwatch.StartNew(); // Start a new stopwatch for the catch block duration
                  methodStopwatch.Stop();
                  logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                      nameof(GetPdftxt), "Extract text content from a PDF file", methodStopwatch.ElapsedMilliseconds, $"Error creating FileInfo or checking existence in GetPdftxt for: {fileFullName}. Error: {ex.Message}");
                  logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                      nameof(GetPdftxt) + " - FileInfo", methodStopwatch.ElapsedMilliseconds, $"Error creating FileInfo or checking existence in GetPdftxt for: {fileFullName}. Error: {ex.Message}");
                  return null; // Indicate failure
              }


             // Create a minimal context for the step
             logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(GetPdftxt), "ContextCreation", "Creating minimal context for GetPdftxt.", "", "");
             context = new InvoiceProcessingContext(logger)
             {
                 FilePath = fileFullName,
                 FileInfo = fileInfo, // Use validated FileInfo
                 PdfText = new StringBuilder()
                 // No need for other properties like FileType, Client etc. for this specific task
             };
              logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(GetPdftxt), "ContextCreation", "Minimal context created for GetPdftxt.", $"ContextHashCode: {context.GetHashCode()}", "");


             var pdfStep = new GetPdfTextStep();
             bool stepResult = false;

             // --- Step Execution ---
             var stepExecutionStopwatch = Stopwatch.StartNew();
             logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                              "GetPdfTextStep.Execute", "ASYNC_EXPECTED");
             try
             {
                  logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]", nameof(GetPdftxt), "StepExecution", "Executing GetPdfTextStep.", $"FileName: {fileFullName}");
                 stepResult = await pdfStep.Execute(context).ConfigureAwait(false); // Pass logger to step execution
                  stepExecutionStopwatch.Stop();
                  logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                       "GetPdfTextStep.Execute", stepExecutionStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");
             }
             catch (Exception stepEx)
             {
                 stepExecutionStopwatch.Stop();
                 logger.Error(stepEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     "GetPdfTextStep.Execute", "Step execution", stepExecutionStopwatch.ElapsedMilliseconds, $"Error executing GetPdfTextStep for file: {fileFullName}. Error: {stepEx.Message}");
                 // Cannot add to context.Errors as it's local and not returned. Logging is key.
                 stepResult = false; // Indicate failure
             }
              // --- End Step Execution ---

             if (!stepResult)
             {
                 methodStopwatch.Stop();
                 logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]", nameof(GetPdftxt), "StepResultCheck", "GetPdfTextStep reported failure.", $"FileName: {fileFullName}");
                 logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(GetPdftxt), "Extract text content from a PDF file", methodStopwatch.ElapsedMilliseconds, $"GetPdfTextStep reported failure for file: {fileFullName}.");
                 logger.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(GetPdftxt), "Step result check", methodStopwatch.ElapsedMilliseconds, $"GetPdfTextStep reported failure for file: {fileFullName}.");
                 return null; // Indicate failure
             }

             methodStopwatch.Stop();
             logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                 nameof(GetPdftxt), "Successfully extracted text", $"ExtractedTextLength: {context.PdfText.Length}", methodStopwatch.ElapsedMilliseconds);
             logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                 nameof(GetPdftxt), $"Successfully extracted text from file: {fileFullName}", methodStopwatch.ElapsedMilliseconds);
             return context.PdfText.ToString();
         }
         catch (Exception ex)
         {
             // Catch broader errors (e.g., context creation - unlikely here)
             methodStopwatch.Stop();
             logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                 nameof(GetPdftxt), "Extract text content from a PDF file", methodStopwatch.ElapsedMilliseconds, $"Unexpected error during GetPdftxt for file: {fileFullName}. Error: {ex.Message}");
             logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                 nameof(GetPdftxt), "Unexpected error", methodStopwatch.ElapsedMilliseconds, $"Unexpected error during GetPdftxt for file: {fileFullName}. Error: {ex.Message}");
             return null; // Indicate failure
         }
     }
        public static bool IsInvoiceDocument(Templates ocrTemplate, string fileText, string fileName, ILogger logger)
        {
            return GetPossibleInvoicesStep.IsInvoiceDocument(new Template(ocrTemplate, logger), fileText, fileName, logger);
        }

        private static void LogStartPDFImport(int fileCount, FileTypes fileType, ILogger logger)
        {
            logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ImportPDF), "Initialization", "Starting PDF import process.", $"FileCount: {fileCount}, FileTypeId: {fileType?.Id}, FileTypeName: {fileType?.Description}", "");
        }

        private static void LogProcessingFile(string fileName, ILogger logger)
        {
             logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            "ProcessSinglePDFFile", "StartProcessing", "Processing individual file.", $"FileName: {fileName}");
        }

        private static void LogQueryingDatabase(ILogger logger)
        {
            logger.Information("INVOKING_OPERATION: {OperationDescription}. Async Expectation: {AsyncExpectation}",
                                    "Query database for existing attachments", "No");
        }

        private static void LogResolvedEmailAndFileType(string emailId, int? fileTypeId, ILogger logger)
        {
             logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                                                        "ProcessSinglePDFFile", "ResolveMetadata", "Resolved file metadata.", $"EmailId: {emailId}, FileTypeId: {fileTypeId}", "");
        }

        private static void LogCreatingInvoiceProcessingContext(ILogger logger)
        {
            logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(Import), "ContextCreation", "Creating InvoiceProcessingContext.", "", "");
        }

        private static void LogStartingPipeline(string fileName, ILogger logger)
        {
            logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(Import), "PipelineSetup", "Setting up InvoiceProcessingPipeline.", $"FileName: {fileName}", "");
        }

        private static void LogPipelineCompleted(int importCount, ILogger logger)
        {
            logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                    nameof(Import), "PipelineCompletion", "Pipeline processing completed.", $"ImportsCount: {importCount}", "");
        }

        private static void LogPDFImportCompleted(int successCount, ILogger logger)
        {
            logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ImportPDF), "CompletionSummary", "PDF import loop completed.", $"SuccessfulImports: {successCount}", "");
        }



    }
}
