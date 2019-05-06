namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemXMap : EntityTypeConfiguration<InventoryItemX>
    {
        public InventoryItemXMap()
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
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<InventoryItemX>) t.InventoryItemsEx).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.InventoryItemAliasEx).WithRequired(t => (InventoryItemX)t.InventoryItemsEx);
              this.HasMany(t => t.AsycudaDocumentItem).WithOptional(t => t.InventoryItemsEx).HasForeignKey(d => new {d.ItemNumber, d.ApplicationSettingsId});
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
