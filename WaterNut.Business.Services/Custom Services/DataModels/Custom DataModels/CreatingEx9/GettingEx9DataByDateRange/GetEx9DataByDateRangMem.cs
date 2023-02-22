using System.Collections.Generic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9DataByDateRange
{
    public class GetEx9DataByDateRangeMem : IGetEx9DataByDateRange
    {
        private GetEx9AsycudaSalesAllocationsMem _getEx9AsycudaSalesAllocationsMem;

        public GetEx9DataByDateRangeMem(string filterExp, string rdateFilter)
        {
            _getEx9AsycudaSalesAllocationsMem = new GetEx9AsycudaSalesAllocationsMem(filterExp, rdateFilter);
        }

        public async Task<List<EX9AsycudaSalesAllocations>> Execute(string dateFilter)
        {
            
            return await _getEx9AsycudaSalesAllocationsMem.Execute(dateFilter).ConfigureAwait(false);
        }
    }
}