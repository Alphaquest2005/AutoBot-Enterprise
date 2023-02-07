using System.Collections.Generic;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.ExistingEntryDataProcessing
{
    public interface IExistingEntryDataSetProcessor
    {
        List<(RawEntryDataValue rawItem, dynamic existingEntryData, List<EntryDataDetails> details)> GetExistingEntryData(
            List<AsycudaDocumentSet> docSet, bool overWriteExisting, List<RawEntryData> itemList);
    }
}