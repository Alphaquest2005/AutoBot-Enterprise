namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class vw_OCRCorrectionAnalysisMap : EntityTypeConfiguration<vw_OCRCorrectionAnalysis>
    {
        public vw_OCRCorrectionAnalysisMap()
        {                        
              this.HasKey(t => new {t.FieldName, t.CorrectionType});        
              this.ToTable("vw_OCRCorrectionAnalysis");
              this.Property(t => t.FieldName).HasColumnName("FieldName").IsRequired().HasMaxLength(100);
              this.Property(t => t.CorrectionType).HasColumnName("CorrectionType").IsRequired().HasMaxLength(50);
              this.Property(t => t.TotalCorrections).HasColumnName("TotalCorrections");
              this.Property(t => t.AvgConfidence).HasColumnName("AvgConfidence");
              this.Property(t => t.SuccessfulCorrections).HasColumnName("SuccessfulCorrections");
              this.Property(t => t.FailedCorrections).HasColumnName("FailedCorrections");
              this.Property(t => t.SuccessRate).HasColumnName("SuccessRate");
              this.Property(t => t.FirstCorrection).HasColumnName("FirstCorrection");
              this.Property(t => t.LastCorrection).HasColumnName("LastCorrection");
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
