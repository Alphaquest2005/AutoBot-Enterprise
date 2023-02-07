using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AdjustmentQS.Business.Entities;
using MoreLinq;

namespace AdjustmentQS.Business.Services
{
    public class GetAllDiscrepancyDetailsMem : IGetAllDiscrepancyDetailsProcessor
    {
        private static ConcurrentDictionary<(int EntryDataDetailsId, int InventoryItemId), AdjustmentDetail>
            _adjustmentDetails;

        private static ConcurrentDictionary<(int EntryDataDetailsId, int InventoryItemId), EntryDataDetail>
            _entryDataDetails;

        static readonly object Identity = new object();

        public GetAllDiscrepancyDetailsMem()
        {
            lock (Identity)
            {
                if (_adjustmentDetails == null)
                    using (var ctx = new AdjustmentQSContext())
                    {
                        ctx.Database.CommandTimeout = 10;

                        var lst = ctx.AdjustmentDetails.AsNoTracking()
                            .Include(x => x.AdjustmentEx)
                            //.Where(x => x.ApplicationSettingsId == applicationSettingsId)
                            .Where(x => x.SystemDocumentSet != null)
                            .Where(x => x.Type == "DIS")
                            .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)

                            .Where(x => !x.ShortAllocations.Any())
                            .OrderBy(x => x.EntryDataDetailsId)
                            .DistinctBy(x => x.EntryDataDetailsId)
                            .ToDictionary(x => (x.EntryDataDetailsId, x.InventoryItemId), x => x);
                        _adjustmentDetails =
                            new ConcurrentDictionary<(int EntryDataDetailsId, int InventoryItemId), AdjustmentDetail>(
                                lst);
                    }

                if (_entryDataDetails == null)
                    using (var ctx = new AdjustmentQSContext())
                    {
                        ctx.Database.CommandTimeout = 10;

                        var lst = ctx.EntryDataDetails.AsNoTracking()
                            .Include(x => x.AdjustmentEx)
                            //.Where(x => x.ApplicationSettingsId == applicationSettingsId)
                            .Where(x => x.AdjustmentEx != null)
                            .Where(x => x.AdjustmentEx.Type == "DIS")
                            .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                            .ToDictionary(x => (x.EntryDataDetailsId, x.InventoryItemId), x => x);
                        _entryDataDetails =
                            new ConcurrentDictionary<(int EntryDataDetailsId, int InventoryItemId), EntryDataDetail>(lst);
                    }
            }
        }

        public List<AdjustmentDetail> Execute(List<(string ItemNumber, int InventoryItemId)> itemList,
            bool overwriteExisting)
        {

            var lst = _adjustmentDetails
                .Join(itemList, a => a.Key.InventoryItemId, i => i.InventoryItemId, (a, i) => a.Value)
                .Where(x => overwriteExisting
                    ? x != null
                    : x.EffectiveDate == null) // take out other check cuz of existing entries 
                .ToList();
            return lst;
        }

        public EntryDataDetail Execute(int sEntryDataDetailsId)
        {
           return _entryDataDetails.FirstOrDefault(x => x.Key.EntryDataDetailsId == sEntryDataDetailsId).Value;
        }
    }
}

