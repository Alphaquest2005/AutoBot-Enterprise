﻿using System;
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

        public async Task ProcessDroppedFile(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting)
        {
            try
            {
                
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }

        }
        public  async Task ProcessDroppedFile(string droppedFilePath, FileTypes fileType, bool overWriteExisting)
        {
            try
            {
                var docSet = Utils.GetDocSets(fileType);
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
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


        private async Task SaveCSV(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting)
        {
            var rawDataFile = CreateRawDataFile(droppedFilePath, fileType, docSet, overWriteExisting);

            await extractors[fileType.FileImporterInfos.EntryType].Extract(rawDataFile).ConfigureAwait(false);

        }

        private static RawDataFile CreateRawDataFile(string droppedFilePath, FileTypes fileType,
            List<AsycudaDocumentSet> docSet,
            bool overWriteExisting)
        {
            try
            {
                var csvImporter = new CSVImporter(fileType);

                var lines = csvImporter.GetFileLines(droppedFilePath).ToArray();

                var fixedHeadings = csvImporter.GetHeadings(lines).ToArray();

                var emailId = Utils.GetExistingEmailId(droppedFilePath, fileType);
                var rawDataFile =
                    new RawDataFile(fileType, lines, fixedHeadings, docSet, overWriteExisting, emailId,
                        droppedFilePath);
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
