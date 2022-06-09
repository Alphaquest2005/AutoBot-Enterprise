using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class SummaryDataProcessor
    {
        //private readonly RiderImporter _riderImporter;
        //private readonly ManifestImporter _manifestImporter;
        //private readonly BillOfLadenImporter _billOfLadenImporter;
        //private readonly FreightImporter _freightImporter;
        //private readonly ExpiredEntriesImporter _expiredEntriesImporter;
        //private readonly CancelledEntriesImporter _cancelledEntriesImporter;
        //private readonly EntryDataProcessor _entryDataImporter;
        //private readonly PDFShipmentInvoiceImporter _pdfShipmentInvoiceImporter;
        //private readonly CSVShipmentImporter _csvShipmentImporter;

        static SummaryDataProcessor()
        {
        }

        public SummaryDataProcessor()
        {
           
        }

        private static Dictionary<string, Action<DataFile>> _csvDataFileActions = new Dictionary<string, Action<DataFile>>()
        {
            {FileTypeManager.EntryTypes.Rider, (dataFile) => new RiderImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ExpiredEntries, (dataFile) => new ExpiredEntriesImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.CancelledEntries, (dataFile) => new CancelledEntriesImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ShipmentInvoice,async (dataFile) => await new CSVShipmentImporter().Process(dataFile).ConfigureAwait(false)},
            {FileTypeManager.EntryTypes.Po, async (dataFile) => await new EntryDataProcessor().Process(dataFile).ConfigureAwait(false)},
        };

        private static Dictionary<string, Action<DataFile>> _pdfDataFileActions = new Dictionary<string, Action<DataFile>>()
        {
            
            {FileTypeManager.EntryTypes.Manifest, (dataFile) => new ManifestImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.BL, (dataFile) => new BillOfLadenImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Freight, (dataFile) => new FreightImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ShipmentInvoice, async (dataFile) => await new PDFShipmentInvoiceImporter().Process(dataFile).ConfigureAwait(false)},
            
            
        };

        private Dictionary<string, Dictionary<string, Action<DataFile>>> _dataFileActions = new Dictionary<string, Dictionary<string, Action<DataFile>>>()
        {
            {FileTypeManager.FileFormats.Csv, _csvDataFileActions},
            {FileTypeManager.FileFormats.PDF, _pdfDataFileActions}
        };

        public async Task<bool> ProcessCsvSummaryData(DataFile dataFile)
        {
            try
            {

                if (dataFile.DocSet.Any(x =>
                        x.ApplicationSettingsId !=
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    throw new ApplicationException("Doc Set not belonging to Current Company");


                _dataFileActions
                    [dataFile.FileType.FileImporterInfos.Format]
                    [dataFile.FileType.FileImporterInfos.EntryType]
                    .Invoke(dataFile);

                return true;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}