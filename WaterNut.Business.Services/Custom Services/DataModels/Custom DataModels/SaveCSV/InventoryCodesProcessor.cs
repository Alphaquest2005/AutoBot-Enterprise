using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class InventoryCodesProcessor
    {
        public static void SaveInventoryCodes(InventorySource inventorySource,
            List<(string SupplierItemNumber, string SupplierItemDescription)> itemCodes,
            InventoryItem i)
        {
            try
            {



                using (var ctx = new InventoryDSContext() {StartTracking = true})
                {
                    foreach (var invItemCode in itemCodes)
                    {


                        var supplierItemNumber = invItemCode.SupplierItemNumber.ToString();
                        var invItem = ctx.InventoryItems
                            .FirstOrDefault(x => x.ApplicationSettingsId ==
                                                 BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                                 x.ItemNumber == supplierItemNumber);
                        if (invItem == null)
                        {
                            invItem = CreateInventoryItem(inventorySource, invItemCode);
                            ctx.InventoryItems.Add(invItem);
                           // ctx.SaveChanges();
                        }

                        if (i.InventoryItemAlias.FirstOrDefault(x => x.AliasName == supplierItemNumber) == null &&
                            supplierItemNumber.ToUpper() != i.ItemNumber.ToUpper() && !string.IsNullOrEmpty(i.ItemNumber))
                        {
                            var inventoryItemAlia = CreateInventoryItemAlia(i, supplierItemNumber, invItem);
                            ctx.InventoryItemAlias.Add(inventoryItemAlia);
                           // ctx.SaveChanges();

                            i.InventoryItemAlias.Add(inventoryItemAlia);
                        }
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static InventoryItemAlia CreateInventoryItemAlia(InventoryItem i, string supplierItemNumber,
            InventoryItem invItem)
        {
            var inventoryItemAlia = new InventoryItemAlia(true)
            {
                InventoryItemId = i.Id,
                AliasName = ((string)supplierItemNumber).Truncate(20),
                AliasItemId = invItem.Id,
                AliasId = invItem.Id,
                TrackingState = TrackingState.Added
            };
            return inventoryItemAlia;
        }

        private static InventoryItem CreateInventoryItem(InventorySource inventorySource,
            (string SupplierItemNumber, string SupplierItemDescription) invItemCode)
        {
            InventoryItem invItem;
            invItem = new InventoryItem(true)
            {
                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings
                    .ApplicationSettingsId,
                Description = invItemCode.SupplierItemDescription,
                ItemNumber = ((string)invItemCode.SupplierItemNumber).Truncate(20),
                InventoryItemSources = new List<InventoryItemSource>()
                {
                    new InventoryItemSource(true)
                    {
                        InventorySourceId = inventorySource.Id,
                        TrackingState = TrackingState.Added
                    }
                },
                TrackingState = TrackingState.Added
            };
            return invItem;
        }

        public static List<(string SupplierItemNumber, string SupplierItemDescription)> GetInventoryItemCodes(InventoryData item, InventoryItem i)
        {
            var invItemCodes = item.Data
                .Select(x => (
                    SupplierItemNumber: (string)x.SupplierItemNumber,
                    SupplierItemDescription: (string)x.SupplierItemDescription
                ))
                .Where(x => !string.IsNullOrEmpty(x.SupplierItemNumber) && i.ItemNumber != x.SupplierItemNumber)
                .DistinctBy(x => x.SupplierItemNumber)
                .ToList();
            return invItemCodes;
        }
    }
}