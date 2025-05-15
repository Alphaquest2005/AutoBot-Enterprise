using System;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.DataSpace;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace AutoBot
{
    public class AllocateSalesUtils
    {
        private static readonly ILogger _log = Log.ForContext<AllocateSalesUtils>(); // Add static logger

        public static async Task AllocateSales(ILogger log)
        {
            string methodName = nameof(AllocateSales);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ }}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for unallocated sales.", methodName, "CheckUnallocatedSales");
                bool hasUnallocated = HasUnallocatedSales(log); // Pass log to HasUnallocatedSales

                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true && !hasUnallocated)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): AssessEX is true and no unallocated sales found. Exiting.", methodName, "ConditionalExit");
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                //AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait(); // This uses .Wait(), needs refactoring and instrumentation

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "new AllocateSales().Execute", "SYNC_EXPECTED"); // .Wait() makes it sync
                var executeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false).Wait(); // This needs refactoring to await
                executeStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "new AllocateSales().Execute", executeStopwatch.ElapsedMilliseconds, "Sync call returned.");


                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EmailSalesErrorsUtils.EmailSalesErrors", "ASYNC_EXPECTED");
                var emailErrorsStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await EmailSalesErrorsUtils.EmailSalesErrors(log).ConfigureAwait(false); // Pass log to EmailSalesErrorsUtils.EmailSalesErrors
                emailErrorsStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EmailSalesErrorsUtils.EmailSalesErrors", emailErrorsStopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        private static bool HasUnallocatedSales(ILogger log) // Already accepts ILogger, keep as is
        {
            string methodName = nameof(HasUnallocatedSales);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ }}",
                methodName, "AllocateSales");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for any unallocated sales entries in DB.", methodName, "QueryDB");
                var result = new CoreEntitiesContext().TODO_UnallocatedSales.Any(x =>
                    x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {HasUnallocated}",
                    methodName, stopwatch.ElapsedMilliseconds, result);
                return result;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }
    }
}