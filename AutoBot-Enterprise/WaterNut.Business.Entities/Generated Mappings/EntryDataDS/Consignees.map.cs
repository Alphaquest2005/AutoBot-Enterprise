namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ConsigneesMap : EntityTypeConfiguration<Consignees>
    {
        public ConsigneesMap()
        {                        
              this.HasKey(t => t.ConsigneeName);        
              this.ToTable("Consignees");
              this.Property(t => t.ConsigneeName).HasColumnName("ConsigneeName").IsRequired().HasMaxLength(100);
              this.Property(t => t.ConsigneeCode).HasColumnName("ConsigneeCode").HasMaxLength(100);
              this.Property(t => t.Address).HasColumnName("Address").HasMaxLength(300);
              this.Property(t => t.CountryCode).HasColumnName("CountryCode").HasMaxLength(3);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasMany(t => t.Shipment).WithOptional(t => t.Consignees).HasForeignKey(d => d.ConsigneeName);
              this.HasMany(t => t.ShipmentBL).WithOptional(t => t.Consignees).HasForeignKey(d => d.ConsigneeName);
              this.HasMany(t => t.ShipmentManifest).WithOptional(t => t.Consignees).HasForeignKey(d => d.ConsigneeName);
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
