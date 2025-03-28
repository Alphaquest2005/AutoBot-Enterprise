﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileImporterInfoMap : EntityTypeConfiguration<FileImporterInfo>
    {
        public FileImporterInfoMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypes-FileImporterInfo");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EntryType).HasColumnName("EntryType").IsRequired().HasMaxLength(50);
              this.Property(t => t.Format).HasColumnName("Format").IsRequired().HasMaxLength(50);
              this.HasMany(t => t.FileTypes).WithOptional(t => t.FileImporterInfos).HasForeignKey(d => d.FileInfoId);
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
