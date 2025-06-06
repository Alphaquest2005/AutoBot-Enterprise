using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities; // Assuming ExportToCSV is here
using Core.Common.Converters; // Assuming ExportToCSV might be here too?
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_SubmitIncompleteSupplierInfo, Contacts, ActionDocSetLogs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here
using AutoBot.Services; // Assuming ErrorReportingService is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void SubmitIncompleteSuppliers(FileTypes ft)
        {
            try
            {
                var info = BaseDataModel.CurrentSalesInfo(-1); // Assuming CurrentSalesInfo exists
                var directory = info.Item4;

                using (var ctx = new CoreEntitiesContext())
                {
                    var suppliers = ctx.TODO_SubmitIncompleteSupplierInfo
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .GroupBy(x => new { x.SupplierCode }).ToList();

                    if (!suppliers.Any()) return;

                    // Assuming ErrorReportingService.GetDocSetActions exists
                    if (Enumerable.Any<ActionDocSetLogs>(ErrorReportingService.GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitIncompleteSuppliers"))) return;

                    var errorfile = Path.Combine(directory, $"IncompleteSuppliers.csv");
                    // Assuming IncompleteSupplier class exists
                    var errors = suppliers.SelectMany(x => x.Select(z => new IncompleteSupplier()
                    {
                        SupplierName = z.SupplierName,
                        SupplierCode = z.SupplierCode,
                        SupplierAddress = z.Street,
                        CountryCode = z.CountryCode
                    }).ToList()).ToList();

                    // Assuming ExportToCSV exists and works this way
                    var res =
                        new ExportToCSV<IncompleteSupplier,
                            List<IncompleteSupplier>>()
                        {
                            dataToPrint = errors
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    var contacts = new CoreEntitiesContext().Contacts
                        .Where(x => x.Role == "Broker" || x.Role == "PO Clerk").ToList();
                    if (File.Exists(errorfile))
                        // Assuming Utils.Client exists
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                            $"Error:InComplete Supplier Info", contacts.Select(x => x.EmailAddress).ToArray(),
                            "Please Fill out the attached Supplier Info and resend CSV...",
                            new string[] { errorfile });

                    // Assuming ErrorReportingService.LogDocSetAction exists
                    //ErrorReportingService.LogDocSetAction(ft.AsycudaDocumentSetId, "SubmitIncompleteSuppliers");
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