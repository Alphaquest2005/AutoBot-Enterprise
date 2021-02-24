namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentRiderDetailsMap : EntityTypeConfiguration<ShipmentRiderDetails>
    {
        public ShipmentRiderDetailsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentRiderDetails");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Consignee).HasColumnName("Consignee").IsRequired().HasMaxLength(50);
              this.Property(t => t.Code).HasColumnName("Code").IsRequired().HasMaxLength(50);
              this.Property(t => t.Shipper).HasColumnName("Shipper").IsRequired().HasMaxLength(50);
              this.Property(t => t.TrackingNumber).HasColumnName("TrackingNumber").HasMaxLength(255);
              this.Property(t => t.Pieces).HasColumnName("Pieces");
              this.Property(t => t.WarehouseCode).HasColumnName("WarehouseCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber").HasMaxLength(255);
              this.Property(t => t.GrossWeightKg).HasColumnName("GrossWeightKg");
              this.Property(t => t.CubicFeet).HasColumnName("CubicFeet");
              this.Property(t => t.RiderId).HasColumnName("RiderId");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.HasRequired(t => t.ShipmentRider).WithMany(t =>(ICollection<ShipmentRiderDetails>) t.ShipmentRiderDetails).HasForeignKey(d => d.RiderId);
              this.HasMany(t => t.ShipmentRiderInvoice).WithRequired(t => (ShipmentRiderDetails)t.ShipmentRiderDetails);
              this.HasMany(t => t.ShipmentRiderBLs).WithRequired(t => (ShipmentRiderDetails)t.ShipmentRiderDetails);
              this.HasMany(t => t.ShipmentInvoiceRiderDetails).WithRequired(t => (ShipmentRiderDetails)t.ShipmentRiderDetails);
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
