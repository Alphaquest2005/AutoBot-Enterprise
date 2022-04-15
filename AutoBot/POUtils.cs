using System;
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
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class POUtils
    {
        public static void DeletePONumber(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                Console.WriteLine("Delete PO Numbers");
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

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void SubmitAssessPOErrors(FileTypes ft)
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


                    var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        docSet.Declarant_Reference_Number);

                    var summaryFile = Path.Combine(directory, "POAssesErrors.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<TODO_PODocSetToAssessErrors, List<TODO_PODocSetToAssessErrors>>()
                        {
                            dataToPrint = res
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }


                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                        $"PO Assessment Errors for Shipment: {docSet.Declarant_Reference_Number}",
                        poContacts, body, new string[] { summaryFile });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo(int docSetId)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var docSet = ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId);
                    var dirPath = StringExtensions.UpdateToCurrentUser(Path.Combine(
                        BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        docSet.Declarant_Reference_Number));
                    return new List<Tuple<AsycudaDocumentSetEx, string>>()
                        { new Tuple<AsycudaDocumentSetEx, string>(docSet, dirPath) };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SubmitPOs()
        {
            try
            {
                Console.WriteLine("Submit POs");

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

                    var lst = MoreEnumerable.MaxBy(ctx.TODO_SubmitPOInfo
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                        x.FileTypeId != null)
                            // .Where(x => !sysLst.Contains(x.AsycudaDocumentSetId))
                            .Where(x => x.IsSubmitted == false)
                            .Where(x => x.CNumber != null)
                            .Where(x => x.Reference.Contains(docset.Declarant_Reference_Number))
                            .ToList(), x => x.AsycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Join(ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments"),
                            x => x.Key, z => z.AsycudaDocumentSetId, (x, z) => new { x, z })
                        .ToList();

                    foreach (var doc in lst)
                    {
                        SubmitPOs(doc.z, doc.x.ToList(), contacts, poContacts);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void EmailLatestPOEntries()
        {
            Console.WriteLine("Create Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.AsycudaDocumentSetExs.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();

                if (docset != null)
                {
                    EmailPOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        public static List<DocumentCT> RecreateLatestPOEntries()
        {
            Console.WriteLine("Create Latest PO Entries");

            var docSet = EntryDocSetUtils.GetLatestDocSet();
            var res = EntryDocSetUtils.GetDocSetEntryData(docSet.AsycudaDocumentSetId);


            return CreatePOEntries(docSet.AsycudaDocumentSetId, res);
        }

        public static void SubmitPOs(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                Console.WriteLine("Submit POs");

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
                        .Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number))
                        .ToList();
                    List<TODO_SubmitPOInfo> lst;
                    if (rlst.Any())
                    {
                        lst = rlst.Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number))
                            .ToList();
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


                    SubmitPOs(docSet, lst, contacts, poContacts);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void SubmitPOs(AsycudaDocumentSetEx docSet, List<TODO_SubmitPOInfo> pOs, string[] contacts,
            string[] poContacts)
        {
            if (!pOs.Any())
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                    $"Document Package for {docSet.Declarant_Reference_Number}",
                    contacts, "No Entries imported", Array.Empty<string>());

                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                    $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                    poContacts, "No Entries imported", Array.Empty<string>());
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
                        Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSetEx, string>>(
                            CurrentPOInfo(docSet.AsycudaDocumentSetId));
                    if (!Directory.Exists(poInfo.Item2)) return;
                    foreach (var itm in pOs)
                    {
                        List<string> ndp = new List<string>();
                        var newEntry = ctx.AsycudaDocuments.FirstOrDefault(x =>
                            x.ReferenceNumber == itm.Reference &&
                            (x.ImportComplete == null || x.ImportComplete == false));
                        if (newEntry != null)
                        {
                            ndp = ctx.AsycudaDocument_Attachments
                                .Where(x => x.AsycudaDocumentId == newEntry.ASYCUDA_Id)
                                .GroupBy(x => x.Attachments.Reference)
                                .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                                .Select(x => x.Attachments.FilePath)
                                .Where(x => x.ToLower().EndsWith(".pdf"))
                                .ToList();
                        }


                        var adp = ctx.AsycudaDocument_Attachments
                            .Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                            .GroupBy(x => x.Attachments.Reference)
                            .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                            .Select(x => x.Attachments.FilePath)
                            .Where(x => x.ToLower().EndsWith(".pdf"))
                            .ToList();

                        if (!adp.Any())
                        {
                            BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id });
                            adp = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                                .Select(x => x.Attachments.FilePath).ToList();
                        }

                        pdfs.AddRange(ndp);
                        pdfs.AddRange(adp);
                        Assessedpdfs.AddRange(adp);
                    }

                    pdfs = pdfs.Distinct().ToList();
                    Assessedpdfs = Assessedpdfs.Distinct().ToList();

                    var body =
                        $"Please see attached documents entries for {docSet.Declarant_Reference_Number}.\r\n" +
                        $"\r\n" +
                        $"Please open the attached email to view Email Thread.\r\n" +
                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                        $"\r\n" +
                        $"Regards,\r\n" +
                        $"AutoBot";


                    var xRes = Enumerable.Select<TODO_SubmitPOInfo, AssessedEntryInfo>(pOs, x =>
                        new AssessedEntryInfo()
                        {
                            DocumentType = x.DocumentType,
                            CNumber = x.CNumber,
                            Reference = x.Reference,
                            Date = x.Date,
                            PONumber = x.PONumber,
                            Invoice = x.SupplierInvoiceNo,
                            Taxes = x.Totals_taxes.GetValueOrDefault().ToString("C"),
                            CIF = x.Total_CIF.ToString("C"),
                            BillingLine = x.BillingLine
                        }).ToList();


                    var summaryFile = Path.Combine(poInfo.Item2, "Summary.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<AssessedEntryInfo, List<AssessedEntryInfo>>
                        {
                            dataToPrint = xRes
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    pdfs.Add(summaryFile);
                    Assessedpdfs.Add(summaryFile);


                    var emailIds = pOs.FirstOrDefault().EmailId;

                    if (emailIds == null)
                    {
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"Document Package for {docSet.Declarant_Reference_Number}",
                            contacts, body, pdfs.ToArray());

                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                            poContacts, body, Assessedpdfs.ToArray());
                    }
                    else
                    {
                        EmailDownloader.EmailDownloader.ForwardMsg(emailIds, Utils.Client,
                            $"Document Package for {docSet.Declarant_Reference_Number}", body, contacts,
                            pdfs.ToArray());

                        EmailDownloader.EmailDownloader.ForwardMsg(emailIds, Utils.Client,
                            $"Assessed Entries for {docSet.Declarant_Reference_Number}", body, poContacts,
                            Assessedpdfs.ToArray());
                    }

                    foreach (var item in pOs)
                    {
                        var sfile = Queryable.FirstOrDefault(ctx.AsycudaDocuments, x =>
                            x.ASYCUDA_Id == item.ASYCUDA_Id &&
                            x.ApplicationSettingsId == item.ApplicationSettingsId);
                        var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                            x.Attachments.FilePath == sfile.SourceFileName);
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

                        ctx.AttachmentLog.Add(new AttachmentLog(true)
                        {
                            DocSetAttachment = eAtt.Id,
                            AsycudaDocumentSet_Attachments = eAtt,
                            Status = "Submit PO Entries",
                            TrackingState = TrackingState.Added
                        });
                    }

                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public static void ExportPOEntries()
        {
            try
            {
                Console.WriteLine("Export PO Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    foreach (var docset in
                             ctx.TODO_PODocSetToExport
                                 .Where(x => x.ApplicationSettingsId ==
                                             BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    {
                        ExportPOEntries(docset.AsycudaDocumentSetId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ExportLatestPOEntries()
        {
            Console.WriteLine("Export Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.AsycudaDocumentSetExs.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    ExportPOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        public static void RecreatePOEntries()
        {
            Console.WriteLine("Create PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docset =
                    ctx.TODO_PODocSet
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    RecreatePOEntries(docset.AsycudaDocumentSetId);
                }
            }
        }

        public static void AssessPOEntries(FileTypes ft)
        {
            Console.WriteLine("Assessing PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_PODocSetToAssess
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId) //ft.AsycudaDocumentSetId == 0 ||
                    .ToList();
                foreach (var doc in res)
                {
                    AssessPOEntry(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                }
            }

            SubmitAssessPOErrors(ft);
        }

        public static void ClearPOEntries()
        {
            Console.WriteLine("Clear PO Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_PODocSetToExport.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                }
            }
        }

        public static void RecreatePOEntries(int asycudaDocumentSetId)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7.GetValueOrDefault())
                    {
                        if (ctx.TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                    }

                    Console.WriteLine("RecreatePOEntries");


                    var res = ctx.ToDo_POToXML.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == asycudaDocumentSetId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new
                        {
                            DocSetId = x.Key,
                            Entrylst = x.Select(z => z.EntryDataDetailsId).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        CreatePOEntries(docSetId.DocSetId, docSetId.Entrylst);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ExportPOEntries(int asycudaDocumentSetId)
        {
            using (var ctx = new DocumentDSContext())
            {
                try
                {
                    IQueryable<xcuda_ASYCUDA> docs;
                    if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true)
                    {
                        if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                                x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                        Console.WriteLine("Export PO Entries");
                        docs = ctx.xcuda_ASYCUDA
                            .Include(x => x.xcuda_Declarant)
                            .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                        asycudaDocumentSetId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                        && (x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsOperationId ==
                                            (int)CustomsOperations.Import
                                            || x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                                .CustomsOperationId == (int)CustomsOperations.Warehouse));
                    }
                    else
                    {
                        docs = ctx.xcuda_ASYCUDA
                            .Include(x => x.xcuda_Declarant)
                            .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId ==
                                        asycudaDocumentSetId
                                        && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                        && (x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsOperationId ==
                                            (int)CustomsOperations.Import
                                            || x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure
                                                .CustomsOperationId == (int)CustomsOperations.Warehouse));
                    }

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
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            docSetId.DocSet.ReferenceNumber);
                        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName); //continue;
                        if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
                            File.Delete(Path.Combine(directoryName, "Instructions.txt"));
                        foreach (var item in docSetId.Entrylst)
                        {
                            var expectedfileName = Path.Combine(directoryName, item.ReferenceNumber + ".xml");
                            //if (File.Exists(expectedfileName)) continue;
                            BaseDataModel.Instance.ExportDocument(expectedfileName, item.z).Wait();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public static void AssessPOEntry(string docReference, int asycudaDocumentSetId)
        {
            try
            {
                if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x =>
                        x.AsycudaDocumentSetId != asycudaDocumentSetId))
                {
                    return;
                }

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
                    Utils.RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void EmailPOEntries(int asycudaDocumentSetId)
        {
            try
            {
                var contacts = new CoreEntitiesContext().Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "PO Clerk")
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();

                if (!contacts.Any() || BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true) return;
                //if (new CoreEntitiesContext().TODO_PODocSetToExport.All(x => x.AsycudaDocumentSetId != asycudaDocumentSetId)) return;
                var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == asycudaDocumentSetId); //CurrentPOInfo();
                if (docSet == null) return;
                //foreach (var poInfo in lst.Where(x => x.Item1.AsycudaDocumentSetId == asycudaDocumentSetId))
                //{

                // if (poInfo.Item1 == null) return;

                var reference = docSet.Declarant_Reference_Number;
                var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    reference);
                if (!Directory.Exists(directory)) return;
                var sourcefiles = Directory.GetFiles(directory, "*.xml");

                var emailres = new FileInfo(Path.Combine(directory, "EmailResults.txt"));
                var instructions = new FileInfo(Path.Combine(directory, "Instructions.txt"));
                if (!instructions.Exists) return;
                if (emailres.Exists) File.Delete(emailres.FullName);
                Console.WriteLine("Emailing Po Entries");
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
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory, $"Entries for {reference}",
                        contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", files);

                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<DocumentCT> CreatePOEntries(int docSetId, List<int> entrylst)
        {
            try
            {
                BaseDataModel.Instance.ClearAsycudaDocumentSet((int)docSetId).Wait();
                BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docSetId, 0);

                var po = CurrentPOInfo(docSetId).FirstOrDefault();
                var insts = Path.Combine(po.Item2, "InstructionResults.txt");
                if (File.Exists(insts)) File.Delete(insts);


                return BaseDataModel.Instance.AddToEntry(entrylst, docSetId,
                    (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true), true, false).Result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}