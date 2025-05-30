﻿namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentManifestMap : EntityTypeConfiguration<ShipmentManifest>
    {
        public ShipmentManifestMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentManifest");
              this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.RegistrationDate).HasColumnName("RegistrationDate");
              this.Property(t => t.CustomsOffice).HasColumnName("CustomsOffice").IsRequired().HasMaxLength(50);
              this.Property(t => t.Voyage).HasColumnName("Voyage").HasMaxLength(50);
              this.Property(t => t.ETD).HasColumnName("ETD");
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.Vessel).HasColumnName("Vessel").HasMaxLength(50);
              this.Property(t => t.WayBill).HasColumnName("WayBill").IsRequired().HasMaxLength(50);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.LoadingPort).HasColumnName("LoadingPort").HasMaxLength(50);
              this.Property(t => t.ModeOfTransport).HasColumnName("ModeOfTransport").HasMaxLength(50);
              this.Property(t => t.TypeOfBL).HasColumnName("TypeOfBL").HasMaxLength(50);
              this.Property(t => t.CargoReporter).HasColumnName("CargoReporter").HasMaxLength(255);
              this.Property(t => t.Exporter).HasColumnName("Exporter").HasMaxLength(50);
              this.Property(t => t.Notify).HasColumnName("Notify").HasMaxLength(50);
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.Property(t => t.PackageType).HasColumnName("PackageType").IsRequired().HasMaxLength(50);
              this.Property(t => t.GrossWeightKG).HasColumnName("GrossWeightKG");
              this.Property(t => t.Volume).HasColumnName("Volume");
              this.Property(t => t.Freight).HasColumnName("Freight");
              this.Property(t => t.LocationOfGoods).HasColumnName("LocationOfGoods").HasMaxLength(255);
              this.Property(t => t.Goods).HasColumnName("Goods").HasMaxLength(1000);
              this.Property(t => t.Marks).HasColumnName("Marks").HasMaxLength(1000);
              this.Property(t => t.Containers).HasColumnName("Containers");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.FreightCurrency).HasColumnName("FreightCurrency").HasMaxLength(10);
              this.Property(t => t.ConsigneeName).HasColumnName("ConsigneeName").HasMaxLength(100);
              this.HasOptional(t => t.Consignees).WithMany(t =>(ICollection<ShipmentManifest>) t.ShipmentManifest).HasForeignKey(d => d.ConsigneeName);
              this.HasMany(t => t.ShipmentAttachedManifest).WithRequired(t => (ShipmentManifest)t.ShipmentManifest);
              this.HasMany(t => t.ShipmentManifestDetails).WithRequired(t => (ShipmentManifest)t.ShipmentManifest);
              this.HasMany(t => t.ShipmentManifestBLs).WithRequired(t => (ShipmentManifest)t.ShipmentManifest);
              this.HasMany(t => t.ShipmentRiderManifests).WithRequired(t => (ShipmentManifest)t.ShipmentManifest);
              this.HasMany(t => t.ShipmentFreightManifests).WithRequired(t => (ShipmentManifest)t.ShipmentManifest);
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
