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
        private readonly ILogger _logger;
        
        public AllocateSalesUtils(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AllocateSales()
        {
            string methodName = nameof(AllocateSales);
            _logger.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ }}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for unallocated sales.", methodName, "CheckUnallocatedSales");
                bool hasUnallocated = HasUnallocatedSales(); // Use instance method

                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true && !hasUnallocated)
                {
                    _logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): AssessEX is true and no unallocated sales found. Exiting.", methodName, "ConditionalExit");
                    stopwatch.Stop();
                    _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                //AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait(); // This uses .Wait(), needs refactoring and instrumentation

                _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "new AllocateSales().Execute", "SYNC_EXPECTED"); // .Wait() makes it sync
                var executeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false).ConfigureAwait(false); // This needs refactoring to await
                executeStopwatch.Stop();
                _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "new AllocateSales().Execute", executeStopwatch.ElapsedMilliseconds, "Sync call returned.");


                _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EmailSalesErrorsUtils.EmailSalesErrors", "ASYNC_EXPECTED");
                var emailErrorsStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await new EmailSalesErrorsUtils(_logger).EmailSalesErrors().ConfigureAwait(false); // Create instance with logger
                emailErrorsStopwatch.Stop();
                _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EmailSalesErrorsUtils.EmailSalesErrors", emailErrorsStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                stopwatch.Stop();
                _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                _logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        private bool HasUnallocatedSales() // Convert to instance method
        {
            string methodName = nameof(HasUnallocatedSales);
            _logger.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ }}",
                methodName, "AllocateSales");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for any unallocated sales entries in DB.", methodName, "QueryDB");
                var result = new CoreEntitiesContext().TODO_UnallocatedSales.Any(x =>
                    x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);

                stopwatch.Stop();
                _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Result: {HasUnallocated}",
                    methodName, stopwatch.ElapsedMilliseconds, result);
                return result;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                _logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }
    }
}