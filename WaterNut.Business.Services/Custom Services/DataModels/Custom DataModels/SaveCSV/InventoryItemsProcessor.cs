using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class InventoryItemsProcessor
    {
        public static void UpdateInventoryItems(
            RawEntryData item)
        {
            int applicationSettingsId = item.Item.EntryData.ApplicationSettingsId;
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var e in item.Item.InventoryItems
                             .Where(x => !string.IsNullOrEmpty(x.ItemAlias) && x.ItemAlias != x.ItemNumber &&
                                         x.ItemAlias != null).ToList())
                {
                    string itemNumber = e.ItemNumber;
                    var inventoryItem = ctx.InventoryItems
                        .Include("InventoryItemAlias")
                        .First(x => x.ApplicationSettingsId == applicationSettingsId &&
                                    x.ItemNumber == itemNumber);
                    if (inventoryItem == null) continue;
                    {
                        if (inventoryItem.InventoryItemAlias.FirstOrDefault(x =>
                                x.AliasName == e.ItemAlias) ==
                            null)
                        {
                            string aliasName = ((string)e.ItemAlias).Truncate(20);
                            var aliasItm = ctx.InventoryItems
                                .FirstOrDefault(x => x.ApplicationSettingsId == applicationSettingsId &&
                                                     x.ItemNumber == aliasName);
                            if (aliasItm == null)
                                throw new ApplicationException(
                                    $"No Alias Inventory Item Found... need to add it before creating Alias {aliasName} for InventoryItem {inventoryItem.ItemNumber}");

                            inventoryItem.InventoryItemAlias.Add(new InventoryItemAlia(true)
                            {
                                InventoryItemId = inventoryItem.Id,
                                AliasName = aliasName,
                                AliasItemId = aliasItm.Id,
                            });
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }
    }
}