namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypeMappingsValuesMap : EntityTypeConfiguration<FileTypeMappingsValues>
    {
        public FileTypeMappingsValuesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypeMappingsValues");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FileTypeMappingId).HasColumnName("FileTypeMappingId");
              this.Property(t => t.Value).HasColumnName("Value").IsRequired().HasMaxLength(50);
              this.HasRequired(t => t.FileTypeMappings).WithMany(t =>(ICollection<FileTypeMappingsValues>) t.FileTypeMappingValues).HasForeignKey(d => d.FileTypeMappingId);
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
