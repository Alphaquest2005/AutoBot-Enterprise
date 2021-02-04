namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentBLDetailsMap : EntityTypeConfiguration<ShipmentBLDetails>
    {
        public ShipmentBLDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentBLDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.PackageType).HasColumnName("PackageType").IsRequired().HasMaxLength(50);
              this.Property(t => t.Marks).HasColumnName("Marks").IsRequired().HasMaxLength(50);
              this.Property(t => t.Comments).HasColumnName("Comments").HasMaxLength(1000);
              this.Property(t => t.BLId).HasColumnName("BLId");
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShipmentBLDetails>) t.ShipmentBLDetails).HasForeignKey(d => d.BLId);
              this.HasMany(t => t.ShipmentRiderBLs).WithRequired(t => (ShipmentBLDetails)t.ShipmentBLDetails);
              this.HasMany(t => t.ShipmentFreightBLs).WithRequired(t => (ShipmentBLDetails)t.ShipmentBLDetails);
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
