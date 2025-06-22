using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoBotUtilities;
using AutoBotUtilities.CSV;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

// Serilog usings
using Serilog;
using Serilog.Context;
using ExcelDataReader.Log;

namespace AutoBot
{
    public class FileUtils
    {

        public static Dictionary<string, Func<ILogger, FileTypes, FileInfo[], Task>> FileActions =>
            new Dictionary<string, Func<ILogger, FileTypes, FileInfo[], Task>>(WaterNut.DataSpace.Utils.ignoreCase)
            {
                // Modify lambdas to accept ILogger and pass it down where applicable
                {"ImportSalesEntries",(log, ft, fs) => DocumentUtils.ImportSalesEntries(log, false) }, // Signature needs update in DocumentUtils
                {"AllocateSales",(log, ft, fs) => new AllocateSalesUtils(log).AllocateSales() }, // Create instance with logger
                {"CreateEx9",(log, ft, fs) => CreateEX9Utils.CreateEx9(log,false, -1) }, // Signature needs update in CreateEX9Utils
                {"ExportEx9Entries",(log, ft, fs) => EX9Utils.ExportEx9Entries(log,-1) }, // Signature needs update in EX9Utils
                {"AssessEx9Entries",(log, ft, fs) => EX9Utils.AssessEx9Entries(log, -1) }, // Signature needs update in EX9Utils
                {"SaveCsv", (log, ft, fs) => CSVUtils.SaveCsv(fs, ft, log) }, // Signature needs update in CSVUtils
                {"ReplaceCSV",(log, ft, fs) => CSVUtils.ReplaceCSV(fs, ft, log) }, // Signature needs update in CSVUtils
                {"RecreatePOEntries",(log, ft, fs) => POUtils.RecreatePOEntries(ft.AsycudaDocumentSetId, log) }, // Signature needs update in POUtils
                {"ExportPOEntries",(log, ft, fs) => POUtils.ExportPOEntries(ft.AsycudaDocumentSetId, log) }, // Signature needs update in POUtils
                {"AssessPOEntry", (log, ft, fs) => POUtils.AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId, log)}, // Signature needs update in POUtils
                {"EmailPOEntries", (log, ft, fs) => POUtils.EmailPOEntries(ft.AsycudaDocumentSetId, log) }, // Signature needs update in POUtils
                {"DownloadSalesFiles",(log, ft, fs) => EX9Utils.DownloadSalesFiles(log, 10, "IM7History",false) }, // Signature needs update in EX9Utils
                {"Xlsx2csv",  (log, ft, fs) => XLSXProcessor.Xlsx2csv(fs, new List < FileTypes >() { ft }, log) }, // Signature needs update in XLSXProcessor
                {"SaveInfo", (log, ft, fs) => new EmailTextProcessor(log).Execute(fs, ft) }, // Create instance with logger
                {"CleanupEntries",(log, ft, fs) => EntryDocSetUtils.CleanupEntries() }, // Signature needs update in EntryDocSetUtils
                {"SubmitToCustoms",(log, ft, fs) => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1, log) }, // Signature needs update in SubmitSalesXmlToCustomsUtils
                {"MapUnClassifiedItems", (log, ft, fs) => ShipmentUtils.MapUnClassifiedItems(ft,fs, log) }, // Signature needs update in ShipmentUtils
                {"UpdateSupplierInfo", (log, ft, fs) => ShipmentUtils.UpdateSupplierInfo(ft,fs) }, // Signature needs update in ShipmentUtils
                
                /// commented out "ImportPDF" because i didnt update the database with the new "ImportPDFWithDeepSeekFallback" action
                /// so disabled the orignal code and rewired ImportPDF to use the new action with DeepSeek fallback
                //{"ImportPDF", (log, ft, fs) => InvoiceReader.InvoiceReader.ImportPDF(fs, ft, log) },//PDFUtils.ImportPDF(fs, ft).GetAwaiter().GetResult() }, // Signature needs update in InvoiceReader.InvoiceReader
                //{"ImportPDFWithDeepSeekFallback", (log, ft, fs) => ImportPDFWithDeepSeekFallbackAsync(log, ft, fs) }, // New action with DeepSeek fallback

