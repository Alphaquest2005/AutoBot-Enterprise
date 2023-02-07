using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using Core.Common.Extensions;
using MoreLinq;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.SavingAllocations
{
    public class SaveAllocationBulk : ISaveAllocationSQLProcessor
    {
        public void SaveAllocations(List<List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>> alloLst)
        {
            var allocations = alloLst.SelectMany(x => x.Select(z => z.Value.dbAllocations.ToList()).ToList()).SelectMany(x => x.ToList());
            SaveAllocations(allocations);

            var entryDataDetails = alloLst.SelectMany(x => x.Select(z => z.Value.EntryDataDetails.ToList()).ToList()).SelectMany(x => x.ToList());
            SaveEntryDataDetails(entryDataDetails);

            var xCudaItems = alloLst.SelectMany(x => x.Select(z => z.Value.XcudaItems.ToList()).ToList()).SelectMany(x => x.ToList());
            SaveXcudaItems(xCudaItems);
        }

        public void SaveAllocations(List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)> alloLst)
        {
            SaveAllocations(alloLst.SelectMany(z => z.Sales.Select(s => s.AsycudaSalesAllocations.ToList())).SelectMany(x => x.ToList()).ToList());
            SaveEntryDataDetails(alloLst.SelectMany(e => e.Sales.ToList()));
            SaveXcudaItems(alloLst.SelectMany(e => e.asycudaItems.ToList()));
        }

       
        public  void SaveXcudaItems(IEnumerable<xcuda_Item> xCudaItems) => new AllocationDSContext().BulkUpdate(xCudaItems);

        public  void SaveEntryDataDetails(IEnumerable<EntryDataDetails> entryDataDetails) => new AllocationDSContext().BulkUpdate(entryDataDetails);

        public  void SaveAllocations(IEnumerable<AsycudaSalesAllocations> allocations) => new AllocationDSContext().BulkUpdate(allocations);


       
    }
}