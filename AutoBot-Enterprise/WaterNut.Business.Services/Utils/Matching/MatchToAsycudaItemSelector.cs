using System.Collections.Generic;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils.MatchingToAsycudaItem;

namespace AdjustmentQS.Business.Services
{
    public class MatchToAsycudaItemSelector
    {
        private bool isDBMem = false;

        public MatchToAsycudaItemSelector()
        {
        }

        public void Execute(AdjustmentDetail adjustmentDetail, List<AsycudaDocumentItem> aItem, EntryDataDetail entryDataDetail)
        {
            if (isDBMem == true)
                new MatchToAsycudaItem().Execute(adjustmentDetail, aItem, entryDataDetail);
            else
                new MatchToAsycudaItemBulkSave().Execute(adjustmentDetail, aItem, entryDataDetail);
        }
    }
}