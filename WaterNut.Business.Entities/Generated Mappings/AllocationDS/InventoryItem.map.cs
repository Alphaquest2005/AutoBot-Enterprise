﻿namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemMap : EntityTypeConfiguration<InventoryItem>
    {
        public InventoryItemMap()
        {                        
              this.HasKey(t => t.InventoryItemId);        
              this.ToTable("InventoryItemsEx");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
              this.Property(t => t.Category).HasColumnName("Category").HasMaxLength(60);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.SuppUnitCode2).HasColumnName("SuppUnitCode2").HasMaxLength(50);
              this.Property(t => t.SuppQty).HasColumnName("SuppQty");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasOptional(t => t.TariffCodes).WithMany(t =>(ICollection<InventoryItem>) t.InventoryItemsEx).HasForeignKey(d => d.TariffCode);
              this.HasMany(t => t.EX9AsycudaSalesAllocations).WithRequired(t => (InventoryItem)t.InventoryItemsEx);
              this.HasMany(t => t.EntryDataDetailsEx).WithRequired(t => (InventoryItem)t.InventoryItemsEx);
              this.HasMany(t => t.InventoryItemAliasEx).WithRequired(t => (InventoryItem)t.InventoryItem);
              this.HasOptional(t => t.LumpedItem).WithRequired(t => (InventoryItem)t.InventoryItem);
              this.HasMany(t => t.EntryDataDetails).WithRequired(t => (InventoryItem)t.InventoryItem);
              this.HasMany(t => t.InventoryItemAliasEx_NoReverseMappings).WithRequired(t => (InventoryItem)t.InventoryItem);
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
