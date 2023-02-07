using System.Collections.Concurrent;
using System.Linq;
using AdjustmentQS.Business.Entities;

namespace WaterNut.Business.Services.Utils.MatchingToAsycudaItem
{
   

    public class GetAsycudaDocumentEntryDataDetailMem : IGetAsycudaDocumentEntryDataDetailProcessor
    {
        static readonly object Identity = new object();
        private static ConcurrentDictionary<(long Id, int EntryDataDetailsId), AsycudaDocumentItemEntryDataDetail> _asycudaEntryDetails = null;

        public GetAsycudaDocumentEntryDataDetailMem()
        {
            lock (Identity)
            {
                if (_asycudaEntryDetails == null)
                    using (var ctx = new AdjustmentQSContext())
                    {
                        var lst = new AdjustmentQSContext()
                            .AsycudaDocumentItemEntryDataDetails
                            .ToDictionary(x => (x.Id, x.EntryDataDetailsId), x => x);
                        _asycudaEntryDetails =
                            new ConcurrentDictionary<(long Id, int EntryDataDetailsId),
                                AsycudaDocumentItemEntryDataDetail>(lst);
                    }

            }
        }

        public  AsycudaDocumentItemEntryDataDetail Execute(EntryDataDetail ed)
        {
            return _asycudaEntryDetails
                .FirstOrDefault(x => x.Key.EntryDataDetailsId == ed.EntryDataDetailsId).Value;
        }
    }
}