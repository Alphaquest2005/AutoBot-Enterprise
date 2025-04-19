using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingEntryDataDetails
{
    public class GetEntryDataDetailsOld : IGetEntryDataDetailsProcessor
    {
        private static List<EntryDataDetails> _entryDataDetails;
        static readonly object Identity = new object();

        public static List<EntryDataDetails> Execute(List<(string ItemNumber, int InventoryItemId)> lst, bool redo)
        {
            lock (Identity)
            {
                if (_entryDataDetails != null && redo == false) return _entryDataDetails;

                using (var ctx = new AllocationDSContext() { StartTracking = false })
                {
                    ctx.Database.CommandTimeout = 0;
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    var list = lst.Select(z => z.InventoryItemId).ToList();
                    _entryDataDetails = ctx.EntryDataDetails
                        .Join(list, x => x.InventoryItemId, i => i, (x, i) => x)
                        .AsNoTracking()
                        .Include(x => x.Sales)
                        .Include(x => x.Adjustments)
                        .Include(x => x.AsycudaSalesAllocations)
                        .Include(x => x.ManualAllocations)
                        .Where(x => list.Contains(x.InventoryItemId))
                        .Where(x => x.EntryData.EntryDataDate >=
                                    WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate
                                    && x.EntryData.ApplicationSettingsId ==
                                    WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                    && x.Quantity != x.QtyAllocated
                                    && x.DoNotAllocate != true)
                        .ToList();
                    return _entryDataDetails;
                }
            }
        }
    }
}