﻿namespace EntryDataDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AttachmentsMap : EntityTypeConfiguration<Attachments>
    {
        public AttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("Attachments");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FilePath).HasColumnName("FilePath").IsRequired().HasMaxLength(255);
              this.Property(t => t.DocumentCode).HasColumnName("DocumentCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.Reference).HasColumnName("Reference").IsRequired().HasMaxLength(255);
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(10);
              this.HasMany(t => t.ShipmentAttachments).WithRequired(t => (Attachments)t.Attachments);
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