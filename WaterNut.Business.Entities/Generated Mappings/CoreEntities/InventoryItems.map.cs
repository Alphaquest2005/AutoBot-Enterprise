namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemsMap : EntityTypeConfiguration<InventoryItems>
    {
        public InventoryItemsMap()
        {                        
              this.HasKey(t => t.ItemNumber);        
              this.ToTable("InventoryItems");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
              this.Property(t => t.Category).HasColumnName("Category").HasMaxLength(60);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(50);
              this.Property(t => t.EntryTimeStamp).HasColumnName("EntryTimeStamp");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<InventoryItems>) t.InventoryItems).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.InventoryItemAlias).WithRequired(t => (InventoryItems)t.InventoryItems);
              this.HasMany(t => t.AsycudaDocumentItem).WithOptional(t => t.InventoryItems).HasForeignKey(d => d.ItemNumber);
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
