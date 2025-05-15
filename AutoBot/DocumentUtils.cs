using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace AutoBot
{
    public class DocumentUtils
    {
        private static readonly ILogger _log = Log.ForContext<DocumentUtils>(); // Add static logger

        public static async Task ImportPOEntries(ILogger log, bool overwriteExisting) // Add ILogger parameter
        {
            string methodName = nameof(ImportPOEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ OverwriteExisting: {OverwriteExisting} }}",
                methodName, "N/A", overwriteExisting); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Starting PO entries import.", methodName, "StartImport"); // Add step log
                // Console.WriteLine("Import Entries"); // Remove Console.WriteLine

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting XML file types.", methodName, "GetFileTypes"); // Add step log
                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportEntries (with fileTypes and DateTime.Today.AddHours(-12))", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var importEntriesStopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (await ImportEntries(overwriteExisting, fileTypes, log, DateTime.Today.AddHours(-12)).ConfigureAwait(false)) // Pass log
                {
                    importEntriesStopwatch.Stop();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: true",
                        "ImportEntries (with fileTypes and DateTime.Today.AddHours(-12))", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned true, indicating no files to import."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                    stopwatch.Stop(); // Stop stopwatch
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                    return;
                }
                importEntriesStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: false",
                       "ImportEntries (with fileTypes and DateTime.Today.AddHours(-12))", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned false, indicating files were processed.");


                //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting); // This call needs instrumentation too

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.RemoveDuplicateEntries", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var removeDuplicatesStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false); // Need to check if this method accepts ILogger
                removeDuplicatesStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EntryDocSetUtils.RemoveDuplicateEntries", removeDuplicatesStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.FixIncompleteEntries", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var fixIncompleteStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false); // Need to check if this method accepts ILogger
                fixIncompleteStopwatch.Stop();
                 log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EntryDocSetUtils.FixIncompleteEntries", fixIncompleteStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        public static async Task ImportAllSalesEntries(ILogger log, bool overwriteExisting) // Add ILogger parameter
        {
            string methodName = nameof(ImportAllSalesEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ OverwriteExisting: {OverwriteExisting} }}",
                methodName, "N/A", overwriteExisting); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Starting all sales entries import.", methodName, "StartImport"); // Add step log
                // Console.WriteLine("Import Entries"); // Remove Console.WriteLine

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting XML file types.", methodName, "GetFileTypes"); // Add step log
                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportEntries (with fileTypes and DateTime.MinValue)", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var importEntriesStopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (await ImportEntries(overwriteExisting, fileTypes, log, DateTime.MinValue).ConfigureAwait(false)) // Pass log
                {
                    importEntriesStopwatch.Stop();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: true",
                        "ImportEntries (with fileTypes and DateTime.MinValue)", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned true, indicating no files to import."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                    stopwatch.Stop(); // Stop stopwatch
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                    return;
                }
                importEntriesStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: false",
                       "ImportEntries (with fileTypes and DateTime.MinValue)", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned false, indicating files were processed."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log


                //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting); // This call needs instrumentation too

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.RemoveDuplicateEntries", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var removeDuplicatesStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false); // Need to check if this method accepts ILogger
                removeDuplicatesStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EntryDocSetUtils.RemoveDuplicateEntries", removeDuplicatesStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.FixIncompleteEntries", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var fixIncompleteStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false); // Need to check if this method accepts ILogger
                fixIncompleteStopwatch.Stop();
                 log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EntryDocSetUtils.FixIncompleteEntries", fixIncompleteStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        public static async Task ImportSalesEntries(ILogger log, bool overwriteExisting) // Already instrumented, keep as is
        {
            string methodName = nameof(ImportSalesEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ OverwriteExisting: {OverwriteExisting} }}",
                methodName, "N/A", overwriteExisting);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting XML file types.", methodName, "GetFileTypes");
                var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportEntries (with fileTypes)", "ASYNC_EXPECTED");
                var importEntriesStopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (await ImportEntries(overwriteExisting, fileTypes, log).ConfigureAwait(false))
                {
                    importEntriesStopwatch.Stop();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: true",
                        "ImportEntries (with fileTypes)", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned true, indicating no files to import.");

                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }
                 importEntriesStopwatch.Stop();
                 log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ImportEntries (with fileTypes)", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned false, indicating files were processed.");


                //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting); // This call needs instrumentation too

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.RemoveDuplicateEntries", "ASYNC_EXPECTED");
                var removeDuplicatesStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false);
                removeDuplicatesStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EntryDocSetUtils.RemoveDuplicateEntries", removeDuplicatesStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.FixIncompleteEntries", "ASYNC_EXPECTED");
                var fixIncompleteStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false);
                fixIncompleteStopwatch.Stop();
                 log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EntryDocSetUtils.FixIncompleteEntries", fixIncompleteStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task ImportEntries(bool overwriteExisting, string fileLst, ILogger log) // Already instrumented, keep as is
        {
           string methodName = nameof(ImportEntries);
           log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ OverwriteExisting: {OverwriteExisting}, FileList: {FileList} }}",
               methodName, "N/A", overwriteExisting, fileLst);
           var stopwatch = System.Diagnostics.Stopwatch.StartNew();

           try
           {
               log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting XML file types.", methodName, "GetFileTypes");
               var fileTypes = FileTypeManager.FileFormats.GetFileTypes(FileTypeManager.FileFormats.XML);

               log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportEntries (with fileTypes and fileLst)", "ASYNC_EXPECTED");
               var importEntriesStopwatch = System.Diagnostics.Stopwatch.StartNew();
               if (await ImportEntries(overwriteExisting, fileTypes, fileLst).ConfigureAwait(false))
               {
                   importEntriesStopwatch.Stop();
                   log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: true",
                       "ImportEntries (with fileTypes and fileLst)", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned true, indicating no files to import.");

                   stopwatch.Stop();
                   log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                       methodName, stopwatch.ElapsedMilliseconds);
                   return;
               }
               importEntriesStopwatch.Stop();
               log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                      "ImportEntries (with fileTypes and fileLst)", importEntriesStopwatch.ElapsedMilliseconds, "Async call completed (await). ImportEntries returned false, indicating files were processed.");


               //ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(overwriteExisting); // This call needs instrumentation too

               log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.RemoveDuplicateEntries", "ASYNC_EXPECTED");
               var removeDuplicatesStopwatch = System.Diagnostics.Stopwatch.StartNew();
               await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false);
               removeDuplicatesStopwatch.Stop();
               log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                   "EntryDocSetUtils.RemoveDuplicateEntries", removeDuplicatesStopwatch.ElapsedMilliseconds, "Async call completed (await).");

               log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.FixIncompleteEntries", "ASYNC_EXPECTED");
               var fixIncompleteStopwatch = System.Diagnostics.Stopwatch.StartNew();
               await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false);
               fixIncompleteStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                   "EntryDocSetUtils.FixIncompleteEntries", fixIncompleteStopwatch.ElapsedMilliseconds, "Async call completed (await).");

               stopwatch.Stop();
               log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                   methodName, stopwatch.ElapsedMilliseconds);
               return false;
           }
           catch (Exception e)
           {
               stopwatch.Stop();
               log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                   methodName, stopwatch.ElapsedMilliseconds, e.Message);
               throw;
           }
        }

        private static async Task<bool> ImportEntries(bool overwriteExisting, List<FileTypes> fileTypes, ILogger log, DateTime? getMinFileDate = null) // Already instrumented, keep as is
        {
            string methodName = nameof(ImportEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ OverwriteExisting: {OverwriteExisting}, FileTypeCount: {FileTypeCount}, GetMinFileDate: {GetMinFileDate} }}",
                methodName, "DocumentUtils.ImportSalesEntries/ImportAllSalesEntries", overwriteExisting, fileTypes?.Count ?? 0, getMinFileDate);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (!fileTypes.Any())
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No file types provided. Exiting.", methodName, "Validation");
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return true;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetDefaultAsycudaDocumentSetId", "ASYNC_EXPECTED");
                var docSetId = await GetDefaultAsycudaDocumentSetId(log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: {DocSetId}",
                    "GetDefaultAsycudaDocumentSetId", stopwatch.ElapsedMilliseconds, "Async call completed (await).", docSetId);


                DateTime minFileDate = GetDocSetLastFileDate(log, docSetId); // Pass log
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Using minimum file date: {MinFileDate}", methodName, "DetermineMinDate", minFileDate);

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting file type files.", methodName, "GetFileTypeFiles");
                var fileTypeFiles = GetFileTypeFiles(log, fileTypes, docSetId, minFileDate); // Pass log

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Importing entries.", methodName, "ImportEntries");
                ImportEntries(overwriteExisting, fileTypeFiles, log); // Pass the logger to the void method

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
                return false;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        private static IEnumerable<(FileTypes FileType, List<FileInfo> Files)> GetFileTypeFiles(ILogger log, List<FileTypes> fileTypes, int docSetId, DateTime lastfiledate)
        {
            string methodName = nameof(GetFileTypeFiles);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeCount: {FileTypeCount}, DocSetId: {DocSetId}, LastFileDate: {LastFileDate} }}",
                methodName, "DocumentUtils.ImportEntries", fileTypes?.Count ?? 0, docSetId, lastfiledate);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetDocSetDirectoryInfo", "SYNC_EXPECTED");
                var directoryInfo = GetDocSetDirectoryInfo(log, docSetId);
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetDocSetDirectoryInfo", "Sync call returned.");

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Refreshing directory info.", methodName, "Refresh");
                directoryInfo.Refresh();

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Filtering files by pattern and date.", methodName, "FilterFiles");
                var fileTypeFiles = fileTypes.Select(ft => (FileType: ft, Files: directoryInfo.GetFiles()
                    .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                    .Where(x => x.LastWriteTime >= lastfiledate)
                    .ToList()));

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Directory: {DirectoryPath}",
                    methodName, stopwatch.ElapsedMilliseconds, directoryInfo.FullName);
                return fileTypeFiles;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        private static IEnumerable<(FileTypes FileType, List<FileInfo> Files)> GetFileTypeFiles(ILogger log, List<FileTypes> fileTypes, int docSetId, string filelst)
        {
            string methodName = nameof(GetFileTypeFiles);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeCount: {FileTypeCount}, DocSetId: {DocSetId}, FileListLength: {FileListLength} }}",
                methodName, "DocumentUtils.ImportEntries", fileTypes?.Count ?? 0, docSetId, filelst?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetDocSetDirectoryInfo", "SYNC_EXPECTED");
                var directoryInfo = GetDocSetDirectoryInfo(log, docSetId);
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetDocSetDirectoryInfo", "Sync call returned.");

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Splitting file list and filtering files.", methodName, "FilterFiles");
                var files = filelst.Split(new[] { "\r\n", " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                var fileTypeFiles = fileTypes.Select(ft => (FileType: ft, Files: directoryInfo.GetFiles()
                                                                   .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                                                                   .Where(x => files.Any(z => x.FullName.Contains($"-{z}.xml")))
                                                                   .ToList()));

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
                return fileTypeFiles;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        private static void ImportEntries(bool overwriteExisting, IEnumerable<(FileTypes FileType, List<FileInfo> Files)> fileTypeFiles, ILogger log)
       {
           string methodName = nameof(ImportEntries);
           // Note: This is a void method, so METHOD_ENTRY/EXIT might be less critical, but we'll add for consistency.
           // Also, it uses ForEach with async lambda, which can be tricky.
           log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ OverwriteExisting: {OverwriteExisting} }}",
               methodName, "DocumentUtils.ImportEntries", overwriteExisting);
           var stopwatch = System.Diagnostics.Stopwatch.StartNew();

           try
           {
               fileTypeFiles.ForEach(async ft =>
               {
                   // Each iteration of ForEach is a separate async operation.
                   // Propagating logger here is important.
                   using (LogContext.PushProperty("FileTypeId", ft.FileType.Id))
                   using (LogContext.PushProperty("FileCount", ft.Files.Count))
                   {
                       log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Processing FileType {FileTypeId} with {FileCount} files.",
                           methodName, "ProcessFileType", ft.FileType.Id, ft.Files.Count);

                       log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.GetAsycudaDocumentSet", "ASYNC_EXPECTED");
                       var getDocSetStopwatch = System.Diagnostics.Stopwatch.StartNew();
                       var asycudaDocumentSet = await WaterNut.DataSpace.EntryDocSetUtils.GetAsycudaDocumentSet(ft.FileType.DocSetRefernece, true).ConfigureAwait(false); // Need to check if this method accepts ILogger
                       getDocSetStopwatch.Stop();
                       log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: {DocSetSetReference}",
                           "EntryDocSetUtils.GetAsycudaDocumentSet", getDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await).", asycudaDocumentSet?.Declarant_Reference_Number);

                       log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.ImportDocuments", "ASYNC_EXPECTED");
                       var importDocsStopwatch = System.Diagnostics.Stopwatch.StartNew();
                       await BaseDataModel.Instance.ImportDocuments(
                           asycudaDocumentSet
                               .AsycudaDocumentSetId,
                           ft.Files.Select(x => x.FullName).ToList(), true, true, false, overwriteExisting, true).ConfigureAwait(false); // Need to check if this method accepts ILogger
                       importDocsStopwatch.Stop();
                       log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                           "BaseDataModel.Instance.ImportDocuments", importDocsStopwatch.ElapsedMilliseconds, "Async call completed (await).");
                   }
               });

               stopwatch.Stop();
               log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                   methodName, stopwatch.ElapsedMilliseconds);
           }
           catch (Exception e)
           {
               stopwatch.Stop();
               log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                   methodName, stopwatch.ElapsedMilliseconds, e.Message);
               // Note: Rethrowing from a void async method in ForEach can be problematic.
               // Consider logging and potentially handling differently if this causes issues.
               throw;
           }
       }

        private static DateTime GetDocSetLastFileDate(ILogger log, int docSetId)
        {
            string methodName = nameof(GetDocSetLastFileDate);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.ImportEntries", docSetId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetAsycudaDocumentSetAttachments", "SYNC_EXPECTED");
                var lastDbFile = GetAsycudaDocumentSetAttachments(log, docSetId);
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetAsycudaDocumentSetAttachments", "Sync call returned.");

                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetFileCreationDate", "SYNC_EXPECTED");
                var result = GetFileCreationDate(log, lastDbFile);
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetFileCreationDate", "Sync call returned.");

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {ResultDate}",
                    methodName, stopwatch.ElapsedMilliseconds, result);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        private static DirectoryInfo GetDocSetDirectoryInfo(ILogger log, int docSetId)
        {
            string methodName = nameof(GetDocSetDirectoryInfo);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetFileTypeFiles", docSetId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetDocSetDestinationFolder", "SYNC_EXPECTED");
                var docSetDirectoryInfo = new DirectoryInfo(GetDocSetDestinationFolder(log, docSetId));
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetDocSetDestinationFolder", "Sync call returned.");

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Refreshing directory info.", methodName, "Refresh");
                docSetDirectoryInfo.Refresh();

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Directory: {DirectoryPath}",
                    methodName, stopwatch.ElapsedMilliseconds, docSetDirectoryInfo.FullName);
                return docSetDirectoryInfo;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        private static string GetDocSetDestinationFolder(ILogger log, int docSetId) // Add ILogger parameter
        {
            string methodName = nameof(GetDocSetDestinationFolder);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetDocSetDirectoryInfo", docSetId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetDocSetReference", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var docSetReference = GetDocSetReference(log, docSetId); // Pass log
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetDocSetReference", "Sync call returned."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Getting application data path.", methodName, "GetAppDataPath"); // Add step log
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Combining path components.", methodName, "CombinePath"); // Add step log
                var result = Path.Combine(appDataPath, "WaterNut", "AsycudaDocumentSets", docSetReference);

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {ResultPath}",
                    methodName, stopwatch.ElapsedMilliseconds, result); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static string GetDocSetReference(ILogger log, int docSetId) // Add ILogger parameter
        {
            string methodName = nameof(GetDocSetReference);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetDocSetDestinationFolder", docSetId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Getting DocSet reference from database.", methodName, "QueryDb"); // Add step log
                using (var ctx = new CoreEntitiesContext())
                {
                    // INVOKING_OPERATION for LINQ query
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "LINQ query to get DocSet reference", "SYNC_EXPECTED");
                    var docSetReference = ctx.AsycudaDocumentSets.Where(x => x.AsycudaDocumentSetId == docSetId)
                        .Select(x => x.Declarant_Reference_Number).FirstOrDefault();
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "LINQ query to get DocSet reference", "Sync call returned.");

                    stopwatch.Stop(); // Stop stopwatch
                    log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {DocSetReference}",
                        methodName, stopwatch.ElapsedMilliseconds, docSetReference); // Add METHOD_EXIT_SUCCESS log
                    return docSetReference;
                }
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static DateTime GetFileCreationDate(ILogger log, AsycudaDocumentSet_Attachments lastDbFile) // Add ILogger parameter
        {
            string methodName = nameof(GetFileCreationDate);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ LastDbFileId: {LastDbFileId} }}",
                methodName, "DocumentUtils.GetDocSetLastFileDate", lastDbFile?.AsycudaDocumentSet_AttachmentsId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Determining file creation date.", methodName, "DetermineDate"); // Add step log
                var result = lastDbFile?.FileCreationDate ?? DateTime.MinValue;

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {ResultDate}",
                    methodName, stopwatch.ElapsedMilliseconds, result); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static AsycudaDocumentSet_Attachments GetAsycudaDocumentSetAttachments(ILogger log, int docSetId) // Add ILogger parameter
        {
            string methodName = nameof(GetAsycudaDocumentSetAttachments);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetDocSetLastFileDate", docSetId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Getting last attachment from database.", methodName, "QueryDb"); // Add step log
                using (var ctx = new CoreEntitiesContext())
                {
                    // INVOKING_OPERATION for LINQ query
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "LINQ query to get last attachment", "SYNC_EXPECTED");
                    var lastDbFile = ctx.AsycudaDocumentSet_Attachments.Where(x => x.AsycudaDocumentSetId == docSetId)
                        .OrderByDescending(x => x.FileCreationDate).FirstOrDefault();
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "LINQ query to get last attachment", "Sync call returned.");

                    stopwatch.Stop(); // Stop stopwatch
                    log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result Found: {ResultFound}",
                        methodName, stopwatch.ElapsedMilliseconds, lastDbFile != null); // Add METHOD_EXIT_SUCCESS log
                    return lastDbFile;
                }
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static async Task<int> GetDefaultAsycudaDocumentSetId(ILogger log) // Add ILogger parameter
        {
            string methodName = nameof(GetDefaultAsycudaDocumentSetId);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "DocumentUtils.ImportEntries"); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Getting default Asycuda Document Set ID.", methodName, "GetDefault"); // Add step log
                // Assuming BaseDataModel.Instance.CurrentApplicationSettings is accessible and contains DefaultAsycudaDocumentSetId
                var result = BaseDataModel.Instance.CurrentApplicationSettings.DefaultAsycudaDocumentSetId;

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {ResultId}",
                    methodName, stopwatch.ElapsedMilliseconds, result); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }
    }
}

        private static string GetDocSetDestinationFolder(ILogger log, int docSetId) // Add ILogger parameter
        {
            string methodName = nameof(GetDocSetDestinationFolder);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetDocSetDirectoryInfo", docSetId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetDocSetReference", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var result = BaseDataModel.GetDocSetDirectoryName(GetDocSetReference(log, docSetId)); // Pass log
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "GetDocSetReference", "Sync call returned."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Folder: {FolderPath}",
                    methodName, stopwatch.ElapsedMilliseconds, result); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static string GetDocSetReference(ILogger log, int docSetId) // Add ILogger parameter
        {
            string methodName = nameof(GetDocSetReference);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetDocSetDestinationFolder", docSetId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "DocumentDSContext.AsycudaDocumentSets.Where(...).Select(...).First()", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var result = new DocumentDSContext().AsycudaDocumentSets.Where(x => x.AsycudaDocumentSetId == docSetId)
                    .Select(x => x.Declarant_Reference_Number).First();
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}). Result: {DocSetReference}",
                    "DocumentDSContext.AsycudaDocumentSets.Where(...).Select(...).First()", stopwatch.ElapsedMilliseconds, "Sync call returned.", result); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static DateTime GetFileCreationDate(ILogger log, AsycudaDocumentSet_Attachments lastDbFile) // Add ILogger parameter
        {
            string methodName = nameof(GetFileCreationDate);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ LastDbFileNull: {LastDbFileNull} }}",
                methodName, "DocumentUtils.GetDocSetLastFileDate", lastDbFile == null); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                DateTime result;
                if (lastDbFile != null)
                {
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "File.GetCreationTime", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                    result = File.GetCreationTime(lastDbFile.Attachments.FilePath);
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}). Result: {CreationTime}",
                        "File.GetCreationTime", stopwatch.ElapsedMilliseconds, "Sync call returned.", result); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log
                }
                else
                {
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Last DB file is null, using DateTime.Today.AddDays(-1).", methodName, "HandleNull"); // Add step log
                    result = DateTime.Today.AddDays(-1);
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {ResultDate}",
                    methodName, stopwatch.ElapsedMilliseconds, result); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static AsycudaDocumentSet_Attachments GetAsycudaDocumentSetAttachments(ILogger log, int docSetId) // Add ILogger parameter
        {
            string methodName = nameof(GetAsycudaDocumentSetAttachments);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId} }}",
                methodName, "DocumentUtils.GetDocSetLastFileDate", docSetId); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CoreEntitiesContext.AsycudaDocumentSet_Attachments.Include(...).OrderByDescending(...).FirstOrDefault(...)", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var result = new CoreEntitiesContext().AsycudaDocumentSet_Attachments
                    .Include(x => x.Attachments)
                    .OrderByDescending(x => x.AttachmentId)
                    .FirstOrDefault(x => x.AsycudaDocumentSetId == docSetId);
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}). ResultNull: {ResultNull}",
                    "CoreEntitiesContext.AsycudaDocumentSet_Attachments.Include(...).OrderByDescending(...).FirstOrDefault(...)", stopwatch.ElapsedMilliseconds, "Sync call returned.", result == null); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static async Task<int> GetDefaultAsycudaDocumentSetId(ILogger log) // Add ILogger parameter
        {
            string methodName = nameof(GetDefaultAsycudaDocumentSetId);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "DocumentUtils.ImportEntries"); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var currentSalesInfo = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CoreEntitiesContext.AsycudaDocumentSetExs.FirstOrDefault(...)", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                           x.ApplicationSettingsId ==
                           BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                           x.Declarant_Reference_Number == "Imports");
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}). ResultNull: {ResultNull}",
                    "CoreEntitiesContext.AsycudaDocumentSetExs.FirstOrDefault(...)", stopwatch.ElapsedMilliseconds, "Sync call returned.", docSet == null); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                var result = docSet?.AsycudaDocumentSetId ?? currentSalesInfo.Item3.AsycudaDocumentSetId;

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {DocSetId}",
                    methodName, stopwatch.ElapsedMilliseconds, result); // Add METHOD_EXIT_SUCCESS log
                return result;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }
    }
}