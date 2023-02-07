using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;


namespace WaterNut.DataSpace
{
    public class GetExistingEx9sMem : IGetExistingEx9Processor
    {
         private static ConcurrentDictionary<(long Id, int InventoryItemId), PreAllocations> _existingEx9s = null;
       
        static readonly object Identity = new object();

        public GetExistingEx9sMem()
        {
            lock (Identity)
            {
                if (_existingEx9s != null) return;
            
                using (var ctx = new AllocationDSContext() { StartTracking = false })
                {
                    ctx.Database.CommandTimeout = 0;
                    var res = ctx.ExistingAllocations.AsNoTracking()
                        .ToList()
                        .Select(x => new PreAllocations()
                        {
                            EntryDataDetailsId = x.EntryDataDetailsId, PItemId = x.pItemId, XItemId = x.xItemId,
                            QtyAllocated = (x.xQuantity <= x.SalesQuantity ? x.xQuantity : x.SalesQuantity) ?? 0,
                            DutyFreePaid = x.DutyFreePaid, InventoryItemId = x.InventoryItemId, Id = x.Id, SalesQuantity = x.SalesQuantity,
                            InvoiceDate = x.Date
                        })
                        .ToList();
                    _existingEx9s = //res;
                        new ConcurrentDictionary<(long Id, int InventoryItemId), PreAllocations>(
                            res.ToDictionary(x => (x.Id, x.InventoryItemId), x => x));
                    //  new ConcurrentBag<PreAllocations>(res);
                }

            }
        }

        public List<PreAllocations> Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            return _existingEx9s
                .Join(itemSetLst.Select(z => z.InventoryItemId).ToList(), x => x.Key.InventoryItemId, i => i, (x, i) => x)
               // .Join(itemSetLst.Select(z => z.InventoryItemId).ToList(), x => x.InventoryItemId, i => i, (x, i) => x)
                .Select(x => x.Value)
                .ToList();


        }
    }
}