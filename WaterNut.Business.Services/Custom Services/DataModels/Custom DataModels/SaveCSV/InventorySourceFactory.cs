using System;
using System.Linq;
using CoreEntities.Business.Entities;
using InventoryDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class InventorySourceFactory
    {
        public static InventorySource GetInventorySource(FileTypes fileType)
        
        
        {
            InventorySource inventorySource;
            using (var dctx = new InventoryDSContext())
            {
                switch (fileType.FileImporterInfos.EntryType)
                {
                    case "Shipment Template":
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
    }
}