using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public class ShortPreviousNumberMatcher : IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;

        public ShortPreviousNumberMatcher(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }  
        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail, _entryDataDetail)) return;
            List<AsycudaDocumentItem> aItem;
            aItem = await AutoMatchUtils.GetAsycudaEntriesWithInvoiceNumber(_adjustmentDetail.ApplicationSettingsId,
                    _adjustmentDetail.PreviousInvoiceNumber, _adjustmentDetail.EntryDataId, _adjustmentDetail.ItemNumber)
                .ConfigureAwait(false);
            if (aItem.Any()) AutoMatchUtils.MatchToAsycudaItem(_adjustmentDetail, aItem, _entryDataDetail);
        }

        public bool IsApplicable(AdjustmentDetail s, EntryDataDetail ed) => _adjustmentDetail.InvoiceQty > 0;
    }
}