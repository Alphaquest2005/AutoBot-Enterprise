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
 
        public Task<Result<List<RawEntryData>>> Execute(List<RawEntryData> data)
        {
            return  Task.FromResult(new Result<List<RawEntryData>>(RawEntryDataExtractor.CreateRawEntryData(this._lines, this._importSettings.DocSet, this._importSettings.EmailId, this._importSettings.FileType, this._importSettings.DroppedFilePath),true,""));
        }
    }
}