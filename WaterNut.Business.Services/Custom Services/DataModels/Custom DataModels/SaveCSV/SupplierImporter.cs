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
    public class SupplierImporter
    {
        static SupplierImporter()
        {
        }

        public SupplierImporter()
        {
        }

        public async Task ImportSuppliers(DataFile dataFile)
        {
            try
            {
                var applicationSettingsId = dataFile.DocSet.First().ApplicationSettingsId;
                var itmlst = dataFile.Data
                    .GroupBy(x => new {x.SupplierCode, x.SupplierName, x.SupplierAddress, x.CountryCode})
                    .ToList();

                if (BaseDataModel.Instance.CurrentApplicationSettings.AssessIM7 == true && dataFile.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Po)
                {
                    if (itmlst.All(x => string.IsNullOrEmpty(x.Key?.SupplierCode ?? x.Key?.SupplierName)))
                    {
                        throw new ApplicationException(
                            $"Supplier Code/Name Missing for :{itmlst.Where(x => string.IsNullOrEmpty(x.Key?.SupplierCode ?? x.Key?.SupplierName)).Select(x => x.FirstOrDefault()?.EntryDataId).Aggregate((current, next) => current + ", " + next)}");
                    }
                }


                using (var ctx = new SuppliersService() {StartTracking = true})
                {
                    foreach (var item in itmlst.Where(x =>
                                 !string.IsNullOrEmpty(x.Key?.SupplierCode) || !string.IsNullOrEmpty(x.Key?.SupplierAddress)))
                    {
                        var supplierLst = await ctx.GetSuppliersByExpression(
                            $"{(item.Key.SupplierCode == null ? "SupplierName == \"" + item.Key.SupplierName.ToUpper() + "\"" : "SupplierCode == \"" + item.Key.SupplierCode.ToUpper() + "\"")} && ApplicationSettingsId == \"{applicationSettingsId}\"",
                            null, true).ConfigureAwait(false);
                        var i = supplierLst.FirstOrDefault();
                        if (i != null) continue;
                        i = new Suppliers(true)
                        {
                            ApplicationSettingsId = applicationSettingsId,
                            SupplierCode = item.Key.SupplierCode?.ToUpper() ?? item.Key.SupplierName.ToUpper(),
                            SupplierName = item.Key.SupplierName,
                            Street = item.Key.SupplierAddress,
                            CountryCode = item.Key.CountryCode,

                            TrackingState = TrackingState.Added
                        };

                        await ctx.CreateSuppliers(i).ConfigureAwait(false);

                    }
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