//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace AdjustmentQS.Business.Entities
//{
//    public partial class OverShortDetailsEX
//    {
//        [IgnoreDataMember]
//        [NotMapped]
//        public double ShortQuantity
//        {
//            get { return ReceivedQty < InvoiceQty ? Convert.ToDouble(InvoiceQty - ReceivedQty) : 0; }
//        }
//        [IgnoreDataMember]
//        [NotMapped]
//        public double OversQuantity
//        {
//            get { return InvoiceQty < ReceivedQty ? Convert.ToDouble(ReceivedQty - InvoiceQty) : 0; }
//        }
//        [IgnoreDataMember]
//        [NotMapped]
//        public string Type
//        {

//            get
//            {
//                return ReceivedQty < InvoiceQty ? "Short" : "Over";
//            }

//        }
//        [IgnoreDataMember]
//        [NotMapped]
//        public double RemainingQty
//        {
//            get { return Convert.ToDouble(OverShortAllocationsEXes.Sum(x => x.AsycudaDocumentItem.ItemQuantity - x.AsycudaDocumentItem.PiQuantity)); }
//        }
//        [IgnoreDataMember]
//        [NotMapped]
//        public OverShortDetailAllocation LastOverShortDetailAllocation
//        {
//            get { return OverShortDetailAllocations.LastOrDefault(); }
//        }
//    }
//}
