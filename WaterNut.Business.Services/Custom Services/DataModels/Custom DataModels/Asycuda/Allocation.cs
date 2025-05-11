using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdjustmentQS.Business.Services;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;
using Asycuda421;
using Core.Common.Data;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using sun.invoke.util;
using TrackableEntities;
using TrackableEntities.EF6;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using EntryDataDetails = AllocationDS.Business.Entities.EntryDataDetails;
using InventoryItemAlias = AllocationDS.Business.Entities.InventoryItemAlias;
using Sales = AllocationDS.Business.Entities.Sales;
using SubItems = AllocationDS.Business.Entities.SubItems;
//using IEnumeratorToList =  Core.Common.Extensions.IEnumeratorToList;
using Core.Common.Extensions;
using static sun.awt.SunHints;
using System.Runtime.Remoting.Contexts;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingPreAllocations;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingGetUOAllocations;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.SavingAllocations;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingUnallocatedXSales;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets;

namespace WaterNut.DataSpace
{
	public partial class AllocationsBaseModel
	{
        public static bool isDBMem = false;
        private static readonly AllocationsBaseModel instance;
        internal static readonly object Identity = new object();
        // Semaphore for asynchronous locking
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        static AllocationsBaseModel()
  {
   instance = new AllocationsBaseModel();
  }

  private DataCache<InventoryItemAlias> _inventoryAliasCache;
        

        public static AllocationsBaseModel Instance
  {
   get { return instance; }
  }

  public DataCache<InventoryItemAlias> InventoryAliasCache
  {
            // NOTE: Calling .Result here can block. Consider initializing the cache asynchronously elsewhere.
   get => _inventoryAliasCache ?? GetDataCache().Result;
            set => _inventoryAliasCache = value;
        }


       

        private async Task<DataCache<InventoryItemAlias>> GetDataCache()
        {
            // Use semaphore instead of lock for async operations
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                 if (_inventoryAliasCache == null) // Double-check locking pattern
                 {
                    var aliases = await AllocationDS.DataModels.BaseDataModel.Instance.SearchInventoryItemAlias(
                                        new List<string> { "All" }, new List<string> { "InventoryItem.LumpedItem" })
                                        .ConfigureAwait(false);
                    _inventoryAliasCache = new DataCache<InventoryItemAlias>(aliases);
                 }
            }
            finally
            {
                _semaphore.Release();
            }
            return _inventoryAliasCache;
        }

        public class ItemSales
		{
			public (DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId) Key { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
		}

		internal class InventoryItemSales
		{
			public InventoryItem InventoryItem { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
		}

        public class ItemSet
		{
			public (DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId) Key { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
			public List<xcuda_Item> EntriesList { get; set; }


		}


        //private static List<XSales_UnAllocated> GetUnAllocatedxSales()
        //{
        //    List<XSales_UnAllocated> existingAllocations;
        //    using (var ctx = new AllocationDSContext() { StartTracking = true })
        //    {
        //        existingAllocations = ctx.XSales_UnAllocated.Where(x =>
        //                x.ApplicationSettingsId ==
        //                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
        //            // .Where(x => x.pItemId == 31702 )//&& x.xItemId == 30700
        //            // .Where(x => x.EntryDataDetailsId == 1852995)//&& x.xItemId == 30700
        //            .ToList();
        //    }

        //    return existingAllocations;
        //}


        //private static void SaveAllocations(
        //    IEnumerable<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails>
        //            EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>
        //        groupAllocations)
        //{

        //    groupAllocations
        //        .AsParallel()
        //        .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
        //                                                 BaseDataModel.Instance.ResourcePercentage)
        //        .ForAll(set =>
        //        {
        //            var tasks = new List<Task>();
        //            tasks.Add(Task.Run(() =>
        //            {
        //                SaveData(set.Value.EntryDataDetails ?? new List<EntryDataDetails>());
        //            }));
        //            tasks.Add(Task.Run(() =>
        //            {
        //                SaveData(set.Value.XcudaItems ?? new List<xcuda_Item>());
        //            }));
        //            tasks.Add(Task.Run(() =>
        //            {
        //                SaveData(set.Value.dbAllocations ?? new List<AsycudaSalesAllocations>());
        //            }));
        //            Task.WaitAll(tasks.ToArray());
        //        });


        //}

        //private static void SaveData(IEnumerable<ITrackable> set)
        //{
        //    var ctx = new AllocationDSContext()
        //        {StartTracking = false, Configuration = {AutoDetectChangesEnabled = false, ValidateOnSaveEnabled = false}};
        //    ctx.ApplyChanges(set);
        //    ctx.SaveChanges();
        //}


