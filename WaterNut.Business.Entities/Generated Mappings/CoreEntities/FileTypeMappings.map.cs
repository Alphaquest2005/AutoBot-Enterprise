namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypeMappingsMap : EntityTypeConfiguration<FileTypeMappings>
    {
        public FileTypeMappingsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypeMappings");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.OriginalName).HasColumnName("OriginalName").IsRequired().HasMaxLength(50);
              this.Property(t => t.DestinationName).HasColumnName("DestinationName").IsRequired().HasMaxLength(50);
              this.Property(t => t.DataType).HasColumnName("DataType").IsRequired().HasMaxLength(50);
              this.Property(t => t.Required).HasColumnName("Required");
              this.HasRequired(t => t.FileTypes).WithMany(t =>(ICollection<FileTypeMappings>) t.FileTypeMappings).HasForeignKey(d => d.FileTypeId);
              this.HasMany(t => t.FileTypeMappingRegExs).WithRequired(t => (FileTypeMappings)t.FileTypeMappings);
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
