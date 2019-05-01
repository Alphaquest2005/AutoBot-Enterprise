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
        public string TariffCode
        {
            get
            {
                return (PreviousDocumentItem != null && PreviousDocumentItem.ItemCost != null && PreviousDocumentItem.TariffCode != null)
                    ? PreviousDocumentItem.TariffCode
                    : "";
            }
        }

       
        [IgnoreDataMember]
        [NotMapped]
        public xcuda_Item xBondEntry
        {
            get
            {
                if(xBondAllocations.Any()) return xBondAllocations.FirstOrDefault().xcuda_Item;
                return null;
            }
        }


    }
}
