namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderMap : EntityTypeConfiguration<ShipmentRider>
    {
        public ShipmentRiderMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentRider");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.DocumentDate).HasColumnName("DocumentDate");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasMany(t => t.ShipmentRiderDetails).WithRequired(t => (ShipmentRider)t.ShipmentRider);
              this.HasMany(t => t.ShipmentRiderInvoice).WithRequired(t => (ShipmentRider)t.ShipmentRider);
              this.HasMany(t => t.ShipmentRiderBLs).WithRequired(t => (ShipmentRider)t.ShipmentRider);
              this.HasMany(t => t.ShipmentAttachedRider).WithRequired(t => (ShipmentRider)t.ShipmentRider);
              this.HasOptional(t => t.ShipmentRiderEx).WithRequired(t => (ShipmentRider) t.ShipmentRider);
              this.HasMany(t => t.ShipmentRiderManifests).WithRequired(t => (ShipmentRider)t.ShipmentRider);
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
