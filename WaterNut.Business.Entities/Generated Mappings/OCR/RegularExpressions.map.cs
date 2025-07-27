namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class RegularExpressionsMap : EntityTypeConfiguration<RegularExpressions>
    {
        public RegularExpressionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-RegularExpressions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.RegEx).HasColumnName("RegEx").IsRequired();
              this.Property(t => t.MultiLine).HasColumnName("MultiLine");
              this.Property(t => t.MaxLines).HasColumnName("MaxLines");
              this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
              this.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
              this.Property(t => t.Description).HasColumnName("Description").HasMaxLength(500);
              this.HasMany(t => t.End).WithRequired(t => (RegularExpressions)t.RegularExpressions);
              this.HasMany(t => t.Lines).WithRequired(t => (RegularExpressions)t.RegularExpressions);
              this.HasMany(t => t.Start).WithRequired(t => (RegularExpressions)t.RegularExpressions);
              this.HasMany(t => t.TemplateRegEx).WithRequired(t => (RegularExpressions)t.RegEx);
              this.HasMany(t => t.TemplateRepRegEx).WithRequired(t => (RegularExpressions)t.ReplacementRegEx);
              this.HasMany(t => t.FieldFormatRegEx).WithRequired(t => (RegularExpressions)t.RegEx);
              this.HasMany(t => t.FieldFormatRepRegEx).WithRequired(t => (RegularExpressions)t.ReplacementRegEx);
              this.HasMany(t => t.TemplateIdentificatonRegEx).WithRequired(t => (RegularExpressions)t.OCR_RegularExpressions);
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
