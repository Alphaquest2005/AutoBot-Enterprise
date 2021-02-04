namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderBLsMap : EntityTypeConfiguration<ShipmentRiderBLs>
    {
        public ShipmentRiderBLsMap()
        {                        
              this.HasKey(t => t.id);        
              this.ToTable("ShipmentRiderBLs");
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Marks).HasColumnName("Marks").IsRequired().HasMaxLength(50);
              this.Property(t => t.id).HasColumnName("id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.RiderId).HasColumnName("RiderId");
              this.Property(t => t.RiderDetailId).HasColumnName("RiderDetailId");
              this.Property(t => t.BLDetailId).HasColumnName("BLDetailId");
              this.Property(t => t.BLId).HasColumnName("BLId");
              this.HasRequired(t => t.ShipmentRider).WithMany(t =>(ICollection<ShipmentRiderBLs>) t.ShipmentRiderBLs).HasForeignKey(d => d.RiderId);
              this.HasRequired(t => t.ShipmentBLDetails).WithMany(t =>(ICollection<ShipmentRiderBLs>) t.ShipmentRiderBLs).HasForeignKey(d => d.BLDetailId);
              this.HasRequired(t => t.ShipmentRiderDetails).WithMany(t =>(ICollection<ShipmentRiderBLs>) t.ShipmentRiderBLs).HasForeignKey(d => d.RiderDetailId);
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShipmentRiderBLs>) t.ShipmentRiderBLs).HasForeignKey(d => d.BLId);
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
