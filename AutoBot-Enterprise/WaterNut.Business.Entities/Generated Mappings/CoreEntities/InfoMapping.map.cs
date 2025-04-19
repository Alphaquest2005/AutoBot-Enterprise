namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InfoMappingMap : EntityTypeConfiguration<InfoMapping>
    {
        public InfoMappingMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("InfoMapping");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Key).HasColumnName("Key").IsRequired().HasMaxLength(50);
              this.Property(t => t.Field).HasColumnName("Field").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntityType).HasColumnName("EntityType").IsRequired().HasMaxLength(255);
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.EntityKeyField).HasColumnName("EntityKeyField").HasMaxLength(50);
              this.HasRequired(t => t.ApplicationSettings).WithMany(t =>(ICollection<InfoMapping>) t.InfoMapping).HasForeignKey(d => d.ApplicationSettingsId);
              this.HasMany(t => t.InfoMappingRegEx).WithRequired(t => (InfoMapping)t.InfoMapping);
              this.HasMany(t => t.EmailInfoMappings).WithRequired(t => (InfoMapping)t.InfoMapping);
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
