using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using MoreLinq;

namespace WaterNut.Business.Services.Utils.ProcessingDISErrorsForAllocations
{
    public class ProcessDISErrorsForAllocationNoDB
    {
       
        private static ConcurrentDictionary<int, EntryDataDetail> _entryDataDetails;
        static readonly object Identity = new object();

        public ProcessDISErrorsForAllocationNoDB()
        {
            lock (Identity)
            {
                GetEntryDataDetails();
            }
        }

      

        private static void GetEntryDataDetails()
        {
            if (_entryDataDetails == null)
                try
                {
                    // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
                    using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                    {
                        ctx.Database.CommandTimeout = 60;

                        var lst = ctx.EntryDataDetails.Include(x => x.AdjustmentEx).Where(x => x.AdjustmentEx.Type == "DIS").ToList()
                            .OrderBy(x => x.EntryDataDetailsId)
                            .DistinctBy(x => x.EntryDataDetailsId)
                            .ToDictionary(x => x.EntryDataDetailsId, x => x);
                        _entryDataDetails = new ConcurrentDictionary<int, EntryDataDetail>(lst);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
        }


        public async Task<List<EntryDataDetail>> Execute(int applicationSettingsId, string res)
        {
            try
            {
                var elst = res.Split(',',' ').Select(x => Convert.ToInt32(x)).ToList();
                // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
              
               
                
               return GetUpdatedEntryDataDetails(elst);
                
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static List<EntryDataDetail> GetUpdatedEntryDataDetails(List<int> lst)
        {
            StatusModel.StartStatusUpdate("Preparing Discrepancy Errors for Re-Allocation", lst.Count());
            var res = lst
                .Select(s =>
                {
                    var ed = _entryDataDetails.First(x => x.Key == s).Value;
                    ed.EffectiveDate =
                        ed.AdjustmentEx
                            .InvoiceDate; // BaseDataModel.CurrentSalesInfo().Item2; reset to invoicedate because this just wrong duh make sense
                    ed.QtyAllocated = 0;
                    ed.Comment = $@"DISERROR: NoData Found";
                    ed.Status = null;
                    return ed;
                })
                .ToList();
            return res;
        }
    }
}