                {"ImportPDF", (log, ft, fs) => ImportPDFWithDeepSeekFallbackAsync(log, ft, fs) }, // New action with DeepSeek fallback

                {"CreateShipmentEmail", (log, types, infos) => ShipmentUtils.CreateShipmentEmail(types, infos, log) }, // Signature needs update in ShipmentUtils
                //{"SaveAttachments",(log, ft, fs) => SaveAttachments(fs, ft) }, // Signature needs update if uncommented

                //{"AttachToDocSetByRef", (log, ft, fs) => AttachToDocSetByRef(ft.AsycudaDocumentSetId) }, // Signature needs update if uncommented


                {"SyncConsigneeInDB", (log, types, infos) => EntryDocSetUtils.SyncConsigneeInDB(types, infos, log) }, // Signature needs update in EntryDocSetUtils

                {"ClearDocSetEntries",(log, ft, fs) => EntryDocSetUtils.ClearDocSetEntries(log ,ft) }, // Signature needs update in EntryDocSetUtils

                {"SubmitDocSetUnclassifiedItems",(log, ft, fs) => ShipmentUtils.SubmitDocSetUnclassifiedItems(ft, log) }, // Signature needs update in ShipmentUtils
                {"AllocateDocSetDiscrepancies",(log, ft, fs) => DISUtils.AllocateDocSetDiscrepancies(ft, log) }, // Signature needs update in DISUtils
                {"CleanupDocSetDiscpancies",(log, ft, fs) => DISUtils.CleanupDocSetDiscpancies(ft, log) }, // Signature needs update in DISUtils
                {"RecreateDocSetDiscrepanciesEntries", (log, ft, fs) => DISUtils.RecreateDocSetDiscrepanciesEntries(ft, log ) }, // Signature needs update in DISUtils
                {"ExportDocSetDiscpancyEntries", (log, ft, fs) => DISUtils.ExportDocSetDiscpancyEntries("DIS",ft) }, // Signature needs update in DISUtils
                {"SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms", (log, ft, fs) => DISUtils.SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(ft, log) }, // Signature needs update in DISUtils
                {"AssessDiscrepancyExecutions", (log, ft, fs) => DISUtils.AssessDiscrepancyExecutions(ft, fs, log) }, // Signature needs update in DISUtils
                {"AttachEmailPDF", (log, ft, fs) => PDFUtils.AttachEmailPDF(ft, fs) }, // Signature needs update in PDFUtils
                {"ReSubmitDiscrepanciesToCustoms", (log, types, infos) => DISUtils.ReSubmitDiscrepanciesToCustoms(types, infos, log)
                }, // Signature needs update in DISUtils
                {"ReSubmitSalesToCustoms", (log, types, infos) => SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms(types, infos, log)
                }, // Signature needs update in SubmitSalesToCustomsUtils


                {"SubmitMissingInvoices",  (log, ft, fs) => Utils.SubmitMissingInvoices(ft, log) }, // Signature needs update in Utils
                {"SubmitIncompleteEntryData",(log, ft, fs) => Utils.SubmitIncompleteEntryData(ft, log) }, // Signature needs update in Utils
                {"SubmitUnclassifiedItems",(log, ft, fs) => ShipmentUtils.SubmitUnclassifiedItems(ft, log) }, // Signature needs update in ShipmentUtils
                {"SubmitInadequatePackages",(log, ft, fs) => ShipmentUtils.SubmitInadequatePackages(ft, log) }, // Signature needs update in ShipmentUtils
                {"SubmitIncompleteSuppliers",(log, ft, fs) => ShipmentUtils.SubmitIncompleteSuppliers(ft, log) }, // Signature needs update in ShipmentUtils
                {"CreateC71",(log, ft, fs) => C71Utils.CreateC71(ft) }, // Signature needs update in C71Utils
                {"CreateLicense", (log, ft, fs) =>  LICUtils.CreateLicence(ft)}, // Signature needs update in LICUtils
                { "AssessC71",(log, ft, fs) => C71Utils.AssessC71(ft) }, // Signature needs update in C71Utils
                {"AssessLicense",(log, ft, fs) => LICUtils.AssessLicense(ft) }, // Signature needs update in LICUtils
                {"DownLoadC71", (log, ft, fs) => C71Utils.DownLoadC71(ft) }, // Signature needs update in C71Utils
                {"DownLoadLicense", (log, ft, fs) => LICUtils.DownLoadLicence(false, ft) }, // Signature needs update in LICUtils
                { "ImportC71", (log, ft, fs) => C71Utils.ImportC71(ft, log) }, // Signature needs update in C72Utils
                {"ImportLicense", (log, ft, fs) => LICUtils.ImportLicense(ft) }, // Signature needs update in LICUtils

