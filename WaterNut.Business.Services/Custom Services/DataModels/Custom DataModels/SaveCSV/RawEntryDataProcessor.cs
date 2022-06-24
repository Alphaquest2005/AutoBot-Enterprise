using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public class RawEntryDataProcessor
    {
        public static List<RawEntryData> GetValidRawEntryData(List<RawEntryData> entryDataLst)
        {
            var goodLst = entryDataLst
                .Where(x => x.Item.EntryDataDetails.Any())
                .Where(x => x.Item.EntryData.EntryDataId != null && x.Item.EntryData.EntryDataDate != null)
                .ToList();
            return goodLst;
        }

        public static async Task CreateEntryData(DataFile dataFile, List<RawEntryData> goodLst)
        {
            foreach (RawEntryData item in goodLst)
            {
                var entryData = await new EntryDataCreator()
                    .GetSaveEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, item)
                    .ConfigureAwait(false);

                await new EntryDataDetailsCreator().SaveEntryDataDetails(dataFile.DocSet, dataFile.OverWriteExisting, entryData)
                    .ConfigureAwait(false);

                InventoryItemsProcessor.UpdateInventoryItems(item);
            }
        }
    }
}