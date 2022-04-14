using System;

namespace AutoBot
{
    public class DiscrepancyPreExecutionReport
    {
        public string Type { get; set; }

        public System.DateTime InvoiceDate { get; set; }

        public Nullable<System.DateTime> EffectiveDate { get; set; }

        public string InvoiceNo { get; set; }

        public Nullable<int> LineNumber { get; set; }

        public string ItemNumber { get; set; }

        public string ItemDescription { get; set; }

        public Nullable<double> InvoiceQty { get; set; }

        public Nullable<double> ReceivedQty { get; set; }

        public double Cost { get; set; }

        public string PreviousCNumber { get; set; }

        public string PreviousInvoiceNumber { get; set; }

        public string comment { get; set; }

        public string Status { get; set; }

        public string DutyFreePaid { get; set; }
        public string subject { get; set; }

        public System.DateTime emailDate { get; set; }
        public string DocumentType { get; set; }
        public string Reference { get; set; }
        public double Quantity { get; set; }
    }
}