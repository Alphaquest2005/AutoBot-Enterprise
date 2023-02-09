using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.ExistingEntryDataProcessing
{
    public interface IExistingEntryDataProcessor
    {
        Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetExistingEntryData(
            List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            RawEntryDataValue item);
        
    }
}