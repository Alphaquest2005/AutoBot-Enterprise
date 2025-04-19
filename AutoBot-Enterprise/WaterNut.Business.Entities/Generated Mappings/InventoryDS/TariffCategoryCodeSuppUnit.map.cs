namespace InventoryDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TariffCategoryCodeSuppUnitMap : EntityTypeConfiguration<TariffCategoryCodeSuppUnit>
    {
        public TariffCategoryCodeSuppUnitMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("TariffCategoryCodeSuppUnit");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.TariffCategoryCode).HasColumnName("TariffCategoryCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.TariffSupUnitId).HasColumnName("TariffSupUnitId");
              this.HasRequired(t => t.TariffCategory).WithMany(t =>(ICollection<TariffCategoryCodeSuppUnit>) t.TariffCategoryCodeSuppUnits).HasForeignKey(d => d.TariffCategoryCode);
              this.HasRequired(t => t.TariffSupUnitLkp).WithMany(t =>(ICollection<TariffCategoryCodeSuppUnit>) t.TariffCategoryCodeSuppUnits).HasForeignKey(d => d.TariffSupUnitId);
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
