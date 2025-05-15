using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AdjustmentQS.Business.Services;
using AllocationDS.Business.Entities;
using Core.Common.Extensions;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities.EF6;
using WaterNut.Business.Services.Utils.ProcessingDISErrorsForAllocations;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Utils.AutoMatching
{
    public class AutoMatchSingleSetBasedProcessor
    {
      

        public Task AutoMatch(int applicationSettingsId, bool overwriteExisting, string lst)
        {
            var itemSets = BaseDataModel.GetItemSets(lst);
            itemSets.ForEach(async x => await AutoMatch(applicationSettingsId, overwriteExisting, x).ConfigureAwait(false));
            return Task.CompletedTask;
        }

        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting, List<(string ItemNumber, int InventoryItemId)> itemSet)
        {
            try
            {
                //AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance
                //    .CurrentApplicationSettings);

                var rawData =
                    GetAllDiscrepancyDetails(overwriteExisting, itemSet);
                             //.Where(x => x.EntryDataId == "Asycuda-C#33687-24").ToList();

                if (!rawData.Any()) return;

                ClearDocSetAllocations(rawData, itemSet);

                var matches = await DoAutoMatch(applicationSettingsId, rawData).ConfigureAwait(false);

                SaveEntryDataDetails(matches.DistinctBy(x => x.EntryDataDetailsId).ToList());

                var processedErrors = await ProcessDISErrorsForAllocation(rawData).ConfigureAwait(false);

                var noDataErrors = await ProcessNoDataDISForAllocation(matches.Where(x => Math.Abs(x.QtyAllocated - x.Quantity) > 0).ExceptBy(processedErrors,x => x.EntryDataDetailsId).ToList()).ConfigureAwait(false);

                SaveEntryDataDetails(noDataErrors.DistinctBy(x => x.EntryDataDetailsId).ToList());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public void ClearDocSetAllocations(List<AdjustmentDetail> rawData, List<(string ItemNumber, int InventoryItemId)> itemSet)
        {
       
            try
            {
                var entryDataDetailsLst =
                    rawData.Select(x => $"{x.EntryDataDetailsId}").Aggregate((o, n) => $"{o},{n}");

                var xcudaItemLst =
                    rawData.Select(x => $"{x.EntryDataDetailsId}").Aggregate((o, n) => $"{o},{n}");

                var itemNumbers = rawData.Select(x => $"'{x.ItemNumber}'").Aggregate((o, n) => $"{o},{n}");

                StatusModel.Timer("Clear DocSet Existing Allocations");

                using (var ctx = new AllocationDSContext())
                {
                    var sql = $@" DELETE FROM AdjustmentOversAllocations
							FROM    AdjustmentOversAllocations INNER JOIN
											 EntryDataDetails ON AdjustmentOversAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
											 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId 
                            WHERE (EntryDataDetails.EntryDataDetailsId in ({entryDataDetailsLst}))

                            DELETE FROM AsycudaSalesAllocations
                            FROM    AsycudaSalesAllocations INNER JOIN
                                             EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId 
                            WHERE (EntryDataDetails.EntryDataDetailsId in ({entryDataDetailsLst}))

                            UPDATE xcuda_Item
                            SET         DFQtyAllocated = 0, DPQtyAllocated = 0
                            FROM    xcuda_Item 
                            Where Item_Id in ({xcudaItemLst})


                            UPDATE EntryDataDetails
                            SET         QtyAllocated = 0, Status = NULL--, EffectiveDate = NULL
                            FROM    EntryDataDetails INNER JOIN
                                             EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId 
                            WHERE (EntryDataDetails.EntryDataDetailsId in ({entryDataDetailsLst}))


                            UPDATE xcuda_PreviousItem
                            SET         QtyAllocated = 0
                            FROM  xcuda_PreviousItem
                            Where PreviousItem_Id in ({xcudaItemLst})


                            UPDATE SubItems
                            SET         QtyAllocated = 0
                            FROM    xcuda_Item INNER JOIN
                                             SubItems ON xcuda_Item.Item_Id = SubItems.Item_Id INNER JOIN
                                             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                                                 (SELECT InventoryItemsWithAlias.ItemNumber
                                                  FROM     InventoryItems INNER JOIN
                                                                   InventoryItemsWithAlias ON InventoryItems.Id = InventoryItemsWithAlias.Id
                                                  WHERE  (InventoryItems.ItemNumber IN ({itemNumbers}))) AS items ON xcuda_HScode.Precision_4 = items.ItemNumber";



                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
                    ///.ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private void SaveEntryDataDetails(List<EntryDataDetail> matches)
        {
            try
            {
                var overAllocations = matches.SelectMany(x => x.AdjustmentOversAllocations).ToList();
                if (overAllocations.Any())
                {
                    new AdjustmentQSContext().BulkMerge(overAllocations);
                }

                var shortAllocations = matches.SelectMany(x => x.AsycudaSalesAllocations).ToList();
                if (shortAllocations.Any())
                {
                    new AdjustmentQSContext().BulkMerge(shortAllocations);
                }

                new AdjustmentQSContext().BulkMerge(matches);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public List<AdjustmentDetail> GetAllDiscrepancyDetails(bool overwriteExisting, List<(string ItemNumber, int InventoryItemId)> x)
        {
            return AllocationsBaseModel.isDBMem
                ? new GetAllDiscrepancyDetails().Execute(x, overwriteExisting)
                : new GetAllDiscrepancyDetailsMem().Execute(x, overwriteExisting);
        }

       
        private async Task<List<EntryDataDetail>> ProcessDISErrorsForAllocation(List<AdjustmentDetail> lst)
        {
            // Apply ConfigureAwait to the Task before awaiting
            var errors = await new ProcessDISErrorsForAllocation().Execute( // Move ConfigureAwait here
                             BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                             lst
                                 .Select(v => v).Select(x => $"{x.EntryDataDetailsId}")
                                 .Aggregate((o, n) => $"{o},{n}")
                         ).ConfigureAwait(false); // Correct placement


            //var errors = await  new ProcessDISErrorsForAllocationMem().Execute(
            //          BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
            //          lst
            //              .Select(v => v).Select(x => $"{x.EntryDataDetailsId}")
            //              .Aggregate((o, n) => $"{o},{n}")).ConfigureAwait(false);
            //  SaveEntryDataDetails(errors);
            //  await DeleteAllocationsForEntryDataDetails(errors).ConfigureAwait(false);
            return errors;
        }

        private async Task<List<EntryDataDetail>> ProcessNoDataDISForAllocation(List<EntryDataDetail> lst)
        {
            if(!lst.Any() ) return lst;
            var errors = await new ProcessDISErrorsForAllocationNoDB().Execute(
                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                lst
                    .Select(v => v).Select(x => $"{x.EntryDataDetailsId}")
                    .Aggregate((o, n) => $"{o},{n}")).ConfigureAwait(false);

            return errors;
        }

        private async Task DeleteAllocationsForEntryDataDetails(List<EntryDataDetail> errors)
        {
            if(!errors.Any()) return;
            var sql = errors.Select(x => $"delete from AsycudaSalesAllocations where EntryDataDetailsId = {x.EntryDataDetailsId};").Aggregate((o,n) => $"{o}\r\n{n}");
            await new AllocationDSContext().Database.ExecuteSqlCommandAsync(sql).ConfigureAwait(false);
        }




        public static async Task<List<EntryDataDetail>> DoAutoMatch(int applicationSettingsId, List<AdjustmentDetail> lst)
        {
            try
            {

                if (!lst.Any()) return new List<EntryDataDetail>();
                StatusModel.StartStatusUpdate("Matching Shorts To Asycuda Entries", lst.Count());

                // Create a list of tasks
                var tasks = lst
                    .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                    .Select(s => AutoMatchItemNumber(s)) // Don't await here
                    .ToList();

                // Await all tasks concurrently
                var results = await Task.WhenAll(tasks).ConfigureAwait(false);
                
                // Collect non-null results (assuming AutoMatchItemNumber returns EntryDataDetail or null)
                var edLst = results.Where(x => x != null).ToList();


                if(edLst.Any()) SetMinimumEffectDate(edLst);
                return edLst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static Task<EntryDataDetail> AutoMatchItemNumber(AdjustmentDetail s)
        {
            try
            {

                var ed = GetEntryDataDetail(s);
                if(ed == null) return Task.FromResult<EntryDataDetail>(null);
                var processors = new List<IAutoMatchProcessor>()
                {
                    new PreviousInvoiceNumberMatcher(s, ed),
                    new CNumberMatcher(s, ed),
                    new EffectiveDatefProcessor(s, ed),
                    new MissingCostProcessor(s, ed),

                };


                processors.Where(x => x.IsApplicable(s, ed))
                    .ForEach(async x => await x.Execute().ConfigureAwait(false));

                
                return Task.FromResult(ed);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

       

        private static EntryDataDetail GetEntryDataDetail(AdjustmentDetail s)
        {
            var ed = new GetAllDiscrepancyDetailsMem().Execute(s.EntryDataDetailsId);
            if (ed == null) return null;
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
                                   ?? edLst.Where(x => x.AdjustmentEx != null).Min(x => x.AdjustmentEx.EffectiveDate ?? x.AdjustmentEx.InvoiceDate);

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