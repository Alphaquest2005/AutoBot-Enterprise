using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AdjustmentQS.Business.Entities;
using AdjustmentQS.Business.Services;
using AutoBotUtilities;
using Core.Common.Converters;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class DISUtils
    {
        private static readonly int _databaseCommandTimeout = 30;
        private static readonly int __ColumnWidth = 15;
        private static readonly int _tripleLongDatabaseCommandTimeout = _databaseCommandTimeout* 3;

        public static void AssessDiscrepancyExecutions(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext() {StartTracking = true})
                {
                    ctx.Database.CommandTimeout = 10;
                    foreach (var file in fs)
                    {
                        var dt = CSVUtils.CSV2DataTable(file, "YES");
                        if (dt.Rows.Count == 0) continue;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (string.IsNullOrEmpty(row["Reference"].ToString())) continue;
                            var reference = row["Reference"].ToString();
                            var doc = ctx.AsycudaDocuments.Include(x => x.AsycudaDocumentSetEx).FirstOrDefault(x =>
                                    x.ReferenceNumber == reference && x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                ?.AsycudaDocumentSetEx;
                            if(doc != null) EntryDocSetUtils.AssessEntries(doc.Declarant_Reference_Number, doc.AsycudaDocumentSetId);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ReSubmitDiscrepanciesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                var emailId = ft.EmailId;
                var lst = GetSubmitEntryData(ft);
                SubmitDiscrepanciesToCustoms(Enumerable.Where<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>>(lst, x => x.Key == emailId));

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void SubmitDiscrepanciesToCustoms(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = _databaseCommandTimeout;

                    IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
                    lst = ctx.TODO_SubmitDiscrepanciesToCustoms.Where(x => x.EmailId == ft.EmailId
                                                                           && x.ApplicationSettingsId ==
                                                                           BaseDataModel.Instance
                                                                               .CurrentApplicationSettings
                                                                               .ApplicationSettingsId)

                        .ToList()

                        .GroupBy(x => x.EmailId);
                    SubmitDiscrepanciesToCustoms(lst);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void SubmitDiscrepanciesToCustoms()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
               
                IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
                lst = ctx.TODO_SubmitDiscrepanciesToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId)

                    .ToList()

                    .GroupBy(x => x.EmailId);
                SubmitDiscrepanciesToCustoms(lst);

            }
        }

        public static IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> GetSubmitEntryData(FileTypes ft)
        {
            try
            {
                
                    var cnumberList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();
                    
                    var cplst = BaseDataModel.Instance.Customs_Procedures
                        .Where(x => x.CustomsOperation.Name == "Exwarehouse" || (x.CustomsOperation.Name == "Warehouse" && x.Stock == true)).Select(x => x.CustomsProcedure).ToList();

                    var res = cnumberList.Any() 
                        ? GetSubmitEntryDataPerCNumber(cplst, cnumberList) 
                        : GetSubmitEntryDataPerDocSet(ft, cplst);

                    var lst = CreateDISEntryDataFromSubmitData(res);
                    return lst;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> CreateDISEntryDataFromSubmitData(List<TODO_SubmitAllXMLToCustoms> res)
        {
            IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
            lst = res.Select(x => new TODO_SubmitDiscrepanciesToCustoms()
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
            return lst;
        }

        private static List<TODO_SubmitAllXMLToCustoms> GetSubmitEntryDataPerDocSet(FileTypes ft, List<string> cplst)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                
                ctx.Database.CommandTimeout = _tripleLongDatabaseCommandTimeout;
                List<TODO_SubmitAllXMLToCustoms> res;
                var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(ft.AsycudaDocumentSetId).Result;
                res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId && cplst.Any(z => z == x.CustomsProcedure))
                    .ToList()
                    .Where(x => x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number))
                    .ToList();
                return res;
            }
        }

        private static List<TODO_SubmitAllXMLToCustoms> GetSubmitEntryDataPerCNumber(List<string> cplst, List<string> cnumberList)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                List<TODO_SubmitAllXMLToCustoms> res;
                res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId && cplst.Any(z => z == x.CustomsProcedure))
                    .ToList()
                    .Where(x => cnumberList.Contains(x.CNumber))
                    .ToList();
                return res;
            }
        }

        private static void SubmitDiscrepanciesToCustoms(IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst)
        {
            try
            {


                Console.WriteLine("Submit Discrepancies To Customs");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = _databaseCommandTimeout;
                    var contacts = GetContacts(new List<string>(){ "Customs" });
                   
                    foreach (var data in lst)
                    {
                        CreateDiscrepancyEmail(data, contacts);
                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void CreateDiscrepancyEmail(IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data, string[] contacts)
        {
            
                var cNumbers = GetCNumbers(data);
                var directory = GetDiscrepancyDirectory(cNumbers);
                var executionFile = CreateExecutionFile(directory, data);

                var otherCNumbers = executionFile.execData
                    .Where(x => cNumbers.All(z => z.CNumber != x.xCNumber))
                    .Select(x => new TODO_SubmitDiscrepanciesToCustoms()
                    {
                        CNumber = x.xCNumber,
                        ApplicationSettingsId = x.ApplicationSettingsId,
                        AssessmentDate = DateTime.Parse(x.xRegistrationDate),
                        AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                        CustomsProcedure = x.CustomsProcedure,
                        DocumentType = x.DocumentType,
                        EmailId = x.EmailId,
                        ReferenceNumber = x.ReferenceNumber,
                        RegistrationDate = DateTime.Parse(x.xRegistrationDate),
                        Status = x.Status,
                        ToBePaid = "",
                        ASYCUDA_Id = x.ASYCUDA_Id.GetValueOrDefault()

                    })
                    .GroupBy(x => x.EmailId);


            var pdfs = AttachDiscrepancyPdfs(cNumbers);

                //if (pdfs.Count == 0) return;

                var body = CreateEmailBody(cNumbers);

            if (pdfs.Count == 0) body = body + $"\r\n Sorry no pdfs were downloaded for this discrepancy.";

           


                
                pdfs.Add(executionFile.executionFile);

                var summaryFile = CreateSummaryFile(cNumbers, directory);
                pdfs.Add(summaryFile);

                SendDiscrepancyEmail(cNumbers, data, contacts, body, pdfs);

                AttachDocumentsPerEmail(cNumbers);

               
            
        }

        private static string CreateEmailBody(List<TODO_SubmitDiscrepanciesToCustoms> cNumbers)
        {
            var body = "The Following Discrepancies Entries were Assessed. \r\n" +
                       $"\t{"pCNumber".FormatedSpace(__ColumnWidth)}{"Reference".FormatedSpace(__ColumnWidth)}{"To Be Paid".FormatedSpace(__ColumnWidth)}{"AssessmentDate".FormatedSpace(__ColumnWidth)}\r\n" +
                       $"{cNumbers.Select(current => $"\t{current.CNumber.FormatedSpace(__ColumnWidth)}{current.ReferenceNumber.FormatedSpace(__ColumnWidth)}{current.ToBePaid.FormatedSpace(__ColumnWidth)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(__ColumnWidth)} \r\n").Aggregate((old, current) => old + current)}" +
                       $"\r\n" +
                       $"Please open the attached email to view Email Thread.\r\n" +
                       $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                       $"\r\n" +
                       $"Regards,\r\n" +
                       $"AutoBot";
            return body;
        }

        private static List<TODO_SubmitDiscrepanciesToCustoms> GetCNumbers(IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data)
        {
            var emailIds = MoreEnumerable.DistinctBy(data, x => x.CNumber).ToList();
            if (!emailIds.Any())
            {
                throw new NotImplementedException();
            }

            return emailIds;
        }

        private static List<string> AttachDiscrepancyPdfs(List<TODO_SubmitDiscrepanciesToCustoms> emailIds)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var pdfs = new List<string>();
                foreach (var itm in emailIds)
                {
                    var res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                        .Select(x => x.Attachments.FilePath).ToArray();
                    if (!res.Any())
                    {
                        BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id });
                        res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                            .Select(x => x.Attachments.FilePath).ToArray();
                    }

                    pdfs.AddRange(res);
                }

                return pdfs;
            }
        }

        private static string GetDiscrepancyDirectory(List<TODO_SubmitDiscrepanciesToCustoms> emailIds)
        {
            var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(
                POUtils.CurrentPOInfo(emailIds.First().AsycudaDocumentSetId));
            var directory = info.Item2;
            return directory;
        }

        private static (string executionFile, List<DiscpancyExecData> execData) CreateExecutionFile(string directory, IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data)
        {
            var executionFile = Path.Combine(directory, $"ExecutionReport.csv");
            var execData = new CoreEntitiesContext().TODO_DiscrepanciesExecutionReport
                .Where(x => x.EmailId == data.Key)
                .Select(x => new DiscpancyExecData()
                {
                    InvoiceNo = x.InvoiceNo,
                    InvoiceDate = x.InvoiceDate,
                    ItemNumber = x.ItemNumber,
                    InvoiceQty = x.InvoiceQty,
                    ReceivedQty = x.ReceivedQty,
                    CNumber = x.CNumber,
                    Status = x.Status,
                    xCNumber = x.xCNumber,
                    xLineNumber = x.xLineNumber,
                    xRegistrationDate = x.xRegistrationDate.ToString(),
                    EmailId = x.EmailId,
                    ApplicationSettingsId = x.ApplicationSettingsId,
                    AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                    DocumentType = x.DocumentType,
                    ReferenceNumber = x.Declarant_Reference_Number,
                    CustomsProcedure = x.CustomsProcedure,
                    ASYCUDA_Id = x.ASYCUDA_Id

                })
                .ToList();

            var exeRes =
                new ExportToCSV<DiscpancyExecData, List<DiscpancyExecData>>()
                {
                    dataToPrint = execData
                };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => exeRes.SaveReport(executionFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }

            return (executionFile, execData);
        }

        private static string CreateSummaryFile(List<TODO_SubmitDiscrepanciesToCustoms> emailIds, string directory)
        {

            var summaryFile = Path.Combine(directory, $"Summary.csv");
            var sumData = emailIds
                .Select(x => new SubmitEntryData()
                {
                    CNumber = x.CNumber,
                    ReferenceNumber = x.ReferenceNumber,
                    DocumentType = x.DocumentType,
                    CustomsProcedure = x.CustomsProcedure,
                    RegistrationDate = x.RegistrationDate,
                    AssessmentDate = x.AssessmentDate
                })
                .ToList();

            var sumres =
                new ExportToCSV<SubmitEntryData, List<SubmitEntryData>>()
                {
                    dataToPrint = sumData
                };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => sumres.SaveReport(summaryFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }
            return summaryFile;
        }

        private static void SendDiscrepancyEmail(List<TODO_SubmitDiscrepanciesToCustoms> emailIds, IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data, string[] contacts, string body, List<string> pdfs)
        {
            string reference = emailIds.First().ReferenceNumber.Substring(0, emailIds.First().ReferenceNumber.IndexOf('-'));

            if (data.Key == null)
            {
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                    $"Assessed Shipping Discrepancy Entries: {reference}",
                    contacts, body, pdfs.ToArray());
            }
            else
            {
                EmailDownloader.EmailDownloader.ForwardMsg(data.Key, Utils.Client,
                    $"Assessed Shipping Discrepancy Entries: {reference}", body, contacts, pdfs.ToArray());
            }
        }

        private static void AttachDocumentsPerEmail(List<TODO_SubmitDiscrepanciesToCustoms> emailIds)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
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
                    else
                    {
                        var attachment =
                            ctx.Attachments.First(x => x.FilePath == sfile.SourceFileName);
                        ctx.AsycudaDocumentSet_Attachments.Add(new AsycudaDocumentSet_Attachments()
                        {
                            TrackingState = TrackingState.Added,
                            AsycudaDocumentSetId = ctx.AsycudaDocumentSetExs.First(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.Declarant_Reference_Number == "Imports").AsycudaDocumentSetId,
                            AttachmentId = attachment.Id,
                            DocumentSpecific = true,
                            EmailId = item.EmailId,
                            AttachmentLog = new List<AttachmentLog>()
                            {
                                new AttachmentLog(true)
                                {
                                    Status = "Submit XML To Customs",
                                    TrackingState = TrackingState.Added
                                }
                            }
                        });
                    }
                }

                ctx.SaveChanges();
            }
        }


        private static string[] GetContacts(List<string> roles)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var contacts = ctx.Contacts.Where(x => roles.Contains(x.Role))
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.EmailAddress)
                    .Distinct()
                    .ToArray();
                return contacts;
            }
        }

    
        public static void SubmitDiscrepanciesPreAssessmentReportToCustoms()
        {
            try
            {


                Console.WriteLine("Submit Discrepancies PreAssessment Report to Customs");


                var info = BaseDataModel.CurrentSalesInfo(-1);
                var directory = info.Item4;

               
                    var contacts = GetContacts(new List<string>() {"Customs", "Clerk"});

                    var totaladjustments = GetAllDiscrepanciesToProcesses();

                    var errors = SubmitDiscrepanciesErrorReports();

                    var goodadj = GetAllDiscrepancyPreExecutionReports();

                    var errBreakdown = errors.GroupBy(x => x.Status).ToList();
                    var attachments = new List<string>();
                    attachments.AddRange(AttachErrors(errors, directory));

                    attachments.AddRange(AttachExecutions(goodadj, directory));
                    if (attachments.Any())
                    {
                        SendDiscrepancyPreAssessmentEmail(totaladjustments, errBreakdown, goodadj, directory, contacts, attachments);
                    }
                
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static bool SendDiscrepancyPreAssessmentEmail(List<TODO_TotalAdjustmentsToProcess> totaladjustments, List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown, List<DiscrepancyPreExecutionReport> goodadj,
            string directory, string[] contacts, List<string> attachments)
        {
            var body = CreateDiscrepancyPreAssesmentEmailBody(totaladjustments, errBreakdown, goodadj);
            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                $"Discrepancy Pre-Assessment Report for  {totaladjustments.Min(x => x.EffectiveDate)} To: {totaladjustments.Max(x => x.EffectiveDate)}",
                contacts.ToArray(), body, attachments.ToArray());
            return true;
        }

        private static string CreateDiscrepancyPreAssesmentEmailBody(List<TODO_TotalAdjustmentsToProcess> totaladjustments, List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown, List<DiscrepancyPreExecutionReport> goodadj)
        {
            var body =
                $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                $"\r\n" +
                $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(_databaseCommandTimeout)}{"Percentage".FormatedSpace(_databaseCommandTimeout)}\r\n" +
                $"{string.Join(",", errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(_databaseCommandTimeout)}{(Math.Round((double)(((double)current.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(_databaseCommandTimeout)}% \r\n"))}" +
                $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(_databaseCommandTimeout)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(_databaseCommandTimeout)}% \r\n" +
                $"\r\n" +
                $"Please see attached for list of Errors and Executions details.\r\n" +
                $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                $"\r\n" +
                $"Regards,\r\n" +
                $"AutoBot";
            return body;
        }

        private static List<string> AttachExecutions(List<DiscrepancyPreExecutionReport> goodadj, string directory)
        {
            List<string> attachments = new List<string>();
            if (goodadj.Any())
            {
                var goodfile = Path.Combine(directory, "DiscrepancyExecutions.csv");
                if (File.Exists(goodfile)) File.Delete(goodfile);
                var goodRes =
                    new ExportToCSV<DiscrepancyPreExecutionReport, List<DiscrepancyPreExecutionReport>>()
                    {
                        dataToPrint = goodadj
                    };
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => goodRes.SaveReport(goodfile), CancellationToken.None,
                        TaskCreationOptions.None, sta);
                }

                attachments.Add(goodfile);
            }
            return attachments;
        }

        private static List<string> AttachErrors(List<SubmitDiscrepanciesErrorReport> errors, string directory)
        {
            List<string> attachments = new List<string>();
            if (errors.Any())
            {
                
                var errorfile = Path.Combine(directory, "DiscrepancyExecutionErrors.csv");
                if (File.Exists(errorfile)) File.Delete(errorfile);
                var errRes =
                    new ExportToCSV<SubmitDiscrepanciesErrorReport, List<SubmitDiscrepanciesErrorReport>>()
                    {
                        dataToPrint = errors
                    };
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => errRes.SaveReport(errorfile), CancellationToken.None,
                        TaskCreationOptions.None, sta);
                }

                attachments.Add(errorfile);
            }

            return attachments;
        }

        private static List<DiscrepancyPreExecutionReport> GetAllDiscrepancyPreExecutionReports()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var goodadj = ctx.TODO_DiscrepancyPreExecutionReport.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId)
                    .Select(x => new DiscrepancyPreExecutionReport()
                    {
                        Type = x.Type,
                        InvoiceDate = x.InvoiceDate,
                        EffectiveDate = x.EffectiveDate,
                        InvoiceNo = x.EntryDataId,
                        LineNumber = x.LineNumber,
                        ItemNumber = x.ItemNumber,
                        ItemDescription = x.ItemDescription,
                        InvoiceQty = x.InvoiceQty,
                        ReceivedQty = x.ReceivedQty,
                        Quantity = x.Quantity,
                        Cost = x.Cost,
                        PreviousCNumber = x.PreviousCNumber,
                        PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                        comment = x.Comment,
                        Status = x.Status,
                        DutyFreePaid = x.DutyFreePaid,
                        subject = x.Subject,
                        emailDate = x.EmailDate ?? DateTime.MinValue,
                        Reference = x.Reference,
                        DocumentType = x.DocumentType,
                    })
                    .ToList();
                return goodadj;
            }
        }

        private static List<SubmitDiscrepanciesErrorReport> SubmitDiscrepanciesErrorReports()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var errors = ctx.TODO_DiscrepanciesErrors.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId)
                    .Select(x => new SubmitDiscrepanciesErrorReport()
                    {
                        Type = x.Type,
                        InvoiceDate = x.InvoiceDate,
                        EffectiveDate = x.EffectiveDate,
                        InvoiceNo = x.InvoiceNo,
                        LineNumber = x.LineNumber,
                        ItemNumber = x.ItemNumber,
                        ItemDescription = x.ItemDescription,
                        InvoiceQty = x.InvoiceQty,
                        ReceivedQty = x.ReceivedQty,
                        Quantity = x.Quantity,
                        Cost = x.Cost,
                        PreviousCNumber = x.PreviousCNumber,
                        PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                        comment = x.Comment,
                        Status = x.Status,
                        DutyFreePaid = x.DutyFreePaid,
                        subject = x.Subject,
                        emailDate = x.EmailDate
                    })
                    .ToList();
                return errors;
            }
        }

        private static List<TODO_TotalAdjustmentsToProcess> GetAllDiscrepanciesToProcesses()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId ==
                    BaseDataModel
                        .Instance.CurrentApplicationSettings
                        .ApplicationSettingsId && x.Type == "DIS").ToList();
                return totaladjustments;
            }
        }


        public static void AssessDISEntries(string adjustmentType)
        {
            Console.WriteLine("Assessing Discrepancy Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.TODO_DiscrepanciesToAssess
                    .Where(x => x.Type == adjustmentType)
                    .ToList();
                foreach (var doc in res)
                {
                    if (doc.Declarant_Reference_Number == null) return;
                    var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number);
                    var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "InstructionResults.txt");
                    var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        doc.Declarant_Reference_Number, "Instructions.txt");

                    var lcont = 0;
                    while (Utils.AssessComplete(instrFile, resultsFile, out lcont) == false)
                    {
                        // RunSiKuLi(doc.AsycudaDocumentSetId, "AssessIM7", lcont.ToString());
                        Utils.RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
                    }
                }

            }

        }

        public static void SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(FileTypes fileType)
        {
            try
            {


                Console.WriteLine("Submit Discrepancies PreAssessment Report to Customs");





               
                    var docset = GetAsycudaDocumentSetEx(fileType);
                    if (docset == null) return;

                    var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(POUtils.CurrentPOInfo(fileType.AsycudaDocumentSetId));
                    var directory = info.Item2;

                    var contacts = GetContacts(new List<string>() { "Broker", "Clerk", "Customs" });
                        
                        
                        
                    var totaladjustments = GetDocSetDiscrepanciesToProcess(fileType.AsycudaDocumentSetId);
                    
                    var errors = GetDocSetDiscrepancyErrors(fileType.AsycudaDocumentSetId);

                    var goodadj = GetDocSetDiscrepancyPreExecutionReports(fileType.AsycudaDocumentSetId);
                    var attachments = new List<string>();

                    var errBreakdown = errors.GroupBy(x => x.Status).ToList();

                    attachments.AddRange(AttachErrors(errors, directory));

                    attachments.AddRange(AttachExecutions(goodadj, directory));
                   
                    if (attachments.Any())
                    {
                        SendDocSetDiscrepancyEmail(fileType, errors, totaladjustments, errBreakdown, goodadj, docset, contacts, attachments);
                    }
                
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private static void SendDocSetDiscrepancyEmail(FileTypes fileType, List<SubmitDiscrepanciesErrorReport> errors, List<TODO_TotalAdjustmentsToProcess> totaladjustments,
            List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown, List<DiscrepancyPreExecutionReport> goodadj, AsycudaDocumentSetEx docset, string[] contacts, List<string> attachments)
        {
            var body = CreateDiscrepancyWithErrorsEmailBody(errors, totaladjustments, errBreakdown, goodadj);
            EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId, Utils.Client,
                $"Discrepancy Pre-Assessment Report for  {docset.Declarant_Reference_Number}", body, contacts,
                attachments.ToArray());
        }

        private static string CreateDiscrepancyWithErrorsEmailBody(List<SubmitDiscrepanciesErrorReport> errors, List<TODO_TotalAdjustmentsToProcess> totaladjustments, List<IGrouping<string, SubmitDiscrepanciesErrorReport>> errBreakdown,
            List<DiscrepancyPreExecutionReport> goodadj)
        {
            var errorBody = errors.Any()
                ? "Discrepancies Found: \r\n" +
                  "System could not Generate Entries the following items on the CNumbers Stated: \r\n" +
                  $"\t{"Data Number".FormatedSpace(_databaseCommandTimeout)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                  $"{errors.Select(current => $"\t{current.ItemNumber.FormatedSpace(_databaseCommandTimeout)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.PreviousCNumber.FormatedSpace(15)}{(current.Status + " | " + current.comment).FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
                  "\r\n" +
                  "\r\n" +
                  " The Attached File contains details of errors found trying to assessed the attached email's Shipping Discrepancies \r\n"
                : "";

            var body =
                $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                $"\r\n" +
                $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(_databaseCommandTimeout)}{"Percentage".FormatedSpace(_databaseCommandTimeout)}\r\n" +
                $"{string.Join(",", errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(_databaseCommandTimeout)}{(Math.Round((double)(((double)current.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(_databaseCommandTimeout)}% \r\n"))}" +
                $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(_databaseCommandTimeout)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(_databaseCommandTimeout)}% \r\n" +
                $"\r\n" +
                $"{errorBody}" +
                $"Please see attached for list of Errors and Executions details.\r\n" +
                $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                $"\r\n" +
                $"Regards,\r\n" +
                $"AutoBot";
            return body;
        }

        private static List<DiscrepancyPreExecutionReport> GetDocSetDiscrepancyPreExecutionReports(int docSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                return ctx.TODO_DiscrepancyPreExecutionReport.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        && x.AsycudaDocumentSetId == docSetId)

                    .Select(x => new DiscrepancyPreExecutionReport()
                    {
                        Type = x.Type,
                        InvoiceDate = x.InvoiceDate,
                        EffectiveDate = x.EffectiveDate,
                        InvoiceNo = x.EntryDataId,
                        LineNumber = x.LineNumber,
                        ItemNumber = x.ItemNumber,
                        ItemDescription = x.ItemDescription,
                        InvoiceQty = x.InvoiceQty,
                        ReceivedQty = x.ReceivedQty,
                        Quantity = x.Quantity,
                        Cost = x.Cost,
                        PreviousCNumber = x.PreviousCNumber,
                        PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                        comment = x.Comment,
                        Status = x.Status,
                        DutyFreePaid = x.DutyFreePaid,
                        subject = x.Subject,
                        emailDate = x.EmailDate ?? DateTime.MinValue,
                        Reference = x.Reference,
                        DocumentType = x.DocumentType,

                    })
                    .ToList();
            }
        }

        private static List<SubmitDiscrepanciesErrorReport> GetDocSetDiscrepancyErrors(int docSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                return ctx.TODO_DiscrepanciesErrors.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        && x.AsycudaDocumentSetId == docSetId)
                    .Select(x => new SubmitDiscrepanciesErrorReport()
                    {
                        Type = x.Type,
                        InvoiceDate = x.InvoiceDate,
                        EffectiveDate = x.EffectiveDate,
                        InvoiceNo = x.InvoiceNo,
                        LineNumber = x.LineNumber,
                        ItemNumber = x.ItemNumber,
                        ItemDescription = x.ItemDescription,
                        InvoiceQty = x.InvoiceQty,
                        ReceivedQty = x.ReceivedQty,
                        Quantity = x.Quantity,
                        Cost = x.Cost,
                        PreviousCNumber = x.PreviousCNumber,
                        PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                        comment = x.Comment,
                        Status = x.Status,
                        DutyFreePaid = x.DutyFreePaid,
                        subject = x.Subject,
                        emailDate = x.EmailDate

                    })
                    .ToList();
            }
        }

        private static List<TODO_TotalAdjustmentsToProcess> GetDocSetDiscrepanciesToProcess(int docSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                return ctx.TODO_TotalAdjustmentsToProcess
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                && x.Type == "DIS"
                                && x.AsycudaDocumentSetId == docSetId
                    ).ToList();
            }
        }

        public static void CleanupDiscpancies()
        {
            try
            {
                Console.WriteLine("Clean up Discrepancies");

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var lst = ctx.TODO_DiscrepanciesAlreadyXMLed.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                            //    && x.AsycudaDocumentSetId == 1462
                            //     && x.InvoiceNo == "53371108"
                            //&& x.InvoiceDate >= saleInfo.Item1
                            //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        .Select(x => new { x.AsycudaDocumentSetId, x.EntryData_Id }).Distinct().ToList();

                    using (var etx = new EntryDataDSContext())
                    {
                        var res = etx.EntryDataDetailsEx
                            .Where(x => x.DoNotAllocate == true || x.IsReconciled == true)
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .Select(x => new {x.AsycudaDocumentSetId, x.EntryData_Id }).Distinct().ToList();

                        lst.AddRange(res);
                    }

                    using (var dtx = new DocumentDSContext())
                    {


                        foreach (var doc in lst)
                        {
                            var res = dtx.AsycudaDocumentSetEntryDatas
                                .FirstOrDefault(x =>
                                    x.AsycudaDocumentSet.SystemDocumentSet == null
                                    && x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId
                                    && x.EntryData_Id == doc.EntryData_Id);
                            if (res != null)
                                dtx.AsycudaDocumentSetEntryDatas.Remove(res);

                        }



                        dtx.SaveChanges();
                    }

                    var docsToDelete = ctx.TODO_DeleteDocumentSet.ToList();
                    foreach (var doc in docsToDelete)
                    {
                        BaseDataModel.Instance.DeleteAsycudaDocumentSet(doc.AsycudaDocumentSetId).Wait();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void CleanupDocSetDiscpancies(FileTypes fileType)
        {
            try
            {
                Console.WriteLine("Clean up Discrepancies");

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var disLst = ctx.TODO_DiscrepanciesAlreadyXMLed
                        .Where(x => //x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                            //     && x.InvoiceNo == "53371108"
                            //&& x.InvoiceDate >= saleInfo.Item1
                            //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        .GroupBy(x => new {x.AsycudaDocumentSetId, x.EntryData_Id, x.InvoiceNo})
                        .ToList();
                    foreach (var g in disLst)
                    {
                        var body = "Discrepancy Already Executed: \r\n" +
                                   "The following were already executed: \r\n" +
                                   $"\t{"Data Number".FormatedSpace(_databaseCommandTimeout)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                                   $"{g.Select(current => $"\t{current.ItemNumber.FormatedSpace(_databaseCommandTimeout)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.pCNumber.FormatedSpace(15)}{current.Comment.FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        EmailDownloader.EmailDownloader.SendBackMsg(fileType.EmailId, Utils.Client,
                            body);



                        var entryLst = new EntryDataDSContext().EntryDataDetailsEx
                            //.Where(x => x.DoNotAllocate == true || x.IsReconciled == true)
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                                        && x.EntryData_Id == g.Key.EntryData_Id)
                            .ToList();
                        if (entryLst.Count() != g.Count()) return;

                        using (var dtx = new DocumentDSContext())
                        {
                            var res = Queryable.FirstOrDefault(dtx.AsycudaDocumentSetEntryDatas, x =>
                                x.AsycudaDocumentSet.SystemDocumentSet == null
                                && x.AsycudaDocumentSetId == g.Key.AsycudaDocumentSetId
                                && x.EntryData_Id == g.Key.EntryData_Id);
                            if (res != null)
                                dtx.AsycudaDocumentSetEntryDatas.Remove(res);

                            dtx.SaveChanges();
                        }

                        //var docsToDelete = ctx.TODO_DeleteDocumentSet.ToList();
                        //foreach (var doc in docsToDelete)
                        //{

                        BaseDataModel.Instance.DeleteAsycudaDocumentSet(g.Key.AsycudaDocumentSetId).Wait();
                        // }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void RecreateDocSetDiscrepanciesEntries(FileTypes fileType)
        {
            ADJUtils.CreateAdjustmentEntries(true,"DIS", fileType);
        }

        public static void AssessDiscpancyEntries(FileTypes ft, FileInfo[] fs)
        {
            Console.WriteLine("Assess Discrepancy Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_AssessDiscrepancyEntries
                    .Where(x =>// x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId
                            && x.AdjustmentType == "DIS"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                    )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                foreach (var doc in lst)
                {
                    EntryDocSetUtils.AssessEntries(doc.Key.Declarant_Reference_Number, doc.Key.AsycudaDocumentSetId);
                }

            }

        }

       
        public static void AllocateDocSetDiscrepancies(FileTypes fileType)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox();

                Console.WriteLine("Allocate DocSet Discrepancies");


                var lst = GetDISList(fileType);

                if (!lst.Any()) return;



                AllocationsModel.Instance
                    .ClearDocSetAllocations(lst.Select(x => $"'{x.Value}'").Aggregate((o, n) => $"{o},{n}")).Wait();

                AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);

                
                new AdjustmentShortService().AutoMatchUtils
                    .AutoMatchDocSet(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        fileType.AsycudaDocumentSetId).Wait();

                new AdjustmentShortService().AutoMatchUtils
                    .ProcessDISErrorsForAllocation(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        lst.Select(x => $"{x.Key}-{x.Value}").Aggregate((o, n) => $"{o},{n}")).Wait();

               

                var shortlst = GetShorts(lst, fileType);
                if (string.IsNullOrEmpty(shortlst)) return;

                new AllocationsBaseModel()
                    .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false,
                        shortlst).Wait();

                new AllocationsBaseModel()
                    .MarkErrors(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, shortlst)
                    .Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static string GetShorts(List<KeyValuePair<int, string>> lst, FileTypes fileType)
        {
            var ualst = new AdjustmentQSContext()
                .AdjustmentDetails
                .Where(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                            && x.Type == "DIS" && !x.ShortAllocations.Any()).ToList();

            var shortlst = lst
                .Where(x => ualst.Any(z =>
                    z.EntryDataDetailsId == x.Key &&
                    z.InvoiceQty.GetValueOrDefault() > z.ReceivedQty.GetValueOrDefault()))
                .Select(x => $"{x.Key}-{x.Value}").DefaultIfEmpty("").Aggregate((o, n) => $"{o},{n}");
            return shortlst;
        }

        private static List<KeyValuePair<int, string>> GetDISList(FileTypes fileType)
        {
            List<KeyValuePair<int, string>> lst;
            var alst = GetAdjustmentDetailsForFileType(fileType);


            lst = GetDistinctKeyValuePairs(alst);

            var ids = lst.Select(x => x.Key).ToList();

            var itemEntryDataDetails = GetItemEntryDataDetails(ids, lst);
            ProcessExistingItems(fileType, itemEntryDataDetails);

            RemoveExistingItemsFromLst(lst, itemEntryDataDetails);


            return lst;
        }

        private static void RemoveExistingItemsFromLst(List<KeyValuePair<int, string>> lst, List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)> itemEntryDataDetails)
        {
            var alreadyExecuted = lst.Where(x => itemEntryDataDetails.Any(z => z.key.Key == x.Key)).ToList();

            foreach (var itm in alreadyExecuted)
            {
                lst.Remove(itm);
            }
        }

        private static void ProcessExistingItems(FileTypes fileType, List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)> itemEntryDataDetails)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var sourceFileLst = new List<string>();
                foreach (var itm in itemEntryDataDetails)
                {
                    var sourcefile = ctx.AsycudaDocuments.First(x => x.ASYCUDA_Id == itm.doc.Asycuda_id)
                        .SourceFileName;
                    if (ctx.AttachmentLog
                            .FirstOrDefault(x =>
                                x.AsycudaDocumentSet_Attachments.Attachments.FilePath == sourcefile &&
                                x.Status == "Submit XML To Customs") == null)
                    {
                        fileType.ProcessNextStep = null;
                        break;
                    }
                    else
                    {
                        if(sourceFileLst.Contains(sourcefile) && fileType.ProcessNextStep.Contains("ReSubmitDiscrepanciesToCustoms")) continue;
                        fileType.ProcessNextStep.Add("ReSubmitDiscrepanciesToCustoms");
                        fileType.ProcessNextStep.Add("Continue");
                        sourceFileLst.Add(sourcefile);
                    }
                }
            }
        }

        private static List<(KeyValuePair<int, string> key, AsycudaDocumentItemEntryDataDetails doc)> GetItemEntryDataDetails(List<int> ids, List<KeyValuePair<int, string>> lst)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = _databaseCommandTimeout;

                    var itemEntryDataDetails = ctx.AsycudaDocumentItemEntryDataDetails
                        .Where(x => x.ImportComplete == true && ids.Contains(x.EntryDataDetailsId))
                        .ToList()
                        .Join(lst, x => x.EntryDataDetailsId, z => z.Key, (x, z) => (key: z, doc: x))
                        .ToList();
                    return itemEntryDataDetails;
                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        private static List<KeyValuePair<int, string>> GetDistinctKeyValuePairs(List<AdjustmentDetail> alst)
        {
            List<KeyValuePair<int, string>> lst;
            lst = alst
                .Select(x => new { x.EntryDataDetailsId, x.ItemNumber })
                .Distinct()
                .ToList()
                .Select(x => new KeyValuePair<int, string>(x.EntryDataDetailsId, x.ItemNumber))
                .ToList();
            return lst;
        }

        private static List<AdjustmentDetail> GetAdjustmentDetailsForFileType(FileTypes fileType)
        {
            var alst = new AdjustmentQSContext()
                .AdjustmentDetails
                .Where(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                            && x.Type == "DIS").ToList();
            return alst;
        }

        public static void ExportDiscpancyEntries(string adjustmentType)
        {
            Console.WriteLine($"Export Last {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var lst = ctx.TODO_DiscrepanciesToAssess.AsNoTracking()
                        .Where(x => x.Type == adjustmentType
                            //&& x.InvoiceDate >= saleInfo.Item1
                            //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )
                        .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                    foreach (var doc in lst)
                    {

                        string directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Key.Declarant_Reference_Number);
                        SalesUtils.ExportLastDocSetSalesReport(doc.Key.AsycudaDocumentSetId,
                            directoryName).Wait();
                        BaseDataModel.Instance.ExportLastDocumentInDocSet(doc.Key.AsycudaDocumentSetId,
                            directoryName, true).Wait();


                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ExportDocSetDiscpancyEntries(string adjustmentType, FileTypes fileType)
        {
            Console.WriteLine($"Export Last {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {



                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var lst = ctx.TODO_DiscrepanciesToAssess.AsNoTracking()
                        .Where(x => x.Type == adjustmentType
                                    && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )
                        .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                    foreach (var doc in lst)
                    {
                        var docSetEx = ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == doc.Key.AsycudaDocumentSetId);
                        //if (docSetEx.ClassifiedLines != docSetEx.TotalLines) continue;

                        BaseDataModel.Instance.ExportLastDocumentInDocSet(doc.Key.AsycudaDocumentSetId,
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                doc.Key.Declarant_Reference_Number), true).Wait();
                        SalesUtils.ExportLastDocSetSalesReport(doc.Key.AsycudaDocumentSetId,
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                doc.Key.Declarant_Reference_Number)).Wait();

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static void AutoMatch()
        {
            Console.WriteLine("AutoMatch ...");
            new AdjustmentShortService().AutoMatch(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, true).Wait();
        }

        public static void AssessDiscpancyEntries()
        {
            Console.WriteLine("Assess Discrepancy Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_CreateDiscrepancyEntries.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                            && x.AdjustmentType == "DIS"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                    )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                foreach (var doc in lst)
                {

                    POUtils.AssessPOEntry(doc.Key.Declarant_Reference_Number, doc.Key.AsycudaDocumentSetId);
                }

            }

        }
        private static AsycudaDocumentSetEx GetAsycudaDocumentSetEx(FileTypes fileType)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                return ctx.AsycudaDocumentSetExs.FirstOrDefault(x =>
                    x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
            }
        }
    }

       
}