using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using MoreLinq;
using OCR.Business.Services;
using WaterNut.Business.Services.Importers.EntryData;
using WaterNut.Business.Services.Utils;
using Serilog; // Added Serilog using
// using InvoiceReader.InvoiceReader.PipelineInfrastructure; // Removed as PDFImporter moved

namespace WaterNut.Business.Services.Importers
{
    public class FileTypeImporter : IFileTypeImporter
    {
        public  FileTypes FileType { get; private set; }
        private readonly ILogger _logger; // Added private ILogger field

        public FileTypeImporter(FileTypes fileType, ILogger logger) // Added ILogger parameter to constructor
        {
            FileType = fileType;
            _logger = logger; // Assign logger to private field
            _importers = new Dictionary<string, IImporter>()
            {
                {FileTypeManager.FileFormats.Csv, new CSVImporter(FileType, _logger)}, // Pass logger to CSVImporter constructor
                {FileTypeManager.FileFormats.Xlsx, new XLSXImporter(FileType, _logger)}, // Pass logger to XLSXImporter constructor
               // {FileTypeManager.FileFormats.PDF, new PDFImporter(FileType, _logger)}, // Pass logger to PDFImporter constructor

            };
        }
        public async Task Import(string fileName)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]", "ImportFile", $"FileName: {fileName}, FileType: {FileType.Description}");

            try
            {
                var importer = _importers[FileType.FileImporterInfos.Format];
                _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", $"Importer.Import for {FileType.FileImporterInfos.Format}", "ASYNC_EXPECTED");
                var importStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await importer.Import(fileName, true, this._logger).ConfigureAwait(false);
                importStopwatch.Stop();
                _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"Importer.Import for {FileType.FileImporterInfos.Format}", importStopwatch.ElapsedMilliseconds, "Async call completed (await).");

                stopwatch.Stop();
                _logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    "ImportFile", $"Successfully imported file {fileName}", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    "ImportFile", "File Import Execution", stopwatch.ElapsedMilliseconds, ex.Message);
                throw; // Re-throw the exception after logging
            }
        }

        private readonly Dictionary<string, IImporter> _importers;

    }

    public class XLSXImporter : IImporter
    {
        private readonly FileTypes _fileType;
        private readonly ILogger _logger; // Added private ILogger field

        public XLSXImporter(FileTypes fileType, ILogger logger) // Added ILogger parameter to constructor
        {
            _fileType = fileType;
            _logger = logger; // Assign logger to private field
        }

        public async Task Import(string fileName, bool overWrite, ILogger log)
        {
            try
            {
                var result = XLSXUtils.ExtractTables(new FileInfo(fileName));
                var DataSetProcessors = new Dictionary<string, IProcessor<DataTable>>()
                {
                    { FileTypeManager.EntryTypes.ShipmentInvoice, new MisMatchesProcessor(_fileType) },
                    { FileTypeManager.EntryTypes.Po, new XlsxEntryDataProcessor(_fileType, fileName, overWrite, _logger) }, // Pass logger
                    { FileTypeManager.EntryTypes.Sales, new XlsxEntryDataProcessor(_fileType, fileName, overWrite, _logger) }, // Pass logger
                    { FileTypeManager.EntryTypes.Dis, new XlsxEntryDataProcessor(_fileType, fileName, overWrite, _logger) }, // Pass logger
                    { FileTypeManager.EntryTypes.Unknown, new XlsxEntryDataProcessor(_fileType, fileName, overWrite, _logger) }, // Pass logger
                };

                await DataSetProcessors[_fileType.FileImporterInfos.EntryType]
                    .Execute(result.Tables.GetEnumerator().ToList<DataTable>(), log).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }

    
}
