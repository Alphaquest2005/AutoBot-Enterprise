using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            .Select(x =>
            {
                try
                {
                    if (!x.ContainsKey("Packages")) return null;
                    return new ShipmentManifest()
                    {
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        RegistrationNumber = $"{x["ManifestYear"]} {x["ManifestNumber"]}",
                        CustomsOffice = x["CustomsOffice"].ToString(),
                        Consignee = x["Consignee"].ToString(),
                        WayBill = x["BLNumber"].ToString(),
                        Packages = Convert.ToInt32(x["Packages"].ToString()),
                        PackageType = x["PackageType"].ToString(),
                        GrossWeightKG = x.ContainsKey("GrossWeightKG") ? Convert.ToDouble(x["GrossWeightKG"].ToString()) : 0,
                        Freight = x.ContainsKey("Freight") ? Convert.ToDouble(x["Freight"].ToString()) : 0,
                        FreightCurrency = x.ContainsKey("FreightCurrency") ? x["FreightCurrency"].ToString() : "",
                        LocationOfGoods = x.ContainsKey("LocationOfGoods") ? x["LocationOfGoods"].ToString() : "",
                        EmailId = dataFile.EmailId,
                        SourceFile = dataFile.DroppedFilePath,
                        FileTypeId = dataFile.FileType.Id,
                        TrackingState = TrackingState.Added,
                    };
                }
                catch (Exception e)
                {
                    return null;
                }

            })
            .Where(x => x != null)
            .ToList();
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