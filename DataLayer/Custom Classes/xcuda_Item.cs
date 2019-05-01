using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace WaterNut.DataLayer
{
    public partial class xcuda_Item : IHasEntryTimeStamp
    {
        public xcuda_Item()
        {
            xcuda_ASYCUDAReference.AssociationChanged += xcuda_ASYCUDAReference_AssociationChanged;
            
        }

        void xcuda_ASYCUDAReference_AssociationChanged(object sender, CollectionChangeEventArgs e)
        {
                   if (xcuda_ASYCUDA != null)
            {
                if (xcuda_ASYCUDA.EntityState == EntityState.Added && LineNumber == 0)
                {
                    LineNumber = xcuda_ASYCUDA.xcuda_Item.Count;
                }
            }
        }

        partial void OnLineNumberChanged()
        {
            if(xcuda_PreviousItem != null)
            xcuda_PreviousItem.Current_item_number = LineNumber.ToString();
        }

        public void SetupProperties()
        {
            if (xcuda_Tarification == null) xcuda_Tarification = new xcuda_Tarification();
            if (xcuda_Tarification.xcuda_HScode == null) xcuda_Tarification.xcuda_HScode = new xcuda_HScode();
            if (xcuda_Valuation_item == null) xcuda_Valuation_item = new xcuda_Valuation_item();
            if (xcuda_Valuation_item.xcuda_Weight_itm == null) xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm();

            if (xcuda_Goods_description == null) xcuda_Goods_description = new xcuda_Goods_description();
            if (xcuda_Previous_doc == null) xcuda_Previous_doc = new xcuda_Previous_doc();

            

            
           
        }

        public string TariffCode
        {
            get
            {
                if ((IsAssessed == null || IsAssessed == false) && xcuda_Tarification.xcuda_HScode.InventoryItems.Count > 0 
                            && xcuda_Tarification.xcuda_HScode.InventoryItems.FirstOrDefault().TariffCode != null)
                {
                    return xcuda_Tarification.xcuda_HScode.InventoryItems.FirstOrDefault().TariffCode.ToString();
                }
                else
                {
                    if (xcuda_Tarification == null) return "";
                    if (xcuda_Tarification.xcuda_HScode.Commodity_code != null)
                    {
                        return xcuda_Tarification.xcuda_HScode.Commodity_code.ToString();
                    }
                    else
                    {
                        return "";
                    }

                }

            }
        }
        public string ItemNumber
        {
            get
            {
                if(xcuda_Tarification != null)
                return xcuda_Tarification.xcuda_HScode.Precision_4;

                return null;
            }
        }
  

        public double ItemCost
        {
            get
            {
                //if ((this.IsAssessed == null || this.IsAssessed == false) && this.EntryDataDetails.FirstOrDefault() != null)
                //{
                //    return Convert.ToDouble(this.EntryDataDetails.Cost);
                //}
                //else
                //{
                if (xcuda_Tarification == null || ItemQuantity == 0) return 0;
                    return xcuda_Tarification.Item_price / ItemQuantity;
                //}
            }
        }

        public double QtyAllocated
        {
            get
            {
                return Convert.ToDouble(DFQtyAllocated) + Convert.ToDouble(DPQtyAllocated);
            }
        }

        public double ItemQuantity
        {
            get
            {
                if (xcuda_Tarification != null)
                {
                    var xcudaSupplementaryUnit = xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault();
                    if (xcudaSupplementaryUnit != null)
                        return Convert.ToDouble(xcudaSupplementaryUnit.Suppplementary_unit_quantity);
                }
                return 0;
            }
            set
            {
                if (xcuda_Tarification != null)
                {
                    var xcudaSupplementaryUnit = xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault();
                    if (xcudaSupplementaryUnit != null)
                    {
                        xcudaSupplementaryUnit.Suppplementary_unit_quantity =
                            xcudaSupplementaryUnit.Suppplementary_unit_quantity + value;
                    }
                }
            }
        }

        //public double PiQuantity
        //{
        //    get
        //    {
        //        return xcuda_PreviousItems.Any() == true?xcuda_PreviousItems.Sum(x => x.Suplementary_Quantity):0;
        //    }
        //}
        public double Statistical_value
        {
            get
            {
                return xcuda_Valuation_item.Statistical_value;
            }
            
        }

        public double DutyLiability
        {
            get
            {
                return
                    xcuda_Taxation.SelectMany(x => x.xcuda_Taxation_line)
                        .Where(x => x.Duty_tax_code != "CSC")
                        .Sum(y => Convert.ToDouble(y.Duty_tax_amount));
            }
        }

        public double CIF
        {
            get { return xcuda_Valuation_item.Total_CIF_itm; }
        }

        public bool MaxedOut { get; set; }
    }
}
