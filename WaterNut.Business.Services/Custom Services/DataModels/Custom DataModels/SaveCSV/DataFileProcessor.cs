using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class DataFileProcessor
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


        private Dictionary<string, Dictionary<string, Action<DataFile>>> _dataFileActions = new Dictionary<string, Dictionary<string, Action<DataFile>>>()
        {
            {FileTypeManager.FileFormats.Csv, CSVDataFileActions.Actions},
            {FileTypeManager.FileFormats.PDF, PDFDataFileActions.Actions}
        };

        public async Task<bool> Process(DataFile dataFile)
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