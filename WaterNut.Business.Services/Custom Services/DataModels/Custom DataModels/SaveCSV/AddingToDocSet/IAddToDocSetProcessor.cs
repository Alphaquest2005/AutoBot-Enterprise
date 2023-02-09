using System.Collections.Generic;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.AddingToDocSet
{
    public interface IAddToDocSetProcessor
    {
        void Execute(List<AsycudaDocumentSet> docSet, EntryData entryData);
    }
}