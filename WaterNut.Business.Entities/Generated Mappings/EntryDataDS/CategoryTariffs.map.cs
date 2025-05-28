namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class CategoryTariffsMap : EntityTypeConfiguration<CategoryTariffs>
    {
        public CategoryTariffsMap()
        {                        
              this.HasKey(t => new {t.Category, t.TariffCode});        
              this.ToTable("CategoryTariffs");
              this.Property(t => t.Category).HasColumnName("Category").IsRequired().HasMaxLength(50);
              this.Property(t => t.TariffCode).HasColumnName("TariffCode").IsRequired().HasMaxLength(12);
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
