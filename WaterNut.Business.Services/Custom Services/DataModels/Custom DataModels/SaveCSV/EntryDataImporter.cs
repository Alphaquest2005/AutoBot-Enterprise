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

                
                var emptyDetailsLst = entryDataLst.Where(x => !x.EntryDataDetails.Any()).ToList();
                var errlst = emptyDetailsLst.Select(x => new { DataFile = x ,Error = new ApplicationException(x.EntryData.EntryDataId + " has no details")}).ToList();
                if (errlst.Any())
                {

                }

                var goodLst = entryDataLst
                    .Where(x => x.EntryDataDetails.Any())
                    .Where(x => x.EntryData.EntryDataId != null && x.EntryData.EntryDataDate != null)
                    .ToList();

                foreach (var item in goodLst)
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
