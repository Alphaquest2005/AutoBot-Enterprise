﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ImportActionsMap : EntityTypeConfiguration<ImportActions>
    {
        public ImportActionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ImportActions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
              this.Property(t => t.Action).HasColumnName("Action").IsRequired().HasMaxLength(500);
              this.HasRequired(t => t.FileTypes).WithMany(t =>(ICollection<ImportActions>) t.ImportActions).HasForeignKey(d => d.FileTypeId);
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
