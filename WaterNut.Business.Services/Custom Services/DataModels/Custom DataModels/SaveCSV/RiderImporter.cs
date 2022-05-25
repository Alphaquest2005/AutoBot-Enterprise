using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class RiderImporter
    {
        public void ProcessCsvRider(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            string emailId,  string droppedFilePath, List<dynamic> eslst)
        {
            try
            {
                

                var csvRiders = eslst.Select(x => ((IDictionary<string, object>) x))
                    .GroupBy(x => new
                    {
                        ETA = x[nameof(ShipmentRider.ETA)],
                        Date = x["Date"],
                        Code = x[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim()
                    }).ToList();



                var rawRiders = csvRiders
                    .Select(x => new{
                        x.Key.ETA,
                        DocumentDate = x.Key.Date,
                        ShipmentRiderDetails = x.Select(z => new 
                        {
                            Consignee = z[nameof(ShipmentRiderDetails.Consignee)]?.ToString().Trim()?? z[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim(),
                            Code = z[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim(),
                            Shipper = z[nameof(ShipmentRiderDetails.Shipper)]?.ToString().Trim(),
                            TrackingNumber = z[nameof(ShipmentRiderDetails.TrackingNumber)]?.ToString().Trim(),
                            Pieces = int.TryParse(z[nameof(ShipmentRiderDetails.Pieces)]?.ToString().Trim(),out var test)? test : 0,//Convert.ToInt32(z[nameof(ShipmentRiderDetails.Pieces)]?.ToString().Trim())
                            WarehouseCode = z[nameof(ShipmentRiderDetails.WarehouseCode)]?.ToString().Trim(),
                            InvoiceNumber = z[nameof(ShipmentRiderDetails.InvoiceNumber)]?.ToString().Trim().ReplaceRegex(@"[^0-9a-zA-Z\s\-/\,]+",""),
                            InvoiceTotal = z.ContainsKey(nameof(ShipmentRiderDetails.InvoiceTotal)) ? z[nameof(ShipmentRiderDetails.InvoiceTotal)]?.ToString().Trim() : "0",
                            GrossWeightLB = Convert.ToDouble(z["GrossWeightLB"]?.ToString().Trim()),
                            CubicFeet = z.ContainsKey("VolumeCF") ? Convert.ToDouble(z["VolumeCF"]?.ToString().Trim()) : 0.0
                            

                        }).OrderByDescending(z => z.Pieces).ToList()

                    }).ToList();

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var rawRider in rawRiders)//.Where(x => x.ETA != null) // bad data duh make sense
                    {
                        DateTime eta = (DateTime) (rawRider.ETA ?? DateTime.MinValue);
                        var existingRider = ctx.ShipmentRider.Where(x => x.ETA == eta).ToList()
                            .Where(x => new FileInfo(x.SourceFile).Name == new FileInfo(droppedFilePath).Name);
                        if (existingRider.Any()) ctx.ShipmentRider.RemoveRange(existingRider);
                        
                        var invoiceLst = rawRider.ShipmentRiderDetails.Select(x => new
                            {
                                x.WarehouseCode,
                                Totalpkgs = x.Pieces,
                                totalKgs = x.GrossWeightLB * ImportLibary.lb2Kg,
                                totalCF = x.CubicFeet,
                                Number = (x.InvoiceNumber??"").Contains(',') ? x.InvoiceNumber.Split(',').Where(z => !string.IsNullOrEmpty(z)).ToArray() : (x.InvoiceNumber??"").Split('/').Where(z => !string.IsNullOrEmpty(z)).ToArray(),
                                Total =  x.InvoiceTotal.Contains(',') ? x.InvoiceTotal.Split(',').Where(z => !string.IsNullOrEmpty(z)).ToArray() : x.InvoiceTotal.Split('/').Where(z => !string.IsNullOrEmpty(z)).ToArray(),
                                
                            })
                            .ToList();
                        var riderDetails = new List<ShipmentRiderDetails>();
                        foreach (var i in invoiceLst)
                        {
                            var totalst = i.Total.Select(x => Convert.ToDouble(string.IsNullOrEmpty(x)?"1":x.Replace("$","").Trim())).ToList();
                            var totalSum = totalst.Sum();
                            var usedPieces = 0;
                            var usedKgs = 0.0;
                            var usedCF = 0.0;
                            for (int j = 0; j < i.Number.Length; j++)
                            {
                                
                                var rate =  (totalSum == 0 || j >= totalst.Count || totalst[j] == 0) ? (1 / Convert.ToDouble(i.Number.Length) ) : totalst[j] / totalSum;
                                var pkgs = j == i.Number.Length - 1
                                    ? i.Totalpkgs - usedPieces
                                    : Convert.ToInt32(i.Totalpkgs * rate);
                                if (pkgs == 0) pkgs = 1;
                                var kgs = j == i.Number.Length - 1
                                    ? i.totalKgs - usedKgs
                                    : Convert.ToDouble(i.totalKgs * rate);
                                var cf = j == i.Number.Length - 1
                                    ? i.totalCF - usedCF
                                    : Convert.ToDouble(i.totalCF * rate);

                                List<ShipmentRiderDetails> res = new List<ShipmentRiderDetails>();
                                if (usedPieces >= i.Totalpkgs)
                                {
                                    var riderDetail = riderDetails.FirstOrDefault(x => x.Pieces > 1);
                                    if (riderDetail != null)
                                    {
                                        riderDetail.Pieces -= 1;
                                        res.Add(new ShipmentRiderDetails()
                                        {
                                            Consignee = riderDetail.Consignee,
                                            Code = riderDetail.Code,
                                            Shipper = riderDetail.Shipper,
                                            TrackingNumber = riderDetail.TrackingNumber,
                                            Pieces = 1,
                                            WarehouseCode = i.WarehouseCode.Trim(),//riderDetail.WarehouseCode,
                                            InvoiceNumber = i.Number[j].Trim(),
                                            InvoiceTotal = j >= totalst.Count 
                                                ? 0 : totalst[j],
                                            GrossWeightKg = kgs,
                                            CubicFeet = cf,
                                            TrackingState = TrackingState.Added

                                        });
                                    }
                                    else
                                    {
                                        res = rawRider.ShipmentRiderDetails
                                            .Where(x => x.WarehouseCode == i.WarehouseCode &&
                                                        (x.InvoiceNumber ?? "").Contains(i.Number[j])).Select(
                                                z =>
                                                    new ShipmentRiderDetails()
                                                    {
                                                        Consignee = z.Consignee,
                                                        Code = z.Code,
                                                        Shipper = z.Shipper,
                                                        TrackingNumber = z.TrackingNumber,
                                                        Pieces = 0,//pkgs,
                                                        WarehouseCode = z.WarehouseCode.Trim(),
                                                        InvoiceNumber = i.Number[j].Trim(),
                                                        InvoiceTotal = j >= totalst.Count ? 0 : totalst[j],
                                                        GrossWeightKg = kgs,
                                                        CubicFeet = cf,
                                                        TrackingState = TrackingState.Added

                                                    }).ToList();

                                    }

                                }
                                else
                                {
                                   
                                    res = rawRider.ShipmentRiderDetails
                                        .Where(x => x.WarehouseCode == i.WarehouseCode &&
                                                    (x.InvoiceNumber??"").Contains(i.Number[j])).Select(
                                            z =>
                                                new ShipmentRiderDetails()
                                                {
                                                    Consignee = z.Consignee,
                                                    Code = z.Code,
                                                    Shipper = z.Shipper,
                                                    TrackingNumber = z.TrackingNumber,
                                                    Pieces = pkgs,
                                                    WarehouseCode = z.WarehouseCode.Trim(),
                                                    InvoiceNumber = i.Number[j].Trim(),
                                                    InvoiceTotal = j >= totalst.Count ? 0 : totalst[j],
                                                    GrossWeightKg = kgs,
                                                    CubicFeet = cf,
                                                    TrackingState = TrackingState.Added

                                                }).ToList();
                                }


                                usedCF += cf;
                                usedKgs += kgs;
                                usedPieces += pkgs;
                                riderDetails.AddRange(res);
                                if (totalst.Sum() != 0 && i.Number.Length != i.Total.Length) break;
                            }

                        }

                        

                        ctx.ShipmentRider.Add(new ShipmentRider()
                        {
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            ETA = (DateTime)(rawRider.ETA ?? DateTime.MinValue),
                            DocumentDate = (DateTime)(rawRider.DocumentDate ?? DateTime.MinValue),
                            FileTypeId =fileType.Id,
                            EmailId = emailId,
                            SourceFile = droppedFilePath,
                            TrackingState = TrackingState.Added,
                            ShipmentRiderDetails = riderDetails
                        });
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