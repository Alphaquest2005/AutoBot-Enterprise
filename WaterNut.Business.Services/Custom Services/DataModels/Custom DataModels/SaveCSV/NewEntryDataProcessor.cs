using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class NewEntryDataProcessor
    {
        private EntryDataCreator _entryDataCreator;

        private Dictionary<string, IEntryDataCreator> entryDataCreators = new Dictionary<string, IEntryDataCreator>()
        {
            {FileTypeManager.EntryTypes.Sales, new SalesCreator()},
            {FileTypeManager.EntryTypes.Inv, new InvoiceCreator()},
            {FileTypeManager.EntryTypes.Po, new POCreator()},
            {FileTypeManager.EntryTypes.ShipmentInvoice, new POCreator()},
            {FileTypeManager.EntryTypes.Ops, new OPSCreator()},
            {FileTypeManager.EntryTypes.Adj , new ADJCreator(FileTypeManager.EntryTypes.Adj)},
            {FileTypeManager.EntryTypes.Dis ,new ADJCreator(FileTypeManager.EntryTypes.Dis) },
            {FileTypeManager.EntryTypes.Rcon , new ADJCreator(FileTypeManager.EntryTypes.Rcon)},

        };

      

        public async Task<EntryData> GetNewEntryData(FileTypes fileType, List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId,
                dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId,
                dynamic DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation,
                dynamic Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails>
                EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    ,
                    double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems)  item)
        {
            int applicationSettingsId = item.EntryData.ApplicationSettingsId;
            string entryDataId = item.EntryData.EntryDataId;
            EntryData entryData = null;

            entryData = await entryDataCreators[fileType.FileImporterInfos.EntryType]
                .Create(docSet, item, applicationSettingsId, entryDataId).ConfigureAwait(false);

            return entryData;
        }
    }
}