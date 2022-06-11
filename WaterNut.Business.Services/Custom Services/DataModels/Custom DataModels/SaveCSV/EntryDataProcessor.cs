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
        private EntryDataImporter _entryDataImporter = new EntryDataImporter();

        
        
        public async Task Process(DataFile dataFile)
        {
            await _inventoryImporter.ImportInventory(dataFile)
                .ConfigureAwait(false);
            await _supplierImporter.ImportSuppliers(dataFile)
                .ConfigureAwait(false);
            await _entryDataFileImporter.ImportEntryDataFile(dataFile)
                .ConfigureAwait(false);
            await _entryDataImporter.ImportEntryData(dataFile).ConfigureAwait(false);
            
        }
    }
}