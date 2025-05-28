using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.CSV;
using System.IO;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using MoreLinq.Extensions;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Serilog; // Added Serilog using


namespace WaterNut.DataSpace
{
    public partial class SaveCSVModel
    {
        private static readonly SaveCSVModel instance;
        static SaveCSVModel()
        {
            instance = new SaveCSVModel();
        }

        public static SaveCSVModel Instance
        {
            get { return instance; }
        }

        // Modified to accept ILogger
        public async Task ProcessDroppedFile(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, ILogger log)
        {
            try
            {
                // Pass log to SaveCSV
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting, log).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }

        }
        // Modified to accept optional docSet and ILogger
        public  async Task ProcessDroppedFile(string droppedFilePath, FileTypes fileType, bool overWriteExisting, List<AsycudaDocumentSet> docSet = null, ILogger log = null)
        {
            try
            {
                var currentDocSet = docSet ?? await Utils.GetDocSets(fileType, log).ConfigureAwait(false); // Get docSet if null
                await SaveCSV(droppedFilePath, fileType, currentDocSet, overWriteExisting, log).ConfigureAwait(false); // Pass log to SaveCSV
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }
         
        }

        Dictionary<string, IRawDataExtractor> extractors = new Dictionary<string, IRawDataExtractor>()
        {
            {FileTypeManager.EntryTypes.SubItems, new SaveCsvSubItems()},
            {FileTypeManager.EntryTypes.Po, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.Unknown, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.ShipmentInvoice, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.Sales, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.Dis, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.Adj, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.Ops, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.Rider, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.ExpiredEntries, new SaveCsvEntryData()},
            {FileTypeManager.EntryTypes.CancelledEntries, new SaveCsvEntryData()},

            {FileTypeManager.EntryTypes.ItemHistory, new SaveCsvEntryData()},

        };


        // Modified to accept ILogger
        private async Task SaveCSV(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, ILogger log)
        {
            Console.WriteLine($"SaveCSVModel.SaveCSV: START - File: '{droppedFilePath}', FTID: {fileType.Id}");
            // Pass log to CreateRawDataFile
            var rawDataFile = CreateRawDataFile(droppedFilePath, fileType, docSet, overWriteExisting, log);

            var extractorKey = fileType.FileImporterInfos.EntryType;
            Console.WriteLine($"SaveCSVModel.SaveCSV: Using Extractor Key: '{extractorKey}'");
            if (!extractors.ContainsKey(extractorKey)) throw new ApplicationException($"No extractor found for EntryType '{extractorKey}'");

            await extractors[extractorKey].Extract(rawDataFile).ConfigureAwait(false);
            Console.WriteLine($"SaveCSVModel.SaveCSV: END - Extractor '{extractorKey}' completed.");
        }

        // Modified to accept ILogger
        private static RawDataFile CreateRawDataFile(string droppedFilePath, FileTypes fileType,
            List<AsycudaDocumentSet> docSet,
            bool overWriteExisting, ILogger log)
        {
            try
            {
                Console.WriteLine($"SaveCSVModel.CreateRawDataFile: START - File: '{droppedFilePath}', FTID: {fileType.Id}");
                // Pass log to CSVImporter constructor
                var csvImporter = new CSVImporter(fileType, log);
                Console.WriteLine($"SaveCSVModel.CreateRawDataFile: CSVImporter created with FTID: {fileType.Id}");

                var lines = csvImporter.GetFileLines(droppedFilePath).ToArray();
                Console.WriteLine($"SaveCSVModel.CreateRawDataFile: Read {lines.Length} lines from file.");

                var fixedHeadings = csvImporter.GetHeadings(lines).ToArray();
                Console.WriteLine($"SaveCSVModel.CreateRawDataFile: Extracted {fixedHeadings.Length} headings: [{string.Join(", ", fixedHeadings)}]");

                var emailId = Utils.GetExistingEmailId(droppedFilePath, fileType);
                Console.WriteLine($"SaveCSVModel.CreateRawDataFile: Found EmailId: {emailId ?? "N/A"}");
                var rawDataFile =
                    new RawDataFile(fileType, lines, fixedHeadings, docSet, overWriteExisting, emailId,
                        droppedFilePath);
                Console.WriteLine($"SaveCSVModel.CreateRawDataFile: END - RawDataFile created.");
                return rawDataFile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
