using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private static decimal UpdateWeight(DocumentDSContext ctx, KeyValuePair<int, double> doc, decimal weightRate)
    {
        try
        {
            if (Instance.CurrentApplicationSettings.WeightCalculationMethod.ToLower() !=
                "Value".ToLower()) return 0;
            var val = ctx.xcuda_Valuation.Include(x => x.xcuda_Weight).FirstOrDefault(x => x.ASYCUDA_Id == doc.Key);
            if (val == null) return 0;
            var xcuda_Weight = val.xcuda_Weight;
            if (xcuda_Weight == null)
            {
                xcuda_Weight = new xcuda_Weight(true)
                {
                    Valuation_Id = doc.Key,
                    xcuda_Valuation = val,
                    TrackingState = TrackingState.Added
                };
                val.xcuda_Weight = xcuda_Weight;
            }

            xcuda_Weight.Gross_weight = (double)(weightRate < _minimumPossibleAsycudaWeight
                ? _minimumPossibleAsycudaWeight
                : weightRate);

            decimal weightUsed = 0;
            using (var ictx = new DocumentItemDSContext { StartTracking = true })
            {
                var lst = ictx.xcuda_Weight_itm
                    .Include(x => x.xcuda_Valuation_item)
                    .Where(x => x.xcuda_Valuation_item.xcuda_Item.ASYCUDA_Id == doc.Key).ToList();


                foreach (var itm in lst)
                {
                    var itmQuantity = ictx.xcuda_Tarification.Include(x => x.Unordered_xcuda_Supplementary_unit)
                        .First(z => z.Item_Id == itm.Valuation_item_Id)
                        .Unordered_xcuda_Supplementary_unit.First(x => x.IsFirstRow == true)
                        .Suppplementary_unit_quantity.GetValueOrDefault();

                    var calWgt = weightRate * (decimal)(itmQuantity / doc.Value);
                    var minWgt = ((decimal)itmQuantity) * _minimumPossibleAsycudaWeight;

                    if (calWgt - _runningMiniumWeight < minWgt)
                    {
                        itm.Gross_weight_itm = minWgt;
                        _runningMiniumWeight += minWgt;
                    }
                    else
                    {
                        itm.Gross_weight_itm = calWgt - _runningMiniumWeight;
                        _runningMiniumWeight = 0;
                    }

                    // itm.Gross_weight_itm -= (itm == lst.First() ? _weightAsycudaNormallyOffBy : 0);

                    itm.Net_weight_itm =
                        itm.Gross_weight_itm;
                    weightUsed += itm.Gross_weight_itm;
                }

                ictx.SaveChanges();
            }

            return weightUsed;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}