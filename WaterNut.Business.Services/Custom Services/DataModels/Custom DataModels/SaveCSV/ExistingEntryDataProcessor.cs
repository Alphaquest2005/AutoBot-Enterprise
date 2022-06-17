using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;

namespace WaterNut.DataSpace
{
    public class ExistingEntryDataProcessor
    {
        public async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> GetExistingEntryData(
            List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            List<EntryData> existingEntryDataList =
                GetExistingEntryData(item.EntryData.EntryDataId, item.EntryData.ApplicationSettingsId);

             return overWriteExisting
                 ? await ExistingEntryData(item, existingEntryDataList)
                 : OverExistingEntryData(docSet, item, existingEntryDataList);
        }

        private static (dynamic existingEntryData, List<EntryDataDetails> details) OverExistingEntryData(List<AsycudaDocumentSet> docSet,
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, List<EntryData> existingEntryDataList)
        {
            var existingEntryData = existingEntryDataList.FirstOrDefault();
            existingEntryData.EmailId = item.EntryData.EmailId;
            return (existingEntryData, details: LoadExistingDetails(docSet, existingEntryData.EntryDataDetails,
                item.EntryDataDetails.ToList()));
        }

        private async Task<(dynamic existingEntryData, List<EntryDataDetails> details)> ExistingEntryData(
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails, IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost, double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages, dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item, List<EntryData> existingEntryDataList)
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

        private static List<EntryDataDetails> LoadExistingDetails(List<AsycudaDocumentSet> docSet,
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