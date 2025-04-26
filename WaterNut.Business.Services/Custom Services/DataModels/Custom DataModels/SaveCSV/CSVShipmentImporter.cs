using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using TrackableEntities; // Added for TrackingState
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



        public async Task<bool> ProcessAsync(DataFile dataFile)
        {
            try
            {


                if (dataFile.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice &&
                    dataFile.FileType.FileImporterInfos.Format == FileTypeManager.FileFormats.Csv)
                {
                    var itm = (IDictionary<string, object>)dataFile.Data
                        .FirstOrDefault(); //&& itm.Keys.Contains("EntryDataId")

                    if (itm == null) return false;
                    if (!itm.ContainsKey("EntryDataId")) return false;

                    var entrydataid = itm["EntryDataId"];

                    var xeslst = _csvToShipmentInvoiceConverter.ConvertCSVToShipmentInvoice(dataFile.Data);

                    var xdroppedFilePath = new CoreEntitiesContext().Attachments.Where(x =>
                            x.FilePath.Contains(entrydataid + ".pdf")).OrderByDescending(x => x.Id).FirstOrDefault()
                        ?.FilePath;

                    //if (string.IsNullOrEmpty(xdroppedFilePath)) return false;
                    if (xeslst == null) return false;

                    var invoicePOsData = xeslst.SelectMany(x =>
                            ((List<IDictionary<string, object>>)x).Select(z =>
                                new { InvoiceNo = z["InvoiceNo"], PONumber = z["PONumber"] }))
                        .Distinct()
                        .Where(x => !string.IsNullOrEmpty(x.InvoiceNo?.ToString()))
                        .ToList();



                    var invoicePOs = new Dictionary<string, string>();

                    foreach (var ip in invoicePOsData.GroupBy(x => x.InvoiceNo))
                    {
                        foreach (var a in ip)
                        {
                            if (ip.Count() > 1 && a.InvoiceNo == a.PONumber) continue;
                            invoicePOs.Add(a.InvoiceNo.ToString(), a.PONumber?.ToString() ?? "");
                        }
                    }

                    if (!invoicePOs.Any()) return false;


                    var file = new DataFile(
                        dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, dataFile.EmailId,
                        dataFile.DroppedFilePath, xeslst.SelectMany(x =>
                            ((List<IDictionary<string, object>>)x).Select(z => z["InvoiceDetails"])).SelectMany(x =>
                            ((List<IDictionary<string, object>>)x).Select(z => (dynamic)z)).ToList());

                    await _inventoryImporter.ImportInventory(file).ConfigureAwait(false);




                    await _shipmentInvoiceImporter.ProcessShipmentInvoice(dataFile.FileType, dataFile.DocSet,
                        dataFile.OverWriteExisting, dataFile.EmailId,
                        xdroppedFilePath ?? dataFile.DroppedFilePath, xeslst, invoicePOs).ConfigureAwait(true);

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
                //return false;
            }

           
        }


    }
}