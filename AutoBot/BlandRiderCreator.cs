using System;
using System.Collections.Generic;
using System.Linq;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.DataSpace;
using xlsxWriter;

namespace AutoBotUtilities
{
    public static class BlandRiderCreator
    {
        public static void CreateShipmentFromBLsAndRider(Shipment masterShipment, List<Shipment> shipments)
        {
            
            var bls = masterShipment.ShipmentAttachedBL.Select(x => x.ShipmentBL)
                .OrderByDescending(x => x.ShipmentBLDetails.Count).ToList();
            
            AddMasterBl(bls);


            foreach (var bl in bls)
            {
                var riderDetails = masterShipment.ShipmentAttachedRider.Where(x =>
                    x.ShipmentRider.ShipmentRiderDetails.Any(z =>
                        z.ShipmentRiderBLs.Any(b => bls.Any(r => r.Id == b.BLId)) || !bl.ShipmentBLDetails.Any()))
                    .SelectMany(x => x.ShipmentRider.ShipmentRiderDetails)
                    .ToList();
                var packingDetails = riderDetails;

                if (!packingDetails.Any()) packingDetails = GetManifestBlPackingDetails(masterShipment, bl);

                
                if (!packingDetails.Any())  packingDetails = GetBlPackingDetails(bl); 

                if (!packingDetails.Any()) continue;
                var clients = GetPackageSetsList(masterShipment, packingDetails, bl);

                shipments.AddRange(clients.Select(client => CreateShipment(masterShipment, client, bl)));
            }

          

        }

        private static void AddMasterBl(List<ShipmentBL> bls)
        {
            if (bls.Count <= 1) return;

            ShipmentBL masterBL = null;
            foreach (var bl in bls.OrderBy(x => x.BLNumber))
            {
                var otherBls = bls.Where(x => x.BLNumber != bl.BLNumber)
                    .SelectMany(x => x.ShipmentBLDetails.Select(z => z.Marks)).ToList();
                var marks = bl.ShipmentBLDetails.Select(x => x.Marks).ToList();
                var matches = otherBls.Where(x =>
                    marks.Any(z => z == x /*z.ToCharArray().Except(x.ToCharArray()).Any()*/)).ToList();
                
                if (matches.Count() != otherBls.Count()) continue;

                masterBL = bl;
                masterBL.SourceFile =
                    BaseDataModel.SetFilename(masterBL.SourceFile, masterBL.BLNumber, "-MasterBL.pdf");
                matches.ForEach(x => bls.First(z => z.BLNumber == x).MasterBl = masterBL);
                bls.Remove(bl);
                break;

            }

        }

        private static List<IGrouping<(string Code, int RiderId, string BLNumber), ShipmentRiderDetails>> GetPackageSetsList(Shipment masterShipment, List<ShipmentRiderDetails> blPackingDetails, ShipmentBL bl)
        {
            var clients = blPackingDetails.Where(x =>
                    masterShipment.ShipmentAttachedRider.Any(z => z.ShipmentRider.Id == x.RiderId))
                .DistinctBy(x => x.Id)
                .GroupBy(x =>
                    (x.Code, x.RiderId,
                        bl.BLNumber)) //.GroupBy(x => new Tuple<string, int, string>(x.Code, x.RiderId, bl.BLNumber))
                // .MaxBy(x => x.Key.Item2)
                .ToList();
            return clients;
        }

        private static List<ShipmentRiderDetails> GetBlPackingDetails(ShipmentBL bl)
        {
            List<ShipmentRiderDetails> blPackingDetails;
            blPackingDetails = bl.ShipmentBLDetails
                .SelectMany(x =>
                    x.ShipmentRiderBLs.SelectMany(z =>
                        z.ShipmentRider.ShipmentRiderDetails)) //Utils.CreatePackingList(z.ShipmentRider)
                .Where(x => x != null)
                .ToList();
            return blPackingDetails;
        }

        private static List<ShipmentRiderDetails> GetManifestBlPackingDetails(Shipment masterShipment, ShipmentBL bl)
        {
            var blPackingDetails = new List<ShipmentRiderDetails>();

            var blManifests = masterShipment.ShipmentAttachedManifest
                .Where(x => x.ShipmentManifest.WayBill == bl.BLNumber).Select(x => x.ShipmentManifest)
                .DistinctBy(x => x.Id)
                .ToList();

            foreach (var rider in blManifests.SelectMany(x => x.ShipmentRiderManifests)
                         .DistinctBy(x => x.RiderId).OrderByDescending(x => x.BLNumber == x.ShipmentManifest.WayBill).ToList()
                         .Select(manifest => masterShipment.ShipmentAttachedRider
                             .FirstOrDefault(x => x.ShipmentId == manifest.RiderId)?.ShipmentRider)
                         .Where(rider => rider != null))
            {
                blPackingDetails.AddRange(rider.ShipmentRiderDetails); //Utils.CreatePackingList(rider)
            }

            return blPackingDetails;
        }

