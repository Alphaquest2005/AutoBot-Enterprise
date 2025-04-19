using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EntryDataDS.Business.Entities
{
    public partial class ShipmentBL
    {
        [IgnoreDataMember]
        [NotMapped]
        public ShipmentBL MasterBl { get; set; }
    }
}