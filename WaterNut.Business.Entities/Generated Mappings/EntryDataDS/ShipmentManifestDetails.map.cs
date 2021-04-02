namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentManifestDetailsMap : EntityTypeConfiguration<ShipmentManifestDetails>
    {
        public ShipmentManifestDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentManifestDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ManifestId).HasColumnName("ManifestId");
              this.Property(t => t.ContainerID).HasColumnName("ContainerID").HasMaxLength(50);
              this.Property(t => t.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
              this.HasRequired(t => t.ShipmentManifest).WithMany(t =>(ICollection<ShipmentManifestDetails>) t.ShipmentManifestDetails).HasForeignKey(d => d.ManifestId);
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
