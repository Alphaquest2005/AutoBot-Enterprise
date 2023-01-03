using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class SubmitSalesXmlToCustomsUtils
    {
        public static void SubmitSalesXMLToCustoms()
        {
            try
            {
                Console.WriteLine("Submit XML To Customs");

                // var saleInfo = CurrentSalesInfo();
                var previousMonth = -1;
                var salesinfo = BaseDataModel.CurrentSalesInfo(previousMonth);

                GetSalesXmls(salesinfo).ForEach(emailIds =>  ProcessSalesData(salesinfo, emailIds));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void ProcessSalesData(
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) salesinfo, IGrouping<string, TODO_SubmitXMLToCustoms> emailIds)
        {
            var body = CreateEmailBody(salesinfo, emailIds);
            var attachments = GetAttachments(emailIds);
            SendEmails(emailIds, GetContacts(), body, attachments);
            UpdateEmailLog(emailIds);
        }

        private static string CreateEmailBody(
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) salesinfo, IGrouping<string, TODO_SubmitXMLToCustoms> emailIds) =>
            "The Following Ex-Warehouse Entries were Assessed. \r\n" +
            $"Start Date: {salesinfo.Item1} End Date {salesinfo.Item2}: \r\n" +
            "\tCNumber\t\tReference\t\tAssessmentDate\r\n" +
            $"{emailIds.Select(current => $"\t{current.CNumber}\t\t{current.ReferenceNumber}\t\t{current.RegistrationDate.Value:yyyy-MM-dd} \r\n").Aggregate((old, current) => old + current)}" +
            "\r\n" +
            "Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
            "\r\n" +
            "Regards,\r\n" +
            "AutoBot";

        private static void UpdateEmailLog(IGrouping<string, TODO_SubmitXMLToCustoms> emailIds) => emailIds.ForEach(SaveEmailLog);

        private static void SaveEmailLog(TODO_SubmitXMLToCustoms item)
        {
            using (var ctx = new CoreEntitiesContext())
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
                        Status = "Submit XML To Customs",
                        TrackingState = TrackingState.Added
                    });

                ctx.SaveChanges();
            }
        }


        private static void SendEmails(IGrouping<string, TODO_SubmitXMLToCustoms> emailIds, string[] contacts, string body, List<string> attachments)
        {
            if (emailIds.Key == null)
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", "Assessed Ex-Warehouse Entries",
                    contacts, body, attachments.ToArray());
            else
                EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key, Utils.Client,
                    "Assessed Ex-Warehouse Entries", body, contacts, attachments.ToArray());
        }

        private static List<string> GetAttachments(IGrouping<string, TODO_SubmitXMLToCustoms> emailIds) => emailIds.SelectMany(GetAttachment).ToList();

        private static List<string> GetAttachment(TODO_SubmitXMLToCustoms itm) =>
            new CoreEntitiesContext().AsycudaDocument_Attachments
                .Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id).Select(x => x.Attachments.FilePath)
                .ToList();

        private static IEnumerable<IGrouping<string, TODO_SubmitXMLToCustoms>> GetSalesXmls(
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) salesinfo)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 60;
                var lst = ctx.TODO_SubmitXMLToCustoms.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId
                        && x.AssessmentDate >= salesinfo.Item1 && x.AssessmentDate <= salesinfo.Item2)
                    .ToList()
                    .Where(x => x.ReferenceNumber.Contains(salesinfo.Item3.Declarant_Reference_Number))
                    .ToList()
                    .GroupBy(x => x.EmailId);
                return lst;
            }
        }

        private static string[] GetContacts() =>
            new CoreEntitiesContext().Contacts.Where(x => x.Role == "Customs")
                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Select(x => x.EmailAddress).ToArray();
    }
}