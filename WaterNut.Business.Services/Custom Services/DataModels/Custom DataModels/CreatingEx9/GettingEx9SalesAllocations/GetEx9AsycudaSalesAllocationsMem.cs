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
                    var lst = GetEx9AsycudaSalesAllocationsList(rdateFilter).ToList();//.Where(x => x.AllocationId == 36652618)

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

        private void AddPreviousItem(List<EX9AsycudaSalesAllocations> lst)
        {
            try
            {
                var plst = lst.Select(x => x.PreviousItem_Id).Distinct().ToList();

                var pItemLst = new AllocationDSContext().xcuda_Item.WhereBulkContains(plst, x => x.Item_Id).ToList();

                var entryPreviousItemsTask = Task.Run(() =>
                        new AllocationDSContext().EntryPreviousItems.WhereBulkContains(plst, p => p.Item_Id).ToList())
                    .ContinueWith(x => AddXcudaPreviousItems(x.Result))
                    .ContinueWith(x => pItemLst.GroupJoin(x.Result, i => i.Item_Id, a => a.Item_Id,
                            (i, a) => (i, a))
                        .ForEach(z => z.i.EntryPreviousItems = z.a.ToList()));
                var tarificationTask = Task.Run(() => GetXcudaTarifications(plst))

                    .ContinueWith(x => AddTarifications(pItemLst, x));

                var documentsTask = Task.Run(() => AddAsycudaDocuments(pItemLst));

                Task.WaitAll(entryPreviousItemsTask, tarificationTask, documentsTask);

                lst.Join(pItemLst, a => a.PreviousItem_Id, p => p.Item_Id, (a, p) => (a, p))
                    .ForEach(x => x.a.PreviousDocumentItem = x.p);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //.Include(
            //    "PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
            //.Include("PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations")
            //.Include(
            //    "PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps")
        }

        private static List<xcuda_Item> AddTarifications(List<xcuda_Item> pItemLst, Task<List<xcuda_Tarification>> tarifcations)
        {
             pItemLst.Join(tarifcations.Result, i => i.Item_Id, a => a.Item_Id,
                (i, a) => (i,a))
                .ForEach(x => x.i.xcuda_Tarification = x.a);
             return pItemLst;
        }

        private List<xcuda_Tarification> GetXcudaTarifications(List<int> plst)
        {
            var xcudaTarificationTask = Task.Run(() => new AllocationDSContext().xcuda_Tarification.WhereBulkContains(plst, x => x.Item_Id).ToList());

            var hsCodesTask = Task.Run(() => GetHsCodes(plst));

            Task.WaitAll(xcudaTarificationTask, hsCodesTask);
            var xcudaTarifications = xcudaTarificationTask.Result;
            var hsCodes = hsCodesTask.Result;

            xcudaTarifications.Join(hsCodes, t => t.Item_Id, h => h.Item_Id, (t,h) => (t,h))
                .ForEach(x => x.t.xcuda_HScode = x.h);

            return xcudaTarifications;
        }

        private List<xcuda_HScode> GetHsCodes(List<int> plst)
        {
            var xcudaHScodes = new AllocationDSContext().xcuda_HScode.WhereBulkContains(plst, x => x.Item_Id).ToList();
            return AddTariffCodes(xcudaHScodes);
            
        }

        private static List<xcuda_HScode> AddTariffCodes(List<xcuda_HScode> xcudaHScodes)
        {
            var tariffCodeLst = xcudaHScodes.Select(x => x.Commodity_code).Distinct().ToList();
            var tariffCodes = new AllocationDSContext().TariffCodes.WhereBulkContains(tariffCodeLst, t => t.TariffCode).ToList();
            AddTariffCategorys(tariffCodes);


           xcudaHScodes.Join(tariffCodes, h => h.Commodity_code, t => t.TariffCode, (h, t) => (h, t))
               .ForEach(x => x.h.TariffCodes = x.t);

           return xcudaHScodes;
        }

        private static void AddTariffCategorys(List<TariffCodes> TariffCodes)
        {
            var tarifCatLst = TariffCodes.Select(x => x.TariffCategoryCode).Distinct().ToList();
            var tariffCategorys = new AllocationDSContext().TariffCategory.WhereBulkContains(tarifCatLst, x => x.TariffCategoryCode)
                .Distinct().ToList();
            AddTarifCategoryCodeSuppUnits(tariffCategorys);

            TariffCodes.Join(tariffCategorys, t => t.TariffCategoryCode, c => c.TariffCategoryCode, (t, c) => (t,c))
                .ForEach(x => x.t.TariffCategory = x.c);
        }

        private static void AddTarifCategoryCodeSuppUnits(List<TariffCategory> tariffCategorys)
        {
            var tcLst  = tariffCategorys.Select(x => x.TariffCategoryCode).ToList();
            var tarifSuplst = new AllocationDSContext().TariffCategoryCodeSuppUnit.Include(z => z.TariffSupUnitLkps)
                .WhereBulkContains(tcLst, x => x.TariffCategoryCode).ToList();

            tariffCategorys.GroupJoin(tarifSuplst, c => c.TariffCategoryCode, s => s.TariffCategoryCode, (c, s) => (c,s))
                .ForEach(x => x.c.TariffCategoryCodeSuppUnit = x.s.ToList());
        }

        private static List<EntryPreviousItems> AddXcudaPreviousItems(List<EntryPreviousItems> entryPreviousItems)
        {
            var prevIds = entryPreviousItems.Select((q => q.PreviousItem_Id)).Distinct()
                .ToList();
            var pitms = GetXcudaPreviousItems(prevIds);
            entryPreviousItems.Join(pitms, i => i.PreviousItem_Id, a => a.PreviousItem_Id, (i, a) =>(i,a))
                .ForEach(x => x.i.xcuda_PreviousItem = x.a);
            return entryPreviousItems.Where(x => x.xcuda_PreviousItem != null).ToList();
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

             pitms.Join(xitms, p => p.PreviousItem_Id, x => x.Item_Id, (p, x) => (p,x))
                .ForEach(z => z.p.xcuda_Item = z.x);

            return pitms;
        }

        private static List<xcuda_Item> AddAsycudaDocuments(List<xcuda_Item> xcudaItems)
        {
            var docs = GetAsycudaDocuments(xcudaItems);

            xcudaItems.Join(docs, i => i.ASYCUDA_Id, d => d.ASYCUDA_Id, (i, d) =>(i,d))
                .ForEach(z => z.i.AsycudaDocument = z.d);
            return xcudaItems;
        }

        private static List<AsycudaDocument> GetAsycudaDocuments(List<xcuda_Item> x)
        {
            var docLst = x.Select(d => d.ASYCUDA_Id).Distinct().ToList();

            var docs = new AllocationDSContext().AsycudaDocument.WhereBulkContains(docLst, a => a.ASYCUDA_Id).ToList();

            return AddCustomsProcedures(docs);
        }

        private static List<AsycudaDocument> AddCustomsProcedures(List<AsycudaDocument> docs)
        {
            var cps = GetCustomsProcedures(docs);

           docs.Join(cps, d => d.CustomsProcedure, c => c.CustomsProcedure, (a, c) => (a,c))
               .ForEach(x => x.a.Customs_Procedure = x.c);
            return docs;
        }

        private static List<Customs_Procedure> GetCustomsProcedures(List<AsycudaDocument> docs)
        {
            var cpList = docs.Select(c => c.CustomsProcedure).Distinct().ToList();

            var cps = new AllocationDSContext().Customs_Procedure.Include(x => x.CustomsOperations).WhereBulkContains(cpList, x => x.CustomsProcedure).ToList();
            return cps;
        }

        private static Task AddPiData(List<EX9AsycudaSalesAllocations> lst)
        {
            try
            {
                var allocationIdLst = lst.Select(x => x.AllocationId).Distinct().ToList();


                var piDataTask = Task.Run(() => new AllocationDSContext().AsycudaSalesAllocationsPIData
                        .WhereBulkContains(allocationIdLst, x => x.AllocationId).ToList())
                    .ContinueWith((t) => lst
                        .GroupJoin(t.Result, a => a.AllocationId, p => p.AllocationId, (a, p) => (a, p))
                        .ForEach(x => x.a.AsycudaSalesAllocationsPIData = x.p.ToList()));
                return piDataTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddEntryDataDetails(List<EX9AsycudaSalesAllocations> lst)
        {
            try
            {
                var entryDataDetailsLst = lst.Select(x => x.EntryDataDetailsId).Distinct().ToList();
                var entryDataTask = Task.Run(() => new AllocationDSContext().EntryDataDetails
                    .WhereBulkContains(entryDataDetailsLst, x => x.EntryDataDetailsId).ToList());

                var itemEntryDataTask = Task.Run(() => new AllocationDSContext().AsycudaDocumentItemEntryDataDetails
                    .WhereBulkContains(entryDataDetailsLst, x => x.EntryDataDetailsId).ToList());

                Task.WaitAll(entryDataTask, itemEntryDataTask);

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