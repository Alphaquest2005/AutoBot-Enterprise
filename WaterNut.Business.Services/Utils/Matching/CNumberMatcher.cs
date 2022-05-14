using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public class CNumberMatcher: IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;

        public CNumberMatcher(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }

        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail, _entryDataDetail)) return;
            await new ShortCNumberMatcher(_adjustmentDetail, _entryDataDetail).Execute().ConfigureAwait(false);
            await new OverCNumberMatcher(_adjustmentDetail, _entryDataDetail).Execute().ConfigureAwait(false);

        }

        public bool IsApplicable(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail) 
            => entryDataDetail.EffectiveDate == null && !string.IsNullOrEmpty(adjustmentDetail.PreviousCNumber);
    }
}