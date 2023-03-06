using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingEntryDataDetails;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.SavingAllocations;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets;
using static sun.security.jca.GetInstance;

namespace WaterNut.DataSpace
{
    public class OldSalesAllocator
    {
       
        


        public static bool isDBMem { get; } = false;

        static OldSalesAllocator()
        {
        }

        public OldSalesAllocator()
        {
       
        }

   

        public async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
            int applicationSettingsId, bool allocateToLastAdjustment, string lst)
        {
            var itemSets = BaseDataModel.GetItemSets(lst);
            itemSets.AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .ForAll(async x =>
                    await AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment,x).ConfigureAwait(false));

        }

        public async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(bool allocateToLastAdjustment,
            List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            try
            {
                var itemSets = await MatchSalestoAsycudaEntriesOnItemNumber(itemSetLst).ConfigureAwait(false);
                StatusModel.StopStatusUpdate();

                var alloLst = await new SalesAllocator(itemSets.asycudaItems)
                    .AllocateSales(allocateToLastAdjustment, itemSets.itmLst.Values.Select(x => x).ToList())
                    .ConfigureAwait(false);


                SaveAllocations(alloLst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void SaveAllocations(List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)> alloLst)
        {
            new  SaveAllocationSQL().SaveAllocations(alloLst);
        }


        private async Task<(ConcurrentDictionary<int, xcuda_Item> asycudaItems, ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> itmLst)> MatchSalestoAsycudaEntriesOnItemNumber(
            List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            try
            {
                var startTime = DateTime.Now;
               

                var asycudaEntriesTask = Task.Run(async () => await GeAsycudaEntriesWithItemNumber.GetAsycudaEntriesWithItemNumber(itemSetLst, null).ConfigureAwait(false));
                var saleslstTask = Task.Run(async () => await GetSaleslstWithItemNumber(itemSetLst).ConfigureAwait(false));
                var adjlstTask = Task.Run(async () => await GetAdjustmentslstWithItemNumber(itemSetLst).ConfigureAwait(false));
                var dislstTask = Task.Run(async () => await GetDiscrepancieslstWithItemNumber(itemSetLst).ConfigureAwait(false));

                Task.WaitAll(asycudaEntriesTask, saleslstTask, adjlstTask, dislstTask);

                var asycudaEntries = asycudaEntriesTask.Result;
                var saleslst = saleslstTask.Result;
                var adjlst = adjlstTask.Result;
                var dislst = dislstTask.Result;

                var duration = startTime.Subtract(DateTime.Now).TotalSeconds;
                saleslst.AddRange(adjlst);
                saleslst.AddRange(dislst);


                var sSales = saleslst.OrderBy(x => x.Key.EntryDataDate).ToList();
                var itmLst = CreateItemSetsWithItemNumbers(sSales, asycudaEntries.asycudaEntries);

                return (asycudaEntries.asycudaItems, itmLst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private 
            ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> CreateItemSetsWithItemNumbers(
                IEnumerable<AllocationsBaseModel.ItemSales> saleslst, IEnumerable<AllocationsBaseModel.ItemEntries> asycudaEntries)
        {
            try
            {
                var itmLst = CreateItemSets(saleslst, asycudaEntries);

                var res = CreateItemSets(itmLst);

                AddLumpAndAliasData(asycudaEntries, res);

                return res;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private  IEnumerable<AllocationsBaseModel.ItemSet> CreateItemSets(IEnumerable<AllocationsBaseModel.ItemSales> saleslst, IEnumerable<AllocationsBaseModel.ItemEntries> asycudaEntries)
        {
            var itmLst = from s in saleslst
                join a in asycudaEntries on s.Key.ItemNumber equals a.Key into j
                from a in j.DefaultIfEmpty()
                select new AllocationsBaseModel.ItemSet
                {
                    Key = s.Key,
                    SalesList = s.SalesList,
                    EntriesList = a?.EntriesList ?? new List<xcuda_Item>()
                };
            return itmLst;
        }

        private  void AddLumpAndAliasData(IEnumerable<AllocationsBaseModel.ItemEntries> asycudaEntries, ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> res)
        {
            var lumpedItems = GetLumpedItems()
                .ToList();
            var aliasCache = new ConcurrentDictionary<(string ItemNumber, string AliasName), InventoryItemAlias>(GetInventoryItemAliases());
            var itemsCache = new ConcurrentDictionary<string, List<xcuda_Item>>(asycudaEntries.ToDictionary(k => k.Key, v => v.EntriesList));

         
            var aliasLst = aliasCache.GroupJoin(res, a => a.Key.ItemNumber, r => r.Key.ItemNumber,
                (a, r) => (Alias: a, Item: r)).Where(x => x.Item.Any()).ToList();

            var raliasLst = aliasCache.GroupJoin(res, a => a.Value.AliasName, r => r.Key.ItemNumber,
                (a, r) => (Alias: a, Item: r)).Where(x => x.Item.Any()).ToList();

      


            aliasLst.AddRange(raliasLst);

            var lumpedAliasLst = aliasLst.Join(lumpedItems, x => x.Alias.Key.AliasName, y => y.AliasName,
                (x, y) => x).Distinct().ToList();


            aliasLst
                //.AsParallel()
                //.WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                //                                         BaseDataModel.Instance.ResourcePercentage))
                //.Where(x => x.Item.Any())
                //.ForAll(r => AddData(r, itemsCache));
                .ForEach(r => AddData(r, itemsCache));

            lumpedAliasLst
                //.AsParallel()
                //.WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                //                                         BaseDataModel.Instance.ResourcePercentage))
                //.ForAll(r => AddData(r, itemsCache ));
                .ForEach(r => AddData(r, itemsCache));

            res
                //.AsParallel()
                //.WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                //                                         BaseDataModel.Instance.ResourcePercentage))
                .Where(x => x.Value.SalesList.Any(s => s.ManualAllocations != null))
                //.ForAll(r => AddManualAllocation(r, itemsCache));
                .ForEach(r => AddManualAllocation(r, itemsCache));
            res
                .Where(x => x.Value.EntriesList.Any(a => !string.IsNullOrEmpty(a.PreviousInvoiceItemNumber)))
                .ToList()
                //.AsParallel()
                //.WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                //                                         BaseDataModel.Instance.ResourcePercentage))
                .ForEach(r => AddPreviousInvoiceNumber(r, itemsCache));

            res.Where(x => x.Value.EntriesList.Any())
                //.AsParallel()
                //.WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                //                                         BaseDataModel.Instance.ResourcePercentage))
                //.ForAll(r => r.Value.EntriesList = r.Value.EntriesList.DistinctBy(x => x.Item_Id).ToList());
                .ForEach(r => r.Value.EntriesList = r.Value.EntriesList.DistinctBy(x => x.Item_Id).ToList());
        }

        private static Dictionary<(string, string), InventoryItemAlias> GetInventoryItemAliases()
        {
           // var duplicates = AllocationsBaseModel.Instance.InventoryAliasCache.Data.GroupBy(k => (k.ItemNumber.ToUpper(), k.AliasName.ToUpper())).Where(x => x.Count() > 1).ToList();
           /// took out the trim in case of spaces lots of other comparisons include the space 
            return AllocationsBaseModel.Instance.InventoryAliasCache.Data.ToDictionary(k => (k.ItemNumber.ToUpper(), k.AliasName.ToUpper()), v => v);
        }

        private static IEnumerable<InventoryItemAlias> GetLumpedItems()
        {
            return AllocationsBaseModel.Instance.InventoryAliasCache.Data.Where(x => x.InventoryItem.LumpedItem != null);
        }

        private void AddData(
            (KeyValuePair<(string ItemNumber, string AliasName), InventoryItemAlias> Alias, IEnumerable<
                KeyValuePair<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId),
                    AllocationsBaseModel.ItemSet>> Item) itemSet,
            ConcurrentDictionary<string, List<xcuda_Item>> itemsCache)
        {
            try
            {
                var res = itemsCache.Where(i => i.Key == itemSet.Alias.Key.AliasName)
                    .SelectMany(y => y.Value)
                    .ToList();

                var res1 = itemsCache.Where(i => i.Key == itemSet.Alias.Key.ItemNumber)
                    .SelectMany(y => y.Value)
                    .ToList();

                

                res.AddRange(res1);

                if (res.Any()) itemSet.Item.ForEach(i => i.Value.EntriesList.AddRange(res));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }

        }



        private  void AddPreviousInvoiceNumber(KeyValuePair<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> itemSet, ConcurrentDictionary<string, List<xcuda_Item>> itemsCache)
        {
            if (itemsCache.ContainsKey(itemSet.Key.ItemNumber))
                itemSet.Value.EntriesList.AddRange(itemsCache[itemSet.Key.ItemNumber].Where(x =>
                    x.PreviousInvoiceItemNumber == itemSet.Key.ItemNumber));
        }

        private  void AddManualAllocation(KeyValuePair<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> itemSet, ConcurrentDictionary<string, List<xcuda_Item>> itemsCache)
        {
            foreach (var itm in itemSet.Value.SalesList.Where(x => x.ManualAllocations != null))
            {
                var ritm = itemsCache[itm.ItemNumber]
                    .FirstOrDefault(x => x.Item_Id == itm.ManualAllocations.Item_Id);
                if (ritm != null) itemSet.Value.EntriesList.Add(ritm);
            }
        }

        private  ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> CreateItemSets(IEnumerable<AllocationsBaseModel.ItemSet> itmLst)
        {
            var res =
                new ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet>();

            itmLst //.Where(x => x.Key.ItemNumber == "TRC/1206-QC").ToList()//.Where(x => x.Key.ItemNumber.StartsWith("T")).ToList()//.Where(x => x.SalesList.Any(z => z.EntryDataId == "61091010")).ToList()
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .ForAll(itm =>
                {
                    res.AddOrUpdate(itm.Key, itm, (key, value) =>
                    {
                        //value.EntriesList.AddRange(itm.EntriesList);  ------ causes Duplicated entries
                        value.SalesList.AddRange(itm.SalesList);
                        value.SalesList = value.SalesList.OrderBy(x => x.Sales.EntryDataDate).ThenBy(x => x.EntryDataId)
                            .ToList();
                        return value;
                    });
                });

            return res;
        }

        //private static async Task<IEnumerable<AllocationsBaseModel.ItemEntries>> GetAsycudaEntriesWithItemNumber(int applicationSettingsId,
        //    int? asycudaDocumentSetId)
        //{
        //    var itemSets = BaseDataModel.GetItemSets(applicationSettingsId, null);
        //    return ParallelEnumerable.Select(itemSets
        //            .AsParallel()
        //            .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
        //                                                     BaseDataModel.Instance.ResourcePercentage))
        //            , async x => await GetAsycudaEntriesWithItemNumber(x, asycudaDocumentSetId).ConfigureAwait(false))
        //        .SelectMany(x => x.Result)
        //        .ToList();
        
        //}


        private static async Task<List<AllocationsBaseModel.ItemSales>> GetSaleslstWithItemNumber(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            try
            {
                StatusModel.Timer("Getting Data - Sales Entries...");

                return CreateSalesItemSetList(GetSales(itemSetLst));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<AllocationsBaseModel.ItemSales> CreateSalesItemSetList(IEnumerable<EntryDataDetails> entryDataDetails)
        {
            string lst;
            return entryDataDetails
                .GroupBy(d => (d.Sales.EntryDataDate, d.EntryDataId, d.ItemNumber.ToUpper().Trim(),
                    d.InventoryItemId))
                .Select(g => new AllocationsBaseModel.ItemSales
                {
                    Key = g.Key,
                    SalesList = g.Where(xy => xy != null && xy.Sales != null)
                        .OrderBy(x => x.Sales.EntryDataDate)
                        .ThenBy(x => x.EntryDataId)
                        .ToList()
                }).ToList();
        }

        private static IEnumerable<EntryDataDetails> GetSales(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            return Enumerable.Where<EntryDataDetails>(GetEntryDataDetails(itemSetLst), x => x.Sales != null)
                .Where(x => !string.IsNullOrEmpty(x.Sales.INVNumber))
                .ToList();
        }

        public static List<EntryDataDetails> GetEntryDataDetails(List<(string ItemNumber, int InventoryItemId)> lst, bool redo = false)
        {
            return isDBMem == true 
                ?  GetEntryDataDetailsOld.Execute(lst, redo)
                : GetEntryDataDetailsMem.Execute(lst);

            
        }

        private static async Task<List<AllocationsBaseModel.ItemSales>> GetAdjustmentslstWithItemNumber(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            
            StatusModel.Timer("Getting Data - Adjustments Entries...");

            return  UpdateAdjList(CreateADJItemSalesList(GetAdjustmentsData(itemSetLst)));
			
        }

        private static List<AllocationsBaseModel.ItemSales> UpdateAdjList(List<AllocationsBaseModel.ItemSales> adjlst)
        {
            adjlst.SelectMany(x => x.SalesList).ForEach(x =>
            {
                x.Sales = new Sales
                {
                    EntryDataId = x.Adjustments.EntryDataId,
                    EntryDataDate = Convert.ToDateTime(x.EffectiveDate),
                    INVNumber = x.Adjustments.EntryDataId,
                    Tax = x.Adjustments.Tax,
                    EntryDataType = "ADJ"
                };
                x.Comment = "Adjustment";
            });
            return adjlst;
        }

        private static List<AllocationsBaseModel.ItemSales> CreateADJItemSalesList(List<EntryDataDetails> salesData)
        {
            List<AllocationsBaseModel.ItemSales> adjlst;
            adjlst = (salesData
                //.Where(x => lst == null || lst.Contains(x.ItemNumber))
                .Where(x => x.Adjustments.Type == "ADJ")
                .GroupBy(d => (EntryDataDate: d.EffectiveDate ?? d.Adjustments.EntryDataDate, d.EntryDataId,
                    ItemNumber: d.ItemNumber.ToUpper().Trim(), d.InventoryItemId))
                .Select(g => new AllocationsBaseModel.ItemSales
                {
                    Key = g.Key,
                    SalesList = g.Where(xy => xy != null & xy.Adjustments != null)
                        .OrderBy(x => x.EffectiveDate)
                        .ThenBy(x => x.Adjustments.EntryDataDate)
                        .ThenBy(x => x.EntryDataId)
                        .ToList()
                })).ToList();
            return adjlst;
        }

        private static List<EntryDataDetails> GetAdjustmentsData(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            List<EntryDataDetails> salesData;
            using (var ctx = new AllocationDSContext() { StartTracking = false })
            {
                salesData = GetEntryDataDetails(itemSetLst)
                    //.Include(x => x.Adjustments)
                    //.Include(x => x.AsycudaSalesAllocations)
                    //.Where(x => string.IsNullOrEmpty(lst) || lst.Contains(x.ItemNumber))
                    .Where(x => x.Adjustments != null)
                    .Where(x => !string.IsNullOrEmpty(x.Adjustments.EntryDataId))
                    //.Where(x => x.Adjustments.EntryDataDate >=
                    //            BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate)
                    .Where(x => x.Adjustments.Type == "ADJ")
                    .Where(x => (x.PreviousInvoiceNumber == null) ||
                                (x.PreviousInvoiceNumber != null && x.QtyAllocated == 0))
                    .Where(x => x.ReceivedQty - x.InvoiceQty <= 0 && (x.EffectiveDate == null ||
                                                                      (x.EffectiveDate >= BaseDataModel.Instance
                                                                          .CurrentApplicationSettings
                                                                          .OpeningStockDate)))
                    //.Where(x => x.DoNotAllocate != true)
                    .ToList();

            }

            return salesData;
        }

        private static async Task<List<AllocationsBaseModel.ItemSales>> GetDiscrepancieslstWithItemNumber(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            try
            { 
                StatusModel.Timer("Getting Data - Discrepancy Errors ...");

                return  UpdateDISItemSets(CreateDISItemSalesList(GetDiscrepanciesData(itemSetLst)));
				
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<AllocationsBaseModel.ItemSales> UpdateDISItemSets(List<AllocationsBaseModel.ItemSales> adjlst)
        {
            adjlst.SelectMany(x => x.SalesList).ForEach(x =>
            {
                x.Sales = new Sales
                {
                    EntryDataId = x.Adjustments.EntryDataId,
                    EntryDataDate = Convert.ToDateTime(x.EffectiveDate),
                    INVNumber = x.Adjustments.EntryDataId,
                    EntryDataType = "DIS"
                };
                x.Comment = "Adjustment";
                x.Quantity = (double) (x.InvoiceQty - x.ReceivedQty); // switched it so its positive
            });
            return adjlst;
        }

        private static List<AllocationsBaseModel.ItemSales> CreateDISItemSalesList(List<EntryDataDetails> salesData)
        {
            List<AllocationsBaseModel.ItemSales> adjlst;
            adjlst = (salesData
                //.Where(x => lst == null || lst.Contains(x.ItemNumber))
                //                  .Where(x => x.Adjustments.Type == "DIS")
                .Where(x => x.IsReconciled != true)
                .GroupBy(d => (EntryDataDate: d.EffectiveDate ?? d.Adjustments.EntryDataDate, d.EntryDataId,
                    ItemNumber: d.ItemNumber.ToUpper().Trim(), d.InventoryItemId))
                .Select(g => new AllocationsBaseModel.ItemSales
                {
                    Key = g.Key,
                    SalesList = g.Where(xy => xy != null & xy.Adjustments != null)
                        .OrderBy(x => x.EffectiveDate)
                        .ThenBy(x => x.Adjustments.EntryDataDate)
                        .ThenBy(x => x.EntryDataId)
                        .ToList()
                })).ToList();
            return adjlst;
        }

        private static List<EntryDataDetails> GetDiscrepanciesData(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            
           
            var salesData = GetEntryDataDetails(itemSetLst)
                .Where(x => x.Adjustments != null)
                //.Where(x => string.IsNullOrEmpty(lst) || lst.Contains(x.ItemNumber))
                //.Where(x => !string.IsNullOrEmpty(x.Adjustments.EntryDataId))
                .Where(x => x.Adjustments.Type == "DIS")
                //.Where(x => x.Adjustments.EntryDataDate >=
                //            BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate)
                //.Where(x => x.Adjustments.ApplicationSettingsId ==
                //            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Where(x => x.QtyAllocated != x.Quantity || x.Adjustments.EntryDataId.Contains("Asycuda"))
                .Where(x => (x.Comment != null && x.Comment.StartsWith("DISERROR:")) ||  x.Adjustments.EntryDataId.Contains("Asycuda"))
                .Where(x => (x.PreviousInvoiceNumber == null) ||
                            (x.PreviousInvoiceNumber != null && x.QtyAllocated == 0))
                .Where(x => x.ReceivedQty - x.InvoiceQty <= 0 && (x.EffectiveDate == null ||
                                                                  (x.EffectiveDate >= BaseDataModel.Instance
                                                                      .CurrentApplicationSettings
                                                                      .OpeningStockDate)))
                // .Where(x => x.DoNotAllocate != true)
                .ToList();
            
           

            return salesData;
        }
    }
}