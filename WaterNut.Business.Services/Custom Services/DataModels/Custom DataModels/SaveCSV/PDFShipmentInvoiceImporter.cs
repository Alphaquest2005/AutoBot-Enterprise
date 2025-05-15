using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Added for ILogger
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class PDFShipmentInvoiceImporter
    {
        private readonly ILogger _logger; // Added ILogger field
        private InventoryImporter _inventoryImporter = new InventoryImporter();
        private ShipmentInvoiceImporter _shipmentInvoiceImporter = new ShipmentInvoiceImporter();

        public PDFShipmentInvoiceImporter(ILogger logger) // Added ILogger to constructor
        {
            _logger = logger; // Initialize logger
        }

        public async Task<bool> Process(DataFile dataFile)
        {
            try
            {

                if (dataFile.FileType.FileImporterInfos.EntryType != FileTypeManager.EntryTypes.ShipmentInvoice
                    || dataFile.FileType.FileImporterInfos.Format != FileTypeManager.FileFormats.PDF) return false;

                
                 return await _shipmentInvoiceImporter.ProcessShipmentInvoice(dataFile.FileType, dataFile.DocSet,
                    dataFile.OverWriteExisting, dataFile.EmailId,
                    dataFile.DroppedFilePath, dataFile.Data, null, _logger).ConfigureAwait(false); // Pass logger

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                return false;
            }
            
        }

        private Task ImportInventory(DataFile dataFile)
        {
            var file = new DataFile(dataFile.FileType, dataFile.DocSet,dataFile.OverWriteExisting, dataFile.EmailId, dataFile.DroppedFilePath,
                Enumerable.SelectMany<dynamic, object>(dataFile.Data, x =>
                        ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"]))
                    .Where(x => x != null)
                    .SelectMany(x => ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList());

            return _inventoryImporter.ImportInventory( file);
        }
    }
}