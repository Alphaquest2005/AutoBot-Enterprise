using System;
using System.Collections.Generic;
using System.Linq;
using EntryDataDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public class SimplifiedDeclarationImporter
{
    public void ProcessSimplifiedDeclaration(DataFile dataFile)
    {
        var lst = ExtractShipmentManifests(dataFile);
        SaveManifest(dataFile, lst);
    }

    private static List<ShipmentManifest> ExtractShipmentManifests(DataFile dataFile)
    {
        var lst = dataFile.Data.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>)x))
            .Select(x => new ShipmentManifest()
            {
                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                //RegistrationDate = DateTime.Parse(x["RegistrationDate"].ToString()),
                RegistrationNumber = $"{x["ManifestYear"]} {x["ManifestNumber"]}",
                CustomsOffice = x["CustomsOffice"].ToString(),
                Consignee = x["Consignee"].ToString(),
                //Voyage = x["Voyage"].ToString(),
                //ETD = x.ContainsKey("ETD") ? DateTime.Parse(x["ETD"].ToString()) : DateTime.MinValue,
                //ETA = DateTime.Parse(x["ETA"].ToString()),
                //Vessel = x["Vessel"].ToString(),
                WayBill = x["BLNumber"].ToString(),
                //LineNumber = Convert.ToInt32(x["LineNumber"].ToString()),
                //LoadingPort = x.ContainsKey("ETD") ? x["LoadingPort"].ToString() : null,
                //ModeOfTransport = x["ModeOfTransport"].ToString(),
                //TypeOfBL = x["TypeOfBL"].ToString(),
                //CargoReporter = x["CargoReporter"].ToString(),
                //Exporter = x["Exporter"].ToString(),
                //Consignee = x["Consignee"].ToString(),
                //Notify = x["Notify"].ToString(),
                Packages = Convert.ToInt32(x["Packages"].ToString()),
                PackageType = x["PackageType"].ToString(),
                GrossWeightKG = Convert.ToDouble(x["GrossWeightKG"].ToString()),
                //Volume = x.ContainsKey("Volume") ? Convert.ToDouble(x["Volume"].ToString()) : 0.0,
                Freight = Convert.ToDouble(x["Freight"].ToString()),
                FreightCurrency = x["FreightCurrency"].ToString(),
                //LocationOfGoods = x["LocationOfGoods"].ToString(),
                //Goods = x.ContainsKey("Goods") ? x["Goods"].ToString() : "",
                //Marks = x.ContainsKey("Marks") ? x["Marks"].ToString() : "",
                //Containers = Convert.ToInt32(x["Containers"].ToString()),
                EmailId = dataFile.EmailId,
                SourceFile = dataFile.DroppedFilePath,
                FileTypeId = dataFile.FileType.Id,
                TrackingState = TrackingState.Added,

            }).ToList();
        return lst;
    }

    private static void SaveManifest(DataFile dataFile, List<ShipmentManifest> lst)
    {
        using (var ctx = new EntryDataDSContext())
        {
            foreach (var manifest in lst)
            {
                var filename = BaseDataModel.SetFilename(dataFile.DroppedFilePath, manifest.WayBill, "-Manifest.pdf");
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
}