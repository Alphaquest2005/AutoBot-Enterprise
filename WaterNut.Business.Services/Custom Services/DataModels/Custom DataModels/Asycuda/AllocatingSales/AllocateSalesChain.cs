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
        
        public async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, string lst)
        {
            try
            {
                if (lst == null)
                    throw new ApplicationException(
                        "this method cannot function properly on all items unless the caching issue is addressed");
                var itemSets = DataSpace.BaseDataModel.GetItemSets(lst);
               
                Execute(applicationSettings, allocateToLastAdjustment, itemSets);

                StatusModel.StopStatusUpdate();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment,
            List<List<(string ItemNumber, int InventoryItemId)>> itemSets)
        {
            SQLBlackBox.RunSqlBlackBox();

            AllocationsBaseModel.PrepareDataForAllocation(applicationSettings);

            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))

                // chained implementation can only work with discrepancies of when memory data is loaded its is outdated
                .ForAll(x =>
                {
                    new ReAllocatedExistingXSales().Execute(x).Wait();
                    new ReallocateExistingEx9().Execute(x).Wait();
                    new AutoMatchSingleSetBasedProcessor().AutoMatch(applicationSettings.ApplicationSettingsId, true, x).Wait();
                    new OldSalesAllocator()
                        .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment, x).Wait();
                    new MarkErrors().Execute(x).Wait();
                });
        }
    }
}