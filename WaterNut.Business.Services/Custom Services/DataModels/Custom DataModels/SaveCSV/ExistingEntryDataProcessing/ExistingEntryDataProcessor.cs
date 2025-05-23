﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.ExistingEntryDataProcessing
{
    public class ExistingEntryDataProcessor : IExistingEntryDataProcessor
    {
        public async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetExistingEntryData(
            List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            RawEntryDataValue item)
        {
            List<EntryData> existingEntryDataList =
                GetExistingEntryData(item.EntryData.EntryDataId, item.EntryData.ApplicationSettingsId);

             return overWriteExisting
                 ? await ExistingEntryData(item, existingEntryDataList).ConfigureAwait(false)
                 : await OverWriteExistingEntryData(docSet, item, existingEntryDataList).ConfigureAwait(false);
        }

        public async Task<List<(dynamic existingEntryData, List<EntryDataDetails> details)>> GetExistingEntryData(List<AsycudaDocumentSet> docSet, bool overWriteExisting, List<RawEntryDataValue> itemList)
        {
             // Create a list of tasks by calling the async overload
            var tasks = itemList
                .Select(item => GetExistingEntryData(docSet, overWriteExisting, item)) // Calls the async overload
                .ToList();
            
            // Await all tasks concurrently
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return results.ToList();
        }

        // Made async
        private static async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> OverWriteExistingEntryData(List<AsycudaDocumentSet> docSet,
            RawEntryDataValue item, List<EntryData> existingEntryDataList)
        {
            var existingEntryData = existingEntryDataList.FirstOrDefault();
            existingEntryData.EmailId = item.EntryData.EmailId;
            var details = await LoadExistingDetails(docSet, existingEntryData.EntryDataDetails,
                            item.EntryDataDetails.ToList()).ConfigureAwait(false);
            return (existingEntryData, details);
        }

        private async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> ExistingEntryData(
            RawEntryDataValue item, List<EntryData> existingEntryDataList)
        {
            await DeleteExistingEntryData(existingEntryDataList).ConfigureAwait(false);
            var details = item.EntryDataDetails.ToList();
            return (null, details);
        }

        private async Task DeleteExistingEntryData(List<EntryData> existingEntryDataList)
        {
            foreach (var itm in existingEntryDataList)
            {
                await ClearEntryDataDetails(itm).ConfigureAwait(false);
                await DeleteEntryData(itm).ConfigureAwait(false);
            }
        }

        // Made async
        private static async Task<List<EntryDataDetails>> LoadExistingDetails(List<AsycudaDocumentSet> docSet,
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
                        await new EntryDataDetailsService() // Assuming DeleteEntryDataDetails returns Task
                            .DeleteEntryDataDetails(oldEntryDataDetails.EntryDataDetailsId.ToString()).ConfigureAwait(false);
                }
            }

            return details;
        }

        private static bool DetailsContainsNewEntryDataDetails(List<EntryDataDetails> details,
            EntryDataDetails newEntryDataDetails)
        {
            return details.FirstOrDefault(x =>
                       x.ItemNumber == newEntryDataDetails.ItemNumber &&
                       x.LineNumber == newEntryDataDetails.LineNumber) !=
                   null;
        }

        private static bool EntryDataDetailsMatch(EntryDataDetails newEntryDataDetails,
            EntryDataDetails oldEntryDataDetails)
        {
            return (Math.Abs(newEntryDataDetails.Quantity - oldEntryDataDetails.Quantity) < .0001 &&
                    Math.Abs(newEntryDataDetails.Cost - oldEntryDataDetails.Cost) < .0001);
        }

        private static EntryDataDetails GetOldEntryDataDetails(List<EntryDataDetails> existingEntryDataDetails, int l,
            EntryDataDetails newEntryDataDetails, AsycudaDocumentSet doc)
        {
            var oldEntryDataDetails = existingEntryDataDetails.FirstOrDefault(x =>
                x.LineNumber == l && x.ItemNumber == newEntryDataDetails.ItemNumber &&
                x.EntryData.AsycudaDocumentSets.Any(z =>
                    z.AsycudaDocumentSetId == doc.AsycudaDocumentSetId));
            return oldEntryDataDetails;
        }

        private static List<EntryData> GetExistingEntryData(string entryDataId, int applicationSettingsId)
        {
            var oldeds = new EntryDataDSContext().EntryData
                .Include("AsycudaDocumentSets")
                .Include("EntryDataDetails")
                .Where(x => x.EntryDataId == entryDataId
                            && x.ApplicationSettingsId == applicationSettingsId)
                // this was to prevent deleting entrydata from other folders discrepancy with piece here and there with same entry data. but i changed the discrepancy to work with only one folder.
                //.Where(x => !docSet.Select(z => z.AsycudaDocumentSetId).Except(x.AsycudaDocumentSets.Select(z => z.AsycudaDocumentSetId)).Any())
                .ToList();
            return oldeds;
        }

        private async Task DeleteEntryData(EntryData olded)
        {
            using (var ctx = new EntryDataService())
            {
                await ctx.DeleteEntryData(olded.EntryData_Id.ToString()).ConfigureAwait(false);
            }
        }

        private async Task ClearEntryDataDetails(EntryData olded)
        {
            using (var ctx = new EntryDataDetailsService())
            {
                foreach (var itm in olded.EntryDataDetails.ToList())
                {
                    await ctx.DeleteEntryDataDetails(itm.EntryDataDetailsId.ToString()).ConfigureAwait(false);
                }
            }
        }

       
    }
}