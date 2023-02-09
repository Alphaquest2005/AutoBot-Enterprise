using System;
using System.Collections.Generic;
using System.Linq;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Entities;
using TrackableEntities;

namespace WaterNut.Business.Services.Utils.MatchingToAsycudaItem
{
    public class MatchToAsycudaItemBulkSave 
    {
        private static bool isDBMem = false;

        public void Execute(AdjustmentDetail os, List<AsycudaDocumentItem> alst, EntryDataDetail ed)
        {
            //ed.Status = null;
            //ed.QtyAllocated = 0;
            DateTime? minEffectiveDate = null;
            var remainingShortQty = os.InvoiceQty.GetValueOrDefault() - os.ReceivedQty.GetValueOrDefault();

            if (!alst.Any())
            {
                //Mark here cuz Allocations don't do Discrepancies only Adjustments
                ed.Status = "No Asycuda Item Found";
                //return minEffectiveDate;
            }

            /// took PiQuantity out because then entries can exist already and this just preventing it from relinking
            /// 

            ed.EffectiveDate = alst.FirstOrDefault()?.AsycudaDocument.AssessmentDate;
            minEffectiveDate = ed.EffectiveDate;
            if (alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.DFQtyAllocated.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error
                ed.Status = "Insufficent Quantities";
                //return minEffectiveDate;
            }

            if ((ed.IsReconciled??false) == false && alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.PiQuantity.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                var existingExecution = GetAsycudaDocumentItemEntryDataDetail(ed);

                if (existingExecution == null)
                {
                    ed.Status = "PiQuantity Already eXed-Out";
                   // return minEffectiveDate;
                }

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error

            }


            if (remainingShortQty < 0 && alst.Any())
            {
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = alst.First().AsycudaDocumentId.GetValueOrDefault(),
                    TrackingState = TrackingState.Added

                });
               // return minEffectiveDate;
            } // if overs just mark and return

            foreach (var aItem in alst)
            {
                var pitm = new AdjustmentQSContext().xcuda_Item
                    //   .Include("AsycudaSalesAllocations.EntryDataDetail.AdjustmentEx")
                    .First(x => x.Item_Id == aItem.Item_Id);
                //pitm.DFQtyAllocated = 0;

                //if (Math.Abs(pitm.AsycudaSalesAllocations.Where(x => x.EntryDataDetailsId == os.EntryDataDetailsId)
                //    .Select(x => x.QtyAllocated).Sum() - os.Quantity) < 0.001)
                //    return;

                var asycudaQty =
                    Convert.ToDouble(aItem.ItemQuantity.GetValueOrDefault() - pitm.DFQtyAllocated);///??TODO: why only dfq?

                if (asycudaQty <= 0) continue;

                var osa = CreateAsycudaSalesAllocation(os, aItem);
                 SaveAsycudaSalesAllocation(osa);
                if (asycudaQty >= remainingShortQty)
                {

                    osa.QtyAllocated = remainingShortQty;
                    ed.QtyAllocated += remainingShortQty;
                    pitm.DFQtyAllocated += remainingShortQty;
                    aItem.DFQtyAllocated += remainingShortQty;
                    SaveAsycudaSalesAllocation(osa);
                    SaveEntryDataDetails(ed);
                    SaveAsycudaDocumentItem(pitm);
                    break;
                }
                else
                {

                    osa.QtyAllocated = asycudaQty;
                    ed.QtyAllocated += asycudaQty;
                    pitm.DFQtyAllocated += asycudaQty;
                    aItem.DFQtyAllocated += asycudaQty;
                    remainingShortQty -= asycudaQty;
                    SaveAsycudaSalesAllocation(osa);
                    SaveEntryDataDetails(ed);
                    SaveAsycudaDocumentItem(pitm);
                }

            }
            

           // return minEffectiveDate;

        }

        private static void SaveAsycudaDocumentItem(xcuda_Item pitm)
        {
            new AdjustmentQSContext().BulkUpdate(new List<xcuda_Item>(){pitm});
            //using (var ctx = new AdjustmentQSContext())
            //{
            //    ctx.Database.CommandTimeout = 10;
            //    var res = ctx.xcuda_Item.First(x => x.Item_Id == pitm.Item_Id);
            //    res.DFQtyAllocated = pitm.DFQtyAllocated;
            //    res.DPQtyAllocated = pitm.DPQtyAllocated;
            //    ctx.SaveChanges();
            //}
        }

        private static void SaveEntryDataDetails(EntryDataDetail sitm)
        {
            new AdjustmentQSContext().BulkUpdate(new List<EntryDataDetail>() { sitm });
            //using (var ctx = new AdjustmentQSContext())
            //{
            //    ctx.Database.CommandTimeout = 10;
            //    var res = ctx.xcuda_Item.First(x => x.Item_Id == pitm.Item_Id);
            //    res.DFQtyAllocated = pitm.DFQtyAllocated;
            //    res.DPQtyAllocated = pitm.DPQtyAllocated;
            //    ctx.SaveChanges();
            //}
        }

        private static void SaveAsycudaSalesAllocation(AsycudaSalesAllocation osa)
        {
            if (osa.AllocationId == 0)
                new AdjustmentQSContext().BulkInsert(new List<AsycudaSalesAllocation>() { osa });
            else
                new AdjustmentQSContext().BulkMerge(new List<AsycudaSalesAllocation>() { osa });
            //using (var ctx = new AdjustmentQSContext() { StartTracking = false })
            //{
            //    ctx.Database.CommandTimeout = 10;
            //    var res = ctx.AsycudaSalesAllocations.First(x => x.AllocationId == osa.AllocationId);
            //    res.QtyAllocated = osa.QtyAllocated;
            //    ctx.SaveChanges();
            //}

        }

        //private static AsycudaSalesAllocation SaveAsycudaSalesAllocation(AdjustmentDetail os, AsycudaDocumentItem aItem)
        //{
        //    using (var ctx = new AdjustmentQSContext())
        //    {
        //        ctx.Database.CommandTimeout = 10;
        //        var osa = new AsycudaSalesAllocation(true)
        //        {
        //            PreviousItem_Id = aItem.Item_Id,
        //            EntryDataDetailsId = os.EntryDataDetailsId,
        //            //Status = "Short Shipped",
        //            TrackingState = TrackingState.Added,
        //        };
        //        ctx.AsycudaSalesAllocations.Add(osa);
        //        ctx.SaveChanges();
        //        return osa;
        //    }
        //}

        private static AsycudaDocumentItemEntryDataDetail GetAsycudaDocumentItemEntryDataDetail(EntryDataDetail ed)
        {
            return isDBMem
                ? new GetAsycudaDocumentEntryDataDetail().Execute(ed)
                : new GetAsycudaDocumentEntryDataDetailMem().Execute(ed);

        }


        private static AsycudaSalesAllocation CreateAsycudaSalesAllocation(AdjustmentDetail os, AsycudaDocumentItem aItem)
        {
            
                var osa = new AsycudaSalesAllocation(true)
                {
                    PreviousItem_Id = aItem.Item_Id,
                    EntryDataDetailsId = os.EntryDataDetailsId,
                    TrackingState = TrackingState.Added,
                };
              
                return osa;
           
        }
    }
}