using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetRawEntryData : IProcessor<RawEntryData>
    {
        private readonly ImportSettings _importSettings;
        private readonly List<dynamic> _lines;
 
        public GetRawEntryData(ImportSettings importSettings, List<dynamic> lines)
        {
            _importSettings = importSettings;
            _lines = lines;
        }
 
        public async Task<Result<List<RawEntryData>>> Execute(List<RawEntryData> data)
        {
            return  new Result<List<RawEntryData>>(RawEntryDataExtractor.CreateRawEntryData(_lines, _importSettings.DocSet, _importSettings.EmailId, _importSettings.FileType, _importSettings.DroppedFilePath),true,"");
        }
    }
}