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

        

        public async Task<bool> Process(DataFile dataFile)
        {
            if (dataFile.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice &&
                dataFile.FileType.FileImporterInfos.Format == FileTypeManager.FileFormats.Csv)
            {
                var itm = (IDictionary<string, object>)dataFile.Data.FirstOrDefault(); //&& itm.Keys.Contains("EntryDataId")
                var entrydataid = itm["EntryDataId"];
                var xeslst = _csvToShipmentInvoiceConverter.ConvertCSVToShipmentInvoice(dataFile.Data);
                var xdroppedFilePath = new CoreEntitiesContext().Attachments.Where(x =>
                        x.FilePath.Contains(entrydataid + ".pdf")).OrderByDescending(x => x.Id).FirstOrDefault()
                    ?.FilePath;
                if (xeslst == null) return false;
                await _inventoryImporter.ImportInventory(
                    xeslst.SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList(),
                    dataFile.DocSet.First().ApplicationSettingsId, dataFile.FileType).ConfigureAwait(false);
                var invoicePOs = xeslst.SelectMany(x =>
                        ((List<IDictionary<string, object>>)x).Select(z =>
                            new { InvoiceNo = z["InvoiceNo"], PONumber = z["PONumber"] }))
                    .Distinct()
                    .ToDictionary(x => x.InvoiceNo.ToString(), x => x.PONumber?.ToString() ?? "");
                _shipmentInvoiceImporter.ProcessShipmentInvoice(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, dataFile.EmailId,
                    xdroppedFilePath, xeslst, invoicePOs);

                return true;
            }

            return false;
        }
    }
}