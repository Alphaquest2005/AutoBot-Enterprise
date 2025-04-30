using System;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.Interfaces;
using EntryData = EntryDataDS.Business.Entities.EntryData;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;

namespace WaterNut.DataSpace;

public class CategoryGroupEntryLineData
{
    public static IEnumerable<BaseDataModel.EntryLineData> Execute(IEnumerable<EntryDataDetails> slstSource)
    {
        var slst = from s in slstSource.AsEnumerable()
            group s by ( s.Category, s.CategoryTariffCode)
            into g
            select CreateEntryLineData(g);
        return slst;
    }

    private static BaseDataModel.EntryLineData CreateEntryLineData(IGrouping<(string Category, string CategoryTariffCode), EntryDataDetails> g)
    {
        return new BaseDataModel.EntryLineData
        {
            ItemNumber = g.Key.Category.Trim(),
            ItemDescription = g.Key.Category.Trim(),
            TariffCode = g.Key.CategoryTariffCode,
            Cost = g.Sum(x => x.Quantity * x.Cost)/ g.Sum(x => x.Quantity),
            Quantity = g.Sum(x => x.Quantity),
            EntryDataDetails = g.Select(x => new EntryDataDetailSummary
            {
                EntryDataDetailsId = x.EntryDataDetailsId,
                EntryData_Id = x.EntryData_Id,
                EntryDataId = x.EntryDataId,
                EffectiveDate = x.EffectiveDate.GetValueOrDefault(),
                EntryDataDate = x.EntryData.EntryDataDate,
                QtyAllocated = x.QtyAllocated,
                Currency = x.EntryData.Currency,
                LineNumber = x.LineNumber
            }).ToList(),
            EntryData = g.First().EntryData,

            Freight = Convert.ToDouble(g.Sum(x => x.Freight)),
            Weight = Convert.ToDouble(g.Sum(x => x.Weight)),
            InternalFreight = Convert.ToDouble(g.Sum(x => x.InternalFreight)),
            TariffSupUnitLkps = g.First().InventoryItemEx.SuppUnitCode2 != null
                ? new List<ITariffSupUnitLkp>
                {
                    new TariffSupUnitLkps
                    {
                        SuppUnitCode2 = g.First().InventoryItemEx.SuppUnitCode2,
                        SuppQty = g.First().InventoryItemEx.SuppQty.GetValueOrDefault()
                    }
                }
                : null,
            InventoryItemEx = g.First().InventoryItemEx
        };
    }
}