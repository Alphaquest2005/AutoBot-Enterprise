using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Data;
using CoreEntities.Business.Entities;
using InventoryDS.Business.Entities;
using WaterNut.Interfaces;

namespace WaterNut.Business.Services.Utils
{
    public static class InventoryItemUtils
    {

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


         public static List<InventoryItem> GetInventoryItems(int applicationSettingsId)
         {
             
                 return GetInventoryItems()
                     .Where(x => x.ApplicationSettingsId == applicationSettingsId).ToList();
             
         }

        public static List<InventoryItem> GetInventoryItems(List<string> itemNumbers, int applicationSettingsId) =>
            itemNumbers.Select(itemNumber => GetInventoryItems().FirstOrDefault(x => x.ItemNumber == itemNumber && x.ApplicationSettingsId == applicationSettingsId))
                .Where(x => x != null)
                .ToList();

        public static InventorySource GetInventorySource(FileTypes fileType)
        {
            InventorySource inventorySource;
            using (var dctx = new InventoryDSContext())
            {
                switch (fileType.Type)
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
                throw new ApplicationException($"No Inventory source setup for FileType:{fileType.Type}");
            return inventorySource;
        }

    }
}
