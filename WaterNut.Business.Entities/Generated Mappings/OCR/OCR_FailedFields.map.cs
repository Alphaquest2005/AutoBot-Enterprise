﻿namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_FailedFieldsMap : EntityTypeConfiguration<OCR_FailedFields>
    {
        public OCR_FailedFieldsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR_FailedFields");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.FailedLineEventId).HasColumnName("FailedLineEventId");
              this.Property(t => t.FieldId).HasColumnName("FieldId");
              this.HasRequired(t => t.OCR_FailedLines).WithMany(t =>(ICollection<OCR_FailedFields>) t.OCR_FailedFields).HasForeignKey(d => d.FailedLineEventId);
              this.HasRequired(t => t.OCR_Fields).WithMany(t =>(ICollection<OCR_FailedFields>) t.FailedFields).HasForeignKey(d => d.FieldId);
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
