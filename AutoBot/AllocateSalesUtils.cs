using System;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class AllocateSalesUtils
    {
        public static async Task AllocateSales()
        {
            Console.WriteLine("Allocations Started");
           
            if (BaseDataModel.Instance.CurrentApplicationSettings.AssessEX == true && !HasUnallocatedSales()) return;

            //AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

            new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false).Wait();

            await EmailSalesErrorsUtils.EmailSalesErrors().ConfigureAwait(false);
            
        }

        private static bool HasUnallocatedSales()
        {
            return new CoreEntitiesContext().TODO_UnallocatedSales.Any(x =>
                x.ApplicationSettingsId ==
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
        }
    }
}