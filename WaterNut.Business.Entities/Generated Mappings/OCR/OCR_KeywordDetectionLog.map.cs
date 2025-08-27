namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_KeywordDetectionLogMap : EntityTypeConfiguration<OCR_KeywordDetectionLog>
    {
        public OCR_KeywordDetectionLogMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR_KeywordDetectionLog");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.DocumentPath).HasColumnName("DocumentPath").HasMaxLength(500);
              this.Property(t => t.DocumentContent).HasColumnName("DocumentContent");
              this.Property(t => t.DetectedMappingId).HasColumnName("DetectedMappingId");
              this.Property(t => t.KeywordMatches).HasColumnName("KeywordMatches");
              this.Property(t => t.MatchScore).HasColumnName("MatchScore");
              this.Property(t => t.ProcessingTimeMs).HasColumnName("ProcessingTimeMs");
              this.Property(t => t.Success).HasColumnName("Success");
              this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
              this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
              this.HasOptional(t => t.OCR_TemplateTableMapping).WithMany(t =>(ICollection<OCR_KeywordDetectionLog>) t.OCR_KeywordDetectionLog).HasForeignKey(d => d.DetectedMappingId);
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
