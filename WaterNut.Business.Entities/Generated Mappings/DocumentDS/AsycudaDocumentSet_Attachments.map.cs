namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSet_AttachmentsMap : EntityTypeConfiguration<AsycudaDocumentSet_Attachments>
    {
        public AsycudaDocumentSet_AttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("AsycudaDocumentSet_Attachments");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.AttachmentId).HasColumnName("AttachmentId");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.DocumentSpecific).HasColumnName("DocumentSpecific");
              this.Property(t => t.FileDate).HasColumnName("FileDate");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.EmailId).HasColumnName("EmailId").HasMaxLength(255);
              this.HasRequired(t => t.AsycudaDocumentSet).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AsycudaDocumentSetId);
              this.HasRequired(t => t.Attachment).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AttachmentId);
              this.HasOptional(t => t.FileType).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.FileTypeId);
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
