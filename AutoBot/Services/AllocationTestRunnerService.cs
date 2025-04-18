using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Services; // For AdjustmentShortService
using AllocationDS.Business.Entities; // For AllocationDSContext, AllocationsTestCases
using AllocationQS.Business.Entities; // For AllocationsModel, AllocationsBaseModel, OldSalesAllocator, MarkErrors
using WaterNut.DataSpace; // For BaseDataModel

namespace AutoBot.Services
{
    /// <summary>
    /// Runs allocation test cases.
    /// Extracted from AutoBot.Utils class to adhere to SRP.
    /// </summary>
    public static class AllocationTestRunnerService
    {
       public static async Task RunAllocationTestCases()
       {
           try
           {
               Console.WriteLine("Running Test Cases");
               List<KeyValuePair<int, string>> lst;
               using (var ctx = new AllocationDSContext())
               {
                   ctx.Database.CommandTimeout = 10; // Consider making timeout configurable

                   lst = ctx
                       .AllocationsTestCases
                       .Select(x => new { x.EntryDataDetailsId, x.ItemNumber })
                       .Distinct()
                       .ToList() // Materialize before creating KeyValuePair
                       .Select(x => new KeyValuePair<int, string>(x.EntryDataDetailsId, x.ItemNumber))
                       .ToList();
               }

               if (!lst.Any())
               {
                   Console.WriteLine("No allocation test cases found.");
                   return;
               }

               // Assuming AllocationsModel.Instance is a valid singleton or static access pattern
               await AllocationsModel.Instance.ClearDocSetAllocations(lst.Select(x => $"'{x.Value}'").Aggregate((o, n) => $"{o},{n}")).ConfigureAwait(false);

               // Assuming AllocationsBaseModel is static or instance is accessible
               AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);

               var strLst = lst.Select(x => $"{x.Key.ToString()}-{x.Value}").Aggregate((o, n) => $"{o},{n}");

               // Assuming AdjustmentShortService can be instantiated directly
               await new AdjustmentShortService().AutoMatchUtils.AutoMatchItems(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, strLst).ConfigureAwait(false);

               // Assuming ProcessDisErrorsForAllocation is accessible this way
               new AdjustmentShortService().AutoMatchUtils.AutoMatchProcessor.ProcessDisErrorsForAllocation
                   .Execute(
                       BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                       strLst
                   );

               // Assuming OldSalesAllocator can be instantiated directly
               await new OldSalesAllocator()
                   .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                       BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false, false, strLst).ConfigureAwait(false);

               // Assuming MarkErrors can be instantiated directly
               await new MarkErrors()
                   .Execute(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);

                Console.WriteLine("Allocation test cases completed.");
           }
           catch (Exception e)
           {
               Console.WriteLine($"Error running allocation test cases: {e}");
               // Consider logging the exception details
               throw; // Re-throw to allow higher-level handling
           }
       }
    }
}