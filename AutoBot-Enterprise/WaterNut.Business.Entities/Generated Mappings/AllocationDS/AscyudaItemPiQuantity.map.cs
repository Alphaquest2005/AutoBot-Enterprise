namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AscyudaItemPiQuantityMap : EntityTypeConfiguration<AscyudaItemPiQuantity>
    {
        public AscyudaItemPiQuantityMap()
        {                        
              this.HasKey(t => t.Item_Id);        
              this.ToTable("AscyudaItemPiQuantity");
              this.Property(t => t.Item_Id).HasColumnName("Item_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.PiQuantity).HasColumnName("PiQuantity");
              this.Property(t => t.PiWeight).HasColumnName("PiWeight");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasMany(t => t.AdjustmentShortAllocations).WithOptional(t => t.AscyudaItemPiQuantity).HasForeignKey(d => d.PreviousItem_Id);
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
