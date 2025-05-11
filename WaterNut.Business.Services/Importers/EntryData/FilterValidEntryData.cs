using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.RawEntryDataProcessing;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    public class FilterValidEntryData : IProcessor<RawEntryData>
    {
        public Task<Result<List<RawEntryData>>> Execute(List<RawEntryData> data)
        {
            var validRawEntryData = new RawEntryDataProcessor().GetValidRawEntryData(data);
            return Task.FromResult(Task.FromResult(new Result<List<RawEntryData>>(validRawEntryData,true,"")).Result); // Wrap in Task.FromResult
        }
    }
}