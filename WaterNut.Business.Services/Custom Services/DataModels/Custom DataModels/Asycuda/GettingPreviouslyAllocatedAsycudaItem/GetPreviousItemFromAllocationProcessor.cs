﻿using System.Collections.Generic;
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
                // because its single now add 2 to get over the count forward
                return i;
            }
            // refreash all items from cache and set currentindex to last previous item
            //and continue

            var lastIndex = asycudaEntries.FindLastIndex(x =>
                x.Item_Id == lastAllocation.PreviousItem_Id && lastAllocation.DutyFreePaid == "Duty Free" ? x.DFQtyAllocated > 0 : x.DPQtyAllocated > 0);
            previousI = lastIndex == 0 ? lastIndex :lastIndex - 1;
            return previousI;
        }
    }
}