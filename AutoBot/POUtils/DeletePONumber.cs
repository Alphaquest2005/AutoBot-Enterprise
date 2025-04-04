using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, EntryData are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void DeletePONumber(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                Console.WriteLine("Delete PO Numbers");
                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var cnumberList = ft.Data.Where(z => z.Key == "PONumber").Select(x => x.Value).ToList();

                    foreach (var itm in cnumberList)
                    {
                        var res = ctx.EntryData.FirstOrDefault(x => x.EntryDataId == itm &&
                                                                    x.ApplicationSettingsId == BaseDataModel.Instance
                                                                        .CurrentApplicationSettings
                                                                        .ApplicationSettingsId);
                        if (res != null) ctx.EntryData.Remove(res);
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