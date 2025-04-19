namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class OCR_FieldMappingsMap : EntityTypeConfiguration<OCR_FieldMappings>
    {
        public OCR_FieldMappingsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-FieldMappings");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.Key).HasColumnName("Key").IsRequired().HasMaxLength(50);
              this.Property(t => t.Field).HasColumnName("Field").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntityType).HasColumnName("EntityType").IsRequired().HasMaxLength(50);
              this.Property(t => t.IsRequired).HasColumnName("IsRequired");
              this.Property(t => t.DataType).HasColumnName("DataType").IsRequired().HasMaxLength(50);
              this.Property(t => t.AppendValues).HasColumnName("AppendValues");
              this.Property(t => t.FileTypeId).HasColumnName("FileTypeId");
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
