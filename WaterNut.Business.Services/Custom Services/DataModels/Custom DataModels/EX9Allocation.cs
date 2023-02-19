using System;

namespace WaterNut.DataSpace
{
    public class EX9Allocation
    {
        public string pPrecision1;
        public string pTariffCode { get; set; }
        public double pQuantity { get; set; }
        public string Country_of_origin_code { get; set; }
        public double Total_CIF_itm { get; set; }
        public string pCNumber { get; set; }
        public DateTime pRegistrationDate { get; set; }
        public string Customs_clearance_office_code { get; set; }
        public double Net_weight_itm { get; set; }
        public double pQtyAllocated { get; set; }
        public double SalesFactor { get; set; }
    }
}