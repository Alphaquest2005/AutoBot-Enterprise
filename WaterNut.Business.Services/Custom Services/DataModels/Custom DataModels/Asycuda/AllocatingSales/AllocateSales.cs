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
                await Execute(applicationSettings, allocateToLastAdjustment, onlyNewAllocations, itemSets).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        static List<string> _progressLst = new List<string>();

        public async Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, bool onlyNewAllocations,
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
            
            UpdateStatus($"Status = Start ReAllocatedExistingXSales ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            var task1 = Task.WhenAll(itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .Select(x => new ReAllocatedExistingXSales().Execute(x))); // Assuming Execute returns Task
            await task1.ConfigureAwait(false);
            UpdateStatus($"Status = Finish ReAllocatedExistingXSales ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start ReallocateExistingEx9 ..., StartTime = {DateTime.Now.ToShortTimeString()}");
             var task2 = Task.WhenAll(itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .Select(x => new ReallocateExistingEx9().Execute(x))); // Assuming Execute returns Task
            await task2.ConfigureAwait(false);
            UpdateStatus($"Status = Finish ReallocateExistingEx9 ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            StatusModel.Timer("Auto Match Adjustments");

            
            UpdateStatus($"Status = Start AutoMatch ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            var task3 = Task.WhenAll(itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .Select(x => new AutoMatchSingleSetBasedProcessor().AutoMatch(applicationSettings.ApplicationSettingsId, true, x))); // AutoMatch is already async Task
            await task3.ConfigureAwait(false);
            UpdateStatus($"Status = Finish AutoMatch ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start Allocate Sales ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            var task4 = Task.WhenAll(itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                // .WithDegreeOfParallelism(1)
                .Select(x =>
                    new OldSalesAllocator()
                        .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(allocateToLastAdjustment, onlyNewAllocations, x))); // AllocateSalesByMatching... is already async Task
            await task4.ConfigureAwait(false);
            UpdateStatus($"Status = Finish Allocate Sales ..., EndTime = {DateTime.Now.ToShortTimeString()}");

            UpdateStatus($"Status = Start MarkErrors ..., StartTime = {DateTime.Now.ToShortTimeString()}");
            var task5 = Task.WhenAll(itemSets
                .AsParallel()
                .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount *
                                                         DataSpace.BaseDataModel.Instance.ResourcePercentage))
                .Select(x => new MarkErrors().Execute(x))); // MarkErrors.Execute is now async Task
            await task5.ConfigureAwait(false);
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