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
            ((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId, dynamic
                CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId, dynamic
                DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation, dynamic
                Vendor,
                dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails> EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    , double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems) item)
        {
            int applicationSettingsId = item.EntryData.ApplicationSettingsId;
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var e in item.InventoryItems
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