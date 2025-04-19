namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmailFileTypesMap : EntityTypeConfiguration<EmailFileTypes>
    {
        public EmailFileTypesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EmailFileTypes");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EmailMappingId).HasColumnName("EmailMappingId");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.IsRequired).HasColumnName("IsRequired");
              this.HasRequired(t => t.EmailMapping).WithMany(t =>(ICollection<EmailFileTypes>) t.EmailFileTypes).HasForeignKey(d => d.EmailMappingId);
              this.HasRequired(t => t.FileTypes).WithMany(t =>(ICollection<EmailFileTypes>) t.EmailFileTypes).HasForeignKey(d => d.FileTypeId);
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
