﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AdjustmentQS.Business.Services;
using AllocationDS.Business.Entities;
using Core.Common.Data;
using InventoryDS.Business.Entities;
using Core.Common.UI;
using AllocationDS.Business.Services;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using EntryDataDetails = AllocationDS.Business.Entities.EntryDataDetails;
using InventoryItem = AllocationDS.Business.Entities.InventoryItem;
using InventoryItemAlias = AllocationDS.Business.Entities.InventoryItemAlias;
using Sales = AllocationDS.Business.Entities.Sales;
using SubItems = AllocationDS.Business.Entities.SubItems;
using xcuda_Item = AllocationDS.Business.Entities.xcuda_Item;
using xcuda_ItemService = AllocationDS.Business.Services.xcuda_ItemService;


namespace WaterNut.DataSpace
{
	public partial class AllocationsBaseModel
	{

		private static readonly AllocationsBaseModel instance;
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
			get {
				return _inventoryAliasCache ??
					   (_inventoryAliasCache =
						   new DataCache<InventoryItemAlias>(
							   AllocationDS.DataModels.BaseDataModel.Instance.SearchInventoryItemAlias(
								   new List<string>() {"All"}, null).Result));
			}
			set { _inventoryAliasCache = value; }
		}

		internal class ItemSales
		{
			public string Key { get; set; }
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
			public string Key { get; set; }
			public List<EntryDataDetails> SalesList { get; set; }
			public List<xcuda_Item> EntriesList { get; set; }
		}


