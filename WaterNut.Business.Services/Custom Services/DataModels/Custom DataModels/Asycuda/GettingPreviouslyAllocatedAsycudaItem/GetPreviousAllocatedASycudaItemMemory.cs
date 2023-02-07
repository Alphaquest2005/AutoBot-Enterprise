using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Asycuda421;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingPreviouslyAllocatedAsycudaItem
{ 
    internal class GetPreviousAllocatedASycudaItemMemory: IGetPreviousAllocatedAsycudaItem
    {
    public List<EntryDataDetails> SalesItems { get; }

    public GetPreviousAllocatedASycudaItemMemory(List<EntryDataDetails> salesItems)
    {
        SalesItems = salesItems;

    }

    //public async Task<int> Execute(List<xcuda_Item> asycudaEntries, EntryDataDetails saleitm, int i)
    //{
    //    var previousI = 0;
    //    var pitmsIds = asycudaEntries.Select(x => x.Item_Id).ToList();
    //    var dfp = saleitm.DutyFreePaid;
    //    var lastAllocation = new AllocationDSContext()
    //        .AsycudaSalesAllocations
    //        .Where(x => x.EntryDataDetails.InventoryItemId ==
    //                    saleitm.InventoryItemId
    //                    && pitmsIds.Any(z => z == x.PreviousItem_Id)
    //                    && (dfp == "Duty Free"
    //                        ? x.PreviousDocumentItem.DFQtyAllocated > 0
    //                        : x.PreviousDocumentItem.DPQtyAllocated > 0))
    //        .OrderByDescending(x => x.AllocationId).FirstOrDefault();

    //    if (lastAllocation == null)
    //    {
    //        //if (asycudaEntries.Sum(x => x.AsycudaSalesAllocations.Count()) != 0)
    //        //await AddExceptionAllocation(saleitm, "Returned More than Sold")
    //        //	.ConfigureAwait(false);
    //        return i;
    //    }
    //    // refreash all items from cache and set currentindex to last previous item
    //    //and continue

    //    var lastIndex = asycudaEntries.FindLastIndex(x =>
    //        x.Item_Id == lastAllocation.PreviousItem_Id);
    //    previousI = lastIndex - 1;
    //    return previousI;
    //}

      public int Execute(List<xcuda_Item> asycudaEntries, EntryDataDetails saleitm, int i)
        {
            var lastAllocation = SalesItems.SelectMany(x => x.AsycudaSalesAllocations.ToList())
                .Where(x => x?.EntryDataDetails?.InventoryItemId == saleitm.InventoryItemId)
                .Where(x => saleitm.DutyFreePaid == "Duty Free"
                    ? x?.PreviousDocumentItem?.DFQtyAllocated > 0
                    : x?.PreviousDocumentItem?.DPQtyAllocated > 0)
                .OrderByDescending(x => x.AllocationId)
                .FirstOrDefault();
            return GetPreviousItemFromAllocationProcessor.GetPreviousItemFromAllocation(asycudaEntries, i, lastAllocation);
        }
    }
}
