using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public interface IEntryDataCreator
    {
        Task<EntryData> Create(List<AsycudaDocumentSet> docSet, ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
            CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
            DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
            Vendor,
            dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
            IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, int applicationSettingsId, string entryDataId);
    }
}