namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentMap : EntityTypeConfiguration<Shipment>
    {
        public ShipmentMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("Shipment");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ManifestNumber).HasColumnName("ManifestNumber").HasMaxLength(50);
              this.Property(t => t.ExpectedEntries).HasColumnName("ExpectedEntries");
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").HasMaxLength(50);
              this.Property(t => t.ShipmentName).HasColumnName("ShipmentName").IsRequired().HasMaxLength(50);
              this.Property(t => t.WeightKG).HasColumnName("WeightKG");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(3);
              this.Property(t => t.Origin).HasColumnName("Origin").HasMaxLength(2);
              this.Property(t => t.TotalInvoices).HasColumnName("TotalInvoices");
              this.Property(t => t.Packages).HasColumnName("Packages");
              this.Property(t => t.FreightCurrency).HasColumnName("FreightCurrency").HasMaxLength(3);
              this.Property(t => t.Location).HasColumnName("Location").HasMaxLength(50);
              this.Property(t => t.Maxlines).HasColumnName("Maxlines");
              this.Property(t => t.Office).HasColumnName("Office").HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Freight).HasColumnName("Freight");
              this.Property(t => t.ConsigneeCode).HasColumnName("ConsigneeCode").HasMaxLength(100);
              this.Property(t => t.ConsigneeName).HasColumnName("ConsigneeName").HasMaxLength(100);
              this.Property(t => t.ConsigneeAddress).HasColumnName("ConsigneeAddress").HasMaxLength(300);
              this.HasOptional(t => t.Consignees).WithMany(t =>(ICollection<Shipment>) t.Shipment).HasForeignKey(d => d.ConsigneeName);
              this.HasMany(t => t.ShipmentAttachedBL).WithRequired(t => (Shipment)t.Shipment);
              this.HasMany(t => t.ShipmentAttachedFreight).WithRequired(t => (Shipment)t.Shipment);
              this.HasMany(t => t.ShipmentAttachedInvoices).WithRequired(t => (Shipment)t.Shipment);
              this.HasMany(t => t.ShipmentAttachedManifest).WithRequired(t => (Shipment)t.Shipment);
              this.HasMany(t => t.ShipmentAttachedRider).WithRequired(t => (Shipment)t.Shipment);
              this.HasMany(t => t.ShipmentAttachments).WithRequired(t => (Shipment)t.Shipment);
              this.HasMany(t => t.ShipmentAttachedPOs).WithRequired(t => (Shipment)t.Shipment);
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
