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
    
    internal partial class xcuda_Total_Mapping : EntityTypeConfiguration<xcuda_Total>
    {
        public xcuda_Total_Mapping()
        {                        
              this.HasKey(t => t.Valuation_Id);        
              this.ToTable("xcuda_Total");
              this.Property(t => t.Total_invoice).HasColumnName("Total_invoice");
              this.Property(t => t.Total_weight).HasColumnName("Total_weight");
              this.Property(t => t.Valuation_Id).HasColumnName("Valuation_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xcuda_Valuation).WithOptional(t => t.xcuda_Total);
         }
    }
}
