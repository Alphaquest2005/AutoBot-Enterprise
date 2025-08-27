namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_TemplateTableMappingMap : EntityTypeConfiguration<OCR_TemplateTableMapping>
    {
        public OCR_TemplateTableMappingMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR_TemplateTableMapping");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.DocumentType).HasColumnName("DocumentType").IsRequired().HasMaxLength(100);
              this.Property(t => t.TargetTable).HasColumnName("TargetTable").IsRequired().HasMaxLength(100);
              this.Property(t => t.ParentId).HasColumnName("ParentId");
              this.Property(t => t.Keywords).HasColumnName("Keywords").IsRequired();
              this.Property(t => t.RequiredFields).HasColumnName("RequiredFields").IsRequired().HasMaxLength(500);
              this.Property(t => t.TemplatePrefix).HasColumnName("TemplatePrefix").IsRequired().HasMaxLength(10);
              this.Property(t => t.Priority).HasColumnName("Priority");
              this.Property(t => t.IsActive).HasColumnName("IsActive");
              this.Property(t => t.Description).HasColumnName("Description").HasMaxLength(255);
              this.Property(t => t.ParentTemplateTableId).HasColumnName("ParentTemplateTableId");
              this.Property(t => t.OptionalFields).HasColumnName("OptionalFields").HasMaxLength(500);
              this.Property(t => t.MatchThreshold).HasColumnName("MatchThreshold");
              this.Property(t => t.IsSystemEntity).HasColumnName("IsSystemEntity");
              this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
              this.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
              this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
              this.Property(t => t.ProcessingNotes).HasColumnName("ProcessingNotes");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
              this.HasOptional(t => t.OCR_TemplateTableMapping2).WithMany(t =>(ICollection<OCR_TemplateTableMapping>) t.OCR_TemplateTableMapping1).HasForeignKey(d => d.ParentId);
              this.HasMany(t => t.OCR_KeywordDetectionLog).WithOptional(t => t.OCR_TemplateTableMapping).HasForeignKey(d => d.DetectedMappingId);
              this.HasMany(t => t.OCR_TemplateTableMapping1).WithOptional(t => t.OCR_TemplateTableMapping2).HasForeignKey(d => d.ParentId);
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
