using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using Core.Common.UI;
using MoreLinq;

namespace WaterNut.Business.Services.Utils.ProcessingDISErrorsForAllocations
{
    public class ProcessDISErrorsForAllocation 
    {
        public async Task Execute(int applicationSettingsId, string res)
        {
            try
            {
                // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 60;

                    var lst = ctx.TODO_PreDiscrepancyErrors.ToList()
                        .OrderBy(x => x.EntryDataDetailsId)
                        .DistinctBy(x => x.EntryDataDetailsId)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                    && res.Contains(x.EntryDataDetailsId.ToString()))
                        //.Where(x => x.ItemNumber == "HEL/003361001")
                        .ToList();

                    // looking for 'INT/YBA473/3GL'
                    Execute(lst, ctx);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static void Execute(List<TODO_PreDiscrepancyErrors> lst, AdjustmentQSContext ctx)
        {
            StatusModel.StartStatusUpdate("Preparing Discrepancy Errors for Re-Allocation", lst.Count());
            foreach (var s in lst.Where(x =>  x.IsReconciled != true))//x.ReceivedQty > x.InvoiceQty &&
            {
                var ed = ctx.EntryDataDetails.Include(x => x.AdjustmentEx).First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                ed.EffectiveDate = ed.AdjustmentEx.InvoiceDate;// BaseDataModel.CurrentSalesInfo().Item2; reset to invoicedate because this just wrong duh make sense
                ed.QtyAllocated = 0;
                ed.Comment = $@"DISERROR:{s.Status}";
                ed.Status = null;
                ctx.Database.ExecuteSqlCommand(
                    $"delete from AsycudaSalesAllocations where EntryDataDetailsId = {ed.EntryDataDetailsId}");
            }

            ctx.SaveChanges();
        }
    }
}