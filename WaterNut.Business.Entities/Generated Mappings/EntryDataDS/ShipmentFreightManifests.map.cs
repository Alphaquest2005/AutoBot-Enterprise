namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentFreightManifestsMap : EntityTypeConfiguration<ShipmentFreightManifests>
    {
        public ShipmentFreightManifestsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentFreightManifests");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.FreightDetailId).HasColumnName("FreightDetailId");
              this.Property(t => t.FreightId).HasColumnName("FreightId");
              this.Property(t => t.ManifestId).HasColumnName("ManifestId");
              this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").HasMaxLength(50);
              this.HasRequired(t => t.ShipmentFreight).WithMany(t =>(ICollection<ShipmentFreightManifests>) t.ShipmentFreightManifests).HasForeignKey(d => d.FreightId);
              this.HasRequired(t => t.ShipmentManifest).WithMany(t =>(ICollection<ShipmentFreightManifests>) t.ShipmentFreightManifests).HasForeignKey(d => d.ManifestId);
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
