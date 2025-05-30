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
    
    internal partial class AsycudaEntries_Mapping : EntityTypeConfiguration<AsycudaEntries>
    {
        public AsycudaEntries_Mapping()
        {                        
              this.HasKey(t => new {t.Item_Id, t.ASYCUDA_Id, t.LineNumber, t.ItemNumber, t.ItemPrice, t.Expr1, t.Precision_4});        
              this.ToTable("AsycudaEntries", "WaterNutDBDataLayerStoreContainer");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.LineNumber).HasColumnName("LineNumber").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.QtyAllocated).HasColumnName("QtyAllocated");
              this.Property(t => t.ItemQuantity).HasColumnName("ItemQuantity").IsUnicode(false);
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().IsUnicode(false).HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").IsUnicode(false).HasMaxLength(8);
              this.Property(t => t.DeclarantReferenceNumber).HasColumnName("DeclarantReferenceNumber").HasMaxLength(16);
              this.Property(t => t.ItemPrice).HasColumnName("ItemPrice");
              this.Property(t => t.CNumber).HasColumnName("CNumber").IsUnicode(false);
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.EffectiveRegistrationDate).HasColumnName("EffectiveRegistrationDate");
              this.Property(t => t.Expr1).HasColumnName("Expr1").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Precision_4).HasColumnName("Precision_4").IsRequired().IsUnicode(false).HasMaxLength(50);
         }
    }
}
