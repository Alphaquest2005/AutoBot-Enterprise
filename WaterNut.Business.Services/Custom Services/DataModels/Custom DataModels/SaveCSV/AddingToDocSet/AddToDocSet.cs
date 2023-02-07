using System.Collections.Generic;
using System.Linq;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.AddingToDocSet
{
    public class AddToDocSet : IAddToDocSetProcessor
    {
        public void Execute(List<AsycudaDocumentSet> docSet, EntryData entryData)
        {
            foreach (var doc in docSet.DistinctBy(x => x.AsycudaDocumentSetId))
            {
                if ((new EntryDataDSContext()).AsycudaDocumentSetEntryData.FirstOrDefault(
                        x => x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId &&
                             x.EntryData_Id == entryData.EntryData_Id) != null) continue;
                if (entryData.AsycudaDocumentSets.FirstOrDefault(
                        x => x.AsycudaDocumentSetId == doc.AsycudaDocumentSetId) != null) continue;
                entryData.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                {
                    AsycudaDocumentSetId = doc.AsycudaDocumentSetId,
                    EntryData_Id = entryData.EntryData_Id,
                    TrackingState = TrackingState.Added
                });
            }
        }
    }
}