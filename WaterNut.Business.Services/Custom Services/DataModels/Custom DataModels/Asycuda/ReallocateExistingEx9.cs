using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public class ReallocateExistingEx9
    {
        static ReallocateExistingEx9()
        {
        }

        public ReallocateExistingEx9()
        {
        }

        public async Task Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.PreAllocateEx9s == true)
                await new ProcessPreAllocations().Execute(GetExistingEx9s(itemSetLst)).ConfigureAwait(false);
        }

        private static List<PreAllocations> GetExistingEx9s(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            return AllocationsBaseModel.isDBMem == true 
                ? new GetExistingEx9s().Execute(itemSetLst)
                : new GetExistingEx9sMem().Execute(itemSetLst);
        }
    }
}