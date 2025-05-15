using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace AutoBot
{
    public class EmailSalesErrorsUtils
    {
        private static readonly ILogger _log = Log.ForContext<EmailSalesErrorsUtils>(); // Add static logger

        public static async Task EmailSalesErrors(ILogger log) // Add ILogger parameter
        {
            string methodName = nameof(EmailSalesErrors);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ }}",
                methodName, "AllocateSalesUtils.AllocateSales"); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var infoTuple = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                var directory = infoTuple.DirPath; // Or infoTuple.Item4 if you prefer direct item access
                var errorfile = Path.Combine(directory, "SalesErrors.csv");

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if error file exists: {ErrorFilePath}", methodName, "CheckFileExists", errorfile); // Add step log
                if (!File.Exists(errorfile))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Error file does not exist. Exiting.", methodName, "ConditionalExit"); // Add step log
                    stopwatch.Stop(); // Stop stopwatch
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                    return;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetSalesReportData", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var getReportDataStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var errors = GetSalesReportData(log, infoTuple); // Pass log
                getReportDataStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). ErrorCount: {ErrorCount}",
                    "GetSalesReportData", getReportDataStopwatch.ElapsedMilliseconds, "Sync call returned.", errors?.Count ?? 0); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CreateSalesReport", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var createReportStopwatch = System.Diagnostics.Stopwatch.StartNew();
                CreateSalesReport(log, errors, errorfile); // Pass log
                createReportStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "CreateSalesReport", createReportStopwatch.ElapsedMilliseconds, "Sync call returned."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetContactsList", "SYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var getContactsStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var contactsLst = GetContactsList(log); // Pass log
                getContactsStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}). ContactCount: {ContactCount}",
                    "GetContactsList", getContactsStopwatch.ElapsedMilliseconds, "Sync call returned.", contactsLst?.Count ?? 0); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "SendSalesReport", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var sendReportStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await SendSalesReport(log, errorfile, directory, infoTuple, contactsLst).ConfigureAwait(false); // Pass log
                sendReportStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "SendSalesReport", sendReportStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static void CreateSalesReport(ILogger log, List<AsycudaSalesAndAdjustmentAllocationsEx> errors, string errorfile) // Add ILogger parameter
        {
            string methodName = nameof(CreateSalesReport);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ ErrorCount: {ErrorCount}, ErrorFilePath: {ErrorFilePath} }}",
                methodName, "EmailSalesErrors", errors?.Count ?? 0, errorfile); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                var res =
                    new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx,
                        List<AsycudaSalesAndAdjustmentAllocationsEx>>
                    {
                        dataToPrint = errors
                    };

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Using StaTaskScheduler for SaveReport.", methodName, "SchedulerSetup"); // Add step log
                using (var sta = new StaTaskScheduler(1))
                {
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Task.Factory.StartNew(res.SaveReport)", "SYNC_EXPECTED"); // Task.Factory.StartNew can be sync or async depending on scheduler
                    var saveReportTask = Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                        TaskCreationOptions.None, sta);
                    // Note: This is a fire-and-forget task within a void method.
                    // We can't easily await it here. Log the start, but no completion log for the task itself.
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "Task.Factory.StartNew(res.SaveReport)", "Task started on STA scheduler."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static List<AsycudaSalesAndAdjustmentAllocationsEx> GetSalesReportData(ILogger log,
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) infoTuple) // Add ILogger parameter
        {
            string methodName = nameof(GetSalesReportData);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ StartDate: {StartDate}, EndDate: {EndDate} }}",
                methodName, "EmailSalesErrors", infoTuple.StartDate, infoTuple.EndDate); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Querying database for sales report data.", methodName, "QueryDB"); // Add step log
                var result = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Status != null)
                    .Where(x => x.InvoiceDate >= infoTuple.StartDate.Date && x.InvoiceDate <= infoTuple.EndDate.Date).ToList();

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. ResultCount: {ResultCount}",
                    methodName, stopwatch.ElapsedMilliseconds, result?.Count ?? 0); // Add METHOD_EXIT_SUCCESS log
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

        private static async Task SendSalesReport(ILogger log, string errorfile, string directory,
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) infoTuple, List<Contacts> contactsLst) // Add ILogger parameter
        {
            string methodName = nameof(SendSalesReport);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ ErrorFilePath: {ErrorFilePath}, Directory: {Directory}, ContactCount: {ContactCount} }}",
                methodName, "EmailSalesErrors", errorfile, directory, contactsLst?.Count ?? 0); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EmailDownloader.EmailDownloader.SendEmailAsync", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var sendEmailStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory,
                    $"Sales Errors for {infoTuple.StartDate.ToString("yyyy-MM-dd")} - {infoTuple.EndDate.ToString("yyyy-MM-dd")}",
                    contactsLst.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new[]
                    {
                        errorfile
                    }).ConfigureAwait(false); // Need to check if this method accepts ILogger
                sendEmailStopwatch.Stop();
                log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EmailDownloader.EmailDownloader.SendEmailAsync", sendEmailStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        private static List<Contacts> GetContactsList(ILogger log) // Add ILogger parameter
        {
            string methodName = nameof(GetContactsList);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ }}",
                methodName, "EmailSalesErrors"); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                List<Contacts> contacts;
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Querying database for contacts.", methodName, "QueryDB"); // Add step log
                using (var ctx = new CoreEntitiesContext())
                {
                    contacts = ctx.Contacts
                        .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .ToList();
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. ResultCount: {ResultCount}",
                    methodName, stopwatch.ElapsedMilliseconds, contacts?.Count ?? 0); // Add METHOD_EXIT_SUCCESS log
                return contacts;
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