using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class PDFShipmentInvoiceImporter
    {
        private SaveCsvEntryData _saveCsvEntryData;
        private InventoryImporter _inventoryImporter = new InventoryImporter();
        private ShipmentInvoiceImporter _shipmentInvoiceImporter = new ShipmentInvoiceImporter();


        public async Task<bool> Process(DataFile dataFile)
        {
            if (dataFile.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice &&
                dataFile.FileType.FileImporterInfos.Format == FileTypeManager.FileFormats.PDF)
            {
                await _inventoryImporter.ImportInventory(
                    Enumerable.SelectMany<dynamic, object>(dataFile.Data, x =>
                        ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(),
                    Enumerable.First<AsycudaDocumentSet>(dataFile.DocSet).ApplicationSettingsId, dataFile.FileType).ConfigureAwait(false);


                _shipmentInvoiceImporter.ProcessShipmentInvoice(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, dataFile.EmailId,
                    dataFile.DroppedFilePath, dataFile.Data, null);

                return true;
            }

            return false;
        }
    }
}