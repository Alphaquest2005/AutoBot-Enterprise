using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace WaterNut.DataLayer
{

    public enum DocumentApportionMethod
    {
        ByValue,
        TenPercentItemQuantity
    }

   public partial class xcuda_ASYCUDA_ExtendedProperties
    {
       public xcuda_ASYCUDA_ExtendedProperties()
        {
            
        }

       
  

      decimal totalGrossWeight = 0;
      public decimal TotalGrossWeight
      {
          get
          {
              return Convert.ToDecimal(xcuda_ASYCUDA.xcuda_Item.Where(xe => xe.xcuda_ASYCUDA != null).Sum(x => x.xcuda_Valuation_item == null || x.xcuda_Valuation_item.xcuda_Weight_itm == null ? 0 : x.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm));
          }
          set
          {
              if (xcuda_ASYCUDA.xcuda_Identification.xcuda_Registration.Number == null)
              {
                  if (xcuda_ASYCUDA.xcuda_Valuation == null || xcuda_ASYCUDA.xcuda_Valuation.xcuda_Weight == null || xcuda_ASYCUDA.xcuda_Valuation.xcuda_Weight.Gross_weight == 0)
                  {
                      totalGrossWeight = SetDocumentGrossWeight(value);
                  }
              }
          }
      }

      private decimal SetDocumentGrossWeight(decimal value)
      {
          // Set the Document Weightsis
          

          decimal totweight = 0;
          var xlist = xcuda_ASYCUDA.xcuda_Item;
          // working 

          for (var i = 0; i < xlist.Count(); i++)
          {
              var x = xlist.ElementAt(i);
              Single w = 0;

              

              //switch (WeightApportionMethod)
              //{
              //    case DocumentApportionMethod.TenPercentItemQuantity:
              //        w = TenPercentItemQuantity(x.ItemQuantity);
              //        break;
              //    case DocumentApportionMethod.ByValue:
              //        w = WeightByValue(Convert.ToSingle(x.ItemCost) * Convert.ToSingle(x.ItemQuantity));
              //        break;
              //    default:
              //        break;
              //}

              
              var gw = x.xcuda_Valuation_item.xcuda_Weight_itm;
              if (gw == null)
              {
                gw = new xcuda_Weight_itm();
                x.xcuda_Valuation_item.xcuda_Weight_itm = gw;
              }
              
              gw.Gross_weight_itm = w;
              gw.Net_weight_itm = w;

              totweight += Convert.ToDecimal(w);
          }
          return totweight;
      }



      private float TenPercentItemQuantity(double qty)
      {
          return Convert.ToSingle(qty * 0.1);
      }


    }
}
