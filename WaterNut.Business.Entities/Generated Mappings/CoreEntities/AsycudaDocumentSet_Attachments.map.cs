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
              this.HasRequired(t => t.Attachments).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AttachmentId);
              this.HasRequired(t => t.AsycudaDocumentSetEx).WithMany(t =>(ICollection<AsycudaDocumentSet_Attachments>) t.AsycudaDocumentSet_Attachments).HasForeignKey(d => d.AsycudaDocumentSetId);
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
