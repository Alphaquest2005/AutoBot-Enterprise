namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderManifestsMap : EntityTypeConfiguration<ShipmentRiderManifests>
    {
        public ShipmentRiderManifestsMap()
        {                        
              this.HasKey(t => new {t.RiderId, t.RiderDetailId, t.ManifestId, t.ETA, t.BLNumber, t.Quantity, t.Marks, t.ManifestMark});        
              this.ToTable("ShipmentRiderManifests");
              this.Property(t => t.id).HasColumnName("id");
              this.Property(t => t.RiderId).HasColumnName("RiderId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.RiderDetailId).HasColumnName("RiderDetailId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ManifestId).HasColumnName("ManifestId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.Quantity).HasColumnName("Quantity").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Marks).HasColumnName("Marks").IsRequired().HasMaxLength(50);
              this.Property(t => t.ManifestMark).HasColumnName("ManifestMark").IsRequired().HasMaxLength(1000);
              this.HasRequired(t => t.ShipmentRider).WithMany(t =>(ICollection<ShipmentRiderManifests>) t.ShipmentRiderManifests).HasForeignKey(d => d.RiderId);
              this.HasRequired(t => t.ShipmentManifest).WithMany(t =>(ICollection<ShipmentRiderManifests>) t.ShipmentRiderManifests).HasForeignKey(d => d.ManifestId);
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
