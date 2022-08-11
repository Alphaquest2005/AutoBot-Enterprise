namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SupplierItemDescriptionMap : EntityTypeConfiguration<SupplierItemDescription>
    {
        public SupplierItemDescriptionMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("SupplierItemDescription");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.SupplierCode).HasColumnName("SupplierCode").IsRequired().HasMaxLength(100);
              this.Property(t => t.Item).HasColumnName("Item").IsRequired().HasMaxLength(50);
              this.HasMany(t => t.SupplierItemDescriptionRegEx).WithRequired(t => (SupplierItemDescription)t.SupplierItemDescription);
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
