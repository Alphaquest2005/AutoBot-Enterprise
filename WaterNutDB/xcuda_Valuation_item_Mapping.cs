//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WaterNutDB
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Infrastructure;
    
    internal partial class xcuda_Valuation_item_Mapping : EntityTypeConfiguration<xcuda_Valuation_item>
    {
        public xcuda_Valuation_item_Mapping()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Valuation_item");
              this.Property(t => t.Total_cost_itm).HasColumnName("Total_cost_itm");
              this.Property(t => t.Total_CIF_itm).HasColumnName("Total_CIF_itm");
              this.Property(t => t.Rate_of_adjustement).HasColumnName("Rate_of_adjustement").IsUnicode(false);
              this.Property(t => t.Statistical_value).HasColumnName("Statistical_value");
              this.Property(t => t.Alpha_coeficient_of_apportionment).HasColumnName("Alpha_coeficient_of_apportionment").IsUnicode(false);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Item).WithOptional(t => t.xcuda_Valuation_item);
         }
    }
}
