using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.CreatingEntryData
{
    public class CreateEntryDataSetBased : ICreateEntryDataProcessor
    {
        public async Task Execute(DataFile dataFile, List<RawEntryData> goodLst)
        {

            if (!goodLst.Any()) return;

            try
            { 
                new InventoryItemsAliasProcessor().UpdateInventoryItems(goodLst);
                // Await the asynchronous call to get the actual list of tuples
                // Each tuple is expected to be (dynamic existingEntryData, List<EntryDataDetails> details)
                var entryDataTuples = await new EntryDataCreator()
                    .GetEntryData(dataFile.FileType, dataFile.DocSet, dataFile.OverWriteExisting, goodLst)
                    .ConfigureAwait(false); // Added ConfigureAwait(false)

                // --- Process EntryData ---
                // Select the 'existingEntryData' part from each tuple
                var extractedEntryData = entryDataTuples.Select(tuple => tuple.existingEntryData).ToList();

                // Filter the extracted EntryData list using the new variable name
                var existingEntries = extractedEntryData.Where(ed => ed.EntryData_Id > 0).ToList();
                var newEntries = extractedEntryData.Where(ed => ed.EntryData_Id == 0).ToList();

                // Use the filtered lists for bulk operations
                new EntryDataDSContext().BulkMerge<EntryData>(existingEntries, o => o.IncludeGraph = true);
                new EntryDataDSContext().BulkInsert<EntryData>(newEntries, o => o.IncludeGraph = true);

                // --- Process EntryDataDetails ---
                // Use the original tuple list 'entryDataTuples'
                var allEntryDataDetails = entryDataTuples
                        .Select(entryDataTuple => new EntryDataDetailsCreator()
                        .CreateEntryDataDetails(entryDataTuple)) // Pass the tuple
                        .SelectMany(detailsList => detailsList) // Flatten the list of lists
                        .ToList();
               
                // Filter the details list
                var existingEntryDataDetails = allEntryDataDetails.Where(x => x.EntryDataDetailsId > 0).ToList();
                var newEntryDataDetails = allEntryDataDetails.Where(x => x.EntryDataDetailsId == 0).ToList();

                new EntryDataDSContext().BulkMerge(existingEntryDataDetails);
                new EntryDataDSContext().BulkInsert(newEntryDataDetails);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DataSpace.BaseDataModel.EmailExceptionHandler(e);

            }


        }
    }
}