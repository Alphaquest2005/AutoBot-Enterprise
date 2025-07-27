namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TemplatesMap : EntityTypeConfiguration<Templates>
    {
        public TemplatesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-Templates");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.Property(t => t.ApplicationSettingsId).HasColumnName("ApplicationSettingsId");
              this.Property(t => t.IsActive).HasColumnName("IsActive");
              this.HasMany(t => t.Parts).WithRequired(t => (Templates)t.Templates);
              this.HasMany(t => t.RegEx).WithRequired(t => (Templates)t.OCR_Templates);
              this.HasMany(t => t.TemplateIdentificatonRegEx).WithRequired(t => (Templates)t.OCR_Templates);
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
