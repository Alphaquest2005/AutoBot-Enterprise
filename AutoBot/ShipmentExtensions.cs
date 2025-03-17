using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoBot;
using EntryDataDS.Business.Entities;
using MoreLinq;
//using MoreLinq.Extensions;
using TrackableEntities;
using WaterNut.DataSpace;
using xlsxWriter;
using static System.String;
using Utils = WaterNut.DataSpace.Utils;

namespace AutoBotUtilities
{
    public static class ShipmentExtensions
    {
     

        private static List<ShipmentInvoice> shipmentInvoices;

        public static Shipment LoadEmailPOs(this Shipment shipment)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var invoices = ctx.EntryData.OfType<PurchaseOrders>()
                    //.Include(x => x.EntryDataDetails)
                    .Include(x => x.ShipmentInvoicePOs)
                    .Include("ShipmentInvoicePOs.PurchaseOrders.WarehouseInfo")
                    .Where(x => x.EmailId == shipment.EmailId)
                    .ToList();

                invoices.ForEach(x => shipment.ShipmentAttachedPOs.Add(new ShipmentAttachedPOs
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
                    var invoices = GetShipmentInvoices().Where(x => x.EmailId == shipment.EmailId).ToList();
                    var ctxShipmentInvoicePoItemMisMatches = ctx.ShipmentInvoicePOItemMISMatches.ToList();

                    var poInvoices = shipment.ShipmentAttachedPOs
                        .SelectMany(x => x.PurchaseOrders.ShipmentInvoicePOs)
                        .DistinctBy(x => x.InvoiceId)
                        .Where(x => invoices.All(z => z.Id != x.InvoiceId))
                        .Select(x => GetShipmentInvoices().First(z => z.Id == x.InvoiceId))
                        .ToList();
                    invoices.AddRange(poInvoices);

                    foreach (var inv in invoices.SelectMany(x => x.ShipmentInvoicePOs))
                        inv.POMISMatches = ctxShipmentInvoicePoItemMisMatches
                            .Where(x => x.POId == inv.EntryData_Id || x.INVId == inv.InvoiceId).ToList();

                    invoices.ForEach(x => shipment.ShipmentAttachedInvoices.Add(new ShipmentAttachedInvoices
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

        private static List<ShipmentInvoice> GetShipmentInvoices()
        {
            try
            {


                if (shipmentInvoices == null)
                    using (var ctx = new EntryDataDSContext())
                    {
                        ctx.Database.CommandTimeout = 0;
                        shipmentInvoices = ctx.ShipmentInvoice
                            .Include(x => x.ShipmentRiderInvoice)
                            .Include("ShipmentRiderInvoice.ShipmentRider")
                            .Include("ShipmentRiderInvoice.ShipmentRiderDetails")
                            .Include("InvoiceDetails.ItemAlias")
                            .Include("InvoiceDetails.Volume")
                            //.Include("ShipmentInvoicePOs.POMISMatches")
                            .Include("ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails.INVItems")
                            .Include("ShipmentInvoicePOs.PurchaseOrders.WarehouseInfo")
                            .Include("InvoiceDetails.ItemAlias")
                            .Include("InvoiceDetails.POItems")
                            .Include("ShipmentBLInvoice.ShipmentBL")
                            .Include("ShipmentBLInvoice.ShipmentBLDetails")

                            .ToList();

                        //var invItems = ctx.ShipmentInvoicePOItemQueryMatches.ToList();

                        //foreach (var itm in shipmentInvoices.SelectMany(inv => inv.ShipmentInvoicePOs.SelectMany(x => x.PurchaseOrders.EntryDataDetails.Where(z => !z.INVItems.Any())).ToList()))
                        //{
                        //    itm.INVItems = invItems.Where(x => x.PODetailsId == itm.EntryDataDetailsId).ToList();
                        //}
                    }

                return shipmentInvoices;
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
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .Include(z => z.ShipmentRiderManifests)
                        .Include(z => z.ShipmentRiderEx)
                    .Where(x => x.EmailId == shipment.EmailId)
                    .ToList();
                   

                shipment.ShipmentAttachedRider.AddRange(riders.Select(x => new ShipmentAttachedRider
                {
                    ShipmentRider = x,
                    Shipment = shipment,
                    ShipmentId = x.Id,
                    RiderId = x.Id,
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
                        .Include(z => z.ShipmentRiderManifests)
                        .Where(x => x.EmailId == shipment.EmailId)
                        .ToList();

                    shipment.ShipmentAttachedManifest.AddRange(manifests.Select(x => new ShipmentAttachedManifest
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
                    .Include(x => x.ShipmentFreightManifests)
                    .Where(x => x.EmailId == shipment.EmailId).AsEnumerable()
                    .DistinctBy(x => x.Id)
                    .ToList();

                shipment.ShipmentAttachedFreight.AddRange(freights.Select(x => new ShipmentAttachedFreight
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
                        .SelectMany(x =>
                            x.ShipmentRider.ShipmentRiderInvoice.Select(z => z.InvoiceId).Where(z => z != null))
                        .Distinct()
                        .Where(x => shipment.ShipmentAttachedInvoices.All(z => z.ShipmentInvoiceId != x))
                        .Select(x => GetShipmentInvoices().First(z => z.Id == x))
                        .Select(x => new ShipmentAttachedInvoices
                        {
                            Shipment = shipment,
                            ShipmentInvoice = x,
                            ShipmentInvoiceId = x.Id,
                            TrackingState = TrackingState.Added
                        })
                        .ToList();


                    var blInvoices = shipment.ShipmentAttachedBL
                        .SelectMany(x =>
                            x.ShipmentBL.ShipmentBLInvoice.Select(z => z.InvoiceId).Where(z => z != null))
                        .Distinct()
                        .Where(x => shipment.ShipmentAttachedInvoices.All(z => z.ShipmentInvoiceId != x))
                        .Select(x => GetShipmentInvoices().First(z => z.Id == x))
                        .Select(x => new ShipmentAttachedInvoices
                        {
                            Shipment = shipment,
                            ShipmentInvoice = x,
                            ShipmentInvoiceId = x.Id,
                            TrackingState = TrackingState.Added
                        })
                        .ToList();

                    invoices.AddRange(blInvoices);


                    var refInvoices = shipment.ShipmentAttachedRider
                        .SelectMany(x =>
                            x.ShipmentRider.ShipmentRiderDetails.Select(z => z.InvoiceNumber).Where(z => z != null))
                        .Distinct()
                        .Where(x => invoices.All(z => z.ShipmentInvoice.InvoiceNo != x))
                        .SelectMany(x => GetShipmentInvoices().Where(z => z.InvoiceNo == x))
                       .Select(x => new ShipmentAttachedInvoices
                        {
                            Shipment = shipment,
                            ShipmentInvoice = x,
                            ShipmentInvoiceId = x.Id,
                            TrackingState = TrackingState.Added
                        })
                        .ToList();

                    invoices.AddRange(refInvoices);

                    var shipmentInvoicePoItemMisMatcheses = ctx.ShipmentInvoicePOItemMISMatches.ToList();
                    foreach (var inv in invoices.SelectMany(x => x.ShipmentInvoice.ShipmentInvoicePOs))
                        inv.POMISMatches = shipmentInvoicePoItemMisMatcheses
                            .Where(x => x.POId == inv.EntryData_Id || x.INVId == inv.InvoiceId).ToList();

                    var emailLst = invoices.Select(x => x.ShipmentInvoice.EmailId).Distinct().ToList();
                    var invoiceLst = invoices.Select(x => x.ShipmentInvoice.Id).Distinct().ToList();

                  

                    var inattachedInvoices = GetShipmentInvoices()
                        .Where(z => !invoiceLst.Contains(z.Id) && emailLst.Contains(z.EmailId))
                        .ToList()
                        .Select(x => new ShipmentAttachedInvoices
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
            using (var ctx = new EntryDataDSContext())
            {
                var bls = GetShipmentBls()
                    .Where(x => x.EmailId == shipment.EmailId)
                    //.Select(x =>
                    //{
                    //    x.ShipmentBLDetails = Utils.CreatePackingList(x);
                    //    return x;
                    //})
                    .ToList();


                shipment.ShipmentAttachedBL.AddRange(bls.Select(x => new ShipmentAttachedBL
                {
                    ShipmentBL = x,
                    Shipment = shipment,
                    TrackingState = TrackingState.Added
                }));
            }

            return shipment;
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
                    .Where(x => x != 0)
                    .Select(x => ctx.ShipmentRider
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .Include(z => z.ShipmentRiderManifests)
                        .Include(z => z.ShipmentRiderEx)
                        .First(z => z.Id == x))
                    .MaxBy(x => x.EmailId)
                    .ToList();
                riders.AddRange(invoiceRiders);
                newriders.AddRange(invoiceRiders);

                var blRiders = shipment.ShipmentAttachedBL
                    .SelectMany(x => x.ShipmentBL.ShipmentRiderBLs.Select(z => z.RiderId))
                    .Distinct()
                    .Where(x => riders.All(z => z.Id != x))
                    .Where(x => x != 0)
                    .Select(x => ctx.ShipmentRider
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .Include(z => z.ShipmentRiderManifests)
                        .Include(z => z.ShipmentRiderEx)
                        .First(z => z.Id == x))
                    .MaxBy(x => x.EmailId)
                    .ToList();

                riders.AddRange(blRiders);
                newriders.AddRange(blRiders);

                var manifestRiders = shipment.ShipmentAttachedManifest
                    .SelectMany(x => x.ShipmentManifest.ShipmentRiderManifests.Select(z => z.RiderId))
                    .Distinct()
                    .Where(x => riders.All(z => z.Id != x))
                    .Where(x => x != 0)
                    .Select(x => ctx.ShipmentRider
                        .Include(z => z.ShipmentRiderBLs)
                        .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                        .Include(z => z.ShipmentRiderInvoice)
                        .Include(z => z.ShipmentRiderManifests)
                        .Include(z => z.ShipmentRiderEx)
                        .First(z => z.Id == x))
                    .MaxBy(x => x.EmailId)
                    .ToList();

                riders.AddRange(manifestRiders);
                newriders.AddRange(manifestRiders);


                if (!riders.Any())
                {
                    var poRiders = shipment.ShipmentAttachedPOs
                        .SelectMany(x => x.PurchaseOrders.ShipmentInvoicePOs.Select(z => z.InvoiceId))
                        .Distinct()
                        .SelectMany(x =>
                            GetShipmentInvoices().Where(i => i.Id == x)
                                .SelectMany(i => i.ShipmentRiderInvoice.Select(ri => ri.RiderID)).ToList()).Distinct()
                        .ToList()
                        .Where(x => riders.All(z => z.Id != x))
                        .Where(x => x != 0)
                        .Select(x => ctx.ShipmentRider
                            .Include(z => z.ShipmentRiderBLs)
                            .Include("ShipmentRiderDetails.ShipmentRiderBLs")
                            .Include(z => z.ShipmentRiderInvoice)
                            .Include(z => z.ShipmentRiderManifests)
                            .Include(z => z.ShipmentRiderEx)
                            .First(z => z.Id == x))
                        .OrderByDescending(r => r.Id)
                        .MaxBy(x => x.EmailId)
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

                shipment.ShipmentAttachedRider.AddRange(newriders.Where(x => x.Id != 0).Select(x => new ShipmentAttachedRider
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
                            .Include(z => z.ShipmentFreightManifests)
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
                            .Include(z => z.ShipmentFreightManifests)
                            .Where(z => z.Id == x.FreightInvoiceId))
                        .DistinctBy(x => x.Id)
                        .ToList();
                    freightList.AddRange(frightBls);

                    var manifestFreight = shipment.ShipmentAttachedManifest
                        .SelectMany(x => x.ShipmentManifest.ShipmentFreightManifests)
                        .DistinctBy(x => x.FreightId)
                        .Where(x => freightList.All(z => z.Id != x.FreightId))
                        .SelectMany(x => ctx.ShipmentFreight
                            .Include("ShipmentFreightDetails.ShipmentFreightBLs.ShipmentBLDetails")
                            .Include(z => z.ShipmentBLFreight)
                            .Include(z => z.ShipmentFreightManifests)
                            .Where(z => z.Id == x.FreightId))
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
                            .Include(z => z.ShipmentFreightManifests)
                            .Where(z => z.ShipmentFreightDetails.Any(f => f.WarehouseCode == x.WarehouseCode))
                            .ToList()
                            .DistinctBy(f => f.Id)
                            .Where(q => freightList.All(z => z.Id != x.Id)));
                    freightList.AddRange(riderfrightDetail);


                    shipment.ShipmentAttachedFreight.AddRange(freightList.Select(x => new ShipmentAttachedFreight
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
                
                    var manifests = shipment.ShipmentAttachedManifest.Select(x => x.ShipmentManifest).ToList();

                    manifests.AddRange(GetBlManifests(shipment).Where(x => manifests.All(z => z.Id != x.Id)));

                    manifests.AddRange(GetRiderManifests(shipment).Where(x => manifests.All(z => z.Id != x.Id)));

                    manifests.AddRange(GetFreightManifests(shipment).Where(x => manifests.All(z => z.Id != x.Id)));


                    shipment.ShipmentAttachedManifest.AddRange(manifests
                        .Where(x => shipment.ShipmentAttachedManifest.All(z => z.ManifestId != x.Id)).Select(x =>
                            new ShipmentAttachedManifest
                            {
                                ShipmentManifest = x,
                                ManifestId = x.Id,
                                Shipment = shipment,
                                TrackingState = TrackingState.Added
                            }));
                

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<ShipmentManifest> GetRiderManifests(Shipment shipment)
        {
            var riderManifests = shipment.ShipmentAttachedRider
                .SelectMany(x => x.ShipmentRider.ShipmentRiderManifests)
                .DistinctBy(x => x.ManifestId)
                .Where(x => x.ManifestId != 0)
                .Select(x => GetShipmentManifests()
                    .First(z => z.Id == x.ManifestId))
                .ToList();
            return riderManifests;
        }

        private static List<ShipmentManifest> GetFreightManifests(Shipment shipment)
        {
            var freightManifests = shipment.ShipmentAttachedFreight
                .SelectMany(x => x.ShipmentFreight.ShipmentFreightManifests)
                .DistinctBy(x => x.ManifestId)
                .Where(x => x.ManifestId != 0)
                .Select(x => GetShipmentManifests()
                    .First(z => z.Id == x.ManifestId))
                .ToList();
            return freightManifests;
        }

        private static List<ShipmentManifest> GetBlManifests(Shipment shipment)
        {
           
            var blManifests = shipment.ShipmentAttachedBL
                .SelectMany(x => x.ShipmentBL.ShipmentManifestBLs)
                .DistinctBy(x => x.ManifestId)
                .Where(x => x.Id != 0)
                .Select(x => GetShipmentManifests()
                    .First(z => z.Id == x.ManifestId))
                .ToList();
            return blManifests;
        }

        private static List<ShipmentManifest> shipmentManifests = null;
        private static List<ShipmentManifest> GetShipmentManifests()
        {
            if (shipmentManifests != null) return shipmentManifests;

            using (var ctx = new EntryDataDSContext())
            {
                
                shipmentManifests = ctx.ShipmentManifest
                    .Include("ShipmentManifestBLs.ShipmentBL")
                    .Include(z => z.ShipmentRiderManifests)
                    .ToList();
            }
            return shipmentManifests;
        }

        public static Shipment LoadDBBL(this Shipment shipment)
        {
            try
            {
                
                    var bls = shipment.ShipmentAttachedBL.Select(x => x.ShipmentBL).ToList();
                    
                    bls.AddRange(GetRiderBls(shipment).Where(x => bls.All(z => z.BLNumber != x.BLNumber)));

                    bls.AddRange(GetFrightBls(shipment).Where(x => bls.All(z => z.BLNumber != x.BLNumber)));

                    bls.AddRange(GetFrightDetailBls(shipment).Where(x => bls.All(z => z.BLNumber != x.BLNumber)));

                    bls.AddRange(GetManifestBls(shipment).Where(x => bls.All(z => z.BLNumber != x.BLNumber)));

                    bls.AddRange(GetInvoiceBls(shipment).Where(x => bls.All(z => z.BLNumber != x.BLNumber)));


                    shipment.ShipmentAttachedBL = new List<ShipmentAttachedBL>(bls.Select(x => new ShipmentAttachedBL
                    {
                        ShipmentBL = x,
                        Shipment = shipment,
                        TrackingState = TrackingState.Added
                    }));

                    
              

                return shipment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<ShipmentBL> GetInvoiceBls(Shipment shipment)
        {
            var invoiceBls = shipment.ShipmentAttachedInvoices
                .SelectMany(x => x.ShipmentInvoice.ShipmentBLInvoice.Select(z => z.ShipmentBLDetails))
                .DistinctBy(x => x.ShipmentBL.BLNumber)
                .Where(x => x.Id != 0)
                .SelectMany(x => GetShipmentBls()
                    .Where(z => z.BLNumber == x.ShipmentBL.BLNumber && z.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                //.Select(x =>
                //{
                //    x.ShipmentBLDetails = Utils.CreatePackingList(x);
                //    return x;
                //})
                .ToList();
            return invoiceBls;
        }

        private static List<ShipmentBL> GetManifestBls(Shipment shipment)
        {
            var manifestBls = shipment.ShipmentAttachedManifest
                .Where(x => x.ManifestId != 0)
                .Select(x => x.ShipmentManifest.WayBill)
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .SelectMany(x => GetShipmentBls()
                    .Where(z => z.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && (z.BLNumber.Contains(x) || x.Contains(z.BLNumber)))) // &&
                //.Select(x =>
                //{
                //    x.ShipmentBLDetails = Utils.CreatePackingList(x);
                //    return x;
                //})
                .ToList();
            return manifestBls;
        }

        private static List<ShipmentBL> GetFrightDetailBls(Shipment shipment)
        {
            var frightDetailBls = shipment.ShipmentAttachedFreight
                .SelectMany(x => x.ShipmentFreight.ShipmentFreightDetails.SelectMany(z => z.ShipmentFreightBLs))
                .DistinctBy(x => x.BLNumber)
                .Where(x => x.Id != 0)
                .SelectMany(x => GetShipmentBls()
                    .Where(z => z.BLNumber == x.BLNumber && z.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                .Select(x =>
                {
                    x.ShipmentBLDetails = Utils.CreatePackingList(x);
                    return x;
                })
                .ToList();
            return frightDetailBls;
        }

        private static List<ShipmentBL> GetFrightBls(Shipment shipment)
        {
            var frightBls = shipment.ShipmentAttachedFreight
                .Select(x => x.ShipmentFreight.BLNumber)
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .SelectMany(x => GetShipmentBls()
                    .Where(z => z.BLNumber == x && z.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                //.Select(x =>
                //{
                //    x.ShipmentBLDetails = Utils.CreatePackingList(x);
                //    return x;
                //})
                .ToList();
            return frightBls;
        }

        private static List<ShipmentBL> GetRiderBls(Shipment shipment)
        {
            var riderBls = shipment.ShipmentAttachedRider
                .SelectMany(x => x.ShipmentRider.ShipmentRiderBLs.Select(z => z.BLId))
                .Distinct()
                .Where(x => shipment.ShipmentAttachedBL.All(z => z.ShipmentBL.Id != x))
                .Where(x => x != 0)
                .Select(x => GetShipmentBls()
                    .First(z => z.Id == x && z.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                //.Select(x =>
                //{
                //    x.ShipmentBLDetails = Utils.CreatePackingList(x);
                //    return x;
                //})
                .ToList();
            return riderBls;
        }

        private static List<ShipmentBL> shipmentBls = null;

        private static List<ShipmentBL> GetShipmentBls()
        {
            if (shipmentBls != null) return shipmentBls;

            using (var ctx = new EntryDataDSContext())
            {
                shipmentBls = ctx.ShipmentBL
                    .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentBLDetails")
                    .Include(
                        "ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRider.ShipmentRiderEx")
                    .Include("ShipmentBLDetails.ShipmentRiderBLs.ShipmentRiderDetails.ShipmentRiderInvoice")
                    .Include("ShipmentBLDetails.ShipmentFreightBLs.ShipmentFreightDetails")
                    .Include("ShipmentBLInvoice.ShipmentInvoice")
                    .Include(z => z.ShipmentBLFreight)
                    .Include(z => z.ShipmentRiderBLs)
                    .Include(z => z.ShipmentManifestBLs)
                    .Include(z => z.ShipmentAttachedBL)
                    .ToList();
            }

            return shipmentBls;
        }

        public static Shipment AutoCorrect(this Shipment masterShipment)
        {

            return masterShipment;
        }


        public static List<Shipment> ProcessShipment(this Shipment masterShipment)
        {
            try
            {
                
                var shipments = new List<Shipment>();

                BlandRiderCreator.CreateShipmentFromBLsAndRider(masterShipment, shipments);

                CreateShipmentFromBLsNoRider(masterShipment, shipments);

                CreateShipmentFromNoBLAndRider(masterShipment,  shipments);

                CreateShipmentNoBLNoRider(masterShipment, shipments);

                return shipments.Where(x => x.ShipmentAttachments.Any() && (x.ExpectedEntries > 0 || x.TotalInvoices > 0)).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateShipmentFromBLsNoRider(Shipment masterShipment, List<Shipment> shipments)
        {
            if (!masterShipment.ShipmentAttachedBL.Any(x => x.ShipmentBL.ShipmentBLDetails.Any())  || masterShipment.ShipmentAttachedRider.Any()) return;

            var HasMasterBL = false;
            var bls = masterShipment.ShipmentAttachedBL.Select(x => x.ShipmentBL)
                .OrderByDescending(x => x.ShipmentBLDetails.Count).ToList();
            ShipmentBL masterBL = null;
            bool sendMaster = true; 
            if (HasMasterBL && bls.Count > 1)
                foreach (var bl in bls.OrderBy(x => x.BLNumber))
                {
                    var otherBls = bls.Where(x => x.BLNumber != bl.BLNumber)
                        .SelectMany(x => x.ShipmentBLDetails.Select(z => z.Marks)).ToList();
                    var marks = bl.ShipmentBLDetails.Select(x => x.Marks).ToList();
                    var matches = otherBls.Where(x =>
                        marks.Any(z => z == x /*z.ToCharArray().Except(x.ToCharArray()).Any()*/)).ToList();
                    if (matches.Count() == otherBls.Count())
                    {
                        masterBL = bl;
                        break;
                    }
                }

            if (HasMasterBL && masterBL != null)
                masterBL.SourceFile =
                    BaseDataModel.SetFilename(masterBL.SourceFile, masterBL.BLNumber, "-MasterBL.pdf");

            foreach (var aBl in masterShipment.ShipmentAttachedBL.Where(x =>
                         !HasMasterBL || (sendMaster || x.ShipmentBL.BLNumber != masterBL?.BLNumber)).ToList())
            {
                var bl = aBl.ShipmentBL;
                //var riderDetails = new List<ShipmentRiderDetails>();

                var packingDetails = Utils.CreatePackingList(bl)
                    .Where(x => x.Quantity > 0)
                    .Select(x => (x.Marks, Packages: x.Quantity, x.InvoiceNumber))
                    .ToList();
                if(!packingDetails.Any())  packingDetails = masterShipment.ShipmentAttachedInvoices
                    .Select(x => (WarehouseCode: "Marks", Packages: 1, x.ShipmentInvoice.InvoiceNo)).ToList();

                var blManifests = masterShipment.ShipmentAttachedManifest
                    .Where(x => x.ShipmentManifest.WayBill == bl.BLNumber || x.ShipmentManifest.WayBill.Contains(bl.BLNumber) || bl.BLNumber.Contains(x.ShipmentManifest.WayBill)).Select(x => x.ShipmentManifest)
                    .DistinctBy(x => x.Id)
                    .ToList();


                var manifests = masterShipment.ShipmentAttachedManifest
                    .Where(x => (x.ShipmentManifest.WayBill == bl.BLNumber || x.ShipmentManifest.WayBill.Contains(bl.BLNumber) || bl.BLNumber.Contains(x.ShipmentManifest.WayBill)) ||
                                (!IsNullOrEmpty(bl.Voyage) && x.ShipmentManifest.Voyage.Contains(bl.Voyage)))
                    .Select(x => x.ShipmentManifest)
                    .OrderByDescending(x => (x.WayBill == bl.BLNumber || x.WayBill.Contains(bl.BLNumber) ||  bl.BLNumber.Contains(x.WayBill)))
                    .ToList();


                var freightInvoicesbyBL = masterShipment.ShipmentAttachedFreight
                    .Where(x => x.ShipmentFreight.ShipmentBLFreight.Any(f => f.BLNumber == bl.BLNumber))
                    .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();

                var freightInvoices = new List<ShipmentFreight>();
                freightInvoices.AddRange(freightInvoicesbyBL);


                var blDetails = bl.ShipmentBLDetails
                    .SelectMany(x => x.ShipmentRiderBLs.Select(z => z.ShipmentBLDetails)).DistinctBy(x => x.Id)

                    .ToList();


                var shipment = new Shipment
                {
                    ShipmentName =
                        $"{bl.BLNumber.Split('-').First()}",
                    ManifestNumber = manifests.LastOrDefault()?.RegistrationNumber,
                    BLNumber = manifests.FirstOrDefault()?.WayBill ?? bl?.BLNumber,
                    WeightKG = !manifests.Any()
                        ? bl?.WeightKG
                        : manifests.FirstOrDefault()?.GrossWeightKG ?? bl?.WeightKG,
                    ExpectedEntries = 0,
                    TotalInvoices = 0,
                    FreightCurrency = freightInvoices.LastOrDefault()?.Currency ?? bl.FreightCurrency ?? "USD",
                    // dont understand why i checking manifest if there is no freight invoice
                    //Freight = freightInvoices.LastOrDefault()?.InvoiceTotal ?? (bl?.BLNumber ==  manifests.LastOrDefault()?.WayBill || manifests.Any(x => x.Voyage.Contains(bl.Voyage)) ?  bl.Freight : 0),
                    Freight = freightInvoices.LastOrDefault()?.InvoiceTotal ?? bl.Freight,
                    Origin = "US",
                    Packages = manifests.FirstOrDefault()?.Packages ?? (bl.PackagesNo),

                    Location = manifests.FirstOrDefault()?.LocationOfGoods,
                    Office = manifests.FirstOrDefault()?.CustomsOffice,
                    TrackingState = TrackingState.Added
                };


                var attachments = new List<Attachments>();


                //var unattachedShippment = masterShipment.CreateUnattachedShipment(client.Key);
                //shipments.Add(unattachedShippment);
                var summaryPkg = new UnAttachedWorkBookPkg
                {
                    Reference = $"{masterShipment.EmailId}"
                };

                var allShipmentRiderDetails = new List<ShipmentRiderDetails>();

                
                var invoices = GetInvoicesAndCreateSummaryFile(masterShipment, null, bl.ShipmentBLDetails, allShipmentRiderDetails, packingDetails,
                    summaryPkg, out var summaryFile);

               

                attachments.Add(new Attachments
                {
                    FilePath = summaryFile,
                    DocumentCode = "NA",
                    Reference = "Summary",
                    TrackingState = TrackingState.Added
                });

                shipment.Currency = "USD"; //invoices.Select(x => x.Currency).FirstOrDefault() ??

                //if (shipment.Currency.Length != 3) throw new ApplicationException("Currency must be 3 letters");


                shipment.ExpectedEntries += invoices.Count(); //invoices.Select(x => x.Id).Count(),
                shipment.TotalInvoices += invoices.Select(x => x.Id).Count();



               


                var invoiceAttachments = invoices
                    .GroupBy(x => x.ShipmentBLInvoice.FirstOrDefault()?.WarehouseCode ?? "")
                    .Select(shipmentInvoice =>
                        XlsxWriter.CreateCSV(shipmentInvoice.Key,
                            shipmentInvoice.OrderByDescending(z =>
                                z.ShipmentBLInvoice.FirstOrDefault()?.Packages ?? 0).ToList(),
                            masterShipment.EmailId, packingDetails))
                    .SelectMany(x => x.ToList())
                    .ToList();

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
                shipment.ShipmentAttachedInvoices.AddRange(invoices.Select(z => new ShipmentAttachedInvoices
                {
                    ShipmentInvoice = z,
                    Shipment = shipment,
                    TrackingState = TrackingState.Added
                }).ToList());


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
                if (masterBL != null)
                    attachments.Add(new Attachments
                    {
                        FilePath = masterBL.SourceFile,
                        DocumentCode = "BL05",
                        Reference = bl.BLNumber,
                        TrackingState = TrackingState.Added
                    });


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

                shipments.Add(shipment);
            }
        }


        private static void CreateShipmentFromNoBLAndRider(Shipment masterShipment,  List<Shipment> shipments)
        {
            if ( masterShipment.ShipmentAttachedBL.Any(x => x.ShipmentBL.ShipmentBLDetails.Any()) || !masterShipment.ShipmentAttachedRider.Any()) return;

            var bls = masterShipment.ShipmentAttachedBL.Select(x => x.ShipmentBL)
                .OrderByDescending(x => x.ShipmentBLDetails.Count).ToList();
            var ridersWithNoBLs = masterShipment.ShipmentAttachedRider.Where(x =>
                x.ShipmentRider.ShipmentRiderDetails.Any(z =>
                    !z.ShipmentRiderBLs.Any(b => bls.Any(r => r.Id == b.BLId)))).ToList();
            if (ridersWithNoBLs.Any())
                //TODO:// Copy the system above
                foreach (var sRider in ridersWithNoBLs)
                {
                    var rider = sRider.ShipmentRider;

                    var clients = rider.ShipmentRiderDetails///Utils.CreatePackingList(rider)
                        .Where(z => !z.ShipmentRiderBLs.Any(b => bls.All(r => r.Id != b.BLId)))
                        .GroupBy(x => (x.Code, x.RiderId, "Next Shipment"))
                        .ToList().ToList();

                    foreach (IGrouping<(string Code, int RiderId, string BLNumber), ShipmentRiderDetails> client in clients)
                    {
                        var summaryPkg = new UnAttachedWorkBookPkg
                        {
                            Reference = $"{masterShipment.EmailId}"
                        };

                        ;
                       var packingDetails = rider.ShipmentRiderInvoice
                            .Where(x => x.ShipmentRiderDetails != null &&  x.Packages > 0)
                            .Select(x => (x.WarehouseCode, Packages: x.Packages ?? 0, x.InvoiceNo))
                           .ToList();
                    

                        var freightInvoices = masterShipment.ShipmentAttachedFreight
                            .Where(x => x.ShipmentFreight.ShipmentFreightDetails.Any(z =>
                                client.Any(q => q.WarehouseCode == z.WarehouseCode)))
                            .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();

                        var manifests = masterShipment.ShipmentAttachedManifest
                            .Where(x => x.ShipmentManifest.ShipmentRiderManifests.Any(z => z.RiderId == rider.Id))
                            .Select(x => x.ShipmentManifest)
                            .ToList();

                        var invoices = GetInvoicesAndCreateSummaryFile(masterShipment, client,new List<ShipmentBLDetails>(), rider.ShipmentRiderDetails, packingDetails, summaryPkg, out var summaryFile);

                        var attachments = new List<Attachments>
                        {
                            new Attachments
                            {
                                FilePath = summaryFile,
                                DocumentCode = "NA",
                                Reference = "Summary",
                                TrackingState = TrackingState.Added
                            }
                        };


                        var invocieAttachments = invoices
                            .GroupBy(x => x.ShipmentRiderInvoice.FirstOrDefault()?.WarehouseCode ?? "")
                            .Select(shipmentInvoice =>
                                XlsxWriter.CreateCSV(shipmentInvoice
                                        .OrderBy(z => z.SupplierCode)
                                        //.OrderByDescending(z => z.ShipmentRiderInvoice.FirstOrDefault()?.Packages ?? 0)
                                        .OrderBy(r => r.ShipmentRiderInvoice.FirstOrDefault(q => q.InvoiceNo == r.InvoiceNo)?.RiderLineID ??0)
                                        .ToList(),
                                    client.Key.RiderId,
                                    packingDetails.Where(z => shipmentInvoice.Any(s => s.InvoiceNo == z.InvoiceNo)).ToList()))
                            .SelectMany(x => x.ToList())
                            .ToList();


                        attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".pdf")).Select(x =>
                            new Attachments
                            {
                                FilePath = x.filepath,
                                DocumentCode = "IV05",
                                Reference = x.reference,
                                TrackingState = TrackingState.Added
                            }));
                        attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".xlsx")).Select(x =>
                            new Attachments
                            {
                                FilePath = x.filepath,
                                DocumentCode = "NA",
                                Reference = x.reference,
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
                            FilePath = rider.SourceFile,
                            DocumentCode = "NA",
                            Reference = "Rider",
                            TrackingState = TrackingState.Added
                        });

                        var bl = masterShipment.ShipmentAttachedBL.FirstOrDefault(x =>
                            x.ShipmentBL.EmailId == masterShipment.EmailId)?.ShipmentBL;
                        if (bl != null)
                            attachments.Add(new Attachments
                            {
                                FilePath = bl.SourceFile,
                                DocumentCode = "BL10",
                                Reference = bl.BLNumber,
                                TrackingState = TrackingState.Added
                            });

                        var shipment = new Shipment
                        {
                            ShipmentName = client.Key.Code.Split(' ').FirstOrDefault(),
                            ManifestNumber = manifests.LastOrDefault()?.RegistrationNumber ,
                            BLNumber = manifests.LastOrDefault()?.WayBill ?? bl?.BLNumber,
                            WeightKG = bl?.WeightKG ?? client.Sum(x => x.GrossWeightKg) ,
                            Currency = "USD", //invoices.Select(x => x.Currency).FirstOrDefault() ??
                            ExpectedEntries = invoices.Count(x =>
                                (x.ShipmentRiderInvoice.FirstOrDefault()
                                    ?.Packages ?? 0) > 0),
                            TotalInvoices = invoices.Select(x => x.Id).Count(),
                            FreightCurrency = freightInvoices.LastOrDefault()?.Currency ?? "USD",
                            Freight =  bl?.Freight ?? freightInvoices.LastOrDefault()?.InvoiceTotal,
                            Origin = "US",
                            Packages = client.Sum(x => x.Pieces),
                            Location = manifests.LastOrDefault()?.LocationOfGoods,
                            Office = manifests.LastOrDefault()?.CustomsOffice,
                            TrackingState = TrackingState.Added
                        };
                        //if (shipment.Currency.Length != 3) throw new ApplicationException("Currency must be 3 letters");
                        shipment.ShipmentAttachedInvoices.AddRange(invoices.Select(z =>
                            new ShipmentAttachedInvoices
                            {
                                ShipmentInvoice = z,
                                Shipment = shipment,
                                TrackingState = TrackingState.Added
                            }).ToList());
                        shipment.ShipmentAttachedRider.AddRange(new List<ShipmentAttachedRider>
                        {
                            new ShipmentAttachedRider
                            {
                                Shipment = shipment,
                                ShipmentRider = rider,
                                TrackingState = TrackingState.Added
                            }
                        });
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

                        shipments.Add(shipment);
                    }
                }
        }

        private static void CreateShipmentNoBLNoRider(Shipment masterShipment, List<Shipment> shipments)
        {
            if (!masterShipment.ShipmentAttachedRider.Any() && !masterShipment.ShipmentAttachedBL.Any(x => x.ShipmentBL.ShipmentBLDetails.Any()))
            {
                var summaryPkg = new UnAttachedWorkBookPkg
                {
                    Reference = $"{masterShipment.EmailId}",
                    RiderDetails = new List<ShipmentRiderDetails>(),
                    RiderManualMatches = new List<ShipmentInvoiceRiderManualMatches>(),
                    UnMatchedPOs = new List<ShipmentMIS_POs>(),
                    Invoices = new List<ShipmentInvoice>(),
                    UnAttachedInvoices = new List<ShipmentInvoice>(),
                    UnMatchedInvoices = new List<ShipmentMIS_Invoices>()
                };

                ;
              

                var packingDetails = masterShipment.ShipmentAttachedInvoices
                    .Select(x => (WarehouseCode: "Marks", Packages: 1, x.ShipmentInvoice.InvoiceNo)).ToList(); //new List<(string Marks, int Packages, string InvoiceNumber)>();

                var freightInvoices = masterShipment.ShipmentAttachedFreight
                    .DistinctBy(x => x.FreightInvoiceId).Select(x => x.ShipmentFreight).ToList();

                var manifests = masterShipment.ShipmentAttachedManifest
                    .Select(x => x.ShipmentManifest)
                    .ToList();

                var invoices = GetInvoicesAndCreateSummaryFile(masterShipment, null, new List<ShipmentBLDetails>(), new List<ShipmentRiderDetails>(), packingDetails, summaryPkg, out var summaryFile);

                var attachments = new List<Attachments>
                {
                    new Attachments
                    {
                        FilePath = summaryFile,
                        DocumentCode = "NA",
                        Reference = "Summary",
                        TrackingState = TrackingState.Added
                    }
                };

               

                var invocieAttachments = invoices
                    .GroupBy(x => x.ShipmentRiderInvoice.FirstOrDefault()?.WarehouseCode ?? "")
                    .Select(shipmentInvoice =>
                        XlsxWriter.CreateCSV(shipmentInvoice.Key,
                            shipmentInvoice
                                .OrderByDescending(z => z.ShipmentRiderInvoice.FirstOrDefault()?.Packages ?? 0)
                                .ToList(), masterShipment.EmailId, packingDetails))
                    .SelectMany(x => x.ToList())
                    .ToList();


                attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".pdf")).Select(x =>
                    new Attachments
                    {
                        FilePath = x.filepath,
                        DocumentCode = "IV05",
                        Reference = x.reference,
                        TrackingState = TrackingState.Added
                    }));
                attachments.AddRange(invocieAttachments.Where(x => x.filepath.EndsWith(".xlsx")).Select(x =>
                    new Attachments
                    {
                        FilePath = x.filepath,
                        DocumentCode = "NA",
                        Reference = x.reference,
                        TrackingState = TrackingState.Added
                    }));


                attachments.AddRange(freightInvoices.Select(x => new Attachments
                {
                    FilePath = x.SourceFile,
                    DocumentCode = "IV04",
                    Reference = x.InvoiceNumber,
                    TrackingState = TrackingState.Added
                }));

                attachments.AddRange(manifests.Select(x => new Attachments
                {
                    FilePath = x.SourceFile,
                    DocumentCode = "IV04",
                    Reference = x.RegistrationNumber,
                    TrackingState = TrackingState.Added
                }));

                var bl = masterShipment.ShipmentAttachedBL.FirstOrDefault(x =>
                    x.ShipmentBL.EmailId == masterShipment.EmailId)?.ShipmentBL;
                if(bl != null)
                    attachments.Add(new Attachments
                    {
                        FilePath = bl.SourceFile,
                        DocumentCode = "BL10",
                        Reference = bl.BLNumber,
                        TrackingState = TrackingState.Added
                    });

                Shipment manifestShipments(ShipmentManifest manifest) =>
                    new Shipment
                    {
                        ShipmentName = "NextShipment",
                        ManifestNumber = manifest.RegistrationNumber,
                        BLNumber = manifest.WayBill ?? bl?.BLNumber,
                        WeightKG = manifest?.GrossWeightKG ?? bl?.WeightKG,
                        Currency = invoices.Select(x => x.Currency).FirstOrDefault() ?? "USD", //
                        ExpectedEntries = invoices.Count(),
                        TotalInvoices = invoices.Select(x => x.Id).Count(),
                        FreightCurrency = manifest.FreightCurrency ?? freightInvoices.LastOrDefault()?.Currency ?? "USD",
                        Freight = manifest.Freight ?? freightInvoices.LastOrDefault()?.InvoiceTotal ?? bl?.Freight,
                        Origin = "US",
                        Packages = manifest?.Packages ?? bl?.PackagesNo ?? 0,
                        Location = manifest.LocationOfGoods,
                        Office = manifest.CustomsOffice,
                        TrackingState = TrackingState.Added
                    };

                foreach (var shipment in manifests.Select(manifestShipments))
                {
                    //if (shipment.Currency.Length != 3)
                    //    throw new ApplicationException("Currency must be 3 letters");


                    shipments.Add(shipment);
                    
                    shipment.ShipmentAttachments.AddRange(attachments.Select(x =>
                        new ShipmentAttachments
                        {
                            Attachments = x,
                            Shipment = shipment,
                            TrackingState = TrackingState.Added
                        }));
                }


                
            }
        }

        public static List<ShipmentInvoice> GetInvoicesAndCreateSummaryFile(Shipment masterShipment,
            IGrouping<(string Code, int RiderId, string BLNumber), ShipmentRiderDetails> client,
            List<ShipmentBLDetails> shipmentBlDetailsList, List<ShipmentRiderDetails> allShipmentRiderDetailsList,
            List<(string Marks, int Packages, string InvoiceNumber)> packingDetails,
            UnAttachedWorkBookPkg summaryPkg,
            out string summaryFile)
        {
            try
            {
                summaryFile = null;
                var invoiceLst = new List<int>();
                var rawInvoices = masterShipment.ShipmentAttachedInvoices
                    .Where(x =>  (x.ShipmentInvoice.ShipmentInvoiceRiderDetails.Any(z => z.ShipmentRiderDetails.RiderId == client?.Key.RiderId) || !x.ShipmentInvoice.ShipmentInvoiceRiderDetails.Any())
                                || (x.ShipmentInvoice.ShipmentRiderInvoice.Any(z => z.RiderID == client?.Key.RiderId) || !x.ShipmentInvoice.ShipmentRiderInvoice.Any()))
                    .Select(x => x.ShipmentInvoice)
                    .Where(x => !String.Equals(x.InvoiceNo, "Null", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

                var riderInvoices = rawInvoices
                        .DistinctBy(x =>
                        x.InvoiceNo) //because it dropping invoices in email id 'D:\OneDrive\Clients\Portage\Emails\Shipments\679\Amazon-com - Order 112-5376880-7024208.pdf'
                    .Where(x => client != null && client.Select(q => q.Id).Any(q => x.ShipmentRiderInvoice.Any(z => z.RiderLineID == q))) // 
                    .ToList(); 
                
                var unAttachedRiderDetails = client != null
                    ? client.Where(x =>
                       !x.ShipmentRiderInvoice.Any() || x.ShipmentRiderInvoice.Any(z => IsNullOrEmpty(z.InvoiceNo)) ).ToList()
                    : new List<ShipmentRiderDetails>();

                invoiceLst.AddRange(riderInvoices.Select(r => r.Id).ToList());

                var blInvoices = rawInvoices
                    .DistinctBy(x =>
                        x.InvoiceNo) //because it dropping invoices in email id 'D:\OneDrive\Clients\Portage\Emails\Shipments\679\Amazon-com - Order 112-5376880-7024208.pdf'
                    .Where(x =>  shipmentBlDetailsList.SelectMany(q => q.ShipmentBLInvoice)
                        .Any(q => x.ShipmentBLInvoice.Any(z => z.InvoiceId == q.InvoiceId))) // 
                    .ToList();

                invoiceLst.AddRange(blInvoices.Where(x => !invoiceLst.Contains(x.Id)).Select(r => r.Id).ToList());

                if (!invoiceLst.Any())
                    invoiceLst.AddRange(rawInvoices.Where(x => packingDetails.Any(z => z.InvoiceNumber == x.InvoiceNo))
                        .Select(z => z.Id)
                        .ToList());

                var invoices = rawInvoices.DistinctBy(x => x.InvoiceNo).Where(x => invoiceLst.Any(z => z == x.Id)).ToList();

                var unAttachedInvoices = rawInvoices
                    .Where(x => !invoiceLst.Contains(x.Id))
                    .Where(x => x.Id > 6155)// just trying to limit useless recommendations
                    .ToList().DistinctBy(x => x.InvoiceNo).ToList(); // distinct by bug

             




                var allUnMatchedInvoices = new EntryDataDSContext().ShipmentMIS_Invoices
                    .Where(x => invoiceLst.Any(z => z == x.Id)).ToList();

                List<ShipmentMIS_POs> allUnMatchedPOs = new List<ShipmentMIS_POs>();
                allUnMatchedPOs = new EntryDataDSContext().ShipmentMIS_POs.ToList();

               var repeatInvoices = new EntryDataDSContext().ShipmentErrors_RepeatInvoices
                   .Where(x => invoiceLst.Any(z => z == x.InvoiceId)).ToList();

               

                var invoiceNOs = invoices.Select(r => r.InvoiceNo).ToList();
                invoiceNOs.AddRange(unAttachedInvoices.Select(x => x.InvoiceNo));

                var repeatMarks = client == null? new List<ShipmentErrors_RepeatMarks>()
                   :new EntryDataDSContext().ShipmentErrors_RepeatMarks
                   .Where(x => x.BLNumber == client.Key.BLNumber || x.RiderID == client.Key.RiderId).ToList();

                var unMatchedBLDetails = client == null 
                        ? new List<ShipmentBLDetails>() 
                        : shipmentBlDetailsList.Where(x => client.All(z => z.WarehouseCode.ToUpper() != x.Marks.ToUpper()))
                            .ToList();
                unMatchedBLDetails.AddRange( shipmentBlDetailsList.Where(x => x.ShipmentBLInvoice.Any(z => string.IsNullOrEmpty(z.InvoiceNo) )).ToList());



                var unMatchedRiderDetails = allShipmentRiderDetailsList.Where(x => shipmentBlDetailsList.All(z => z.Marks.ToUpper() != x.WarehouseCode.ToUpper()))
                    .ToList();


                var poNOs = invoices.SelectMany(r => r.ShipmentInvoicePOs.Select(z => z.PurchaseOrders.PONumber))
                    .ToList();

                var rawPOs = rawInvoices.SelectMany(x => x.ShipmentInvoicePOs.Select(z => z.PurchaseOrders)).ToList();
                var poEmailIds = rawPOs.Select(x => x.EmailId).Distinct();
                var unMatchEmailPOs = new EntryDataDSContext().ShipmentPOs.Where(x => poEmailIds.Contains(x.EmailId) && !poNOs.Contains(x.InvoiceNo)).ToList();
                allUnMatchedPOs.AddRange(unMatchEmailPOs.Select(x => new ShipmentMIS_POs()
                {
                    EntryData_Id = x.EntryData_Id,
                    ImportedLines = x.ImportedLine,
                    InvoiceDate = x.InvoiceDate,
                    InvoiceNo = x.InvoiceNo,
                    InvoiceTotal = x.InvoiceTotal,
                    InvoiceId = x.EntryData_Id,
                    SourceFile = x.SourceFile,
                    SupplierCode = x.SupplierCode,
                    SubTotal = x.SubTotal,
                }));

                poNOs.AddRange(allUnMatchedPOs.Select(x => x.InvoiceNo).ToList());

               


                var classifications = new EntryDataDSContext().ShipmentInvoicePOItemData
                    .Where(x => invoiceNOs.Contains(x.InvoiceNo) || poNOs.Contains(x.PONumber)).ToList();
                summaryPkg.Classifications = classifications;


                summaryPkg.PackingDetails = packingDetails;
                summaryPkg.UnMatchedInvoices = allUnMatchedInvoices;
                summaryPkg.UnMatchedPOs = allUnMatchedPOs;

                summaryPkg.Invoices = invoices;
                summaryPkg.RepeatInvoices = repeatInvoices;
                summaryPkg.RepeatMarks = repeatMarks;
                summaryPkg.UnMatchedBLDetails = unMatchedBLDetails;
                summaryPkg.UnMatchedRiderDetails = unMatchedRiderDetails;
                summaryPkg.UnAttachedInvoices = unAttachedInvoices;
                summaryPkg.UnAttachedRiderDetails = unAttachedRiderDetails;
                summaryPkg.RiderManualMatches = new List<ShipmentInvoiceRiderManualMatches>();
                summaryPkg.RiderDetails = new List<ShipmentRiderDetails>();

                if (client != null)
                {
                    summaryPkg.RiderDetails = client.ToList();

                    var riderMatchKeyCode = client.Select(x => x.WarehouseCode.Trim()).ToList();
                    var riderMatchKeyInv = client.Select(x => x.InvoiceNumber.Trim()).ToList();

                    summaryPkg.RiderSummary = client.First().ShipmentRider.ShipmentRiderEx;

                    summaryPkg.PackagesSummary = new PackagesSummary
                    {
                        BLPackages = shipmentBlDetailsList.DistinctBy(x => x.Marks).Sum(x =>
                            x.Quantity),
                        RiderPackages = client.Sum(x => x.Pieces),
                        InvoicePackages = client.Sum(x =>
                            x.ShipmentRiderInvoice.Where(i => !IsNullOrEmpty(i.InvoiceNo))
                                .Sum(i => i.Packages.GetValueOrDefault()))
                    };


                    summaryPkg.RiderManualMatches = new EntryDataDSContext().ShipmentInvoiceRiderManualMatches
                        .Where(x => riderMatchKeyCode.Any(z => z == x.WarehouseCode) &&
                                    riderMatchKeyInv.Any(z => z == x.RiderInvoiceNumber)).AsEnumerable()
                        .DistinctBy(x => new {x.WarehouseCode, x.RiderInvoiceNumber, x.InvoiceNo})
                        .ToList();

                    summaryFile =
                                        XlsxWriter.CreateUnattachedShipmentWorkBook(
                                            client?.Key ?? ("", 0, "Next Shipment"), summaryPkg);

                }


                if (shipmentBlDetailsList.Any() && string.IsNullOrEmpty(summaryFile))
                {
                    summaryPkg.BlDetails = shipmentBlDetailsList.ToList();

                    var riderMatchKeyCode = shipmentBlDetailsList.Select(x => x.Marks.Trim()).ToList();
                    var riderMatchKeyInv = shipmentBlDetailsList.SelectMany(x => x.ShipmentBLInvoice.Select(z => z.WarehouseCode.Trim())).ToList();

                    //summaryPkg.RiderSummary = client.First().ShipmentRider.ShipmentRiderEx;

                    summaryPkg.PackagesSummary = new PackagesSummary
                    {
                        BLPackages = shipmentBlDetailsList.DistinctBy(x => x.Marks).Sum(x =>
                            x.Quantity),
                        RiderPackages = shipmentBlDetailsList.DistinctBy(x => x.Marks).SelectMany(x => x.ShipmentBLInvoice).Sum(x => x.Packages)??0,
                        InvoicePackages = shipmentBlDetailsList.DistinctBy(x => x.Marks).SelectMany(x => x.ShipmentBLInvoice).Where(x =>
                            !IsNullOrEmpty(x.ShipmentInvoice.InvoiceNo))
                                .Sum(i => i.Packages.GetValueOrDefault())
                    };


                    summaryPkg.BLManualMatches = new EntryDataDSContext().ShipmentInvoiceBLManualMatches
                        .Where(x => riderMatchKeyCode.Any(z => z == x.WarehouseCode) &&
                                    riderMatchKeyInv.Any(z => z == x.BLInvoiceNumber)).AsEnumerable()
                        .DistinctBy(x => new { x.WarehouseCode, x.BLInvoiceNumber, x.InvoiceNo })
                        .ToList();

                    summaryFile =
                        XlsxWriter.CreateUnattachedShipmentWorkBook(
                             ("code", shipmentBlDetailsList.First().ShipmentBL.Id, shipmentBlDetailsList.First().ShipmentBL.BLNumber), summaryPkg);

                }


                if (packingDetails.Any() && string.IsNullOrEmpty(summaryFile))
                {
                    summaryPkg.BlDetails = shipmentBlDetailsList.ToList();

                    var riderMatchKeyCode = packingDetails.Select(x => x.Marks.Trim()).ToList();
                    var riderMatchKeyInv = packingDetails.Select(x => x.Marks.Trim()).ToList();

                    //summaryPkg.RiderSummary = client.First().ShipmentRider.ShipmentRiderEx;

                    summaryPkg.PackagesSummary = new PackagesSummary
                    {
                        BLPackages = packingDetails.DistinctBy(x => x.Marks).Sum(x =>
                            x.Packages),
                        RiderPackages = packingDetails.DistinctBy(x => x.Marks).Sum(x => x.Packages),//shipmentBlDetailsList.DistinctBy(x => x.Marks).SelectMany(x => x.ShipmentBLInvoice).Sum(x => x.Packages) ?? 0,
                        InvoicePackages = packingDetails.DistinctBy(x => x.Marks).Sum(x => x.Packages)
                    };


                    summaryPkg.BLManualMatches = new EntryDataDSContext().ShipmentInvoiceBLManualMatches
                        .Where(x => riderMatchKeyCode.Any(z => z == x.WarehouseCode) &&
                                    riderMatchKeyInv.Any(z => z == x.BLInvoiceNumber)).AsEnumerable()
                        .DistinctBy(x => new { x.WarehouseCode, x.BLInvoiceNumber, x.InvoiceNo })
                        .ToList();

                    summaryFile =
                        XlsxWriter.CreateUnattachedShipmentWorkBook(("", 0, "Next Shipment"), summaryPkg);

                }

                return invoices;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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