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
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using SalesDataQS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;

namespace AutoBot
{
    using Serilog;

    public class SalesUtils
    {
        public static async Task ClearAllocations()
        {
            await AllocationsModel.Instance
                .ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);
        }

        public static async Task SubmitUnknownDFPComments(ILogger log)
        {
            try
            {
                return;
                /// Fix up this
                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages.Where(x =>
                        x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                            .ApplicationSettingsId).ToList();

                    foreach (var docSet in lst)
                    {
                        var body = "Error: UnknownDFP Comments\r\n" +
                                   "\r\n" +
                                   "The Following Comments Classification(DutyFree Or Duty Paid) are Unknown.\r\n" +
                                   $"{docSet.MaxEntryLines}.\r\n" +
                                   "Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   "\r\n" +
                                   "Regards,\r\n" +
                                   "AutoBot";
                        var attlst = new List<string>();


                        await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "",
                            $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray(), log).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Task ReDownloadSalesFiles(ILogger log) => EX9Utils.DownloadSalesFiles(log,100, "IM7History", true);

        public static Task RebuildSalesReport() => BuildSalesReportClass.Instance.ReBuildSalesReports();

        public static async Task<List<AsycudaDocument>> GetSalesDocumentsWithEntryData(int asycudaDocumentSetId) =>
            (await BaseDataModel.Instance.GetDocSetWithEntryDataDocs(asycudaDocumentSetId).ConfigureAwait(false)).Documents.Select<xcuda_ASYCUDA, AsycudaDocument>(x => new SalesDataService().GetSalesDocument(x.ASYCUDA_Id).ConfigureAwait(false).GetAwaiter().GetResult()).ToList<AsycudaDocument>();

        public static async Task<IEnumerable<EX9Utils.SaleReportLine>> GetDocumentSalesReport(int ASYCUDA_Id)
        {
            var alst = await Ex9SalesReportUtils.Ex9SalesReport(ASYCUDA_Id).ConfigureAwait(false);
            if (alst.Any()) return alst;
            alst = ADJUtils.IM9AdjustmentsReport(ASYCUDA_Id);
            return alst;
        }
    }
}