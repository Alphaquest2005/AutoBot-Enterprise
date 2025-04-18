using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_LicenseToXML, Contacts are here
using MoreLinq; // For DistinctBy
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static void SubmitBlankLicenses(FileTypes ft)
        {
            try
            {
                var info = BaseDataModel.CurrentSalesInfo(-1); // Assuming CurrentSalesInfo exists
                var directory = info.Item4; // Potential NullReferenceException if info is null

                using (var ctx = new CoreEntitiesContext())
                {
                    var llst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_LicenseToXML>(
                            $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {ft.AsycudaDocumentSetId}").ToList();

                    var emails = llst
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId )//
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId && x.EmailId != null)//ft.AsycudaDocumentSetId == 0 ||
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {
                        //if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue; // Assuming GetDocSetActions exists

                        var errorfile = Path.Combine(directory, $"BlankLicenseDescription-{email.Key.AsycudaDocumentSetId}.csv");
                        // Assuming Utils.BlankLicenseDescription exists
                        var errors = email.Select(x => new Utils.BlankLicenseDescription()
                        {
                            InvoiceNo = x.EntryDataId,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode,
                            LicenseDescription = x.LicenseDescription
                        }).ToList();

                        var res =
                            new ExportToCSV<Utils.BlankLicenseDescription, List<Utils.BlankLicenseDescription>>()
                            {
                                dataToPrint = errors
                            };
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = MoreEnumerable.DistinctBy(ctx.Contacts.Where(x => x.Role == "Broker")
                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId), x => x.EmailAddress).ToList();
                        if (File.Exists(errorfile))
                            // Assuming Utils.Client exists
                            EmailDownloader.EmailDownloader.ForwardMsg(email.Key.EmailId, Utils.Client,
                                $"Error:Blank License Description",
                                "Please Fill out the attached License Description and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

                        // LogDocSetAction(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems"); // Assuming LogDocSetAction exists
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