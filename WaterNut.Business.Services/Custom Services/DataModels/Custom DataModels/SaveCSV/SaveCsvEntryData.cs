using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class SaveCsvEntryData : IRawDataExtractor
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



        public async Task Extract(RawDataFile rawDataFile)
        {
            try
            {
                var dataFile = CreateCSVDataFile(rawDataFile);

                await new DataFileProcessor().Process(dataFile).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var nex = new ApplicationException($"Error Importing File: {rawDataFile.DroppedFilePath} - {e.Message}", e);
                Console.WriteLine(nex);
                throw nex;
            }
        }

        private static DataFile CreateCSVDataFile(RawDataFile rawDataFile)
        {

            var fileType = FileTypeManager.GetHeadingFileType(rawDataFile.Headings, rawDataFile.FileType);

            var data = new CSVDataExtractor(fileType, rawDataFile.Lines, rawDataFile.Headings, rawDataFile.EmailId).Execute();

            var dataFile = new DataFile(fileType, rawDataFile.DocSet, rawDataFile.OverWriteExisting, rawDataFile.EmailId, rawDataFile.DroppedFilePath, data, null);
            return dataFile;
        }
    }
}