namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCRCorrectionLearningMap : EntityTypeConfiguration<OCRCorrectionLearning>
    {
        public OCRCorrectionLearningMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCRCorrectionLearning");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FieldName).HasColumnName("FieldName").IsRequired().HasMaxLength(255);
              this.Property(t => t.OriginalError).HasColumnName("OriginalError");
              this.Property(t => t.CorrectValue).HasColumnName("CorrectValue");
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.LineText).HasColumnName("LineText");
              this.Property(t => t.WindowText).HasColumnName("WindowText");
              this.Property(t => t.CorrectionType).HasColumnName("CorrectionType").HasMaxLength(100);
              this.Property(t => t.DeepSeekReasoning).HasColumnName("DeepSeekReasoning");
              this.Property(t => t.Confidence).HasColumnName("Confidence");
              this.Property(t => t.FilePath).HasColumnName("FilePath").HasMaxLength(500);
              this.Property(t => t.Success).HasColumnName("Success");
              this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
              this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
              this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").IsRequired().HasMaxLength(100);
              this.Property(t => t.RequiresMultilineRegex).HasColumnName("RequiresMultilineRegex");
              this.Property(t => t.ContextLinesBefore).HasColumnName("ContextLinesBefore");
              this.Property(t => t.ContextLinesAfter).HasColumnName("ContextLinesAfter");
              this.Property(t => t.LineId).HasColumnName("LineId");
              this.Property(t => t.PartId).HasColumnName("PartId");
              this.Property(t => t.RegexId).HasColumnName("RegexId");
              this.Property(t => t.SuggestedRegex).HasColumnName("SuggestedRegex");
              this.Property(t => t.SuggestedRegex_Indexed).HasColumnName("SuggestedRegex_Indexed").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Computed)).HasMaxLength(450);
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").HasMaxLength(100);
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
