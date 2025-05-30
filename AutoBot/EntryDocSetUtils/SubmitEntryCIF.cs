using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
// using TrackableEntities; // Removed duplicate using
using CoreAsycudaDocumentSet = CoreEntities.Business.Entities.AsycudaDocumentSet; // Alias for CoreEntities version
using CoreConsignees = CoreEntities.Business.Entities.Consignees; // Alias for CoreEntities version
using AutoBot.Services;

namespace AutoBot
{
    public partial class EntryDocSetUtils
    {
        public static void SubmitEntryCIF(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                Console.WriteLine("Submit Shipment CIF");

                //   var saleInfo = CurrentSalesInfo();

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .Select(x => x.EmailAddress)
                            .Distinct()
                            .ToArray();
                    var lst = ctx.TODO_SubmitEntryCIF
                            // .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                            .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                            .ToList();
                    if (!lst.Any()) return;
                    if (Enumerable.Any<ActionDocSetLogs>(ErrorReportingService.GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitEntryCIF"))) return;

                    var body = $"Please see attached CIF for Shipment: {lst.First().Declarant_Reference_Number} . \r\n" +
                                        $"Please double check against your shipment rider.\r\n" +
                                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                        $"\r\n" +
                                        $"Regards,\r\n" +
                                        $"AutoBot";
                    List<string> attlst = new List<string>();
                    var poInfo = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(POUtils.CurrentPOInfo(ft.AsycudaDocumentSetId));

                    var summaryFile = Path.Combine(poInfo.Item2, "CIFValues.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                            new ExportToCSV<TODO_SubmitEntryCIF, List<TODO_SubmitEntryCIF>>()
                            {
                                dataToPrint = lst
                            };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                    }

                    attlst.Add(summaryFile);

                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", $"CIF Values for Shipment: {lst.First().Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());

                    LogDocSetAction(ft.AsycudaDocumentSetId, "SubmitEntryCIF");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}