		public async Task AllocateSales(ApplicationSettings applicationSettings, bool allocateToLastAdjustment )
		{
			var forceDiscrepancyExecution = true;

			try
			{
				PrepareDataForAllocation(applicationSettings);


			    StatusModel.Timer("Auto Match Adjustments");
				using (var ctx = new AdjustmentShortService())
				{
					await ctx.AutoMatch(applicationSettings.ApplicationSettingsId).ConfigureAwait(false);
				   if(forceDiscrepancyExecution) await ctx.ProcessDISErrorsForAllocation(applicationSettings.ApplicationSettingsId).ConfigureAwait(false);
				}


					await AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(applicationSettings.ApplicationSettingsId, allocateToLastAdjustment, null).ConfigureAwait(false);
				

				await MarkErrors(applicationSettings.ApplicationSettingsId).ConfigureAwait(false);

				StatusModel.StopStatusUpdate();
			}
			catch (Exception ex)
			{

				throw ex;
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
	            ctx.Database.ExecuteSqlCommand($@"  update EntryDataDetails
						  set taxamount = 1
						  where taxamount is null and comment in ('Promotional Expenses','Charity','Shop / Office / Warehouse Expenses','Office Expenses','PROMOTION','Promotional Expense','Promotional Expenses','Sample',
											'Shop / Office / Warehouse Expenses','SHOP EXPENSES','SP','SPONSORSHIP','STORE / WAREHOUSE EXPENSE','Store / Warehouse Expenses','STORE EXPENSE','VEHICLE EXPENSE'
											,'Vehicle Expenses','WAREHOUSE EXPENSE','WAREHOUSE EXPENSES','DONATION')


						  update EntryDataDetails
						  set taxamount = 0
						  where taxamount is null and comment in ('Annual Stock Take Adjustments','COST CHANGE','Cost of Warranty','Cycle Count Adjustment','Cycle Count Adjustment - item Stolen','Cycle Count Adjustments','Cycle Count Adjustments (Test)',
						  'DAMAGED','Damaged Goods','Error in System - Negative on Hand Report','Expired / Obsolete Goods','Expired Goods','GROSS STOCK USED INTERNATLLY- INCORRECT CODING USED','Incorrect Internal Code Used',
						  'LOST / STOLEN','Lost / Stolen Goods','LOST/ STOLEN','Lost/ Stolen Goods','PHYSICAL COUNT ADJUSTMENT','Warr Exp (Stock Write-Off) - Gen','WARRANTY','WARRANTY REPLACEMENT','WARRANTY REPLACEMENT - WRITTEN OFF','WARRANTY REPLACEMENT/ WRITTEN OFF','WARRANTY REPLACEMENT/DAMAGED IN WAREHOUSE','Warranty Replacment','Written Off - Damage Item','Written Off - Expired Goods')

						update EntryDataDetails
						  set taxamount = 1
						  where entrydataid like 'ADJ%' and TaxAmount is null");

	            ctx.Database.ExecuteSqlCommand($@"EXEC [dbo].[GetMappingFromInventory] @appsettingId
													 EXEC[dbo].[CreateInventoryAliasFromInventoryMapping]",
	                new SqlParameter("@appsettingId", applicationSettings.ApplicationSettingsId));

	            ctx.Database.ExecuteSqlCommand($@"WITH CTE AS(
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

	    public async Task MarkErrors(int applicationSettingsId)
		{
		   // MarkNoAsycudaEntry();
				
			MarkOverAllocatedEntries(applicationSettingsId);

			MarkUnderAllocatedEntries(applicationSettingsId);


		}

		public  async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
			int applicationSettingsId, bool allocateToLastAdjustment, string lst)
		{
			var itemSets = await MatchSalestoAsycudaEntriesOnItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
			StatusModel.StopStatusUpdate();
			
			StatusModel.StartStatusUpdate("Allocating Item Sales", itemSets.Count());
			var t = 0;
			var exceptions = new ConcurrentQueue<Exception>();
			var itemSetsValues = itemSets.Values;
			var count = itemSetsValues.Count();
			Parallel.ForEach(itemSetsValues.OrderBy(x => x.Key)

               //.Where(x => x.SalesList.Any(z => z.EntryDataId.ToLower().Contains("harry")))
				// .Where(x => x.Key.Contains("VET/FTR132063")) //.Where(x => x.Key.Contains("255100")) // 
																		  // .Where(x => "337493".Contains(x.Key))
																		  //.Where(x => "FAA/SCPI18X112".Contains(x.ItemNumber))//SND/IVF1010MPSF,BRG/NAVICOTE-GL,
									 , new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 }, itm => //.Where(x => x.ItemNumber == "AT18547")  
			 {
			//     foreach (var itm in itemSets.Values)//.Where(x => "FAA/SCPI18X112".Contains(x.ItemNumber))
			//{
				try
				{
					t += 1;
				   // Debug.WriteLine($"Processing {itm.Key} - {t} with {itm.SalesList.Count} Sales: {0} of {itm.SalesList.Count}");
					//StatusModel.Refresh();
				var sales = itm.SalesList
				    .OrderBy(x => x.Sales.EntryDataDate)
				    .ThenBy(x => x.EntryDataId)
				    .ThenBy(x => x.LineNumber ?? x.EntryDataDetailsId)
				    .ThenByDescending(x => x.Quantity)/**/.ToList();
				var asycudaItems = itm.EntriesList.OrderBy(x => x.AsycudaDocument.AssessmentDate)
					
					.ThenBy(x => x.IsAssessed == null).ThenBy(x => x.AsycudaDocument.RegistrationDate)
					.ThenBy(x => Convert.ToInt32(x.AsycudaDocument.CNumber))
					.ThenByDescending(x => x.EntryPreviousItems.Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum())//NUO/44545 2 items with same date choose pIed one first
					.ThenBy(x => x.AsycudaDocument.ReferenceNumber)
					.DistinctBy(x => x.Item_Id)
					.ToList();
					
					AllocateSalestoAsycudaByKey(sales, asycudaItems, t, count, allocateToLastAdjustment).Wait();
						//.SalesList.Where(x => x.DoNotAllocate != true).ToList()

						
				}
				catch (Exception ex)
				{

					exceptions.Enqueue(
								new ApplicationException(
									$"Could not Allocate - '{itm.Key}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
				}

			  //   };


			 });




			if (exceptions.Count > 0) throw new AggregateException(exceptions);
		}



		private void MarkOverAllocatedEntries(int applicationSettingsId)
		{


			try
			{
				List<xcuda_Item> IMAsycudaEntries; //"EX"

				using (var ctx = new AllocationDSContext() { StartTracking = false })
				{
				    ctx.Database.CommandTimeout = 100;

					IMAsycudaEntries = ctx.xcuda_Item
                        //.Include(x => x.AsycudaDocument)
						//.Include(x => x.xcuda_Tarification.xcuda_HScode)
						//.Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
						//.Include(x => x.SubItems)
						//.Include(x => x.xcuda_Goods_description)
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
				var alst = IMAsycudaEntries.ToList();

				//var test = IMAsycudaEntries.Where(x => x.Item_Id == 27018).ToList();


				if (alst.Any())
					Parallel.ForEach(alst
						,
						new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount*1}, i =>//
						{
							using (var ctx = new AllocationDSContext() {StartTracking = false})
							{
								var sql = "";

								if (ctx.AsycudaSalesAllocations == null) return;

								var lst =
									ctx.AsycudaSalesAllocations
										//.Include(x => x.EntryDataDetails)
										//.Include(x => x.EntryDataDetails.EntryDataDetailsEx)
										//.Include(x => x.PreviousDocumentItem)
										.Where(x => x != null && x.PreviousItem_Id == i.Item_Id)
										//.Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
										.OrderByDescending(x => x.AllocationId)
                                        .Select(x => new MarkSales()
                                        {
                                            AllocationId = x.AllocationId,
                                            QtyAllocated = x.QtyAllocated,
                                            ItemQuantity = x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity,
                                            SalesQtyAllocated = x.EntryDataDetails.QtyAllocated,
                                            EntryDataDetailsId = x.EntryDataDetails.EntryDataDetailsId,
                                            DutyFreePaid = x.EntryDataDetails.EntryDataDetailsEx.DutyFreePaid,
                                            DFQtyAllocated = x.PreviousDocumentItem.DFQtyAllocated,
                                            Item_Id = x.PreviousDocumentItem.Item_Id,
                                            DPQtyAllocated = x.PreviousDocumentItem.DPQtyAllocated,
                                            Status = x.Status,
                                            PreviousItem_Id = x.PreviousItem_Id

                                        })
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

										allo.QtyAllocated -= r;
										sql += $@" UPDATE       EntryDataDetails
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")}
															where	EntryDataDetailsId = {allo.EntryDataDetailsId}";

										if (allo.DutyFreePaid == "Duty Free")
										{
											allo.DFQtyAllocated -= r;
											i.DFQtyAllocated -= r;

											/////// is the same thing

											sql += $@" UPDATE       xcuda_Item
															SET                DFQtyAllocated = (DFQtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	item_id = {allo.Item_Id}";
										}
										else
										{
											allo.DPQtyAllocated -= r;
											i.DPQtyAllocated -= r;

											

											sql += $@" UPDATE       xcuda_Item
															SET                DPQtyAllocated = (DPQtyAllocated{(r >= 0 ? $"-{r}" : $"+{r * -1}")})
															where	item_id = {allo.Item_Id}";
										}

										if (allo.QtyAllocated == 0)
										{
											allo.QtyAllocated = r; //add back so wont disturb calculations
											allo.Status = $"Over Allocated Entry by {r}";

											sql += $@"  Update AsycudaSalesAllocations
														Set Status = '{allo.Status}', QtyAllocated = {r }
														Where AllocationId = {allo.AllocationId}";
											
										}
										else
										{
										   
											sql += $@" INSERT INTO AsycudaSalesAllocations
														 (EntryDataDetailsId, PreviousItem_Id, QtyAllocated,Status, EANumber, SANumber)
														VALUES        ({allo.EntryDataDetailsId},{allo.PreviousItem_Id},{r},'Over Allocated Entry by {r}',0,0)";
											//ctx.ApplyChanges(nallo);
											break;
										}

									}
									else
									{
									    continue;
									}




                                }

								if(!string.IsNullOrEmpty(sql))
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
			catch (Exception)
			{
				throw;
			}

		
		}

        private class MarkSales
        {
            public int AllocationId { get; set; }
            public double QtyAllocated { get; set; }
            public double? ItemQuantity { get; set; }
            public double SalesQtyAllocated { get; set; }
            public int EntryDataDetailsId { get; set; }
            public string DutyFreePaid { get; set; }
            public double DFQtyAllocated { get; set; }
            public int Item_Id { get; set; }
            public double DPQtyAllocated { get; set; }
            public string Status { get; set; }
            public int? PreviousItem_Id { get; set; }
        }

        private class MarkAllo
        {
        }


        private void MarkUnderAllocatedEntries(int applicationSettingsId)
		{


			try
			{
				List<xcuda_Item> IMAsycudaEntries; //"EX"

				using (var ctx = new AllocationDSContext() { StartTracking = false })
				{
				    ctx.Database.CommandTimeout = 100;
					IMAsycudaEntries = ctx.xcuda_Item.Include(x => x.AsycudaDocument)
						//.Include(x => x.xcuda_Tarification.xcuda_HScode)
						//.Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
						//.Include(x => x.SubItems)
						//.Include(x => x.xcuda_Goods_description)
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
					Parallel.ForEach(alst
						,
						new ParallelOptions() { MaxDegreeOfParallelism = 1 }, i =>//Environment.ProcessorCount*
						{
							using (var ctx = new AllocationDSContext() { StartTracking = false })
							{
								var sql = "";

								if (ctx.AsycudaSalesAllocations == null) return;

								var lst =
									ctx.AsycudaSalesAllocations
										//.Include(x => x.EntryDataDetails)
										//.Include(x => x.EntryDataDetails.EntryDataDetailsEx)
										//.Include(x => x.PreviousDocumentItem)
										.Where(x => x != null && x.PreviousItem_Id == i.Item_Id)
										//.Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
										.OrderBy(x => x.AllocationId)
                                        .Select(x => new MarkSales()
                                        {
                                            AllocationId = x.AllocationId,
                                            QtyAllocated = x.QtyAllocated,
                                            ItemQuantity = x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity,
                                            SalesQtyAllocated = x.EntryDataDetails.QtyAllocated,
                                            EntryDataDetailsId = x.EntryDataDetails.EntryDataDetailsId,
                                            DutyFreePaid = x.EntryDataDetails.EntryDataDetailsEx.DutyFreePaid,
                                            DFQtyAllocated = x.PreviousDocumentItem.DFQtyAllocated,
                                            Item_Id = x.PreviousDocumentItem.Item_Id,
                                            DPQtyAllocated = x.PreviousDocumentItem.DPQtyAllocated,
                                            Status = x.Status,
                                            PreviousItem_Id = x.PreviousItem_Id

                                        })
                                        .DistinctBy(x => x.AllocationId)
										.ToList();
								if(lst.Sum(x => x.QtyAllocated) < 0)
								foreach (var allo in lst)
								{
									var tot = i.QtyAllocated * -1;
									var r = tot > (allo.QtyAllocated *-1) ? allo.QtyAllocated * -1 : tot;
									if (i.QtyAllocated < 0)
									{


										allo.QtyAllocated += r;
										sql += $@" UPDATE       AsycudaSalesAllocations
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"+{r}" : $"-{r *-1}")}
															where	AllocationId = {allo.AllocationId}";

										allo.QtyAllocated += r;
										sql += $@" UPDATE       EntryDataDetails
															SET                QtyAllocated =  QtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")}
															where	EntryDataDetailsId = {allo.EntryDataDetailsId}";

										if (allo.DutyFreePaid == "Duty Free")
										{
											allo.DFQtyAllocated += r;
											i.DFQtyAllocated += r;

											/////// is the same thing

											sql += $@" UPDATE       xcuda_Item
															SET                DFQtyAllocated = (DFQtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")})
															where	item_id = {allo.Item_Id}";
										}
										else
										{
											allo.DPQtyAllocated += r;
											i.DPQtyAllocated += r;



											sql += $@" UPDATE       xcuda_Item
															SET                DPQtyAllocated = (DPQtyAllocated{(r >= 0 ? $"+{r}" : $"-{r * -1}")})
															where	item_id = {allo.Item_Id}";
										}

										if (allo.QtyAllocated == 0)
										{
											allo.QtyAllocated -= r; //add back so wont disturb calculations
											allo.Status = $"Under Allocated by {r}";

											sql += $@"  Update AsycudaSalesAllocations
														Set Status = '{allo.Status}', QtyAllocated = (QtyAllocated{(r >= 0 ? $"-{r}" : $"+{r *-1}")})
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
			catch (Exception)
			{
				throw;
			}


		}



		private async Task<ConcurrentDictionary<string, ItemSet>> MatchSalestoAsycudaEntriesOnItemNumber(
		    int applicationSettingsId, string lst)
		{
			try
			{
				var asycudaEntries = await GetAsycudaEntriesWithItemNumber(applicationSettingsId, null).ConfigureAwait(false);
				//var testr = asycudaEntries.Where(x => x.EntriesList.Any(z => z.ItemNumber == "BM/FGCM150-50")).ToList();

				var saleslst = await GetSaleslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
				//var test = saleslst.Where(x => x.SalesList.Any(z => z.ItemNumber == "BM/FGCM150-50")).ToList();

				var adjlst = await GetAdjustmentslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
				saleslst.AddRange(adjlst);

				var dislst = await GetDiscrepancieslstWithItemNumber(applicationSettingsId, lst).ConfigureAwait(false);
				saleslst.AddRange(dislst);

				var itmLst = CreateItemSetsWithItemNumbers(saleslst, asycudaEntries);

				//var test = itmLst.Where(x => x.Key == "8BM/MK-BAG-REUSE60").ToList();

				return itmLst;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}


		private static ConcurrentDictionary<string,ItemSet> CreateItemSetsWithItemNumbers(IEnumerable<ItemSales> saleslst, IEnumerable<ItemEntries> asycudaEntries)
		{

			var flatAsycudaEntries = asycudaEntries.SelectMany(x => x.EntriesList).ToList();
			

			var itmLst = from s in saleslst
						 join a in asycudaEntries on s.Key equals a.Key into j
						 from a in j.DefaultIfEmpty()
						 select new ItemSet
						 {

							 Key = s.Key,
							 SalesList = s.SalesList,
							 EntriesList = a?.EntriesList ?? new List<xcuda_Item>()
						 };

			var res = new ConcurrentDictionary<string, ItemSet>();
			foreach (var itm in itmLst)
			{

				res.AddOrUpdate(itm.Key, itm,(key,value) =>
				{
					//value.EntriesList.AddRange(itm.EntriesList);  ------ causes Duplicated entries
					value.SalesList.AddRange(itm.SalesList);
						value.SalesList = value.SalesList.OrderBy(x => x.Sales.EntryDataDate).ThenBy(x => x.EntryDataId).ToList();
					return value ;
				});
			}

			
			foreach (var r in res)//.Where(x => x.Key == "5331368").ToList()
			{
				var alias = Instance.InventoryAliasCache.Data.Where(x => x.ItemNumber.ToUpper().Trim() == r.Key).Select(y => y.AliasName.ToUpper().Trim()).ToList();
				if (!alias.Any()) continue;
				//var te = asycudaEntries.Where(x => x.Key == "EVC/508").ToList();
				var ae = asycudaEntries.Where(x => alias.Contains(x.Key)).SelectMany(y => y.EntriesList).ToList();
				if (ae.Any()) r.Value.EntriesList.AddRange(ae);

				// Manual allocation
				foreach (var itm in r.Value.SalesList.Where(x => x.ManualAllocations != null))
				{
					var ritm = flatAsycudaEntries.FirstOrDefault(x => x.Item_Id == itm.ManualAllocations.Item_Id);
					if(ritm != null) r.Value.EntriesList.Add(ritm);
				}
				
				r.Value.EntriesList.AddRange(flatAsycudaEntries.Where(x => x.PreviousInvoiceItemNumber == r.Key));

			}
			return res;
		}

		private static ConcurrentDictionary<string, ItemSet> CreateItemSetsWithDescription(IEnumerable<ItemSales> saleslst, IEnumerable<ItemEntries> asycudaEntries)
		{

			var itmLst = from s in saleslst
				from a in asycudaEntries
						 where s.Key == a.Key || (s.Key.Contains(a.Key) || a.Key.Contains(s.Key))
				select new ItemSet
				{

					Key = s.Key,
					SalesList = s.SalesList,
					EntriesList = a?.EntriesList
				};


			var res = new ConcurrentDictionary<string, ItemSet>();
			foreach (var itm in itmLst)
			{

				res.AddOrUpdate(itm.Key, itm, (key, value) => itm);
			}


			foreach (var r in res.Values.Where(x => x.EntriesList == null))
			{
				//var r = res.FirstOrDefault(x => x.Key == alias.AliasName);
				var alias = Instance.InventoryAliasCache.Data.Where(x => x.ItemNumber == r.Key).Select(y => y.AliasName).ToList();
				var ae = asycudaEntries.Where(x => alias.Contains(x.Key)).SelectMany(y => y.EntriesList).ToList();
				if (ae.Any()) r.EntriesList = ae;
			}
			return res;
		}


		private static async Task<IEnumerable<ItemEntries>> GetAsycudaEntriesWithItemNumber(int applicationSettingsId, int? asycudaDocumentSetId)
		{
			StatusModel.Timer("Getting Data - Asycuda Entries...");
			//string itmnumber = "WMHP24-72";
			IEnumerable<ItemEntries> asycudaEntries = null;
			using (var ctx = new AllocationDSContext() { StartTracking = false })
			{

				//asycudaEntries = from s in ctx.xcuda_Item
				//							.Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
				//							.Where(x => x.xcuda_Tarification.xcuda_HScode.Precision_4 != null)
				//							.Where(x => asycudaDocumentSetId == null || x.AsycudaDocument.AsycudaDocumentSetId == asycudaDocumentSetId)
				//							.Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true)
				//										&& ( x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse)
				//										&& (x.AsycudaDocument.Customs_Procedure.Sales == true || x.AsycudaDocument.Customs_Procedure.Stock == true) &&
				//										 (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
				//										 x.AsycudaDocument.DoNotAllocate != true)
				//							.Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
				//							.OrderBy(x => x.LineNumber)

				//				 group s by s.xcuda_Tarification.xcuda_HScode.Precision_4.ToUpper().Trim()
				//	into g
				//				 select
				//					 new ItemEntries
				//					 {
				//						 Key = g.Key.Trim(),
				//						 EntriesList =
				//							 g.AsEnumerable()
				//								 .OrderBy(
				//									 x =>
				//										 x.AsycudaDocument.EffectiveRegistrationDate == null
				//											 ? Convert.ToDateTime(x.AsycudaDocument.RegistrationDate)
				//											 : x.AsycudaDocument.EffectiveRegistrationDate)
				//								 .ToList()
				//					 };

				//return asycudaEntries.ToList();

				var lst = ctx.xcuda_Item.Include(x => x.AsycudaDocument.Customs_Procedure)
                    .Include(x => x.xcuda_Tarification.xcuda_HScode)
                    .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                    .Include(x => x.SubItems)
                    .Include("EntryPreviousItems.xcuda_PreviousItem")
					.Where(x => x.xcuda_Tarification.xcuda_HScode.Precision_4 != null)
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

                asycudaEntries = from s in lst
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

				return asycudaEntries.ToList();
			}

            //var res = asycudaEntries.Where(x => x.Key == "BM/SHG16B");

        }

		private static async Task<IEnumerable<ItemEntries>> GetAsycudaEntriesWithDescription()
		{
			StatusModel.Timer("Getting Data - Asycuda Entries...");
			//string itmnumber = "WMHP24-72";
			IEnumerable<ItemEntries> asycudaEntries = null;
			using (var ctx = new xcuda_ItemService())
			{
				var lst = await ctx.Getxcuda_ItemByExpressionNav(
					"All",
					// "xcuda_Tarification.xcuda_HScode.Precision_4 == \"1360\"",
					new Dictionary<string, string>() { { "AsycudaDocument", ( $"AssessmentDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && ") +
					                                                        $"(pCNumber != null || IsManuallyAssessed == true) " +
					                                                        $"&& (Customs_Procedure.CustomsOperationId == {(int)CustomsOperations.Import} || Customs_Procedure.CustomsOperationId == {(int)CustomsOperations.Warehouse}) " +
					                                                        $"&& Customs_Procedure.Sales == true)" +
					                                                        $" && DoNotAllocate != true" } }
					, new List<string>() { "AsycudaDocument",
						"xcuda_Tarification.xcuda_HScode", "xcuda_Tarification.xcuda_Supplementary_unit","SubItems", "xcuda_Goods_description",
					}).ConfigureAwait(false);//"EX"
			  


                asycudaEntries = from s in lst.Where(x => x.xcuda_Tarification.xcuda_HScode.Precision_4 != null)
					 //.Where(x => x.ItemDescription == "Hardener-Resin 'A' Slow .44Pt")
					//       .Where(x => x.AsycudaDocument.pCNumber != null).AsEnumerable()
					group s by s.ItemDescription.Trim()
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
	                                "&& DoNotAllocate != true", new Dictionary<string, string>()
	                                {
	                                    {"Sales", "INVNumber != null"}
	                                }, new List<string>() {"Sales", "AsycudaSalesAllocations", "ManualAllocations"}, false)
	                            .ConfigureAwait(false);
	                saleslst = from d in salesData.Where(x => lst == null || lst.Contains(x.ItemNumber))
	                    group d by d.ItemNumber.ToUpper().Trim()
	                    into g
	                    select
	                        new ItemSales
	                        {
	                            Key = g.Key,
	                            SalesList = g.Where(xy => xy != null & xy.Sales != null)
	                                .OrderBy(x => x.Sales.EntryDataDate).ThenBy(x => x.EntryDataId).ToList()
	                        };
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
						ctx.GetEntryDataDetailsByExpressionNav(//"ItemNumber == \"AAA/13576\" &&" +
																($"Adjustments.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && ") +
															   "QtyAllocated != Quantity && " +
																$"Adjustments.ApplicationSettingsId == {applicationSettingsId} && " +
																$"(\"{lst}\" == \"\" || \"{lst}\".Contains(ItemNumber)) && " +
                                                                $"Adjustments.Type == \"ADJ\" && " + /// Only Adjustments not DIS that should have pCNumber to get matched
																"((PreviousInvoiceNumber == null) ||" +//pCNumber == null && 
																" (( PreviousInvoiceNumber != null) && QtyAllocated == 0))" + //trying to capture unallocated adjustments//pCNumber != null ||
																" && (ReceivedQty - InvoiceQty) < 0 && (EffectiveDate != null || " + ( $"EffectiveDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"") +  ")" +
															   //"&& Cost > 0 " + --------Cost don't matter in allocations because it comes from previous doc
															   "&& DoNotAllocate != true", new Dictionary<string, string>()
															   {
																   { "Adjustments", "EntryDataId != null" }
															   }, new List<string>() { "Adjustments", "AsycudaSalesAllocations" }, false)
							.ConfigureAwait(false);
				adjlst = (from d in salesData
							   //.Where(x => x.EntryData == "GB-0009053")                                       
							   //.SelectMany(x => x.EntryDataDetails.Select(ed => ed))
							   //.Where(x => x.QtyAllocated == null || x.QtyAllocated != ((Double) x.Quantity))
							   //.Where(x => x.ItemNumber == itmnumber)
							   // .AsEnumerable()
						   group d by d.ItemNumber.ToUpper().Trim()
					into g
						   select
							   new ItemSales
							   {
								   Key = g.Key,
								   SalesList = g.Where(xy => xy != null & xy.Adjustments != null).OrderBy(x => x.EffectiveDate).ThenBy(x => x.Adjustments.EntryDataDate).ThenBy(x => x.EntryDataId).ToList()
							   }).ToList();
			}
		   adjlst.SelectMany(x => x.SalesList).ForEach(x =>
		   {
			   x.Sales = new Sales()
			   {
				   EntryDataId = x.Adjustments.EntryDataId,
				   EntryDataDate = Convert.ToDateTime(x.EffectiveDate),
				   INVNumber = x.Adjustments.EntryDataId,
                   Tax = x.Adjustments.Tax
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
															   "QtyAllocated != Quantity && " +
																$"Adjustments.ApplicationSettingsId == {applicationSettingsId} && " +
																$"(\"{lst}\" == \"\" || \"{lst}\".Contains(ItemNumber)) && " +
                                                                $"Adjustments.Type == \"DIS\" && " + /// Only Discrepancies with Errors
																$"Comment.StartsWith(\"DISERROR:\") && " +
																"(( PreviousInvoiceNumber == null) ||" +//pCNumber == null &&
																" (( PreviousInvoiceNumber != null) && QtyAllocated == 0))" + //trying to capture unallocated adjustments  // pCNumber != null ||
																" && (ReceivedQty - InvoiceQty) < 0 && (EffectiveDate != null || " + ($"EffectiveDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"") + ")" +
															   //"&& Cost > 0 " + --------Cost don't matter in allocations because it comes from previous doc
															   "&& DoNotAllocate != true", new Dictionary<string, string>()
															   {
																   { "Adjustments", "EntryDataId != null" }
															   }, new List<string>() { "Adjustments", "AsycudaSalesAllocations" }, false)
							.ConfigureAwait(false);
				adjlst = (from d in salesData
							  //.Where(x => x.EntryData == "GB-0009053")                                       
							  //.SelectMany(x => x.EntryDataDetails.Select(ed => ed))
							  //.Where(x => x.QtyAllocated == null || x.QtyAllocated != ((Double) x.Quantity))
							  //.Where(x => x.ItemNumber == itmnumber)
							  // .AsEnumerable()
						  group d by d.ItemNumber.ToUpper().Trim()
					into g
						  select
							  new ItemSales
							  {
								  Key = g.Key,
								  SalesList = g.Where(xy => xy != null & xy.Adjustments != null).OrderBy(x => x.EffectiveDate).ThenBy(x => x.Adjustments.EntryDataDate).ThenBy(x => x.EntryDataId).ToList()
							  }).ToList();
			}
			adjlst.SelectMany(x => x.SalesList).ForEach(x =>
			{
				x.Sales = new Sales()
				{
					EntryDataId = x.Adjustments.EntryDataId,
					EntryDataDate = Convert.ToDateTime(x.EffectiveDate),
					INVNumber = x.Adjustments.EntryDataId,
				};
				x.Comment = "Adjustment";
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
					{
						if (item.AsycudaSalesAllocations.FirstOrDefault(x => x.Status == "No Asycuda Entries Found") == null)
						{
							await AddExceptionAllocation(item, "No Asycuda Entries Found").ConfigureAwait(false);

						}

					}

					return;
				}

				var CurrentAsycudaItemIndex = 0;
				var CurrentSalesItemIndex = 0;
				var cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex);
				var saleitm = GetSaleEntries(saleslst, CurrentSalesItemIndex);

				
				while (cAsycudaItm.QtyAllocated == Convert.ToDouble(cAsycudaItm.ItemQuantity))
				{
					if (CurrentAsycudaItemIndex + 1 < asycudaEntries.Count())
					{
						CurrentAsycudaItemIndex += 1;
						cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex);
					}
					else
					{
						
						break;
					}
				}

				
				for (var s = CurrentSalesItemIndex; s < saleslst.Count(); s++)
				{
					


					if (CurrentSalesItemIndex != s)
					{
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
							await AddExceptionAllocation(saleitm, "Early Sales").ConfigureAwait(false);
							continue;
						}
					}

					for (var i = CurrentAsycudaItemIndex; i < asycudaEntries.Count(); i++)
					{
						// reset in event earlier dat

						if (CurrentAsycudaItemIndex != i || GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex).Item_Id != cAsycudaItm.Item_Id)
						{
							if (i < 0) i = 0;
							CurrentAsycudaItemIndex = i;
							cAsycudaItm = GetAsycudaEntriesWithItemNumber(asycudaEntries, CurrentAsycudaItemIndex);
							
						}
						Debug.WriteLine($"Processing {saleitm.ItemNumber} - {currentSetNo} of {setNo} with {saleslst.Count} Sales: {s} of {saleslst.Count} : {CurrentAsycudaItemIndex} of {asycudaEntries.Count}");

						
						var asycudaItmQtyToAllocate = GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out var subitm);

						// 
						if (asycudaItmQtyToAllocate == 0 && saleitmQtyToallocate > 0 && (CurrentAsycudaItemIndex != 0 || CurrentAsycudaItemIndex != asycudaEntries.Count - 1)
							&& (CurrentAsycudaItemIndex != asycudaEntries.Count -1 && asycudaEntries[i + 1].AsycudaDocument.AssessmentDate <= saleitm.Sales.EntryDataDate))
						{
							CurrentAsycudaItemIndex += 1;
							continue;
						}

						if (cAsycudaItm.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse && (cAsycudaItm.AsycudaDocument.AssessmentDate > saleitm.Sales.EntryDataDate))
						{
							if (CurrentAsycudaItemIndex == 0)
							{
								await AddExceptionAllocation(saleitm, "Early Sales").ConfigureAwait(false);
								break;
							}

							i -= 2;
							continue;
							// set it bac then continue


						}

						

						if (asycudaItmQtyToAllocate < 0 &&
							(CurrentAsycudaItemIndex != asycudaEntries.Count - 1 && asycudaEntries[i + 1].AsycudaDocument.AssessmentDate <= saleitm.Sales.EntryDataDate))
						{
							if(saleitmQtyToallocate > 0)continue;
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

								if (GetAsycudaItmQtyToAllocate(cAsycudaItm, saleitm, out subitm) == 0)
								{
									CurrentAsycudaItemIndex += 1;
								}

								if (ramt == 0) break;
								if (ramt < 0) /// step back 2 so it jumps 1
								{
									if (i == 0)
									{
										if (CurrentSalesItemIndex == 0 && saleslst.Count == 1)
											await AddExceptionAllocation(saleitm, "Returned More than Sold")
												.ConfigureAwait(false);
										break;
									}

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

					
					

				}
					
			}


			catch (Exception e)
			{
				throw e;
			}
		}


	 
		private async Task AddExceptionAllocation(EntryDataDetails saleitm, string error)
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
				await SaveAllocation(ssa).ConfigureAwait(false);
				saleitm.AsycudaSalesAllocations.Add(ssa);
			}
		}



		private static async Task SaveEntryDataDetails(EntryDataDetails item)
		{
			if (item == null) return;
			using (var ctx = new EntryDataDetailsService())
			{
				await ctx.UpdateEntryDataDetails(item).ConfigureAwait(false);
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
				asycudaItmQtyToAllocate = (cAsycudaItm.ItemQuantity * cAsycudaItm.SalesFactor) - (cAsycudaItm.QtyAllocated * cAsycudaItm.SalesFactor);
				subitm = null;
			}

			return asycudaItmQtyToAllocate;
		}

		private  xcuda_Item GetAsycudaEntriesWithItemNumber(IList<xcuda_Item> asycudaEntries, int CurrentAsycudaItemIndex)
		{
			var cAsycudaItm = asycudaEntries.ElementAtOrDefault<xcuda_Item>(CurrentAsycudaItemIndex);
			///////////////////// took this out because returns cross thread with duty free and duty paid -- 'CRC/06037' 'GB00050065'
			//if (cAsycudaItm.QtyAllocated == 0 && (cAsycudaItm.DFQtyAllocated != 0 || cAsycudaItm.DPQtyAllocated != 0))
			//{

			//    cAsycudaItm.DFQtyAllocated = 0;
			//    cAsycudaItm.DPQtyAllocated = 0;
			//}

			return cAsycudaItm;
		}

		private  EntryDataDetails GetSaleEntries(IList<EntryDataDetails> SaleEntries, int CurrentSaleItemIndex)
		{
			return SaleEntries.ElementAtOrDefault<EntryDataDetails>(CurrentSaleItemIndex);
		}

		private  async Task<double> AllocateSaleItem(xcuda_Item cAsycudaItm, EntryDataDetails saleitm,
											 double saleitmQtyToallocate, SubItems subitm)
		{
			try
			{
				//cAsycudaItm.StartTracking();
				//saleitm.StartTracking();
				if (cAsycudaItm.SalesFactor == 0) cAsycudaItm.SalesFactor = 1;

				var dfp = saleitm.DutyFreePaid;
				// allocate Sale item
				var ssa = new AsycudaSalesAllocations()
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
				


				if (saleitmQtyToallocate != 0)//&& removed because of previous return//cAsycudaItm.QtyAllocated >= 0 && 
				   // cAsycudaItm.QtyAllocated <= Convert.ToDouble(cAsycudaItm.ItemQuantity)
				{


					if (saleitmQtyToallocate > 0)
					{

						if (subitm != null)
						{
							subitm.StartTracking();
							subitm.QtyAllocated = subitm.QtyAllocated + saleitmQtyToallocate;
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
								subitm.QtyAllocated = subitm.QtyAllocated - mqty;
							}

						if (dfp == "Duty Free")
						{
							
							if (cAsycudaItm.DFQtyAllocated > 0 && cAsycudaItm.DFQtyAllocated < mqty) mqty = cAsycudaItm.DFQtyAllocated;
							cAsycudaItm.DFQtyAllocated -= mqty / cAsycudaItm.SalesFactor;
						}
						else
						{
							if (cAsycudaItm.DFQtyAllocated != 0 && cAsycudaItm.DPQtyAllocated < mqty) mqty = cAsycudaItm.DPQtyAllocated;
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
				using (var ctx = new AllocationDSContext() {StartTracking = false})
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

					saleitm.AsycudaSalesAllocations.Add(ssa);
				}

				return saleitmQtyToallocate;
				


			}
			catch (Exception)
			{

				throw;
			}
		}

		private async Task SaveAllocation(AsycudaSalesAllocations ssa)
		{
			using (var ctx = new AsycudaSalesAllocationsService())
			{
				await ctx.UpdateAsycudaSalesAllocations(ssa).ConfigureAwait(false);
			}
		}

		private static async Task SaveXcuda_Item(xcuda_Item cAsycudaItm)
		{
			using (var ctx = new xcuda_ItemService())
			{
				await ctx.Updatexcuda_Item(cAsycudaItm).ConfigureAwait(false);
			}
		}

		private static async Task SaveSubItem(SubItems subitm)
		{
			if (subitm == null) return;
			using (var ctx = new SubItemsService())
			{
				await ctx.UpdateSubItems(subitm).ConfigureAwait(false);
			}
		}

		private void SetPreviousItemXbond(AsycudaSalesAllocations ssa, xcuda_Item cAsycudaItm, string dfp, double amt)
		{
			try
			{
				if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate != "Visible") return;


				var alst = cAsycudaItm.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
							.Where(x => x.DutyFreePaid == dfp && x.QtyAllocated <= x.Suplementary_Quantity)
							.Where(x => x.xcuda_Item != null && x.xcuda_Item.AsycudaDocument != null)
							.OrderBy(
									x =>
									x.xcuda_Item.AsycudaDocument.EffectiveRegistrationDate ?? Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate)).ToList();
				foreach (var pitm in alst)
				{
					if (pitm.QtyAllocated == null) pitm.QtyAllocated = 0;
					var atot = pitm.Suplementary_Quantity - Convert.ToSingle(pitm.QtyAllocated);
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
			catch (Exception Ex)
			{
				throw;
			}
		}


	  
	}
}
