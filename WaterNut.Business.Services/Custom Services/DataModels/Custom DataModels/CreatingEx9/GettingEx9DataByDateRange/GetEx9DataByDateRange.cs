using System.Collections.Generic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9DataByDateRange
{
    public class GetEx9DataByDateRange : IGetEx9DataByDateRange
    {
        
        public static async Task<List<EX9AsycudaSalesAllocations>> Execute(string dateFilter)
        {
            return await new GetEx9AsycudaSalesAllocations().Execute(dateFilter).ConfigureAwait(false);
        }
    }
}