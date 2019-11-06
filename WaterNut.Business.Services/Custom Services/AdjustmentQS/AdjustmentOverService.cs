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
                        .Where(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId)
                        .Where(filterExpression)
                        .Where(x => !x.AsycudaDocumentItemEntryDataDetails.Any())
                        .Where(x => (double)x.Cost > 0)
                        .Where(x => (x.EffectiveDate != null || x.EffectiveDate > DateTime.MinValue))
                        .OrderBy(x => x.EffectiveDate)
                        .Select(x => new EntryDataDetails()
                        {
                            EntryDataDetailsId = x.EntryDataDetailsId,
                            EntryDataId = x.EntryDataId,
                            ItemNumber = x.ItemNumber,
                            ItemDescription = x.ItemDescription,
                            Cost = (double)x.Cost,
                            Quantity = (double) x.ReceivedQty - (double) x.InvoiceQty,
                            EffectiveDate = x.EffectiveDate ?? x.AdjustmentEx.InvoiceDate,
                            LineNumber = x.LineNumber,
                            EntryData = new EntryData()
                            {
                                EntryDataId = x.EntryDataId,
                                Currency = x.AdjustmentEx.Currency,
                                EntryDataDate = x.AdjustmentEx.InvoiceDate,
                                EmailId = x.AdjustmentEx.EmailId,
                                FileTypeId = x.AdjustmentEx.FileTypeId
                            },
                            InventoryItemEx = new InventoryItemsEx()
                            {
                                TariffCode = x.TariffCode,
                                ItemNumber = x.ItemNumber,
                                Description = x.ItemDescription,
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