namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetAttachmentsMap : EntityTypeConfiguration<AsycudaDocumentSetAttachments>
    {
        public AsycudaDocumentSetAttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("AsycudaDocumentSetAttachments");
              this.Property(t => t.AsycudaDocumentSetId).HasColumnName("AsycudaDocumentSetId");
              this.Property(t => t.Declarant_Reference_Number).HasColumnName("Declarant_Reference_Number").HasMaxLength(50);
              this.Property(t => t.Description).HasColumnName("Description").HasMaxLength(50);
              this.Property(t => t.FilePath).HasColumnName("FilePath").IsRequired().HasMaxLength(255);
              this.Property(t => t.DocumentCode).HasColumnName("DocumentCode").HasMaxLength(50);
              this.Property(t => t.Reference).HasColumnName("Reference").HasMaxLength(255);
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.HasOptional(t => t.FileTypes).WithMany(t =>(ICollection<AsycudaDocumentSetAttachments>) t.AsycudaDocumentSetAttachments).HasForeignKey(d => d.FileTypeId);
              this.HasRequired(t => t.AsycudaDocumentSetEx).WithMany(t =>(ICollection<AsycudaDocumentSetAttachments>) t.AsycudaDocumentSetAttachments).HasForeignKey(d => d.AsycudaDocumentSetId);
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
