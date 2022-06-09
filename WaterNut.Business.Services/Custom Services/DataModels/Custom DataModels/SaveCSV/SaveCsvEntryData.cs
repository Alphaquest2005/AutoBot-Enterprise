using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class SaveCsvEntryData
    {
        private static readonly SaveCsvEntryData instance;

        static SaveCsvEntryData()
        {
            instance = new SaveCsvEntryData();
        }

        public static SaveCsvEntryData Instance
        {
            get { return instance; }
        }

        

        public async Task<bool> ExtractEntryData(FileTypes suggestedfileType, string[] lines, string[] headings, 
            List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, 
            string droppedFilePath)
        {
            try
            {
                var fileType =FileTypeManager.GetHeadingFileType(headings, suggestedfileType);

                var data = new CSVDataExtractor(fileType, lines, headings, emailId).Execute();

                if (data == null) return true;

                var dataFile = new DataFile(fileType, docSet, overWriteExisting, emailId, droppedFilePath, data);

                return await _summaryDataProcessor.ProcessCsvSummaryData(dataFile).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var nex = new ApplicationException($"Error Importing File: {droppedFilePath} - {e.Message}", e);
                Console.WriteLine(nex);
                throw nex;
            }
        }


        // new List<IDictionary<string, object>>(){(IDictionary<string, object>) header


        private readonly SummaryDataProcessor _summaryDataProcessor;

        public SaveCsvEntryData()
        {
    
            _summaryDataProcessor = new SummaryDataProcessor();
        }
    }
}