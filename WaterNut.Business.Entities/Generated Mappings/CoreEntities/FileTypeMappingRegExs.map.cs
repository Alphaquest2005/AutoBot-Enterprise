﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypeMappingRegExsMap : EntityTypeConfiguration<FileTypeMappingRegExs>
    {
        public FileTypeMappingRegExsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypeMappingRegExs");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FileTypeMappingId).HasColumnName("FileTypeMappingId");
              this.Property(t => t.ReplacementRegex).HasColumnName("ReplacementRegex").IsRequired().HasMaxLength(1000);
              this.Property(t => t.ReplacementValue).HasColumnName("ReplacementValue").HasMaxLength(1000);
              this.HasRequired(t => t.FileTypeMappings).WithMany(t =>(ICollection<FileTypeMappingRegExs>) t.FileTypeMappingRegExs).HasForeignKey(d => d.FileTypeMappingId);
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
