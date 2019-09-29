namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class LineTypeFieldMappingsMap : EntityTypeConfiguration<LineTypeFieldMappings>
    {
        public LineTypeFieldMappingsMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-LineTypeFieldMappings");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.LineTypeId).HasColumnName("LineTypeId");
              this.Property(t => t.Key).HasColumnName("Key").IsRequired().HasMaxLength(50);
              this.Property(t => t.Field).HasColumnName("Field").IsRequired().HasMaxLength(50);
              this.Property(t => t.EntityType).HasColumnName("EntityType").IsRequired().HasMaxLength(50);
              this.Property(t => t.IsRequired).HasColumnName("IsRequired");
              this.HasRequired(t => t.LineTypes).WithMany(t =>(ICollection<LineTypeFieldMappings>) t.LineTypeFieldMappings).HasForeignKey(d => d.LineTypeId);
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
