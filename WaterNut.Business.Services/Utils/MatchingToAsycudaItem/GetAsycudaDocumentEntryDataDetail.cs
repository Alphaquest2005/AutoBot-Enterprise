using System.Linq;
using AdjustmentQS.Business.Entities;

namespace WaterNut.Business.Services.Utils.MatchingToAsycudaItem
{
    public interface IGetAsycudaDocumentEntryDataDetailProcessor
    {
        AsycudaDocumentItemEntryDataDetail Execute(EntryDataDetail ed);
    }

    public class GetAsycudaDocumentEntryDataDetail : IGetAsycudaDocumentEntryDataDetailProcessor
    {
        public  AsycudaDocumentItemEntryDataDetail Execute(EntryDataDetail ed)
        {
            return new AdjustmentQSContext().AsycudaDocumentItemEntryDataDetails
                .FirstOrDefault(x => x.EntryDataDetailsId == ed.EntryDataDetailsId);
        }
    }
}