using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class EmailSalesErrorsUtils
    {
        public static void EmailSalesErrors()
        {
            var info = BaseDataModel.CurrentSalesInfo(-1);
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "SalesErrors.csv");
            if (!File.Exists(errorfile)) return;
            var errors = GetSalesReportData(info);
            CreateSalesReport(errors, errorfile);
            var contactsLst = GetContactsList();
            SendSalesReport(errorfile, directory, info, contactsLst);
        }

        private static void CreateSalesReport(List<AsycudaSalesAndAdjustmentAllocationsEx> errors, string errorfile)
        {
            var res =
                new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx,
                    List<AsycudaSalesAndAdjustmentAllocationsEx>>
                {
                    dataToPrint = errors
                };
            using (var sta = new StaTaskScheduler(1))
            {
                Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                    TaskCreationOptions.None, sta);
            }
        }

        private static List<AsycudaSalesAndAdjustmentAllocationsEx> GetSalesReportData(
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) info) =>
            new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Where(x => x.Status != null)
                .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

        private static void SendSalesReport(string errorfile, string directory,
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) info, List<Contacts> contactsLst)
        {
            
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                        $"Sales Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}",
                        contactsLst.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new[]
                        {
                            errorfile
                        });
        }

        private static List<Contacts> GetContactsList()
        {
            List<Contacts> contacts;
            using (var ctx = new CoreEntitiesContext())
            {
                contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
            }

            return contacts;
        }
    }
}