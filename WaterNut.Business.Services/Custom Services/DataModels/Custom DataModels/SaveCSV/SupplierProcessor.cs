using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using Core.Common.Utils;

namespace WaterNut.DataSpace
{
    public class SupplierProcessor
    {
        public static async Task SaveNewSuppliers(List<SupplierData> itmlst)
        {
            int applicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;
            using (var ctx = new SuppliersService() { StartTracking = true })
            {
                
                foreach (var item in itmlst.Where(x =>
                             !string.IsNullOrEmpty(x.Key.SupplierCode) || !string.IsNullOrEmpty(x.Key.SupplierAddress)))
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
                        Street = ((string)item.Key.SupplierAddress)?.Truncate(100),
                        CountryCode = item.Key.CountryCode,

                        TrackingState = TrackingState.Added
                    };

                    await ctx.CreateSuppliers(i).ConfigureAwait(false);
                }
            }
        }
    }
}