using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.DataSpace;

namespace AutoBot
{
    using Serilog;

    public class SubmitSalesToCustomsUtils
    {
        public static async Task SubmitSalesToCustoms(ILogger log) => await SubmitSalesToCustoms(GetSalesDataToSubmit(), log).ConfigureAwait(false);

        private static IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> GetSalesDataToSubmit()
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 20;

                    return ctx.TODO_SubmitSalesToCustoms.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .ToList()
                        .Select(x => new TODO_SubmitDiscrepanciesToCustoms
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task ReSubmitSalesToCustoms(FileTypes ft, FileInfo[] fs, ILogger log) => await SubmitSalesToCustoms(await DISUtils.GetSubmitEntryData(ft).ConfigureAwait(false), log).ConfigureAwait(false);

        private static async Task SubmitSalesToCustoms(
            IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst,
            ILogger log)
        {
            try
            {
                Console.WriteLine("Submit Sales To Customs");

                // var saleInfo = CurrentSalesInfo();
                
                    var contacts = GetContacts();

                   
                    var RES = lst.SelectMany(x => x).DistinctBy(x => x.ASYCUDA_Id);

                    if (!RES.Any())
                    {
                        Console.WriteLine("No Sales Found!");
                        return;
                    }

                    var pdfs = GetAttachments(RES);

                    var body = CreateEmailBody(RES);


                    var summaryFile = CreateSummaryFile(RES);

                 
                   await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "", "Assessed Ex-Warehoused Entries",
                       contacts, body, pdfs.ToArray(), log).ConfigureAwait(false);
              
                    UpdateAttachmentLog(RES);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void UpdateAttachmentLog(IEnumerable<TODO_SubmitDiscrepanciesToCustoms> RES)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;

                foreach (var item in RES)
                {
                    var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                        x.ASYCUDA_Id == item.ASYCUDA_Id &&
                        x.ApplicationSettingsId == item.ApplicationSettingsId);
                    var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                        x.Attachments.FilePath == sfile.SourceFileName);
                    if (eAtt != null)
                        ctx.AttachmentLog.Add(new AttachmentLog(true)
                        {
                            DocSetAttachment = eAtt.Id,
                            Status = "Submit Sales To Customs",
                            TrackingState = TrackingState.Added
                        });
                }

                ctx.SaveChanges();
            }
        }

        private static string CreateSummaryFile(IEnumerable<TODO_SubmitDiscrepanciesToCustoms> RES)
        {
            var info = POUtils.CurrentPOInfo(RES.First().AsycudaDocumentSetId).FirstOrDefault();
            var directory = info.Item2;

            var summaryFile = Path.Combine(directory, "SalesSummary.csv");


            var sumres =
                new ExportToCSV<TODO_SubmitDiscrepanciesToCustoms, List<TODO_SubmitDiscrepanciesToCustoms>>
                {
                    dataToPrint = RES.ToList()
                };
            using (var sta = new StaTaskScheduler(1))
            {
                Task.Factory.StartNew(() => sumres.SaveReport(summaryFile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }

            return summaryFile;
        }

        private static string CreateEmailBody(IEnumerable<TODO_SubmitDiscrepanciesToCustoms> RES) =>
            "The Following Sales Entries were Assessed. \r\n" +
            $"\t{"pCNumber".FormatedSpace(20)}{"Reference".FormatedSpace(20)}{"AssessmentDate".FormatedSpace(20)}\r\n" +
            $"{RES.Select(current => $"\t{current.CNumber.FormatedSpace(20)}{current.ReferenceNumber.FormatedSpace(20)}{current.RegistrationDate.Value.ToString("yyyy-MM-dd").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
            "\r\n" +
            "Please open the attached email to view Email Thread.\r\n" +
            "Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
            "\r\n" +
            "Regards,\r\n" +
            "AutoBot";

        private static List<string> GetAttachments(IEnumerable<TODO_SubmitDiscrepanciesToCustoms> RES)
        {
            
            var pdfs = new List<string>();
            foreach (var itm in RES)
            {
                BaseDataModel.LinkPDFs(new List<int> { itm.ASYCUDA_Id });

                var res = new CoreEntitiesContext().AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                    .Select(x => x.Attachments.FilePath).ToArray();
               
                pdfs.AddRange(res);
            }

            return pdfs;
        }

        private static string[] GetContacts() =>
            new CoreEntitiesContext().Contacts
                .Where(x => x.Role == "Customs" || x.Role == "Clerk")
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Select(x => x.EmailAddress)
                .Distinct()
                .ToArray();

        public static async Task ReSubmitSalesToCustoms(ILogger log) => await SubmitSalesToCustoms(await DISUtils.GetSubmitEntryData(log).ConfigureAwait(false), log).ConfigureAwait(false);
    }
}