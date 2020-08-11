namespace CoreEntities.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class InfoMappingRegExMap : EntityTypeConfiguration<InfoMappingRegEx>
    {
        public InfoMappingRegExMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("InfoMappingRegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.InfoMappingId).HasColumnName("InfoMappingId");
              this.Property(t => t.KeyRegX).HasColumnName("KeyRegX").IsRequired().HasMaxLength(1000);
              this.Property(t => t.FieldRx).HasColumnName("FieldRx").IsRequired().HasMaxLength(1000);
              this.Property(t => t.KeyReplaceRx).HasColumnName("KeyReplaceRx").HasMaxLength(1000);
              this.Property(t => t.FieldReplaceRx).HasColumnName("FieldReplaceRx").HasMaxLength(1000);
              this.Property(t => t.LineRegx).HasColumnName("LineRegx").IsRequired().HasMaxLength(1000);
              this.Property(t => t.KeyValue).HasColumnName("KeyValue").HasMaxLength(50);
              this.Property(t => t.FieldValue).HasColumnName("FieldValue").HasMaxLength(1000);
              this.HasRequired(t => t.InfoMapping).WithMany(t =>(ICollection<InfoMappingRegEx>) t.InfoMappingRegEx).HasForeignKey(d => d.InfoMappingId);
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
