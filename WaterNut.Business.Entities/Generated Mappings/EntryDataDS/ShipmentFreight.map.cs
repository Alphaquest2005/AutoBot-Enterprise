namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentFreightMap : EntityTypeConfiguration<ShipmentFreight>
    {
        public ShipmentFreightMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentFreight");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.BLNumber).HasColumnName("BLNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.Consignee).HasColumnName("Consignee").IsRequired().HasMaxLength(1000);
              this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.DueDate).HasColumnName("DueDate");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.ETA).HasColumnName("ETA");
              this.Property(t => t.EmailId).HasColumnName("EmailId");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(3);
              this.HasMany(t => t.ShipmentFreightDetails).WithRequired(t => (ShipmentFreight)t.ShipmentFreight);
              this.HasMany(t => t.ShipmentAttachedFreight).WithRequired(t => (ShipmentFreight)t.ShipmentFreight);
              this.HasMany(t => t.ShipmentBLFreight).WithRequired(t => (ShipmentFreight)t.ShipmentFreight);
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
