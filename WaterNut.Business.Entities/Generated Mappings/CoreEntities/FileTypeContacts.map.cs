﻿namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypeContactsMap : EntityTypeConfiguration<FileTypeContacts>
    {
        public FileTypeContactsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypeContacts");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.ContactId).HasColumnName("ContactId");
              this.HasRequired(t => t.Contacts).WithMany(t =>(ICollection<FileTypeContacts>) t.FileTypeContacts).HasForeignKey(d => d.ContactId);
              this.HasRequired(t => t.FileTypes).WithMany(t =>(ICollection<FileTypeContacts>) t.FileTypeContacts).HasForeignKey(d => d.FileTypeId);
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
