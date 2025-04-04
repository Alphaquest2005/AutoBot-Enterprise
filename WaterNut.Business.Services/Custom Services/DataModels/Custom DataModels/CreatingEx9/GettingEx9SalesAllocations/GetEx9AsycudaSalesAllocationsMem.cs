using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using MoreLinq;


namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations
{
    public class GetEx9AsycudaSalesAllocationsMem : IGetEx9AsycudaSalesAllocations
    {
        private static ConcurrentDictionary<int, EX9AsycudaSalesAllocations> _eX9AsycudaSalesAllocations = null;
        private static bool _isInitialized = false;
        private static readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);

        // Private constructor
        private GetEx9AsycudaSalesAllocationsMem() { }

        // Async factory method
        public static async Task<GetEx9AsycudaSalesAllocationsMem> CreateAsync(string filterExp, string rdateFilter)
        {
            await _initSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_isInitialized)
                {
                    var lst = GetEx9AsycudaSalesAllocationsList(rdateFilter).ToList();

                    // Run initialization tasks concurrently
                    var piDataTask = AddPiDataAsync(lst); // Made async
                    var entryDataTask = Task.Run(() => AddEntryDataDetails(lst)); // Keep sync in Task.Run
                    var pItemTask = AddPreviousItemAsync(lst); // Made async

                    await Task.WhenAll(piDataTask, entryDataTask, pItemTask).ConfigureAwait(false);

                    _eX9AsycudaSalesAllocations = new ConcurrentDictionary<int, EX9AsycudaSalesAllocations>(lst.ToDictionary(x => x.AllocationId, x => x));
                    _isInitialized = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Optionally re-throw or handle initialization failure
                throw;
            }
            finally
            {
                _initSemaphore.Release();
            }
            return new GetEx9AsycudaSalesAllocationsMem(); // Return instance after initialization
        }


        private static async Task AddPreviousItemAsync(List<EX9AsycudaSalesAllocations> lst) // Renamed to Async
        {
            try
            {
                var plst = lst.Select(x => x.PreviousItem_Id).Distinct().ToList();
                if (!plst.Any()) return;

                var pItemLst = await new AllocationDSContext().xcuda_Item.WhereBulkContains(plst, x => x.Item_Id).ToListAsync().ConfigureAwait(false);

                // Task to get EntryPreviousItems and link them
                var entryPreviousItemsTask = Task.Run(async () => {
                    var epi = await new AllocationDSContext().EntryPreviousItems.WhereBulkContains(plst, p => p.Item_Id).ToListAsync().ConfigureAwait(false);
                    var xpi = await AddXcudaPreviousItemsAsync(epi).ConfigureAwait(false); // Made async
                    pItemLst.GroupJoin(xpi, i => i.Item_Id, a => a.Item_Id, (i, a) => (i, a))
                           .ForEach(z => z.i.EntryPreviousItems = z.a.ToList());
                });

                // Task to get Tarifications and link them
                var tarificationTask = Task.Run(async () => {
                     var tarifications = await GetXcudaTarificationsAsync(plst).ConfigureAwait(false); // Made async
                     pItemLst.Join(tarifications, i => i.Item_Id, a => a.Item_Id, (i, a) => (i,a))
                             .ForEach(x => x.i.xcuda_Tarification = x.a);
                });


                var documentsTask = AddAsycudaDocumentsAsync(pItemLst); // Made async

                await Task.WhenAll(entryPreviousItemsTask, tarificationTask, documentsTask).ConfigureAwait(false); // Await all

                // This join can happen after awaiting
                lst.Join(pItemLst, a => a.PreviousItem_Id, p => p.Item_Id, (a, p) => (a, p))
                    .ForEach(x => x.a.PreviousDocumentItem = x.p);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        // Made async
        private static async Task<List<xcuda_Tarification>> GetXcudaTarificationsAsync(List<int> plst)
        {
             if (!plst.Any()) return new List<xcuda_Tarification>();
            var xcudaTarificationTask = new AllocationDSContext().xcuda_Tarification.WhereBulkContains(plst, x => x.Item_Id).ToListAsync();

            var hsCodesTask = GetHsCodesAsync(plst); // Made async

            await Task.WhenAll(xcudaTarificationTask, hsCodesTask).ConfigureAwait(false);
            var xcudaTarifications = await xcudaTarificationTask.ConfigureAwait(false);
            var hsCodes = await hsCodesTask.ConfigureAwait(false);

            xcudaTarifications.Join(hsCodes, t => t.Item_Id, h => h.Item_Id, (t,h) => (t,h))
                .ForEach(x => x.t.xcuda_HScode = x.h);

            return xcudaTarifications;
        }

        // Made async
        private static async Task<List<xcuda_HScode>> GetHsCodesAsync(List<int> plst)
        {
             if (!plst.Any()) return new List<xcuda_HScode>();
            var xcudaHScodes = await new AllocationDSContext().xcuda_HScode.WhereBulkContains(plst, x => x.Item_Id).ToListAsync().ConfigureAwait(false);
            return await AddTariffCodesAsync(xcudaHScodes).ConfigureAwait(false); // Made async

        }

        // Made async
        private static async Task<List<xcuda_HScode>> AddTariffCodesAsync(List<xcuda_HScode> xcudaHScodes)
        {
            var tariffCodeLst = xcudaHScodes.Select(x => x.Commodity_code).Distinct().ToList();
             if (!tariffCodeLst.Any()) return xcudaHScodes;
            var tariffCodes = await new AllocationDSContext().TariffCodes.WhereBulkContains(tariffCodeLst, t => t.TariffCode).ToListAsync().ConfigureAwait(false);
            await AddTariffCategorysAsync(tariffCodes).ConfigureAwait(false); // Made async


           xcudaHScodes.Join(tariffCodes, h => h.Commodity_code, t => t.TariffCode, (h, t) => (h, t))
               .ForEach(x => x.h.TariffCodes = x.t);

           return xcudaHScodes;
        }

        // Made async
        private static async Task AddTariffCategorysAsync(List<TariffCodes> TariffCodes)
        {
            var tarifCatLst = TariffCodes.Select(x => x.TariffCategoryCode).Distinct().ToList();
             if (!tarifCatLst.Any()) return;
            var tariffCategorys = await new AllocationDSContext().TariffCategory.WhereBulkContains(tarifCatLst, x => x.TariffCategoryCode)
                .Distinct().ToListAsync().ConfigureAwait(false);
            await AddTarifCategoryCodeSuppUnitsAsync(tariffCategorys).ConfigureAwait(false); // Made async

            TariffCodes.Join(tariffCategorys, t => t.TariffCategoryCode, c => c.TariffCategoryCode, (t, c) => (t,c))
                .ForEach(x => x.t.TariffCategory = x.c);
        }

        // Made async
        private static async Task AddTarifCategoryCodeSuppUnitsAsync(List<TariffCategory> tariffCategorys)
        {
            var tcLst  = tariffCategorys.Select(x => x.TariffCategoryCode).ToList();
             if (!tcLst.Any()) return;
            var tarifSuplst = await new AllocationDSContext().TariffCategoryCodeSuppUnit.Include(z => z.TariffSupUnitLkps)
                .WhereBulkContains(tcLst, x => x.TariffCategoryCode).ToListAsync().ConfigureAwait(false);

            tariffCategorys.GroupJoin(tarifSuplst, c => c.TariffCategoryCode, s => s.TariffCategoryCode, (c, s) => (c,s))
                .ForEach(x => x.c.TariffCategoryCodeSuppUnit = x.s.ToList());
        }

        // Made async
        private static async Task<List<EntryPreviousItems>> AddXcudaPreviousItemsAsync(List<EntryPreviousItems> entryPreviousItems)
        {
            var prevIds = entryPreviousItems.Select((q => q.PreviousItem_Id)).Distinct()
                .ToList();
             if (!prevIds.Any()) return entryPreviousItems;
            var pitms = await GetXcudaPreviousItemsAsync(prevIds).ConfigureAwait(false); // Made async
            entryPreviousItems.Join(pitms, i => i.PreviousItem_Id, a => a.PreviousItem_Id, (i, a) =>(i,a))
                .ForEach(x => x.i.xcuda_PreviousItem = x.a);
            return entryPreviousItems.Where(x => x.xcuda_PreviousItem != null).ToList();
        }

        // Made async
        private static async Task<List<xcuda_PreviousItem>> GetXcudaPreviousItemsAsync(List<int> prevIds)
        {
             if (!prevIds.Any()) return new List<xcuda_PreviousItem>();
            var previousItemsTask = new AllocationDSContext().xcuda_PreviousItem
                .WhereBulkContains(prevIds, z => z.PreviousItem_Id).ToListAsync();


            var xcudaItemsTask = Task.Run(async () => await AddAsycudaDocumentsAsync( // Made async
                                            await new AllocationDSContext().xcuda_Item
                                                .WhereBulkContains(prevIds, z => z.Item_Id).ToListAsync().ConfigureAwait(false))
                                            .ConfigureAwait(false));

            await Task.WhenAll(previousItemsTask, xcudaItemsTask).ConfigureAwait(false);

            var xitms = await xcudaItemsTask.ConfigureAwait(false);
            var pitms = await previousItemsTask.ConfigureAwait(false);

             pitms.Join(xitms, p => p.PreviousItem_Id, x => x.Item_Id, (p, x) => (p,x))
                .ForEach(z => z.p.xcuda_Item = z.x);

            return pitms;
        }

        // Made async
        private static async Task<List<xcuda_Item>> AddAsycudaDocumentsAsync(List<xcuda_Item> xcudaItems)
        {
            var docs = await GetAsycudaDocumentsAsync(xcudaItems).ConfigureAwait(false); // Made async

            xcudaItems.Join(docs, i => i.ASYCUDA_Id, d => d.ASYCUDA_Id, (i, d) =>(i,d))
                .ForEach(z => z.i.AsycudaDocument = z.d);
            return xcudaItems;
        }

        // Made async
        private static async Task<List<AsycudaDocument>> GetAsycudaDocumentsAsync(List<xcuda_Item> x)
        {
            var docLst = x.Select(d => d.ASYCUDA_Id).Distinct().ToList();
             if (!docLst.Any()) return new List<AsycudaDocument>();

            var docs = await new AllocationDSContext().AsycudaDocument.WhereBulkContains(docLst, a => a.ASYCUDA_Id).ToListAsync().ConfigureAwait(false);

            return await AddCustomsProceduresAsync(docs).ConfigureAwait(false); // Made async
        }

        // Made async
        private static async Task<List<AsycudaDocument>> AddCustomsProceduresAsync(List<AsycudaDocument> docs)
        {
            var cps = await GetCustomsProceduresAsync(docs).ConfigureAwait(false); // Made async

           docs.Join(cps, d => d.CustomsProcedure, c => c.CustomsProcedure, (a, c) => (a,c))
               .ForEach(x => x.a.Customs_Procedure = x.c);
            return docs;
        }

        // Made async
        private static async Task<List<Customs_Procedure>> GetCustomsProceduresAsync(List<AsycudaDocument> docs)
        {
            var cpList = docs.Select(c => c.CustomsProcedure).Distinct().ToList();
             if (!cpList.Any()) return new List<Customs_Procedure>();

            var cps = await new AllocationDSContext().Customs_Procedure.Include(x => x.CustomsOperations).WhereBulkContains(cpList, x => x.CustomsProcedure).ToListAsync().ConfigureAwait(false);
            return cps;
        }

        // Made async
        private static async Task AddPiDataAsync(List<EX9AsycudaSalesAllocations> lst)
        {
            try
            {
                var allocationIdLst = lst.Select(x => x.AllocationId).Distinct().ToList();
                 if (!allocationIdLst.Any()) return;


                var piData = await new AllocationDSContext().AsycudaSalesAllocationsPIData
                        .WhereBulkContains(allocationIdLst, x => x.AllocationId).ToListAsync().ConfigureAwait(false);
                
                lst.GroupJoin(piData, a => a.AllocationId, p => p.AllocationId, (a, p) => (a, p))
                   .ForEach(x => x.a.AsycudaSalesAllocationsPIData = x.p.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Kept synchronous as it only involves in-memory operations and DB calls wrapped in Task.Run
        private static void AddEntryDataDetails(List<EX9AsycudaSalesAllocations> lst)
        {
            try
            {
                var entryDataDetailsLst = lst.Select(x => x.EntryDataDetailsId).Distinct().ToList();
                 if (!entryDataDetailsLst.Any()) return;
                var entryDataTask = Task.Run(() => new AllocationDSContext().EntryDataDetails
                    .WhereBulkContains(entryDataDetailsLst, x => x.EntryDataDetailsId).ToList());

                var itemEntryDataTask = Task.Run(() => new AllocationDSContext().AsycudaDocumentItemEntryDataDetails
                    .WhereBulkContains(entryDataDetailsLst, x => x.EntryDataDetailsId).ToList());

                Task.WaitAll(entryDataTask, itemEntryDataTask); // Still blocking here, but contained within the Task.Run in CreateAsync

                var entryDataData = entryDataTask.Result;
                var itemEntryData = itemEntryDataTask.Result;

                entryDataData.GroupJoin(itemEntryData, a => a.EntryDataDetailsId,
                        p => p.EntryDataDetailsId, (a, p) => (a, p))
                    .ForEach(x => x.a.AsycudaDocumentItemEntryDataDetails = x.p.ToList());

                lst.Join(entryDataData, a => a.EntryDataDetailsId, e => e.EntryDataDetailsId, (a, e) => (a, e))
                    .ForEach(x => { x.a.EntryDataDetails = x.e; });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<EX9AsycudaSalesAllocations> GetEx9AsycudaSalesAllocationsList(string rdateFilter)
        {
            List<EX9AsycudaSalesAllocations> lst;
            using (var ctx = new AllocationDSContext() { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;

                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

                lst = ctx.EX9AsycudaSalesAllocations
                    .Where(rdateFilter)
                    .AsNoTracking()
                    .ToList();
            }

            return lst;
        }


        public IEnumerable<EX9AsycudaSalesAllocations> Execute(string filterExpression)
        {
            if (_eX9AsycudaSalesAllocations == null) 
                throw new InvalidOperationException("GetEx9AsycudaSalesAllocationsMem has not been initialized. Call CreateAsync first.");

            foreach (var allocation in _eX9AsycudaSalesAllocations
                         .Select(x => x.Value)
                         .AsQueryable()
                         .Where(filterExpression))
            {
                yield return allocation;
            }

        }
    }
}