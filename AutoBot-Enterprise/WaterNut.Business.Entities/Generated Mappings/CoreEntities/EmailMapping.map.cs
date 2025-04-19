namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class EmailMappingMap : EntityTypeConfiguration<EmailMapping>
    {
        public EmailMappingMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("EmailMapping");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.Pattern).HasColumnName("Pattern").IsRequired();
              this.Property(t => t.IsSingleEmail).HasColumnName("IsSingleEmail");
              this.Property(t => t.ReplacementValue).HasColumnName("ReplacementValue").HasMaxLength(50);
              this.Property(t => t.InfoFirst).HasColumnName("InfoFirst");
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<EmailMapping>) t.EmailMapping).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.EmailFileTypes).WithRequired(t => (EmailMapping)t.EmailMapping);
              this.HasMany(t => t.EmailInfoMappings).WithRequired(t => (EmailMapping)t.EmailMapping);
              this.HasMany(t => t.EmailMappingRexExs).WithRequired(t => (EmailMapping)t.EmailMapping);
              this.HasMany(t => t.EmailMappingActions).WithRequired(t => (EmailMapping)t.EmailMapping);
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
