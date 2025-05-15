using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using Newtonsoft.Json.Linq;
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace AutoBot
{
    using Serilog;

    public partial class EX9Utils
    {


        public static async Task RecreateEx9(ILogger log, int months) // Add ILogger
        {
            string methodName = nameof(RecreateEx9);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ Months: {Months} }}",
                methodName, "EntryDocSetUtils.EmailEntriesExpiringNextMonth", months);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CreateEX9Utils.CreateEx9", "ASYNC_EXPECTED");
                var genDocs = await CreateEX9Utils.CreateEx9(log, true, months).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) GeneratedDocumentCount: {GeneratedDocumentCount}",
                    "CreateEX9Utils.CreateEx9", stopwatch.ElapsedMilliseconds, "Async call completed (await).", genDocs?.Count() ?? 0);


                if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Generated documents found. Starting re-export/re-warehouse process.", methodName, "ReExportProcess");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ExportEx9Entries", "ASYNC_EXPECTED");
                    await ExportEx9Entries(log, months).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ExportEx9Entries", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "AssessEx9Entries", "ASYNC_EXPECTED");
                    await AssessEx9Entries(log, months).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "AssessEx9Entries", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "DownloadSalesFiles", "ASYNC_EXPECTED");
                    DownloadSalesFiles(log, 10, "IM7", false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                        "DownloadSalesFiles", "Sync call returned (Task not awaited here)."); // This method returns Task but is not awaited

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "DocumentUtils.ImportSalesEntries", "ASYNC_EXPECTED");
                    await DocumentUtils.ImportSalesEntries(log, true).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "DocumentUtils.ImportSalesEntries", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportWarehouseErrorsUtils.ImportWarehouseErrors", "ASYNC_EXPECTED");
                    await ImportWarehouseErrorsUtils.ImportWarehouseErrors(log, months).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ImportWarehouseErrorsUtils.ImportWarehouseErrors", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "RecreateEx9 (recursive call)", "ASYNC_EXPECTED");
                    await RecreateEx9(log, months).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "RecreateEx9 (recursive call)", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Re-export process complete. Exiting application.", methodName, "ExitApplication");
                    Application.Exit(); // Consider if this should be logged or handled differently
                }
                else // reimport and submit to customs
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No generated documents found. Starting re-import/submit process.", methodName, "ReimportSubmitProcess");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "PDFUtils.LinkPDFs", "SYNC_EXPECTED");
                    PDFUtils.LinkPDFs(); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                        "PDFUtils.LinkPDFs", "Sync call returned.");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms", "ASYNC_EXPECTED");
                    await SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(log, months).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.CleanupEntries", "ASYNC_EXPECTED");
                    await EntryDocSetUtils.CleanupEntries(log).ConfigureAwait(false); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "EntryDocSetUtils.CleanupEntries", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Re-import/submit process complete. Exiting application.", methodName, "ExitApplication");
                    Application.Exit(); // Consider if this should be logged or handled differently
                }

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }




        public static async Task ExportEx9Entries(ILogger log, int months) // Add ILogger
        {
            string methodName = nameof(ExportEx9Entries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ Months: {Months} }}",
                methodName, "EX9Utils.RecreateEx9", months);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Exporting EX9 entries.", methodName, "StartExport");
                // Console.WriteLine("Export EX9 Entries"); // Remove Console.WriteLine

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED");
                var saleInfo =  await BaseDataModel.CurrentSalesInfo(months).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) SaleInfoFound: {SaleInfoFound}",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await).", saleInfo != null);

                if (saleInfo == null)
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Could not get current sales info for months {Months}. Cannot export.", methodName, "NoSalesInfo", months);
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ExportDocSetSalesReportUtils.ExportDocSetSalesReport", "ASYNC_EXPECTED");
                await ExportDocSetSalesReportUtils.ExportDocSetSalesReport(log, saleInfo.DocSet.AsycudaDocumentSetId, // Pass log
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number)).ConfigureAwait(false); // Need to check if GetDocSetDirectoryName accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ExportDocSetSalesReportUtils.ExportDocSetSalesReport", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.ExportDocSet", "ASYNC_EXPECTED");
                await BaseDataModel.Instance.ExportDocSet(log, saleInfo.DocSet.AsycudaDocumentSetId, // Pass log
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number), true).ConfigureAwait(false); // Need to check if GetDocSetDirectoryName accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "BaseDataModel.Instance.ExportDocSet", stopwatch.ElapsedMilliseconds, "Async call completed (await).");


                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }

        }



        public static async Task AssessEx9Entries(ILogger log, int months) // Add ILogger
        {
            string methodName = nameof(AssessEx9Entries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ Months: {Months} }}",
                methodName, "EX9Utils.RecreateEx9", months);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED");
                var currentSalesInfo = await BaseDataModel.CurrentSalesInfo(months).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) SalesInfoFound: {SalesInfoFound}",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await).", currentSalesInfo != null);

                if (currentSalesInfo == null)
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Could not get current sales info for months {Months}. Cannot assess entries.", methodName, "NoSalesInfo", months);
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "AssessSalesEntry", "ASYNC_EXPECTED");
                await AssessSalesEntry(log, currentSalesInfo.Item3 // Pass log
                    .Declarant_Reference_Number).ConfigureAwait(false);
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "AssessSalesEntry", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }



        public static async Task AssessSalesEntry(ILogger log, string docReference) // Add ILogger
        {
            string methodName = nameof(AssessSalesEntry);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocReference: {DocReference} }}",
                methodName, "EX9Utils.AssessEx9Entries", docReference);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.AssessComplete", "ASYNC_EXPECTED");
                var assessComplete = await Utils.AssessComplete(log, GetInstructionFile(docReference), // Pass log, Need to check if GetInstructionFile accepts ILogger
                    GetInstructionResultsFile(docReference)).ConfigureAwait(false); // Need to check if GetInstructionResultsFile accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) AssessCompleteSuccess: {AssessCompleteSuccess}",
                    "Utils.AssessComplete", stopwatch.ElapsedMilliseconds, "Async call completed (await).", assessComplete.success);

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking assessment completion status.", methodName, "CheckCompletion");
                // This while loop with Utils.RunSiKuLi is a pattern that might block.
                // Consider adding a timeout or more robust check if Sikuli can fail silently.
                while (docReference != null && assessComplete.success == false)
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Assessment not complete for DocReference {DocReference}. Running Sikuli to assess.", methodName, "RunSikuliAssess", docReference);
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for DocReference {DocReference}", "Utils.RunSiKuLi (AssessIM7)", "SYNC_EXPECTED", docReference);
                    Utils.RunSiKuLi(BaseDataModel.GetDocSetDirectoryName(docReference), "AssessIM7", // Need to check if BaseDataModel.GetDocSetDirectoryName accepts ILogger
                        assessComplete.lcontValue.ToString()); //RunSiKuLi(directoryName, "SaveIM7", lcont.ToString()); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for DocReference {DocReference}",
                        "Utils.RunSiKuLi (AssessIM7)", "Sync call returned.", docReference);

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.AssessComplete (re-check)", "ASYNC_EXPECTED");
                    assessComplete = await Utils.AssessComplete(log, GetInstructionFile(docReference), // Pass log
                        GetInstructionResultsFile(docReference)).ConfigureAwait(false); // Need to check if GetInstructionFile/GetInstructionResultsFile accept ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) AssessCompleteSuccess: {AssessCompleteSuccess}",
                        "Utils.AssessComplete (re-check)", stopwatch.ElapsedMilliseconds, "Async call completed (await).", assessComplete.success);

                    // Add a small delay to avoid tight loop
                    if (!assessComplete.success)
                    {
                         await Task.Delay(2000).ConfigureAwait(false);
                    }
                }
                if (docReference == null)
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): DocReference became null during assessment loop. Exiting loop.", methodName, "DocReferenceNull");
                }
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Assessment complete for DocReference {DocReference}.", methodName, "AssessmentComplete", docReference);


                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

       

        private static string GetInstructionResultsFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "InstructionResults.txt");

        private static string GetInstructionFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "Instructions.txt");



        public static async Task DownloadSalesFiles(ILogger log, int trytimes, string script, bool redownload = false) // Add ILogger
        {
            string methodName = nameof(DownloadSalesFiles);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ TryTimes: {TryTimes}, Script: {Script}, Redownload: {Redownload} }}",
                methodName, "EX9Utils.RecreateEx9", trytimes, script, redownload);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Downloading sales files.", methodName, "StartDownload");
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.GetDocSetDirectoryName", "SYNC_EXPECTED");
                var directoryName = BaseDataModel.GetDocSetDirectoryName("Imports"); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DirectoryName: {DirectoryName}",
                    "BaseDataModel.GetDocSetDirectoryName", stopwatch.ElapsedMilliseconds, "Sync call returned.", directoryName);

                // Console.WriteLine("Download Entries"); // Remove Console.WriteLine
                var lcont = 0; // This variable is declared but not used. Consider removing.
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): lcont variable initialized but not used.", methodName, "UnusedVariable");


                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.RetryImport", "ASYNC_EXPECTED");
                await Task.Run(() => Utils.RetryImport(log, trytimes, script, redownload, directoryName)).ConfigureAwait(false); // Pass log, Need to check if Utils.RetryImport accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "Utils.RetryImport", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }


        private static int GetMonths(int nowMonth, int sDateMonth)
        {
            int currentMonth = nowMonth; // June
            int targetMonth = sDateMonth; // July
            int monthsBetween = 0;

            while (currentMonth != targetMonth)
            {
                currentMonth--;
                if (currentMonth == 0)
                {
                    currentMonth = 12;
                }
                monthsBetween++;
            }
            return monthsBetween;
        }


        public class SaleReportLine
        {
            public int Line { get; set; }
            public DateTime Date { get; set; }
            public string InvoiceNo { get; set; }
            public string CustomerName { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double SalesQuantity { get; set; }

            public double SalesFactor { get; set; }
            public double xQuantity { get; set; }
            public double Price { get; set; }
            public string SalesType { get; set; }
            public double GrossSales { get; set; }
            public string PreviousCNumber { get; set; }
            public string PreviousLineNumber { get; set; }
            public string PreviousRegDate { get; set; }
            public double CIFValue { get; set; }
            public double DutyLiablity { get; set; }
            public string Comment { get; set; }
        }



        public static async Task RecreateEx9(ILogger log, FileTypes filetype, FileInfo[] files) // Add ILogger
        {
            string methodName = nameof(RecreateEx9);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", filetype?.Id, files?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CreateEX9Utils.CreateEx9", "ASYNC_EXPECTED");
                var genDocs = await CreateEX9Utils.CreateEx9(log, true, -1).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) GeneratedDocumentCount: {GeneratedDocumentCount}",
                    "CreateEX9Utils.CreateEx9", stopwatch.ElapsedMilliseconds, "Async call completed (await).", genDocs?.Count() ?? 0);

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED");
                var saleInfo = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) SaleInfoFound: {SaleInfoFound}",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await).", saleInfo != null);

                if (saleInfo != null)
                {
                    filetype.AsycudaDocumentSetId = saleInfo.DocSet.AsycudaDocumentSetId;
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Set FileType DocSetId to {DocSetId}.", methodName, "SetDocSetId", filetype.AsycudaDocumentSetId);
                }
                else
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Could not get current sales info. Cannot set FileType DocSetId.", methodName, "NoSalesInfo");
                }


                if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Generated documents found. Adding re-export/re-warehouse process steps to FileType.", methodName, "AddReExportSteps");
                    filetype.ProcessNextStep.AddRange(new List<string>() { "ExportEx9Entries", "AssessEx9Entries", "DownloadPOFiles", "ImportSalesEntries", "ImportWarehouseErrors", "RecreateEx9" });
                }
                else // reimport and submit to customs
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No generated documents found. Adding re-import/submit process steps to FileType.", methodName, "AddReimportSubmitSteps");
                    filetype.ProcessNextStep.AddRange(new List<string>() { "LinkPDFs", "SubmitToCustoms", "CleanupEntries", "Kill" });
                }

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }



        public static async Task ImportXSalesFiles(ILogger log, string testFile) // Add ILogger
        {
            string methodName = nameof(ImportXSalesFiles);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ TestFile: {TestFile} }}",
                methodName, "N/A", testFile);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Importing XSales files.", methodName, "StartImport");
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetxSalesFileType", "ASYNC_EXPECTED");
                var fileTypes = await GetxSalesFileType(log, testFile).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) FileTypeCount: {FileTypeCount}",
                    "GetxSalesFileType", stopwatch.ElapsedMilliseconds, "Async call completed (await).", fileTypes?.Count ?? 0);

                if (fileTypes == null || !fileTypes.Any())
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No XSales file types found for file '{TestFile}'. Cannot import.", methodName, "NoFileTypes", testFile);
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Iterating through file types for import.", methodName, "IterateFileTypes");
                foreach (var fileType in fileTypes)
                {
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Importing file '{TestFile}' with FileType {FileTypeId}.", methodName, "ImportSingleFile", testFile, fileType.Id);
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file '{TestFile}' and FileType {FileTypeId}", "FileTypeImporter.Import", "ASYNC_EXPECTED", testFile, fileType.Id);
                    await new FileTypeImporter(fileType).Import(testFile).ConfigureAwait(false); // Need to check if FileTypeImporter.Import accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for file '{TestFile}' and FileType {FileTypeId}",
                        "FileTypeImporter.Import", stopwatch.ElapsedMilliseconds, "Async call completed (await).", testFile, fileType.Id);
                }

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }



        public static Task<List<FileTypes>> GetxSalesFileType(ILogger log, string fileName) // Add ILogger
        {
            string methodName = nameof(GetxSalesFileType);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileName: {FileName} }}",
                methodName, "EX9Utils.ImportXSalesFiles", fileName);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.GetFileType", "ASYNC_EXPECTED");
                var task = Utils.GetFileType(log, FileTypeManager.EntryTypes.xSales, FileTypeManager.FileFormats.Csv, fileName); // Pass log
                // Since this returns a Task directly, we don't await it here.
                // The caller is responsible for awaiting this task.
                // We can log the start of the operation.
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                    "Utils.GetFileType", "Async call initiated (task returned).");

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
                return task;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
    }

}