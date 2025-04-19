using System.Collections.Generic;
using System.Linq;
using InventoryDS.Business.Entities;
using MoreLinq;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class InventoryAliasCodesProcessor
    {
        public static List<(string SupplierItemNumber, string SupplierItemDescription)> GetInventoryAliasCodes(InventoryData item, InventoryItem i)
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
    }
}