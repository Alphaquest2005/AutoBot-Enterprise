using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters; // Assuming ExportToCSV is here
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_PODocSetToAssessErrors, AsycudaDocumentSetExs, Contacts are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class POUtils
    {
        private static void SubmitAssessPOErrors(FileTypes ft)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = ctx.TODO_PODocSetToAssessErrors
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList();
                    if (!res.Any()) return;
                    Console.WriteLine("Emailing Assessment Errors - please check Mail");
                    var docSet =
                        ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var body =
                        $"Please see attached documents .\r\n" +
                        $"Please open the attached email to view Email Thread.\r\n" +
                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                        $"\r\n" +
                        $"Regards,\r\n" +
                        $"AutoBot";


                    var directory = BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number);

                    var summaryFile = Path.Combine(directory, "POAssesErrors.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<TODO_PODocSetToAssessErrors, List<TODO_PODocSetToAssessErrors>>()
                        {
                            dataToPrint = res
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    // Assuming Utils.Client exists and is accessible
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                        $"PO Assessment Errors for Shipment: {docSet.Declarant_Reference_Number}",
                        poContacts, body, new string[] { summaryFile });
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