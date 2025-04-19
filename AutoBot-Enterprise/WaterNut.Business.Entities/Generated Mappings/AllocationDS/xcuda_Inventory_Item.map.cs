namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_Inventory_ItemMap : EntityTypeConfiguration<xcuda_Inventory_Item>
    {
        public xcuda_Inventory_ItemMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("xcuda_Inventory_Item");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InventoryItemId).HasColumnName("InventoryItemId");
              this.HasRequired(t => t.xcuda_HScode).WithOptional(t => (xcuda_Inventory_Item)t.xcuda_Inventory_Item);
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
