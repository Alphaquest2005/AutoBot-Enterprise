using System;
using System.Collections.Generic;
using EntryDataDS.Business.Entities;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Core.Common.Utils;
using MoreLinq;
using Org.BouncyCastle.Crypto.Operators;
using TrackableEntities;
using WaterNut.DataSpace;

//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;

namespace AutoBotUtilities
{
    public static class ShipmentExtensions
    {
        private static string invoiceDocumentCode = "IV05";
        private static string naDocumentCode = "NA";

        public static Shipment ProcessEmailInvoices(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var invoices = ctx.ShipmentInvoice
                    .Include("ShipmentRiderInvoice.ShipmentRider")
                    .Include("ShipmentRiderInvoice.ShipmentRiderDetails")
                    .Include(x => x.InvoiceDetails)
                    .Include("ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails")
                    .Where(x => x.EmailId == shipment.EmailId)
                    .ToList();

                invoices.ForEach(x => shipment.ShipmentAttachedInvoices.Add(new ShipmentAttachedInvoices()
                {
                    ShipmentInvoice = x,
                    TrackingState = TrackingState.Added
                }));


                return shipment;
            }
        }

        public static Shipment LoadEmailRiders(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var riders = ctx.ShipmentRider
                    .Include(x => x.ShipmentRiderBLs)
                    .Include(x => x.ShipmentRiderDetails)
                    .Include(z => z.ShipmentRiderInvoice)
                    .Where(x => x.EmailId == shipment.EmailId)
                    .ToList();



                shipment.ShipmentAttachedRider.AddRange(riders.Select(x => new ShipmentAttachedRider()
                {
                    ShipmentRider = x,
                    Shipment = shipment,
                    ShipmentId = x.Id,
                    TrackingState = TrackingState.Added
                }));
            }