                { "AttachToDocSetByRef",(log, ft, fs) => EntryDocSetUtils.AttachToDocSetByRef(log, ft) }, // Signature needs update in EntryDocSetUtils

                {"AssessPOEntries", (log, ft, fs) => POUtils.AssessPOEntries(ft, log) }, // Signature needs update in POUtils
                {"AssessDiscpancyEntries", (log, ft, fs) => DISUtils.AssessDiscpancyEntries(ft, fs, log) }, // Signature needs update in DISUtils
                {"DeletePONumber", (log, ft, fs) => POUtils.DeletePONumber(ft, fs, log) }, // Signature needs update in POUtils
                { "SubmitPOs", (log, ft, fs) => POUtils.SubmitPOs(log) }, // Signature needs update in POUtils
                {"SubmitEntryCIF", (log, types, infos) => EntryDocSetUtils.SubmitEntryCIF(log,types, infos) }, // Signature needs update in EntryDocSetUtils
                {"SubmitBlankLicenses", (log, ft, fs) => LICUtils.SubmitBlankLicenses(ft, log) }, // Signature needs update in LICUtils
                {"ProcessUnknownCSVFileType", (log, ft,fs) => CSVUtils.ProcessUnknownCSVFileType(ft, fs) }, // Signature needs update in CSVUtils
               // {"ProcessUnknownPDFFileType", (log, ft,fs) => PDFUtils.ProcessUnknownPDFFileType(ft, fs) }, // Signature needs update if uncommented
                {"ImportUnAttachedSummary", (log, ft, fs) => ShipmentUtils.ImportUnAttachedSummary(ft, fs, log) }, // Signature needs update in ShipmentUtils

