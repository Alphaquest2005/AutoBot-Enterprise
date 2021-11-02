using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using WaterNut.Interfaces;

namespace AllocationDS.Business.Entities
{
    public partial class AsycudaSalesAllocations 
    {
       
        [IgnoreDataMember]
        [NotMapped]
        public string TariffCode =>
            (PreviousDocumentItem != null && PreviousDocumentItem.ItemCost != 0 && PreviousDocumentItem.TariffCode != null)
                ? PreviousDocumentItem.TariffCode
                : "";


        [IgnoreDataMember]
        [NotMapped]
        public xcuda_Item xBondEntry => xBondAllocations.Any() ? xBondAllocations.FirstOrDefault()?.xcuda_Item : null;

        [IgnoreDataMember]
        [NotMapped]
        public string DutyFreePaid =>
            (EntryDataDetails != null)
                ? EntryDataDetails.DutyFreePaid
                : "";

        [IgnoreDataMember]
        [NotMapped]
        public string Type =>
            (EntryDataDetails != null)
                ? EntryDataDetails.Sales != null ? EntryDataDetails.Sales.EntryType : EntryDataDetails.Adjustments.Type
                : "";
    }
}
