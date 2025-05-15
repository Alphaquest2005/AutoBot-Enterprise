using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
// using TrackableEntities; // Removed duplicate using
using CoreAsycudaDocumentSet = CoreEntities.Business.Entities.AsycudaDocumentSet; // Alias for CoreEntities version
using CoreConsignees = CoreEntities.Business.Entities.Consignees; // Alias for CoreEntities version


namespace AutoBot
{
    using System.Diagnostics;
    using Serilog;

    public class EntryDocSetUtils
    {
        public static async Task EmailEntriesExpiringNextMonth(ILogger log) // Add ILogger
        {
            string methodName = nameof(EmailEntriesExpiringNextMonth);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EX9Utils.EmailEntriesExpiringNextMonth", "ASYNC_EXPECTED");
                await EX9Utils.EmailEntriesExpiringNextMonth(log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EX9Utils.EmailEntriesExpiringNextMonth", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task ImportCancelledEntries(ILogger log, FileTypes arg1, FileInfo[] arg2) // Add ILogger
        {
            string methodName = nameof(ImportCancelledEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", arg1?.Id, arg2?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportCancelledEntries()", "ASYNC_EXPECTED");
                await ImportCancelledEntries(log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ImportCancelledEntries()", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task ImportExpiredEntires(ILogger log, FileTypes arg1, FileInfo[] arg2) // Add ILogger
        {
            string methodName = nameof(ImportExpiredEntires);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", arg1?.Id, arg2?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportExpiredEntires()", "ASYNC_EXPECTED");
                await ImportExpiredEntires(log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ImportExpiredEntires()", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task EmailWarehouseErrors(ILogger log, FileTypes arg1, FileInfo[] arg2) // Add ILogger
        {
            string methodName = nameof(EmailWarehouseErrors);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", arg1?.Id, arg2?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EX9Utils.EmailWarehouseErrors", "ASYNC_EXPECTED");
                await EX9Utils.EmailWarehouseErrors(log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "EX9Utils.EmailWarehouseErrors", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task FixIncompleteEntries(ILogger log, FileTypes arg1, FileInfo[] arg2) // Add ILogger
        {
            string methodName = nameof(FixIncompleteEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", arg1?.Id, arg2?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "FixIncompleteEntries()", "ASYNC_EXPECTED");
                await FixIncompleteEntries(log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "FixIncompleteEntries()", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task RemoveDuplicateEntries(ILogger log, FileTypes ft, FileInfo[] fs) // Add ILogger
        {
            string methodName = nameof(RemoveDuplicateEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", ft?.Id, fs?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "RemoveDuplicateEntries()", "ASYNC_EXPECTED");
                await RemoveDuplicateEntries(log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "RemoveDuplicateEntries()", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task LinkEmail(ILogger log) // Add ILogger
        {
            string methodName = nameof(LinkEmail);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Linking emails.", methodName, "StartLink");
                // Console.WriteLine("Link Emails"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.Database.SqlQuery<TODO_ImportCompleteEntries>", "SYNC_EXPECTED");
                    var entries = ctx.Database.SqlQuery<TODO_ImportCompleteEntries>(
                        $"EXEC [dbo].[Stp_TODO_ImportCompleteEntries] @ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ctx.Database.SqlQuery<TODO_ImportCompleteEntries>", "Sync call returned.");

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Grouping and joining entries.", methodName, "GroupAndJoin");
                    var lst = entries
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs
                        .Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Found {DocSetCount} document sets with complete entries.", methodName, "GroupAndJoin", lst.Count);


                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Processing entries.", methodName, "ProcessEntries");
                    foreach (var itm in entries)
                    {
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Processing entry with AssessedAsycuda_Id: {AssessedAsycudaId}", methodName, "ProcessSingleEntry", itm.AssessedAsycuda_Id);
                        log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.AsycudaDocuments.First (for idoc)", "SYNC_EXPECTED");
                        var idoc = ctx.AsycudaDocuments.First(x => x.ASYCUDA_Id == itm.AssessedAsycuda_Id);
                        log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ctx.AsycudaDocuments.First (for idoc)", "Sync call returned.");

                        log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.AsycudaDocuments.First (for cdoc)", "SYNC_EXPECTED");
                        var cdoc = ctx.AsycudaDocuments.First(x => x.ReferenceNumber == idoc.ReferenceNumber);
                        log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ctx.AsycudaDocuments.First (for cdoc)", "Sync call returned.");


                        if (cdoc == null)
                        {
                            log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Corresponding cdoc not found for idoc ReferenceNumber: {ReferenceNumber}. Skipping.", methodName, "ProcessSingleEntry", idoc.ReferenceNumber);
                            continue;
                        }
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Found corresponding cdoc with ASYCUDA_Id: {CdocAsycudaId}", methodName, "ProcessSingleEntry", cdoc.ASYCUDA_Id);

                        // This Task.Run(() => { }) seems like a placeholder or unnecessary await.
                        // If there's actual async work needed here, it should be implemented.
                        // For now, just logging its presence.
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Encountered placeholder async task.", methodName, "PlaceholderAsync");
                        await Task.Run(() => { }).ConfigureAwait(false);
                    }
                }
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

        public static void LogDocSetAction(ILogger log, int docSetId, string actionName) // Add ILogger
        {
            string methodName = nameof(LogDocSetAction);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocSetId: {DocSetId}, ActionName: {ActionName} }}",
                methodName, "N/A", docSetId, actionName);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.Actions.FirstOrDefault", "SYNC_EXPECTED");
                    var action = ctx.Actions.FirstOrDefault(x => x.Name == actionName);
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ActionFound: {ActionFound}",
                        "ctx.Actions.FirstOrDefault", stopwatch.ElapsedMilliseconds, "Sync call returned.", action != null);

                    if (action == null)
                    {
                        log.Error("INTERNAL_STEP ({MethodName} - {Stage}): Action with name: {ActionName} not found.", methodName, "Validation", actionName);
                        throw new ApplicationException($"Action with name: {actionName} not found");
                    }

                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.ActionDocSetLogs.Add", "SYNC_EXPECTED");
                    ctx.ActionDocSetLogs.Add(new ActionDocSetLogs(true)
                    {
                        ActonId = action.Id,
                        AsycudaDocumentSetId = docSetId,
                        TrackingState = TrackingState.Added
                    });
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ctx.ActionDocSetLogs.Add", "Sync call returned.");

                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.SaveChanges", "SYNC_EXPECTED");
                    ctx.SaveChanges();
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ctx.SaveChanges", "Sync call returned.");
                }
                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
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

        public static async Task ImportExpiredEntires(ILogger log) // Add ILogger
        {
            string methodName = nameof(ImportExpiredEntires);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED");
                var docSet = await BaseDataModel.CurrentSalesInfo(-1, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.GetDocSetDirectoryName", "SYNC_EXPECTED");
                var directoryName = BaseDataModel.GetDocSetDirectoryName(docSet.Item3.Declarant_Reference_Number); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DirectoryName: {DirectoryName}",
                    "BaseDataModel.GetDocSetDirectoryName", stopwatch.ElapsedMilliseconds, "Sync call returned.", directoryName);

                var expFile = Path.Combine(directoryName, "ExpiredEntries.csv");
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for existing ExpiredEntries.csv at '{FilePath}'.", methodName, "CheckExistingFile", expFile);
                if (File.Exists(expFile))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Existing ExpiredEntries.csv found, deleting.", methodName, "DeleteExistingFile");
                    File.Delete(expFile);
                }

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Running Sikuli automation to generate ExpiredEntries.csv.", methodName, "RunSikuli");
                // This while loop with File.Exists check and Utils.RunSiKuLi is a pattern that might block.
                // Consider adding a timeout or more robust check if Sikuli can fail silently.
                while (!File.Exists(expFile))
                {
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Waiting for ExpiredEntries.csv to be created by Sikuli.", methodName, "SikuliWait");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.RunSiKuLi (ExpiredEntries)", "SYNC_EXPECTED");
                    Utils.RunSiKuLi(directoryName, "ExpiredEntries", "0"); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "Utils.RunSiKuLi (ExpiredEntries)", "Sync call returned.");
                    // Add a small delay to avoid tight loop
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): ExpiredEntries.csv created by Sikuli.", methodName, "SikuliComplete");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportExpiredEntires (with file path)", "ASYNC_EXPECTED");
                await ImportExpiredEntires(log, expFile).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ImportExpiredEntires (with file path)", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task ImportExpiredEntires(ILogger log, string expFile) // Add ILogger
        {
            string methodName = nameof(ImportExpiredEntires);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ ExpiredFilePath: {ExpiredFilePath} }}",
                methodName, "EntryDocSetUtils.ImportExpiredEntires", expFile);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting FileType for ExpiredEntries.", methodName, "GetFileType");
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CoreEntitiesContext.FileTypes.Include(...).First", "SYNC_EXPECTED");
                var fileType = new CoreEntitiesContext()
                    .FileTypes
                    .Include(x => x.FileImporterInfos)
                    .First(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                        x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ExpiredEntries);
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) FileTypeFound: {FileTypeFound}",
                    "CoreEntitiesContext.FileTypes.Include(...).First", stopwatch.ElapsedMilliseconds, "Sync call returned.", fileType != null);


                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CSVUtils.SaveCsv", "ASYNC_EXPECTED");
                await CSVUtils.SaveCsv(new FileInfo[] { new FileInfo(expFile) }, fileType, log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "CSVUtils.SaveCsv", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task ImportCancelledEntries(ILogger log) // Add ILogger
        {
            string methodName = nameof(ImportCancelledEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.CurrentSalesInfo", "ASYNC_EXPECTED");
                var docSet = await BaseDataModel.CurrentSalesInfo(-1, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "BaseDataModel.CurrentSalesInfo", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.GetDocSetDirectoryName", "SYNC_EXPECTED");
                var directoryName = BaseDataModel.GetDocSetDirectoryName(docSet.Item3.Declarant_Reference_Number); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DirectoryName: {DirectoryName}",
                    "BaseDataModel.GetDocSetDirectoryName", stopwatch.ElapsedMilliseconds, "Sync call returned.", directoryName);

                var expFile = Path.Combine(directoryName, "CancelledEntries.csv");
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for existing CancelledEntries.csv at '{FilePath}'.", methodName, "CheckExistingFile", expFile);
                if (File.Exists(expFile))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Existing CancelledEntries.csv found, deleting.", methodName, "DeleteExistingFile");
                    File.Delete(expFile);
                }

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Running Sikuli automation to generate CancelledEntries.csv.", methodName, "RunSikuli");
                // Similar to ImportExpiredEntires, this while loop might block.
                while (!File.Exists(expFile))
                {
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Waiting for CancelledEntries.csv to be created by Sikuli.", methodName, "SikuliWait");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.RunSiKuLi (CancelledEntries)", "SYNC_EXPECTED");
                    Utils.RunSiKuLi(directoryName, "CancelledEntries", "0"); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "Utils.RunSiKuLi (CancelledEntries)", "Sync call returned.");
                    // Add a small delay
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): CancelledEntries.csv created by Sikuli.", methodName, "SikuliComplete");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ImportCancelledEntries (with file path)", "ASYNC_EXPECTED");
                await ImportCancelledEntries(log, expFile).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ImportCancelledEntries (with file path)", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static async Task ImportCancelledEntries(ILogger log, string expFile) // Add ILogger
        {
            string methodName = nameof(ImportCancelledEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ CancelledFilePath: {CancelledFilePath} }}",
                methodName, "EntryDocSetUtils.ImportCancelledEntries", expFile);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting FileType for CancelledEntries.", methodName, "GetFileType");
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CoreEntitiesContext.FileTypes.First", "SYNC_EXPECTED");
                var fileType = new CoreEntitiesContext().FileTypes.First(x =>
                    x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                    x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.CancelledEntries);
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) FileTypeFound: {FileTypeFound}",
                    "CoreEntitiesContext.FileTypes.First", stopwatch.ElapsedMilliseconds, "Sync call returned.", fileType != null);

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CSVUtils.SaveCsv", "ASYNC_EXPECTED");
                await CSVUtils.SaveCsv(new FileInfo[] { new FileInfo(expFile) }, fileType, log).ConfigureAwait(false); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "CSVUtils.SaveCsv", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task RenameDuplicateDocumentCodes(ILogger log) // Add ILogger
        {
            string methodName = nameof(RenameDuplicateDocumentCodes);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Renaming duplicate document codes.", methodName, "StartRename");
                // Console.WriteLine("Rename Duplicate Documents"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.AsycudaDocumentSetExs.Where(...).OrderByDescending(...).FirstOrDefaultAsync", "ASYNC_EXPECTED");
                    var docset =
                       await ctx.AsycudaDocumentSetExs.Where(x =>
                               x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                   .ApplicationSettingsId)
                           .OrderByDescending(x => x.AsycudaDocumentSetId)
                           .FirstOrDefaultAsync().ConfigureAwait(false);
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) DocSetFound: {DocSetFound}",
                        "ctx.AsycudaDocumentSetExs.Where(...).OrderByDescending(...).FirstOrDefaultAsync", stopwatch.ElapsedMilliseconds, "Async call completed (await).", docset != null);

                    if (docset != null)
                    {
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.AsycudaDocuments.Where(...).Select(...).ToList", "SYNC_EXPECTED");
                        var doclst = ctx.AsycudaDocuments.Where(x => x.AsycudaDocumentSetId == docset.AsycudaDocumentSetId)
                            .Select(x => x.ASYCUDA_Id).ToList();
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DocumentCount: {DocumentCount}",
                            "ctx.AsycudaDocuments.Where(...).Select(...).ToList", stopwatch.ElapsedMilliseconds, "Sync call returned.", doclst.Count);

                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.RenameDuplicateDocumentCodes", "SYNC_EXPECTED");
                        BaseDataModel.RenameDuplicateDocumentCodes(doclst); // Need to check if this method accepts ILogger
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "BaseDataModel.RenameDuplicateDocumentCodes", "Sync call returned.");
                    }
                    else
                    {
                        log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No document set found to rename duplicate codes.", methodName, "NoDocSet");
                    }
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static async Task RenameDuplicateDocuments(ILogger log) // Add ILogger
        {
            string methodName = nameof(RenameDuplicateDocuments);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Renaming duplicate documents.", methodName, "StartRename");
                // Console.WriteLine("Rename Duplicate Documents"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.AsycudaDocumentSetExs.Where(...).OrderByDescending(...).FirstOrDefault", "SYNC_EXPECTED");
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                            .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DocSetFound: {DocSetFound}",
                        "ctx.AsycudaDocumentSetExs.Where(...).OrderByDescending(...).FirstOrDefault", stopwatch.ElapsedMilliseconds, "Sync call returned.", docset != null);

                    if (docset != null)
                    {
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.RenameDuplicateDocuments", "ASYNC_EXPECTED");
                        await BaseDataModel.RenameDuplicateDocuments(docset.AsycudaDocumentSetId).ConfigureAwait(false); // Need to check if this method accepts ILogger
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            "BaseDataModel.RenameDuplicateDocuments", stopwatch.ElapsedMilliseconds, "Async call completed (await).");
                    }
                    else
                    {
                        log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No document set found to rename duplicate documents.", methodName, "NoDocSet");
                    }
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static Task AttachToDocSetByRef(ILogger log, FileTypes ft) // Add ILogger
        {
            string methodName = nameof(AttachToDocSetByRef);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId} }}",
                methodName, "N/A", ft?.Id);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.AttachToExistingDocuments", "ASYNC_EXPECTED");
                var task = BaseDataModel.Instance.AttachToExistingDocuments(ft.AsycudaDocumentSetId); // Need to check if this method accepts ILogger
                // Since this returns a Task directly, we don't await it here.
                // The caller is responsible for awaiting this task.
                // We can log the start of the operation.
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                    "BaseDataModel.Instance.AttachToExistingDocuments", "Async call initiated (task returned).");

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
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

        public static async Task AttachToDocSetByRef(ILogger log) // Add ILogger
        {
            string methodName = nameof(AttachToDocSetByRef);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Attaching documents to document sets.", methodName, "StartAttach");
                // Console.WriteLine("Attach Documents To DocSet"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.TODO_PODocSet.Where(...).GroupBy(...).Select(...).Distinct().ToList", "SYNC_EXPECTED");
                    var lst = ctx.TODO_PODocSet
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => x.Key)
                        .Distinct()
                        .ToList();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DocSetCount: {DocSetCount}",
                        "ctx.TODO_PODocSet.Where(...).GroupBy(...).Select(...).Distinct().ToList", stopwatch.ElapsedMilliseconds, "Sync call returned.", lst.Count);


                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Iterating through document sets.", methodName, "IterateGroups");
                    foreach (var doc in lst)
                    {
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Attaching documents for DocSetId: {DocSetId}", methodName, "ProcessSingleDocSet", doc);
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for DocSetId {DocSetId}", "BaseDataModel.Instance.AttachToExistingDocuments", "ASYNC_EXPECTED", doc);
                        await BaseDataModel.Instance.AttachToExistingDocuments(doc).ConfigureAwait(false); // Need to check if this method accepts ILogger
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for DocSetId {DocSetId}",
                            "BaseDataModel.Instance.AttachToExistingDocuments", stopwatch.ElapsedMilliseconds, "Async call completed (await).", doc);
                    }
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static void SetFileTypeDocSetToLatest(ILogger log, FileTypes ft) // Add ILogger
        {
            string methodName = nameof(SetFileTypeDocSetToLatest);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, CurrentDocSetId: {CurrentDocSetId} }}",
                methodName, "N/A", ft?.Id, ft?.AsycudaDocumentSetId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (ft.AsycudaDocumentSetId == 0)
                {
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): FileType DocSetId is 0, getting latest from DB.", methodName, "GetLatestDocSet");
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CoreEntitiesContext.AsycudaDocumentSetExs.Where(...).OrderByDescending(...).FirstOrDefault", "SYNC_EXPECTED");
                    var latestDocSet =
                        new CoreEntitiesContext().AsycudaDocumentSetExs.Where(x =>
                                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) LatestDocSetFound: {LatestDocSetFound}",
                        "CoreEntitiesContext.AsycudaDocumentSetExs.Where(...).OrderByDescending(...).FirstOrDefault", stopwatch.ElapsedMilliseconds, "Sync call returned.", latestDocSet != null);

                    if (latestDocSet != null)
                    {
                        ft.AsycudaDocumentSetId = latestDocSet.AsycudaDocumentSetId;
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Set FileType DocSetId to latest: {NewDocSetId}", methodName, "SetDocSetId", ft.AsycudaDocumentSetId);
                    }
                    else
                    {
                        log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): No latest document set found to set FileType DocSetId.", methodName, "NoLatestDocSet");
                    }
                }
                else
                {
                    log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): FileType DocSetId is already set ({CurrentDocSetId}). No change needed.", methodName, "DocSetIdAlreadySet", ft.AsycudaDocumentSetId);
                }

                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
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

        public static async Task SubmitEntryCIF(ILogger log, FileTypes ft, FileInfo[] fs) // Add ILogger
        {
            string methodName = nameof(SubmitEntryCIF);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId}, FileCount: {FileCount} }}",
                methodName, "N/A", ft?.Id, fs?.Length ?? 0);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Submitting Shipment CIF.", methodName, "StartSubmit");
                // Console.WriteLine("Submit Shipment CIF"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.Contacts.Where(...).Select(...).Distinct().ToArray", "SYNC_EXPECTED");
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ContactCount: {ContactCount}",
                        "ctx.Contacts.Where(...).Select(...).Distinct().ToArray", stopwatch.ElapsedMilliseconds, "Sync call returned.", contacts.Length);

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.TODO_SubmitEntryCIF.Where(...).ToList", "SYNC_EXPECTED");
                    var lst = ctx.TODO_SubmitEntryCIF
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) EntryCount: {EntryCount}",
                        "ctx.TODO_SubmitEntryCIF.Where(...).ToList", stopwatch.ElapsedMilliseconds, "Sync call returned.", lst.Count);

                    if (!lst.Any())
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No entries to submit for DocSetId {DocSetId}. Exiting.", methodName, "NoEntries", ft.AsycudaDocumentSetId);
                        stopwatch.Stop();
                        log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                            methodName, stopwatch.ElapsedMilliseconds);
                        return;
                    }

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.GetDocSetActions", "SYNC_EXPECTED");
                    if (Enumerable.Any<ActionDocSetLogs>(Utils.GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitEntryCIF"))) // Need to check if this method accepts ILogger
                    {
                        log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Action 'SubmitEntryCIF' already logged for DocSetId {DocSetId}. Skipping submission.", methodName, "ActionAlreadyLogged", ft.AsycudaDocumentSetId);
                        stopwatch.Stop();
                        log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                            methodName, stopwatch.ElapsedMilliseconds);
                        return;
                    }
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ActionLogged: false",
                        "Utils.GetDocSetActions", stopwatch.ElapsedMilliseconds, "Sync call returned.");


                    var body = $"Please see attached CIF for Shipment: {lst.First().Declarant_Reference_Number} . \r\n" +
                                $"Please double check against your shipment rider.\r\n" +
                                $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                $"\r\n" +
                                $"Regards,\r\n" +
                                $"AutoBot";
                    List<string> attlst = new List<string>();

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "POUtils.CurrentPOInfo", "SYNC_EXPECTED");
                    var poInfo = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(POUtils.CurrentPOInfo(ft.AsycudaDocumentSetId)); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) POInfoFound: {POInfoFound}",
                        "POUtils.CurrentPOInfo", stopwatch.ElapsedMilliseconds, "Sync call returned.", poInfo != null);

                    if (poInfo == null)
                    {
                        log.Error("INTERNAL_STEP ({MethodName} - {Stage}): Could not get PO Info for DocSetId {DocSetId}. Cannot create CIF file.", methodName, "POInfoNotFound", ft.AsycudaDocumentSetId);
                        throw new ApplicationException($"Could not get PO Info for DocSetId {ft.AsycudaDocumentSetId}. Cannot create CIF file.");
                    }

                    var summaryFile = Path.Combine(poInfo.Item2, "CIFValues.csv");
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for existing CIFValues.csv at '{FilePath}'.", methodName, "CheckExistingFile", summaryFile);
                    if (File.Exists(summaryFile))
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Existing CIFValues.csv found, deleting.", methodName, "DeleteExistingFile");
                        File.Delete(summaryFile);
                    }

                    var errRes =
                        new ExportToCSV<TODO_SubmitEntryCIF, List<TODO_SubmitEntryCIF>>()
                        {
                            dataToPrint = lst
                        };
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Exporting CIF data to CSV.", methodName, "ExportToCSV");
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ExportToCSV.SaveReport", "ASYNC_EXPECTED");
                        await Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta).ConfigureAwait(false);
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                            "ExportToCSV.SaveReport", stopwatch.ElapsedMilliseconds, "Async call completed (await).");
                    }

                    attlst.Add(summaryFile);

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Sending CIF email.", methodName, "SendEmail");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EmailDownloader.EmailDownloader.SendEmailAsync", "ASYNC_EXPECTED");
                    await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "", $"CIF Values for Shipment: {lst.First().Declarant_Reference_Number}",
                        contacts, body, attlst.ToArray(), log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "EmailDownloader.EmailDownloader.SendEmailAsync", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "LogDocSetAction", "SYNC_EXPECTED");
                    LogDocSetAction(log, ft.AsycudaDocumentSetId, "SubmitEntryCIF"); // Pass log
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "LogDocSetAction", "Sync call returned.");
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // throw ex; // Re-throw the original exception
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static async Task AssessEntries(ILogger log, string docReference, int asycudaDocumentSetId) // Add ILogger
        {
            string methodName = nameof(AssessEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocReference: {DocReference}, AsycudaDocumentSetId: {AsycudaDocumentSetId} }}",
                methodName, "N/A", docReference, asycudaDocumentSetId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (docReference == null)
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): DocReference is null. Cannot assess entries.", methodName, "ParameterValidation");
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.GetDocSetDirectoryName", "SYNC_EXPECTED");
                var directoryName = BaseDataModel.GetDocSetDirectoryName(docReference); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DirectoryName: {DirectoryName}",
                    "BaseDataModel.GetDocSetDirectoryName", stopwatch.ElapsedMilliseconds, "Sync call returned.", directoryName);

                var resultsFile = Path.Combine(directoryName, "InstructionResults.txt");
                var instrFile = Path.Combine(directoryName, "Instructions.txt");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Utils.RetryAssess", "ASYNC_EXPECTED");
                await Utils.RetryAssess(instrFile, resultsFile, directoryName, 3, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "Utils.RetryAssess", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

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

        public static async Task ClearDocSetEntries(ILogger log, FileTypes fileType) // Add ILogger
        {
            string methodName = nameof(ClearDocSetEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ FileTypeId: {FileTypeId} }}",
                methodName, "N/A", fileType?.Id);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Clearing document set entries for FileType {FileTypeId}.", methodName, "StartClear", fileType?.Id);
                // Console.WriteLine($"Clear {fileType.FileImporterInfos.EntryType} Entries"); // Remove Console.WriteLine

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.GetAsycudaDocumentSet", "ASYNC_EXPECTED");
                var docSet = await BaseDataModel.Instance.GetAsycudaDocumentSet(fileType.AsycudaDocumentSetId).ConfigureAwait(false); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) DocSetFound: {DocSetFound}",
                    "BaseDataModel.Instance.GetAsycudaDocumentSet", stopwatch.ElapsedMilliseconds, "Async call completed (await).", docSet != null);

                if (docSet == null)
                {
                    log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Document set with ID {DocSetId} not found. Cannot clear entries.", methodName, "DocSetNotFound", fileType.AsycudaDocumentSetId);
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        methodName, stopwatch.ElapsedMilliseconds);
                    return;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.GetDocSetDirectoryName", "SYNC_EXPECTED");
                string directoryName = BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number); // Need to check if this method accepts ILogger
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DirectoryName: {DirectoryName}",
                    "BaseDataModel.GetDocSetDirectoryName", stopwatch.ElapsedMilliseconds, "Sync call returned.", directoryName);


                var instFile = Path.Combine(directoryName, "Instructions.txt");
                var resFile = Path.Combine(directoryName, "InstructionResults.txt");

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for InstructionResults.txt at '{FilePath}'.", methodName, "CheckResultsFile", resFile);
                if (File.Exists(resFile))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): InstructionResults.txt found. Processing based on results.", methodName, "ProcessWithResults");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "File.ReadAllText", "SYNC_EXPECTED");
                    var resTxt = File.ReadAllText(resFile);
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ResultsFileRead.", "File.ReadAllText", stopwatch.ElapsedMilliseconds, "Sync call returned.");

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Iterating through documents to delete based on results.", methodName, "DeleteBasedOnResults");
                    foreach (var doc in docSet.Documents.ToList())
                    {
                        if (!resTxt.Contains(doc.ReferenceNumber))
                        {
                            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Deleting document with ReferenceNumber: {ReferenceNumber}", methodName, "DeleteDocument", doc.ReferenceNumber);
                            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for document {ReferenceNumber}", "BaseDataModel.Instance.DeleteAsycudaDocument", "ASYNC_EXPECTED", doc.ReferenceNumber);
                            await BaseDataModel.Instance.DeleteAsycudaDocument(doc).ConfigureAwait(false); // Need to check if this method accepts ILogger
                            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for document {ReferenceNumber}",
                                "BaseDataModel.Instance.DeleteAsycudaDocument", stopwatch.ElapsedMilliseconds, "Async call completed (await).", doc.ReferenceNumber);
                        }
                        else
                        {
                            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Keeping document with ReferenceNumber: {ReferenceNumber} (found in results).", methodName, "KeepDocument", doc.ReferenceNumber);
                        }
                    }
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Updating document set last number.", methodName, "UpdateLastNumber");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber", "SYNC_EXPECTED");
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId,
                        docSet.Documents.Count()); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber", "Sync call returned.");
                }
                else
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): InstructionResults.txt not found. Clearing entire document set.", methodName, "ClearEntireDocSet");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.ClearAsycudaDocumentSet", "ASYNC_EXPECTED");
                    await BaseDataModel.Instance.ClearAsycudaDocumentSet(fileType.AsycudaDocumentSetId, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "BaseDataModel.Instance.ClearAsycudaDocumentSet", stopwatch.ElapsedMilliseconds, "Async call completed (await).");

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Updating document set last number to 0.", methodName, "UpdateLastNumber");
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber", "SYNC_EXPECTED");
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId, 0); // Need to check if this method accepts ILogger
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber", "Sync call returned.");
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ClearDocSetEntryData", "SYNC_EXPECTED");
                ClearDocSetEntryData(log, fileType.AsycudaDocumentSetId); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ClearDocSetEntryData", "Sync call returned.");

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ClearDocSetAttachments", "SYNC_EXPECTED");
                ClearDocSetAttachments(log, fileType.AsycudaDocumentSetId, fileType.EmailId); // Pass log
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "ClearDocSetAttachments", "Sync call returned.");

                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Checking for Instructions.txt at '{FilePath}'.", methodName, "CheckInstructionsFile", instFile);
                if (File.Exists(instFile))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Instructions.txt found, deleting.", methodName, "DeleteInstructionsFile");
                    File.Delete(instFile);
                }

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Console.WriteLine(e); // Remove Console.WriteLine
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static async Task FixIncompleteEntries(ILogger log) // Add ILogger
        {
            string methodName = nameof(FixIncompleteEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Re-importing incomplete entries.", methodName, "StartReimport");
                // Console.WriteLine("ReImport Incomplete Entries"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.TODO_Error_IncompleteItems.Where(...).GroupBy(...)", "SYNC_EXPECTED");
                    var lst = ctx.TODO_Error_IncompleteItems
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => new { x.ASYCUDA_Id, x.SourceFileName, x.AsycudaDocumentSetId });
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) GroupCount: {GroupCount}",
                        "ctx.TODO_Error_IncompleteItems.Where(...).GroupBy(...)", stopwatch.ElapsedMilliseconds, "Sync call returned.", lst.Count());

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Iterating through incomplete entry groups.", methodName, "IterateGroups");
                    foreach (var doc in lst)
                    {
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Processing incomplete entry group for ASYCUDA_Id: {AsycudaId}", methodName, "ProcessGroup", doc.Key.ASYCUDA_Id);
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for ASYCUDA_Id {AsycudaId}", "BaseDataModel.Instance.DeleteAsycudaDocument", "ASYNC_EXPECTED", doc.Key.ASYCUDA_Id);
                        await BaseDataModel.Instance.DeleteAsycudaDocument(doc.Key.ASYCUDA_Id).ConfigureAwait(false); // Need to check if this method accepts ILogger
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for ASYCUDA_Id {AsycudaId}",
                            "BaseDataModel.Instance.DeleteAsycudaDocument", stopwatch.ElapsedMilliseconds, "Async call completed (await).", doc.Key.ASYCUDA_Id);

                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file '{FileName}'", "BaseDataModel.Instance.ImportDocuments", "ASYNC_EXPECTED");
                        await BaseDataModel.Instance.ImportDocuments(doc.Key.AsycudaDocumentSetId.GetValueOrDefault(), new List<string>() { doc.Key.SourceFileName }, true, true, false, true, true, log).ConfigureAwait(false); // Need to check if this method accepts ILogger
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for file '{FileName}'",
                            "BaseDataModel.Instance.ImportDocuments", stopwatch.ElapsedMilliseconds, "Async call completed (await).", doc.Key.SourceFileName);
                    }
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // throw ex; // Re-throw the original exception
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static async Task RemoveDuplicateEntries(ILogger log) // Add ILogger
        {
            string methodName = nameof(RemoveDuplicateEntries);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{}}",
                methodName, "N/A");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Removing duplicate entries.", methodName, "StartRemove");
                // Console.WriteLine("Remove DuplicateEntries"); // Remove Console.WriteLine
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ctx.TODO_Error_DuplicateEntry.Where(...).GroupBy(...)", "SYNC_EXPECTED");
                    var lst = ctx.TODO_Error_DuplicateEntry
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.id);
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) GroupCount: {GroupCount}",
                        "ctx.TODO_Error_DuplicateEntry.Where(...).GroupBy(...)", stopwatch.ElapsedMilliseconds, "Sync call returned.", lst.Count());

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Iterating through duplicate entry groups.", methodName, "IterateGroups");
                    foreach (var dup in lst)
                    {
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Processing duplicate entry group for id: {DuplicateId}", methodName, "ProcessGroup", dup.Key);
                        var doc = dup.Last();
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for ASYCUDA_Id {AsycudaId}", "BaseDataModel.Instance.DeleteAsycudaDocument", "ASYNC_EXPECTED", doc.ASYCUDA_Id);
                        await BaseDataModel.Instance.DeleteAsycudaDocument(doc.ASYCUDA_Id).ConfigureAwait(false); // Need to check if this method accepts ILogger
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance}) for ASYCUDA_Id {AsycudaId}",
                            "BaseDataModel.Instance.DeleteAsycudaDocument", stopwatch.ElapsedMilliseconds, "Async call completed (await).", doc.ASYCUDA_Id);
                    }
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // throw ex; // Re-throw the original exception
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static void ClearDocSetEntryData(ILogger log, int asycudaDocumentSetId) // Add ILogger
        {
            string methodName = nameof(ClearDocSetEntryData);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ AsycudaDocumentSetId: {AsycudaDocumentSetId} }}",
                methodName, "EntryDocSetUtils.ClearDocSetEntries", asycudaDocumentSetId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Clearing document set entry data for DocSetId {DocSetId}.", methodName, "StartClear", asycudaDocumentSetId);
                using (var dtx = new DocumentDSContext())
                {
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "dtx.AsycudaDocumentSetEntryDatas.Where(...)", "SYNC_EXPECTED");
                    var res = dtx.AsycudaDocumentSetEntryDatas.Where(x =>
                        x.AsycudaDocumentSet.SystemDocumentSet == null
                        && x.AsycudaDocumentSetId == asycudaDocumentSetId);
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ResultsFound: {ResultsFound}",
                        "dtx.AsycudaDocumentSetEntryDatas.Where(...)", stopwatch.ElapsedMilliseconds, "Sync call returned.", res.Any());

                    if (res.Any())
                    {
                        log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "dtx.AsycudaDocumentSetEntryDatas.RemoveRange", "SYNC_EXPECTED");
                        dtx.AsycudaDocumentSetEntryDatas.RemoveRange(res);
                        log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "dtx.AsycudaDocumentSetEntryDatas.RemoveRange", "Sync call returned.");

                        log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "dtx.SaveChanges", "SYNC_EXPECTED");
                        dtx.SaveChanges();
                        log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "dtx.SaveChanges", "Sync call returned.");
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Removed {RemovedCount} document set entry data records.", methodName, "RemovedRecords", res.Count());
                    }
                    else
                    {
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): No document set entry data records found to clear for DocSetId {DocSetId}.", methodName, "NoRecordsToClear", asycudaDocumentSetId);
                    }
                }
                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
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

        public static void ClearDocSetAttachments(ILogger log, int asycudaDocumentSetId, string emailId = null) // Add ILogger
        {
            string methodName = nameof(ClearDocSetAttachments);
            log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ AsycudaDocumentSetId: {AsycudaDocumentSetId}, EmailId: {EmailId} }}",
                methodName, "EntryDocSetUtils.ClearDocSetEntries", asycudaDocumentSetId, emailId);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Clearing document set attachments for DocSetId {DocSetId}.", methodName, "StartClear", asycudaDocumentSetId);
                using (var dtx = new DocumentDSContext())
                {
                    log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "dtx.AsycudaDocumentSet_Attachments.Where(...).ToList", "SYNC_EXPECTED");
                    var res = dtx.AsycudaDocumentSet_Attachments.Where(x =>
                        x.AsycudaDocumentSet.SystemDocumentSet == null
                        && x.AsycudaDocumentSetId == asycudaDocumentSetId && emailId == null || x.EmailId != emailId).ToList();
                    log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ResultsFound: {ResultsFound}",
                        "dtx.AsycudaDocumentSet_Attachments.Where(...).ToList", stopwatch.ElapsedMilliseconds, "Sync call returned.", res.Any());

                    if (res.Any())
                    {
                        log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "dtx.AsycudaDocumentSet_Attachments.RemoveRange", "SYNC_EXPECTED");
                        dtx.AsycudaDocumentSet_Attachments.RemoveRange(res);
                        log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "dtx.AsycudaDocumentSet_Attachments.RemoveRange", "Sync call returned.");

                        log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "dtx.SaveChanges", "SYNC_EXPECTED");
                        dtx.SaveChanges();
                        log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "dtx.SaveChanges", "Sync call returned.");
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Removed {RemovedCount} document set attachment records.", methodName, "RemovedRecords", res.Count());
                    }
                    else
                    {
                        log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): No document set attachment records found to clear for DocSetId {DocSetId}.", methodName, "NoRecordsToClear", asycudaDocumentSetId);
                    }
                }
                stopwatch.Stop();
                log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
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

        public static void AddUpdateDocSetAttachement(FileTypes fileType, Email email, CoreEntitiesContext ctx, FileInfo file,
            Attachments attachment, string newReference, ILogger log)
        {
            string operationName = nameof(AddUpdateDocSetAttachement);
            var stopwatch = Stopwatch.StartNew();
            log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}",
                operationName, new { FileTypeId = fileType?.Id, EmailId = email?.EmailId, FileName = file?.Name, AttachmentId = attachment?.Id, NewReference = newReference });

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName} and DocSetId {DocSetId}",
                    "ctx.AsycudaDocumentSet_Attachments.Include(...).FirstOrDefault", "SYNC_EXPECTED", file?.Name, fileType?.AsycudaDocumentSetId);
                var docSetAttachment =
                    ctx.AsycudaDocumentSet_Attachments
                        .Include(x => x.Attachments)
                        .FirstOrDefault(x => x.Attachments.FilePath == file.FullName
                                                        && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) DocSetAttachmentFound: {DocSetAttachmentFound} for file {FileName} and DocSetId {DocSetId}",
                    "ctx.AsycudaDocumentSet_Attachments.Include(...).FirstOrDefault", "Sync call returned.", docSetAttachment != null, file?.Name, fileType?.AsycudaDocumentSetId);

                if (docSetAttachment == null)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Document set attachment record not found, creating new one for file {FileName}.", operationName, "CreateDocSetAttachment", file?.Name);
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}",
                        "ctx.AsycudaDocumentSet_Attachments.Add", "SYNC_EXPECTED", file?.Name);
                    ctx.AsycudaDocumentSet_Attachments.Add(
                        new AsycudaDocumentSet_Attachments(true)
                        {
                            AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                            Attachments = attachment,
                            DocumentSpecific = fileType.DocumentSpecific,
                            FileDate = file.LastWriteTime,
                            EmailId = email.EmailId,
                            FileTypeId = fileType.Id,
                            TrackingState = TrackingState.Added
                        });
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for file {FileName}",
                        "ctx.AsycudaDocumentSet_Attachments.Add", "Sync call returned.", file?.Name);
                }
                else
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Document set attachment record found, updating existing one for file {FileName}.", operationName, "UpdateDocSetAttachment", file?.Name);
                    docSetAttachment.DocumentSpecific = fileType.DocumentSpecific;
                    docSetAttachment.FileDate = file.LastWriteTime;
                    docSetAttachment.EmailId = email.EmailId;
                    docSetAttachment.FileTypeId = fileType.Id;
                    docSetAttachment.Attachments.Reference = newReference;
                    docSetAttachment.Attachments.DocumentCode = fileType.DocumentCode;
                    docSetAttachment.Attachments.EmailId = email.EmailId;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    "ctx.SaveChanges (for document set attachment)", "SYNC_EXPECTED");
                ctx.SaveChanges();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})",
                    "ctx.SaveChanges (for document set attachment)", "Sync call returned.");

                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    operationName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    operationName, stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task CleanupEntries()
        {
            Console.WriteLine("Cleanup ...");
            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_DocumentsToDelete
                    .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => new { x.ASYCUDA_Id, x.AsycudaDocumentSetId }).ToList();
                foreach (var itm in lst)
                {
                    using (var dtx = new DocumentDSContext())
                    {
                        var docEds = dtx.AsycudaDocumentEntryDatas.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).ToList();
                        foreach (var ed in docEds)
                        {
                            var docsetEd = dtx.AsycudaDocumentSetEntryDatas.FirstOrDefault(x =>
                                x.AsycudaDocumentSetId == itm.AsycudaDocumentSetId && x.EntryData_Id == ed.EntryData_Id);
                            if (docsetEd != null) dtx.AsycudaDocumentSetEntryDatas.Remove(docsetEd);
                        }
                        dtx.SaveChanges();
                    }
                    await BaseDataModel.Instance.DeleteAsycudaDocument(itm.ASYCUDA_Id).ConfigureAwait(false);
                }

                var doclst = ctx.TODO_DeleteDocumentSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.AsycudaDocumentSetId).ToList();

                foreach (var itm in doclst)
                {
                    await BaseDataModel.Instance.DeleteAsycudaDocumentSet(itm).ConfigureAwait(false);
                }
            }
        }

        public static List<int> GetDocSetEntryData(int docSetId)
        {
            using (var dtx = new DocumentDSContext())
            {
                return dtx.AsycudaDocumentSetEntryDatas.Where(x => x.AsycudaDocumentSetId == docSetId)
                    .Select(x => x.EntryData_Id).ToList();
            }
        }

         public static async Task SyncConsigneeInDB(FileTypes ft, FileInfo[] fs, ILogger log)
        {
            Console.WriteLine($"--- Logging FileType.Data at start of SyncConsigneeInDB (DocSetId: {ft.AsycudaDocumentSetId}) ---");
            if (ft.Data == null || !ft.Data.Any())
            {
                Console.WriteLine(" FileType.Data is null or empty.");
            }
            else
            {
                foreach (var kvp in ft.Data)
                {
                    Console.WriteLine($" Key: '{kvp.Key}', Value: '{kvp.Value}'");
                }
            }
            Console.WriteLine($"--- End Logging ---");

            Console.WriteLine($"Executing -->> SyncConsigneeInDB for DocSetId: {ft.AsycudaDocumentSetId}");
            try
            {
                if (ft.AsycudaDocumentSetId == 0)
                {
                    Console.WriteLine(" - AsycudaDocumentSetId is 0. Skipping.");
                    return;
                }

                var consigneeCode = ft.Data.FirstOrDefault(kvp => kvp.Key == "Consignee Code").Value;
                var consigneeName = ft.Data.FirstOrDefault(kvp => kvp.Key == "Consignee Name").Value;
                var consigneeAddress = ft.Data.FirstOrDefault(kvp => kvp.Key == "Consignee Address").Value;

                if (string.IsNullOrWhiteSpace(consigneeName))
                {
                    Console.WriteLine(" - ConsigneeName not found in FileType.Data or is empty. Skipping.");
                    return;
                }
                consigneeName = consigneeName.Trim();

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    CoreConsignees consignee = ctx.Consignees
                        .FirstOrDefault(c => c.ConsigneeName == consigneeName && c.ApplicationSettingsId == ft.ApplicationSettingsId);

                    if (consignee == null)
                    {
                        Console.WriteLine($" - Consignee '{consigneeName}' not found. Creating...");
                        var newConsignee = new CoreConsignees
                        {
                            ConsigneeName = consigneeName,
                            ConsigneeCode = consigneeCode,
                            Address = consigneeAddress,
                            ApplicationSettingsId = ft.ApplicationSettingsId,
                            TrackingState = TrackingState.Added
                        };
                        ctx.Consignees.Add(newConsignee);
                        ctx.SaveChanges();
                        consignee = newConsignee;
                        Console.WriteLine($" - Created Consignee with Code: {consignee.ConsigneeCode}");
                    }
                    else
                    {
                        Console.WriteLine($" - Found existing Consignee with Code: {consignee.ConsigneeCode}");
                        if (consigneeCode != null && consignee.ConsigneeCode != consigneeCode)
                        {
                            Console.WriteLine($" - Updating Consignee Code from '{consignee.ConsigneeCode}' to '{consigneeCode}'");
                            consignee.ConsigneeCode = consigneeCode;
                        }
                        if (consigneeAddress != null && consignee.Address != consigneeAddress)
                        {
                            Console.WriteLine($" - Updating Consignee Address from '{consignee.Address}' to '{consigneeAddress}'");
                            consignee.Address = consigneeAddress;
                        }
                        ctx.SaveChanges();
                    }

                    var docSet = ctx.AsycudaDocumentSet
                                        .Include(d => d.Consignees)
                                        .FirstOrDefault(d => d.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);

                    if (docSet != null)
                    {
                        bool changed = false;
                        if (docSet.ConsigneeName != consignee.ConsigneeName)
                        {
                            Console.WriteLine($" - Updating DocSet {docSet.AsycudaDocumentSetId} ConsigneeName from '{docSet.ConsigneeName}' to '{consignee.ConsigneeName}'");
                            docSet.ConsigneeName = consignee.ConsigneeName;
                            changed = true;
                        }
                        if (docSet.Consignees?.ConsigneeCode != consignee.ConsigneeCode)
                        {
                            Console.WriteLine($" - Updating DocSet {docSet.AsycudaDocumentSetId} Consignees relationship.");
                            docSet.Consignees = consignee;
                            changed = true;
                        }

                        if (changed)
                        {
                            ctx.SaveChanges();
                            Console.WriteLine($" - Successfully updated DocSet {docSet.AsycudaDocumentSetId}.");
                        }
                        else
                        {
                            Console.WriteLine($" - DocSet {docSet.AsycudaDocumentSetId} already up-to-date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($" - ERROR: AsycudaDocumentSet with Id {ft.AsycudaDocumentSetId} not found in CoreEntitiesContext.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in SyncConsigneeInDB: {ex.Message}");
                await BaseDataModel.EmailExceptionHandlerAsync(ex, log).ConfigureAwait(false);
            }
        }
    }
}