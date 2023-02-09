using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using MoreLinq.Extensions;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.ExistingEntryDataProcessing
{
    public class ExistingEntryDataMem : IExistingEntryDataSetProcessor
    {
        private static ConcurrentDictionary<(int EntryData_ID, string EntryDataId), EntryData> _entryDataLst = null;

        static readonly object Identity = new object();

        public ExistingEntryDataMem()
        {
            lock (Identity)
            {
                if (_entryDataLst == null)
                {
                    var res = new EntryDataDSContext().EntryData
                        .Include("AsycudaDocumentSets")
                        .Include("EntryDataDetails")
                        .Where(x => x.ApplicationSettingsId == DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        // this was to prevent deleting entrydata from other folders discrepancy with piece here and there with same entry data. but i changed the discrepancy to work with only one folder.
                        //.Where(x => !docSet.Select(z => z.AsycudaDocumentSetId).Except(x.AsycudaDocumentSets.Select(z => z.AsycudaDocumentSetId)).Any())
                        .ToDictionary(x => (x.EntryData_Id, x.EntryDataId), x => x);
                    _entryDataLst = new ConcurrentDictionary<(int EntryData_ID, string EntryDataId), EntryData>(res);
                }
            }
        }

      

        public List<(RawEntryDataValue rawItem, dynamic existingEntryData, List<EntryDataDetails> details)> GetExistingEntryData(List<AsycudaDocumentSet> docSet, bool overWriteExisting, List<RawEntryData> itemList)
        {
            var set = itemList.Select(x => x.Item).ToList();
            var existingEntryDataList = GetExistingEntryData(set);

            return overWriteExisting
                ? ExistingEntryData(existingEntryDataList)
                : OverWriteExistingEntryData(docSet, existingEntryDataList);
        }

        private List<(RawEntryDataValue rawItem, dynamic existingEntryData, List<EntryDataDetails> details)> OverWriteExistingEntryData(List<AsycudaDocumentSet> docSet, List<(RawEntryDataValue Item, KeyValuePair<(int EntryData_ID, string EntryDataId), EntryData> EntryData)> existingEntryDataList)
        {
            existingEntryDataList.ForEach(x => x.EntryData.Value.EmailId = x.Item.EntryData.EmailId);
            return existingEntryDataList
                .Select(x => (rawItem:x.Item,existingEntryData:(dynamic)x.EntryData.Value,
                    details: LoadExistingDetails(docSet, x.EntryData.Value.EntryDataDetails, x.Item.EntryDataDetails.ToList())))
                .ToList();
           
        }

        private List<(RawEntryDataValue rawItem, dynamic existingEntryData, List<EntryDataDetails> details)> ExistingEntryData
        (List<(RawEntryDataValue Item, KeyValuePair<(int EntryData_ID, string EntryDataId), EntryData> EntryData)> existingEntryDataList)
        {
             DeleteExistingEntryData(existingEntryDataList.Select(x => x.EntryData.Value).Where(x => x != null).ToList());
             return existingEntryDataList.Select(x => (rawItem: x.Item, existingEntryData: (dynamic)null, details: x.Item.EntryDataDetails.ToList())).ToList();
            
        }

        private void DeleteExistingEntryData(List<EntryData> existingEntryDataList)
        {
            
                ClearEntryDataDetails(existingEntryDataList);
                DeleteEntryData(existingEntryDataList);
            
        }

        private  List<EntryDataDetails> LoadExistingDetails(List<AsycudaDocumentSet> docSet,
            List<EntryDataDetails> oldDetails, List<EntryDataDetails> newDetails)
        {
            var details = new List<EntryDataDetails>();
            foreach (var doc in docSet)
            {
                var l = 0;
                foreach (var newEntryDataDetails in newDetails)
                {
                    l += 1;

                    var oldEntryDataDetails = GetOldEntryDataDetails(oldDetails, l, newEntryDataDetails, doc);


                    if (oldEntryDataDetails != null && EntryDataDetailsMatch(newEntryDataDetails, oldEntryDataDetails))
                        continue;


                    if (!DetailsContainsNewEntryDataDetails(details, newEntryDataDetails))
                        details.Add(newEntryDataDetails);

                    if (oldEntryDataDetails != null)
                        new EntryDataDetailsService()
                            .DeleteEntryDataDetails(oldEntryDataDetails.EntryDataDetailsId.ToString()).Wait();
                }
            }

            return details;
        }

        private  bool DetailsContainsNewEntryDataDetails(List<EntryDataDetails> details,
            EntryDataDetails newEntryDataDetails)
        {
            return details.FirstOrDefault(x =>
                       x.ItemNumber == newEntryDataDetails.ItemNumber &&
                       x.LineNumber == newEntryDataDetails.LineNumber) !=
                   null;
        }

        private  bool EntryDataDetailsMatch(EntryDataDetails newEntryDataDetails,
            EntryDataDetails oldEntryDataDetails)
        {
            return (Math.Abs(newEntryDataDetails.Quantity - oldEntryDataDetails.Quantity) < .0001 &&
                    Math.Abs(newEntryDataDetails.Cost - oldEntryDataDetails.Cost) < .0001);
        }

        private  EntryDataDetails GetOldEntryDataDetails(List<EntryDataDetails> existingEntryDataDetails, int l,
            EntryDataDetails newEntryDataDetails, AsycudaDocumentSet doc)
        {
            var oldEntryDataDetails = existingEntryDataDetails.FirstOrDefault(x =>
                x.LineNumber == l && x.ItemNumber == newEntryDataDetails.ItemNumber &&
                x.EntryData.AsycudaDocumentSets.Any(z =>
                    z.AsycudaDocumentSetId == doc.AsycudaDocumentSetId));
            return oldEntryDataDetails;
        }

        private List<(RawEntryDataValue Item, KeyValuePair<(int EntryData_ID, string EntryDataId), EntryData> EntryData)> GetExistingEntryData(List<RawEntryDataValue> itemSet) =>
           itemSet
            
                .GroupJoin(_entryDataLst, i =>  i.EntryData.EntryDataId, e => e.Key.EntryDataId, (i,e) => (Item:i, EntryData:e.FirstOrDefault()))
                .ToList();

        private void DeleteEntryData(List<EntryData> items) => new EntryDataDSContext().BulkDelete(items);

        private void ClearEntryDataDetails(List<EntryData> items) => new EntryDataDSContext().BulkDelete(items.SelectMany(x => x.EntryDataDetails).ToList());
    }
}