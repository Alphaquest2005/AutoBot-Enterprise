﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    using ExcelDataReader.Log;
    using Serilog;

    public class POUtils
    {
        public static async Task DeletePONumber(FileTypes ft, FileInfo[] fs, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(DeletePONumber), new { FileType = ft.Description, FileCount = fs.Length });
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var cnumberList = ft.Data.Where(z => z.Key == "PONumber").Select(x => x.Value).ToList();

                    foreach (var itm in cnumberList)
                    {
                        var res = ctx.EntryData.FirstOrDefault(x => x.EntryDataId == itm &&
                                                                    x.ApplicationSettingsId == BaseDataModel.Instance
                                                                        .CurrentApplicationSettings
                                                                        .ApplicationSettingsId);
                        if (res != null) ctx.EntryData.Remove(res);
                    }

                   await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(DeletePONumber), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(DeletePONumber), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        private static async Task SubmitAssessPOErrors(FileTypes ft, ILogger logger)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = ctx.TODO_PODocSetToAssessErrors
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();
                    if (!res.Any()) return;
                    Console.WriteLine("Emailing Assessment Errors - please check Mail");
                    var docSet =
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var body =
                        $"Please see attached documents .\r\n" +
                        $"Please open the attached email to view Email Thread.\r\n" +
                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                        $"\r\n" +
                        $"Regards,\r\n" +
                        $"AutoBot";


                    var directory = BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number);

                    var summaryFile = Path.Combine(directory, "POAssesErrors.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<TODO_PODocSetToAssessErrors, List<TODO_PODocSetToAssessErrors>>()
                        {
                            dataToPrint = res
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        await Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta).ConfigureAwait(false);
                    }


                    await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory,
                        $"PO Assessment Errors for Shipment: {docSet.Declarant_Reference_Number}",
                        poContacts, body, new string[] { summaryFile }, logger).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<Tuple<AsycudaDocumentSet, string>> CurrentPOInfo(int docSetId)
        {
            try
            {
                using (var ctx = new DocumentDSContext())
                {
                    var docSet = ctx.AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == docSetId);
                     
                    var dirPath = StringExtensions.UpdateToCurrentUser(BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number));
                    return new List<Tuple<AsycudaDocumentSet, string>>()
                        { new Tuple<AsycudaDocumentSet, string>(docSet, dirPath) };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task SubmitPOs(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(SubmitPOs));
            try
            {

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts
                        .Where(x => x.Role == "PDF Entries" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();

                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();
                    //var sysLst = new DocumentDSContext().SystemDocumentSets.Select(x => x.Id).ToList();   -- dont bother try to filter it
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();

                    if (docset == null)
                    {
                        logger.Warning("INTERNAL_STEP ({MethodName} - {Stage}): docset is null. Cannot proceed with filtering TODO_SubmitPOInfo based on Declarant_Reference_Number.", nameof(SubmitPOs), "CheckDocSet");
                        stopwatch.Stop();
                        logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(SubmitPOs), stopwatch.ElapsedMilliseconds);
                        return;
                    }

                    var lst = ctx.TODO_SubmitPOInfo
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                        x.FileTypeId != null)
                            // .Where(x => !sysLst.Contains(x.AsycudaDocumentSetId))
                            .Where(x => x.IsSubmitted == false)
                            .Where(x => x.CNumber != null)
                            .Where(x => x.Reference.Contains(docset.Declarant_Reference_Number)) // Now safe due to null check
                            .ToList()
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments"),
                            x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z })
                        .ToList();

                    foreach (var doc in lst)
                    {
                        await SubmitPOs(doc.z, doc.x.ToList(), contacts, poContacts, logger).ConfigureAwait(false);
                    }
                }
                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(SubmitPOs), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(SubmitPOs), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task EmailLatestPOEntries(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(EmailLatestPOEntries));
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset = ctx.AsycudaDocumentSetExs
                        .Where(
                            x => x.ApplicationSettingsId
                                 == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId).FirstOrDefault();

                    if (docset != null)
                    {
                        await EmailPOEntries(docset.AsycudaDocumentSetId, logger).ConfigureAwait(false);
                    }
                }

                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(EmailLatestPOEntries), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(EmailLatestPOEntries), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task<List<DocumentCT>> RecreateLatestPOEntries(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(RecreateLatestPOEntries));
            try
            {

                var docSet = await WaterNut.DataSpace.EntryDocSetUtils.GetLatestDocSet().ConfigureAwait(false);
                var res = EntryDocSetUtils.GetDocSetEntryData(docSet.AsycudaDocumentSetId);


                var result = await CreatePOEntries(docSet.AsycudaDocumentSetId, res, logger).ConfigureAwait(false);
                stopwatch.Stop();
                logger.Information(
                    "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(RecreateLatestPOEntries),
                    stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(
                    e,
                    "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(RecreateLatestPOEntries),
                    stopwatch.ElapsedMilliseconds,
                    e.Message);
                throw;
            }
        }

        public static async Task SubmitPOs(FileTypes ft, FileInfo[] fs, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(SubmitPOs), new { FileType = ft.Description, FileCount = fs.Length });
            try
            {

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var poList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();

                    var contacts = ctx.Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();

                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();
                    var docSet = ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments")
                        .FirstOrDefault(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                    if (docSet == null)
                    {
                        throw new ApplicationException($"Asycuda Document Set not Found: {ft.AsycudaDocumentSetId}");
                    }

                    var rlst = ctx.TODO_SubmitPOInfo
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number) || poList.Contains(x.CNumber))
                        .ToList();
                    List<TODO_SubmitPOInfo> lst;
                    if (rlst.Any())
                    {
                        lst = poList.Any()
                            ? rlst.Where(x => poList.Contains(x.CNumber)).ToList()
                            : rlst.Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number)).ToList();

                    }
                    else
                    {
                        lst = ctx.AsycudaDocuments.Where(x =>
                                x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number)
                                || poList.Contains(x.CNumber))
                            .Where(x => x.ImportComplete == true)
                            .Select(x => new
                            {
                                docSet.ApplicationSettingsId,
                                AssessedAsycuda_Id = x.ASYCUDA_Id,
                                x.CNumber
                            }).ToList()
                            .Select(x => new TODO_SubmitPOInfo()
                            {
                                ApplicationSettingsId = docSet.ApplicationSettingsId,
                                ASYCUDA_Id = x.AssessedAsycuda_Id,
                                CNumber = x.CNumber
                            }).ToList();
                    }

                    if (poList.Any())
                    {
                        lst = lst.Where(x => poList.Contains(x.CNumber))
                            .ToList();
                    }


                    await SubmitPOs(docSet, lst, contacts, poContacts, logger).ConfigureAwait(false);
                }
                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(SubmitPOs), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(SubmitPOs), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        private static async Task SubmitPOs(AsycudaDocumentSetEx docSet, List<TODO_SubmitPOInfo> pOs, string[] contacts,
            string[] poContacts, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(SubmitPOs), new { DocSetId = docSet.AsycudaDocumentSetId, POsCount = pOs.Count });
            try
            {
                if (!pOs.Any())
            {
                await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "",
                    $"Document Package for {docSet.Declarant_Reference_Number}",
                    contacts, "No Entries imported", Array.Empty<string>(), logger).ConfigureAwait(false);

               await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "",
                   $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                   poContacts, "No Entries imported", Array.Empty<string>(), logger).ConfigureAwait(false);
                return;
            }

                using (var ctx = new CoreEntitiesContext())
                {
                    try
                    {
                        //var pdfs = Enumerable
                        //    .Select<AsycudaDocumentSet_Attachments, string>(docSet.AsycudaDocumentSet_Attachments,
                        //        x => x.Attachments.FilePath)
                        //    .Where(x => x.ToLower().EndsWith(".pdf"))
                        //    .ToList();

                        var Assessedpdfs = new List<string>();

                        var pdfs = new List<string>();

                        var poInfo =
                            Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(
                                CurrentPOInfo(docSet.AsycudaDocumentSetId));
                        if (!Directory.Exists(poInfo.Item2)) return;
                        foreach (var itm in pOs)
                        {
                            List<string> ndp = new List<string>();
                            var newEntry = ctx.AsycudaDocuments.FirstOrDefault(
                                x => x.ReferenceNumber == itm.Reference
                                     && (x.ImportComplete == null || x.ImportComplete == false));
                            if (newEntry != null)
                            {
                                ndp = ctx.AsycudaDocument_Attachments
                                    .Where(x => x.AsycudaDocumentId == newEntry.ASYCUDA_Id)
                                    .GroupBy(x => x.Attachments.Reference)
                                    .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                                    .Select(x => x.Attachments.FilePath).Where(x => x.ToLower().EndsWith(".pdf"))
                                    .ToList();
                            }


                            var adp = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                                .GroupBy(x => x.Attachments.Reference)
                                .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                                .Select(x => x.Attachments.FilePath).Where(x => x.ToLower().EndsWith(".pdf")).ToList();

                            if (!adp.Any())
                            {
                                await BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id }).ConfigureAwait(false);
                                adp = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                                    .Select(x => x.Attachments.FilePath).ToList();
                            }

                            pdfs.AddRange(ndp);
                            pdfs.AddRange(adp);
                            Assessedpdfs.AddRange(adp);
                        }

                        pdfs = pdfs.Distinct().ToList();
                        Assessedpdfs = Assessedpdfs.Distinct().ToList();

                        var body = $"Please see attached documents entries for {docSet.Declarant_Reference_Number}.\r\n"
                                   + $"\r\n" + $"Please open the attached email to view Email Thread.\r\n"
                                   + $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n"
                                   + $"\r\n" + $"Regards,\r\n" + $"AutoBot";


                        var xRes = Enumerable.Select<TODO_SubmitPOInfo, AssessedEntryInfo>(
                            pOs,
                            x => new AssessedEntryInfo()
                                     {
                                         DocumentType = x.DocumentType,
                                         CNumber = x.CNumber,
                                         Reference = x.Reference,
                                         Date = x.Date.ToString(),
                                         PONumber = x.PONumber,
                                         Invoice = x.SupplierInvoiceNo,
                                         Taxes = x.Totals_taxes.GetValueOrDefault().ToString("C"),
                                         CIF = x.Total_CIF.ToString("C"),
                                         BillingLine = x.BillingLine
                                     }).ToList();


                        var summaryFile = Path.Combine(poInfo.Item2, "Summary.csv");
                        if (File.Exists(summaryFile)) File.Delete(summaryFile);
                        var errRes = new ExportToCSV<AssessedEntryInfo, List<AssessedEntryInfo>> { dataToPrint = xRes };
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            await Task.Factory.StartNew(
                                () => errRes.SaveReport(summaryFile),
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                sta).ConfigureAwait(false);
                        }

                        pdfs.Add(summaryFile);
                        Assessedpdfs.Add(summaryFile);


                        var emailIds = pOs.FirstOrDefault().EmailId;

                        if (emailIds == null)
                        {
                            await EmailDownloader.EmailDownloader.SendEmailAsync(
                                Utils.Client,
                                "",
                                $"Document Package for {docSet.Declarant_Reference_Number}",
                                contacts,
                                body,
                                pdfs.ToArray(), logger).ConfigureAwait(false);

                            await EmailDownloader.EmailDownloader.SendEmailAsync(
                                Utils.Client,
                                "",
                                $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                                poContacts,
                                body,
                                Assessedpdfs.ToArray(), logger).ConfigureAwait(false);
                        }
                        else
                        {
                            await EmailDownloader.EmailDownloader.ForwardMsgAsync(
                                emailIds,
                                Utils.Client,
                                $"Document Package for {docSet.Declarant_Reference_Number}",
                                body,
                                contacts,
                                pdfs.ToArray(), logger).ConfigureAwait(false);

                            await EmailDownloader.EmailDownloader.ForwardMsgAsync(
                                emailIds,
                                Utils.Client,
                                $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                                body,
                                poContacts,
                                Assessedpdfs.ToArray(), logger).ConfigureAwait(false);
                        }

                        foreach (var item in pOs)
                        {
                            var sfile = Queryable.FirstOrDefault(
                                ctx.AsycudaDocuments,
                                x => x.ASYCUDA_Id == item.ASYCUDA_Id
                                     && x.ApplicationSettingsId == item.ApplicationSettingsId);
                            var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(
                                x => x.Attachments.FilePath == sfile.SourceFileName);
                            if (eAtt == null)
                            {
                                var att = ctx.Attachments.OrderByDescending(x => x.Id)
                                    .FirstOrDefault(x => x.FilePath == sfile.SourceFileName);
                                eAtt = new AsycudaDocumentSet_Attachments(true)
                                           {
                                               AsycudaDocumentSetId = item.AsycudaDocumentSetId,
                                               AttachmentId = att.Id,
                                               DocumentSpecific = true,
                                               FileDate = DateTime.Now,
                                               EmailId = att.EmailId,
                                           };
                                ctx.AsycudaDocumentSet_Attachments.Add(eAtt);
                            }

                            ctx.AttachmentLog.Add(
                                new AttachmentLog(true)
                                    {
                                        DocSetAttachment = eAtt.Id,
                                        AsycudaDocumentSet_Attachments = eAtt,
                                        Status = "Submit PO Entries",
                                        TrackingState = TrackingState.Added
                                    });
                        }

                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        stopwatch.Stop();
                        logger.Information(
                            "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                            nameof(SubmitPOs),
                            stopwatch.ElapsedMilliseconds);
                    }


                    
                }
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(SubmitPOs), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task ExportPOEntries(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(ExportPOEntries));
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    foreach (var docset in ctx.TODO_PODocSetToExport.Where(
                                 x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                          .ApplicationSettingsId))
                    {
                        await ExportPOEntries(docset.AsycudaDocumentSetId, logger).ConfigureAwait(false);
                    }
                }

                stopwatch.Stop();
                logger.Information(
                    "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ExportPOEntries),
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(
                    e,
                    "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ExportPOEntries),
                    stopwatch.ElapsedMilliseconds,
                    e.Message);
                throw;
            }
        }

        public static async Task ExportLatestPOEntries(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(ExportLatestPOEntries));
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset = ctx.AsycudaDocumentSetExs
                        .Where(
                            x => x.ApplicationSettingsId
                                 == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId).FirstOrDefault();
                    if (docset != null)
                    {
                        await ExportPOEntries(docset.AsycudaDocumentSetId, logger).ConfigureAwait(false);
                    }

                    stopwatch.Stop();
                    logger.Information(
                        "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(ExportLatestPOEntries),
                        stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(ExportLatestPOEntries), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task RecreatePOEntries(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(RecreatePOEntries));
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset = ctx.TODO_PODocSet
                        .Where(
                            x => x.ApplicationSettingsId
                                 == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId).FirstOrDefault();
                    if (docset != null)
                    {
                        await RecreatePOEntries(docset.AsycudaDocumentSetId, logger).ConfigureAwait(false);
                    }
                }

                stopwatch.Stop();
                logger.Information(
                    "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(RecreatePOEntries),
                    stopwatch.ElapsedMilliseconds);

            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(
                    e,
                    "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(RecreatePOEntries),
                    stopwatch.ElapsedMilliseconds,
                    e.Message);
                throw;
            }
        }

        public static async Task AssessPOEntries(FileTypes ft, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information(
                "METHOD_ENTRY: {MethodName}. Context: {@MethodContext}",
                nameof(AssessPOEntries),
                new { FileType = ft.Description });
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = ctx.TODO_PODocSetToAssess
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId) //ft.AsycudaDocumentSetId == 0 ||
                        .ToList();
                    foreach (var doc in res)
                    {
                        await AssessPOEntry(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId, logger)
                            .ConfigureAwait(false);
                    }
                }

                await SubmitAssessPOErrors(ft, logger).ConfigureAwait(false);
                stopwatch.Stop();
                logger.Information(
                    "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(AssessPOEntries),
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(
                    e,
                    "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(AssessPOEntries),
                    stopwatch.ElapsedMilliseconds,
                    e.Message);
                throw;
            }
        }

        public static async Task ClearPOEntries(Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}", nameof(ClearPOEntries));
            try
            {

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var lst = ctx.TODO_PODocSetToExport
                        .Where(
                            x => x.ApplicationSettingsId
                                 == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        //.Where(x => x.Key != null)
                        .Select(x => x.Key).Distinct().ToList();

                    foreach (var doc in lst)
                    {
                        await BaseDataModel.Instance.ClearAsycudaDocumentSet(doc, logger).ConfigureAwait(false);
                        BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                    }
                }

                stopwatch.Stop();
                logger.Information(
                    "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ClearPOEntries),
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(
                    e,
                    "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ClearPOEntries),
                    stopwatch.ElapsedMilliseconds,
                    e.Message);
                throw;
            }
        }

        public static async Task RecreatePOEntries(int asycudaDocumentSetId, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information(
                "METHOD_ENTRY: {MethodName}. Context: {@MethodContext}",
                nameof(RecreatePOEntries),
                new { AsycudaDocumentSetId = asycudaDocumentSetId });
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                    {
                        if (ctx.TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId))
                        {
                            stopwatch.Stop();
                            logger.Information(
                                "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                                nameof(RecreatePOEntries),
                                stopwatch.ElapsedMilliseconds);
                            return;
                        }
                    }


                    var res = ctx.ToDo_POToXML_Recreate
                        .Where(
                            x => x.ApplicationSettingsId
                                 == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                 && x.AsycudaDocumentSetId == asycudaDocumentSetId).GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new { DocSetId = x.Key, Entrylst = x.Select(z => z.EntryDataDetailsId).ToList() })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        await CreatePOEntries(docSetId.DocSetId, docSetId.Entrylst, logger).ConfigureAwait(false);
                    }
                }

                stopwatch.Stop();
                logger.Information(
                    "METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(RecreatePOEntries),
                    stopwatch.ElapsedMilliseconds);
            }




            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(
                    e,
                    "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(RecreatePOEntries),
                    stopwatch.ElapsedMilliseconds,
                    e.Message);
                throw;
            }
        }

        public static async Task ExportPOEntries(int asycudaDocumentSetId, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(ExportPOEntries), new { AsycudaDocumentSetId = asycudaDocumentSetId });
            try
            {
                using (var ctx = new DocumentDSContext())
                {
                    IQueryable<xcuda_ASYCUDA> docs;
                    var defaultCustomsOperation = BaseDataModel.GetDefaultCustomsOperation();
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                    {
                        if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                                x.AsycudaDocumentSetId != asycudaDocumentSetId))
                        {
                            stopwatch.Stop();
                            logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(ExportPOEntries), stopwatch.ElapsedMilliseconds);
                            return;
                        }
                    }

                    docs = ctx.xcuda_ASYCUDA
                        .Include(x => x.xcuda_Declarant)
                        .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                    && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                    asycudaDocumentSetId
                                    && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                    && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                            .CustomsOperationId == defaultCustomsOperation);

                    var res = docs.GroupBy(x => new
                        {
                            x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId,
                            ReferenceNumber = x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                .Declarant_Reference_Number
                        }).Select(x => new
                        {
                            DocSet = x.Key,
                            Entrylst = x.Select(z => new { z, ReferenceNumber = z.xcuda_Declarant.Number }).ToList()
                        })
                        .ToList();


                    foreach (var docSetId in res)
                    {
                        var directoryName = BaseDataModel.GetDocSetDirectoryName(docSetId.DocSet.ReferenceNumber);
                        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName); //continue;
                        if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
                            File.Delete(Path.Combine(directoryName, "Instructions.txt"));
                        foreach (var item in docSetId.Entrylst)
                        {
                            var expectedfileName = Path.Combine(directoryName, item.ReferenceNumber + ".xml");
                            //if (File.Exists(expectedfileName)) continue;
                            await BaseDataModel.Instance.ExportDocument(expectedfileName, item.z).ConfigureAwait(false);
                        }
                    }
                }
                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(ExportPOEntries), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(ExportPOEntries), stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

        public static async Task AssessPOEntry(string docReference, int asycudaDocumentSetId, Serilog.ILogger log)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            log.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(AssessPOEntry), new { DocReference = docReference, AsycudaDocumentSetId = asycudaDocumentSetId });
            try
            {
                if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                        x.AsycudaDocumentSetId != asycudaDocumentSetId))
                {
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(AssessPOEntry), stopwatch.ElapsedMilliseconds);
                    return;
                }

                if (docReference == null)
                {
                    stopwatch.Stop();
                    log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(AssessPOEntry), stopwatch.ElapsedMilliseconds);
                    return;
                }
                var directoryName = BaseDataModel.GetDocSetDirectoryName(docReference);
                var resultsFile = Path.Combine(directoryName, "InstructionResults.txt");
                var instrFile = Path.Combine(directoryName, "Instructions.txt");

                int lcont;
                var assessmentResult = await Utils.AssessComplete(instrFile, resultsFile, log).ConfigureAwait(false);
                lcont = assessmentResult.lcontValue;

                while (assessmentResult.success == false)
                {
                    // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                    Utils.RunSiKuLi(directoryName, "SaveIM7", lcont.ToString(), 0, 0, 0, 0, true); // Pass true for enableDebugging
                    assessmentResult = await Utils.AssessComplete(instrFile, resultsFile, log).ConfigureAwait(false);
                    lcont = assessmentResult.lcontValue;
                }
                stopwatch.Stop();
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(AssessPOEntry), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(AssessPOEntry), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task EmailPOEntries(int asycudaDocumentSetId, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(EmailPOEntries), new { AsycudaDocumentSetId = asycudaDocumentSetId });
            try
            {
                var contacts = new CoreEntitiesContext().Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "PO Clerk")
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();

                if (!contacts.Any() || BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                {
                    stopwatch.Stop();
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(EmailPOEntries), stopwatch.ElapsedMilliseconds);
                    return;
                }
                //if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == asycudaDocumentSetId); //CurrentPOInfo();
                if (docSet == null)
                {
                    stopwatch.Stop();
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(EmailPOEntries), stopwatch.ElapsedMilliseconds);
                    return;
                }
                //foreach (var poInfo in lst.Where(x => x.Item1.AsycudaDocumentSetId == asycudaDocumentSetId))
                //{

                // if (poInfo.Item1 == null) return;

                var reference = docSet.Declarant_Reference_Number;
                var directory = BaseDataModel.GetDocSetDirectoryName(reference);
                if (!Directory.Exists(directory))
                {
                    stopwatch.Stop();
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(EmailPOEntries), stopwatch.ElapsedMilliseconds);
                    return;
                }
                var sourcefiles = Directory.GetFiles(directory, "*.xml");

                var emailres = new FileInfo(Path.Combine(directory, "EmailResults.txt"));
                var instructions = new FileInfo(Path.Combine(directory, "Instructions.txt"));
                if (!instructions.Exists)
                {
                    stopwatch.Stop();
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(EmailPOEntries), stopwatch.ElapsedMilliseconds);
                    return;
                }
                if (emailres.Exists) File.Delete(emailres.FullName);
                string[] files;
                if (File.Exists(emailres.FullName))
                {
                    var eRes = File.ReadAllLines(emailres.FullName);
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = sourcefiles.Where(x => insts.Any(z => z.Contains(x)) && !eRes.Any(z => z.Contains(x)))
                        .ToArray();
                }
                else
                {
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = sourcefiles.ToList().Where(x => insts.Any(z => z.Contains(x))).ToArray();
                }

                if (files.Length > 0)
                   await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory, $"Entries for {reference}",
                        contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", files, logger).ConfigureAwait(false);

                //}
                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(EmailPOEntries), stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(EmailPOEntries), stopwatch.ElapsedMilliseconds, e.Message);
                throw;
            }
        }

        public static async Task<List<DocumentCT>> CreatePOEntries(int docSetId, List<int> entrylst, Serilog.ILogger logger)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            logger.Information("METHOD_ENTRY: {MethodName}. Context: {@MethodContext}", nameof(CreatePOEntries), new { DocSetId = docSetId, EntryListCount = entrylst.Count });
            try
            {
                await BaseDataModel.Instance.ClearAsycudaDocumentSet((int)docSetId, logger).ConfigureAwait(false);
                BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docSetId, 0);

                var po = CurrentPOInfo(docSetId).FirstOrDefault();
                var insts = Path.Combine(po.Item2, "InstructionResults.txt");
                if (File.Exists(insts)) File.Delete(insts);


                var result = await BaseDataModel.Instance.AddToEntry(entrylst, docSetId,
                    (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true), true, false, logger).ConfigureAwait(false);
                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", nameof(CreatePOEntries), stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(CreatePOEntries), stopwatch.ElapsedMilliseconds, ex.Message);
                await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null, $"Bug Found",
                    EmailDownloader.EmailDownloader.GetContacts("Developer", logger), $"{ex.Message}\r\n{ex.StackTrace}",
                    Array.Empty<string>(), logger).ConfigureAwait(false);
                throw;
            }
        }
    }
}