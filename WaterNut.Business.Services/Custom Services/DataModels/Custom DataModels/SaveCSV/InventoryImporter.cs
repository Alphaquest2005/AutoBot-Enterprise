using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using InventoryDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class InventoryImporter
    {
        static InventoryImporter()
        {
        }

        public InventoryImporter()
        {
        }

        public async Task ImportInventory(DataFile dataFile)
        {
            try
            {



                var itmlst = InventoryItemDataUtils.CreateItemGroupList(dataFile.Data);


                var inventorySource = GetInventorySource(dataFile.FileType);

                ProcessInventoryItemLst(dataFile.DocSet.First().ApplicationSettingsId, itmlst, inventorySource);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static InventorySource GetInventorySource(FileTypes fileType)
        {
            InventorySource inventorySource;
            using (var dctx = new InventoryDSContext())
            {
                switch (fileType.FileImporterInfos.EntryType)
                {
                    case "Shipment Invoice":
                    case "INV":

                        inventorySource = dctx.InventorySources.FirstOrDefault(x => x.Name == "Supplier");
                        break;
                    case "PO":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "OPS":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "ADJ":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "Sales":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    case "DIS":
                        inventorySource = dctx.InventorySources.First(x => x.Name == "POS");
                        break;
                    default:
                        throw new ApplicationException("Unknown CSV FileType");
                }
            }

            if (inventorySource == null)
                throw new ApplicationException($"No Inventory source setup for FileType:{fileType.FileImporterInfos.EntryType}");
            return inventorySource;
        }

        private static void ProcessInventoryItemLst(int applicationSettingsId,
            List<InventoryData> inventoryDataList,
            InventorySource inventorySource)
        {

            var data = InventoryItemDataUtils.GetInventoryItemFromData(applicationSettingsId, inventoryDataList, inventorySource);

            data.existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.TariffCode))
                .ForEach(x => x.Item.TariffCode = x.Data.Key.TariffCode);

            data.existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Data.Data.ForEach(z => z.ItemDescription = x.Item.Description));

            data.existingInventoryItem.Where(x => x.Item.TariffCode != x.Data.Key.TariffCode)
                .Where(x => !string.IsNullOrEmpty(x.Data.Key.ItemDescription))
                .ForEach(x => x.Item.Description = x.Data.Key.ItemDescription);


            data.existingInventoryItem
                .Where(i => i.Item.InventoryItemSources.All(x => x.InventorySourceId != inventorySource.Id))
                .ForEach(x =>
                    x.Item.InventoryItemSources.Add(CreateItemSource(inventorySource, x.Item)));

            InventoryItemDataUtils.SaveInventoryItems(data.existingInventoryItem);
            ///////////////

            data.existingInventoryItem.ForEach(x => x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id));

            data.existingInventoryItem
                .Select<InventoryDataItem, (InventoryDataItem DataItem, List<(string SupplierItemNumber, string SupplierItemDescription)> Code)>(x => (DataItem: x, Code: GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));


            data.existingInventoryItem
                .Select<InventoryDataItem, (InventoryDataItem DataItem, List<(string SupplierItemNumber, string SupplierItemDescription)> Code)>(x => (DataItem: x, Code: GetInventoryAliasCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));


           

          


            data.newInventoryItems.ForEach(x => x.Data.Data.ForEach(z => z.InventoryItemId = x.Item.Id));

            data.newInventoryItems
                .Select<InventoryDataItem, (InventoryDataItem DataItem, List<(string SupplierItemNumber, string SupplierItemDescription)> Code)>(x => (DataItem: x, Code: GetInventoryItemCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));


            data.newInventoryItems
                .Select<InventoryDataItem, (InventoryDataItem DataItem, List<(string SupplierItemNumber, string SupplierItemDescription)> Code)>(x => (DataItem: x, Code: GetInventoryAliasCodes(x.Data, x.Item)))
                .ForEach(x => SaveInventoryCodes(applicationSettingsId, inventorySource, x.Code, x.DataItem.Item));

        }

        private static void SaveInventoryCodes(int applicationSettingsId, InventorySource inventorySource,
            List<(string SupplierItemNumber, string SupplierItemDescription)> itemCodes,
            InventoryItem i)
        {
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var invItemCode in itemCodes)
                {


                    var supplierItemNumber = invItemCode.SupplierItemNumber.ToString();
                    var invItem = ctx.InventoryItems.FirstOrDefault(x =>
                        x.ApplicationSettingsId == applicationSettingsId && x.ItemNumber == supplierItemNumber);
                    if (invItem == null)
                    {
                        invItem = new InventoryItem(true)
                        {
                            ApplicationSettingsId = applicationSettingsId,
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
                        ctx.InventoryItems.Add(invItem);
                        ctx.SaveChanges();
                    }

                    if (i.InventoryItemAlias.FirstOrDefault(x => x.AliasName == supplierItemNumber) == null)
                    {
                        var inventoryItemAlia = new InventoryItemAlia(true)
                        {
                            InventoryItemId = i.Id,
                            AliasName = ((string)supplierItemNumber).Truncate(20),
                            AliasItemId = invItem.Id,
                            AliasId = invItem.Id,
                            TrackingState = TrackingState.Added
                        };
                        ctx.InventoryItemAlias.Add(inventoryItemAlia);
                        ctx.SaveChanges();

                        i.InventoryItemAlias.Add(inventoryItemAlia);
                    }

                }

                
            }
        }

        private static List<(string SupplierItemNumber, string SupplierItemDescription)> GetInventoryAliasCodes(InventoryData item, InventoryItem i)
        {
            var AliasItemCodes =
                item.Data.Where(x => !string.IsNullOrEmpty(x.ItemAlias))
                    .Select(x => (
                        SupplierItemNumber: (string)x.ItemAlias.ToString(),
                        SupplierItemDescription: (string)x.ItemDescription
                    ))
                    .Where(x => !string.IsNullOrEmpty(x.SupplierItemNumber) &&
                                i.ItemNumber != x.SupplierItemNumber)
                    .DistinctBy(x => x.SupplierItemNumber)
                    .ToList();
            return AliasItemCodes;
        }

        private static List<(string SupplierItemNumber, string SupplierItemDescription)> GetInventoryItemCodes(InventoryData item, InventoryItem i)
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

        private static InventoryItemSource CreateItemSource(InventorySource inventorySource, InventoryItem i)
        {
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                var inventoryItemSource = new InventoryItemSource(true)
                {
                    InventorySourceId = inventorySource.Id,
                    TrackingState = TrackingState.Added,
                    InventoryId = i.Id,
                };
                ctx.InventoryItemSources.Add(inventoryItemSource);
                ctx.SaveChanges();
                return inventoryItemSource;
            }
        }
    }
}