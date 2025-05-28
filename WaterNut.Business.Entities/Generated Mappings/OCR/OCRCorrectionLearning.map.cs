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
              this.Property(t => t.FieldName).HasColumnName("FieldName").IsRequired().HasMaxLength(100);
              this.Property(t => t.OriginalError).HasColumnName("OriginalError").IsRequired().HasMaxLength(500);
              this.Property(t => t.CorrectValue).HasColumnName("CorrectValue").IsRequired().HasMaxLength(500);
              this.Property(t => t.LineNumber).HasColumnName("LineNumber");
              this.Property(t => t.LineText).HasColumnName("LineText").IsRequired().HasMaxLength(1000);
              this.Property(t => t.WindowText).HasColumnName("WindowText");
              this.Property(t => t.ExistingRegex).HasColumnName("ExistingRegex").HasMaxLength(1000);
              this.Property(t => t.CorrectionType).HasColumnName("CorrectionType").IsRequired().HasMaxLength(50);
              this.Property(t => t.NewRegexPattern).HasColumnName("NewRegexPattern").HasMaxLength(1000);
              this.Property(t => t.ReplacementPattern).HasColumnName("ReplacementPattern").HasMaxLength(500);
              this.Property(t => t.DeepSeekReasoning).HasColumnName("DeepSeekReasoning");
              this.Property(t => t.Confidence).HasColumnName("Confidence");
              this.Property(t => t.InvoiceType).HasColumnName("InvoiceType").HasMaxLength(100);
              this.Property(t => t.FilePath).HasColumnName("FilePath").HasMaxLength(500);
              this.Property(t => t.FieldId).HasColumnName("FieldId");
              this.Property(t => t.Success).HasColumnName("Success");
              this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage").HasMaxLength(1000);
              this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
              this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
              this.Property(t => t.ProcessingTimeMs).HasColumnName("ProcessingTimeMs");
              this.Property(t => t.DeepSeekPrompt).HasColumnName("DeepSeekPrompt");
              this.Property(t => t.DeepSeekResponse).HasColumnName("DeepSeekResponse");
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
