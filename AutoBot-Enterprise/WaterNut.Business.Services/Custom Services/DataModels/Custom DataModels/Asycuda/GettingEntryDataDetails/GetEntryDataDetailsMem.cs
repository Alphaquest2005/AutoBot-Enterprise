using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingEntryDataDetails
{
    public class GetEntryDataDetailsMem : IGetEntryDataDetailsProcessor
    {
        private static readonly ConcurrentDictionary<(int EntryDataDetailsId, int InventoryItemId),EntryDataDetails> _entryDataDetails;
        static readonly object Identity = new object();

        static GetEntryDataDetailsMem()
        {
            lock (Identity)
            {
                if(_entryDataDetails == null)
                    using (var ctx = new AllocationDSContext() { StartTracking = false })
                    {
                        ctx.Database.CommandTimeout = 0;
                        ctx.Configuration.ValidateOnSaveEnabled = false;
                        ctx.Configuration.AutoDetectChangesEnabled = false;


                        var lst = ctx.EntryDataDetails
                            .AsNoTracking()
                            .Include(x => x.Sales)
                            .Include(x => x.Adjustments)
                            .Include(x => x.AsycudaSalesAllocations)
                            .Include(x => x.ManualAllocations)
                            .Where(x => x.EntryData.EntryDataDate >=
                                        (WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.AllocationsOpeningStockDate 
                                            ?? WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate)
                                        && x.EntryData.ApplicationSettingsId ==
                                        WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                        && x.Quantity != x.QtyAllocated
                                        && x.DoNotAllocate != true)
                            .ToDictionary(x => (x.EntryDataDetailsId, x.InventoryItemId), x => x);
                        _entryDataDetails =
                            new ConcurrentDictionary<(int EntryDataDetailsId, int InventoryItemId), EntryDataDetails>(lst);
                    }
            }
        }

        public static List<EntryDataDetails> Execute(List<(string ItemNumber, int InventoryItemId)> lst)
        {
            var list = lst.Select(z => z.InventoryItemId).ToList();
           return _entryDataDetails
                .Join(list, e => e.Key.InventoryItemId, i => i, (e,i) => e)
                .Select(x => x.Value)
                .ToList();
        }
    }
}