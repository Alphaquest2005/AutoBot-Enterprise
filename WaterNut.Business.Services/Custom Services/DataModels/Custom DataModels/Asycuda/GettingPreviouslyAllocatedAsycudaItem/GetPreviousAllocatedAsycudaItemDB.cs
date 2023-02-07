using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingPreviouslyAllocatedAsycudaItem
{
    public class GetPreviousAllocatedAsycudaItemDB : IGetPreviousAllocatedAsycudaItem
    {
        static GetPreviousAllocatedAsycudaItemDB()
        {
        }

        public GetPreviousAllocatedAsycudaItemDB()
        {
        }

        public int Execute(List<xcuda_Item> asycudaEntries, EntryDataDetails saleitm, int i)
        {
            var previousI = 0;
            var pitmsIds = asycudaEntries.Select(x => x.Item_Id).ToList();
            var dfp = saleitm.DutyFreePaid;
            var lastAllocation = new AllocationDSContext()
                .AsycudaSalesAllocations
                .Where(x => x.EntryDataDetails.InventoryItemId ==
                            saleitm.InventoryItemId
                            && pitmsIds.Any(z => z == x.PreviousItem_Id)
                            && (dfp == "Duty Free"
                                ? x.PreviousDocumentItem.DFQtyAllocated > 0
                                : x.PreviousDocumentItem.DPQtyAllocated > 0))
                .OrderByDescending(x => x.AllocationId).FirstOrDefault();

            return GetPreviousItemFromAllocationProcessor.GetPreviousItemFromAllocation(asycudaEntries, i, lastAllocation);
        }
    }
}