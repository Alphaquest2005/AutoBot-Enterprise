namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class SupplierItemDescriptionRegExMap : EntityTypeConfiguration<SupplierItemDescriptionRegEx>
    {
        public SupplierItemDescriptionRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("SupplierItemDescriptionRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.SupplierItemDescriptionId).HasColumnName("SupplierItemDescriptionId");
              this.Property(t => t.Attribute).HasColumnName("Attribute").IsRequired().HasMaxLength(50);
              this.Property(t => t.RegEx).HasColumnName("RegEx").IsRequired().HasMaxLength(255);
              this.Property(t => t.POSRegEx).HasColumnName("POSRegEx").IsRequired().HasMaxLength(50);
              this.HasRequired(t => t.SupplierItemDescription).WithMany(t =>(ICollection<SupplierItemDescriptionRegEx>) t.SupplierItemDescriptionRegEx).HasForeignKey(d => d.SupplierItemDescriptionId);
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
