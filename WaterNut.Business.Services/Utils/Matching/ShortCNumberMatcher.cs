using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils.MatchingToAsycudaItem;

namespace AdjustmentQS.Business.Services
{
    public class ShortCNumberMatcher : IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;

        public ShortCNumberMatcher(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }

        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail, _entryDataDetail)) return;

            var aItems = await AutoMatchUtils.GetAsycudaEntriesInCNumber(_adjustmentDetail.PreviousCNumber,
                    _adjustmentDetail.PreviousCLineNumber,
                    _adjustmentDetail.ItemNumber)
                .ConfigureAwait(false);

            if (!aItems.Any())
            {
                aItems = await AutoMatchUtils.GetAsycudaEntriesInCNumberReference(
                        _adjustmentDetail.ApplicationSettingsId,
                        _adjustmentDetail.PreviousCNumber, _adjustmentDetail.ItemNumber)
                    .ConfigureAwait(false);
            }

            if (!aItems.Any()) return;

            new MatchToAsycudaItemSelector().Execute(_adjustmentDetail,
                aItems.OrderBy(x => x.AsycudaDocument.AssessmentDate)
                    .ToList(), _entryDataDetail);
        }
        
        public bool IsApplicable(AdjustmentDetail s, EntryDataDetail ed) => s.InvoiceQty > 0;
    }
}