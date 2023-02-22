using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using MoreLinq;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations
{
    public class GetEx9AsycudaSalesAllocationsMem : IGetEx9AsycudaSalesAllocations
    {
        private ConcurrentDictionary<int , EX9AsycudaSalesAllocations> _eX9AsycudaSalesAllocations = null;

        static readonly object Identity = new object();
        public GetEx9AsycudaSalesAllocationsMem(string filterExp, string rdateFilter)
        {
            lock (Identity)
            {
                if (_eX9AsycudaSalesAllocations != null) return;
                try
                {
                    var lst = GetEx9AsycudaSalesAllocationsList(rdateFilter);

                    var piDataTask = AddPiData(lst);

                    var entryDataTask = Task.Run(() => AddEntryDataDetails(lst));

                    var pItemTask = Task.Run(() => AddPreviousItem(lst));

                    Task.WaitAll(piDataTask,entryDataTask, pItemTask);

                    _eX9AsycudaSalesAllocations = new ConcurrentDictionary<int, EX9AsycudaSalesAllocations>(lst.ToDictionary(x => x.AllocationId, x => x));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private Task AddPreviousItem(List<EX9AsycudaSalesAllocations> lst)
        {
            var plst = lst.Select(x => x.PreviousItem_Id).Distinct().ToList();

            var pItemLst = new AllocationDSContext().xcuda_Item.WhereBulkContains(plst, x => x.Item_Id).ToList();
            var docLst = pItemLst.Select(x => x.ASYCUDA_Id).Distinct().ToList();

            var entryPreviousItemsTask = Task.Run(() =>
                    new AllocationDSContext().EntryPreviousItems.WhereBulkContains(plst).ToList())
                .ContinueWith(x =>
                {
                    var prevIds = x.Result.Select((q => q.PreviousItem_Id)).Distinct()
                        .ToList();
                    var pitms = GetXcudaPreviousItems(prevIds);
                    x.Result.Join(pitms, i => i.PreviousItem_Id, a => a.PreviousItem_Id, (i, a) =>
                    {
                        i.xcuda_PreviousItem = a;
                        return i;
                    });
                    return x.Result;
                })
                .ContinueWith(x => pItemLst.GroupJoin(x.Result, i => i.Item_Id, a => a.Item_Id,
                    (i, a) =>
                    {
                        i.EntryPreviousItems = a.ToList();
                        return i;
                    }));
            var tarificationTask = Task.Run(() =>
                new AllocationDSContext().xcuda_Tarification.WhereBulkContains(plst, x => x.Item_Id).ToList()).ContinueWith(x => pItemLst.Join(x.Result, i => i.Item_Id, a => a.Item_Id,
                (i, a) =>
                {
                    i.xcuda_Tarification = a;
                    return i;
                }));

            var DocumentsTask = Task.Run(() =>
                new AllocationDSContext().AsycudaDocument.WhereBulkContains(docLst).ToList()).ContinueWith(x => pItemLst.Join(x.Result, i => i.ASYCUDA_Id, a => a.ASYCUDA_Id,
                (i, a) =>
                {
                    i.AsycudaDocument = a;
                    return i;
                }));

            return Task.WhenAll(entryPreviousItemsTask, tarificationTask, DocumentsTask);

            //.Include(
            //    "PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
            //.Include("PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations")
            //.Include(
            //    "PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps")
        }

        private static List<xcuda_PreviousItem> GetXcudaPreviousItems(List<int> prevIds)
        {
            var previousItemsTask = Task.Run(() => new AllocationDSContext().xcuda_PreviousItem
                .WhereBulkContains(prevIds, z => z.PreviousItem_Id).ToList());


            var xcudaItemsTask = Task.Run(() => new AllocationDSContext().xcuda_Item
                .WhereBulkContains(prevIds, z => z.Item_Id).ToList()).ContinueWith(x => AddAsycudaDocuments(x.Result));

            Task.WaitAll(previousItemsTask, xcudaItemsTask);

            var xitms = xcudaItemsTask.Result;
            var pitms = previousItemsTask.Result;

            var xcudaPreviousItems = pitms.Join(xitms, p => p.PreviousItem_Id, x => x.Item_Id, (p, x) =>
            {
                p.xcuda_Item = x;
                return p;
            }).ToList();

            return xcudaPreviousItems;
        }

        private static List<xcuda_Item> AddAsycudaDocuments(List<xcuda_Item> x)
        {
            var docs = GetAsycudaDocuments(x);

            var xcudaItems = x.Join(docs, i => i.ASYCUDA_Id, d => d.ASYCUDA_Id, (i, d) =>
            {
                i.AsycudaDocument = d;
                return i;
            }).ToList();
            return xcudaItems;
        }

        private static List<AsycudaDocument> GetAsycudaDocuments(List<xcuda_Item> x)
        {
            var docLst = x.Select(d => d.ASYCUDA_Id).ToList();

            var docs = new AllocationDSContext().AsycudaDocument.WhereBulkContains(docLst).ToList();

            return AddCustomsProcedures(docs);
        }

        private static List<AsycudaDocument> AddCustomsProcedures(List<AsycudaDocument> docs)
        {
            var cps = GetCustomsProcedures(docs);

            var rdos = docs.Join(cps, d => d.CustomsProcedure, c => c.CustomsProcedure, (a, c) =>
            {
                a.Customs_Procedure = c;
                return a;
            }).ToList();
            return rdos;
        }

        private static List<Customs_Procedure> GetCustomsProcedures(List<AsycudaDocument> docs)
        {
            var cpList = docs.Select(c => c.CustomsProcedure).ToList();

            var cps = new AllocationDSContext().Customs_Procedure.WhereBulkContains(cpList).ToList();
            return cps;
        }

        private static Task AddPiData(List<EX9AsycudaSalesAllocations> lst)
        {
            var allocationIdLst = lst.Select(x => x.AllocationId).Distinct().ToList();


            var piDataTask = Task.Run(() =>
            {
                return new AllocationDSContext().AsycudaSalesAllocationsPIData
                    .WhereBulkContains(allocationIdLst, x => x.AllocationId).ToList();
            }).ContinueWith((t) => lst.GroupJoin(t.Result, a => a.AllocationId, p => p.AllocationId, (a, p) => (a, p))
                .ForEach(x => x.a.AsycudaSalesAllocationsPIData = x.p.ToList()));
            return piDataTask;
        }

        private static void AddEntryDataDetails(List<EX9AsycudaSalesAllocations> lst)
        {
            var entryDataDetailsLst = lst.Select(x => x.EntryDataDetailsId).Distinct().ToList();
            var entryDataTask = Task.Run(() => new AllocationDSContext().EntryDataDetails
                .WhereBulkContains(entryDataDetailsLst, x => x.EntryDataDetailsId).ToList());

            var itemEntryDataTask = Task.Run(() => new AllocationDSContext().AsycudaDocumentItemEntryDataDetails
                .WhereBulkContains(entryDataDetailsLst, x => x.EntryDataDetailsId).ToList());

            Task.WaitAll(entryDataTask, itemEntryDataTask);

            var entryDataData = entryDataTask.Result;
            var itemEntryData = itemEntryDataTask.Result;



            lst.Join(entryDataData.GroupJoin(itemEntryData, a => a.EntryDataDetailsId,
                        p => p.EntryDataDetailsId, (a, p) => (a, p))
                    .Select(x =>
                    {
                        x.a.AsycudaDocumentItemEntryDataDetails = x.p.ToList();
                        return x.a;
                    }), a => a.EntryDataDetailsId, e => e.EntryDataDetailsId, (a, e) => (a, e))
                .ForEach(x => { x.a.EntryDataDetails = x.e; });
           
        }

        private static List<EX9AsycudaSalesAllocations> GetEx9AsycudaSalesAllocationsList(string rdateFilter)
        {
            List<EX9AsycudaSalesAllocations> lst;
            using (var ctx = new AllocationDSContext() { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;

                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

                //if (_ex9AsycudaSalesAllocations == null)
                //{
                //////////////////////Cant load all data in memory too much
                lst = ctx.EX9AsycudaSalesAllocations
                    .Where(rdateFilter)
                    //.Include(x => x.AsycudaSalesAllocationsPIData)
                    //.Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
                    //.Include(
                    //    "PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
                    //.Include("PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations")
                    //.Include(
                    //    "PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps")
                    .AsNoTracking()
                    .ToList();
                //.AsQueryable()
                //.Where(filterExp).ToList();
                //}
            }

            return lst;
        }


        public async Task<List<EX9AsycudaSalesAllocations>> Execute(string filterExpression)
        {
            try
            {
                return _eX9AsycudaSalesAllocations
                    .Select(x => x.Value)
                    .AsQueryable()
                    .Where(filterExpression)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}