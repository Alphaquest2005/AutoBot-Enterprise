
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using WaterNut.Interfaces;

using TrackableEntities.Client;
using Core.Common.Validation;

namespace CoreEntities.Client.Entities
{
       public partial class AsycudaDocumentItem
    {
           public double ItemCost
           {
               get
               {
                   return Convert.ToDouble(Item_price) / Convert.ToDouble(ItemQuantity);
               }
           }

           public double QtyAllocated
           {
               get
               {
                   return Convert.ToDouble(DFQtyAllocated) + Convert.ToDouble(DPQtyAllocated);
               }
           }

           public double Weight
           {
               get { return Gross_weight; }
               set
               {
                   Gross_weight = value;
                   Net_weight = value;
                   NotifyPropertyChanged();//x => x.Weight
               }
           }
    }
}


