using System.Data;
using System.IO;
using System.Linq;
using AutoBotUtilities.CSV; // Assuming CSVUtils is here
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, Suppliers are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void UpdateSupplierInfo(FileTypes ft, FileInfo[] fs)
        {
            using (var ctx = new EntryDataDSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;
                foreach (var file in fs)
                {
                    var dt = CSVUtils.CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["SupplierCode"].ToString())) continue;
                        var supplierCode = row["SupplierCode"].ToString();
                        var itm = ctx.Suppliers.First(x => x.SupplierCode == supplierCode && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        itm.SupplierName = row["SupplierName"].ToString();
                        itm.Street = row["SupplierAddress"].ToString();
                        itm.CountryCode = row["CountryCode"].ToString();

                        ctx.SaveChanges();
                    }

                }
            }
        }
    }
}