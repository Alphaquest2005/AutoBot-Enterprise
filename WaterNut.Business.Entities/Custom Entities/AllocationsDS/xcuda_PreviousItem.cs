

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using CoreEntities.Business.Enums;

namespace AllocationDS.Business.Entities
{
    public partial class xcuda_PreviousItem
    {

        [IgnoreDataMember]
        [NotMapped]
        public string DutyFreePaid
        {
            get
            {
                if (xcuda_Item == null) return null;
                if (xcuda_Item.AsycudaDocument == null) return null;
                if (xcuda_Item.AsycudaDocument.Type_of_declaration == null) return null;
                return xcuda_Item.AsycudaDocument.CustomsOperationId == (int)CoreEntities.Business.Enums.CustomsOperations.Exwarehouse && xcuda_Item.AsycudaDocument.IsPaid == true
                            ? "Duty Paid" 
                            : "Duty Free";
            }
        }




    }
}


