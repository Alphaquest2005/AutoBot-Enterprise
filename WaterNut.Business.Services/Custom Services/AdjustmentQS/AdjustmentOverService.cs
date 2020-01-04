using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;
using System.Linq.Dynamic;
using InventoryItemsEx = EntryDataDS.Business.Entities.InventoryItemsEx;

namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentOverService
    {
        public async Task CreateOPS(string filterExpression, bool perInvoice, int asycudaDocumentSetId)
        {
            try
            {


                var docSet =
                    await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                        .ConfigureAwait(false);

                // inject custom procedure in docset
                docSet.Customs_Procedure = BaseDataModel.Instance.Customs_Procedures.First(x =>
                    x.DisplayName == BaseDataModel.Instance.ExportTemplates.First(z => z.Description == "OS7")
                        .Customs_Procedure);

                docSet.Document_Type = docSet.Customs_Procedure.Document_Type;

                using (var ctx = new AdjustmentQSContext())
                {
                   

                    var olst = ctx.AdjustmentOvers
                        .Include("AdjustmentEx.AsycudaDocumentSets.SystemDocumentSet")
                        .Where(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId)
                        .Where(filterExpression)
                        .Where(x => !x.AsycudaDocumentItemEntryDataDetails.Any())
                        .Where(x => (double)x.Cost > 0)
                        .Where(x => (x.EffectiveDate != null || x.EffectiveDate > DateTime.MinValue))
                        .OrderBy(x => x.EffectiveDate)
                        .GroupJoin(ctx.SystemDocumentSets, a => a.AsycudaDocumentSetId, s => s.Id, (x, y) => new { adjustment = x, sys = y.FirstOrDefault() })
                        .Where(x => x.sys == null)
                        .Select(x => new EntryDataDetails()
                        {
                            EntryDataDetailsId = x.adjustment.EntryDataDetailsId,
                            EntryDataId = x.adjustment.EntryDataId,
                            ItemNumber = x.adjustment.ItemNumber,
                            ItemDescription = x.adjustment.ItemDescription,
                            Cost = (double)x.adjustment.Cost,
                            Quantity = (double)x.adjustment.ReceivedQty - (double)x.adjustment.InvoiceQty,
                            EffectiveDate = x.adjustment.EffectiveDate ?? x.adjustment.AdjustmentEx.InvoiceDate,
                            LineNumber = x.adjustment.LineNumber,
                            EntryData = new EntryData()
                            {
                                EntryDataId = x.adjustment.EntryDataId,
                                Currency = x.adjustment.AdjustmentEx.Currency,
                                EntryDataDate = x.adjustment.AdjustmentEx.InvoiceDate,
                                EmailId = x.adjustment.AdjustmentEx.EmailId,
                                FileTypeId = x.adjustment.AdjustmentEx.FileTypeId
                            },
                            InventoryItemEx = new InventoryItemsEx()
                            {
                                TariffCode = x.adjustment.TariffCode,
                                ItemNumber = x.adjustment.ItemNumber,
                                Description = x.adjustment.ItemDescription,
                            }


                        }).ToList().GroupBy(x => x.EffectiveDate.GetValueOrDefault().ToString("MM-yyyy"));
                    foreach (var set in olst)
                    {
                        await BaseDataModel.Instance.CreateEntryItems(set.ToList(), docSet, perInvoice, false, true, false)
                        .ConfigureAwait(false);
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}