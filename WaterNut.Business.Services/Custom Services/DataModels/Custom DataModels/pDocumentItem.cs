using System;
using System.Collections.Generic;

namespace WaterNut.DataSpace
{
    public class pDocumentItem
    {
        public int LineNumber { get; set; }
        public double DPQtyAllocated { get; set; }
        public List<PreviousItems> previousItems { get; set; }
        public double DFQtyAllocated { get; set; }

        public double QtyAllocated => DFQtyAllocated + DPQtyAllocated;
        public DateTime AssessmentDate { get; set; }
        public double ItemQuantity { get; set; }
        public string ItemNumber { get; set; }
        public int xcuda_ItemId { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}