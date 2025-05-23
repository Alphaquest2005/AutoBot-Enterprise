using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, Contacts are here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, Shipment, Attachments are here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here
using MoreLinq; // For ForEach

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void CreateShipmentEmail(FileTypes fileType, FileInfo[] files)
        {
            try
            {
                var emailId = fileType.EmailId;

                // Todo quick paks etc put Man reg#, bl numer, TotalWeight & Totalfreight in spreadsheet. so invoice can generate the entry.
                // change the required documents to match too

                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Database.ExecuteSqlCommand(@"EXEC [dbo].[PreProcessShipmentSP]");
                }

                // Assuming Shipment class and its fluent methods exist and are accessible
                var shipments = new Shipment(){ShipmentName = "Next Shipment",EmailId = emailId, TrackingState = TrackingState.Added}
                        .LoadEmailPOs()
                        .LoadEmailInvoices()
                        .LoadEmailRiders()
                        .LoadEmailBL()
                        .LoadEmailManifest()
                        .LoadEmailFreight()
                        .LoadDBBL()
                        .LoadDBManifest()
                        .LoadDBRiders()
                        .LoadDBBL()
                        .LoadDBManifest()
                        .LoadDBRiders()
                        .LoadDBFreight()
                        .LoadDBInvoices()
                        .AutoCorrect()
                        .ProcessShipment()
                    //.SaveShipment()
                    ;

                var contacts = new CoreEntitiesContext().Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer" || x.Role == "PO Clerk")
                    .Select(x => x.EmailAddress)
                    .Distinct()
                    .ToArray();


                using (var ctx = new EntryDataDSContext())
                {
                    shipments.ForEach(shipment =>
                    {
                        // Assuming Utils.Client exists and is accessible
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"Shipment: {shipment.ShipmentName}", contacts, shipment.ToString(),
                            shipment.ShipmentAttachments.Select(x => x.Attachments.FilePath).ToArray());


                        ctx.Attachments.AddRange(shipment.ShipmentAttachments.Select(x => x.Attachments).ToList());

                    });
                    ctx.SaveChanges();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                BaseDataModel.EmailExceptionHandler(e); // Assuming EmailExceptionHandler exists
                throw;
            }
        }
    }
}