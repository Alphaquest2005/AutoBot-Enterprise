namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentManifestBLsMap : EntityTypeConfiguration<ShipmentManifestBLs>
    {
        public ShipmentManifestBLsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentManifestBLs");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ManifestId).HasColumnName("ManifestId");
              this.Property(t => t.BLId).HasColumnName("BLId");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.WayBill).HasColumnName("WayBill").IsRequired().HasMaxLength(50);
              this.HasRequired(t => t.ShipmentBL).WithMany(t =>(ICollection<ShipmentManifestBLs>) t.ShipmentManifestBLs).HasForeignKey(d => d.BLId);
              this.HasRequired(t => t.ShipmentManifest).WithMany(t =>(ICollection<ShipmentManifestBLs>) t.ShipmentManifestBLs).HasForeignKey(d => d.ManifestId);
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
