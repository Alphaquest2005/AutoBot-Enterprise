using System;
using System.Collections.Generic;
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
                //ValidateEntryDataProcessor.ValidationChecks(dataFile);


                var entryDataLst = new RawEntryDataExtractor().GetRawEntryData(dataFile);

                
                //var emptyDetailsLst = entryDataLst.Where(x => !x.Item.EntryDataDetails.Any()).ToList();
                //var errlst = emptyDetailsLst.Select(x => new { DataFile = x ,Error = new ApplicationException(x.Item.EntryData.EntryDataId + " has no details")}).ToList();
                //if (errlst.Any())
                //{

                //}

                var goodLst = RawEntryDataProcessor.GetValidRawEntryData(entryDataLst);

                await RawEntryDataProcessor.CreateEntryData(dataFile, goodLst).ConfigureAwait(false); 

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
