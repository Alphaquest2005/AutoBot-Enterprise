using System;
using System.Collections.Generic;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9
{
    public class EX9Allocations
    {
        public string pItemDescription;
        public string pTariffCode { get; set; }
        public string DutyFreePaid { get; set; }
        public string pCNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int? PreviousItem_Id { get; set; }
        public double SalesQuantity { get; set; }
        public int AllocationId { get; set; }
        public int? EntryDataDetailsId { get; set; }
        public string Status { get; set; }
        public string InvoiceNo { get; set; }
        public string ItemNumber { get; set; }
        public string pItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public double? pItemCost { get; set; }
        public double QtyAllocated { get; set; }
        public double SalesQtyAllocated { get; set; }
        public double DFQtyAllocated { get; set; }
        public double DPQtyAllocated { get; set; }
        public int? LineNumber { get; set; }
        public List<PreviousItems> previousItems { get; set; }
        public string Country_of_origin_code { get; set; }
        public string Customs_clearance_office_code { get; set; }
        public double? pQuantity { get; set; }
        public DateTime pRegistrationDate { get; set; }
        public double Net_weight_itm { get; set; }
        public double Total_CIF_itm { get; set; }
        public double Freight { get; set; }
        public double InternalFreight { get; set; }
        public double Weight { get; set; }
        public List<TariffSupUnitLkps> TariffSupUnitLkps { get; set; }
        public double SalesFactor { get; set; }
        public DateTime pAssessmentDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int pLineNumber { get; set; }
        public string Currency { get; set; }
        public int? FileTypeId { get; set; }
        public string EmailId { get; set; }
        public int EntryData_Id { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }
        public int InventoryItemId { get; set; }
        public string pPrecision1 { get; set; }
        public List<AsycudaSalesAllocationsPIData> PIData { get; set; }
        public DateTime pExpiryDate { get; set; }
        public EntryDataDetails EntryDataDetails { get; set; }
        public string SourceFile
        { get; set; }
    }
}