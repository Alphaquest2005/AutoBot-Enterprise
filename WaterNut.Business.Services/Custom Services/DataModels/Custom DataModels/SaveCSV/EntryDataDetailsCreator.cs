using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using MoreLinq;
using TrackableEntities;
using TrackableEntities.EF6;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;

namespace WaterNut.DataSpace
{
    public class EntryDataDetailsCreator
    {
        public async Task CreateAndSaveEntryDataDetails(List<AsycudaDocumentSet> docSet, bool overWriteExisting,
            (dynamic existingEntryData, List<EntryDataDetails> details) data)
        {
            try
            {

                using (var ctx = new EntryDataDSContext())
                {
                   var newDetails = CreateEntryDataDetails(data);
                   ctx.EntryDataDetails.AttachRange(newDetails);
                   ctx.SaveChanges();
                    ////////////// took this out because seem like circular logic
                    /// unless it was intended to merge. but not sure so leaving this out

                    //if (!overWriteExisting && data.existingEntryData != null)
                    //{
                    //    //update entrydatadetails
                    //    foreach (var itm in data.details)
                    //    {
                    //        var d = data.details.FirstOrDefault(z =>
                    //            z.ItemNumber == itm.ItemNumber && z.LineNumber == itm.LineNumber &&
                    //            (Math.Abs(z.Cost - itm.Cost) > 0 || Math.Abs(z.Quantity - itm.Quantity) > 0 ||
                    //             Math.Abs(z.TaxAmount.GetValueOrDefault() - itm.TaxAmount.GetValueOrDefault()) >
                    //             0));
                    //        if (d == null || d.Quantity == 0) continue;

                    //        itm.Quantity = d.Quantity;
                    //        itm.Cost = d.Cost;
                    //        itm.TaxAmount = d.TaxAmount;
                    //       ctx.ApplyChanges(itm);
                    //    }
                        
                    //    ctx.SaveChanges();
                    //    AddToDocSet(docSet, data.existingEntryData);
                    //    await UpdateEntryData(data.existingEntryData).ConfigureAwait(false).ConfigureAwait(false);
                    //}

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<EntryDataDetails> CreateEntryDataDetails(
            (dynamic existingEntryData, List<EntryDataDetails> details) data)
        {
            var lineNumber = 0;
            return data.details
                .Select(e =>
                {
                    lineNumber += 1;
                    return new EntryDataDetails(true)
                    {
                        EntryDataId = e.EntryDataId,
                        EntryData_Id = data.existingEntryData.EntryData_Id ?? 0,
                        ItemNumber = ((string)e.ItemNumber).Truncate(20),
                        InventoryItemId = e.InventoryItemId,
                        ItemDescription = e.ItemDescription,
                        Quantity = e.Quantity,
                        Cost = e.Cost,
                        TotalCost = e.TotalCost,
                        Units = e.Units,
                        TrackingState = TrackingState.Added,
                        Freight = e.Freight,
                        Weight = e.Weight,
                        InternalFreight = e.InternalFreight,
                        ReceivedQty = e.ReceivedQty,
                        InvoiceQty = data.existingEntryData.EntryType == "ADJ" && e.InvoiceQty == 0 &&
                                     e.ReceivedQty == 0
                            ? e.Quantity
                            : e.InvoiceQty,
                        CNumber = string.IsNullOrEmpty(e.CNumber) ? null : e.CNumber,
                        CLineNumber = e.CLineNumber,
                        PreviousInvoiceNumber = string.IsNullOrEmpty(e.PreviousInvoiceNumber)
                            ? null
                            : e.PreviousInvoiceNumber,
                        Comment = string.IsNullOrEmpty(e.Comment) ? null : e.Comment,
                        FileLineNumber = e.FileLineNumber ?? lineNumber,
                        LineNumber = e.EntryDataDetailsId == 0 ? lineNumber : e.LineNumber,
                        EffectiveDate = data.existingEntryData.EntryType == "ADJ"
                            ? e.EffectiveDate ?? data.existingEntryData.EntryDataDate
                            : e.EffectiveDate,
                        TaxAmount = e.TaxAmount,
                        VolumeLiters = e.VolumeLiters,
                        Category = string.IsNullOrEmpty(e.Category) ? null : e.Category,
                        CategoryTariffCode = string.IsNullOrEmpty(e.CategoryTariffCode) ? null : e.CategoryTariffCode,
                    };
                })
                .ToList();

        }


  

        private async Task UpdateEntryData(EntryData olded)
        {
            using (var ctx = new EntryDataService())
            {
                await ctx.UpdateEntryData(olded).ConfigureAwait(false);
            }
        }
    }
}