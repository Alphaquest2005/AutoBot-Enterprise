﻿namespace LicenseDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TODO_LicenceAvailableQtyMap : EntityTypeConfiguration<TODO_LicenceAvailableQty>
    {
        public TODO_LicenceAvailableQtyMap()
        {                        
              this.HasKey(t => t.SegmentId);        
              this.ToTable("TODO-LicenceAvailableQty");
              this.Property(t => t.RegistrationNumber).HasColumnName("RegistrationNumber").IsRequired().HasMaxLength(50);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").HasMaxLength(8);
              this.Property(t => t.Origin).HasColumnName("Origin").HasMaxLength(255);
              this.Property(t => t.Quantity_requested).HasColumnName("Quantity_requested");
              this.Property(t => t.Application_date).HasColumnName("Application_date");
              this.Property(t => t.Importation_date).HasColumnName("Importation_date");
              this.Property(t => t.Key).HasColumnName("Key").HasMaxLength(55);
              this.Property(t => t.Balance).HasColumnName("Balance");
              this.Property(t => t.LicenseId).HasColumnName("LicenseId");
              this.Property(t => t.SourceFile).HasColumnName("SourceFile").IsRequired().HasMaxLength(300);
              this.Property(t => t.DocumentReference).HasColumnName("DocumentReference").HasMaxLength(50);
              this.Property(t => t.SegmentId).HasColumnName("SegmentId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.HasRequired(t => t.xLIC_Lic_item_segment).WithOptional(t => (TODO_LicenceAvailableQty)t.TODO_LicenceAvailableQty);
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