using System;
using System.Linq;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class AllocateSalesUtils
    {
        public static void AllocateSales()
        {
            Console.WriteLine("Allocations Started");
           
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true && !HasUnallocatedSales()) return;

            AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

            AllocationsBaseModel.Instance.AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false).Wait();

            EmailSalesErrorsUtils.EmailSalesErrors();
            
        }

        private static bool HasUnallocatedSales()
        {
            return new CoreEntitiesContext().TODO_UnallocatedSales.Any(x =>
                x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
        }
    }
}