namespace DocumentItemDS.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class xcuda_AttachmentsMap : EntityTypeConfiguration<xcuda_Attachments>
    {
        public xcuda_AttachmentsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("xcuda_Attachments");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.AttachmentId).HasColumnName("AttachmentId");
              this.Property(t => t.Attached_documents_Id).HasColumnName("Attached_documents_Id");
              this.HasRequired(t => t.Attachments).WithMany(t =>(ICollection<xcuda_Attachments>) t.xcuda_Attachments).HasForeignKey(d => d.AttachmentId);
              this.HasRequired(t => t.xcuda_Attached_documents).WithMany(t =>(ICollection<xcuda_Attachments>) t.xcuda_Attachments).HasForeignKey(d => d.Attached_documents_Id);
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
