﻿using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public class OverCNumberMatcher : IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;

        public OverCNumberMatcher(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }

        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail,_entryDataDetail)) return;

            var asycudaDocument =
                AutoMatchUtils.GetAsycudaDocumentInCNumber(_adjustmentDetail.ApplicationSettingsId, _adjustmentDetail.PreviousCNumber);
            AutoMatchUtils.MatchToAsycudaDocument(asycudaDocument, _entryDataDetail);
        }

        public bool IsApplicable(AdjustmentDetail s, EntryDataDetail ed)
        {
            return s.InvoiceQty <= 0;
        }
    }
}