using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
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


        public async Task AllocateSales(ApplicationSettings applicationSettings, bool allocateToLastAdjustment)
        {
            try
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
                        WHERE (EntryData.ApplicationSettingsId = {applicationSettings.ApplicationSettingsId}) AND (EntryDataDetails.Status IS NULL)");
                }


                StatusModel.Timer("Auto Match Adjustments");
                using (var ctx = new AdjustmentShortService())
                {
                    await ctx.AutoMatch(applicationSettings.ApplicationSettingsId).ConfigureAwait(false);
                }


                
                    //     StatusModel.Timer("Allocating Sales");
                    //if (itemDescriptionContainsAsycudaAttribute == true)
                    //{
                    //    await AllocateSalesWhereItemDescriptionContainsAsycudaAttribute().ConfigureAwait(false);
                    //}
                    //else
                    //{
                    await AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(applicationSettings.ApplicationSettingsId, allocateToLastAdjustment).ConfigureAwait(false);
                //}

                await MarkErrors(applicationSettings.ApplicationSettingsId).ConfigureAwait(false);

                StatusModel.StopStatusUpdate();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private async Task MarkErrors(int applicationSettingsId)
        {
           // MarkNoAsycudaEntry();
                
            MarkOverAllocatedEntries(applicationSettingsId);

            MarkUnderAllocatedEntries(applicationSettingsId);


        }

        private async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
            int applicationSettingsId, bool allocateToLastAdjustment)
        {
            var itemSets = await MatchSalestoAsycudaEntriesOnItemNumber(applicationSettingsId).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
            
            StatusModel.StartStatusUpdate("Allocating Item Sales", itemSets.Count());
            var t = 0;
            var exceptions = new ConcurrentQueue<Exception>();
            var itemSetsValues = itemSets.Values;
            var count = itemSetsValues.Count();
            Parallel.ForEach(itemSetsValues.OrderBy(x => x.Key)

                    .Where(x => x.Key.Contains("CRC/06037")) //.Where(x => x.Key.Contains("255100")) // 
                                                                          // .Where(x => "337493".Contains(x.Key))
                                                                          //.Where(x => "FAA/SCPI18X112".Contains(x.ItemNumber))//SND/IVF1010MPSF,BRG/NAVICOTE-GL,
                                     , new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount *  1 }, itm => //.Where(x => x.ItemNumber == "AT18547")
             {
            //     foreach (var itm in itemSets.Values)//.Where(x => "FAA/SCPI18X112".Contains(x.ItemNumber))
            //{
                try
                {
                    t += 1;
                   // Debug.WriteLine($"Processing {itm.Key} - {t} with {itm.SalesList.Count} Sales: {0} of {itm.SalesList.Count}");
                    //StatusModel.Refresh();
                var sales = itm.SalesList.OrderBy(x => x.Sales.EntryDataDate).ThenBy(x => x.EntryDataId).ThenBy(x => x.LineNumber ?? x.EntryDataDetailsId).ThenByDescending(x => x.Quantity)/**/.ToList();
                var asycudaItems = itm.EntriesList.OrderBy(x => x.AsycudaDocument.AssessmentDate)
                    
                    .ThenBy(x => x.IsAssessed == null).ThenBy(x => x.AsycudaDocument.RegistrationDate)
                    .ThenBy(x => Convert.ToInt32(x.AsycudaDocument.CNumber))
                    .ThenByDescending(x => x.EntryPreviousItems.Select(z => z.xcuda_PreviousItem.Suplementary_Quantity).DefaultIfEmpty(0).Sum())//NUO/44545 2 items with same date choose pIed one first
                    .ThenBy(x => x.AsycudaDocument.ReferenceNumber).ToList();
                    
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


            // var subitms = itemSets.Values.Where(x => x != null && x.EntriesList != null).SelectMany(x => x.EntriesList).SelectMany(x => x.SubItems)
            //         .Where(x => x != null && x.ChangeTracker != null)
            //         .ToList();

            // await SaveSubItems(subitms).ConfigureAwait(false);

            // var alst =
            //     itemSets.Values.Where(x => x != null && x.EntriesList != null).SelectMany(x => x.EntriesList)
            //         .Where(x => x != null && x.ChangeTracker != null)
            //         .ToList();

            //await SaveAsycudaEntries(alst).ConfigureAwait(false);

            //var slst =
            //    itemSets.Values.Where(x => x != null && x.SalesList != null).SelectMany(x => x.SalesList)
            //        .Where(x => x != null && x.ChangeTracker != null)
            //        .ToList();

            //await SaveEntryDataDetails(slst).ConfigureAwait(false);

            //await MarkOverAllocatedEntries(alst).ConfigureAwait(false);

            // await MarkNoAsycudaEntry(alst).ConfigureAwait(false);



            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }


        //private async Task AllocateSalesByMatchingSalestoAsycudaEntriesOnDescription()
        //{
        //    var itemSets = await MatchSalestoAsycudaEntriesOnDescription().ConfigureAwait(false);

        //    StatusModel.StartStatusUpdate("Allocating Item Sales", itemSets.Count());
        //    var t = 0;
        //    var exceptions = new ConcurrentQueue<Exception>();
        //    Parallel.ForEach(itemSets.Values
        //        // .Where(x => "Paint-B Micron 66 Bl Ga".Contains(x.Key))
        //        //.Where(x => "FAA/SCPI18X112".Contains(x.ItemNumber))//SND/IVF1010MPSF,BRG/NAVICOTE-GL,
        //        , new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 }, itm => //.Where(x => x.ItemNumber == "AT18547")
        //        {
        //            //     foreach (var itm in itemSets.Values)//.Where(x => "FAA/SCPI18X112".Contains(x.ItemNumber))
        //            //{
        //            try
        //            {
        //                StatusModel.StatusUpdate();
        //                AllocateSalestoAsycudaByKey(itm.SalesList,
        //                    //.SalesList.Where(x => x.DoNotAllocate != true).ToList()
        //                    itm.EntriesList).Wait();
        //            }
        //            catch (Exception ex)
        //            {

        //                exceptions.Enqueue(
        //                    new ApplicationException(
        //                        string.Format("Could not Allocate - '{0}. Error:{1} Stacktrace:{2}", itm.Key,
        //                            ex.Message, ex.StackTrace)));
        //            }

        //            //   };


        //        });


        //    var subitms = itemSets.Values.Where(x => x != null && x.EntriesList != null).SelectMany(x => x.EntriesList).SelectMany(x => x.SubItems)
        //        .Where(x => x != null && x.ChangeTracker != null)
        //        .ToList();

        //    await SaveSubItems(subitms).ConfigureAwait(false);

        //    var alst =
        //        itemSets.Values.Where(x => x != null && x.EntriesList != null).SelectMany(x => x.EntriesList)
        //            .Where(x => x != null && x.ChangeTracker != null)
        //            .ToList();

        //    await SaveAsycudaEntries(alst).ConfigureAwait(false);

        //    var slst =
        //        itemSets.Values.Where(x => x != null && x.SalesList != null).SelectMany(x => x.SalesList)
        //            .Where(x => x != null && x.ChangeTracker != null)
        //            .ToList();

        //    await SaveEntryDataDetails(slst).ConfigureAwait(false);

        //    //await MarkOverAllocatedEntries(alst).ConfigureAwait(false);
        //    //await MarkNoAsycudaEntry(alst).ConfigureAwait(false);

        //    if (exceptions.Count > 0) throw new AggregateException(exceptions);
        //}


        //private async Task AllocateSalesWhereItemDescriptionContainsAsycudaAttribute()
        //{


        //    StatusModel.Timer("Loading Sales Data...");
        //    var sales = (await GetSales().ConfigureAwait(false)).ToList();//.Where(x => x.ItemDescription.Contains("26196-0008"))

        //    StatusModel.Timer("Loading Asycuda Data...");
        //    var IMAsycudaEntries = (await GetAllAsycudaEntries().ConfigureAwait(false)).ToList();

           
        //    StatusModel.StartStatusUpdate("Allocating Sales", sales.Count());
        //    var t = 0;

        //    var exceptions = new ConcurrentQueue<Exception>();

        //    for (int i = 0; i < sales.Count(); i++)
        //    {
        //        var g = sales.ElementAtOrDefault(i);

               
        //        //Parallel.ForEach(salesGrps,
        //        //    new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount}, g =>
        //        //    {
        //        try
        //        {
        //            var salesDescrip = g.ItemNumber + "|" + g.ItemDescription;
                    
        //            var strs = salesDescrip.Split('|');
  
        //            //string attrib = strs.Length >= 4
        //            //    ? strs[3].ToUpper().Replace(" ", "")
        //            //    : null;

        //            string attrib = strs.LastOrDefault();


        //            StatusModel.StatusUpdate();
        //            var alst = GetAsycudaEntriesWithItemNumber(IMAsycudaEntries, attrib, salesDescrip,
        //                new List<string>(){g.ItemNumber}).ToList(); //

        //            var slst = new List<EntryDataDetails>(){g};
                    
        //            if (slst.Any())
        //                AllocateSalestoAsycudaByKey(slst.OrderByDescending(x => x.Quantity).ToList(), alst, i).Wait();


        //            Debug.WriteLine(g.ItemDescription + " " + DateTime.Now.ToShortTimeString());
        //        }
        //        catch (Exception ex)
        //        {

        //            exceptions.Enqueue(ex);
        //        }
        //    }
        //    ///  });

        //    //await MarkOverAllocatedEntries(IMAsycudaEntries).ConfigureAwait(false);
        //    //await MarkNoAsycudaEntry(IMAsycudaEntries).ConfigureAwait(false);


        //    await SaveAsycudaEntries(IMAsycudaEntries.Where(x => x.ChangeTracker != null)).ConfigureAwait(false);

        //    await SaveEntryDataDetails(sales.Where(x => x.ChangeTracker != null)).ConfigureAwait(false);

        //    if (exceptions.Count > 0) throw new AggregateException(exceptions);
        //    //  );
        //}

        private void MarkOverAllocatedEntries(int applicationSettingsId)
        {


            try
            {
                List<xcuda_Item> IMAsycudaEntries; //"EX"

                using (var ctx = new AllocationDSContext() { StartTracking = false })
                {
                    IMAsycudaEntries = ctx.xcuda_Item.Include(x => x.AsycudaDocument)
                        .Include(x => x.xcuda_Tarification.xcuda_HScode)
                        .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                        .Include(x => x.SubItems)
                        .Include(x => x.xcuda_Goods_description)
                        .Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                    (x.AsycudaDocument.Extended_customs_procedure == "7000" || x.AsycudaDocument.Extended_customs_procedure == "7400" || x.AsycudaDocument.Extended_customs_procedure == "7100" || x.AsycudaDocument.Extended_customs_procedure == "7500" || 
                                     x.AsycudaDocument.Extended_customs_procedure == "9000") &&
                                    x.AsycudaDocument.DoNotAllocate != true)
                        .Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings
                                                                             .OpeningStockDate ?? DateTime.MinValue.Date))
                        .ToList();

                   
                }



                if (IMAsycudaEntries == null || !IMAsycudaEntries.Any()) return;
                var alst = IMAsycudaEntries.ToList();
                if (alst.Any())
                    Parallel.ForEach(alst.Where(x => x != null 
                                        && ((x.DFQtyAllocated + x.DPQtyAllocated) > Convert.ToDouble(x.ItemQuantity)))
                        ,
                        new ParallelOptions() {MaxDegreeOfParallelism = 1}, i =>//Environment.ProcessorCount*
                        {
                            using (var ctx = new AllocationDSContext() {StartTracking = false})
                            {
                                var sql = "";

                                if (ctx.AsycudaSalesAllocations == null) return;

                                var lst =
                                    ctx.AsycudaSalesAllocations.Where(
                                            x => x != null && x.PreviousItem_Id == i.Item_Id)
                                        .OrderByDescending(x => x.AllocationId)
                                        .Include(x => x.EntryDataDetails)
                                        .Include(x => x.EntryDataDetails.EntryDataDetailsEx)
                                        .Include(x => x.PreviousDocumentItem).ToList();

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

        
        private void MarkUnderAllocatedEntries(int applicationSettingsId)
        {


            try
            {
                List<xcuda_Item> IMAsycudaEntries; //"EX"

                using (var ctx = new AllocationDSContext() { StartTracking = false })
                {
                    IMAsycudaEntries = ctx.xcuda_Item.Include(x => x.AsycudaDocument)
                        .Include(x => x.xcuda_Tarification.xcuda_HScode)
                        .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                        .Include(x => x.SubItems)
                        .Include(x => x.xcuda_Goods_description)
                        .Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                    (x.AsycudaDocument.Extended_customs_procedure == "7000" || x.AsycudaDocument.Extended_customs_procedure == "7400" || x.AsycudaDocument.Extended_customs_procedure == "7100" || x.AsycudaDocument.Extended_customs_procedure == "7500" ||
                                     x.AsycudaDocument.Extended_customs_procedure == "9000") &&
                                    x.AsycudaDocument.DoNotAllocate != true)
                        .Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings
                                                                             .OpeningStockDate ?? DateTime.MinValue.Date))
                        .ToList();


                }



                if (IMAsycudaEntries == null || !IMAsycudaEntries.Any()) return;
                var alst = IMAsycudaEntries.ToList();
                if (alst.Any())
                    Parallel.ForEach(alst.Where(x => x != null
                                        && ((x.DFQtyAllocated + x.DPQtyAllocated) < 0))
                        ,
                        new ParallelOptions() { MaxDegreeOfParallelism = 1 }, i =>//Environment.ProcessorCount*
                        {
                            using (var ctx = new AllocationDSContext() { StartTracking = false })
                            {
                                var sql = "";

                                if (ctx.AsycudaSalesAllocations == null) return;

                                var lst =
                                    ctx.AsycudaSalesAllocations.Where(
                                            x => x != null && x.PreviousItem_Id == i.Item_Id)
                                        .OrderBy(x => x.AllocationId)
                                        .Include(x => x.EntryDataDetails)
                                        .Include(x => x.EntryDataDetails.EntryDataDetailsEx)
                                        .Include(x => x.PreviousDocumentItem).ToList();
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


        private async Task SaveEntryDataDetails(IEnumerable<EntryDataDetails> sales)
        {

            //await Task.Run(() => sales.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(itm =>
            //   {
                   using (var ctx = new AllocationDSContext(){StartTracking = false})
                   {
                       foreach (var itm in sales)
                       {
                           ctx.ApplyChanges(itm);
                           ctx.SaveChanges();
                           
                       }
                       
                   }
               //})).ConfigureAwait(false);


        }

        private async Task SaveSubItems(IEnumerable<SubItems> itms)
        {
            await Task.Run(() => itms.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(itm =>
            {
                using (var ctx = new AllocationDSContext())
                {
                    ctx.ApplyChanges(itm);
                    ctx.SaveChanges();
                }
            })).ConfigureAwait(false);
        }

        private async Task SaveAsycudaEntries(IEnumerable<xcuda_Item> IMAsycudaEntries)
        {
            await Task.Run(() =>
            {
                IMAsycudaEntries.Where(x => x.ChangeTracker != null).AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(itm =>
                {
                   // if(itm.DFQtyAllocated < 10000 && itm.DPQtyAllocated < 10000)
                        using (var ctx = new AllocationDSContext())
                    {
                        ctx.ApplyChanges(itm);
                        ctx.SaveChanges();
                    }
                });
            }).ConfigureAwait(false);
        }


        public List<xcuda_Item> GetAsycudaEntriesWithItemNumber(IEnumerable<xcuda_Item> IMAsycudaEntries, string attrib,
            string salesDescrip, List<string> itemNumber)
        {
            var alst = new List<xcuda_Item>();
            var taskLst = new List<Task>();
            if (attrib != null)
            {
                taskLst.Add(Task.Run(() =>
                {
                    alst.AddRange(IMAsycudaEntries.Where(x => //x.QtyAllocated != x.ItemQuantity &&
                                                                     x.SubItems.Any() == false &&
                                                                     (x.AttributeOnlyAllocation != null &&
                                                                      x.AttributeOnlyAllocation == true)
                                                                     && x.ItemNumber.ToLower().Replace(" ", "").Replace("-", "") == attrib.ToLower().Replace("-", "")));
                }));
            }
            taskLst.Add(Task.Run(() =>
            {
                alst.AddRange(IMAsycudaEntries.Where(x => //x.QtyAllocated != x.ItemQuantity &&
                                                                 x.SubItems.Any() == false &&
                                                                 (x.AttributeOnlyAllocation == null ||
                                                                  x.AttributeOnlyAllocation != true)
                                                                 &&
                                                                 salesDescrip.ToLower().Replace(" ", "").Replace("-", "")
                                                                     .Contains(x.ItemNumber.ToLower().Replace(" ", "").Replace("-", ""))));
            }));

            //item alias
            taskLst.Add(Task.Run(() =>
            {
                var aliasLst = InventoryAliasCache.Data.Where(x => salesDescrip.ToLower().Replace(" ", "").Replace("-", "")
                    .Contains(x.AliasName.ToLower().Replace(" ", "").Replace("-", "")));
                var alias = new StringBuilder();
                foreach (var itm in aliasLst)
                {
                    alias.Append(itm.ItemNumber + ",");
                }
                alst.AddRange(IMAsycudaEntries.Where(x => //x.QtyAllocated != x.ItemQuantity &&
                                                                 x.SubItems.Any() == false &&
                                                                 (x.AttributeOnlyAllocation == null ||
                                                                  x.AttributeOnlyAllocation != true)
                                                                 && alias.ToString().Contains(x.ItemNumber)));
            }));

            taskLst.Add(Task.Run(() =>
            {
                var sublst = IMAsycudaEntries.Where(x => x.SubItems.Any() == true
                                                         &&
                                                         x.SubItems.Any(z => itemNumber.Contains(z.ItemNumber.ToLower()))).ToList();
               if(sublst.Any()) alst.AddRange(sublst);//&& z.QtyAllocated != z.Quantity)
            }));
            Task.WhenAll(taskLst).Wait();
            return alst.Distinct().OrderBy(x => x.AsycudaDocument.RegistrationDate).ToList();
        }



        private async Task<List<xcuda_Item>> GetAllAsycudaEntries()
        {
            var alst = new List<xcuda_Item>();
            using (var ctx = new xcuda_ItemService())
            {
                alst.AddRange(ctx.Getxcuda_ItemByExpressionLst(
                    new List<string>()
                    {
                        (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? $"AsycudaDocument.RegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\""
                            : "AsycudaDocument.RegistrationDate >= \"1/1/2010\"") ,
                        "DoNotAllocate == null || DoNotAllocate != true",
                        "(AsycudaDocument.Extended_customs_procedure == \"7000\" || AsycudaDocument.Extended_customs_procedure == \"7400\" || AsycudaDocument.Extended_customs_procedure == \"7100\" || AsycudaDocument.Extended_customs_procedure == \"7500\" || AsycudaDocument.Extended_customs_procedure == \"9000\")",
                       //"SubItems.Count > 0",
                       // "AttributeOnlyAllocation == true"
                        //string.Format("EX.Precision_4.ToUpper() == \"{0}\"", attrib)
                    },
                    new List<string>() { "SubItems",
                        "AsycudaDocument",
                        "xcuda_Tarification.xcuda_HScode",
                        "xcuda_Tarification.xcuda_Supplementary_unit"    
                    }).Result.Distinct());//, "EX"

            }

            return alst;
        }

        private static async Task<List<EntryDataDetails>> GetSales()
        {
            List<EntryDataDetails> sales = null;
            using (var ctx = new EntryDataDetailsService())
            {
                sales = (await ctx.GetEntryDataDetailsByExpressionLst(new List<string>()
                {
                  // "EntryDataDetailsId == 85371",
                  (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? $"Sales.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\""
                      : "Sales.EntryDataDate >= \"1/1/2010\""),
                    "Sales.INVNumber != null",
                    "QtyAllocated != Quantity",
                    "DoNotAllocate != true"
                },
                    new List<string>() { "Sales", "AsycudaSalesAllocations" }).ConfigureAwait(false))
                    //.Where(x => Convert.ToDouble(x.QtyAllocated) != Convert.ToDouble(x.Quantity) && x.DoNotAllocate != true)
                    .OrderBy(x => x.Sales.EntryDataDate).ToList();//.Take(100)
            }
            return sales;
        }

        private static void GetSubItems(List<xcuda_Item> alst, EntryDataDetails salesDetails)
        {
            using (var ctx = new SubItemsService())
            {
                alst.AddRange(
                    ctx.GetSubItemsByExpressionLst(
                        new List<string>() {$"ItemNumber == \"{salesDetails.ItemNumber}\""}
                        , new List<string>() { "xcuda_Item", "xcuda_Item.AsycudaDocument", "xcuda_Item.xcuda_Tarification.xcuda_HScode" }).Result//"xcuda_Item.EX",
                        //.Where(y => y.ItemNumber == salesDetails.ItemNumber)
                        .Select(x => x.xcuda_Item).ToList());
            }
        }

        private static List<xcuda_Item> GetAsycudaEntriesWithItemNumber(string attrib, string salesDescrip)
        {
            var alst = new List<xcuda_Item>();


            // alst.AddRange(db.xcuda_Item.Where(x => x.PreviousDocumentItem.SubItems.Any(y => y.ItemNumber == salesDetails.InventoryItems.ItemNumber)));
            //match by attribute
            //alst = db.xcuda_Item.AsEnumerable().Where(x => salesDetails.ItemDescription.ToUpper().Split('|')[2] == x.ItemNumber.ToUpper()).ToList();
            using (var ctx = new xcuda_ItemService())
            {
                if (attrib != null)
                {
                    alst =
                        (ctx.Getxcuda_ItemByExpressionLst(
                            new List<string>()
                            {
                                "DoNotAllocate == null || DoNotAllocate != true",
                                "(AsycudaDocument.Extended_customs_procedure == \"7000\" ||AsycudaDocument.Extended_customs_procedure == \"7400\" || AsycudaDocument.Extended_customs_procedure == \"7100\" || AsycudaDocument.Extended_customs_procedure == \"7500\" || AsycudaDocument.Extended_customs_procedure == \"9000\")",
                                "SubItems.Count == 0",
                                "AttributeOnlyAllocation == true",
                                $"xcuda_Tarification.xcuda_HScode.Precision_4.ToUpper() == \"{attrib}\""
                            },
                            new List<string>() {"SubItems", "AsycudaDocument","xcuda_Tarification.xcuda_HScode" }).Result)//"EX"
                            .ToList();
                    //.Where(x => x.SubItems.Any() == false && (x.AttributeOnlyAllocation == true) && string.IsNullOrEmpty(salesDetails.ItemDescription.ToUpper().Split('|')[2]) == false && salesDetails.ItemDescription.ToUpper().Split('|')[2].ToUpper().Replace(" ", "") == x.ItemNumber.ToUpper().Replace(" ", "")).ToList();
                }
                alst.AddRange((ctx.Getxcuda_ItemByExpressionLst(new List<string>()
                {
                    "DoNotAllocate == null || DoNotAllocate != true",
                    "(AsycudaDocument.Extended_customs_procedure == \"7000\" || AsycudaDocument.Extended_customs_procedure == \"7400\" ||AsycudaDocument.Extended_customs_procedure == \"7100\" ||AsycudaDocument.Extended_customs_procedure == \"7500\" || AsycudaDocument.Extended_customs_procedure == \"9000\")",
                   "SubItems.Count == 0",
                    "AttributeOnlyAllocation == null || AttributeOnlyAllocation != true",
                    $"\"{salesDescrip}\".Contains(xcuda_Tarification.xcuda_HScode.Precision_4.ToUpper())"
                },
                    new List<string>() { "SubItems", "AsycudaDocument", "xcuda_Tarification.xcuda_HScode" })).Result);//"EX"
                //.Where(x => x.SubItems.Any() == false 
                //            && (x.AttributeOnlyAllocation == null || x.AttributeOnlyAllocation != true) 
                //            && salesDetails.ItemDescription.ToUpper().Replace(" ", "").Contains(x.ItemNumber.ToUpper().Replace(" ", ""))));
            }
            return alst;
        }

        private async Task SaveItemLst(ItemSet itm)
        {
                foreach (var item in itm.SalesList)
                {
                    await SaveEntryDataDetails(item).ConfigureAwait(false);
                }
           
           
                foreach (var item in itm.EntriesList)
                {
                   await SaveXcuda_Item(item).ConfigureAwait(false);
                }
        }

        private async Task<ConcurrentDictionary<string, ItemSet>> MatchSalestoAsycudaEntriesOnItemNumber(
            int applicationSettingsId)
        {
            try
            {
                var asycudaEntries = await GetAsycudaEntriesWithItemNumber(applicationSettingsId).ConfigureAwait(false);

                var saleslst = await GetSaleslstWithItemNumber(applicationSettingsId).ConfigureAwait(false);

                var adjlst = await GetAdjustmentslstWithItemNumber(applicationSettingsId).ConfigureAwait(false);
                saleslst.AddRange(adjlst);

                var itmLst = CreateItemSetsWithItemNumbers(saleslst, asycudaEntries);

                return itmLst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        //private async Task<ConcurrentDictionary<string, ItemSet>> MatchSalestoAsycudaEntriesOnDescription()
        //{
        //    List<ItemEntries> asycudaEntries = new List<ItemEntries>();
        //    asycudaEntries.AddRange(await GetAsycudaEntriesWithDescription().ConfigureAwait(false));
        //    asycudaEntries.AddRange(await GetAsycudaEntriesWithItemNumber().ConfigureAwait(false));

        //    List<ItemSales> saleslst = new List<ItemSales>();
        //    saleslst.AddRange(await GetSaleslstWithDescription().ConfigureAwait(false));
        //    saleslst.AddRange(await GetSaleslstWithItemNumber().ConfigureAwait(false));

        //    var itmLst = CreateItemSetsWithDescription(saleslst, asycudaEntries);

        //    return itmLst; //.Where(x => x.ItemNumber == "OC1719907");
        //}

        private static ConcurrentDictionary<string,ItemSet> CreateItemSetsWithItemNumbers(IEnumerable<ItemSales> saleslst, IEnumerable<ItemEntries> asycudaEntries)
        {

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


        private static async Task<IEnumerable<ItemEntries>> GetAsycudaEntriesWithItemNumber(int applicationSettingsId)
        {
            StatusModel.Timer("Getting Data - Asycuda Entries...");
            //string itmnumber = "WMHP24-72";
            IEnumerable<ItemEntries> asycudaEntries = null;
            using (var ctx = new AllocationDSContext(){StartTracking = false})
            {
                var lst = ctx.xcuda_Item.Include(x => x.AsycudaDocument)
                    .Include(x => x.xcuda_Tarification.xcuda_HScode)
                    .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
                    .Include(x => x.SubItems)
                    .Include("EntryPreviousItems.xcuda_PreviousItem")
                    .Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true) &&
                                (x.AsycudaDocument.Extended_customs_procedure == "7000" || x.AsycudaDocument.Extended_customs_procedure == "7400" || x.AsycudaDocument.Extended_customs_procedure == "7100" || x.AsycudaDocument.Extended_customs_procedure == "7500" || x.AsycudaDocument.Extended_customs_procedure == "9000") &&
                                // x.WarehouseError == null && 
                                 (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
                                 x.AsycudaDocument.DoNotAllocate != true )
                    .Where(x => x.AsycudaDocument.AssessmentDate >= (BaseDataModel.Instance.CurrentApplicationSettings
                                    .OpeningStockDate ?? DateTime.MinValue.Date))
                    .OrderBy(x => x.LineNumber)
                    .ToList();

                // var res2 = lst.Where(x => x.ItemNumber == "PRM/84101");

                asycudaEntries = from s in lst.Where(x => x.ItemNumber != null)
                   // .Where(x => x.ItemNumber == itmnumber)
                    //       .Where(x => x.AsycudaDocument.CNumber != null).AsEnumerable()
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

           // var res = asycudaEntries.Where(x => x.Key == "PRM/84101");
            return asycudaEntries;
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
                    new Dictionary<string, string>() { { "AsycudaDocument", (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? $"AssessmentDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && "
                                                                                : "") + "(CNumber != null || IsManuallyAssessed == true) && (Extended_customs_procedure == \"7000\" ||Extended_customs_procedure == \"7400\" ||Extended_customs_procedure == \"7100\" ||Extended_customs_procedure == \"7500\" || Extended_customs_procedure == \"9000\") && DoNotAllocate != true" } }
                    , new List<string>() { "AsycudaDocument",
                        "xcuda_Tarification.xcuda_HScode", "xcuda_Tarification.xcuda_Supplementary_unit","SubItems", "xcuda_Goods_description",
                    }).ConfigureAwait(false);//"EX"


                asycudaEntries = from s in lst.Where(x => x.xcuda_Tarification.xcuda_HScode.Precision_4 != null)
                     //.Where(x => x.ItemDescription == "Hardener-Resin 'A' Slow .44Pt")
                    //       .Where(x => x.AsycudaDocument.CNumber != null).AsEnumerable()
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

        private static async Task<List<ItemSales>> GetSaleslstWithItemNumber(int applicationSettingsId)
        {
            StatusModel.Timer("Getting Data - Sales Entries...");

            IEnumerable<ItemSales> saleslst = null;
            using (var ctx = new EntryDataDetailsService())
            {
                var salesData =

                await
                        ctx.GetEntryDataDetailsByExpressionNav(//"ItemNumber == \"PNW/30-53700\" &&" +
                                                                (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? $"Sales.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && "
                                                                    : "") +

                                                               "QtyAllocated != Quantity " +
                                                               $"&& Sales.ApplicationSettingsId == {applicationSettingsId} " +
                                                               //"&& Cost > 0 " + --------Cost don't matter in allocations because it comes from previous doc
                                                               "&& DoNotAllocate != true", new Dictionary<string, string>()
                                                               {
                                                                   { "Sales", "INVNumber != null" }
                                                               }, new List<string>() { "Sales", "AsycudaSalesAllocations" },false)
                            .ConfigureAwait(false);
                saleslst = from d in salesData
                    group d by d.ItemNumber.ToUpper().Trim()
                    into g
                    select
                        new ItemSales
                        {
                            Key = g.Key,
                            SalesList = g.Where(xy => xy != null & xy.Sales != null).OrderBy(x => x.Sales.EntryDataDate).ThenBy(x => x.EntryDataId).ToList()
                        };
            }
            return saleslst.ToList();
        }

        private static async Task<List<ItemSales>> GetAdjustmentslstWithItemNumber(int applicationSettingsId)
        {
            StatusModel.Timer("Getting Data - Adjustments Entries...");

            List<ItemSales> adjlst = null;
            using (var ctx = new EntryDataDetailsService())
            {
                var salesData =

                await
                        ctx.GetEntryDataDetailsByExpressionNav(//"ItemNumber == \"AAA/13576\" &&" +
                                                                (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? $"Adjustments.EntryDataDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\" && "
                                                                    : "") +
                                                               "QtyAllocated != Quantity && " +
                                                                $"Adjustments.ApplicationSettingsId == {applicationSettingsId} && " +
                                                                $"Adjustments.Type == \"ADJ\" && " + /// Only Adjustments not DIS that should have CNumber to get matched
                                                                "((CNumber == null && PreviousInvoiceNumber == null) ||" +
                                                                " ((CNumber != null || PreviousInvoiceNumber != null) && QtyAllocated == 0))" + //trying to capture unallocated adjustments
                                                                " && (ReceivedQty - InvoiceQty) < 0 && (EffectiveDate != null || " + (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue ? $"EffectiveDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"": "") +  ")" +
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

                    if (cAsycudaItm.AsycudaDocument.DocumentType == "IM7" &&
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

                        if (cAsycudaItm.AsycudaDocument.DocumentType == "IM7" && (cAsycudaItm.AsycudaDocument.AssessmentDate > saleitm.Sales.EntryDataDate))
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
            if (cAsycudaItm.QtyAllocated == 0)
            {
                cAsycudaItm.DFQtyAllocated = 0;
                cAsycudaItm.DPQtyAllocated = 0;
            }

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
