namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class LineMap : EntityTypeConfiguration<Line>
    {
        public LineMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-LinesView");
              this.Property(t => t.Invoice).HasColumnName("Invoice").IsRequired().HasMaxLength(50);
              this.Property(t => t.PartName).HasColumnName("Part").HasMaxLength(50);
              this.Property(t => t.Name).HasColumnName("Line").HasMaxLength(50);
              this.Property(t => t.MultiLine).HasColumnName("MultiLine");
              this.Property(t => t.RegEx).HasColumnName("RegEx");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.None));
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.Property(t => t.ParentId).HasColumnName("ParentId");
              this.Property(t => t.PartId).HasColumnName("PartId");
              this.HasOptional(t => t.Part).WithMany(t =>(ICollection<Line>) t.OCR_LinesView).HasForeignKey(d => d.PartId);
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
