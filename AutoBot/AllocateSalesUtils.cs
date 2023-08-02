using System;
using System.Linq;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class AllocateSalesUtils
    {
        public static void AllocateSales()
        {
            Console.WriteLine("Allocations Started");
           
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true && !HasUnallocatedSales()) return;

            //AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

            new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false).Wait();

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