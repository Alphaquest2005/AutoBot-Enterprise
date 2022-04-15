using System;
using System.Collections.Generic;
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
using WaterNut.DataSpace;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class DISUtils
    {
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
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 20;

                IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
                lst = ctx.TODO_SubmitDiscrepanciesToCustoms.Where(x => x.EmailId == ft.EmailId
                                                                       && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )

                    .ToList()

                    .GroupBy(x => x.EmailId);
                SubmitDiscrepanciesToCustoms(lst);

            }
        }

        public static void SubmitDiscrepanciesToCustoms()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 20;
               
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


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 20;
                    var cnumberList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();
                    var cplst = BaseDataModel.Instance.Customs_Procedures
                        .Where(x => x.CustomsOperation.Name == "Exwarehouse").Select(x => x.CustomsProcedure).ToList();
                    IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
                    List<TODO_SubmitAllXMLToCustoms> res;
                    if (cnumberList.Any())
                    {
                        res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId && cnumberList.Contains(x.CNumber) && cplst.Any(z => z == x.CustomsProcedure))
                            .ToList();
                    }
                    else
                    {
                        var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(ft.AsycudaDocumentSetId).Result;
                        res = ctx.TODO_SubmitAllXMLToCustoms.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId &&
                                x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number) && cplst.Any(z => z == x.CustomsProcedure))
                            .ToList();
                    }

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();
                   
                    foreach (var data in lst)
                    {
                        var emailIds = MoreEnumerable.DistinctBy(data, x => x.CNumber).ToList();
                        if(!emailIds.Any())
                        {
                            throw new NotImplementedException();
                        }
                        var pdfs = new List<string>();
                        foreach (var itm in emailIds)
                        {
                            
                            var res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                            if (!res.Any())
                            {
                                BaseDataModel.LinkPDFs( new List<int>() {itm.ASYCUDA_Id});
                                res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath).ToArray();
                            }
                            pdfs.AddRange(res);
                        }

                        if (pdfs.Count == 0) continue;

                        var body = "The Following Discrepancies Entries were Assessed. \r\n" +

                                   $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"To Be Paid".FormatedSpace(20)}{"AssessmentDate".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.ReferenceNumber.FormatedSpace(20)}{current.ToBePaid.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"\r\n" +
                                   $"Please open the attached email to view Email Thread.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";


                        var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSetEx, string>>(POUtils.CurrentPOInfo(emailIds.First().AsycudaDocumentSetId));
                        var directory = info.Item2;

                        var summaryFile = Path.Combine(directory, $"Summary.csv");

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
                                xRegistrationDate = x.xRegistrationDate

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
                        pdfs.Add(executionFile);

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
                        pdfs.Add(summaryFile);

                        string reference = emailIds.First().ReferenceNumber.Substring(0, emailIds.First().ReferenceNumber.IndexOf('-'));

                        if (data.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", $"Assessed Shipping Discrepancy Entries: {reference}",
                                contacts, body, pdfs.ToArray());
                        }
                        else
                        {
                            
                            EmailDownloader.EmailDownloader.ForwardMsg(data.Key, Utils.Client, $"Assessed Shipping Discrepancy Entries: {reference}", body, contacts, pdfs.ToArray());
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
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void SubmitDiscrepanciesPreAssessmentReportToCustoms()
        {
            try
            {


                Console.WriteLine("Submit Discrepancies PreAssessment Report to Customs");


                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs" || x.Role == "Clerk")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                        .ApplicationSettingsId && x.Type == "DIS").ToList();
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
                    var attachments = new List<string>();

                    var errBreakdown = errors.GroupBy(x => x.Status).ToList();

                    if (errors.Any())
                    {

                        //foreach (var emailIds in lst)
                        //{



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
                    if (attachments.Any())
                    {
                        var body =
                            $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                            $"\r\n" +
                            $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(20)}{"Percentage".FormatedSpace(20)}\r\n" +
                            $"{string.Join(",", errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(20)}{(Math.Round((double)(((double)current.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n"))}" +
                            $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(20)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n" +
                            $"\r\n" +
                            $"Please see attached for list of Errors and Executions details.\r\n" +
                            $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                            $"\r\n" +
                            $"Regards,\r\n" +
                            $"AutoBot";
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                            $"Discrepancy Pre-Assessment Report for  {totaladjustments.Min(x => x.EffectiveDate)} To: {totaladjustments.Max(x => x.EffectiveDate)}",
                            contacts.ToArray(), body, attachments.ToArray());
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
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


                

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 20;
                    var docset = ctx.AsycudaDocumentSetExs.FirstOrDefault(x =>
                        x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                    if (docset == null) return;

                    var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSetEx, string>>(POUtils.CurrentPOInfo(fileType.AsycudaDocumentSetId));
                    var directory = info.Item2;

                    var contacts = ctx.Contacts.Where(x => x.Role == "Broker" || x.Role == "Clerk" || x.Role == "Customs")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var totaladjustments = ctx.TODO_TotalAdjustmentsToProcess
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId 
                                    && x.Type == "DIS"
                                    && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                        ).ToList();
                    var errors = ctx.TODO_DiscrepanciesErrors.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                            && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
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

                    var goodadj = ctx.TODO_DiscrepancyPreExecutionReport.Where(x =>
                            x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                            && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)

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
                            emailDate = x.EmailDate??DateTime.MinValue,
                            Reference = x.Reference,
                            DocumentType = x.DocumentType,

                        })
                        .ToList();
                    var attachments = new List<string>();

                    var errBreakdown = errors.GroupBy(x => x.Status).ToList();

                    if (errors.Any())
                    {

                        //foreach (var emailIds in lst)
                        //{



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
                    if (attachments.Any())
                    {
                        var errorBody = errors.Any()
                            ? "Discrepancies Found: \r\n" +
                              "System could not Generate Entries the following items on the CNumbers Stated: \r\n" +
                              $"\t{"Item Number".FormatedSpace(20)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                              $"{errors.Select(current => $"\t{current.ItemNumber.FormatedSpace(20)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.PreviousCNumber.FormatedSpace(15)}{(current.Status+ " | "+current.comment).FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
                              "\r\n" +
                              "\r\n" +
                              " The Attached File contains details of errors found trying to assessed the attached email's Shipping Discrepancies \r\n"
                            : "";

                        var body =
                            $"For the Effective Period From: {totaladjustments.Min(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")} To: {totaladjustments.Max(x => x.EffectiveDate)?.ToString("yyyy-MM-dd")}. \r\n" +
                            $"\r\n" +
                            $"\t{"Reason".FormatedSpace(40)}{"Count".FormatedSpace(20)}{"Percentage".FormatedSpace(20)}\r\n" +
                            $"{string.Join(",", errBreakdown.Select(current => $"\t{current.Key.FormatedSpace(40)}{current.Count().ToString().FormatedSpace(20)}{(Math.Round((double)(((double)current.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n"))}" +
                            $"\t{"Executions".FormatedSpace(40)}{goodadj.Count.ToString().FormatedSpace(20)}{(Math.Round((double)(((double)goodadj.Count() / (double)totaladjustments.Count()) * 100), 0)).ToString().FormatedSpace(20)}% \r\n" +
                            $"\r\n" +
                            $"{errorBody}"+
                            $"Please see attached for list of Errors and Executions details.\r\n" +
                            $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                            $"\r\n" +
                            $"Regards,\r\n" +
                            $"AutoBot";

                        
                        EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId, Utils.Client, $"Discrepancy Pre-Assessment Report for  {docset.Declarant_Reference_Number}", body, contacts, attachments.ToArray());


                        //EmailDownloader.EmailDownloader.SendEmail(Client, directory,
                        //        $"Discrepancy Pre-Assessment Report for  {docset.Declarant_Reference_Number}",
                        //        contacts.ToArray(), body, attachments.ToArray());
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
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
                                   $"\t{"Item Number".FormatedSpace(20)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                                   $"{g.Select(current => $"\t{current.ItemNumber.FormatedSpace(20)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.pCNumber.FormatedSpace(15)}{current.Comment.FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
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
            Utils.CreateAdjustmentEntries(true,"DIS", fileType);
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

        public static void SubmitDocSetDiscrepancyErrors(FileTypes fileType)
        {
            try
            {

                return; // combined into pre assessment report email
                //Console.WriteLine("Submit Discrepancy Entries");

                //// var saleInfo = CurrentSalesInfo();


                //using (var ctx = new CoreEntitiesContext())
                //{
                //    ctx.Database.CommandTimeout = 20;
                //    var lst = ctx.TODO_DiscrepanciesErrors
                //        .Where(x =>//x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                //                                                      x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                //                                                      && x.Type == "DIS").ToList()
                //        .GroupBy(x => new { x.AsycudaDocumentSetId, x.EmailId });
                //    foreach (var doc in lst)
                //    {
                //        var emailIds = ctx.AsycudaDocumentSet_Attachments
                //            .Where(x => x.AsycudaDocumentSetId == doc.Key.AsycudaDocumentSetId &&
                //                        x.Attachments.FilePath.EndsWith(".xlsx"))
                //            .Where(x => x.AttachmentLog.All(z => z.Status != "Submit Discrepancy Errors"))
                //            .ToList();
                //        foreach (var eId in emailIds)
                //        {

                //            var body = "Discrepancies Found: \r\n" +
                //                       "System could not Generate Entries the following items on the CNumbers Stated: \r\n" +
                //                       $"\t{"Item Number".FormatedSpace(20)}{"InvoiceQty".FormatedSpace(15)}{"Recieved Qty".FormatedSpace(15)}{"pCNumber".FormatedSpace(15)}{"Reason".FormatedSpace(30)}\r\n" +
                //                       $"{doc.Select(current => $"\t{current.ItemNumber.FormatedSpace(20)}{current.InvoiceQty.ToString().FormatedSpace(15)}{current.ReceivedQty.ToString().FormatedSpace(15)}{current.PreviousCNumber.FormatedSpace(15)}{current.Status.FormatedSpace(30)}\r\n").Aggregate((old, current) => old + current)}" +
                //                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                //                       $"Regards,\r\n" +
                //                       $"AutoBot";
                //            if (!EmailDownloader.EmailDownloader.SendBackMsg(eId.EmailId, Client,
                //                body)) continue;
                //            ctx.AttachmentLog.Add(new AttachmentLog(true)
                //            {
                //                DocSetAttachment = eId.Id,
                //                Status = "Submit Discrepancy Errors",
                //                TrackingState = TrackingState.Added
                //            });
                //            ctx.SaveChanges();


                //        }


                //    }

                //}
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public static void AllocateDocSetDiscrepancies(FileTypes fileType)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox();

                Console.WriteLine("Allocate DocSet Discrepancies");
                List<KeyValuePair<int, string>> lst;



                var alst = new AdjustmentQSContext()
                    .AdjustmentDetails
                    .Where(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                                && x.Type == "DIS").ToList();
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 20;


                    lst = alst
                        .Select(x => new { x.EntryDataDetailsId, x.ItemNumber })
                        .Distinct()
                        .ToList()
                        .Select(x => new KeyValuePair<int, string>(x.EntryDataDetailsId, x.ItemNumber))
                        .ToList();
                    var ids = lst.Select(x => x.Key).ToList();
                    var itemEntryDataDetails = ctx.AsycudaDocumentItemEntryDataDetails
                        .Where(x => x.ImportComplete == true && ids.Contains(x.EntryDataDetailsId))
                        .ToList()
                        .Join(lst, x => x.EntryDataDetailsId, z => z.Key, (x, z) => new { key = z, doc = x })
                        .ToList();
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
                            fileType.ProcessNextStep.Add("ReSubmitDiscrepanciesToCustoms");
                            continue;
                        }



                    }

                    var alreadyExecuted = lst.Where(x => itemEntryDataDetails.Any(z => z.key.Key == x.Key)).ToList();
                    foreach (var itm in alreadyExecuted)
                    {
                        lst.Remove(itm);
                    }


                }

                if (!lst.Any()) return;



                AllocationsModel.Instance
                    .ClearDocSetAllocations(lst.Select(x => $"'{x.Value}'").Aggregate((o, n) => $"{o},{n}")).Wait();

                AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);






                new AdjustmentShortService()
                    .AutoMatchDocSet(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        fileType.AsycudaDocumentSetId).Wait();

                new AdjustmentShortService()
                    .ProcessDISErrorsForAllocation(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        lst.Select(x => $"{x.Key}-{x.Value}").Aggregate((o, n) => $"{o},{n}")).Wait();

                var ualst = new AdjustmentQSContext()
                    .AdjustmentDetails
                    .Where(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId
                                && x.Type == "DIS" && !x.ShortAllocations.Any()).ToList();

                var shortlst = lst
                    .Where(x => ualst.Any(z =>
                        z.EntryDataDetailsId == x.Key &&
                        z.InvoiceQty.GetValueOrDefault() > z.ReceivedQty.GetValueOrDefault()))
                    .Select(x => $"{x.Key}-{x.Value}").DefaultIfEmpty("").Aggregate((o, n) => $"{o},{n}");
                if (!string.IsNullOrEmpty(shortlst))
                {
                    new AllocationsBaseModel()
                        .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false,
                            shortlst).Wait();

                    new AllocationsBaseModel()
                        .MarkErrors(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, shortlst)
                        .Wait();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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
                        if (docSetEx.ClassifiedLines != docSetEx.TotalLines) continue;

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
    }
}