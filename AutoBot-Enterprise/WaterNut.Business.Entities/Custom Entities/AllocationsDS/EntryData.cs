

using WaterNut.Interfaces;

namespace EntryDataDS.Business.Entities
{
    public partial class EntryData
    {
        public double ExpectedTotal => (this.EntryDataTotals?.Total.GetValueOrDefault()??0) + this.TotalInternalFreight.GetValueOrDefault() +
                                                 this.TotalInsurance.GetValueOrDefault() + this.TotalOtherCost.GetValueOrDefault() -
                                                 this.TotalDeduction.GetValueOrDefault();
    }
}
