using System.Collections.Generic;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.
    GettingPreviouslyAllocatedAsycudaItem
{
    public class GetPreviousItemFromAllocationProcessor
    {
        public static int GetPreviousItemFromAllocation(List<xcuda_Item> asycudaEntries, int i, AsycudaSalesAllocations lastAllocation)
        {
            int previousI;
            if (lastAllocation == null)
            {
                //if (asycudaEntries.Sum(x => x.AsycudaSalesAllocations.Count()) != 0)
                //await AddExceptionAllocation(saleitm, "Returned More than Sold")
                //	.ConfigureAwait(false);
                return i;
            }
            // refreash all items from cache and set currentindex to last previous item
            //and continue

            var lastIndex = asycudaEntries.FindLastIndex(x =>
                x.Item_Id == lastAllocation.PreviousItem_Id);
            previousI = lastIndex - 1;
            return previousI;
        }
    }
}