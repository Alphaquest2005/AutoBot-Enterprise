namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemAliasMap : EntityTypeConfiguration<InventoryItemAlias>
    {
        public InventoryItemAliasMap()
        {                        
              this.HasKey(t => t.AliasId);        
              this.ToTable("InventoryItemAliasEx");
              this.Property(t => t.AliasId).HasColumnName("AliasId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.AliasName).HasColumnName("AliasName").IsRequired().HasMaxLength(20);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.AliasItemId).HasColumnName("AliasItemId");
              this.HasRequired(t => t.InventoryItem).WithMany(t =>(ICollection<InventoryItemAlias>) t.InventoryItemAliasEx).HasForeignKey(d => d.InventoryItemId);
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
