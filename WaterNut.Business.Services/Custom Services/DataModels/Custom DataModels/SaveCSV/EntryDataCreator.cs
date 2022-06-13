using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class EntryDataCreator
    {
       
        public async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetSaveEntryData(
            FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId,
                dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId,
                dynamic DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation,
                dynamic Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails>
                EntryDataDetails,
            IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost,
                double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            (dynamic existingEntryData, List<EntryDataDetails> details) existingEntryData = await new ExistingEntryDataProcessor().GetExistingEntryData(docSet, overWriteExisting, item).ConfigureAwait(false);
            var entryData = existingEntryData.existingEntryData
                            ?? await new NewEntryDataProcessor().GetNewEntryData(fileType, docSet, item)
                                .ConfigureAwait(false);
            return (entryData, existingEntryData.details);
        }
    }
}