using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class FreightImporter
    {
        static FreightImporter()
        {
        }

        public FreightImporter()
        {
        }

        public void ProcessFreight(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst)
        {
            try
            {
                
                var lst = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>)x))
                    .Select(x => new ShipmentFreight()
                    {

                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        InvoiceNumber = x["InvoiceNumber"].ToString(),
                        Consignee = x["Consignee"].ToString(),
                        Currency = x.ContainsKey("Currency") ?  x["Currency"].ToString().Truncate(3): null,
                        InvoiceDate =(DateTime) x["ETA"],
                        DueDate = x.ContainsKey("DueDate") ? DateTime.Parse(x["DueDate"].ToString()) : DateTime.MinValue,
                        ETA = (DateTime)x["ETA"],
                        BLNumber = x.ContainsKey("BLNumber") ? x["BLNumber"].ToString(): null,
                        InvoiceTotal = Convert.ToDouble(x["InvoiceTotal"].ToString()),
                        ShipmentFreightDetails = ((List<IDictionary<string, object>>)x["ShipmentFreightDetails"])
                            .Select(z => new ShipmentFreightDetails()
                            {
                                Quantity = Convert.ToDouble(z["Quantity"].ToString()),
                                Description = z["Description"].ToString(),
                                WarehouseCode = z["WarehouseCode"]?.ToString(),
                                Rate = Convert.ToDouble(z["Rate"].ToString()),
                                Total = Convert.ToDouble(z["Amount"].ToString()),
                                TrackingState = TrackingState.Added,

                            }).ToList(),

                        EmailId = emailId,
                        // SourceFile = filename,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var manifest in lst)
                    {
                        var filename = BaseDataModel.SetFilename(droppedFilePath, manifest.BLNumber, "-Freight.pdf");
                        manifest.SourceFile = filename;
                        var existingManifest =
                            ctx.ShipmentFreight.FirstOrDefault(
                                x => x.InvoiceNumber == manifest.InvoiceNumber);
                        if (existingManifest != null)
                            ctx.ShipmentFreight.Remove(existingManifest);
                        ctx.ShipmentFreight.Add(manifest);

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