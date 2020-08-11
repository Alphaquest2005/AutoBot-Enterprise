namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmailInfoMappingsMap : EntityTypeConfiguration<EmailInfoMappings>
    {
        public EmailInfoMappingsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EmailInfoMappings");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.EmailMappingId).HasColumnName("EmailMappingId");
              this.Property(t => t.InfoMappingId).HasColumnName("InfoMappingId");
              this.Property(t => t.UpdateDatabase).HasColumnName("UpdateDatabase");
              this.HasRequired(t => t.EmailMapping).WithMany(t =>(ICollection<EmailInfoMappings>) t.EmailInfoMappings).HasForeignKey(d => d.EmailMappingId);
              this.HasRequired(t => t.InfoMapping).WithMany(t =>(ICollection<EmailInfoMappings>) t.EmailInfoMappings).HasForeignKey(d => d.InfoMappingId);
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
