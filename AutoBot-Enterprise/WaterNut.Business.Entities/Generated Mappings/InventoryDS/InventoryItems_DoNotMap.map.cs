namespace InventoryDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InventoryItems_DoNotMapMap : EntityTypeConfiguration<InventoryItems_DoNotMap>
    {
        public InventoryItems_DoNotMapMap()
        {                        
              this.HasKey(t => t.InventoryItemId);        
              this.ToTable("InventoryItems-DoNotMap");
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.InventoryItem).WithOptional(t => (InventoryItems_DoNotMap)t.InventoryItems_DoNotMap);
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
