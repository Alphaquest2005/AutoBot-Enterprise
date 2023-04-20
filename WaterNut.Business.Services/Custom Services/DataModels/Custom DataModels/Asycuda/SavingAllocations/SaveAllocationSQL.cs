using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using AllocationDS.Business.Entities;
using Core.Common.Extensions;
using MoreLinq;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.SavingAllocations
{
    public class SaveAllocationSQL : ISaveAllocationSQLProcessor
    {
        //public void SaveAllocations(List<List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>> alloLst)
        //{
        //    var allocations = alloLst.SelectMany(x => x.Select(z => z.Value.dbAllocations.ToList()).ToList()).SelectMany(x => x.ToList());
        //    SaveAllocations(allocations);

        //    var entryDataDetails = alloLst.SelectMany(x => x.Select(z => z.Value.EntryDataDetails.ToList()).ToList()).SelectMany(x => x.ToList());
        //    SaveEntryDataDetails(entryDataDetails);

        //    var xCudaItems = alloLst.SelectMany(x => x.Select(z => z.Value.XcudaItems.ToList()).ToList()).SelectMany(x => x.ToList());
        //    SaveXcudaItems(xCudaItems);
        //}

        public void SaveAllocations(List<List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>> alloLst)
        {
            var sql = "";
            var allocations = alloLst.SelectMany(x => x.Select(z => z.Value.dbAllocations.ToList()).ToList()).SelectMany(x => x.ToList());
            sql += allocations.Select(a => SaveAllocationSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            
            var entryDataDetails = alloLst.SelectMany(x => x.Select(z => z.Value.EntryDataDetails.ToList()).ToList()).SelectMany(x => x.ToList());
            sql += entryDataDetails
                .Select(a => SaveEntryDataDetailsSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            
            var xCudaItems = alloLst.SelectMany(x => x.Select(z => z.Value.XcudaItems.ToList()).ToList()).SelectMany(x => x.ToList());
            sql += xCudaItems.Select(a => SaveAsycudaItemSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            SaveSql(sql);
        }

        //public void SaveAllocations(List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)> alloLst)
        //{

        //    SaveAllocations(alloLst.SelectMany(z => z.Sales.Select(s => s.AsycudaSalesAllocations.ToList())).SelectMany(x => x.ToList()).ToList());
        //    SaveEntryDataDetails(alloLst.SelectMany(e => e.Sales.ToList()));
        //    SaveXcudaItems(alloLst.SelectMany(e => e.asycudaItems.ToList()));
        //}

        public void SaveAllocations(List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)> alloLst)
        {
            var sql = "";
            // Allocations
            sql += alloLst
                    .SelectMany(z => z.Sales.Select(s => s.AsycudaSalesAllocations.Where(a => a.AllocationId == 0)// to prevent re-saving existing allocations
                        .ToList()))
                    .SelectMany(x => x.ToList())
                    .ToList()
                    .Select(a => SaveAllocationSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            sql += alloLst.SelectMany(e => e.Sales.ToList()).DistinctBy(a => a.EntryDataDetailsId)
                .Select(a => SaveEntryDataDetailsSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            sql += alloLst.SelectMany(e => e.asycudaItems.ToList()).DistinctBy(a => a.Item_Id)
                .Select(a => SaveAsycudaItemSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            SaveSql(sql);
        }

        public void SaveAllocations((List<AsycudaSalesAllocations> allocations, List<EntryDataDetails> entryDataDetails, List<xcuda_Item> pItems) alloLst)
        {
            var sql = "";
            sql += alloLst.allocations
                .Select(a => SaveAllocationSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            sql += alloLst.entryDataDetails
                .Select(a => SaveEntryDataDetailsSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            sql += alloLst.pItems
                .Select(a => SaveAsycudaItemSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");

            SaveSql(sql);
        }


        public  void SaveXcudaItems(IEnumerable<xcuda_Item> xCudaItems)
        {
            //xCudaItems
            //    .Partition(100)
            //    .ForEach(x =>
            //    {
            //        var sql = x
            //            .DistinctBy(a => a.Item_Id)
            //            .Select<xcuda_Item, string>(a => SaveAsycudaItemSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            //        SaveSql(sql);
            //    });
            var sql = xCudaItems
                        .DistinctBy(a => a.Item_Id)
                        .Select(a => SaveAsycudaItemSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            SaveSql(sql);
        }

        public  void SaveEntryDataDetails(IEnumerable<EntryDataDetails> entryDataDetails)
        {
            //entryDataDetails
            //    .Partition(100)
            //    .ForEach(x =>
            //    {
            //        var sql = x
            //            .DistinctBy(a => a.EntryDataDetailsId)
            //            .Select<EntryDataDetails, string>(a => SaveEntryDataDetailsSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            //        SaveSql(sql);
            //    });
            var sql = entryDataDetails.DistinctBy(a => a.EntryDataDetailsId)
                .Select(a => SaveEntryDataDetailsSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            SaveSql(sql);
        }

        public  void SaveAllocations(IEnumerable<AsycudaSalesAllocations> allocations)
        {
            //allocations
            //    .Partition(100)
            //    .ForEach(x =>
            //    {
            //        var sql = x
            //            .Select<AsycudaSalesAllocations, string>(a => SaveAllocationSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            //        SaveSql(sql);
            //    });
            var sql = allocations.Select(a => SaveAllocationSql(a)).DefaultIfEmpty("").Aggregate((o, n) => $"{o}\r\n{n}");
            SaveSql(sql);


        }

        private  void SaveSql(string sql)
        {
            try
            {
                if (string.IsNullOrEmpty(sql)) return;
                int retryCount = 0;

                int maxRetries = 3;
                while (retryCount < maxRetries)
                {
                    try
                    {
                       new AllocationDSContext {StartTracking = false}.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                       break;
                    }
                    catch (SqlException e) // This example is for SQL Server, change the exception type/logic if you're using another DBMS
                    {
                        if (e.Number == 1205)  // SQL Server error code for deadlock
                        {
                            retryCount++;
                        }
                        else
                        {
                            throw;  // Not a deadlock so throw the exception
                        }
                        // Add some code to do whatever you want with the exception once you've exceeded the max. retries
                    }
                }
                
                
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
															SET                QtyAllocated = {saleitm.QtyAllocated}{(saleitm.EffectiveDate == null? "" : $",EffectiveDate = \'{saleitm.EffectiveDate}\'")}
															where	EntryDataDetailsId = {saleitm.EntryDataDetailsId} ;";

        }

        private static string SaveAsycudaItemSql(xcuda_Item cAsycudaItm)
        {
            return $@"UPDATE       xcuda_Item
															SET                DPQtyAllocated = {
                                                                cAsycudaItm.DPQtyAllocated
                                                            }, DFQtyAllocated = {cAsycudaItm.DFQtyAllocated}
															where	item_id = {cAsycudaItm.Item_Id};";
          
        }


       
    }
}