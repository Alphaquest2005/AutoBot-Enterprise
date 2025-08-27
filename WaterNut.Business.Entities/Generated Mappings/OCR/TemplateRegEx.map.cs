namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class TemplateRegExMap : EntityTypeConfiguration<TemplateRegEx>
    {
        public TemplateRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-TemplateRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.TemplateId).HasColumnName("TemplateId");
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.Property(t => t.ReplacementRegExId).HasColumnName("ReplacementRegExId");
              this.HasRequired(t => t.OCR_Templates).WithMany(t =>(ICollection<TemplateRegEx>) t.RegEx).HasForeignKey(d => d.TemplateId);
              this.HasRequired(t => t.RegEx).WithMany(t =>(ICollection<TemplateRegEx>) t.TemplateRegEx).HasForeignKey(d => d.RegExId);
              this.HasRequired(t => t.ReplacementRegEx).WithMany(t =>(ICollection<TemplateRegEx>) t.TemplateRepRegEx).HasForeignKey(d => d.ReplacementRegExId);
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
