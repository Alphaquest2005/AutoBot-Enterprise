using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class SupplierDataProcessor
    {
        public static List<SupplierData> GetSupplierData(DataFile dataFile)
        {
            var itmlst = dataFile.Data
                .GroupBy(x => (SupplierCode:x.SupplierCode ?? x.Vendor, x.SupplierName, x.SupplierAddress, x.CountryCode ))
                .Select(x => new SupplierData(x.Key, x.ToList()))
                .ToList();
            return itmlst;
        }
    }

    public class SupplierImporter
    {

        public async Task ImportSuppliers(DataFile dataFile)
        {
            try
            {
                
                var itmlst = SupplierDataProcessor.GetSupplierData(dataFile);

                await SupplierProcessor.SaveNewSuppliers(itmlst).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}