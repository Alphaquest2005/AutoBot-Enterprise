using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DocumentDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities;

namespace DocumentItemDS.Business.Entities
{
    public partial class xcuda_Item
    {
       

        //void xcuda_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "xcuda_ASYCUDA")
        //    {
        //        if (xcuda_ASYCUDA != null)
        //        {
        //            if (xcuda_ASYCUDA.TrackingState == TrackingState.Added && LineNumber == 0)
        //            {
        //                LineNumber = xcuda_ASYCUDA.xcuda_Item.Count;
        //            }
        //        }
        //    }

        //    //implement naveprop
        //    if (e.PropertyName == "ASYCUDA_Id")
        //    {
        //        using (var ctx = new xcuda_ASYCUDABusiness())
        //        {

        //            xcuda_ASYCUDA = new xcuda_ASYCUDA(ctx.Getxcuda_ASYCUDAByKey(ASYCUDA_Id.ToString()).Result);
        //        }
        //    }

        //    if (e.PropertyName == "LineNumber")
        //    {
        //        if (xcuda_PreviousItem != null)
        //            xcuda_PreviousItem.Current_item_number = LineNumber.ToString();
        //    }
        //}

        xcuda_ASYCUDA _xcuda_ASYCUDA = null;
        [IgnoreDataMember]
        [NotMapped]
        public xcuda_ASYCUDA xcuda_ASYCUDA
        {
            get
            {
                return _xcuda_ASYCUDA;
            }
            set
            {
                if (value != null)
                {
                    _xcuda_ASYCUDA = value;

                    ASYCUDA_Id = _xcuda_ASYCUDA.ASYCUDA_Id;
                 
                }
            }

        }



        partial void SetupProperties()
        {
            if (xcuda_Tarification == null) xcuda_Tarification = new xcuda_Tarification(true) { TrackingState = TrackingState.Added };
            if (xcuda_Tarification.xcuda_HScode == null) xcuda_Tarification.xcuda_HScode = new xcuda_HScode(true) { TrackingState = TrackingState.Added };
            if (xcuda_Valuation_item == null) xcuda_Valuation_item = new xcuda_Valuation_item(true) { TrackingState = TrackingState.Added };
            if (xcuda_Valuation_item.xcuda_Weight_itm == null) xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true) { TrackingState = TrackingState.Added };

            if (xcuda_Goods_description == null) xcuda_Goods_description = new xcuda_Goods_description(true) { TrackingState = TrackingState.Added };
            if (xcuda_Previous_doc == null) xcuda_Previous_doc = new xcuda_Previous_doc(true) { TrackingState = TrackingState.Added };
            if (xcuda_Packages == null) xcuda_Packages = new List<xcuda_Packages>() { new xcuda_Packages(true) { TrackingState = TrackingState.Added } };


            if (xcuda_Tarification == null) xcuda_Tarification = new xcuda_Tarification(true) { TrackingState = TrackingState.Added };
            if (xcuda_Tarification.xcuda_HScode == null) xcuda_Tarification.xcuda_HScode = new xcuda_HScode(true) { TrackingState = TrackingState.Added };
            if (xcuda_Tarification.Unordered_xcuda_Supplementary_unit == null) xcuda_Tarification.Unordered_xcuda_Supplementary_unit = new List<xcuda_Supplementary_unit>()  {new xcuda_Supplementary_unit(true) {TrackingState = TrackingState.Added}};
            if (xcuda_Valuation_item == null) xcuda_Valuation_item = new xcuda_Valuation_item(true) { TrackingState = TrackingState.Added };

            if (xcuda_Valuation_item.xcuda_Item_Invoice == null) xcuda_Valuation_item.xcuda_Item_Invoice = new xcuda_Item_Invoice(true){TrackingState = TrackingState.Added};
            if (xcuda_Valuation_item.xcuda_Weight_itm == null)
                xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true) { TrackingState = TrackingState.Added };
            if (xcuda_Valuation_item.xcuda_item_internal_freight == null)
                xcuda_Valuation_item.xcuda_item_internal_freight = new xcuda_item_internal_freight(true) { TrackingState = TrackingState.Added };
            if (xcuda_Valuation_item.xcuda_item_external_freight == null)
                xcuda_Valuation_item.xcuda_item_external_freight = new xcuda_item_external_freight(true) { TrackingState = TrackingState.Added };
            
        }

        [IgnoreDataMember]
        [NotMapped]
        public string TariffCode
        {
            get
            {
                //if ((IsAssessed == null || IsAssessed == false) && xcuda_Tarification.xcuda_HScode.xcuda_Inventory_Item.FirstOrDefault() != null//.Count() > 0 
                //            && xcuda_Tarification.xcuda_HScode.xcuda_Inventory_Item.FirstOrDefault()..TariffCode != null)//.FirstOrDefault()
                //{
                //    return xcuda_Tarification.xcuda_HScode.InventoryItems.TariffCode.ToString();//.FirstOrDefault()
                //}
                //else
                //{
                    if (xcuda_Tarification == null) return "";
                    if (xcuda_Tarification.xcuda_HScode.Commodity_code != null)
                    {
                        return xcuda_Tarification.xcuda_HScode.Commodity_code.ToString();
                    }
                    else
                    {
                        return "";
                    }

                //}

            }
        }

        [IgnoreDataMember]
        [NotMapped]
        public double ItemCost
        {
            get
            {

                if (xcuda_Tarification == null || ItemQuantity == 0) return 0;
                return xcuda_Tarification.Item_price / ItemQuantity;

            }
        }


        [IgnoreDataMember]
        [NotMapped]
        public double ItemQuantity
        {
            get
            {
                if (xcuda_Tarification != null)
                {
                    var xcudaSupplementaryUnit = xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(x => x.IsFirstRow == true);
                    
                    if (xcudaSupplementaryUnit != null)
                        return Convert.ToSingle(xcudaSupplementaryUnit.Suppplementary_unit_quantity);
                }
                return 0;
            }
            set
            {
                if (xcuda_Tarification != null)
                {
                    var xcudaSupplementaryUnit = xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault();
                    if (xcudaSupplementaryUnit == null)
                    {

                        xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit(true){ Suppplementary_unit_quantity = value, TrackingState = TrackingState.Added});
                    }
                    if (xcudaSupplementaryUnit != null)
                    {
                        xcudaSupplementaryUnit.Suppplementary_unit_quantity =
                           xcudaSupplementaryUnit.Suppplementary_unit_quantity + value;
                    }
                }
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public double PiQuantity
        {
            get
            {
                return xcuda_PreviousItems.Any() == true ? xcuda_PreviousItems.Select(x => x.xcuda_PreviousItem).Sum(x => x.Suplementary_Quantity) : 0;
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public double Statistical_value
        {
            get
            {
                return xcuda_Valuation_item.Statistical_value;
            }

        }
        [IgnoreDataMember]
        [NotMapped]
        public double DutyLiability
        {
            get
            {
                return
                    xcuda_Taxation.SelectMany(x => x.xcuda_Taxation_line)
                        .Where(x => x.Duty_tax_code != "CSC")
                        .Sum(y => y.Duty_tax_amount);
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public double CIF
        {
            get { return xcuda_Valuation_item.Total_CIF_itm; }
        }

        [IgnoreDataMember]
        [NotMapped]
        public bool MaxedOut { get; set; }
        [IgnoreDataMember]
        [NotMapped]
        public double QtyAllocated
        {
            get
            {
                return Convert.ToDouble(DFQtyAllocated) + Convert.ToDouble(DPQtyAllocated);
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public int? EmailId { get; set; }
    }
}
