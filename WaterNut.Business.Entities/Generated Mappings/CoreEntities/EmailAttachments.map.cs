namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmailAttachmentsMap : EntityTypeConfiguration<EmailAttachments>
    {
        public EmailAttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EmailAttachments");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EmailId).HasColumnName("EmailId").IsRequired().HasMaxLength(255);
              this.Property(t => t.AttachmentId).HasColumnName("AttachmentId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.DocumentSpecific).HasColumnName("DocumentSpecific");
              this.HasRequired(t => t.Attachments).WithMany(t =>(ICollection<EmailAttachments>) t.EmailAttachments).HasForeignKey(d => d.AttachmentId);
              this.HasRequired(t => t.Emails).WithMany(t =>(ICollection<EmailAttachments>) t.EmailAttachments).HasForeignKey(d => d.EmailId);
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
