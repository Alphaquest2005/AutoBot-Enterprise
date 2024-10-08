﻿namespace PreviousDocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PreviousEntryMap : EntityTypeConfiguration<PreviousEntry>
    {
        public PreviousEntryMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Item");
              this.Property(t => t.Amount_deducted_from_licence).HasColumnName("Amount_deducted_from_licence").HasMaxLength(10);
              this.Property(t => t.Quantity_deducted_from_licence).HasColumnName("Quantity_deducted_from_licence").HasMaxLength(4);
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ASYCUDA_Id).HasColumnName("ASYCUDA_Id");
              this.Property(t => t.Licence_number).HasColumnName("Licence_number").HasMaxLength(50);
              this.Property(t => t.Free_text_1).HasColumnName("Free_text_1").HasMaxLength(35);
              this.Property(t => t.Free_text_2).HasColumnName("Free_text_2").HasMaxLength(30);
              this.Property(t => t.EntryDataDetailsId).HasColumnName("EntryDataDetailsId");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.IsAssessed).HasColumnName("IsAssessed");
              this.Property(t => t.DPQtyAllocated).HasColumnName("DPQtyAllocated");
              this.Property(t => t.DFQtyAllocated).HasColumnName("DFQtyAllocated");
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.AttributeOnlyAllocation).HasColumnName("AttributeOnlyAllocation");
              this.Property(t => t.DoNotAllocate).HasColumnName("DoNotAllocate");
              this.Property(t => t.DoNotEX).HasColumnName("DoNotEX");
              this.Property(t => t.ImportComplete).HasColumnName("ImportComplete");
              this.Property(t => t.WarehouseError).HasColumnName("WarehouseError").HasMaxLength(50);
              this.Property(t => t.SalesFactor).HasColumnName("SalesFactor");
              this.Property(t => t.PreviousInvoiceNumber).HasColumnName("PreviousInvoiceNumber").HasMaxLength(50);
              this.Property(t => t.PreviousInvoiceLineNumber).HasColumnName("PreviousInvoiceLineNumber").HasMaxLength(50);
              this.Property(t => t.PreviousInvoiceItemNumber).HasColumnName("PreviousInvoiceItemNumber").HasMaxLength(50);
              this.Property(t => t.EntryDataType).HasColumnName("EntryDataType").HasMaxLength(50);
              this.Property(t => t.UpgradeKey).HasColumnName("UpgradeKey");
              this.Property(t => t.PreviousInvoiceKey).HasColumnName("PreviousInvoiceKey").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Computed)).HasMaxLength(4000);
              this.Property(t => t.xWarehouseError).HasColumnName("xWarehouseError").HasMaxLength(255);
              this.HasOptional(t => t.xcuda_Tarification).WithRequired(t => (PreviousEntry)t.xcuda_Item);
              this.HasOptional(t => t.xcuda_Valuation_item).WithRequired(t => (PreviousEntry)t.xcuda_Item);
              this.HasOptional(t => t.xcuda_PreviousItem).WithRequired(t => (PreviousEntry)t.PreviousEntry);
              this.HasMany(t => t.EntryPreviousItems).WithRequired(t => (PreviousEntry)t.xcuda_Item);
             // Tracking Properties
    			this.Ignore(t => t.TrackingState);
    			this.Ignore(t => t.ModifiedProperties);
    
    
             // IIdentifibleEntity
                this.Ignore(t => t.EntityId);
                this.Ignore(t => t.EntityName); 
    
                this.Ignore(t => t.EntityKey);
             // Nav Property Names
                  
    
    
              
    
         }
    }
}
