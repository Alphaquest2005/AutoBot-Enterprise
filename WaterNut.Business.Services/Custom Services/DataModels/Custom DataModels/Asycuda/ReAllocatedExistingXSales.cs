using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingUnallocatedXSales;

namespace WaterNut.DataSpace
{
    public class ReAllocatedExistingXSales
    {
        static ReAllocatedExistingXSales()
        {
        }

        public ReAllocatedExistingXSales()
        {
        }

        public async Task Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.PreAllocateEx9s != true) return;

            var preAllocations = GetPreAllocations(itemSetLst);
            await new ProcessPreAllocations().Execute(preAllocations).ConfigureAwait(false);
        }

        private  List<PreAllocations> GetPreAllocations(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            return AllocationsBaseModel.isDBMem == true
                ? new GetUnAllocatedxSales().Execute(itemSetLst)
                : new GetUnAllocatedxSalesMem().Execute(itemSetLst);
        }
    }
}