                {"RemoveDuplicateEntries", (log, ft,fs) => EntryDocSetUtils.RemoveDuplicateEntries(log) }, // Signature needs update in EntryDocSetUtils
                {"FixIncompleteEntries", (log, ft, fs) => EntryDocSetUtils.FixIncompleteEntries(log) }, // Signature needs update in EntryDocSetUtils
                {"EmailWarehouseErrors", (log, ft, fs) => EntryDocSetUtils.EmailWarehouseErrors(log, ft, fs) }, // Signature needs update in EntryDocSetUtils
                {"ImportExpiredEntires", (log, ft, fs) => EntryDocSetUtils.ImportExpiredEntires(log) }, // Signature needs update in EntryDocSetUtils
                {"ImportCancelledEntries", (log, ft, fs) => EntryDocSetUtils.ImportCancelledEntries(log) }, // Signature needs update in EntryDocSetUtils
                {"EmailEntriesExpiringNextMonth", (log, ft, fs) => EntryDocSetUtils.EmailEntriesExpiringNextMonth(log) }, // Signature needs update in EntryDocSetUtils
                {"RecreateEx9", (log, types, infos) => EX9Utils.RecreateEx9(log, types, infos) },// // Signature needs update in EX9Utils
                {"UpdateRegEx", (log, ft, fs) => UpdateInvoice.UpdateRegEx(ft,fs, log) }, // Signature needs update in UpdateInvoice
                {"ImportWarehouseErrors", (log, ft, fs) => ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1, log)}, // Signature needs update in ImportWarehouseErrorsUtils
                {"Kill", (log, ft, fs) => Utils.Kill(ft, fs) }, // Signature needs update in Utils
                {"Continue", (log, ft, fs) => { return Task.Run(() => { });}}, // No change needed in lambda body
                {"LinkPDFs", (log, ft,fs) => PDFUtils.LinkPDFs()}, // Signature needs update in PDFUtils
                {"DownloadPOFiles", (log, ft,fs) => EX9Utils.DownloadSalesFiles(log, 10, "IM7", false)}, // Signature needs update in EX9Utils
                {"ReDownloadPOFiles", (log, ft,fs) => EX9Utils.DownloadSalesFiles(log, 10, "IM7", true)}, // Signature needs update in EX9Utils
                {"SubmitDiscrepanciesToCustoms", (log, types, infos) => DISUtils.SubmitDiscrepanciesToCustoms(types, infos, log) }, // Signature needs update in DISUtils
                {"ClearShipmentData", (log, ft, fs) => ShipmentUtils.ClearShipmentData(ft, fs) }, // Signature needs update in ShipmentUtils
                {"ImportPOEntries", (log, ft, fs) => new DocumentUtils(log).ImportPOEntries(false) }, // Using instance method
                {"ImportAllAsycudaDocumentsInDataFolder", (log, ft,fs) => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false, log) }, // Signature needs update in ImportAllAsycudaDocumentsInDataFolderUtils
                {"ImportEntries",(log, ft, fs) => DocumentUtils.ImportEntries(false, ft.Data.ToString(), log) }, // Signature needs update in DocumentUtils
                {"ImportShipmentInfoFromTxt", (log, types, infos) => ShipmentUtils.ImportShipmentInfoFromTxt(types, infos, log) }, // Added mapping for new action, Signature needs update in ShipmentUtils
                {"CorrectImportIssues", (log, ft, fs) => ShipmentUtils.CorrectImportIssues(ft, fs, log) }, // New action for correcting import issues

            };

        /// <summary>
        /// Imports PDF files with DeepSeek fallback functionality.
        /// First tries standard ImportPDF, then falls back to DeepSeek if imports fail.
        /// </summary>
        /// <param name="log">Logger instance</param>
        /// <param name="fileType">File type configuration</param>
        /// <param name="files">Array of files to import</param>
        /// <returns>Task representing the async operation</returns>
        private static async Task ImportPDFWithDeepSeekFallbackAsync(ILogger log, FileTypes fileType, FileInfo[] files)
        {
            string operationName = nameof(ImportPDFWithDeepSeekFallbackAsync);
            string operationInvocationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("OperationInvocationId", operationInvocationId))
            using (LogContext.PushProperty("FileTypeId", fileType?.Id))
            {
                var stopwatch = Stopwatch.StartNew();
                log.Information("ACTION_START: {ActionName}. FileTypeId: {FileTypeId}, FileCount: {FileCount}",
                    operationName, fileType?.Id, files?.Length ?? 0);

                try
                {
                    // Filter to only PDF files
                    var pdfFiles = files?.Where(f => f.Extension.ToLower() == ".pdf").ToArray() ?? new FileInfo[0];

                    if (!pdfFiles.Any())
                    {
                        log.Information("INTERNAL_STEP ({OperationName} - {Stage}): No PDF files found to import.",
                            operationName, "FileFiltering");
                        stopwatch.Stop();
                        log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms. Reason: No PDF files to process",
                            operationName, stopwatch.ElapsedMilliseconds);
                        return;
                    }

                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {PdfFileCount} PDF files to import.",
                        operationName, "FileFiltering", pdfFiles.Length);

                    // --- Step 1: Try Standard Import ---
                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Attempting standard PDF import first.",
                        operationName, "StandardImportAttempt");

                    var standardImportStopwatch = Stopwatch.StartNew();
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        "PDFUtils.ImportPDF (Standard)", "ASYNC_EXPECTED");

                    var standardImportResults = await PDFUtils.ImportPDF(pdfFiles, fileType, log).ConfigureAwait(false);

                    standardImportStopwatch.Stop();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "PDFUtils.ImportPDF (Standard)", standardImportStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    // --- Step 2: Check Results ---
                    bool allImportsSuccessful = standardImportResults.All(x => x.Value.Status == ImportStatus.Success);
                    int successCount = standardImportResults.Count(x => x.Value.Status == ImportStatus.Success);
                    int failedCount = standardImportResults.Count(x => x.Value.Status == ImportStatus.Failed);
                    int hasErrorsCount = standardImportResults.Count(x => x.Value.Status == ImportStatus.HasErrors);

                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Standard import results - Total: {TotalCount}, Success: {SuccessCount}, Failed: {FailedCount}, HasErrors: {HasErrorsCount}. AllSuccessful: {AllSuccessful}",
                        operationName, "StandardImportResultCheck", standardImportResults.Count, successCount, failedCount, hasErrorsCount, allImportsSuccessful);

                    if (allImportsSuccessful)
                    {
                        log.Information("INTERNAL_STEP ({OperationName} - {Stage}): All imports successful with standard method. No DeepSeek fallback needed.",
                            operationName, "StandardImportSuccess");
                        stopwatch.Stop();
                        log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms. Outcome: Standard import successful",
                            operationName, stopwatch.ElapsedMilliseconds);
                        return;
                    }

                    // --- Step 3: DeepSeek Fallback ---
                    log.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Standard import had failures. Attempting DeepSeek fallback for failed files.",
                        operationName, "DeepSeekFallbackAttempt");

                    // Get the files that failed in standard import
                    var failedFiles = standardImportResults
                        .Where(x => x.Value.Status == ImportStatus.Failed)
                        .Select(x => x.Key)
                        .ToList();

                    if (!failedFiles.Any())
                    {
                        log.Information("INTERNAL_STEP ({OperationName} - {Stage}): No failed files found for DeepSeek retry (only HasErrors status). Skipping DeepSeek fallback.",
                            operationName, "DeepSeekFallbackSkip");
                        stopwatch.Stop();
                        log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms. Outcome: Standard import completed with some errors, no failures to retry",
                            operationName, stopwatch.ElapsedMilliseconds);
                        return;
                    }

                    // Convert failed file names back to FileInfo objects
                    var failedFileInfos = pdfFiles.Where(f => failedFiles.Contains(f.FullName)).ToArray();

                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Retrying {FailedFileCount} failed files with DeepSeek API.",
                        operationName, "DeepSeekFallbackExecution", failedFileInfos.Length);

                    var deepSeekImportStopwatch = Stopwatch.StartNew();
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        "PDFUtils.ImportPDFDeepSeek (Fallback)", "ASYNC_EXPECTED");

                    var deepSeekResults = await PDFUtils.ImportPDFDeepSeek(failedFileInfos, fileType, log).ConfigureAwait(false);

                    deepSeekImportStopwatch.Stop();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "PDFUtils.ImportPDFDeepSeek (Fallback)", deepSeekImportStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    // --- Step 4: Log Final Results ---
                    int deepSeekSuccessCount = deepSeekResults.Count(x => x.Value.status == ImportStatus.Success);
                    int deepSeekFailedCount = deepSeekResults.Count(x => x.Value.status == ImportStatus.Failed);
                    int deepSeekHasErrorsCount = deepSeekResults.Count(x => x.Value.status == ImportStatus.HasErrors);

                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): DeepSeek fallback results - Total: {TotalCount}, Success: {SuccessCount}, Failed: {FailedCount}, HasErrors: {HasErrorsCount}",
                        operationName, "DeepSeekFallbackResultCheck", deepSeekResults.Count, deepSeekSuccessCount, deepSeekFailedCount, deepSeekHasErrorsCount);

                    // Calculate overall final results
                    int totalFinalSuccess = successCount + deepSeekSuccessCount;
                    int totalFinalFailed = (failedCount - failedFileInfos.Length) + deepSeekFailedCount; // Subtract retried files from original failed count
                    int totalFinalHasErrors = hasErrorsCount + deepSeekHasErrorsCount;

                    log.Information("INTERNAL_STEP ({OperationName} - {Stage}): Final combined results - Success: {FinalSuccessCount}, Failed: {FinalFailedCount}, HasErrors: {FinalHasErrorsCount}",
                        operationName, "FinalResultSummary", totalFinalSuccess, totalFinalFailed, totalFinalHasErrors);

                    stopwatch.Stop();
                    log.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms. Outcome: Import completed with DeepSeek fallback",
                        operationName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    log.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        operationName, stopwatch.ElapsedMilliseconds, ex.Message);
                    throw; // Re-throw to allow higher-level error handling
                }
            }
        }
            // Removed extra closing brace here
    }
}