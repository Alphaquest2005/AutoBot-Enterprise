
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using WaterNut.Interfaces;

namespace AllocationDS.Business.Entities
{
    public partial class xcuda_Item 
    {
         
        [NotMapped]
        public String ItemDescription
        {
            get
            {
                try
                {
                    return this.xcuda_Goods_description.Commercial_Description;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
            set { this.xcuda_Goods_description.Commercial_Description = value; }
        }

        [IgnoreDataMember]
        [NotMapped]
        public double QtyAllocated
        {
            get
            {
                return Convert.ToSingle(DFQtyAllocated) + Convert.ToSingle(DPQtyAllocated);
            }
        }

        //public string ItemNumber
        //{
        //    get { return this._xcuda_Tarification.xcuda_HScode.Precision_4; }

        //}
        [IgnoreDataMember]
        [NotMapped]
        public double ItemQuantity
        {
            get
            {
                try
                {
                    if (xcuda_Tarification != null && xcuda_Tarification.xcuda_Supplementary_unit != null)
                    {
                        var res =
                            xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(x => x.IsFirstRow == true);
                            //(x => x.Suppplementary_unit_code == "NMB");
                        return
                            Convert.ToDouble(
                                res == null ? 0 : res.Suppplementary_unit_quantity);
                    }
                    return 0;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public double ItemCost
        {
            get
            {
                if (xcuda_Tarification != null) return xcuda_Tarification.Item_price / ItemQuantity;
                return 0;
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public string TariffCode
        {
            get
            {
                var xcudaTarification = this.xcuda_Tarification;
                if (xcudaTarification != null && xcudaTarification.xcuda_HScode != null) return xcudaTarification.xcuda_HScode.Commodity_code;
                return "NULL";
            }
        }
        [IgnoreDataMember]
        [NotMapped]
        public double DutyLiability
        {
            get
            {
                if (xcuda_Taxation != null)
                    return
                        xcuda_Taxation.SelectMany(x => x.xcuda_Taxation_line)
                            .Where(x => x.Duty_tax_code != "CSC")
                            .Sum(x => x.Duty_tax_amount);
                return 0;
            }
        }

    }
}


