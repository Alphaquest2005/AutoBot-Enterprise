namespace CoreEntities.Business.Entities.Mapping
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
              this.HasRequired(t => t.Attachments).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AttachmentId);
              this.HasRequired(t => t.AsycudaDocumentSetEx).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AsycudaDocumentSetId);
              this.HasOptional(t => t.FileTypes).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.FileTypeId);
              this.HasOptional(t => t.Emails).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.EmailId);
              this.HasRequired(t => t.AsycudaDocumentSet).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AsycudaDocumentSetId);
              this.HasMany(t => t.AttachmentLog).WithRequired(t => (AsycudaDocumentSet_Attachments)t.AsycudaDocumentSet_Attachments);
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
