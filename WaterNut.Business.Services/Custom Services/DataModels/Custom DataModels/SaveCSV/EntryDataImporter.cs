using System;
using System.Linq;
using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public class EntryDataImporter
    {
        
        public async Task ImportEntryData(DataFile dataFile)
        {
            try
            {
                ValidateEntryDataProcessor.ValidationChecks(dataFile);


                var entryDataLst = new RawEntryDataExtractor().GetRawEntryData(dataFile);

                
                var emptyDetailsLst = entryDataLst.Where(x => !x.Item.EntryDataDetails.Any()).ToList();
                var errlst = emptyDetailsLst.Select(x => new { DataFile = x ,Error = new ApplicationException(x.Item.EntryData.EntryDataId + " has no details")}).ToList();
                if (errlst.Any())
                {

                }

                var goodLst = entryDataLst
                    .Where(x => x.Item.EntryDataDetails.Any())
                    .Where(x => x.Item.EntryData.EntryDataId != null && x.Item.EntryData.EntryDataDate != null)
                    .ToList();

                foreach (RawEntryData item in goodLst)
                {
                    var entryData = await  new EntryDataCreator().GetSaveEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, item).ConfigureAwait(false);

                    await new EntryDataDetailsCreator().SaveEntryDataDetails(dataFile.DocSet, dataFile.OverWriteExisting, entryData).ConfigureAwait(false);

                    InventoryItemsProcessor.UpdateInventoryItems(item);
                } 

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
