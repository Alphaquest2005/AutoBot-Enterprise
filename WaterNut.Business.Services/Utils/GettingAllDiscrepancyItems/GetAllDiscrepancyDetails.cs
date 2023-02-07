using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AdjustmentQS.Business.Entities;
using MoreLinq;

namespace AdjustmentQS.Business.Services
{
    public class GetAllDiscrepancyDetails : IGetAllDiscrepancyDetailsProcessor
    {
        public List<AdjustmentDetail> Execute(List<(string ItemNumber, int InventoryItemId)> itemList, bool overwriteExisting)
        {
       
            using (var ctx = new AdjustmentQSContext())
            {
                ctx.Database.CommandTimeout = 10;

                var lst = ctx.AdjustmentDetails.AsNoTracking()
                    .Include(x => x.AdjustmentEx)
                    //.Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    .Where(x => x.SystemDocumentSet != null)
                    .Where(x => x.Type == "DIS")
                    .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                    .Where(x => overwriteExisting
                        ? x != null
                        : x.EffectiveDate == null) // take out other check cuz of existing entries 
                    .Where(x => !x.ShortAllocations.Any())
                    .OrderBy(x => x.EntryDataDetailsId)
                    .Join(itemList.Select(z => z.InventoryItemId).ToList(), a => a.InventoryItemId, i => i, (a,i) => a)
                    .AsEnumerable()
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();
                return lst;
            }
        }
    }
}