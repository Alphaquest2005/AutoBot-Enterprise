using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;

namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentShortService
    {
        public async Task DoAutoMatch(int applicationSettingsId, List<AdjustmentDetail> lst)
        {
            try
            {
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 10;


                    if (!lst.Any()) return;
                    StatusModel.StartStatusUpdate("Matching Shorts To Asycuda Entries", lst.Count());
                    var edLst = new List<EntryDataDetail>();
                    DateTime? minEffectiveDate = null;
                    foreach (var s in lst)
                    {
                        //StatusModel.StatusUpdate("Matching Shorts To Asycuda Entries");
                        var tryCNumber = false;
                        try
                        {
                            ctx.SaveChanges();

                            if (string.IsNullOrEmpty(s.ItemNumber)) continue;
                            var ed = ctx.EntryDataDetails.Include(x => x.AdjustmentEx)
                                .First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                            edLst.Add(ed);
                            ed.Comment = null;
                            if (!string.IsNullOrEmpty(s.PreviousInvoiceNumber))
                            {
                                var aItem = await GetAsycudaDocumentItemForPreviousInvoiceNumber(applicationSettingsId, s, ed, ctx).ConfigureAwait(false);

                                if (!aItem.Any())
                                {
                                    tryCNumber = true;
                                }
                            }

                            if ((tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber)) &&
                                s.InvoiceQty.GetValueOrDefault() > 0 && !string.IsNullOrEmpty(s.PreviousCNumber))
                            {
                                await GetAsycudaDocumentItemForPreviousCNumber(applicationSettingsId, s, ed, ctx).ConfigureAwait(false);
                                // continue;
                            }

                            else if ((tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber)) &&
                                     s.InvoiceQty.GetValueOrDefault() <= 0 && !string.IsNullOrEmpty(s.PreviousCNumber))
                            {
                                var asycudaDocument =
                                    GetAsycudaDocumentInCNumber(applicationSettingsId, s.PreviousCNumber);
                                MatchToAsycudaDocument(asycudaDocument, ed);
                            }

                            if (ed.EffectiveDate == null)
                            {
                                //Set Overs 1st and Shorts to Last of Month
                                //Try match effective date if i find the invoice if not leave it blank

                                // if (ed.EffectiveDate == null) ed.EffectiveDate = s.AdjustmentEx.InvoiceDate;
                                SetEffectiveDate(s, ed);
                            }

                            if (ed.Cost != 0) continue;
                            if (s.InvoiceQty.GetValueOrDefault() > 0)
                                continue; // if invoice > 0 it should have been imported

                            //////////// only apply to Adjustments because they are after shipment... discrepancies have to be provided.
                            if (s.Type != "ADJ") continue;
                            SetAdjustmentCost(applicationSettingsId, ctx, ed);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }


                    SetMiniumnEffectiveDate(edLst);

                    ctx.SaveChanges();
                    StatusModel.StopStatusUpdate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void SetMiniumnEffectiveDate(List<EntryDataDetail> edLst)
        {
            DateTime? minEffectiveDate;
            minEffectiveDate = edLst.Min(x => x.EffectiveDate)
                               ?? edLst.Where(x => x.AdjustmentEx != null).Min(x => x.AdjustmentEx.InvoiceDate);

            foreach (var ed in edLst.Where(x => x.EffectiveDate == null))
            {
                ed.EffectiveDate = minEffectiveDate;
                ed.Status = null;
            }
        }

        private static void SetAdjustmentCost(int applicationSettingsId, AdjustmentQSContext ctx, EntryDataDetail ed)
        {
            var lastItemCost = ctx.AsycudaDocumentItemLastItemCosts
                .Where(x => x.assessmentdate <= ed.EffectiveDate)
                .OrderByDescending(x => x.assessmentdate)
                .FirstOrDefault(x =>
                    x.ItemNumber == ed.ItemNumber &&
                    x.applicationsettingsid == applicationSettingsId);
            if (lastItemCost != null)
                ed.LastCost = (double)lastItemCost.LocalItemCost.GetValueOrDefault();
        }

        private static void SetEffectiveDate(AdjustmentDetail s, EntryDataDetail ed)
        {
            if (s.Type == "DIS")
            {
                var po = new EntryDataDSContext().EntryData.OfType<PurchaseOrders>()
                    .FirstOrDefault(x =>
                        x.EntryDataId ==
                        ed.EntryDataId); // || ed.PreviousInvoiceNumber.EndsWith(x.EntryDataId) Contains too random
                ed.EffectiveDate = po?.EntryDataDate;
            }
            else
            {
                ed.EffectiveDate = s.InvoiceDate;
            }
        }

        private async Task GetAsycudaDocumentItemForPreviousCNumber(int applicationSettingsId, AdjustmentDetail s,
            EntryDataDetail ed, AdjustmentQSContext ctx)
        {
            var aItem = await GetAsycudaEntriesInCNumber(s.PreviousCNumber, s.PreviousCLineNumber,
                    s.ItemNumber)
                .ConfigureAwait(false);

            if (!aItem.Any())
                aItem = await GetAsycudaEntriesInCNumberReference(applicationSettingsId,
                        s.PreviousCNumber, s.ItemNumber)
                    .ConfigureAwait(false);
            MatchToAsycudaItem(s, aItem.OrderBy(x => x.AsycudaDocument.AssessmentDate).ToList(), ed,
                ctx);
        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaDocumentItemForPreviousInvoiceNumber(int applicationSettingsId, AdjustmentDetail s,
            EntryDataDetail ed, AdjustmentQSContext ctx)
        {
            List<AsycudaDocumentItem> aItem;
            if (s.InvoiceQty > 0)
            {
                aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                        s.PreviousInvoiceNumber, s.EntryDataId, s.ItemNumber)
                    .ConfigureAwait(false);
                if (aItem.Any()) MatchToAsycudaItem(s, aItem, ed, ctx);
            }
            else
            {
                aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                        s.PreviousInvoiceNumber, s.EntryDataId)
                    .ConfigureAwait(false);
                if (aItem.Any()) MatchToAsycudaDocument(aItem.First().AsycudaDocument, ed);
            }

            return aItem;
        }
    }
}