        private static Shipment CreateShipment(Shipment masterShipment, IGrouping<(string Code, int RiderId, string BLNumber), ShipmentRiderDetails> client, ShipmentBL bl)
        {
            var packingDetails = client.FirstOrDefault().ShipmentRider.ShipmentRiderInvoice
                .Where(x => x.ShipmentRiderDetails != null && x.Packages > 0)
                .Select(x => (x.WarehouseCode, Packages: x.Packages ?? 0, x.InvoiceNo))
                .ToList();

            var manifests = masterShipment.ShipmentAttachedManifest
                .Where(x => x.ShipmentManifest.WayBill == bl.BLNumber ||
                            (!String.IsNullOrEmpty(bl.Voyage) && x.ShipmentManifest.Voyage.Contains(bl.Voyage)) ||
                            x.ShipmentManifest.ShipmentRiderManifests.Any(z =>
                                z.RiderId == client.First().RiderId)).Select(x => x.ShipmentManifest)
                .OrderByDescending(x => x.WayBill == bl.BLNumber)
                .ToList();


            var freightInvoicesbyBL = masterShipment.ShipmentAttachedFreight
                .Where(x => x.ShipmentFreight.ShipmentBLFreight.Any(f => f.BLNumber == bl.BLNumber))
                .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
            var freightInvoicesbyDetails = masterShipment.ShipmentAttachedFreight
                .Where(x => x.ShipmentFreight.ShipmentFreightDetails.Any(z =>
                    client.Any(r => r.WarehouseCode == z.WarehouseCode)))
                .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
            var freightInvoices = new List<ShipmentFreight>();
            freightInvoices.AddRange(freightInvoicesbyBL);
            freightInvoices.AddRange(freightInvoicesbyDetails
                .Where(x => freightInvoices.All(z => z.Id != x.Id)).ToList());


            var blDetails = bl.ShipmentBLDetails
                .SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentBLDetails)).DistinctBy(x => x.Id)
                .Where(x => x.ShipmentBL.ShipmentBLDetails.Any(z =>
                    client.Any(r => r.WarehouseCode == z.Marks)))
                .ToList();


            var shipment = new Shipment
            {
                ShipmentName =
                    $"{bl.BLNumber.Split('-').First()}-{client.Key.Code.Split(' ').FirstOrDefault()}",
                ManifestNumber = manifests.LastOrDefault()?.RegistrationNumber,
                BLNumber = manifests.FirstOrDefault()?.WayBill ?? bl?.BLNumber,
                WeightKG = !manifests.Any()
                    ? client.Select(r => r.GrossWeightKg).Sum()
                    : manifests.FirstOrDefault()?.GrossWeightKG ?? bl?.WeightKG,
                ExpectedEntries = 0,
                TotalInvoices = 0,
                FreightCurrency = freightInvoices.LastOrDefault()?.Currency ?? bl.FreightCurrency ?? "USD",
                // dont understand why i checking manifest if there is no freight invoice
                //Freight = freightInvoices.LastOrDefault()?.InvoiceTotal ?? (bl?.BLNumber ==  manifests.LastOrDefault()?.WayBill || manifests.Any(x => x.Voyage.Contains(bl.Voyage)) ?  bl.Freight : 0),
                Freight = freightInvoices.LastOrDefault()?.InvoiceTotal ?? bl.Freight,
                Origin = "US",
                Packages = manifests.FirstOrDefault()?.Packages ?? (!blDetails.Any()
                    ? client.Select(r => r.Pieces).Sum()
                    : blDetails.Sum(x => x.Quantity)),

                Location = manifests.FirstOrDefault()?.LocationOfGoods,
                Office = manifests.FirstOrDefault()?.CustomsOffice,
                TrackingState = TrackingState.Added
            };


           


            //var unattachedShippment = masterShipment.CreateUnattachedShipment(client.Key);
            //shipments.Add(unattachedShippment);
            var summaryPkg = new UnAttachedWorkBookPkg
            {
                Reference = $"{masterShipment.EmailId}"
            };

            var allShipmentRiderDetails = masterShipment.ShipmentAttachedRider
                .SelectMany(x => x.ShipmentRider.ShipmentRiderDetails).Where(x => x.RiderId == client.Key.RiderId).ToList();

