using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class EntryDataCreator
    {
       
        public async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetSaveEntryData(
            FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            RawEntryData item)
        {
            (dynamic existingEntryData, List<EntryDataDetails> details) existingEntryData = await new ExistingEntryDataProcessor().GetExistingEntryData(docSet, overWriteExisting, item).ConfigureAwait(false);
            var entryData = existingEntryData.existingEntryData
                            ?? await new NewEntryDataProcessor().GetNewEntryData(fileType, docSet, item)
                                .ConfigureAwait(false);
            return (entryData, existingEntryData.details);
        }
    }
}