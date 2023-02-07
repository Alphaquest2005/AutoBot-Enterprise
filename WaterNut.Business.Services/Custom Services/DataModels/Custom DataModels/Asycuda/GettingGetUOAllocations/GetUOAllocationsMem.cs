using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingGetUOAllocations
{
    public class GetUOAllocationsMem : IGetUOAllocationsProcessor
    {
        static readonly object Identity = new object();
        private static ConcurrentDictionary<int , IGrouping<xcuda_Item, AsycudaSalesAllocations>> _allocations = null;

        public GetUOAllocationsMem()
        {
            lock (Identity)
            {
                if (_allocations != null) return;
                    using (var ctx = new AllocationDSContext { StartTracking = false })
                {
                    var res = ctx.AsycudaSalesAllocations.AsNoTracking()

                        .Include(x => x.EntryDataDetails.EntryData)
                        .Include(x => x.EntryDataDetails.EntryDataDetailsEx)
                        .Include(x => x.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode)
                        .Where(x => x.EntryDataDetails.IsReconciled != true)
                        .Where(x => x != null && x.PreviousDocumentItem != null)
                        .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                        .OrderBy(x => x.AllocationId)
                        .ToList();

                      
                        
                     var dic =   res.GroupBy(x => new xcuda_Item()
                        {
                            Item_Id = x.PreviousItem_Id ?? 0,
                            DFQtyAllocated = x?.PreviousDocumentItem?.DFQtyAllocated??0,
                            DPQtyAllocated = x?.PreviousDocumentItem?.DPQtyAllocated ??0,
                            xcuda_Tarification = x?.PreviousDocumentItem?.xcuda_Tarification
                        })
                        .ToDictionary(x => x.Key.Item_Id, x => x);
                    _allocations = new ConcurrentDictionary<int, IGrouping<xcuda_Item, AsycudaSalesAllocations>>(dic);
                }
            }
        }

        public List<IGrouping<xcuda_Item, AsycudaSalesAllocations>> Execute(List<int> itemList)
        {
            return _allocations
                .Join(itemList, a => a.Key, i => i, (a,i) => a)
                .Select(x => x.Value)
                .ToList();
        }


    }

}