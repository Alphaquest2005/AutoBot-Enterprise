using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using Core.Common.Extensions;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using MoreLinq;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Utils.MatchingToAsycudaItem;
using WaterNut.Business.Services.Utils.ProcessingDISErrorsForAllocations;
using WaterNut.DataSpace;

namespace AdjustmentQS.Business.Services
{
    public class AutoMatchProcessor
    {
        private static List<AsycudaDocumentItem> _itemCache;
        private readonly ProcessDISErrorsForAllocation _processDisErrorsForAllocation = new ProcessDISErrorsForAllocation();

        public static List<AsycudaDocumentItem> AsycudaDocumentItemCache
        {
            get
            {
                lock (AllocationsBaseModel.Identity)
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
                                            (x.AsycudaDocument.Customs_Procedure.CustomsOperations.Name ==
                                             "Warehouse") &&
                                            // x.WarehouseError == null && 
                                            (x.AsycudaDocument.Cancelled == null ||
                                             x.AsycudaDocument.Cancelled == false) &&
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
        }

        public ProcessDISErrorsForAllocation ProcessDisErrorsForAllocation
        {
            get { return _processDisErrorsForAllocation; }
        }

        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting, string lst)
        {
            var itemSets = BaseDataModel.GetItemSets(lst);
            await AutoMatch(applicationSettingsId, overwriteExisting, itemSets).ConfigureAwait(false);
        }

        public async Task AutoMatch( int applicationSettingsId, bool overwriteExisting, List<List<(string ItemNumber, int InventoryItemId)>> itemSets)
        {
            try
            {
                AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance
                    .CurrentApplicationSettings);

                _itemCache = null;

                var rawData = ParallelEnumerable.Select<List<(string ItemNumber, int InventoryItemId)>, List<AdjustmentDetail>>(itemSets
                        .AsParallel(), x => GetAllDiscrepancyDetails(overwriteExisting, x))
                    .SelectMany(x => x.ToList())
                    .ToList();//.Where(x => x.EntryDataId == "Asycuda-C#33687-24").ToList();

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
                    .ForAll(async l => await AutoMatch(applicationSettingsId, l).ConfigureAwait(false));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<AdjustmentDetail> GetAllDiscrepancyDetails(bool overwriteExisting, List<(string ItemNumber, int InventoryItemId)> x)
        {
            return AllocationsBaseModel.isDBMem
                ? new GetAllDiscrepancyDetails().Execute(x, overwriteExisting)
                : new GetAllDiscrepancyDetailsMem().Execute(x, overwriteExisting);
        }

        private static async Task AutoMatch(int applicationSettingsId, KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<AdjustmentDetail> Value)>> lst)
        {
            AllocationsModel.Instance
                .ClearDocSetAllocations(lst.Value.Select(x => $"'{x.Key.ItemNumber}'").Aggregate((o, n) => $"{o},{n}"))
                .Wait();

           
            await DoAutoMatch(applicationSettingsId, lst.Value.SelectMany(v => v.Value).ToList()).ConfigureAwait(false);


            ProcessDISErrorsForAllocation(lst);
        }

        private static void ProcessDISErrorsForAllocation(KeyValuePair<int, List<((string ItemNumber, int InventoryItemId) Key, List<AdjustmentDetail> Value)>> lst)
        {
            new AdjustmentShortService().AutoMatchUtils.AutoMatchProcessor.ProcessDisErrorsForAllocation
                .Execute(
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
                    new MatchToAsycudaItemSelector().Execute(os, new List<AsycudaDocumentItem>() { item }, ed);
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

                var edLst = ParallelEnumerable.Select<AdjustmentDetail, EntryDataDetail>(lst
                        .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .AsParallel(), s =>  AutoMatchItemNumber(s).Result)
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
    }
}