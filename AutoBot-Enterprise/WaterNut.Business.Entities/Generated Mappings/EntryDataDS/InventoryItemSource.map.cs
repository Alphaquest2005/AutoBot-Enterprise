namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItemSourceMap : EntityTypeConfiguration<InventoryItemSource>
    {
        public InventoryItemSourceMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("InventoryItemSource");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InventorySourceId).HasColumnName("InventorySourceId");
              this.Property(t => t.InventoryId).HasColumnName("InventoryId");
              this.HasRequired(t => t.InventorySources).WithMany(t =>(ICollection<InventoryItemSource>) t.InventoryItemSource).HasForeignKey(d => d.InventorySourceId);
              this.HasRequired(t => t.InventoryItems).WithMany(t =>(ICollection<InventoryItemSource>) t.InventoryItemSource).HasForeignKey(d => d.InventoryId);
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
