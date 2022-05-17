namespace OCR.Business.Entities.Mapping
{
    //#pragma warning disable 1573
    using Entities;
    using System.Data.Entity.ModelConfiguration;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    
    public partial class LinesMap : EntityTypeConfiguration<Lines>
    {
        public LinesMap()
        {                        
              this.HasKey(t => t.Id);        
              this.ToTable("OCR-Lines");
              this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(new Nullable<DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity));
              this.Property(t => t.PartId).HasColumnName("PartId");
              this.Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
              this.Property(t => t.RegExId).HasColumnName("RegExId");
              this.Property(t => t.ParentId).HasColumnName("ParentId");
              this.Property(t => t.DistinctValues).HasColumnName("DistinctValues");
              this.Property(t => t.IsColumn).HasColumnName("IsColumn");
              this.Property(t => t.IsActive).HasColumnName("IsActive");
              this.HasRequired(t => t.Parts).WithMany(t =>(ICollection<Lines>) t.Lines).HasForeignKey(d => d.PartId);
              this.HasRequired(t => t.RegularExpressions).WithMany(t =>(ICollection<Lines>) t.Lines).HasForeignKey(d => d.RegExId);
              this.HasOptional(t => t.ParentLine).WithMany(t =>(ICollection<Lines>) t.ChildLines).HasForeignKey(d => d.ParentId);
              this.HasMany(t => t.ChildLines).WithOptional(t => t.ParentLine).HasForeignKey(d => d.ParentId);
              this.HasMany(t => t.FailedLines).WithRequired(t => (Lines)t.OCR_Lines);
              this.HasMany(t => t.Fields).WithRequired(t => (Lines)t.Lines);
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
