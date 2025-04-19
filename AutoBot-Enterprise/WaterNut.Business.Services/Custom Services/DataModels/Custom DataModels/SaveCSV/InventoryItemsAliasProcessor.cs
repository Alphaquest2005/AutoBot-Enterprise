using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.GettingInventoryItem;

namespace WaterNut.DataSpace
{
    public class InventoryItemsAliasProcessor
    {

        public void UpdateInventoryItems(List<RawEntryData> items)
        {
            try
            {
                var inventoryItms = items.SelectMany(x => x.Item.InventoryItems).DistinctBy(x => new {x.ItemAlias, x.ItemNumber})
                   .Select(GetInventoryItem).DistinctBy(x => x.ItemNumber).ToList();
                var aliases = inventoryItms.SelectMany(x => x.InventoryItemAlias)
                    .DistinctBy(x => new { x.InventoryItemId, x.AliasItemId }).ToList();
                using (var ctx = new InventoryDSContext() { StartTracking = true })
                {
                    //foreach (var itm in inventoryItms)
                    //{
                    //    try
                    //    {
                    //        ctx.BulkMerge(new List<InventoryItem>(){itm});
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e);

                    //    }
                    //}

                    ctx.BulkUpdate(inventoryItms.Where(x => x.Id != 0).ToList());
                    ctx.BulkInsert(inventoryItms.Where(x => x.Id == 0).ToList());

                    ctx.BulkUpdate(aliases.Where(x => x.AliasId != 0).ToList());
                    ctx.BulkInsert(aliases.Where(x => x.AliasId == 0).ToList());

                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private InventoryItem GetInventoryItemWithAlias(RawEntryDataValue.InventoryItemsValue item)
        {
            return GetInventoryItem(item);
        }

        public void UpdateInventoryItems(
            RawEntryData item)
        {
            try
            {
                var inventoryItms = GetInventoryItemWithAlias(item);

                using (var ctx = new InventoryDSContext() { StartTracking = true })
                {
                    ctx.BulkMerge(inventoryItms, operation => operation.IncludeGraph = true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<InventoryItem> GetInventoryItemWithAlias(RawEntryData item)
        {
            var itms = item.Item.InventoryItems
                .Where(x => !string.IsNullOrEmpty(x.ItemAlias) && x.ItemAlias != x.ItemNumber).ToList();
            return GetInventoryItemWithAlias(itms);
        }

        private List<InventoryItem> GetInventoryItemWithAlias(List<RawEntryDataValue.InventoryItemsValue> itms)
        {
            return itms.Select(GetInventoryItem).ToList();
        }

        private InventoryItem GetInventoryItem(RawEntryDataValue.InventoryItemsValue e)
        {
            var inventoryItem = new GetInventoryItemSelector().Execute((string)e.ItemNumber, (string) e.Description);
            var aliasItems = e.ItemAlias == null ? null : new GetInventoryItemSelector().Execute((string)e.ItemAlias, (string)e.Description);
            if (aliasItems != null)
            {
                if (aliasItems.InventoryItems_DoNotMap != null) return inventoryItem;
                inventoryItem.InventoryItemAlias.Add(new InventoryItemAlia(true)
                {
                    InventoryItemId = inventoryItem.Id,
                    AliasItemId = aliasItems.Id,
                });
                return inventoryItem;
            }
            if (inventoryItem.InventoryItemAlias.FirstOrDefault(x =>
                    x.AliasItem?.ItemNumber == e.ItemAlias) ==
                null)
            {
                CreateItemAlias(e, inventoryItem);
            }

            return inventoryItem;
        }

        private  void CreateItemAlias(RawEntryDataValue.InventoryItemsValue e,
            InventoryItem inventoryItem)
        {
            
            string aliasName = ((string)e.ItemAlias).Truncate(20);
            if(string.IsNullOrEmpty(aliasName)) return;
            var aliasItm = new GetInventoryItemSelector().Execute(aliasName, e.Description);
            if (aliasItm == null)
                throw new ApplicationException(
                    $"No Alias Inventory Item Found... need to add it before creating Alias {aliasName} for InventoryItem {inventoryItem.ItemNumber}");

            inventoryItem.InventoryItemAlias.Add(new InventoryItemAlia(true)
            {
                InventoryItemId = inventoryItem.Id,
                AliasItemId = aliasItm.Id,
            });
        }
    }
}