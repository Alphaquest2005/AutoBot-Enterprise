namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItems_NonStockMap : EntityTypeConfiguration<InventoryItems_NonStock>
    {
        public InventoryItems_NonStockMap()
        {                        
              this.HasKey(t => t.InventoryItemId);        
              this.ToTable("InventoryItems-NonStock");
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.InventoryItems).WithOptional(t => (InventoryItems_NonStock)t.InventoryItems_NonStock);
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
