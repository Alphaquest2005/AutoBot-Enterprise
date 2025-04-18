﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypeActionsMap : EntityTypeConfiguration<FileTypeActions>
    {
        public FileTypeActionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypeActions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.ActionId).HasColumnName("ActionId");
              this.Property(t => t.AssessIM7).HasColumnName("AssessIM7");
              this.Property(t => t.AssessEX).HasColumnName("AssessEX");
              this.HasRequired(t => t.Actions).WithMany(t =>(ICollection<FileTypeActions>) t.FileTypeActions).HasForeignKey(d => d.ActionId);
              this.HasRequired(t => t.FileTypes).WithMany(t =>(ICollection<FileTypeActions>) t.FileTypeActions).HasForeignKey(d => d.FileTypeId);
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
