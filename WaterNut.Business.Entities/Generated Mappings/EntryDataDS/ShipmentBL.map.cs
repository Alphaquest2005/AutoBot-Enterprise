namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentBLMap : EntityTypeConfiguration<ShipmentBL>
    {
        public ShipmentBLMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentBL");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.Vessel).HasColumnName("Vessel").HasMaxLength(50);
              this.Property(t => t.Voyage).HasColumnName("Voyage").HasMaxLength(50);
              this.Property(t => t.Container).HasColumnName("Container").HasMaxLength(50);
              this.Property(t => t.Seals).HasColumnName("Seals").HasMaxLength(50);
              this.Property(t => t.Type).HasColumnName("Type").HasMaxLength(50);
              this.Property(t => t.PackagesNo).HasColumnName("PackagesNo");
              this.Property(t => t.PackagesType).HasColumnName("PackagesType").IsRequired().HasMaxLength(50);
              this.Property(t => t.WeightKG).HasColumnName("WeightKG");
              this.Property(t => t.VolumeM3).HasColumnName("VolumeM3");
              this.Property(t => t.WeightLB).HasColumnName("WeightLB");
              this.Property(t => t.VolumeCF).HasColumnName("VolumeCF");
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(50);
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Freight).HasColumnName("Freight");
              this.Property(t => t.FreightCurrency).HasColumnName("FreightCurrency").HasMaxLength(10);
              this.HasMany(t => t.ShimentBLCharges).WithRequired(t => (ShipmentBL)t.ShipmentBL);
              this.HasMany(t => t.ShipmentAttachedBL).WithRequired(t => (ShipmentBL)t.ShipmentBL);
              this.HasMany(t => t.ShipmentBLDetails).WithRequired(t => (ShipmentBL)t.ShipmentBL);
              this.HasMany(t => t.ShipmentManifestBLs).WithRequired(t => (ShipmentBL)t.ShipmentBL);
              this.HasMany(t => t.ShipmentRiderBLs).WithRequired(t => (ShipmentBL)t.ShipmentBL);
              this.HasMany(t => t.ShipmentBLFreight).WithRequired(t => (ShipmentBL)t.ShipmentBL);
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
