namespace InventoryQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TariffSupUnitLkpsMap : EntityTypeConfiguration<TariffSupUnitLkps>
    {
        public TariffSupUnitLkpsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("TariffSupUnitLkps");
              this.Property(t => t.SuppUnitCode2).HasColumnName("SuppUnitCode2").IsRequired().HasMaxLength(50);
              this.Property(t => t.SuppUnitName2).HasColumnName("SuppUnitName2").HasMaxLength(50);
              this.Property(t => t.SuppQty).HasColumnName("SuppQty");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.HasMany(t => t.TariffCategoryCodeSuppUnit).WithRequired(t => (TariffSupUnitLkps)t.TariffSupUnitLkps);
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
