using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Services;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils.AutoMatching;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales
{
    public class AllocateSalesChain : IAllocateSalesProcessor
    {
        
        public async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment,
            bool onlyNewAllocations, string lst)
        {
            try
            {
                if (lst == null)
                    throw new ApplicationException(
                        "this method cannot function properly on all items unless the caching issue is addressed");
                var itemSets = DataSpace.BaseDataModel.GetItemSets(lst);
               
                await Execute(applicationSettings, allocateToLastAdjustment, onlyNewAllocations, itemSets).ConfigureAwait(false);

                StatusModel.StopStatusUpdate();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, bool onlyNewAllocations,
            List<List<(string ItemNumber, int InventoryItemId)>> itemSets)
        {
            SQLBlackBox.RunSqlBlackBox();
 
            AllocationsBaseModel.PrepareDataForAllocation(applicationSettings);
 
            // Use Parallel.ForEachAsync for asynchronous parallel processing
            var tasks = itemSets.Select(async x =>
            {
                // chained implementation can only work with discrepancies of when memory data is loaded its is outdated
                await new ReAllocatedExistingXSales().Execute(x).ConfigureAwait(false);
                await new ReallocateExistingEx9().Execute(x).ConfigureAwait(false);
                await new AutoMatchSingleSetBasedProcessor().AutoMatch(applicationSettings.ApplicationSettingsId, true, x).ConfigureAwait(false);
                await new OldSalesAllocator()
                    .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment, onlyNewAllocations, x).ConfigureAwait(false);
                await new MarkErrors().Execute(x).ConfigureAwait(false);
            }).ToList();
 
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}