            return shipment;
        }

        public static Shipment LoadEmailManifest(this Shipment shipment)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    var manifests = ctx.ShipmentManifest
                        .Include("ShipmentManifestBLs.ShipmentBL")
                        .Where(x => x.EmailId == shipment.EmailId)
                        .ToList();

                    shipment.ShipmentAttachedManifest.AddRange(manifests.Select(x => new ShipmentAttachedManifest()
                    {
                        ShipmentManifest = x,
                        ManifestId = x.Id,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }));
                }

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static Shipment LoadEmailFreight(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var freights = ctx.ShipmentFreight
                        .Include("ShipmentFreightDetails.ShipmentFreightBLs.ShipmentBLDetails")
                    .Where(x => x.EmailId == shipment.EmailId)
                    .ToList();

                shipment.ShipmentAttachedFreight.AddRange(freights.Select(x => new ShipmentAttachedFreight()
                {
                    ShipmentFreight = x,
                    FreightInvoiceId = x.Id,
                    Shipment = shipment,
                    TrackingState = TrackingState.Added
                }));
            }

            return shipment;
        }

        public static Shipment LoadDBInvoices(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var invoices = shipment.ShipmentAttachedRider.SelectMany(x => x.ShipmentRider.ShipmentRiderInvoice.Select(z => z.InvoiceId))
                    .Distinct()
                    .Where(x => shipment.ShipmentAttachedInvoices.All(z => z.ShipmentInvoiceId != x))
                    .Select(x => ctx.ShipmentInvoice
                        .Include("ShipmentRiderInvoice.ShipmentRider")
                        .Include("ShipmentRiderInvoice.ShipmentRiderDetails")
                        .Include("InvoiceDetails.ShipmentInvoiceDetailPOSSuggestions")
                        .Include("ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails")

                        .First(z => z.Id == x))
                    .Select(x => new ShipmentAttachedInvoices()
                    {
                        Shipment = shipment,
                        ShipmentInvoice = x,
                        ShipmentInvoiceId = x.Id,
                        TrackingState = TrackingState.Added
                    })
                    .ToList();
                shipment.ShipmentAttachedInvoices.AddRange(invoices);




              
              
            }

            return shipment;
        }

        public static Shipment LoadEmailBL(this Shipment shipment)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    var bls = ctx.ShipmentBL
                        .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                        .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails")
                        .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                        //.Include(x => x.ShipmentFreight)
                        .Include(x => x.ShipmentManifestBLs)
                        .Include(x => x.ShipmentRiderBLs)
                        .Include(x => x.ShipmentAttachedBL)
                        .Where(x => x.EmailId == shipment.EmailId)
                        .ToList();



                    shipment.ShipmentAttachedBL.AddRange(bls.Select(x => new ShipmentAttachedBL()
                    {
                        ShipmentBL = x,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }));

                }

                return shipment;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Shipment LoadDBRiders(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var getBL = false;
                var riders = shipment.ShipmentAttachedRider.Select(x => x.ShipmentRider).ToList();
                if (!riders.Any()) getBL = true;
                var newriders = new List<ShipmentRider>();
                var invoiceRiders = shipment.ShipmentAttachedInvoices
                    .SelectMany(x => x.ShipmentInvoice.ShipmentRiderInvoice.Select(z => z.RiderID))
                    .Distinct()
                    .Where(x => riders.All(z => z.Id != x))
                    .Select(x => ctx.ShipmentRider
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .First(z => z.Id == x))
                    .ToList();
                riders.AddRange(invoiceRiders);
                newriders.AddRange(invoiceRiders);
                var blRiders = shipment.ShipmentAttachedBL
                    .SelectMany(x => x.ShipmentBL.ShipmentRiderBLs.Select(z => z.RiderId))
                    .Distinct()
                    .Where(x => riders.All(z => z.Id != x))
                    .Select(x => ctx.ShipmentRider
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .First(z => z.Id == x))
                    .ToList();

                riders.AddRange(blRiders);
                newriders.AddRange(blRiders);

                //var freightRiders = shipment.ShipmentAttachedFreight
                //    .SelectMany(x => x.ShipmentFreight..Select(z => z.RiderID))
                //    .Distinct()
                //    .Where(x => riders.All(z => z.Id != x))
                //    .Select(x => ctx.ShipmentRider
                //        .Include(z => z.ShipmentRiderBLs)
                //        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                //        .Include(z => z.ShipmentRiderInvoice)
                //        .First(z => z.Id == x))
                //    .ToList();
                //riders.AddRange(invoiceRiders);

                shipment.ShipmentAttachedRider.AddRange(newriders.Select(x => new ShipmentAttachedRider()
                {
                    ShipmentRider = x,
                    Shipment = shipment,
                    ShipmentId = x.Id,
                    TrackingState = TrackingState.Added
                }));

                if (getBL) shipment.LoadDBBL();
                return shipment;
            }
        }

        public static Shipment LoadDBFreight(this Shipment shipment)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    var freightList = shipment.ShipmentAttachedFreight.Select(x => x.ShipmentFreight).ToList();
                    var frightDetailBls = shipment.ShipmentAttachedBL
                        .SelectMany(x => x.ShipmentBL.ShipmentBLDetails.SelectMany(z => z.ShipmentFreightBLs))
                        .DistinctBy(x => x.Id)
                        .Where(x => freightList.All(z => z.Id != x.FreightId))
                        .SelectMany(x => ctx.ShipmentFreight
                            .Include("ShipmentFreightDetails.ShipmentFreightBLs.ShipmentBLDetails")
                            .Where(z => z.Id == x.FreightId))
                        .DistinctBy(x => x.Id)
                        .ToList();
                    freightList.AddRange(frightDetailBls);

                    var riderfrightDetail = shipment.ShipmentAttachedRider
                        .SelectMany(x => x.ShipmentRider.ShipmentRiderDetails.Select(z => z))
                        .DistinctBy(x => x.Id)
                        .ToList()
                        .SelectMany(x => ctx.ShipmentFreight
                            .Include("ShipmentFreightDetails.ShipmentFreightBLs.ShipmentBLDetails")
                            .Where(z => z.ShipmentFreightDetails.Any(f => f.WarehouseCode == x.WarehouseCode))
                            .ToList()
                            .DistinctBy(f => f.Id)
                            .Where(q => freightList.All(z => z.Id != x.Id)));
                    freightList.AddRange(riderfrightDetail);


                    shipment.ShipmentAttachedFreight.AddRange(freightList.Select(x => new ShipmentAttachedFreight()
                    {
                        ShipmentFreight = x,
                        FreightInvoiceId = x.Id,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }));

                }

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static Shipment LoadDBManifest(this Shipment shipment)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    var manifests = shipment.ShipmentAttachedManifest.Select(x => x.ShipmentManifest).ToList();
                    var blManifests = shipment.ShipmentAttachedBL
                        .SelectMany(x => x.ShipmentBL.ShipmentManifestBLs)
                        .DistinctBy(x => x.ManifestId)
                        .Where(x => manifests.All(z => z.Id != x.ManifestId))
                        .Select(x => ctx.ShipmentManifest
                            .Include("ShipmentManifestBLs.ShipmentBL")
                            .First(z => z.Id == x.ManifestId))
                        .ToList();
                    manifests.AddRange(blManifests);

                    shipment.ShipmentAttachedManifest.AddRange(blManifests.Select(x => new ShipmentAttachedManifest()
                    {
                        ShipmentManifest = x,
                        ManifestId = x.Id,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }));

                }

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static Shipment LoadDBBL(this Shipment shipment)
        {
            try
            {


                using (var ctx = new EntryDataDSContext())
                {
                    var bls = shipment.ShipmentAttachedBL.Select(x => x.ShipmentBL).ToList();
                    var doRider = false;

                    var riderBls = shipment.ShipmentAttachedRider
                        .SelectMany(x => x.ShipmentRider.ShipmentRiderBLs.Select(z => z.BLId))
                        .Distinct()
                        .Where(x => shipment.ShipmentAttachedBL.All(z => z.ShipmentBL.Id != x))
                        .Select(x => ctx.ShipmentBL
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails")
                            .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Include(z => z.ShipmentRiderBLs)
                            .Include(z => z.ShipmentManifestBLs)
                            .Include(z => z.ShipmentAttachedBL)
                            .First(z => z.Id == x && z.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        .ToList();
                    bls.AddRange(riderBls);

                    if (!riderBls.Any()) doRider = true;

                    var frightBls = shipment.ShipmentAttachedFreight
                        .Select(x => x.ShipmentFreight.BLNumber)
                        .Distinct()
                        .Where(x => bls.All(z => z.BLNumber != x))
                        .SelectMany(x => ctx.ShipmentBL
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails")
                            .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Include(z => z.ShipmentRiderBLs)
                            .Include(z => z.ShipmentManifestBLs)
                            .Include(z => z.ShipmentAttachedBL)
                            .Where(z => z.BLNumber == x && z.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        .ToList();
                    bls.AddRange(frightBls);

                    var frightDetailBls = shipment.ShipmentAttachedFreight
                        .SelectMany(x => x.ShipmentFreight.ShipmentFreightDetails.SelectMany(z => z.ShipmentFreightBLs))
                        .DistinctBy(x => x.BLNumber)
                        .Where(x => bls.All(z => z.BLNumber != x.BLNumber))
                        .SelectMany(x => ctx.ShipmentBL
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails")
                            .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Include(z => z.ShipmentRiderBLs)
                            .Include(z => z.ShipmentManifestBLs)
                            .Include(z => z.ShipmentAttachedBL)
                            .Where(z => z.BLNumber == x.BLNumber && z.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        .ToList();
                    bls.AddRange(frightDetailBls);

                    var manifestBls = shipment.ShipmentAttachedManifest
                        .Select(x => x.ShipmentManifest.WayBill)
                        .Distinct()
                        .Where(x => bls.All(z => z.BLNumber != x))
                        .SelectMany(x => ctx.ShipmentBL
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails")
                            .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Include(z => z.ShipmentRiderBLs)
                            .Include(z => z.ShipmentManifestBLs)
                            .Include(z => z.ShipmentAttachedBL)
                            .Where(z => z.BLNumber == x && z.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        .ToList();
                    bls.AddRange(manifestBls);

                    shipment.ShipmentAttachedBL = new List<ShipmentAttachedBL>(bls.Select(x => new ShipmentAttachedBL()
                    {
                        ShipmentBL = x,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }));

                    if (doRider) shipment.LoadDBRiders();

                }

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static Shipment CreateUnattachedShipment(this Shipment masterShipment)
        {
            var unAttachedShipment = new Shipment()
            {
                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                TrackingState = TrackingState.Added,
                ShipmentName = "Unattached Documents",
            };

            var emailList = new List<int>();
            emailList.AddRange(masterShipment.ShipmentAttachedBL.Select(x => x.ShipmentBL.EmailId).ToList());
            emailList.AddRange(masterShipment.ShipmentAttachedFreight.Select(x => x.ShipmentFreight.EmailId).ToList());
            emailList.AddRange(masterShipment.ShipmentAttachedManifest.Select(x => x.ShipmentManifest.EmailId).ToList());
            emailList.AddRange(masterShipment.ShipmentAttachedInvoices.Select(x => x.ShipmentInvoice.EmailId).ToList());
            emailList.AddRange(masterShipment.ShipmentAttachedRider.Select(x => x.ShipmentRider.EmailId).ToList());
            emailList = emailList.DistinctBy(x => x).ToList();
            using (var ctx = new EntryDataDSContext())
            {
                var unAttachedInvoices = ctx.ShipmentInvoice
                    .Include(x => x.ShipmentRiderInvoice)
                    .Include(x => x.ShipmentAttachedInvoices)
                    .Include(x => x.InvoiceDetails)
                    .Where(x => emailList.Any(z => z == x.EmailId) &&
                    (!x.ShipmentRiderInvoice.Any() && !x.ShipmentAttachedInvoices.Any())).ToList();

                unAttachedInvoices.ForEach(x => xlsxWriter.XlsxWriter.CreatCSV(x, 0));



                var unAttachedManifest = ctx.ShipmentManifest.Include("ShipmentManifestBLs.ShipmentBL").Where(x =>
                    emailList.Any(z => z == x.EmailId) &&
                    (x.ShipmentManifestBLs.All(z => z.ShipmentBL == null) && !x.ShipmentAttachedManifest.Any())).ToList();

                var unAttachedFreight = ctx.ShipmentFreight.Where(x =>
                    emailList.Any(z => z == x.EmailId) &&
                    (x.ShipmentBLFreight.All(z => z.ShipmentBL == null) && x.ShipmentFreightDetails.All(z => !z.ShipmentFreightBLs.Any()))).ToList();

                var unAttachedRider = ctx.ShipmentRider.Where(x =>
                    emailList.Any(z => z == x.EmailId) &&
                    (!x.ShipmentRiderBLs.Any() ||
                     x.ShipmentRiderDetails.Any(z => !z.ShipmentRiderBLs.Any() || !z.ShipmentRiderInvoice.Any()))).ToList();

                var unAttachedBls = ctx.ShipmentBL.Where(x =>
                    emailList.Any(z => z == x.EmailId) &&
                    (!x.ShipmentManifestBLs.Any() ||
                     x.ShipmentBLDetails.Any(z => !z.ShipmentRiderBLs.Any() || !z.ShipmentFreightBLs.Any()))).ToList();


                var riderDetailWithNoInvoice = masterShipment.ShipmentAttachedRider.Where(x =>
                    x.ShipmentRider.ShipmentRiderDetails.Any(z => !z.ShipmentRiderInvoice.Any())).ToList();

                unAttachedShipment.ShipmentAttachedRider.AddRange(riderDetailWithNoInvoice);

                var unAttachedInvoiceses = masterShipment.ShipmentAttachedInvoices
                    .Where(x => !x.ShipmentInvoice.ShipmentRiderInvoice.Any()).ToList();
                unAttachedShipment.ShipmentAttachedInvoices.AddRange(unAttachedInvoiceses);
                var invoiceswithNoRider = unAttachedInvoiceses.Select(x => x.ShipmentInvoice).ToList();

                var riderDetailWithNoBL = masterShipment.ShipmentAttachedRider.SelectMany(x =>
                    x.ShipmentRider.ShipmentRiderDetails.Where(z => !z.ShipmentRiderBLs.Any()));
                var blWithNoRider = masterShipment.ShipmentAttachedBL.Where(x => !x.ShipmentBL.ShipmentRiderBLs.Any()).Select(x => x.ShipmentBL);
                var blDetailswithNoRiderDetails = masterShipment.ShipmentAttachedBL.SelectMany(x =>
                    x.ShipmentBL.ShipmentBLDetails.Where(d => !d.ShipmentRiderBLs.Any())).ToList();
                var blDetailswithNoRiderDetailsBLs = masterShipment.ShipmentAttachedBL.Where(x =>
                    x.ShipmentBL.ShipmentBLDetails.Any(d => !d.ShipmentRiderBLs.Any())).ToList();
                unAttachedShipment.ShipmentAttachedBL.AddRange(blDetailswithNoRiderDetailsBLs);
                var blDetailswithNoFreight = masterShipment.ShipmentAttachedBL.SelectMany(x =>
                    x.ShipmentBL.ShipmentBLDetails.Where(d => !d.ShipmentFreightBLs.Any()));
                var freightWithNoBl =
                    masterShipment.ShipmentAttachedFreight.Where(x => x.ShipmentFreight.ShipmentBLFreight.Any(z => z.ShipmentBL == null)).ToList();
                unAttachedShipment.ShipmentAttachedFreight.AddRange(freightWithNoBl);
                var freightDetailswithNoBlDetails = masterShipment.ShipmentAttachedFreight.SelectMany(x =>
                    x.ShipmentFreight.ShipmentFreightDetails.Where(f =>!string.IsNullOrEmpty(f.WarehouseCode) && !f.ShipmentFreightBLs.Any())).ToList();

                var blWithNoManifest = masterShipment.ShipmentAttachedBL.Where(x => !x.ShipmentBL.ShipmentManifestBLs.Any()).ToList();
                var manifestWithNoBl = masterShipment.ShipmentAttachedManifest
                    .Where(x => x.ShipmentManifest.ShipmentManifestBLs.Any(z => z.ShipmentBL == null)).ToList();
                unAttachedShipment.ShipmentAttachedManifest.AddRange(manifestWithNoBl);
                unAttachedShipment.ShipmentAttachedManifest.AddRange(unAttachedManifest.Select(x => new ShipmentAttachedManifest(){Shipment = unAttachedShipment, ShipmentManifest = x, TrackingState = TrackingState.Added}));

                unAttachedShipment.Body =
                    $"This email contains all unattached documents for next shipment. Please review and correct the documents if necessary or \r\n" +
                    $"link the information below. \r\n" +
                    $"Any Questions or Concerns please contact Joseph@auto-brokerage.com\r\n" +
                    $"\r\n" +
                    $"\r\n" +
                    $"UnLinked Rider & Invoices:\r\n" +
                    $"\tRider Warehouse Code:\r\n" +
                    $"{riderDetailWithNoInvoice.SelectMany(x => x.ShipmentRider.ShipmentRiderDetails.Where(z => !z.ShipmentRiderInvoice.Any())).Select(x => $"\t\t{x.WarehouseCode} =\r\n").DefaultIfEmpty("").Aggregate((o, c) => o + c)}\r\n" +
                    $"\r\n" +
                    $"\tInvoices Numbers:(Cut and paste Warehouse code above after '=' to link)\r\n" +
                    $"\t{invoiceswithNoRider.Select(x => x.InvoiceNo).Union(unAttachedInvoices.Select(x => x.InvoiceNo)).Select(x => $"\t{x} = \r\n").DefaultIfEmpty("").Aggregate((o, c) => o + c)}\r\n" +
                    $"\r\n" +
                    $"BL Marks unlinked to Rider Warehouse Codes: (Cut and paste Warehouse code above or type Correct info after '=' to link)\r\n" +
                    $"\t Bl Marks:\r\n" +
                    $"\t{blDetailswithNoRiderDetails.SelectMany(x => x.ShipmentBL.ShipmentBLDetails.Where(z => !z.ShipmentRiderBLs.Any())).Select(x => $"\t{x.Marks} = \r\n").DefaultIfEmpty("").Aggregate((o, c) => o + c)}\r\n" +
                    $"\r\n" +
                    $"\r\n" +
                    $"\r\n" +
                    $"Unlinked BL Documents(Freight & Manifest)\r\n" +
                    $"BLs Found:\r\n" +
                    $"\t{masterShipment.ShipmentAttachedBL.Select(x => x.ShipmentBL.BLNumber).DefaultIfEmpty("").Aggregate((o, c) => o + "\r\n\t"+ c)}\r\n" +
                    $"\r\n" +
                    $"Unlinked Freight Invoices:(Cut and paste BL Number above or type Correct info after '=' to link)\r\n" +
                    $"\t{freightWithNoBl.Select(x => $"\t{x.ShipmentFreight.InvoiceNumber} - '{x.ShipmentFreight.Consignee}' = \r\n").DefaultIfEmpty("").Aggregate((o, c) => o + c)}\r\n" +
                    $"\r\n" +
                    $"Unlinked Manifest: (Cut and paste BL Number above or type Correct info after '=' to link)\r\n" +
                    $"\t{manifestWithNoBl.Select(x => x.ShipmentManifest.WayBill).Union(unAttachedManifest.Select(x => x.WayBill)).Select(x => $"{x} = \r\n").DefaultIfEmpty("").Aggregate((o, c) => o + c)}\r\n";

                var attachments = new List<Attachments>();
                attachments.AddRange(invoiceswithNoRider.Union(unAttachedInvoices).DistinctBy(x => x.Id).Select(x => new Attachments()
                {
                    FilePath = Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.InvoiceNo}.pdf"),
                    DocumentCode = "IV05",
                    Reference = x.InvoiceNo,
                    TrackingState = TrackingState.Added
                }));
                attachments.AddRange(invoiceswithNoRider.Union(unAttachedInvoices).DistinctBy(x => x.Id).Select(x => new Attachments()
                {
                    FilePath = Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.InvoiceNo}.xlsx"),
                    DocumentCode = "NA",
                    Reference = x.InvoiceNo,
                    TrackingState = TrackingState.Added
                }));
                attachments.AddRange(manifestWithNoBl.Select(x => x.ShipmentManifest).Union(unAttachedManifest).DistinctBy(x => x.Id).Select(x => new Attachments()
                {
                    FilePath = x.SourceFile,
                    DocumentCode = "BL07",
                    Reference = x.RegistrationNumber,
                    TrackingState = TrackingState.Added
                }));
                attachments.AddRange(freightWithNoBl.Select(x => x.ShipmentFreight).Union(unAttachedFreight).DistinctBy(x => x.Id).Select(x => new Attachments()
                {
                    FilePath = x.SourceFile,
                    DocumentCode = "IV04",
                    Reference = x.InvoiceNumber,
                    TrackingState = TrackingState.Added
                }));
                attachments.AddRange(blDetailswithNoRiderDetails.Select(x => x.ShipmentBL).Union(unAttachedBls).DistinctBy(x => x.BLNumber)
                    .Select(bl => new Attachments()
                    {
                        FilePath = bl.SourceFile,
                        DocumentCode = "BL10",
                        Reference = bl.BLNumber,
                        TrackingState = TrackingState.Added
                    }));



                unAttachedShipment.ShipmentAttachments.AddRange(attachments.Select(x =>
                    new ShipmentAttachments()
                    {
                        Attachments = x,
                        Shipment = unAttachedShipment,
                        TrackingState = TrackingState.Added
                    }));

                return unAttachedShipment;
            }
        }

        public static List<Shipment> ProcessShipment(this Shipment masterShipment)
        {
            var shipments = new List<Shipment>();
            var unattachedShippment = masterShipment.CreateUnattachedShipment();
            shipments.Add(unattachedShippment);
            foreach (var aBl in masterShipment.ShipmentAttachedBL)
            {
                var bl = aBl.ShipmentBL;
                var riderDetails = bl.ShipmentBLDetails.SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentRiderDetails)).Where(x => x != null).ToList();

                var clients = riderDetails.GroupBy(x => new{x.Code, x.RiderId}).ToList();
                foreach (var client in clients)
                {
                    var manifests = masterShipment.ShipmentAttachedManifest
                        .Where(x => x.ShipmentManifest.WayBill == bl.BLNumber).Select(x => x.ShipmentManifest).ToList();
                    var freightInvoices = masterShipment.ShipmentAttachedFreight
                        .Where(x => x.ShipmentFreight.BLNumber == bl.BLNumber ||
                                    x.ShipmentFreight.ShipmentFreightDetails.Any(z =>
                                        z.ShipmentFreightBLs.Any(f => f.BLNumber == bl.BLNumber)))
                        .Where(x => x.ShipmentFreight.ShipmentFreightDetails.Any(z =>client.Any(q => q.WarehouseCode == z.WarehouseCode)))
                        .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
                    var blDetails = bl.ShipmentBLDetails
                        .SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentBLDetails)).DistinctBy(x => x.Id)
                        .Where(x => x.ShipmentBL.ShipmentBLDetails.Any(z =>
                            client.Any(q => q.WarehouseCode == z.Marks)))
                        .ToList();





                    var invoices = masterShipment.ShipmentAttachedInvoices
                        .Where(x => x.ShipmentInvoice.ShipmentRiderInvoice.Any(z => client.Any(q => q.Id == z.RiderLineID)))// 
                        //Todo: add to unmaped bl rider lines invoice "client.Where(riderDetail => !riderDetail.ShipmentRiderInvoice.Any())"
                        .Select(x => x.ShipmentInvoice)
                        .DistinctBy(x => x.InvoiceNo)
                       .ToList();

                    var attachments = new List<Attachments>();

                    var invocieAttachments = invoices.Select(shipmentInvoice => xlsxWriter.XlsxWriter.CreatCSV(shipmentInvoice, client.Key.RiderId))
                        .SelectMany(x => x.ToList())
                        .ToList();

                    attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".pdf")).Select(x => new Attachments()
                    {
                        FilePath = x.filepath,
                        DocumentCode = "IV05",
                        Reference = x.reference,
                        TrackingState = TrackingState.Added
                    }));
                    attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".xlsx")).Select(x => new Attachments()
                    {
                        FilePath = x.filepath,
                        DocumentCode = "NA",
                        Reference = x.reference,
                        TrackingState = TrackingState.Added
                    }));
                    attachments.AddRange(manifests.Select(x => new Attachments()
                    {
                        FilePath = x.SourceFile,
                        DocumentCode = "BL07",
                        Reference = x.RegistrationNumber,
                        TrackingState = TrackingState.Added
                    }));
                    attachments.AddRange(freightInvoices.Select(x => new Attachments()
                    {
                        FilePath = x.SourceFile,
                        DocumentCode = "IV04",
                        Reference = x.InvoiceNumber,
                        TrackingState = TrackingState.Added
                    }));
                    attachments.Add(new Attachments()
                    {
                        FilePath = bl.SourceFile,
                        DocumentCode = "BL10",
                        Reference = bl.BLNumber,
                        TrackingState = TrackingState.Added
                    });

                    var shipment = new Shipment()
                    {
                        ShipmentName = $"{bl.BLNumber.Split('-').First()}-{client.Key.Code.Split(' ').FirstOrDefault()}",
                        ManifestNumber = manifests.LastOrDefault()?.RegistrationNumber,
                        BLNumber = bl?.BLNumber,
                        WeightKG = client.Any()? client.Sum(x => x.GrossWeightKg) : manifests.LastOrDefault()?.GrossWeightKG ?? bl?.WeightKG,
                        Currency = invoices.Select(x => x.Currency).FirstOrDefault() ?? "USD",
                        ExpectedEntries = invoices.Select(x => x.Id).Count(),
                        TotalInvoices = invoices.Select(x => x.Id).Count(),
                        FreightCurrency = freightInvoices.LastOrDefault()?.Currency ?? "USD",
                        Freight = freightInvoices.LastOrDefault()?.InvoiceTotal,
                        Origin = "US",
                        Packages = client.Any() ? client.Sum(x => x.Pieces) : blDetails.Sum(x => x.Quantity),
                        Location = manifests.LastOrDefault()?.LocationOfGoods,
                        Office = manifests.LastOrDefault()?.CustomsOffice,
                        TrackingState = TrackingState.Added
                    };
                    shipment.ShipmentAttachedInvoices.AddRange(invoices.Select(z => new ShipmentAttachedInvoices()
                    {
                        ShipmentInvoice = z,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }).ToList());
                    shipment.ShipmentAttachedBL.AddRange(new List<ShipmentAttachedBL>()
                    {
                        new ShipmentAttachedBL()
                        {
                            Shipment = shipment,
                            ShipmentBL = bl,
                            TrackingState = TrackingState.Added
                        }
                    });
                    shipment.ShipmentAttachedManifest.AddRange(manifests.Select(x =>
                        new ShipmentAttachedManifest()
                        {
                            ShipmentManifest = x,
                            Shipment = shipment,
                            TrackingState = TrackingState.Added
                        }));
                    shipment.ShipmentAttachedFreight.AddRange(freightInvoices.Select(x =>
                        new ShipmentAttachedFreight()
                        {
                            ShipmentFreight = x,
                            Shipment = shipment,
                            TrackingState = TrackingState.Added
                        }));
                    shipment.ShipmentAttachments.AddRange(attachments.Select(x =>
                        new ShipmentAttachments()
                        {
                            Attachments = x,
                            Shipment = shipment,
                            TrackingState = TrackingState.Added
                        }));

                    shipments.Add(shipment);
                }
            }

            if (!masterShipment.ShipmentAttachedBL.Any() && masterShipment.ShipmentAttachedRider.Any())
            { //TODO:// Copy the system above
                foreach (var sRider in masterShipment.ShipmentAttachedRider)
                {
                    var rider = sRider.ShipmentRider;

                    var clients = rider.ShipmentRiderDetails.GroupBy(x => x.Code).ToList();
                    foreach (var client in clients)
                    {



                        var freightInvoices = masterShipment.ShipmentAttachedFreight
                            .Where(x => x.ShipmentFreight.ShipmentFreightDetails.Any(z =>
                                client.Any(q => q.WarehouseCode == z.WarehouseCode)))
                            .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
                        //var blDetails = bl.ShipmentBLDetails.SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentBLDetails)).DistinctBy(x => x.Id).ToList();

                        // var riderDetails = bl.ShipmentBLDetails.SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentRiderDetails)).DistinctBy(x => x.Id).ToList();

                        // riderDetails.FirstOrDefault()?.Code;

                        var invoices = masterShipment.ShipmentAttachedInvoices
                            .Where(x => x.ShipmentInvoice.ShipmentRiderInvoice.Any(z =>
                                client.Any(q => q.InvoiceNumber == x.ShipmentInvoice.InvoiceNo)))
                            .Select(x => x.ShipmentInvoice).DistinctBy(x => x.InvoiceNo).ToList();

                        var attachments = new List<Attachments>();
                        attachments.AddRange(invoices.Select(x => new Attachments()
                        {
                            FilePath = Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.InvoiceNo}.pdf"),
                            DocumentCode = "IV05",
                            Reference = x.InvoiceNo,
                            TrackingState = TrackingState.Added
                        }));
                        attachments.AddRange(invoices.Select(x => new Attachments()
                        {
                            FilePath = Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.InvoiceNo}.xlsx"),
                            DocumentCode = "NA",
                            Reference = x.InvoiceNo,
                            TrackingState = TrackingState.Added
                        }));
                        attachments.AddRange(freightInvoices.Select(x => new Attachments()
                        {
                            FilePath = x.SourceFile,
                            DocumentCode = "IV04",
                            Reference = x.InvoiceNumber,
                            TrackingState = TrackingState.Added
                        }));
                        attachments.Add(new Attachments()
                        {
                            FilePath = rider.SourceFile,
                            DocumentCode = "NA",
                            Reference = "Rider",
                            TrackingState = TrackingState.Added
                        });

                        var shipment = new Shipment()
                        {
                            ShipmentName = client.Key.Split(' ').FirstOrDefault(),
                            //ManifestNumber = manifests.LastOrDefault()?.RegistrationNumber,
                            //BLNumber = bl?.BLNumber,
                            WeightKG = client.Sum(x => x.GrossWeightKg),
                            Currency = invoices.Select(x => x.Currency).FirstOrDefault() ?? "USD",
                            ExpectedEntries = invoices.Select(x => x.Id).Count(),
                            TotalInvoices = invoices.Select(x => x.Id).Count(),
                            FreightCurrency = freightInvoices.LastOrDefault()?.Currency ?? "USD",
                            Freight = freightInvoices.LastOrDefault()?.InvoiceTotal,
                            Origin = "US",
                            Packages = client.Sum(x => x.Pieces),
                            //Location = manifests.LastOrDefault()?.LocationOfGoods,
                            //Office = manifests.LastOrDefault()?.CustomsOffice,
                            TrackingState = TrackingState.Added
                        };
                        shipment.ShipmentAttachedInvoices.AddRange(invoices.Select(z => new ShipmentAttachedInvoices()
                        {
                            ShipmentInvoice = z,
                            Shipment = shipment,
                            TrackingState = TrackingState.Added
                        }).ToList());
                        shipment.ShipmentAttachedRider.AddRange(new List<ShipmentAttachedRider>()
                        {
                            new ShipmentAttachedRider()
                            {
                                Shipment = shipment,
                                ShipmentRider = rider,
                                TrackingState = TrackingState.Added
                            }
                        });
                        shipment.ShipmentAttachedFreight.AddRange(freightInvoices.Select(x =>
                            new ShipmentAttachedFreight()
                            {
                                ShipmentFreight = x,
                                Shipment = shipment,
                                TrackingState = TrackingState.Added
                            }));
                        shipment.ShipmentAttachments.AddRange(attachments.Select(x =>
                            new ShipmentAttachments()
                            {
                                Attachments = x,
                                Shipment = shipment,
                                TrackingState = TrackingState.Added
                            }));

                        shipments.Add(shipment);
                    }
                }
            }

            
            return shipments.Where(x => x.ShipmentAttachments.Any()).ToList() ;
        }

        public static List<Shipment> SaveShipment(this List<Shipment> shipments)
        {
            
            //using (var ctx = new EntryDataDSContext())
            //{
            //    shipments.ForEach(x =>
            //    {
            //        ctx.Shipment.Add(x);
            //        ctx.SaveChanges();
            //    } );
               
            //}

            return shipments;
        }
    }
}
