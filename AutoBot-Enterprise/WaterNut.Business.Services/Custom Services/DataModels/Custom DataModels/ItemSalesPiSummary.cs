using System;

namespace WaterNut.DataSpace
{
    public class ItemSalesPiSummary
    {
        public string ItemNumber { get; set; }
        //public double SalesQuantity { get; set; }
        public double? QtyAllocated { get; set; }
        public double PiQuantity { get; set; }
        public string pCNumber { get; set; }
        public int pLineNumber { get; set; }
        //public DateTime? pRegistrationDate { get; set; }
        public string DutyFreePaid { get; set; }
        public string Type { get; set; }
        public int PreviousItem_Id { get; set; }
        //public double pQtyAllocated { get; set; }
        public string EntryDataType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}