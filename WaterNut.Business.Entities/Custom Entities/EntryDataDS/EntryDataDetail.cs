using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using AllocationDS.Business.Entities;


namespace EntryDataDS.Business.Entities
{
    public partial class EntryDataDetails
    {
        [IgnoreDataMember]
        [NotMapped]
        public string TariffCode
        {
            get
            {
                if (InventoryItems != null && InventoryItems.TariffCode != null)
                {
                    return InventoryItems.TariffCode;
                }
                else
                {
                    return "Null";
                }
            }
        }
    }
}
