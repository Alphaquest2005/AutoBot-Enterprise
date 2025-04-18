﻿namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentInvoiceMap : EntityTypeConfiguration<ShipmentInvoice>
    {
        public ShipmentInvoiceMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoice");
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.ImportedLines).HasColumnName("ImportedLines");
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").HasMaxLength(100);
              this.Property(t => t.TotalInternalFreight).HasColumnName("TotalInternalFreight");
              this.Property(t => t.Currency).HasColumnName("Currency").HasMaxLength(4);
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.TotalOtherCost).HasColumnName("TotalOtherCost");
              this.Property(t => t.TotalInsurance).HasColumnName("TotalInsurance");
              this.Property(t => t.TotalDeduction).HasColumnName("TotalDeduction");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired();
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.FileLineNumber).HasColumnName("FileLineNumber");
              this.Property(t => t.SubTotal).HasColumnName("SubTotal");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.SupplierName).HasColumnName("SupplierName").HasMaxLength(100);
              this.Property(t => t.SupplierAddress).HasColumnName("SupplierAddress").HasMaxLength(500);
              this.Property(t => t.SupplierCountry).HasColumnName("SupplierCountry").HasMaxLength(50);
              this.Property(t => t.ConsigneeName).HasColumnName("ConsigneeName").HasMaxLength(100);
              this.Property(t => t.ConsigneeAddress).HasColumnName("ConsigneeAddress").HasMaxLength(500);
              this.Property(t => t.ConsigneeCountry).HasColumnName("ConsigneeCountry").HasMaxLength(50);
              this.HasMany(t => t.InvoiceDetails).WithRequired(t => (ShipmentInvoice)t.Invoice);
              this.HasMany(t => t.InvoiceExtraInfo).WithRequired(t => (ShipmentInvoice)t.Invoice);
              this.HasMany(t => t.ShipmentRiderInvoice).WithOptional(t => t.ShipmentInvoice).HasForeignKey(d => d.InvoiceId);
              this.HasMany(t => t.ShipmentAttachedInvoices).WithRequired(t => (ShipmentInvoice)t.ShipmentInvoice);
              this.HasMany(t => t.ShipmentInvoicePOs).WithRequired(t => (ShipmentInvoice)t.ShipmentInvoice);
              this.HasMany(t => t.ShipmentInvoiceRiderDetails).WithRequired(t => (ShipmentInvoice)t.ShipmentInvoice);
              this.HasMany(t => t.ShipmentBLInvoice).WithOptional(t => t.ShipmentInvoice).HasForeignKey(d => d.InvoiceId);
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
