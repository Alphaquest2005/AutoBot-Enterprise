//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AdjustmentQS.Client.Entities
//{
//    public partial class OverShortDetailsEX
//    {
//        public double ShortQuantity
//        {
//            get { return ReceivedQty < InvoiceQty ? Convert.ToDouble(InvoiceQty - ReceivedQty) : 0; }
//        }

//        public double OversQuantity
//        {
//            get { return InvoiceQty < ReceivedQty ? Convert.ToDouble(ReceivedQty - InvoiceQty) : 0; }
//        }

//        public string Type
//        {

//            get
//            {
//                return ReceivedQty < InvoiceQty ? "Short" : "Over";
//            }

//        }

//        public double RemainingQty
//        {
//            get { return Convert.ToDouble(OverShortAllocationsEXes.Sum(x => x.AsycudaDocumentItem.ItemQuantity - x.AsycudaDocumentItem.PiQuantity)); }
//        }

//        public OverShortDetailAllocation LastOverShortDetailAllocation
//        {
//            get { return OverShortDetailAllocations.LastOrDefault(); }
//        }
//    }
//}
