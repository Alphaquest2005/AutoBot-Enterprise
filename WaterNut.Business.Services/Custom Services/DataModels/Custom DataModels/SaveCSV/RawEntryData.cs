using System.Collections.Generic;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class RawEntryData
    {
        public RawEntryData(RawEntryDataValue item)
        {
            Item = item;
        }

        public RawEntryDataValue Item
        {
            get;
            private set;
        }

        
    }

    public class RawEntryDataValue
    {
        public EntryDataValue EntryData { get; set; }
        public List<EntryDataDetails> EntryDataDetails { get; set; }
        public List<InventoryItemsValue> InventoryItems { get; set; }
        public List<TotalsValue> Totals { get; set; }

        public class EntryDataValue
        {
            public dynamic EntryDataId { get; }
            public dynamic EntryDataDate { get; }
            public int AsycudaDocumentSetId { get; }
            public int ApplicationSettingsId { get; }
            public dynamic CustomerName { get; }
            public dynamic Tax { get; }
            public dynamic Supplier { get; }
            public dynamic Currency { get; }
            public string EmailId { get; }
            public int FileTypeId { get; }
            public dynamic DocumentType { get; }
            public dynamic SupplierInvoiceNo { get; }
            public dynamic PreviousCNumber { get; }
            public dynamic FinancialInformation { get; }
            public dynamic Vendor { get; }
            public dynamic PoNumber { get; }
            public string SourceFile { get; }

            public EntryDataValue(dynamic entryDataId, dynamic entryDataDate, int asycudaDocumentSetId,
                int applicationSettingsId, dynamic customerName, dynamic tax, dynamic supplier, dynamic currency,
                string emailId, int fileTypeId,
                dynamic documentType, dynamic supplierInvoiceNo, dynamic previousCNumber, dynamic financialInformation,
                dynamic vendor, dynamic poNumber, string sourceFile)
            {
                EntryDataId = entryDataId;
                EntryDataDate = entryDataDate;
                AsycudaDocumentSetId = asycudaDocumentSetId;
                ApplicationSettingsId = applicationSettingsId;
                CustomerName = customerName;
                Tax = tax;
                Supplier = supplier;
                Currency = currency;
                EmailId = emailId;
                FileTypeId = fileTypeId;
                DocumentType = documentType;
                SupplierInvoiceNo = supplierInvoiceNo;
                PreviousCNumber = previousCNumber;
                FinancialInformation = financialInformation;
                Vendor = vendor;
                PoNumber = poNumber;
                SourceFile = sourceFile;
            }
        }

        public class InventoryItemsValue
        {
            public dynamic ItemNumber { get; }
            public dynamic ItemAlias { get; }

            public InventoryItemsValue(dynamic itemNumber, dynamic itemAlias)
            {
                ItemNumber = itemNumber;
                ItemAlias = itemAlias;
            }
        }
        public class TotalsValue
        {
            public double TotalWeight { get; }
            public double TotalFreight { get; }
            public double TotalInternalFreight { get; }
            public double TotalOtherCost { get; }
            public double TotalInsurance { get; }
            public double TotalDeductions { get; }
            public double InvoiceTotal { get; }
            public double TotalTax { get; }
            public int Packages { get; }
            public dynamic WarehouseNo { get; }

            public TotalsValue(double totalWeight, double totalFreight, double totalInternalFreight, double totalOtherCost,
                double totalInsurance, double totalDeductions, double invoiceTotal, double totalTax, int packages,
                dynamic warehouseNo)
            {
                TotalWeight = totalWeight;
                TotalFreight = totalFreight;
                TotalInternalFreight = totalInternalFreight;
                TotalOtherCost = totalOtherCost;
                TotalInsurance = totalInsurance;
                TotalDeductions = totalDeductions;
                InvoiceTotal = invoiceTotal;
                TotalTax = totalTax;
                Packages = packages;
                WarehouseNo = warehouseNo;
            }
        }
    }

    
}