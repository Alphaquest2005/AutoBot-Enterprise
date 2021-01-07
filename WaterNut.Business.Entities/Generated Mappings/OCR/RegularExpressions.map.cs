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
              this.HasMany(t => t.End).WithRequired(t => (RegularExpressions)t.RegularExpressions);
              this.HasMany(t => t.Lines).WithRequired(t => (RegularExpressions)t.RegularExpressions);
              this.HasMany(t => t.Start).WithRequired(t => (RegularExpressions)t.RegularExpressions);
              this.HasMany(t => t.OCR_InvoiceRegEx).WithRequired(t => (RegularExpressions)t.RegEx);
              this.HasMany(t => t.OCR_InvoiceRegEx1).WithRequired(t => (RegularExpressions)t.ReplacementRegEx);
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
