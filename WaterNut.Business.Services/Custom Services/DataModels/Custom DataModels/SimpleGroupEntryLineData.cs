using System;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.Interfaces;
using EntryData = EntryDataDS.Business.Entities.EntryData;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;

namespace WaterNut.DataSpace;

public class SimpleGroupEntryLineData
{
    public static IEnumerable<BaseDataModel.EntryLineData> Execute(IEnumerable<EntryDataDetails> slstSource)
    {
        var slst = from s in slstSource.AsEnumerable()
            group s by ( s.ItemNumber, s.ItemDescription, s.TariffCode, s.Cost, s.EntryData, s.InventoryItemEx)
            into g
            select CreateEntryLineData(g);
        return slst;
    }

    private static BaseDataModel.EntryLineData CreateEntryLineData(IGrouping<(string ItemNumber, string ItemDescription, string TariffCode, double Cost, EntryData EntryData, InventoryItemsEx InventoryItemEx), EntryDataDetails> g)
    {
        return new BaseDataModel.EntryLineData
        {
            ItemNumber = g.Key.ItemNumber.Trim(),
            ItemDescription = g.Key.ItemDescription.Trim(),
            TariffCode = g.Key.TariffCode,
            Cost = g.Key.Cost,
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
            EntryData = g.Key.EntryData,

            Freight = Convert.ToDouble(g.Sum(x => x.Freight)),
            Weight = Convert.ToDouble(g.Sum(x => x.Weight)),
            InternalFreight = Convert.ToDouble(g.Sum(x => x.InternalFreight)),
            TariffSupUnitLkps = g.Key.InventoryItemEx.SuppUnitCode2 != null
                ? new List<ITariffSupUnitLkp>
                {
                    new TariffSupUnitLkps
                    {
                        SuppUnitCode2 = g.Key.InventoryItemEx.SuppUnitCode2,
                        SuppQty = g.Key.InventoryItemEx.SuppQty.GetValueOrDefault()
                    }
                }
                : null,
            InventoryItemEx = g.Key.InventoryItemEx
        };
    }
}