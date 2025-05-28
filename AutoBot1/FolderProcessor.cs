using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoBotUtilities;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums; // Added for ImportStatus
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils; // Added for FileTypeManager
using FileTypes = CoreEntities.Business.Entities.FileTypes; // Alias to resolve ambiguity
using WaterNut.DataSpace; // Assuming PDFUtils, ShipmentUtils etc. are accessible via this
using DocumentDS.Business.Entities; // Added for Attachments - Assuming this is the correct namespace
using TrackableEntities; // Added for TrackingState


using Serilog;
using Serilog.Context;
using System.Diagnostics; // Added for Stopwatch

namespace AutoBot
{
    // ReSharper disable once HollowTypeName
    public class FolderProcessor
    {
        private static readonly ILogger _log = Log.ForContext<FolderProcessor>();

        // Constructor could potentially take dependencies like ILogger, IEmailService etc. in a fuller refactoring
        public FolderProcessor()
        {
            // Initialize dependencies if needed
        }

        public async Task ProcessDownloadFolder(ApplicationSettings appSetting)
        {
            string invocationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("InvocationId", invocationId))
            using (LogContext.PushProperty("AppSettingId", appSetting?.ApplicationSettingsId))
            {
                var stopwatch = Stopwatch.StartNew();
                _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ProcessDownloadFolder), $"AppSettingId: {appSetting?.ApplicationSettingsId}");

                try
                {
                    var downloadFolder = new DirectoryInfo(Path.Combine(appSetting.DataFolder, "Downloads"));
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Checking download folder: {DownloadFolderPath}",
                        nameof(ProcessDownloadFolder), "CheckFolder", downloadFolder.FullName);

