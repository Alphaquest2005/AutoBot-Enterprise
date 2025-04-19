namespace DocumentDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocument_AttachmentsMap : EntityTypeConfiguration<AsycudaDocument_Attachments>
    {
        public AsycudaDocument_AttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("AsycudaDocument_Attachments");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.AttachmentId).HasColumnName("AttachmentId");
              this.Property(t => t.AsycudaDocumentId).HasColumnName("AsycudaDocumentId");
              this.HasRequired(t => t.Attachment).WithMany(t =>(ICollection<AsycudaDocument_Attachments>) t.AsycudaDocument_Attachments).HasForeignKey(d => d.AttachmentId);
              this.HasRequired(t => t.xcuda_ASYCUDA).WithMany(t =>(ICollection<AsycudaDocument_Attachments>) t.AsycudaDocument_Attachments).HasForeignKey(d => d.AsycudaDocumentId);
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
