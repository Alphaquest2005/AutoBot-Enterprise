using System.Collections.Generic;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.SavingAllocations
{
    public interface ISaveAllocationSQLProcessor
    {
        void SaveAllocations(List<List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>> alloLst);
        void SaveAllocations(List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)> alloLst);
        void SaveAllocations(IEnumerable<AsycudaSalesAllocations> allocations);
        void SaveXcudaItems(IEnumerable<xcuda_Item> xCudaItems);
        void SaveEntryDataDetails(IEnumerable<EntryDataDetails> entryDataDetails);
    }
}