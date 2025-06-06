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
    
    internal partial class CounterPointPODetails_Mapping : EntityTypeConfiguration<CounterPointPODetails>
    {
        public CounterPointPODetails_Mapping()
        {                        
              this.HasKey(t => new {t.PO_NO, t.SEQ_NO, t.ITEM_NO, t.ORD_QTY, t.ITEM_DESCR, t.ORD_COST});        
              this.ToTable("CounterPointPODetails", "WaterNutDBDataLayerStoreContainer");
              this.Property(t => t.PO_NO).HasColumnName("PO_NO").IsRequired().IsUnicode(false).HasMaxLength(20);
              this.Property(t => t.SEQ_NO).HasColumnName("SEQ_NO").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO").IsRequired().IsUnicode(false).HasMaxLength(20);
              this.Property(t => t.ORD_QTY).HasColumnName("ORD_QTY");
              this.Property(t => t.ORD_UNIT).HasColumnName("ORD_UNIT").IsUnicode(false).HasMaxLength(15);
              this.Property(t => t.ITEM_DESCR).HasColumnName("ITEM_DESCR").IsRequired().IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.ORD_COST).HasColumnName("ORD_COST");
              this.Property(t => t.UNIT_WEIGHT).HasColumnName("UNIT_WEIGHT");
         }
    }
}
