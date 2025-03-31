using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.ExistingEntryDataProcessing;
using WaterNut.DataSpace;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.EntryDataCreating
{
    public class EntryDataCreator
    {
       
        public async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetSaveEntryData(
            FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            RawEntryDataValue item)
        {
            (dynamic existingEntryData, List<EntryDataDetails> details) existingEntryData = await new ExistingEntryDataProcessor().GetExistingEntryData(docSet, overWriteExisting, item).ConfigureAwait(false);
            var entryData = existingEntryData.existingEntryData
                            ?? await new NewEntryDataProcessor().Execute(fileType, docSet, item)
                                .ConfigureAwait(false);
            return (entryData, existingEntryData.details);
        }

        public async Task<List<(dynamic existingEntryData, List<EntryDataDetails> details)>> GetEntryData(
            FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            List<RawEntryData> itemList)
        {
           var existingEntryDataTuples = new ExistingEntryDataMem().GetExistingEntryData(docSet, overWriteExisting, itemList);
           
           // Select tasks for items that need new entry data created
           var tasks = existingEntryDataTuples
               .Where(x => x.existingEntryData == null)
               .Select(async x => (existingEntryData: (dynamic)await new NewEntryDataProcessorNoSave().Execute(fileType, docSet, x.rawItem).ConfigureAwait(false), details: x.details))
               .ToList();

            // Await all tasks and collect results
           var newEntryDataResults = await Task.WhenAll(tasks).ConfigureAwait(false);
           var newEntryData = newEntryDataResults.ToList();

           // Combine with existing data that didn't need processing (if any)
           var existingProcessedData = existingEntryDataTuples
                .Where(x => x.existingEntryData != null)
                .Select(x => (existingEntryData: (dynamic)x.existingEntryData, details: x.details))
                .ToList();
            
           newEntryData.AddRange(existingProcessedData);

           return newEntryData;

        }
    }
}