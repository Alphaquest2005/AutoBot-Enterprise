using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public interface INewEntryDataProcessor
    {
        Task<EntryData> Execute(FileTypes fileType, List<AsycudaDocumentSet> docSet, RawEntryDataValue rawEntryData);
    }
}