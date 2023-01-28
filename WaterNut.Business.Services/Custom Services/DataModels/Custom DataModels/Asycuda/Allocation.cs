using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace WaterNut.DataSpace
{
	public class AllocationsBaseModel
	{

		private static readonly AllocationsBaseModel instance;
		static AllocationsBaseModel()
		{
			instance = new AllocationsBaseModel();
		}

		private DataCache<InventoryItemAlias> _inventoryAliasCache;
		private static ConcurrentDictionary<int, xcuda_Item> _asycudaItems;

		public static AllocationsBaseModel Instance
		{
			get { return instance; }
		}

		public DataCache<InventoryItemAlias> InventoryAliasCache
		{
			get
			{
				return _inventoryAliasCache ??
					   (_inventoryAliasCache =
						   new DataCache<InventoryItemAlias>(
							   AllocationDS.DataModels.BaseDataModel.Instance.SearchInventoryItemAlias(
								   new List<string> { "All" }, new List<string> { "InventoryItem.LumpedItem" }).Result));
			}
			set { _inventoryAliasCache = value; }
		}

		internal class ItemSales
		{
			public (DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId) Key { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
		}

		internal class InventoryItemSales
		{
			public InventoryItem InventoryItem { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
		}

		internal class ItemEntries
		{
			public string Key { get; set; }
			public List<xcuda_Item> EntriesList { get; set; }
		}

		internal class ItemSet
		{
			public (DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId) Key { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
			public List<xcuda_Item> EntriesList { get; set; }


		}


		public async Task AllocateSales(ApplicationSettings applicationSettings, bool allocateToLastAdjustment)
		{
            try
			{
                SQLBlackBox.RunSqlBlackBox();

				PrepareDataForAllocation(applicationSettings);
                
                ReallocateExistingxSales();

                ReallocateExistingEx9(); // to prevent changing allocations when im7 info changes

               
				StatusModel.Timer("Auto Match Adjustments");
				
				await new AdjustmentShortService().AutoMatch(applicationSettings.ApplicationSettingsId, true).ConfigureAwait(false);
					// if(forceDiscrepancyExecution) await ctx.ProcessDISErrorsForAllocation(applicationSettings.ApplicationSettingsId).ConfigureAwait(false); // automatch doing everything now
				


				await AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(applicationSettings.ApplicationSettingsId, allocateToLastAdjustment,
                   //"317229"
                      null
                    ).ConfigureAwait(false);


				await MarkErrors(applicationSettings.ApplicationSettingsId).ConfigureAwait(false);

				StatusModel.StopStatusUpdate();
			}
			catch (Exception ex)
			{

				throw ex;
			}

		}

        private void ReallocateExistingxSales()
        {
			if (BaseDataModel.Instance.CurrentApplicationSettings.PreAllocateEx9s != true) return;
            
            List<XSales_UnAllocated> unAllocatedxSales;
            unAllocatedxSales = GetUnAllocatedxSales();

            var xSalesByItemId = unAllocatedxSales.Select(x => (Item: (x.ItemNumber, x.InventoryItemId), xSale:x))
                .GroupBy(x => x.Item)
                .Select(x => (Key: x.Key, Value: x.Select(i => i.xSale).ToList()))
                .ToList();

           var itemGroups = Utils.CreateItemSet(xSalesByItemId).Partition(100);

           
           itemGroups
               .AsParallel()
               .AsOrdered()
               .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage))
               .ForEach( x => AllocateXSales(x.SelectMany(g => g.Value.SelectMany(z => z.Value.Select(v => v).ToList()).ToList()).ToList()));

        }


        private static void AllocateXSales(List<XSales_UnAllocated> unAllocatedxSales)
        {
            using (var ctx = new AllocationDSContext() { StartTracking = true })
            {
                foreach (var allocation in unAllocatedxSales)
                {
                    var allo = new AsycudaSalesAllocations()
                    {
                        EntryDataDetailsId = allocation.EntryDataDetailsId,
                        PreviousItem_Id = allocation.pItemId,
                        xEntryItem_Id = allocation.xItemId,
                        QtyAllocated = allocation.SalesQuantity ?? 0,
                        TrackingState = TrackingState.Added
                    };

                    var entrydataDetails =
                        ctx.EntryDataDetails.First(x => x.EntryDataDetailsId == allocation.EntryDataDetailsId);
                    entrydataDetails.QtyAllocated += allocation.SalesQuantity ?? 0;

                    var pitem = ctx.xcuda_Item.First(x => x.Item_Id == allocation.pItemId);

                    if (allocation.DutyFreePaid == "Duty Free")
                    {
                        pitem.DFQtyAllocated += allocation.SalesQuantity ?? 0;
                    }
                    else
                    {
                        pitem.DPQtyAllocated += allocation.SalesQuantity ?? 0;
                    }


                    ctx.AsycudaSalesAllocations.Add(allo);
                }

                ctx.SaveChanges();
            }
        }

        private static List<XSales_UnAllocated> GetUnAllocatedxSales()
        {
            List<XSales_UnAllocated> existingAllocations;
            using (var ctx = new AllocationDSContext() { StartTracking = true })
            {
                existingAllocations = ctx.XSales_UnAllocated.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    // .Where(x => x.pItemId == 31702 )//&& x.xItemId == 30700
                    // .Where(x => x.EntryDataDetailsId == 1852995)//&& x.xItemId == 30700
                    .ToList();
            }

            return existingAllocations;
        }

        private void ReallocateExistingEx9()
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.PreAllocateEx9s != true) return;
            var existingAllocations = GetExistingEx9s()
                                                                //.Where(x => x.EntryDataDetailsId == 0)
                                                                ;

            var rawSet = existingAllocations.Select(x => (Item: (x.ItemNumber, x.InventoryItemId), xSale: (dynamic)x))
                .GroupBy(x => x.Item)
                .Select(x => (Key: x.Key, Value: x.Select(i => i.xSale).ToList()))
                .ToList();

            var itemGroups = Utils.CreateItemSet(rawSet);

            var res = AddExtraData(itemGroups).ToList().Partition(100).ToList();

           // Parallel.ForEach(res, new ParallelOptions() { MaxDegreeOfParallelism = Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage) }, x =>  AllocateExistingEx9s(x));
		   res
			   .AsParallel()
               //.AsOrdered()
               .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                        BaseDataModel.Instance.ResourcePercentage))
               .ForEach(x => AllocateExistingEx9s(x.ToList()));
			
        }

        private Dictionary<int, (List<ExistingAllocations> ExistingAllocations,
                                List<EntryDataDetails>EntryDataDetails,
                                List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)> AddExtraData(
            Dictionary<int, List<((string ItemNumber, int InventoryItemId) Key, List<object> Value)>> itemGroups)
        {


            var allEntryDataDetailsLst = AllEntryDataDetailsLst();
            var allXcudaItems = AllXcudaItems();


            //var res =
            //    new Dictionary<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails>
            //        EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>();


           var res = itemGroups
               .AsParallel()
               .AsOrdered()
               .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage))
               .Select(set => new {set.Key, value = GetData(set, allEntryDataDetailsLst, allXcudaItems)})
               .ToDictionary(x => x.Key, v => v.value);
           

            return res;

        }

        private static (List<ExistingAllocations> existingAllocations, List<EntryDataDetails> entryDataDetailsLst, List<xcuda_Item> xcudaItems, List<AsycudaSalesAllocations>) GetData(KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<object> Value)>> set,
            List<EntryDataDetails> allEntryDataDetailsLst, List<xcuda_Item> allXcudaItems)
        {
            var existingAllocations = set.Value
                .SelectMany(z => z.Value.Select(v => (ExistingAllocations) v).ToList()).ToList();
            var elst = existingAllocations.Select(z => z.EntryDataDetailsId).ToList();
            var entryDataDetailsLst =
                allEntryDataDetailsLst.Where(x => elst.Contains(x.EntryDataDetailsId)).ToList();
            var plst = existingAllocations.Select(z => z.pItemId).ToList();
            var xcudaItems = allXcudaItems.Where(x => plst.Contains(x.Item_Id)).ToList();

            var itm = (existingAllocations, entryDataDetailsLst, xcudaItems, new List<AsycudaSalesAllocations>());
            return itm;
        }

        private static List<xcuda_Item> AllXcudaItems() =>
            new AllocationDSContext()
            {
                StartTracking = false, Configuration = {AutoDetectChangesEnabled = false, ValidateOnSaveEnabled = false}
            }.xcuda_Item.AsNoTracking().ToList();

        private static List<EntryDataDetails> AllEntryDataDetailsLst() =>
            new AllocationDSContext()
            {
                StartTracking = false, Configuration = {AutoDetectChangesEnabled = false, ValidateOnSaveEnabled = false}
            }.EntryDataDetails.AsNoTracking().ToList();

        private static void AllocateExistingEx9s(
            List<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>> groupAllocations)
        {
            groupAllocations
				.AsParallel()
                //.AsOrdered()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
				.ForEach(AddAsycudaSalesAllocations);


            SaveAllocations(groupAllocations);
        }

        private static void SaveAllocations(
            IEnumerable<KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails>
                    EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)>>
                groupAllocations)
        {

            groupAllocations
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .ForEach(set =>
                {
                    var tasks = new List<Task>();
                    tasks.Add(Task.Run(()=>SaveData(set.Value.EntryDataDetails)));
					tasks.Add(Task.Run(() => SaveData(set.Value.XcudaItems)));
					tasks.Add(Task.Run(() => SaveData(set.Value.dbAllocations)));
                    Task.WaitAll(tasks.ToArray());
                });


        }

        private static void SaveData(IEnumerable<ITrackable> set)
        {
            var ctx = new AllocationDSContext()
                {StartTracking = false, Configuration = {AutoDetectChangesEnabled = false, ValidateOnSaveEnabled = false}};
            ctx.ApplyChanges(set);
            ctx.SaveChanges();
        }
    

        private static void AddAsycudaSalesAllocations(KeyValuePair<int, (List<ExistingAllocations> ExistingAllocations, List<EntryDataDetails> EntryDataDetails, List<xcuda_Item> XcudaItems, List<AsycudaSalesAllocations> dbAllocations)> set)
        {
            var newAllocations = new List<AsycudaSalesAllocations>();
            foreach (var allocation in set.Value.ExistingAllocations)
            {
                var allo = new AsycudaSalesAllocations()
                {
                    EntryDataDetailsId = allocation.EntryDataDetailsId,
                    PreviousItem_Id = allocation.pItemId,
                    xEntryItem_Id = allocation.xItemId,
                    QtyAllocated = allocation.xQuantity ?? 0,
                    TrackingState = TrackingState.Added
                };
                /// ----- took this out because i changed query not to include sales
                //if(allocation.EntryDataDetailsId == 0 && ctx.EntryDataDetails.Any(x => x.EntryData.EntryType == "Sales" || x.EntryData.EntryType == "ADJ" 
                //                                                                                                   &&  (x.EntryData.EntryDataDate >=  BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate && x.EntryData.EntryDataDate <= allocation.EntryDataDate)
                //                                                                                                   && (allocation.ItemNumber.Contains(x.ItemNumber) || x.ItemNumber.Contains(allocation.ItemNumber))) ) continue;


                set.Value.EntryDataDetails.First(x => x.EntryDataDetailsId == allocation.EntryDataDetailsId)
                    .QtyAllocated += allocation.xQuantity ?? 0;


                var pItem = set.Value.XcudaItems.First(x => x.Item_Id == allocation.pItemId);

                if (allocation.DutyFreePaid == "Duty Free")
                {
                    pItem.DFQtyAllocated += allocation.xQuantity ?? 0;
                }
                else
                {
                    pItem.DPQtyAllocated += allocation.xQuantity ?? 0;
                }

                newAllocations.Add(allo);
            }

            set.Value.dbAllocations.AddRange(newAllocations);
        }

        private static List<ExistingAllocations> GetExistingEx9s()
        {
            using (var ctx = new AllocationDSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 0;
               return  ctx.ExistingAllocations.AsNoTracking()
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    // .Where(x => x.EntryDataDetailsId == 1852995)
                    .ToList();
            }
        }


        public static void PrepareDataForAllocation(ApplicationSettings applicationSettings)
		{
			// update nonstock entrydetails status
			using (var ctx = new EntryDataDSContext())
			{
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

				//ctx.Database.ExecuteSqlCommand($@"WITH CTE AS(
				//SELECT EntryDataDetails.EntryDataId, FileLineNumber,ItemNumber, Quantity, InvoiceQty, ReceivedQty,
				//RN = ROW_NUMBER()OVER(PARTITION BY EntryDataDetails.EntryDataId, FileLineNumber, ItemNumber, Quantity, InvoiceQty, ReceivedQty  ORDER BY EntryDataDetails.EntryDataId, FileLineNumber, ItemNumber, Quantity, InvoiceQty, ReceivedQty)
				//FROM EntryDataDetails
				//	)
				//DELETE FROM CTE WHERE RN > 1

				//delete from entrydata where entrydata_id not in (select distinct Entrydata_id from entrydatadetails)");
			}
		}

		public async Task MarkErrors(int applicationSettingsId, string shortlst=null)
		{
			// MarkNoAsycudaEntry();

			MarkOverAllocatedEntries(applicationSettingsId);

			MarkUnderAllocatedEntries(applicationSettingsId);


		}

		public async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
			int applicationSettingsId, bool allocateToLastAdjustment, string lst)
		{
			var itemSets = await MatchSalestoAsycudaEntriesOnItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
			StatusModel.StopStatusUpdate();

            var rawSet = itemSets.Select(x => (Item: (x.Key.ItemNumber, x.Key.InventoryItemId), xSale: (dynamic)x))
                .GroupBy(x => x.Item)
                .Select(x => (Key: x.Key, Value: x.Select(i => i.xSale).ToList()))
                .ToList();

            

			var itemGroups = Utils.CreateItemSet(rawSet);


            var ggitms = GroupOfGroups(itemGroups);

         
			Parallel.ForEach(ggitms, new ParallelOptions() { MaxDegreeOfParallelism = Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage) }, x =>
            {
                AllocateSales(allocateToLastAdjustment, x);
                //AllocateSales(allocateToLastAdjustment, x.Value.group.Value.SelectMany(z => z.Value.Select(v => (ItemSet)v.Value).ToList()).ToList());
            });


			
        }

        private void AllocateSales(bool allocateToLastAdjustment, KeyValuePair<int, (List<int> Lst, KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<object> Value)>> group)> groupItemSets)
        {
           var res =  groupItemSets.Value.group.Value.ToList()
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .SelectMany(x =>  AllocateSales(allocateToLastAdjustment, x.Value.Select(v => (ItemSet)v).ToList()))
                .ToList();

            res
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .ForEach(x =>
                {
                    SaveData(x.Sales);
                    SaveData(x.asycudaItems);
                });




        }

        private Dictionary<int, (List<int> Lst, KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<dynamic> Value)>> group)> GroupOfGroups(
                Dictionary<int, List<((string ItemNumber, int InventoryItemId) Key, List<dynamic> Value)>> itemGroups)
        {

            var workingitemGroups = itemGroups.Select(parentgroup => (Lst: parentgroup.Value.SelectMany(x =>
                    x.Value.SelectMany(z => ((ItemSet)z.Value).EntriesList.Select(a => a.Item_Id).ToList()).Distinct()
                        .ToList()).ToList(),
                group: parentgroup)).ToList();
            //.Where(x =>
            //                x.group.Value.Any(z => z.Value.Any(q => ((ItemSet)q.Value).EntriesList.Any(a => a.Item_Id == 32207))))
            //            .ToList();

            var res =
                new Dictionary<int, (List<int> Lst, KeyValuePair<int,
                    List<((string ItemNumber, int InventoryItemId) Key, List<dynamic> Value)>> group)>();

            var set = 0;
            while (workingitemGroups.Any())
            {
                var parentgroup = workingitemGroups.First();


                workingitemGroups.Remove(parentgroup);
                var childlst = workingitemGroups.Where(x => x.Lst.Any(z => parentgroup.Lst.Contains(z))).ToList();
				var clst = childlst.SelectMany(c => c.Lst).ToList();

				var resLst = res.Where(x => x.Value.Lst.Any(z => parentgroup.Lst.Contains(z)) || x.Value.Lst.Any(z => clst.Contains(z))).ToList();

                if (resLst.Any())
                {
                    resLst.First().Value.group.Value.AddRange(childlst.SelectMany(x => x.group.Value));
                    resLst.First().Value.Lst.AddRange(childlst.SelectMany(x => x.Lst));
					resLst.First().Value.group.Value.AddRange(parentgroup.group.Value);
					resLst.First().Value.Lst.AddRange(parentgroup.Lst);

				}
				else
                {
                    //var childlst =
                    //    new List<(IEnumerable<int> Lst,
                    //        KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<dynamic> Value)>>
                    //        group)>();
                    //foreach (var itmGroup in workingitemGroups)
                    //{
                    //    if(itmGroup.)
                    //}
                    //var cList = childlst.Where(x =>
                    //        x.group.Value.Any(z =>
                    //            z.Value.Any(q => ((ItemSet)q.Value).EntriesList.Any(a => a.Item_Id == 32207))))
                    //    .ToList();
                    //if (cList.Any()) Debugger.Break();

                    parentgroup.group.Value.AddRange(childlst.SelectMany(x => x.group.Value));
					parentgroup.Lst.AddRange(childlst.SelectMany(x => x.Lst));
				}

                workingitemGroups = workingitemGroups.ExceptBy(childlst, x => x.group.Key).ToList();


                set++;
                res.Add(parentgroup.group.Key, parentgroup);


                //if (parentgroup.Lst.Contains(32207))
                //{
                //    Debugger.Break();
                //    var tList = workingitemGroups.Where(x =>
                //            x.group.Value.Any(z =>
                //                z.Value.Any(q => ((ItemSet)q.Value).EntriesList.Any(a => a.Item_Id == 32207))))
                //        .ToList();

                //}

            }

            return res;
        }

        private ConcurrentDictionary<int, xcuda_Item> currentItems = new ConcurrentDictionary<int, xcuda_Item>();


        private List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)> AllocateSales(bool allocateToLastAdjustment, List<ItemSet> itemSets)
        {
            StatusModel.StartStatusUpdate("Allocating Item Sales", itemSets.Count());
            var t = 0;
            var exceptions = new ConcurrentQueue<Exception>();
            var itemSetsValues = itemSets.ToList();

            var count = itemSetsValues.Count();
            var res = new List<(List<EntryDataDetails> Sales, List<xcuda_Item> asycudaItems)>();
            foreach (var itm in itemSetsValues.OrderBy(x => x.Key.EntryDataDate))
                try
                {
                    t += 1;
                    // Debug.WriteLine($"Processing {itm.Key} - {t} with {itm.SalesList.Count} Sales: {0} of {itm.SalesList.Count}");
                    //StatusModel.Refresh();
                    var sales = SortEntryDataDetailsList(itm);
                    var asycudaItems = SortAsycudaItems(itm);

                    AllocateSalestoAsycudaByKey(sales, asycudaItems, t, count, allocateToLastAdjustment).Wait();
                    res.Add((sales, asycudaItems));

                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not Allocate - '{itm.Key}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
                }



            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return res;
        }

        private static List<xcuda_Item> SortAsycudaItems(ItemSet itm)
        {
            var asycudaItems = itm.EntriesList.OrderBy(x => x.AsycudaDocument.AssessmentDate)
                .ThenBy(x => x.IsAssessed == null).ThenBy(x => x.AsycudaDocument.RegistrationDate)
                .ThenBy(x => Convert.ToInt32(x.AsycudaDocument.CNumber))
                .ThenByDescending(x =>
                    x.EntryPreviousItems.Select(z => z.xcuda_PreviousItem.Suplementary_Quantity)
                        .DefaultIfEmpty(0).Sum()) //NUO/44545 2 items with same date choose pIed one first
                .ThenBy(x => x.AsycudaDocument.ReferenceNumber)
                .DistinctBy(x => x.Item_Id)
                .ToList();
            return asycudaItems;
        }

        private static List<EntryDataDetails> SortEntryDataDetailsList(ItemSet itm)
        {
            return itm.SalesList
                .OrderBy(x => x.Sales.EntryDataDate)
                .ThenBy(x => x.EntryDataId)
                .ThenBy(x => x.LineNumber ?? x.EntryDataDetailsId)
                .ThenByDescending(x => x.Quantity) /**/.ToList();
        }


        private void MarkOverAllocatedEntries(int applicationSettingsId)
        {
            List<xcuda_Item> IMAsycudaEntries; //"EX"

            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;

                IMAsycudaEntries = ctx.xcuda_Item.Include(x => x.AsycudaDocument)
                    .Include(x => x.xcuda_Tarification.xcuda_HScode)
                    .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                    .Include(x => x.SubItems)
                    .Include(x => x.xcuda_Goods_description)
                    .Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => (x.DFQtyAllocated + x.DPQtyAllocated) > x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity)
                    .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                (x.AsycudaDocument.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Import
                                 || x.AsycudaDocument.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Warehouse)
                                //&& x.AsycudaDocument.Customs_Procedure.Sales == true 
                                && x.AsycudaDocument.DoNotAllocate != true)
                    .Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                    .AsNoTracking()
                    .ToList();


            }



            if (IMAsycudaEntries == null || !IMAsycudaEntries.Any()) return;
            var alst = IMAsycudaEntries.Where(x => x != null
                                                   && (x.DFQtyAllocated + x.DPQtyAllocated) > Convert.ToDouble(x.ItemQuantity)).ToList();

            //var test = IMAsycudaEntries.Where(x => x.Item_Id == 27018).ToList();


            if (alst.Any())
                Parallel.ForEach(alst
                    ,
                    new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>//Environment.ProcessorCount * 1
                    {
                        using (var ctx = new AllocationDSContext { StartTracking = false })
                        {
                            var sql = "";

                            if (ctx.AsycudaSalesAllocations == null) return;

                            var lst =
                                ctx.AsycudaSalesAllocations
                                    .Include(x => x.EntryDataDetails.EntryData)
                                    .Include(x => x.EntryDataDetails.EntryDataDetailsEx)
                                    .Include(x => x.PreviousDocumentItem.xcuda_Tarification.xcuda_HScode)
                           
                                    .Where(x => x != null && x.PreviousItem_Id == i.Item_Id)
                                    .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                                    .OrderByDescending(x => x.AllocationId)
                                    .DistinctBy(x => x.AllocationId)
                                    .ToList();

                            foreach (var allo in lst)
                            {
                                var tot = i.QtyAllocated - i.ItemQuantity;
                                var r = tot > allo.QtyAllocated ? allo.QtyAllocated : tot;
                                if (i.QtyAllocated > i.ItemQuantity)
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
                                        i.DFQtyAllocated -= r;

                                        /////// is the same thing

                                        sql += $@" UPDATE       xcuda_Item
															SET                DFQtyAllocated = (DFQtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                                    }
                                    else
                                    {
                                        allo.PreviousDocumentItem.DPQtyAllocated -= r;
                                        i.DPQtyAllocated -= r;



                                        sql += $@" UPDATE       xcuda_Item
															SET                DPQtyAllocated = (DPQtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                                    }
                                    var existingStock = CheckExistingStock(allo.PreviousDocumentItem.ItemNumber, allo.EntryDataDetails.EntryData.EntryDataDate);
                                    if (allo.QtyAllocated == 0)
                                    {
                                        allo.QtyAllocated = r; //add back so wont disturb calculations
                                        allo.Status = $"Over Allocated Entry by {r}{existingStock}";

                                        sql += $@"  Update AsycudaSalesAllocations
														Set Status = '{allo.Status}', QtyAllocated = {r }
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

                            if (!string.IsNullOrEmpty(sql))
                                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);

                        }
                    });
            using (var ctx = new AllocationDSContext())
            {
                var sql = @" DELETE FROM AsycudaSalesAllocations
								WHERE(Status IS NULL) AND(QtyAllocated = 0)";

                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
            }
        }


		private void MarkUnderAllocatedEntries(int applicationSettingsId)
        {
            List<xcuda_Item> IMAsycudaEntries; //"EX"

            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;
                IMAsycudaEntries = ctx.xcuda_Item.Include(x => x.AsycudaDocument)
                    .Include(x => x.xcuda_Tarification.xcuda_HScode)
                    .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                    .Include(x => x.SubItems)
                    .Include(x => x.xcuda_Goods_description)
                    .Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => (x.DFQtyAllocated + x.DPQtyAllocated) < 0)
                    .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                (x.AsycudaDocument.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Import
                                 || x.AsycudaDocument.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Warehouse)
                                && x.AsycudaDocument.Customs_Procedure.Sales == true
                                && x.AsycudaDocument.DoNotAllocate != true)
                    .Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                    .AsNoTracking()
                    .ToList();


            }



            if (IMAsycudaEntries == null || !IMAsycudaEntries.Any()) return;
            var alst = IMAsycudaEntries.ToList();
            if (alst.Any())
                Parallel.ForEach(alst.Where(x => x != null
                                                 && ((x.DFQtyAllocated + x.DPQtyAllocated) < 0))
                    ,
                    new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>//Environment.ProcessorCount*
                    {
                        using (var ctx = new AllocationDSContext { StartTracking = false })
                        {
                            var sql = "";

                            if (ctx.AsycudaSalesAllocations == null) return;

                            var lst =
                                ctx.AsycudaSalesAllocations
                                    .Include(x => x.EntryDataDetails)
                                    .Include(x => x.EntryDataDetails.EntryDataDetailsEx)
                                    .Include(x => x.PreviousDocumentItem)
                                    .Where(x => x.EntryDataDetails.IsReconciled != true)
                                    .Where(x => x != null && x.PreviousItem_Id == i.Item_Id)
                                    .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                                    .OrderBy(x => x.AllocationId)
                                    .DistinctBy(x => x.AllocationId)
                                    .ToList();
                            if (lst.Sum(x => x.QtyAllocated) < 0)
                                foreach (var allo in lst)
                                {
                                    var tot = i.QtyAllocated * -1;
                                    var r = tot > (allo.QtyAllocated * -1) ? allo.QtyAllocated * -1 : tot;
                                    if (i.QtyAllocated < 0)
                                    {


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
                                            i.DFQtyAllocated += r;

                                            /////// is the same thing

                                            sql += $@" UPDATE       xcuda_Item
															SET                DFQtyAllocated = (DFQtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")})
															where	item_id = {allo.PreviousDocumentItem.Item_Id}";
                                        }
                                        else
                                        {
                                            allo.PreviousDocumentItem.DPQtyAllocated += r;
                                            i.DPQtyAllocated += r;



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

                                        }
                                        else
                                        {

                                            sql += $@" INSERT INTO AsycudaSalesAllocations
														 (EntryDataDetailsId, PreviousItem_Id, QtyAllocated,Status, EANumber, SANumber)
														VALUES        ({allo.EntryDataDetailsId},{allo.PreviousItem_Id},{r},'Under Allocated by {r}',0,0)";
                                            //ctx.ApplyChanges(nallo);
                                            break;
                                        }

                                    }




                                }

                            if (!string.IsNullOrEmpty(sql))
                                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);

                        }
                    });
            using (var ctx = new AllocationDSContext())
            {
                var sql = @" DELETE FROM AsycudaSalesAllocations
								WHERE(Status IS NULL) AND(QtyAllocated = 0)";

                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
            }
        }



		private async Task<ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), ItemSet>> MatchSalestoAsycudaEntriesOnItemNumber(
			int applicationSettingsId, string lst)
		{
			try
			{
                var asycudaEntriesTask = Task.Run(async () => await GetAsycudaEntriesWithItemNumber(applicationSettingsId, null).ConfigureAwait(false));
				var saleslstTask = Task.Run(async () => await GetSaleslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false));
                var adjlstTask = Task.Run(async () => await GetAdjustmentslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false));
                var dislstTask = Task.Run(async () => await GetDiscrepancieslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false));

                Task.WaitAll(asycudaEntriesTask, saleslstTask, adjlstTask, dislstTask);

				var asycudaEntries = asycudaEntriesTask.Result;
                var saleslst = saleslstTask.Result;
                var adjlst = adjlstTask.Result;
                var dislst = dislstTask.Result;

                saleslst.AddRange(adjlst);
                saleslst.AddRange(dislst);


                var sSales = saleslst.OrderBy(x => x.Key.EntryDataDate).ToList();
                var itmLst = CreateItemSetsWithItemNumbers(sSales, asycudaEntries);

				//var asycudaEntries = await GetAsycudaEntriesWithItemNumber(applicationSettingsId, null).ConfigureAwait(false);
				////var testr = asycudaEntries.Where(x => x.EntriesList.Any(z => z.ItemNumber == "BM/FGCM150-50")).ToList();

				//var saleslst = await GetSaleslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
				////var test = saleslst.Where(x => x.SalesList.Any(z => z.ItemNumber == "BM/FGCM150-50")).ToList();

				//var adjlst = await GetAdjustmentslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
				//saleslst.AddRange(adjlst);

				//var dislst = await GetDiscrepancieslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
				//saleslst.AddRange(dislst);

				//var itmLst = CreateItemSetsWithItemNumbers(saleslst.OrderBy(x => x.Key.EntryDataDate).ToList(), asycudaEntries);

				//var test = itmLst.Where(x => x.Key.EntryDataId == "Asycuda-C#33687-19").ToList();

				return itmLst;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}


        private static
            ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId),
                ItemSet> CreateItemSetsWithItemNumbers(
                IEnumerable<ItemSales> saleslst, IEnumerable<ItemEntries> asycudaEntries)
        {
            try
            {

                
                var itmLst = CreateItemSets(saleslst, asycudaEntries);

                var res = CreateItemSets(itmLst);

                AddLumpAndAliasData(asycudaEntries, res);

                return res;

                //return
                //    new ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber), ItemSet>(
                //        res.Where(x => x.Value.EntriesList.Any(z => z.AsycudaDocument.CNumber == "44887")).ToList());//res.Where(x => x.Value.EntriesList.Any(z => z.AsycudaDocument.CNumber == "44887")).ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static IEnumerable<ItemSet> CreateItemSets(IEnumerable<ItemSales> saleslst, IEnumerable<ItemEntries> asycudaEntries)
        {
            var itmLst = from s in saleslst
                join a in asycudaEntries on s.Key.ItemNumber equals a.Key into j
                from a in j.DefaultIfEmpty()
                select new ItemSet
                {
                    Key = s.Key,
                    SalesList = s.SalesList,
                    EntriesList = a?.EntriesList ?? new List<xcuda_Item>()
                };
            return itmLst;
        }

        private static void AddLumpAndAliasData(IEnumerable<ItemEntries> asycudaEntries, ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), ItemSet> res)
        {
            var flatAsycudaEntries = asycudaEntries.SelectMany(x => x.EntriesList).ToList();
            var lumpedItems = Instance.InventoryAliasCache.Data.Where(x => x.InventoryItem.LumpedItem != null)
                .ToList();

             res
                 .AsParallel()
                 .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                          BaseDataModel.Instance.ResourcePercentage))
                 .ForEach(r => 
            {
                var alias = Instance.InventoryAliasCache.Data.Where(x => x.ItemNumber.ToUpper().Trim() == r.Key.ItemNumber)
                    .Select(y => y.AliasName.ToUpper().Trim()).Distinct().ToList();


                var lumpedAlias = alias.Join(lumpedItems, x => x, y => y.AliasName,
                    (x, y) => y.ItemNumber).Distinct().ToList();

                if (alias.Any() || lumpedAlias.Any())
                {
                    //var te = asycudaEntries.Where(x => x.Key == "EVC/508").ToList();
                    var ae = asycudaEntries.Where(x => alias.Contains(x.Key) || lumpedAlias.Contains(x.Key))
                        .SelectMany(y => y.EntriesList).ToList();
                    if (ae.Any()) r.Value.EntriesList.AddRange(ae);

                    // Manual allocation
                    foreach (var itm in r.Value.SalesList.Where(x => x.ManualAllocations != null))
                    {
                        var ritm = flatAsycudaEntries.FirstOrDefault(x => x.Item_Id == itm.ManualAllocations.Item_Id);
                        if (ritm != null) r.Value.EntriesList.Add(ritm);
                    }

                    r.Value.EntriesList.AddRange(flatAsycudaEntries.Where(x =>
                        x.PreviousInvoiceItemNumber == r.Key.ItemNumber));
                }
            });
        }

        private static ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId), ItemSet> CreateItemSets(IEnumerable<ItemSet> itmLst)
        {
            var res =
                new ConcurrentDictionary<(DateTime EntryDataDate, string EntryDataId, string ItemNumber, int InventoryItemId),
                    ItemSet>();

            itmLst //.Where(x => x.Key.ItemNumber == "TRC/1206-QC").ToList()//.Where(x => x.Key.ItemNumber.StartsWith("T")).ToList()//.Where(x => x.SalesList.Any(z => z.EntryDataId == "61091010")).ToList()
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         BaseDataModel.Instance.ResourcePercentage))
                .ForEach(itm =>
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


        private static async Task<IEnumerable<ItemEntries>> GetAsycudaEntriesWithItemNumber(int applicationSettingsId, int? asycudaDocumentSetId)
		{
			StatusModel.Timer("Getting Data - Asycuda Entries...");
			//string itmnumber = "WMHP24-72";
			IEnumerable<ItemEntries> asycudaEntries = null;
			using (var ctx = new AllocationDSContext { StartTracking = false })
			{
				var lst = ctx.xcuda_Item.Include(x => x.AsycudaDocument.Customs_Procedure)
					.Include(x => x.xcuda_Tarification.xcuda_HScode)
					.Include("EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument")
					.Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
					.Include(x => x.SubItems)
					.Include("EntryPreviousItems.xcuda_PreviousItem")
					.Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
					.Where(x => asycudaDocumentSetId == null || x.AsycudaDocument.AsycudaDocumentSetId == asycudaDocumentSetId)
					.Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true)
								&& (/*x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Import
								 ||*/ x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse)
								&& (x.AsycudaDocument.Customs_Procedure.Sales == true || x.AsycudaDocument.Customs_Procedure.Stock == true) &&
								 (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
								 x.AsycudaDocument.DoNotAllocate != true)
					.Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
					.OrderBy(x => x.LineNumber)
					.ToList();

				// var res2 = lst.Where(x => x.ItemNumber == "PRM/84101");
				_asycudaItems = new ConcurrentDictionary<int, xcuda_Item>(lst.ToDictionary(x => x.Item_Id, x => x));
				asycudaEntries = from s in lst.Where(x => x.ItemNumber != null)
									 // .Where(x => x.ItemNumber == itmnumber)
									 //       .Where(x => x.AsycudaDocument.pCNumber != null).AsEnumerable()
								 group s by s.ItemNumber.ToUpper().Trim()
					into g
								 select
									 new ItemEntries
									 {
										 Key = g.Key.Trim(),
										 EntriesList =
											 g.AsEnumerable()
												 .OrderBy(
													 x =>
														 x.AsycudaDocument.EffectiveRegistrationDate == null
															 ? Convert.ToDateTime(x.AsycudaDocument.RegistrationDate)
															 : x.AsycudaDocument.EffectiveRegistrationDate)
												 .ToList()
									 };
			}

			//var res = asycudaEntries.Where(x => x.Key.Contains("8309"));
			return asycudaEntries;
		}



		private static async Task<List<ItemSales>> GetSaleslstWithItemNumber(int applicationSettingsId,
			string lst)
		{
           


				try
			{
				StatusModel.Timer("Getting Data - Sales Entries...");

				IEnumerable<ItemSales> saleslst = null;
				using (var ctx = new EntryDataDetailsService())
				{
					var salesData =

						await
							ctx.GetEntryDataDetailsByExpressionNav( //"ItemNumber == \"PNW/30-53700\" &&" +
									($"Sales.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && ") +

									"QtyAllocated != Quantity " +
									$"&& Sales.ApplicationSettingsId == {applicationSettingsId} " +
									//	$" && (\"{lst}\" == \"\" || \"{lst}\".Contains(ItemNumber)) " +
									//"&& Cost > 0 " + --------Cost don't matter in allocations because it comes from previous doc
									"&& DoNotAllocate != true", new Dictionary<string, string>
                                    {
										{"Sales", "INVNumber != null"}
									}, new List<string> { "Sales", "AsycudaSalesAllocations", "ManualAllocations" }, false)
								.ConfigureAwait(false);
					saleslst = salesData.Where(x => lst == null || lst.Contains(x.ItemNumber))
						.GroupBy(d => (d.Sales.EntryDataDate, d.EntryDataId, d.ItemNumber.ToUpper().Trim(), d.InventoryItemId))
						.Select(g => new ItemSales
						{
							Key = g.Key,
							SalesList = g.Where(xy => xy != null && xy.Sales != null)
								.OrderBy(x => x.Sales.EntryDataDate)
								.ThenBy(x => x.EntryDataId)
								.ToList()
						});
				}

				return saleslst.ToList();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}

		private static async Task<List<ItemSales>> GetAdjustmentslstWithItemNumber(int applicationSettingsId,
			string lst)
		{

            

			StatusModel.Timer("Getting Data - Adjustments Entries...");

			List<ItemSales> adjlst = null;
			using (var ctx = new EntryDataDetailsService())
			{
				var salesData =

				await
						ctx.GetEntryDataDetailsByExpressionNav(//"ItemNumber == \"318451\" &&" +
																($"Adjustments.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && ") +
															   "QtyAllocated != Quantity && " +
																$"Adjustments.ApplicationSettingsId == {applicationSettingsId} && " +
																$"(\"{lst}\" == \"\" || \"{lst}\".Contains(ItemNumber)) && " +
																"Adjustments.Type == \"ADJ\" && " + /// Only Adjustments not DIS that should have pCNumber to get matched
																"((PreviousInvoiceNumber == null) ||" +//pCNumber == null && 
																" (( PreviousInvoiceNumber != null) && QtyAllocated == 0))" + //trying to capture unallocated adjustments//pCNumber != null ||
																" && (ReceivedQty - InvoiceQty) <= 0 && (EffectiveDate != null || " + ($"EffectiveDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"") + ")" +
															   //"&& Cost > 0 " + --------Cost don't matter in allocations because it comes from previous doc
															   "&& DoNotAllocate != true", new Dictionary<string, string>
                                                                {
																   { "Adjustments", "EntryDataId != null" }
															   }, new List<string> { "Adjustments", "AsycudaSalesAllocations" }, false)
							.ConfigureAwait(false);
				adjlst = (salesData
						.Where(x => lst == null || lst.Contains(x.ItemNumber))
                        .Where(x => x.Adjustments.Type == "ADJ")
					  .GroupBy(d => (EntryDataDate: d.EffectiveDate ?? d.Adjustments.EntryDataDate, d.EntryDataId, ItemNumber: d.ItemNumber.ToUpper().Trim(), d.InventoryItemId))
					.Select(g => new ItemSales
					{
						Key = g.Key,
						SalesList = g.Where(xy => xy != null & xy.Adjustments != null)
							.OrderBy(x => x.EffectiveDate)
							.ThenBy(x => x.Adjustments.EntryDataDate)
							.ThenBy(x => x.EntryDataId)
							.ToList()
					})).ToList();
			}
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


		private static async Task<List<ItemSales>> GetDiscrepancieslstWithItemNumber(int applicationSettingsId,
			string lst)
		{
			try
			{


				StatusModel.Timer("Getting Data - Discrepancy Errors ...");

				List<ItemSales> adjlst = null;
				using (var ctx = new EntryDataDetailsService())
				{
					var salesData =

					await
							ctx.GetEntryDataDetailsByExpressionNav(//"ItemNumber == \"AAA/13576\" &&" +
																	($"Adjustments.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && ") +
																   "(QtyAllocated != Quantity || EntryDataId.Contains(\"Asycuda\")) && " +
																	$"Adjustments.ApplicationSettingsId == {applicationSettingsId} && " +
																	$"(\"{lst}\" == \"\" || \"{lst}\".Contains(ItemNumber)) && " +
																	"Adjustments.Type == \"DIS\" && " + /// Only Discrepancies with Errors
															   "(Comment.StartsWith(\"DISERROR:\") || EntryDataId.Contains(\"Asycuda\")) && " +  //"Asycuda is for Sales treated as discrepancies"
																	"( PreviousInvoiceNumber == null) ||" +//pCNumber == null &&
																	" ( PreviousInvoiceNumber != null &&  (QtyAllocated == 0 || EntryDataId.Contains(\"Asycuda\")))" + //trying to capture unallocated adjustments  // pCNumber != null ||
																	" && (ReceivedQty - InvoiceQty < 0) && (EffectiveDate != null || " + ($"EffectiveDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"") + ")" +
																   //"&& Cost > 0 " + --------Cost don't matter in allocations because it comes from previous doc
																   "&& DoNotAllocate != true", new Dictionary<string, string>
                                                                    {
																   { "Adjustments", "EntryDataId != null" }
																   }, new List<string> { "Adjustments", "AsycudaSalesAllocations" }, false)
								.ConfigureAwait(false);
					adjlst = (salesData
						.Where(x => lst == null || lst.Contains(x.ItemNumber))
                        .Where(x => x.Adjustments.Type == "DIS")
                        .Where(x => x.IsReconciled != true)
						.GroupBy(d => (EntryDataDate: d.EffectiveDate ?? d.Adjustments.EntryDataDate, d.EntryDataId, ItemNumber: d.ItemNumber.ToUpper().Trim(), d.InventoryItemId))
						.Select(g => new ItemSales
						{
							Key = g.Key,
							SalesList = g.Where(xy => xy != null & xy.Adjustments != null)
								.OrderBy(x => x.EffectiveDate)
								.ThenBy(x => x.Adjustments.EntryDataDate)
								.ThenBy(x => x.EntryDataId)
								.ToList()
						})).ToList();
				}
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
					x.Quantity = (double)(x.InvoiceQty - x.ReceivedQty);// switched it so its positive
				});
				return adjlst;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}


		private async Task AllocateSalestoAsycudaByKey(List<EntryDataDetails> saleslst, List<xcuda_Item> asycudaEntries,
			double currentSetNo, int setNo, bool allocateToLastAdjustment)
		{
			try
			{
				if (allocateToLastAdjustment && saleslst.All(x => x.Adjustments == null)) return;

				if (asycudaEntries == null || !asycudaEntries.Any())
				{
					foreach (var item in saleslst)
                        if (item.AsycudaSalesAllocations.FirstOrDefault(x => x.Status == "No Asycuda Entries Found") == null)
                            await AddExceptionAllocation(item, "No Asycuda Entries Found").ConfigureAwait(false);
                    return;
				}

				var startAsycudaItemIndex = 0;
				var CurrentSalesItemIndex = 0;
				var cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, startAsycudaItemIndex);
				var saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);


				while (cAsycudaItm.QtyAllocated == Convert.ToDouble(cAsycudaItm.ItemQuantity))
				{
					if (startAsycudaItemIndex + 1 < asycudaEntries.Count())
					{
						startAsycudaItemIndex += 1;
						cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, startAsycudaItemIndex);
					}
					else
                        break;
				}


				for (var s = CurrentSalesItemIndex; s < saleslst.Count(); s++)
				{
					var CurrentAsycudaItemIndex = startAsycudaItemIndex;// foreach sale start at beginning to search for possible qty to allocate



					if (CurrentSalesItemIndex != s)
					{
						if (saleitm.AsycudaSalesAllocations.Count == 0) Debugger.Break();
						CurrentSalesItemIndex = s;
						saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);
					}


					// StatusModel.Refresh();

					var saleitmQtyToallocate = saleitm.Quantity - saleitm.QtyAllocated;
					if (saleitmQtyToallocate > 0 && CurrentAsycudaItemIndex == asycudaEntries.Count())
					{
						// over allocate to handle out of stock in case returns deal with it
						await AllocateSaleItem(cAsycudaItm, saleitm, saleitmQtyToallocate, null)
							.ConfigureAwait(false);
						continue;
					}

					if (cAsycudaItm.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse &&
						(cAsycudaItm.AsycudaDocument.AssessmentDate > saleitm.Sales.EntryDataDate))
					{
						if (CurrentAsycudaItemIndex == 0)
                        {
                            await AddExceptionAllocation(saleitm, cAsycudaItm, "Early Sales" ).ConfigureAwait(false);
							continue;
						}
					}

					for (var i = CurrentAsycudaItemIndex; i < asycudaEntries.Count(); i++)
					{
						// reset in event earlier dat
						if (saleitmQtyToallocate == 0) break;
						if (CurrentAsycudaItemIndex != i || GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex).Item_Id != cAsycudaItm.Item_Id)
						{
							if (i < 0) i = 0;
							CurrentAsycudaItemIndex = i;
							cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex);

						}
						Debug.WriteLine($"Processing {saleitm.ItemNumber} - {currentSetNo} of {setNo} with {saleslst.Count} Sales: {s} of {saleslst.Count} : {CurrentAsycudaItemIndex} of {asycudaEntries.Count}");


						var asycudaItmQtyToAllocate = GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out var subitm);


						if (asycudaItmQtyToAllocate == 0 && saleitmQtyToallocate > 0)
						{
							CurrentAsycudaItemIndex += 1;
							continue;
						}

						if (cAsycudaItm.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse && (cAsycudaItm.AsycudaDocument.AssessmentDate > saleitm.Sales.EntryDataDate))
						{
                           await AddExceptionAllocation(saleitm, cAsycudaItm , "Early Sales").ConfigureAwait(false);
							break;
                        }

						if (saleitmQtyToallocate < 0 && cAsycudaItm.AsycudaSalesAllocations.Where(x => x.DutyFreePaid == saleitm.DutyFreePaid).Sum(x => x.QtyAllocated) == 0)
						{
							var previousI = GetPreviousAllocatedAsycudaItem(asycudaEntries, saleitm, i).Result;
							if (previousI != i && previousI != i - 1)
							{
								i = previousI;
								continue;
							}
						}
                        
						if (asycudaItmQtyToAllocate < 0 &&
							(CurrentAsycudaItemIndex != asycudaEntries.Count - 1 && asycudaEntries[i + 1].AsycudaDocument.AssessmentDate <= saleitm.Sales.EntryDataDate))
						{
							if (saleitmQtyToallocate > 0) continue;
						}

						if (cAsycudaItm.QtyAllocated == 0 && saleitmQtyToallocate < 0 && CurrentSalesItemIndex > 0)
						{
							if (CurrentAsycudaItemIndex == 0)
							{
								await AddExceptionAllocation(saleitm, "Returned More than Sold").ConfigureAwait(false);
								break;
							}
							i -= 2;

						}
						else
						{
                            if ((asycudaItmQtyToAllocate > 0 && asycudaItmQtyToAllocate >= saleitmQtyToallocate) ||
								CurrentAsycudaItemIndex == asycudaEntries.Count - 1)
							{
                                var ramt = await AllocateSaleItem(cAsycudaItm, saleitm, saleitmQtyToallocate, subitm)
									.ConfigureAwait(false);
								saleitmQtyToallocate = ramt;

								if (GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out subitm) == 0 && ramt != 0)
								{
									CurrentAsycudaItemIndex += 1;
									continue;
								}

								if (ramt == 0) break;
								if (ramt < 0) /// step back 2 so it jumps 1
								{
									if (i == 0)
									{
										if (CurrentSalesItemIndex == 0 && saleslst.Count == 1)
                                            i = GetPreviousAllocatedAsycudaItem(asycudaEntries, saleitm, i).Result;

                                    }
									else
                                        i -= 2;

                                }
                            }
							else
							{

								if (asycudaItmQtyToAllocate <= 0) asycudaItmQtyToAllocate = saleitmQtyToallocate;
								var ramt = await AllocateSaleItem(cAsycudaItm, saleitm, asycudaItmQtyToAllocate, subitm)
									.ConfigureAwait(false);
								if (saleitmQtyToallocate < 0 && asycudaItmQtyToAllocate < 0)
								{
									saleitmQtyToallocate += asycudaItmQtyToAllocate * -1;
									if (GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out subitm) == 0)
									{
										CurrentAsycudaItemIndex += 1;
									}
								}
								else
								{
									saleitmQtyToallocate -= asycudaItmQtyToAllocate;
								}

								// set here just incase
								if (saleitmQtyToallocate == 0) break;
								if (saleitmQtyToallocate < 0)
								{
									throw new ApplicationException("saleitmQtyToallocate < 0 check this out");
								}
							}
						}

					}

					if (saleitm.AsycudaSalesAllocations.Count == 0)
					{
						await AddExceptionAllocation(saleitm, "Over Sold").ConfigureAwait(false);
						//Debugger.Break();
					}


				}

			}


			catch (Exception e)
			{
				throw e;
			}
		}

        private string CheckExistingStock(string itemNumber, DateTime salesEntryDataDate)
        {
            var itm = new AllocationDSContext().AsycudaItemRemainingQuantities.FirstOrDefault(x =>
                x.ItemNumber == itemNumber && x.AssessmentDate <= salesEntryDataDate && x.xRemainingBalance > 0);
           return  itm == null
                ? ""
                : $": Last Available Qty on C#{itm.CNumber}-{itm.LineNumber} RegDate:{itm.RegistrationDate.GetValueOrDefault().ToShortDateString()} AstDate:{itm.AssessmentDate.GetValueOrDefault().ToShortDateString()} ItemQty:{itm.ItemQuantity}, AlloQty:{itm.QtyAllocated}, piQty:{itm.PiQuantity}";
        }

        private List<AsycudaItemRemainingQuantities> _existingStock = null;
        public List<AsycudaItemRemainingQuantities> ExistingStock =>
            _existingStock ?? (_existingStock = new AllocationDSContext().AsycudaItemRemainingQuantities
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList());

        private async Task<int> GetPreviousAllocatedAsycudaItem(List<xcuda_Item> asycudaEntries, EntryDataDetails saleitm, int i)
		{
			var previousI = 0;
			var pitmsIds = asycudaEntries.Select(x => x.Item_Id).ToList();
			var dfp = saleitm.DutyFreePaid;
			var lastAllocation = new AllocationDSContext()
				.AsycudaSalesAllocations
				.Where(x => x.EntryDataDetails.InventoryItemId ==
							saleitm.InventoryItemId
							&& pitmsIds.Any(z => z == x.PreviousItem_Id)
							&& (dfp == "Duty Free"
								? x.PreviousDocumentItem.DFQtyAllocated > 0
								: x.PreviousDocumentItem.DPQtyAllocated > 0))
				.OrderByDescending(x => x.AllocationId).FirstOrDefault();

			if (lastAllocation == null)
			{
				//if (asycudaEntries.Sum(x => x.AsycudaSalesAllocations.Count()) != 0)
					//await AddExceptionAllocation(saleitm, "Returned More than Sold")
					//	.ConfigureAwait(false);
				return i;
			}
			// refreash all items from cache and set currentindex to last previous item
			//and continue

			var lastIndex = asycudaEntries.FindLastIndex(x =>
				x.Item_Id == lastAllocation.PreviousItem_Id);
			previousI = lastIndex - 1;
			return previousI;
		}

        private async Task AddExceptionAllocation(EntryDataDetails saleitm, xcuda_Item cAsycudaItm, string error)
        {
            var existingStock = CheckExistingStock(cAsycudaItm.ItemNumber, saleitm.Sales.EntryDataDate);
            await AddExceptionAllocation(saleitm, error + existingStock).ConfigureAwait(false);
        }

        private async Task AddExceptionAllocation(EntryDataDetails saleitm,  string error)
		{
			if (saleitm.AsycudaSalesAllocations.FirstOrDefault(x => x.Status == error) == null)
			{
                var ssa = new AsycudaSalesAllocations(true)
				{
					EntryDataDetailsId = saleitm.EntryDataDetailsId,
					//EntryDataDetails = saleitm,
					QtyAllocated = saleitm.Quantity - saleitm.QtyAllocated,
					Status = error,
					TrackingState = TrackingState.Added
				};
				//await SaveAllocation(ssa).ConfigureAwait(false);
				saleitm.AsycudaSalesAllocations.Add(ssa);
			}
		}





		private double GetAsycudaItmQtyToAllocate(xcuda_Item cAsycudaItm, EntryDataDetails saleitm, out SubItems subitm)
		{
			double asycudaItmQtyToAllocate;
			if (cAsycudaItm.SalesFactor == 0) cAsycudaItm.SalesFactor = 1;
			if (cAsycudaItm.SubItems.Any())
			{
				subitm = cAsycudaItm.SubItems.FirstOrDefault(x => x.ItemNumber == saleitm.ItemNumber);
				if (subitm != null)
				{
					// TODO: Add code here to CalculateAsycudaItmQtyToAllocate like non sub items
					Debugger.Break();
					// TODO: Add code here to CalculateAsycudaItmQtyToAllocate like non sub items

					asycudaItmQtyToAllocate = subitm.Quantity - subitm.QtyAllocated;
					//if (Convert.ToDouble(asycudaItmQtyToAllocate) > (Convert.ToDouble(cAsycudaItm.ItemQuantity) - cAsycudaItm.QtyAllocated))
					//{
					//    asycudaItmQtyToAllocate = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
					//}
				}
				else
				{
					asycudaItmQtyToAllocate = 0;
				}
			}
			else
			{
				asycudaItmQtyToAllocate = CalculateAsycudaItmQtyToAllocate(cAsycudaItm, saleitm);
				subitm = null;
			}

			return asycudaItmQtyToAllocate;
		}

		private static double CalculateAsycudaItmQtyToAllocate(xcuda_Item cAsycudaItm,
			EntryDataDetails saleItem)
		{
            try
            {

          

			var totalAvailableToAllocate = cAsycudaItm.ItemQuantity - cAsycudaItm.QtyAllocated;
			//var TotalPiQty = (double)cAsycudaItm.EntryPreviousItems
			//	.Select(x => x.xcuda_PreviousItem)
			//	.Sum(x => x.Suplementary_Quantity);
			var nonDFPQty = cAsycudaItm.EntryPreviousItems.Any() ? (double)cAsycudaItm.EntryPreviousItems
				.Select(x => x.xcuda_PreviousItem)
				.Where(x => x.DutyFreePaid != saleItem.DutyFreePaid || (x.xcuda_Item.EntryDataType ?? "Sales") != saleItem.Sales.EntryDataType)
				.Sum(x => x.Suplementary_Quantity) : (saleItem.DutyFreePaid == "Duty Free" ? cAsycudaItm.DPQtyAllocated : cAsycudaItm.DFQtyAllocated);



			//var previousItems = cAsycudaItm.EntryPreviousItems
			//	.Select(x => x.xcuda_PreviousItem)
			//	.Where(x => x.DutyFreePaid == saleItem.DutyFreePaid).ToList();

			var totalDfPQtyAllocated = saleItem.DutyFreePaid == "Duty Free" ? cAsycudaItm.DFQtyAllocated : cAsycudaItm.DPQtyAllocated;

			//var TotalDFPtoAllocate = previousItems.Any() ? (double)previousItems
			//	.Sum(x => x.Suplementary_Quantity) : totalDfPQtyAllocated;
			//var TotalDFPAllocatedQty = previousItems.Any() ? previousItems
			//	.Sum(x => x.QtyAllocated) : totalDfPQtyAllocated;
			//var remainingDFPAllocation = TotalDFPtoAllocate - TotalDFPAllocatedQty;
			//var freeToAllocate = cAsycudaItm.ItemQuantity - TotalDFPtoAllocate; //TotalDFPAllocatedQty + nonDFPQty + cAsycudaItm.QtyAllocated;

			//var allocatedQty = cAsycudaItm.AsycudaSalesAllocations.Where(x => x.DutyFreePaid == saleItem.DutyFreePaid).Sum(x => x.QtyAllocated);
			var nonAllocatedQty = cAsycudaItm.AsycudaSalesAllocations.Where(x => x.DutyFreePaid != saleItem.DutyFreePaid).Sum(x => x.QtyAllocated);

			var finalNonDFPQty = nonDFPQty > nonAllocatedQty ? nonDFPQty : nonAllocatedQty;

			var TakeOut = (finalNonDFPQty + totalDfPQtyAllocated) >= cAsycudaItm.ItemQuantity 
								? finalNonDFPQty >= cAsycudaItm.ItemQuantity ? cAsycudaItm.ItemQuantity : cAsycudaItm.QtyAllocated 
								: (finalNonDFPQty + totalDfPQtyAllocated);


			var res = cAsycudaItm.ItemQuantity - TakeOut;
			if (totalAvailableToAllocate == 0) res = 0;
			return res * cAsycudaItm.SalesFactor;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
		}

		private xcuda_Item GetAsycudaEntriesWithItemNumber(IList<xcuda_Item> asycudaEntries, int CurrentAsycudaItemIndex)
		{
            _asycudaItems.TryGetValue(asycudaEntries.ElementAtOrDefault(CurrentAsycudaItemIndex).Item_Id, out var cAsycudaItm);
			///////////////////// took this out because returns cross thread with duty free and duty paid -- 'CRC/06037' 'GB00050065'
			//if (cAsycudaItm.QtyAllocated == 0 && (cAsycudaItm.DFQtyAllocated != 0 || cAsycudaItm.DPQtyAllocated != 0))
			//{

			//    cAsycudaItm.DFQtyAllocated = 0;
			//    cAsycudaItm.DPQtyAllocated = 0;
			//}

			return cAsycudaItm;
		}

		private EntryDataDetails GetSaleEntries(IList<EntryDataDetails> SaleEntries, int CurrentSaleItemIndex)
		{
			return SaleEntries.ElementAtOrDefault(CurrentSaleItemIndex);
		}

        private async Task<double> AllocateSaleItem(xcuda_Item cAsycudaItm, EntryDataDetails saleitm,
            double saleitmQtyToallocate, SubItems subitm)
        {
            //cAsycudaItm.StartTracking();
            //saleitm.StartTracking();
            if (cAsycudaItm.SalesFactor == 0) cAsycudaItm.SalesFactor = 1;

            var dfp = saleitm.DutyFreePaid;
            // allocate Sale item
            var ssa = new AsycudaSalesAllocations
            {
                EntryDataDetailsId = saleitm.EntryDataDetailsId,
                PreviousItem_Id = cAsycudaItm.Item_Id,
                QtyAllocated = 0,
                TrackingState = TrackingState.Added
            };

            if (!string.IsNullOrEmpty(cAsycudaItm.WarehouseError))
            {
                ssa.Status = cAsycudaItm.WarehouseError;
            }




            if (saleitmQtyToallocate != 0) //&& removed because of previous return//cAsycudaItm.QtyAllocated >= 0 && 
                // cAsycudaItm.QtyAllocated <= Convert.ToDouble(cAsycudaItm.ItemQuantity)
            {


                if (saleitmQtyToallocate > 0)
                {

                    if (subitm != null)
                    {
                        subitm.StartTracking();
                        subitm.QtyAllocated += saleitmQtyToallocate;
                    }

                    if (dfp == "Duty Free")
                    {
                        cAsycudaItm.DFQtyAllocated += saleitmQtyToallocate / cAsycudaItm.SalesFactor;
                    }
                    else
                    {
                        cAsycudaItm.DPQtyAllocated += saleitmQtyToallocate / cAsycudaItm.SalesFactor;
                    }

                    if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                    {
                        SetPreviousItemXbond(ssa, cAsycudaItm, dfp, saleitmQtyToallocate / cAsycudaItm.SalesFactor);
                    }



                    saleitm.QtyAllocated += saleitmQtyToallocate;

                    ssa.QtyAllocated += saleitmQtyToallocate;

                    saleitmQtyToallocate = 0;
                }
                else
                {
                    double mqty = saleitmQtyToallocate * -1;


                    if (subitm != null)
                    {
                        subitm.StartTracking();
                        subitm.QtyAllocated -= mqty;
                    }

                    if (dfp == "Duty Free")
                    {
                        //if (cAsycudaItm.DFQtyAllocated !=/*> change to != 0 to match below to mark return more than sold like below*/ 0 && cAsycudaItm.DFQtyAllocated < mqty) mqty = cAsycudaItm.DFQtyAllocated;
                        cAsycudaItm.DFQtyAllocated -= mqty / cAsycudaItm.SalesFactor;
                    }
                    else
                    {
                        //if (cAsycudaItm.DPQtyAllocated != 0 && cAsycudaItm.DPQtyAllocated < mqty) mqty = cAsycudaItm.DPQtyAllocated;
                        cAsycudaItm.DPQtyAllocated -= mqty / cAsycudaItm.SalesFactor;
                    }



                    if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate == "Visible")
                    {
                        SetPreviousItemXbond(ssa, cAsycudaItm, dfp, -mqty / cAsycudaItm.SalesFactor);
                    }

                    saleitmQtyToallocate += mqty;

                    saleitm.QtyAllocated -= mqty;


                    ssa.QtyAllocated -= mqty;

                    //}
                }
            }

            if (ssa.QtyAllocated == 0) return saleitmQtyToallocate;
            //SaveAllocation(cAsycudaItm, saleitm, subitm, ssa);

            saleitm.AsycudaSalesAllocations.Add(ssa);
            ssa.EntryDataDetails = saleitm;
            ssa.PreviousDocumentItem = cAsycudaItm;
            cAsycudaItm.AsycudaSalesAllocations.Add(ssa);
            _asycudaItems.AddOrUpdate(cAsycudaItm.Item_Id, cAsycudaItm, (key, oldValue) => cAsycudaItm);


            return saleitmQtyToallocate;
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

      
		private void SetPreviousItemXbond(AsycudaSalesAllocations ssa, xcuda_Item cAsycudaItm, string dfp, double amt)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate != "Visible") return;


            var alst = cAsycudaItm.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
                .Where(x => x.DutyFreePaid == dfp && x.QtyAllocated <= (double)x.Suplementary_Quantity)
                .Where(x => x.xcuda_Item != null && x.xcuda_Item.AsycudaDocument != null && x.xcuda_Item.AsycudaDocument.Cancelled != true)
                .OrderBy(
                    x =>
                        x.xcuda_Item.AsycudaDocument.EffectiveRegistrationDate ?? Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate)).ToList();
            foreach (var pitm in alst)
            {

                var atot = (double)pitm.Suplementary_Quantity - Convert.ToDouble(pitm.QtyAllocated);
                if (atot == 0) continue;
                if (amt <= atot)
                {
                    pitm.QtyAllocated += amt;
                    var xbond = new xBondAllocations(true)
                    {
                        AllocationId = ssa.AllocationId,
                        xEntryItem_Id = pitm.xcuda_Item.Item_Id,
                        TrackingState = TrackingState.Added
                    };

                    ssa.xBondAllocations.Add(xbond);
                    pitm.xcuda_Item.xBondAllocations.Add(xbond);
                    break;
                }
                else
                {
                    pitm.QtyAllocated += atot;
                    var xbond = new xBondAllocations(true)
                    {
                        AllocationId = ssa.AllocationId,
                        xEntryItem_Id = pitm.xcuda_Item.Item_Id,
                        TrackingState = TrackingState.Added
                    };
                    ssa.xBondAllocations.Add(xbond);
                    pitm.xcuda_Item.xBondAllocations.Add(xbond);
                    amt -= atot;
                }

            }
        }



	}
}
