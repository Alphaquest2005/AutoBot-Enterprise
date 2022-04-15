using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using MoreLinq;
using SalesDataQS.Business.Services;
using TrackableEntities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class SalesUtils
    {
        public static void SubmitSalesToCustoms()
        {
            try
            {

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 20;
                

                    IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
                    lst = ctx.TODO_SubmitSalesToCustoms.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)

                        .ToList()
                        .Select(x => new TODO_SubmitDiscrepanciesToCustoms()
                        {
                            CNumber = x.CNumber,
                            ApplicationSettingsId = x.ApplicationSettingsId,
                            AssessmentDate = x.AssessmentDate,
                            ASYCUDA_Id = x.ASYCUDA_Id,
                            AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                            CustomsProcedure = x.CustomsProcedure,
                            DocumentType = x.DocumentType,
                            EmailId = x.EmailId,
                            ReferenceNumber = x.ReferenceNumber,
                            RegistrationDate = x.RegistrationDate,
                            Status = x.Status,
                            ToBePaid = x.ToBePaid
                        })
                        .GroupBy(x => x.EmailId);
                    SubmitSalesToCustoms(lst);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ClearAllocations()
        {
            Console.WriteLine("Clear Allocations");
            AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
        }

        public static void ReSubmitSalesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {


                var lst = DISUtils.GetSubmitEntryData(ft);


                SalesUtils.SubmitSalesToCustoms(lst);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void SubmitUnknownDFPComments()
        {
            try
            {

                Console.WriteLine("Submit UnknownDFP Comments");
                return;
                /// Fix up this
                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId).ToList();

                    foreach (var docSet in lst)
                    {



                        var body = $"Error: UnknownDFP Comments\r\n" +
                                   $"\r\n" +
                                   $"The Following Comments Classification(DutyFree Or Duty Paid) are Unknown.\r\n" +
                                   $"{docSet.MaxEntryLines}.\r\n" +

                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();




                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SubmitSalesToCustoms(IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst)
        {
            try
            {


                Console.WriteLine("Submit Sales To Customs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts
                        .Where(x => x.Role == "Customs" || x.Role == "Clerk")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var pdfs = new List<string>();
                    var RES = MoreEnumerable.DistinctBy(lst.SelectMany(x => x), x => x.ASYCUDA_Id);
                    if (!RES.Any())
                    {
                        Console.WriteLine("No Sales Found!");
                        return;
                    }
                    foreach (var itm in RES)
                    {

                       

                        var res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                        if (!res.Any())
                        {
                            BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id });
                            res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                        }
                        pdfs.AddRange(res);
                       

                        if (pdfs.Count == 0) continue;
                    }
                    var body = "The Following Sales Entries were Assessed. \r\n" +

                               $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"AssessmentDate".FormatedSpace(20)}\r\n" +
                               $"{RES.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.ReferenceNumber.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                               $"\r\n" +
                               $"Please open the attached email to view Email Thread.\r\n" +
                               $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                               $"\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";


                    var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSetEx, string>>(POUtils.CurrentPOInfo(RES.First().AsycudaDocumentSetId));
                    var directory = info.Item2;

                    var summaryFile = Path.Combine(directory, $"SalesSummary.csv");
                       


                    var sumres =
                        new ExportToCSV<TODO_SubmitDiscrepanciesToCustoms, List<TODO_SubmitDiscrepanciesToCustoms>>()
                        {
                            dataToPrint = RES.ToList()
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => sumres.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    //if (RES.Key == null)
                    //{
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", "Assessed Ex-Warehoused Entries",
                        contacts, body, pdfs.ToArray());
                    //}
                    //else
                    //{
                    //    EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Assessed Ex-Warehoused Entries", body, contacts, pdfs.ToArray());
                    //}

                    foreach (var item in RES)
                    {
                        var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                            x.ASYCUDA_Id == item.ASYCUDA_Id &&
                            x.ApplicationSettingsId == item.ApplicationSettingsId);
                        var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                            x.Attachments.FilePath == sfile.SourceFileName);
                        if (eAtt != null)
                        {
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = eAtt.Id,
                                Status = "Submit Sales To Customs",
                                TrackingState = TrackingState.Added
                            });
                        }
                    }

                    ctx.SaveChanges();

                    

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void ReDownloadSalesFiles()
        {
            //var directoryName = CurrentSalesInfo().Item4;
            //var overview = Path.Combine(directoryName, "OverView.txt");
            //if(File.Exists(overview)) File.Delete(overview);
            EX9Utils.DownloadSalesFiles(100, "IM7History", true);
        }

        public static void RebuildSalesReport()
        {
            Console.WriteLine("Allocations Started");
            BuildSalesReportClass.Instance.ReBuildSalesReports();
        }

        public static void ImportAllSalesEntries()
        {
            try
            {
                Console.WriteLine("Import Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    //var docSetId = BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                    var docSetId = ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                        x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ?? BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;

                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.Type == "XML" && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                            .Declarant_Reference_Number);
                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                        .ToArray();
                    if (csvFiles.Length > 0)
                        BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();

                    Utils.ImportAllFilesInDataFolder();

                    EntryDocSetUtils.RemoveDuplicateEntries();
                    EntryDocSetUtils.FixIncompleteEntries();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task ExportLastDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            IEnumerable<AsycudaDocument> doclst = await GetSalesDocumentsWithEntryData
                (asycudaDocumentSetId).ConfigureAwait(false);
            if (doclst == null || !doclst.ToList().Any()) return;


            var exceptions = new ConcurrentQueue<Exception>();

            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                {
                    var s = new ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>>();
                    s.StartUp();
                    foreach (var doc in doclst.Where(x => x != null).ToList())
                    {
                        try
                        {
                            var data = GetDocumentSalesReport(doc.ASYCUDA_Id)?.Result;
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv.pdf");
                                s.dataToPrint = Enumerable.ToList<EX9Utils.SaleReportLine>(data);
                                s.SaveReport(path);
                            }
                            else
                            {
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                            }

                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    }
                    s.ShutDown();
                },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        private static async Task<IEnumerable<AsycudaDocument>> GetSalesDocumentsWithEntryData(int asycudaDocumentSetId)
        {
            try
            {
                var docset = await BaseDataModel.Instance.GetDocSetWithEntryDataDocs(asycudaDocumentSetId).ConfigureAwait(false);

                return docset.Documents.Select(x =>
                    new SalesDataService().GetSalesDocument(
                        x.ASYCUDA_Id).Result).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static async Task<IEnumerable<EX9Utils.SaleReportLine>> GetDocumentSalesReport(int ASYCUDA_Id)
        {
            var alst = await EX9Utils.Ex9SalesReport(ASYCUDA_Id).ConfigureAwait(false);
            if (Enumerable.Any<EX9Utils.SaleReportLine>(alst)) return alst;

            alst = ADJUtils.IM9AdjustmentsReport(ASYCUDA_Id);
            return alst;
        }

        public static void ImportSalesEntries()
        {
            try
            {
                Console.WriteLine("Import Entries");
                using (var ctx = new CoreEntitiesContext())
                {
                    //var docSetId = BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                    var docSetId = ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                        x.Declarant_Reference_Number == "Imports")?.AsycudaDocumentSetId ?? BaseDataModel.CurrentSalesInfo().Item3.AsycudaDocumentSetId;

                    var lastdbfile =
                        ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == docSetId);
                    var lastfiledate = lastdbfile != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1);
                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.Type == "XML" && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return;
                    var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                            .Declarant_Reference_Number);
                    var directoryInfo = new DirectoryInfo(desFolder);
                    directoryInfo.Refresh();
                    var csvFiles = directoryInfo.GetFiles()
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase))
                        .Where(x => x.LastWriteTime >= lastfiledate)
                        .ToArray();

                    if (csvFiles.Length > 0)
                        BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList(), true, true, false, true, true).Wait();

                    Utils.ImportAllFilesInDataFolder();

                    EntryDocSetUtils.RemoveDuplicateEntries();
                    EntryDocSetUtils.FixIncompleteEntries();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void EmailSalesErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "SalesErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Status != null)
                    .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>
                {
                    dataToPrint = errors
                };
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory, $"Sales Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[]
                    {
                        errorfile
                    });
            }

        }

        public static void AllocateSales()
        {
            Console.WriteLine("Allocations Started");
            using (var ctx = new CoreEntitiesContext())
            {
                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true
                    && !ctx.TODO_UnallocatedSales.Any(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)) return;
                AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings
                    .ApplicationSettingsId).Wait();
                AllocationsBaseModel.Instance
                    .AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false)
                    .Wait();
                EmailSalesErrors();
            }
        }

        public static async Task ExportDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            var doclst =
                await
                    new SalesDataService().GetSalesDocuments(
                            asycudaDocumentSetId)
                        .ConfigureAwait(false);
            if (doclst == null || !doclst.ToList().Any()) return;


            var exceptions = new ConcurrentQueue<Exception>();

            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                    {
                        var s = new ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>>();
                        s.StartUp();
                        foreach (var doc in doclst)
                        {
                            try
                            {
                                var data = SalesUtils.GetDocumentSalesReport(doc.ASYCUDA_Id).Result;
                                if (data != null)
                                {
                                    string path = Path.Combine(folder,
                                        !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv.pdf");
                                    s.dataToPrint = Enumerable.ToList<EX9Utils.SaleReportLine>(data);
                                    s.SaveReport(path);
                                }
                                else
                                {
                                    File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                                }

                            }
                            catch (Exception ex)
                            {
                                exceptions.Enqueue(ex);
                            }
                        }
                        s.ShutDown();
                    },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public static void SubmitSalesXMLToCustoms()
        {
            try
            {


                Console.WriteLine("Submit XML To Customs");

                // var saleInfo = CurrentSalesInfo();
                var salesinfo = BaseDataModel.CurrentSalesInfo();

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 60;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitXMLToCustoms.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                .ApplicationSettingsId
                            && x.AssessmentDate >= salesinfo.Item1 && x.AssessmentDate <= salesinfo.Item2)
                        .ToList()
                        .Where(x => x.ReferenceNumber.Contains(salesinfo.Item3.Declarant_Reference_Number))
                        .ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Ex-Warehouse Entries were Assessed. \r\n" +
                                   $"Start Date: {salesinfo.Item1} End Date {salesinfo.Item2}: \r\n" +
                                   "\tCNumber\t\tReference\t\tAssessmentDate\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.CNumber}\t\t{current.ReferenceNumber}\t\t{current.RegistrationDate.Value:yyyy-MM-dd} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                        foreach (var itm in emailIds)
                        {

                            attlst.AddRange(ctx.AsycudaDocument_Attachments
                                .Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath)
                                .ToList());
                        }


                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", "Assessed Ex-Warehouse Entries",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key, Utils.Client, "Assessed Ex-Warehouse Entries", body, contacts, attlst.ToArray());
                        }

                        foreach (var item in emailIds)
                        {
                            var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                                x.ASYCUDA_Id == item.ASYCUDA_Id &&
                                x.ApplicationSettingsId == item.ApplicationSettingsId);
                            var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                                x.Attachments.FilePath == sfile.SourceFileName);
                            if (eAtt != null)
                            {
                                ctx.AttachmentLog.Add(new AttachmentLog(true)
                                {
                                    DocSetAttachment = eAtt.Id,
                                    Status = "Submit XML To Customs",
                                    TrackingState = TrackingState.Added
                                });
                            }
                        }

                        ctx.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}