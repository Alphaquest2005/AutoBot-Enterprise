using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;


namespace WaterNut.DataSpace
{
    public class RiderPdfImporter
    {
        public Task<bool> Process(DataFile dataFile)
        {
            try
            {


                var rawRiders = CreateRawRider(dataFile.Data, dataFile.DocSet, dataFile.EmailId, dataFile.FileType,
                    dataFile.DroppedFilePath);

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var itm in rawRiders)
                    {
                        var rider = ctx.ShipmentRider.Include(x => x.ShipmentRiderDetails)
                            .FirstOrDefault(x => x.ETA == itm.ETA);
                        if (rider != null)
                        {
                            //////// check if this updating
                            rider.DocumentDate = itm.DocumentDate;
                            ctx.ShipmentRiderDetails.RemoveRange(rider.ShipmentRiderDetails);
                            rider.ShipmentRiderDetails = itm.ShipmentRiderDetails;
                        }
                        else
                        {
                            rider = itm;
                            ctx.ShipmentRider.Add(rider);
                        }


                    }

                    ctx.SaveChanges();
                }

                return Task.FromResult(true);
                /// pass to rider importer            
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromResult(false);
            }
        }

        public static List<ShipmentRider> CreateRawRider(List<dynamic> data, List<AsycudaDocumentSet> docSet, string emailId, FileTypes fileType, string droppedFilePath)
        {
           
            var itms = data.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).ToList();
            var lstdata = itms.Select(x => (IDictionary<string, object>)x)
                .Select(x =>
                {
                    var invoice = new ShipmentRider();
                    invoice.ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;
                    invoice.ETA = Convert.ToDateTime(x.ContainsKey("ETA") && x["ETA"] != null ? x["ETA"].ToString() : "1/1/0001");
                    invoice.DocumentDate = Convert.ToDateTime(x.ContainsKey("DocumentDate") && x["DocumentDate"] != null ? x["DocumentDate"].ToString() : "1/1/0001");
                    
                    invoice.ShipmentRiderDetails = !x.ContainsKey("ShipmentRiderDetails") ? new List<ShipmentRiderDetails>() : ((List<IDictionary<string, object>>)x["ShipmentRiderDetails"])
                        .Where(z => z != null)
                        .Where(z => z.ContainsKey("InvoiceNumber"))

                        .Select(z =>
                        {
                            var details = new ShipmentRiderDetails();
                            details.Pieces = string.IsNullOrEmpty(z["Pieces"].ToString()) ? 0 : Convert.ToInt32(z["Pieces"].ToString());
                            details.InvoiceNumber = z.ContainsKey("InvoiceNumber") ? z["InvoiceNumber"].ToString().ToUpper().PadLeft(6,'0').Truncate(20) : null;
                            details.TrackingNumber = z.ContainsKey("PONumber") ? z["PONumber"].ToString().PadLeft(5, '0').Truncate(255): null;//TODO: move this formating out
                            details.Consignee = z.ContainsKey("Consignee") ? z["Consignee"].ToString().ToUpper().Truncate(20) : "Budget Marine";
                            details.Code = z.ContainsKey("Code") ? z["Code"].ToString().ToUpper().Truncate(20) : details.Consignee;
                            details.InvoiceTotal =  Convert.ToDouble(z["InvoiceTotal"].ToString()) ;
                            details.WarehouseCode = "Marks";
                            details.GrossWeightKg = 0;
                            details.CubicFeet = 0;
                           
                            details.TrackingState = TrackingState.Added;
                            return details;
                        }).ToList();
                    
                    invoice.EmailId = emailId;
                    invoice.SourceFile = droppedFilePath;
                    invoice.FileTypeId = fileType.Id;
                    invoice.TrackingState = TrackingState.Added;
                    return invoice;
                }).ToList();

            return lstdata;
        }
    }
}