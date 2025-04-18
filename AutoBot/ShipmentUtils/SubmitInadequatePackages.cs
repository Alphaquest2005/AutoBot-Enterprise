using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, Contacts, TODO_SubmitInadequatePackages, ActionDocSetLogs are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here
using AutoBot.Services; // Assuming ErrorReportingService is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void SubmitInadequatePackages(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Submit Inadequate Packages");

                // var saleInfo = CurrentSalesInfo(); // Assuming CurrentSalesInfo exists if needed

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();

                    foreach (var docSet in lst)
                    {
                        // Assuming ErrorReportingService.GetDocSetActions exists
                        if (Enumerable.Any<ActionDocSetLogs>(ErrorReportingService.GetDocSetActions(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages"))) continue;

                        var body = $"Error: Inadequate Lines\r\n" +
                                   $"\r\n" +
                                   $"The {docSet.Declarant_Reference_Number} has {docSet.TotalPackages} Packages with {docSet.TotalLines} Lines.\r\n" +
                                   $"Your Maximum Lines per Entry is {docSet.MaxEntryLines}.\r\n" +
                                   $"The minium required packages is {docSet.RequiredPackages}\r\n" +
                                   $"Please increase the Maxlines using \"MaxLines:\"\r\n" +
                                   $"Please note the System will automatically switch from \"Entry per Invoice\" to \"Group Invoices per Entry\", if there are not enough packages per invoice. \r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                        // Assuming Utils.Client exists
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());

                        // Assuming ErrorReportingService.LogDocSetAction exists
                        //ErrorReportingService.LogDocSetAction(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}