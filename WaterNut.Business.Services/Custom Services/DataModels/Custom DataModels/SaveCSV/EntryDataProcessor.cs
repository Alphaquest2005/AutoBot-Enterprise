using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class EntryDataProcessor
    {
        private SaveCsvEntryData _saveCsvEntryData;
        private InventoryImporter _inventoryImporter = new InventoryImporter();
        private SupplierImporter _supplierImporter = new SupplierImporter();
        private EntryDataFileImporter _entryDataFileImporter = new EntryDataFileImporter();
        private CSVToShipmentInvoiceConverter _csvToShipmentInvoiceConverter = new CSVToShipmentInvoiceConverter();

        
        
        public async Task<bool> ImportEntryData(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId,
            string droppedFilePath, List<dynamic> eslst)
        {
            await _inventoryImporter.ImportInventory(eslst, docSet.First().ApplicationSettingsId, fileType)
                .ConfigureAwait(false);
            await _supplierImporter.ImportSuppliers(eslst, docSet.First().ApplicationSettingsId, fileType)
                .ConfigureAwait(false);
            await _entryDataFileImporter.ImportEntryDataFile(fileType,
                    eslst.Where(x => !string.IsNullOrEmpty(x.SourceRow)).ToList(),
                    emailId, fileType.Id, droppedFilePath, docSet.First().ApplicationSettingsId)
                .ConfigureAwait(false);
            if (await _csvToShipmentInvoiceConverter._entryDataImporter.ImportEntryData(fileType, eslst, docSet,
                        overWriteExisting, emailId,
                        droppedFilePath)
                    .ConfigureAwait(false)) return true;
            return false;
        }
    }
}