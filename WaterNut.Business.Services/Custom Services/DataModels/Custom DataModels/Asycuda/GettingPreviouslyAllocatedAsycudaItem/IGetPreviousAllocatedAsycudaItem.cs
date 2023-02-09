using System.Collections.Generic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingPreviouslyAllocatedAsycudaItem
{
    public interface IGetPreviousAllocatedAsycudaItem
    {
        int Execute(List<xcuda_Item> asycudaEntries, EntryDataDetails saleitm, int i);
    }
}