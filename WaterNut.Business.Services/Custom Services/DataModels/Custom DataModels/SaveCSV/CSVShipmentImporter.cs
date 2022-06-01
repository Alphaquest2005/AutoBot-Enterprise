using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class CSVShipmentImporter
    {
        private SaveCsvEntryData _saveCsvEntryData;
        private CSVToShipmentInvoiceConverter _csvToShipmentInvoiceConverter = new CSVToShipmentInvoiceConverter();
        private InventoryImporter _inventoryImporter = new InventoryImporter();
        private ShipmentInvoiceImporter _shipmentInvoiceImporter = new ShipmentInvoiceImporter();

        static CSVShipmentImporter()
        {
        }

        

        public async Task<bool> ImportCSVShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId,
            List<dynamic> eslst)
        {
            if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice &&
                fileType.FileImporterInfos.Format == FileTypeManager.FileFormats.Csv)
            {
                var itm = (IDictionary<string, object>)eslst.FirstOrDefault(); //&& itm.Keys.Contains("EntryDataId")
                var entrydataid = itm["EntryDataId"];
                var xeslst = _csvToShipmentInvoiceConverter.ConvertCSVToShipmentInvoice(eslst);
                var xdroppedFilePath = new CoreEntitiesContext().Attachments.Where(x =>
                        x.FilePath.Contains(entrydataid + ".pdf")).OrderByDescending(x => x.Id).FirstOrDefault()
                    ?.FilePath;
                if (xeslst == null) return false;
                await _inventoryImporter.ImportInventory(
                    xeslst.SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(),
                    docSet.First().ApplicationSettingsId, fileType).ConfigureAwait(false);
                var invoicePOs = xeslst.SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z =>
                            new { InvoiceNo = z["InvoiceNo"], PONumber = z["PONumber"] }))
                    .Distinct()
                    .ToDictionary(x => x.InvoiceNo.ToString(), x => x.PONumber?.ToString() ?? "");
                _shipmentInvoiceImporter.ProcessShipmentInvoice(fileType, docSet, overWriteExisting, emailId,
                    xdroppedFilePath, xeslst, invoicePOs);

                return true;
            }

            return false;
        }
    }
}