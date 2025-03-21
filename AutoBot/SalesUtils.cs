﻿using System;
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
    public class SalesUtils
    {
        public static void ClearAllocations()
        {
            AllocationsModel.Instance
                .ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
        }

        public static void SubmitUnknownDFPComments()
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


                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void ReDownloadSalesFiles() => EX9Utils.DownloadSalesFiles(100, "IM7History", true);

        public static void RebuildSalesReport() => BuildSalesReportClass.Instance.ReBuildSalesReports();

        public static async Task<List<AsycudaDocument>> GetSalesDocumentsWithEntryData(int asycudaDocumentSetId) =>
            (await BaseDataModel.Instance.GetDocSetWithEntryDataDocs(asycudaDocumentSetId).ConfigureAwait(false)).Documents.Select(x => new SalesDataService().GetSalesDocument(x.ASYCUDA_Id).Result).ToList();

        public static async Task<IEnumerable<EX9Utils.SaleReportLine>> GetDocumentSalesReport(int ASYCUDA_Id)
        {
            var alst = await Ex9SalesReportUtils.Ex9SalesReport(ASYCUDA_Id).ConfigureAwait(false);
            if (alst.Any()) return alst;
            alst = ADJUtils.IM9AdjustmentsReport(ASYCUDA_Id);
            return alst;
        }
    }
}