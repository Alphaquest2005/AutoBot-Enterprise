namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TemplateLinesRegularExpressionsMap : EntityTypeConfiguration<TemplateLinesRegularExpressions>
    {
        public TemplateLinesRegularExpressionsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-TemplateLinesRegularExpressions");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.TemplateLineId).HasColumnName("TemplateLineId");
              this.Property(t => t.RegExChainId).HasColumnName("RegExChainId");
              this.HasRequired(t => t.RegExChain).WithMany(t =>(ICollection<TemplateLinesRegularExpressions>) t.TemplateLinesRegularExpressions).HasForeignKey(d => d.RegExChainId);
              this.HasRequired(t => t.TemplateLines).WithMany(t =>(ICollection<TemplateLinesRegularExpressions>) t.TemplateLinesRegularExpressions).HasForeignKey(d => d.TemplateLineId);
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
