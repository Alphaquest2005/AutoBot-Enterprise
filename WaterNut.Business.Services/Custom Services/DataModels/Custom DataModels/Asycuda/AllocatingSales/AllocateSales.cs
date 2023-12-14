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

     

        public async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment,
            bool onlyNewAllocations, string lst = null)
        {
            try
            {
                List<List<(string ItemNumber, int InventoryItemId)>> itemSets = DataSpace.BaseDataModel.GetItemSets(lst);
                //var dupitemsets = itemSets.Where(x => x.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
                Execute(applicationSettings, allocateToLastAdjustment, onlyNewAllocations, itemSets);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        static List<string> _progressLst = new List<string>();

        public void Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, bool onlyNewAllocations,
            List<List<(string ItemNumber, int InventoryItemId)>> itemSets)
        {
            
            _progressLst.Clear();
            UpdateStatus($"Status = Starting Allocation..., TotalRecords = {itemSets.Count}");
            UpdateStatus($"Status = Start SQLBlackBox ..., StartTime = {DateTime.Now.ToShortTimeString()}");

            SQLBlackBox.RunSqlBlackBox();

            UpdateStatus($"Status = Finish SQLBlackBox ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start PrepareDataForAllocation ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            AllocationsBaseModel.PrepareDataForAllocation(applicationSettings);
            UpdateStatus($"Status = Finish PrepareDataForAllocation ..., EndTime = {DateTime.Now.ToShortTimeString()}");
            //var dupitemsets = itemSets.Where(x => x.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
            UpdateStatus($"Status = Start ReAllocatedExistingXSales ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new ReAllocatedExistingXSales().Execute(x).Wait());
            UpdateStatus($"Status = Finish PrepareDataForAllocation ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start ReallocateExistingEx9 ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new ReallocateExistingEx9().Execute(x).Wait()); // to prevent changing allocations when im7 info changes

            UpdateStatus($"Status = Finish ReallocateExistingEx9 ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            StatusModel.Timer("Auto Match Adjustments");

            //new AdjustmentShortService().AutoMatch(applicationSettings.ApplicationSettingsId, true, itemSets)
            //    .Wait();
            UpdateStatus($"Status = Start AutoMatch ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new AutoMatchSingleSetBasedProcessor().AutoMatch(applicationSettings.ApplicationSettingsId, true, x).Wait());
            UpdateStatus($"Status = Finish AutoMatch ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start Allocate Sales ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                // .WithDegreeOfParallelism(1)
                .ForAll(x =>
                    new OldSalesAllocator()
                        .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment, onlyNewAllocations, x).Wait());
            UpdateStatus($"Status = Finish Allocate Sales ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start MarkErrors ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .ForAll(x => new MarkErrors().Execute(x).Wait());
            UpdateStatus($"Status = Finish MarkErrors ..., EndTime = {DateTime.Now.ToShortTimeString()}");
            UpdateStatus($"Status = Finish Allocation..., TotalRecords = {itemSets.Count},  EndTime = {DateTime.Now.ToShortTimeString()}");

            StatusModel.StopStatusUpdate();

            Console.WriteLine(_progressLst.Aggregate((o,n) => $"{o}\r\n{n}"));
            Console.WriteLine("Done");
        }

        private static void UpdateStatus(string status)
        {
           
            _progressLst.Add(status);
            Console.WriteLine(status);
        }
    }
}