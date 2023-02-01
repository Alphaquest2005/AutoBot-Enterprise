using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using Core.Common.Extensions;
using MoreLinq;

namespace WaterNut.DataSpace
{
    public class SQLSaveAllocationData
    {
        public  void SaveAllocations(List<List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>> alloLst)
        {
            var allocations = alloLst.SelectMany(x => x.Select(z => z.Value.dbAllocations.ToList()).ToList()).SelectMany(x => x.ToList());
            SaveAllocations(allocations);

            var entryDataDetails = alloLst.SelectMany(x => x.Select(z => z.Value.EntryDataDetails.ToList()).ToList()).SelectMany(x => x.ToList());
            SaveEntryDataDetails(entryDataDetails);

            var xCudaItems = alloLst.SelectMany(x => x.Select(z => z.Value.XcudaItems.ToList()).ToList()).SelectMany(x => x.ToList());
            SaveXcudaItems(xCudaItems);
        }

        public  void SaveXcudaItems(IEnumerable<xcuda_Item> xCudaItems)
        {
            xCudaItems
                .Partition(100)
                .ForEach(x =>
                {
                    var sql = x
                        .DistinctBy(a => a.Item_Id)
                        .Select<xcuda_Item, string>(a => SaveAsycudaItemSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
                    SaveSql(sql);
                });
        }

        public  void SaveEntryDataDetails(IEnumerable<EntryDataDetails> entryDataDetails)
        {
            entryDataDetails
                .Partition(100)
                .ForEach(x =>
                {
                    var sql = x
                        .DistinctBy(a => a.EntryDataDetailsId)
                        .Select<EntryDataDetails, string>(a => SaveEntryDataDetailsSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
                    SaveSql(sql);
                });
        }

        public  void SaveAllocations(IEnumerable<AsycudaSalesAllocations> allocations)
        {
            allocations
                .Partition(100)
                .ForEach(x =>
                {
                    var sql = x
                        .Select<AsycudaSalesAllocations, string>(a => SaveAllocationSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
                    SaveSql(sql);
                });
        }

        private  void SaveSql(string sql)
        {
            try
            {
                if (string.IsNullOrEmpty(sql)) return;
                new AllocationDSContext {StartTracking = false}.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private  string SaveAllocationSql(AsycudaSalesAllocations ssa)
        {
          
            return $@" INSERT INTO AsycudaSalesAllocations
														 (EntryDataDetailsId, PreviousItem_Id, QtyAllocated, EANumber, SANumber, Status, xEntryItem_Id, xStatus)
														VALUES        ({ssa.EntryDataDetailsId},{(ssa.PreviousItem_Id == null ? "NULL" : ssa.PreviousItem_Id.ToString())},{ssa.QtyAllocated},0,0,{(ssa.Status == null ? "NULL" : $"'{ssa.Status}'")},{(ssa.xEntryItem_Id == null ? "Null" : ssa.xEntryItem_Id.ToString()) },{(ssa.xStatus == null ? "NULL" : $"'{ssa.xStatus}'")});";
            
        }

        private  string SaveEntryDataDetailsSql(EntryDataDetails saleitm)
        {
            return $@"UPDATE       EntryDataDetails
															SET                QtyAllocated = {saleitm.QtyAllocated}
															where	EntryDataDetailsId = {saleitm.EntryDataDetailsId} ;";

        }

        private  string SaveAsycudaItemSql(xcuda_Item cAsycudaItm)
        {
            return $@"UPDATE       xcuda_Item
															SET                DPQtyAllocated = {
                                                                cAsycudaItm.DPQtyAllocated
                                                            }, DFQtyAllocated = {cAsycudaItm.DFQtyAllocated}
															where	item_id = {cAsycudaItm.Item_Id};";
          
        }

        public void Save((List<AsycudaSalesAllocations> allocations, List<EntryDataDetails> entryDataDetails, List<xcuda_Item> pItems) allocations)
        {
            SaveAllocations(allocations.allocations);
            SaveEntryDataDetails(allocations.entryDataDetails);
            SaveXcudaItems(allocations.pItems);
        }
    }
}