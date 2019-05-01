using System;

namespace WaterNut.Business.BusinessModels.Custom_DataModels
{
    public class SaleReportLine
    {
        public int Line { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public string TariffCode { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string SalesType { get; set; }
        public double GrossSales { get; set; }
        public string PreviousCNumber { get; set; }
        public string PreviousLineNumber { get; set; }
        public string PreviousRegDate { get; set; }
        public double CIFValue { get; set; }
        public double DutyLiablity { get; set; }
    }
}
