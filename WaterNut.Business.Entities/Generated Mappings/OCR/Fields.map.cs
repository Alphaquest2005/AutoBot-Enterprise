﻿namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class FieldsMap : EntityTypeConfiguration<Fields>
    {
        public FieldsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-Fields");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Field).HasColumnName("Field").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntityType).HasColumnName("EntityType").IsRequired().HasMaxLength(50);
              this.Property(t => t.IsRequired).HasColumnName("IsRequired");
              this.Property(t => t.DataType).HasColumnName("DataType").IsRequired().HasMaxLength(50);
              this.Property(t => t.LineId).HasColumnName("LineId");
              this.Property(t => t.Key).HasColumnName("Key").HasMaxLength(50);
              this.HasRequired(t => t.Lines).WithMany(t =>(ICollection<Fields>) t.Fields).HasForeignKey(d => d.LineId);
              this.HasOptional(t => t.FieldValue).WithRequired(t => (Fields)t.Field);
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