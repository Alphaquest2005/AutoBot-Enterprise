using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingGetUOAllocations
{
    public class GetUOAllocations : IGetUOAllocationsProcessor
    {
        public List<IGrouping<xcuda_Item, AsycudaSalesAllocations>> Execute(List<int> itemList)
        {
            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                return ctx.AsycudaSalesAllocations.AsNoTracking()
                    .Include(x => x.EntryDataDetails)
                    .Include(x => x.EntryDataDetails.EntryDataDetailsEx)
                    .Include(x => x.PreviousDocumentItem)
                    .Where(x => x.EntryDataDetails.IsReconciled != true)
                    //.Where(x => x != null && x.PreviousItem_Id == i.Item_Id)
                    .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                    .OrderBy(x => x.AllocationId)
                    .Join(itemList, x => x.PreviousItem_Id, i => i, (a, i) => (a))
                    .ToList()
                    .GroupBy(x => new xcuda_Item()
                    {
                        Item_Id = x.PreviousItem_Id ?? 0, DFQtyAllocated = x.PreviousDocumentItem.DFQtyAllocated,
                        DPQtyAllocated = x.PreviousDocumentItem.DPQtyAllocated
                    })
                    //.DistinctBy(x => x.AllocationId)
                    .ToList();
            }
        }
    }
}