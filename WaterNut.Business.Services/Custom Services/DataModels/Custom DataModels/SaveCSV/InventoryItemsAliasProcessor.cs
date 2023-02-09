using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV.GettingInventoryItem;

namespace WaterNut.DataSpace
{
    public class InventoryItemsAliasProcessor
    {

        public void UpdateInventoryItems(List<RawEntryData> items)
        {
            try
            {
                var inventoryItms = items.SelectMany(GetInventoryItemWithAlias).ToList();

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
           return itms.Select(e =>
            {
                var inventoryItem = new GetInventoryItemSelector().Execute((string)e.ItemNumber);

                if (inventoryItem.InventoryItemAlias.FirstOrDefault(x =>
                        x.AliasName == e.ItemAlias) ==
                    null)
                {
                    CreateItemAlias(e, inventoryItem);
                }

                return inventoryItem;
            }).ToList();
        }

        private  void CreateItemAlias(RawEntryDataValue.InventoryItemsValue e,
            InventoryItem inventoryItem)
        {
            
            string aliasName = ((string)e.ItemAlias).Truncate(20);
            var aliasItm = new GetInventoryItemSelector().Execute(aliasName);
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