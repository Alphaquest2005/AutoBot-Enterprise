using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using EntryDataDS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public class EffectiveDatefProcessor: IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;

        public EffectiveDatefProcessor(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }

        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail, _entryDataDetail)) return;
            await SetEffectiveDateStrategy(_adjustmentDetail, _entryDataDetail).ConfigureAwait(false);
        }

        public bool IsApplicable(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            return entryDataDetail.EffectiveDate == null;
        }

        private static async Task SetEffectiveDateStrategy(AdjustmentDetail s, EntryDataDetail ed)
        {
            if (s.Type == "DIS")
            {
                var po = await new EntryDataDSContext().EntryData.OfType<PurchaseOrders>()
                    .FirstOrDefaultAsync(x =>
                        x.EntryDataId == ed.EntryDataId && x.ApplicationSettingsId == s.ApplicationSettingsId).ConfigureAwait(false); // || ed.PreviousInvoiceNumber.EndsWith(x.EntryDataId) Contains too random
                ed.EffectiveDate = po?.EntryDataDate ?? s.InvoiceDate;
            }
            else
            {
                ed.EffectiveDate = s.InvoiceDate;
            }
        }
    }
}