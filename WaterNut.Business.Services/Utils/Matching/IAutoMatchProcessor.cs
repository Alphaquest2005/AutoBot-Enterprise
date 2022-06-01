using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public interface IAutoMatchProcessor
    {
        Task Execute();
        bool IsApplicable(AdjustmentDetail s, EntryDataDetail ed);
    }
}