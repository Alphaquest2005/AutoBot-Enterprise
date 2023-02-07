using System.Collections.Generic;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class RawRiderData
    {
        //((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId,
        //    dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId,
        //    dynamic DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation,
        //    dynamic Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails>
        //    EntryDataDetails,
        //    IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost,
        //        double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
        //        dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems)
        public RawRiderData(((dynamic ETA, dynamic DocumentDate, int AsycudaDocumentSetId, int ApplicationSettingsId, string EmailId, int FileTypeId,
            string SourceFile) Rider, IEnumerable<ShipmentRiderDetails> RiderDetails)
            item)
        {
            Item = item;
        }

        public ((dynamic ETA, dynamic DocumentDate, int AsycudaDocumentSetId, int ApplicationSettingsId, string EmailId, int FileTypeId, string SourceFile) Rider, IEnumerable<ShipmentRiderDetails> RiderDetails) Item
        {
            get;
            private set;
        }


    }
}