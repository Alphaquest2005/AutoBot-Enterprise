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
using xlsxWriter;

//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;

namespace AutoBotUtilities
{
    public static class ShipmentExtensions
    {
        private static string invoiceDocumentCode = "IV05";
        private static string naDocumentCode = "NA";

        public static Shipment LoadEmailPOs(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var invoices = ctx.EntryData.OfType<PurchaseOrders>()
                    //.Include(x => x.EntryDataDetails)
                    .Include(x => x.ShipmentInvoicePOs)
                    .Where(x => x.EmailId == shipment.EmailId)
                    .ToList();

                invoices.ForEach(x => shipment.ShipmentAttachedPOs.Add(new ShipmentAttachedPOs()
                {
                    PurchaseOrders = x,
                    TrackingState = TrackingState.Added
                }));


                return shipment;
            }
        }

        public static Shipment LoadEmailInvoices(this Shipment shipment)
        {
            try
            {


                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = ctx.ShipmentInvoice
                        .Include(x => x.ShipmentRiderInvoice)
                        .Include("ShipmentRiderInvoice.ShipmentRider")
                        .Include("ShipmentRiderInvoice.ShipmentRiderDetails")
                        .Include("InvoiceDetails.ItemAlias")
                        //.Include("ShipmentInvoicePOs.POMISMatches")
                        .Include("ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails.INVItems")
                        .Include("InvoiceDetails.ItemAlias")
                        .Include("InvoiceDetails.POItems")
                        .Where(x => x.EmailId == shipment.EmailId)
                        .ToList();
                    var ctxShipmentInvoicePoItemMisMatches = ctx.ShipmentInvoicePOItemMISMatches.ToList();
                    foreach (var inv in invoices.SelectMany(x => x.ShipmentInvoicePOs))
                    {

                        inv.POMISMatches = ctxShipmentInvoicePoItemMisMatches
                            .Where(x => x.POId == inv.EntryData_Id || x.INVId == inv.InvoiceId).ToList();
                    }

                    invoices.ForEach(x => shipment.ShipmentAttachedInvoices.Add(new ShipmentAttachedInvoices()
                    {
                        ShipmentInvoice = x,
                        TrackingState = TrackingState.Added
                    }));


                    return shipment;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
                    .Include(x => x.ShipmentRiderEx)
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
                    .Include(x => x.ShipmentBLFreight)
                    .Where(x => x.EmailId == shipment.EmailId).AsEnumerable()
                    .DistinctBy(x => x.Id)
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
            try
            {


                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = shipment.ShipmentAttachedRider
                        .SelectMany(x => x.ShipmentRider.ShipmentRiderInvoice.Select(z => z.InvoiceId).Where(z => z != null))
                        .Distinct()
                        
                        .Where(x => shipment.ShipmentAttachedInvoices.All(z => z.ShipmentInvoiceId != x))
                        .Select(x => ctx.ShipmentInvoice
                            .Include("ShipmentRiderInvoice.ShipmentRider")
                            .Include("ShipmentRiderInvoice.ShipmentRiderDetails")
                            .Include("InvoiceDetails.ItemAlias")
                            .Include("InvoiceDetails.POItems")
                            // .Include("ShipmentInvoicePOs.POMISMatches")
                            .Include("ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails.INVItems")
                            .First(z => z.Id == x))
                        .Select(x => new ShipmentAttachedInvoices()
                        {
                            Shipment = shipment,
                            ShipmentInvoice = x,
                            ShipmentInvoiceId = x.Id,
                            TrackingState = TrackingState.Added
                        })
                        .ToList();

                    var shipmentInvoicePoItemMisMatcheses = ctx.ShipmentInvoicePOItemMISMatches.ToList();
                    foreach (var inv in invoices.SelectMany(x => x.ShipmentInvoice.ShipmentInvoicePOs))
                    {
                        
                        inv.POMISMatches = shipmentInvoicePoItemMisMatcheses
                            .Where(x => x.POId == inv.EntryData_Id || x.INVId == inv.InvoiceId).ToList();
                    }

                    var emailLst = invoices.Select(x => x.ShipmentInvoice.EmailId).Distinct().ToList();
                    var invoiceLst = invoices.Select(x => x.ShipmentInvoice.Id).Distinct().ToList();
                   var inattachedInvoices = ctx.ShipmentInvoice
                        .Include("ShipmentRiderInvoice.ShipmentRider")
                        .Include("ShipmentRiderInvoice.ShipmentRiderDetails")
                        .Include("InvoiceDetails.ItemAlias")
                        .Include("InvoiceDetails.POItems")
                        // .Include("ShipmentInvoicePOs.POMISMatches")
                        .Include("ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails.INVItems")
                        .Where(z => !invoiceLst.Contains(z.Id) && emailLst.Contains(z.EmailId))
                       .ToList()
                        .Select(x => new ShipmentAttachedInvoices()
                        {
                            Shipment = shipment,
                            ShipmentInvoice = x,
                            ShipmentInvoiceId = x.Id,
                            TrackingState = TrackingState.Added
                        })
                        .ToList();

                    shipment.ShipmentAttachedInvoices.AddRange(invoices);
                    shipment.ShipmentAttachedInvoices.AddRange(inattachedInvoices);

                }

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static Shipment LoadEmailBL(this Shipment shipment)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    var bls = ctx.ShipmentBL
                        .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                        .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRider.ShipmentRiderEx")
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
               // var getBL = false;
                var riders = shipment.ShipmentAttachedRider.Select(x => x.ShipmentRider).ToList();
                //if (!riders.Any()) getBL = true;
                var newriders = new List<ShipmentRider>();


                var invoiceRiders = shipment.ShipmentAttachedInvoices
                    .SelectMany(x => x.ShipmentInvoice.ShipmentRiderInvoice.Select(z => z.RiderID))
                    .Distinct()
                    .Where(x => riders.All(z => z.Id != x))
                    .Select(x => ctx.ShipmentRider
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .Include(z => z.ShipmentRiderEx)
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
                        .Include(z => z.ShipmentRiderEx)
                        .First(z => z.Id == x))
                    .ToList();

                riders.AddRange(blRiders);
                newriders.AddRange(blRiders);


                if (!riders.Any())
                {
                    var poRiders = shipment.ShipmentAttachedPOs
                        .SelectMany(x => x.PurchaseOrders.ShipmentInvoicePOs.Select(z => z.InvoiceId))
                        .Distinct()
                        .SelectMany(x =>
                            ctx.ShipmentInvoice.Where(i => i.Id == x)
                                .SelectMany(i => i.ShipmentRiderInvoice.Select(ri => ri.RiderID)).ToList()).Distinct()
                        .ToList()
                        .Where(x => riders.All(z => z.Id != x))
                        .Select(x => ctx.ShipmentRider
                            .Include(z => z.ShipmentRiderBLs)
                            .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                            .Include(z => z.ShipmentRiderInvoice)
                            .Include(z => z.ShipmentRiderEx)
                            .First(z => z.Id == x))
                        .OrderByDescending(r => r.Id)
                        .Take(1) // take most recent rider to prevent loading multiple riders
                        .ToList();


                    riders.AddRange(poRiders);
                    newriders.AddRange(poRiders);

                }

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

              //  if (getBL) shipment.LoadDBBL();
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
                            .Include(z => z.ShipmentBLFreight)
                            .Where(z => z.Id == x.FreightId))
                        .DistinctBy(x => x.Id)
                        .ToList();
                    freightList.AddRange(frightDetailBls);

                    var frightBls = shipment.ShipmentAttachedBL
                        .SelectMany(x => x.ShipmentBL.ShipmentBLFreight)
                        .DistinctBy(x => x.Id)
                        .Where(x => freightList.All(z => z.Id != x.FreightInvoiceId))
                        .SelectMany(x => ctx.ShipmentFreight
                            .Include("ShipmentFreightDetails.ShipmentFreightBLs.ShipmentBLDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Where(z => z.Id == x.FreightInvoiceId))
                        .DistinctBy(x => x.Id)
                        .ToList();
                    freightList.AddRange(frightBls);

                    var riderfrightDetail = shipment.ShipmentAttachedRider
                        .SelectMany(x => x.ShipmentRider.ShipmentRiderDetails.Select(z => z))
                        .DistinctBy(x => x.Id)
                        .ToList()
                        .SelectMany(x => ctx.ShipmentFreight
                            .Include("ShipmentFreightDetails.ShipmentFreightBLs.ShipmentBLDetails")
                            .Include(z => z.ShipmentBLFreight)
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
                   // var doRider = false;

                    var riderBls = shipment.ShipmentAttachedRider
                        .SelectMany(x => x.ShipmentRider.ShipmentRiderBLs.Select(z => z.BLId))
                        .Distinct()
                        .Where(x => shipment.ShipmentAttachedBL.All(z => z.ShipmentBL.Id != x))
                        .Select(x => ctx.ShipmentBL
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRider.ShipmentRiderEx")
                            .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Include(z => z.ShipmentRiderBLs)
                            .Include(z => z.ShipmentManifestBLs)
                            .Include(z => z.ShipmentAttachedBL)
                            .First(z => z.Id == x && z.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        .ToList();
                    bls.AddRange(riderBls);

                  //  if (!riderBls.Any()) doRider = true;

                    var frightBls = shipment.ShipmentAttachedFreight
                        .Select(x => x.ShipmentFreight.BLNumber)
                        .Distinct()
                        .Where(x => bls.All(z => z.BLNumber != x))
                        .SelectMany(x => ctx.ShipmentBL
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRider.ShipmentRiderEx")
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
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRider.ShipmentRiderEx")
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
                            .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRider.ShipmentRiderEx")
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

                  //  if (doRider) shipment.LoadDBRiders();

                }

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

       

        public static List<Shipment> ProcessShipment(this Shipment masterShipment)
        {
            try
            {
                var shipments = new List<Shipment>();
                var sendMaster = true;

                ShipmentBL masterBL = null;
                var bls = masterShipment.ShipmentAttachedBL.Select(x => x.ShipmentBL)
                    .OrderByDescending(x => x.ShipmentBLDetails.Count).ToList();
                if (bls.Count > 1)
                    foreach (var bl in bls.OrderBy(x => x.BLNumber))
                    {
                        var otherBls = bls.Where(x => x.BLNumber != bl.BLNumber)
                            .SelectMany(x => x.ShipmentBLDetails.Select(z => z.Marks)).ToList();
                        var marks = bl.ShipmentBLDetails.Select(x => x.Marks).ToList();
                        var matches = otherBls.Where(x => marks.Any(z => z.ToCharArray().Except(x.ToCharArray()).Any())).ToList();
                        if (matches.Count() == otherBls.Count())
                        {
                            masterBL = bl;
                            break;
                        }
                    }

                if (masterBL != null)
                    masterBL.SourceFile =
                        BaseDataModel.SetFilename(masterBL.SourceFile, masterBL.BLNumber, "-MasterBL.pdf");

                foreach (var aBl in masterShipment.ShipmentAttachedBL.Where(x =>
                   sendMaster ? true : x.ShipmentBL.BLNumber != masterBL?.BLNumber))
                {
                    var bl = aBl.ShipmentBL;
                    var riderDetails = bl.ShipmentBLDetails
                        .SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentRiderDetails)).Where(x => x != null)
                        .ToList();

                    var clients = riderDetails.DistinctBy(x => x.Id).GroupBy(x => new Tuple<string, int, string>(x.Code, x.RiderId, bl.BLNumber)).ToList();
                    foreach (var client in clients)
                    {

                        //var unattachedShippment = masterShipment.CreateUnattachedShipment(client.Key);
                        //shipments.Add(unattachedShippment);
                        var summaryPkg = new UnAttachedWorkBookPkg()
                        {
                            Reference = $"{client.Key.Item1}-{client.Key.Item2}-{masterShipment.EmailId}"
                        };


                        var manifests = masterShipment.ShipmentAttachedManifest
                            .Where(x => x.ShipmentManifest.WayBill == bl.BLNumber).Select(x => x.ShipmentManifest)
                            .ToList();
                        var freightInvoicesbyBL = masterShipment.ShipmentAttachedFreight
                            .Where(x => x.ShipmentFreight.ShipmentBLFreight.Any(f => f.BLNumber == bl.BLNumber))
                            .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
                        var freightInvoicesbyDetails = masterShipment.ShipmentAttachedFreight
                           .Where(x => x.ShipmentFreight.ShipmentFreightDetails.Any(z =>
                                client.Any(q => q.WarehouseCode == z.WarehouseCode)))
                            .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
                        var freightInvoices = new List<ShipmentFreight>();
                        freightInvoices.AddRange(freightInvoicesbyBL);
                        freightInvoices.AddRange(freightInvoicesbyDetails.Where(x => freightInvoices.All(z => z.Id != x.Id)).ToList());


                        var blDetails = bl.ShipmentBLDetails
                            .SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentBLDetails)).DistinctBy(x => x.Id)
                            .Where(x => x.ShipmentBL.ShipmentBLDetails.Any(z =>
                                client.Any(q => q.WarehouseCode == z.Marks)))
                            .ToList();


                        var invoices = DoRiderInvoices(masterShipment, client, summaryPkg, out var summaryFile);

                        var attachments = new List<Attachments>();

                        attachments.Add(new Attachments()
                        {
                            FilePath = summaryFile,
                            DocumentCode = "NA",
                            Reference = "Summary",
                            TrackingState = TrackingState.Added
                        });



                        var invocieAttachments = invoices.Select(shipmentInvoice =>
                                xlsxWriter.XlsxWriter.CreatCSV(shipmentInvoice, client.Key.Item2))
                            .SelectMany(x => x.ToList())
                            .ToList();

                        attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".pdf")).Select(x =>
                            new Attachments()
                            {
                                FilePath = x.filepath,
                                DocumentCode = "IV05",
                                Reference = x.reference,
                                TrackingState = TrackingState.Added
                            }));
                        attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".xlsx")).Select(x =>
                            new Attachments()
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
                        if (masterBL != null)
                            attachments.Add(new Attachments()
                            {
                                FilePath = masterBL.SourceFile,
                                DocumentCode = "BL05",
                                Reference = bl.BLNumber,
                                TrackingState = TrackingState.Added
                            });

                        var shipment = new Shipment()
                        {
                            ShipmentName =
                                $"{bl.BLNumber.Split('-').First()}-{client.Key.Item1.Split(' ').FirstOrDefault()}",
                            ManifestNumber = manifests.LastOrDefault()?.RegistrationNumber,
                            BLNumber = bl?.BLNumber,
                            WeightKG = client.Any()
                                ? client.Sum(x => x.GrossWeightKg)
                                : manifests.LastOrDefault()?.GrossWeightKG ?? bl?.WeightKG,
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

                var ridersWithNoBLs = masterShipment.ShipmentAttachedRider.Where(x => !x.ShipmentRider.ShipmentRiderBLs.Any()).ToList();
                if (ridersWithNoBLs.Any())
                {
                    //TODO:// Copy the system above
                    foreach (var sRider in ridersWithNoBLs)
                    {
                        var rider = sRider.ShipmentRider;

                        var clients = rider.ShipmentRiderDetails.GroupBy(x => new Tuple<string, int, string>(x.Code, x.RiderId,"Next Shipment"))
                            .ToList().ToList();

                        foreach (var client in clients)
                        {
                            
                            var summaryPkg = new UnAttachedWorkBookPkg()
                            {
                                Reference = $"{client.Key.Item1}-{client.Key.Item2}-{masterShipment.EmailId}"
                            };


                            var freightInvoices = masterShipment.ShipmentAttachedFreight
                                .Where(x => x.ShipmentFreight.ShipmentFreightDetails.Any(z =>
                                    client.Any(q => q.WarehouseCode == z.WarehouseCode)))
                                .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();
                            
                            var invoices = DoRiderInvoices(masterShipment, client, summaryPkg, out var summaryFile);

                            var attachments = new List<Attachments>();

                            attachments.Add(new Attachments()
                            {
                                FilePath = summaryFile,
                                DocumentCode = "NA",
                                Reference = "Summary",
                                TrackingState = TrackingState.Added
                            });


                            var invocieAttachments = invoices.Select(shipmentInvoice =>
                                    xlsxWriter.XlsxWriter.CreatCSV(shipmentInvoice, client.Key.Item2))
                                .SelectMany(x => x.ToList())
                                .ToList();



                            attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".pdf")).Select(x =>
                                new Attachments()
                                {
                                    FilePath = x.filepath,
                                    DocumentCode = "IV05",
                                    Reference = x.reference,
                                    TrackingState = TrackingState.Added
                                }));
                            attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".xlsx")).Select(x =>
                                new Attachments()
                                {
                                    FilePath = x.filepath,
                                    DocumentCode = "NA",
                                    Reference = x.reference,
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
                                ShipmentName = client.Key.Item1.Split(' ').FirstOrDefault(),
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
                            shipment.ShipmentAttachedInvoices.AddRange(invoices.Select(z =>
                                new ShipmentAttachedInvoices()
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


                return shipments.Where(x => x.ShipmentAttachments.Any()).ToList();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<ShipmentInvoice> DoRiderInvoices(Shipment masterShipment, IGrouping<Tuple<string, int, string>, ShipmentRiderDetails> client, UnAttachedWorkBookPkg summaryPkg,
            out string summaryFile)
        {
            var invoices = masterShipment.ShipmentAttachedInvoices
                .Where(x => x.ShipmentInvoice.ShipmentRiderInvoice.Any(z =>
                    client.Any(q => q.Id == z.RiderLineID))) // 
                //Todo: add to unmaped bl rider lines invoice "client.Where(riderDetail => !riderDetail.ShipmentRiderInvoice.Any())"
                .Select(x => x.ShipmentInvoice)
                .DistinctBy(x => x.InvoiceNo)
                .ToList();

            var invoiceLst = invoices.Select(r => r.Id).ToList();


            var unAttachedInvoices = masterShipment.ShipmentAttachedInvoices
                .Where(x => !invoiceLst.Contains(x.ShipmentInvoiceId))
                .Select(x => x.ShipmentInvoice).DistinctBy(x => x.InvoiceNo).ToList();

            var unAttachedRiderDetails = client.SelectMany(x =>
                x.ShipmentInvoiceRiderDetails.Where(r => r.ShipmentInvoice == null)).ToList();


            var allUnMatchedInvoices = new EntryDataDSContext().ShipmentMIS_Invoices
                .Where(x => invoiceLst.Any(z => z == x.Id)).ToList();
            var allUnMatchedPOs = new EntryDataDSContext().ShipmentMIS_POs.ToList();

            var invoiceNOs = invoices.Select(r => r.InvoiceNo).ToList();
            invoiceNOs.AddRange(unAttachedInvoices.Select(x => x.InvoiceNo));
            var poNOs = invoices.SelectMany(r => r.ShipmentInvoicePOs.Select(z => z.PurchaseOrders.PONumber)).ToList();
            poNOs.AddRange(allUnMatchedPOs.Select(x => x.InvoiceNo).ToList());
            var classifications = new EntryDataDSContext().ShipmentInvoicePOItemData
                .Where(x => invoiceNOs.Contains(x.InvoiceNo) || poNOs.Contains(x.PONumber)).ToList();
            summaryPkg.Classifications = classifications;


            summaryPkg.UnMatchedInvoices = allUnMatchedInvoices;
            summaryPkg.UnMatchedPOs = allUnMatchedPOs;
            summaryPkg.Invoices = invoices;

            summaryPkg.UnAttachedInvoices = unAttachedInvoices;
            summaryPkg.UnAttachedRiderDetails = unAttachedRiderDetails;
            summaryPkg.RiderDetails = client.ToList();

            var riderMatchKeyCode = client.Select(x => x.WarehouseCode.Trim()).ToList();
            var riderMatchKeyInv = client.Select(x => x.InvoiceNumber.Trim()).ToList();

            summaryPkg.RiderSummary = client.First().ShipmentRider.ShipmentRiderEx;


            summaryPkg.RiderManualMatches = new EntryDataDSContext().ShipmentInvoiceRiderManualMatches
                .Where(x => riderMatchKeyCode.Any(z => z == x.WarehouseCode) &&
                            riderMatchKeyInv.Any(z => z == x.RiderInvoiceNumber)).AsEnumerable()
                .DistinctBy(x => new {x.WarehouseCode, x.RiderInvoiceNumber, x.InvoiceNo})
                .ToList();


            summaryFile = XlsxWriter.CreateUnattachedShipmentWorkBook(client.Key, summaryPkg);
            return invoices;
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
