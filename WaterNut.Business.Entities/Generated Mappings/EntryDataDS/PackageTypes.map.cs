namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PackageTypesMap : EntityTypeConfiguration<PackageTypes>
    {
        public PackageTypesMap()
        {                        
              this.HasKey(t => t.PackageDescription);        
              this.ToTable("PackageTypes");
              this.Property(t => t.PackageType).HasColumnName("PackageType").IsRequired().HasMaxLength(4);
              this.Property(t => t.PackageDescription).HasColumnName("PackageDescription").IsRequired().HasMaxLength(50);
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
