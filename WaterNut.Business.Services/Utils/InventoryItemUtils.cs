using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Data;
using CoreEntities.Business.Entities;
using InventoryDS.Business.Entities;
using WaterNut.DataSpace;
using WaterNut.Interfaces;

namespace WaterNut.Business.Services.Utils
{
    public static class InventoryItemUtils
    {
        private static List<InventoryItem> _inventoryCache;

        public static IInventoryItem GetInventoryItem(Func<IInventoryItem, bool> p)
        {
            
                return GetInventoryItems().FirstOrDefault(p);
            
        }

         private static IQueryable<InventoryItem> GetInventoryItems()
         {
             return new InventoryDSContext().InventoryItems
                 .Include("InventoryItemSources.InventorySource")
                 .Include(x => x.InventoryItemAlias)
                 ;
         }

         private static List<InventoryItem> GetInventoryItemsCache(int applicationSettingsId, bool fresh)
         {
             if(_inventoryCache == null || fresh)
                 _inventoryCache = 
              new InventoryDSContext().InventoryItems
                     .Include("InventoryItemSources.InventorySource")
                     .Include(x => x.InventoryItemAlias)
                     .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                     .ToList() ;

             return _inventoryCache;
         }

        public static List<InventoryItem> GetInventoryItems(int applicationSettingsId, bool fresh)
        {

            return GetInventoryItemsCache(applicationSettingsId, fresh);

        }

        public static List<InventoryItem> GetInventoryItems(List<string> itemNumbers, int applicationSettingsId) =>
            itemNumbers.Select(itemNumber => GetInventoryItemsCache(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false).FirstOrDefault(x => x.ItemNumber == itemNumber && x.ApplicationSettingsId == applicationSettingsId))
                .Where(x => x != null)
                .ToList();

        public static InventorySource GetInventorySource(FileTypes fileType)
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
                        throw new ApplicationException("Unknown FileType");
                }
            }

            if (inventorySource == null)
                throw new ApplicationException($"No Inventory source setup for FileType:{fileType.FileImporterInfos.EntryType}");
            return inventorySource;
        }

    }
}
