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


        public async Task<bool> ImportPDFShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, List<dynamic> eslst, string droppedFilePath)
        {
            if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice &&
                fileType.FileImporterInfos.Format == FileTypeManager.FileFormats.PDF)
            {
                await _inventoryImporter.ImportInventory(
                    Enumerable.SelectMany<dynamic, object>(eslst, x =>
                        ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(),
                    Enumerable.First<AsycudaDocumentSet>(docSet).ApplicationSettingsId, fileType).ConfigureAwait(false);


                _shipmentInvoiceImporter.ProcessShipmentInvoice(fileType, docSet, overWriteExisting, emailId,
                    droppedFilePath, eslst, null);

                return true;
            }

            return false;
        }
    }
}