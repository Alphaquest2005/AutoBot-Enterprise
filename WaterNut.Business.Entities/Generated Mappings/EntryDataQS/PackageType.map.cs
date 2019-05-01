namespace EntryDataQS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PackageTypeMap : EntityTypeConfiguration<PackageType>
    {
        public PackageTypeMap()
        {                        
              this.HasKey(t => t.PackageCode);        
              this.ToTable("PackageTypes");
              this.Property(t => t.PackageCode).HasColumnName("PackageType").IsRequired().HasMaxLength(4);
              this.Property(t => t.PackageDescription).HasColumnName("PackageDescription").HasMaxLength(50);
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
