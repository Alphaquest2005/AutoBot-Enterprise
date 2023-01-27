using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using Core.Common.Extensions;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.EF6;
using WaterNut.DataSpace;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using xcuda_Item = AdjustmentQS.Business.Entities.xcuda_Item;

namespace AdjustmentQS.Business.Services
{
    public class AutoMatchUtils
    {
        private static List<AsycudaDocumentItem> _itemCache;
        private static List<InventoryItemAliasX> _itemAliaCache;

        private static List<InventoryItemAliasX> ItemAliasCache
        {
            get
            {
                if (_itemAliaCache != null) return _itemAliaCache;
                using (var ctx = new CoreEntitiesContext())
                {
                    _itemAliaCache = ctx.InventoryItemAliasX
                        .Include(x => x.InventoryItemsEx)
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                }

                return _itemAliaCache;
            }
        }

        private static List<AsycudaDocumentItem> AsycudaDocumentItemCache
        {
            get
            {
                try
                {
                    if (_itemCache != null) return _itemCache;
                    using (var ctx = new CoreEntitiesContext())
                    {
                        ctx.Database.CommandTimeout = 0;
                        _itemCache = ctx.AsycudaDocumentItems
                            .Include(x => x.AsycudaDocument)
                            .Include(x => x.AsycudaDocumentItemEntryDataDetails)
                            .Where(x => x.AsycudaDocument.ApplicationSettingsId == BaseDataModel.Instance
                                .CurrentApplicationSettings.ApplicationSettingsId)
                            .Where(x => (x.AsycudaDocument.CNumber != null ||
                                         x.AsycudaDocument.IsManuallyAssessed == true) &&
                                        (x.AsycudaDocument.Customs_Procedure.CustomsOperations.Name == "Warehouse") &&
                                        // x.WarehouseError == null && 
                                        (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
                                        x.AsycudaDocument.DoNotAllocate != true).ToList();
                    }

                    return _itemCache;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

        }

        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting)
        {
            try
            {
                _itemCache = null;


                var rawData = GetAllDiscrepancyDetails(applicationSettingsId, overwriteExisting);//.Where(x => x.EntryDataId == "Asycuda-C#33687-24").ToList();
                 if (!rawData.Any()) return;


                 var data = rawData.Select(x => (Item: (x.ItemNumber, x.InventoryItemId), xSale: x))
                     .GroupBy(x => x.Item)
                     .Select(x => (Key: x.Key, Value: x.Select(i => i.xSale).ToList()))
                     .ToList();

                 var itemGrps = Utils.CreateItemSet(data).Partition(100);

               

                itemGrps.SelectMany(x => x.ToList())
                    .AsParallel()
                    .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                             BaseDataModel.Instance.ResourcePercentage))
                    .ForEach(async lst => await AutoMatch(applicationSettingsId, lst).ConfigureAwait(false));
                    
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static async Task AutoMatch(int applicationSettingsId, KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<AdjustmentDetail> Value)>> lst)
        {
            AllocationsModel.Instance
                .ClearDocSetAllocations(
                    lst.Value.Select(x => $"'{x.Key.ItemNumber}'").Aggregate((o, n) => $"{o},{n}"))
                .Wait();

            AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance
                .CurrentApplicationSettings);


            await DoAutoMatch(applicationSettingsId, lst.Value.SelectMany(v => v.Value).ToList()).ConfigureAwait(false);


            new AdjustmentShortService().AutoMatchUtils
                .ProcessDISErrorsForAllocation(
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                    lst.Value
                        .SelectMany(g => g.Value)
                        .Select(v => v).Select(x => $"{x.EntryDataDetailsId}-{x.ItemNumber}")
                        .Aggregate((o, n) => $"{o},{n}")).Wait();
        }

