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
    
    internal partial class xcuda_Gs_Invoice_Mapping : EntityTypeConfiguration<xcuda_Gs_Invoice>
    {
        public xcuda_Gs_Invoice_Mapping()
        {                        
              this.HasKey(t => t.Valuation_Id);        
              this.ToTable("xcuda_Gs_Invoice");
              this.Property(t => t.Amount_national_currency).HasColumnName("Amount_national_currency");
              this.Property(t => t.Amount_foreign_currency).HasColumnName("Amount_foreign_currency");
              this.Property(t => t.Currency_code).HasColumnName("Currency_code").IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.Currency_rate).HasColumnName("Currency_rate");
              this.Property(t => t.Valuation_Id).HasColumnName("Valuation_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Currency_name).HasColumnName("Currency_name").IsUnicode(false).HasMaxLength(50);
              this.HasRequired(t => t.xcuda_Valuation).WithOptional(t => t.xcuda_Gs_Invoice);
         }
    }
}
