using System.Collections.Generic;
using WaterNut.Interfaces;

namespace WaterNut.DataSpace
{
  public class AlloEntryLineData: BaseDataModel.IEntryLineData //: AllocationsModel.AlloEntryLineData
    {
        public double Cost { get; set; }
        public List<EntryDataDetailSummary> EntryDataDetails { get; set; }
        public double Weight { get; set; }
        public double InternalFreight { get; set; }
        public double Freight { get; set; }
        public List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }
        public string ItemDescription { get; set; }
        public string ItemNumber { get; set; }
        public int PreviousDocumentItemId { get; set; }
        public double Quantity { get; set; }
        public string TariffCode { get; set; }
        public pDocumentItem pDocumentItem { get; set; }
        public EX9Allocation EX9Allocation { get; set; }
        public int? FileTypeId { get; set; }
        public string EmailId { get; set; }
    }
}