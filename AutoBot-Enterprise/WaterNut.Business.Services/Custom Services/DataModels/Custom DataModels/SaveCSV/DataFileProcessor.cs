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
        private Dictionary<string, Dictionary<string, Func<DataFile, Task<bool>>>> _dataFileActions = new Dictionary<string, Dictionary<string, Func<DataFile, Task<bool>>>>()
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

                return await _dataFileActions
                        [dataFile.FileType.FileImporterInfos.Format]
                    [dataFile.FileType.FileImporterInfos.EntryType]
                    .Invoke(dataFile).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}