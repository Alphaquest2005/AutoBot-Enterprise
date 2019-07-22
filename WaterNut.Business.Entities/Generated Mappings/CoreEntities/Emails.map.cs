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
              this.HasKey(t => t.EmailUniqueId);        
              this.ToTable("Emails");
              this.Property(t => t.EmailUniqueId).HasColumnName("EmailUniqueId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Subject).HasColumnName("Subject").IsRequired();
              this.Property(t => t.EmailDate).HasColumnName("EmailDate");
              this.HasMany(t => t.AsycudaDocumentSet_Attachments).WithOptional(t => t.Emails).HasForeignKey(d => d.EmailUniqueId);
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
