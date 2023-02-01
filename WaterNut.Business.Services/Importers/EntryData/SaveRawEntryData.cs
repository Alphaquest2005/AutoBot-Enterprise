using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveRawEntryData : IProcessor<RawEntryData>
    {
        private readonly ImportSettings _importSettings;

        public SaveRawEntryData(ImportSettings importSettings)
        {
            _importSettings = importSettings;
            
        }

        public Result<List<RawEntryData>> Execute(List<RawEntryData> data)
        {
            var dataFile = new DataFile(_importSettings.FileType, _importSettings.DocSet, _importSettings.OverWrite,
                _importSettings.EmailId, _importSettings.DroppedFilePath, new List<dynamic>());
            new RawEntryDataProcessor().CreateEntryData(dataFile, data).Wait();
            return new Result<List<RawEntryData>>(data, true, "") ;
        }
    }
}