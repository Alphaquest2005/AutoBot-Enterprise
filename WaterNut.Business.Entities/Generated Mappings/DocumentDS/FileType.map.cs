namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FileTypeMap : EntityTypeConfiguration<FileType>
    {
        public FileTypeMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("FileTypes");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.FilePattern).HasColumnName("FilePattern").IsRequired().HasMaxLength(255);
              this.Property(t => t.Type).HasColumnName("Type").IsRequired().HasMaxLength(50);
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.CreateDocumentSet).HasColumnName("CreateDocumentSet");
              this.Property(t => t.DocumentSpecific).HasColumnName("DocumentSpecific");
              this.Property(t => t.DocumentCode).HasColumnName("DocumentCode").IsRequired().HasMaxLength(50);
              this.Property(t => t.ReplyToMail).HasColumnName("ReplyToMail");
              this.Property(t => t.FileGroupId).HasColumnName("FileGroupId");
              this.Property(t => t.MergeEmails).HasColumnName("MergeEmails");
              this.Property(t => t.CopyEntryData).HasColumnName("CopyEntryData");
              this.Property(t => t.ParentFileTypeId).HasColumnName("ParentFileTypeId");
              this.Property(t => t.OverwriteFiles).HasColumnName("OverwriteFiles");
              this.Property(t => t.HasFiles).HasColumnName("HasFiles");
              this.HasRequired(t => t.AsycudaDocumentSet).WithMany(t =>(ICollection<FileType>) t.FileTypes).HasForeignKey(d => d.AsycudaDocumentSetId);
              this.HasOptional(t => t.FileType1).WithMany(t =>(ICollection<FileType>) t.FileTypes1).HasForeignKey(d => d.ParentFileTypeId);
              this.HasMany(t => t.AsycudaDocumentSet_Attachments).WithOptional(t => t.FileType).HasForeignKey(d => d.FileTypeId);
              this.HasMany(t => t.FileTypes1).WithOptional(t => t.FileType1).HasForeignKey(d => d.ParentFileTypeId);
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
