using System.Collections.Generic;
using System.Linq;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.AddingToDocSet
{
    public class AddToDocSetSelector : IAddToDocSetProcessor
    {
        private readonly bool isDBMem = false;

        public void Execute(List<AsycudaDocumentSet> docSet, EntryData entryData)
        {
            if (isDBMem)
                new AddToDocSet().Execute(docSet, entryData);
            else
                new AddToDocSetMem().Execute(docSet, entryData);
        }
    }
}