        public async Task MatchToAsycudaItem(int entryDataDetailId, int itemId)
        {
            try
            {
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    var os = ctx.AdjustmentDetails.First(x => x.EntryDataDetailsId == entryDataDetailId);
                    var ed = ctx.EntryDataDetails.First(x => x.EntryDataDetailsId == entryDataDetailId);
                    var item = AsycudaDocumentItemCache.FirstOrDefault(x => x.Item_Id == itemId);
                    MatchToAsycudaItem(os, new List<AsycudaDocumentItem>() { item }, ed);
                    ed.Status = null;
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<AdjustmentDetail> GetAllDiscrepancyDetails(int applicationSettingsId, bool overwriteExisting)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                ctx.Database.CommandTimeout = 10;

                var lst = ctx.AdjustmentDetails.AsNoTracking()
                    .Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => x.SystemDocumentSet != null)
                    .Where(x => x.Type == "DIS")
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => overwriteExisting
                        ? x != null
                        : x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .Where(x => !x.ShortAllocations.Any())
                    .OrderBy(x => x.EntryDataDetailsId)
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();
                return lst;
            }
        }

        public async Task AutoMatchDocSet(int applicationSettingsId, int docSetId)
        {
            try
            {

            

                var lst = GetDocSetAdjustmentDetails(applicationSettingsId, docSetId);

                await DoAutoMatch(applicationSettingsId, lst);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<AdjustmentDetail> GetDocSetAdjustmentDetails(int applicationSettingsId, int docSetId)
        {
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;


                var lst = ctx.AdjustmentDetails
                    //.Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => x.AsycudaDocumentSetId == docSetId)
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .OrderBy(x => x.EntryDataDetailsId)
                    .ToList();
                return lst;
            }
        }

 

        public static async Task DoAutoMatch(int applicationSettingsId, List<AdjustmentDetail> lst)
        {
            try
            {



                if (!lst.Any()) return;
                StatusModel.StartStatusUpdate("Matching Shorts To Asycuda Entries", lst.Count());

                var edLst = lst
                    .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                    .AsParallel()
                    .Select( s =>  AutoMatchItemNumber(s).Result)
                    .ToList();
               

                SetMinimumEffectDate(edLst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<EntryDataDetail> AutoMatchItemNumber(AdjustmentDetail s)
        {

            
            try
            {

                var ed = GetEntryDataDetail(s);

                var processors = new List<IAutoMatchProcessor>()
                {
                    new PreviousInvoiceNumberMatcher(s, ed),
                    new CNumberMatcher(s, ed),
                    new EffectiveDatefProcessor(s, ed),
                    new MissingCostProcessor(s, ed),

                };

                
                processors.Where(x => x.IsApplicable(s, ed))
                    .ForEach(async x => await x.Execute().ConfigureAwait(false));


                SaveEntryDataDetails(ed);

                return ed;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private static void SaveEntryDataDetails(EntryDataDetail ed)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                ctx.Database.CommandTimeout = 10;
                ctx.EntryDataDetails.Attach(ed);
                ctx.ApplyChanges(ed);
                ctx.SaveChanges();
            }
        }

        private static EntryDataDetail GetEntryDataDetail(AdjustmentDetail s)
        {
            var ed = new AdjustmentQSContext(){StartTracking = true}.EntryDataDetails.Include(x => x.AdjustmentEx)
                .First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);

            ed.Comment = null;
            ed.EffectiveDate = null;
            return ed;
        }

    

        private static void SetMinimumEffectDate(List<EntryDataDetail> edLst)
        {
            DateTime? minEffectiveDate;
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;
                minEffectiveDate = edLst.Min(x => x.EffectiveDate)
                                   ?? edLst.Where(x => x.AdjustmentEx != null).Min(x => x.AdjustmentEx.InvoiceDate);

                foreach (var ed in edLst.Where(x => x.EffectiveDate == null))
                {
                    ed.EffectiveDate = minEffectiveDate;
                    ed.Status = null;
                }

                ctx.SaveChanges();
                StatusModel.StopStatusUpdate();
            }
        }

        public static void MatchToAsycudaDocument(AsycudaDocument asycudaDocument, EntryDataDetail ed)
        {
            if (asycudaDocument != null)
            {
                ed.EffectiveDate = asycudaDocument.AssessmentDate;
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    //PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = asycudaDocument.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                });
            }
            else
            {
                ed.Status = "No Asycuda Entry Found";
            }
        }

        public static async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
            string sPreviousInvoiceNumber, string sEntryDataId)
        {
            try
            {

                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache.Where( x => x.PreviousInvoiceNumber != null
                                                                               && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                               && x.AsycudaDocument.ImportComplete == true
                                                                               && x.ApplicationSettingsId == applicationSettingsId &&
                                                                               (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                                                                   || sEntryDataId.ToUpper().Trim() == x.PreviousInvoiceNumber.ToUpper().Trim()
                                                                               ));// contains is too vague
                var res = aItem.ToList();


                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
            string sPreviousInvoiceNumber, string sEntryDataId, string sItemNumber)
        {
            try
            {

                // get document item in cnumber
                var aItem = Enumerable.Where<AsycudaDocumentItem>(AsycudaDocumentItemCache, x => x.PreviousInvoiceNumber != null
                                                                               && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                               && x.AsycudaDocument.ImportComplete == true
                                                                               && x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim() &&
                                                                               (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                                                                   || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())));
                var res = aItem.ToList();
                var alias = Enumerable.Where<InventoryItemAliasX>(ItemAliasCache, x => x.InventoryItemsEx.ApplicationSettingsId == applicationSettingsId &&
                                                                     x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim())
                    .Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = Enumerable.Where<AsycudaDocumentItem>(AsycudaDocumentItemCache, x => x.PreviousInvoiceNumber != null
                                                                            && alias.Contains(x.ItemNumber)
                                                                            && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                            && x.AsycudaDocument.ImportComplete == true
                                                                            && (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                                                                || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())))
                    .ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static AsycudaDocument GetAsycudaDocumentInCNumber(int applicationSettingsId, string cNumber)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                try
                {


                    // get document item in cnumber
                    var clst = GetCNumbersFromString(cNumber);
                    var res = Queryable.FirstOrDefault(ctx.AsycudaDocuments
                            .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                            .Where(x => (x.CNumber != null || x.IsManuallyAssessed == true) &&
                                        (x.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Warehouse) &&
                                        // x.WarehouseError == null && 
                                        (x.Cancelled == null || x.Cancelled == false) &&
                                        x.DoNotAllocate != true)
                            .OrderByDescending(x => x.AssessmentDate), x => clst.Contains(x.CNumber));
                    return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public static DateTime? MatchToAsycudaItem(AdjustmentDetail os, List<AsycudaDocumentItem> alst, EntryDataDetail ed)
        {
            //ed.Status = null;
            //ed.QtyAllocated = 0;
            DateTime? minEffectiveDate = null;
            var remainingShortQty = os.InvoiceQty.GetValueOrDefault() - os.ReceivedQty.GetValueOrDefault();

            if (!alst.Any())
            {
                //Mark here cuz Allocations don't do Discrepancies only Adjustments
                ed.Status = "No Asycuda Item Found";
                return minEffectiveDate;
            }

            /// took PiQuantity out because then entries can exist already and this just preventing it from relinking
            /// 

            ed.EffectiveDate = alst.FirstOrDefault().AsycudaDocument.AssessmentDate;
            minEffectiveDate = ed.EffectiveDate;
            if (alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.DFQtyAllocated.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error
                ed.Status = "Insufficent Quantities";
                return minEffectiveDate;
            }

            if ((ed.IsReconciled??false) == false && alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.PiQuantity.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                var existingExecution = new AdjustmentQSContext().AsycudaDocumentItemEntryDataDetails
                    .FirstOrDefault(x => x.EntryDataDetailsId == ed.EntryDataDetailsId);

                if (existingExecution == null)
                {
                    ed.Status = "PiQuantity Already eXed-Out";
                    return minEffectiveDate;
                }

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error

            }


            if (remainingShortQty < 0)
            {
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = alst.First().AsycudaDocumentId.GetValueOrDefault(),
                    TrackingState = TrackingState.Added

                });
                return minEffectiveDate;
            } // if overs just mark and return

            foreach (var aItem in alst)
            {
                var pitm = new AdjustmentQSContext().xcuda_Item
                    //   .Include("AsycudaSalesAllocations.EntryDataDetail.AdjustmentEx")
                    .First(x => x.Item_Id == aItem.Item_Id);
                //pitm.DFQtyAllocated = 0;

                //if (Math.Abs(pitm.AsycudaSalesAllocations.Where(x => x.EntryDataDetailsId == os.EntryDataDetailsId)
                //    .Select(x => x.QtyAllocated).Sum() - os.Quantity) < 0.001)
                //    return;

                var asycudaQty =
                    Convert.ToDouble(aItem.ItemQuantity.GetValueOrDefault() - pitm.DFQtyAllocated);///??TODO: why only dfq?

                if (asycudaQty <= 0) continue;

                var osa = SaveAsycudaSalesAllocation(os, aItem);
                if (asycudaQty >= remainingShortQty)
                {

                    osa.QtyAllocated = remainingShortQty;
                    ed.QtyAllocated += remainingShortQty;
                    pitm.DFQtyAllocated += remainingShortQty;
                    aItem.DFQtyAllocated += remainingShortQty;
                    SaveAsycudaSalesAllocation(osa);
                    SaveAsycudaDocumentItem(pitm);
                    break;
                }
                else
                {

                    osa.QtyAllocated = asycudaQty;
                    ed.QtyAllocated += asycudaQty;
                    pitm.DFQtyAllocated += asycudaQty;
                    aItem.DFQtyAllocated += asycudaQty;
                    remainingShortQty -= asycudaQty;
                    SaveAsycudaSalesAllocation(osa);
                    SaveAsycudaDocumentItem(pitm);
                }

            }
            

            return minEffectiveDate;

        }

        private static void SaveAsycudaDocumentItem(xcuda_Item pitm)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                ctx.Database.CommandTimeout = 10;
                var res = ctx.xcuda_Item.First(x => x.Item_Id == pitm.Item_Id);
                res.DFQtyAllocated = pitm.DFQtyAllocated;
                res.DPQtyAllocated = pitm.DPQtyAllocated;
                ctx.SaveChanges();
            }
        }

        private static void SaveAsycudaSalesAllocation(AsycudaSalesAllocation osa)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                ctx.Database.CommandTimeout = 10;
                var res = ctx.AsycudaSalesAllocations.First(x => x.AllocationId == osa.AllocationId);
                res.QtyAllocated = osa.QtyAllocated;
                ctx.SaveChanges();
            }

        }

        private static AsycudaSalesAllocation SaveAsycudaSalesAllocation(AdjustmentDetail os, AsycudaDocumentItem aItem)
        {
            using (var ctx = new AdjustmentQSContext())
            {
                ctx.Database.CommandTimeout = 10;
                var osa = new AsycudaSalesAllocation(true)
                {
                    PreviousItem_Id = aItem.Item_Id,
                    EntryDataDetailsId = os.EntryDataDetailsId,
                    //Status = "Short Shipped",
                    TrackingState = TrackingState.Added,
                };
                ctx.AsycudaSalesAllocations.Add(osa);
                ctx.SaveChanges();
                return osa;
            }
        }

        public static async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumber(string cNumber,
            int? previousCLineNumber, string itemNumber)
        {

            try
            {

                var clst = GetCNumbersFromString(cNumber);
                // get document item in cnumber
                var aItem = Enumerable.Where<AsycudaDocumentItem>(AsycudaDocumentItemCache, x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                                                               && x.AsycudaDocument.CNumber != null
                                                                               && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                                                               && x.AsycudaDocument.ImportComplete == true
                                                                               && x.ItemNumber == itemNumber && Enumerable.Any<string>(clst, z => z == x.AsycudaDocument.CNumber)
                                                                               && x.LineNumber == (previousCLineNumber == null ? x.LineNumber : previousCLineNumber.Value.ToString()));
                var res = aItem.ToList();
                var alias = Enumerable.Where<InventoryItemAliasX>(ItemAliasCache, x => x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = Enumerable.Where<AsycudaDocumentItem>(AsycudaDocumentItemCache, x => x.AsycudaDocument.CNumber != null
                                                                            && x.AsycudaDocument.DocumentType == "IM7"
                                                                            && x.AsycudaDocument.ImportComplete == true
                                                                            && alias.Contains(x.ItemNumber) && cNumber.Contains(x.AsycudaDocument.CNumber)).ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public static async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumberReference(int applicationSettingsId,
            string cNumber, string itemNumber)
        {

            try
            {
                List<string> cnumberlst = GetCNumbersFromString(cNumber);
                var doc = AsycudaDocumentItemCache.FirstOrDefault(x => x.CNumber != null && cnumberlst.Contains(x.CNumber));
                
                if (doc == null) return new List<AsycudaDocumentItem>();
                var docref = doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal) > 0
                    ? doc.ReferenceNumber.Substring(0, doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal))
                    : doc.ReferenceNumber;


                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.ItemNumber == itemNumber
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref));
                var res = aItem.ToList();
                var alias = ItemAliasCache.Where(x => x.ApplicationSettingsId == applicationSettingsId && x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => alias.Contains(x.ItemNumber)
                                && x.AsycudaDocument.DocumentType == "IM7"
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref)).ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private static List<string> GetCNumbersFromString(string cNumber)
        {
            var str = cNumber.ToUpper().Replace("C", "");
            var res = str.Split(',', ' ').ToList();
            return res;
        }

        public async Task ProcessDISErrorsForAllocation(int applicationSettingsId, string res)
        {
            try
            {
                // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 60;



                    var lst = ctx.TODO_PreDiscrepancyErrors.ToList()
                        .OrderBy(x => x.EntryDataDetailsId)
                        .DistinctBy(x => x.EntryDataDetailsId)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                    && res.Contains(x.EntryDataDetailsId.ToString()))
                        //.Where(x => x.ItemNumber == "HEL/003361001")

                        .ToList();
                    // looking for 'INT/YBA473/3GL'

                    ProcessDISErrorsForAllocation(lst, ctx);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static void ProcessDISErrorsForAllocation(List<TODO_PreDiscrepancyErrors> lst, AdjustmentQSContext ctx)
        {
            StatusModel.StartStatusUpdate("Preparing Discrepancy Errors for Re-Allocation", lst.Count());
            foreach (var s in lst.Where(x =>  x.IsReconciled != true))//x.ReceivedQty > x.InvoiceQty &&
            {
                var ed = ctx.EntryDataDetails.Include(x => x.AdjustmentEx).First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                ed.EffectiveDate = ed.AdjustmentEx.InvoiceDate;// BaseDataModel.CurrentSalesInfo().Item2; reset to invoicedate because this just wrong duh make sense
                ed.QtyAllocated = 0;
                ed.Comment = $@"DISERROR:{s.Status}";
                ed.Status = null;
                ctx.Database.ExecuteSqlCommand(
                    $"delete from AsycudaSalesAllocations where EntryDataDetailsId = {ed.EntryDataDetailsId}");
            }

            ctx.SaveChanges();
        }

        public async Task AutoMatchItems(int applicationSettingsId, string strLst)
        {
            try
            {

                var lst = GetSelectedAdjustmentDetails(applicationSettingsId, strLst);

                await DoAutoMatch(applicationSettingsId, lst);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<AdjustmentDetail> GetSelectedAdjustmentDetails(int applicationSettingsId, string strLst)
        {
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;

                var lst = ctx.AdjustmentDetails
                    //.Include(x => x.AdjustmentEx)
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                && strLst.Contains(x.EntryDataDetailsId.ToString()))
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .OrderBy(x => x.EntryDataDetailsId)
                    .ToList();
                return lst;
            }
        }
    }
}