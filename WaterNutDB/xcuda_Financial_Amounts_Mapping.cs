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
    
    internal partial class xcuda_Financial_Amounts_Mapping : EntityTypeConfiguration<xcuda_Financial_Amounts>
    {
        public xcuda_Financial_Amounts_Mapping()
        {                        
              this.HasKey(t => t.Amounts_Id);        
              this.ToTable("xcuda_Financial_Amounts");
              this.Property(t => t.Amounts_Id).HasColumnName("Amounts_Id");
              this.Property(t => t.Financial_Id).HasColumnName("Financial_Id");
              this.Property(t => t.Total_manual_taxes).HasColumnName("Total_manual_taxes");
              this.Property(t => t.Global_taxes).HasColumnName("Global_taxes");
              this.Property(t => t.Totals_taxes).HasColumnName("Totals_taxes");
              this.HasRequired(t => t.xcuda_Financial).WithMany(t => t.xcuda_Financial_Amounts).HasForeignKey(d => d.Financial_Id);
         }
    }
}
