using System.Collections.Generic;
using Core.Common.Extensions;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class FilterValidEntryData : IProcessor<RawEntryData>
    {
        public Result<List<RawEntryData>> Execute(List<RawEntryData> data)
        {
            var validRawEntryData = RawEntryDataProcessor.GetValidRawEntryData(data);
            return new Result<List<RawEntryData>>(validRawEntryData,true,"");
        }
    }
}