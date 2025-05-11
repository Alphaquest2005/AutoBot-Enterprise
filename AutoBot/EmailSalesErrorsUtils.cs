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
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace AutoBot
{
    public class EmailSalesErrorsUtils
    {
        public static async Task EmailSalesErrors()
        {
            var infoTuple = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
            var directory = infoTuple.DirPath; // Or infoTuple.Item4 if you prefer direct item access
            var errorfile = Path.Combine(directory, "SalesErrors.csv");
            if (!File.Exists(errorfile)) return;
            var errors = GetSalesReportData(infoTuple);
            CreateSalesReport(errors, errorfile);
            var contactsLst = GetContactsList();
            await SendSalesReport(errorfile, directory, infoTuple, contactsLst).ConfigureAwait(false);
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
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) infoTuple) =>
            new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Where(x => x.Status != null)
                .Where(x => x.InvoiceDate >= infoTuple.StartDate.Date && x.InvoiceDate <= infoTuple.EndDate.Date).ToList();

        private static async Task SendSalesReport(string errorfile, string directory,
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) infoTuple, List<Contacts> contactsLst)
        {
            
                    await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory,
                        $"Sales Errors for {infoTuple.StartDate.ToString("yyyy-MM-dd")} - {infoTuple.EndDate.ToString("yyyy-MM-dd")}",
                        contactsLst.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new[]
                        {
                            errorfile
                        }).ConfigureAwait(false);
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