﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.DataLayer
{
   public partial class xcuda_Weight_itm
    {
       public xcuda_Weight_itm()
       {
           PropertyChanged += xcuda_Weight_itm_PropertyChanged;
       }

       void xcuda_Weight_itm_PropertyChanged(object sender, PropertyChangedEventArgs e)
       {
           if (e.PropertyName == "Gross_weight_itm" && Gross_weight_itm != Net_weight_itm)
           {
               Net_weight_itm = Gross_weight_itm;
               if (xcuda_Valuation_item != null && xcuda_Valuation_item.xcuda_Item.xcuda_PreviousItem != null)
               {
                   xcuda_Valuation_item.xcuda_Item.xcuda_PreviousItem.Net_weight = (decimal) Gross_weight_itm;
               }

           }
       }
    }
}
