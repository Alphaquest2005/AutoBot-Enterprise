namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class PartsMap : EntityTypeConfiguration<Parts>
    {
        public PartsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-Parts");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.TemplateId).HasColumnName("TemplateId");
              this.Property(t => t.PartTypeId).HasColumnName("PartTypeId");
              this.HasRequired(t => t.Templates).WithMany(t =>(ICollection<Parts>) t.Parts).HasForeignKey(d => d.TemplateId);
              this.HasRequired(t => t.PartTypes).WithMany(t =>(ICollection<Parts>) t.Parts).HasForeignKey(d => d.PartTypeId);
              this.HasMany(t => t.End).WithRequired(t => (Parts)t.Parts);
              this.HasMany(t => t.Start).WithRequired(t => (Parts)t.Parts);
              this.HasMany(t => t.Lines).WithRequired(t => (Parts)t.Parts);
              this.HasOptional(t => t.RecuringPart).WithRequired(t => (Parts)t.Parts);
              this.HasMany(t => t.ChildParts).WithRequired(t => (Parts)t.ChildPart);
              this.HasMany(t => t.ParentParts).WithRequired(t => (Parts)t.ParentPart);
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
