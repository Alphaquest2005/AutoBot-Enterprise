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
        
        public async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, string lst= null)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox();

                AllocationsBaseModel.PrepareDataForAllocation(applicationSettings);
              
                var itemSets = DataSpace.BaseDataModel.GetItemSets(lst);
               
                itemSets
                    .AsParallel()
                    .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * DataSpace.BaseDataModel.Instance.ResourcePercentage))
                    .ForAll( x =>
                    {
                         new ReAllocatedExistingXSales().Execute(x).Wait();
                        new ReallocateExistingEx9().Execute(x).Wait();
                        new AutoMatchSingleSetBasedProcessor().AutoMatch(applicationSettings.ApplicationSettingsId, true, x).Wait();
                        new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment, x).Wait();
                        new MarkErrors().Execute(x).Wait();

                    });

                StatusModel.StopStatusUpdate();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}