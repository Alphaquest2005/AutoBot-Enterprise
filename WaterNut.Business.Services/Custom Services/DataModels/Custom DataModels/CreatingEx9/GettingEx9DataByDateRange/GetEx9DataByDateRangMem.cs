using System; // Added for InvalidOperationException
using System.Collections.Generic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9DataByDateRange
{
    public class GetEx9DataByDateRangeMem : IGetEx9DataByDateRange
    {
        private readonly GetEx9AsycudaSalesAllocationsMem _getEx9AsycudaSalesAllocationsMem;
        // It's unclear if this class itself needs singleton/static initialization logic
        // Assuming for now that multiple instances might be created via the factory.

        // Private constructor
        private GetEx9DataByDateRangeMem(GetEx9AsycudaSalesAllocationsMem instance)
        {
            _getEx9AsycudaSalesAllocationsMem = instance;
        }

        // Async factory method
        public static async Task<GetEx9DataByDateRangeMem> CreateAsync(string filterExp, string rdateFilter)
        {
            // Await the creation of the dependency
            var instance = await GetEx9AsycudaSalesAllocationsMem.CreateAsync(filterExp, rdateFilter).ConfigureAwait(false);
            return new GetEx9DataByDateRangeMem(instance);
        }


        public IEnumerable<EX9AsycudaSalesAllocations> Execute(string dateFilter)
        {
            if (_getEx9AsycudaSalesAllocationsMem == null)
                throw new InvalidOperationException("GetEx9DataByDateRangeMem has not been initialized. Call CreateAsync first.");

            return _getEx9AsycudaSalesAllocationsMem.Execute(dateFilter);
        }
    }
}