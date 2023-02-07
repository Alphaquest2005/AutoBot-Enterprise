using System.Threading.Tasks;
using CoreEntities.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales
{
    public interface IAllocateSalesProcessor
    {
        Task Execute(ApplicationSettings applicationSettings, bool allocateToLastAdjustment, string lst= null);
    }
}