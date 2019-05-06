namespace AdjustmentQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemsExMap : EntityTypeConfiguration<InventoryItemsEx>
    {
        public InventoryItemsExMap()
        {                        
              this.HasKey(t => new {t.ItemNumber, t.ApplicationSettingsId});        
              this.ToTable("InventoryItemsEx");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
              this.Property(t => t.Category).HasColumnName("Category").HasMaxLength(60);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.SuppUnitCode2).HasColumnName("SuppUnitCode2").HasMaxLength(50);
              this.Property(t => t.SuppQty).HasColumnName("SuppQty");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasMany(t => t.InventoryItemAliasExes).WithRequired(t => (InventoryItemsEx)t.InventoryItemsEx);
              this.HasMany(t => t.EntryDataDetails).WithRequired(t => (InventoryItemsEx)t.InventoryItemsEx);
              this.HasMany(t => t.AdjustmentOvers).WithRequired(t => (InventoryItemsEx)t.InventoryItemsEx);
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
