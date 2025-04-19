namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemAliasEx_NoReverseMappingsMap : EntityTypeConfiguration<InventoryItemAliasEx_NoReverseMappings>
    {
        public InventoryItemAliasEx_NoReverseMappingsMap()
        {                        
              this.HasKey(t => new {t.AliasId, t.AliasName});        
              this.ToTable("InventoryItemAliasEx-NoReverseMappings");
              this.Property(t => t.AliasId).HasColumnName("AliasId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId");
              this.Property(t => t.ItemNumber).HasColumnName("ItemNumber").IsRequired().HasMaxLength(20);
              this.Property(t => t.AliasName).HasColumnName("AliasName").IsRequired().HasMaxLength(20);
              this.Property(t => t.AliasItemId).HasColumnName("AliasItemId");
              this.Property(t => t.ep).HasColumnName("ep");
              this.HasRequired(t => t.InventoryItem).WithMany(t =>(ICollection<InventoryItemAliasEx_NoReverseMappings>) t.InventoryItemAliasEx_NoReverseMappings).HasForeignKey(d => d.InventoryItemId);
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
