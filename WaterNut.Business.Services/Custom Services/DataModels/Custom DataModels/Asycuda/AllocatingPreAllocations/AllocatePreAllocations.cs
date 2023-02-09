using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using TrackableEntities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingPreAllocations
{
    public class AllocatePreAllocations : IAllocatePreAllocationsProcessor
    {
        public (List<AsycudaSalesAllocations> allocations, List<EntryDataDetails> entryDataDetails, List<xcuda_Item> pItems) Execute(List<PreAllocations> unAllocatedxSales)
        {
            var allocations = unAllocatedxSales
                .Select(allocation => new AsycudaSalesAllocations()
                {
                    EntryDataDetailsId = allocation.EntryDataDetailsId,
                    PreviousItem_Id = allocation.PItemId,
                    xEntryItem_Id = allocation.XItemId,
                    QtyAllocated = allocation.QtyAllocated,
                    TrackingState = TrackingState.Added
                })
                .ToList();

            var entryDataDetails = unAllocatedxSales
                .GroupBy(x => x.EntryDataDetailsId)
                .Select(x => new EntryDataDetails()
                {
                    EntryDataDetailsId = x.Key,
                    QtyAllocated = x.Sum(z => z.QtyAllocated),
                })
                .ToList();

            var pItems = unAllocatedxSales
                .GroupBy(x => (x.PItemId, x.DutyFreePaid))
                .Select(x => new xcuda_Item()
                {
                    Item_Id = x.Key.PItemId,
                    DFQtyAllocated = x.Key.DutyFreePaid == "Duty Free" ? x.Sum(z => z.QtyAllocated) : 0,
                    DPQtyAllocated = x.Key.DutyFreePaid == "Duty Paid" ? x.Sum(z => z.QtyAllocated) : 0,
                })
                .ToList();

            return (allocations, entryDataDetails, pItems);

        }
    }
}