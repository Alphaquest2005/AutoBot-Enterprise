namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_FailedLinesMap : EntityTypeConfiguration<OCR_FailedLines>
    {
        public OCR_FailedLinesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR_FailedLines");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.DocSetAttachmentId).HasColumnName("DocSetAttachmentId");
              this.Property(t => t.LineId).HasColumnName("LineId");
              this.Property(t => t.Resolved).HasColumnName("Resolved");
              this.HasRequired(t => t.ImportErrors).WithMany(t =>(ICollection<OCR_FailedLines>) t.OCR_FailedLines).HasForeignKey(d => d.DocSetAttachmentId);
              this.HasRequired(t => t.OCR_Lines).WithMany(t =>(ICollection<OCR_FailedLines>) t.OCR_FailedLines).HasForeignKey(d => d.LineId);
              this.HasMany(t => t.OCR_FailedFields).WithRequired(t => (OCR_FailedLines)t.OCR_FailedLines);
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