                    if (!downloadFolder.Exists)
                    {
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Download folder does not exist. Creating: {DownloadFolderPath}",
                            nameof(ProcessDownloadFolder), "CreateFolder", downloadFolder.FullName);
                        downloadFolder.Create();
                    }

                    var pdfFiles = downloadFolder.GetFiles("*.pdf").ToList();
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {FileCount} PDF files in download folder.",
                        nameof(ProcessDownloadFolder), "ListFiles", pdfFiles.Count);

                    if (!pdfFiles.Any())
                    {
                         _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                            nameof(ProcessDownloadFolder), "ProcessFilesLoop", "PDF files", "No PDF files found to process.");
                    }

                    foreach (var file in pdfFiles)
                    {
                        string fileInvocationId = Guid.NewGuid().ToString();
                        using (LogContext.PushProperty("InvocationId", fileInvocationId)) // New InvocationId for each file
                        {
                            var fileStopwatch = Stopwatch.StartNew();
                            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                                "ProcessDownloadedFile", $"FileName: {file.Name}");

                            try
                            {
                                await ProcessFile(appSetting, file).ConfigureAwait(false);
                                fileStopwatch.Stop();
                                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                                    "ProcessDownloadedFile", fileStopwatch.ElapsedMilliseconds);
                            }
                            catch (Exception e)
                            {
                                fileStopwatch.Stop();
                                _log.Error(e, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                                    "ProcessDownloadedFile", fileStopwatch.ElapsedMilliseconds);
                                // Decide if processing should continue or stop on error
                                continue;
                            }
                        } // End fileInvocationId LogContext
                    }

                    stopwatch.Stop();
                    _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ProcessDownloadFolder), stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                        nameof(ProcessDownloadFolder), stopwatch.ElapsedMilliseconds);
                    throw; // Re-throw if this is a critical failure for the overall process
                }
            }
        }

        private async Task ProcessFile(ApplicationSettings appSetting, FileInfo file)
        {
            // InvocationId is already pushed in the calling loop
            var stopwatch = Stopwatch.StartNew();
            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(ProcessFile), $"FileName: {file.Name}");

            try
            {
                var documentsFolder = CreateDocumentsFolder(appSetting, file);
                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Documents folder created/verified: {DocumentsFolderPath}",
                    nameof(ProcessFile), "CreateDocumentsFolder", documentsFolder.FullName);

                var destFileName = CopyFileToDocumentsFolder(file, documentsFolder);
                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): File copied to destination: {DestinationFileName}",
                    nameof(ProcessFile), "CopyFile", destFileName);

                var fileTypesStopwatch = Stopwatch.StartNew();
                _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}",
                    "GetUnknownFileTypes", "ASYNC_EXPECTED", file.Name);
                var fileTypes = await GetUnknownFileTypes(file).ConfigureAwait(false);
                fileTypesStopwatch.Stop();
                _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Found {FileTypeCount} unknown file types.",
                    "GetUnknownFileTypes", fileTypesStopwatch.ElapsedMilliseconds, "Async call completed (await).", fileTypes.Count);

                fileTypes.ForEach(x => x.EmailId = file.Name); // Set EmailId based on filename for context
                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Set EmailId for {FileTypeCount} file types based on filename.",
                    nameof(ProcessFile), "SetEmailId", fileTypes.Count);

                var processFileTypesStopwatch = Stopwatch.StartNew();
                _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}",
                    "ProcessFileTypes", "ASYNC_EXPECTED", file.Name);
                var allgood = await ProcessFileTypes(fileTypes, destFileName, file).ConfigureAwait(false);
                processFileTypesStopwatch.Stop();
                 _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: {Result}",
                    "ProcessFileTypes", processFileTypesStopwatch.ElapsedMilliseconds, "Async call completed (await).", allgood);


                if (allgood)
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): File processing successful. Attempting to delete original file: {OriginalFileName}",
                        nameof(ProcessFile), "DeleteOriginalFileAttempt", file.Name);
                    try
                    {
                        file.Delete();
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Original file deleted successfully: {OriginalFileName}",
                            nameof(ProcessFile), "DeleteOriginalFileSuccess", file.Name);
                    }
                    catch (IOException ex)
                    {
                         _log.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): Error deleting processed file {FileName}.",
                             nameof(ProcessFile), "DeleteOriginalFileError", file.Name);
                         // Log error, maybe retry later?
                    }
                }
                else
                {
                    _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): File processing not entirely successful for {FileName}. Skipping original file deletion.",
                        nameof(ProcessFile), "SkipDelete", file.Name);
                }

                stopwatch.Stop();
                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. AllGood: {AllGood}",
                    nameof(ProcessFile), stopwatch.ElapsedMilliseconds, allgood);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                    nameof(ProcessFile), stopwatch.ElapsedMilliseconds);
                throw; // Re-throw if this is a critical failure for processing this file
            }
        }

        private DirectoryInfo CreateDocumentsFolder(ApplicationSettings appSetting, FileInfo file)
        {
            // InvocationId is already pushed in the calling method
            var stopwatch = Stopwatch.StartNew();
            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to create/verify documents folder for file: {FileName}",
                nameof(CreateDocumentsFolder), "Start", file.Name);

            try
            {
                // Ensure filename characters are valid for a directory path
                var safeFolderName = string.Join("_", file.Name.Replace(file.Extension, "").Split(Path.GetInvalidFileNameChars()));
                var documentsFolderPath = Path.Combine(appSetting.DataFolder, "Documents", safeFolderName);
                var documentsFolder = new DirectoryInfo(documentsFolderPath);

                if (!documentsFolder.Exists)
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Documents folder does not exist. Creating: {DocumentsFolderPath}",
                        nameof(CreateDocumentsFolder), "CreateFolder", documentsFolderPath);
                    try
                    {
                         documentsFolder.Create();
                         _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Documents folder created successfully: {DocumentsFolderPath}",
                             nameof(CreateDocumentsFolder), "CreateFolderSuccess", documentsFolderPath);
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _log.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): Error creating directory {DocumentsFolderPath}.",
                            nameof(CreateDocumentsFolder), "CreateFolderError", documentsFolderPath);
                        // Handle error appropriately, maybe throw or return null/indicator
                        throw; // Rethrowing for now
                    }
                }
                else
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Documents folder already exists: {DocumentsFolderPath}",
                        nameof(CreateDocumentsFolder), "FolderExists", documentsFolderPath);
                }

                stopwatch.Stop();
                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Documents folder creation/verification complete. Duration: {DurationMs}ms. FolderPath: {DocumentsFolderPath}",
                    nameof(CreateDocumentsFolder), "End", stopwatch.ElapsedMilliseconds, documentsFolder.FullName);
                return documentsFolder;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): Unexpected error in CreateDocumentsFolder for file {FileName}. Duration: {DurationMs}ms.",
                    nameof(CreateDocumentsFolder), "UnexpectedError", file.Name, stopwatch.ElapsedMilliseconds);
                throw; // Re-throw if this is a critical failure
            }
        }

        private string CopyFileToDocumentsFolder(FileInfo file, DirectoryInfo documentsFolder)
        {
            // InvocationId is already pushed in the calling method
            var stopwatch = Stopwatch.StartNew();
            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to copy file '{FileName}' to '{DestinationFolder}'",
                nameof(CopyFileToDocumentsFolder), "Start", file.Name, documentsFolder.FullName);

            var destFileName = Path.Combine(documentsFolder.FullName, file.Name);
            try
            {
                if (!File.Exists(destFileName))
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Destination file does not exist. Copying '{FileName}' to '{DestinationFileName}'",
                        nameof(CopyFileToDocumentsFolder), "Copy", file.Name, destFileName);
                    file.CopyTo(destFileName);
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): File copied successfully to '{DestinationFileName}'",
                        nameof(CopyFileToDocumentsFolder), "CopySuccess", destFileName);
                }
                else
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Destination file already exists: '{DestinationFileName}'. Skipping copy.",
                        nameof(CopyFileToDocumentsFolder), "FileExists", destFileName);
                    // Else: Decide on overwrite logic if needed - Current logic skips copy if exists
                }

                stopwatch.Stop();
                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): File copy operation complete. Duration: {DurationMs}ms. Destination: {DestinationFileName}",
                    nameof(CopyFileToDocumentsFolder), "End", stopwatch.ElapsedMilliseconds, destFileName);
                return destFileName;
            }
            catch (IOException ex)
            {
                 stopwatch.Stop();
                 _log.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): Error copying file '{FileName}' to '{DestinationFileName}': {ErrorMessage}",
                     nameof(CopyFileToDocumentsFolder), "CopyError", file.Name, destFileName, ex.Message);
                 // Handle error, maybe throw
                 throw; // Rethrowing for now
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): Unexpected error in CopyFileToDocumentsFolder for file '{FileName}'. Duration: {DurationMs}ms.",
                    nameof(CopyFileToDocumentsFolder), "UnexpectedError", file.Name, stopwatch.ElapsedMilliseconds);
                throw; // Re-throw if this is a critical failure
            }
        }

        private async Task<List<FileTypes>> GetUnknownFileTypes(FileInfo file)
        {
            // InvocationId is already pushed in the calling method
            var stopwatch = Stopwatch.StartNew();
            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(GetUnknownFileTypes), $"FileName: {file.Name}");

            try
            {
                // Assuming FileTypeManager is static or accessible
                // Consider injecting IFileTypeManager if refactoring further
                _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}",
                    "FileTypeManager.GetImportableFileType", "ASYNC_EXPECTED", file.Name);
                var importableFileType = await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, file.FullName).ConfigureAwait(false);
                stopwatch.Stop();
                _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Found {Count} importable file types.",
                    "FileTypeManager.GetImportableFileType", stopwatch.ElapsedMilliseconds, "Async call completed (await).", importableFileType?.Count ?? 0);

                var unknownFileTypes = importableFileType
                    .Where(x => x.Description == "Unknown") // Filter specifically for "Unknown" type
                    .ToList();

                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Found {Count} 'Unknown' file types.",
                    nameof(GetUnknownFileTypes), stopwatch.ElapsedMilliseconds, unknownFileTypes.Count);
                return unknownFileTypes;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                    nameof(GetUnknownFileTypes), stopwatch.ElapsedMilliseconds);
                throw; // Re-throw if this is a critical failure
            }
        }

        private async Task<bool> ProcessFileTypes(List<FileTypes> fileTypes, string destFileName, FileInfo originalFile)
        {
            // InvocationId is already pushed in the calling method
            var stopwatch = Stopwatch.StartNew();
            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(ProcessFileTypes), $"FileTypesCount: {fileTypes.Count}, FileName: {originalFile.Name}");

            var allgood = true;
            try
            {
                if (!fileTypes.Any())
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                        nameof(ProcessFileTypes), "ProcessLoop", "fileTypes", "No file types to process.");
                }

                foreach (var fileType in fileTypes)
                {
                    string fileTypeProcessingInvocationId = Guid.NewGuid().ToString();
                    using (LogContext.PushProperty("InvocationId", fileTypeProcessingInvocationId)) // New InvocationId for each file type
                    using (LogContext.PushProperty("FileTypeId", fileType.Id))
                    using (LogContext.PushProperty("FilePattern", fileType.FilePattern))
                    {
                        var fileTypeStopwatch = Stopwatch.StartNew();
                        _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                            "ProcessSingleFileType", $"FileTypeId: {fileType.Id}, FilePattern: {fileType.FilePattern}");

                        try
                        {
                            var fileInfos = new FileInfo[] { new FileInfo(destFileName) };
                            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Preparing file info for import: {FileName}",
                                "ProcessSingleFileType", "PrepareFileInfo", destFileName);

                            var importPdfStopwatch = Stopwatch.StartNew();
                            _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}, FileType {FileTypeId}",
                                "InvoiceReader.InvoiceReader.ImportPDF", "ASYNC_EXPECTED", originalFile.Name, fileType.Id);
                            var res = await InvoiceReader.InvoiceReader.ImportPDF(fileInfos, fileType, _log).ConfigureAwait(false);
                            importPdfStopwatch.Stop();
                            _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). ImportResultCount: {ResultCount}",
                                "InvoiceReader.InvoiceReader.ImportPDF", importPdfStopwatch.ElapsedMilliseconds, "Async call completed (await).", res?.Count ?? 0);

                            // Call to TryDeepSeekImport is now synchronous as the method itself will be made synchronous
                            _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}, FileType {FileTypeId}",
                                "TryDeepSeekImport", "SYNC_EXPECTED", originalFile.Name, fileType.Id);
                            allgood = TryDeepSeekImport(originalFile, res, fileInfos, fileType, allgood);
                            _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Result: {Result}",
                                "TryDeepSeekImport", "Result", allgood);


                            // Only create shipment if initial PDF import or DeepSeek was successful for this fileType
                            var sendShipmentStopwatch = Stopwatch.StartNew();
                            _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}, FileType {FileTypeId}",
                                "TrySendShipmentEmail", "ASYNC_EXPECTED", originalFile.Name, fileType.Id);
                             allgood = await TrySendShipmentEmail(originalFile, allgood, fileType, fileInfos).ConfigureAwait(false);
                            sendShipmentStopwatch.Stop();
                            _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: {Result}",
                                "TrySendShipmentEmail", sendShipmentStopwatch.ElapsedMilliseconds, "Async call completed (await).", allgood);

                            fileTypeStopwatch.Stop();
                            _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. AllGood: {AllGood}",
                                "ProcessSingleFileType", fileTypeStopwatch.ElapsedMilliseconds, allgood);
                        }
                        catch (Exception ex)
                        {
                            fileTypeStopwatch.Stop();
                            _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                                "ProcessSingleFileType", fileTypeStopwatch.ElapsedMilliseconds);
                            allgood = false; // Mark as not all good if any file type fails
                            // Decide if processing should continue or stop on error for this file type
                        }
                    } // End fileTypeProcessingInvocationId LogContext
                }

                stopwatch.Stop();
                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Overall AllGood: {AllGood}",
                    nameof(ProcessFileTypes), stopwatch.ElapsedMilliseconds, allgood);
                return allgood; // Returns true only if all fileTypes processed successfully
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                    nameof(ProcessFileTypes), stopwatch.ElapsedMilliseconds);
                throw; // Re-throw if this is a critical failure for processing file types
            }
        }

        private static async Task<bool> TrySendShipmentEmail(FileInfo originalFile, bool allgood, FileTypes fileType, FileInfo[] fileInfos)
        {
            // InvocationId is already pushed in the calling method
            var stopwatch = Stopwatch.StartNew();
            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(TrySendShipmentEmail), $"FileName: {originalFile.Name}, FileTypeId: {fileType.Id}, InitialAllGood: {allgood}");

            bool resultAllGood = allgood; // Start with the incoming allgood status

            try
            {
                if (resultAllGood) // Check if still good after potential DeepSeek
                {
                    _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}, FileType {FileTypeId}",
                        "ShipmentUtils.CreateShipmentEmail", "ASYNC_EXPECTED", originalFile.Name, fileType.Id);
                    resultAllGood = await ShipmentUtils.CreateShipmentEmail(fileType, fileInfos, _log).ConfigureAwait(false);
                    stopwatch.Stop();
                    _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). Result: {Result}",
                        "ShipmentUtils.CreateShipmentEmail", stopwatch.ElapsedMilliseconds, "Async call completed (await).", resultAllGood);
                }
                else
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Initial 'allgood' status is false. Skipping shipment email creation for file {FileName}, FileType {FileTypeId}.",
                        nameof(TrySendShipmentEmail), "SkipCreateEmail", originalFile.Name, fileType.Id);
                }

                stopwatch.Stop();
                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Final AllGood: {FinalAllGood}",
                    nameof(TrySendShipmentEmail), stopwatch.ElapsedMilliseconds, resultAllGood);
                return resultAllGood;
            }
            catch(Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                    nameof(TrySendShipmentEmail), stopwatch.ElapsedMilliseconds);
                // Decide if this error should mark 'allgood' as false
                return false; // Mark as false on error
            }
        }

        // Removed async and Task<bool> as no await is performed with current commented code
        private bool TryDeepSeekImport(FileInfo originalFile, List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>> res, FileInfo[] fileInfos, FileTypes fileType,
            bool allgood)
        {
            // InvocationId is already pushed in the calling method
            var stopwatch = Stopwatch.StartNew();
            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(TryDeepSeekImport), $"FileName: {originalFile.Name}, FileTypeId: {fileType.Id}, InitialAllGood: {allgood}");

            bool resultAllGood = allgood; // Start with the incoming allgood status

            try
            {
                if (!res.All(x =>
                        // x.Value.DocumentType.ToString() == FileTypeManager.EntryTypes.ShipmentInvoice &&  // --- took this out because failed sad cant email cuz need manifest
                        x.Value.Status == ImportStatus.Success))
                {
                    _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Initial import results indicate failure for file {FileName}, FileType {FileTypeId}. Attempting DeepSeek (commented out).",
                        nameof(TryDeepSeekImport), "InitialImportFailed", originalFile.Name, fileType.Id);

                    //var res2 = await PDFUtils.ImportPDFDeepSeek(fileInfos, fileType).ConfigureAwait(false); // This line is commented out
                    //if (res2.Any()
                    //    && !res2.Any(x => x.Value.status != ImportStatus.Success)) return allgood; // This line is commented out
                    //NotifyUnknownPDF(originalFile, res2); // If this were uncommented, TryDeepSeekImport would need to be async Task // This line is commented out

                    resultAllGood = false; // Mark as false since initial import failed and DeepSeek is commented out
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): DeepSeek logic is commented out. Marking 'allgood' as false for file {FileName}, FileType {FileTypeId}.",
                        nameof(TryDeepSeekImport), "DeepSeekCommented", originalFile.Name, fileType.Id);
                }
                else
                {
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Initial import results indicate success for file {FileName}, FileType {FileTypeId}. Skipping DeepSeek logic.",
                        nameof(TryDeepSeekImport), "InitialImportSuccess", originalFile.Name, fileType.Id);
                }

                stopwatch.Stop();
                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): {ActionName} result. Total observed duration: {TotalObservedDurationMs}ms. Final AllGood: {FinalAllGood}",
                    nameof(TryDeepSeekImport), "Result", nameof(TryDeepSeekImport), stopwatch.ElapsedMilliseconds, resultAllGood);
                return resultAllGood;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms.",
                    nameof(TryDeepSeekImport), stopwatch.ElapsedMilliseconds);
                throw; // Re-throw if this is a critical failure
            }
        }

        private async Task NotifyUnknownPDF(FileInfo file, List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>> res2)
        {
            // InvocationId is already pushed in the calling method (if called)
            var stopwatch = Stopwatch.StartNew();
            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(NotifyUnknownPDF), $"FileName: {file.Name}");

            try
            {
                // Assuming Utils.Client and EmailDownloader are static or accessible
                // Consider injecting IEmailService if refactoring further
                var errorDetails = res2.FirstOrDefault(x => x.Value.status != ImportStatus.Success);
                var errorMessage = $"Unknown PDF Found: {file.Name}\r\n" +
                                   (errorDetails.Value.status != ImportStatus.Success ? $"Failed Step: {errorDetails.Value.DocumentType}" : "Details unavailable.");

                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Preparing unknown PDF notification email for {FileName}.",
                    nameof(NotifyUnknownPDF), "PrepareEmail", file.Name);

                var sendEmailStopwatch = Stopwatch.StartNew();
                _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}",
                    "EmailDownloader.EmailDownloader.SendEmailAsync", "ASYNC_EXPECTED", file.Name);
                await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, null, $"Unknown PDF Found",
                    EmailDownloader.EmailDownloader.GetContacts("Developer", _log),
                    errorMessage,
                    res2.Select(x => x.Value.FileName).Distinct().ToArray(), _log).ConfigureAwait(false);
                sendEmailStopwatch.Stop();
                _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). For file {FileName}.",
                    "EmailDownloader.EmailDownloader.SendEmailAsync", sendEmailStopwatch.ElapsedMilliseconds, "Async call completed (await).", file.Name);

                stopwatch.Stop();
                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. For file {FileName}.",
                    nameof(NotifyUnknownPDF), stopwatch.ElapsedMilliseconds, file.Name);
            }
            catch (Exception ex)
            {
                 stopwatch.Stop();
                 _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error sending notification email for {FileName}: {ErrorMessage}",
                     nameof(NotifyUnknownPDF), stopwatch.ElapsedMilliseconds, file.Name, ex.Message);
                 // Log error
            }
        }

        // --- New Method for Processing Shipment Folders ---
        public async Task ProcessShipmentFolders(ApplicationSettings appSetting)
        {
            string invocationId = Guid.NewGuid().ToString();
            using (LogContext.PushProperty("InvocationId", invocationId))
            using (LogContext.PushProperty("AppSettingId", appSetting?.ApplicationSettingsId))
            {
                var stopwatch = Stopwatch.StartNew();
                _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ProcessShipmentFolders), $"AppSettingId: {appSetting?.ApplicationSettingsId}");

                try
                {
                    var inputFolder = new DirectoryInfo(Path.Combine(appSetting.DataFolder, "ShipmentInput")); // Define input folder
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Checking for shipment folders in: {InputFolderPath}",
                        nameof(ProcessShipmentFolders), "CheckInputFolder", inputFolder.FullName);

                    if (!inputFolder.Exists)
                    {
                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Input folder does not exist. Creating: {InputFolderPath}",
                            nameof(ProcessShipmentFolders), "CreateInputFolder", inputFolder.FullName);
                        try
                        {
                            inputFolder.Create();
                            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Input folder created successfully: {InputFolderPath}",
                                nameof(ProcessShipmentFolders), "CreateInputFolderSuccess", inputFolder.FullName);
                        }
                        catch (Exception ex)
                        {
                            stopwatch.Stop();
                            _log.Error(ex, "ACTION_END_FAILURE: {ActionName} (Input Folder Creation Failed). Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(ProcessShipmentFolders), stopwatch.ElapsedMilliseconds, ex.Message);
                            return; // Cannot proceed
                        }
                    }

                    var subfolders = inputFolder.GetDirectories().ToList();
                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {FolderCount} subfolders in shipment input folder.",
                        nameof(ProcessShipmentFolders), "ListSubfolders", subfolders.Count);

                    if (!subfolders.Any())
                    {
                         _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                            nameof(ProcessShipmentFolders), "ProcessSubfoldersLoop", "Subfolders", "No subfolders found to process.");
                    }

                    foreach (var subfolder in subfolders)
                    {
                        string subfolderInvocationId = Guid.NewGuid().ToString();
                        using (LogContext.PushProperty("InvocationId", subfolderInvocationId)) // New InvocationId for each subfolder
                        {
                            var subfolderStopwatch = Stopwatch.StartNew();
                            _log.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                                "ProcessShipmentSubfolder", $"SubfolderName: {subfolder.Name}");

                            var infoFilePath = Path.Combine(subfolder.FullName, "Info.txt");
                            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Checking for Info.txt in subfolder: {InfoFilePath}",
                                "ProcessShipmentSubfolder", "CheckInfoFile", infoFilePath);

                            if (!File.Exists(infoFilePath))
                            {
                                _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Skipping folder '{SubfolderName}': Info.txt not found.",
                                    "ProcessShipmentSubfolder", "SkipNoInfoFile", subfolder.Name);
                                subfolderStopwatch.Stop();
                                _log.Information("ACTION_END_SUCCESS: {ActionName} (Skipped: No Info.txt). Total observed duration: {TotalObservedDurationMs}ms.",
                                    "ProcessShipmentSubfolder", subfolderStopwatch.ElapsedMilliseconds);
                                continue;
                            }
                            _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Info.txt found in subfolder: {InfoFilePath}",
                                "ProcessShipmentSubfolder", "InfoFileFound", infoFilePath);

                            string placeholderEmailId = null; // Use string for EmailId
                            List<FileInfo> allFilesInFolder = null;

                            try
                            {
                                // --- Get FileType ---
                                FileTypes shipmentFolderType = null;
                                using (var coreCtx = new CoreEntitiesContext()) // Use Core context for FileTypes
                                {
                                     _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to find FileType 'ShipmentFolder' (ID 1186) in database.",
                                         "ProcessShipmentSubfolder", "FindFileType");
                                     // Assuming ID 1186 is stable, otherwise query by description
                                     shipmentFolderType = coreCtx.FileTypes.Find(1186);
                                }

                                if (shipmentFolderType == null)
                                {
                                    _log.Error("ACTION_END_FAILURE: {ActionName} (FileType Not Found). Error: FileType 'ShipmentFolder' (ID 1186) not found in database. Duration: {TotalObservedDurationMs}ms.",
                                        "ProcessShipmentSubfolder", subfolderStopwatch.ElapsedMilliseconds);
                                    continue; // Skip folder if FileType is missing
                                }
                                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): FileType 'ShipmentFolder' (ID 1186) found.",
                                    "ProcessShipmentSubfolder", "FileTypeFound");

                                // --- Extract BL Number ---
                                string blNumber = null;
                                try
                                {
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to extract BL Number from Info.txt: {InfoFilePath}",
                                        "ProcessShipmentSubfolder", "ExtractBLAttempt", infoFilePath);
                                    var lines = File.ReadAllLines(infoFilePath);
                                    foreach (var line in lines)
                                    {
                                         if (string.IsNullOrWhiteSpace(line)) continue;
                                         var parts = line.Split(new[] { ':' }, 2);
                                         if (parts.Length == 2 && parts[0].Trim().Equals("BL", StringComparison.OrdinalIgnoreCase))
                                         {
                                             blNumber = parts[1].Trim();
                                             break;
                                         }
                                    }
                                    if (string.IsNullOrWhiteSpace(blNumber))
                                    {
                                        throw new ApplicationException("BL Number key not found or value is empty in Info.txt");
                                    }
                                     _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Extracted BL Number: {BLNumber}",
                                         "ProcessShipmentSubfolder", "ExtractBLSuccess", blNumber);
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex, "ACTION_END_FAILURE: {ActionName} (BL Extraction Failed). Duration: {TotalObservedDurationMs}ms. Error reading BL number from Info.txt in folder '{SubfolderName}': {ErrorMessage}",
                                        "ProcessShipmentSubfolder", subfolderStopwatch.ElapsedMilliseconds, subfolder.Name, ex.Message);
                                    continue; // Skip folder on error
                                }


                                // --- Create Placeholder Email & Attachments ---
                                allFilesInFolder = subfolder.GetFiles().ToList(); // Get all files for context and attachments
                                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {FileCount} files in subfolder for attachment creation.",
                                    "ProcessShipmentSubfolder", "ListFilesInSubfolder", allFilesInFolder.Count);

                                using (var coreCtx = new CoreEntitiesContext())
                                {
                                    // Removed check for existing email

                                    // Generate a unique string ID for the email
                                    placeholderEmailId = Guid.NewGuid().ToString();
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Generated placeholder Email ID: {PlaceholderEmailId}",
                                        "ProcessShipmentSubfolder", "GenerateEmailId", placeholderEmailId);

                                    var email = new Emails(true) // Assuming constructor enables tracking
                                    {
                                        TrackingState = TrackingState.Added,
                                        EmailId = placeholderEmailId, // Assign generated string ID
                                        Subject = $"Shipment Folder: {blNumber}", // Set Subject
                                        EmailDate = DateTime.Now, // Corrected: Use EmailDate property
                                        ApplicationSettingsId = appSetting.ApplicationSettingsId
                                    };
                                    coreCtx.Emails.Add(email);
                                    coreCtx.SaveChanges(); // Save Email first
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Created placeholder Email record with ID: {PlaceholderEmailId}",
                                        "ProcessShipmentSubfolder", "CreateEmailRecord", placeholderEmailId);

                                    // Create Attachment records
                                    if (!allFilesInFolder.Any())
                                    {
                                         _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                                            "ProcessShipmentSubfolder", "CreateAttachmentRecords", "Files in subfolder", "No files found to create attachment records for.");
                                    }
                                    foreach (var file in allFilesInFolder)
                                    {
                                        // Use correct Attachments entity and properties
                                        var attachment = new CoreEntities.Business.Entities.Attachments(true)
                                        {
                                            TrackingState = TrackingState.Added,
                                            EmailId = placeholderEmailId, // Link to the new Email record
                                            FilePath = file.FullName
                                            // DocumentCode and Reference are optional
                                        };
                                        coreCtx.Attachments.Add(attachment);
                                        _log.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Created attachment record for file: {FilePath}",
                                            "ProcessShipmentSubfolder", "CreateAttachmentRecord", file.FullName);
                                    }
                                    coreCtx.SaveChanges(); // Save Attachments
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Created {AttachmentCount} Attachment records for EmailId: {PlaceholderEmailId}",
                                        "ProcessShipmentSubfolder", "SaveAttachmentRecords", allFilesInFolder.Count, placeholderEmailId);
                                }

                                // --- Prepare Context and Execute Actions ---
                                shipmentFolderType.EmailId = placeholderEmailId; // Pass context (string)
                                shipmentFolderType.Data = new List<KeyValuePair<string, string>>(); // Initialize Data list
                                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Prepared FileType context for action execution. FileTypeId: {FileTypeId}, EmailId: {EmailId}",
                                    "ProcessShipmentSubfolder", "PrepareFileTypeContext", shipmentFolderType.Id, placeholderEmailId);


                                _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for FileType {FileTypeId}, EmailId {EmailId}",
                                    "ImportUtils.ExecuteDataSpecificFileActions", "ASYNC_EXPECTED", shipmentFolderType.Id, placeholderEmailId);
                                var dataSpecificStopwatch = Stopwatch.StartNew();
                                await ImportUtils.ExecuteDataSpecificFileActions(shipmentFolderType, allFilesInFolder.ToArray(), appSetting).ConfigureAwait(false);
                                dataSpecificStopwatch.Stop();
                                _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). For FileType {FileTypeId}, EmailId {EmailId}.",
                                    "ImportUtils.ExecuteDataSpecificFileActions", dataSpecificStopwatch.ElapsedMilliseconds, "Async call completed (await).", shipmentFolderType.Id, placeholderEmailId);


                                _log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for FileType {FileTypeId}, EmailId {EmailId}",
                                    "ImportUtils.ExecuteNonSpecificFileActions", "ASYNC_EXPECTED", shipmentFolderType.Id, placeholderEmailId);
                                var nonSpecificStopwatch = Stopwatch.StartNew();
                                await ImportUtils.ExecuteNonSpecificFileActions(shipmentFolderType, allFilesInFolder.ToArray(), appSetting).ConfigureAwait(false);
                                nonSpecificStopwatch.Stop();
                                _log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). For FileType {FileTypeId}, EmailId {EmailId}.",
                                    "ImportUtils.ExecuteNonSpecificFileActions", nonSpecificStopwatch.ElapsedMilliseconds, "Async call completed (await).", shipmentFolderType.Id, placeholderEmailId);

                                _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Finished actions for folder: {SubfolderName}",
                                    "ProcessShipmentSubfolder", "ActionsFinished", subfolder.Name);

                                // --- Post-Processing (Example: Move to Archive) ---
                                try
                                {
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to move processed folder '{SubfolderName}' to archive.",
                                        "ProcessShipmentSubfolder", "MoveToArchiveAttempt", subfolder.Name);
                                    var archiveFolder = new DirectoryInfo(Path.Combine(inputFolder.FullName, "Archive", subfolder.Name));
                                    if (!archiveFolder.Parent.Exists)
                                    {
                                        _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Archive parent folder does not exist. Creating: {ArchiveParentPath}",
                                            "ProcessShipmentSubfolder", "CreateArchiveParent", archiveFolder.Parent.FullName);
                                        archiveFolder.Parent.Create();
                                    }
                                    if (archiveFolder.Exists)
                                    {
                                         _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Archive folder '{ArchiveFolderPath}' already exists. Handling potential name collision.",
                                             "ProcessShipmentSubfolder", "ArchiveFolderExists", archiveFolder.FullName);
                                         // Handle potential name collision in archive
                                         archiveFolder = new DirectoryInfo($"{archiveFolder.FullName}_{DateTime.Now:yyyyMMddHHmmss}");
                                         _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Renaming archive destination to: {NewArchiveFolderPath}",
                                             "ProcessShipmentSubfolder", "RenameArchiveDestination", archiveFolder.FullName);
                                    }
                                    subfolder.MoveTo(archiveFolder.FullName);
                                    _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Moved processed folder '{SubfolderName}' to archive: {ArchiveFolderPath}",
                                        "ProcessShipmentSubfolder", "MoveToArchiveSuccess", subfolder.Name, archiveFolder.FullName);
                                }
                                catch (Exception mvEx)
                                {
                                     _log.Error(mvEx, "INTERNAL_STEP ({OperationName} - {Stage}): Error moving processed folder '{SubfolderName}' to archive: {ErrorMessage}",
                                         "ProcessShipmentSubfolder", "MoveToArchiveError", subfolder.Name, mvEx.Message);
                                     // Log error, maybe leave folder in place?
                                }

                                subfolderStopwatch.Stop();
                                _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                                    "ProcessShipmentSubfolder", subfolderStopwatch.ElapsedMilliseconds);

                            }
                            catch (Exception ex)
                            {
                                subfolderStopwatch.Stop();
                                _log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Critical error processing folder '{SubfolderName}': {ErrorMessage}",
                                    "ProcessShipmentSubfolder", subfolderStopwatch.ElapsedMilliseconds, subfolder.Name, ex.Message);
                                // Log critical error, potentially move to an error folder
                                try
                                {
                                     _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting to move error folder '{SubfolderName}' to error directory.",
                                         "ProcessShipmentSubfolder", "MoveToErrorAttempt", subfolder.Name);
                                     var errorFolder = new DirectoryInfo(Path.Combine(inputFolder.FullName, "Error", subfolder.Name));
                                     if (!errorFolder.Parent.Exists)
                                     {
                                         _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Error parent folder does not exist. Creating: {ErrorParentPath}",
                                             "ProcessShipmentSubfolder", "CreateErrorParent", errorFolder.Parent.FullName);
                                         errorFolder.Parent.Create();
                                     }
                                      if (errorFolder.Exists)
                                      {
                                          _log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Error folder '{ErrorFolderPath}' already exists. Handling potential name collision.",
                                              "ProcessShipmentSubfolder", "ErrorFolderExists", errorFolder.FullName);
                                          errorFolder = new DirectoryInfo($"{errorFolder.FullName}_{DateTime.Now:yyyyMMddHHmmss}");
                                          _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Renaming error destination to: {NewErrorFolderPath}",
                                              "ProcessShipmentSubfolder", "RenameErrorDestination", errorFolder.FullName);
                                      }
                                     subfolder.MoveTo(errorFolder.FullName);
                                     _log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Moved error folder '{SubfolderName}' to error directory: {ErrorFolderPath}",
                                         "ProcessShipmentSubfolder", "MoveToErrorSuccess", subfolder.Name, errorFolder.FullName);
                                }
                                catch(Exception mvErrEx)
                                {
                                     _log.Error(mvErrEx, "INTERNAL_STEP ({OperationName} - {Stage}): Failed to move error folder '{SubfolderName}': {ErrorMessage}",
                                         "ProcessShipmentSubfolder", "MoveToErrorError", subfolder.Name, mvErrEx.Message);
                                }
                                continue; // Continue to next folder
                            }
                        } // End subfolderInvocationId LogContext
                    } // End foreach subfolder

                    stopwatch.Stop();
                    _log.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ProcessShipmentFolders), stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _log.Fatal(ex, "GLOBAL_ERROR: Unhandled {ExceptionType} in {ActionName}. Error: {ErrorMessage}. Duration: {TotalObservedDurationMs}ms.",
                        ex.GetType().Name, nameof(ProcessShipmentFolders), ex.Message, stopwatch.ElapsedMilliseconds);
                    throw; // Re-throw if this is a critical failure for the overall process
                }
            }
        }

    } // End of FolderProcessor class
} // End of namespace AutoBot