using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryDataSetBased : ICreateEntryDataProcessor
    {
        public async Task Execute(DataFile dataFile, List<RawEntryData> goodLst)
        {



            try
            {
                var entryDataLst = new EntryDataCreator()
                    .GetEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, goodLst);

                await new EntryDataDSContext().BulkMergeAsync(entryDataLst.Select(x => x.existingEntryData).ToList()).ConfigureAwait(false);

                var entryDataWithDetailsLst = entryDataLst
                        .Select(entryData => new EntryDataDetailsCreator()
                        .CreateEntryDataDetails(entryData))
                        .SelectMany(x => x.ToList())
                        .ToList();

                await new EntryDataDSContext().BulkMergeAsync(entryDataWithDetailsLst).ConfigureAwait(false);

                new InventoryItemsAliasProcessor().UpdateInventoryItems(goodLst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DataSpace.BaseDataModel.EmailExceptionHandler(e);

            }


        }
    }
}