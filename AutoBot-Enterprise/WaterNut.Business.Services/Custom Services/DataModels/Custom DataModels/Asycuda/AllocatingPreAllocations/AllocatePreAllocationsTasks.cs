using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingPreAllocations
{
    public class AllocatePreAllocationsTask : IAllocatePreAllocationsProcessor
    {
        public async Task<(List<AsycudaSalesAllocations> allocations, List<EntryDataDetails> entryDataDetails, List<xcuda_Item> pItems)> Execute(List<PreAllocations> preAllocations)
        {
            List<AsycudaSalesAllocations> allocations = null;
            var res = new List<PreAllocations>();

            var overExWarehoused = preAllocations
                .Where(x => x.EntryDataDetailsId != 0)
                .GroupBy(x => x.EntryDataDetailsId)
                .Where(x => x.Count() > 1 && x.Sum(z => z.QtyAllocated) > x.First().SalesQuantity);
            var firstofoverExwarehoused = overExWarehoused
                .Select(x => x.First())
                .ToList();
            res.AddRange(firstofoverExwarehoused);
            //var overExWarehousedAllocations = GetAllocationsFromOverExwarehoused(overExWarehoused);

            var splitrows = preAllocations
                .Where(x => x.EntryDataDetailsId != 0)
                .GroupBy(x => x.EntryDataDetailsId)
                .Where(x => x.Count() > 1 && x.Sum(z => z.QtyAllocated) < x.First().SalesQuantity)
                .SelectMany(x => x.ToList())
                .ToList();
            res.AddRange(splitrows);

            var partialRows = preAllocations
                .Where(x => x.EntryDataDetailsId != 0)
                .GroupBy(x => x.EntryDataDetailsId)
                .Where(x => x.Count() == 1 && x.Sum(z => z.QtyAllocated) < x.First().SalesQuantity)
                .SelectMany(x => x.ToList())
                .ToList(); 
            res.AddRange(partialRows);

            var normalrows = preAllocations
                .Where(x => x.EntryDataDetailsId != 0)
                .GroupBy(x => x.EntryDataDetailsId)
                .Where(x => x.Count() == 1 && x.Sum(z => z.QtyAllocated) == x.First().SalesQuantity)
                .SelectMany(x => x.ToList())
                .ToList();
            res.AddRange(normalrows);

            var nonSalesRows = preAllocations
                .Where(x => x.EntryDataDetailsId == 0)
                .Where(x => !res.Any(z => z.XItemId ==  x.XItemId))
                .GroupBy(x => x.PItemId)
                //.Where(x => x.Count() == 1 && x.Sum(z => z.QtyAllocated) < x.First().)
                .SelectMany(x => x.ToList())
                .ToList();
            res.AddRange(nonSalesRows);

            var d = nonSalesRows.Where(x => x.XItemId == 228319).ToList();
            var f = normalrows.Where(x => x.XItemId == 228319).ToList();
            var h = partialRows.Where(x => x.XItemId == 228319).ToList();
            var j = splitrows.Where(x => x.XItemId == 228319).ToList();
            var gj = overExWarehoused.SelectMany(z => z.ToList()).Where(x => x.XItemId == 228319).ToList();

            
            
            
           
            
            

            res = res.OrderBy(x => x.InvoiceDate).ToList();

            var task1 = Task.Run(() =>
            {
                allocations = res
                    .Select(allocation => new AsycudaSalesAllocations()
                    {
                        EntryDataDetailsId = allocation.EntryDataDetailsId,
                        PreviousItem_Id = allocation.PItemId,
                        xEntryItem_Id = allocation.XItemId,
                        QtyAllocated = allocation.QtyAllocated,
                        TrackingState = TrackingState.Added
                    })
                    .ToList();
            });

            List<EntryDataDetails> entryDataDetails = null;
            var task2 = Task.Run(() =>
            {
                entryDataDetails = res
                    .GroupBy(x => x.EntryDataDetailsId)
                    .Select(x => new EntryDataDetails()
                    {
                        EntryDataDetailsId = x.Key,
                        QtyAllocated = x.Sum(z => z.QtyAllocated),
                    })
                    .ToList();
            });

            List<xcuda_Item> pItems = null;
            var task3 = Task.Run(() =>
            {
                pItems = res
                    .GroupBy(x => (x.PItemId))//, x.DutyFreePaid
                    .Select(x => new xcuda_Item()
                    {
                        Item_Id = x.Key,
                        DFQtyAllocated = x.Where(z => z.DutyFreePaid == "Duty Free").Sum(z => z.QtyAllocated),
                        DPQtyAllocated = x.Where(z => z.DutyFreePaid == "Duty Paid").Sum(z => z.QtyAllocated),//x.Key.DutyFreePaid == "Duty Paid" ? x.Sum(z => z.QtyAllocated) : 0,
                    })
                    .ToList();
            });

            await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);

            return (allocations, entryDataDetails, pItems);

        }

        private List<PreAllocations> GetAllocationsFromOverExwarehoused(IEnumerable<IGrouping<int, PreAllocations>> overExWarehoused)
        {
            var res = new List<PreAllocations>();

            foreach (var itm in overExWarehoused)
            {
                throw new NotImplementedException();
            }

            return res;
        }
    }
}