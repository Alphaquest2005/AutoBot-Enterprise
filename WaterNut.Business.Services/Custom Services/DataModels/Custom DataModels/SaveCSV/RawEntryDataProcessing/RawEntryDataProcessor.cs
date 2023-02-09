using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.RawEntryDataProcessing
{
    public interface IRawEntryDataProcessor
    {
        List<RawEntryData> GetValidRawEntryData(List<RawEntryData> entryDataLst);
        Task CreateEntryData(DataFile dataFile, List<RawEntryData> goodLst);
    }

    public class RawEntryDataProcessor : IRawEntryDataProcessor
    {
       

        public  List<RawEntryData> GetValidRawEntryData(List<RawEntryData> entryDataLst)
        {
            var allowNullEntryDataDate = true;
            var goodLst = entryDataLst
                .Where(x => x.Item.EntryDataDetails.Any())
                .Where(x => x.Item.EntryData.EntryDataId != null && (allowNullEntryDataDate || x.Item.EntryData.EntryDataDate != null))
                .DistinctBy(x => x.Item.EntryData.EntryDataId)
                .ToList();
            return goodLst;
        }

        public  async Task CreateEntryData(DataFile dataFile, List<RawEntryData> goodLst)
        {
            await new CreateEntryDataSelector().Execute(dataFile, goodLst).ConfigureAwait(false);
        }
    }
}