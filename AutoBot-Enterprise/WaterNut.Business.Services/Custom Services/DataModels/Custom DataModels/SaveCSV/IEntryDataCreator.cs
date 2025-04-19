using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public interface IEntryDataCreator
    {
        Task<EntryData> CreateAndSave(List<AsycudaDocumentSet> docSet, RawEntryDataValue item,
            int applicationSettingsId, string entryDataId);

        EntryData Create(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item,
            int applicationSettingsId, string entryDataId);

    }
}