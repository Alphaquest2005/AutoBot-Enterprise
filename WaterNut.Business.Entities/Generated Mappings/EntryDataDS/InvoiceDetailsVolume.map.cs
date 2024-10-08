﻿namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InvoiceDetailsVolumeMap : EntityTypeConfiguration<InvoiceDetailsVolume>
    {
        public InvoiceDetailsVolumeMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ShipmentInvoiceDetails-Volume");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Quantity).HasColumnName("Quantity");
              this.Property(t => t.Units).HasColumnName("Units").IsRequired().HasMaxLength(50);
              this.HasRequired(t => t.ShipmentInvoiceDetails).WithOptional(t => (InvoiceDetailsVolume)t.Volume);
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
