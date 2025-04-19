namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SuppliersMap : EntityTypeConfiguration<Suppliers>
    {
        public SuppliersMap()
        {                        
              this.HasKey(t => new {t.SupplierCode, t.ApplicationSettingsId});        
              this.ToTable("Suppliers");
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").IsRequired().HasMaxLength(100);
              this.Property(t => t.SupplierName).HasColumnName("SupplierName").HasMaxLength(510);
              this.Property(t => t.Street).HasColumnName("Street").HasMaxLength(100);
              this.Property(t => t.City).HasColumnName("City").HasMaxLength(38);
              this.Property(t => t.Country).HasColumnName("Country").HasMaxLength(100);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.CountryCode).HasColumnName("CountryCode").HasMaxLength(3);
              this.HasMany(t => t.EntryData).WithOptional(t => t.Suppliers).HasForeignKey(d => new {d.SupplierCode, d.ApplicationSettingsId});
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
