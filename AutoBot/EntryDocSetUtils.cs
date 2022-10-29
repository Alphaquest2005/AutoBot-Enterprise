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
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class EntryDocSetUtils
    {
        public static void EmailEntriesExpiringNextMonth()
        {
            EX9Utils.EmailEntriesExpiringNextMonth();
        }

        public static void ImportCancelledEntries(FileTypes arg1, FileInfo[] arg2)
        {
            ImportCancelledEntries();
        }

        public static void ImportExpiredEntires(FileTypes arg1, FileInfo[] arg2)
        {
            ImportExpiredEntires();
        }

        public static void EmailWarehouseErrors(FileTypes arg1, FileInfo[] arg2)
        {
            EX9Utils.EmailWarehouseErrors();
        }

        public static void FixIncompleteEntries(FileTypes arg1, FileInfo[] arg2)
        {
            FixIncompleteEntries();
        }

        public static void RemoveDuplicateEntries(FileTypes ft, FileInfo[] fs)
        {
            RemoveDuplicateEntries();
        }

        public static void LinkEmail()
        {
            try

            {
                Console.WriteLine("Link Emails");
                using (var ctx = new CoreEntitiesContext())
                {
                    var entries = ctx.Database.SqlQuery<TODO_ImportCompleteEntries>(
                        $"EXEC [dbo].[Stp_TODO_ImportCompleteEntries] @ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");

                    //var entries = ctx.TODO_ImportCompleteEntries
                    //    .Where(x => x.ApplicationSettingsId ==
                    //                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                    var lst = entries
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Where(x => x.Declarant_Reference_Number != "Imports"), x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z }).ToList();

                    foreach (var itm in entries)
                    {



                        var idoc = ctx.AsycudaDocuments.First(x => x.ASYCUDA_Id == itm.AssessedAsycuda_Id);
                        var cdoc = ctx.AsycudaDocuments.First(x => x.ReferenceNumber == idoc.ReferenceNumber);

                        if (cdoc == null) continue;


                        //ctx.AsycudaDocument_Attachments.Add(
                        //    new AsycudaDocument_Attachments(true)
                        //    {
                        //        AsycudaDocumentId = cdoc.ASYCUDA_Id,
                        //        Attachments = new Attachments(true)
                        //        {
                        //            FilePath = file.FullName,
                        //            DocumentCode = "NA",
                        //            Reference = file.Name.Replace(file.Extension, ""),
                        //            TrackingState = TrackingState.Added

                        //        },

                        //        TrackingState = TrackingState.Added
                        //    });






                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void LogDocSetAction(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var action = ctx.Actions.FirstOrDefault(x => x.Name == actionName);
                if (action == null) throw new ApplicationException($"Action with name:{actionName} not found");
                ctx.ActionDocSetLogs.Add(new ActionDocSetLogs(true)
                {
                    ActonId = action.Id,
                    AsycudaDocumentSetId = docSetId,
                    TrackingState = TrackingState.Added
                });
                ctx.SaveChanges();
            }
        }

        public static void ImportExpiredEntires()
        {

            try
            {
                var docSet = BaseDataModel.CurrentSalesInfo(-1);
                var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docSet.Item3.Declarant_Reference_Number);
                var expFile = Path.Combine(directoryName, "ExpiredEntries.csv");
                if (File.Exists(expFile)) File.Delete(expFile);

                while (!File.Exists(expFile))
                {
                    Utils.RunSiKuLi(directoryName, "ExpiredEntries", "0");
                }
                ImportExpiredEntires(expFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportExpiredEntires(string expFile)
        {
            
            var fileType = new CoreEntitiesContext()
                
                .FileTypes
                .Include(x => x.FileImporterInfos)
                .First(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ExpiredEntries);
            CSVUtils.SaveCsv(new FileInfo[] {new FileInfo(expFile)}, fileType);
        }

        public static void ImportCancelledEntries()
        {

            try
            {
                var docSet = BaseDataModel.CurrentSalesInfo(-1);
                var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docSet.Item3.Declarant_Reference_Number);
                var expFile = Path.Combine(directoryName, "CancelledEntries.csv");
                if (File.Exists(expFile)) File.Delete(expFile);

                while (!File.Exists(expFile))
                {
                    Utils.RunSiKuLi(directoryName, "CancelledEntries", "0");
                }
                ImportCancelledEntries(expFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ImportCancelledEntries(string expFile)
        {

            var fileType = new CoreEntitiesContext().FileTypes.First(x =>
                x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.CancelledEntries);
            CSVUtils.SaveCsv(new FileInfo[] { new FileInfo(expFile) }, fileType);
        }

        public static void RenameDuplicateDocumentCodes()
        {
            try
            {
                Console.WriteLine("Rename Duplicate Documents");
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();
                    var doclst = ctx.AsycudaDocuments.Where(x => x.AsycudaDocumentSetId == docset.AsycudaDocumentSetId)
                        .Select(x => x.ASYCUDA_Id).ToList();
                    if (docset != null)
                    {
                        BaseDataModel.RenameDuplicateDocumentCodes(doclst);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void RenameDuplicateDocuments()
        {
            try
            {
                Console.WriteLine("Rename Duplicate Documents");
                using (var ctx = new CoreEntitiesContext())
                {
                    var docset =
                        ctx.AsycudaDocumentSetExs.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId)
                            .OrderByDescending(x => x.AsycudaDocumentSetId)
                            .FirstOrDefault();
                    if (docset != null)
                    {
                        BaseDataModel.RenameDuplicateDocuments(docset.AsycudaDocumentSetId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void AttachToDocSetByRef(FileTypes ft)
        {
            BaseDataModel.Instance.AttachToExistingDocuments(ft.AsycudaDocumentSetId);
            // BaseDataModel.Instance.CalculateDocumentSetFreight(asycudaDocumentSetId).Wait();
        }

        public static void AttachToDocSetByRef()
        {
            
                
            Console.WriteLine("Attach Documents To DocSet");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var lst = ctx.TODO_PODocSet
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.AttachToExistingDocuments(doc);
                }

            }
        }


        public static void SetFileTypeDocSetToLatest(FileTypes ft)
        {
            if (ft.AsycudaDocumentSetId == 0)
                ft.AsycudaDocumentSetId =
                    new CoreEntitiesContext().AsycudaDocumentSetExs.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault().AsycudaDocumentSetId;
        }

        public static void SubmitEntryCIF(FileTypes ft, FileInfo[] fs)
        {
            try
            {


                Console.WriteLine("Submit Shipment CIF");


                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitEntryCIF
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();
                    if (!lst.Any()) return;
                    if (Enumerable.Any<ActionDocSetLogs>(Utils.GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitEntryCIF"))) return;

                   
                        


                    var body = $"Please see attached CIF for Shipment: {lst.First().Declarant_Reference_Number} . \r\n" +
                                 
                               $"Please double check against your shipment rider.\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    List<string> attlst = new List<string>();
                    var poInfo = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(POUtils.CurrentPOInfo(ft.AsycudaDocumentSetId));

                    var summaryFile = Path.Combine(poInfo.Item2, "CIFValues.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<TODO_SubmitEntryCIF, List<TODO_SubmitEntryCIF>>()
                        {
                            dataToPrint = lst
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    attlst.Add(summaryFile);
                  

                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", $"CIF Values for Shipment: {lst.First().Declarant_Reference_Number}",
                        contacts, body, attlst.ToArray());


                    LogDocSetAction(ft.AsycudaDocumentSetId, "SubmitEntryCIF");


                  

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void AssessEntries(string docReference, int asycudaDocumentSetId)
        {
            

            if (docReference == null) return;
            var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference);
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "Instructions.txt");

            var lcont = 0;
            while (Utils.AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                Utils.RunSiKuLi(directoryName, "AssessIM7", lcont.ToString());//SaveIM7
            }
        }

        public static void ClearDocSetEntries(FileTypes fileType)
        {
            try
            {


                Console.WriteLine($"Clear {fileType.FileImporterInfos.EntryType} Entries");

                // var saleInfo = CurrentSalesInfo();
                var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Result;
                string directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docSet.Declarant_Reference_Number);

                var instFile = Path.Combine(directoryName, "Instructions.txt");
                var resFile = Path.Combine(directoryName, "InstructionResults.txt");
                if (File.Exists(resFile))
                {


                    var resTxt = File.ReadAllText(resFile);

                    foreach (var doc in docSet.Documents.ToList())
                    {
                        if (!resTxt.Contains(doc.ReferenceNumber))
                            BaseDataModel.Instance.DeleteAsycudaDocument(doc).Wait();
                    }

                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId,
                        docSet.Documents.Count());
                }
                else
                {

                    BaseDataModel.Instance.ClearAsycudaDocumentSet(fileType.AsycudaDocumentSetId).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(fileType.AsycudaDocumentSetId, 0);

                    //if (File.Exists(resFile)) File.Delete(resFile);

                }

                ClearDocSetEntryData(fileType.AsycudaDocumentSetId);
                ClearDocSetAttachments(fileType.AsycudaDocumentSetId, fileType.EmailId);
                if (File.Exists(instFile)) File.Delete(instFile);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public static void FixIncompleteEntries()
        {
            try
            {


                Console.WriteLine("ReImport Incomplete Entries");



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;

                    var lst = ctx.TODO_Error_IncompleteItems
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => new { x.ASYCUDA_Id, x.SourceFileName, x.AsycudaDocumentSetId });

                    foreach (var doc in lst)
                    {
                        BaseDataModel.Instance.DeleteAsycudaDocument(doc.Key.ASYCUDA_Id).Wait();
                        BaseDataModel.Instance.ImportDocuments(doc.Key.AsycudaDocumentSetId.GetValueOrDefault(), new List<string>() { doc.Key.SourceFileName }, true, true, false, true, true).Wait();
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void RemoveDuplicateEntries()
        {
            try
            {


                Console.WriteLine("Remove DuplicateEntries");



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var lst = ctx.TODO_Error_DuplicateEntry
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.id);

                    foreach (var dup in lst)
                    {
                        var doc = dup.Last();
                        BaseDataModel.Instance.DeleteAsycudaDocument(doc.ASYCUDA_Id).Wait();
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void ClearDocSetEntryData(int asycudaDocumentSetId)
        {
            using (var dtx = new DocumentDSContext())
            {
                var res = dtx.AsycudaDocumentSetEntryDatas.Where(x =>
                    x.AsycudaDocumentSet.SystemDocumentSet == null
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (res.Any())
                {
                    dtx.AsycudaDocumentSetEntryDatas.RemoveRange(res);
                    dtx.SaveChanges();
                }

            }
        }

        public static void ClearDocSetAttachments(int asycudaDocumentSetId, string emailId = null)
        {
            using (var dtx = new DocumentDSContext())
            {
                var res = dtx.AsycudaDocumentSet_Attachments.Where(x =>
                    x.AsycudaDocumentSet.SystemDocumentSet == null
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId && emailId == null || x.EmailId != emailId);
                if (res.Any())
                {
                    dtx.AsycudaDocumentSet_Attachments.RemoveRange(res);
                    dtx.SaveChanges();
                }

            }
        }

        public static void AddUpdateDocSetAttachement(FileTypes fileType, Email email, CoreEntitiesContext ctx, FileInfo file,
            Attachments attachment, string newReference)
        {
            var docSetAttachment =
                ctx.AsycudaDocumentSet_Attachments
                    .Include(x => x.Attachments)
                    .FirstOrDefault(x => x.Attachments.FilePath == file.FullName
                                         && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
            if (docSetAttachment == null)
            {
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
            }
            else
            {
                docSetAttachment.DocumentSpecific = fileType.DocumentSpecific;
                docSetAttachment.FileDate = file.LastWriteTime;
                docSetAttachment.EmailId = email.EmailId;
                docSetAttachment.FileTypeId = fileType.Id;
                docSetAttachment.Attachments.Reference = newReference;
                docSetAttachment.Attachments.DocumentCode = fileType.DocumentCode;
                docSetAttachment.Attachments.EmailId = email.EmailId;
            }

            ctx.SaveChanges();
        }

        public static void CleanupEntries()
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

                    BaseDataModel.Instance.DeleteAsycudaDocument(itm.ASYCUDA_Id).Wait();
                }

                var doclst = ctx.TODO_DeleteDocumentSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.AsycudaDocumentSetId).ToList();


                foreach (var itm in doclst)
                {
                    BaseDataModel.Instance.DeleteAsycudaDocumentSet(itm).Wait();
                }

                // this wont work because i am saving entrydata in system documentsets

                //ctx.Database.ExecuteSqlCommand(@"delete from xcuda_ASYCUDA where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where AsycudaDocumentSetId is null)

                //                                delete from EntryData where EntryDataId not in (SELECT EntryDataId
                //                                FROM    AsycudaDocumentSetEntryData)");

            }
        }

        public static List<int> GetDocSetEntryData(int docSetId)
        {
            var res = new EntryDataDSContext().EntryDataDetails.Where(x =>
                    x.EntryData.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                    x.EntryData.AsycudaDocumentSets.Any(z => z.AsycudaDocumentSetId == docSetId))
                .Select(z => z.EntryDataDetailsId).ToList();
            return res;
        }


    }
}