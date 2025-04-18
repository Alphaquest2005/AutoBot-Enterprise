using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_SubmitUnclassifiedItems, Contacts, ActionDocSetLogs are here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here
using AutoBot.Services; // Assuming ErrorReportingService is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void SubmitDocSetUnclassifiedItems(FileTypes fileType)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var emails = ctx.TODO_SubmitUnclassifiedItems
                        .Where(x => //x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.EmailId != null
                            && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {
                        // Assuming ErrorReportingService.GetDocSetActions exists
                        // if (ErrorReportingService.GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;

                        // Assuming POUtils.CurrentPOInfo exists
                        var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(POUtils.CurrentPOInfo(email.Key.AsycudaDocumentSetId));
                        var directory = info.Item2;

                        var errorfile = Path.Combine(directory, $"UnclassifiedItems-{email.Key.AsycudaDocumentSetId}.csv");
                        // Assuming Utils.UnClassifiedItem exists
                        var errors = email.Select(x => new Utils.UnClassifiedItem()
                        {
                            InvoiceNo = x.InvoiceNo,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode
                        }).ToList();

                        // Assuming ExportToCSV exists and works this way
                        var res =
                            new ExportToCSV<Utils.UnClassifiedItem,
                                List<Utils.UnClassifiedItem>>()
                            {
                                dataToPrint = errors
                            };
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = ctx.Contacts
                            .Where(x => x.Role == "Broker")
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();
                        if (File.Exists(errorfile))
                            // Assuming Utils.Client exists
                            EmailDownloader.EmailDownloader.ForwardMsg(email.Key.EmailId, Utils.Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

                        // Assuming ErrorReportingService.LogDocSetAction exists
                        // ErrorReportingService.LogDocSetAction(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}