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
    public class BillOfLadenImporter
    {
        private double CF2M3 = 0.0283168;
        private const double lb2Kg = 0.453592;

        static BillOfLadenImporter()
        {
        }

        public BillOfLadenImporter()
        {
        }

        public void Process(DataFile dataFile)
        {
            try
            {
                
                var lst = dataFile.Data.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).Select(x => ((IDictionary<string, object>)x))
                    .Select(x => new ShipmentBL()
                    {
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        //RegistrationNumber = x["RegistrationNumber"].ToString(),
                        //x.ContainsKey("TotalOtherCost")? Convert.ToDouble(x["TotalOtherCost"].ToString()): (double?) null,
                        Reference = x.ContainsKey("Reference") ? x["Reference"].ToString(): null,
                        Voyage = x.ContainsKey("Voyage") ? x["Voyage"].ToString(): null,
                        //ETD = DateTime.ParseExact(x["ETD"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        //ETA = DateTime.ParseExact(x["ETA"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Vessel = x.ContainsKey("Vessel") ? x["Vessel"].ToString(): null,
                        BLNumber = x["BLNumber"].ToString(),
                        //LineNumber = Convert.ToInt32(x["LineNumber"].ToString()),
                        Container = x.ContainsKey("Container") ? x["Container"].ToString(): null,
                        Seals = x.ContainsKey("Seals") ? x["Seals"].ToString(): null,
                        Type = x.ContainsKey("Type") ? x["Type"].ToString(): null,
                        //CargoReporter = x["CargoReporter"].ToString(),
                        //Exporter = x["Exporter"].ToString(),
                        //Consignee = x["Consignee"].ToString(),
                        //Notify = x["Notify"].ToString(),
                        PackagesNo = x.ContainsKey("PackagesNo") ? Convert.ToInt32(x["PackagesNo"].ToString()) : 0,
                        PackagesType = x.ContainsKey("PackagesType") ? x["PackagesType"].ToString().Truncate(10): "PK",
                        WeightKG = x.ContainsKey("WeightKG") ? Convert.ToDouble(x["WeightKG"].ToString()): x.ContainsKey("WeightLB") ? Convert.ToDouble(x["WeightLB"].ToString())* lb2Kg : 0,
                        VolumeM3 = x.ContainsKey("VolumeM3") ? Convert.ToDouble(x["VolumeM3"].ToString()) : x.ContainsKey("VolumeCF") ? Convert.ToDouble(x["VolumeCF"].ToString())* CF2M3 : 0,

                        WeightLB = x.ContainsKey("WeightLB") ? Convert.ToDouble(x["WeightLB"].ToString()): (double?) null,
                        VolumeCF = x.ContainsKey("VolumeCF") ? Convert.ToDouble(x["VolumeCF"].ToString()): (double?) null,



                        Freight = x.ContainsKey("Freight") ? Convert.ToDouble(x["Freight"].ToString()): (double?) null,
                        FreightCurrency = x.ContainsKey("FreightCurrency") ? x["FreightCurrency"].ToString(): null,
                        //LocationOfGoods = x["LocationOfGoods"].ToString(),
                        //Goods = x["Goods"].ToString(),
                        //Marks = x["Marks"].ToString(),
                        //Containers = Convert.ToInt32(x["Containers"].ToString()),
                        ShipmentBLDetails = ((List<IDictionary<string, object>>)x["ShipmentBLDetails"])
                            .Select(z => new ShipmentBLDetails()
                            {
                                Quantity = z.ContainsKey("Quantity") ? Convert.ToInt32(z["Quantity"].ToString()): 0,
                                Marks = z.ContainsKey("Marks") ? z["Marks"].ToString(): "",
                                PackageType = z.ContainsKey("PackageType") ? z["PackageType"].ToString().Truncate(10): "",
                                Weight = z.ContainsKey("Weight") ? z["Weight"].ToString() : "",
                                Section = z.ContainsKey("Section") ? z["Section"].ToString() : null,
                                Comments = z.ContainsKey("Comments") ? z["Comments"].ToString() : null,
                                TrackingState = TrackingState.Added,
                                                               
                            }).ToList(),

                        EmailId = dataFile.EmailId,
                        // SourceFile = droppedFilePath,
                        FileTypeId = dataFile.FileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

                

                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var bl in lst)
                    {
                        var blDetails = AutoFixShipmentBlDetails(bl);

                        bl.ShipmentBLDetails = blDetails;

                      
                       var detailsQty = bl.ShipmentBLDetails.Sum(x => x.Quantity);


                        var filename = BaseDataModel.SetFilename(dataFile.DroppedFilePath, bl.BLNumber, "-BL.pdf");
                        if (!File.Exists(filename)) File.Copy(dataFile.DroppedFilePath, filename);
                        bl.SourceFile = filename;
                        var existingBl =
                            ctx.ShipmentBL.FirstOrDefault(
                                x => x.BLNumber == bl.BLNumber);
                        if (existingBl != null)
                            ctx.ShipmentBL.Remove(existingBl);
                        ctx.ShipmentBL.Add(bl);

                        ctx.SaveChanges();
                        //if (bl.PackagesNo != detailsQty)
                        //{
                        //    throw new ApplicationException(
                        //        $"BL Details Quantity don't add up to BL Total Packages! - BL{bl.PackagesNo} vs Details{detailsQty}");
                        //}

                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<ShipmentBLDetails> AutoFixShipmentBlDetails(ShipmentBL bl)
        {
            var details = bl.ShipmentBLDetails.Any(x => !string.IsNullOrEmpty(x.Weight))? bl.ShipmentBLDetails.Where(x => !string.IsNullOrEmpty(x.Weight)).ToList() :bl.ShipmentBLDetails.ToList() ;

           
            var secList = details.GroupBy(x => x.Section).ToList();
            var lst = new List<ShipmentBLDetails>();
            foreach (var section in secList)
            {
                foreach (var detail in section)
                {
                    if (lst.Any(x =>
                            x.Quantity == detail.Quantity && x.Marks == detail.Marks &&
                            x.PackageType == detail.PackageType && x.Section != detail.Section)) continue;
                    lst.Add(detail);
                }
            }

            return lst;
            
        }
    }
}