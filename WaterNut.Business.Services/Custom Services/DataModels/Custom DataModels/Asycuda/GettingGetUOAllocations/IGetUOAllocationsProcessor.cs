using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingGetUOAllocations
{
    public interface IGetUOAllocationsProcessor
    {
        List<IGrouping<xcuda_Item, AsycudaSalesAllocations>> Execute(List<int> itemList);
    }
}