using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class RiderImporter
    {
        public async Task<bool> Process(DataFile dataFile)
        {
            try
            {
                

                var csvRiders = dataFile.Data.Select(x => ((IDictionary<string, dynamic>) x))
                    .GroupBy(x => 
                    (
                        ETA : x[nameof(ShipmentRider.ETA)],
                        Date : x["Date"],
                        Code : x[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim()
                    )
                    ).ToList();



                var rawRiders = csvRiders
                    .Select(x => new ShipmentRider() 
                    {
                        ETA = x.Key.ETA ?? DateTime.MinValue,
                        DocumentDate = x.Key.Date ?? DateTime.MinValue,
                        ShipmentRiderDetails = x.Select(z => new ShipmentRiderDetails()
                        {
                            Consignee = z[nameof(ShipmentRiderDetails.Consignee)]?.ToString().Trim()?? z[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim(),
                            Code = z[nameof(ShipmentRiderDetails.Code)]?.ToString().Trim(),
                            Shipper = z[nameof(ShipmentRiderDetails.Shipper)]?.ToString().Trim(),
                            TrackingNumber = z[nameof(ShipmentRiderDetails.TrackingNumber)]?.ToString().Trim(),
                            Pieces = int.TryParse(z[nameof(ShipmentRiderDetails.Pieces)]?.ToString().Trim(),out int test)? test : 0,//Convert.ToInt32(z[nameof(ShipmentRiderDetails.Pieces)]?.ToString().Trim())
                            WarehouseCode = z[nameof(ShipmentRiderDetails.WarehouseCode)]?.ToString().Trim(),
                            InvoiceNumber = ((string)z[nameof(ShipmentRiderDetails.InvoiceNumber)]?.ToString().Trim())?.ReplaceRegex(@"[^0-9a-zA-Z\s\-/\,]+",""),

                            //////////// --------------- give up the invoice total concept as its not used and complicates details
                            //InvoiceTotal = z.ContainsKey(nameof(ShipmentRiderDetails.InvoiceTotal)) ? z[nameof(ShipmentRiderDetails.InvoiceTotal)]?.ToString().Trim() : "0",
                            //x.InvoiceTotal.Contains(',')
                            //                    ? x.InvoiceTotal.Split(',').Where(z => !string.IsNullOrEmpty(z)).ToArray()
                            //                    : x.InvoiceTotal.Split('/').Where(z => !string.IsNullOrEmpty(z)).ToArray()


                            GrossWeightKg = Convert.ToDouble(z["GrossWeightLB"]?.ToString().Trim()) * ImportLibary.lb2Kg,
                            CubicFeet = z.ContainsKey("VolumeCF") ? Convert.ToDouble(z["VolumeCF"]?.ToString().Trim()) : 0.0
                            

                        }).OrderByDescending(z => z.Pieces).ToList()

                    }).ToList();
                


                    foreach (var rawRider in rawRiders.Where(x => x.ShipmentRiderDetails.All(z => !string.IsNullOrEmpty(z.WarehouseCode))))//.Where(x => x.ETA != null) // bad data duh make sense
                    {
                        using (var ctx = new EntryDataDSContext())
                        {
                            DateTime eta = (DateTime)(rawRider.ETA);
                            var existingRider = ctx.ShipmentRider.Where(x => x.ETA == eta).ToList()
                                .Where(x => new FileInfo(x.SourceFile).Name ==
                                            new FileInfo(dataFile.DroppedFilePath).Name);
                            if (existingRider.Any()) ctx.ShipmentRider.RemoveRange(existingRider);

                            var riderDetails = Utils.CreatePackingList(rawRider);


                            ctx.ShipmentRider.Add(new ShipmentRider()
                            {
                                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId,
                                ETA = (DateTime)(rawRider.ETA ),
                                DocumentDate = (DateTime)(rawRider.DocumentDate),
                                FileTypeId = dataFile.FileType.Id,
                                EmailId = dataFile.EmailId,
                                SourceFile = dataFile.DroppedFilePath,
                                TrackingState = TrackingState.Added,
                                ShipmentRiderDetails = riderDetails//rawRider.ShipmentRiderDetails --- put because all matching logic depends on this
                            });
                            await ctx.SaveChangesAsync().ConfigureAwait(false);

                        }


                    }

                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
                //throw;
            }
        }
    }
}