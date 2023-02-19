using System;

namespace WaterNut.DataSpace
{
    public class PiSummary
    {
        public string ItemNumber { get; set; }
        public string DutyFreePaid { get; set; }
        public double TotalQuantity { get; set; }
        public string Type
        { get; set; }

        public string pCNumber { get; set; }
        public string pLineNumber { get; set; }
        public string pOffice { get; set; }
        public double pItemQuantity { get; set; }
        public DateTime pAssessmentDate { get; set; }
        public int PreviousItem_Id { get; set; }
    }
}