            var invoices = ShipmentExtensions.GetInvoicesAndCreateSummaryFile(masterShipment, client, bl.ShipmentBLDetails,
                allShipmentRiderDetails, packingDetails, summaryPkg, out var summaryFile);



            shipment.Currency = "USD"; //invoices.Select(x => x.Currency).FirstOrDefault() ??

            //if (shipment.Currency.Length != 3) throw new ApplicationException("Currency must be 3 letters");


            shipment.ExpectedEntries += invoices.Count(x =>
                (x.ShipmentRiderInvoice.FirstOrDefault()?.Packages ?? 0) >
                0); //invoices.Select(x => x.Id).Count(),
            shipment.TotalInvoices += invoices.Select(x => x.Id).Count();


            var invoiceAttachments = invoices
                .GroupBy(x => x.ShipmentRiderInvoice.FirstOrDefault()?.WarehouseCode ?? "")
                .Select(shipmentInvoice =>
                    XlsxWriter.CreateCSV(shipmentInvoice.Key,
                        shipmentInvoice.OrderByDescending(z =>
                            z.ShipmentRiderInvoice.FirstOrDefault()?.Packages ?? 0).ToList(),
                        client.Key.RiderId, packingDetails))
                .SelectMany(x => x.ToList())
                .ToList();

            var attachments = AddAttachments(bl, summaryFile, invoiceAttachments, manifests, freightInvoices);

            shipment.ShipmentAttachedInvoices.AddRange(invoices.Select(z => new ShipmentAttachedInvoices
            {
                ShipmentInvoice = z,
                Shipment = shipment,
                TrackingState = TrackingState.Added
            }).ToList());

            shipment.ShipmentAttachedBL.AddRange(new List<ShipmentAttachedBL>
            {
                new ShipmentAttachedBL
                {
                    Shipment = shipment,
                    ShipmentBL = bl,
                    TrackingState = TrackingState.Added
                }
            });
            shipment.ShipmentAttachedManifest.AddRange(manifests.Select(x =>
                new ShipmentAttachedManifest
                {
                    ShipmentManifest = x,
                    Shipment = shipment,
                    TrackingState = TrackingState.Added
                }));
            shipment.ShipmentAttachedFreight.AddRange(freightInvoices.Select(x =>
                new ShipmentAttachedFreight
                {
                    ShipmentFreight = x,
                    Shipment = shipment,
                    TrackingState = TrackingState.Added
                }));
            shipment.ShipmentAttachments.AddRange(attachments.Select(x =>
                new ShipmentAttachments
                {
                    Attachments = x,
                    Shipment = shipment,
                    TrackingState = TrackingState.Added
                }));
            return shipment;
        }

        private static List<Attachments> AddAttachments(ShipmentBL bl, string summaryFile, List<(string reference, string filepath)> invoiceAttachments, List<ShipmentManifest> manifests,
            List<ShipmentFreight> freightInvoices)
        {
            var attachments = new List<Attachments>();

            attachments.Add(new Attachments
            {
                FilePath = summaryFile,
                DocumentCode = "NA",
                Reference = "Summary",
                TrackingState = TrackingState.Added
            });

            attachments.AddRange(invoiceAttachments.Where(x => x.filepath.EndsWith(".pdf")).Select(x =>
                new Attachments
                {
                    FilePath = x.filepath,
                    DocumentCode = "IV05",
                    Reference = x.reference,
                    TrackingState = TrackingState.Added
                }));
            attachments.AddRange(invoiceAttachments.Where(x => x.filepath.EndsWith(".xlsx")).Select(x =>
                new Attachments
                {
                    FilePath = x.filepath,
                    DocumentCode = "NA",
                    Reference = x.reference,
                    TrackingState = TrackingState.Added
                }));


            attachments.AddRange(manifests.Select(x => new Attachments
            {
                FilePath = x.SourceFile,
                DocumentCode = "BL07",
                Reference = x.RegistrationNumber,
                TrackingState = TrackingState.Added
            }));
            attachments.AddRange(freightInvoices.Select(x => new Attachments
            {
                FilePath = x.SourceFile,
                DocumentCode = "IV04",
                Reference = x.InvoiceNumber,
                TrackingState = TrackingState.Added
            }));
            attachments.Add(new Attachments
            {
                FilePath = bl.SourceFile,
                DocumentCode = "BL10",
                Reference = bl.BLNumber,
                TrackingState = TrackingState.Added
            });
            if (bl.MasterBl != null)
                attachments.Add(new Attachments
                {
                    FilePath = bl.MasterBl.SourceFile,
                    DocumentCode = "BL05",
                    Reference = bl.BLNumber,
                    TrackingState = TrackingState.Added
                });
            return attachments;
        }
    }
}