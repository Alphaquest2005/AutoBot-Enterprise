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

        
        
        public async Task<bool> Process(DataFile dataFile)
        {
            await _inventoryImporter.ImportInventory(dataFile.Data, dataFile.DocSet.First().ApplicationSettingsId, dataFile.FileType)
                .ConfigureAwait(false);
            await _supplierImporter.ImportSuppliers(dataFile.Data, dataFile.DocSet.First().ApplicationSettingsId, dataFile.FileType)
                .ConfigureAwait(false);
            await _entryDataFileImporter.ImportEntryDataFile(dataFile.FileType,
                    dataFile.Data.Where(x => !string.IsNullOrEmpty(x.SourceRow)).ToList(),
                    dataFile.EmailId, dataFile.FileType.Id, dataFile.DroppedFilePath, dataFile.DocSet.First().ApplicationSettingsId)
                .ConfigureAwait(false);
            if (await _csvToShipmentInvoiceConverter._entryDataImporter.ImportEntryData(dataFile.FileType, dataFile.Data, dataFile.DocSet,
                        dataFile.OverWriteExisting, dataFile.EmailId,
                        dataFile.DroppedFilePath)
                    .ConfigureAwait(false)) return true;
            return false;
        }
    }
}