﻿using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class ManifestImporter
    {
        static ManifestImporter()
        {
        }

        public ManifestImporter()
        {
        }

        public void ProcessManifest(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst)
        {
            try
            {
               

                var lst = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>) x))
                    .Select(x => new ShipmentManifest()
                    {
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        RegistrationDate = DateTime.Parse(x["RegistrationDate"].ToString()),
                        RegistrationNumber = x["RegistrationNumber"].ToString(),
                        CustomsOffice = x["CustomsOffice"].ToString(),
                        Voyage = x["Voyage"].ToString(),
                        ETD = x.ContainsKey("ETD") ? DateTime.Parse(x["ETD"].ToString()): DateTime.MinValue,
                        ETA = DateTime.Parse(x["ETA"].ToString()),
                        Vessel = x["Vessel"].ToString(),
                        WayBill = x["WayBill"].ToString(),
                        LineNumber = Convert.ToInt32(x["LineNumber"].ToString()),
                        LoadingPort = x.ContainsKey("ETD") ? x["LoadingPort"].ToString():null,
                        ModeOfTransport = x["ModeOfTransport"].ToString(),
                        TypeOfBL = x["TypeOfBL"].ToString(),
                        //CargoReporter = x["CargoReporter"].ToString(),
                        //Exporter = x["Exporter"].ToString(),
                        //Consignee = x["Consignee"].ToString(),
                        //Notify = x["Notify"].ToString(),
                        Packages = Convert.ToInt32(x["Packages"].ToString()),
                        PackageType = x["PackageType"].ToString(),
                        GrossWeightKG = Convert.ToDouble(x["GrossWeightKG"].ToString()),
                        Volume = x.ContainsKey("Volume") ? Convert.ToDouble(x["Volume"].ToString()):0.0,
                        //Freight = Convert.ToDouble(x["Freight"].ToString()),
                        LocationOfGoods = x["LocationOfGoods"].ToString(),
                        Goods = x.ContainsKey("Goods")?x["Goods"].ToString():"",
                        Marks = x.ContainsKey("Marks") ? x["Marks"].ToString() : "",
                        //Containers = Convert.ToInt32(x["Containers"].ToString()),
                        EmailId = emailId,
                        // SourceFile = filename,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var manifest in lst)
                    {
                        var filename = BaseDataModel.SetFilename(droppedFilePath,manifest.WayBill, "-Manifest.pdf");
                        manifest.SourceFile = filename;
                        var existingManifest =
                            ctx.ShipmentManifest.FirstOrDefault(
                                x => x.RegistrationNumber == manifest.RegistrationNumber && x.WayBill == manifest.WayBill);
                        if (existingManifest != null)
                            ctx.ShipmentManifest.Remove(existingManifest);
                        ctx.ShipmentManifest.Add(manifest);
                        var bls = ctx.ShipmentBL
                            .Where(x => x.BLNumber == manifest.WayBill || x.Voyage == manifest.Voyage).ToList();
                        if (bls.Any() && bls.All(x => x.PackagesNo != manifest.Packages))
                        {
                            throw new ApplicationException(
                                $"Manifest:{manifest.RegistrationNumber} Packages <> BL:{bls.Select(x => x.BLNumber).Aggregate((o, n) => o + ", " + n)} Packages");
                        }


                    }

                    ctx.SaveChanges();
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