using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingGetUOAllocations;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.DataSpace
{
    public class MarkErrors
    {
        private ConcurrentDictionary<int, xcuda_Item> currentItems = new ConcurrentDictionary<int, xcuda_Item>();
        private static bool isDBMem = false;

        static MarkErrors()
        {
        }

        public MarkErrors()
        {
        }

        public Task Execute(int applicationSettingsId, string lst=null)
        {
            var itemSets = BaseDataModel.GetItemSets(lst);
            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage))
                .ForAll(async x => await Execute(x).ConfigureAwait(false));
            return Task.CompletedTask;
        }

        public async Task Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)
        {
            var lst = new GetXcudaItemsMem().Execute(null,itemSetLst);

            // var res2 = lst.Where(x => x.ItemNumber == "PRM/84101");
            currentItems = new ConcurrentDictionary<int, xcuda_Item>(Enumerable.ToDictionary<xcuda_Item, int, xcuda_Item>(lst, x => x.Item_Id, x => x));
                
            
            // MarkNoAsycudaEntry();

            await MarkOverAllocatedEntries(itemSetLst).ConfigureAwait(false);

            await MarkUnderAllocatedEntries(itemSetLst).ConfigureAwait(false);


        }

        private async Task MarkOverAllocatedEntries(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            
            var allocations = await GetUOAllocations(GetOverAllocatedAsycudaEntries(itemList)).ConfigureAwait(false);

            var sqlLst = Enumerable.ToList<string>(allocations.Select(CreateOverAllocatedSQL));

            sqlLst.Where(x => !string.IsNullOrEmpty(x)).ForEach(sql => new AllocationDSContext().Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql));

            CleanupZeroAllocated();

        }

        private static List<int> GetOverAllocatedAsycudaEntries(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            return isDBMem == true
                ? new GetOverAllocatedAsycudaEntries().Execute(itemList)
                : new GetOverAllocatedAsycudaEntriesMem().Execute(itemList);
        }

        private string CreateOverAllocatedSQL(IGrouping<xcuda_Item, AsycudaSalesAllocations> lst)
        {
          
            var sql = "";
            
            foreach (var allo in lst)
            {
                var tot = lst.Key.QtyAllocated - lst.Key.ItemQuantity;
                var r = tot > allo.QtyAllocated ? allo.QtyAllocated : tot;
                if (lst.Key.QtyAllocated > lst.Key.ItemQuantity)
                {
                    allo.QtyAllocated -= r;
                    sql += $@" UPDATE       AsycudaSalesAllocations
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")}
															where	AllocationId = {allo.AllocationId}";

                    allo.EntryDataDetails.QtyAllocated -= r;
                    sql += $@" UPDATE       EntryDataDetails
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")}
															where	EntryDataDetailsId = {allo.EntryDataDetails.EntryDataDetailsId}";

                    if (allo.EntryDataDetails.EntryDataDetailsEx.DutyFreePaid == "Duty Free")
                    {
                        allo.PreviousDocumentItem.DFQtyAllocated -= r;
                        lst.Key.DFQtyAllocated -= r;

                        /////// is the same thing

                        sql += $@" UPDATE       xcuda_Item
															SET                DFQtyAllocated = (DFQtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                    }
                    else
                    {
                        allo.PreviousDocumentItem.DPQtyAllocated -= r;
                        lst.Key.DPQtyAllocated -= r;


                        sql += $@" UPDATE       xcuda_Item
															SET                DPQtyAllocated = (DPQtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                    }

                    var existingStock = CheckExistingStock(allo);
                    if (allo.QtyAllocated == 0)
                    {
                        allo.QtyAllocated = r; //add back so wont disturb calculations
                        allo.Status = $"Over Allocated Entry by {r}{existingStock}";

                        sql += $@"  Update AsycudaSalesAllocations
														Set Status = '{allo.Status}', QtyAllocated = {r}
														Where AllocationId = {allo.AllocationId}";
                    }
                    else
                    {
                        sql += $@" INSERT INTO AsycudaSalesAllocations
														 (EntryDataDetailsId, PreviousItem_Id, QtyAllocated,Status, EANumber, SANumber)
														VALUES        ({allo.EntryDataDetailsId},{allo.PreviousItem_Id},{r},'Over Allocated Entry by {r}{existingStock}',0,0)";
                        //ctx.ApplyChanges(nallo);
                        break;
                    }
                }
            }

            return sql;

        }

        private string CheckExistingStock(AsycudaSalesAllocations allo)
        {
          
            return AllocationsBaseModel.isDBMem
                ? new CheckExistingStockDB().Execute(allo.PreviousDocumentItem.ItemNumber,
                    allo.EntryDataDetails.EntryData.EntryDataDate)
                : new CheckExistingStockMemory(currentItems).Execute(allo.PreviousDocumentItem.ItemNumber,
                    allo.EntryDataDetails.EntryData.EntryDataDate);
        }

        private async Task MarkUnderAllocatedEntries(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            var allocations = await GetUOAllocations(GetUnderAllocatedAsycudaItems(itemList)).ConfigureAwait(false);

            var sqlLst =Enumerable.ToList<string>(allocations.Select(CreateUnderAllocatedSql));

            sqlLst.Where(x => !string.IsNullOrEmpty(x)).ForEach(sql => new AllocationDSContext().Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql));
            
            CleanupZeroAllocated();
        }

        private static List<int> GetUnderAllocatedAsycudaItems(List<(string ItemNumber, int InventoryItemId)> itemList)
        {
            return isDBMem
                ? new GetUnderAllocatedAsycudaItems().Execute(itemList)
                : new GetUnderAllocatedAsycudaItemsMem().Execute(itemList);
        }

        private static void CleanupZeroAllocated()
        {
            using (var ctx = new AllocationDSContext())
            {
                var sql = @" DELETE FROM AsycudaSalesAllocations
								WHERE(Status IS NULL) AND(QtyAllocated = 0)";

                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
            }
        }

        private static string CreateUnderAllocatedSql(IGrouping<xcuda_Item, AsycudaSalesAllocations> data)
        {
            var sql = "";
            var lst = data.DistinctBy(x => x.AllocationId).ToList();
            if (lst.Sum(x => x.QtyAllocated) >= 0) return sql;
            foreach (var allo in lst)
            {
                var tot = data.Key.QtyAllocated * -1;
                var r = tot > (allo.QtyAllocated * -1) ? allo.QtyAllocated * -1 : tot;
                if (data.Key.QtyAllocated >= 0) return sql;
                allo.QtyAllocated += r;
                sql += $@" UPDATE       AsycudaSalesAllocations
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")}
															where	AllocationId = {allo.AllocationId}";

                allo.EntryDataDetails.QtyAllocated += r;
                sql += $@" UPDATE       EntryDataDetails
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")}
															where	EntryDataDetailsId = {allo.EntryDataDetails.EntryDataDetailsId}";

                if (allo.EntryDataDetails.EntryDataDetailsEx.DutyFreePaid == "Duty Free")
                {
                    allo.PreviousDocumentItem.DFQtyAllocated += r;
                    data.Key.DFQtyAllocated += r;

                    /////// is the same thing

                    sql += $@" UPDATE       xcuda_Item
															SET                DFQtyAllocated = (DFQtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                }
                else
                {
                    allo.PreviousDocumentItem.DPQtyAllocated += r;
                    data.Key.DPQtyAllocated += r;


                    sql += $@" UPDATE       xcuda_Item
															SET                DPQtyAllocated = (DPQtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                }

                if (allo.QtyAllocated == 0)
                {
                    allo.QtyAllocated -= r; //add back so wont disturb calculations
                    allo.Status = $"Under Allocated by {r}";

                    sql += $@"  Update AsycudaSalesAllocations
														Set Status = '{allo.Status}', QtyAllocated = (QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
														Where AllocationId = {allo.AllocationId}";

                    allo.EntryDataDetails.QtyAllocated -= r;
                    sql += $@" UPDATE       EntryDataDetails
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")}
															where	EntryDataDetailsId = {allo.EntryDataDetails.EntryDataDetailsId}";
                }
                else
                {
                    sql += $@" INSERT INTO AsycudaSalesAllocations
														 (EntryDataDetailsId, PreviousItem_Id, QtyAllocated,Status, EANumber, SANumber)
														VALUES        ({allo.EntryDataDetailsId},{allo.PreviousItem_Id},{r},'Under Allocated by {r}',0,0)";
                    //allo.EntryDataDetails.QtyAllocated -= r;
                    sql += $@" UPDATE       EntryDataDetails
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"+{r}" : $"+{r * -1}")}
															where	EntryDataDetailsId = {allo.EntryDataDetails.EntryDataDetailsId}";
                    //ctx.ApplyChanges(nallo);
                    break;
                }

                return sql;
            }

            return sql;
        }

        private static async Task<List<IGrouping<xcuda_Item, AsycudaSalesAllocations>>> GetUOAllocations(List<int> itemList)
        {
            // Assuming GetUOAllocations().Execute is synchronous or already handled
            return AllocationsBaseModel.isDBMem
                ? new GetUOAllocations().Execute(itemList)
                : (await GetUOAllocationsMem.CreateAsync().ConfigureAwait(false)).Execute(itemList);
        }
    }
}