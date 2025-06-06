using System;
using System.Data;
using System.IO;
using System.Linq;
using AutoBotUtilities.CSV; // Assuming CSVUtils is here
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using InventoryDS.Business.Entities; // Assuming InventoryDSContext, InventoryItems are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void MapUnClassifiedItems(FileTypes ft, FileInfo[] fs)
        {
            Console.WriteLine("Mapping unclassified items");
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var file in fs)
                {
                    var dt = CSVUtils.CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["TariffCode"].ToString())) continue;
                        var itmNumber = row["ItemNumber"].ToString();
                        var itms = ctx.InventoryItems.Where(x => x.ItemNumber == itmNumber && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                        foreach (var itm in itms)
                        {
                            itm.TariffCode = row["TariffCode"].ToString();
                        }

                        ctx.SaveChanges();
                    }

                }
            }
            // Need to ensure EntryDocSetUtils.SetFileTypeDocSetToLatest is accessible
            EntryDocSetUtils.SetFileTypeDocSetToLatest(ft);
        }
    }
}