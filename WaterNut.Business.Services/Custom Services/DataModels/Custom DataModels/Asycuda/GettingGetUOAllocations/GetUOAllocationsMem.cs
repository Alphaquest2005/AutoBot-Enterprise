using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using MoreLinq;


namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingGetUOAllocations
{
    public class GetUOAllocationsMem : IGetUOAllocationsProcessor
    {
        static readonly object Identity = new object();
        private static ConcurrentDictionary<int , IGrouping<xcuda_Item, AsycudaSalesAllocations>> _allocations = null;
        private static bool _isInitialized = false;
        private static readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);

        // Private constructor
        private GetUOAllocationsMem() {}

        // Async factory method
        public static async Task<GetUOAllocationsMem> CreateAsync()
        {
            await _initSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_isInitialized)
                {
                    var res = await GetAsycudaSalesAllocations().ConfigureAwait(false);
                      
                        
                     var dic =   res.GroupBy(x => new xcuda_Item()
                        {
                            Item_Id = x.PreviousItem_Id ?? 0,
                            DFQtyAllocated = x?.PreviousDocumentItem?.DFQtyAllocated??0,
                            DPQtyAllocated = x?.PreviousDocumentItem?.DPQtyAllocated ??0,
                            xcuda_Tarification = x?.PreviousDocumentItem?.xcuda_Tarification
                        })
                        .ToDictionary(x => x.Key.Item_Id, x => x);
                    _allocations = new ConcurrentDictionary<int, IGrouping<xcuda_Item, AsycudaSalesAllocations>>(dic);
                    _isInitialized = true;
                }
            }
            finally
            {
                _initSemaphore.Release();
            }
            return new GetUOAllocationsMem(); // Return instance after initialization
        }


        private static async Task<List<AsycudaSalesAllocations>> GetAsycudaSalesAllocations()
        {
            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

                var res = ctx.AsycudaSalesAllocations.AsNoTracking()
                    .Where(x => x.EntryDataDetails.IsReconciled != true)
                    .Where(x => x != null && x.PreviousDocumentItem != null)
                    .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                    .OrderByDescending(x => x.AllocationId)
                    .ToList();

               

                var enttask = Task.Run(() => AddEntryDataDetails(res)); // Keep sync method in Task.Run
                var prevtask = AddPreviousDocumentItem(res); // Call async method directly
                await Task.WhenAll(enttask, prevtask).ConfigureAwait(false); // Await both

                return res;
            }
        }

        private static async Task AddPreviousDocumentItem(List<AsycudaSalesAllocations> res)
        {
            //.Include(x => x.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode)
            //.Include(x => x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit)

            var prevItemlst = res.Select(x => x.PreviousItem_Id).Distinct().ToList();
            var prevItemTask = Task.Run(() => new AllocationDSContext().xcuda_Item
                           .Include(x => x.xcuda_Tarification.xcuda_HScode)
                           .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                           .WhereBulkContains( prevItemlst, x => x.Item_Id)
                           .ToList());
            await prevItemTask.ConfigureAwait(false);
            var prevItem = prevItemTask.Result;
            res.Join(prevItem, a => a.PreviousItem_Id, p => p.Item_Id, (a, p) => (a, p))
                .ForEach(x => x.a.PreviousDocumentItem = x.p);
        }

        private static void AddEntryDataDetails(List<AsycudaSalesAllocations> lst)
        {
            var entrydatadetailIdlst = lst.Select(x => x.EntryDataDetailsId).Distinct().ToList();
            //.Include(x => x.EntryDataDetails.EntryData)
            //.Include(x => x.EntryDataDetails.EntryDataDetailsEx)

            var entryDataDetails = new AllocationDSContext().EntryDataDetails
                .Include(x => x.EntryData)
                .Include(x => x.EntryDataDetailsEx)
                .WhereBulkContains(entrydatadetailIdlst, x => x.EntryDataDetailsId).ToList();

               
                lst.Join(entryDataDetails, a => a.EntryDataDetailsId, p => p.EntryDataDetailsId, (a, p) => (a, p))
                    .ForEach(x => x.a.EntryDataDetails = x.p);

            
        }

        public List<IGrouping<xcuda_Item, AsycudaSalesAllocations>> Execute(List<int> itemList)
        {
            return _allocations
                .Join(itemList, a => a.Key, i => i, (a,i) => a)
                .Select(x => x.Value)
                .DistinctBy(x => x.Key.Item_Id)
                .ToList();
        }


    }

}