using System.Collections.Generic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingPreAllocations;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.SavingAllocations;

namespace WaterNut.DataSpace
{
    public class ProcessPreAllocations
    {
        static ProcessPreAllocations()
        {
        }

        public ProcessPreAllocations()
        {
        }

        public async Task Execute(List<PreAllocations> preAllocations)
        {
            var allocations = await new  AllocatePreAllocationsTask().Execute(preAllocations).ConfigureAwait(false);
            SaveAllocations(allocations);
        }

        private  void SaveAllocations((List<AsycudaSalesAllocations> allocations, List<EntryDataDetails> entryDataDetails, List<xcuda_Item> pItems) allocations)
        {
            new SaveAllocationSQL().SaveAllocations(allocations.allocations);
            new SaveAllocationSQL().SaveEntryDataDetails(allocations.entryDataDetails);
            new SaveAllocationSQL().SaveXcudaItems(allocations.pItems);
        }
    }
}