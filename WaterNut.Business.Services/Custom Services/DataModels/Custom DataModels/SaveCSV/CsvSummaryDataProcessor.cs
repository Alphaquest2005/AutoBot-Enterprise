using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class CsvSummaryDataProcessor
    {
        private readonly RiderImporter _riderImporter;
        private readonly ManifestImporter _manifestImporter;
        private readonly BillOfLadenImporter _billOfLadenImporter;
        private readonly FreightImporter _freightImporter;
        private readonly ExpiredEntriesImporter _expiredEntriesImporter;
        private readonly CancelledEntriesImporter _cancelledEntriesImporter;
        private readonly EntryDataProcessor _entryDataImporter;
        private readonly PDFShipmentInvoiceImporter _pdfShipmentInvoiceImporter;
        private readonly CSVShipmentImporter _csvShipmentImporter;

        static CsvSummaryDataProcessor()
        {
        }

        public CsvSummaryDataProcessor()
        {
            _riderImporter = new RiderImporter();
            _manifestImporter = new ManifestImporter();
            _billOfLadenImporter = new BillOfLadenImporter();
            _freightImporter = new FreightImporter();
            _expiredEntriesImporter = new ExpiredEntriesImporter();
            _cancelledEntriesImporter = new CancelledEntriesImporter();
            _entryDataImporter = new EntryDataProcessor();
            _pdfShipmentInvoiceImporter = new PDFShipmentInvoiceImporter();
            _csvShipmentImporter = new CSVShipmentImporter();
        }

        public async Task<bool> ProcessCsvSummaryData(FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> eslst)
        {
            try
            {

                if (docSet.Any(x =>
                        x.ApplicationSettingsId !=
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    throw new ApplicationException("Doc Set not belonging to Current Company");


                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Rider)
                {
                    _riderImporter.ProcessCsvRider(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Manifest)
                {
                    _manifestImporter.ProcessManifest(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.BL)
                {
                    _billOfLadenImporter.ProcessBL(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Freight)
                {
                    _freightImporter.ProcessFreight(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice && fileType.FileImporterInfos.Format == FileTypeManager.FileFormats.Csv)
                {
                    return await _csvShipmentImporter.ImportCSVShipmentInvoice(fileType, docSet, overWriteExisting, emailId, eslst).ConfigureAwait(false);
                    
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice &&
                    fileType.FileImporterInfos.Format == FileTypeManager.FileFormats.PDF)
                {
                    return await _pdfShipmentInvoiceImporter.ImportPDFShipmentInvoice(fileType, docSet, overWriteExisting, emailId, eslst, droppedFilePath).ConfigureAwait(false);
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ExpiredEntries)
                {
                    _expiredEntriesImporter.ProcessCsvExpiredEntries(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.CancelledEntries)
                {
                    _cancelledEntriesImporter.ProcessCsvCancelledEntries(fileType, docSet, overWriteExisting, emailId,
                        droppedFilePath, eslst);
                    return true;
                }


                
                return await _entryDataImporter.ImportEntryData(fileType, docSet, overWriteExisting, emailId, droppedFilePath, eslst).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}