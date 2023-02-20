using System.Collections.Generic;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.AdjustmentQS.GettingEx9AllocationsList
{
    public interface IgetEx9AllocationsListProcessor
    {
        List<EX9Allocations> Execute(string filterExpression);
    }
}