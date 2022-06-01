using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Enums;
using DocumentItemDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;
using InventoryItemsEx = EntryDataDS.Business.Entities.InventoryItemsEx;

namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentOverService
    {
        public async Task CreateOPS(string filterExpression, bool perInvoice, string adjustmentType,
            int asycudaDocumentSetId)
        {
            await CreateOPS(filterExpression, perInvoice, asycudaDocumentSetId, adjustmentType).ConfigureAwait(false);
        }

        /// <summary>
        ///     one entry for kim
        public async Task CreateOPS(string filterExpression, bool perInvoice, int asycudaDocumentSetId,
            string adjustmentType,
            List<int> entryDataDetailsIds = null, string emailId = null)
        {
            try
            {
                entryDataDetailsIds = entryDataDetailsIds ?? new List<int>();
                var docSet =
                    await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                        .ConfigureAwait(false);
                var cp =
                    BaseDataModel.Instance.Customs_Procedures
                        .Single(x =>
                            x.CustomsOperationId == (int) CustomsOperations.Warehouse && x.Discrepancy == true &&
                            x.IsPaid == true);

                var exportTemplate =
                    BaseDataModel.Instance.ExportTemplates.First(z => z.Customs_Procedure == cp.CustomsProcedure);

                docSet.Customs_Procedure = cp;

                // inject custom procedure in docset
                BaseDataModel.ConfigureDocSet(docSet, exportTemplate);
                using (var ctx = new AdjustmentQSContext())
                {
                    ctx.Database.CommandTimeout = 0;
                    IOrderedQueryable<TODO_AdjustmentOversToXML> lst;
                    if (adjustmentType == "DIS")
                        lst = ctx.TODO_AdjustmentOversToXML
                            .Where(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId)
                            .Where(x => x.AsycudaDocumentSetId == asycudaDocumentSetId)
                            .Where(filterExpression)
                            .Where(x => !entryDataDetailsIds.Any() ||
                                        entryDataDetailsIds.Any(z => z == x.EntryDataDetailsId))
                            //.Where(x => (x.EffectiveDate != null || x.EffectiveDate > DateTime.MinValue))
                            .OrderBy(x => x.EntryDataDetailsId);
                    else
                        lst = ctx.TODO_AdjustmentOversToXML
                            .Where(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId)
                            .Where(x => x.AsycudaDocumentSetId == asycudaDocumentSetId)
                            .Where(filterExpression)
                            //.Where(x => !entryDataDetailsIds.Any() || entryDataDetailsIds.Any(z => z == x.EntryDataDetailsId))
                            .Where(x => x.EffectiveDate != null || x.EffectiveDate > DateTime.MinValue)
                            .OrderBy(x => x.EffectiveDate);

                    var olst = lst
                        .Select(x => new EntryDataDetails
                        {
                            EntryDataDetailsId = x.EntryDataDetailsId,
                            EntryDataId = x.EntryDataId,
                            EntryData_Id = x.EntryData_Id,
                            ItemNumber = x.ItemNumber,
                            ItemDescription = x.ItemDescription,
                            Cost = (double) x.Cost,
                            Quantity = Math.Abs((double) x.ReceivedQty - (double) x.InvoiceQty) < 0.0001 &&
                                       (double) x.ReceivedQty > 0
                                ? (double) x.ReceivedQty
                                : (double) x.ReceivedQty - (double) x.InvoiceQty,
                            EffectiveDate = x.EffectiveDate ?? x.InvoiceDate,
                            LineNumber = x.LineNumber,
                            Comment = x.Comment,
                            EntryData = new Adjustments()
                            {
                                EntryDataId = x.EntryDataId,
                                EntryData_Id = x.EntryData_Id,
                                Currency = x.Currency,
                                EntryDataDate = (DateTime) x.InvoiceDate,
                                EmailId = x.EmailId,
                                Vendor = x.Vendor,
                                FileTypeId = x.FileTypeId
                            },
                            InventoryItemEx = new InventoryItemsEx
                            {
                                TariffCode = x.TariffCode,
                                ItemNumber = x.ItemNumber,
                                Description = x.ItemDescription
                            }
                        }).ToList();

                    var docList = await BaseDataModel.Instance
                        .CreateEntryItems(olst, docSet, perInvoice, false, true, false, false, false, "O")
                        .ConfigureAwait(false);

                    if (emailId != null)
                    {
                        BaseDataModel.StripAttachments(docList, emailId);
                        BaseDataModel.AttachEmailPDF(asycudaDocumentSetId, emailId);
                        BaseDataModel.AttachBlankC71(docList);
                    }

                    BaseDataModel.SetInvoicePerline(docList.Select(x => x.Document.ASYCUDA_Id).ToList());
                    BaseDataModel.RenameDuplicateDocumentCodes(docList.Select(x => x.Document.ASYCUDA_Id).ToList());
                    //ConvertFirstInvoicetoWarehouseCode(docList);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static void ConvertFirstInvoicetoWarehouseCode(List<DocumentCT> docList)
        {
            try
            {
                using (var ctx = new DocumentItemDSContext())
                {
                    foreach (var doc in docList)
                    {
                        var itm = doc.DocumentItems.First();
                        var att = ctx.xcuda_Item
                            .Where(x => x.Item_Id == itm.Item_Id)
                            .SelectMany(x => x.xcuda_Attached_documents)
                            .FirstOrDefault(x => x.Attached_document_code == "IV05");

                        if (att == null) continue;
                        att.Attached_document_code = "W02";
                        att.Attached_document_reference = $"W02-{att.Attached_document_reference}";
                        att.TrackingState = TrackingState.Modified;
                        ctx.SaveChanges();
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