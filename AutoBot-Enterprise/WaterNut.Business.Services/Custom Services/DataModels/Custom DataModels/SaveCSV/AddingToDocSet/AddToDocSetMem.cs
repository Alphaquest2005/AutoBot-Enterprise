using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.AddingToDocSet
{
    public class AddToDocSetMem : IAddToDocSetProcessor
    {
        private static ConcurrentDictionary<(int AsycudaDocumentSetId, int EntryData_Id), AsycudaDocumentSetEntryData> _asycudaDocumentSetEntryData = null;

        static readonly object Identity = new object();

        public AddToDocSetMem()
        {
            lock (Identity)
            {
                if (_asycudaDocumentSetEntryData == null)
                {
                    var lst = new EntryDataDSContext().AsycudaDocumentSetEntryData.ToDictionary(
                        x => (x.AsycudaDocumentSetId, x.EntryData_Id), x => x);
                    _asycudaDocumentSetEntryData =
                        new ConcurrentDictionary<(int AsycudaDocumentSetId, int EntryData_Id),
                            AsycudaDocumentSetEntryData>(lst);
                }
            }
        }
        public void Execute(List<AsycudaDocumentSet> docSet, EntryData entryData)
        {
            foreach (var doc in docSet.DistinctBy(x => x.AsycudaDocumentSetId))
            {
                if (_asycudaDocumentSetEntryData.FirstOrDefault(
                        x => x.Key.AsycudaDocumentSetId == doc.AsycudaDocumentSetId &&
                             x.Key.EntryData_Id == entryData.EntryData_Id).Value != null) continue;
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