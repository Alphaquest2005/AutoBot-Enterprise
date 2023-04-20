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
    public class AllocateSales : IAllocateSalesProcessor
    {
        


        static AllocateSales()
        {
        }

        public AllocateSales()
        {
          
        }

     

        public async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, string lst= null)
        {
            try
            {
                List<List<(string ItemNumber, int InventoryItemId)>> itemSets = DataSpace.BaseDataModel.GetItemSets(lst);
                //var dupitemsets = itemSets.Where(x => x.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
                Execute(applicationSettings, allocateToLastAdjustment, itemSets);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, List<List<(string ItemNumber, int InventoryItemId)>> itemSets)
        {
            SQLBlackBox.RunSqlBlackBox();

            AllocationsBaseModel.PrepareDataForAllocation(applicationSettings);

            //var dupitemsets = itemSets.Where(x => x.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();

            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new ReAllocatedExistingXSales().Execute(x).Wait());

            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new ReallocateExistingEx9().Execute(x).Wait()); // to prevent changing allocations when im7 info changes


            StatusModel.Timer("Auto Match Adjustments");

            //new AdjustmentShortService().AutoMatch(applicationSettings.ApplicationSettingsId, true, itemSets)
            //    .Wait();

                itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new AutoMatchSingleSetBasedProcessor().AutoMatch(applicationSettings.ApplicationSettingsId, true, x).Wait());
            


            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                // .WithDegreeOfParallelism(1)
                .ForAll(x =>
                    new OldSalesAllocator()
                        .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment, x).Wait());


            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new MarkErrors().Execute(x).Wait());


            StatusModel.StopStatusUpdate();
        }
    }
}