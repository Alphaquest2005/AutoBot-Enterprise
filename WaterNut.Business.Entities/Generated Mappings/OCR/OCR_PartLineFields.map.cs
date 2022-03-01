namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_PartLineFieldsMap : EntityTypeConfiguration<OCR_PartLineFields>
    {
        public OCR_PartLineFieldsMap()
        {                        
              this.HasKey(t => new {t.Id, t.LineId, t.FieldId});        
              this.ToTable("OCR-PartLineFields");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.Invoice).HasColumnName("Invoice").IsRequired().HasMaxLength(50);
              this.Property(t => t.Part).HasColumnName("Part").IsRequired().HasMaxLength(50);
              this.Property(t => t.Line).HasColumnName("Line").IsRequired().HasMaxLength(50);
              this.Property(t => t.RegEx).HasColumnName("RegEx").IsRequired();
              this.Property(t => t.Key).HasColumnName("Key").HasMaxLength(50);
              this.Property(t => t.Field).HasColumnName("Field").HasMaxLength(50);
              this.Property(t => t.EntityType).HasColumnName("EntityType").HasMaxLength(50);
              this.Property(t => t.IsRequired).HasColumnName("IsRequired");
              this.Property(t => t.DataType).HasColumnName("DataType").HasMaxLength(50);
              this.Property(t => t.Value).HasColumnName("Value");
              this.Property(t => t.AppendValues).HasColumnName("AppendValues");
              this.Property(t => t.FieldId).HasColumnName("FieldId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.ParentId).HasColumnName("ParentId");
              this.Property(t => t.LineId).HasColumnName("LineId").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
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
