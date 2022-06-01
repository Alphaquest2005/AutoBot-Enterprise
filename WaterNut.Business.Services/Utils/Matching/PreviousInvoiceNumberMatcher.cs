using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public class OverPreviousInvoiceNumberMatcher : IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;


        public OverPreviousInvoiceNumberMatcher(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }

        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail,_entryDataDetail)) return;
           
                List<AsycudaDocumentItem> aItem;
                aItem = await AutoMatchUtils.GetAsycudaEntriesWithInvoiceNumber(_adjustmentDetail.ApplicationSettingsId,
                        _adjustmentDetail.PreviousInvoiceNumber, _adjustmentDetail.EntryDataId)
                    .ConfigureAwait(false);
                if (aItem.Any()) AutoMatchUtils.MatchToAsycudaDocument(aItem.First().AsycudaDocument, _entryDataDetail);
           
        }

        public bool IsApplicable(AdjustmentDetail adjustmentDetail, EntryDataDetail ed)
        {
            return ed.InvoiceQty <= 0;
        }
    }

    public class PreviousInvoiceNumberMatcher : IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;
        private readonly OverPreviousInvoiceNumberMatcher _overPreviousInvoiceNumberMatcher;

        public PreviousInvoiceNumberMatcher(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
           
        }

        public async Task Execute()
        {

            if (!IsApplicable(_adjustmentDetail, _entryDataDetail)) return;


            await new ShortPreviousNumberMatcher(_adjustmentDetail, _entryDataDetail).Execute().ConfigureAwait(false);
            await new OverPreviousInvoiceNumberMatcher(_adjustmentDetail, _entryDataDetail).Execute().ConfigureAwait(false);


        }

        public  bool IsApplicable(AdjustmentDetail s, EntryDataDetail ed)
        {
            return ed.EffectiveDate == null && !string.IsNullOrEmpty(s.PreviousInvoiceNumber);
        }
    }
}