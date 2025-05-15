using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.RawEntryDataProcessing;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class SaveRawEntryData : IProcessor<RawEntryData>
    {
        private readonly ImportSettings _importSettings;
 
        public SaveRawEntryData(ImportSettings importSettings)
        {
            _importSettings = importSettings;
            
        }
 
        public async Task<Result<List<RawEntryData>>> Execute(List<RawEntryData> data, ILogger log)
        {
            var dataFile = new DataFile(_importSettings.FileType, _importSettings.DocSet, _importSettings.OverWrite,
                _importSettings.EmailId, _importSettings.DroppedFilePath, new List<dynamic>());
            await new RawEntryDataProcessor().CreateEntryData(dataFile, data).ConfigureAwait(false);
            return new Result<List<RawEntryData>>(data, true, "") ;
        }
    }
}