        public static void PrepareDataForAllocation(ApplicationSettings applicationSettings)
		{

			// update nonstock entrydetails status
			using (var ctx = new EntryDataDSContext())
            {
                ctx.Database.ExecuteSqlCommand(
                    $@"update xcuda_item
                        set xWarehouseError = null
                        where xWarehouseError is not null");

                ctx.Database.ExecuteSqlCommand($@"UPDATE EntryDataDetails
						SET         Status = N'Non Stock', DoNotAllocate = 1
						FROM    EntryData INNER JOIN
										 EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId INNER JOIN
										 [InventoryItems-NonStock] INNER JOIN
										 InventoryItems ON [InventoryItems-NonStock].InventoryItemId = InventoryItems.Id ON EntryDataDetails.ItemNumber = InventoryItems.ItemNumber AND 
										 EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId
						WHERE (EntryData.ApplicationSettingsId = {
						applicationSettings.ApplicationSettingsId
					}) AND (EntryDataDetails.Status IS NULL)");

				// Consider moving this this is shit code
				ctx.Database.ExecuteSqlCommand(@"UPDATE EntryDataDetails
													SET         TaxAmount = CASE WHEN dutyfreepaid = 'Duty Paid' THEN 1 ELSE 0 END
													--select EntryDataDetails.*, CASE WHEN dutyfreepaid = 'Duty Paid' THEN 1 ELSE 0 END as taxamount
													FROM    EntryDataDetails INNER JOIN
																	 EntryData_Adjustments ON EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id INNER JOIN
																	 AdjustmentComments ON EntryDataDetails.Comment = AdjustmentComments.Comments");

				ctx.Database.ExecuteSqlCommand(@"EXEC [dbo].[GetMappingFromInventory] @appsettingId
													 EXEC[dbo].[CreateInventoryAliasFromInventoryMapping]",
					new SqlParameter("@appsettingId", applicationSettings.ApplicationSettingsId));

				ctx.Database.ExecuteSqlCommand(@"WITH CTE AS(
													SELECT EntryDataDetails.EntryDataId, FileLineNumber,ItemNumber, Quantity, InvoiceQty, ReceivedQty,
													RN = ROW_NUMBER()OVER(PARTITION BY EntryDataDetails.EntryDataId, FileLineNumber, ItemNumber, Quantity, InvoiceQty, ReceivedQty  ORDER BY EntryDataDetailsId desc)
													FROM EntryDataDetails
														)
													DELETE FROM CTE WHERE RN > 1

													delete from entrydata where entrydata_id not in (select distinct Entrydata_id from entrydatadetails)");
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                       ctx.Database.ExecuteSqlCommand("EXEc [dbo].[FixItemIssues]");
                    }
                    catch (SqlException)
                    {
                        // Handle the exception here
                    }
                   
                    
                }
               
    
            }
		}


        //public List<((string, int InventoryItemId) Key, List<(string, int AliasItemId)> Alias)> CreateItemSets(int applicationSettingsId, string lst)
        //{
        //    using (var ctx = new AllocationDSContext() { StartTracking = false })
        //    {
        //        return ctx.InventoryItems
        //            .AsNoTracking()
        //            .Include(x => x.InventoryItemAliasEx)
        //            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
        //            .Where(x => string.IsNullOrEmpty(x.ItemNumber))
        //            .Where(x =>lst == null ||  lst.ToUpper().Trim().Contains(x.ItemNumber.ToUpper().Trim()))
        //            .ToList()
        //            .Select(x => (Key: (x.ItemNumber.ToUpper().Trim(), x.InventoryItemId) ,Alias :x.InventoryItemAliasEx.Select(a => (a.AliasName.ToUpper().Trim(), a.AliasItemId)).ToList()))
        //            .ToList();

        //    }
        //}


        


        private static void AddAlias(KeyValuePair<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), AllocationsBaseModel.ItemSet> itemSet, ConcurrentDictionary<string, List<xcuda_Item>> itemsCache, List<string> alias)
        {
            
        }


        
        private static void SaveAllocation(xcuda_Item cAsycudaItm, EntryDataDetails saleitm, SubItems subitm,
            AsycudaSalesAllocations ssa)
        {
            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                var sql = $@" INSERT INTO AsycudaSalesAllocations
														 (EntryDataDetailsId, PreviousItem_Id, QtyAllocated, EANumber, SANumber, Status)
														VALUES        ({ssa.EntryDataDetailsId},{ssa.PreviousItem_Id},{
                                                            ssa.QtyAllocated
                                                        },0,0,{
                                                            (ssa.Status == null ? "NULL" : $"'{ssa.Status}'")
                                                        })                                                      
														 
														
														 UPDATE       xcuda_Item
															SET                DPQtyAllocated = {
                                                                cAsycudaItm.DPQtyAllocated
                                                            }, DFQtyAllocated = {cAsycudaItm.DFQtyAllocated}
															where	item_id = {cAsycudaItm.Item_Id}
														
														UPDATE       EntryDataDetails
															SET                QtyAllocated = {saleitm.QtyAllocated}
															where	EntryDataDetailsId = {saleitm.EntryDataDetailsId} "
                          + (subitm != null
                              ? $@"UPDATE       SubItems
															SET                QtyAllocated = {subitm.QtyAllocated}
															where	SubItem_Id = {subitm.SubItem_Id}"
                              : "");
                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
            }
        }


        private async Task SaveAllocation(AsycudaSalesAllocations ssa)
		{
			using (var ctx = new AsycudaSalesAllocationsService())
			{
				await ctx.UpdateAsycudaSalesAllocations(ssa).ConfigureAwait(false);
			}
		}
    }

    public class PreAllocations
    {
        public PreAllocations(int entryDataDetailsId, int pItemId, int xItemId, double qtyAllocated, string dutyFreePaid)
        {
            EntryDataDetailsId = entryDataDetailsId;
            PItemId = pItemId;
            XItemId = xItemId;
            QtyAllocated = qtyAllocated;
            DutyFreePaid = dutyFreePaid;
        }

        public PreAllocations()
        {
            
        }

        public int EntryDataDetailsId { get; set; }
        public int PItemId { get; set; }
        public int XItemId { get; set; }
        public double QtyAllocated { get; set; }
        public string DutyFreePaid { get; set; }
        public int InventoryItemId { get; set; }
        public long Id { get; set; }
        public double SalesQuantity { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
}
