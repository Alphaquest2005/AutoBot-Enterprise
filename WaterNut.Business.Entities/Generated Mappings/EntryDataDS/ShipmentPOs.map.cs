﻿namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ShipmentPOsMap : EntityTypeConfiguration<ShipmentPOs>
    {
        public ShipmentPOsMap()
        {                        
              this.HasKey(t => new {t.EntryData_Id, t.EmailDate});        
              this.ToTable("ShipmentPOs");
              this.Property(t => t.EntryData_Id).HasColumnName("EntryData_Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.InvoiceNo).HasColumnName("InvoiceNo").IsRequired().HasMaxLength(50);
              this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
              this.Property(t => t.SubTotal).HasColumnName("SubTotal");
              this.Property(t => t.InvoiceTotal).HasColumnName("InvoiceTotal");
              this.Property(t => t.ImportedLine).HasColumnName("ImportedLine");
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").HasMaxLength(100);
              this.Property(t => t.SourceFile).HasColumnName("SourceFile");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.Property(t => t.EmailDate).HasColumnName("EmailDate");
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
