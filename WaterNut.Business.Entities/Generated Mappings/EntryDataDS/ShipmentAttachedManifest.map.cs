namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentAttachedManifestMap : EntityTypeConfiguration<ShipmentAttachedManifest>
    {
        public ShipmentAttachedManifestMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentAttachedManifest");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ShipmentId).HasColumnName("ShipmentId");
              this.Property(t => t.ManifestId).HasColumnName("ManifestId");
              this.HasRequired(t => t.Shipment).WithMany(t =>(ICollection<ShipmentAttachedManifest>) t.ShipmentAttachedManifest).HasForeignKey(d => d.ShipmentId);
              this.HasRequired(t => t.ShipmentManifest).WithMany(t =>(ICollection<ShipmentAttachedManifest>) t.ShipmentAttachedManifest).HasForeignKey(d => d.ManifestId);
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
