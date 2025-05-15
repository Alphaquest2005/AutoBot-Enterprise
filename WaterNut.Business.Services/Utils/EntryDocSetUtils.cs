using System.Threading.Tasks;
﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.EF6;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace WaterNut.DataSpace
{
    public class EntryDocSetUtils
    {
        private static readonly ILogger _log = Log.ForContext<EntryDocSetUtils>(); // Add static logger

        public static async Task<(AsycudaDocumentSet docSet, List<IGrouping<string, xcuda_ASYCUDA>> lst)>
            GetDuplicateDocuments(int docKey)
        {
            try
            {

                var docSet = await GetAsycudaDocumentSet(docKey).ConfigureAwait(false);

                var res = await GetRelatedDocuments(docSet).ConfigureAwait(false);

                var lst = FilterForDuplicateDocuments(res, docSet);

                return (docSet, lst);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int docKey)
        {
            using (var ctx = new DocumentDSContext())
            {
                return await ctx.AsycudaDocumentSets.Include("xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA")
                    .FirstAsync(x => x.AsycudaDocumentSetId == docKey).ConfigureAwait(false);
            }
        }

        private static List<IGrouping<string, xcuda_ASYCUDA>> FilterForDuplicateDocuments(
            List<IGrouping<string, xcuda_ASYCUDA>> res, AsycudaDocumentSet docSet)
        {
            return res
                .Where(x => x.Key != null && x.Count() > 1 &&
                            x.Any(z => z.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false)
                            || x.Any(z => z.xcuda_ASYCUDA_ExtendedProperties.FileNumber == docSet.LastFileNumber))
                .ToList();
        }

        public static async Task<List<IGrouping<string, xcuda_ASYCUDA>>> GetRelatedDocuments(AsycudaDocumentSet docSet)
        {

            using (var ctx = new DocumentDSContext())
            {
                var res = await ctx.xcuda_ASYCUDA
                    .Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
                    .Include(x => x.xcuda_Declarant)
                    .Where(
                        x => x != null && x.xcuda_Declarant != null &&
                             x.xcuda_Declarant.Number.Contains(docSet.Declarant_Reference_Number) &&
                             ((x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                               x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false) ||
                               (x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId != docSet.AsycudaDocumentSetId &&
                                x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete))
                    )
                    .ToListAsync() // must load first to get the navigation properties
                    .ConfigureAwait(false);
                return res.GroupBy(x => x.xcuda_Declarant.Number).ToList();

            }
        }

        public static async Task RenameDuplicateDocuments(List<IGrouping<string, xcuda_ASYCUDA>> lst,
            AsycudaDocumentSet docSet) // Removed 'ref' keyword
        {
            try
            {
                var docSetAsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                using (var ctx = new DocumentDSContext { StartTracking = true })
                {
                    foreach (var g in lst)
                    foreach (var doc in g.Where(x => x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false).ToList())
                    {
                        //AppendDocSetLastFileNumber(docSet);
                        var prop = await GetXcudaAsycudaExtendedProperties(doc, docSetAsycudaDocumentSetId).ConfigureAwait(false);
                        var declarant = ctx.xcuda_Declarant.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                        var oldRef = declarant.Number;
                        var newRef = GetNewReference(docSet, declarant, ctx);
                        declarant.Number = newRef;
                        declarant.TrackingState = TrackingState.Modified;
                        //prop.FileNumber = docSet.LastFileNumber;
                        ctx.SaveChanges();
                        ctx.ApplyChanges(docSet);
                        ctx.SaveChanges();

                        await UpdateNameDependentAttachments(prop.ASYCUDA_Id, oldRef, newRef).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string GetNewReference(AsycudaDocumentSet docSet, xcuda_Declarant declarant, DocumentDSContext ctx)
        {
            var newNum = Convert.ToInt32(Regex.Match(declarant.Number, @"\d+(?=[^\d]*$)").Value);

            string newRef;
            while (true)
            {
                newNum += 1;
                newRef = Regex.Replace(declarant.Number, @"\d+(?=[^\d]*$)", newNum.ToString());
                if (!ctx.xcuda_Declarant.Any(x => x.Number == newRef)) break;
            }

            //newRef = Regex.Replace(declarant.Number, @"\d+(?=[^\d]*$)", docSet.LastFileNumber.ToString());
            return newRef;
        }

        private static async Task<xcuda_ASYCUDA_ExtendedProperties> GetXcudaAsycudaExtendedProperties(xcuda_ASYCUDA doc,
            int docSetAsycudaDocumentSetId)
        {
            try
            {
                using (var ctx = new DocumentDSContext { StartTracking = true })
                {
                    return await ctx.xcuda_ASYCUDA_ExtendedProperties.FirstAsync(x =>
                        x.ASYCUDA_Id == doc.ASYCUDA_Id).ConfigureAwait(false); //&& x.AsycudaDocumentSetId == docSetAsycudaDocumentSetId
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void AppendDocSetLastFileNumber(AsycudaDocumentSet docSet)
        {
            docSet.LastFileNumber += 1;
            docSet.TrackingState = TrackingState.Modified;
        }

        private static async Task UpdateNameDependentAttachments(int asycudaId, string oldRef, string newRef)
        {
            using (var ctx = new DocumentItemDSContext { StartTracking = true })
            {
                var itms = await GetAttachementItems(asycudaId, oldRef, ctx).ConfigureAwait(false);

                foreach (var itm in itms)
                {
                    UpdateAttachments(oldRef, newRef, itm);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static void UpdateAttachments(string oldRef, string newRef, xcuda_Attached_documents itm)
        {
            itm.Attached_document_reference = newRef;
            foreach (var att in itm.xcuda_Attachments.Where(x => x.Attachments.Reference == oldRef)
                         .ToList())
            {
                att.Attachments.Reference = newRef;
                att.Attachments.FilePath = att.Attachments.FilePath.Replace(oldRef, newRef);
            }
        }

        private static async Task<List<xcuda_Attached_documents>> GetAttachementItems(int asycudaId, string oldRef,
            DocumentItemDSContext ctx)
        {
            return await ctx.xcuda_Attached_documents
                .Include("xcuda_Attachments.Attachments")
                .Where(x => x.Attached_document_reference == oldRef)
                .Where(x => x.xcuda_Item.ASYCUDA_Id == asycudaId)
                .ToListAsync().ConfigureAwait(false);
        }

        public static async Task<AsycudaDocumentSet> GetLatestDocSet()
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = await ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                return docSet;
            }
        }

        public static async Task<AsycudaDocumentSet> GetDocSet(string Reference)
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = await ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Declarant_Reference_Number.Contains(Reference))
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                return docSet;
            }
        }

        public static async Task<List<AsycudaDocumentSet>> GetLatestModifiedDocSet()
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = await ctx.AsycudaDocumentSets.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.Any(z => z.ImportComplete == false))
                    .OrderByDescending(x => x.xcuda_ASYCUDA_ExtendedProperties.Max(z => z.ASYCUDA_Id))
                    .ToListAsync().ConfigureAwait(false);
                return docSet;
            }
        }

        public static async Task<(DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath)> CreateMonthYearAsycudaDocSet(
            ILogger log, DateTime startDate) // Add ILogger parameter
        {
            string methodName = nameof(CreateMonthYearAsycudaDocSet);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ StartDate: {StartDate} }}",
                methodName, "BaseDataModel.GetCurrentSalesInfo", startDate); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Calculating end date and document reference.", methodName, "CalculateEndDateAndDocRef"); // Add step log
                var endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23);
                var docRef = startDate.ToString("MMMM") + " " + startDate.Year;

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetAsycudaDocumentSet", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                var getDocSetStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var docSet = await GetAsycudaDocumentSet(log, docRef, false).ConfigureAwait(false); // Pass log
                getDocSetStopwatch.Stop();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "GetAsycudaDocumentSet", getDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log


                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting/Creating directory path.", methodName, "GetOrCreateDirectory"); // Add step log
                var dirPath =
                    StringExtensions.UpdateToCurrentUser(
                        BaseDataModel.GetDocSetDirectoryName(docRef));
                if (!Directory.Exists(dirPath))
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Directory does not exist, creating it.", methodName, "CreateDirectory"); // Add step log
                    Directory.CreateDirectory(dirPath);
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                return (StartDate: startDate, EndDate: endDate, DocSet: docSet, DirPath: dirPath);
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        public static async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(ILogger log, string docRef, bool isSystemDocSet) // Add ILogger parameter
        {
            string methodName = nameof(GetAsycudaDocumentSet);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocRef: {DocRef}, IsSystemDocSet: {IsSystemDocSet} }}",
                methodName, "EntryDocSetUtils.CreateMonthYearAsycudaDocSet", docRef, isSystemDocSet); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            AsycudaDocumentSet docSet;
            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Searching for existing document set.", methodName, "SearchExistingDocSet"); // Add step log
                using (var ctx = new DocumentDSContext()) // Add using for proper disposal
                {
                    docSet = await ctx.AsycudaDocumentSets.FirstOrDefaultAsync(x => // Use FirstOrDefaultAsync
                        x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);
                }

                if (docSet == null)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Document set not found, creating a new one.", methodName, "CreateNewDocSet"); // Add step log
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CreateAsycudaDocumentSet", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
                    var createDocSetStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    docSet = await CreateAsycudaDocumentSet(log, docRef, isSystemDocSet).ConfigureAwait(false); // Pass log
                    createDocSetStopwatch.Stop();
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "CreateAsycudaDocumentSet", createDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log
                }
                else
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Existing document set found.", methodName, "ExistingDocSetFound"); // Add step log
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                return docSet;
            }
            catch (Exception ex) // Catch specific exception variable
            {
                stopwatch.Stop(); // Stop stopwatch
                log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
                throw; // Re-throw the original exception
            }
        }

        public static async Task<AsycudaDocumentSet> CreateAsycudaDocumentSet(ILogger log, string docRef, bool isSystemDocSet) // Add ILogger parameter
        {
            string methodName = nameof(CreateAsycudaDocumentSet);
            log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ DocRef: {DocRef}, IsSystemDocSet: {IsSystemDocSet} }}",
                methodName, "EntryDocSetUtils.GetAsycudaDocumentSet", docRef, isSystemDocSet); // Add METHOD_ENTRY log
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

            try
            {
                AsycudaDocumentSet docSet;
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting default customs operation.", methodName, "GetDefaultCustomsOperation"); // Add step log
                var defaultCustomsOperation = BaseDataModel.GetDefaultCustomsOperation();

                using (var ctx = new DocumentDSContext())
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Finding document type.", methodName, "FindDocumentType"); // Add step log
                    var doctype = BaseDataModel.Instance.Customs_Procedures
                        .First(x =>
                        {

                            return x.CustomsOperationId == defaultCustomsOperation
                                   && x.IsDefault == true;
                        }); //&& x.Discrepancy != true

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Inserting new document set into database.", methodName, "InsertDocSet"); // Add step log
                    await ctx.Database.ExecuteSqlCommandAsync($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId},'{docRef}',{doctype.Customs_ProcedureId},0)").ConfigureAwait(false);

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Retrieving newly created document set.", methodName, "RetrieveNewDocSet"); // Add step log
                    docSet = await ctx.AsycudaDocumentSets.FirstOrDefaultAsync(x =>
                        x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);

                    if (!isSystemDocSet)
                    {
                        stopwatch.Stop(); // Stop stopwatch
                        log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                            methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                        return docSet;
                    }

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Adding to system document sets.", methodName, "AddToSystemDocSets"); // Add step log
                    ctx.SystemDocumentSets.Add(new SystemDocumentSet(true)
                        { AsycudaDocumentSet = docSet, TrackingState = TrackingState.Added });
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Saving changes to database.", methodName, "SaveChanges"); // Add step log
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                return docSet;
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