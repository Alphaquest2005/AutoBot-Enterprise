using System.Collections.Generic;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    internal class SummaryData
    {
        public ItemSalesAsycudaPiSummary Summary { get; set; }
        public IEnumerable<AsycudaItemPiQuantityData> pIData { get; set; }
    }
}