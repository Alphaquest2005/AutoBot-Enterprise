namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FieldFormatRegExMap : EntityTypeConfiguration<FieldFormatRegEx>
    {
        public FieldFormatRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-FieldFormatRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FieldId).HasColumnName("FieldId");
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.Property(t => t.ReplacementRegExId).HasColumnName("ReplacementRegExId");
              this.HasRequired(t => t.RegEx).WithMany(t =>(ICollection<FieldFormatRegEx>) t.FieldFormatRegEx).HasForeignKey(d => d.RegExId);
              this.HasRequired(t => t.ReplacementRegEx).WithMany(t =>(ICollection<FieldFormatRegEx>) t.FieldFormatRepRegEx).HasForeignKey(d => d.ReplacementRegExId);
              this.HasRequired(t => t.Fields).WithMany(t =>(ICollection<FieldFormatRegEx>) t.FormatRegEx).HasForeignKey(d => d.FieldId);
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
