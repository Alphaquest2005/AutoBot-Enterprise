namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class ImportErrorsMap : EntityTypeConfiguration<ImportErrors>
    {
        public ImportErrorsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("ImportErrors");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.PdfText).HasColumnName("PdfText").IsRequired();
              this.Property(t => t.Error).HasColumnName("Error").IsRequired().HasMaxLength(500);
              this.Property(t => t.EntryDateTime).HasColumnName("EntryDateTime");
              this.HasMany(t => t.OCR_FailedLines).WithRequired(t => (ImportErrors)t.ImportErrors);
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
