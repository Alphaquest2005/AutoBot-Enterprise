using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public class MissingCostProcessor: IAutoMatchProcessor
    {
        private readonly AdjustmentDetail _adjustmentDetail;
        private readonly EntryDataDetail _entryDataDetail;

        public MissingCostProcessor(AdjustmentDetail adjustmentDetail, EntryDataDetail entryDataDetail)
        {
            _adjustmentDetail = adjustmentDetail;
            _entryDataDetail = entryDataDetail;
        }



        public async Task Execute()
        {
            if (!IsApplicable(_adjustmentDetail, _entryDataDetail)) return;


            var lastItemCost = await new AdjustmentQSContext().AsycudaDocumentItemLastItemCosts
                .Where(x => x.assessmentdate <= _entryDataDetail.EffectiveDate)
                .OrderByDescending(x => x.assessmentdate)
                .FirstOrDefaultAsync(x =>
                    x.ItemNumber == _entryDataDetail.ItemNumber &&
                    x.applicationsettingsid == _adjustmentDetail.ApplicationSettingsId).ConfigureAwait(false);
            if (lastItemCost != null)
                _entryDataDetail.LastCost = (double)lastItemCost.LocalItemCost.GetValueOrDefault();
        }

        public bool IsApplicable(AdjustmentDetail s, EntryDataDetail ed)
        {
            return (ed.Cost == 0) && (s.InvoiceQty.GetValueOrDefault() <= 0) && 
                   //////////// only apply to Adjustments because they are after shipment... discrepancies have to be provided.
                   (s.Type == "DIS");
        } 
        
        
    }
}