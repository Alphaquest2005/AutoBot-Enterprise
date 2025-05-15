using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace AutoBot
{
    using Serilog;

    public class CreateEX9Utils
    {


        public static async System.Threading.Tasks.Task<List<DocumentCT>> CreateEx9(ILogger log, bool overwrite, int months) // Add ILogger
        {
            string methodName = nameof(CreateEx9);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ Overwrite: {Overwrite}, Months: {Months} }}",
                methodName, "EX9Utils.RecreateEx9", overwrite, months);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "SQLBlackBox.RunSqlBlackBox", "SYNC_EXPECTED");
                SQLBlackBox.RunSqlBlackBox(); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                    "SQLBlackBox.RunSqlBlackBox", "Sync call returned.");

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Creating Ex9 entries.", methodName, "StartCreate");
                // Console.WriteLine("Create Ex9"); // Remove Console.WriteLine

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED");
                var saleInfoTuple = await BaseDataModel.CurrentSalesInfo(months, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) SaleInfoFound: {SaleInfoFound}",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await).", saleInfoTuple.DocSet != null);


                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking if document set exists or has data.", methodName, "CheckDocSetData");
                if (saleInfoTuple.DocSet.AsycudaDocumentSetId == 0 || !HasData(log, saleInfoTuple)) // Need to check if HasData accepts ILogger
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Document set ID is 0 or no data found. Returning empty list.", methodName, "NoDocSetOrData");
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return new List<DocumentCT>();
                }
               
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Document set found and has data.", methodName, "DocSetDataFound");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for DocSetId {DocSetId}", "GetDocset", "ASYNC_EXPECTED", saleInfoTuple.DocSet.AsycudaDocumentSetId);
                var docSet = await GetDocset(saleInfoTuple.DocSet.AsycudaDocumentSetId).ConfigureAwait(false); // Need to check if GetDocset accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) DocSetFound: {DocSetFound}",
                    "GetDocset", stopwatch.ElapsedMilliseconds, "Async call completed (await).", docSet != null);

                if (docSet == null)
                {
                     log.Error("INTERNAL_STEP ({MethodName} - {Stage}): Failed to retrieve document set with ID {DocSetId}.", methodName, "GetDocsetFailed", saleInfoTuple.DocSet.AsycudaDocumentSetId);
                     throw new ApplicationException($"Failed to retrieve document set with ID {saleInfoTuple.DocSet.AsycudaDocumentSetId}.");
                }


                if (overwrite)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Overwrite is true. Clearing document set with ID {DocSetId}.", methodName, "ClearDocSet", docSet.AsycudaDocumentSetId);
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for DocSetId {DocSetId}", "BaseDataModel.Instance.ClearAsycudaDocumentSet", "ASYNC_EXPECTED", docSet.AsycudaDocumentSetId);
                    await BaseDataModel.Instance.ClearAsycudaDocumentSet(docSet.AsycudaDocumentSetId, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for DocSetId {DocSetId}",
                        "BaseDataModel.Instance.ClearAsycudaDocumentSet", stopwatch.ElapsedMilliseconds, "Async call completed (await).", docSet.AsycudaDocumentSetId);
                }


                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CreateFilterExpression", "SYNC_EXPECTED");
                var filterExpression = CreateFilterExpression(log, saleInfoTuple); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) FilterExpression: {FilterExpression}",
                    "CreateFilterExpression", stopwatch.ElapsedMilliseconds, "Sync call returned.", filterExpression);


                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "AllocationsModel.Instance.CreateEx9.Execute", "ASYNC_EXPECTED");
                var result = await AllocationsModel.Instance.CreateEx9.Execute(filterExpression, false, false, true, docSet, "Sales", "Historic", BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9.GetValueOrDefault(), true, true, true, true, false, true, true, true, true).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) ResultCount: {ResultCount}",
                    "AllocationsModel.Instance.CreateEx9.Execute", stopwatch.ElapsedMilliseconds, "Async call completed (await).", result?.Count ?? 0);
                
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
                return result;

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

        private static async System.Threading.Tasks.Task<AsycudaDocumentSet> GetDocset(int asycudaDocumentSetId) => await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).ConfigureAwait(false);



        private static string CreateFilterExpression(ILogger log, // Add ILogger
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) saleInfoTuple)
        {
            string methodName = nameof(CreateFilterExpression);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ StartDate: {StartDate}, EndDate: {EndDate}, DocSetId: {DocSetId}, DirPath: {DirPath} }}",
                methodName, "CreateEX9Utils.CreateEx9", saleInfoTuple.StartDate, saleInfoTuple.EndDate, saleInfoTuple.DocSet?.AsycudaDocumentSetId, saleInfoTuple.DirPath);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Creating filter expression.", methodName, "StartCreate");
                var filterExpression = $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" + // Need to check if CurrentApplicationSettings accepts ILogger
                                       $"&& (InvoiceDate >= \"{saleInfoTuple.StartDate:MM/01/yyyy}\" " +
                                       $" && InvoiceDate <= \"{saleInfoTuple.EndDate:MM/dd/yyyy HH:mm:ss}\")" +
                                       //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfoTuple.StartDate:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfoTuple.EndDate:MM/dd/yyyy HH:mm:ss}\"))" +
                                       "&& ( TaxAmount == 0 ||  TaxAmount != 0)" +
                                       //"&& PreviousItem_Id != null" +
                                       //"&& (xBond_Item_Id == 0 )" +
                                       //"&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                                       //"&& (PiQuantity < pQtyAllocated)" +
                                       //"&& (Status == null || Status == \"\")" +
                                       //(BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                                       //    ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                                       //    : "") +
                                       ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Filter expression created.", methodName, "EndCreate");

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
                return filterExpression;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }



        private static bool HasData(ILogger log, // Add ILogger
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) saleInfoTuple)
        {
            string methodName = nameof(HasData);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ StartDate: {StartDate}, EndDate: {EndDate}, DocSetId: {DocSetId}, DirPath: {DirPath} }}",
                methodName, "CreateEX9Utils.CreateEx9", saleInfoTuple.StartDate, saleInfoTuple.EndDate, saleInfoTuple.DocSet?.AsycudaDocumentSetId, saleInfoTuple.DirPath);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Checking if data exists for sales info tuple.", methodName, "StartCheck");
                using (var ctx = new CoreEntitiesContext()) // Need to check if CoreEntitiesContext accepts ILogger
                {
                    ctx.Database.CommandTimeout = 0; // Consider logging this setting

                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetSqlStr", "SYNC_EXPECTED");
                    var sqlQuery = GetSqlStr(log, saleInfoTuple); // Pass log
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                        "GetSqlStr", stopwatch.ElapsedMilliseconds, "Sync call returned.");

                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.Database.SqlQuery<string>(...).Any()", "SYNC_EXPECTED");
                    var result = ctx.Database.SqlQuery<string>(sqlQuery).Any(); // Consider if this should be awaited or is truly synchronous
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) HasDataResult: {HasDataResult}",
                        "ctx.Database.SqlQuery<string>(...).Any()", stopwatch.ElapsedMilliseconds, "Sync call returned.", result);

                    stopwatch.Stop();
                    log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return result;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }



        private static string GetSqlStr(ILogger log, // Add ILogger
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) saleInfoTuple)
        {
            string methodName = nameof(GetSqlStr);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ StartDate: {StartDate}, EndDate: {EndDate}, DocSetId: {DocSetId}, DirPath: {DirPath} }}",
                methodName, "CreateEX9Utils.HasData", saleInfoTuple.StartDate, saleInfoTuple.EndDate, saleInfoTuple.DocSet?.AsycudaDocumentSetId, saleInfoTuple.DirPath);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Generating SQL query string.", methodName, "StartGenerate");
                var sqlQuery = $@"SELECT EX9AsycudaSalesAllocations.ItemNumber
                    FROM    EX9AsycudaSalesAllocations INNER JOIN
                                     ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND
                                     EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                                     AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
                    WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND
                                     (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                                     EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND
                                     (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) AND (EX9AsycudaSalesAllocations.CustomsOperationId = {(int)CustomsOperations.Warehouse}) AND (AllocationErrors.ItemNumber IS NULL)
                          AND (ApplicationSettings.ApplicationSettingsId = {
                              BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId // Need to check if CurrentApplicationSettings accepts ILogger
                          }) AND (EX9AsycudaSalesAllocations.InvoiceDate >= '{
                              saleInfoTuple.StartDate.ToShortDateString()
                          }') AND
                                     (EX9AsycudaSalesAllocations.InvoiceDate <= '{saleInfoTuple.EndDate.ToShortDateString()}')
                    GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId--, EX9AsycudaSalesAllocations.pQuantity, EX9AsycudaSalesAllocations.PreviousItem_Id
                    HAVING (SUM(EX9AsycudaSalesAllocations.PiQuantity) < SUM(EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL)";

                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): SQL query string generated.", methodName, "EndGenerate");

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
                return sqlQuery;
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