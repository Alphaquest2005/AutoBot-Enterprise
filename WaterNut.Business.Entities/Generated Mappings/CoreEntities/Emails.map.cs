namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmailsMap : EntityTypeConfiguration<Emails>
    {
        public EmailsMap()
        {                        
              this.HasKey(t => t.EmailId);        
              this.ToTable("Emails");
              this.Property(t => t.Subject).HasColumnName("Subject").IsRequired();
              this.Property(t => t.EmailDate).HasColumnName("EmailDate");
              this.Property(t => t.EmailId).HasColumnName("EmailId").IsRequired().HasMaxLength(255);
              this.Property(t => t.MachineName).HasColumnName("MachineName").HasMaxLength(50);
              this.Property(t => t.EmailUniqueId).HasColumnName("EmailUniqueId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.HasMany(t => t.AsycudaDocumentSet_Attachments).WithOptional(t => t.Emails).HasForeignKey(d => d.EmailId);
              this.HasMany(t => t.EmailAttachments).WithRequired(t => (Emails)t.Emails);
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
