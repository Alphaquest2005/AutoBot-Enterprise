namespace AllocationDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TariffCodesMap : EntityTypeConfiguration<TariffCodes>
    {
        public TariffCodesMap()
        {                        
              this.HasKey(t => t.TariffCode);        
              this.ToTable("TariffCodes");
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.Description).HasColumnName("Description").HasMaxLength(999);
              this.Property(t => t.RateofDuty).HasColumnName("RateofDuty").HasMaxLength(50);
              this.Property(t => t.EnvironmentalLevy).HasColumnName("EnvironmentalLevy").HasMaxLength(50);
              this.Property(t => t.CustomsServiceCharge).HasColumnName("CustomsServiceCharge").HasMaxLength(50);
              this.Property(t => t.ExciseTax).HasColumnName("ExciseTax").HasMaxLength(50);
              this.Property(t => t.VatRate).HasColumnName("VatRate").HasMaxLength(50);
              this.Property(t => t.PetrolTax).HasColumnName("PetrolTax").HasMaxLength(50);
              this.Property(t => t.Units).HasColumnName("Units").HasMaxLength(50);
              this.Property(t => t.SiteRev3).HasColumnName("SiteRev3").HasMaxLength(50);
              this.Property(t => t.TariffCategoryCode).HasColumnName("TariffCategoryCode").HasMaxLength(50);
              this.Property(t => t.LicenseRequired).HasColumnName("LicenseRequired");
              this.Property(t => t.Invalid).HasColumnName("Invalid");
              this.Property(t => t.LicenseDescription).HasColumnName("LicenseDescription").HasMaxLength(50);
              this.HasOptional(t => t.TariffCategory).WithMany(t =>(ICollection<TariffCodes>) t.TariffCodes).HasForeignKey(d => d.TariffCategoryCode);
              this.HasMany(t => t.InventoryItemsEx).WithOptional(t => t.TariffCodes).HasForeignKey(d => d.TariffCode);
              this.HasMany(t => t.xcuda_HScode).WithRequired(t => (TariffCodes)t.TariffCodes);
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
