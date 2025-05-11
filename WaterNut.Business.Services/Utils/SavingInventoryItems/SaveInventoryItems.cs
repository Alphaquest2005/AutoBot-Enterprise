using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Utils.SavingInventoryItems
{
    public class SaveInventoryItems: ISaveInventoryItemsProcessor
    {
        public async Task Execute(List<InventoryDataItem> processedInventoryItems)
        {
            foreach (var processedInventoryItem in processedInventoryItems.Where(x =>
                         x.Item.TrackingState != TrackingState.Unchanged))
            {
 
                 using (var ctx = new InventoryDSContext() { StartTracking = true })
                 {
 
                     var itm = processedInventoryItem.Item.ChangeTracker.GetChanges().FirstOrDefault();
                     ctx.ApplyChanges(itm);
                     await ctx.SaveChangesAsync().ConfigureAwait(false);
                     itm.AcceptChanges();
                     processedInventoryItem.Item.Id = itm.Id;
 
                 }
             }
         }
     }
 }